using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace NativePlugin.Utils
{
    public sealed class AsyncOperationHandle : AsyncOperationHandleBase, IDisposable
    {
        #region Callback Delegates

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnSuccessCallback(
            [MarshalAs(UnmanagedType.I4)] Int32 instanceId
        );

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnErrorCallback(
            [MarshalAs(UnmanagedType.I4)] Int32 instanceId,
            [MarshalAs(UnmanagedType.I4)] Int32 errorCode,
            [MarshalAs(UnmanagedType.LPStr)] string errorMessage
        );
        #endregion


        private TaskCompletionSource<bool> _tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task Task => _tcs.Task;


        private AsyncOperationHandle(int controlId) : base(controlId) { }

        internal (IntPtr successCallbackPtr, IntPtr errorCallbackPtr) GetCallbackPointers()
        {
            if (Status is not AsyncOperationStatus.Stop)
                throw new InvalidOperationException("Handle already started or completed.");

            Status = AsyncOperationStatus.Running;

            // Callback
            _completionCallback = new OnSuccessCallback(StaticCompletionCallback);
            _errorCallback = new OnErrorCallback(StaticFailedCallback);

            // GC handle
            _completionCallbackHandle = GCHandle.Alloc(_completionCallback);
            _errorCallbackHandle = GCHandle.Alloc(_errorCallback);

            return (
                Marshal.GetFunctionPointerForDelegate(_completionCallback),
                Marshal.GetFunctionPointerForDelegate(_errorCallback)
            );
        }


        private void SetCompletion()
        {
            if (Status is AsyncOperationStatus.Running)
            {
                Status = AsyncOperationStatus.Succeeded;
                _tcs.TrySetResult(true);
            }

            // Relese
            Dispose();
        }

        private void SetException(int errorCode, string errorMessage)
        {
            if (Status is not AsyncOperationStatus.Running)
            {
                Status = AsyncOperationStatus.Failed;
                OperationException = new InvalidOperationException(errorMessage);
                _tcs.SetException(OperationException);
            }

            // Relese
            Dispose();
        }


        #region Static

        internal static AsyncOperationHandle CreateInstance()
        {
            var handle = new AsyncOperationHandle(GetNextControlId());
            return handle;
        }

        // [NOTE]
        //  - Native code can only managed static method in AOT platform. (Can not instance method)
        //  - MonoPInvoke attribute must be set

        [AOT.MonoPInvokeCallback(typeof(OnSuccessCallback))]
        internal static void StaticCompletionCallback(int id)
        {
            var handle = GetHandle<AsyncOperationHandle>(id);
            handle.SetCompletion();
        }

        [AOT.MonoPInvokeCallback(typeof(OnErrorCallback))]
        internal static void StaticFailedCallback(int id, int errorCode, string errorMessage)
        {
            var handle = GetHandle<AsyncOperationHandle>(id);
            handle.SetException(errorCode, errorMessage);
        }
        #endregion
    }

    /*

    public sealed class AsyncOperationHandle<T> : AsyncOperationHandleBase, IDisposable {

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

            // Debug.Log($"Dispose [{ControlId}]");
        }


        private void SetResult(T result) {
            if (Status is not NativeOperationStatus.Running) return;

            Result = result;
            _tcs.TrySetResult(result);
            Status = NativeOperationStatus.Succeeded;

            // Relese functions
            FreeFunctionHandles();
        }

        private void SetException(int errorCode, string errorMessage) {
            if (Status is not NativeOperationStatus.Running) return;

            var exception = new InvalidOperationException(errorMessage);
            OperationException = exception;
            _tcs.SetException(exception);
            Status = NativeOperationStatus.Failed;

            // Relese functions
            FreeFunctionHandles();
        }

        internal (IntPtr successPtr, IntPtr errorPtr) GetFunctionPointers(ICallbackFactory factory) {
            if (Status != NativeOperationStatus.Stop)
                throw new InvalidOperationException("Handle already started or completed.");

            // Callback
            _completionCallback = factory.CreateSuccessCallback<T>(SetResult);
            _errorCallback = new SwiftCallback_withIntString(SetException);

            // GC handle
            _completionCallbackHandle = GCHandle.Alloc(_completionCallback);
            _errorCallbackHandle = GCHandle.Alloc(_errorCallback);

            Status = NativeOperationStatus.Running;

            return (
                Marshal.GetFunctionPointerForDelegate(_completionCallback),
                Marshal.GetFunctionPointerForDelegate(_errorCallback)
            );
        }

        #region Static

        internal static AsyncOperationHandle<T> GetInstance()
        {
            var handle = new AsyncOperationHandle<T>(GetNextControlId());
            return handle;
        }

        internal static AsyncOperationHandle<T> Completed(T result) {
            var handle = new AsyncOperationHandle<T>(GetNextControlId());
            handle.Result = result;
            handle._tcs.TrySetResult(result);
            handle.Status = NativeOperationStatus.Succeeded;

            return handle;
        }

        internal static AsyncOperationHandle<T> Failed(Exception exception) {
            var handle = new AsyncOperationHandle<T>(GetNextControlId());
            handle.OperationException = exception;
            handle._tcs.TrySetException(exception);
            handle.Status = NativeOperationStatus.Failed;

            return handle;
        }
        #endregion
    }

    */
}
