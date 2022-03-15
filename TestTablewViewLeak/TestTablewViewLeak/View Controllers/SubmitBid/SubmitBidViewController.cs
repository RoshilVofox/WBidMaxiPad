#region NameSpace
using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary.Utility;
using iOSPasswordStorage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.iOS.Utility;
using System.ServiceModel;
using WBidDataDownloadAuthorizationService.Model;
using WBid.WBidiPad.SharedLibrary.SWA;
using System.Text.RegularExpressions;
#endregion



namespace WBid.WBidiPad.iOS
{
    public partial class SubmitBidViewController : UIViewController
    {

        // private string _sessionCredentials = string.Empty;
        //  private static int _selectedSeniorityNumber = 0;

        //  WBidDataDwonloadAuthServiceClient client;
		public UIPopoverController popoverController;
        
		public MyPopDelegate objPopDelegate;
        LoadingOverlay loadingOverlay;
        NSObject notif;

        public SubmitBidViewController()
            : base("SubmitBidViewController", null)
        {

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
            this.NavigationItem.BackBarButtonItem = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null);

            var appearance = new UINavigationBarAppearance();
            appearance.ConfigureWithOpaqueBackground();
            appearance.BackgroundColor = ColorClass.TopHeaderColor;
            this.NavigationItem.StandardAppearance = appearance;
            this.NavigationItem.ScrollEdgeAppearance = this.NavigationItem.StandardAppearance;

            this.SetUpView();
			CommonClass.ObjSubmitView = this;
           
            //  BasicHttpBinding binding = ServiceUtils.CreateBasicHttp();
            // client = new WBidDataDwonloadAuthServiceClient(binding, ServiceUtils.EndPoint);
            //  client.GetAuthorizationDetailsCompleted += client_GetAuthorizationDetailsCompleted;
            //client.GetAuthorizationforMultiPlatformCompleted += client_GetAuthorizationforMultiPlatformCompleted;
            // Perform any additional setup after loading the view, typically from a nib.
            this.btnCancel.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            this.btnSubmitBid.SetBackgroundImage(UIImage.FromBundle("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);

            this.btnChangeEmp.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            this.btnChangeAvoidanceType.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            this.btnChangeBuddyBid.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);

        }

        //		public override void ViewDidAppear (bool animated)
        //		{
        //			base.ViewDidAppear (animated);
        //			this.Title = "Sumit Bid For " + GlobalSettings.TemporaryEmployeeNumber;
        //		}

        
        private void SetUpView()
        {


            GlobalSettings.TemporaryEmployeeNumber = (GlobalSettings.WbidUserContent != null && GlobalSettings.WbidUserContent.UserInformation != null) ? GlobalSettings.WbidUserContent.UserInformation.EmpNo : string.Empty;
            txtSeniorityNo.Text = GlobalSettings.WbidUserContent.UserInformation.SeniorityNumber.ToString();

            //pckrSeniorityNUmber.Model = new pickerViewModel();
            //vwChangeEmployeeNumber.Layer.BorderWidth = 1.0f;
            //vwChangeEmployeeNumber.Layer.BorderColor = UIColor.Black.CGColor;
            //vwEmployeeToBeAvoided.Layer.BorderColor = UIColor.Black.CGColor;
            //vwEmployeeToBeAvoided.Layer.BorderWidth = 1.0f;
            //vwViewOrAddBuddyBidders.Layer.BorderWidth = 1.0f;
            //vwViewOrAddBuddyBidders.Layer.BorderColor = UIColor.Black.CGColor;
            ////= GlobalSettings.WbidUserContent.UserInformation.SeniorityNumber;


            //            lblSubmitBidForId.Text = "Sumit Bid For " + GlobalSettings.TemporaryEmployeeNumber;
            this.Title = "Submit Bid For " + GlobalSettings.TemporaryEmployeeNumber;
            if (GlobalSettings.CurrentBidDetails.Postion == "CP")
            {
                //hide buddy bid, show avoidance, avoidance disabled
                vwBuddyBid.Hidden = true;
                vwAvoidanceBid.Hidden = false;
                //vwAvoidanceBid.UserInteractionEnabled = false;
                btnChangeAvoidanceType.Enabled = false;
                //vwAvoidanceBid.Alpha = 0.8f;
                this.setUpAvoidanceLabel(null);
            }
            else if (GlobalSettings.CurrentBidDetails.Postion == "FO")
            {
                //hide buddy bid, show avoidance
                vwBuddyBid.Hidden = true;
                vwAvoidanceBid.Hidden = false;
                //vwAvoidanceBid.UserInteractionEnabled = true;
                btnChangeAvoidanceType.Enabled = true;
                //vwAvoidanceBid.Alpha = 1.0f;

                this.setUpAvoidanceLabel(GetAvoidanceBid());
            }
            else
            {
                ////hide avoidance bid, hide buddy
                vwAvoidanceBid.Hidden = true;
                vwBuddyBid.Hidden = false;
                this.setUpBuddyBidLabel(GetBuddyBid());
            }
            txtSeniorityNo.Enabled = false;
            txtSeniorityNo.ShouldChangeCharacters = (textField, range, replacementString) =>
            {
                string text = textField.Text;
				string newText = text.Substring(0, (int)range.Location) + replacementString + text.Substring((int)range.Location + (int)range.Length);
                int val;
                if (newText == "")
                    return true;
                else
                    return Int32.TryParse(newText, out val);
            };
            if (GlobalSettings.CurrentBidDetails.Round == "S")
            {
                btnChangeBuddyBid.Enabled = false;
            }
        }

        private void setUpAvoidanceLabel(string str)
        {
            if (str == null)
            {
                lblNoAvoidanceBid.Text = "No Avoidance Bid";
                sgNoAvoidanceSubmissionType.Enabled = false;

            }
            else
            {

                if (str == string.Empty)
                {
                    sgNoAvoidanceSubmissionType.SelectedSegment = 1;
                    sgNoAvoidanceSubmissionType.Enabled = false;
                }
                else
                {
                    sgNoAvoidanceSubmissionType.SelectedSegment = 0;
                    sgNoAvoidanceSubmissionType.Enabled = true;

                }
                lblNoAvoidanceBid.Text = "Avoidance Bid : " + str;//TODO: XXX to be replaced by original value.
            }
        }
        //private void setUpBidChoiceLabel(string str)
        //{
        //    if (str == null)
        //    {
        //        lblSubmittingBidChoices.Text = "No Bid choices.";
        //    }
        //    else
        //    {
        //        lblSubmittingBidChoices.Text = "Submitting" + " " + str + " " + "Bid Choices for";
        //    }
        //}
        public void setUpBuddyBidLabel(string str)
        {
            if (str == string.Empty)
            {
                sgBuddyBidSubmissionType.SelectedSegment = 1;
                sgBuddyBidSubmissionType.Enabled = false;
            }
            else
            {
                sgBuddyBidSubmissionType.SelectedSegment = 0;
                sgBuddyBidSubmissionType.Enabled = true;

            }

            if (GlobalSettings.CurrentBidDetails.Round == "S")
                lblBuddyBid.Text = "No Buddy Bids";
            else
                lblBuddyBid.Text = "Buddy Bid : " + str;

        }


        partial void btnSubmitBidCancelButtonTapped(Foundation.NSObject sender)
        {
            this.DismissViewController(true, null);
			foreach (UIView view in this.View.Subviews) {

				DisposeClass.DisposeEx(view);
			}
        }

        partial void btnSubmitBidTapped(Foundation.NSObject sender)
        {
            try
            {

                if (!RegXHandler.NumberValidation(txtSeniorityNo.Text))
                {
                    if (!RegXHandler.EmployeeNumberValidation(txtSeniorityNo.Text))
                    {
                        UIAlertController okAlertController = UIAlertController.Create("Error", "Invalid Seniority Number.", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);
                        return;
                    }
                }


                GlobalSettings.SubmitBid = SetSubmitDetails();


              
                if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Round == "M")
                {
                    FAPostionChoiceViewController FAPosVC = new FAPostionChoiceViewController();
                    //FAPosVC.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;

                    this.NavigationController.PushViewController(FAPosVC, true);
                }
                else
                {
                    queryViewController qv = new queryViewController();
					qv.isFirstTime=true;
					qv.isFromView = queryViewController.queryFromView.querySubmitBid;
                    qv.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                    this.NavigationController.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                    this.NavigationController.PushViewController(qv, true);
                }


            }
            catch (Exception ex)
            {


                InvokeOnMainThread(() =>
            {

                throw ex;
            });
            }
            //if (GlobalSettings.CurrentBidDetails.Postion == "CP")
            //{
            //    ////hide buddy bid, show avoidance, avoidance disabled
            //    //vwBuddyBidChoice.Hidden = true;
            //    //vwAvoidanceBidChoice.Hidden = false;
            //    //vwAvoidanceBidChoice.UserInteractionEnabled = false;
            //    //vwAvoidanceBidChoice.Alpha = 0.8f;


            //}
            //else if (GlobalSettings.CurrentBidDetails.Postion == "FO")
            //{
            //    ////hide buddy bid, show avoidance
            //    //vwBuddyBidChoice.Hidden = true;
            //    //vwAvoidanceBidChoice.Hidden = false;
            //    //vwAvoidanceBidChoice.UserInteractionEnabled = true;
            //    //vwAvoidanceBidChoice.Alpha = 1.0f;

            //}
            //else if (GlobalSettings.CurrentBidDetails.Postion == "FA")
            //{
            //    //hide avoidance bid, show buddy
            //    //                vwAvoidanceBidChoice.Hidden = true;
            //    //                vwAvoidanceBidChoice.UserInteractionEnabled = false;
            //    //
            //    //                vwBuddyBidChoice.Hidden = false;


            //    // vwSubmittingBidChoicesFor.RemoveFromSuperview();
            //    FAPostionChoiceViewController FAPosVC = new FAPostionChoiceViewController();
            //    FAPosVC.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            //    //                this.PresentViewController(FAPosVC, true, null);
            //    this.NavigationController.PushViewController(FAPosVC, true);


            //}

            //int linesCount;
            //if (sgBidSubmissionType.SelectedSegment == 0)
            //{
            //    linesCount = GlobalSettings.Lines.Count();

            //}
            //else
            //{
            //    linesCount = _selectedSeniorityNumber;
            //}
            //setUpBidChoiceLabel(linesCount.ToString());

            //txtSubmittingBidChoicesId.Text = GlobalSettings.TemporaryEmployeeNumber;
        }
        //private void setupAvoidanceChoiceLabel(string str)
        //{
        //    if (str == null)
        //    {
        //        lblAvoidanceBidChoice.Text = "No Avoidance Bid";
        //        txtAvoidanceBidChoiceId.Text = "";
        //    }
        //    else
        //    {
        //        lblAvoidanceBidChoice.Text = "Avoidance Bid: ";
        //        txtAvoidanceBidChoiceId.Text = "XXX";//TODO: XXX to be replaced by original value.
        //    }
        //}
        //partial void btnChoicesCancelTapped(MonoTouch.UIKit.UIButton sender)
        //{
        //    vwSubmittingBidChoicesFor.RemoveFromSuperview();
        //}
        //partial void btnSubmitChoiceTapped(MonoTouch.UIKit.UIButton sender)
        //{



        //    loginViewController login = new loginViewController();
        //    login.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
        //    this.PresentViewController(login, true, () =>
        //    {
        //        NSNotificationCenter.DefaultCenter.AddObserver("loginDetailsEntered", loginCredetialsEntered);
        //    });

        //}
        //public void loginCredetialsEntered(NSNotification n)
        //{
        //    //			try{
        //    //				this.txtUserName.Text = KeychainHelpers.GetPasswordForUsername("user", "WBid.WBidiPad.cwa", false);
        //    //				this.txtPassword.Text = KeychainHelpers.GetPasswordForUsername("pass", "WBid.WBidiPad.cwa", false);
        //    //			}catch{
        //    //				Console.WriteLine ("Setting credentials execprion");
        //    //			}

        //    if (loadingOverlay == null)
        //    {
        //        loadingOverlay = new LoadingOverlay(this.View.Frame, "Authentication Checking..");
        //    }
        //    else
        //    {
        //        loadingOverlay.updateLoadingText("Authentication Checking..");
        //    }
        //    View.Add(loadingOverlay);
        //    this.PerformSelector(new MonoTouch.ObjCRuntime.Selector("AuthenticationCheck"), null, 0);


        //}

        partial void btnChangeEmpTapped(UIKit.UIButton sender)
        {
            ChangeEmpNumberView changeEmp = new ChangeEmpNumberView();
            changeEmp.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            UINavigationController nav = new UINavigationController(changeEmp);
            nav.NavigationBar.Hidden = true;
            nav.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            this.PresentViewController(nav, true, null);
            //changeEmp.View.Superview.BackgroundColor = UIColor.Clear;
            //changeEmp.View.Frame = new RectangleF(0, 130, 540, 200);
            //changeEmp.View.Layer.BorderWidth = 1;


            notif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ChangeEmpNumber"), (NSNotification obj) =>
            {
                this.Title = "Submit Bid For " + GlobalSettings.TemporaryEmployeeNumber;
                NSNotificationCenter.DefaultCenter.RemoveObserver(notif);
            });
           
        }

        //        partial void btnChangeEmployeeNumberTapped(MonoTouch.Foundation.NSObject sender)
        //        {
        //            //txtChangeEmployeeNumber.Text = GlobalSettings.TemporaryEmployeeNumber;
        //            //this.View.AddSubview(vwChangeEmployeeNumber);
        //
        //            //vwChangeEmployeeNumber.Center = this.View.Center;
        //

        //
        //        }

        //        void HandleAlertClicked(object sender, UIButtonEventArgs e)
        //        {
        //            UIAlertView alertt = (UIAlertView)sender;
        //            UITextField txtField = alertt.GetTextField(0);
        //            if (e.ButtonIndex == 1)
        //            {
        //                string empNumber = alertt.GetTextField(0).Text.Trim();
        //                if (empNumber != string.Empty)
        //                {
        //                    GlobalSettings.TemporaryEmployeeNumber = empNumber;
        ////                    lblSubmitBidForId.Text = "Sumit Bid For " + GlobalSettings.TemporaryEmployeeNumber;
        //					this.Title = "Sumit Bid For " + GlobalSettings.TemporaryEmployeeNumber;
        //                }
        //                // GlobalSettings.TemporaryEmployeeNumber = alert.Message;
        //            }
        //            else
        //            {
        //                //leave it.
        //
        //            }
        //            txtField.ResignFirstResponder();
        //            alertt.ResignFirstResponder();
        //        }

        //partial void btnEmployeeNumberOKTapped(MonoTouch.Foundation.NSObject sender)
        //{
        //    vwChangeEmployeeNumber.RemoveFromSuperview();
        //    GlobalSettings.TemporaryEmployeeNumber = txtChangeEmployeeNumber.Text;
        //    lblSubmitBidForId.Text = "Sumit Bid For " + GlobalSettings.TemporaryEmployeeNumber;
        //}
        //partial void btnEmployeeNumberCancelTapped(MonoTouch.Foundation.NSObject sender)
        //{
        //    vwChangeEmployeeNumber.RemoveFromSuperview();
        //}
        partial void btnChangeAvoidanceTapped(Foundation.NSObject sender)
        {

            AvoidanceBidVC avoidanceBidVC = new AvoidanceBidVC();
            avoidanceBidVC.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            this.PresentViewController(avoidanceBidVC, true, null);
            //avoidanceBidVC.View.Superview.BackgroundColor = UIColor.Clear;
            //avoidanceBidVC.View.Frame = new RectangleF(0, 100, 540, 350);
            //avoidanceBidVC.View.Layer.BorderWidth = 1;

			notif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("changeAvoidanceBids"), ChangeAvoidanceBidder);


            //txtEmployeeToBeAvoidedOne.Text = GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance1;
            //txtEmployeeToBeAvoidedTwo.Text = GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance2;
            //txtEmployeeToBeAvoidedThree.Text = GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance3;
            //this.View.AddSubview(vwEmployeeToBeAvoided);
            //vwEmployeeToBeAvoided.Center = this.View.Center;
        }

        public void ChangeAvoidanceBidder(NSNotification n)
        {
            this.setUpAvoidanceLabel(GetAvoidanceBid());
            NSNotificationCenter.DefaultCenter.RemoveObserver(notif);
        }
        //partial void btnAvodEmployeeOK(MonoTouch.Foundation.NSObject sender)
        //{
        //    //GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance1 = txtEmployeeToBeAvoidedOne.Text;
        //    //GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance2 = txtEmployeeToBeAvoidedTwo.Text;
        //    //GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance3 = txtEmployeeToBeAvoidedThree.Text;
        //    ////save the state of the INI File
        //    //WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
        //    // vwEmployeeToBeAvoided.RemoveFromSuperview();
        //    //this.setUpAvoidanceLabel(GetAvoidanceBid());
        //}
        partial void sgSubmitBidTypeChanged(UIKit.UISegmentedControl sender)
        {
            if (sender.SelectedSegment == 1)
            {
                // pckrSeniorityNUmber.UserInteractionEnabled = true;
                txtSeniorityNo.Enabled = true;
            }
            else
            {
                //pckrSeniorityNUmber.UserInteractionEnabled = false;
                txtSeniorityNo.Enabled = false;
            }
        }
        //partial void btnAvoidEmployeeCancel(MonoTouch.Foundation.NSObject sender)
        //{
        //    vwEmployeeToBeAvoided.RemoveFromSuperview();
        //}
        partial void btnChangeBuddyBidders(Foundation.NSObject sender)
        {

            //            txtBuddyBidderOne.Text = GlobalSettings.WBidINIContent.BuddyBids.Buddy1;
            //            txtBuddyBidderTwo.Text = GlobalSettings.WBidINIContent.BuddyBids.Buddy2;

            //            this.View.AddSubview(vwViewOrAddBuddyBidders);
            //            vwViewOrAddBuddyBidders.Center = this.View.Center;
            ChangeBuddyViewController BuddyView = new ChangeBuddyViewController();
            BuddyView.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            this.PresentViewController(BuddyView, true, null);
            //BuddyView.View.Superview.BackgroundColor = UIColor.Clear;
            //BuddyView.View.Frame = new RectangleF(0, 130, 540, 250);
            //BuddyView.View.Layer.BorderWidth = 1;
			notif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("addedBuddyBidder"), addedBuddyBidder);

        }
        public void addedBuddyBidder(NSNotification n)
        {
            this.setUpBuddyBidLabel(GetBuddyBid());
            NSNotificationCenter.DefaultCenter.RemoveObserver(notif);
        }

        //partial void btnChangeOrAddBuddyBidderOk(MonoTouch.Foundation.NSObject sender)
        //{
        //    GlobalSettings.WBidINIContent.BuddyBids.Buddy1 = txtBuddyBidderOne.Text;
        //    GlobalSettings.WBidINIContent.BuddyBids.Buddy2 = txtBuddyBidderTwo.Text;
        //    WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
        //    this.setUpBuddyBidLabel(GetBuddyBid());
        //    vwViewOrAddBuddyBidders.RemoveFromSuperview();
        //}
        //partial void btnChangeOrAddBuddyBidderCancel(MonoTouch.Foundation.NSObject sender)
        //{
        //    vwViewOrAddBuddyBidders.RemoveFromSuperview();
        //}

        //public class pickerViewModel : UIPickerViewModel
        //{
        //    public override int GetComponentCount(UIPickerView picker)
        //    {
        //        return 1;
        //    }

        //    public override int GetRowsInComponent(UIPickerView picker, int component)
        //    {
        //        return 5;
        //    }

        //    public override string GetTitle(UIPickerView picker, int row, int component)
        //    {
        //        return row.ToString();
        //    }
        //    public override void Selected(UIPickerView picker, int row, int component)
        //    {
        //        _selectedSeniorityNumber = row;
        //        //Here we get the selected number by accessing Row.
        //    }

        //}


        //private void client_GetAuthorizationforMultiPlatformCompleted(object sender, GetAuthorizationforMultiPlatformCompletedEventArgs e)
        //{
        //    if (e.Result != null)
        //    {
        //        ServiceResponseModel serviceResponseModel = e.Result;

        //        if (serviceResponseModel.IsAuthorized)
        //        {
        //            SubmitBid submitBid = null;
        //            InvokeOnMainThread(() =>
        //                {

        //                    submitBid = SetSubmitDetails();
        //                });
        //            SWASubmitBid swaSubmit = new SWASubmitBid();
        //            InvokeOnMainThread(() =>
        //            {
        //                loadingOverlay.updateLoadingText("Submitting Bid...");
        //            });

        //            swaSubmit.SubmitBid(submitBid, _sessionCredentials);
        //            InvokeOnMainThread(() =>
        //                             {
        //                                 loadingOverlay.RemoveFromSuperview();
        //                             });

        //            // NSNotificationCenter.DefaultCenter.PostNotificationName("authCheckSuccess", null);
        //            //this.startProgress();

        //            //Need to set empid,Password  and download file list.

        //            // _downloadFileDetails.SessionCredentials = _sessionCredentials;
        //            //  _downloadFileDetails.DownloadList = PortableLibrary.WBidCollection.GenarateDownloadFileslist(GlobalSettings.DownloadBidDetails);

        //        }
        //        else
        //        {
        //            InvokeOnMainThread(() =>
        //            {
        //               
        //            });
        //        }
        //    }
        //}

        #region Private Methods

        /// <summary>
        /// Get Avoidance Bid string
        /// </summary>
        /// <returns></returns>
        private string GetAvoidanceBid()
        {
            string avoidanceBidsStr = string.Empty;
            AvoidanceBids avoidancebids = GlobalSettings.WBidINIContent.AvoidanceBids;
            avoidanceBidsStr += (avoidancebids.Avoidance1 != "0") ? avoidancebids.Avoidance1.ToString() + "," : "";
            avoidanceBidsStr += (avoidancebids.Avoidance2 != "0") ? avoidancebids.Avoidance2.ToString() + "," : "";
            avoidanceBidsStr += (avoidancebids.Avoidance3 != "0") ? avoidancebids.Avoidance3.ToString() : "";
            avoidanceBidsStr = avoidanceBidsStr.TrimEnd(',');
            return avoidanceBidsStr;



        }

        /// <summary>
        /// Get Buddy Bid string
        /// </summary>
        /// <returns></returns>
        public string GetBuddyBid()
        {
            string buddyBidStr = string.Empty;
            BuddyBids buddyBids = GlobalSettings.WBidINIContent.BuddyBids;
            //disable buddy bid
            buddyBidStr += (buddyBids.Buddy1 != "0") ? buddyBids.Buddy1.ToString() + "," : "";
            buddyBidStr += (buddyBids.Buddy2 != "0") ? buddyBids.Buddy2.ToString() + "," : "";
            buddyBidStr = buddyBidStr.TrimEnd(',');
            return buddyBidStr;

        }

        /// <summary>
        /// Set submit Details
        /// </summary>
        /// <returns></returns>
        private SubmitBid SetSubmitDetails()
        {
			WBid.WBidiPad.Model.BidDetails bidDetails = GlobalSettings.CurrentBidDetails;
            SubmitBid submitBid = new SubmitBid();
            //set the properties required to POST the webrequest to SWA server.
            submitBid.Base = bidDetails.Domicile;
            submitBid.Bidder = GlobalSettings.TemporaryEmployeeNumber;
            submitBid.BidRound = (bidDetails.Round == "S") ? "Round 2" : "Round 1";
            submitBid.PacketId = GenaratePacketId(bidDetails);
            submitBid.Seat = bidDetails.Postion;
            submitBid.IsSubmitAllChoices = (sgBidSubmissionType.SelectedSegment == 0);
            //int aa = sgNoAvoidanceSubmissionType.SelectedSegment;
            if (bidDetails.Postion == "FO" && (sgBuddyBidSubmissionType.SelectedSegment == 0))
            {
                AvoidanceBids avoidanceBids = GlobalSettings.WBidINIContent.AvoidanceBids;
                submitBid.Pilot1 = (avoidanceBids.Avoidance1 == "0") ? null : avoidanceBids.Avoidance1;
                submitBid.Pilot2 = (avoidanceBids.Avoidance2 == "0") ? null : avoidanceBids.Avoidance2;
                submitBid.Pilot3 = (avoidanceBids.Avoidance3 == "0") ? null : avoidanceBids.Avoidance3;
            }

            if (bidDetails.Postion == "FA" && (sgBuddyBidSubmissionType.SelectedSegment == 0))
            {
                BuddyBids buddyBids = GlobalSettings.WBidINIContent.BuddyBids;
                //comment out this to disable buddy bid
                submitBid.Buddy1 = (buddyBids.Buddy1 == "0") ? null : buddyBids.Buddy1;
                submitBid.Buddy2 = (buddyBids.Buddy2 == "0") ? null : buddyBids.Buddy2;

            }
            int seniorityNumber = int.Parse((txtSeniorityNo.Text.Trim() == string.Empty) ? "0" : txtSeniorityNo.Text.Trim());
            if (submitBid.IsSubmitAllChoices)
            {
                submitBid.SeniorityNumber = GlobalSettings.Lines.Count();
                submitBid.TotalBidChoices = GlobalSettings.Lines.Count();
                //submitBid.Bid = string.Join(",", GlobalSettings.Lines.ToList().Select(x => x.LineNum));
            }
            else
            {
                submitBid.SeniorityNumber = seniorityNumber;
                submitBid.TotalBidChoices = seniorityNumber;
                //submitBid.Bid = string.Join(",", GlobalSettings.Lines.ToList().Take(seniorityNumber).Select(x => x.LineNum));

            }

            if (bidDetails.Postion == "FO" || bidDetails.Postion == "CP" ||(bidDetails.Postion=="FA" && bidDetails.Round=="S" ))
            {
                submitBid.Bid = string.Join(",", GlobalSettings.Lines.ToList().Take(submitBid.TotalBidChoices).Select(x => x.LineNum));
            }




            return submitBid;
        }

        /// <summary>
        /// Genarate Packet Id for Submit Bid Format:
        // Format: BASE || Year || Month || bid-round-number eg(Value=BWI2001032)
        /// </summary>
        /// <param name="bidDetails"></param>
        /// <returns></returns>
		private string GenaratePacketId(WBid.WBidiPad.Model.BidDetails bidDetails)
        {
            string packetid = string.Empty;
            packetid = bidDetails.Domicile + bidDetails.Year + bidDetails.Month.ToString("d2");

            //Set-round-numbers:
            //1 - F/A monthly bids
            //2 - F/A supplemental bids
            //3 - reserved
            //4 - Pilot monthly bids
            //5 - Pilot supplemental bids

            if (bidDetails.Round == "M" && bidDetails.Postion == "FA")
            {
                packetid += "1";
            }
            else if (bidDetails.Round == "S" && bidDetails.Postion == "FA")
            {
                packetid += "2";
            }
            else if (bidDetails.Round == "M" && (bidDetails.Postion == "FO" || bidDetails.Postion == "CP"))
            {
                packetid += "4";
            }
            else if (bidDetails.Round == "S" && (bidDetails.Postion == "FO" || bidDetails.Postion == "CP"))
            {
                packetid += "5";
            }
            return packetid;
        }
        //[Export("AuthenticationCheck")]
        //private void AuthenticationCheck()
        //{
        //    string userName = KeychainHelpers.GetPasswordForUsername("user", "WBid.WBidiPad.cwa", false);
        //    string password = KeychainHelpers.GetPasswordForUsername("pass", "WBid.WBidiPad.cwa", false);

        //    //checking  the internet connection available
        //    //==================================================================================================================
        //    if (Reachability.IsHostReachable(GlobalSettings.ServerUrl))
        //    {
        //        //  NSNotificationCenter.DefaultCenter.PostNotificationName("reachabilityCheckSuccess", null);
        //        //checking CWA credential
        //        //==================================================================================================================

        //        //this.startProgress();
        //        Authentication authentication = new Authentication();
        //        string authResult = authentication.CheckCredential(userName, password);
        //        if (authResult.Contains("ERROR: "))
        //        {
        //            InvokeOnMainThread(() =>
        //            {
        //                loadingOverlay.RemoveFromSuperview();

        //            });
        //        }
        //        else if (authResult.Contains("Exception"))
        //        {
        //            InvokeOnMainThread(() =>
        //            {
        //              
        //                this.DismissViewController(true, null);
        //                loadingOverlay.RemoveFromSuperview();

        //            });
        //        }
        //        else
        //        {
        //            // NSNotificationCenter.DefaultCenter.PostNotificationName("cwaCheckSuccess", null);
        //            // this.startProgress();

        //            _sessionCredentials = authResult;

        //            InvokeOnMainThread(() =>
        //            {
        //                loadingOverlay.updateLoadingText("Authorization Checking...");
        //            });

        //            ClientRequestModel clientRequestModel = new ClientRequestModel();
        //            clientRequestModel.Base = GlobalSettings.CurrentBidDetails.Domicile;
        //            clientRequestModel.BidRound = (GlobalSettings.CurrentBidDetails.Round == "D") ? 1 : 2;
        //            clientRequestModel.Month = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).ToString("MMM").ToUpper();
        //            clientRequestModel.Postion = GlobalSettings.CurrentBidDetails.Postion;
        //            clientRequestModel.OperatingSystem = "Ipad Os";
        //            clientRequestModel.Platform = "iPad";
        //            clientRequestModel.Token = new Guid();
        //            clientRequestModel.Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        //            //clientRequestModel.Version = "4.0.31.2";
        //            clientRequestModel.EmployeeNumber = Convert.ToInt32(Regex.Match(userName, @"\d+").Value);

        //            // client.GetAuthorizationDetailsAsync(clientRequestModel);
        //            client.GetAuthorizationforMultiPlatformAsync(clientRequestModel);
        //        }
        //    }
        //    else
        //    {
        //        InvokeOnMainThread(() =>
        //        {
      
        //            NSNotificationCenter.DefaultCenter.PostNotificationName("reachabilityCheckFailed", null);
        //            this.DismissViewController(true, null);
        //            loadingOverlay.RemoveFromSuperview();

        //        });
        //    }
        //}
		public class MyPopDelegate : UIPopoverControllerDelegate
		{
			SubmitBidViewController _parent;
			public bool CanDismiss;
			public MyPopDelegate(SubmitBidViewController parent)
			{
				_parent = parent;
			}

			public override bool ShouldDismiss(UIPopoverController popoverController)
			{
				if (CanDismiss)
				{
					return true;
				}
				else {

					return false;
				}
			}
		}

        #endregion



    }


}

