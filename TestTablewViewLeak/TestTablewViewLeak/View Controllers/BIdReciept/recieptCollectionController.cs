using System;
using CoreGraphics;
using Foundation;
using UIKit;
using System.Collections.Generic;

namespace WBid.WBidiPad.iOS
{
	public class recieptCollectionController : UICollectionViewController
	{
		List<string> recieptFileNames;
		BidRecieptViewController parentVC;
		public recieptCollectionController (UICollectionViewLayout layout, List<string> linefilenames, BidRecieptViewController parent) : base (layout)
		{
			recieptFileNames = linefilenames;
			parentVC = parent;
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
			CollectionView.RegisterClassForCell (typeof(recieptCollectionCell), recieptCollectionCell.Key);

			this.CollectionView.BackgroundColor = UIColor.White;
			this.View.Layer.BorderWidth = 2;
			this.View.Layer.BorderColor = ColorClass.SummaryHeaderBorderColor.CGColor;
			this.View.Layer.CornerRadius = 3;

			// Note: If you use one of the Collection View Cell templates to create a new cell type,
			// you can register it using the RegisterNibForCell() method like this:
			//
			// CollectionView.RegisterNibForCell (MyCollectionViewCell.Nib, MyCollectionViewCell.Key);
		}

		public override nint NumberOfSections (UICollectionView collectionView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			// TODO: return the actual number of items in the section
			return recieptFileNames.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.DequeueReusableCell (recieptCollectionCell.Key, indexPath) as recieptCollectionCell;
			cell.title = new NSString(recieptFileNames[indexPath.Row]);
			// TODO: populate the cell with the appropriate data based on the indexPath
			
			return cell;
		}
		public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
		{
			parentVC.selectedFileName = recieptFileNames [indexPath.Row];

			if (parentVC.isPrintView)
				CommonClass.PrintReceipt (collectionView.CellForItem(indexPath).ContentView,recieptFileNames [indexPath.Row]);
			else 
				parentVC.ShowReceipt() ;

		}
	}
}

