using System;

namespace TinyRx {

	internal class Disposable : IDisposable {

		private bool _disposed;
		private readonly Action _action;

		public Disposable(Action action) {
			_action = action;
		}

		public void Dispose() {
			if (_disposed)
				return;

			_action?.Invoke();
			_disposed = true;
		}
	}


	internal static class DisposableExtensions {
		
		
	}
}
