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
    [Register ("userRegistrationViewController")]
    partial class userRegistrationViewController
    {
        [Outlet]
        UIKit.UIButton btnAccept { get; set; }


        [Outlet]
        UIKit.UIButton btnCancel { get; set; }


        [Outlet]
        UIKit.UIButton btnCellCarrier { get; set; }






        [Outlet]
        UIKit.UIButton btnLicence { get; set; }


        [Outlet]
        UIKit.UIButton btnPrivacy { get; set; }


        [Outlet]
        UIKit.UIButton btnRePass { get; set; }


        [Outlet]
        UIKit.UIButton btnSelectDomicle { get; set; }


        [Outlet]
        UIKit.UIButton btnSubmit { get; set; }






        [Outlet]
        UIKit.UILabel lblHeaderUserTerms { get; set; }






        [Outlet]
        UIKit.UILabel lblTitle { get; set; }


        [Outlet]
        UIKit.UIPickerView pckrSelectDomicile { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgMaleFemale { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgPosition { get; set; }


        [Outlet]
        UIKit.UIStepper stprAutoSave { get; set; }


        [Outlet]
        UIKit.UISwitch swAutoSave { get; set; }


        [Outlet]
        UIKit.UISwitch swCrashReport { get; set; }


        [Outlet]
        UIKit.UISwitch swEmail { get; set; }


        [Outlet]
        UIKit.UISwitch switchTermsAndCondition { get; set; }


        [Outlet]
        UIKit.UISwitch swSync { get; set; }


        [Outlet]
        UIKit.UITextField txtAutoSaveValue { get; set; }


        [Outlet]
        UIKit.UITextField txtCellNumber { get; set; }


        [Outlet]
        UIKit.UITextField txtEmail { get; set; }


        [Outlet]
        UIKit.UITextField txtEmployeeNumber { get; set; }


        [Outlet]
        UIKit.UITextField txtFirstName { get; set; }


        [Outlet]
        UIKit.UITextField txtLastName { get; set; }


        [Outlet]
        UIKit.UITextField txtRePass { get; set; }


        [Outlet]
        UIKit.UITextField txtSeniorityNumber { get; set; }


        [Outlet]
        UIKit.UIView vwRePass { get; set; }


        [Outlet]
        UIKit.UIView vwUserManage { get; set; }


        [Outlet]
        UIKit.UISwitch waitchAcceptMail { get; set; }


        [Action ("btnCancelTapped:")]
        partial void btnCancelTapped (UIKit.UIButton sender);


        [Action ("sgMaleFemaleTapped:")]
        partial void sgMaleFemaleTapped (UIKit.UISegmentedControl sender);


        [Action ("sgPositionValueChanged:")]
        partial void sgPositionValueChanged (UIKit.UISegmentedControl sender);


        [Action ("btnSubmitTapped:")]
        partial void btnSubmitTapped (UIKit.UIButton sender);


        [Action ("btnTipTapped:")]
        partial void btnTipTapped (UIKit.UIButton sender);


        [Action ("btnAcceptTapped:")]
        partial void btnAcceptTapped (UIKit.UIButton sender);


        [Action ("btnCellCarrierClicked:")]
        partial void btnCellCarrierClicked (Foundation.NSObject sender);






        //[Action ("btnCreatePasswordClicked:")]
        //partial void btnCreatePasswordClicked (Foundation.NSObject sender);


        [Action ("ConditionsBtnClicked:")]
        partial void ConditionsBtnClicked (Foundation.NSObject sender);


        [Action ("btnHelpTapped:")]
        partial void btnHelpTapped (UIKit.UIButton sender);


        [Action ("btnSelectDomicleTapped:")]
        partial void btnSelectDomicleTapped (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}