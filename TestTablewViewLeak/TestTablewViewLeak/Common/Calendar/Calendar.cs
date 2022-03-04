using System;
using EventKit;
using EventKitUI;

namespace WBid.WBidiPad.iOS
{
	public class Calendar
	{
		public Calendar ()
		{
			es = new EKEventStore ( );
		}
		public EKEventStore es;
		public EKEventStore EventStore {
			get { return es; }
		}
	}
}

