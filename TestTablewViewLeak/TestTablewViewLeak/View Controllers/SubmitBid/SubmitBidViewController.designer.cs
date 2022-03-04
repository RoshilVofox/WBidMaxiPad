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
    [Register ("SubmitBidViewController")]
    partial class SubmitBidViewController
    {
        [Outlet]
        UIKit.UIButton btnBuddyBidderCancel { get; set; }


        [Outlet]
        UIKit.UIButton btnBuddyBidderOK { get; set; }


        [Outlet]
        UIKit.UIButton btnCancel { get; set; }


        [Outlet]
        UIKit.UIButton btnCancelChoice { get; set; }


        [Outlet]
        UIKit.UIButton btnChangeAvoidanceType { get; set; }


        [Outlet]
        UIKit.UIButton btnChangeBuddyBid { get; set; }


        [Outlet]
        UIKit.UIButton btnChangeEmp { get; set; }


        [Outlet]
        UIKit.UIButton btnChangeEmployeeCancel { get; set; }


        [Outlet]
        UIKit.UIButton btnChangeEmployeeNumber { get; set; }


        [Outlet]
        UIKit.UIButton btnChangeEmployeeOK { get; set; }


        [Outlet]
        UIKit.UIButton btnChoiceSubmit { get; set; }


        [Outlet]
        UIKit.UIButton btnEmployeesTobeVoidedCancel { get; set; }


        [Outlet]
        UIKit.UIButton btnEmployeeToBeSsvoidedOk { get; set; }


        [Outlet]
        UIKit.UIButton btnSubmitBid { get; set; }


        [Outlet]
        UIKit.UILabel lblAvoidanceBidChoice { get; set; }


        [Outlet]
        UIKit.UILabel lblBuddyBid { get; set; }


        [Outlet]
        UIKit.UILabel lblBuddyBidChoice { get; set; }


        [Outlet]
        UIKit.UILabel lblNoAvoidanceBid { get; set; }


        [Outlet]
        UIKit.UILabel lblSubmitBidForId { get; set; }


        [Outlet]
        UIKit.UILabel lblSubmittingBidChoices { get; set; }


        [Outlet]
        UIKit.UIPickerView pckrSeniorityNUmber { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgBidSubmissionType { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgBuddyBidSubmissionType { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgNoAvoidanceSubmissionType { get; set; }


        [Outlet]
        UIKit.UITextField txtAvoidanceBidChoiceId { get; set; }


        [Outlet]
        UIKit.UITextField txtBuddyBidChoiceId { get; set; }


        [Outlet]
        UIKit.UITextField txtBuddyBidderOne { get; set; }


        [Outlet]
        UIKit.UITextField txtBuddyBidderTwo { get; set; }


        [Outlet]
        UIKit.UITextField txtChangeEmployeeNumber { get; set; }


        [Outlet]
        UIKit.UITextField txtEmployeeToBeAvoidedOne { get; set; }


        [Outlet]
        UIKit.UITextField txtEmployeeToBeAvoidedThree { get; set; }


        [Outlet]
        UIKit.UITextField txtEmployeeToBeAvoidedTwo { get; set; }


        [Outlet]
        UIKit.UITextField txtSeniorityNo { get; set; }


        [Outlet]
        UIKit.UITextField txtSubmittingBidChoicesId { get; set; }


        [Outlet]
        UIKit.UIView vwAvoidanceBid { get; set; }


        [Outlet]
        UIKit.UIView vwAvoidanceBidChoice { get; set; }


        [Outlet]
        UIKit.UIView vwBuddyBid { get; set; }


        [Outlet]
        UIKit.UIView vwBuddyBidChoice { get; set; }


        [Outlet]
        UIKit.UIView vwChangeEmployeeNumber { get; set; }


        [Outlet]
        UIKit.UIView vwEmployeeToBeAvoided { get; set; }


        [Outlet]
        UIKit.UIView vwSubmittingBidChoicesFor { get; set; }


        [Outlet]
        UIKit.UIView vwViewOrAddBuddyBidders { get; set; }


        [Action ("btnSubmitBidCancelButtonTapped:")]
        partial void btnSubmitBidCancelButtonTapped (Foundation.NSObject sender);


        [Action ("btnSubmitBidTapped:")]
        partial void btnSubmitBidTapped (Foundation.NSObject sender);


        [Action ("btnChangeEmpTapped:")]
        partial void btnChangeEmpTapped (UIKit.UIButton sender);


        [Action ("btnChangeAvoidanceTapped:")]
        partial void btnChangeAvoidanceTapped (Foundation.NSObject sender);


        [Action ("sgSubmitBidTypeChanged:")]
        partial void sgSubmitBidTypeChanged (UIKit.UISegmentedControl sender);


        [Action ("btnChangeBuddyBidders:")]
        partial void btnChangeBuddyBidders (Foundation.NSObject sender);


        [Action ("btnAvodEmployeeOK:")]
        partial void btnAvodEmployeeOK (Foundation.NSObject sender);


        [Action ("btnAvoidEmployeeCancel:")]
        partial void btnAvoidEmployeeCancel (Foundation.NSObject sender);


        [Action ("btnChangeEmployeeNumberTapped:")]
        partial void btnChangeEmployeeNumberTapped (Foundation.NSObject sender);


        [Action ("btnChangeOrAddBuddyBidderCancel:")]
        partial void btnChangeOrAddBuddyBidderCancel (Foundation.NSObject sender);


        [Action ("btnChangeOrAddBuddyBidderOk:")]
        partial void btnChangeOrAddBuddyBidderOk (Foundation.NSObject sender);


        [Action ("btnChoicesCancelTapped:")]
        partial void btnChoicesCancelTapped (UIKit.UIButton sender);


        [Action ("btnEmployeeNumberCancelTapped:")]
        partial void btnEmployeeNumberCancelTapped (Foundation.NSObject sender);


        [Action ("btnEmployeeNumberOKTapped:")]
        partial void btnEmployeeNumberOKTapped (Foundation.NSObject sender);


        [Action ("btnSubmitChoiceTapped:")]
        partial void btnSubmitChoiceTapped (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}