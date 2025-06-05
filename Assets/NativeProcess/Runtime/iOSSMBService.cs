#if UNITY_IOS || UNITY_EDITOR
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NativePlugin;
using Project.Networking.SMB;
using UnityEngine;

namespace Project.Networking.SMB.iOS
{
    public sealed class iOSSMBService : INativeSMBService
    {

        private static SMBConnectionInfo info;

        void INativeSMBService.SetConfig(SMBConnectionInfo info)
        {
            iOSSMBService.info = info;
        }
        async UniTask<string[]> INativeSMBService.GetRemoteFileNames(string remoteDirectoryPath)
        {
            return await NativeMethods.GetFileNames(info, remoteDirectoryPath);
        }

        async UniTask INativeSMBService.DownloadFile(string remoteFilePath, string localFilePath)
        {
            await NativeMethods.DownloadFile(info, remoteFilePath, localFilePath);
        }
        async UniTask INativeSMBService.UploadFile(string localFilePath, string remoteFilePath)
        {
            await NativeMethods.UploadFile(info, localFilePath, remoteFilePath);   
        }

        UniTask INativeSMBService.DeleteRemoteFile(string remoteFilePath)
        {
            throw new System.NotImplementedException();
        }
    }

}
#endif