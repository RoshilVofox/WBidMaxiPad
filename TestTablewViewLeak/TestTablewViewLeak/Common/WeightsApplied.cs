using System;
using System.Collections.Generic;
using CoreGraphics;

namespace WBid.WBidiPad.iOS
{
	public class WeightsApplied
	{
		public WeightsApplied ()
		{
		}

		public static List <string> MainList = new List <string> ()
		{ "Aircraft Changes", "AM/PM", "Blocks of Days Off", "Cmut DHs", "Commutable Lines - Manual", "Days of the Month", "Days of the Week", 
			"DH - first - last", "Duty period", "Equipment Type", "ETOPS","ETOPS-Res","Flight Time", "Ground Time", "Intl – NonConus", 
			"Largest Block of Days Off", "Legs Per Duty Period", "Legs Per Pairing", "Normalize Days Off", "Number of Days Off", "Overnight Cities", 
			"Overnight Cities - Bulk", "PDO-after", "PDO-before", "Position", "Start Day of Week", "Rest", "Time-Away-From-Base", "Trip Length", 
			"Work Blk Length", "Work Days","Commutable Line - Auto","Cities-Legs"
		};

		public static List <string> AMPMWeights = new List <string> ();
		public static List <string> BlocksOfDaysOffWeights = new List <string> ();
		public static List <string> CmutDHsWeights = new List <string> ();
		public static List <string> dhFirstLastWeights = new List <string> ();
		public static List <string> DutyPeriodWeights = new List <string> ();
		public static List <string> EQTypeWeights = new List <string> ();
		public static List<string> ETOPSWeights = new List<string>();
		public static List<string> ETOPSResWeights = new List<string>();
		public static List <string> FlightTimeWeights = new List <string> ();
		public static List <string> GroundTimeWeights = new List <string> ();
		public static List <string> IntlNonConusWeights = new List <string> ();
		public static List <string> LegsPerDutyPeriodWeights = new List <string> ();
		public static List <string> LegsPerPairingWeights = new List <string> ();
		public static List <string> NumOfDaysOffWeights = new List <string> ();
		public static List <string> OvernightCitiesWeights = new List <string> ();
		public static List <string> CitiesLegsWeights = new List <string> ();
		public static List <string> Commutabilityweights = new List <string> ();
		public static List <string> PDOAfterWeights = new List <string> ();
		public static List <string> PDOBeforeWeights = new List <string> ();
		public static List <string> PositionWeights = new List <string> ();
		public static List <string> StartDOWWeights = new List <string> ();
		public static List <string> TripLengthWeights = new List <string> ();
		public static List <string> WorkBlockLengthWeights = new List <string> ();
		public static List <string> WorkDaysWeights = new List <string> ();
		public static List <string> RestWeights = new List <string> ();

		public static void clearAll () {

			AMPMWeights.Clear ();
			BlocksOfDaysOffWeights.Clear ();
			CmutDHsWeights.Clear ();
			dhFirstLastWeights.Clear ();
			DutyPeriodWeights.Clear ();
			EQTypeWeights.Clear ();
			ETOPSWeights.Clear();
			ETOPSResWeights.Clear();
			FlightTimeWeights.Clear ();
			GroundTimeWeights.Clear ();
			IntlNonConusWeights.Clear ();
			LegsPerDutyPeriodWeights.Clear ();
			LegsPerPairingWeights.Clear ();
			NumOfDaysOffWeights.Clear ();
			OvernightCitiesWeights.Clear ();
			PDOAfterWeights.Clear ();
			PDOBeforeWeights.Clear ();
			PositionWeights.Clear ();
			StartDOWWeights.Clear ();
			TripLengthWeights.Clear ();
			WorkBlockLengthWeights.Clear ();
			WorkDaysWeights.Clear ();
			RestWeights.Clear ();
			CitiesLegsWeights.Clear ();
			Commutabilityweights.Clear ();

		}

		public static Dictionary <string,CGPoint> HelpPageOffset = new Dictionary <string, CGPoint> () {
			{ "Aircraft Changes" , new CGPoint (0, 1698) },
			{ "AM/PM" , new CGPoint (0, 2227) },
			{ "Blocks of Days Off" , new CGPoint (0, 3003) },
			{ "Cmut DHs" , new CGPoint (0, 3483) },
			{ "Commutable Lines" , new CGPoint (0, 3892) },
			{ "Commutability" , new CGPoint (0, 3892) },
			{ "Days of the Month" , new CGPoint (0, 5081) },
			{ "Days of the Week" , new CGPoint (0, 5595) },
			{ "DH - first - last" , new CGPoint (0, 6065) },
			{ "Duty period" , new CGPoint (0, 6354) },
			{ "Equipment Type" , new CGPoint (0, 6862) },
			{ "ETOPS" , new CGPoint (0, 6862) },
			{ "ETOPS-Res" , new CGPoint (0, 6862) },
			{ "Flight Time" , new CGPoint (0, 7141) },
			{ "Ground Time" , new CGPoint (0, 7397) },
			{ "Intl – NonConus" , new CGPoint (0, 7808) },
			{ "Largest Block of Days Off" , new CGPoint (0, 8109) },
			{ "Legs Per Duty Period" , new CGPoint (0, 8661) },
			{ "Legs Per Pairing" , new CGPoint (0, 8881) },
			{ "Normalize Days Off" , new CGPoint (0, 9117) },
			{ "Number of Days Off" , new CGPoint (0, 9591) },
			{ "Overnight Cities" , new CGPoint (0, 9973) },
			{ "Cities-Legs" , new CGPoint (0, 9973) },
			{ "Overnight Cities - Bulk" , new CGPoint (0, 0) },
			{ "PDO-after" , new CGPoint (0, 10323) },
			{ "PDO-before" , new CGPoint (0, 10545) },
			{ "Position" , new CGPoint (0, 10764) },
			{ "Rest" , new CGPoint (0, 11121) },
			{ "Start Day of Week" , new CGPoint (0, 11516) },
			{ "Time-Away-From-Base" , new CGPoint (0, 11913) },
			{ "Trip Length" , new CGPoint (0, 12446) },
			{ "Work Blk Length" , new CGPoint (0, 12938) },
			{ "Work Days" , new CGPoint (0, 13697) }
		};

	}
}

