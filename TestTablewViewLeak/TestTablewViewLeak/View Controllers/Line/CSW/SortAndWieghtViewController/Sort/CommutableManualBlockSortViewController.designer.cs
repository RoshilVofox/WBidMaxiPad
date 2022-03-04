// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WBid.WBidiPad.iOS
{
    [Register ("CommutableManualBlockSortViewController")]
    partial class CommutableManualBlockSortViewController
    {
        [Outlet]
        UIKit.UIButton btnAnyNight { get; set; }

        [Outlet]
        UIKit.UIButton btnBothEnds { get; set; }

        [Outlet]
        UIKit.UIButton btnCommHome { get; set; }

        [Outlet]
        UIKit.UIButton btnCommWork { get; set; }

        [Outlet]
        UIKit.UIButton btnFriArr { get; set; }

        [Outlet]
        UIKit.UIButton btnFriDep { get; set; }

        [Outlet]
        UIKit.UIButton btnLoadDefaults { get; set; }

        [Outlet]
        UIKit.UIButton btnMonThuArr { get; set; }

        [Outlet]
        UIKit.UIButton btnMonThuDep { get; set; }

        [Outlet]
        UIKit.UIButton btnRemove { get; set; }

        [Outlet]
        UIKit.UIButton btnSatArr { get; set; }

        [Outlet]
        UIKit.UIButton btnSatDep { get; set; }

        [Outlet]
        UIKit.UIButton btnSaveDefaults { get; set; }

        [Outlet]
        UIKit.UIButton btnSunArr { get; set; }

        [Outlet]
        UIKit.UIButton btnSunDep { get; set; }

        [Action ("btnAnyNightBothEndsTapped:")]
        partial void btnAnyNightBothEndsTapped (UIKit.UIButton sender);

        [Action ("btnBothEndsWtTapped:")]
        partial void btnBothEndsWtTapped (UIKit.UIButton sender);

        [Action ("btnHelpIconTapped:")]
        partial void btnHelpIconTapped (UIKit.UIButton sender);

        [Action ("btnInDomWtTapped:")]
        partial void btnInDomWtTapped (UIKit.UIButton sender);

        [Action ("btnLoadDefaultsTapped:")]
        partial void btnLoadDefaultsTapped (UIKit.UIButton sender);

        [Action ("btnRemoveTapped:")]
        partial void btnRemoveTapped (UIKit.UIButton sender);

        [Action ("btnSaveDefaultsTapped:")]
        partial void btnSaveDefaultsTapped (UIKit.UIButton sender);

        [Action ("btnTimePopoverTapped:")]
        partial void btnTimePopoverTapped (UIKit.UIButton sender);

        [Action ("btnWorkOffTapped:")]
        partial void btnWorkOffTapped (UIKit.UIButton sender);

        [Action ("commuteOptionsTapped:")]
        partial void commuteOptionsTapped (UIKit.UIButton sender);
        
        void ReleaseDesignerOutlets ()
        {
            if (btnAnyNight != null) {
                btnAnyNight.Dispose ();
                btnAnyNight = null;
            }

            if (btnBothEnds != null) {
                btnBothEnds.Dispose ();
                btnBothEnds = null;
            }

            if (btnCommHome != null) {
                btnCommHome.Dispose ();
                btnCommHome = null;
            }

            if (btnCommWork != null) {
                btnCommWork.Dispose ();
                btnCommWork = null;
            }

            if (btnFriArr != null) {
                btnFriArr.Dispose ();
                btnFriArr = null;
            }

            if (btnFriDep != null) {
                btnFriDep.Dispose ();
                btnFriDep = null;
            }

            if (btnLoadDefaults != null) {
                btnLoadDefaults.Dispose ();
                btnLoadDefaults = null;
            }

            if (btnMonThuArr != null) {
                btnMonThuArr.Dispose ();
                btnMonThuArr = null;
            }

            if (btnMonThuDep != null) {
                btnMonThuDep.Dispose ();
                btnMonThuDep = null;
            }

            if (btnRemove != null) {
                btnRemove.Dispose ();
                btnRemove = null;
            }

            if (btnSatArr != null) {
                btnSatArr.Dispose ();
                btnSatArr = null;
            }

            if (btnSatDep != null) {
                btnSatDep.Dispose ();
                btnSatDep = null;
            }

            if (btnSaveDefaults != null) {
                btnSaveDefaults.Dispose ();
                btnSaveDefaults = null;
            }

            if (btnSunArr != null) {
                btnSunArr.Dispose ();
                btnSunArr = null;
            }

            if (btnSunDep != null) {
                btnSunDep.Dispose ();
                btnSunDep = null;
            }
        }
    }
}
