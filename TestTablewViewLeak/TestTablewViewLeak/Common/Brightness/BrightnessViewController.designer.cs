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
    [Register ("BrightnessViewController")]
    partial class BrightnessViewController
    {
        [Outlet]
        UIKit.UISlider sldrBrightness { get; set; }


        [Action ("sldrBrightnessChanged:")]
        partial void sldrBrightnessChanged (UIKit.UISlider sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}