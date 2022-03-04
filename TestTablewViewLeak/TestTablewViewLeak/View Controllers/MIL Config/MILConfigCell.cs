
using System;
using System.Drawing;

using Foundation;
using UIKit;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using CoreGraphics;

namespace WBid.WBidiPad.iOS
{
	public partial class MILConfigCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("MILConfigCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("MILConfigCell");

		public MILConfigCell (IntPtr handle) : base (handle)
		{
		}

		public static MILConfigCell Create ()
		{
			return (MILConfigCell)Nib.Instantiate (null, null) [0];
		}

		public void BindData (Absense abs, int index)
		{
			if (CommonClass.MILList.Count == 1)
				btnClose.Hidden = true;
			else
				btnClose.Hidden = false;

			btnClose.Tag = index;
			btnStart.Tag = index;
			btnEnd.Tag = index;
			btnStart.SetTitle (abs.StartAbsenceDate.ToString ("MM/dd/yyyy HH:mm"), UIControlState.Normal);
			btnEnd.SetTitle (abs.EndAbsenceDate.ToString ("MM/dd/yyyy HH:mm"), UIControlState.Normal);
		}

		partial void btnStartTapped (UIKit.UIButton sender)
		{
			var content = new UIViewController ();
			content.View.Frame = new CGRect (0, 0, 320, 200);
			var picker = new UIDatePicker (new CGRect (0, 0, 320, 200));
			picker.TimeZone = NSTimeZone.SystemTimeZone;
			picker.Mode = UIDatePickerMode.DateAndTime;
//			picker.Locale = new NSLocale ("NL");
			content.View.AddSubview (picker);

			//EditedByGregory
			int SystemVersion = Convert.ToInt16(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]);

			if (SystemVersion == 14)
			{
				picker.PreferredDatePickerStyle = UIDatePickerStyle.Wheels;
			}
			
			picker.ValueChanged += (object pkr, EventArgs e) => {
				CommonClass.MILList [(int)((UIDatePicker)pkr).Tag].StartAbsenceDate = ((UIDatePicker)pkr).Date.NSDateToDateTime ();
				NSNotificationCenter.DefaultCenter.PostNotificationName ("MILReload", null);
			};
			picker.Date = DateTime.Parse (sender.Title (UIControlState.Normal)).DateTimeToNSDate ();
			picker.Tag = sender.Tag;
			var popover = new UIPopoverController (content);
			popover.PopoverContentSize = new CGSize(320, 200);
			popover.PresentFromRect (sender.Frame, this.ContentView, UIPopoverArrowDirection.Any, true);
			popover.DidDismiss+= delegate {
				popover = null;
			};
		}

		partial void btnEndTapped (UIKit.UIButton sender)
		{
			var content = new UIViewController ();
			content.View.Frame = new CGRect (0, 0, 320, 200);
			var picker = new UIDatePicker (new CGRect (0, 0, 320, 200));
			//EditedByGregory
			int SystemVersion = Convert.ToInt16(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]);

			if (SystemVersion == 14)
			{
				picker.PreferredDatePickerStyle = UIDatePickerStyle.Wheels;
			}
			picker.TimeZone = NSTimeZone.SystemTimeZone;
			picker.Mode = UIDatePickerMode.DateAndTime;
//			picker.Locale = new NSLocale ("NL");
			content.View.AddSubview (picker);
			picker.ValueChanged += (object pkr, EventArgs e) => {
				CommonClass.MILList [(int)((UIDatePicker)pkr).Tag].EndAbsenceDate = ((UIDatePicker)pkr).Date.NSDateToDateTime ();
				NSNotificationCenter.DefaultCenter.PostNotificationName ("MILReload", null);
			};
			picker.Date = DateTime.Parse (sender.Title (UIControlState.Normal)).DateTimeToNSDate ();
			picker.Tag = sender.Tag;
			var popover = new UIPopoverController (content);
			popover.PopoverContentSize = new CGSize(320, 200);
			popover.PresentFromRect (sender.Frame, this.ContentView, UIPopoverArrowDirection.Any, true);
			popover.DidDismiss+= delegate {
				popover = null;
			};
		}

		partial void btnCloseTapped (UIKit.UIButton sender)
		{
			CommonClass.MILList.RemoveAt ((int)sender.Tag);
			NSNotificationCenter.DefaultCenter.PostNotificationName ("MILReload", null);
		}
	}
}

