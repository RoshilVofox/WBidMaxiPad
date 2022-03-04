// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace TestTablewViewLeak.ViewControllers
{
    [Register ("SecretDataDownload")]
    partial class SecretDataDownload
    {
        [Outlet]
        UIKit.UIButton btnAllFA { get; set; }

        [Outlet]
        UIKit.UIButton btnAllPilot { get; set; }

        [Outlet]
        UIKit.UIButton btnBoth { get; set; }

        [Outlet]
        UIKit.UIButton btnCp { get; set; }

        [Outlet]
        UIKit.UIButton btnFA { get; set; }

        [Outlet]
        UIKit.UIButton btnFirstRound { get; set; }

        [Outlet]
        UIKit.UIButton btnFo { get; set; }

        [Outlet]
        UIKit.UIButton btnMonth { get; set; }

        [Outlet]
        UIKit.UIButton btnSecondRound { get; set; }

        [Outlet]
        UIKit.UIActivityIndicatorView ObjActivity { get; set; }

        [Outlet]
        UIKit.UITableView tblDomiciles { get; set; }

        [Outlet]
        UIKit.UITextField txtPassword { get; set; }

        [Outlet]
        UIKit.UITextView txtProgressView { get; set; }

        [Outlet]
        UIKit.UITextField txtUserId { get; set; }

        [Action ("btnAllFltAttAction:")]
        partial void btnAllFltAttAction (Foundation.NSObject sender);

        [Action ("btnAllPilotAction:")]
        partial void btnAllPilotAction (Foundation.NSObject sender);

        [Action ("btnBaseAction:")]
        partial void btnBaseAction (Foundation.NSObject sender);

        [Action ("btnCancel:")]
        partial void btnCancel (Foundation.NSObject sender);

        [Action ("btnClose:")]
        partial void btnClose (Foundation.NSObject sender);

        [Action ("btnDownloadAction:")]
        partial void btnDownloadAction (Foundation.NSObject sender);

        [Action ("btnMonthAction:")]
        partial void btnMonthAction (Foundation.NSObject sender);

        [Action ("btnPositionsClick:")]
        partial void btnPositionsClick (Foundation.NSObject sender);

        [Action ("btnRoundClick:")]
        partial void btnRoundClick (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}