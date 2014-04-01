using KnockoutCS.Fields;

namespace KnockoutCS.UnitTest
{
	public class IndirectDependent
	{
		private DirectDependent _indermediateDependent;

        private Computed<int> _property;

        public IndirectDependent(DirectDependent indermediateDependent)
		{
			_indermediateDependent = indermediateDependent;
            _property = new Computed<int>(() => _indermediateDependent.DependentProperty);
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
