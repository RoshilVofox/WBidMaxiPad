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
using WBid.WBidiPad.PortableLibrary;
using System.Collections.ObjectModel;
namespace WBid.WBidiPad.iOS
{
	public partial class BAblockSortCollectionCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("BAblockSortCollectionCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("BAblockSortCollectionCell");
		WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		public SortTempValues BASort ;
		public BAblockSortCollectionCell (IntPtr handle) : base (handle)
		{
		}

		public static BAblockSortCollectionCell Create ()
		{
			return (BAblockSortCollectionCell)Nib.Instantiate (null, null) [0];
		}
		public void bindData (NSIndexPath indexpath) {
			this.btnRemove.Tag = indexpath.Row;
			ObservableCollection<BlockSort> lstblockData = WBidCollection.GetBlockSortListData ();
			string value = BASort.BlokSort [indexpath.Row];
			this.lblTitle.Text = lstblockData.FirstOrDefault (x => x.Id.ToString() == value).Name;

		}
		partial void btnRemoveTapped (UIKit.UIButton sender)
		{
			WBidHelper.PushToUndoStack ();
			if(BASort.BlokSort.Count>sender.Tag){
				BASort.BlokSort.RemoveAt((int)sender.Tag);
				NSNotificationCenter.DefaultCenter.PostNotificationName("BAreloadBlockSort",null);
			}
		}
	}
}

