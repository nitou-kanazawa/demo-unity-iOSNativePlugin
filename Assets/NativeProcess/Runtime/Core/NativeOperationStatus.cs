using System;

namespace NativeProcess {

    public enum NativeOperationStatus {
        Stop = 0,
        Running = 1,
        Succeeded = 2,
        Failed = 3,
        Canceled = 4, 
    }

    public static class NativeOperationStatusExtensions {

        public static bool IsDone(this NativeOperationStatus self) => self is
            NativeOperationStatus.Succeeded or 
            NativeOperationStatus.Failed or
            NativeOperationStatus.Canceled;
    }
}
