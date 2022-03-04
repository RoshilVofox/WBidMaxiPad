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
    [Register ("SubSortViewController")]
    partial class SubSortViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnContinue { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnDelete { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbBlockSortFirst { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbCorrectBasic { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbCorrectBlock { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbSelectedSort { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewBlockSort { get; set; }

        [Action ("OnContinueClickEvent:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnContinueClickEvent (UIKit.UIButton sender);

        [Action ("OnDeleteClickEvent:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnDeleteClickEvent (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnContinue != null) {
                btnContinue.Dispose ();
                btnContinue = null;
            }

            if (btnDelete != null) {
                btnDelete.Dispose ();
                btnDelete = null;
            }

            if (lbBlockSortFirst != null) {
                lbBlockSortFirst.Dispose ();
                lbBlockSortFirst = null;
            }

            if (lbCorrectBasic != null) {
                lbCorrectBasic.Dispose ();
                lbCorrectBasic = null;
            }

            if (lbCorrectBlock != null) {
                lbCorrectBlock.Dispose ();
                lbCorrectBlock = null;
            }

            if (lbSelectedSort != null) {
                lbSelectedSort.Dispose ();
                lbSelectedSort = null;
            }

            if (viewBlockSort != null) {
                viewBlockSort.Dispose ();
                viewBlockSort = null;
            }
        }
    }
}