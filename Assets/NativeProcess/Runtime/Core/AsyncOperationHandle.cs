using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace NativeProcess {

    public abstract class AsyncOperationHandle {

        public int ControlId { get; }

        public NativeOperationStatus Status { get; protected set; }

        public Exception OperationException { get; protected set; }

        protected AsyncOperationHandle(int controlId) {
            ControlId = controlId;
            Status = NativeOperationStatus.Stop;

            Debug.Log($"Create [{controlId}]");
        }
    }

    
    public sealed class AsyncOperationHandle<T> : AsyncOperationHandle, IDisposable {

        // Callback 
        private Delegate _completionCallback;
        private Delegate _errorCallback;
        // ???R?[???o?b?N??GC?????????????????????????D
        private GCHandle _completionCallbackHandle;     
        private GCHandle _errorCallbackHandle;
        
        private readonly TaskCompletionSource<T> _tcs =
            new(TaskCreationOptions.RunContinuationsAsynchronously);    // ???w??????????SetResult?????C???X???b?h??????????????

        // 
        private bool _disposed;
        private CancellationTokenRegistration _cancellationRegistration;

        public T Result { get; private set; }
        public Task<T> Task => _tcs.Task;


        private AsyncOperationHandle(int controlId) : base(controlId) { }
        ~AsyncOperationHandle() => Dispose();

        public void Dispose() {
            if (_disposed) return;

            _cancellationRegistration.Dispose();
            FreeFunctionHandles();

            _disposed = true;
            GC.SuppressFinalize(this);
        }


        private void SetResult(T result) {
            if (Status is not NativeOperationStatus.Running) return;

            Result = result;
            _tcs.TrySetResult(result);
            Status = NativeOperationStatus.Succeeded;

            // Relese functions
            FreeFunctionHandles();
        }

        private void SetException(string errorMessage) {
            if (Status is not NativeOperationStatus.Running) return;

            var exception = new InvalidOperationException(errorMessage);
            OperationException = exception;
            _tcs.SetException(exception);
            Status = NativeOperationStatus.Failed;

            // Relese functions
            FreeFunctionHandles();
        }

        private (IntPtr successPtr, IntPtr errorPtr) GetFunctionPointers(ICallbackFactory factory) {
            if (Status != NativeOperationStatus.Stop)
                throw new InvalidOperationException("Handle already started or completed.");

            // Callback
            _completionCallback = factory.CreateSuccessCallback<T>(SetResult);
            _errorCallback = factory.CreateErrorCallback(SetException);

            // GC handle
            _completionCallbackHandle = GCHandle.Alloc(_completionCallback);
            _errorCallbackHandle = GCHandle.Alloc(_errorCallback);

            Status = NativeOperationStatus.Running;

            return (
                Marshal.GetFunctionPointerForDelegate(_completionCallback),
                Marshal.GetFunctionPointerForDelegate(_errorCallback)
            );
        }

        private void FreeFunctionHandles() {
            _completionCallback = null;
            _errorCallback = null;
            
            if (_completionCallbackHandle.IsAllocated)
                _completionCallbackHandle.Free();
            if (_errorCallbackHandle.IsAllocated)
                _errorCallbackHandle.Free();
        }

        #region Static

        private static int _nextControlId = 0;

        internal static AsyncOperationHandle<T> Create(
            Action<IntPtr, IntPtr> nativeMethod,
            ICallbackFactory factory,
            CancellationToken cancellationToken = default) {

            var controlId = Interlocked.Increment(ref _nextControlId);
            var handle = new AsyncOperationHandle<T>(controlId);

            // Cancel
            if (cancellationToken.CanBeCanceled) {
                handle._cancellationRegistration = cancellationToken.Register(() => {
                    if (handle.Status is not NativeOperationStatus.Running) return;

                    handle._tcs.TrySetCanceled(cancellationToken);
                    handle.Status = NativeOperationStatus.Canceled;

                    handle.Dispose();
                });
            }

            // Execute native method
            var (successPtr, errorPtr) = handle.GetFunctionPointers(factory);
            nativeMethod.Invoke(successPtr, errorPtr);

            return handle;
        }

        internal static AsyncOperationHandle<T> Completed(T result) {
            var controlId = Interlocked.Increment(ref _nextControlId);
            var handle = new AsyncOperationHandle<T>(controlId);
            handle.Result = result;
            handle._tcs.TrySetResult(result);
            handle.Status = NativeOperationStatus.Succeeded;

            return handle;
        }

        internal static AsyncOperationHandle<T> Failed(Exception exception) {
            var controlId = Interlocked.Increment(ref _nextControlId);
            var handle = new AsyncOperationHandle<T>(controlId);
            handle.OperationException = exception;
            handle._tcs.TrySetException(exception);
            handle.Status = NativeOperationStatus.Failed;

            return handle;
        }
        #endregion
    }
}
