using System;
using System.Collections.Generic;
using CoreGraphics;

namespace WBid.WBidiPad.iOS
{
	public class ConstraintsApplied
	{
		public ConstraintsApplied ()
		{
		}
        //Constraint name pop up on the csw view  is binding this data
		public static List <string> MainList = new List <string> () { "Aircraft Changes", "Blocks of Days Off", "Cmut DHs", "Commutable Lines - Manual", "Days of the Week", "Days of the Month",
			"DH - first - last", "Duty period", "Equipment Type", "Flight Time", "Ground Time", "Intl – NonConus", "Legs Per Duty Period", "Legs Per Pairing", 
			"Number of Days Off", "Overnight Cities", "Overnight Cities - Bulk", "PDO", "Start Day of Week", "Rest", "Time-Away-From-Base", "Trip Length", "Work Blk Length", "Work Days",
			"Min Pay", "3-on-3-off","Commutable Line - Auto","Cities-Legs","Start Day","Report-Release","Mixed Hard/Reserve"
		};
		//, "Overlap Days"
        //multiple constraints
		public static List <string> daysOfWeekConstraints = new List <string> ();
		public static List <string> DhFirstLastConstraints = new List <string> ();
		public static List <string> EQTypeConstraints = new List <string> ();
		public static List <string> OvernightCitiesConstraints = new List <string> ();
		public static List <string> CitiesLegsConstraints = new List <string> ();
		public static List <string> CommutabilityConstraints = new List <string> ();
		public static List <string> StartDayofWeekConstraints = new List <string> ();
		public static List <string> RestConstraints = new List <string> ();
		public static List <string> TripLengthConstraints = new List <string> ();
		public static List <string> WorkBlockLengthConstraints = new List <string> ();
		public static List <string> CmutDHsConstraints = new List <string> ();
		public static List <string> PDOConstraints = new List <string> ();
		public static List <string> IntlNonConusConstraints = new List <string> ();
        public static List<string> StartDayConstraints = new List<string>();
        public static List<string> ReportReleaseConstraints = new List<string>();

		public static void clearAll ()
		{
            //multiple constraints
			daysOfWeekConstraints.Clear ();
			DhFirstLastConstraints.Clear ();
			EQTypeConstraints.Clear ();
			OvernightCitiesConstraints.Clear ();
			CitiesLegsConstraints.Clear ();
			CommutabilityConstraints.Clear ();
			StartDayofWeekConstraints.Clear ();
			RestConstraints.Clear ();
			TripLengthConstraints.Clear ();
			WorkBlockLengthConstraints.Clear ();
			CmutDHsConstraints.Clear ();
			PDOConstraints.Clear ();
			IntlNonConusConstraints.Clear ();
            StartDayConstraints.Clear();
            ReportReleaseConstraints.Clear();
		}

		public static Dictionary <string,CGPoint> HelpPageOffset = new Dictionary <string, CGPoint> () {
			{ "Aircraft Changes" , new CGPoint (0, 4232) },
			{ "Blocks of Days Off" , new CGPoint (0, 4653) },
			{ "Cmut DHs" , new CGPoint (0, 5274) },
			{ "Commutable Lines" , new CGPoint (0, 6083) },
			{ "Commutability" , new CGPoint (0, 6083) },
			{ "Days of the Month" , new CGPoint (0, 8813) },
			{ "Days of the Week" , new CGPoint (0, 8312) },
			{ "DH - first - last" , new CGPoint (0, 9129) },
			{ "Duty period" , new CGPoint (0, 9719) },
			{ "Equipment Type" , new CGPoint (0, 10230) },
			{ "Flight Time" , new CGPoint (0, 10609) },
			{ "Ground Time" , new CGPoint (0, 11100) },
			{ "Intl – NonConus" , new CGPoint (0, 11436) },
			{ "Legs Per Duty Period" , new CGPoint (0, 11983) },
			{ "Legs Per Pairing" , new CGPoint (0, 12298) },
			{ "Min Pay" , new CGPoint (0, 16753) },
			{ "Number of Days Off" , new CGPoint (0, 12814) },
			{ "Overnight Cities" , new CGPoint (0, 13111) },
			{ "Cities-Legs" , new CGPoint (0, 13111) },
			{ "Overnight Cities - Bulk" , new CGPoint (0, 0) },
			{ "PDO" , new CGPoint (0, 13637) },
			{ "Rest" , new CGPoint (0, 14453) },
            { "Start Day" , new CGPoint (0, 14453) },
			{ "Start Day of Week" , new CGPoint (0, 13951) },
			{ "Time-Away-From-Base" , new CGPoint (0, 14941) },
			{ "Trip Length" , new CGPoint (0, 15507) },
			{ "Work Blk Length" , new CGPoint (0, 15901) },
			{ "Work Days" , new CGPoint (0, 16452) },
			{ "3-on-3-off" , new CGPoint (0, 17219) },
			{ "Mixed Hard/Reserve" , new CGPoint (0, 17219) }
		};

	}
}

