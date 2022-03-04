using System;
using UIKit;
using WBid.WBidiPad.Model;
namespace WBid.WBidiPad.iOS
{
	public class CitiesOvernightSource: UICollectionViewSource
	{
		OvernightCitiesCx _data;
		public CitiesOvernightSource (OvernightCitiesCx data)
		{
			_data = data;
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
			CityCell cell = (CityCell)collectionView.DequeueReusableCell (CityCell.Key, indexPath);
			cell.FillData (Constants.listCities [indexPath.Row], _data);
			return cell;
		}

	}
}

