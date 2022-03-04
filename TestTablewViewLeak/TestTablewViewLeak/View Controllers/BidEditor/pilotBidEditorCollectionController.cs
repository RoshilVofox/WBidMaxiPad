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

namespace WBid.WBidiPad.iOS
{
	public class pilotBidEditorCollectionController : UICollectionViewController
	{
		BidEditorForPilot parent;
		public List <string> availableLines;
		public pilotBidEditorCollectionController (UICollectionViewLayout layout, BidEditorForPilot pilot) : base (layout)
		{
			parent = pilot;
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		public override void ViewWillDisappear(bool animated)
		{
			
			base.ViewWillDisappear(animated);
			foreach (UIView view in this.View.Subviews) {

				DisposeClass.DisposeEx(view);
			}

		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			CollectionView.BackgroundColor = UIColor.White;
			availableLines = new List<string>();
			// Register any custom UICollectionViewCell classes
			CollectionView.RegisterClassForCell (typeof(pilotBidEditorCollectionCell), pilotBidEditorCollectionCell.Key);

			
            if (GlobalSettings.BidPrepDetails.IsOnStartWithCurrentLine ||(GlobalSettings.CurrentBidDetails.Postion=="FA" &&  GlobalSettings.CurrentBidDetails.Round=="S"))
            {
                int count = GlobalSettings.BidPrepDetails.LineFrom;
                foreach (Line line in GlobalSettings.Lines)
                {
                    availableLines.Add(count.ToString());
                    count++;
                }
            }
            else
            {
                for (int count = GlobalSettings.BidPrepDetails.LineFrom; count <= GlobalSettings.BidPrepDetails.LineTo; count++)
                {
                    availableLines.Add(count.ToString());
                }
            }
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
			return availableLines.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.DequeueReusableCell (pilotBidEditorCollectionCell.Key, indexPath) as pilotBidEditorCollectionCell;

			// TODO: populate the cell with the appropriate data based on the indexPath


			string aLine = availableLines[indexPath.Row];

			if (parent.selectedLines.Contains (aLine)) {
				cell.BackgroundColor = ColorClass.activeOrange;
			} else {
				cell.BackgroundColor = UIColor.White;
			}
			cell.title = new NSString(availableLines[indexPath.Row]);
			return cell;
		}
		public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
		{

			//Line aLine = GlobalSettings.Lines [indexPath.Row];
			pilotBidEditorCollectionCell cell = (pilotBidEditorCollectionCell)collectionView.CellForItem (indexPath);

			if (!parent.selectedLines.Contains (availableLines[indexPath.Row])) {
				parent.selectedLines.Add (availableLines[indexPath.Row]);

				cell.BackgroundColor = ColorClass.activeOrange;
			} else {
				parent.selectedLines.Remove (availableLines[indexPath.Row]);
				cell.BackgroundColor = UIColor.White;
			}
			parent.tblSelectedLInes.ReloadData ();
			parent.setBidCountLabel ();
		}
	}
}

