using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Model.SWA;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.SharedLibrary.SWA;
using WBidDataDownloadAuthorizationService.Model;
using System.Text.RegularExpressions;
using System.ServiceModel;
using System.Collections.Generic;
using WBid.WBidiPad.Model;
using System.Linq;
using System.IO;
using WBid.WBidiPad.PortableLibrary;
using System.Collections.ObjectModel;

namespace WBid.WBidiPad.iOS
{
    public partial class bidEditorPrepViewController : UIViewController
    {
		NSObject notif;
        private bool _isStartwithCurrentLineChecked;
        private bool _isAvodenceBidChecked;
        String domicileName;
        public BidPrep BidPrepDetails { get; set; }
        public ObservableCollection<BidPeriod> BidPeriods;
        public bidEditorPrepViewController()
            : base("bidEditorPrepViewController", null)
        {
        }
        partial void sgPositionTapped(Foundation.NSObject sender)
        {
            UISegmentedControl sg = (UISegmentedControl)sender;

            //			string round = (GlobalSettings.CurrentBidDetails.Round == "M") ? "D" : "B";
            //
            SetUIForSgSelection();
            //updateUIForSgSelection();

        }

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);


			foreach (UIView view in this.View.Subviews) {

				//DisposeClass.DisposeEx(view);
			}



		}
        partial void sgBidRoundTapped(Foundation.NSObject sender)
        {


            SetUIForSgSelection();
            //updateUIForSgSelection();

        }

        //        private void updateUIForSgSelection()
        //        {
        //            if (sgBidRound.SelectedSegment == 0)
        //            {
        //                btnAvoidanceBid.Enabled = true;
        //            }
        //            else
        //            {
        //                btnAvoidanceBid.Enabled = false;
        //            }
        //
        //            if (sgPosition.SelectedSegment == 0)
        //            {
        //                lblAvoidanceText.Text = "Avoidance Bids not allowed for Captains";
        //                btnAvoidanceBid.Enabled = false;
        //            }
        //            else if (sgPosition.SelectedSegment == 1)
        //            {
        //                lblAvoidanceText.Text = "Clear Avoidance Bids for this bid";
        //            }
        //            else
        //            {
        //                lblAvoidanceText.Text = "Clear Buddy Bids for this bid";
        //            }
        //            int originalValue;
        //            string pos = GlobalSettings.CurrentBidDetails.Postion;
        //            if (pos == "CP")
        //                originalValue = 0;
        //            else if (pos == "FO")
        //                originalValue = 1;
        //            else
        //                originalValue = 2;
        //
        //
        //            if (sgPosition.SelectedSegment == originalValue)
        //            {
        //                txtLineRangeParamOne.Text = "1";
        //                txtLineRangeParamTwo.Text = GlobalSettings.Lines.Count.ToString();
        //                txtLineRangeParamOne.Enabled = false;
        //                txtLineRangeParamTwo.Enabled = false;
        //                btnStartWithCurrentLine.Enabled = true;
        //            }
        //            else
        //            {
        //                txtLineRangeParamOne.Text = "1";
        //                txtLineRangeParamTwo.Text = "750";
        //                txtLineRangeParamOne.Enabled = true;
        //                txtLineRangeParamTwo.Enabled = true;
        //
        //                btnStartWithCurrentLine.Enabled = false;
        //            }
        //
        //
        //
        //        }

        public void SetUIForSgSelection()
        {

            string position = string.Empty;
            string round = string.Empty;



            //Captain
            if (sgPosition.SelectedSegment == 0)
            {
                lblAvoidanceText.Text = "Avoidance Bids not allowed for Captains";
                btnAvoidanceBid.Enabled = false;
                btnAvoidance.Enabled = false;
                btnAvoidance.SetTitle("Avoidance Bid", UIControlState.Normal);
                position = "CP";
            }
            //First Officer
            else if (sgPosition.SelectedSegment == 1)
            {
                lblAvoidanceText.Text = "Clear Avoidance Bids for this bid";
                btnAvoidanceBid.Enabled = (sgBidRound.SelectedSegment == 0);
                btnAvoidance.SetTitle("Avoidance Bid", UIControlState.Normal);
                btnAvoidance.Enabled = (sgBidRound.SelectedSegment == 0);
                position = "FO";
            }
            //Flight Attendant
            else
            {
                lblAvoidanceText.Text = "Clear Buddy Bids for this bid";
                btnAvoidanceBid.Enabled = (sgBidRound.SelectedSegment == 0);
                btnAvoidance.SetTitle("Buddy Bid", UIControlState.Normal);
                btnAvoidance.Enabled = (sgBidRound.SelectedSegment == 0);
                position = "FA";
            }


            round = (sgBidRound.SelectedSegment == 0) ? "M" : "S";




			if (position == GlobalSettings.CurrentBidDetails.Postion && round == GlobalSettings.CurrentBidDetails.Round && domicileName == GlobalSettings.CurrentBidDetails.Domicile && BidPeriods[(int)sgBidPeriod.SelectedSegment].BidPeriodId == GlobalSettings.CurrentBidDetails.Month)
            {
                var startline = GlobalSettings.Lines.Min(x => x.LineNum);
                txtLineRangeParamOne.Text = startline.ToString();
                txtLineRangeParamTwo.Text =(startline+ GlobalSettings.Lines.Count).ToString();
                txtLineRangeParamOne.Enabled = false;
                txtLineRangeParamTwo.Enabled = false;
                btnStartWithCurrentLine.Enabled = true;

            }
            else
            {
                txtLineRangeParamOne.Text = "1";
                txtLineRangeParamTwo.Text = "750";
                txtLineRangeParamOne.Enabled = true;
                txtLineRangeParamTwo.Enabled = true;

                btnStartWithCurrentLine.Enabled = false;
            }


        }

        partial void sgBidPeriodTapped(UIKit.UISegmentedControl sender)
        {
            SetUIForSgSelection();
        }

        partial void btnStartWithCurrentLineTapped(UIKit.UIButton sender)
        {
            sender.Selected = !sender.Selected;
            _isStartwithCurrentLineChecked = sender.Selected;
        }
        partial void btnOkTapped(UIKit.UIButton sender)
        {
			if(txtLineRangeParamOne.Enabled)
			{
				if(!RegXHandler.NumberValidation(txtLineRangeParamOne.Text))
				{
					if(!RegXHandler.EmployeeNumberValidation(txtLineRangeParamOne.Text))
					{
                        UIAlertController okAlertController = UIAlertController.Create("Error", "Invalid First parameter in range of lines to bid.", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);

                        return;
					}
				}
			}
			if(txtLineRangeParamTwo.Enabled)
			{
				if(!RegXHandler.NumberValidation(txtLineRangeParamTwo.Text))
				{
					if(!RegXHandler.EmployeeNumberValidation(txtLineRangeParamTwo.Text))
					{
                        UIAlertController okAlertController = UIAlertController.Create("Error", "Invalid second parameter in range of lines to bid.", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);
                        return;
					}
				}
			}


            try
            {
                BidPrepDetails = new BidPrep();
                BidPrepDetails.BidYear = GlobalSettings.CurrentBidDetails.Year;
				BidPrepDetails.BidPeriod = BidPeriods[(int)sgBidPeriod.SelectedSegment].BidPeriodId;
                if (sgPosition.SelectedSegment == 0)
                {
                    BidPrepDetails.Position = "CP";// SelectedPosition.LongStr;
                }
                else if (sgPosition.SelectedSegment == 1)
                {
                    BidPrepDetails.Position = "FO";// SelectedPosition.LongStr;
                }
                else if (sgPosition.SelectedSegment == 2)
                {
                    BidPrepDetails.Position = "FA";// SelectedPosition.LongStr;
                }
                BidPrepDetails.BidRound = (sgBidRound.SelectedSegment == 0) ? "D" : "B";// SelectedBidRound.ShortStr;
                BidPrepDetails.Domicile = domicileName;//SelectedDomicile.DomicileName;
                BidPrepDetails.IsChkAvoidanceBid = _isAvodenceBidChecked;
                
                BidPrepDetails.IsOnStartWithCurrentLine = _isStartwithCurrentLineChecked;
                BidPrepDetails.LineFrom = int.Parse(txtLineRangeParamOne.Text.Trim());
                BidPrepDetails.LineTo = int.Parse(txtLineRangeParamTwo.Text.Trim());//LineTo;
                GlobalSettings.BidPrepDetails = BidPrepDetails;

            }
            catch (Exception ex)
            {
                throw ex;


            }



              if (BidPrepDetails.Position == "FA" && GlobalSettings.CurrentBidDetails.Postion != "FA")
                {
                UIAlertController okAlertController = UIAlertController.Create("FA Bid Editor", "Invalid Bid Editor Initailization", UIAlertControllerStyle.Alert);                 okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));                 this.PresentViewController(okAlertController, true, null); 
                return;
            }

            this.DismissViewController(true, () =>
            {
                NSString nsPosition = new NSString(BidPrepDetails.Position);
                NSNotificationCenter.DefaultCenter.PostNotificationName("bidEditorPrepared", nsPosition);
            });
        }

		partial void btnChangeEmpTapped (UIKit.UIButton sender)
		{
			ChangeEmpNumberView changeEmp = new ChangeEmpNumberView();

            int SystemVersion = Convert.ToInt16(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]);

            if (SystemVersion > 12)
            {
                changeEmp.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            }
            else
            {
                changeEmp.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            }
            
			this.PresentViewController(changeEmp, true, null);
			//changeEmp.View.Superview.BackgroundColor = UIColor.Clear;
			//changeEmp.View.Frame = new RectangleF (0,130,540,200);
			//changeEmp.View.Layer.BorderWidth = 1;
//			notif = NSNotificationCenter.DefaultCenter.AddObserver("ChangeEmpNumber",(NSNotification obj) =>{
//				NSNotificationCenter.DefaultCenter.RemoveObserver(notif);
//			});

		}

//		void HandleAlertClicked(object sender, UIButtonEventArgs e)
//		{
//			if (e.ButtonIndex == 1)
//			{
//
//				
//				string empNumber = alert.GetTextField(0).Text.Trim();
//				if (empNumber != string.Empty)
//				{
//					GlobalSettings.TemporaryEmployeeNumber = empNumber;
//				}
//				// GlobalSettings.TemporaryEmployeeNumber = alert.Message;
//			}
//			else
//			{
//				//leave it.
//			}
//		}
        partial void btnCancelTapped(UIKit.UIButton sender)
        {
            this.DismissViewController(true, null);
        }
        partial void btnAvoidanceTapped(UIKit.UIButton sender)
        {
            if (sgPosition.SelectedSegment == 1)
            {

                AvoidanceBidVC avoidanceBidVC = new AvoidanceBidVC();
                avoidanceBidVC.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                this.PresentViewController(avoidanceBidVC, true, null);
                //avoidanceBidVC.View.Superview.BackgroundColor = UIColor.Clear;
                //avoidanceBidVC.View.Frame = new RectangleF(0, 100, 540, 350);
                //avoidanceBidVC.View.Layer.BorderWidth = 1;
            }
            else if (sgPosition.SelectedSegment == 2)
            {
                ChangeBuddyViewController BuddyView = new ChangeBuddyViewController();
                BuddyView.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                this.PresentViewController(BuddyView, true, null);
                //BuddyView.View.Superview.BackgroundColor = UIColor.Clear;
                //BuddyView.View.Frame = new RectangleF(0, 130, 540, 250);
                //BuddyView.View.Layer.BorderWidth = 1;
            }
        }
        partial void btnAvoidanceBidTapped(UIKit.UIButton sender)
        {
            sender.Selected = !sender.Selected;
            _isAvodenceBidChecked = sender.Selected;
        }
        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            pckrDomicileSelect.Model = new pickerViewModel(this);
            GlobalSettings.TemporaryEmployeeNumber = GlobalSettings.WbidUserContent.UserInformation.EmpNo;
            // Perform any additional setup after loading the view, typically from a nib.
			this.btnAvoidance.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnChangeEmp.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);

			txtLineRangeParamOne.EditingDidBegin+= (object sender, EventArgs e) => {
				scrlVW.SetContentOffset(new CGPoint(0,80),true);
			};
			txtLineRangeParamOne.EditingDidEnd += (object sender, EventArgs e) => {
				scrlVW.SetContentOffset(new CGPoint(0,0),true);
			};

			txtLineRangeParamTwo.EditingDidBegin+= (object sender, EventArgs e) => {
				scrlVW.SetContentOffset(new CGPoint(0,80),true);
			};
			txtLineRangeParamTwo.EditingDidEnd += (object sender, EventArgs e) => {
				scrlVW.SetContentOffset(new CGPoint(0,0),true);
			};

        }

        public override void ViewWillAppear(bool animated)
        {
            setupViews();

        }
        public void setupViews()
        {
            domicileName = GlobalSettings.WBidINIContent.Domiciles.ToList().FirstOrDefault(x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile).DomicileName;

            string[] doms = GlobalSettings.WBidINIContent.Domiciles.OrderBy(x => x.DomicileName).Select(y => y.DomicileName).ToArray();
            if (domicileName == null)
            {
                domicileName = doms[0];
                pckrDomicileSelect.Select(0, 0, true);
            }
            else
            {
                pckrDomicileSelect.Select(Array.IndexOf(doms, domicileName), 0, true);
            }

            List<int> lstMonth = new List<int>();
            int currentMonth = GlobalSettings.CurrentBidDetails.Month;
            lstMonth.Add((currentMonth - 1 == 0) ? 12 : (currentMonth - 1));
            lstMonth.Add(currentMonth);
            lstMonth.Add((currentMonth + 1 == 13) ? 1 : (currentMonth + 1));

            //			BidPeriods = new ObservableCollection<BidPeriod>(WBidCollection.GetBidPeriods().Where(x => lstMonth.Any(y => y == x.BidPeriodId)));

            BidPeriods = new ObservableCollection<BidPeriod>();
            foreach (int monthId in lstMonth)
            {
                BidPeriod bidPerid = WBidCollection.GetBidPeriods().FirstOrDefault(x => x.BidPeriodId == monthId);
                if (bidPerid != null)
                {
                    BidPeriods.Add(bidPerid);
                }
            }

            //Load appropraite values to sgBidPeriod
            string[] arr = { BidPeriods[0].Period, BidPeriods[1].Period, BidPeriods[2].Period };//Replace with appropriate array.
            for (int i = 0; i < 3; i++)
            {
                sgBidPeriod.SetTitle(arr[i], i);
            }

            sgBidPeriod.SelectedSegment = 1;//Always middle value;


            string pos = GlobalSettings.CurrentBidDetails.Postion;
            if (pos == "CP")
                sgPosition.SelectedSegment = 0;
            else if (pos == "FO")
                sgPosition.SelectedSegment = 1;
            else
                sgPosition.SelectedSegment = 2;

            string round = GlobalSettings.CurrentBidDetails.Round;
            if (round == "M")
                sgBidRound.SelectedSegment = 0;
            else
                sgBidRound.SelectedSegment = 1;





            SetUIForSgSelection();
            //updateUIForSgSelection ();

            //			string round = (GlobalSettings.CurrentBidDetails.Round == "M") ? "D" : "B";
            //
            //			if (GlobalSettings.CurrentBidDetails.Postion == "CP") {
            //				sgPosition.SelectedSegment = 0;
            //
            //				btnAvoidanceBid.Enabled = false;
            //				btnAvoidanceBid.Selected = false;
            //				lblAvoidanceText.Text = "Avoidance Bids not allowed for Captains";
            //			} else if (GlobalSettings.CurrentBidDetails.Postion == "FO") {
            //				sgPosition.SelectedSegment = 1;
            //
            //				if (round == "D") 
            //				btnAvoidanceBid.Enabled = true;
            //				btnAvoidanceBid.Selected = false;
            //				lblAvoidanceText.Text = "Clear Avoidance Bids for this bid";
            //			} else {
            //				sgPosition.SelectedSegment = 2;
            //				if (round == "D") 
            //					btnAvoidanceBid.Enabled = true;
            //				btnAvoidanceBid.Selected = false;
            //				lblAvoidanceText.Text = "Clear Buddy Bids for this bid";
            //				btnAvoidanceBid.SetTitle ("Buddy Bid", UIControlState.Normal);
            //
            //			}
            //
            //			if (round == "D") {
            //				sgBidRound.SelectedSegment = 0;//D
            //				txtLineRangeParamOne.Enabled = false;
            //				txtLineRangeParamTwo.Enabled = false;
            //				txtLineRangeParamOne.Text = "1";
            //				txtLineRangeParamTwo.Text = GlobalSettings.Lines.Count.ToString();
            //				btnStartWithCurrentLine.Enabled = true;
            //			} else {
            //				sgBidRound.SelectedSegment = 1;//B
            //				txtLineRangeParamOne.Enabled = false;
            //				txtLineRangeParamTwo.Enabled = false;
            //				txtLineRangeParamOne.Text = "1";
            //				txtLineRangeParamTwo.Text = "750";
            //				btnStartWithCurrentLine.Enabled = false;
            //
            //			}

        }

        public class pickerViewModel : UIPickerViewModel
        {
            bidEditorPrepViewController parent;
            public pickerViewModel(bidEditorPrepViewController parentVC)
            {
                parent = parentVC;
            }

            public override nint GetComponentCount(UIPickerView picker)
            {
                return 1;
            }

            public override nint GetRowsInComponent(UIPickerView picker, nint component)
            {
                string[] arr = GlobalSettings.WBidINIContent.Domiciles.OrderBy(x => x.DomicileName).Select(y => y.DomicileName).ToArray();
                return arr.Length;
            }

            public override string GetTitle(UIPickerView picker, nint row, nint component)
            {
                return GlobalSettings.WBidINIContent.Domiciles.OrderBy(x => x.DomicileName).Select(y => y.DomicileName).ToArray()[row];
            }
            public override void Selected(UIPickerView picker, nint row, nint component)
            {
                parent.domicileName = GlobalSettings.WBidINIContent.Domiciles.OrderBy(x => x.DomicileName).Select(y => y.DomicileName).ToArray()[row];
                parent.SetUIForSgSelection();
            }
        }
    }
}

