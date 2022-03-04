using System;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Core.Enum;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.SharedLibrary.Utility;

namespace WBid.WBidiPad.iOS
{
    public partial class CommutableManualBlockSortViewController : UIViewController
    {
        #region Properties

        public object CommutableLine { get; set; }

        public CommuteFromView CommuteType { get; set; }

        #endregion

        #region Member variables

        public string DisplayMode;
        public bool isInDomNumeric;
        NSIndexPath path;
        NSObject commLineManualTimeNotif;
        UIPopoverController popoverController;
        WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
        static WBidIntialState wbidIntialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
        ConstraintCalculations constCalc = new ConstraintCalculations();
        WeightCalculation weightCalc = new WeightCalculation();
        NSObject arrObserver;

        #endregion

        #region Nested Class

        class MyPopDelegate : UIPopoverControllerDelegate
        {
            CommutableManualBlockSortViewController _parent;
            public MyPopDelegate(CommutableManualBlockSortViewController parent)
            {
                _parent = parent;
            }
            public override void DidDismiss(UIPopoverController popoverController)
            {
                _parent.popoverController = null;
                if (_parent.commLineManualTimeNotif != null)
                {
                    NSNotificationCenter.DefaultCenter.RemoveObserver(_parent.commLineManualTimeNotif);
                    _parent.commLineManualTimeNotif = null;
                }
                else
                {
                    
                }
                _parent.btnMonThuDep.Selected = false;
                _parent.btnMonThuArr.Selected = false;
                _parent.btnFriDep.Selected = false;
                _parent.btnFriArr.Selected = false;
                _parent.btnSatDep.Selected = false;
                _parent.btnSatArr.Selected = false;
                _parent.btnSunDep.Selected = false;
                _parent.btnSunArr.Selected = false;

            }

            public override bool ShouldDismiss(UIPopoverController popoverController)
            {
                return true;
            }


        }

        #endregion



        public CommutableManualBlockSortViewController(IntPtr handle): base(handle)
        {

        }

        public CommutableManualBlockSortViewController() : base("CommutableManualBlockSortViewController", null)
        {
        }

        private void SetTimeValue()
        {
            wBIdStateContent.Constraints.CL.MondayThu.Checkin = Helper.ConvertHHMMtoMinute(btnMonThuDep.TitleLabel.Text);
            wBIdStateContent.Constraints.CL.MondayThu.BackToBase = Helper.ConvertHHMMtoMinute(btnMonThuArr.TitleLabel.Text);
            wBIdStateContent.Constraints.CL.Friday.Checkin = Helper.ConvertHHMMtoMinute(btnFriDep.TitleLabel.Text);
            wBIdStateContent.Constraints.CL.Friday.BackToBase = Helper.ConvertHHMMtoMinute(btnFriArr.TitleLabel.Text);
            wBIdStateContent.Constraints.CL.Saturday.Checkin = Helper.ConvertHHMMtoMinute(btnSatDep.TitleLabel.Text);
            wBIdStateContent.Constraints.CL.Saturday.BackToBase = Helper.ConvertHHMMtoMinute(btnSatArr.TitleLabel.Text);
            wBIdStateContent.Constraints.CL.Sunday.Checkin = Helper.ConvertHHMMtoMinute(btnSunDep.TitleLabel.Text);
            wBIdStateContent.Constraints.CL.Sunday.BackToBase = Helper.ConvertHHMMtoMinute(btnSunArr.TitleLabel.Text);

            wBIdStateContent.Weights.CL.TimesList[0].Checkin = Helper.ConvertHHMMtoMinute(btnMonThuDep.TitleLabel.Text);
            wBIdStateContent.Weights.CL.TimesList[0].BackToBase = Helper.ConvertHHMMtoMinute(btnMonThuArr.TitleLabel.Text);
            wBIdStateContent.Weights.CL.TimesList[1].Checkin = Helper.ConvertHHMMtoMinute(btnFriDep.TitleLabel.Text);
            wBIdStateContent.Weights.CL.TimesList[1].BackToBase = Helper.ConvertHHMMtoMinute(btnFriArr.TitleLabel.Text);
            wBIdStateContent.Weights.CL.TimesList[2].Checkin = Helper.ConvertHHMMtoMinute(btnSatDep.TitleLabel.Text);
            wBIdStateContent.Weights.CL.TimesList[2].BackToBase = Helper.ConvertHHMMtoMinute(btnSatArr.TitleLabel.Text);
            wBIdStateContent.Weights.CL.TimesList[3].Checkin = Helper.ConvertHHMMtoMinute(btnSunDep.TitleLabel.Text);
            wBIdStateContent.Weights.CL.TimesList[3].BackToBase = Helper.ConvertHHMMtoMinute(btnSunArr.TitleLabel.Text);
        }

        public void handleWeightChange(NSNotification n)
        {
           // NSNotificationCenter.DefaultCenter.RemoveObserver(commLineWeightNotif);
            //popoverController.Dismiss(true);
        }

        partial void btnAnyNightBothEndsTapped(UIButton sender)
        {
            WBidHelper.PushToUndoStack();
            if (sender.Tag == 0)
            {
                btnAnyNight.Selected = true;
                btnBothEnds.Selected = false;
            }
            else
            {
                btnAnyNight.Selected = false;
                btnBothEnds.Selected = true;
            }
        }

        partial void btnBothEndsWtTapped(UIButton sender)
        {
           
            string supPopType = WeightsApplied.MainList[path.Section];
            PopoverViewController popoverContent = new PopoverViewController();
            popoverContent.PopType = "changeWeightParamInCommutableLine";
            popoverContent.SubPopType = supPopType;
            popoverContent.index = 0;
            popoverContent.numValue = sender.TitleLabel.Text;
            popoverController = new UIPopoverController(popoverContent);
            popoverController.Delegate = new MyPopDelegate(this);
            popoverController.PopoverContentSize = new CGSize(210, 300);
        }


        partial void btnLoadDefaultsTapped(UIButton sender)
        {
            wbidIntialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
            btnLoadDefaults.Enabled = false;
            btnSaveDefaults.Enabled = false;

            //setting properties to defauly value
            btnMonThuDep.SetTitle(Helper.ConvertMinuteToHHMM(wbidIntialState.Weights.CL.DefaultTimes[0].Checkin), UIControlState.Normal);
            btnMonThuDep.SetTitle(Helper.ConvertMinuteToHHMM(wbidIntialState.Weights.CL.DefaultTimes[0].Checkin), UIControlState.Selected);
            btnMonThuArr.SetTitle(Helper.ConvertMinuteToHHMM(wbidIntialState.Weights.CL.DefaultTimes[0].BackToBase), UIControlState.Normal);
            btnMonThuArr.SetTitle(Helper.ConvertMinuteToHHMM(wbidIntialState.Weights.CL.DefaultTimes[0].BackToBase), UIControlState.Selected);
            btnFriDep.SetTitle(Helper.ConvertMinuteToHHMM(wbidIntialState.Weights.CL.DefaultTimes[1].Checkin), UIControlState.Normal);
            btnFriDep.SetTitle(Helper.ConvertMinuteToHHMM(wbidIntialState.Weights.CL.DefaultTimes[1].Checkin), UIControlState.Selected);
            btnFriArr.SetTitle(Helper.ConvertMinuteToHHMM(wbidIntialState.Weights.CL.DefaultTimes[1].BackToBase), UIControlState.Normal);
            btnFriArr.SetTitle(Helper.ConvertMinuteToHHMM(wbidIntialState.Weights.CL.DefaultTimes[1].BackToBase), UIControlState.Selected);
            btnSatDep.SetTitle(Helper.ConvertMinuteToHHMM(wbidIntialState.Weights.CL.DefaultTimes[2].Checkin), UIControlState.Normal);
            btnSatDep.SetTitle(Helper.ConvertMinuteToHHMM(wbidIntialState.Weights.CL.DefaultTimes[2].Checkin), UIControlState.Selected);
            btnSatArr.SetTitle(Helper.ConvertMinuteToHHMM(wbidIntialState.Weights.CL.DefaultTimes[2].BackToBase), UIControlState.Normal);
            btnSatArr.SetTitle(Helper.ConvertMinuteToHHMM(wbidIntialState.Weights.CL.DefaultTimes[2].BackToBase), UIControlState.Selected);
            btnSunDep.SetTitle(Helper.ConvertMinuteToHHMM(wbidIntialState.Weights.CL.DefaultTimes[3].Checkin), UIControlState.Normal);
            btnSunDep.SetTitle(Helper.ConvertMinuteToHHMM(wbidIntialState.Weights.CL.DefaultTimes[3].Checkin), UIControlState.Selected);
            btnSunArr.SetTitle(Helper.ConvertMinuteToHHMM(wbidIntialState.Weights.CL.DefaultTimes[3].BackToBase), UIControlState.Normal);
            btnSunArr.SetTitle(Helper.ConvertMinuteToHHMM(wbidIntialState.Weights.CL.DefaultTimes[3].BackToBase), UIControlState.Selected);
        }

        partial void btnSaveDefaultsTapped(UIButton sender)
        {
            btnLoadDefaults.Enabled = true;
            btnSaveDefaults.Enabled = false;

            wbidIntialState.Weights.CL.DefaultTimes[0].Checkin = Helper.ConvertHHMMtoMinute(btnMonThuDep.TitleLabel.Text);
            wbidIntialState.Weights.CL.DefaultTimes[1].Checkin = Helper.ConvertHHMMtoMinute(btnFriDep.TitleLabel.Text);
            wbidIntialState.Weights.CL.DefaultTimes[2].Checkin = Helper.ConvertHHMMtoMinute(btnSatDep.TitleLabel.Text);
            wbidIntialState.Weights.CL.DefaultTimes[3].Checkin = Helper.ConvertHHMMtoMinute(btnSunDep.TitleLabel.Text);

            wbidIntialState.Weights.CL.DefaultTimes[0].BackToBase = Helper.ConvertHHMMtoMinute(btnMonThuArr.TitleLabel.Text);
            wbidIntialState.Weights.CL.DefaultTimes[1].BackToBase = Helper.ConvertHHMMtoMinute(btnFriArr.TitleLabel.Text);
            wbidIntialState.Weights.CL.DefaultTimes[2].BackToBase = Helper.ConvertHHMMtoMinute(btnSatArr.TitleLabel.Text);
            wbidIntialState.Weights.CL.DefaultTimes[3].BackToBase = Helper.ConvertHHMMtoMinute(btnSunArr.TitleLabel.Text);

            XmlHelper.SerializeToXml(wbidIntialState, WBidHelper.GetWBidDWCFilePath());
            try
            {
                wbidIntialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
            }
            catch (Exception ex)
            {
                wbidIntialState = WBidCollection.CreateDWCFile(GlobalSettings.DwcVersion);
                XmlHelper.SerializeToXml(wbidIntialState, WBidHelper.GetWBidDWCFilePath());
                WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
                obgWBidLogEvent.LogAllEvents(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "dwcRecreate", "0", "0","");

            }
            //if defaults saved,then disable load defaults button.
            btnLoadDefaults.Enabled = false;
        }

        partial void btnTimePopoverTapped(UIButton sender)
        {
            UIPopoverController popoverController;

            sender.Selected = true;



            for (int i = 11; i <= 18; i++)
            {

                UIButton button = (UIButton)this.View.ViewWithTag(i);


                  if (sender.Tag == i)
                  {
                 button.Selected = true;
                  }
                 else
                  {
                  button.Selected = false;
                 }
            }



            //if (commLineManualTimeNotif == null)
            //{
            //    commLineManualTimeNotif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ChangeTimeText"), handleChangeTimeTextBlockSort);
            //}
            PopoverViewController popoverContent = new PopoverViewController();
                popoverContent.PopType = "timePad";
                popoverContent.timeValue = sender.TitleLabel.Text;
                popoverController = new UIPopoverController(popoverContent);
              //  popoverController.Delegate = new MyPopDelegate(this);
                popoverController.PopoverContentSize = new CGSize(200, 200);
                popoverController.PresentFromRect(sender.Frame, this.View, UIPopoverArrowDirection.Any, true);
           
        }
        partial void commuteOptionsTapped(UIButton sender)
        {
            WBidHelper.PushToUndoStack();

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
            }

            if (wBIdStateContent.CxWtState.CL.Wt)
            {
                // if weight already set we need to recalculate weights
                weightCalc.ApplyCommutableLine(wBIdStateContent.Weights.CL);
            }
            GlobalSettings.isModified = true;
            CommonClass.cswVC.UpdateSaveButton();

        }

        private void changeInputStates()
        {
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

        partial void btnDoneButtonClicked(NSObject sender)
        {
            try
            {
                arrObserver = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("dismissCommutableLineBlockSortPopover"), dismissPopover);
                // set time list for the commutable line manual. Needs to set it for both constraints and weights
                SetTimeValue();


                // set other button values from the UI
                wBIdStateContent.Constraints.CL.AnyNight = btnAnyNight.Selected;
                wBIdStateContent.Constraints.CL.RunBoth = btnBothEnds.Selected;
                wBIdStateContent.Constraints.CL.CommuteToHome = btnCommHome.Selected;
                wBIdStateContent.Constraints.CL.CommuteToWork = btnCommWork.Selected;
                if (btnCommHome.Selected && btnCommWork.Selected)
                    wBIdStateContent.Weights.CL.Type = 1;
                else if(btnCommHome.Selected)
                    wBIdStateContent.Weights.CL.Type = 2;
                else if (btnCommWork.Selected)
                    wBIdStateContent.Weights.CL.Type = 3;
                else
                    wBIdStateContent.Weights.CL.Type = 0;


                //apply constrains if it already set
                if (wBIdStateContent.CxWtState.CL.Cx)
                {
                    constCalc.ApplyCommutableLinesConstraint(wBIdStateContent.Constraints.CL);
                    NSNotificationCenter.DefaultCenter.PostNotificationName("LineCountReload", null);
                }
                //apply weight if it already set
                if (wBIdStateContent.CxWtState.CL.Wt)
                {
                    weightCalc.ApplyCommutableLine(wBIdStateContent.Weights.CL);
                }
                //if there is no constrain and weights , we have to calculate the commutable line properties
                if (wBIdStateContent.CxWtState.CL.Cx==false && wBIdStateContent.CxWtState.CL.Wt==false)
                {
                    CalculateCommutabaleLineProperties(wBIdStateContent.Constraints.CL);
                }
                if (!((wBIdStateContent.SortDetails.BlokSort.Contains("36")) || (wBIdStateContent.SortDetails.BlokSort.Contains("37")) || (wBIdStateContent.SortDetails.BlokSort.Contains("38"))))
                {

                    wBIdStateContent.SortDetails.BlokSort.Add("36");
                }

                
                NSNotificationCenter.DefaultCenter.PostNotificationName("reloadBlockSort", null);

                NSNotificationCenter.DefaultCenter.PostNotificationName("dismissCommutableLineBlockSortPopover", null);


                if (wBIdStateContent.CxWtState.CL.Cx == true) {
                    
                    NSNotificationCenter.DefaultCenter.PostNotificationName("AddConstraints", null);
                }

             



            }
            catch(Exception e)
            {
                throw e;
            }
        }


        private void dismissPopover(NSNotification n)
        {
           
            if (commLineManualTimeNotif != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(commLineManualTimeNotif);
                commLineManualTimeNotif = null;
            }

            NSNotificationCenter.DefaultCenter.RemoveObserver(arrObserver);
            this.DismissViewController(true, null);
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.


            if (commLineManualTimeNotif == null)
            {
                commLineManualTimeNotif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ChangeTimeText"), handleChangeTimeTextBlockSort);
            }


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
            if (wBIdStateContent.Constraints.CL.CommuteToHome)
            {
                btnCommHome.Selected = true;
            }
            if (wBIdStateContent.Constraints.CL.CommuteToWork)
            {
                btnCommWork.Selected = true;
            }

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

            bool enableDefBtns = checkChangesInDefaultsValue();
            btnLoadDefaults.Enabled = enableDefBtns;
            btnSaveDefaults.Enabled = enableDefBtns;

        }
        /// <summary>
        /// retuurn true if any value chnages from the dafault state.
        /// </summary>
        /// <returns></returns>
        private bool checkChangesInDefaultsValue()
        {



            if ((wbidIntialState.Weights.CL.DefaultTimes[0].Checkin != Helper.ConvertHHMMtoMinute(btnMonThuDep.TitleLabel.Text)) || (wbidIntialState.Weights.CL.DefaultTimes[0].BackToBase != Helper.ConvertHHMMtoMinute(btnMonThuArr.TitleLabel.Text)))
            {
                return true;
            }
            if ((wbidIntialState.Weights.CL.DefaultTimes[1].Checkin != Helper.ConvertHHMMtoMinute(btnFriDep.TitleLabel.Text)) || (wbidIntialState.Weights.CL.DefaultTimes[1].BackToBase != Helper.ConvertHHMMtoMinute(btnFriArr.TitleLabel.Text)))
            {
                return true;
            }
            if ((wbidIntialState.Weights.CL.DefaultTimes[2].Checkin != Helper.ConvertHHMMtoMinute(btnSatDep.TitleLabel.Text)) || (wbidIntialState.Weights.CL.DefaultTimes[2].BackToBase != Helper.ConvertHHMMtoMinute(btnSatArr.TitleLabel.Text)))
            {
                return true;
            }
            if ((wbidIntialState.Weights.CL.DefaultTimes[3].Checkin != Helper.ConvertHHMMtoMinute(btnSunDep.TitleLabel.Text)) || (wbidIntialState.Weights.CL.DefaultTimes[3].BackToBase != Helper.ConvertHHMMtoMinute(btnSunArr.TitleLabel.Text)))
            {
                return true;
            }

            return false;

        }
        partial void btnRemoveTapped(UIButton sender)
        {
            if (commLineManualTimeNotif != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(commLineManualTimeNotif);
                commLineManualTimeNotif = null;
            }

            this.DismissViewController(true, null);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        /// <summary>
        /// Calculates the commutabale line properties.
        /// </summary>
        /// <param name="cxCommutableLine">Cx commutable line.</param>
        private void CalculateCommutabaleLineProperties(CxCommutableLine cxCommutableLine)
        {
            var _lines = GlobalSettings.Lines;
            var requiredLines = _lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.CommutableBacks = 0;
                line.commutableFronts = 0;
                line.CommutabilityFront = 0;
                line.CommutabilityBack = 0;
                line.CommutabilityOverall = 0;
                line.TotalCommutes = 0;

                bool isCommuteFrontEnd = false;
                bool isCommuteBackEnd = false;

                foreach (WorkBlockDetails workBlock in line.WorkBlockList)
                {

                    int checkIntime = 0;
                    int backToBaseTime = 0;

                    switch (workBlock.StartDay)
                    {
                        //Monday--Thurs
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                            checkIntime = cxCommutableLine.MondayThu.Checkin;
                            backToBaseTime = cxCommutableLine.MondayThu.BackToBase;
                            break;
                        //Friday
                        case 5:
                            checkIntime = cxCommutableLine.Friday.Checkin;
                            backToBaseTime = cxCommutableLine.Friday.BackToBase;
                            break;
                        // saturday
                        case 6:
                            checkIntime = cxCommutableLine.Saturday.Checkin;
                            backToBaseTime = cxCommutableLine.Saturday.BackToBase;
                            break;
                        //sunday
                        case 0:
                            checkIntime = cxCommutableLine.Sunday.Checkin;
                            backToBaseTime = cxCommutableLine.Sunday.BackToBase;
                            break;

                    }
                    isCommuteFrontEnd = checkIntime <= workBlock.StartTime; ;
                    if (isCommuteFrontEnd)
                    {
                        line.commutableFronts++;
                    }

                    switch (workBlock.EndDay)
                    {
                        //Monday--Thurs
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                            checkIntime = cxCommutableLine.MondayThu.Checkin;
                            backToBaseTime = cxCommutableLine.MondayThu.BackToBase;
                            backToBaseTime += (checkIntime > backToBaseTime) ? 1440 : 0;
                            break;
                        //Friday
                        case 5:
                            checkIntime = cxCommutableLine.Friday.Checkin;
                            backToBaseTime = cxCommutableLine.Friday.BackToBase;
                            backToBaseTime += (checkIntime > backToBaseTime) ? 1440 : 0;
                            break;
                        // saturday
                        case 6:
                            checkIntime = cxCommutableLine.Saturday.Checkin;
                            backToBaseTime = cxCommutableLine.Saturday.BackToBase;
                            backToBaseTime += (checkIntime > backToBaseTime) ? 1440 : 0;
                            break;
                        //sunday
                        case 0:
                            checkIntime = cxCommutableLine.Sunday.Checkin;
                            backToBaseTime = cxCommutableLine.Sunday.BackToBase;
                            backToBaseTime += (checkIntime > backToBaseTime) ? 1440 : 0;
                            break;

                    }

                    isCommuteBackEnd = backToBaseTime >= workBlock.EndTime;
                    if (isCommuteBackEnd)
                    {
                        line.CommutableBacks++;
                    }

                    line.NightsInMid += workBlock.nightINDomicile;


                }
                line.TotalCommutes = line.WorkBlockList.Count;

                if (line.TotalCommutes > 0)
                {
                    line.CommutabilityFront = Math.Round((line.commutableFronts / line.TotalCommutes) * 100, 2);
                    line.CommutabilityBack = Math.Round((line.CommutableBacks / line.TotalCommutes) * 100, 2);
                    line.CommutabilityOverall = Math.Round((line.commutableFronts + line.CommutableBacks) / (2 * line.TotalCommutes) * 100, 2);
                }

            }
        }
        public void handleChangeTimeTextBlockSort(NSNotification n)
        {



            //enabling the save default and load default on any change in time
            btnLoadDefaults.Enabled = true;
            btnSaveDefaults.Enabled = true;


            if (btnMonThuDep.Selected == true)
            {
                btnMonThuDep.SetTitle(n.Object.ToString(), UIControlState.Normal);
                btnMonThuDep.SetTitle(n.Object.ToString(), UIControlState.Selected);
            }
            else if (btnMonThuArr.Selected == true)
            {
                btnMonThuArr.SetTitle(n.Object.ToString(), UIControlState.Normal);
                btnMonThuArr.SetTitle(n.Object.ToString(), UIControlState.Selected);
            }
            else if (btnFriDep.Selected == true)
            {
                btnFriDep.SetTitle(n.Object.ToString(), UIControlState.Normal);
                btnFriDep.SetTitle(n.Object.ToString(), UIControlState.Selected);
            }
            else if (btnFriArr.Selected == true)
            {
                btnFriArr.SetTitle(n.Object.ToString(), UIControlState.Normal);
                btnFriArr.SetTitle(n.Object.ToString(), UIControlState.Selected);
            }
            else if (btnSatDep.Selected == true)
            {
                btnSatDep.SetTitle(n.Object.ToString(), UIControlState.Normal);
                btnSatDep.SetTitle(n.Object.ToString(), UIControlState.Selected);
            }
            else if (btnSatArr.Selected == true)
            {
                btnSatArr.SetTitle(n.Object.ToString(), UIControlState.Normal);
                btnSatArr.SetTitle(n.Object.ToString(), UIControlState.Selected);
            }
            else if (btnSunDep.Selected == true)
            {
                btnSunDep.SetTitle(n.Object.ToString(), UIControlState.Normal);
                btnSunDep.SetTitle(n.Object.ToString(), UIControlState.Selected);
            }
            else if (btnSunArr.Selected == true)
            {
                btnSunArr.SetTitle(n.Object.ToString(), UIControlState.Normal);
                btnSunArr.SetTitle(n.Object.ToString(), UIControlState.Selected);
            }
            NSNotificationCenter.DefaultCenter.PostNotificationName("LineCountReload", null);


           //btnMonThuDep.Selected = false;
           //btnMonThuArr.Selected = false;
           //btnFriDep.Selected = false;
           //btnFriArr.Selected = false;
           //btnSatDep.Selected = false;
           //btnSatArr.Selected = false;
           //btnSunDep.Selected = false;
           //btnSunArr.Selected = false;


        }
    }
}

