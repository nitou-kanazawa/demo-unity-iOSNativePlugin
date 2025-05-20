using UnityEngine;
using System;
using System.Threading;
using System.Runtime.InteropServices;

namespace NativeProcess
{
    public static class SwiftNativeFunctions
    {
#if UNITY_IOS && !UNITY_EDITOR
        private const string DLL_NAME = "__Internal";
#else 
    private const string DLL_NAME = "MySwiftPlugin";
#endif

        private static readonly CallbackFactory _factory;

        static SwiftNativeFunctions() {
            _factory = new CallbackFactory();
            _factory.Register<int, IntCallback>(callback => new IntCallback(callback));
            _factory.Register<float, FloatCallback>(callback => new FloatCallback(callback));
        }


        // ????????
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern long SwiftPmPlugin_ToNumber(string str);

        // ??????????
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern void DoWorkAsync(IntPtr onSuccess, IntPtr onError);


        public static AsyncOperationHandle<int> DoWorkAsync(CancellationToken token = default) {
            return AsyncOperationHandle<int>.Create(DoWorkAsync, _factory, token);
        }


    }
}
