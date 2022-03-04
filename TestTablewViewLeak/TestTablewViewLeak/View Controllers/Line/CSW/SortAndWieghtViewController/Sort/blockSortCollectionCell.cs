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
	public partial class blockSortCollectionCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("blockSortCollectionCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("blockSortCollectionCell");
		WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

		public blockSortCollectionCell (IntPtr handle) : base (handle)
		{
		}

		public static blockSortCollectionCell Create ()
		{
			return (blockSortCollectionCell)Nib.Instantiate (null, null) [0];
		}
		public void bindData (NSIndexPath indexpath) {
			this.btnRemove.Tag = indexpath.Row;
			ObservableCollection<BlockSort> lstblockData = WBidCollection.GetBlockSortListDataForInternalCalculation();
			string value = wBIdStateContent.SortDetails.BlokSort [indexpath.Row];
			var data = lstblockData.FirstOrDefault (x => x.Id.ToString () == value);
			if (data != null) {
				this.lblTitle.Text = lstblockData.FirstOrDefault (x => x.Id.ToString () == value).Name;
			}

		}
		partial void btnRemoveTapped (UIKit.UIButton sender)
		{
			WBidHelper.PushToUndoStack ();
			if(wBIdStateContent.SortDetails.BlokSort.Count>sender.Tag)
            {
                var sortToremove = wBIdStateContent.SortDetails.BlokSort [(int)sender.Tag];
                if (sortToremove == "33" || sortToremove == "34" || sortToremove == "35") 
                {
                    
                    if (wBIdStateContent.CxWtState.CLAuto.Wt == false && wBIdStateContent.CxWtState.CLAuto.Cx==false)
                    {
                        ResetCommutablelinePropertiies();

                    }
                }
                else if (sortToremove == "36" || sortToremove == "37" || sortToremove == "38")
                {
                    //remove commutable manual sort. Need to reset commutable properties only consrtaints and weights are not already set
                    if (wBIdStateContent.CxWtState.CL.Wt == false && wBIdStateContent.CxWtState.CL.Cx == false)
                    {

                        ResetCommutablelinePropertiies();
                    }
                }
				wBIdStateContent.SortDetails.BlokSort.RemoveAt((int)sender.Tag);
			NSNotificationCenter.DefaultCenter.PostNotificationName("reloadBlockSort",null);
			}
		}

        private void ResetCommutablelinePropertiies()
        {
            foreach (var line in GlobalSettings.Lines)
            {
                line.CommutableBacks = 0;
                line.commutableFronts = 0;
                line.CommutabilityFront = 0;
                line.CommutabilityBack = 0;
                line.CommutabilityOverall = 0;
            }
        }
	}
}

