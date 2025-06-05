using System;
using System.Collections.Generic;

namespace TinyRx {
	internal sealed class CompositeDisposable : IDisposable {

		private readonly List<IDisposable> _disposables;

		public bool IsDisposed { get; private set; }

		public int Count => _disposables.Count;

		public CompositeDisposable() {
			_disposables = new();
		}

		public CompositeDisposable(int capacity) {
			if (capacity < 0)
				throw new ArgumentOutOfRangeException("Capacity must be positive number.");

			_disposables = new(capacity);
		}

		public void Dispose() {
			Clear();
			IsDisposed = true;
		}

		public void Add(IDisposable item) {
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			if (IsDisposed) {
				item.Dispose();
				return;
			}

			_disposables.Add(item);
		}

		public bool Remove(IDisposable item) {
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			if (IsDisposed) {
				return false;
			}

			item.Dispose();
			_disposables.Remove(item);
			return true;
		}

		public void Clear() {
			foreach (var disposable in _disposables)
				disposable.Dispose();

			_disposables.Clear();
		}

		public bool Contains(IDisposable item) {
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			return _disposables.Contains(item);
		}
	}
}
