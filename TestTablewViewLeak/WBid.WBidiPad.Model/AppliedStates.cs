using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace WBid.WBidiPad.Model
{
	public class AppliedStates
	{
		public AppliedStates ()
		{
		}

		public string Key { get; set; }
		public List<AppliedStateType> AppliedStateTypes { get; set; }
	}
}

