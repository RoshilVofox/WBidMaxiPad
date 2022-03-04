// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace WBid.WBidiPad.iOS
{
    [Register ("FAPostionChoiceViewController")]
    partial class FAPostionChoiceViewController
    {
        [Outlet]
        UIKit.UIButton btnAdd { get; set; }


        [Outlet]
        UIKit.UIButton btnCancel { get; set; }


        [Outlet]
        UIKit.UIButton btnChangeBuddy { get; set; }


        [Outlet]
        UIKit.UIButton btnClear { get; set; }


        [Outlet]
        UIKit.UIButton btnCtrl { get; set; }


        [Outlet]
        UIKit.UIButton btnCustomCancel { get; set; }


        [Outlet]
        UIKit.UIButton btnCustomOK { get; set; }


        [Outlet]
        UIKit.UIButton btnFirstPos { get; set; }


        [Outlet]
        UIKit.UIButton btnFourthPos { get; set; }


        [Outlet]
        UIKit.UIButton btnInsert { get; set; }


        [Outlet]
        UIKit.UIButton btnRemove { get; set; }


        [Outlet]
        UIKit.UIButton btnReserveCheck { get; set; }


        [Outlet]
        UIKit.UIButton btnSecondPos { get; set; }


        [Outlet]
        UIKit.UIButton btnShift { get; set; }


        [Outlet]
        UIKit.UIButton btnSubmit { get; set; }


        [Outlet]
        UIKit.UIButton btnThirdPos { get; set; }


        [Outlet]
        UIKit.UILabel lblBidCount { get; set; }


        [Outlet]
        UIKit.UILabel lblBuddy1 { get; set; }


        [Outlet]
        UIKit.UILabel lblBuddy2 { get; set; }


        [Outlet]
        UIKit.UILabel lblLinesCount { get; set; }


        [Outlet]
        UIKit.UILabel lblLinesInPosition { get; set; }


        [Outlet]
        UIKit.UILabel lblPosExistValue1 { get; set; }


        [Outlet]
        UIKit.UILabel lblPosExistValue2 { get; set; }


        [Outlet]
        UIKit.UILabel lblPosExistValue3 { get; set; }


        [Outlet]
        UIKit.UILabel lblPosExistValue4 { get; set; }


        [Outlet]
        UIKit.UILabel lblPositionsExist { get; set; }


        [Outlet]
        UIKit.UILabel lblPositionsExistValues { get; set; }


        [Outlet]
        UIKit.UILabel lblPositionTotal { get; set; }


        [Outlet]
        UIKit.UILabel lblSelectedFirstPos { get; set; }


        [Outlet]
        UIKit.UILabel lblSelectedFourthPos { get; set; }


        [Outlet]
        UIKit.UILabel lblSelectedSecondPos { get; set; }


        [Outlet]
        UIKit.UILabel lblSelectedThirdPos { get; set; }


        [Outlet]
        UIKit.UILabel lblSubmitTotal { get; set; }


        [Outlet]
        UIKit.UIPickerView pkrBuddyPos1 { get; set; }


        [Outlet]
        UIKit.UIPickerView pkrBuddyPos2 { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgCustomOrder { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgPosCombination { get; set; }


        [Outlet]
        UIKit.UITableView tblAvailable { get; set; }


        [Outlet]
        UIKit.UITableView tblBid { get; set; }


        [Outlet]
        UIKit.UITextField txtLinesInFirstPos { get; set; }


        [Outlet]
        UIKit.UITextField txtLinesInFourthPos { get; set; }


        [Outlet]
        UIKit.UITextField txtLinesInSecondPos { get; set; }


        [Outlet]
        UIKit.UITextField txtLinesInThirdPos { get; set; }


        [Outlet]
        UIKit.UIView vwCustomBid { get; set; }


        [Outlet]
        UIKit.UIView vwPositionDetails { get; set; }


        [Outlet]
        UIKit.UIView vwRepeatPositionShow { get; set; }


        [Action ("btnAddTapped:")]
        partial void btnAddTapped (UIKit.UIButton sender);


        [Action ("btnCancelTapped:")]
        partial void btnCancelTapped (UIKit.UIButton sender);


        [Action ("btnChangeBuddyTapped:")]
        partial void btnChangeBuddyTapped (UIKit.UIButton sender);


        [Action ("btnClearTapped:")]
        partial void btnClearTapped (UIKit.UIButton sender);


        [Action ("btnCtrlTapped:")]
        partial void btnCtrlTapped (UIKit.UIButton sender);


        [Action ("btnCustomCancelTapped:")]
        partial void btnCustomCancelTapped (UIKit.UIButton sender);


        [Action ("btnCustomOkTapped:")]
        partial void btnCustomOkTapped (UIKit.UIButton sender);


        [Action ("btnDismissTapped:")]
        partial void btnDismissTapped (UIKit.UIBarButtonItem sender);


        [Action ("btnFirstPosTapped:")]
        partial void btnFirstPosTapped (UIKit.UIButton sender);


        [Action ("btnFourthPosTapped:")]
        partial void btnFourthPosTapped (UIKit.UIButton sender);


        [Action ("btnInsertTapped:")]
        partial void btnInsertTapped (UIKit.UIButton sender);


        [Action ("btnRemoveTapped:")]
        partial void btnRemoveTapped (UIKit.UIButton sender);


        [Action ("btnReserveCheckTapped:")]
        partial void btnReserveCheckTapped (UIKit.UIButton sender);


        [Action ("btnSecondPosTapped:")]
        partial void btnSecondPosTapped (UIKit.UIButton sender);


        [Action ("btnShiftTapped:")]
        partial void btnShiftTapped (UIKit.UIButton sender);


        [Action ("btnSubmitTapped:")]
        partial void btnSubmitTapped (UIKit.UIButton sender);


        [Action ("btnThirdPosTapped:")]
        partial void btnThirdPosTapped (UIKit.UIButton sender);


        [Action ("sgCustomOrderTapped:")]
        partial void sgCustomOrderTapped (UIKit.UISegmentedControl sender);


        [Action ("sgPosCombinationTapped:")]
        partial void sgPosCombinationTapped (UIKit.UISegmentedControl sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}