using System;

namespace TinyRx.Tests {


	public class Tests {

		[SetUp]
		public void Setup() {
		}

		[Test]
		public void Test1() {
			var result = 0;
			var dis = new AnonymousDisposable(
				() => { result = 99; }
			);
			dis.Dispose();

			Assert.That(result, Is.EqualTo(99).Within(0.001));
		}
	}
}
