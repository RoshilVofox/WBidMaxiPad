using System;
using UIKit;
using System.Drawing;

namespace WBid.WBidiPad.iOS
{
	public class CalendarButton: UIButton
	{

		public int ID;
		public int State;
		public DateTime Date;
		public CalendarButton (string text, int id)
//			: base (UIButtonType.Custom)
		{
			ID = id;
			SetTitle (text, UIControlState.Normal);
		}
		public CalendarButton ():base(UIButtonType.System){
			State = 0;
		}
		public CalendarButton(IntPtr handle) : base(handle) { }
	}
}

