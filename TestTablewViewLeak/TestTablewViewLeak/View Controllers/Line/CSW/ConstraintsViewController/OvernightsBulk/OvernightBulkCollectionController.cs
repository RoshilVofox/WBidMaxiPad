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
using System.IO;

namespace WBid.WBidiPad.iOS
{
	public class OvernightBulkCollectionController : UICollectionViewController
	{
//		class MyPopDelegate : UIPopoverControllerDelegate
//		{
//			OvernightBulkCollectionController _parent;
//			public MyPopDelegate (OvernightBulkCollectionController parent)
//			{
//				_parent = parent;
//			}
//			public override void DidDismiss (UIPopoverController popoverController)
//			{
//				_parent.popoverController = null;
//				NSNotificationCenter.DefaultCenter.RemoveObserver (_parent.overnightBulkWeightNotif);
//			}
//		}

		public string DisplayMode;
		WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		ConstraintCalculations constCalc = new ConstraintCalculations ();
		//List<City> lstOvernightCities = GlobalSettings.OverNightCitiesInBid;
		List<City> lstOvernightCities= GlobalSettings.WBidINIContent.Cities.OrderBy(x=>x.Name).ToList();
//		NSObject overnightBulkWeightNotif;
//		UIPopoverController popoverController;

		public OvernightBulkCollectionController (UICollectionViewLayout layout,string displaymode) : base (layout)
		{
			DisplayMode = displaymode;
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
			lstOvernightCities = GlobalSettings.WBidINIContent.Cities.OrderBy(x=>x.Name).ToList();

				//lstOvernightCities = GlobalSettings.OverNightCitiesInBid;
			CollectionView.BackgroundColor = UIColor.White;
			// Register any custom UICollectionViewCell classes
			CollectionView.RegisterClassForCell (typeof(OvernightBulkColl), OvernightBulkColl.Key);

			// Note: If you use one of the Collection View Cell templates to create a new cell type,
			// you can register it using the RegisterNibForCell() method like this:
			//
			// CollectionView.RegisterNibForCell (MyCollectionViewCell.Nib, MyCollectionViewCell.Key);
			//lstOvernightCities.Clear();
			//foreach (var city in GlobalSettings.WBidINIContent.Cities)
			//{
			//	if (GlobalSettings.OverNightCitiesInBid.Any(x => x.Id == city.Id))
			//	{
			//		lstOvernightCities.Add(new City() { Id = city.Id,Code=city.Code,dst=city.dst,International=city.International,Name=city.Name,NonConus=city.NonConus,Status=0});
			//	}
			//	else
			//	{
			//		lstOvernightCities.Add(new City() { Id = city.Id, Code = city.Code, dst = city.dst, International = city.International, Name = city.Name, NonConus = city.NonConus, Status = 3 });
			//	}
			//}
			foreach (var item in lstOvernightCities) 
			{
				if (GlobalSettings.OverNightCitiesInBid.Any(x => x.Id == item.Id))
					item.Status = 0;
				else
					item.Status = 3;
			}
			List<int> noDays = wBIdStateContent.Constraints.BulkOvernightCity.OverNightNo;
			if (noDays != null) {
				foreach (var item in noDays) {
					lstOvernightCities.FirstOrDefault (x => x.Id == item).Status = 2;
				}
			}
			List<int> yesDays = wBIdStateContent.Constraints.BulkOvernightCity.OverNightYes;
			if (yesDays != null) {
				foreach (var item in yesDays) {
					lstOvernightCities.FirstOrDefault (x => x.Id == item).Status = 1;
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
			return lstOvernightCities.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			collectionView.RegisterNibForCell (UINib.FromName ("OvernightBulkColl", NSBundle.MainBundle),new NSString ("OvernightBulkColl"));
			var cell = collectionView.DequeueReusableCell (OvernightBulkColl.Key, indexPath) as OvernightBulkColl;

			// TODO: populate the cell with the appropriate data based on the indexPath
			cell.DisplayMode = DisplayMode;
			cell.bindData (lstOvernightCities [indexPath.Row]);
			return cell;
		}

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			WBidHelper.PushToUndoStack ();
			if (DisplayMode == "Constraints") {
				City city = lstOvernightCities [indexPath.Row];
				if (city.Status == 0) {
					city.Status = 2;
					wBIdStateContent.Constraints.BulkOvernightCity.OverNightNo.Add (city.Id);
				} else if (city.Status == 2) {
					city.Status = 1;
					wBIdStateContent.Constraints.BulkOvernightCity.OverNightNo.Remove (city.Id);
					wBIdStateContent.Constraints.BulkOvernightCity.OverNightYes.Add (city.Id);
				} else if (city.Status == 1) {
					city.Status = 0;
					wBIdStateContent.Constraints.BulkOvernightCity.OverNightYes.Remove (city.Id);
				}

				collectionView.ReloadData ();
				constCalc.ApplyOvernightBulkConstraint (wBIdStateContent.Constraints.BulkOvernightCity);
				NSNotificationCenter.DefaultCenter.PostNotificationName ("LineCountReload", null);

			} else {
//				City city = lstOvernightCities [indexPath.Row];
//				List<Wt2Parameter> lstWeight = wBIdStateContent.Weights.OvernightCitybulk;
//
//				overnightBulkWeightNotif = NSNotificationCenter.DefaultCenter.AddObserver ("changeOvernightBulkWeight", handleOvernightBulkWeight);
//				PopoverViewController popoverContent = new PopoverViewController ();
//				popoverContent.PopType = "changeOvernightBulkWeight";
//				popoverContent.SubPopType = "Overnight Cities - Bulk";
//				if (lstWeight.Any (x => x.Type == city.Id)) {
//					popoverContent.numValue = lstWeight.FirstOrDefault (x => x.Type == city.Id).Weight.ToString ();
//				} else {
//					popoverContent.numValue = "0";
//				}
//				popoverContent.index = city.Id;
//				popoverController = new UIPopoverController (popoverContent);
//				popoverController.Delegate = new MyPopDelegate (this);
//				popoverController.PopoverContentSize = new SizeF (210, 300);
//				UICollectionViewCell cell = collectionView.CellForItem (indexPath);
//				popoverController.PresentFromRect(cell.Frame,collectionView,UIPopoverArrowDirection.Any,true);
			}

		}

//		void handleOvernightBulkWeight (NSNotification obj)
//		{
//			NSNotificationCenter.DefaultCenter.RemoveObserver (overnightBulkWeightNotif);
//			popoverController.Dismiss (true);
//		}
	}
}

