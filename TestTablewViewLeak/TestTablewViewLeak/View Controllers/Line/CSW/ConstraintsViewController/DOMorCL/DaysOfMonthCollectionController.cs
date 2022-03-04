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


namespace WBid.WBidiPad.iOS
{
	public class DaysOfMonthCollectionController : UICollectionViewController
	{
		class MyPopDelegate : UIPopoverControllerDelegate
		{
			DaysOfMonthCollectionController _parent;
			public MyPopDelegate (DaysOfMonthCollectionController parent)
			{
				_parent = parent;
			}
			public override void DidDismiss (UIPopoverController popoverController)
			{
				_parent.popoverController = null;
				NSNotificationCenter.DefaultCenter.RemoveObserver (_parent.changeDOMWeightNotif);
			}
		}

		List<DayOfMonth> lstDaysOfMonth = ConstraintBL.GetDaysOfMonthList();
		WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		ConstraintCalculations constCalc = new ConstraintCalculations ();
		public string DisplayMode;
		NSObject changeDOMWeightNotif;
		UIPopoverController popoverController;

		public DaysOfMonthCollectionController (UICollectionViewLayout layout) : base (layout)
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
			
			// Register any custom UICollectionViewCell classes
			CollectionView.RegisterClassForCell (typeof(DaysOfMonthColl), DaysOfMonthColl.Key);
			
			// Note: If you use one of the Collection View Cell templates to create a new cell type,
			// you can register it using the RegisterNibForCell() method like this:
			//
			// CollectionView.RegisterNibForCell (MyCollectionViewCell.Nib, MyCollectionViewCell.Key);
			this.View.BackgroundColor = UIColor.Clear;

			List<int> offDays = wBIdStateContent.Constraints.DaysOfMonth.OFFDays;
			if (offDays != null) {
				foreach (var item in offDays) {
					lstDaysOfMonth.FirstOrDefault (x => x.Id == item).Status = 2;
				}
			}
			List<int> workDays = wBIdStateContent.Constraints.DaysOfMonth.WorkDays;
			if (workDays != null) {
				foreach (var item in workDays) {
					lstDaysOfMonth.FirstOrDefault (x => x.Id == item).Status = 1;
				}
			}
		}

		public override nint NumberOfSections (UICollectionView collectionView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			// TODO: return the actual number of items in the section
			return lstDaysOfMonth.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			collectionView.RegisterNibForCell (UINib.FromName ("DaysOfMonthColl", NSBundle.MainBundle),new NSString ("DaysOfMonthColl"));
			var cell = collectionView.DequeueReusableCell (DaysOfMonthColl.Key, indexPath) as DaysOfMonthColl;

			// TODO: populate the cell with the appropriate data based on the indexPath
			cell.DisplayMode = DisplayMode;
			cell.bindData (lstDaysOfMonth [indexPath.Row]);
			return cell;
		}

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			WBidHelper.PushToUndoStack ();
			if (DisplayMode == "Constraints") {
				DayOfMonth dom = lstDaysOfMonth [indexPath.Row];
				if (dom.Day != null) {
					if (wBIdStateContent.Constraints.DaysOfMonth.OFFDays == null)
						wBIdStateContent.Constraints.DaysOfMonth.OFFDays = new List<int> ();
					if (wBIdStateContent.Constraints.DaysOfMonth.WorkDays == null)
						wBIdStateContent.Constraints.DaysOfMonth.WorkDays = new List<int> ();
					if (dom.Status == 0) {
						dom.Status = 2;
						wBIdStateContent.Constraints.DaysOfMonth.OFFDays.Add (dom.Id);
						if (wBIdStateContent.Constraints.DaysOfMonth.WorkDays.Contains (dom.Id))
							wBIdStateContent.Constraints.DaysOfMonth.WorkDays.Remove (dom.Id);
					} else if (dom.Status == 2) {
						dom.Status = 1;
						wBIdStateContent.Constraints.DaysOfMonth.WorkDays.Add (dom.Id);
						if (wBIdStateContent.Constraints.DaysOfMonth.OFFDays.Contains (dom.Id))
							wBIdStateContent.Constraints.DaysOfMonth.OFFDays.Remove (dom.Id);
					} else if (dom.Status == 1) {
						dom.Status = 0;
						wBIdStateContent.Constraints.DaysOfMonth.OFFDays.Remove (dom.Id);
						wBIdStateContent.Constraints.DaysOfMonth.WorkDays.Remove (dom.Id);
					}
					collectionView.ReloadData ();
					constCalc.ApplyDaysOfMonthConstraint (wBIdStateContent.Constraints.DaysOfMonth);
					NSNotificationCenter.DefaultCenter.PostNotificationName ("LineCountReload", null);
				}

			}else {
				DayOfMonth dom = lstDaysOfMonth [indexPath.Row];
				List<Wt> lstWeight = wBIdStateContent.Weights.SDO.Weights;

				if (dom.Day != null) {
					changeDOMWeightNotif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString("changeWeightParamInDOMCell"), handleDOMWeightChange);
					PopoverViewController popoverContent = new PopoverViewController ();
					popoverContent.PopType = "changeWeightParamInDOMCell";
					popoverContent.SubPopType = "Days of the Month";
					if (lstWeight.Any (x => x.Key == dom.Id)) {
						popoverContent.numValue = lstWeight.FirstOrDefault (x => x.Key == dom.Id).Value.ToString ();
					} else {
						popoverContent.numValue = "0";
					}
					popoverContent.index = dom.Id;
					popoverController = new UIPopoverController (popoverContent);
					popoverController.Delegate = new MyPopDelegate (this);
					popoverController.PopoverContentSize = new CGSize (210, 300);
					UICollectionViewCell cell = collectionView.CellForItem (indexPath);
					popoverController.PresentFromRect(cell.Frame,collectionView,UIPopoverArrowDirection.Any,true);

				}
			}
		}
		public void handleDOMWeightChange (NSNotification n)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (changeDOMWeightNotif);
			popoverController.Dismiss (true);
		}
	}
}

