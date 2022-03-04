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
    [Register ("RatioViewController")]
    partial class RatioViewController
    {
        [Outlet]
        UIKit.UIButton btnDenominator { get; set; }


        [Outlet]
        UIKit.UIButton btnNumerator { get; set; }


        [Action ("CancelButtonClicked:")]
        partial void CancelButtonClicked (Foundation.NSObject sender);


        [Action ("DenominatorClicked:")]
        partial void DenominatorClicked (Foundation.NSObject sender);


        [Action ("NumeratorClicked:")]
        partial void NumeratorClicked (Foundation.NSObject sender);


        [Action ("OkButtonClicked:")]
        partial void OkButtonClicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}