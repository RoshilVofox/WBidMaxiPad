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
    [Register ("SubmitwebPrint")]
    partial class SubmitwebPrint
    {
        [Outlet]
        UIKit.UISearchBar searchBar { get; set; }


        [Outlet]
        UIKit.UIWebView webView { get; set; }


        [Action ("btnBackTapped:")]
        partial void btnBackTapped (UIKit.UIBarButtonItem sender);


        [Action ("btnReloadTapped:")]
        partial void btnReloadTapped (UIKit.UIBarButtonItem sender);


        [Action ("btnSearchTapped:")]
        partial void btnSearchTapped (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}