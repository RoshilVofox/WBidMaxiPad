using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using TestTablewViewLeak.ViewControllers.VacationDifferenceView;
using UIKit;
using WBid.WBidiPad.iOS.Utility;

namespace TestTablewViewLeak.ViewControllers.CommuteDifferenceView
{
    public partial class FltDiffButtonViewController : UIViewController
    {
        public FltDiffButtonViewController() : base("FltDiffButtonViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        partial void btnCmtDiffClick(NSObject sender)
        {
            CommuteDifferenceViewController vacdiff = new CommuteDifferenceViewController();
            vacdiff.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            UINavigationController nav = new UINavigationController(vacdiff);
            vacdiff.PreferredContentSize = new CGSize(1020, 700);
            nav.NavigationBarHidden = true;
            nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            this.PresentViewController(nav, true, null);
            //this.DismissViewController(true, null);

            
        }
        partial void btnVacDiffClick(NSObject sender)
        {
            if (Reachability.CheckVPSAvailable())
            {
                VacationDifferenceViewController vacdiff = new VacationDifferenceViewController();
                vacdiff.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                UINavigationController nav = new UINavigationController(vacdiff);
                vacdiff.PreferredContentSize = new CGSize(1020, 700);
                nav.NavigationBarHidden = true;
                nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                this.PresentViewController(nav, true, null);
            }
            else
            {
                if (WBidHelper.IsSouthWestWifiOr2wire())
                {

                    UIAlertController okAlertController = UIAlertController.Create("WBidMax", WBid.WBidiPad.iOS.Constants.SouthWestConnectionAlert, UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                }
                else
                {
                    UIAlertController okAlertController = UIAlertController.Create("WBidMax", WBid.WBidiPad.iOS.Constants.VPSDownAlert, UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                }
            }
        }
    }
}

