using KnockoutCS.Fields;

namespace KnockoutCS.UnitTest
{
	public class DirectDependent
	{
		private SourceData _source;

        private Computed<int> _property;

		public DirectDependent(SourceData source)
		{
			_source = source;
            _property = new Computed<int>(() => _source.SourceProperty);
		}

		public int DependentProperty
		{
            get { return _property; }
		}

		public bool IsUpToDate
		{
			get { return _property.DependentSentry.IsUpToDate; }
		}
	}
}
