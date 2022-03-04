using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.SharedLibrary;
using System.Linq;
using WBid.WBidiPad.Core;
using System.Collections.Generic;

namespace WBid.WBidiPad.iOS
{
	public partial class BidLineViewCell : UITableViewCell
	{
		class MyPopDelegate : UIPopoverControllerDelegate
		{
			BidLineViewCell _parent;
			public MyPopDelegate (BidLineViewCell parent)
			{
				_parent = parent;
			}

			public override bool ShouldDismiss (UIPopoverController popoverController)
			{
				return true;
				//commented by Roshil on 26-8-2021 to allow the user to set less than 5 properties
				//if (CommonClass.bidLineProperties.Count == 5) {
				//	return true;
				//} else {
				//                UIWindow WindowAlert = new UIWindow(UIScreen.MainScreen.Bounds);
				//                WindowAlert.RootViewController = new UIViewController();
				//                UIAlertController okAlertController = UIAlertController.Create("Additional Columns", "Please select \"5\" columns", UIAlertControllerStyle.Alert);
				//                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => { WindowAlert.Dispose(); }));
				//                WindowAlert.MakeKeyAndVisible();
				//                WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);

				//                return false;
				//}
			}
		}

		public static readonly UINib Nib = UINib.FromName ("BidLineViewCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("BidLineViewCell");
		UIPopoverController popoverController;
		UITapGestureRecognizer singleTap;
		UITapGestureRecognizer singleTap2;
		NSObject arrObserver,arrShowPopOverNotification;
//		UITapGestureRecognizer tipGest;
//		UITapGestureRecognizer tipGest2;
//		List <BidLineTemplate> template1 = new List<BidLineTemplate> ();

		public BidLineViewCell (IntPtr handle) : base (handle)
		{
			
		}

		public static BidLineViewCell Create ()
		{
			return (BidLineViewCell)Nib.Instantiate (null, null) [0];
		}


		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			UILongPressGestureRecognizer longPress1 = new UILongPressGestureRecognizer(handleLongPress);
			this.vwChild1.AddGestureRecognizer(longPress1);
			longPress1.DelaysTouchesBegan = true;

			UILongPressGestureRecognizer longPress2 = new UILongPressGestureRecognizer(handleLongPress);
			this.vwChild2.AddGestureRecognizer(longPress2);
			longPress2.DelaysTouchesBegan = true;

			foreach (UIView vw in vwChild3)
			{
				UILongPressGestureRecognizer longPress3 = new UILongPressGestureRecognizer(handleLongPress);
				vw.AddGestureRecognizer(longPress3);
				longPress3.DelaysTouchesBegan = true;
			}

			UITapGestureRecognizer addColTap = new UITapGestureRecognizer (HandleAdditionalColumn);
			vwChild2.AddGestureRecognizer (addColTap);
			addColTap.NumberOfTapsRequired = 1;

		}

        public void bindData(Line line, NSIndexPath indexpath)
        {
			//			template1 = line.BidLineTemplates;

			if (indexpath.Row == 0)
			{
				if(arrShowPopOverNotification == null)
				arrShowPopOverNotification = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ShowPopOverView"), ShowPopOverView);
			}

            btnLineOption.Hidden = true;
            this.lblSlNo.Text = (indexpath.Row + 1).ToString();
            this.lblLineNumber.Text = line.LineDisplay;
            this.lblLineName.Text = string.Join("", line.FAPositions.ToArray());
            this.lblPairingDesc.Text = line.Pairingdesription;

            this.imgLockIcon.Image = null;
            this.imgCrossIcon.Image = null;
            this.imgOverlapIcon.Image = null;

            if (line.TopLock)
                this.imgLockIcon.Image = UIImage.FromBundle("lockIconGreen.png");
            if (line.BotLock)
                this.imgLockIcon.Image = UIImage.FromBundle("lockIconRed.png");
            if (line.Constrained)
                this.imgCrossIcon.Image = UIImage.FromBundle("deleteIconBold.png");
            if (line.ShowOverLap)
                this.imgOverlapIcon.Image = UIImage.FromBundle("overlayIconBold.png");

            this.btnLineSelect.Tag = line.LineNum;
            this.btnLineSelect.SetImage(UIImage.FromBundle("roundActive.png"), UIControlState.Selected);
            this.btnLineSelect.SetImage(UIImage.FromBundle("roundNormal.png"), UIControlState.Normal);
            setSelectButton(line.LineNum);
			for (int i = 0; i < 5; i++)
			{
				lblPropName[i].Text = string.Empty;
				lblPropValue[i].Text = string.Empty;
			}
			for (int i = 0; i < CommonClass.bidLineProperties.Count; i++) {
				lblPropName [i].Text = CommonClass.bidLineProperties [i];
				lblPropValue [i].Text = CommonClass.GetLineProperty (CommonClass.bidLineProperties [i], line);
			}

			int index = 0;
			foreach (BidLineTemplate temp in line.BidLineTemplates) {
				lblPairName [index].Tag = line.LineNum;
				lblTripName [index].Tag = line.LineNum;
				this.lblCalDate [index].Text = temp.Date.Day.ToString ();
				this.lblCalDay [index].Text = temp.Date.DayOfWeek.ToString ().Substring (0, 2).ToUpper ();
				this.lblPairName [index].Text = temp.TripNum;
				this.lblTripName [index].Text = temp.ArrStaLastLeg;

                if (lblCalDay[index].Text == "SA" || lblCalDay[index].Text == "SU")
                { // Weekend Days
                    this.lblCalDate[index].TextColor = UIColor.Red;
                    this.lblCalDay[index].TextColor = UIColor.Red;
                    this.lblPairName[index].TextColor = UIColor.Red;
                    this.lblTripName[index].TextColor = UIColor.Red;
                }
                else
                {
                    this.lblCalDate[index].TextColor = UIColor.Black;
                    this.lblCalDay[index].TextColor = UIColor.Black;
                    this.lblPairName[index].TextColor = UIColor.Black;
                    this.lblTripName[index].TextColor = UIColor.Black;
                }

//				lblPairName [index].UserInteractionEnabled = true;
//				tipGest = new UITapGestureRecognizer (HandleToolTip);
//				tipGest.NumberOfTapsRequired = 1;
//				lblPairName [index].AddGestureRecognizer (tipGest);

//				lblTripName [index].UserInteractionEnabled = true;
//				tipGest2 = new UITapGestureRecognizer (HandleToolTip);
//				tipGest2.NumberOfTapsRequired = 1;
//				lblTripName [index].AddGestureRecognizer (tipGest2);

				if (temp.TripName != null) {
					this.lblPairName [index].UserInteractionEnabled = true;
					this.lblTripName [index].UserInteractionEnabled = true;
					singleTap = new UITapGestureRecognizer (() => {
						CommonClass.selectedTrip = temp.TripName;
						CommonClass.selectedLine = (int)singleTap.View.Tag;
						NSNotificationCenter.DefaultCenter.PostNotificationName ("TripPopShow", new NSString (temp.TripName));
						NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
					});
					singleTap2 = new UITapGestureRecognizer (() => {
						CommonClass.selectedTrip = temp.TripName;
						CommonClass.selectedLine = (int)singleTap2.View.Tag;
						NSNotificationCenter.DefaultCenter.PostNotificationName ("TripPopShow", new NSString (temp.TripName));
						NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
					});
					singleTap.NumberOfTapsRequired = 1;
					singleTap2.NumberOfTapsRequired = 1;

					this.lblPairName [index].AddGestureRecognizer (singleTap);
					this.lblTripName [index].AddGestureRecognizer (singleTap2);
//					tipGest.RequireGestureRecognizerToFail (singleTap);
//					tipGest2.RequireGestureRecognizerToFail (singleTap2);

				} else {
					this.lblPairName [index].UserInteractionEnabled = false;
					this.lblTripName [index].UserInteractionEnabled = false;
				}


                if (CommonClass.selectedTrip != null && CommonClass.selectedTrip == temp.TripName)
                {
                    this.lblPairName[index].BackgroundColor = ColorClass.SummaryHeaderColor;
                    this.lblTripName[index].BackgroundColor = ColorClass.SummaryHeaderColor;
                }
                else
                {
                    this.lblPairName[index].BackgroundColor = UIColor.Clear;
                    this.lblTripName[index].BackgroundColor = UIColor.Clear;
                }

                if (index == 33)
                    break;
                index++;
            }
            this.Tag = indexpath.Row;
            vwChild1.Tag = this.Tag;
            vwChild2.Tag = this.Tag;
            foreach (UIView vw in vwChild3)
            {
                vw.Tag = this.Tag;
            }
//            UILongPressGestureRecognizer longPress1 = new UILongPressGestureRecognizer(handleLongPress);
//            this.vwChild1.AddGestureRecognizer(longPress1);
//            longPress1.DelaysTouchesBegan = true;
//
//            UILongPressGestureRecognizer longPress2 = new UILongPressGestureRecognizer(handleLongPress);
//            this.vwChild2.AddGestureRecognizer(longPress2);
//            longPress2.DelaysTouchesBegan = true;
//
//            foreach (UIView vw in vwChild3)
//            {
//                UILongPressGestureRecognizer longPress3 = new UILongPressGestureRecognizer(handleLongPress);
//                vw.AddGestureRecognizer(longPress3);
//                longPress3.DelaysTouchesBegan = true;
//            }
//
//			UITapGestureRecognizer addColTap = new UITapGestureRecognizer (HandleAdditionalColumn);
//			vwChild2.AddGestureRecognizer (addColTap);
//			addColTap.NumberOfTapsRequired = 1;

		}

//        void HandleToolTip (UITapGestureRecognizer obj)
//		{
//			string strTip = string.Empty;
//
//			int index = 0;
//			foreach (UILabel lbl in lblPairName) {
//				if (lbl == obj.View)
//					strTip = template1 [index].ToolTip;
//				index++;
//			}
//			index = 0;
//			foreach (UILabel lbl in lblTripName) {
//				if (lbl == obj.View)
//					strTip = template1 [index].ToolTip;
//				index++;
//			}
//			TooltipViewController toolTip = new TooltipViewController ();
//			toolTip.tipMessage = strTip;
//			popoverController = new UIPopoverController (toolTip);
//			popoverController.PopoverContentSize = new SizeF (100, 40);
//			popoverController.PresentFromRect (obj.View.ConvertRectToView (obj.View.Bounds, this), this, UIPopoverArrowDirection.Any, true);
//        }

//		private string GetLineProperty (string displayName ,Line line)
//		{
//			if (displayName == "$/Day") {
//				return line.TfpPerDay.ToString ();
//			} else if (displayName == "$/DHr") {
//				return line.TfpPerDhr.ToString ();
//			} else if (displayName == "$/Hr") {
//				return line.TfpPerFltHr.ToString ();
//			} else if (displayName == "$/TAFB") {
//				return line.TfpPerTafb.ToString ();
//			} else if (displayName == "+Grd") {
//				return line.LongestGrndTime.ToString ();
//			} else if (displayName == "+Legs") {
//				return line.MostLegs.ToString ();
//			} else if (displayName == "+Off") {
//				return line.LargestBlkOfDaysOff.ToString ();
//			} else if (displayName == "1Dy") {
//				return line.Trips1Day.ToString ();
//			} else if (displayName == "2Dy") {
//				return line.Trips2Day.ToString ();
//			} else if (displayName == "3Dy") {
//				return line.Trips3Day.ToString ();
//			} else if (displayName == "4Dy") {
//				return line.Trips4Day.ToString ();
//			} else if (displayName == "8753") {
//				return line.Equip8753.ToString ();
//			} else if (displayName == "A/P") {
//				return line.AMPM.ToString ();
//			} else if (displayName == "ACChg") {
//				return line.AcftChanges.ToString ();
//			} else if (displayName == "ACDay") {
//				return line.AcftChgDay.ToString ();
//			} else if (displayName == "CO") {
//				return line.CarryOverTfp.ToString ();
//			} else if (displayName == "DP") {
//				return line.TotDutyPds.ToString ();
//			} else if (displayName == "DPinBP") {
//				return line.TotDutyPdsInBp.ToString ();
//			} else if (displayName == "EDomPush") {
//				return line.EDomPush;
//			} else if (displayName == "EPush") {
//				return line.EPush;
//			} else if (displayName == "FA Posn") {
//				return string.Join ("", line.FAPositions.ToArray ());
//			} else if (displayName == "Flt") {
//				return line.BlkHrsInBp;
//			} else if (displayName == "LArr") {
//				return line.LastArrTime.ToString ();
//			} else if (displayName == "LDomArr") {
//				return line.LastDomArrTime.ToString ();
//			} else if (displayName == "Legs") {
//				return line.Legs.ToString ();
//			} else if (displayName == "LgDay") {
//				return line.LegsPerDay.ToString ();
//			} else if (displayName == "LgPair") {
//				return line.LegsPerPair.ToString ();
//			} else if (displayName == "ODrop") {
//				return line.OverlapDrop.ToString ();
//			} else if (displayName == "Off") {
//				return line.DaysOff.ToString ();
//			} else if (displayName == "Pairs") {
//				return line.TotPairings.ToString ();
//			} else if (displayName == "Pay") {
//				return  Decimal.Round (line.Tfp, 2).ToString ();
//			} else if (displayName == "PDiem") {
//				return line.TafbInBp;
//			} else if (displayName == "MyValue") {
//				return Decimal.Round (line.Points, 2).ToString ();
//			} else if (displayName == "SIPs") {
//				return line.Sips.ToString ();
//			} else if (displayName == "StartDOW") {
//				return line.StartDow;
//			} else if (displayName == "T234") {
//				return line.T234;
//			} else if (displayName == "VDrop") {
//				return line.VacationDrop.ToString ();
//			} else if (displayName == "WkEnd") {
//				if (line.Weekend != null)
//					return line.Weekend.ToLower ();
//				else
//					return "";
//			} else if (displayName == "FltRig") {
//				return line.RigFltInBP.ToString ();
//			} else if (displayName == "MinPayRig") {
//				return line.RigDailyMinInBp.ToString ();
//			} else if (displayName == "DhrRig") {
//				return line.RigDhrInBp.ToString ();
//			} else if (displayName == "AdgRig") {
//				return line.RigAdgInBp.ToString ();
//			} else if (displayName == "TafbRig") {
//				return line.RigTafbInBp.ToString ();
//			} else if (displayName == "TotRig") {
//				return line.RigTotalInBp.ToString ();
//			} else if (displayName == "VacPay") {
//				return  Decimal.Round (line.VacPay, 2).ToString ();
//			} else if (displayName == "Vofrnt") {
//				return  Decimal.Round (line.VacationOverlapFront, 2).ToString ();
//			} else if (displayName == "Vobk") {
//				return  Decimal.Round (line.VacationOverlapBack, 2).ToString ();
//			} else if (displayName == "800legs") {
//				return line.LegsIn800.ToString ();
//			} else if (displayName == "700legs") {
//				return line.LegsIn700.ToString ();
//			} else if (displayName == "500legs") {
//				return line.LegsIn500.ToString ();
//			} else if (displayName == "300legs") {
//				return line.LegsIn300.ToString ();
//			} else if (displayName == "DhrInBp") {
//				return line.DutyHrsInBp;
//			} else if (displayName == "DhrInLine") {
//				return line.DutyHrsInLine;
//			} else if (displayName == "Wts") {
//				return Decimal.Round (line.TotWeight, 2).ToString ();
//			} else if (displayName == "LineRig") {
//				return Decimal.Round (line.LineRig, 2).ToString ();
//			} else {
//				return "";
//			}
//		}

		void HandleAdditionalColumn (UITapGestureRecognizer obj)
		{
			arrObserver = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("dismissClassicPopover"), dismissPopover);
			PopoverViewController popoverContent = new PopoverViewController ();
			popoverContent.PopType = "BidlineColumns";
			popoverController = new UIPopoverController (popoverContent);
			popoverController.Delegate = new MyPopDelegate (this);
			popoverController.PopoverContentSize = new CGSize (200, 500);
			popoverController.PresentFromRect (obj.View.Frame, this, UIPopoverArrowDirection.Any, true);
		}

		private void ShowPopOverView(NSNotification n)
		{
			arrObserver = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("dismissClassicPopover"), dismissPopover);
			PopoverViewController popoverContent = new PopoverViewController();
			popoverContent.PopType = "BidlineColumns";
			popoverController = new UIPopoverController(popoverContent);
			popoverController.Delegate = new MyPopDelegate(this);
			popoverController.PopoverContentSize = new CGSize(200, 500);
			popoverController.PresentFromRect(vwChild2.Frame, this, UIPopoverArrowDirection.Left, true);
		}

		private void dismissPopover(NSNotification n)
		{
			popoverController.Dismiss(true);
			if (arrObserver != null)
				NSNotificationCenter.DefaultCenter.RemoveObserver(arrObserver);

		}
		public void handleLongPress (UILongPressGestureRecognizer gest) 
		{
			
			NSIndexPath path = NSIndexPath.FromRowSection (gest.View.Tag, 0);
			if (path != null) {
				CommonClass.selectedLine = GlobalSettings.Lines [path.Row].LineNum;
				if (gest.State == UIGestureRecognizerState.Began)
					//this.PerformSelector(new ObjCRuntime.Selector("ViewShow1"), null, 0.5);
					NSNotificationCenter.DefaultCenter.PostNotificationName ("ShowPopover", path);
			}
		}

		private void setSelectButton(int row)
		{
			if (CommonClass.selectedRows.Contains(row))
			{
				this.btnLineSelect.Selected = true;
			}
			else
			{
				this.btnLineSelect.Selected = false;
			}

		}

		// Handles selection of lines.
		partial void btnLineSelectTap (UIKit.UIButton sender)
		{
			sender.Selected = !sender.Selected;
			NSNumber row = new NSNumber(sender.Tag);
			NSNotificationCenter.DefaultCenter.PostNotificationName("SumRowSelected", row);
		}

	}
}

