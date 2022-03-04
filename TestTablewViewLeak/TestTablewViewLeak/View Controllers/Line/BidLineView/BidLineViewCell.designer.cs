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
    [Register ("BidLineViewCell")]
    partial class BidLineViewCell
    {
        [Outlet]
        UIKit.UIButton btnLineOption { get; set; }


        [Outlet]
        UIKit.UIButton btnLineSelect { get; set; }


        [Outlet]
        UIKit.UIImageView imgCrossIcon { get; set; }


        [Outlet]
        UIKit.UIImageView imgLockIcon { get; set; }


        [Outlet]
        UIKit.UIImageView imgOverlapIcon { get; set; }


        [Outlet]
        UIKit.UILabel [] lblCalDate { get; set; }


        [Outlet]
        UIKit.UILabel [] lblCalDay { get; set; }


        [Outlet]
        UIKit.UILabel lblLineName { get; set; }


        [Outlet]
        UIKit.UILabel lblLineNumber { get; set; }


        [Outlet]
        UIKit.UILabel lblPairingDesc { get; set; }


        [Outlet]
        UIKit.UILabel [] lblPairName { get; set; }


        [Outlet]
        UIKit.UILabel [] lblProperty1 { get; set; }


        [Outlet]
        UIKit.UILabel [] lblProperty2 { get; set; }


        [Outlet]
        UIKit.UILabel [] lblProperty3 { get; set; }


        [Outlet]
        UIKit.UILabel [] lblProperty4 { get; set; }


        [Outlet]
        UIKit.UILabel [] lblProperty5 { get; set; }


        [Outlet]
        UIKit.UILabel [] lblPropName { get; set; }


        [Outlet]
        UIKit.UILabel [] lblPropValue { get; set; }


        [Outlet]
        UIKit.UILabel lblSlNo { get; set; }


        [Outlet]
        UIKit.UILabel [] lblTripName { get; set; }


        [Outlet]
        UIKit.UILabel [] lblValue { get; set; }


        [Outlet]
        UIKit.UIView vwChild1 { get; set; }


        [Outlet]
        UIKit.UIView vwChild2 { get; set; }


        [Outlet]
        UIKit.UIView [] vwChild3 { get; set; }


        [Action ("btnLineSelectTap:")]
        partial void btnLineSelectTap (UIKit.UIButton sender);


        [Action ("btnLineOptionTap:")]
        partial void btnLineOptionTap (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}