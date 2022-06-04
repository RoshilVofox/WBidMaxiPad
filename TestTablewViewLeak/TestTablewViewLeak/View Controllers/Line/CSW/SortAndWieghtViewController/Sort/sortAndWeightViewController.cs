using System;
using CoreGraphics;
using Foundation;
using UIKit;
using CoreGraphics;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using System.Linq;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using System.Collections.Generic;
using WBid.WBidiPad.iOS.Utility;
using System.Collections.ObjectModel;
using WBid.WBidiPad.PortableLibrary;
using System.Json;

namespace WBid.WBidiPad.iOS
{
    public partial class sortAndWeightViewController : UIViewController, IServiceDelegate
	{
		WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		UIPopoverController popoverController;
		blockSortTableControllerController tblSortList;
		WeightsTableController weightsController;
		List <NSObject> arrObserver = new List <NSObject> ();
		WeightCalculation weightCalc = new WeightCalculation();
		private NSObject _DayOfcellNotification;
		UIPopoverController popover ;
        
        string servicecURL = string.Empty;
		class MyPopDelegate : UIPopoverControllerDelegate
		{
			sortAndWeightViewController _parent;
			public MyPopDelegate (sortAndWeightViewController parent)
			{
				_parent = parent;
			}
			public override void DidDismiss (UIPopoverController popoverController)
			{
				_parent.popoverController = null;

			}
		}

		public sortAndWeightViewController () : base ("sortAndWeightViewController", null)
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
			observeNotifs ();
			//this.tlbSort.SetBackgroundImage (UIImage.FromBundle ("headerBottomGray"), UIToolbarPosition.Any, UIBarMetrics.Default);
			btnPPFDP.Enabled = false;
			this.btnPlus2.Hidden = true;
			if (CommonClass.weightSelected) {
				this.sgSortnWeights.SelectedSegment = 1;
				this.toggleSortWeight ();
			}

			this.setGraphics ();
			setValuesToFixedSorts ();
			loadSortTable ();
//			observeNotifs ();
			listSavedWeights ();
			vwSort.BackgroundColor = ColorClass.ListSeparatorColor;
		}
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

		}
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		

		}

		private void observeNotifs ()
		{
			arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("reloadBlockSort"), reloadSortList));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString("AddWeights"), handleWeightsReload));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("dismissWtPopover"),dismissPopover));
		arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ShowRatioScreenSort"), ShowRatioScreenSort));
		arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ClosingRatioScreenSort"), ClosingRatioScreenSort));
		}

		void ClosingRatioScreenSort(NSNotification obj)
		{
			var type = obj.Object.ToString();
			if (type == "OK")
			{
				AddRatioBlockSort();
			}
			else
			{
			}
		}
		private void AddRatioBlockSort()
		{
			WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			WBidHelper.PushToUndoStack();
			ObservableCollection<BlockSort> lstblockData = WBidCollection.GetBlockSortListDataCSW();
			var value = lstblockData.FirstOrDefault(x => x.Name == "Ratio");

			if (!wBIdStateContent.SortDetails.BlokSort.Contains(value.Id.ToString()))
			{
				wBIdStateContent.SortDetails.BlokSort.Add(value.Id.ToString());
			}
			NSNotificationCenter.DefaultCenter.PostNotificationName("reloadBlockSort", null);
			NSNotificationCenter.DefaultCenter.PostNotificationName("dismissWtPopover", null);
		}
		void ShowRatioScreenSort(NSNotification obj)
		{
			
			popoverController.Dismiss(true);
			if (IsRatioPropertiesSetFromOtherViews())
			{
				AddRatioBlockSort();
			}
			else
			{
				this.PerformSelector(new ObjCRuntime.Selector("ViewShow"), null, 0.5);
			}



		}
		private bool IsRatioPropertiesSetFromOtherViews()
		{
			WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			bool isNormalMode = !(GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM);

			if (isNormalMode)
			{
				if (GlobalSettings.WBidINIContent.BidLineNormalColumns.Any(x => x == 75) ||
				GlobalSettings.WBidINIContent.ModernNormalColumns.Any(x => x == 75) ||
				 GlobalSettings.WBidINIContent.DataColumns.Any(x => x.Id == 75) ||
					wBIdStateContent.SortDetails.BlokSort.Contains("19")
				//wbidSta
				)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				if (
				GlobalSettings.WBidINIContent.BidLineVacationColumns.Any(x => x == 75) ||
				GlobalSettings.WBidINIContent.ModernVacationColumns.Any(x => x == 75) ||
				 GlobalSettings.WBidINIContent.SummaryVacationColumns.Any(x => x.Id == 75) ||
					wBIdStateContent.SortDetails.BlokSort.Contains("19")

				)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		[Export("ViewShow")]
		void ViewShow()
		{
			Console.WriteLine("Success");
			UIStoryboard storyboard = UIStoryboard.FromName("Main", null);
			RatioViewController ratioView = storyboard.InstantiateViewController("RatioViewController") as RatioViewController;
			ratioView.isFromLineViewController = false;
			ratioView.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;

			//this.PresentViewController(cmtView, true,null);
			if (this.NavigationController == null)
			{
			}

			this.NavigationController.PresentViewController(ratioView, true, null);
		}
		public void reloadSortList(NSNotification n)
		{
			tblSortList.TableView.ReloadData ();
			if (wBidStateContent.SortDetails.BlokSort.Count != 0) {
				btnBlockSort.Enabled = true;
				btnBlockSort.Selected = true;
				//buttonColor (btnBlockSort);
				ApplySort ("Block Sort");
				wBidStateContent.SortDetails.SortColumn = "BlockSort";
			} else {
				btnBlockSort.Enabled = false;
				btnBlockSort.Selected = false;
				//buttonColor (btnBlockSort);
				btnLineNum.Selected = true;
				//buttonColor (btnLineNum);
				ApplySort ("Line Number");
				wBidStateContent.SortDetails.SortColumn = "Line";

				this.btnBlankToBottom.Selected = false;
				this.btnReserveToBottom.Selected = false;
				//this.buttonColor (btnBlankToBottom);
				//this.buttonColor (btnReserveToBottom);
				this.btnBlankToBottom.Enabled = false;
				this.btnReserveToBottom.Enabled = false;
				wBidStateContent.ForceLine.IsBlankLinetoBottom = false;
				wBidStateContent.ForceLine.IsReverseLinetoBottom = false;

			}
			setValuesToFixedSorts ();

		}
		private void dismissPopover (NSNotification n)
		{
			if(popoverController!=null)
			popoverController.Dismiss (true);
		}
		private void handleWeightsReload (NSNotification n)
		{
			if (weightsController != null)
				weightsController.TableView.ReloadData ();
			GlobalSettings.isModified = true;
			CommonClass.cswVC.UpdateSaveButton ();
		}

		private void loadSortTable()
		{
			tblSortList = new blockSortTableControllerController ();
			this.AddChildViewController (tblSortList);
			vwSortList.AddSubview (tblSortList.View);
			tblSortList.View.Frame = vwSortList.Bounds;
			tblSortList.TableView.SetEditing (true, true);

			if (wBidStateContent.SortDetails.BlokSort.Count == 0) {
				this.btnBlockSort.Enabled = false;
			}
            if(GlobalSettings.CurrentBidDetails.Postion=="FA" && GlobalSettings.CurrentBidDetails.Round=="M")
            {
                btnSortByAward.Enabled = false;
                btnSortBySubmittedBid.Enabled = false;
            }
            else
            {
                btnSortByAward.Enabled = true;
                btnSortBySubmittedBid.Enabled = true;
            }

		}
		private void setGraphics ()
		{
//			this.tlbSort.Layer.BorderWidth = 1;
//			this.btnClear.Layer.BorderWidth = 1;
//			this.sgSortnWeights.Layer.BorderWidth = 1.4f;
//			this.sgSortnWeights.Layer.CornerRadius = 5;
//			UIImageView divImg = new UIImageView (new RectangleF (0, 0, 1, 29));
//			divImg.BackgroundColor = UIColor.Black;
//			divImg.Center = sgSortnWeights.Center;
//			this.View.AddSubview (divImg);
//			this.btnBlankToBottom.Layer.BorderWidth = 1;
//			this.btnReserveToBottom.Layer.BorderWidth = 1;
//			this.btnLineNum.Layer.BorderWidth = 1;
//			this.btnLinePay.Layer.BorderWidth = 1;
//			this.btnPPDay.Layer.BorderWidth = 1;
//			this.btnPPDutyHr.Layer.BorderWidth = 1;
//			this.btnPPFlightHr.Layer.BorderWidth = 1;
//			this.btnPPFDP.Layer.BorderWidth = 1;
//			this.btnPPTimeAway.Layer.BorderWidth = 1;
//			this.btnBlockSort.Layer.BorderWidth = 1;
			//this.btnSelColumn.Layer.BorderWidth = 1;
//			this.btnLines.Layer.BorderWidth = 1;
			//this.lblSelected.Layer.BorderWidth = 1;
			//this.lblManual.Layer.BorderWidth = 1;

			this.btnBlankToBottom.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnBlankToBottom.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			this.btnReserveToBottom.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnReserveToBottom.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			this.btnLineNum.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnLineNum.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			this.btnLinePay.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnLinePay.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			this.btnPPDay.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnPPDay.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			this.btnPPDutyHr.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnPPDutyHr.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			this.btnPPFlightHr.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnPPFlightHr.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			this.btnPPFDP.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnPPFDP.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			this.btnPPTimeAway.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnPPTimeAway.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			this.btnBlockSort.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnBlockSort.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
            this.btnSortByAward.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            this.btnSortByAward.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            this.btnSortBySubmittedBid.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            this.btnSortBySubmittedBid.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);

		}

//		public void buttonColor (UIButton btn)
//		{
//			if (btn.Selected)
//				btn.BackgroundColor = UIColor.FromRGB (150, 241, 250);
//			else
//				btn.BackgroundColor = UIColor.Clear;
//		}

		public void listSavedWeights () {
			WeightsApplied.clearAll ();
			if (wBidStateContent.CxWtState.AMPM.Wt&&WeightsApplied.AMPMWeights.Count==0) {
				foreach (Wt2Parameter para in wBidStateContent.Weights.AM_PM.lstParameters) {
					WeightsApplied.AMPMWeights.Add ("AM/PM");
				}
			}
			if (wBidStateContent.CxWtState.BDO.Wt&&WeightsApplied.BlocksOfDaysOffWeights.Count==0) {
				foreach (Wt3Parameter para in wBidStateContent.Weights.BDO.lstParameters) {
					WeightsApplied.BlocksOfDaysOffWeights.Add ("Blocks of Days Off");
				}
			}
			if (wBidStateContent.CxWtState.DHD.Wt&&WeightsApplied.CmutDHsWeights.Count==0) {
				foreach (Wt3Parameter para in wBidStateContent.Weights.DHD.lstParameters) {
					WeightsApplied.CmutDHsWeights.Add ("Cmut DHs");
				}
			}
			if (wBidStateContent.CxWtState.DHDFoL.Wt&&WeightsApplied.dhFirstLastWeights.Count==0) {
				foreach (Wt2Parameter para in wBidStateContent.Weights.DHDFoL.lstParameters) {
					WeightsApplied.dhFirstLastWeights.Add ("DH - first - last");
				}
			}
			if (wBidStateContent.CxWtState.DP.Wt&&WeightsApplied.DutyPeriodWeights.Count==0) {
				foreach (Wt3Parameter para in wBidStateContent.Weights.DP.lstParameters) {
					WeightsApplied.DutyPeriodWeights.Add ("Duty period");
				}
			}
			if (wBidStateContent.CxWtState.EQUIP.Wt&&WeightsApplied.EQTypeWeights.Count==0) {
				foreach (Wt3Parameter para in wBidStateContent.Weights.EQUIP.lstParameters) {
					WeightsApplied.EQTypeWeights.Add ("Equipment Type");
				}
			}
			if (wBidStateContent.CxWtState.ETOPS.Wt && WeightsApplied.ETOPSWeights.Count == 0)
			{
				foreach (Wt1Parameter para in wBidStateContent.Weights.ETOPS.lstParameters)
				{
					WeightsApplied.ETOPSWeights.Add("ETOPS");
				}
			}
			if (wBidStateContent.CxWtState.ETOPSRes.Wt && WeightsApplied.ETOPSResWeights.Count == 0)
			{
				foreach (Wt1Parameter para in wBidStateContent.Weights.ETOPSRes.lstParameters)
				{
					WeightsApplied.ETOPSResWeights.Add("ETOPS-Res");
				}
			}
			if (wBidStateContent.CxWtState.FLTMIN.Wt&&WeightsApplied.FlightTimeWeights.Count==0) {
				foreach (Wt3Parameter para in wBidStateContent.Weights.FLTMIN.lstParameters) {
					WeightsApplied.FlightTimeWeights.Add ("Flight Time");
				}
			}
			if (wBidStateContent.CxWtState.GRD.Wt&&WeightsApplied.GroundTimeWeights.Count==0) {
				foreach (Wt3Parameter para in wBidStateContent.Weights.GRD.lstParameters) {
					WeightsApplied.GroundTimeWeights.Add ("Intl – NonConus");
				}
			}
			if (wBidStateContent.CxWtState.InterConus.Wt&&WeightsApplied.IntlNonConusWeights.Count==0) {
				foreach (Wt2Parameter para in wBidStateContent.Weights.InterConus.lstParameters) {
					WeightsApplied.IntlNonConusWeights.Add ("Legs Per Duty Period");
				}
			}

			if (wBidStateContent.CxWtState.LEGS.Wt&&WeightsApplied.LegsPerDutyPeriodWeights.Count==0) {
				foreach (Wt3Parameter para in wBidStateContent.Weights.LEGS.lstParameters) {
					WeightsApplied.LegsPerDutyPeriodWeights.Add ("Legs Per Duty Period");
				}
			}
			if (wBidStateContent.CxWtState.LegsPerPairing.Wt&&WeightsApplied.LegsPerPairingWeights.Count==0) {
				foreach (Wt3Parameter para in wBidStateContent.Weights.WtLegsPerPairing.lstParameters) {
					WeightsApplied.LegsPerPairingWeights.Add ("Legs Per Pairing");
				}
			}
			if (wBidStateContent.CxWtState.NODO.Wt&&WeightsApplied.NumOfDaysOffWeights.Count==0) {
				foreach (Wt2Parameter para in wBidStateContent.Weights.NODO.lstParameters) {
					WeightsApplied.NumOfDaysOffWeights.Add ("Number of Days Off");
				}
			}
			if (wBidStateContent.CxWtState.RON.Wt&&WeightsApplied.OvernightCitiesWeights.Count==0) {
				foreach (Wt2Parameter para in wBidStateContent.Weights.RON.lstParameters) {
					WeightsApplied.OvernightCitiesWeights.Add ("Overnight Cities");
				}
			}
			if (wBidStateContent.CxWtState.CitiesLegs.Wt&&WeightsApplied.CitiesLegsWeights.Count==0) {
				foreach (Wt2Parameter para in wBidStateContent.Weights.CitiesLegs.lstParameters) {
					WeightsApplied.CitiesLegsWeights.Add ("Cities-Legs");
				}
			}
			if (wBidStateContent.CxWtState.PDAfter.Wt&&WeightsApplied.PDOAfterWeights.Count==0) {
				foreach (Wt4Parameter para in wBidStateContent.Weights.PDAfter.lstParameters) {
					WeightsApplied.PDOAfterWeights.Add ("PDO-after");
				}
			}
			if (wBidStateContent.CxWtState.PDBefore.Wt&&WeightsApplied.PDOBeforeWeights.Count==0) {
				foreach (Wt4Parameter para in wBidStateContent.Weights.PDBefore.lstParameters) {
					WeightsApplied.PDOBeforeWeights.Add ("PDO-before");
				}
			}
			if (wBidStateContent.CxWtState.Position.Wt&&WeightsApplied.PositionWeights.Count==0) {
				foreach (Wt2Parameter para in wBidStateContent.Weights.POS.lstParameters) {
					WeightsApplied.PositionWeights.Add ("Position");
				}
			}
			if (wBidStateContent.CxWtState.SDOW.Wt&&WeightsApplied.StartDOWWeights.Count==0) {
				foreach (Wt2Parameter para in wBidStateContent.Weights.SDOW.lstParameters) {
					WeightsApplied.StartDOWWeights.Add ("Start Day of Week");
				}
			}
			if (wBidStateContent.CxWtState.TL.Wt&&WeightsApplied.TripLengthWeights.Count==0) {
				foreach (Wt2Parameter para in wBidStateContent.Weights.TL.lstParameters) {
					WeightsApplied.TripLengthWeights.Add ("Trip Length");
				}
			}
			if (wBidStateContent.CxWtState.WB.Wt&&WeightsApplied.WorkBlockLengthWeights.Count==0) {
				foreach (Wt2Parameter para in wBidStateContent.Weights.WB.lstParameters) {
					WeightsApplied.WorkBlockLengthWeights.Add ("Work Blk Length");
				}
			}
			if (wBidStateContent.CxWtState.WorkDay.Wt&&WeightsApplied.WorkDaysWeights.Count==0) {
				foreach (Wt3Parameter para in wBidStateContent.Weights.WorkDays.lstParameters) {
					WeightsApplied.WorkDaysWeights.Add ("Work Days");
				}
			}
			if (wBidStateContent.CxWtState.Rest.Wt&&WeightsApplied.RestWeights.Count==0) {
				foreach (Wt4Parameter para in wBidStateContent.Weights.WtRest.lstParameters) {
					WeightsApplied.RestWeights.Add ("Rest");
				}
			}
			if (wBidStateContent.CxWtState.Commute.Wt&&WeightsApplied.Commutabilityweights.Count==0) {
				
				//	WeightsApplied.RestWeights.Add ("Commutability");
				WeightsApplied.Commutabilityweights.Add("Commutability");
			}


		}

		private void setValuesToFixedSorts()
		{
			if (wBidStateContent.ForceLine.IsBlankLinetoBottom) {
				btnBlankToBottom.Selected = true;
				//buttonColor (btnBlankToBottom);
			} else
				btnBlankToBottom.Selected = false;
			if (wBidStateContent.ForceLine.IsReverseLinetoBottom) {
				btnReserveToBottom.Selected = true;
				//buttonColor (btnReserveToBottom);
			} else
				btnReserveToBottom.Selected = false;
			ClearAllButtons();
			SortDetails stateSortDetails = wBidStateContent.SortDetails;
			if (stateSortDetails.SortColumn == "Line" || stateSortDetails.SortColumn == string.Empty)
			{
				this.btnLineNum.Selected = true;
				//buttonColor (btnLineNum);
				btnBlankToBottom.Enabled = false;
				btnReserveToBottom.Enabled = false;
			}
			else if (stateSortDetails.SortColumn == "LinePay")
			{
				this.btnLinePay.Selected = true;
				//buttonColor (btnLinePay);
			}
			else if (stateSortDetails.SortColumn == "PayPerDay")
			{
				this.btnPPDay.Selected = true;
				//buttonColor (btnPPDay);
			}
			else if (stateSortDetails.SortColumn == "PayPerDutyHour")
			{
				this.btnPPDutyHr.Selected = true;
				//buttonColor (btnPPDutyHr);
			}
			else if (stateSortDetails.SortColumn == "PayPerFlightHour")
			{
				this.btnPPFlightHr.Selected = true;
				//buttonColor (btnPPFlightHr);
			}
			else if (stateSortDetails.SortColumn == "BlockSort")
			{
				this.btnBlockSort.Selected = true;
				//buttonColor (btnBlockSort);
			}
			else if (stateSortDetails.SortColumn == "PayPerTimeAway")
			{
				this.btnPPTimeAway.Selected = true;
				//buttonColor (btnPPTimeAway);
			}
            else if (stateSortDetails.SortColumn == "Award")
            {
                this.btnSortByAward.Selected = true;
               
            }
            else if (stateSortDetails.SortColumn == "SubmittedBid")
            {
                this.btnSortBySubmittedBid.Selected = true;

            }
			else if (stateSortDetails.SortColumn == "BlockSort")
			{
				this.btnBlockSort.Selected = true;
				//buttonColor (btnBlockSort);
			}

			if (stateSortDetails.SortColumn == "SelectedColumn") {
				this.lblSelected.BackgroundColor = ColorClass.activeOrange;
				this.lblSelected.TextColor = UIColor.White;
			}

			if (stateSortDetails.SortColumn == "Manual") {
				this.lblManual.BackgroundColor = ColorClass.activeOrange;
				this.lblManual.TextColor = UIColor.White;
			}


		}
		partial void btnHelpIconTapped (UIKit.UIButton sender)
		{
			if (sgSortnWeights.SelectedSegment == 0) {
				HelpViewController helpVC = new HelpViewController ();
				helpVC.pdfFileName = "Sorts";
				helpVC.selectRow = 2;
				UINavigationController navCont = new UINavigationController (helpVC);
				navCont.NavigationBar.BarStyle = UIBarStyle.Black;
				navCont.NavigationBar.Hidden = true;
				navCont.ModalPresentationStyle = UIModalPresentationStyle.Custom;
				this.PresentViewController (navCont, true, null);
			} else {
				HelpViewController helpVC = new HelpViewController ();
				helpVC.pdfFileName = "Weights";
				helpVC.selectRow = 1;
				UINavigationController navCont = new UINavigationController (helpVC);
				navCont.NavigationBar.BarStyle = UIBarStyle.Black;
				navCont.NavigationBar.Hidden = true;
				navCont.ModalPresentationStyle = UIModalPresentationStyle.Custom;
				this.PresentViewController (navCont, true, null);
			}
		}
		partial void sgSortnWeightsTapped (UIKit.UISegmentedControl sender)
		{
			this.toggleSortWeight ();
		}
		public void toggleSortWeight ()
		{
			if (this.sgSortnWeights.SelectedSegment==0) {
				CommonClass.weightSelected = false;
				this.btnPlus2.Hidden = true;
				this.vwSort.Hidden = false;
				this.vwWeights.Hidden = true;
				if(weightsController!=null){
					weightsController.View.RemoveFromSuperview();
					weightsController = null;
				}

			} else {
                CommonClass.weightSelected = true;
                this.btnPlus2.Hidden = false;
                this.vwSort.Hidden = true;
                this.vwWeights.Hidden = false;
                weightsController = new WeightsTableController();
                this.AddChildViewController(weightsController);
                vwWeights.AddSubview(weightsController.TableView);
				//  this.PerformSelector(new ObjCRuntime.Selector("ViewShow11"), null, 0.5)
				weightsController.TableView.Frame = vwWeights.Bounds;
				
            }
		}

		[Export("ViewShow11")]
		void viewShow()
        {
			CommonClass.weightSelected = true;
			this.btnPlus2.Hidden = false;
			this.vwSort.Hidden = true;
			this.vwWeights.Hidden = false;
			weightsController = new WeightsTableController();
			this.AddChildViewController(weightsController);
			vwWeights.AddSubview(weightsController.TableView);
			//this.PerformSelector(new ObjCRuntime.Selector("ViewShow11"), null, 0.1);
			weightsController.TableView.Frame = new CGRect(0, 0, this.View.Frame.Width, this.View.Frame.Height);//-236
		}

		partial void btnBlankToBotTap (UIKit.UIButton sender)
		{
			sender.Selected = !sender.Selected;
			WBidHelper.PushToUndoStack ();
            if (sender.Selected)
            {
                wBidStateContent.ForceLine.IsBlankLinetoBottom = true;
                LineOperations.ForceBlankLinestoBottom();
            }
            else 
            {
                wBidStateContent.ForceLine.IsBlankLinetoBottom = false;
            }
			//this.buttonColor (sender);
			GlobalSettings.isModified = true;
			CommonClass.cswVC.UpdateSaveButton ();
		}
		partial void btnReserveToBotTap (UIKit.UIButton sender)
		{
			sender.Selected = !sender.Selected;
			WBidHelper.PushToUndoStack ();
            if (sender.Selected)
            {
                wBidStateContent.ForceLine.IsReverseLinetoBottom = true;
                LineOperations.ForceReserveLinestoBottom();
            }
            else
            {
                wBidStateContent.ForceLine.IsReverseLinetoBottom = false;
            }
			//this.buttonColor (sender);
			GlobalSettings.isModified = true;
			CommonClass.cswVC.UpdateSaveButton ();
		}
		partial void btnSortParaTap (UIKit.UIButton sender)
		{
            if (sender.Selected)
                return;
			WBidHelper.PushToUndoStack ();
            GetCurrentSortButton();

            ClearAllButtons();
            sender.Selected = true;
			if(sender.TitleLabel.Text=="Line Number"){
				this.btnBlankToBottom.Selected = false;
				this.btnReserveToBottom.Selected = false;
				//this.buttonColor (btnBlankToBottom);
				//this.buttonColor (btnReserveToBottom);
				this.btnBlankToBottom.Enabled = false;
				this.btnReserveToBottom.Enabled = false;
				wBidStateContent.ForceLine.IsBlankLinetoBottom = false;
				wBidStateContent.ForceLine.IsReverseLinetoBottom = false;
			}
			else {
				this.btnBlankToBottom.Enabled = true;
				this.btnReserveToBottom.Enabled = true;
			}
            //if (sender.TitleLabel.Text == "Sort By Award" && (GlobalSettings.WBidStateCollection.BidAwards == null || GlobalSettings.WBidStateCollection.BidAwards.Count == 0))
				if (sender.TitleLabel.Text == "Sort By Award")
				{
                OdataBuilder ObjOdata = new OdataBuilder();
                MonthlyBidDetails objbiddetails = new MonthlyBidDetails();
                ObjOdata.RestService.Objdelegate = this;
                objbiddetails.Domicile = GlobalSettings.CurrentBidDetails.Domicile;
                objbiddetails.Month = GlobalSettings.CurrentBidDetails.Month;
                objbiddetails.Year = GlobalSettings.CurrentBidDetails.Year;
                objbiddetails.Position = GlobalSettings.CurrentBidDetails.Postion;
                objbiddetails.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
                servicecURL = "GetMonthlyAwards";
                ObjOdata.GetMonthlyAwards(objbiddetails);
            }
            if (sender.TitleLabel.Text == "Sort By Submitted Bid" && (GlobalSettings.WBidStateCollection.SubmittedResult == null || GlobalSettings.WBidStateCollection.SubmittedResult == string.Empty))
            {
                OdataBuilder ObjOdata = new OdataBuilder();
                BidSubmittedData objbiddetails = new BidSubmittedData();
                ObjOdata.RestService.Objdelegate = this;
                objbiddetails.Domicile = GlobalSettings.CurrentBidDetails.Domicile;
                objbiddetails.Month = GlobalSettings.CurrentBidDetails.Month;
                objbiddetails.Year = GlobalSettings.CurrentBidDetails.Year;
                objbiddetails.Position = GlobalSettings.CurrentBidDetails.Postion;
                objbiddetails.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
                objbiddetails.EmpNum = Convert.ToInt32( GlobalSettings.WbidUserContent.UserInformation.EmpNo);
                servicecURL = "GetBidSubmittedData";
                ObjOdata.GetBidSubmittedData(objbiddetails);
            }
            else
            {
                ApplySort(sender.TitleLabel.Text);
            }
			//this.buttonColor (sender);
		}
        UIButton currentSortbutton;
        private void GetCurrentSortButton()
        {
            if (this.btnLineNum.Selected == true)
                currentSortbutton = this.btnLineNum;
            if (this.btnLinePay.Selected == true)
                currentSortbutton = this.btnLinePay;
            if (this.btnPPDay.Selected == true)
                currentSortbutton = this.btnPPDay;
            if (this.btnPPDutyHr.Selected == true)
                currentSortbutton = this.btnPPDutyHr;
            if (this.btnPPFlightHr.Selected == true)
                currentSortbutton = this.btnPPFlightHr;
            if (this.btnPPFDP.Selected == true)
                currentSortbutton = this.btnPPFDP;
            if (this.btnPPTimeAway.Selected == true)
                currentSortbutton = this.btnPPTimeAway;
            if (this.btnBlockSort.Selected == true)
                currentSortbutton = this.btnBlockSort;
            if (this.btnSortByAward.Selected == true)
                currentSortbutton = this.btnSortByAward;
            if (this.btnSortBySubmittedBid.Selected == true)
                currentSortbutton = this.btnSortBySubmittedBid;
        }
        public void ServiceResponce(JsonValue jsonDoc)
        {
            InvokeOnMainThread(() => {
                Console.WriteLine("Service Success");
                if (servicecURL == "GetBidSubmittedData")
                {
                    GlobalSettings.WBidStateCollection.SubmittedResult = CommonClass.ConvertJSonToObject<BidSubmittedData>(jsonDoc.ToString()).SubmittedResult;
                    if (GlobalSettings.WBidStateCollection.SubmittedResult == null || GlobalSettings.WBidStateCollection.SubmittedResult == string.Empty)
                    {
                        btnSortBySubmittedBid.Selected = false;
                        currentSortbutton.Selected = true;
                        UIAlertController okAlertController = UIAlertController.Create("WBidMax", "Bid Submitted data is not available", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);
                    }
                    else
                    {
                        ApplySort("Sort By Submitted Bid");
                    }
                }
                else if (servicecURL == "GetMonthlyAwards")
                {
                    //ActivityIndicator.Hide ();
                    GlobalSettings.WBidStateCollection.BidAwards = CommonClass.ConvertJSonToObject<BidAwardDetails>(jsonDoc.ToString()).BidAwards;
                    if (GlobalSettings.WBidStateCollection.BidAwards == null || GlobalSettings.WBidStateCollection.BidAwards.Count == 0)
                    {
                        btnSortByAward.Selected = false;
                        currentSortbutton.Selected = true;
                        UIAlertController okAlertController = UIAlertController.Create("WBidMax", "Bid Award is not available", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);
                    }
                    else
                    {
                        ApplySort("Sort By Award");
                    }
                }
            });
        }

        public void ResponceError(string Error)
        {
            InvokeOnMainThread(() => {
                //ActivityIndicator.Hide();
                Console.WriteLine("Service Fail");

                UIAlertController okAlertController = UIAlertController.Create("WBidMax", Error, UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
            });
        }
		partial void btnLinesTapped (UIKit.UIButton sender)
		{
            SortCalculation sort = new SortCalculation();
            WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            if (wBidStateContent.SortDetails != null && wBidStateContent.SortDetails.SortColumn != null && wBidStateContent.SortDetails.SortColumn != string.Empty)
            {
                sort.SortLines(wBidStateContent.SortDetails.SortColumn);
            }
            //added by Roshil Aug 2020
            CommonClass.lineVC.UpdateSaveButton();
			//NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);
			NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
			NSNotificationCenter.DefaultCenter.PostNotificationName("SetFlightDataDifferenceButton", null);
			this.ParentViewController.NavigationController.DismissViewController (true, null);

			//rmSelector(new ObjCRuntime.Selector("memoryRelease"), null, 1);
			memoryRelease ();

		}
	//	[Export("memoryRelease")]

		public void memoryRelease()
		{
			foreach (NSObject obj in arrObserver) {
				NSNotificationCenter.DefaultCenter.RemoveObserver (obj);
			}
			CommonClass.cswVC.memoryRelease ();
			foreach (UIView view in this.ParentViewController.View.Subviews) {

				DisposeClass.DisposeEx(view);
			}
			this.View.Dispose ();

		}

		public void  removeObservers()
		{
			foreach (NSObject obj in arrObserver) {
				NSNotificationCenter.DefaultCenter.RemoveObserver (obj);
			}
		}
		partial void btnPlus2Tapped (UIKit.UIButton sender)
		{
			PopoverViewController popoverContent = new PopoverViewController ();
			popoverContent.PopType = "addWeights";
			popoverController = new UIPopoverController (popoverContent);
			popoverController.Delegate = new MyPopDelegate (this);
			popoverController.PopoverContentSize = new CGSize (300, 600);
			popoverController.PresentFromRect(sender.Frame,this.View,UIPopoverArrowDirection.Any,true);




		}


        private void ApplySort(string sortParameter)
        {
            string sortKey = string.Empty;
            SortCalculation sort = new SortCalculation();

            switch (sortParameter)
            {
                case "Line Number":
                    sortKey = "Line";
                    break;
                case "Line Pay":
                    sortKey = "LinePay";
                    break;
                case "Pay Per Day":
                    sortKey = "PayPerDay";
                    break;
                case "Pay Per Duty Hour":
                    sortKey="PayPerDutyHour";
                    break;

                case "Pay Per Flight Hour":
                    sortKey = "PayPerFlightHour";
                    break;
                case "Pay Per Time Away From Base":
                    sortKey = "PayPerTimeAway";
                    break;
                case "Sort By Award":
                    sortKey = "Award";
                    break;
                case "Sort By Submitted Bid":
                    sortKey = "SubmittedBid";
                    break;
                case "Selected Column":
                    sortKey = "SelectedColumn";
                    break;
				case "Block Sort":
					sortKey = "BlockSort";
					break;

            }

            if (sortKey != string.Empty)
            {
                sort.SortLines(sortKey);
				wBidStateContent.SortDetails.SortColumn = sortKey;
				GlobalSettings.isModified = true;
				CommonClass.cswVC.UpdateSaveButton ();
            }
        }

   
		partial void btnClearTap (UIKit.UIButton sender)
		{
			if (sgSortnWeights.SelectedSegment==0) {
				UIActionSheet sheet = new UIActionSheet("Really want to clear all Sort?",null,null,"YES",null);
				//sheet.ShowFrom(sender.Frame,this.View,true);

				CGRect senderframe = sender.Frame;
				senderframe.X = sender.Frame.GetMidX();
				sheet.ShowFrom(senderframe, this.btnClear, true);

				sheet.Clicked += HandleClearSortWeights;
			} else {
				UIActionSheet sheet = new UIActionSheet("Really want to clear all Weights?",null,null,"YES",null);
				sheet.ShowFrom(sender.Frame,this.View,true);
				sheet.Clicked += HandleClearSortWeights;
			}

		}
		void HandleClearSortWeights (object sender, UIButtonEventArgs e)
		{
			if (sgSortnWeights.SelectedSegment == 0) {
				if (e.ButtonIndex == 0) {
					WBidHelper.PushToUndoStack ();
					ClearAllButtons ();
					ApplySort ("Line Number");

					btnLineNum.Selected = true;
					//this.buttonColor (btnLineNum);
					this.btnBlankToBottom.Selected = false;
					this.btnReserveToBottom.Selected = false;
					//this.buttonColor (btnBlankToBottom);
					//this.buttonColor (btnReserveToBottom);
					this.btnBlankToBottom.Enabled = false;
					this.btnReserveToBottom.Enabled = false;
					wBidStateContent.ForceLine.IsBlankLinetoBottom = false;
					wBidStateContent.ForceLine.IsReverseLinetoBottom = false;
                    wBidStateContent.SortDetails.BlokSort = new List<string>();
					tblSortList.TableView.ReloadData ();
				}
			} else {
				if (e.ButtonIndex == 0) {
					WBidHelper.PushToUndoStack ();
					weightCalc.ClearWeights ();
					WeightsApplied.clearAll ();
					NSNotificationCenter.DefaultCenter.PostNotificationName ("AddWeights", null);
				}
			}
		}
        private void ClearAllButtons()
        {
//            this.btnBlankToBottom.Selected = false;
//            this.btnReserveToBottom.Selected = false;
            this.btnLineNum.Selected = false;
            this.btnLinePay.Selected = false;
            this.btnPPDay.Selected = false;
            this.btnPPDutyHr.Selected = false;
            this.btnPPFlightHr.Selected = false;
            this.btnPPFDP.Selected = false;
            this.btnPPTimeAway.Selected = false;
            this.btnBlockSort.Selected = false;
            this.btnSortByAward.Selected = false;
            this.btnSortBySubmittedBid.Selected = false;
			//this.btnSelColumn.Selected = false;

//            this.btnBlankToBottom.BackgroundColor = UIColor.Clear;
//            this.btnReserveToBottom.BackgroundColor = UIColor.Clear;
            this.btnLineNum.BackgroundColor = UIColor.Clear;
            this.btnLinePay.BackgroundColor = UIColor.Clear;
            this.btnPPDay.BackgroundColor = UIColor.Clear;
            this.btnPPDutyHr.BackgroundColor = UIColor.Clear;
            this.btnPPFlightHr.BackgroundColor = UIColor.Clear;
            this.btnPPFDP.BackgroundColor = UIColor.Clear;
            this.btnPPTimeAway.BackgroundColor = UIColor.Clear;
            this.btnBlockSort.BackgroundColor = UIColor.Clear;
            //this.btnSelColumn.BackgroundColor = UIColor.Clear;
            this.btnSortByAward.BackgroundColor = UIColor.Clear;
            this.btnSortBySubmittedBid.BackgroundColor = UIColor.Clear;
                
			this.lblSelected.BackgroundColor = UIColor.Clear;
			this.lblSelected.TextColor = UIColor.Black;
			this.lblManual.BackgroundColor = UIColor.Clear;
			this.lblManual.TextColor = UIColor.Black;
			CommonClass.columnID = 0;

        }
		partial void btnAddBlockSort (UIKit.UIButton sender)
		{
			PopoverViewController popoverContent = new PopoverViewController ();
			popoverContent.PopType = "blockSort";	
			popoverController = new UIPopoverController (popoverContent);
			popoverController.Delegate = new MyPopDelegate (this);
			popoverController.PopoverContentSize = new CGSize (300, 400);
            //var superViewY = ObjButton.Superview.Frame.GetMinY();
            //CGRect NewFrame = ObjButton.Frame;
            //NewFrame.Y = ObjButton.Frame.Y + superViewY;

            //popover.PresentFromRect(NewFrame, this.View, UIPopoverArrowDirection.Any, true);
			popoverController.PresentFromRect (sender.Frame, this.vwSortContainer, UIPopoverArrowDirection.Any, true);

	
		}

	
	}
}

