using System;

namespace KnockoutCS.Test
{
	public class PrefixViewModel
	{
		private PrefixID _prefix;

		public PrefixViewModel(PrefixID prefix)
		{
			_prefix = prefix;
		}

		public PrefixID Prefix
		{
			get { return _prefix; }
		}

		public override string ToString()
		{
			return _prefix.ToString();
		}
	}
}
