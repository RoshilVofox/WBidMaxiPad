using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace WBid.WBidiPad.iOS
{
	public class ColorClass
	{
		public ColorClass ()
		{
		}
		// Summary View Colors.
		public static UIColor TopHeaderColor = UIColor.FromRGB (133, 192, 57);	//Main bar color.
		public static UIColor BottomHeaderColor = UIColor.FromRGB (238, 255, 217);	//View bar color.
		public static UIColor SummaryHeaderColor = UIColor.FromRGB (207, 226, 183);	//Summary header background.
		public static UIColor SummaryHeaderBorderColor = UIColor.FromRGB (158, 179, 131); //Summary header Border.

		public static UIColor AlternatingLineColor1 = UIColor.FromRGB (255, 255, 255); 
		public static UIColor AlternatingLineColor2 = UIColor.FromRGB (249, 249, 249); 
		public static UIColor ListSeparatorColor = UIColor.FromRGB (220, 220, 220);

		public static UIColor BlankLineColor = UIColor.FromRGB (254, 255, 0);
		public static UIColor ReserveLineColor = UIColor.FromRGB (253, 181, 193);

		public static UIColor activeGreen = UIColor.FromRGB (133, 192, 57);
		public static UIColor activeOrange = UIColor.FromRGB (248, 128, 0);

		public static UIColor OffDayColor = UIColor.FromRGB (226, 0, 9);
		public static UIColor WorkDayColor = UIColor.FromRGB (135,201,39);

		public static UIColor SummaryViewAMlineColor = UIColor.FromRGB(216, 109, 148);
		public static UIColor SummaryViewPMLineColor = UIColor.FromRGB(105, 143, 239);

		// Modern View Colors.
		public static UIColor normDayColor = UIColor.FromRGB (254, 247, 170); 
		public static UIColor weekendDayColor = UIColor.FromRGB (253, 209, 163);
		public static UIColor nextMonthDayColor = UIColor.FromRGB (211, 124, 97);

		public static UIColor normTripColor = UIColor.FromRGB (255, 250, 240);
		public static UIColor weekendTripColor = UIColor.FromRGB (254, 235, 211);
		public static UIColor nextMonthTripColor = UIColor.FromRGB (212, 150, 144);

		public static UIColor AMTripColor = UIColor.FromRGB (216, 109, 148);
		public static UIColor PMTripColor = UIColor.FromRGB (105, 143, 239);
		public static UIColor MixedTripColor = UIColor.FromRGB (254, 255, 0);

		public static UIColor AMReserveTripColor = UIColor.FromRGB (253, 191, 203);
		public static UIColor PMReserveTripColor = UIColor.FromRGB (177, 195, 223);

        public static UIColor VacationTripColor = UIColor.FromRGB(0, 255, 0);
        public static UIColor VacationTripDayColor = UIColor.FromRGB(0, 240, 0);
        public static UIColor VAPColor = UIColor.FromRGB(64, 179, 26);
		public static UIColor VacationDropTripColor = UIColor.FromRGB(222, 54, 37);
        public static UIColor VacationOverlapTripColor = UIColor.FromRGB(152, 251, 152);
        public static UIColor OverlapColor = UIColor.FromRGB(243, 108, 108);
        public static UIColor FVVacationColor = UIColor.FromRGB(102, 179, 255);

		public static UIColor CFVVacationColor = UIColor.FromRGB(135, 131, 120);
		public static UIColor ReadyReserveTripColor = UIColor.FromRGB(255, 165, 0);


	}
}

