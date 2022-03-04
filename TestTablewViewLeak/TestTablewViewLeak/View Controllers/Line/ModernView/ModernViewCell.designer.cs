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
    [Register ("ModernViewCell")]
    partial class ModernViewCell
    {
        [Outlet]
        UIKit.UIButton btnLineSelect { get; set; }


        [Outlet]
        UIKit.UIImageView imgBorder { get; set; }


        [Outlet]
        UIKit.UIImageView imgCrossIcon { get; set; }


        [Outlet]
        UIKit.UIImageView imgLockIcon { get; set; }


        [Outlet]
        UIKit.UIImageView imgOverlapIcon { get; set; }


        [Outlet]
        UIKit.UIImageView imgVLayer { get; set; }


        [Outlet]
        UIKit.UILabel [] lblCalDate { get; set; }


        [Outlet]
        UIKit.UILabel [] lblCalDay { get; set; }


        [Outlet]
        UIKit.UILabel lblLineNum { get; set; }


        [Outlet]
        UIKit.UILabel lblLinePos { get; set; }


        [Outlet]
        UIKit.UILabel lblLineSlNo { get; set; }


        [Outlet]
        UIKit.UILabel lblOverLayLineNumber { get; set; }


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
        UIKit.UILabel [] lblTripArrival { get; set; }


        [Outlet]
        UIKit.UILabel [] lblTripName { get; set; }


        [Outlet]
        UIKit.UILabel [] lblVacType { get; set; }


        [Outlet]
        UIKit.UIView OverLayView { get; set; }


        [Outlet]
        UIKit.UIView viewOrder { get; set; }


        [Outlet]
        UIKit.UIView vwChild1 { get; set; }


        [Outlet]
        UIKit.UIView vwChild2 { get; set; }


        [Outlet]
        UIKit.UIView [] vwChild3 { get; set; }


        [Action ("btnLineSelectTapped:")]
        partial void btnLineSelectTapped (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}