using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NativeProcess {

    #region Callback delegates

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void IntCallback(int value);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FloatCallback(float value);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void StringCallback(string value);
    #endregion


    public interface ICallbackFactory {

        Delegate CreateSuccessCallback<T>(Action<T> onSuccess);
        Delegate CreateErrorCallback(Action<string> onError);
    }


    public sealed class CallbackFactory : ICallbackFactory {

        private readonly Dictionary<Type, Func<Delegate, Delegate>> _callbackCreators = new();

        public void Register<T, TCallback>(Func<Action<T>, TCallback> creator)
        where TCallback : Delegate {
            _callbackCreators[typeof(T)] = action => creator((Action<T>)action);
        }

        public Delegate CreateSuccessCallback<T>(Action<T> onSuccess) {
            if (!_callbackCreators.TryGetValue(typeof(T), out var creator))
                throw new NotSupportedException($"型 {typeof(T)} に対するコールバックが登録されていません。");

            return creator(onSuccess);
        }

        public Delegate CreateErrorCallback(Action<string> onError) {
            return new StringCallback(onError);
        }
    }
}
