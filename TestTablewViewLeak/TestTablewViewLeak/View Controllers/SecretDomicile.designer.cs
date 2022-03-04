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
    [Register ("SecretDomicile")]
    partial class SecretDomicile
    {
        [Outlet]
        UIKit.UIButton btncheckBox { get; set; }

        [Outlet]
        UIKit.UILabel lblBase { get; set; }

        [Action ("btncheckBoxclicked:")]
        partial void btncheckBoxclicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}