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
using System.IO;


namespace WBid.WBidiPad.iOS
{
	public partial class OvernightBulkColl : UICollectionViewCell
	{
		class MyPopDelegate : UIPopoverControllerDelegate
		{
			OvernightBulkColl _parent;
			public MyPopDelegate (OvernightBulkColl parent)
			{
				_parent = parent;
			}
			public override void DidDismiss (UIPopoverController popoverController)
			{
				_parent.popoverController = null;
				NSNotificationCenter.DefaultCenter.RemoveObserver (_parent.overnightBulkWeightNotif);
			}
		}

		public static readonly UINib Nib = UINib.FromName ("OvernightBulkColl", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("OvernightBulkColl");
		public string DisplayMode;
		NSObject overnightBulkWeightNotif;
		UIPopoverController popoverController;
		WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		List<City> lstOvernightCities = GlobalSettings.WBidINIContent.Cities;

		public OvernightBulkColl (IntPtr handle) : base (handle)
		{
		}

		public static OvernightBulkColl Create ()
		{
			return (OvernightBulkColl)Nib.Instantiate (null, null) [0];
		}

		public void bindData (City city)
		{
			if (DisplayMode == "Constraints") {
				btnCityWt.Hidden = true;
				lblCity.Text = city.Name;

				if (city.Status == 0) {
					lblCity.TextColor = UIColor.Black;
					lblCity.BackgroundColor = UIColor.Clear;
				} else if (city.Status == 2) {
					lblCity.TextColor = UIColor.White;
					lblCity.BackgroundColor = ColorClass.OffDayColor;
				} else if (city.Status == 1) {
					lblCity.TextColor = UIColor.White;
					lblCity.BackgroundColor = ColorClass.WorkDayColor;
				}
				else if (city.Status == 3)
				{
					lblCity.TextColor = UIColor.White;
					lblCity.BackgroundColor = UIColor.Black;
				}
			} else {
				if (GlobalSettings.OverNightCitiesInBid.Any(x => x.Name == city.Name))
				{
					lblCity.BackgroundColor = UIColor.White;
					lblCity.TextColor = UIColor.Black;

				}
				else
				{
					lblCity.BackgroundColor = UIColor.Black;
					lblCity.TextColor = UIColor.White;
				}
				btnCityWt.Tag = city.Id;
				//lblCity.TextAlignment = UITextAlignment.Left;
				btnCityWt.Layer.BorderWidth = 1;
				lblCity.Text = city.Name;
				//lblCity.BackgroundColor = UIColor.Black;
				//lblCity.TextColor = UIColor.White;
				List<Wt2Parameter> lstWeight = wBIdStateContent.Weights.OvernightCitybulk;
				btnCityWt.SetTitleColor (UIColor.Black, UIControlState.Normal);
				btnCityWt.BackgroundColor = UIColor.Clear;

				if (lstWeight.Any (x => x.Type == city.Id)) {
					decimal weight = lstWeight.FirstOrDefault (x => x.Type == city.Id).Weight;
					btnCityWt.SetTitle (weight.ToString (), UIControlState.Normal);
					if (weight < 0) {
						btnCityWt.SetTitleColor (UIColor.White, UIControlState.Normal);
						btnCityWt.BackgroundColor = ColorClass.OffDayColor;
					} else if (weight > 0) {
						btnCityWt.SetTitleColor (UIColor.Black, UIControlState.Normal);
						btnCityWt.BackgroundColor = ColorClass.WorkDayColor;
					}
				} else
					btnCityWt.SetTitle ("0", UIControlState.Normal);

			}

		}

		partial void btnCityWtTapped (UIKit.UIButton sender)
		{
			List<Wt2Parameter> lstWeight = wBIdStateContent.Weights.OvernightCitybulk;

			City city = lstOvernightCities.FirstOrDefault(x=>x.Id == sender.Tag);

			overnightBulkWeightNotif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString("changeOvernightBulkWeight"), handleOvernightBulkWeight);
			PopoverViewController popoverContent = new PopoverViewController ();
			popoverContent.PopType = "changeOvernightBulkWeight";
			popoverContent.SubPopType = "Overnight Cities - Bulk";
			if (lstWeight.Any (x => x.Type == city.Id)) {
				popoverContent.numValue = lstWeight.FirstOrDefault (x => x.Type == city.Id).Weight.ToString ();
			} else {
				popoverContent.numValue = "0";
			}
			popoverContent.index = city.Id;
			popoverController = new UIPopoverController (popoverContent);
			popoverController.Delegate = new MyPopDelegate (this);
			popoverController.PopoverContentSize = new CGSize (210, 300);
			popoverController.PresentFromRect(sender.Frame,this,UIPopoverArrowDirection.Any,true);

		}
		void handleOvernightBulkWeight (NSNotification obj)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (overnightBulkWeightNotif);
			popoverController.Dismiss (true);
			btnCityWt.SetTitle (obj.Object.ToString (), UIControlState.Normal);
		}

	}
}

