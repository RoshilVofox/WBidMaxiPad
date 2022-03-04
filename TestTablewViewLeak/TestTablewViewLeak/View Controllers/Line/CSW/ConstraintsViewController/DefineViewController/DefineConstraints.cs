using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using System.IO;
using WBid.WBidiPad.PortableLibrary;
using System.Collections.Generic;
using System.Linq;
namespace WBid.WBidiPad.iOS
{
	public partial class DefineConstraints : UIViewController
	{
		class MyPopDelegate : UIPopoverControllerDelegate
		{
			DefineConstraints _parent;
			public MyPopDelegate (DefineConstraints parent)
			{
				_parent = parent;
			}
			public override void DidDismiss (UIPopoverController popoverController)
			{
				_parent.popoverController = null;
				NSNotificationCenter.DefaultCenter.RemoveObserver (_parent.defineNotif);

				_parent.btnAMPush.Selected = false;
				_parent.btnAmArrive.Selected = false;
				_parent.btnPMPush.Selected = false;
				_parent.btnPMArrive.Selected = false;
				_parent.btnNTEPush.Selected = false;
				_parent.btnNTEArrive.Selected = false;
				_parent.btnMaxNumber.Selected = false;
				_parent.btnMaxPercentage.Selected = false;
			}
		}

		NSObject defineNotif;
		UIPopoverController popoverController;

		public DefineConstraints () : base ("DefineConstraints", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
            sgAMPM.SelectedSegment = GlobalSettings.WBidINIContent.AmPmConfigure.HowCalcAmPm - 1;
            sgLineParameter.SelectedSegment = GlobalSettings.WBidINIContent.AmPmConfigure.NumberOrPercentageCalc - 1;
            txtFirstAMsPushAfter.Text = GlobalSettings.WBidINIContent.AmPmConfigure.AmPush.ToString(@"hh\:mm");
            txtPMsPushAfter.Text = GlobalSettings.WBidINIContent.AmPmConfigure.PmPush.ToString(@"hh\:mm");
			txtNTEPushAfter.Text = GlobalSettings.WBidINIContent.AmPmConfigure.NitePush.ToString(@"hh\:mm");
            txtArriveBeforeFirst.Text = GlobalSettings.WBidINIContent.AmPmConfigure.AmLand.ToString(@"hh\:mm");
            txtArriveBeforeSecond.Text = GlobalSettings.WBidINIContent.AmPmConfigure.PmLand.ToString(@"hh\:mm");
            txtArriveBeforeThird.Text = GlobalSettings.WBidINIContent.AmPmConfigure.NiteLand.ToString(@"hh\:mm");
            txtMaxNumber.Text = GlobalSettings.WBidINIContent.AmPmConfigure.NumOpposites.ToString();
            txtMaxPercentage.Text = GlobalSettings.WBidINIContent.AmPmConfigure.PctOpposities.ToString();

			this.btnCancel.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnOK.SetBackgroundImage(UIImage.FromBundle ("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);


			// Perform any additional setup after loading the view, typically from a nib.
		}
		partial void btnResetTapped (Foundation.NSObject sender)
		{
            //SelectedAmPmDefemition = AmPmDefemition.Where(x => x.Id == 3).FirstOrDefault();
            //SelectedAMPmLineParameter = AMPmLineParameter.Where(x => x.Id == 2).FirstOrDefault();
            txtFirstAMsPushAfter.Text = TimeSpan.FromHours(4).ToString(@"hh\:mm");
            txtPMsPushAfter.Text = TimeSpan.FromHours(11).ToString(@"hh\:mm");
			txtNTEPushAfter.Text = TimeSpan.FromHours(22).ToString(@"hh\:mm");
            txtArriveBeforeFirst.Text = TimeSpan.FromHours(19).ToString(@"hh\:mm");
            txtArriveBeforeSecond.Text = TimeSpan.FromHours(2).ToString(@"hh\:mm");
            txtArriveBeforeThird.Text = TimeSpan.FromHours(7).ToString(@"hh\:mm");

            txtMaxNumber.Text = "3";
            txtMaxPercentage.Text = "20";
		}
		partial void sgAMPMTapped (UIKit.UISegmentedControl sender)
		{
			if(sender.SelectedSegment == 0)
			{
			}else if(sender.SelectedSegment == 1){

			}else if(sender.SelectedSegment == 2){

			}else{
				return;
			}
		}
		partial void sgLineParamTapped (UIKit.UISegmentedControl sender)
		{
			if(sender.SelectedSegment == 0)
			{
				txtMaxNumber.Enabled = true;
				txtMaxPercentage.Enabled = false;

			}else if(sender.SelectedSegment == 1){
				txtMaxNumber.Enabled = false;
				txtMaxPercentage.Enabled = true;

			}else{
				return;
			}
		}
		partial void btnCancelTapped (Foundation.NSObject sender)
		{
			NSNotificationCenter.DefaultCenter.PostNotificationName("dismissPopover",null);
		}
		partial void btnAMPMTimeTapped (UIKit.UIButton sender)
		{
			sender.Selected = true;
			defineNotif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ChangeDefineTimeText"),handleChangeDefineTimeText);

			PopoverViewController popoverContent = new PopoverViewController ();
			popoverContent.PopType = "timePad";
			popoverContent.SubPopType = "Define";
			if(sender==btnAMPush)
				popoverContent.timeValue = txtFirstAMsPushAfter.Text;
			else if(sender==btnAmArrive)
				popoverContent.timeValue = txtArriveBeforeFirst.Text;
			else if(sender==btnPMPush)
				popoverContent.timeValue = txtPMsPushAfter.Text;
			else if(sender==btnPMArrive)
				popoverContent.timeValue = txtArriveBeforeSecond.Text;
			else if(sender==btnNTEPush)
				popoverContent.timeValue = txtNTEPushAfter.Text;
			else if(sender==btnNTEArrive)
				popoverContent.timeValue = txtArriveBeforeThird.Text;
			popoverController = new UIPopoverController (popoverContent);
			popoverController.Delegate = new MyPopDelegate (this);
			popoverController.PopoverContentSize = new CGSize (200, 200);
			popoverController.PresentFromRect(sender.Frame,this.View,UIPopoverArrowDirection.Any,true);

		}
		public void handleChangeDefineTimeText (NSNotification n)
		{
			if (btnAMPush.Selected)
				txtFirstAMsPushAfter.Text = n.Object.ToString ();
			else if (btnAmArrive.Selected)
				txtArriveBeforeFirst.Text = n.Object.ToString ();
			else if (btnPMPush.Selected)
				txtPMsPushAfter.Text = n.Object.ToString ();
			else if (btnPMArrive.Selected)
				txtArriveBeforeSecond.Text = n.Object.ToString ();
			else if (btnNTEPush.Selected)
				txtNTEPushAfter.Text = n.Object.ToString ();
			else if (btnNTEArrive.Selected)
				txtArriveBeforeThird.Text = n.Object.ToString ();
		}
		partial void btnMaxInputTapped (UIKit.UIButton sender)
		{
			sender.Selected = true;
			defineNotif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ChangeMaxInputText"),handleChangeMaxInputText);
			PopoverViewController popoverContent = new PopoverViewController ();
			popoverContent.PopType = "numberPad";
			popoverContent.SubPopType = "Define";
			if(sender==btnMaxNumber)
				popoverContent.numValue = txtMaxNumber.Text;
			else if(sender==btnMaxPercentage)
				popoverContent.numValue = txtMaxPercentage.Text;
			popoverController = new UIPopoverController (popoverContent);
			popoverController.Delegate = new MyPopDelegate (this);
			popoverController.PopoverContentSize = new CGSize (210, 300);
			popoverController.PresentFromRect(sender.Frame,this.View,UIPopoverArrowDirection.Any,true);

		}
		public void handleChangeMaxInputText (NSNotification n)
		{
			if (btnMaxNumber.Selected)
				txtMaxNumber.Text = n.Object.ToString ();
			else if (btnMaxPercentage.Selected)
				txtMaxPercentage.Text = n.Object.ToString ();

			btnMaxNumber.Selected = false;
			btnMaxPercentage.Selected = false;
			NSNotificationCenter.DefaultCenter.RemoveObserver (defineNotif);
			popoverController.Dismiss (true);
		}
		partial void btnOkayTapped (Foundation.NSObject sender)
		{
            AmPmConfigure AmPmConfigure = GlobalSettings.WBidINIContent.AmPmConfigure;
            TimeSpan timeSpan;
            if (TimeSpan.TryParse(txtFirstAMsPushAfter.Text, out timeSpan))
            {
                AmPmConfigure.AmPush = timeSpan;
            }

            if (TimeSpan.TryParse(txtPMsPushAfter.Text, out timeSpan))
            {
                AmPmConfigure.PmPush = timeSpan;
            }
			if (TimeSpan.TryParse(txtNTEPushAfter.Text, out timeSpan))
            {
                AmPmConfigure.NitePush = timeSpan;
            }

            if (TimeSpan.TryParse(txtArriveBeforeFirst.Text, out timeSpan))
            {
                AmPmConfigure.AmLand = timeSpan;
            }

            if (TimeSpan.TryParse(txtArriveBeforeSecond.Text, out timeSpan))
            {
                AmPmConfigure.PmLand = timeSpan;
            }

            if (TimeSpan.TryParse(txtArriveBeforeThird.Text, out timeSpan))
            {
                AmPmConfigure.NiteLand = timeSpan;
            }
			AmPmConfigure.HowCalcAmPm = (int)sgAMPM.SelectedSegment + 1;
			AmPmConfigure.NumberOrPercentageCalc = (int)sgLineParameter.SelectedSegment + 1;
            AmPmConfigure.NumOpposites = Convert.ToInt32(txtMaxNumber.Text);
            AmPmConfigure.PctOpposities = Convert.ToInt32(txtMaxPercentage.Text);

            WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());

            RecalculateAndSaveLineFile();

			NSNotificationCenter.DefaultCenter.PostNotificationName("dismissPopover",null);
		}

        private void RecalculateAndSaveLineFile()
        {
			Dictionary<string, Line> lines = new Dictionary<string, Line>();

			RecalcalculateLineProperties calculateLineProperties = new RecalcalculateLineProperties();
			foreach (Line line in GlobalSettings.Lines)
			{
				calculateLineProperties.RecalculateAMPMPropertiesAfterAMPMDefenitionChanges(line);
				lines.Add(line.LineNum.ToString(), line);
			}
			
            LineInfo lineInfo = new LineInfo()
            {
                LineVersion = GlobalSettings.LineVersion,
                Lines = lines

            };
			
			//var fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails();
   //         var linestream = File.Create(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBL");
   //         ProtoSerailizer.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBL", lineInfo, linestream);
   //         linestream.Dispose();
   //         linestream.Close();
        }

        //private Dictionary<string, Line> RecalculateAMPMProperties()
        //{

        //    Dictionary<string, Line> lines = new Dictionary<string, Line>();
        //    CalculateLineProperties calculateLineProperties = new CalculateLineProperties();
        //    foreach (Line line in GlobalSettings.Lines)
        //    {
        //        line.AMPM = calculateLineProperties.CalcAmPmProp(line,true);
        //        line.AMPMSortOrder = calculateLineProperties.CalcAmPmSortOrder(line);
        //        lines.Add(line.LineNum.ToString(), line);
        //    }
        //    return lines;
        //}

	}
  
}

