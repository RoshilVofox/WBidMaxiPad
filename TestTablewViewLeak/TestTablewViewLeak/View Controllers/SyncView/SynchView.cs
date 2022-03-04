using System;
using UIKit;
using CoreGraphics;
using Foundation;


namespace WBid.WBidiPad.iOS
{
    public partial class SynchView : UIViewController
    {

        // radio selection

        int selectedIndex;

        // server updated date

       public string serverupdated;

        // local updated

       public string localupdated;

        public DateTime ServerStateSynchTime;
        public DateTime ServerQSSynchTime;
        public DateTime LocalStateSynchTime;
        public DateTime LocalQSSynchTime;
        // notification

        NSObject synchViewNotif;

        // selection string passing through notification

        string selectionString;

        public SynchView() : base("SynchView", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // set elected index when load
            selectedIndex = 3;
            this.btnState.SetImage(UIImage.FromBundle("radioNo.png"), UIControlState.Normal);
            this.btnQuickSet.SetImage(UIImage.FromBundle("radioNo"), UIControlState.Normal);
            this.btnBoth.SetImage(UIImage.FromBundle("radioYes.png"), UIControlState.Normal);
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void ViewDidDisappear(bool animated)
        {
            if (selectionString != null)
            {

                NSNotificationCenter.DefaultCenter.PostNotificationName("SynchSelectioNotif", new NSString(selectionString));

            }



        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        partial void actionState(UIKit.UIButton sender)
        {
            selectedIndex = 1;
            this.btnState.SetImage(UIImage.FromBundle("radioYes.png"), UIControlState.Normal);
            this.btnQuickSet.SetImage(UIImage.FromBundle("radioNo.png"), UIControlState.Normal);
            this.btnBoth.SetImage(UIImage.FromBundle("radioNo.png"), UIControlState.Normal);
        }

    

        partial void ActionQuickset(UIKit.UIButton sender)
        {
            selectedIndex = 2;
            this.btnState.SetImage(UIImage.FromBundle("radioNo.png"), UIControlState.Normal);
            this.btnQuickSet.SetImage(UIImage.FromBundle("radioYes.png"), UIControlState.Normal);
            this.btnBoth.SetImage(UIImage.FromBundle("radioNo.png"), UIControlState.Normal);

        }

        partial void ActionBoth(UIKit.UIButton sender)
        {
            selectedIndex = 3;
            this.btnState.SetImage(UIImage.FromBundle("radioNo.png"), UIControlState.Normal);
            this.btnQuickSet.SetImage(UIImage.FromBundle("radioNo"), UIControlState.Normal);
            this.btnBoth.SetImage(UIImage.FromBundle("radioYes.png"), UIControlState.Normal);

        }

        partial void ActionOK(UIKit.UIButton sender)
        {



            synchViewNotif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("synchViewNotif"), (NSNotification notification) =>
            {
                this.DismissViewController(true, null);
                this.selectionString = notification.Object.ToString();
                NSNotificationCenter.DefaultCenter.RemoveObserver(synchViewNotif);

            });

            SynchSelectionViewController synchConf = new SynchSelectionViewController();
            synchConf.ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;

            synchConf.ServerStateSynchTime = ServerStateSynchTime;
            synchConf.ServerQSSynchTime = ServerQSSynchTime;
            synchConf.LocalStateSynchTime = LocalStateSynchTime;
            synchConf.LocalQSSynchTime = LocalQSSynchTime;
            // Action on State Synch

            if (selectedIndex == 1)
            {
                
                synchConf.selectedIndex = 1;
               
                this.PresentViewController(synchConf, true, null);
            }



            // Action on Quick Set Synch

            else if (selectedIndex == 2)
            {
               
                synchConf.selectedIndex = 2;
               
                this.PresentViewController(synchConf, true, null);

            }


            // Action on both Synch

            else if (selectedIndex == 3)
            {
               
                synchConf.selectedIndex = 3;
               
                this.PresentViewController(synchConf, true, null);

            }


        }

        partial void actionCancel(UIKit.UIButton sender)
        {
                 selectionString = "";
                 NSNotificationCenter.DefaultCenter.PostNotificationName("SynchSelectioNotif", new NSString(selectionString));
            this.DismissViewController(true, null);
        }

        
    }
}

