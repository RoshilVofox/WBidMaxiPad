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
namespace WBid.WBidiPad.iOS
{
	public partial class FAPostionChoiceViewController : UIViewController
	{
        //private  ObservableCollection<string> _bidLineList { get; set; }
        private string _buddyposition1 { get; set; }
        private string _buddyposition2 { get; set; }
        private string _buddyposition3 { get; set; }


		string[] availablePositions = {"A","B","C","D","None"};
		string[] availablePositionsWithNone = {"A","B","C","D","None"};
		public string[] availablePositionSelections = { "ABC", "ACB", "BAC", "BCA", "CAB", "CBA", "AB", "AC", "BA", "BC", "CA", "CB", "A", "B", "C" };
		public bool IsAddReserveLine;
		public string SelectedBuddyBidder1Postion;
		public string SelectedBuddyBidder2Postion;
		public List<string> AvaialbleLineList = new List<string>();
		public List<string> selectedFromAvailableList = new List<string>();
        public List<Line> Lines { get; set; }
		NSObject notif;

		//holds the all items in the bid Line list
		public List<string> BidLineList = new List<string>();
		//holds the selected bid line list.
		public List<string> SelectedBidLineList = new List<string>();
        private ObservableCollection<string> AvailalbelistOrder;
		public string SelectedBidLines;
		bool BPositionLineEnable = false;
		bool CPositionLineEnable = false;
		bool DPositionLineEnable = false;
		int APositionLineCount = 0;
		int BPositionLineCount = 0;
		int CPositionLineCount = 0;
		int DPositionLineCount = 0;
		int PositionLineTotal = 0;
		public FAPostionChoiceViewController () : base ("FAPostionChoiceViewController", null)
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

			this.Title = "FA Position Choice";
			this.btnChangeBuddy.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnFirstPos.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnSecondPos.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnThirdPos.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnFourthPos.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);

			// Perform any additional setup after loading the view, typically from a nib.
			pkrBuddyPos1.Model = new bidderPositionModel (this);
			pkrBuddyPos2.Model = new bidderPositionModel (this);
            pkrBuddyPos1.Tag = 0;
            pkrBuddyPos2.Tag = 1;
            SelectedBuddyBidder1Postion = "ABC";
            SelectedBuddyBidder2Postion = "ABC";

			sgCustomOrder.Enabled = false;
			vwCustomBid.Hidden = true;
			tblAvailable.Source = new availableBidSource(this);
			tblBid.Source = new selectedBidsSource(this);

			btnCtrl.BackgroundColor = UIColor.White;
			btnCtrl.Layer.BorderWidth = 1.0f;

			btnShift.BackgroundColor = UIColor.White;
			btnShift.Layer.BorderWidth = 1.0f;
			setBuddybidLabel ();

			tblAvailable.Layer.BorderWidth = 1;
			tblBid.Layer.BorderWidth = 1;
			hideRepeatPositionViews();

			if (GlobalSettings.CurrentBidDetails.Postion == "FA")
			{
				Lines = GlobalSettings.Lines.ToList();
				var fapositions = Lines.Select(x => x.FAPositions);
				APositionLineCount = fapositions.Count(x => x.Contains("A"));
				BPositionLineCount = fapositions.Count(x => x.Contains("B"));
				CPositionLineCount = fapositions.Count(x => x.Contains("C"));
				DPositionLineCount=fapositions.Count(x => x.Contains("D"));

				lblPosExistValue1.Text = APositionLineCount.ToString();
				lblPosExistValue2.Text = BPositionLineCount.ToString();
				lblPosExistValue3.Text = CPositionLineCount.ToString();

				lblSelectedFirstPos.Text = btnFirstPos.TitleLabel.Text;
				lblSelectedSecondPos.Text = btnSecondPos.TitleLabel.Text;
				lblSelectedThirdPos.Text = btnThirdPos .TitleLabel.Text;


				//IsNeedToEnterPosition = (SubmitBidDetails.IsSubmitAllChoices == false && IsRepeatPositions);
				//SubmitTotal = "Submit " + SubmitBidDetails.SeniorityNumber + " Total";
			}
			txtLinesInFirstPos.Text = "0";
			txtLinesInSecondPos.Text = "0";
			txtLinesInThirdPos.Text = "0";
			txtLinesInFourthPos.Text = "0";
			setTextLimitForRepeatPosFields();
			SetRepeatPositionUI();

			//txtLinesInFirstPos.ShouldChangeCharacters = (textField, range, replacementString) =>
   //         {
			//	var newLength = textField.Text.Length + replacementString.Length - range.Length;
			//	return newLength <= 4;

			//	string text = textField.Text;
			//	string newText = text.Substring(0, (int)range.Location) + replacementString + text.Substring((int)range.Location + (int)range.Length);
			//	int val;
			//	if (newText == "")
			//		return true;
			//	else
			//		return Int32.TryParse(newText, out val);
				
	             
   //         };


		}
		private void SetRepeatPositionUI()
		{
			if ((GlobalSettings.SubmitBid.IsSubmitAllChoices == false && sgPosCombination.SelectedSegment == 1))
			{
				var fapositions = new List<List<string>>();
				if (Lines != null)
					fapositions = Lines.Select(x => x.FAPositions).ToList();


				lblPosExistValue1.Text = fapositions.Count(x => x.Contains(btnFirstPos.TitleLabel.Text)).ToString();

				lblSelectedFirstPos.Text = btnFirstPos.TitleLabel.Text;

				if (btnSecondPos.TitleLabel.Text == "None")
				{
					txtLinesInSecondPos.Enabled = false;
					txtLinesInSecondPos.Text = "0";
					lblPosExistValue2.Text = string.Empty;
					lblSelectedSecondPos.Text = string.Empty;
				}
				else
				{
					txtLinesInSecondPos.Enabled = true;
					lblPosExistValue2.Text = fapositions.Count(x => x.Contains(btnSecondPos.TitleLabel.Text)).ToString();
					lblSelectedSecondPos.Text = btnSecondPos.TitleLabel.Text;
				}
				if (btnThirdPos.TitleLabel.Text == "None")
				{
					txtLinesInThirdPos.Enabled = false;
					txtLinesInThirdPos.Text = "0";
					lblPosExistValue3.Text = string.Empty;
					lblSelectedThirdPos.Text = string.Empty;
				}
				else
				{
					txtLinesInThirdPos.Enabled = true;
					lblPosExistValue3.Text = fapositions.Count(x => x.Contains(btnThirdPos.TitleLabel.Text)).ToString();
					lblSelectedThirdPos.Text = btnThirdPos.TitleLabel.Text;
				}
				if (btnFourthPos.TitleLabel.Text == "None")
				{
					txtLinesInFourthPos.Enabled = false;
					txtLinesInFourthPos.Text = "0";
					lblPosExistValue4.Text = string.Empty;
					lblSelectedFourthPos.Text = string.Empty;
				}
				else
				{
					txtLinesInFourthPos.Enabled = true;
					lblPosExistValue4.Text = fapositions.Count(x => x.Contains(btnFourthPos.TitleLabel.Text)).ToString();
					lblSelectedFourthPos.Text = btnFourthPos.TitleLabel.Text;
			}
				calcualteTotalbidchoiceselected();
			}
		}
		private void setTextLimitForRepeatPosFields()
		{
			txtLinesInFirstPos.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				string text = textField.Text;
				string newText = text.Substring(0, (int)range.Location) + replacementString + text.Substring((int)range.Location + (int)range.Length);
				int val;
				if (newText == "")
					return true;
				else if (newText.Length <= 4) 
					return Int32.TryParse(newText, out val);
				else
					return false;
			};

			txtLinesInSecondPos.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				string text = textField.Text;
				string newText = text.Substring(0, (int)range.Location) + replacementString + text.Substring((int)range.Location + (int)range.Length);
				int val;
				if (newText == "")
					return true;
				else
					return Int32.TryParse(newText, out val);
			};
			txtLinesInThirdPos.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				string text = textField.Text;
				string newText = text.Substring(0, (int)range.Location) + replacementString + text.Substring((int)range.Location + (int)range.Length);
				int val;
				if (newText == "")
					return true;
				else
					return Int32.TryParse(newText, out val);
			};
			txtLinesInFourthPos.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				string text = textField.Text;
				string newText = text.Substring(0, (int)range.Location) + replacementString + text.Substring((int)range.Location + (int)range.Length);
				int val;
				if (newText == "")
					return true;
				else
					return Int32.TryParse(newText, out val);
			};


			txtLinesInFirstPos.EditingDidEnd += (object sender, EventArgs e) =>
			{
				if (txtLinesInFirstPos.Text == "")
					txtLinesInFirstPos.Text = "0";
				
				calcualteTotalbidchoiceselected();
			};

			txtLinesInSecondPos.EditingDidEnd += (object sender, EventArgs e) =>
			{
				if (txtLinesInSecondPos.Text == "")
					txtLinesInSecondPos.Text = "0";

				calcualteTotalbidchoiceselected();
			};


			txtLinesInThirdPos.EditingDidEnd += (object sender, EventArgs e) =>
			{
				if (txtLinesInThirdPos.Text == "")
					txtLinesInThirdPos.Text = "0";

				calcualteTotalbidchoiceselected();
			};

			txtLinesInFourthPos.EditingDidEnd += (object sender, EventArgs e) =>
			{
				if (txtLinesInFourthPos.Text == "")
					txtLinesInFourthPos.Text = "0";

				calcualteTotalbidchoiceselected();
			};
		}
		/// <summary>
		/// Calcualtes the total position lines entered by the user.
		/// </summary>
		private void calcualteTotalbidchoiceselected()
		{
			lblPositionTotal.Text = (Convert.ToInt32(txtLinesInFirstPos.Text) + Convert.ToInt32(txtLinesInSecondPos.Text) + Convert.ToInt32(txtLinesInThirdPos.Text) + Convert.ToInt32(txtLinesInFourthPos.Text)).ToString();
		}
												  
		private void setBuddybidLabel ()
		{
            lblBuddy1.Text = (GlobalSettings.WBidINIContent.BuddyBids.Buddy1 == "0") ? string.Empty : GlobalSettings.WBidINIContent.BuddyBids.Buddy1;
            lblBuddy2.Text = (GlobalSettings.WBidINIContent.BuddyBids.Buddy2 == "0") ? string.Empty : GlobalSettings.WBidINIContent.BuddyBids.Buddy2;
			lblBuddy1.Text = (GlobalSettings.SubmitBid.Buddy1 == null) ? string.Empty : GlobalSettings.SubmitBid.Buddy1;
			lblBuddy2.Text = (GlobalSettings.SubmitBid.Buddy2 == "0") ? string.Empty : GlobalSettings.SubmitBid.Buddy2;
			if (lblBuddy1.Text == "")
				pkrBuddyPos1.UserInteractionEnabled = false;
			else
				pkrBuddyPos1.UserInteractionEnabled = true;

			if (lblBuddy2.Text == "")
				pkrBuddyPos2.UserInteractionEnabled = false;
			else
				pkrBuddyPos2.UserInteractionEnabled = true;

		}

		[Export("SetSubmitButtonStatus")]
        private void SetSubmitButtonStatus()
        {


            btnSubmit.Enabled = (btnFirstPos.TitleLabel.Text != "None" || btnSecondPos.TitleLabel.Text != "None" || btnThirdPos.TitleLabel.Text != "None" || btnFourthPos.TitleLabel.Text != "None");
			SetRepeatPositionUI();

            
        }

		partial void btnDismissTapped (UIKit.UIBarButtonItem sender)
		{
			this.DismissViewController(true,null);
		}
		partial void sgPosCombinationTapped (UIKit.UISegmentedControl sender)
		{
			Lines = GlobalSettings.Lines.ToList();
			if (sender.SelectedSegment == 0) {
				sgCustomOrder.Enabled = false;
				hideRepeatPositionViews();

			} else if (sender.SelectedSegment == 1) {
				sgCustomOrder.Enabled = false;
				if (GlobalSettings.SubmitBid.IsSubmitAllChoices == false)
				{
					SetRepeatPositionUI();
					lblSubmitTotal.Text = "Submit " + GlobalSettings.SubmitBid.SeniorityNumber + " Total";
					displayRepeatPositionViews();
				}



			}
			else if (sender.SelectedSegment == 2) {
				sgCustomOrder.Enabled = true;
				hideRepeatPositionViews();
			}
		}

		public void hideRepeatPositionViews()
		{
			//lblLinesInPosition.Hidden = true;
			//lblPositionsExist.Hidden = true;

			//txtLinesInFirstPos.Hidden = true;
			//txtLinesInSecondPos.Hidden = true;
			//txtLinesInThirdPos.Hidden = true;
			//txtLinesInFourthPos.Hidden = true;
			vwRepeatPositionShow.Hidden = true;
			vwPositionDetails.Frame = new CGRect(10, 210, 510, 116);

		}
		public void displayRepeatPositionViews()
		{
			//lblLinesInPosition.Hidden = false;
			//lblPositionsExist.Hidden = false;

			//txtLinesInFirstPos.Hidden = false;
			//txtLinesInSecondPos.Hidden = false;
			//txtLinesInThirdPos.Hidden = false;
			//txtLinesInFourthPos.Hidden = false;
			vwRepeatPositionShow.Hidden = false;
			vwPositionDetails.Frame = new CGRect(10, 150, 510, 116);
			vwRepeatPositionShow.Layer.BorderWidth = 1;
		}


		partial void sgCustomOrderTapped (UIKit.UISegmentedControl sender)
		{

		}
		partial void btnFirstPosTapped (UIKit.UIButton sender)
		{
			int index = Array.IndexOf(availablePositions,btnFirstPos.TitleLabel.Text);
			if (index == 3)
				index = 0;
			else
				index++;
			btnFirstPos.SetTitle (availablePositions[index], UIControlState.Normal);

			if(btnSecondPos.TitleLabel.Text == availablePositions[index])
			{
				btnSecondPos.SetTitle ("None", UIControlState.Normal);
				btnFourthPos.SetTitle ("None", UIControlState.Normal);
				btnThirdPos.SetTitle ("None", UIControlState.Normal);
			}
			if(btnThirdPos.TitleLabel.Text == availablePositions[index])
			{
				btnThirdPos.SetTitle ("None", UIControlState.Normal);
				btnFourthPos.SetTitle ("None", UIControlState.Normal);

			}
			if(btnFourthPos.TitleLabel.Text == availablePositions[index])
			{
				btnFourthPos.SetTitle ("None", UIControlState.Normal);
			}

			//SetSubmitButtonStatus();
			this.PerformSelector(new ObjCRuntime.Selector("SetSubmitButtonStatus"),null,0.5);
		}
		partial void btnSecondPosTapped (UIKit.UIButton sender)
		{
			
			int index = Array.IndexOf(availablePositions,btnSecondPos.TitleLabel.Text);
			if(index == 3)
			{
				btnThirdPos.SetTitle ("None", UIControlState.Normal);
				btnFourthPos.SetTitle ("None", UIControlState.Normal);
				btnSecondPos.SetTitle ("None", UIControlState.Normal);
				this.PerformSelector(new ObjCRuntime.Selector("SetSubmitButtonStatus"),null,0.5);
				return;
			}



			for(int i=0; i<5; i++)
			{
				if (index == 4)
					index = 0;
				else
					index++;

				if(btnFirstPos.TitleLabel.Text != availablePositions[index] && (btnSecondPos.TitleLabel.Text != availablePositions[index]) )
				{
					btnSecondPos.SetTitle (availablePositions[index], UIControlState.Normal);
					break;
				}
			}


			if(btnThirdPos.TitleLabel.Text == availablePositions[index])
			{
				btnThirdPos.SetTitle ("None", UIControlState.Normal);
				btnFourthPos.SetTitle ("None", UIControlState.Normal);
			}
			if(btnFourthPos.TitleLabel.Text == availablePositions[index])
			{
				btnFourthPos.SetTitle ("None", UIControlState.Normal);
			}
 
			//SetRepeatPositionUI();
			//SetSubmitButtonStatus();
			this.PerformSelector(new ObjCRuntime.Selector("SetSubmitButtonStatus"),null,0.5);

		}
		partial void btnThirdPosTapped (UIKit.UIButton sender)
		{
			int index = Array.IndexOf(availablePositions,btnThirdPos.TitleLabel.Text);
			if(index == 3)
			{
				btnFourthPos.SetTitle ("None", UIControlState.Normal);
				btnThirdPos.SetTitle ("None", UIControlState.Normal);
				this.PerformSelector(new ObjCRuntime.Selector("SetSubmitButtonStatus"),null,0.5);
				return;
			}

			if(btnSecondPos.TitleLabel.Text == "None")
			{
				btnThirdPos.TitleLabel.Text="None";
				

                UIAlertController okAlertController = UIAlertController.Create("You cannot set the 3rd or 4th position while the 2nd position is “NONE”.  Change the 2nd position first", GlobalSettings.SouthWestWifiMessage, UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);

                this.PerformSelector(new ObjCRuntime.Selector("SetSubmitButtonStatus"),null,0.5);
				return;
			}

			for(int i=0; i<5; i++)
			{
				if (index == 4)
					index = 0;
				else
					index++;

				if((btnFirstPos.TitleLabel.Text != availablePositions[index]) && (btnSecondPos.TitleLabel.Text != availablePositions[index]) && (btnThirdPos.TitleLabel.Text != availablePositions[index]))
				{
					btnThirdPos.SetTitle (availablePositions[index], UIControlState.Normal);
					break;
				}
			}
			if(btnFourthPos.TitleLabel.Text == availablePositions[index])
			{
				btnFourthPos.SetTitle ("None", UIControlState.Normal);
			}

			//SetRepeatPositionUI();
			//SetSubmitButtonStatus();
			this.PerformSelector(new ObjCRuntime.Selector("SetSubmitButtonStatus"),null,0.5);

		}
		partial void btnFourthPosTapped (UIKit.UIButton sender)
		{
			int index = Array.IndexOf(availablePositions,btnFourthPos.TitleLabel.Text);
			if(index == 3)
			{
				btnFourthPos.SetTitle ("None", UIControlState.Normal);
				return;
			}
			if(btnThirdPos.TitleLabel.Text == "None")
			{
				btnFourthPos.TitleLabel.Text="None";
				
                UIAlertController okAlertController = UIAlertController.Create("You cannot set the 4th position while the 3rd position is “NONE”.  Change the 3rd position first","", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);

                this.PerformSelector(new ObjCRuntime.Selector("SetSubmitButtonStatus"),null,0.5);
				return;
			}
			for(int i=0; i<5; i++)
			{
				if (index == 4)
					index = 0;
				else
					index++;

				if((btnFirstPos.TitleLabel.Text != availablePositions[index]) && (btnSecondPos.TitleLabel.Text != availablePositions[index]) && (btnThirdPos.TitleLabel.Text != availablePositions[index])  && (btnFourthPos.TitleLabel.Text != availablePositions[index]))
				{btnFourthPos.SetTitle (availablePositions[index], UIControlState.Normal);
					break;}

			}
			//SetRepeatPositionUI();
			//SetSubmitButtonStatus();
			this.PerformSelector(new ObjCRuntime.Selector("SetSubmitButtonStatus"),null,0.5);

		}
		partial void btnChangeBuddyTapped (UIKit.UIButton sender)
		{
			ChangeBuddyViewController BuddyView = new ChangeBuddyViewController();
			BuddyView.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
			this.PresentViewController(BuddyView,true,null);
			//BuddyView.View.Superview.BackgroundColor = UIColor.Clear;
			//BuddyView.View.Frame = new RectangleF (0,130,540,250);
			//BuddyView.View.Layer.BorderWidth = 1;
			notif =  NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("addedBuddyBidder"),addedBuddyBidder);

		}
		public void addedBuddyBidder(NSNotification n)
		{
			string buddy1 = GlobalSettings.WBidINIContent.BuddyBids.Buddy1;
			string buddy2 = GlobalSettings.WBidINIContent.BuddyBids.Buddy2;

			if ((buddy1 != "0" || buddy2 != "0")&& btnReserveCheck.Selected==true)
			{
				//sender.Selected = false;
				btnReserveCheck.Selected=false;
				IsAddReserveLine = false;
				

                UIAlertController okAlertController = UIAlertController.Create("You cannot add Reserve to the end of your Bid when Buddy Bidding", "", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);

            }
			this.setBuddybidLabel ();
			NSNotificationCenter.DefaultCenter.RemoveObserver (notif);
		}

		partial void btnReserveCheckTapped (UIKit.UIButton sender)
		{
			sender.Selected = !sender.Selected;
			string buddy1 = GlobalSettings.WBidINIContent.BuddyBids.Buddy1;
			string buddy2 = GlobalSettings.WBidINIContent.BuddyBids.Buddy2;

			if ((buddy1 != "0" || buddy2 != "0")&& sender.Selected==true)
			{
				sender.Selected = false;
				IsAddReserveLine = false;
				
                UIAlertController okAlertController = UIAlertController.Create("You cannot add Reserve to the end of your Bid when Buddy Bidding", "", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);

            }
			else
			{

			IsAddReserveLine = sender.Selected;
			}
		}


		private void SubmitBidopearation ()
		{
			//Repeat line and Repeat postion
			if (sgPosCombination.SelectedSegment == 0 || sgPosCombination.SelectedSegment == 1) {
				queryViewController queryView = new queryViewController ();
				queryView.isFirstTime = true;
				queryView.isFromView = queryViewController.queryFromView.querySubmitBid;
				queryView.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				this.NavigationController.PushViewController (queryView, true);
				//                this.PresentViewController(queryView, true, null);
			}
			//custom
			else
				if (sgPosCombination.SelectedSegment == 2) {
					vwCustomBid.Hidden = false;
					//if (sgCustomOrder.SelectedSegment == 0)
					//    AvaialbleLineList = GlobalSettings.Lines.Select(x => x.LineNum).ToList();
					//else
					//    AvaialbleLineList = GlobalSettings.Lines.OrderBy(x => x.LineNum).Select(x => x.LineNum).ToList();
					//bid order
					if (sgCustomOrder.SelectedSegment == 0) {
						if (GlobalSettings.SubmitBid.Buddy1 != null || GlobalSettings.SubmitBid.Buddy2 != null) {
							Lines = GlobalSettings.Lines.Where (x => !x.FAPositions.Contains ("D")).ToList ();
						}
						else {
							Lines = GlobalSettings.Lines.ToList ();
						}
					}
					// Line order
					else {
						if (GlobalSettings.SubmitBid.Buddy1 != null || GlobalSettings.SubmitBid.Buddy2 != null) {
							Lines = GlobalSettings.Lines.Where (x => !x.FAPositions.Contains ("D")).OrderBy (x => x.LineNum).ToList ();
						}
						else {
							Lines = GlobalSettings.Lines.OrderBy (x => x.LineNum).ToList ();
						}
					}
					SetAvailalbeLines ();
					tblAvailable.ReloadData ();
					tblBid.ReloadData ();
				}
				else {
				}
		}

        partial void btnSubmitTapped(UIKit.UIButton sender)
        {
            SetSubmitBid();
            
        }
        private void SetAvailalbeLines()
        {
            List<string> itemstoadd = new List<string>();
            AvaialbleLineList = new List<string>();
            List<string> postionlist=GetpositionList(false);
            foreach (var line in Lines)
            {
                foreach (string position in postionlist)
                {
                    if (line.FAPositions.Contains(position))
                    {
                        itemstoadd.Add(line.LineNum + position);
                    }
                }
            }
            itemstoadd.ForEach(AvaialbleLineList.Add);
            AvailalbelistOrder = new ObservableCollection<string>();
            itemstoadd.ForEach(AvailalbelistOrder.Add);

        }

		public void updateAvailbleCount ()
		{
			lblLinesCount.Text = AvaialbleLineList.Count.ToString() + "Lines";
		}
		public void updateBidCount ()
		{
			lblBidCount.Text = BidLineList.Count.ToString() + "Bids";
		}

		public void updateOkButtonStatus(bool st)
		{
			btnCustomOK.Enabled = st;
		}

		partial void btnCancelTapped (UIKit.UIButton sender)
		{
			this.DismissViewController(true,null);
		}
		partial void btnAddTapped (UIKit.UIButton sender)
		{
			AddBidlinesCommand();
			ResetStates ();
			tblAvailable.ReloadData();
			tblBid.ReloadData();
		}

		void ResetStates ()
		{
			selectedFromAvailableList.Clear ();
			SelectedBidLineList.Clear ();
			if (BidLineList.Count == 0)
				btnClear.Enabled = false;
			else
				btnClear.Enabled = true;
			btnShift.Selected = false;
			btnCtrl.Selected = false;
			setShiftStatus (false);
			setCntrlStatus (false);
		}
        
		partial void btnInsertTapped (UIKit.UIButton sender)
		{
            InsertBidLineCommand();
			ResetStates ();
            tblAvailable.ReloadData();
            tblBid.ReloadData();
		}
		partial void btnRemoveTapped (UIKit.UIButton sender)
		{
            RemoveBidlinesCommand();
			ResetStates ();
            tblAvailable.ReloadData();
            tblBid.ReloadData();
		}
		partial void btnClearTapped (UIKit.UIButton sender)
		{
            UIAlertController alert = UIAlertController.Create("Are you sure you want to clear are bid choices?","", UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("No", UIAlertActionStyle.Cancel, (actionCancel) => {

            }));

            alert.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, (actionOK) => {
                BidLineList.Clear();
                SetAvailalbeLines();
                ResetStates();
                tblAvailable.ReloadData();
                tblBid.ReloadData();
            }));

            this.PresentViewController(alert, true, null);
        }

		partial void btnCustomOkTapped (UIKit.UIButton sender)
		{

            GlobalSettings.SubmitBid.Bid = GenarateBidLineString();

            GlobalSettings.SubmitBid.TotalBidChoices = BidLineList.Count();
            if (IsAddReserveLine)
            {

                GlobalSettings.SubmitBid.Bid += string.IsNullOrEmpty(GlobalSettings.SubmitBid.Bid) ? string.Empty : ",";
                GlobalSettings.SubmitBid.Bid += "R";
                 
                 GlobalSettings.SubmitBid.TotalBidChoices += 1;
            }

            //GlobalSettings.SubmitBid.BuddyBidderBids = new List<BuddyBidderBid>();
            //if (CustomBidSubmitDetails.SubmitBiddetails.Buddy1 != null)
            //{
            //    CustomBidSubmitDetails.SubmitBiddetails.BuddyBidderBids.Add(new BuddyBidderBid() { BuddyBidder = CustomBidSubmitDetails.SubmitBiddetails.Buddy1, BidLines = GenarateBuddyBidBidLineString(SelectedBuddyBidder1Postion) });
            //}

            //if (CustomBidSubmitDetails.SubmitBiddetails.Buddy2 != null)
            //{
            //    CustomBidSubmitDetails.SubmitBiddetails.BuddyBidderBids.Add(new BuddyBidderBid() { BuddyBidder = CustomBidSubmitDetails.SubmitBiddetails.Buddy2, BidLines = GenarateBuddyBidBidLineString(SelectedBuddyBidder2Postion) });
            //}

//			queryViewController queryView = new queryViewController();
//			queryView.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
//			this.PresentViewController(queryView,true,null);
			queryViewController queryView = new queryViewController();
			queryView.isFirstTime=true;
			queryView.isFromView = queryViewController.queryFromView.querySubmitBid;
			queryView.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
			this.NavigationController.PushViewController(queryView,true);
		}

        /// <summary>
        /// Add selected "avaialble lines" to the the "Bid line" list
        /// </summary>
        /// <param name="obj"></param>
        private void AddBidlinesCommand()
        {
            try
            {

                if (selectedFromAvailableList.Count > 0)
                {
                    List<string> itemstoadd = new List<string>();
                    BidLineList = BidLineList ?? new List<string>();
                    foreach (string line in selectedFromAvailableList)
                    {

                        BidLineList.Add(line);
                        AvaialbleLineList.Remove(line);
                    }
                    ////////////////////
                    SelectedBidLines = BidLineList.FirstOrDefault(x => x == selectedFromAvailableList.LastOrDefault());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Insert the lines to the Bid line list in a specfied position  selected by the user
        /// </summary>
        /// <param name="obj"></param>
        private void InsertBidLineCommand()
        {
            try
            {
                int index = 0;
                if (BidLineList != null && BidLineList.Count > 0)
                {
                    if (SelectedBidLines != null)
                    {
                        index = BidLineList.IndexOf(SelectedBidLines);
                        index = (index < 0) ? 0 : index;
                    }
                }

                List<string> bidlines = new List<string>();
                //if (BidChoice != string.Empty)
                if (selectedFromAvailableList.Count > 0)
                {
                    BidLineList = BidLineList ?? new List<string>();
                    foreach (string line in selectedFromAvailableList)
                    {
                        bidlines.Add(line);
                        AvaialbleLineList.Remove(line);
                    }
                }
                BidLineList.InsertRange(index, bidlines);

                ////
                SelectedBidLines = BidLineList.FirstOrDefault(x => x == selectedFromAvailableList.LastOrDefault());

            }
            catch (Exception ex)
            {
                throw ex;
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
        private void AddBidLineToAvaialableLines(string column)
        {
            //get the line number
            if (!AvaialbleLineList.Contains(column))
            {
                //insert into the avaialbe line list
                AvaialbleLineList.Add(column);
                AvaialbleLineList = new List<string>(AvaialbleLineList.OrderBy(x => AvailalbelistOrder.IndexOf(x)).ToList());
                AvaialbleLineList = new List<string>(AvaialbleLineList);
            }
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
		partial void btnCustomCancelTapped (UIKit.UIButton sender)
		{
			vwCustomBid.Hidden = true;
		}
		partial void btnCtrlTapped (UIKit.UIButton sender)
		{
			if (!btnCtrl.Selected)
				setCntrlStatus(true);
			else
				setCntrlStatus(false);
			setShiftStatus(false);

		}
		partial void btnShiftTapped (UIKit.UIButton sender)
		{
			if (!btnShift.Selected)
				setShiftStatus(true);
			else
				setShiftStatus(false);
			setCntrlStatus(false);

		}
		private void setCntrlStatus(bool n)
		{
			btnCtrl.Selected = n;
			if (n)
			{
				btnCtrl.BackgroundColor = UIColor.Cyan;
				btnCtrl.Layer.BorderWidth = 0.0f;
				btnCtrl.Selected = true;

			}
			else
			{
				btnCtrl.BackgroundColor = UIColor.White;
				btnCtrl.Layer.BorderWidth = 1.0f;
				btnCtrl.Selected = false;
			}
		}
		private void setShiftStatus(bool n)
		{
			btnShift.Selected = n;
			if (n)
			{
				btnShift.BackgroundColor = UIColor.Cyan;
				btnShift.Layer.BorderWidth = 0.0f;
				btnShift.Selected = true;
			}
			else
			{
				btnShift.BackgroundColor = UIColor.White;
				btnShift.Layer.BorderWidth = 1.0f;
				btnShift.Selected = false;

			}
		}
		public void availableBidSelectedWithIndexPath(NSIndexPath path)
		{
			string selectedLineNumber = AvaialbleLineList [path.Row];

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
				if (selectedFromAvailableList.Count > 1) {
					selectedFromAvailableList.Clear ();
					selectedFromAvailableList.Add (selectedLineNumber);
				} else if (selectedFromAvailableList.Count == 1) {
					//TODO


					int currentSelected = AvaialbleLineList.IndexOf(selectedFromAvailableList [0]);
					int till = path.Row;
					selectedFromAvailableList.Clear ();

					if (currentSelected > path.Row) {
						for (int i = path.Row; i <= currentSelected; i++) {
							string selecteLN = AvaialbleLineList [i];
							selectedFromAvailableList.Add (selecteLN);
						}
					} else {
						for (int i = currentSelected; i <= path.Row; i++) {
							string selecteLN = AvaialbleLineList [i];
							selectedFromAvailableList.Add (selecteLN);
						}
					}

				} else if(selectedFromAvailableList.Count == 0){
					selectedFromAvailableList.Add(selectedLineNumber);
				}
			}
			tblAvailable.ReloadData();
		}





        private void SetSubmitBid()
		{

			SubmitBid submitBid = GlobalSettings.SubmitBid;
			try {

				if (lblBuddy1.Text == string.Empty && lblBuddy2.Text == string.Empty) {
					submitBid.Buddy1 = null;
					submitBid.Buddy2 = null;
				} else {

					BuddyBids buddyBids = GlobalSettings.WBidINIContent.BuddyBids;
					if (IsAddReserveLine) {
						submitBid.Buddy1 = submitBid.Buddy2 = null;
                       
					} else {
						submitBid.Buddy1 = (buddyBids.Buddy1 == "0") ? null : buddyBids.Buddy1;
						submitBid.Buddy2 = (buddyBids.Buddy2 == "0") ? null : buddyBids.Buddy2;
					}
					//Genarate and store buddy bid line string to the BuddyBidderBids list
					submitBid.BuddyBidderBids = new List<BuddyBidderBid> ();
					//buddy bidder1
					if (submitBid.Buddy1 != null) {
						if (SelectedBuddyBidder1Postion != string.Empty) {
							var buddybidline = GenarateBidLineStringForBuddyBidder (SelectedBuddyBidder1Postion);
							submitBid.BuddyBidderBids.Add (new BuddyBidderBid {
								BidLines = buddybidline,
								BuddyBidder = submitBid.Buddy1
							});
						}
					}
					//buddy bidder 2
					if (submitBid.Buddy2 != null) {
						if (SelectedBuddyBidder2Postion != string.Empty) {
							var buddybidline = GenarateBidLineStringForBuddyBidder (SelectedBuddyBidder2Postion);
							submitBid.BuddyBidderBids.Add (new BuddyBidderBid {
								BidLines = buddybidline,
								BuddyBidder = submitBid.Buddy2
							});
						}
					}
				}

				//genarate bid line to submit
				submitBid.Bid = GenarateBidLineString (false);
				submitBid.TotalBidChoices = BidLineList.Count;
				submitBid.Bidder = GlobalSettings.TemporaryEmployeeNumber;

				if (IsAddReserveLine) {
					submitBid.Bid += string.IsNullOrEmpty (submitBid.Bid) ? string.Empty : ",";
					submitBid.Bid += "R";
					submitBid.TotalBidChoices++;
				}


			} catch (Exception ex) {
				throw ex;
                
			}

			if (GlobalSettings.SubmitBid.IsSubmitAllChoices == false && sgPosCombination.SelectedSegment == 1)
			{
				APositionLineCount = Convert.ToInt32(txtLinesInFirstPos.Text);
				BPositionLineCount = Convert.ToInt32(txtLinesInSecondPos.Text);
				CPositionLineCount = Convert.ToInt32(txtLinesInThirdPos.Text);
				DPositionLineCount = Convert.ToInt32(txtLinesInFourthPos.Text);
				if (GlobalSettings.SubmitBid.SeniorityNumber != Convert.ToInt32(lblPositionTotal.Text))
				{
					//Xceed.Wpf.Toolkit.MessageBox.Show("You have requested to submit  " + GlobalSettings.SubmitBid.SeniorityNumber + " bid choices, but your individual bid choices for each position do not equal " + GlobalSettings.SubmitBid.SeniorityNumber + " - go back and adjust your individual position numbers", "WBidMax", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

					

                    UIAlertController okAlertController = UIAlertController.Create("You have requested to submit  " + GlobalSettings.SubmitBid.SeniorityNumber + " bid choices, but your individual bid choices for each position do not equal " + GlobalSettings.SubmitBid.SeniorityNumber + " - go back and adjust your individual position numbers", "", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                    return;
				}

				if (APositionLineCount> Convert.ToInt32(lblPosExistValue1.Text==string.Empty?"0":lblPosExistValue1.Text))
				{
					

                    UIAlertController okAlertController = UIAlertController.Create("You cannot submit " + APositionLineCount + " positions. since only " + lblPosExistValue1.Text + " " + lblSelectedFirstPos.Text + " exist. \n\nChange your number of " + lblSelectedFirstPos.Text + " positions to " + lblPosExistValue1.Text + " or less", "", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);

                    return;
				}
				if (BPositionLineCount>Convert.ToInt32(lblPosExistValue2.Text==string.Empty?"0":lblPosExistValue2.Text))
				{
					

                    UIAlertController okAlertController = UIAlertController.Create("You cannot submit " + BPositionLineCount + " positions. since only " + lblPosExistValue2.Text + " " + lblSelectedSecondPos.Text + " exist. \n\nChange your number of " + lblSelectedSecondPos.Text + " positions to " + lblPosExistValue2.Text + " or less", "", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                    return;
				}
				if (CPositionLineCount >Convert.ToInt32(lblPosExistValue3.Text==string.Empty?"0":lblPosExistValue3.Text))
				{
					

                    UIAlertController okAlertController = UIAlertController.Create("You cannot submit " + CPositionLineCount + " positions. since only " + lblPosExistValue3.Text + " " + lblSelectedThirdPos.Text + " exist. \n\nChange your number of " + lblSelectedThirdPos.Text + " positions to " + lblPosExistValue3.Text + " or less", "", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                    return;
				}
				if (DPositionLineCount> Convert.ToInt32(lblPosExistValue4.Text==string.Empty?"0":lblPosExistValue4.Text))
				{

					

                    UIAlertController okAlertController = UIAlertController.Create("You cannot submit " + DPositionLineCount + " positions. since only " + lblPosExistValue4.Text + " " + lblSelectedFourthPos.Text + " exist. \n\nChange your number of " + lblSelectedFourthPos.Text + " positions to " + lblPosExistValue4.Text + " or less", "", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                    return;
				}
			}
			//int dPositionCount = Lines.SelectMany(x => x.FAPositions).Where(y => y == "D").Count();
			int dPositionCount = GlobalSettings.Lines.Where (z => z.FAPositions.Count == 1).SelectMany (x => x.FAPositions).Where (y => y == "D").Count ();

			bool isNeedtoshownextscreen = true;
			if ((submitBid.Buddy1 != null || submitBid.Buddy2 != null) && dPositionCount > 0) {
				isNeedtoshownextscreen = false;
				

                UIAlertController alert = UIAlertController.Create(dPositionCount + " D Lines  were deleted from the bid. \nBuddy Bid Lines can have A, B or C positions. \nPress OK to continue or CANCEL to return the position Choices. ","", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (actionCancel) => {
                    return;
                }));

                alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {
                    SubmitBidopearation();
                }));

                this.PresentViewController(alert, true, null);

            }

			if (isNeedtoshownextscreen)
				SubmitBidopearation ();
			//if (IsCustomOrder)
			//{
			//    CustomBidSubmitDetails = new Models.CustomBidSubmitDetails();
			//    CustomBidSubmitDetails.IsBidOder = IsBidOrder;
			//    CustomBidSubmitDetails.SeniorityNumber = SubmitBidDetails.SeniorityNumber;
			//    CustomBidSubmitDetails.Postions = GetpositionList(false);
			//    CustomBidSubmitDetails.SubmitBiddetails = SubmitBidDetails;
			//    CustomBidSubmitDetails.IsAddReserveLine = IsAddReserveLine;
			//    Messenger.Default.Send<CustomBidSubmitDetails>(CustomBidSubmitDetails, WBidMessages.FaPostionVM_Notofication_ShowCustomWindow);
			//}
			//else
			//{
			//    //  SubmitBidDetails.SeniorityNumber = BidLineList.Count();
			//    SubmitBidDetails.SeniorityNumber += (IsAddReserveLine) ? 1 : 0;
			//    Messenger.Default.Send<SubmitBid>(SubmitBidDetails, WBidMessages.FaPostionVM_Notofication_ShowBidQueryWindow);
			//}

		}



        private string GenarateBidLineStringForBuddyBidder(string postion)
        {
            _buddyposition1 = string.Empty;
            _buddyposition2 = string.Empty;
            _buddyposition3 = string.Empty;
            char[] positionchar = postion.ToCharArray();
            if (positionchar.Length == 3)
            {
                _buddyposition1 = positionchar[0].ToString();
                _buddyposition2 = positionchar[1].ToString();
                _buddyposition3 = positionchar[2].ToString();
            }
            else if (positionchar.Length == 2)
            {
                _buddyposition1 = positionchar[0].ToString();
                _buddyposition2 = positionchar[1].ToString();
            }
            else
            {
                _buddyposition1 = positionchar[0].ToString();
            }
            string bidline = GenarateBidLineString(true);
            return bidline;
        }


        private string GenarateBidLineString(bool isBuddyBid)
        {
            string bidlines = string.Empty;

			List<int> linesToSubmit;
			if (GlobalSettings.SubmitBid.Buddy1 != null || GlobalSettings.SubmitBid.Buddy2 != null)
			{
				linesToSubmit = GlobalSettings.Lines.Where(x => !x.FAPositions.Contains("D")).Select(x => x.LineNum).ToList();
			}
			else
			{
				linesToSubmit = GlobalSettings.Lines.Select(x => x.LineNum).ToList();
			}
           // var linesToSubmit = GlobalSettings.Lines.Select(x => x.LineNum);
           BidLineList = new List<string>();
           int seniorityCount = GlobalSettings.SubmitBid.TotalBidChoices;
            //Repeat line
            if (sgPosCombination.SelectedSegment==0)
            {
                List<string> itemstoadd = null;
                foreach (int lineNumber in linesToSubmit)
                {
                    itemstoadd = new List<string>();
                    itemstoadd.AddRange(GetBidLinelistforPositions(lineNumber, isBuddyBid));

                    foreach (var item in itemstoadd)
                    {
                        BidLineList.Add(item);
                        seniorityCount--;
                        if ( !GlobalSettings.SubmitBid.IsSubmitAllChoices  && seniorityCount == 0)
                        {
                            break;
                        }
                    }
                    if ( !GlobalSettings.SubmitBid.IsSubmitAllChoices  && seniorityCount == 0)
                    {
                        break;
                    }
                }
                // itemstoadd.ForEach(BidLineList.Add);
            }         // IsRepeatPositions
            else if (sgPosCombination.SelectedSegment == 1)
            {
				if (GlobalSettings.SubmitBid.IsSubmitAllChoices)
				{

					var positions = GetpositionList(isBuddyBid);
					foreach (string position in positions)
					{
						foreach (int lineNumber in linesToSubmit)
						{
							List<string> fapositions = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == lineNumber).FAPositions;
							if (fapositions.Contains(position))
							{
								if (!GlobalSettings.SubmitBid.IsSubmitAllChoices && seniorityCount == 0)
								{
									break;
								}
								seniorityCount--;
								BidLineList.Add(lineNumber + position);
							}

						}

						if (!GlobalSettings.SubmitBid.IsSubmitAllChoices && seniorityCount == 0)
						{
							break;
						}
					}
				}
				else
				{
					AddPositionLines(btnFirstPos.TitleLabel.Text,Convert.ToInt32(txtLinesInFirstPos.Text));
					AddPositionLines(btnSecondPos.TitleLabel.Text, Convert.ToInt32(txtLinesInSecondPos.Text));
					AddPositionLines(btnThirdPos.TitleLabel.Text, Convert.ToInt32(txtLinesInThirdPos.Text));
					AddPositionLines(btnFourthPos.TitleLabel.Text, Convert.ToInt32(txtLinesInFourthPos.Text));

				}

            }
                //Custom
            else if (sgPosCombination.SelectedSegment == 2)
            {

            }
            bidlines = string.Join(",", BidLineList.Select(x => x.ToString()));
            return bidlines;
        }

		private void AddPositionLines(string position, int positionCount)
		{
			List<Line> lines = new List<Line>();
			if (GlobalSettings.SubmitBid.Buddy1 != null || GlobalSettings.SubmitBid.Buddy2 != null)
			{
				lines = GlobalSettings.Lines.Where(x => !x.FAPositions.Contains("D")).ToList();
			}
			else
			{
				lines = GlobalSettings.Lines.ToList();
			}
			foreach (var item in lines)
			{
				if (item.FAPositions.Contains(position))
				{
					positionCount--;
					if (positionCount < 0)
						break;
					BidLineList.Add(item.LineNum + position);

				}



			}
		}

        private List<string> GetBidLinelistforPositions(int line, bool isBuddyBid)
        {

            List<string> fapositions = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == line).FAPositions;

            List<string> itemstoadd = new List<string>();
            if (fapositions != null)
            {
                if (isBuddyBid)
                {
                    if (_buddyposition1 != string.Empty && fapositions.Contains(btnFirstPos.TitleLabel.Text))
                        itemstoadd.Add(line + _buddyposition1);
                    if (_buddyposition2 != string.Empty && fapositions.Contains(btnSecondPos.TitleLabel.Text))
                        itemstoadd.Add(line + _buddyposition2);
                    if (_buddyposition3 != string.Empty && fapositions.Contains(btnThirdPos.TitleLabel.Text))
                        itemstoadd.Add(line + _buddyposition3);

                }
                else
                {
                    if (btnFirstPos.TitleLabel.Text != "[None]" && fapositions.Contains(btnFirstPos.TitleLabel.Text))
                        itemstoadd.Add(line + btnFirstPos.TitleLabel.Text);
                    if (btnSecondPos.TitleLabel.Text != "[None]" && fapositions.Contains(btnSecondPos.TitleLabel.Text))
                        itemstoadd.Add(line + btnSecondPos.TitleLabel.Text);
                    if (btnThirdPos.TitleLabel.Text != "[None]" && fapositions.Contains(btnThirdPos.TitleLabel.Text))
                        itemstoadd.Add(line + btnThirdPos.TitleLabel.Text);
                    if (btnFourthPos.TitleLabel.Text != "[None]" && fapositions.Contains(btnFourthPos.TitleLabel.Text))
                        itemstoadd.Add(line + btnFourthPos.TitleLabel.Text);
                }
            }
            else
            {
                //needs to remove
                //MessageBox.Show("FA Position is Null.Please Reparse :Temporary Message");
            }
            return itemstoadd;

        }


        private List<string> GetpositionList(bool isBuddyBid)
        {
            List<string> positions = new List<string>();
            if (isBuddyBid)
            {
                if (_buddyposition1 != string.Empty)
                    positions.Add(_buddyposition1);
                if (_buddyposition2 != string.Empty)
                    positions.Add(_buddyposition2);
                if (_buddyposition3 != string.Empty)
                    positions.Add(_buddyposition3);
            }
            else
            {

                if (btnFirstPos.TitleLabel.Text != "[None]")
                    positions.Add(btnFirstPos.TitleLabel.Text);
                if (btnSecondPos.TitleLabel.Text != "[None]")
                    positions.Add(btnSecondPos.TitleLabel.Text);
                if (btnThirdPos.TitleLabel.Text != "[None]")
                    positions.Add(btnThirdPos.TitleLabel.Text);
                if (btnFourthPos.TitleLabel.Text != "[None]")
                    positions.Add(btnFourthPos.TitleLabel.Text);
            }
            return positions;

        }
		public void bidSelectedWithIndexPath(NSIndexPath path)
		{
			//			string selectedLineNumber = BidLineList[path.Row].ToString();



			string selectedLineNumber = BidLineList [path.Row].ToString ();

			if (!btnCtrl.Selected && !btnShift.Selected) {
				//Simply select the new one and deselect all old ones
				if (!SelectedBidLineList.Contains (selectedLineNumber)) {
					SelectedBidLineList.Clear ();
					SelectedBidLineList.Add (selectedLineNumber);
				} else
					SelectedBidLineList.Remove (selectedLineNumber);

			}

			if (btnCtrl.Selected) {
				//Add this to selected list, no need to remove anything
				if (!SelectedBidLineList.Contains (selectedLineNumber)) {
					SelectedBidLineList.Add (selectedLineNumber);
				} else
					SelectedBidLineList.Remove (selectedLineNumber);

			}

			if (btnShift.Selected) {
				//if more than one already selected, remove all and add this to selected list. if only one selected, then select everything in between old and new.
				if (SelectedBidLineList.Count > 1) {
					SelectedBidLineList.Clear ();
					SelectedBidLineList.Add (selectedLineNumber);
				} else if (SelectedBidLineList.Count == 1) {
					//TODO



					int currentSelected = BidLineList.IndexOf (SelectedBidLineList [0]);
					int till = path.Row;
					SelectedBidLineList.Clear ();

					if (currentSelected > path.Row) {
						for (int i = path.Row; i <= currentSelected; i++) {
							string selecteLN = BidLineList [i];
							SelectedBidLineList.Add (selecteLN);
						}
					} else {
						for (int i = currentSelected; i <= path.Row; i++) {
							string selecteLN = BidLineList [i];
							SelectedBidLineList.Add (selecteLN);
						}
					}

				} else if (SelectedBidLineList.Count == 0) {
					SelectedBidLineList.Add (selectedLineNumber);
				}
			}
			SelectedBidLines = selectedLineNumber;
			tblBid.ReloadData ();
		}


		// Available Lines Table data source
		public class availableBidSource : UITableViewSource
		{
			FAPostionChoiceViewController parentVC;
			public availableBidSource(FAPostionChoiceViewController parent)
			{
				parentVC = parent;
			}

			public override nint NumberOfSections(UITableView tableView)
			{
				return 1;
			}
			public override nint RowsInSection (UITableView tableview, nint section)
			{
				parentVC.updateAvailbleCount ();
				return parentVC.AvaialbleLineList.Count;
			}
			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				NSString cellIdentifier = new NSString ("cell1");
				UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier) as UITableViewCell;
				if (cell == null)
					cell = new UITableViewCell ();
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
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				parentVC.availableBidSelectedWithIndexPath(indexPath);
			}

		}

		// Selected Lines table data source
		public class selectedBidsSource : UITableViewSource
		{
			FAPostionChoiceViewController parentVC;
			public selectedBidsSource(FAPostionChoiceViewController parent)
			{
				parentVC = parent;
			}

			public override nint NumberOfSections(UITableView tableView)
			{
				return 1;
			}
			public override nint RowsInSection (UITableView tableview, nint section)
			{
				parentVC.updateBidCount ();
				parentVC.updateOkButtonStatus (parentVC.BidLineList.Count != 0);

				return parentVC.BidLineList.Count;
			}
			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				NSString cellIdentifier = new NSString ("cell2");
				UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier) as UITableViewCell;
				if (cell == null)
					cell = new UITableViewCell ();

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
				return cell;
			}

			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				parentVC.bidSelectedWithIndexPath(indexPath);
			}

		}

		// Pickerview Data source
		public class bidderPositionModel : UIPickerViewModel
		{
			FAPostionChoiceViewController parent;
			public bidderPositionModel(FAPostionChoiceViewController parentVC)
			{
				parent = parentVC;
			}

			public override nint GetComponentCount (UIPickerView picker)
			{
				return 1;
			}

			public override nint GetRowsInComponent (UIPickerView picker, nint component)
			{
				return parent.availablePositionSelections.Length;
			}

			public override string GetTitle (UIPickerView picker, nint row, nint component)
			{
				return parent.availablePositionSelections [row];
			}
			public override void Selected (UIPickerView picker, nint row, nint component)
			{
				if (picker.Tag == 0)
					parent.SelectedBuddyBidder1Postion = parent.availablePositionSelections [row];
				else
					parent.SelectedBuddyBidder2Postion = parent.availablePositionSelections [row];
			}
		}

	}
}

