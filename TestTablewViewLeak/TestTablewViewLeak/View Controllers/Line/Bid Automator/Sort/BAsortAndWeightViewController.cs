using System;
using CoreGraphics;
using Foundation;
using UIKit;

using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using System.Linq;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using System.Collections.Generic;
using WBid.WBidiPad.iOS.Utility;
using System.Reflection;


namespace WBid.WBidiPad.iOS
{
	public partial class BAsortAndWeightViewController : UIViewController
	{
		WBidState wBidStateContent =  GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		UIPopoverController popoverController;
		BAblockSortTableControllerController tblSortList;
		WeightsTableController weightsController;
		List <NSObject> arrObserver = new List <NSObject> ();
		WeightCalculation weightCalc = new WeightCalculation();
		private NSObject _DayOfcellNotification;
		UIPopoverController popover ;
		//public BidAutomator BidAuto;
		public SortTempValues sortTempValue;
		class MyPopDelegate : UIPopoverControllerDelegate
		{
			BAsortAndWeightViewController _parent;
			public MyPopDelegate (BAsortAndWeightViewController parent)
			{
				_parent = parent;
			}
			public override void DidDismiss (UIPopoverController popoverController)
			{
				_parent.popoverController = null;

			}
		}

		public BAsortAndWeightViewController () : base ("BAsortAndWeightViewController", null)
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
//			if (CommonClass.weightSelected) {
//				this.sgSortnWeights.SelectedSegment = 1;
//				this.toggleSortWeight ();
//			}
			this.setGraphics ();



			if (wBidStateContent.BidAuto != null) 
			{
				
				//BidAuto = UtilityClass.CloneObject (wBidStateContent.BidAuto) as BidAutomator;

				//BidAuto= new BidAutomator(wBidStateContent.BidAuto);
				if (wBidStateContent.BidAuto.BASort == null)
				{
					wBidStateContent.BidAuto.BASort = new SortDetails ();
				
				}

				
				//setValuesToFixedSorts ();

			}  else {
				wBidStateContent.BidAuto = new BidAutomator ();
				wBidStateContent.BidAuto.BASort = new SortDetails ();
			
			}

			sortTempValue = new SortTempValues ();
			sortTempValue.IsBlankBottom = wBidStateContent.BidAuto.IsBlankBottom;
			sortTempValue.IsReserveBottom = wBidStateContent.BidAuto.IsReserveBottom;
			sortTempValue.IsReserveFirst = wBidStateContent.BidAuto.IsReserveFirst;
			sortTempValue.SortColumn = wBidStateContent.BidAuto.BASort.SortColumn;
			sortTempValue.SortColumnName = wBidStateContent.BidAuto.BASort.SortColumnName;
			sortTempValue.SortDirection = wBidStateContent.BidAuto.BASort.SortDirection;
			sortTempValue.BlokSort = new List<string> ();
			if (wBidStateContent.BidAuto.BASort.BlokSort != null) {
				foreach (var blocksortItem in wBidStateContent.BidAuto.BASort.BlokSort) {
					sortTempValue.BlokSort.Add (blocksortItem);
				}
			}


			setValuesToFixedSorts ();
			loadSortTable ();
			//			observeNotifs ();
			listSavedWeights ();
			vwSort.BackgroundColor = ColorClass.ListSeparatorColor;
			//HandleSegmentButton ();
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
			arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("BAreloadBlockSort"), reloadSortList));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString("BAAddWeights"), handleWeightsReload));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("BAdismissWtPopover"),dismissPopover));
		}
		public void removeObservers ()
		{
			foreach (NSObject obj in arrObserver) {
				NSNotificationCenter.DefaultCenter.RemoveObserver (obj);
			}
		}

	
		public void memoryRelease()
		{
			foreach (NSObject obj in arrObserver) {
				NSNotificationCenter.DefaultCenter.RemoveObserver (obj);
			}
			foreach (UIView view in this.ParentViewController.View.Subviews) {

				DisposeClass.DisposeEx(view);
			}
			this.View.Dispose ();

		}

		public void reloadSortList(NSNotification n)
		{
			tblSortList.TableView.ReloadData ();
			if (sortTempValue.BlokSort.Count != 0) {
				btnBlockSort.Enabled = true;
				btnBlockSort.Selected = true;
				//buttonColor (btnBlockSort);
				ApplySort ("Block Sort");
				sortTempValue.SortColumn = "BlockSort";
			}  else {
				btnBlockSort.Enabled = false;
				btnBlockSort.Selected = false;
				//buttonColor (btnBlockSort);
				//btnLineNum.Selected = true;
				//buttonColor (btnLineNum);
				//ApplySort ("Line Number");
				//wBidStateContent.BidAuto.BASort.SortColumn = "Line";
				sortTempValue.SortColumn="";
				this.btnBlankToBottom.Selected = false;
				this.btnReserveToBottom.Selected = false;
				//this.buttonColor (btnBlankToBottom);
				//this.buttonColor (btnReserveToBottom);
				this.btnBlankToBottom.Enabled = false;
				this.btnReserveToBottom.Enabled = false;
				sortTempValue.IsBlankBottom = false;
				sortTempValue.IsReserveBottom = false;


			}
			setValuesToFixedSorts ();

		}
		private void dismissPopover (NSNotification n)
		{
			if(popoverController !=null)
			popoverController.Dismiss (true);
		}
		private void handleWeightsReload (NSNotification n)
		{
			if (weightsController != null)
				weightsController.TableView.ReloadData ();
			GlobalSettings.isModified = true;
			CommonClass.BAVC.UpdateSaveButton ();
		}

		private void loadSortTable()
		{
			tblSortList = new BAblockSortTableControllerController (sortTempValue);

			this.AddChildViewController (tblSortList);
			vwSortList.AddSubview (tblSortList.View);
			tblSortList.View.Frame = vwSortList.Bounds;
			tblSortList.TableView.SetEditing (true, true);

			if (sortTempValue.BlokSort.Count == 0) {
				this.btnBlockSort.Enabled = false;
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

			this.btnBlankToBottom.SetBackgroundImage(UIImage.FromBundle ("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			this.btnBlankToBottom.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnReserveToBottom.SetBackgroundImage(UIImage.FromBundle ("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			this.btnReserveToBottom.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);

			this.btnLineNum.SetBackgroundImage(UIImage.FromBundle ("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			this.btnLineNum.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnLinePay.SetBackgroundImage(UIImage.FromBundle ("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			this.btnLinePay.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnPPDay.SetBackgroundImage(UIImage.FromBundle ("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			this.btnPPDay.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnPPDutyHr.SetBackgroundImage(UIImage.FromBundle ("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			this.btnPPDutyHr.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnPPFlightHr.SetBackgroundImage(UIImage.FromBundle ("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			this.btnPPFlightHr.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnPPFDP.SetBackgroundImage(UIImage.FromBundle ("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			this.btnPPFDP.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnPPTimeAway.SetBackgroundImage(UIImage.FromBundle ("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			this.btnPPTimeAway.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnBlockSort.SetBackgroundImage(UIImage.FromBundle ("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			this.btnBlockSort.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);

		}

		//		public void buttonColor (UIButton btn)
		//		{
		//			if (btn.Selected)
		//				btn.BackgroundColor = UIColor.FromRGB (150, 241, 250);
		//			else
		//				btn.BackgroundColor = UIColor.Clear;
		//		}

		public void listSavedWeights () {
			//			WeightsApplied.clearAll ();
			//
			//			if (wBidStateContent.CxWtState.AMPM.Wt&&WeightsApplied.AMPMWeights.Count==0) {
			//				foreach (Wt2Parameter para in wBidStateContent.Weights.AM_PM.lstParameters) {
			//					WeightsApplied.AMPMWeights.Add ("AM/PM");
			//				}
			//			}
			//			if (wBidStateContent.CxWtState.BDO.Wt&&WeightsApplied.BlocksOfDaysOffWeights.Count==0) {
			//				foreach (Wt3Parameter para in wBidStateContent.Weights.BDO.lstParameters) {
			//					WeightsApplied.BlocksOfDaysOffWeights.Add ("Blocks of Days Off");
			//				}
			//			}
			//			if (wBidStateContent.CxWtState.DHD.Wt&&WeightsApplied.CmutDHsWeights.Count==0) {
			//				foreach (Wt3Parameter para in wBidStateContent.Weights.DHD.lstParameters) {
			//					WeightsApplied.CmutDHsWeights.Add ("Cmut DHs");
			//				}
			//			}
			//			if (wBidStateContent.CxWtState.DHDFoL.Wt&&WeightsApplied.dhFirstLastWeights.Count==0) {
			//				foreach (Wt2Parameter para in wBidStateContent.Weights.DHDFoL.lstParameters) {
			//					WeightsApplied.dhFirstLastWeights.Add ("DH - first - last");
			//				}
			//			}
			//			if (wBidStateContent.CxWtState.DP.Wt&&WeightsApplied.DutyPeriodWeights.Count==0) {
			//				foreach (Wt3Parameter para in wBidStateContent.Weights.DP.lstParameters) {
			//					WeightsApplied.DutyPeriodWeights.Add ("Duty period");
			//				}
			//			}
			//			if (wBidStateContent.CxWtState.EQUIP.Wt&&WeightsApplied.EQTypeWeights.Count==0) {
			//				foreach (Wt3Parameter para in wBidStateContent.Weights.EQUIP.lstParameters) {
			//					WeightsApplied.EQTypeWeights.Add ("Equipment Type");
			//				}
			//			}
			//			if (wBidStateContent.CxWtState.FLTMIN.Wt&&WeightsApplied.FlightTimeWeights.Count==0) {
			//				foreach (Wt3Parameter para in wBidStateContent.Weights.FLTMIN.lstParameters) {
			//					WeightsApplied.FlightTimeWeights.Add ("Flight Time");
			//				}
			//			}
			//			if (wBidStateContent.CxWtState.GRD.Wt&&WeightsApplied.GroundTimeWeights.Count==0) {
			//				foreach (Wt3Parameter para in wBidStateContent.Weights.GRD.lstParameters) {
			//					WeightsApplied.GroundTimeWeights.Add ("Intl – NonConus");
			//				}
			//			}
			//			if (wBidStateContent.CxWtState.InterConus.Wt&&WeightsApplied.IntlNonConusWeights.Count==0) {
			//				foreach (Wt2Parameter para in wBidStateContent.Weights.InterConus.lstParameters) {
			//					WeightsApplied.IntlNonConusWeights.Add ("Legs Per Duty Period");
			//				}
			//			}
			//
			//			if (wBidStateContent.CxWtState.LEGS.Wt&&WeightsApplied.LegsPerDutyPeriodWeights.Count==0) {
			//				foreach (Wt3Parameter para in wBidStateContent.Weights.LEGS.lstParameters) {
			//					WeightsApplied.LegsPerDutyPeriodWeights.Add ("Legs Per Duty Period");
			//				}
			//			}
			//			if (wBidStateContent.CxWtState.LegsPerPairing.Wt&&WeightsApplied.LegsPerPairingWeights.Count==0) {
			//				foreach (Wt3Parameter para in wBidStateContent.Weights.WtLegsPerPairing.lstParameters) {
			//					WeightsApplied.LegsPerPairingWeights.Add ("Legs Per Pairing");
			//				}
			//			}
			//			if (wBidStateContent.CxWtState.NODO.Wt&&WeightsApplied.NumOfDaysOffWeights.Count==0) {
			//				foreach (Wt2Parameter para in wBidStateContent.Weights.NODO.lstParameters) {
			//					WeightsApplied.NumOfDaysOffWeights.Add ("Number of Days Off");
			//				}
			//			}
			//			if (wBidStateContent.CxWtState.RON.Wt&&WeightsApplied.OvernightCitiesWeights.Count==0) {
			//				foreach (Wt2Parameter para in wBidStateContent.Weights.RON.lstParameters) {
			//					WeightsApplied.OvernightCitiesWeights.Add ("Overnight Cities");
			//				}
			//			}
			//			if (wBidStateContent.CxWtState.PDAfter.Wt&&WeightsApplied.PDOAfterWeights.Count==0) {
			//				foreach (Wt4Parameter para in wBidStateContent.Weights.PDAfter.lstParameters) {
			//					WeightsApplied.PDOAfterWeights.Add ("PDO-after");
			//				}
			//			}
			//			if (wBidStateContent.CxWtState.PDBefore.Wt&&WeightsApplied.PDOBeforeWeights.Count==0) {
			//				foreach (Wt4Parameter para in wBidStateContent.Weights.PDBefore.lstParameters) {
			//					WeightsApplied.PDOBeforeWeights.Add ("PDO-before");
			//				}
			//			}
			//			if (wBidStateContent.CxWtState.Position.Wt&&WeightsApplied.PositionWeights.Count==0) {
			//				foreach (Wt2Parameter para in wBidStateContent.Weights.POS.lstParameters) {
			//					WeightsApplied.PositionWeights.Add ("Position");
			//				}
			//			}
			//			if (wBidStateContent.CxWtState.SDOW.Wt&&WeightsApplied.StartDOWWeights.Count==0) {
			//				foreach (Wt2Parameter para in wBidStateContent.Weights.SDOW.lstParameters) {
			//					WeightsApplied.StartDOWWeights.Add ("Start Day of Week");
			//				}
			//			}
			//			if (wBidStateContent.CxWtState.TL.Wt&&WeightsApplied.TripLengthWeights.Count==0) {
			//				foreach (Wt2Parameter para in wBidStateContent.Weights.TL.lstParameters) {
			//					WeightsApplied.TripLengthWeights.Add ("Trip Length");
			//				}
			//			}
			//			if (wBidStateContent.CxWtState.WB.Wt&&WeightsApplied.WorkBlockLengthWeights.Count==0) {
			//				foreach (Wt2Parameter para in wBidStateContent.Weights.WB.lstParameters) {
			//					WeightsApplied.WorkBlockLengthWeights.Add ("Work Blk Length");
			//				}
			//			}
			//			if (wBidStateContent.CxWtState.WorkDay.Wt&&WeightsApplied.WorkDaysWeights.Count==0) {
			//				foreach (Wt3Parameter para in wBidStateContent.Weights.WorkDays.lstParameters) {
			//					WeightsApplied.WorkDaysWeights.Add ("Work Days");
			//				}
			//			}
			//			if (wBidStateContent.CxWtState.Rest.Wt&&WeightsApplied.RestWeights.Count==0) {
			//				foreach (Wt4Parameter para in wBidStateContent.Weights.WtRest.lstParameters) {
			//					WeightsApplied.RestWeights.Add ("Rest");
			//				}
			//			}
			//
		}

		private void setValuesToFixedSorts()
		{
			ClearAllButtons();





			SortTempValues stateSortDetails = sortTempValue;
			if (stateSortDetails.SortColumn == "Line" || stateSortDetails.SortColumn == string.Empty)
			{
				this.btnLineNum.Selected = false;
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

			if (sortTempValue.IsBlankBottom) {
				btnBlankToBottom.Selected = true;
				//buttonColor (btnBlankToBottom);
			}  else
				btnBlankToBottom.Selected = false;
			if (sortTempValue.IsReserveBottom) {
				btnReserveToBottom.Selected = true;
				//buttonColor (btnReserveToBottom);
			}  else
				btnReserveToBottom.Selected = false;


			if (this.btnBlankToBottom.Selected && this.btnReserveToBottom.Selected) {
				if (sortTempValue.IsReserveFirst) {
					SegReserveAndBlank.SelectedSegment = 0;
					SegReserveAndBlank.Enabled = true;
				}  else {
					SegReserveAndBlank.SelectedSegment = 1;
					SegReserveAndBlank.Enabled = true;
				}
			}  else {
				SegReserveAndBlank.SelectedSegment = -1;
				SegReserveAndBlank.Enabled = false;
			}




		}
		void HandleSegmentButton()
		{

			if (this.btnBlankToBottom.Selected && this.btnReserveToBottom.Selected)
			{
				SegReserveAndBlank.Enabled = true;
				SegReserveAndBlank.SelectedSegment = 0;
			}
			else
			{
				SegReserveAndBlank.Enabled = false;
				SegReserveAndBlank.SelectedSegment = -1;
			}

			//			SegReserveAndBlank.Enabled = false;
			//			if (!this.btnBlankToBottom.Selected && !this.btnReserveToBottom.Selected) {
			//				SegReserveAndBlank.SelectedSegment = -1;
			//
			//			}
			//			else SegReserveAndBlank.Enabled = true;
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
			}  else {
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
			if(sender.SelectedSegment==1)
			{
				sender.SelectedSegment=0;

                UIAlertController okAlertController = UIAlertController.Create("WBidMax", "This feature is coming soon", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
            }
			//this.toggleSortWeight ();
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

			}  else {
//				CommonClass.weightSelected = true;
//				this.btnPlus2.Hidden = false;
//				this.vwSort.Hidden = true;
//				this.vwWeights.Hidden = false;
//				weightsController = new WeightsTableController ();
//				this.AddChildViewController (weightsController);
//				vwWeights.AddSubview (weightsController.View);
//				weightsController.View.Frame = vwWeights.Bounds;
			}
		}
		partial void reserveAndBlankSegmentClicked (NSObject sender)
		{
			UISegmentedControl Objseg= (UISegmentedControl)sender;
			switch(Objseg.SelectedSegment)
			{
			case 0:
				sortTempValue.IsReserveFirst= true;

				break;
			case 1:
				sortTempValue.IsReserveFirst= false;
				break;

			}
		}
		partial void btnBlankToBotTap (UIKit.UIButton sender)
		{
			sender.Selected = !sender.Selected;
			WBidHelper.PushToUndoStack ();
			if (sender.Selected)
			{
				sortTempValue.IsBlankBottom = true;
				// LineOperations.ForceBlankLinestoBottom();
			}
			else 
			{
				sortTempValue.IsBlankBottom = false;
				//  wBidStateContent.ForceLine.IsBlankLinetoBottom = false;
			}
			//this.buttonColor (sender);
			GlobalSettings.isModified = true;
			CommonClass.BAVC.UpdateSaveButton ();
			HandleSegmentButton();
		}
		partial void btnReserveToBotTap (UIKit.UIButton sender)
		{
			sender.Selected = !sender.Selected;
			WBidHelper.PushToUndoStack ();
			if (sender.Selected)
			{
				sortTempValue.IsReserveBottom = true; 
				// LineOperations.ForceReserveLinestoBottom();
			}
			else
			{
				sortTempValue.IsReserveBottom= false;
			}
			//this.buttonColor (sender);
			GlobalSettings.isModified = true;
			CommonClass.BAVC.UpdateSaveButton ();
			HandleSegmentButton();
		}
		partial void btnSortParaTap (UIKit.UIButton sender)
		{
			if (sender.Selected)
				return;
			WBidHelper.PushToUndoStack ();
			ClearAllButtons();
			sender.Selected = true;
			if(sender.TitleLabel.Text=="Line Number"){
				//				this.btnBlankToBottom.Selected = false;
				//				this.btnReserveToBottom.Selected = false;
				//				//this.buttonColor (btnBlankToBottom);
				//				//this.buttonColor (btnReserveToBottom);
				//				this.btnBlankToBottom.Enabled = false;
				//				this.btnReserveToBottom.Enabled = false;
				//				wBidStateContent.BidAuto.IsReserveBottom= false;
				//				wBidStateContent.BidAuto.IsBlankBottom= false;

			}
			else {
				this.btnBlankToBottom.Enabled = true;
				this.btnReserveToBottom.Enabled = true;
			}
			ApplySort(sender.TitleLabel.Text);
			//this.buttonColor (sender);
		}
		partial void btnLinesTapped (UIKit.UIButton sender)
		{
			//            SortCalculation sort = new SortCalculation();
			//            WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			//            if (wBidStateContent.SortDetails != null && wBidStateContent.SortDetails.SortColumn != null && wBidStateContent.SortDetails.SortColumn != string.Empty)
			//            {
			//                sort.SortLines(wBidStateContent.SortDetails.SortColumn);
			//            }
			//
			//			//NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);
			//			NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
			//			this.ParentViewController.NavigationController.DismissViewController (true, null);
		}
		partial void btnPlus2Tapped (UIKit.UIButton sender)
		{
			PopoverViewController popoverContent = new PopoverViewController ();
			popoverContent.PopType = "addWeights";
			popoverController = new UIPopoverController (popoverContent);
			popoverController.Delegate = new MyPopDelegate (this);
			popoverController.PopoverContentSize = new CGSize (270, 600);
			popoverController.PresentFromRect(sender.Frame,this.View,UIPopoverArrowDirection.Any,true);
		}

//		public void RemoveNotifications()
//		{
//			foreach (NSObject obj in arrObserver) {
//				NSNotificationCenter.DefaultCenter.RemoveObserver (obj);
//			}
//
//		}
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
				sortTempValue.SortColumn = sortKey;
				GlobalSettings.isModified = true;
				CommonClass.BAVC.UpdateSaveButton ();
			}
		}


		partial void btnClearTap (UIKit.UIButton sender)
		{
            if (sgSortnWeights.SelectedSegment == 0)
            {
                UIActionSheet sheet = new UIActionSheet("Really want to clear all Sort?", null, null, "YES", null);
                sheet.ShowFrom(sender.Frame, this.View, true);
                sheet.Clicked += HandleClearSortWeights;
            }
            else
            {
                UIActionSheet sheet = new UIActionSheet("Really want to clear all Weights?", null, null, "YES", null);
                sheet.ShowFrom(sender.Frame, this.View, true);
                sheet.Clicked += HandleClearSortWeights;
            }

        }

        void HandleClearSortWeights(object sender, UIButtonEventArgs e)
        {
            if (sgSortnWeights.SelectedSegment == 0)
            {
                if (e.ButtonIndex == 0)
                {
                    WBidHelper.PushToUndoStack();
                    ClearAllButtons();
                    //ApplySort ("Line Number");

                    //btnLineNum.Selected = true;
                    //this.buttonColor (btnLineNum);
                    this.btnBlankToBottom.Selected = false;
                    this.btnReserveToBottom.Selected = false;
                    //this.buttonColor (btnBlankToBottom);
                    //this.buttonColor (btnReserveToBottom);
                    this.btnBlankToBottom.Enabled = false;
                    this.btnReserveToBottom.Enabled = false;

                    sortTempValue.IsReserveBottom = false;
                    sortTempValue.IsBlankBottom = false;
                    sortTempValue.BlokSort = new List<string>();
                    tblSortList.TableView.ReloadData();
                    sortTempValue.SortColumn = "";
                }
            }
            else
            {
                if (e.ButtonIndex == 0)
                {
                    WBidHelper.PushToUndoStack();
                    weightCalc.ClearWeights();
                    WeightsApplied.clearAll();
                    NSNotificationCenter.DefaultCenter.PostNotificationName("BAAddWeights", null);
                }
            }
        }
        private void ClearAllButtons()
		{
			this.btnBlankToBottom.Selected = false;
			this.btnReserveToBottom.Selected = false;
			this.btnLineNum.Selected = false;
			this.btnLinePay.Selected = false;
			this.btnPPDay.Selected = false;
			this.btnPPDutyHr.Selected = false;
			this.btnPPFlightHr.Selected = false;
			this.btnPPFDP.Selected = false;
			this.btnPPTimeAway.Selected = false;
			this.btnBlockSort.Selected = false;

			SegReserveAndBlank.SelectedSegment = -1;
			SegReserveAndBlank.Enabled = false;
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

			this.lblSelected.BackgroundColor = UIColor.Clear;
			this.lblSelected.TextColor = UIColor.Black;
			this.lblManual.BackgroundColor = UIColor.Clear;
			this.lblManual.TextColor = UIColor.Black;
			CommonClass.columnID = 0;

		}
		partial void btnAddBlockSort (UIKit.UIButton sender)
		{
			//			PopoverViewController popoverContent = new PopoverViewController ();
			//			popoverContent.PopType = "blockSort";	
			//			popoverController = new UIPopoverController (popoverContent);
			//			popoverController.Delegate = new MyPopDelegate (this);
			//			popoverController.PopoverContentSize = new CGSize (250, 400);
			//			popoverController.PresentFromRect (sender.Frame, this.vwSortContainer, UIPopoverArrowDirection.Any, true);

			UIButton ObjButton= (UIButton)sender;
			BAPopOverViewController ObjPopOver= new BAPopOverViewController();
			ObjPopOver.PopOverType="AddSort";
			popover = new UIPopoverController (ObjPopOver);
			popover.PopoverContentSize = new CGSize (250, 400);
            var superViewY = ObjButton.Superview.Frame.GetMinY();
            CGRect NewFrame = ObjButton.Frame;
            NewFrame.Y = ObjButton.Frame.Y + superViewY;

            popover.PresentFromRect(NewFrame, this.View, UIPopoverArrowDirection.Any, true);
			//popover.PresentFromRect(ObjButton.Frame,this.View,UIPopoverArrowDirection.Any,true);
			_DayOfcellNotification=NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("BAAddSort"),UpdateOnBackToBaseEventCell);
		}
		public void UpdateOnBackToBaseEventCell(NSNotification n)
		{
			//n.Object.ToString ()
			string value = n.Object.ToString ();
			WBidHelper.PushToUndoStack ();
			if (sortTempValue.BlokSort == null)
				sortTempValue.BlokSort = new List<string> ();
			if (!sortTempValue.BlokSort.Contains (value)) {
				sortTempValue.BlokSort.Add (value);
			}

			NSNotificationCenter.DefaultCenter.PostNotificationName ("BAreloadBlockSort", null);
			NSNotificationCenter.DefaultCenter.RemoveObserver (_DayOfcellNotification);
			popover.Dismiss(true);



		}
	}
	public class SortTempValues
	{


		public bool IsReserveBottom { get; set; }

		public bool IsBlankBottom { get; set; }

		public bool IsReserveFirst { get; set; }

		public string SortColumn { get; set; }

		public string SortDirection { get; set; }

		public List<string> BlokSort { get; set; }

		public string SortColumnName { get; set; }

	}
}


