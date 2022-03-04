using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.SharedLibrary.SWA;

namespace TestTablewViewLeak.ViewControllers
{
    public partial class MonthToMonthAlertView : UIViewController
    {
        public string  alert;
        public MonthToMonthAlertView() : base("MonthToMonthAlertView", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            lblAlert.Text = alert;
            btnLink1.SetTitle("The Limitations and Opportunity of a Month-to-Month Vacation.",UIControlState.Normal);
            btnLink2.SetTitle("Who Wants 65 tfp for one week of Vacation.", UIControlState.Normal);

        }
        partial void btnLink1Tap(NSObject sender)
        {
            var success = DownloadBid.DownloadWBidFile(WBidHelper.GetAppDataPath(), "The Limitations and Opportunity of a Month-to-Month Vacation.pdf");
            InvokeOnMainThread(() =>
            {
                GlobalSettings.iSNeedToShowMonthtoMonthAlert = false;
                webPrint fileViewer = new webPrint();
                fileViewer.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;

                this.PresentViewController(fileViewer, true, () =>
                {
                    fileViewer.LoadPDFdocument("The Limitations and Opportunity of a Month-to-Month Vacation.pdf");
                });
            });
        }
        partial void btnLink2Tap(NSObject sender)
        {
            var success = DownloadBid.DownloadWBidFile(WBidHelper.GetAppDataPath(), "Who Wants 65 tfp for one week of Vacation.pdf");
            InvokeOnMainThread(() =>
            {
                GlobalSettings.iSNeedToShowMonthtoMonthAlert = false;
                webPrint fileViewer = new webPrint();
                fileViewer.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;

                this.PresentViewController(fileViewer, true, () =>
                {
                    fileViewer.LoadPDFdocument("Who Wants 65 tfp for one week of Vacation.pdf");
                });
            });
        }
       
        partial void btnOkTap(NSObject sender)
        {
            this.DismissViewController(true, null);
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
       
    }
}

