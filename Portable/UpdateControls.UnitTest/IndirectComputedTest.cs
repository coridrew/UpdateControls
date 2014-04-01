using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KnockoutCS.UnitTest
{
    [TestClass]
	public class IndirectComputedTest
	{
		public TestContext TestContext { get; set; }

		private SourceData _source;
		private DirectComputed _intermediateComputed;
		private IndirectComputed _dependent;

		[TestInitialize]
		public void Initialize()
		{
			_source = new SourceData();
			_intermediateComputed = new DirectComputed(_source);
			_dependent = new IndirectComputed(_intermediateComputed);
		}

		[TestMethod]
		public void ComputedIsInitiallyOutOfDate()
		{
			Assert.IsFalse(_dependent.IsUpToDate, "The dependent is initially up to date");
		}

		[TestMethod]
		public void ComputedRemainsOutOfDateOnChange()
		{
			_source.SourceProperty = 3;
			Assert.IsFalse(_dependent.IsUpToDate, "The dependent is up to date after change");
		}

		[TestMethod]
		public void ComputedIsUpdatedOnGet()
		{
			int fetch = _dependent.ComputedProperty;
			Assert.IsTrue(_dependent.IsUpToDate, "The dependent has not been updated");
		}

		[TestMethod]
		public void ComputedIsUpdatedAfterChangeOnGet()
		{
			_source.SourceProperty = 3;
			int fetch = _dependent.ComputedProperty;
			Assert.IsTrue(_dependent.IsUpToDate, "The dependent has not been updated");
		}

		[TestMethod]
		public void ComputedGetsValueFromItsPrecedent()
		{
			_source.SourceProperty = 3;
			Assert.AreEqual(3, _dependent.ComputedProperty);
		}

		[TestMethod]
		public void ComputedIsOutOfDateAgainAfterChange()
		{
			_source.SourceProperty = 3;
			int fetch = _dependent.ComputedProperty;
			_source.SourceProperty = 4;
			Assert.IsFalse(_dependent.IsUpToDate, "The dependent did not go out of date");
		}

		[TestMethod]
		public void ComputedIsUpdatedAgainAfterChange()
		{
			_source.SourceProperty = 3;
			int fetch = _dependent.ComputedProperty;
			_source.SourceProperty = 4;
			fetch = _dependent.ComputedProperty;
			Assert.IsTrue(_dependent.IsUpToDate, "The dependent did not get udpated");
		}

		[TestMethod]
		public void ComputedGetsValueFromItsPrecedentAgainAfterChange()
		{
			_source.SourceProperty = 3;
			int fetch = _dependent.ComputedProperty;
			_source.SourceProperty = 4;
			Assert.AreEqual(4, _dependent.ComputedProperty);
		}

		[TestMethod]
		public void PrecedentIsOnlyAskedOnce()
		{
			int getCount = 0;
			_source.AfterGet += () => ++getCount;

			_source.SourceProperty = 3;
			int fetch = _dependent.ComputedProperty;
			fetch = _dependent.ComputedProperty;

			Assert.AreEqual(1, getCount);
		}

		[TestMethod]
		public void PrecedentIsAskedAgainAfterChange()
		{
			int getCount = 0;
			_source.AfterGet += () => ++getCount;

			_source.SourceProperty = 3;
			int fetch = _dependent.ComputedProperty;
			fetch = _dependent.ComputedProperty;
			_source.SourceProperty = 4;
			fetch = _dependent.ComputedProperty;
			fetch = _dependent.ComputedProperty;

			Assert.AreEqual(2, getCount);
		}
	}
}
