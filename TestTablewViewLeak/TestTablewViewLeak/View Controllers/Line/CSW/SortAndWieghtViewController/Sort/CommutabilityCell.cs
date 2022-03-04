using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using System.Linq;
using System.Collections.Generic;

namespace WBid.WBidiPad.iOS
{
    public partial class CommutabilityCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString ("CommutabilityCell");
        public static readonly UINib Nib;
        NSObject changeContraintSecondcellNotif;
        UIPopoverController popoverController;

        NSObject arrObserver; 
        NSIndexPath path;
        WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
        static CommutabilityCell ()
        {
            Nib = UINib.FromName ("CommutabilityCell", NSBundle.MainBundle);
        }


        public void bindData (NSIndexPath indexpath)
        {
            this.btnRemove.Tag = indexpath.Row;
            if (wBIdStateContent.SortDetails.BlokSort.Contains ("33") || wBIdStateContent.SortDetails.BlokSort.Contains("36")) {
                this.btnValue.SetTitle ("Overall", UIControlState.Normal);
            
            } else if (wBIdStateContent.SortDetails.BlokSort.Contains ("34") || wBIdStateContent.SortDetails.BlokSort.Contains("37")) {
                this.btnValue.SetTitle ("Front", UIControlState.Normal);
            } else if (wBIdStateContent.SortDetails.BlokSort.Contains ("35") || wBIdStateContent.SortDetails.BlokSort.Contains("38")) {
                this.btnValue.SetTitle ("Back", UIControlState.Normal);
            }

            if (wBIdStateContent.SortDetails.BlokSort.Contains("33") || wBIdStateContent.SortDetails.BlokSort.Contains("34") || wBIdStateContent.SortDetails.BlokSort.Contains("35"))
            {
                this.btnTitle.SetTitle("comut %(" + wBIdStateContent.Constraints.CLAuto.City + ")", UIControlState.Normal);
            }
            else
            {
                this.btnTitle.SetTitle("comut Line-Manual", UIControlState.Normal);
            }



        }
        protected CommutabilityCell (IntPtr handle) : base (handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
        partial void btnRemoveTapped (UIKit.UIButton sender)
        {
            if (wBIdStateContent.SortDetails.BlokSort.Count > sender.Tag) {
                wBIdStateContent.SortDetails.BlokSort.RemoveAt ((int)sender.Tag);
                NSNotificationCenter.DefaultCenter.PostNotificationName ("reloadBlockSort", null);
            }
        }
         partial void btnTitleTapped (NSObject sender)
        {

            if (wBIdStateContent.SortDetails.BlokSort.Contains("33") || wBIdStateContent.SortDetails.BlokSort.Contains("34") || wBIdStateContent.SortDetails.BlokSort.Contains("35"))
            {
 
                //commutable line auto pop up display
                NSNotificationCenter.DefaultCenter.PostNotificationName("ShowCommutableAutoSortPopUp", null);
            }
            else
            {
                //commutable line manual pop up display
                NSNotificationCenter.DefaultCenter.PostNotificationName("ShowCommutableManualSortPopUp", null);
            }

        }

       



        partial void btnValueTapped (UIKit.UIButton sender)
        {
            arrObserver=NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("dismissSortPopover"), dismissPopover);
            PopoverViewController popoverContent = new PopoverViewController ();
            popoverContent.PopType = "changeValueCommutabilitySort";
            //popoverContent.SubPopType = ConstraintsApplied.MainList [path.Section];
            //popoverContent.index = (int)this.Tag;
            popoverController = new UIPopoverController (popoverContent);
            popoverController.PopoverContentSize = new CGSize (125, 175);
            popoverController.PresentFromRect (sender.Frame, this, UIPopoverArrowDirection.Any, true); 
        }
        private void dismissPopover (NSNotification n)
        {
            popoverController.Dismiss (true);
            NSNotificationCenter.DefaultCenter.RemoveObserver (arrObserver);
        }
        
    }
}
