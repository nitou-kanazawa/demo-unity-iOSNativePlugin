using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace NativeProcess.Editor {
   
    public sealed class AsyncOperationHandleTest {

        /*

        [Test]
        public async Task �l�C�e�B�u���������������猋�ʂ��Ԃ邱��() {
            // Arrange
            var fakeNative = new Action<IntPtr, IntPtr>((succPtr, errPtr) => {
                // ���t���N�V�����Ŕ���J�l�X�g�f���Q�[�g�^���擾
                var handleType = typeof(NativeProcess.AsyncOperationHandle<int>);
                var delType = handleType.GetNestedType(
                    "CompletionCallback",
                    BindingFlags.NonPublic
                );
                // �|�C���^���f���Q�[�g�ɕϊ����ADynamicInvoke �ŌĂяo��
                var successDel = (Delegate)Marshal.GetDelegateForFunctionPointer(
                    succPtr, delType
                );
                successDel.DynamicInvoke(42);
            });

            var handle = NativeProcess.AsyncOperationHandle<int>.Create(fakeNative);

            // Act & Assert
            Assert.That(async () => await handle.Task,
                Throws.TypeOf<InvalidOperationException>()
                      .With.Message.EqualTo("error!"));
            Assert.That(handle.Status, Is.EqualTo(NativeProcess.NativeOperationStatus.Failed));

        }

        [Test]
        public void �l�C�e�B�u���������s�������O���`�d���邱��() {
            // Arrange
            var fakeNative = new Action<IntPtr, IntPtr>((succ, err) => {
                Marshal.GetDelegateForFunctionPointer<Action<string>>(err)("error!");
            });
            var handle = AsyncOperationHandle<int>.Create(fakeNative);

            // Act & Assert
            Assert.That(async () => await handle.Task, Throws.TypeOf<InvalidOperationException>()
                .With.Message.EqualTo("error!"));
            Assert.That(handle.Status, Is.EqualTo(NativeOperationStatus.Failed));
        }

        [Test]
        public void �L�����Z���\�ȃg�[�N���ŃL�����Z������ƃ^�X�N���L�����Z������邱��() {
            // Arrange
            var cts = new CancellationTokenSource();
            var fakeNative = new Action<IntPtr, IntPtr>((succ, err) => {
                // �������Ȃ�
            });
            var handle = AsyncOperationHandle<int>.Create(fakeNative, cts.Token);

            // Act
            cts.Cancel();

            // Assert
            Assert.That(handle.Task.IsCanceled, Is.True);
            Assert.That(handle.Status, Is.EqualTo(NativeOperationStatus.Canceled));
        }

        */
    }
}
