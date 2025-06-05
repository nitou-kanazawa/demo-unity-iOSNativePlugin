using System;

namespace TinyRx {

	internal sealed class AnonymousDisposable : IDisposable {

		private readonly Action _action;

		public AnonymousDisposable(Action action) {
			_action = action;
		}

		public void Dispose() {
			_action?.Invoke();
		}
	}
}
