// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace WBid.WBidiPad.iOS
{
    [Register ("BidEditorForFA")]
    partial class BidEditorForFA
    {
        [Outlet]
        UIKit.UIButton btnBidSubmit { get; set; }


        [Outlet]
        UIKit.UIButton btnChangeBuddyBid { get; set; }


        [Outlet]
        UIKit.UIButton btnChangeEmp { get; set; }


        [Outlet]
        UIKit.UIButton btnCtrl { get; set; }


        [Outlet]
        UIKit.UIButton btnFirstChoice { get; set; }


        [Outlet]
        UIKit.UIButton btnFourthChoice { get; set; }


        [Outlet]
        UIKit.UIButton btnManualEntry { get; set; }


        [Outlet]
        UIKit.UIButton btnReserve { get; set; }


        [Outlet]
        UIKit.UIButton btnSaveClose { get; set; }


        [Outlet]
        UIKit.UIButton btnSecondChoice { get; set; }


        [Outlet]
        UIKit.UIButton btnShift { get; set; }


        [Outlet]
        UIKit.UIButton btnThirdChoice { get; set; }


        [Outlet]
        UIKit.UILabel lblAvailableCount { get; set; }


        [Outlet]
        UIKit.UILabel lblAvoidanceHeader { get; set; }


        [Outlet]
        UIKit.UILabel lblAvoidanceText { get; set; }


        [Outlet]
        UIKit.UILabel lblBidCount { get; set; }


        [Outlet]
        UIKit.UILabel lblBuddy1 { get; set; }


        [Outlet]
        UIKit.UILabel lblBuddy2 { get; set; }


        [Outlet]
        UIKit.UILabel lblBuddyBidHeader { get; set; }


        [Outlet]
        UIKit.UILabel lblBUddyBidText { get; set; }


        [Outlet]
        UIKit.UILabel lblQueryHeader { get; set; }


        [Outlet]
        UIKit.UIPickerView pckFirstBIddyBidderPositionSelection { get; set; }


        [Outlet]
        UIKit.UIPickerView pckSecondBUddyBidderPositionSelection { get; set; }


        [Outlet]
        UIKit.UITableView tblAvailableLines { get; set; }


        [Outlet]
        UIKit.UITableView tblSelectedLines { get; set; }


        [Outlet]
        UIKit.UITextField txtAvoidance { get; set; }


        [Outlet]
        UIKit.UITextField txtBidChoice { get; set; }


        [Outlet]
        UIKit.UITextField txtBuddyBid { get; set; }


        [Outlet]
        UIKit.UITextField txtSubmitBid { get; set; }


        [Outlet]
        UIKit.UIView vwAvoidance { get; set; }


        [Outlet]
        UIKit.UIView vwBuddyBid { get; set; }


        [Outlet]
        UIKit.UIView vwSubmitBidChoiceFor { get; set; }


        [Action ("btnChangeBuddyBidderTapped:")]
        partial void btnChangeBuddyBidderTapped (Foundation.NSObject sender);


        [Action ("btnAddReserve:")]
        partial void btnAddReserve (Foundation.NSObject sender);


        [Action ("btnEmployeeNumberTapped:")]
        partial void btnEmployeeNumberTapped (Foundation.NSObject sender);


        [Action ("btnCancelClearTapped:")]
        partial void btnCancelClearTapped (Foundation.NSObject sender);


        [Action ("btnSubmitBidCancel:")]
        partial void btnSubmitBidCancel (Foundation.NSObject sender);


        [Action ("btnFirstChoiceTapped:")]
        partial void btnFirstChoiceTapped (Foundation.NSObject sender);


        [Action ("btnSecondChoiceTapped:")]
        partial void btnSecondChoiceTapped (Foundation.NSObject sender);


        [Action ("btnThirdChoiceTapped:")]
        partial void btnThirdChoiceTapped (Foundation.NSObject sender);


        [Action ("btnFourthChoiceTapped:")]
        partial void btnFourthChoiceTapped (Foundation.NSObject sender);


        [Action ("dismiss:")]
        partial void dismiss (UIKit.UIBarButtonItem sender);


        [Action ("btnManualEntryTapped:")]
        partial void btnManualEntryTapped (UIKit.UIButton sender);


        [Action ("btnAdd:")]
        partial void btnAdd (Foundation.NSObject sender);


        [Action ("btnInsert:")]
        partial void btnInsert (Foundation.NSObject sender);


        [Action ("btnRemove:")]
        partial void btnRemove (Foundation.NSObject sender);


        [Action ("btnRepeatA:")]
        partial void btnRepeatA (Foundation.NSObject sender);


        [Action ("btnRepeatB:")]
        partial void btnRepeatB (Foundation.NSObject sender);


        [Action ("btnRepeatC:")]
        partial void btnRepeatC (Foundation.NSObject sender);


        [Action ("btnBidSubmitTapped:")]
        partial void btnBidSubmitTapped (UIKit.UIButton sender);


        [Action ("btnSaveCloseTapped:")]
        partial void btnSaveCloseTapped (Foundation.NSObject sender);


        [Action ("btnShiftTapped:")]
        partial void btnShiftTapped (Foundation.NSObject sender);


        [Action ("btnControlTapped:")]
        partial void btnControlTapped (Foundation.NSObject sender);


        [Action ("btnClear:")]
        partial void btnClear (Foundation.NSObject sender);


        [Action ("btnSubmit:")]
        partial void btnSubmit (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}