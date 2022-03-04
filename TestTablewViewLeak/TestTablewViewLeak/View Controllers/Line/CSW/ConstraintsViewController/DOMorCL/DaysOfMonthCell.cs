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
using WBid.WBidiPad.PortableLibrary;

namespace WBid.WBidiPad.iOS
{
	public partial class DaysOfMonthCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("DaysOfMonthCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("DaysOfMonthCell");
       static WBidIntialState wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
		WeightCalculation weightCalc = new WeightCalculation();

		class MyPopDelegate : UIPopoverControllerDelegate
		{
			DaysOfMonthCell _parent;
			public MyPopDelegate (DaysOfMonthCell parent)
			{
				_parent = parent;
			}
			public override void DidDismiss (UIPopoverController popoverController)
			{
				_parent.popoverController = null;
				if (_parent.commLineTimeNotif != null)
					NSNotificationCenter.DefaultCenter.RemoveObserver (_parent.commLineTimeNotif);
				_parent.btnMonThuDep.Selected = false;
				_parent.btnMonThuArr.Selected = false;
				_parent.btnFriDep.Selected = false;
				_parent.btnFriArr.Selected = false;
				_parent.btnSatDep.Selected = false;
				_parent.btnSatArr.Selected = false;
				_parent.btnSunDep.Selected = false;
				_parent.btnSunArr.Selected = false;

				if (_parent.DisplayMode == "Constraints") {
					WBidHelper.PushToUndoStack ();
					_parent.SetTimeValue ();
					_parent.constCalc.ApplyCommutableLinesConstraint (_parent.wBIdStateContent.Constraints.CL);
					NSNotificationCenter.DefaultCenter.PostNotificationName ("AddConstraints", null);
					NSNotificationCenter.DefaultCenter.PostNotificationName ("LineCountReload", null);
                    if (_parent.wBIdStateContent.CxWtState.CL.Wt == true)
                    {

                        NSNotificationCenter.DefaultCenter.PostNotificationName("AddWeights", null);
                    }

                } else {
					if(!_parent.isInDomNumeric)
						WBidHelper.PushToUndoStack ();
					_parent.SetTimeValue ();
					_parent.weightCalc.ApplyCommutableLine(_parent.wBIdStateContent.Weights.CL);
					NSNotificationCenter.DefaultCenter.PostNotificationName ("AddWeights", null);
					if (_parent.commLineWeightNotif != null)
						NSNotificationCenter.DefaultCenter.RemoveObserver (_parent.commLineWeightNotif);
					GlobalSettings.isModified = true;
					CommonClass.cswVC.UpdateSaveButton ();
					_parent.isInDomNumeric = false;

                    if (_parent.wBIdStateContent.CxWtState.CL.Cx == true)
                    {

                        NSNotificationCenter.DefaultCenter.PostNotificationName("AddConstraints", null);
                    }


                }


            }
		}

		public string DisplayMode;

		NSObject commLineTimeNotif;
		NSObject commLineWeightNotif;

		NSIndexPath path;
		WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		ConstraintCalculations constCalc = new ConstraintCalculations ();
		DaysOfMonthCollectionController calCollection;
		UIPopoverController popoverController;
		public bool isInDomNumeric;

		public DaysOfMonthCell (IntPtr handle) : base (handle)
		{
			if (this != null) {
			
			}
		}

		public static DaysOfMonthCell Create ()
		{
			return (DaysOfMonthCell)Nib.Instantiate (null, null) [0];
		}

        /// <summary>
        /// retuurn true if any value chnages from the dafault state.
        /// </summary>
        /// <returns></returns>
        private bool checkChangesInDefaultsValue()
        {

          

            if ((wbidintialState.Weights.CL.DefaultTimes[0].Checkin != Helper.ConvertHHMMtoMinute(btnMonThuDep.TitleLabel.Text)) || (wbidintialState.Weights.CL.DefaultTimes[0].BackToBase != Helper.ConvertHHMMtoMinute(btnMonThuArr.TitleLabel.Text)))
            {
                return true;
            }
            if ((wbidintialState.Weights.CL.DefaultTimes[1].Checkin != Helper.ConvertHHMMtoMinute(btnFriDep.TitleLabel.Text)) || (wbidintialState.Weights.CL.DefaultTimes[1].BackToBase != Helper.ConvertHHMMtoMinute(btnFriArr.TitleLabel.Text)))
            {
                return true;
            }
            if ((wbidintialState.Weights.CL.DefaultTimes[2].Checkin != Helper.ConvertHHMMtoMinute(btnSatDep.TitleLabel.Text)) || (wbidintialState.Weights.CL.DefaultTimes[2].BackToBase != Helper.ConvertHHMMtoMinute(btnSatArr.TitleLabel.Text)))
            {
                return true;
            }
            if ((wbidintialState.Weights.CL.DefaultTimes[3].Checkin != Helper.ConvertHHMMtoMinute(btnSunDep.TitleLabel.Text)) || (wbidintialState.Weights.CL.DefaultTimes[3].BackToBase != Helper.ConvertHHMMtoMinute(btnSunArr.TitleLabel.Text)))
            {
                return true;
            }

            return false;

        }
        public void RefreshConstraintsandWeightUI(WBidState wBIdStateContent)
        {
            
            this.vwChildWeight.Hidden = true;
            this.vwComLines.Hidden = false;
            //              
            this.btnCommHome.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            this.btnCommHome.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            this.btnCommWork.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            this.btnCommWork.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            this.btnAnyNight.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            this.btnAnyNight.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            this.btnBothEnds.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            this.btnBothEnds.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            this.btnLoadDefaults.SetBackgroundImage(UIImage.FromBundle("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            this.btnSaveDefaults.SetBackgroundImage(UIImage.FromBundle("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);

            this.btnMonThuArr.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            this.btnMonThuArr.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            this.btnMonThuDep.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            this.btnMonThuDep.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            this.btnFriDep.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            this.btnFriDep.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            this.btnFriArr.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            this.btnFriArr.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            this.btnSatDep.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            this.btnSatDep.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            this.btnSatArr.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            this.btnSatArr.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            this.btnSunDep.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            this.btnSunDep.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            this.btnSunArr.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            this.btnSunArr.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);

            if (wBIdStateContent.Constraints.CL.AnyNight)
            {
                btnAnyNight.Selected = true;
            }
            if (wBIdStateContent.Constraints.CL.RunBoth)
            {
                btnBothEnds.Selected = true;
            }
            btnCommHome.Selected = wBIdStateContent.Constraints.CL.CommuteToHome;
            btnCommWork.Selected = wBIdStateContent.Constraints.CL.CommuteToWork;


            changeInputStates();

            btnMonThuDep.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.MondayThu.Checkin), UIControlState.Normal);
            btnMonThuDep.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.MondayThu.Checkin), UIControlState.Selected);
            btnMonThuArr.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.MondayThu.BackToBase), UIControlState.Normal);
            btnMonThuArr.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.MondayThu.BackToBase), UIControlState.Selected);
            btnFriDep.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Friday.Checkin), UIControlState.Normal);
            btnFriDep.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Friday.Checkin), UIControlState.Selected);
            btnFriArr.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Friday.BackToBase), UIControlState.Normal);
            btnFriArr.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Friday.BackToBase), UIControlState.Selected);
            btnSatDep.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Saturday.Checkin), UIControlState.Normal);
            btnSatDep.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Saturday.Checkin), UIControlState.Selected);
            btnSatArr.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Saturday.BackToBase), UIControlState.Normal);
            btnSatArr.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Saturday.BackToBase), UIControlState.Selected);
            btnSunDep.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Sunday.Checkin), UIControlState.Normal);
            btnSunDep.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Sunday.Checkin), UIControlState.Selected);
            btnSunArr.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Sunday.BackToBase), UIControlState.Normal);
            btnSunArr.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Sunday.BackToBase), UIControlState.Selected);
        }
        public void bindData(NSIndexPath indexpath)
        {
            path = indexpath;
            this.vwDayofMonth.Hidden = true;
            this.vwComLines.Hidden = true;
           
            if (DisplayMode == "Constraints")
            {
                if (indexpath.Section == ConstraintsApplied.MainList.IndexOf("Days of the Month"))
                {
                    this.vwDayofMonth.Hidden = false;
                    this.btnWork.Hidden = true;
                    this.btnOff.Hidden = true;
                    this.vwLabel.Transform = CGAffineTransform.MakeRotation((float)(-90 * 3.14 / 180.0));
                    System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                    string strMonthName = mfi.GetMonthName(GlobalSettings.CurrentBidDetails.Month).ToString();
                    this.lblMonth.Text = strMonthName;

                    this.btnWork.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnWork.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    this.btnOff.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnOff.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);

                    if (calCollection != null)
                    {
                        calCollection.View.RemoveFromSuperview();
                        calCollection = null;
                    }
                    var layout = new UICollectionViewFlowLayout();
                    layout.SectionInset = new UIEdgeInsets(0, 0, 0, 0);
                    layout.MinimumInteritemSpacing = 0;
                    layout.MinimumLineSpacing = 0;
                    layout.ItemSize = new CGSize(57, 43);
					calCollection = new DaysOfMonthCollectionController(layout);
                    calCollection.View.Frame = this.vwDays.Bounds;
                    calCollection.DisplayMode = DisplayMode;
                    this.vwDays.AddSubview(calCollection.View);

                }
                else
                {
                    this.vwChildWeight.Hidden = true;
                    this.vwComLines.Hidden = false;
                    //				
                    this.btnCommHome.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnCommHome.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    this.btnCommWork.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnCommWork.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    this.btnAnyNight.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnAnyNight.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    this.btnBothEnds.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnBothEnds.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    this.btnLoadDefaults.SetBackgroundImage(UIImage.FromBundle("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnSaveDefaults.SetBackgroundImage(UIImage.FromBundle("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);

                    this.btnMonThuArr.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnMonThuArr.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    this.btnMonThuDep.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnMonThuDep.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    this.btnFriDep.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnFriDep.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    this.btnFriArr.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnFriArr.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    this.btnSatDep.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnSatDep.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    this.btnSatArr.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnSatArr.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    this.btnSunDep.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnSunDep.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    this.btnSunArr.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnSunArr.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);

                    if (wBIdStateContent.Constraints.CL.AnyNight)
                    {
                        btnAnyNight.Selected = true;
                    }
                    if (wBIdStateContent.Constraints.CL.RunBoth)
                    {
                        btnBothEnds.Selected = true;
                    }
                    btnCommHome.Selected = wBIdStateContent.Constraints.CL.CommuteToHome;
                    btnCommWork.Selected = wBIdStateContent.Constraints.CL.CommuteToWork;


					changeInputStates();

                    btnMonThuDep.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.MondayThu.Checkin), UIControlState.Normal);
                    btnMonThuDep.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.MondayThu.Checkin), UIControlState.Selected);
                    btnMonThuArr.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.MondayThu.BackToBase), UIControlState.Normal);
                    btnMonThuArr.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.MondayThu.BackToBase), UIControlState.Selected);
                    btnFriDep.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Friday.Checkin), UIControlState.Normal);
                    btnFriDep.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Friday.Checkin), UIControlState.Selected);
                    btnFriArr.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Friday.BackToBase), UIControlState.Normal);
                    btnFriArr.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Friday.BackToBase), UIControlState.Selected);
                    btnSatDep.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Saturday.Checkin), UIControlState.Normal);
                    btnSatDep.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Saturday.Checkin), UIControlState.Selected);
                    btnSatArr.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Saturday.BackToBase), UIControlState.Normal);
                    btnSatArr.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Saturday.BackToBase), UIControlState.Selected);
                    btnSunDep.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Sunday.Checkin), UIControlState.Normal);
                    btnSunDep.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Sunday.Checkin), UIControlState.Selected);
                    btnSunArr.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Sunday.BackToBase), UIControlState.Normal);
                    btnSunArr.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Sunday.BackToBase), UIControlState.Selected);
                }

            }
            else
            {
                if (indexpath.Section == WeightsApplied.MainList.IndexOf("Days of the Month"))
                {
                    this.vwDayofMonth.Hidden = false;
                    this.vwLabel.Transform = CGAffineTransform.MakeRotation((float)(-90 * 3.14 / 180.0));
                    System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                    string strMonthName = mfi.GetMonthName(GlobalSettings.CurrentBidDetails.Month).ToString();
                    this.lblMonth.Text = strMonthName;
                    this.btnWork.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnWork.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    this.btnOff.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnOff.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    if (calCollection != null)
                    {
                        calCollection.View.RemoveFromSuperview();
                        calCollection = null;
                    }
                    var layout = new UICollectionViewFlowLayout();
                    layout.SectionInset = new UIEdgeInsets(0, 0, 0, 0);
                    layout.MinimumInteritemSpacing = 0;
                    layout.MinimumLineSpacing = 0;
                    layout.ItemSize = new CGSize(57, 43);
                    calCollection = new DaysOfMonthCollectionController(layout);
                    calCollection.View.Frame = this.vwDays.Bounds;
                    calCollection.DisplayMode = DisplayMode;
                    this.vwDays.AddSubview(calCollection.View);
                    if (wBIdStateContent.Weights.SDO.isWork)
                    {
                        btnWork.Selected = true;
                        btnOff.Selected = false;
                    }
                    else
                    {
                        btnWork.Selected = false;
                        btnOff.Selected = true;
                    }
                }
                else
                {
                    this.btnLoadDefaults.Frame = new CGRect(30, 90, 100, 30);
                    this.btnSaveDefaults.Frame = new CGRect(30, 130, 100, 30);

                    this.vwComLines.Hidden = false;

                    this.btnCommHome.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnCommHome.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    this.btnCommWork.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnCommWork.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    this.btnAnyNight.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnAnyNight.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    this.btnBothEnds.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnBothEnds.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    this.btnLoadDefaults.SetBackgroundImage(UIImage.FromBundle("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnSaveDefaults.SetBackgroundImage(UIImage.FromBundle("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnMonThuArr.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnMonThuArr.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    this.btnMonThuDep.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnMonThuDep.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    this.btnFriDep.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnFriDep.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    this.btnFriArr.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnFriArr.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    this.btnSatDep.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnSatDep.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    this.btnSatArr.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnSatArr.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    this.btnSunDep.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnSunDep.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    this.btnSunArr.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnSunArr.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);

                    this.btnAnyNight.Hidden = true;
                    this.btnBothEnds.Hidden = true;
                    this.vwChildWeight.Hidden = false;
                    this.btnBothEndsWt.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnBothEndsWt.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                    this.btnInDomWt.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                    this.btnInDomWt.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);


                    btnMonThuDep.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[0].Checkin), UIControlState.Normal);
                    btnMonThuDep.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[0].Checkin), UIControlState.Selected);
                    btnMonThuArr.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[0].BackToBase), UIControlState.Normal);
                    btnMonThuArr.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[0].BackToBase), UIControlState.Selected);
                    btnFriDep.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[1].Checkin), UIControlState.Normal);
                    btnFriDep.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[1].Checkin), UIControlState.Selected);
                    btnFriArr.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[1].BackToBase), UIControlState.Normal);
                    btnFriArr.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[1].BackToBase), UIControlState.Selected);
                    btnSatDep.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[2].Checkin), UIControlState.Normal);
                    btnSatDep.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[2].Checkin), UIControlState.Selected);
                    btnSatArr.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[2].BackToBase), UIControlState.Normal);
                    btnSatArr.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[2].BackToBase), UIControlState.Selected);
                    btnSunDep.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[3].Checkin), UIControlState.Normal);
                    btnSunDep.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[3].Checkin), UIControlState.Selected);
                    btnSunArr.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[3].BackToBase), UIControlState.Normal);
                    btnSunArr.SetTitle(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[3].BackToBase), UIControlState.Selected);

                    btnBothEndsWt.SetTitle(wBIdStateContent.Weights.CL.BothEnds.ToString(), UIControlState.Normal);
                    btnInDomWt.SetTitle(wBIdStateContent.Weights.CL.InDomicile.ToString(), UIControlState.Normal);

                    btnCommHome.Selected = false;
                    btnCommWork.Selected = false;
                    if (wBIdStateContent.Weights.CL.Type == 1)
                    {
                        btnCommHome.Selected = true;
                        btnCommWork.Selected = true;
                    }
                    else if (wBIdStateContent.Weights.CL.Type == 2)
                    {
                        btnCommHome.Selected = true;
                    }

                    else if (wBIdStateContent.Weights.CL.Type == 3)
                    {
                        btnCommWork.Selected = true;
                    }

					changeInputStates();

                }
            }

			bool enableDefBtns = checkChangesInDefaultsValue ();
			btnLoadDefaults.Enabled = enableDefBtns;
			btnSaveDefaults.Enabled = enableDefBtns;
        }

		partial void btnWorkOffTapped (UIKit.UIButton sender)
		{
			if (DisplayMode == "Constraints") {

			} else {
				WBidHelper.PushToUndoStack ();

				if (sender.TitleLabel.Text == "Work") {
					btnWork.Selected = true;
					//buttonColor(btnWork);
					btnOff.Selected = false;
					//buttonColor(btnOff);
					wBIdStateContent.Weights.SDO.isWork = true;
				} else {
					btnOff.Selected = true;
					//buttonColor(btnOff);
					btnWork.Selected = false;
					//buttonColor(btnWork);
					wBIdStateContent.Weights.SDO.isWork = false;
				}
				weightCalc.ApplyDaysOfMonthWeight (wBIdStateContent.Weights.SDO);
				GlobalSettings.isModified = true;
				CommonClass.cswVC.UpdateSaveButton ();

			}
		}
		partial void btnRemoveTapped (UIKit.UIButton sender)
		{
			WBidHelper.PushToUndoStack ();
			if (DisplayMode == "Constraints") {
				if (path.Section == ConstraintsApplied.MainList.IndexOf ("Days of the Month")) {
					wBIdStateContent.CxWtState.SDO.Cx = false;
					wBIdStateContent.Constraints.DaysOfMonth.OFFDays.Clear();
					wBIdStateContent.Constraints.DaysOfMonth.WorkDays.Clear();
					constCalc.ApplyDaysOfMonthConstraint (wBIdStateContent.Constraints.DaysOfMonth);
				} else {
					wBIdStateContent.CxWtState.CL.Cx = false;
					//wBIdStateContent.Constraints.CL.CommuteToWork = true;
					//wBIdStateContent.Constraints.CL.CommuteToHome = true;
					constCalc.RemoveCommutableLinesConstraint();
				}
				NSNotificationCenter.DefaultCenter.PostNotificationName ("AddConstraints", null);
				NSNotificationCenter.DefaultCenter.PostNotificationName ("LineCountReload", null);

			} else {
				if (path.Section == WeightsApplied.MainList.IndexOf ("Days of the Month")) {
					wBIdStateContent.CxWtState.SDO.Wt = false;
					wBIdStateContent.Weights.SDO.Weights.Clear();
					wBIdStateContent.Weights.SDO.isWork = false;
					weightCalc.ApplyDaysOfMonthWeight (wBIdStateContent.Weights.SDO);
				} else {
					wBIdStateContent.CxWtState.CL.Wt = false;
					//wBIdStateContent.Weights.CL.Type = 1;
					weightCalc.RemoveCommutableLineWeight();
				}
				NSNotificationCenter.DefaultCenter.PostNotificationName ("AddWeights", null);

			}

		}

        partial void commuteOptionsTapped(UIKit.UIButton sender)
        {
            WBidHelper.PushToUndoStack();
            //if (DisplayMode == "Constraints")
            //{
            sender.Selected = !sender.Selected;
            if (sender == btnCommWork)
            {
                if (!btnCommWork.Selected && !btnCommHome.Selected)
                    btnCommHome.Selected = true;
            }
            else if (sender == btnCommHome)
            {
                if (!btnCommWork.Selected && !btnCommHome.Selected)
                    btnCommWork.Selected = true;
            }
            if (btnCommHome.Selected && btnCommWork.Selected)
                wBIdStateContent.Weights.CL.Type = 1;
            else if (btnCommHome.Selected)
                wBIdStateContent.Weights.CL.Type = 2;
            else if (btnCommWork.Selected)
                wBIdStateContent.Weights.CL.Type = 3;
            else
                wBIdStateContent.Weights.CL.Type = 0;

            wBIdStateContent.Constraints.CL.CommuteToHome = btnCommHome.Selected;
            wBIdStateContent.Constraints.CL.CommuteToWork = btnCommWork.Selected;
           
            changeInputStates();
            if (wBIdStateContent.CxWtState.CL.Cx)
            {
                //if constraints already set, we need to recalculate the constraints
                constCalc.ApplyCommutableLinesConstraint(wBIdStateContent.Constraints.CL);
                NSNotificationCenter.DefaultCenter.PostNotificationName("LineCountReload", null);

                NSNotificationCenter.DefaultCenter.PostNotificationName("AddConstraints", null);
            }

            if (wBIdStateContent.CxWtState.CL.Wt)
            {
                // if weight already set we need to recalculate weights
                weightCalc.ApplyCommutableLine(wBIdStateContent.Weights.CL);

                NSNotificationCenter.DefaultCenter.PostNotificationName("AddWeights", null);
            }

            GlobalSettings.isModified = true;
            CommonClass.cswVC.UpdateSaveButton();
            // }
        }
        private void changeInputStates()
        {
            if (DisplayMode == "Constraints")
            {
                if (btnCommHome.Selected && btnCommWork.Selected)
                    btnBothEnds.Enabled = true;
                else
                {
                    btnBothEnds.Enabled = false;
                    btnBothEnds.Selected = false;
                    wBIdStateContent.Constraints.CL.RunBoth = false;
                    btnAnyNight.Enabled = true;
                    btnAnyNight.Selected = true;
                    wBIdStateContent.Constraints.CL.AnyNight = true;
                }
            }
            else
            {
                if (btnCommHome.Selected && btnCommWork.Selected)
                    btnBothEndsWt.Enabled = true;
                else
                    btnBothEndsWt.Enabled = false;
            }
            if (btnCommWork.Selected)
            {
                btnMonThuDep.Enabled = true;
                btnFriDep.Enabled = true;
                btnSatDep.Enabled = true;
                btnSunDep.Enabled = true;
            }
            else
            {
                btnMonThuDep.Enabled = false;
                btnFriDep.Enabled = false;
                btnSatDep.Enabled = false;
                btnSunDep.Enabled = false;
            }

            if (btnCommHome.Selected)
            {
                btnMonThuArr.Enabled = true;
                btnFriArr.Enabled = true;
                btnSatArr.Enabled = true;
                btnSunArr.Enabled = true;
            }
            else
            {
                btnMonThuArr.Enabled = false;
                btnFriArr.Enabled = false;
                btnSatArr.Enabled = false;
                btnSunArr.Enabled = false;
            }
        }
		partial void btnHelpIconTapped (UIKit.UIButton sender)
		{
			if (DisplayMode == "Constraints") {
				HelpViewController helpVC = new HelpViewController ();
				helpVC.pdfFileName = "Constraints";
				if (path.Section == ConstraintsApplied.MainList.IndexOf ("Days of the Month"))
					helpVC.pdfOffset = ConstraintsApplied.HelpPageOffset ["Days of the Month"];
				else
					helpVC.pdfOffset = ConstraintsApplied.HelpPageOffset ["Commutable Lines"];
				UINavigationController navCont = new UINavigationController (helpVC);
				navCont.NavigationBar.BarStyle = UIBarStyle.Black;
				navCont.NavigationBar.Hidden = true;
                navCont.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                CommonClass.cswVC.PresentViewController (navCont, true, null);
			} else {
				HelpViewController helpVC = new HelpViewController ();
				helpVC.pdfFileName = "Weights";
				helpVC.selectRow = 1;
				if (path.Section == WeightsApplied.MainList.IndexOf ("Days of the Month"))
					helpVC.pdfOffset = WeightsApplied.HelpPageOffset ["Days of the Month"];
				else
					helpVC.pdfOffset = WeightsApplied.HelpPageOffset ["Commutable Lines"];
				UINavigationController navCont = new UINavigationController (helpVC);
				navCont.NavigationBar.BarStyle = UIBarStyle.Black;
				navCont.NavigationBar.Hidden = true;
                navCont.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                CommonClass.cswVC.PresentViewController (navCont, true, null);
			}
		}

		partial void btnAnyNightBothEndsTapped (UIKit.UIButton sender)
		{
			WBidHelper.PushToUndoStack ();
			if (DisplayMode == "Constraints") {
				if (sender.Tag == 0) {
					btnAnyNight.Selected = true;
					//buttonColor(btnAnyNight);
					btnBothEnds.Selected = false;
					//buttonColor(btnBothEnds);
				} else {
					btnAnyNight.Selected = false;
					//buttonColor(btnAnyNight);
					btnBothEnds.Selected = true;
					//buttonColor(btnBothEnds);
				}
				wBIdStateContent.Constraints.CL.AnyNight = btnAnyNight.Selected;
				wBIdStateContent.Constraints.CL.RunBoth = btnBothEnds.Selected;
				constCalc.ApplyCommutableLinesConstraint(wBIdStateContent.Constraints.CL);
				NSNotificationCenter.DefaultCenter.PostNotificationName ("LineCountReload", null);
			}
		}
		partial void btnLoadDefaultsTapped (UIKit.UIButton sender)
		{
            wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
			WBidHelper.PushToUndoStack ();
			btnLoadDefaults.Enabled = false;
			btnSaveDefaults.Enabled = false;

            btnMonThuDep.SetTitle(Helper.ConvertMinuteToHHMM(wbidintialState.Weights.CL.DefaultTimes[0].Checkin), UIControlState.Normal);
            btnMonThuDep.SetTitle(Helper.ConvertMinuteToHHMM(wbidintialState.Weights.CL.DefaultTimes[0].Checkin), UIControlState.Selected);
            btnMonThuArr.SetTitle(Helper.ConvertMinuteToHHMM(wbidintialState.Weights.CL.DefaultTimes[0].BackToBase), UIControlState.Normal);
            btnMonThuArr.SetTitle(Helper.ConvertMinuteToHHMM(wbidintialState.Weights.CL.DefaultTimes[0].BackToBase), UIControlState.Selected);
            btnFriDep.SetTitle(Helper.ConvertMinuteToHHMM(wbidintialState.Weights.CL.DefaultTimes[1].Checkin), UIControlState.Normal);
            btnFriDep.SetTitle(Helper.ConvertMinuteToHHMM(wbidintialState.Weights.CL.DefaultTimes[1].Checkin), UIControlState.Selected);
            btnFriArr.SetTitle(Helper.ConvertMinuteToHHMM(wbidintialState.Weights.CL.DefaultTimes[1].BackToBase), UIControlState.Normal);
            btnFriArr.SetTitle(Helper.ConvertMinuteToHHMM(wbidintialState.Weights.CL.DefaultTimes[1].BackToBase), UIControlState.Selected);
            btnSatDep.SetTitle(Helper.ConvertMinuteToHHMM(wbidintialState.Weights.CL.DefaultTimes[2].Checkin), UIControlState.Normal);
            btnSatDep.SetTitle(Helper.ConvertMinuteToHHMM(wbidintialState.Weights.CL.DefaultTimes[2].Checkin), UIControlState.Selected);
            btnSatArr.SetTitle(Helper.ConvertMinuteToHHMM(wbidintialState.Weights.CL.DefaultTimes[2].BackToBase), UIControlState.Normal);
            btnSatArr.SetTitle(Helper.ConvertMinuteToHHMM(wbidintialState.Weights.CL.DefaultTimes[2].BackToBase), UIControlState.Selected);
            btnSunDep.SetTitle(Helper.ConvertMinuteToHHMM(wbidintialState.Weights.CL.DefaultTimes[3].Checkin), UIControlState.Normal);
            btnSunDep.SetTitle(Helper.ConvertMinuteToHHMM(wbidintialState.Weights.CL.DefaultTimes[3].Checkin), UIControlState.Selected);
            btnSunArr.SetTitle(Helper.ConvertMinuteToHHMM(wbidintialState.Weights.CL.DefaultTimes[3].BackToBase), UIControlState.Normal);
            btnSunArr.SetTitle(Helper.ConvertMinuteToHHMM(wbidintialState.Weights.CL.DefaultTimes[3].BackToBase), UIControlState.Selected);

			
                

                wBIdStateContent.Constraints.CL.MondayThu.Checkin = wbidintialState.Weights.CL.DefaultTimes[0].Checkin;
                wBIdStateContent.Constraints.CL.Friday.Checkin = wbidintialState.Weights.CL.DefaultTimes[1].Checkin;
                wBIdStateContent.Constraints.CL.Saturday.Checkin = wbidintialState.Weights.CL.DefaultTimes[2].Checkin;
                wBIdStateContent.Constraints.CL.Sunday.Checkin = wbidintialState.Weights.CL.DefaultTimes[3].Checkin;
                wBIdStateContent.Constraints.CL.MondayThu.BackToBase = wbidintialState.Weights.CL.DefaultTimes[0].BackToBase;
                wBIdStateContent.Constraints.CL.Friday.BackToBase = wbidintialState.Weights.CL.DefaultTimes[1].BackToBase;
                wBIdStateContent.Constraints.CL.Saturday.BackToBase = wbidintialState.Weights.CL.DefaultTimes[2].BackToBase;
                wBIdStateContent.Constraints.CL.Sunday.BackToBase = wbidintialState.Weights.CL.DefaultTimes[3].BackToBase;

				constCalc.ApplyCommutableLinesConstraint(wBIdStateContent.Constraints.CL);
				NSNotificationCenter.DefaultCenter.PostNotificationName ("LineCountReload", null);

		
				wBIdStateContent.Weights.CL.TimesList[0].Checkin = wbidintialState.Weights.CL.DefaultTimes[0].Checkin;
				wBIdStateContent.Weights.CL.TimesList[1].Checkin = wbidintialState.Weights.CL.DefaultTimes[1].Checkin;
				wBIdStateContent.Weights.CL.TimesList[2].Checkin = wbidintialState.Weights.CL.DefaultTimes[2].Checkin;
				wBIdStateContent.Weights.CL.TimesList[3].Checkin = wbidintialState.Weights.CL.DefaultTimes[3].Checkin;
				wBIdStateContent.Weights.CL.TimesList[0].BackToBase = wbidintialState.Weights.CL.DefaultTimes[0].BackToBase;
				wBIdStateContent.Weights.CL.TimesList[1].BackToBase = wbidintialState.Weights.CL.DefaultTimes[1].BackToBase;
				wBIdStateContent.Weights.CL.TimesList[2].BackToBase = wbidintialState.Weights.CL.DefaultTimes[2].BackToBase;
				wBIdStateContent.Weights.CL.TimesList[3].BackToBase = wbidintialState.Weights.CL.DefaultTimes[3].BackToBase;


				weightCalc.ApplyCommutableLine(wBIdStateContent.Weights.CL);
				GlobalSettings.isModified = true;
				CommonClass.cswVC.UpdateSaveButton ();


            if (DisplayMode == "Constraints")
            {
                if (wBIdStateContent.CxWtState.CL.Wt)
                {
                    NSNotificationCenter.DefaultCenter.PostNotificationName("AddWeights", null);
                }
            }
            else
            {

                if (wBIdStateContent.CxWtState.CL.Cx)
                {
                    NSNotificationCenter.DefaultCenter.PostNotificationName("AddConstraints", null);
                }
            }

        }
		partial void btnSaveDefaultsTapped (UIKit.UIButton sender)
		{
			WBidHelper.PushToUndoStack ();
			btnLoadDefaults.Enabled = false;
			btnSaveDefaults.Enabled = false;

            wbidintialState.Weights.CL.DefaultTimes[0].Checkin = Helper.ConvertHHMMtoMinute(btnMonThuDep.TitleLabel.Text);
            wbidintialState.Weights.CL.DefaultTimes[1].Checkin = Helper.ConvertHHMMtoMinute(btnFriDep.TitleLabel.Text);
            wbidintialState.Weights.CL.DefaultTimes[2].Checkin = Helper.ConvertHHMMtoMinute(btnSatDep.TitleLabel.Text);
            wbidintialState.Weights.CL.DefaultTimes[3].Checkin = Helper.ConvertHHMMtoMinute(btnSunDep.TitleLabel.Text);

            wbidintialState.Weights.CL.DefaultTimes[0].BackToBase = Helper.ConvertHHMMtoMinute(btnMonThuArr.TitleLabel.Text);
            wbidintialState.Weights.CL.DefaultTimes[1].BackToBase = Helper.ConvertHHMMtoMinute(btnFriArr.TitleLabel.Text);
            wbidintialState.Weights.CL.DefaultTimes[2].BackToBase = Helper.ConvertHHMMtoMinute(btnSatArr.TitleLabel.Text);
            wbidintialState.Weights.CL.DefaultTimes[3].BackToBase = Helper.ConvertHHMMtoMinute(btnSunArr.TitleLabel.Text);



            XmlHelper.SerializeToXml(wbidintialState, WBidHelper.GetWBidDWCFilePath());
			try{
           wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
			}catch(Exception ex)
			{
				wbidintialState = WBidCollection.CreateDWCFile (GlobalSettings.DwcVersion);
				XmlHelper.SerializeToXml (wbidintialState, WBidHelper.GetWBidDWCFilePath ());
				//WBidHelper.LogDetails(GlobalSettings.WbidUserContent.UserInformation.EmpNo,"dwcRecreate","0","0");
				WBidLogEvent obgWBidLogEvent = new WBidLogEvent ();
				obgWBidLogEvent.LogAllEvents(GlobalSettings.WbidUserContent.UserInformation.EmpNo,"dwcRecreate","0","0","");

			}
            //if defaults saved,then disable load defaults button.
            btnLoadDefaults.Enabled = false;

			if (DisplayMode == "Constraints") {
				NSNotificationCenter.DefaultCenter.PostNotificationName ("AddWeights", null);
			} else {
				NSNotificationCenter.DefaultCenter.PostNotificationName ("AddConstraints", null);
				GlobalSettings.isModified = true;
				CommonClass.cswVC.UpdateSaveButton ();

			}

            if (DisplayMode == "Constraints")
            {
                if (wBIdStateContent.CxWtState.CL.Wt)
                {
                    NSNotificationCenter.DefaultCenter.PostNotificationName("AddWeights", null);
                }
            }
            else
            {

                if (wBIdStateContent.CxWtState.CL.Cx)
                {
                    NSNotificationCenter.DefaultCenter.PostNotificationName("AddConstraints", null);
                }
            }
        }
		partial void btnBothEndsWtTapped (UIKit.UIButton sender)
		{
			commLineWeightNotif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString("changeWeightParamInCommutableLine"), handleWeightChange);
			string supPopType = WeightsApplied.MainList [path.Section];
			PopoverViewController popoverContent = new PopoverViewController ();
			popoverContent.PopType = "changeWeightParamInCommutableLine";
			popoverContent.SubPopType = supPopType;
			popoverContent.index = 0;
			popoverContent.numValue = sender.TitleLabel.Text;
			popoverController = new UIPopoverController (popoverContent);
			popoverController.Delegate = new MyPopDelegate (this);
			popoverController.PopoverContentSize = new CGSize (210, 300);
			popoverController.PresentFromRect(sender.Frame,this.vwChildWeight,UIPopoverArrowDirection.Any,true);
		}
		partial void btnInDomWtTapped (UIKit.UIButton sender)
		{
			isInDomNumeric = true;
			commLineWeightNotif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString("changeWeightParamInCommutableLine"), handleWeightChange);
			string supPopType = WeightsApplied.MainList [path.Section];
			PopoverViewController popoverContent = new PopoverViewController ();
			popoverContent.PopType = "changeWeightParamInCommutableLine";
			popoverContent.SubPopType = supPopType;
			popoverContent.index = 1;
			popoverContent.numValue = sender.TitleLabel.Text;
			popoverController = new UIPopoverController (popoverContent);
			popoverController.Delegate = new MyPopDelegate (this);
			popoverController.PopoverContentSize = new CGSize (210, 300);
			popoverController.PresentFromRect(sender.Frame,this.vwChildWeight,UIPopoverArrowDirection.Any,true);
		}
		public void handleWeightChange (NSNotification n)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (commLineWeightNotif);
			popoverController.Dismiss (true);
		}
		partial void btnTimePopoverTapped (UIKit.UIButton sender)
		{
			if (DisplayMode == "Constraints") {
				sender.Selected = true;
				commLineTimeNotif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ChangeTimeText"),handleChangeTimeText);

				PopoverViewController popoverContent = new PopoverViewController ();
				popoverContent.PopType = "timePad";
				popoverContent.timeValue = sender.TitleLabel.Text;
				popoverController = new UIPopoverController (popoverContent);
				popoverController.Delegate = new MyPopDelegate (this);
				popoverController.PopoverContentSize = new CGSize (200, 200);
				popoverController.PresentFromRect(sender.Frame,this,UIPopoverArrowDirection.Any,true);

			} else {
				sender.Selected = true;
				commLineTimeNotif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ChangeTimeText"),handleChangeTimeText);

				PopoverViewController popoverContent = new PopoverViewController ();
				popoverContent.PopType = "timePad";
				popoverContent.timeValue = sender.TitleLabel.Text;
				popoverController = new UIPopoverController (popoverContent);
				popoverController.Delegate = new MyPopDelegate (this);
				popoverController.PopoverContentSize = new CGSize (200, 200);
				popoverController.PresentFromRect(sender.Frame,this,UIPopoverArrowDirection.Any,true);

			}

		}
		public void SetTimeValue ()
		{
			//if (DisplayMode == "Constraints") {
				wBIdStateContent.Constraints.CL.MondayThu.Checkin = Helper.ConvertHHMMtoMinute (btnMonThuDep.TitleLabel.Text);
				wBIdStateContent.Constraints.CL.MondayThu.BackToBase = Helper.ConvertHHMMtoMinute (btnMonThuArr.TitleLabel.Text);
				wBIdStateContent.Constraints.CL.Friday.Checkin = Helper.ConvertHHMMtoMinute (btnFriDep.TitleLabel.Text);
				wBIdStateContent.Constraints.CL.Friday.BackToBase = Helper.ConvertHHMMtoMinute (btnFriArr.TitleLabel.Text);
				wBIdStateContent.Constraints.CL.Saturday.Checkin = Helper.ConvertHHMMtoMinute (btnSatDep.TitleLabel.Text);
				wBIdStateContent.Constraints.CL.Saturday.BackToBase = Helper.ConvertHHMMtoMinute (btnSatArr.TitleLabel.Text);
				wBIdStateContent.Constraints.CL.Sunday.Checkin = Helper.ConvertHHMMtoMinute (btnSunDep.TitleLabel.Text);
				wBIdStateContent.Constraints.CL.Sunday.BackToBase = Helper.ConvertHHMMtoMinute (btnSunArr.TitleLabel.Text);
				NSNotificationCenter.DefaultCenter.PostNotificationName ("LineCountReload", null);
			//} else {
				wBIdStateContent.Weights.CL.TimesList[0].Checkin = Helper.ConvertHHMMtoMinute (btnMonThuDep.TitleLabel.Text);
				wBIdStateContent.Weights.CL.TimesList[0].BackToBase = Helper.ConvertHHMMtoMinute (btnMonThuArr.TitleLabel.Text);
				wBIdStateContent.Weights.CL.TimesList[1].Checkin = Helper.ConvertHHMMtoMinute (btnFriDep.TitleLabel.Text);
				wBIdStateContent.Weights.CL.TimesList[1].BackToBase = Helper.ConvertHHMMtoMinute (btnFriArr.TitleLabel.Text);
				wBIdStateContent.Weights.CL.TimesList[2].Checkin = Helper.ConvertHHMMtoMinute (btnSatDep.TitleLabel.Text);
				wBIdStateContent.Weights.CL.TimesList[2].BackToBase = Helper.ConvertHHMMtoMinute (btnSatArr.TitleLabel.Text);
				wBIdStateContent.Weights.CL.TimesList[3].Checkin = Helper.ConvertHHMMtoMinute (btnSunDep.TitleLabel.Text);
				wBIdStateContent.Weights.CL.TimesList[3].BackToBase = Helper.ConvertHHMMtoMinute (btnSunArr.TitleLabel.Text);
			//}
		}

		public void handleChangeTimeText (NSNotification n)
		{
			//if (DisplayMode == "Constraints") {
				if (btnMonThuDep.Selected) {
					btnMonThuDep.SetTitle (n.Object.ToString(), UIControlState.Normal);
					btnMonThuDep.SetTitle (n.Object.ToString(), UIControlState.Selected);
					//wBIdStateContent.Constraints.CL.MondayThu.Checkin = Helper.ConvertHHMMtoMinute (n.Object.ToString ());
				} else if (btnMonThuArr.Selected) {
					btnMonThuArr.SetTitle (n.Object.ToString(), UIControlState.Normal);
					btnMonThuArr.SetTitle (n.Object.ToString(), UIControlState.Selected);
					//wBIdStateContent.Constraints.CL.MondayThu.BackToBase = Helper.ConvertHHMMtoMinute (n.Object.ToString ());
				} else if (btnFriDep.Selected) {
					btnFriDep.SetTitle (n.Object.ToString(), UIControlState.Normal);
					btnFriDep.SetTitle (n.Object.ToString(), UIControlState.Selected);
					//wBIdStateContent.Constraints.CL.Friday.Checkin = Helper.ConvertHHMMtoMinute (n.Object.ToString ());
				} else if (btnFriArr.Selected) {
					btnFriArr.SetTitle (n.Object.ToString(), UIControlState.Normal);
					btnFriArr.SetTitle (n.Object.ToString(), UIControlState.Selected);
					//wBIdStateContent.Constraints.CL.Friday.BackToBase = Helper.ConvertHHMMtoMinute (n.Object.ToString ());
				} else if (btnSatDep.Selected) {
					btnSatDep.SetTitle (n.Object.ToString(), UIControlState.Normal);
					btnSatDep.SetTitle (n.Object.ToString(), UIControlState.Selected);
					//wBIdStateContent.Constraints.CL.Saturday.Checkin = Helper.ConvertHHMMtoMinute (n.Object.ToString ());
				} else if (btnSatArr.Selected) {
					btnSatArr.SetTitle (n.Object.ToString(), UIControlState.Normal);
					btnSatArr.SetTitle (n.Object.ToString(), UIControlState.Selected);
					//wBIdStateContent.Constraints.CL.Saturday.BackToBase = Helper.ConvertHHMMtoMinute (n.Object.ToString ());
				} else if (btnSunDep.Selected) {
					btnSunDep.SetTitle (n.Object.ToString(), UIControlState.Normal);
					btnSunDep.SetTitle (n.Object.ToString(), UIControlState.Selected);
					//wBIdStateContent.Constraints.CL.Sunday.Checkin = Helper.ConvertHHMMtoMinute (n.Object.ToString ());
				} else if (btnSunArr.Selected) {
					btnSunArr.SetTitle (n.Object.ToString(), UIControlState.Normal);
					btnSunArr.SetTitle (n.Object.ToString(), UIControlState.Selected);
					//wBIdStateContent.Constraints.CL.Sunday.BackToBase = Helper.ConvertHHMMtoMinute (n.Object.ToString ());
				}
				NSNotificationCenter.DefaultCenter.PostNotificationName ("LineCountReload", null);
			//} else {
				if (btnMonThuDep.Selected) {
					btnMonThuDep.SetTitle (n.Object.ToString (), UIControlState.Normal);
					btnMonThuDep.SetTitle (n.Object.ToString (), UIControlState.Selected);
					//wBIdStateContent.Weights.CL.TimesList[0].Checkin = Helper.ConvertHHMMtoMinute (n.Object.ToString ());
				} else if (btnMonThuArr.Selected) {
					btnMonThuArr.SetTitle (n.Object.ToString (), UIControlState.Normal);
					btnMonThuArr.SetTitle (n.Object.ToString (), UIControlState.Selected);
					//wBIdStateContent.Weights.CL.TimesList[0].BackToBase = Helper.ConvertHHMMtoMinute (n.Object.ToString ());
				} else if (btnFriDep.Selected) {
					btnFriDep.SetTitle (n.Object.ToString (), UIControlState.Normal);
					btnFriDep.SetTitle (n.Object.ToString (), UIControlState.Selected);
					//wBIdStateContent.Weights.CL.TimesList[1].Checkin = Helper.ConvertHHMMtoMinute (n.Object.ToString ());
				} else if (btnFriArr.Selected) {
					btnFriArr.SetTitle (n.Object.ToString (), UIControlState.Normal);
					btnFriArr.SetTitle (n.Object.ToString (), UIControlState.Selected);
					//wBIdStateContent.Weights.CL.TimesList[1].BackToBase = Helper.ConvertHHMMtoMinute (n.Object.ToString ());
				} else if (btnSatDep.Selected) {
					btnSatDep.SetTitle (n.Object.ToString (), UIControlState.Normal);
					btnSatDep.SetTitle (n.Object.ToString (), UIControlState.Selected);
					//wBIdStateContent.Weights.CL.TimesList[2].Checkin = Helper.ConvertHHMMtoMinute (n.Object.ToString ());
				} else if (btnSatArr.Selected) {
					btnSatArr.SetTitle (n.Object.ToString (), UIControlState.Normal);
					btnSatArr.SetTitle (n.Object.ToString (), UIControlState.Selected);
					//wBIdStateContent.Weights.CL.TimesList[2].BackToBase = Helper.ConvertHHMMtoMinute (n.Object.ToString ());
				} else if (btnSunDep.Selected) {
					btnSunDep.SetTitle (n.Object.ToString (), UIControlState.Normal);
					btnSunDep.SetTitle (n.Object.ToString (), UIControlState.Selected);
					//wBIdStateContent.Weights.CL.TimesList[3].Checkin = Helper.ConvertHHMMtoMinute (n.Object.ToString ());
				} else if (btnSunArr.Selected) {
					btnSunArr.SetTitle (n.Object.ToString (), UIControlState.Normal);
					btnSunArr.SetTitle (n.Object.ToString (), UIControlState.Selected);
					//wBIdStateContent.Weights.CL.TimesList[3].BackToBase = Helper.ConvertHHMMtoMinute (n.Object.ToString ());
				}
            //}


            //NSNotificationCenter.DefaultCenter.PostNotificationName("AddWeights", null);
            //NSNotificationCenter.DefaultCenter.PostNotificationName("reloadBlockSort", null);
            //NSNotificationCenter.DefaultCenter.PostNotificationName("AddConstraints", null);

        }
	}
}

