using System;
using UIKit;
using WBid.WBidiPad.Core;
using System.Linq;

namespace WBid.WBidiPad.iOS
{
	public class CitiesPickerSource: UICollectionViewSource
	{
		CitiesPickerVC _parentVC;
		public CitiesPickerSource (CitiesPickerVC pr)
		{
			_parentVC = pr;

		}

		public override nint NumberOfSections (UICollectionView collectionView)
		{
			return 1;
		}
		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return Constants.listCities.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, Foundation.NSIndexPath indexPath)
		{
			CitiesPickerCell cell = (CitiesPickerCell)collectionView.DequeueReusableCell (CitiesPickerCell.Key, indexPath);
			cell.FillData (Constants.listCities [indexPath.Row]);
			return cell;
		}

		public override void ItemSelected (UICollectionView collectionView, Foundation.NSIndexPath indexPath)
		{
			CitiesPickerCell cell = (CitiesPickerCell)collectionView.DequeueReusableCell (CitiesPickerCell.Key, indexPath);
			cell.changeColor ();
			_parentVC.ItemSelected (Constants.listCities [indexPath.Row]);

		}
	}
}

