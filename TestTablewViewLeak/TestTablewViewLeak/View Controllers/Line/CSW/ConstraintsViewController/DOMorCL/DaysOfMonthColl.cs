using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using System.Linq;
using WBid.WBidiPad.SharedLibrary.Utility;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using System.Collections.Generic;
using WBid.WBidiPad.Core.Enum;
using CoreGraphics;


namespace WBid.WBidiPad.iOS
{
	public partial class DaysOfMonthColl : UICollectionViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("DaysOfMonthColl", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("DaysOfMonthColl");
		public string DisplayMode;
		WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

		public DaysOfMonthColl (IntPtr handle) : base (handle)
		{
		}

		public static DaysOfMonthColl Create ()
		{
			return (DaysOfMonthColl)Nib.Instantiate (null, null) [0];
		}

		public void bindData (DayOfMonth data)
		{
			if (DisplayMode == "Constraints") {
				this.Tag = data.Id;
				if (data.Status == 0) {
					this.lblDay.Text = data.Day.ToString ();
					this.lblDay.TextColor = UIColor.Black;
					this.lblDay.BackgroundColor = UIColor.Clear;

				} else if (data.Status == 1) {
					this.lblDay.Text = "Work";
					this.lblDay.TextColor = UIColor.White;
					this.lblDay.BackgroundColor = ColorClass.WorkDayColor;

				} else if (data.Status == 2) {
					this.lblDay.Text = "Off";
					this.lblDay.TextColor = UIColor.White;
					this.lblDay.BackgroundColor = ColorClass.OffDayColor;
				}

			} else {
				this.Tag = data.Id;
				this.lblDay.Text = data.Day.ToString ();
				this.lblDay.TextColor = UIColor.Black;
				this.lblDay.BackgroundColor = UIColor.Clear;

				List<Wt> lstWeight = wBIdStateContent.Weights.SDO.Weights;
				foreach (Wt wt in lstWeight) {
					if (this.Tag == wt.Key) {
						if (wt.Value < 0) {
							this.lblDay.Text = wt.Value.ToString ();
							this.lblDay.TextColor = UIColor.White;
							this.lblDay.BackgroundColor = ColorClass.OffDayColor;
						} else if (wt.Value > 0) {
							this.lblDay.Text = wt.Value.ToString ();
							this.lblDay.TextColor = UIColor.White;
							this.lblDay.BackgroundColor = ColorClass.WorkDayColor;
						} 
					}
				}
			}
		}
	}
}

