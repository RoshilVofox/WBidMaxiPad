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
    [Register ("TripBlockLengthCell")]
    partial class TripBlockLengthCell
    {
        [Outlet]
        UIKit.UIButton btn2Day { get; set; }


        [Outlet]
        UIKit.UIButton btn3Day { get; set; }


        [Outlet]
        UIKit.UIButton btn4Day { get; set; }


        [Outlet]
        UIKit.UIButton btnBlock { get; set; }


        [Outlet]
        UIKit.UIButton btnDelete { get; set; }


        [Outlet]
        UIKit.UIButton btnTrip { get; set; }


        [Outlet]
        UIKit.UIButton btnTurn { get; set; }


        [Outlet]
        UIKit.UILabel lbTripBlockLenght { get; set; }


        [Action ("OnTripClickEvent:")]
        partial void OnTripClickEvent (UIKit.UIButton sender);


        [Action ("OnDeleteEvent:")]
        partial void OnDeleteEvent (UIKit.UIButton sender);


        [Action ("OnFouthButtonEvent:")]
        partial void OnFouthButtonEvent (UIKit.UIButton sender);


        [Action ("OnThirdButtonEvent:")]
        partial void OnThirdButtonEvent (UIKit.UIButton sender);


        [Action ("OnSecondButtonEvent:")]
        partial void OnSecondButtonEvent (UIKit.UIButton sender);


        [Action ("OnTurnButtonEvent:")]
        partial void OnTurnButtonEvent (UIKit.UIButton sender);


        [Action ("OnBlockEvent:")]
        partial void OnBlockEvent (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}