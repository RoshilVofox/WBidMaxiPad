using System;

using UIKit;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using WBid.WBidiPad.Model;
using CoreGraphics;
using Foundation;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Core.Enum;

namespace WBid.WBidiPad.iOS
{
	public partial class ConstraintsChangeViewController : UITableViewController
	{
		int _heightRow = 40;
		UIView noConstraintView;
        NSObject ObserverObj1;
        NSObject ObserverObj2;
		public ConstraintsChangeViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Title = "Constraints";
			LoadExistingFilters ();
            ObserverObj1=NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("AddConstraintClick"), AddConstraintClick);
            ObserverObj2=NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("ClearButtonClicked"), ClearButtonClicked);



		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			NSNotificationCenter.DefaultCenter.PostNotificationName ("CalPopHide", null);
			//txtCellNumber.Dispose ();

//			foreach (UIView view in this.View.Subviews) {
//
//				DisposeClass.DisposeEx(view);
//			}
//			this.View.Dispose ();
			this.View.UserInteractionEnabled = true;

		}

		private void LoadExistingFilters()
		{
			
				WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
				//  wBIdStateContent.BidAuto = null;
			if (wBIdStateContent.BidAuto != null && wBIdStateContent.BidAuto.BAFilter != null) {
				BidAutoHelper.LoadFilters (wBIdStateContent.BidAuto.BAFilter);
				ReLoadData ();
			} else {
				SharedObject.Instance.ListConstraint.Clear();
			}

		}

		public void PushViewControllView(FtCommutableLine cl){
			CommuteLinesViewController cmtView = Storyboard.InstantiateViewController ("CommuteLinesViewController")as CommuteLinesViewController;
			cmtView.data1 = cl;
			cmtView.ObjFromView = CommuteFromView.BidAutomator;
			cmtView.ObjChangeController = this;
            cmtView.PreferredContentSize = new CGSize(320, 320);
            cmtView.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;

			NavigationController.PresentViewController (cmtView, true,null);
		}

	
		public override void ViewWillAppear (bool animated)
		{
			ReLoadData ();
			base.ViewWillAppear (animated);
		}
		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1; // we only have on section
		}
		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return SharedObject.Instance.ListConstraint.Count;
		}
          public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)	{
			cell.BackgroundColor = UIColor.White;
		}
		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			var currentElement = SharedObject.Instance.ListConstraint.ElementAt (indexPath.Row);
			if (currentElement is AMPMConstriants) {
				AMPMCell cellAmPm = AMPMCell.Create ();
				cellAmPm.Filldata (this, (AMPMConstriants)currentElement);
				return cellAmPm;
			}
			if (currentElement is BlankReserveConstraint) {
				BlankReserveCell cell = BlankReserveCell.Create ();
				cell.Filldata (this, (BlankReserveConstraint)currentElement);
				return cell;
			}
			if (currentElement is FtCommutableLine) {
				CommutableLinesCell cell = CommutableLinesCell.Create ();
				cell.Filldata (this, (FtCommutableLine)currentElement);
				return cell;
			}
			if (currentElement is DaysOfMonthCx) {
//				DaysOfWeekAllCell cell = DaysOfWeekAllCell.Create ();
//				cell.Filldata (this, (DaysOfWeekAll)currentElement);
//				return cell;
//
				BADaysOfMonthCell cell = BADaysOfMonthCell.Create ();

				cell.Filldata (this, (DaysOfMonthCx)currentElement);
				return cell;
			}
			if (currentElement is DaysOfWeekAll) {
				DaysOfWeekAllCell cell = DaysOfWeekAllCell.Create ();
				cell.Filldata (this, (DaysOfWeekAll)currentElement);
				return cell;
			}
			if (currentElement is DaysOfWeekSome) {
				DaysOfWeekSomeCell cell = DaysOfWeekSomeCell.Create ();
				cell.Filldata (this, (DaysOfWeekSome)currentElement);
				return cell;
			}
			if (currentElement is DHFristLastConstraint) {
				DHFirstLastCell cell = DHFirstLastCell.Create ();
				cell.Filldata (this, (DHFristLastConstraint)currentElement);
				return cell;
			}
			if (currentElement is EquirementConstraint) {
				EquipmentCell cell = EquipmentCell.Create ();
				cell.Filldata (this, (EquirementConstraint)currentElement);
				return cell;
			}
			if (currentElement is LineTypeConstraint) {
				LinesTypeCell cell = LinesTypeCell.Create ();
				cell.Filldata (this, (LineTypeConstraint)currentElement);
				return cell;
			}
			if (currentElement is OvernightCitiesCx) {
				OvernightsCityCell cell = OvernightsCityCell.Create ();
				cell.Filldata (this, (OvernightCitiesCx)currentElement);
				return cell;
			}
			if (currentElement is RestCx) {
				RestCell cell = RestCell.Create ();
				cell.Filldata (this, (RestCx)currentElement);
				return cell;
			}
			if (currentElement is StartDayOfWeek) {
				StartDayOfWeekCell cell = StartDayOfWeekCell.Create ();
				cell.Filldata (this, (StartDayOfWeek)currentElement);
				return cell;
			}
			if (currentElement is CxTripBlockLength) {
				TripBlockLengthCell cellTripBlock = TripBlockLengthCell.Create ();
				cellTripBlock.Filldata (this, (CxTripBlockLength)currentElement);
				return cellTripBlock;
			}

			return null;
		}

		public void DeleteObject(object obj)
		{
			if (obj.GetType().Name == "FtCommutableLine")
			{
				var wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
				if (wBIdStateContent != null && wBIdStateContent.BidAuto != null && wBIdStateContent.BidAuto.DailyCommuteTimes != null)
				{
					wBIdStateContent.BidAuto.DailyCommuteTimes.Clear();
				}
                obj= SharedObject.Instance.ListConstraint.FirstOrDefault(x => x.GetType().Name == "FtCommutableLine");
			}

			SharedObject.Instance.ListConstraint.Remove (obj);

			ReLoadData ();
		}

        public void removeObservers ()
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver (ObserverObj1);
            NSNotificationCenter.DefaultCenter.RemoveObserver (ObserverObj2);
        }
		public void AddConstraintAtIndex(int row)
		{
			switch (row)
			{
			case Constants.Am_Pm_Constraint:
				SharedObject.Instance.ListConstraint.Add(new AMPMConstriants(){AM=true,PM=true,MIX=true});
				ReLoadData();
				break;
				//			case Constants.Blank_Reserve_Constraint:
				//				isFound = false;
				//				foreach (var item in SharedObject.Instance.ListConstraint) {
				//					if (item is BlankReserveConstraint) {
				//						isFound = true;
				//						break;
				//					}
				//				}
				//				if (!isFound) {
				//					SharedObject.Instance.ListConstraint.Add (new BlankReserveConstraint ());	
				//				}
				//				break;
			case Constants.Commutable_Lines_Constraint:
				
				if (!CheckFilterAlreadyAdded("FtCommutableLine"))
				{
					var cl = new FtCommutableLine() { ToHome = true,ToWork=false , NoNights=false, BaseTime=10,ConnectTime=30,CheckInTime=120};

					noConstraintView.Hidden = true;
					TableView.Hidden = false;
					PushViewControllView(cl);
				}
				break;
			case Constants.Days_of_Month_Constraint:
				
					var dom = new DaysOfMonthCx ();
					SharedObject.Instance.ListConstraint.Add (dom);
					noConstraintView.Hidden = true;
					TableView.Hidden = false;
					ShowDaysOfMonthVC (dom);

				break;
			case Constants.Days_Week_All_Constraint:
				SharedObject.Instance.ListConstraint.Add(new DaysOfWeekAll(){Su=true,Mo=true,Tu=true,We=true,Th=true,Fr=true,Sa=true});
				ReLoadData();
				break;
			case Constants.Days_of_Week_Some_Constraint:
				SharedObject.Instance.ListConstraint.Add(new DaysOfWeekSome() { Date = "Mon", LessOrMore = "Less than", Value = 0 });
				ReLoadData();
				break;
			case Constants.DH_First_Last_Constraint:
				SharedObject.Instance.ListConstraint.Add(new DHFristLastConstraint() { DH = "first", LessMore = "Less than", Value = 0 });
				ReLoadData();
				break;
			case Constants.Equipment_Constraint:
				SharedObject.Instance.ListConstraint.Add(new EquirementConstraint() { Equipment = 700, LessMore = "Less than", Value = 0 });
				ReLoadData();
				break;
			case Constants.Line_Type_Constraint:
				SharedObject.Instance.ListConstraint.Add(new LineTypeConstraint(){Hard=true,Res=true,Blank=true,Int=true,NonCon=true});
				ReLoadData();
				break;
			case Constants.Overnight_Cities_Constraint:
				var ovn = new OvernightCitiesCx();
				SharedObject.Instance.ListConstraint.Add(ovn);
				noConstraintView.Hidden = true;
				TableView.Hidden = false;
				ShowOvernightCities(ovn);
				//				ReLoadData ();
				break;
			case Constants.Rest_Constraint:
				if (!CheckFilterAlreadyAdded ("RestCx")) {
					SharedObject.Instance.ListConstraint.Add (new RestCx () { Dom = "All", LessMore = "Less than", Value = 8 });

					ReLoadData ();
				}
				break;
			case Constants.Start_Day_Of_Week_Constraint:
				SharedObject.Instance.ListConstraint.Add(new StartDayOfWeek());
				ReLoadData();
				break;
			case Constants.Trip_Work_Block_Length_Constraint:
				SharedObject.Instance.ListConstraint.Add(new CxTripBlockLength(){Turns=false,Twoday=false,ThreeDay=false,FourDay=false});
				ReLoadData();
				break;
			default:
				break;
			}
		}

		void ShowOvernightCities (OvernightCitiesCx ovn)
		{
			CitiesOvernightsViewController viewController = Storyboard.InstantiateViewController("CitiesOvernightsViewController") as CitiesOvernightsViewController;
			viewController.data = ovn;
			viewController.ObjChangeContraints = this;
			//NavigationController.PushViewController(viewController, true);
			viewController.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
			NavigationController.PresentViewController (viewController, true, null);

		}


		void ShowDaysOfMonthVC (DaysOfMonthCx dom)
		{
			//UIStoryboard	storyboard = UIStoryboard.FromName ("Main", null);
			BAConstraintDaysMonthViewController view = new BAConstraintDaysMonthViewController ();
			view.data = dom;
			view.ObjChangeController = this;
			view.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
			NavigationController.PresentViewController (view, true, null);


//			ConstraintDaysMonthViewController viewController = storyboard.InstantiateViewController("ConstraintDaysMonthViewController") as ConstraintDaysMonthViewController;
//			viewController.data = dom;
//			viewController.ObjChangeController = this;
//			viewController.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
//			NavigationController.PresentViewController (viewController, true, null);
			//NavigationController.PresentViewController
//			NavigationController.pre(viewController, true);	
		}

		public void ViewDismissViewControllerfromOvernightCities(CitiesOvernightsViewController viewController)
		{
			viewController.DismissViewController (true, null);
			ReLoadData ();

		}
		public void ViewDismissViewControllerfromCommutableLine(CommuteLinesViewController viewController)
		{
			viewController.DismissViewController (true, null);
			ReLoadData ();
		}
		public void ViewDismissViewControllerfromCommutabilityLine(CommutabilityLinesViewController viewController)
		{
			viewController.DismissViewController (true, null);
			ReLoadData ();
		}

		public void ViewDismissViewController(BAConstraintDaysMonthViewController viewController)
		{
			viewController.DismissViewController (true, null);
			ReLoadData ();
		}
		public void ReLoadData()
		{
			//this.NavigationController.SetNavigationBarHidden (false, false);
			//NavigationController.NavigationBar.BarTintColor = UIColor.White;
			//NavigationController.NavigationBar.Translucent = false;
			//NavigationItem.HidesBackButton = true;
//			NavigationItem.RightBarButtonItem = new UIBarButtonItem ("Done",UIBarButtonItemStyle.Done, ((sender, e) => {
//				if (SharedObject.Instance.ListConstraint != null &&  SharedObject.Instance.ListConstraint.Count > 0) {
//					SortViewController sortView = Storyboard.InstantiateViewController("SortViewController") as SortViewController;
//					NavigationController.PushViewController(sortView, true);	
//				}
//			}));
//			NavigationItem.LeftBarButtonItems= new UIBarButtonItem[]{
//				new UIBarButtonItem ("+", UIBarButtonItemStyle.Plain,(sender,args) => {
//					//AddConstraintClick();
//				}),
//				new UIBarButtonItem ("Clear All", UIBarButtonItemStyle.Bordered,(sender,args) => {
//					// remove all items in list
//					SharedObject.Instance.ListConstraint.Clear();
//					TableView.ReloadData();
//				})};
//			//
			TableView.TableFooterView = new UIView ();
			TableView.SetEditing (true, true);
			if (noConstraintView == null) {
				NoConstraintVC noConstraintViewVC = Storyboard.InstantiateViewController ("NoConstraintVC") as NoConstraintVC;
				noConstraintViewVC.View.Frame = new CGRect (0, 0, 482, 674);
				noConstraintView = noConstraintViewVC.View;

				//this.NavigationController.View.InsertSubviewBelow (noConstraintView, this.NavigationController.NavigationBar);
				//this.View.AddSubview (noConstraintView);
			}
			if (SharedObject.Instance.ListConstraint.Count > 0) {
				// hide intro view
				noConstraintView.Hidden = true;
				TableView.Hidden = false;
			} else {
				// show intro view;
				noConstraintView.Hidden = false;
				TableView.Hidden = true;
			}
			TableView.ReloadData ();
		}
		/*
		 * Tableview methods
		 */
		public void ClearButtonClicked (NSNotification n)
		{
			SharedObject.Instance.ListConstraint.Clear();

			TableView.ReloadData();
		}
		public void AddConstraintClick(NSNotification n)
		{
								ConstraintModalView modal = new ConstraintModalView(this);
//								modal.ProvidesPresentationContextTransitionStyle = true;
//								modal.ModalPresentationStyle  = UIModalPresentationStyle.OverCurrentContext;
//								NavigationController.PresentViewController(modal,true, ()=>{});

			UIPopoverController popoverController= new UIPopoverController(modal);
			CGRect frame = new CGRect(0 ,-20 ,300,518);
			popoverController.PopoverContentSize = new CGSize (300, 518);
			popoverController.PresentFromRect(frame,View,0,true);

		}
		public override bool CanEditRow (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			return true;
		}
		public override bool CanMoveRow (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			return true;
		}
		public override UITableViewCellEditingStyle EditingStyleForRow (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			return UITableViewCellEditingStyle.None;
		}
		public override bool ShouldIndentWhileEditing (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			return false;
		}
		public override void MoveRow (UITableView tableView, Foundation.NSIndexPath sourceIndexPath, Foundation.NSIndexPath destinationIndexPath)
		{
			//base.MoveRow (tableView, sourceIndexPath, destinationIndexPath);

			//---- get a reference to the item
			var item = SharedObject.Instance.ListConstraint[sourceIndexPath.Row];
			int deleteAt = sourceIndexPath.Row;

			//---- if we're moving within the same section, and we're inserting it before
			if ((sourceIndexPath.Section == destinationIndexPath.Section) && (destinationIndexPath.Row < sourceIndexPath.Row)) {
				//---- add one to where we delete, because we're increasing the index by inserting
				deleteAt = sourceIndexPath.Row + 1;
			}
				//---- copy the item to the new location
			SharedObject.Instance.ListConstraint.Insert(destinationIndexPath.Row, item);
				//---- remove from the old
			SharedObject.Instance.ListConstraint.RemoveAt(deleteAt);
		}
		public override nfloat GetHeightForRow (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			var currentElement = SharedObject.Instance.ListConstraint.ElementAt (indexPath.Row);
			if (currentElement is FtCommutableLine)
			{
				return 50;
			}

			return _heightRow;
		}
		private bool CheckFilterAlreadyAdded(string filterName)
		{
			bool exist = false;

			if (SharedObject.Instance.ListConstraint != null && SharedObject.Instance.ListConstraint.Count > 0)
				exist = SharedObject.Instance.ListConstraint.Any(x => x.GetType().Name == filterName);
			return exist;
		}

	}
}


