using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Core;
using CoreGraphics;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.iOS;
using WBid.WBidiPad.Model;
using System.Collections.Generic;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using System.Linq;
using WBid.WBidiPad.iOS.Utility;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using WBid.WBidiPad.SharedLibrary.SWA;
using iOSPasswordStorage;
using WBidDataDownloadAuthorizationService.Model;
using System.ServiceModel;
using System.Threading.Tasks;
using System.IO;

namespace WBid.WBidiPad.iOS
{
    public partial class BidEditorForFA : UIViewController
    {
		class MyPopDelegate : UIPopoverControllerDelegate
		{
			BidEditorForFA _parent;
			public MyPopDelegate (BidEditorForFA parent)
			{
				_parent = parent;
			}
			public override void DidDismiss (UIPopoverController popoverController)
			{
				_parent.popoverController = null;
				_parent.manualBid = string.Empty;
				NSNotificationCenter.DefaultCenter.RemoveObserver (_parent.numPadNotif);
			}
		}

        //		public List<Line> selectedLines;
        //		public List<Line> selectedFromAvailableLines;
        //		public List<Line> selectedFromSelectedLines;
        NSObject notif;
        //holds the selected availalble line items
        public List<int> selectedFromAvailableList;
        //holds all the items in the avaialble line list
        public List<int> AvaialbleLineList;

        //holds the all items in the bid Line list
        public List<string> BidLineList;
        //holds the selected bid line list.
        public List<string> SelectedBidLineList;
        public string SelectedBidLines;
        //public BidPrep BidPrepDetails { get; set; }
        string[] availablePositions = { "A", "B", "C", "D", "None" };
        public string[] availablePositionSelections = { "ABC", "ACB", "BAC", "BCA", "CAB", "CBA", "AB", "AC", "BA", "BC", "CA", "CB", "A", "B", "C" };
        private bool isNeedtosave = true;

        public bool IsAddReserveLine;
        public string SelectedBuddyBidder1Postion;
        public string SelectedBuddyBidder2Postion;

		NSObject numPadNotif;
		UIPopoverController popoverController;
		string manualBid;

       // WBidDataDwonloadAuthServiceClient client;
      //  LoadingOverlay loadingOverlay;
       // private string _sessionCredentials = string.Empty;
        private static int _selectedSeniorityNumber = 0;
        public BidEditorForFA()
            : base("BidEditorForFA", null)
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }
        public override void ViewWillDisappear(bool animated)
        {
            if (isNeedtosave)
            {
                SaveBidLineAndPositions();
            }
            base.ViewWillDisappear(animated);
			foreach (UIView view in this.View.Subviews) {

				DisposeClass.DisposeEx(view);
			}

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            isNeedtosave = true;
			//btnManualEntry.Hidden = true;
            if (GlobalSettings.BidPrepDetails.IsOnStartWithCurrentLine)
            {
                AvaialbleLineList = GlobalSettings.Lines.Select(x => x.LineNum).ToList();
            }
            else
            {
                AvaialbleLineList = GlobalSettings.Lines.Select(x => x.LineNum).OrderBy(x => x).ToList();
            }

            string befFilename = WBidHelper.GetAppDataPath() + "/" + GetBefFileName() + ".BEF";
            selectedFromAvailableList = new List<int>();
            SelectedBidLineList = new List<string>();
            if (File.Exists(befFilename))
            {
                List<string> bidlinelist = (List<string>)WBidHelper.DeSerializeObject(befFilename);

                var listPositions = bidlinelist[0].Split(',').ToList();
                bidlinelist.RemoveAt(0);
                BidLineList = new List<string>(bidlinelist);



                listPositions = listPositions.Where(x => !(x == "[None]")).ToList();
                AvaialbleLineList = new List<int>(AvaialbleLineList.Where(x => WheatherPositionShouldBeRemoved(x, BidLineList.ToList(), listPositions)));
                setupViews();
                btnFirstChoice.SetTitle(listPositions[0], UIControlState.Normal);
                btnSecondChoice.SetTitle(listPositions[1], UIControlState.Normal);
                btnThirdChoice.SetTitle(listPositions[2], UIControlState.Normal);
                btnFourthChoice.SetTitle(listPositions[3], UIControlState.Normal);
                //btnFirstChoice.TitleLabel.Text = listPositions[0];
                // btnSecondChoice.TitleLabel.Text = listPositions[1];
                // btnThirdChoice.TitleLabel.Text = listPositions[2];
                // btnFourthChoice.TitleLabel.Text = listPositions[3];
            }
            else
            {
                
                BidLineList = new List<string>();
                setupViews();
            }
            //BasicHttpBinding binding = ServiceUtils.CreateBasicHttp();
            //client = new WBidDataDwonloadAuthServiceClient(binding, ServiceUtils.EndPoint);
            //client.GetAuthorizationforMultiPlatformCompleted += client_GetAuthorizationforMultiPlatformCompleted;
            // Perform any additional setup after loading the view, typically from a nib.
            btnSaveClose.Enabled = false;
			btnBidSubmit.Enabled = false;

            tblAvailableLines.SeparatorInset = new UIEdgeInsets(0, 3, 0, 3);
            tblSelectedLines.SeparatorInset = new UIEdgeInsets(0, 3, 0, 3);

            this.btnChangeEmp.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            this.btnChangeBuddyBid.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);

			txtBidChoice.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				string text = textField.Text;
				string newText = text.Substring(0, (int)range.Location) + replacementString + text.Substring((int)range.Location + (int)range.Length);
				int val;
				if (newText == "")
					return true;
				else
					return Int32.TryParse(newText, out val);
			};

        }
        private static bool WheatherPositionShouldBeRemoved(int position, List<string> listString, List<string> listPositions)
        {
            var listPositionsIn = listString.Where(y => y.StartsWith(position.ToString()));

            var positionedListPositions = listPositions.Select(x => position.ToString() + x);

            return !positionedListPositions.All(x => listPositionsIn.Contains(x));
        }
        /// <summary>
        /// save the selected bid line and current positions to the BeF file
        /// </summary>
        private void SaveBidLineAndPositions()
        {
            string filename = WBidHelper.GetAppDataPath() + "/" + GetBefFileName() + ".BEF";
            BidLineList.Insert(0, btnFirstChoice.TitleLabel.Text + "," + btnSecondChoice.TitleLabel.Text + "," + btnThirdChoice.TitleLabel.Text + "," + btnFourthChoice.TitleLabel.Text);
            WBidHelper.SerializeObject(filename, BidLineList);
        }
        /// <summary>
        /// Get the filename for the ".BEF" file.
        /// </summary>
        /// <returns></returns>
        private string GetBefFileName()
        {
            string filename = "F";
            filename += (GlobalSettings.CurrentBidDetails.Round == "M") ? "D" : "B";
            filename += GlobalSettings.CurrentBidDetails.Domicile;
            filename += GlobalSettings.CurrentBidDetails.Month.ToString("X");
            return filename;
        }

        partial void btnChangeBuddyBidderTapped(Foundation.NSObject sender)
        {
            ChangeBuddyViewController changeBuddyy = new ChangeBuddyViewController();
            changeBuddyy.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            this.PresentViewController(changeBuddyy, true, () =>
            {
					notif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("addedBuddyBidder"), addedBuddyBidder);
            });
            //This should not be done. We should keep three Apple-said sizes. 
            //changeBuddyy.View.Superview.BackgroundColor = UIColor.Clear;
            //changeBuddyy.View.Frame = new RectangleF(0, 130, 540, 250);
            //changeBuddyy.View.Layer.BorderWidth = 1;
        }
		private bool ShowAddReserveMessage()
		{
			string buddy1 = GlobalSettings.WBidINIContent.BuddyBids.Buddy1;
			string buddy2 = GlobalSettings.WBidINIContent.BuddyBids.Buddy2;

			if ((buddy1 != "0" || buddy2 != "0")&& btnReserve.Selected==true)
			{
				btnReserve.Selected = false;
				IsAddReserveLine = false;
				

                UIAlertController okAlertController = UIAlertController.Create("You cannot add Reserve to the end of your Bid when Buddy Bidding", "", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
                return true;
			}
			return false;
		}
        partial void btnAddReserve(Foundation.NSObject sender)
        {
			btnReserve.Selected = !btnReserve.Selected;
			if(ShowAddReserveMessage())
			{
				
			}
			else
			{
            IsAddReserveLine = !IsAddReserveLine;
            
			}
			if (IsAddReserveLine || BidLineList.Count > 0)
				btnBidSubmit.Enabled = true;
			else
				btnBidSubmit.Enabled = false;

        }


        public void addedBuddyBidder(NSNotification n)
        {
            lblBuddy1.Text = "";
            lblBuddy2.Text = "";
			ShowAddReserveMessage();
            pckFirstBIddyBidderPositionSelection.UserInteractionEnabled = false;
            pckSecondBUddyBidderPositionSelection.UserInteractionEnabled = false;
            if (GlobalSettings.WBidINIContent.BuddyBids.Buddy1 != "0")
            {
                lblBuddy1.Text = GlobalSettings.WBidINIContent.BuddyBids.Buddy1;
                pckFirstBIddyBidderPositionSelection.UserInteractionEnabled = true;
            }
            if (GlobalSettings.WBidINIContent.BuddyBids.Buddy2 != "0")
            {
                lblBuddy2.Text = GlobalSettings.WBidINIContent.BuddyBids.Buddy2;
                pckSecondBUddyBidderPositionSelection.UserInteractionEnabled = true;
            }
            NSNotificationCenter.DefaultCenter.RemoveObserver(notif);
        }

        partial void btnEmployeeNumberTapped(Foundation.NSObject sender)
        {
            ChangeEmpNumberView changeEmp = new ChangeEmpNumberView();
            changeEmp.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            this.PresentViewController(changeEmp, true, null);
          
        }


        partial void btnCancelClearTapped(Foundation.NSObject sender)
        {

            isNeedtosave = false;
            string beffilename = WBidHelper.GetAppDataPath() + "/" + GetBefFileName() + ".BEF";
            if (File.Exists(beffilename))
            {
                File.Delete(beffilename);
            }
            this.DismissViewController(true, null);


        }
     
        partial void btnSubmitBidCancel(Foundation.NSObject sender)
        {
            vwSubmitBidChoiceFor.RemoveFromSuperview();

        }

        partial void btnFirstChoiceTapped(Foundation.NSObject sender)
        {
            int index = Array.IndexOf(availablePositions, btnFirstChoice.TitleLabel.Text);
            if (index == 3)
                index = 0;
            else
                index++;
            btnFirstChoice.SetTitle(availablePositions[index], UIControlState.Normal);

            if (btnSecondChoice.TitleLabel.Text == availablePositions[index])
            {
				
				btnThirdChoice.SetTitle ("None", UIControlState.Normal);
				btnFourthChoice.SetTitle ("None", UIControlState.Normal);
                btnSecondChoice.SetTitle("None", UIControlState.Normal);
            }
            if (btnThirdChoice.TitleLabel.Text == availablePositions[index])
            {
                btnThirdChoice.SetTitle("None", UIControlState.Normal);
				btnFourthChoice.SetTitle ("None", UIControlState.Normal);
            }
            if (btnFourthChoice.TitleLabel.Text == availablePositions[index])
            {
                btnFourthChoice.SetTitle("None", UIControlState.Normal);
            }
        }
        partial void btnSecondChoiceTapped(Foundation.NSObject sender)
        {
			int index = Array.IndexOf(availablePositions, btnSecondChoice.TitleLabel.Text);
//			if (index == 3)
//			{
//				btnThirdChoice.SetTitle("None", UIControlState.Normal);
//				btnFourthChoice.SetTitle("None", UIControlState.Normal);
//				btnSecondChoice.SetTitle("None", UIControlState.Normal);
//
//				return;
//			}



            for (int i = 0; i < 5; i++)
            {
							if (index == 3)
							{
								btnThirdChoice.SetTitle("None", UIControlState.Normal);
								btnFourthChoice.SetTitle("None", UIControlState.Normal);
								btnSecondChoice.SetTitle("None", UIControlState.Normal);
				
								return;
							}
                if (index == 4)
				{
                    index = 0;
				}
                else
                    index++;

                if (btnFirstChoice.TitleLabel.Text != availablePositions[index] && (btnSecondChoice.TitleLabel.Text != availablePositions[index]))
                {
                    btnSecondChoice.SetTitle(availablePositions[index], UIControlState.Normal);
                    break;
                }
            }

            if (btnThirdChoice.TitleLabel.Text == availablePositions[index])
            {
                btnThirdChoice.SetTitle("None", UIControlState.Normal);
				btnFourthChoice.SetTitle("None", UIControlState.Normal);
            }
            if (btnFourthChoice.TitleLabel.Text == availablePositions[index])
            {
                btnFourthChoice.SetTitle("None", UIControlState.Normal);
            }

        }

        partial void btnThirdChoiceTapped(Foundation.NSObject sender)
        {
            int index = Array.IndexOf(availablePositions, btnThirdChoice.TitleLabel.Text);
            if (index == 3)
            {
                btnThirdChoice.SetTitle("None", UIControlState.Normal);
				btnFourthChoice.SetTitle("None", UIControlState.Normal);
                return;
            }
			if(btnSecondChoice.TitleLabel.Text == "None")
			{
				btnThirdChoice.TitleLabel.Text="None";
                UIAlertController okAlertController = UIAlertController.Create("You cannot set the 3rd or 4th position while the 2nd position is “NONE”.  Change the 2nd position first", "", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);

                return;
			}

            for (int i = 0; i < 5; i++)
            {
                if (index == 4)
                    index = 0;
                else
                    index++;

                if ((btnFirstChoice.TitleLabel.Text != availablePositions[index]) && (btnSecondChoice.TitleLabel.Text != availablePositions[index]) && (btnThirdChoice.TitleLabel.Text != availablePositions[index]))
                {
                    btnThirdChoice.SetTitle(availablePositions[index], UIControlState.Normal);
                    break;
                }
            }
            if (btnFourthChoice.TitleLabel.Text == availablePositions[index])
            {
                btnFourthChoice.SetTitle("None", UIControlState.Normal);
            }

        }
        partial void btnFourthChoiceTapped(Foundation.NSObject sender)
        {
            int index = Array.IndexOf(availablePositions, btnFourthChoice.TitleLabel.Text);
            if (index == 3)
            {
                btnFourthChoice.SetTitle("None", UIControlState.Normal);
                return;
            }
			if(btnThirdChoice.TitleLabel.Text == "None")
			{
				btnFourthChoice.TitleLabel.Text="None";
				
                UIAlertController okAlertController = UIAlertController.Create("You cannot set the 4th position while the 3rd position is “NONE”.  Change the 3rd position first", "", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);

                return;
			}
            for (int i = 0; i < 5; i++)
            {
                if (index == 4)
                    index = 0;
                else
                    index++;

                if ((btnFirstChoice.TitleLabel.Text != availablePositions[index]) && (btnSecondChoice.TitleLabel.Text != availablePositions[index]) && (btnThirdChoice.TitleLabel.Text != availablePositions[index]) && (btnFourthChoice.TitleLabel.Text != availablePositions[index]))
                {
                    btnFourthChoice.SetTitle(availablePositions[index], UIControlState.Normal);
                    break;
                }

            }

        }

        public void updateAvailableLinesNumber(string str)
        {
            lblAvailableCount.Text = str;
        }
        public void updateBidLinesNumber(string str)
        {
            lblBidCount.Text = str;
        }
        partial void dismiss(UIKit.UIBarButtonItem sender)
        {
            this.DismissViewController(true, null);
        }
        private void setupViews()
        {

            tblAvailableLines.Layer.BorderWidth = 1;
            tblSelectedLines.Layer.BorderWidth = 1;
            tblSelectedLines.SetEditing(true, true);
            tblSelectedLines.AllowsSelectionDuringEditing = true;
			pckFirstBIddyBidderPositionSelection.Model = new bidderPositionModel(this);
			pckSecondBUddyBidderPositionSelection.Model = new bidderPositionModel(this);
            if (GlobalSettings.WBidINIContent.BuddyBids.Buddy1 != "0")
            {
                pckFirstBIddyBidderPositionSelection.UserInteractionEnabled = true;
                lblBuddy1.Text = GlobalSettings.WBidINIContent.BuddyBids.Buddy1;
            }
            if (GlobalSettings.WBidINIContent.BuddyBids.Buddy2 != "0")
            {
                pckSecondBUddyBidderPositionSelection.UserInteractionEnabled = true;
                lblBuddy2.Text = GlobalSettings.WBidINIContent.BuddyBids.Buddy2;
            }
            SelectedBuddyBidder1Postion = availablePositionSelections[0];
            SelectedBuddyBidder2Postion = availablePositionSelections[0];


            tblAvailableLines.Source = new availableBidSource(this);
            tblSelectedLines.Source = new selectedBidsSource(this);

            btnCtrl.BackgroundColor = UIColor.White;
            btnCtrl.Layer.BorderWidth = 1.0f;

            btnShift.BackgroundColor = UIColor.White;
            btnShift.Layer.BorderWidth = 1.0f;

        }
		partial void btnManualEntryTapped (UIKit.UIButton sender)
		{
			manualBid = string.Empty;
			numPadNotif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ManualBidEntry"),handleManualBidEntry);
			PopoverViewController popoverContent = new PopoverViewController ();
			popoverContent.PopType = "numberPad";
			popoverContent.SubPopType = "ManualBidEntry";
			popoverContent.numValue = "0";
			popoverController = new UIPopoverController (popoverContent);
			popoverController.Delegate = new MyPopDelegate (this);
			popoverController.PopoverContentSize = new CGSize (210, 300);
			popoverController.PresentFromRect(sender.Frame,this.View,UIPopoverArrowDirection.Any,true);
		}

		void handleManualBidEntry (NSNotification obj)
		{
			string[] str = obj.Object.ToString ().Split (new char[]{ '+' });
			manualBid = str [0];

			if (str [1] == "add")
				AddBidlinesCommand ();
			else if (str [1] == "insert")
				InsertBidLineCommand ();

			tblSelectedLines.ReloadData();
			tblAvailableLines.ReloadData();

            if (str[1] == "add" && BidLineList.Count>0)
				tblSelectedLines.ScrollToRow (NSIndexPath.FromRowSection (BidLineList.Count - 1, 0), UITableViewScrollPosition.Bottom, false);

			manualBid = string.Empty;		
			NSNotificationCenter.DefaultCenter.RemoveObserver (numPadNotif);
			popoverController.Dismiss (true);
		}
        partial void btnAdd(Foundation.NSObject sender)
        {
            //Selected lines available in selectedFromAvailableList. Need to add them to BidLineList and remove from AvaialbleLineList,SelectedBidLineList
            try
            {
                AddBidlinesCommand();
                tblSelectedLines.ReloadData();
                tblAvailableLines.ReloadData();
                if(BidLineList.Count>0)
                tblSelectedLines.ScrollToRow(NSIndexPath.FromRowSection(BidLineList.Count - 1, 0), UITableViewScrollPosition.Bottom, false);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        partial void btnInsert(Foundation.NSObject sender)
        {
            string lineNumberToAdd = txtBidChoice.Text;
            // Need to insert this to BidLineList and remove from AvaialbleLineList.	
            InsertBidLineCommand();
            tblSelectedLines.ReloadData();
            tblAvailableLines.ReloadData();
        }

        partial void btnRemove(Foundation.NSObject sender)
        {
            //Selected lines available in SelectedBidLineList. Need to add them to AvaialbleLineList and remove from BidLineList and 	
            RemoveBidlinesCommand();
            tblSelectedLines.ReloadData();
            tblAvailableLines.ReloadData();
        }


        //test
        partial void btnRepeatA(Foundation.NSObject sender)
        {
            RepeatBidLine(SelectedBidLineList, "A");
            tblSelectedLines.ReloadData();
            tblAvailableLines.ReloadData();
        }

        partial void btnRepeatB(Foundation.NSObject sender)
        {
            RepeatBidLine(SelectedBidLineList, "B");
            tblSelectedLines.ReloadData();
            tblAvailableLines.ReloadData();
        }

        partial void btnRepeatC(Foundation.NSObject sender)
        {
            RepeatBidLine(SelectedBidLineList, "C");
            tblSelectedLines.ReloadData();
            tblAvailableLines.ReloadData();
        }
		partial void btnBidSubmitTapped (UIKit.UIButton sender)
		{
			SubmitBid submitBid = new SubmitBid();
			try
			{

				//set the properties required to POST the webrequest to SWA server.
				submitBid.Base = GlobalSettings.BidPrepDetails.Domicile;
				submitBid.Bidder = GlobalSettings.TemporaryEmployeeNumber;
				submitBid.BidRound = (GlobalSettings.BidPrepDetails.BidRound == "B") ? "Round 2" : "Round 1";
				submitBid.PacketId = GenaratePacketId();
				submitBid.Seat = GlobalSettings.BidPrepDetails.Position;
				BuddyBids buddyBids = GlobalSettings.WBidINIContent.BuddyBids;
				if (GlobalSettings.BidPrepDetails.IsChkAvoidanceBid || IsAddReserveLine)
				{
					submitBid.Buddy1 = submitBid.Buddy2 = null;

				}
				else
				{
					submitBid.Buddy1 = (buddyBids.Buddy1 == "0") ? null : buddyBids.Buddy1;
					submitBid.Buddy2 = (buddyBids.Buddy2 == "0") ? null : buddyBids.Buddy2;
				}

				int dPositionCount = BidLineList.Where(x => x.Substring(x.Length - 1, 1) == "D").Count();

				if ((submitBid.Buddy1 != null || submitBid.Buddy2 != null) && dPositionCount > 0)
				{
					

                    UIAlertController alert = UIAlertController.Create(dPositionCount+ " D Lines  were deleted from the bid. Buddy Bid can only have A, B or C positions. \nPress OK to continue or CANCEL to return the position Choices. ", GlobalSettings.TemporaryEmployeeNumber ?? string.Empty, UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("Cancel ", UIAlertActionStyle.Cancel, (actionCancel) => {
                        return;
                    }));

                    alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (actionOK) => {
                        BidLineList = new List<string>(BidLineList.Where(x => !(x.Substring(x.Length - 1, 1) == "D")));

                    }));

                    this.PresentViewController(alert, true, null);


                }


                //genarate bid line to submit
                submitBid.Bid = GenarateBidLineString();
				submitBid.TotalBidChoices = BidLineList.Count();
				if (IsAddReserveLine)
				{
                    submitBid.Bid += string.IsNullOrEmpty(submitBid.Bid) ? string.Empty : ",";
                    submitBid.Bid += "R";
					submitBid.TotalBidChoices += 1;
				}



				submitBid.BuddyBidderBids = new List<BuddyBidderBid>();
				if (submitBid.Buddy1 != null)
				{
					submitBid.BuddyBidderBids.Add(new BuddyBidderBid() { BuddyBidder = submitBid.Buddy1, BidLines = GenarateBuddyBidBidLineString(SelectedBuddyBidder1Postion) });
				}

				if (submitBid.Buddy2 != null)
				{
					submitBid.BuddyBidderBids.Add(new BuddyBidderBid() { BuddyBidder = submitBid.Buddy2, BidLines = GenarateBuddyBidBidLineString(SelectedBuddyBidder2Postion) });
				}
				GlobalSettings.SubmitBid = submitBid;

				CommonClass.ObjBidEditorFA = this;
				queryViewController qv = new queryViewController();
				qv.isFromView = queryViewController.queryFromView.queryBidEditorFA;
				qv.isFirstTime=true;
				qv.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
//this.PresentViewController(qv, true, null);


UINavigationController nav = new UINavigationController(qv);
nav.ModalPresentationStyle =  UIModalPresentationStyle.FormSheet;
				//nav.PushViewController(details, true);
				this.PresentViewController(nav, true, null);



			}


			catch (Exception ex)
			{
				throw ex;
			}
		}

		public void dismissView()
		{
			this.DismissViewController(true, null);
		}
        partial void btnSaveCloseTapped(Foundation.NSObject sender)
        {
			isNeedtosave = true;
			this.DismissViewController(true,null);
        }
       

        partial void btnShiftTapped(Foundation.NSObject sender)
        {
            if (!btnShift.Selected)
                setShiftStatus(true);
            else
                setShiftStatus(false);
            setCntrlStatus(false);
        }
        partial void btnControlTapped(Foundation.NSObject sender)
        {
            if (!btnCtrl.Selected)
                setCntrlStatus(true);
            else
                setCntrlStatus(false);
            setShiftStatus(false);
        }
        private void setCntrlStatus(bool n)
        {
            btnCtrl.Selected = n;
            if (n)
            {
                //btnCtrl.BackgroundColor = ColorClass.activeOrange;
                //btnCtrl.Layer.BorderWidth = 0.0f;
                //btnCtrl.SetTitleColor (UIColor.White, UIControlState.Selected);
                btnCtrl.Selected = true;

            }
            else
            {
                //btnCtrl.BackgroundColor = UIColor.Clear;
                // btnCtrl.Layer.BorderWidth = 1.0f;
                //btnCtrl.SetTitleColor (UIColor.DarkGray, UIControlState.Normal);
                btnCtrl.Selected = false;
            }
        }
        private void setShiftStatus(bool n)
        {
            btnShift.Selected = n;
            if (n)
            {
                //btnShift.BackgroundColor = ColorClass.activeOrange;
                //btnShift.Layer.BorderWidth = 0.0f;
                //btnCtrl.SetTitleColor (UIColor.White, UIControlState.Selected);
                btnShift.Selected = true;
            }
            else
            {
                //btnShift.BackgroundColor = UIColor.Clear;
                //btnShift.Layer.BorderWidth = 1.0f;
                //btnCtrl.SetTitleColor (UIColor.DarkGray, UIControlState.Normal);
                btnShift.Selected = false;

            }
        }


        public void availableBidSelectedWithIndexPath(NSIndexPath path)
        {
            int selectedLineNumber = AvaialbleLineList[path.Row];

            if (!btnCtrl.Selected && !btnShift.Selected)
            {
                //Simply select the new one and deselect all old ones
                if (!selectedFromAvailableList.Contains(selectedLineNumber))
                {
                    selectedFromAvailableList.Clear();
                    selectedFromAvailableList.Add(selectedLineNumber);
                }
                else
                    selectedFromAvailableList.Remove(selectedLineNumber);

            }

            if (btnCtrl.Selected)
            {
                //Add this to selected list, no need to remove anything
                if (!selectedFromAvailableList.Contains(selectedLineNumber))
                {
                    selectedFromAvailableList.Add(selectedLineNumber);
                }
                else
                    selectedFromAvailableList.Remove(selectedLineNumber);

            }

            if (btnShift.Selected)
            {
                //if more than one already selected, remove all and add this to selected list. if only one selected, then select everything in between old and new.
                if (selectedFromAvailableList.Count > 1)
                {
                    selectedFromAvailableList.Clear();
                    selectedFromAvailableList.Add(selectedLineNumber);
                }
                else if (selectedFromAvailableList.Count == 1)
                {
                    //TODO


                    int currentSelected = AvaialbleLineList.IndexOf(selectedFromAvailableList[0]);
                    int till = path.Row;
                    selectedFromAvailableList.Clear();

                    if (currentSelected > path.Row)
                    {
                        for (int i = path.Row; i <= currentSelected; i++)
                        {
                            int selecteLN = AvaialbleLineList[i];
                            selectedFromAvailableList.Add(selecteLN);
                        }
                    }
                    else
                    {
                        for (int i = currentSelected; i <= path.Row; i++)
                        {
                            try
                            {
                                int selecteLN = AvaialbleLineList[i];
                                selectedFromAvailableList.Add(selecteLN);
                            }
                            catch
                            {
                            }

                        }
                    }

                }
                else if (selectedFromAvailableList.Count == 0)
                {
                    selectedFromAvailableList.Add(selectedLineNumber);
                }
            }
            tblAvailableLines.ReloadData();
        }



        public void bidSelectedWithIndexPath(NSIndexPath path)
        {
            //			string selectedLineNumber = BidLineList[path.Row].ToString();



            string selectedLineNumber = BidLineList[path.Row].ToString();

            if (!btnCtrl.Selected && !btnShift.Selected)
            {
                //Simply select the new one and deselect all old ones
                if (!SelectedBidLineList.Contains(selectedLineNumber))
                {
                    SelectedBidLineList.Clear();
                    SelectedBidLineList.Add(selectedLineNumber);
                }
                else
                    SelectedBidLineList.Remove(selectedLineNumber);

            }

            if (btnCtrl.Selected)
            {
                //Add this to selected list, no need to remove anything
                if (!SelectedBidLineList.Contains(selectedLineNumber))
                {
                    SelectedBidLineList.Add(selectedLineNumber);
                }
                else
                    SelectedBidLineList.Remove(selectedLineNumber);

            }

            if (btnShift.Selected)
            {
                //if more than one already selected, remove all and add this to selected list. if only one selected, then select everything in between old and new.
                if (SelectedBidLineList.Count > 1)
                {
                    SelectedBidLineList.Clear();
                    SelectedBidLineList.Add(selectedLineNumber);
                }
                else if (SelectedBidLineList.Count == 1)
                {
                    //TODO



                    int currentSelected = BidLineList.IndexOf(SelectedBidLineList[0]);
                    int till = path.Row;
                    SelectedBidLineList.Clear();

                    if (currentSelected > path.Row)
                    {
                        for (int i = path.Row; i <= currentSelected; i++)
                        {
                            string selecteLN = BidLineList[i];
                            SelectedBidLineList.Add(selecteLN);
                        }
                    }
                    else
                    {
                        for (int i = currentSelected; i <= path.Row; i++)
                        {
                            string selecteLN = BidLineList[i];
                            SelectedBidLineList.Add(selecteLN);
                        }
                    }

                }
                else if (SelectedBidLineList.Count == 0)
                {
                    SelectedBidLineList.Add(selectedLineNumber);
                }
            }
            tblSelectedLines.ReloadData();



            //			if (!btnCtrl.Selected && !btnShift.Selected)
            //			{
            //				//Simply select the new one and deselect all old ones
            //				if (!SelectedBidLineList.Contains(selectedLineNumber))
            //				{
            //					SelectedBidLineList.Clear();
            //					SelectedBidLineList.Add(selectedLineNumber);
            //				}
            //				else
            //					SelectedBidLineList.Remove(selectedLineNumber);
            //
            //			}
            //
            //			if (btnCtrl.Selected)
            //			{
            //				//Add this to selected list, no need to remove anything
            //				if (!SelectedBidLineList.Contains(selectedLineNumber))
            //				{
            //					SelectedBidLineList.Add(selectedLineNumber);
            //				}
            //				else
            //					SelectedBidLineList.Remove(selectedLineNumber);
            //
            //			}
            //
            //			if (btnShift.Selected)
            //			{
            //				//if more than one already selected, remove all and add this to selected list. if only one selected, then select everything in between old and new.
            //				if (SelectedBidLineList.Count > 1)
            //				{
            //					SelectedBidLineList.Clear();
            //					SelectedBidLineList.Add(selectedLineNumber);
            //				}
            //				else if (SelectedBidLineList.Count == 1)
            //				{
            //					//TODO
            //					string currentString = SelectedBidLineList [0];
            //					int currentSelected =  BidLineList.IndexOf (currentString);
            //
            //					int till = path.Row;
            //					SelectedBidLineList.Clear();
            //
            //					if (currentSelected > path.Row)
            //					{
            //						for (int i = path.Row; i <= currentSelected; i++)
            //						{
            //							string selectedLN = BidLineList[i].ToString();
            //							SelectedBidLineList.Add(selectedLN);
            //						}
            //					}
            //					else
            //					{
            //						for (int i = currentSelected; i <= path.Row; i++)
            //						{
            //							string selectedLN = BidLineList[i].ToString();
            //							SelectedBidLineList.Add(selectedLN);
            //						}
            //					}
            //
            //				}
            //				else if (SelectedBidLineList.Count == 0)
            //				{
            //					SelectedBidLineList.Add(selectedLineNumber);
            //				}
            //			}
            //			tblAvailableLines.ReloadData();
        }

        /// <summary>
        /// Add selected "avaialble lines" to the the "Bid line" list
        /// </summary>
        /// <param name="obj"></param>
        private void AddBidlinesCommand()
        {
            try
            {
				if (!string.IsNullOrEmpty(manualBid))
                {
					int line = int.Parse(manualBid);
					var bidlinelist = GetBidLinelistforbidChoice(manualBid);
                    bidlinelist.ForEach(BidLineList.Add);
                    // SelectedBidLines = bidlinelist.LastOrDefault();
                    var objline = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == line);
                    if (objline != null)
                    {
                        List<string> fapositions = objline.FAPositions;
                        if (fapositions.All(x => BidLineList.Contains(string.Format("{0}{1}", line, x))))
                            AvaialbleLineList.Remove(line);
                    }
                }
                else
                    if (selectedFromAvailableList.Count > 0)
                    {
                        // List<string> itemstoadd = new List<string>();
                        List<string> itemstoadd = null;
                        foreach (int line in selectedFromAvailableList)
                        {
                            itemstoadd = new List<string>();
                            var lineitems = GetBidLinelistforPositions(line);
                            if (lineitems.Count > 0)
                            {
                                itemstoadd.AddRange(GetBidLinelistforPositions(line));
                                itemstoadd.ForEach(BidLineList.Add);
                                List<string> fapositions = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == line).FAPositions;

                                if (fapositions.All(x => BidLineList.Contains(string.Format("{0}{1}", line, x))))
                                    AvaialbleLineList.Remove(line);
                            }
                            ////////////////////
                            // SelectedBidLines = BidLineList.LastOrDefault();
                        }

                    }
                //  IsEnableSubmitButton = (BidLineList.Count > 0) ? true : false;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        private List<string> GetBidLinelistforPositions(int line)
        {
            List<string> fapositions = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == line).FAPositions;

            List<string> itemstoadd = new List<string>();
            if (fapositions != null)
            {
                if (btnFirstChoice.TitleLabel.Text != "[None]" && !BidLineList.Contains(line + btnFirstChoice.TitleLabel.Text) && fapositions.Contains(btnFirstChoice.TitleLabel.Text))
                    itemstoadd.Add(line + btnFirstChoice.TitleLabel.Text);
                if (btnSecondChoice.TitleLabel.Text != "[None]" && !BidLineList.Contains(line + btnSecondChoice.TitleLabel.Text) && fapositions.Contains(btnSecondChoice.TitleLabel.Text))
                    itemstoadd.Add(line + btnSecondChoice.TitleLabel.Text);
                if (btnThirdChoice.TitleLabel.Text != "[None]" && !BidLineList.Contains(line + btnThirdChoice.TitleLabel.Text) && fapositions.Contains(btnThirdChoice.TitleLabel.Text))
                    itemstoadd.Add(line + btnThirdChoice.TitleLabel.Text);
                if (btnFourthChoice.TitleLabel.Text != "[None]" && !BidLineList.Contains(line + btnFourthChoice.TitleLabel.Text) && fapositions.Contains(btnFourthChoice.TitleLabel.Text))
                    itemstoadd.Add(line + btnFourthChoice.TitleLabel.Text);
            }
            return itemstoadd;

        }
        /// <summary>
        /// Get the bid lines if the user enters the data in to the Bid line text box(Appended the positions)
        /// </summary>
        /// <param name="bidChoice"></param>
        /// <returns></returns>
        private List<string> GetBidLinelistforbidChoice(string bidChoice)
        {
            List<string> bidline = new List<string>();

            string pattern = "^\\d{1,3}[A-Da-d]{0,4}$";
			if (Regex.Match(manualBid, pattern).Success)
            {
				string numberInTheFileName = string.Join(null, Regex.Split(manualBid, "[^\\d]"));
                int linenumber = Int32.Parse(numberInTheFileName);
                if (linenumber >= AvaialbleLineList[0] && linenumber <= AvaialbleLineList[AvaialbleLineList.Count - 1])
                {
                    List<string> fapositions = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == linenumber).FAPositions;
                    string[] positions = txtBidChoice.Text.Split(new string[] { linenumber.ToString() }, StringSplitOptions.RemoveEmptyEntries);
                    //check any positions entered in the Bid choice
                    if (positions.Count() > 0)
                    {
                        List<string> manualpositonedlines = new List<string>();
                        foreach (var position in positions[0])
                        {
                            string item = linenumber.ToString() + position.ToString().ToUpper();
                            if (!BidLineList.Contains(item) && fapositions.Contains(position.ToString().ToUpper()))
                            {
                                bidline.Add(item);
                            }
                        }
                    }
                    else
                    {
                        bidline = GetBidLinelistforPositions(linenumber);
                    }
                }

                txtBidChoice.Text = string.Empty;

            }
            else
            {
                //  MessageBox.Show("Invalid Bid Choice", "Wrong Bid Choice", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            return bidline;
        }
        /// <summary>
        /// Insert the lines to the Bid line list in a specfied position  selected by the user
        /// </summary>
        /// <param name="obj"></param>
        private void InsertBidLineCommand()
        {
            try
            {
                int index = BidLineList.IndexOf(SelectedBidLines);
                index = (index < 0) ? 0 : index;

                List<string> bidlines = new List<string>();
				if (!string.IsNullOrEmpty(manualBid))
                {
					bidlines = GetBidLinelistforbidChoice(manualBid);
                }
                else if (selectedFromAvailableList.Count > 0)
                {
                    foreach (int line in selectedFromAvailableList)
                    {
                        var lineitems = GetBidLinelistforPositions(line);
                        if (lineitems.Count > 0)
                        {
                            bidlines.AddRange(lineitems);

                        }
                    }
                }
                BidLineList.InsertRange(index, bidlines);
                //BidLineList.InsertRange(bidlines, index);
                SelectedBidLines = bidlines.LastOrDefault();

                foreach (var line in selectedFromAvailableList)
                {
                    List<string> fapositions = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == line).FAPositions;
                    if (fapositions.All(x => BidLineList.Contains(string.Format("{0}{1}", line, x))))
                        AvaialbleLineList.Remove(line);

                }
                //IsEnableSubmitButton = (BidLineList.Count > 0) ? true : false;
            }
            catch (Exception ex)
            {
                throw ex;

            }

        }

        private void RepeatBidLine(List<string> choices, string position)
        {
            if (choices != null)
            {
                foreach (var selectedBid in choices)
                {
                    int linenumber = Convert.ToInt32(string.Join(null, Regex.Split(selectedBid, "[^\\d]")));
                    var fapositions = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == linenumber).FAPositions;
                    if (!BidLineList.Contains(linenumber + position) && fapositions.Contains(position))
                    {
                        BidLineList.Add(linenumber + position);
                        if (fapositions.All(x => BidLineList.Contains(string.Format("{0}{1}", linenumber, x))))
                            AvaialbleLineList.Remove(linenumber);
                    }
                }
            }
        }

        /// <summary>
        /// Remove the lines from the bid line list
        /// </summary>
        /// <param name="obj"></param>
        private void RemoveBidlinesCommand()
        {

            try
            {
                List<string> collection = SelectedBidLineList;
                collection.Reverse();
                if (collection.Count > 0)
                {
                    foreach (string bidline in collection)
                    {
                        int index = BidLineList.IndexOf(bidline);
                        BidLineList.Remove(bidline);

                        AddBidLineToAvaialableLines(bidline);
                        // IsEnableSubmitButton = (BidLineList.Count > 0) ? true : false;
                        //set the list box  selection
                        if (index > 0)
                            index = index - 1;
                        else
                            return;
                        SelectedBidLines = BidLineList[index];

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
		public void enableOrDisableSaveButton(bool status)
		{
			btnSaveClose.Enabled = status;
			//btnBidSubmit.Enabled = status;
			if (IsAddReserveLine || BidLineList.Count > 0)
				btnBidSubmit.Enabled = true;
			else
				btnBidSubmit.Enabled = false;

		}
		 
        private void AddBidLineToAvaialableLines(string column)
        {
            //get the line number
            var linenumber = int.Parse(string.Join(null, Regex.Split(column, "[^\\d]")));
            if (!AvaialbleLineList.Contains(linenumber))
            {
                //insert into the avaialbe line list
                AvaialbleLineList.Add(linenumber);
                if (!GlobalSettings.BidPrepDetails.IsOnStartWithCurrentLine)
                {
                    AvaialbleLineList = new List<int>(AvaialbleLineList.OrderBy(x => x));
                }
                else
                {
                    var orderedlines = AvaialbleLineList.ToList().OrderBy(x => GlobalSettings.Lines.Select(y => y.LineNum).ToList().IndexOf(x));
                    AvaialbleLineList = new List<int>(orderedlines);
                }
            }
        }

        private string GenarateBuddyBidBidLineString(string SelectedBuddyBidder1Postion)
        {
            string bidOrder = string.Empty;

            List<string> tempList = BidLineList.Select(x => x.Substring(0, x.Length - 1)).Distinct().ToList();


            foreach (var line in tempList)
            {
                foreach (char item in SelectedBuddyBidder1Postion)
                {
                    if (GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == Convert.ToInt32(line)).FAPositions.Contains(item.ToString()))
                    {
                        if (bidOrder != string.Empty)
                        {
                            bidOrder += ",";
                        }

                        bidOrder += line + item;
                    }
                }

            }

            return bidOrder;

        }
        /// </summary>
        /// <param name="bidDetails"></param>
        /// <returns></returns>
        private string GenaratePacketId()
        {
            string packetid = string.Empty;
            packetid = GlobalSettings.BidPrepDetails.Domicile + GlobalSettings.BidPrepDetails.BidYear + GlobalSettings.BidPrepDetails.BidPeriod.ToString("d2");

            //Set-round-numbers:
            //1 - F/A monthly bids
            //2 - F/A supplemental bids
            //3 - reserved
            //4 - Pilot monthly bids
            //5 - Pilot supplemental bids

            //D first Round  B second Round
            if (GlobalSettings.BidPrepDetails.Position == "FA")
            {
                packetid += (GlobalSettings.BidPrepDetails.BidRound == "D") ? "1" : "2";
            }
            else if (GlobalSettings.BidPrepDetails.Position == "CP" || GlobalSettings.BidPrepDetails.Position == "FO")
            {
                packetid += (GlobalSettings.BidPrepDetails.BidRound == "D") ? "4" : "5";
            }
            else
            {
                packetid = "3";
            }


            return packetid;
        }
        /// <summary>
        /// PURPOSE : Generate Bid lines
        /// </summary>
        /// <returns></returns>
        private string GenarateBidLineString()
        {
            string bidLines = string.Empty;
            bidLines = string.Join(",", BidLineList.Select(x => x.ToString()));
            return bidLines;
        }



        //private void  LoadDatatoBidQueryWindow()
        //{
        //    string employeeNumber = string.Empty;
        //    employeeNumber = GlobalSettings.TemporaryEmployeeNumber;
        //    lblQueryHeader.Text = "Submitting " + GlobalSettings.SubmitBid.SeniorityNumber + " Bid Choices for";
        //    txtSubmitBid.Text = employeeNumber;
        //    setUpBuddyBidLabel(GetBuddyBid());
        //}
        //private string GetBuddyBid()
        //{
        //    string buddyBidStr = string.Empty;
        //    BuddyBids buddyBids = GlobalSettings.WBidINIContent.BuddyBids;
        //    //disable buddy bid
        //    buddyBidStr += (buddyBids.Buddy1 != "0") ? buddyBids.Buddy1.ToString() + "," : "";
        //    buddyBidStr += (buddyBids.Buddy2 != "0") ? buddyBids.Buddy2.ToString() + "," : "";
        //    buddyBidStr = buddyBidStr.TrimEnd(',');
        //    return buddyBidStr;

        //}
        partial void btnClear(Foundation.NSObject sender)
        {
           UIAlertController alert = UIAlertController.Create("Are you sure you want to clear are bid choices?","", UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("NO", UIAlertActionStyle.Cancel, (actionCancel) => {
               
            }));

            alert.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, (actionOK) => {
                BidLineList.Clear();
                AvaialbleLineList = new List<int>(GlobalSettings.Lines.Select(x => x.LineNum));
                tblSelectedLines.ReloadData();
                tblAvailableLines.ReloadData();
            }));

            this.PresentViewController(alert, true, null);
        }
        public void bidTableSetEditing(bool status)
        {
            tblSelectedLines.SetEditing(status, true);
        }




    }


    public class availableBidSource : UITableViewSource
    {
        BidEditorForFA parentVC;
        public availableBidSource(BidEditorForFA parent)
        {
            parentVC = parent;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            // TODO: return the actual number of sections
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            parentVC.updateAvailableLinesNumber(parentVC.AvaialbleLineList.Count.ToString() + " " + "Lines");
            return parentVC.AvaialbleLineList.Count;
        }
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            NSString cellIdentifier = new NSString("cell1");
            UITableViewCell cell = tableView.DequeueReusableCell(cellIdentifier) as UITableViewCell;
            if (cell == null)
                cell = new UITableViewCell();
            cell.BackgroundColor = UIColor.White;
            try
            {
                if (parentVC.selectedFromAvailableList.Contains(parentVC.AvaialbleLineList[indexPath.Row]))
                {
                    cell.BackgroundColor = UIColor.LightGray;
                }
            }
            catch
            {
                //`execption
            }

            cell.TextLabel.Text = parentVC.AvaialbleLineList[indexPath.Row].ToString();
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            parentVC.availableBidSelectedWithIndexPath(indexPath);

        }
    }


    public class selectedBidsSource : UITableViewSource
    {
        BidEditorForFA parentVC;
        public selectedBidsSource(BidEditorForFA parent)
        {
            parentVC = parent;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            // TODO: return the actual number of sections
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            parentVC.updateBidLinesNumber(parentVC.BidLineList.Count.ToString() + " " + "Bids");

            parentVC.enableOrDisableSaveButton(parentVC.BidLineList.Count != 0);

            return parentVC.BidLineList.Count;
        }
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            NSString cellIdentifier = new NSString("cell2");
            UITableViewCell cell = tableView.DequeueReusableCell(cellIdentifier) as UITableViewCell;
            if (cell == null)
            {
                cell = new UITableViewCell();

            }

            cell.BackgroundColor = UIColor.White;
            try
            {
                if (parentVC.SelectedBidLineList.Contains(parentVC.BidLineList[indexPath.Row]))
                {
                    cell.BackgroundColor = UIColor.LightGray;
                }
            }
            catch
            {
                //`execption
            }
            cell.TextLabel.Text = parentVC.BidLineList[indexPath.Row].ToString();
            //			UILongPressGestureRecognizer lp = new UILongPressGestureRecognizer (lphandle);
            //			cell.AddGestureRecognizer (lp);
            return cell;
        }

        private void lphandle(UILongPressGestureRecognizer lp)
        {

        }
        private void addLongPressGesture(UITableViewCell cell)
        {
            UILongPressGestureRecognizer lp = new UILongPressGestureRecognizer(hlp);
            cell.AddGestureRecognizer(lp);
        }
        private void hlp(UILongPressGestureRecognizer gest)
        {
            if (gest.State == UIGestureRecognizerState.Began)
            {
                //				tblSelectedLines.SetEditing (true, false);
                parentVC.bidTableSetEditing(true);
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            parentVC.SelectedBidLines = parentVC.BidLineList[indexPath.Row].ToString();
            parentVC.bidSelectedWithIndexPath(indexPath);
        }
        public override bool CanMoveRow(UITableView tableView, NSIndexPath indexPath)
        {
            return true;
        }
        public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return UITableViewCellEditingStyle.None;
        }
        public override bool ShouldIndentWhileEditing(UITableView tableView, NSIndexPath indexPath)
        {
            return false;
        }
        public override void MoveRow(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
        {
            string str = parentVC.BidLineList[sourceIndexPath.Row];
            parentVC.BidLineList.RemoveAt(sourceIndexPath.Row);
            parentVC.BidLineList.Insert(destinationIndexPath.Row, str);
        }
    }


    public class bidderPositionModel : UIPickerViewModel
    {
        BidEditorForFA parent;
        public bidderPositionModel(BidEditorForFA parentVC)
        {
            parent = parentVC;
        }

        public override nint GetComponentCount(UIPickerView picker)
        {
            return 1;
        }

        public override nint GetRowsInComponent(UIPickerView picker, nint component)
        {
            return parent.availablePositionSelections.Count();
        }

        public override string GetTitle(UIPickerView picker, nint row, nint component)
        {
            return parent.availablePositionSelections[row];
        }
        public override void Selected(UIPickerView picker, nint row, nint component)
        {
            if (picker.Tag == 0)
                parent.SelectedBuddyBidder1Postion = parent.availablePositionSelections[row];
            else
                parent.SelectedBuddyBidder2Postion = parent.availablePositionSelections[row];
        }
    }

}

