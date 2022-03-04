using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Model;
using System.Collections.ObjectModel;


namespace WBid.WBidiPad.iOS
{
	public class CalenderPopoverController : UICollectionViewController
	{
		public CalenderPopoverController (UICollectionViewLayout layout) : base (layout)
		{
		}
		NSObject notif;
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
			CollectionView.RegisterClassForCell (typeof(CalenderPopCell), CalenderPopCell.Key);
			
			// Note: If you use one of the Collection View Cell templates to create a new cell type,
			// you can register it using the RegisterNibForCell() method like this:
			//
			// CollectionView.RegisterNibForCell (MyCollectionViewCell.Nib, MyCollectionViewCell.Key);
			this.View.BackgroundColor = UIColor.White;
			notif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString("ReloadCal"), handleReloadCalendar);
		}
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			NSNotificationCenter.DefaultCenter.RemoveObserver (notif);
		}
		public override nint NumberOfSections (UICollectionView collectionView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			// TODO: return the actual number of items in the section
			return CommonClass.calData.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			collectionView.RegisterNibForCell (UINib.FromName ("CalenderPopCell", NSBundle.MainBundle),new NSString ("CalenderPopCell"));
			var cell = collectionView.DequeueReusableCell (CalenderPopCell.Key, indexPath) as CalenderPopCell;
			
			// TODO: populate the cell with the appropriate data based on the indexPath
			cell.BackgroundColor = UIColor.White;
			cell.bindData (CommonClass.calData [indexPath.Row]);
			return cell;
		}
		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			string trip = CommonClass.calData [indexPath.Row].TripNumber;
            string ss = CommonClass.selectedTrip;
			if (CommonClass.calData [indexPath.Row].TripNumber != null) {
				NSNotificationCenter.DefaultCenter.PostNotificationName ("TripPopShow", new NSString (trip));
				CommonClass.selectedTrip = trip;
				CommonClass.isLastTrip = CommonClass.calData [indexPath.Row].IsLastTrip;
			} else {
				NSNotificationCenter.DefaultCenter.PostNotificationName ("TripPopHide", null);
				CommonClass.selectedTrip = null;
				CommonClass.isLastTrip = false;
			}
			collectionView.ReloadData ();
		}
		public void handleReloadCalendar (NSNotification n){
			CollectionView.ReloadData ();
		}
	}
}

