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
        public async Task ネイティブ処理が成功したら結果が返ること() {
            // Arrange
            var fakeNative = new Action<IntPtr, IntPtr>((succPtr, errPtr) => {
                // リフレクションで非公開ネストデリゲート型を取得
                var handleType = typeof(NativeProcess.AsyncOperationHandle<int>);
                var delType = handleType.GetNestedType(
                    "CompletionCallback",
                    BindingFlags.NonPublic
                );
                // ポインタ→デリゲートに変換し、DynamicInvoke で呼び出し
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
        public void ネイティブ処理が失敗したら例外が伝播すること() {
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
        public void キャンセル可能なトークンでキャンセルするとタスクがキャンセルされること() {
            // Arrange
            var cts = new CancellationTokenSource();
            var fakeNative = new Action<IntPtr, IntPtr>((succ, err) => {
                // 何もしない
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
