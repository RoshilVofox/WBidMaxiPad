using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Core.Enum;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;

namespace WBid.WBidiPad.iOS
{
    public partial class SynchSelectionViewController : UIViewController
    {



        //index for State = 1, Quick Set = 2 and Both = 3
        public int selectedIndex;
        public IDictionary<string, int> synchDict;

        public DateTime ServerStateSynchTime;
        public DateTime ServerQSSynchTime;
        public DateTime LocalStateSynchTime;
        public DateTime LocalQSSynchTime;
     
        public SynchSelectionViewController() : base("SynchSelectionViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var localStateTimeCST = DateTime.MinValue;
            var serverStateTimeCST = DateTime.MinValue;
            var localQSTimeCST = DateTime.MinValue;
            var serverQSTimeCST = DateTime.MinValue;
            //State

            if (LocalStateSynchTime.Year == 0001 &&this.selectedIndex != 2)
            {
                lblLocalUpdatedDate.Text = "-- /--/--";
                lblLocalUpdatedTime.Text = "-- /--/--";
                this.segmentState.SelectedSegment = 1;
               
            }
            else
            {
               
                try
                {
                    localStateTimeCST = TimeZoneInfo.ConvertTimeFromUtc(LocalStateSynchTime, TimeZoneInfo.FindSystemTimeZoneById("America/Chicago"));
                }
                catch
                {
                    localStateTimeCST = TimeZoneInfo.ConvertTimeFromUtc(LocalStateSynchTime.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById("America/Chicago"));
                }
                lblLocalUpdatedDate.Text = localStateTimeCST.ToShortDateString();
                lblLocalUpdatedTime.Text = localStateTimeCST.ToString("hh:mm:ss tt");
            }

            if (ServerStateSynchTime.Year == 0001)
            {
                lblServerUpdatedDate.Text = "-- /--/--";
                lblServerUpdatedTime.Text = "-- /--/--";
                this.segmentState.SelectedSegment = 0;
               
            }
            else
            {
               
                try
                {
                    serverStateTimeCST = TimeZoneInfo.ConvertTimeFromUtc(ServerStateSynchTime, TimeZoneInfo.FindSystemTimeZoneById("America/Chicago"));
                }
                catch
                {
                    serverStateTimeCST = TimeZoneInfo.ConvertTimeFromUtc(ServerStateSynchTime.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById("America/Chicago"));
                }
                lblServerUpdatedDate.Text = serverStateTimeCST.ToShortDateString();
                lblServerUpdatedTime.Text = serverStateTimeCST.ToString("hh:mm:ss tt");
            }
           

            //quickset

            try
            {
                serverQSTimeCST = TimeZoneInfo.ConvertTimeFromUtc(ServerQSSynchTime, TimeZoneInfo.FindSystemTimeZoneById("America/Chicago"));
            }
            catch
            {
                serverQSTimeCST = TimeZoneInfo.ConvertTimeFromUtc(ServerQSSynchTime.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById("America/Chicago"));
            }

            if (LocalQSSynchTime.Year == 0001 && this.selectedIndex != 1)
            {
                lblQuickLocalDate.Text = "-- /--/--";
                lblQuickLocalTime.Text = "-- /--/--";
                this.segmentQuickSet.SelectedSegment = 1;

            }
            else
            {
                
                try
                {
                    localQSTimeCST = TimeZoneInfo.ConvertTimeFromUtc(LocalQSSynchTime, TimeZoneInfo.FindSystemTimeZoneById("America/Chicago"));
                }
                catch
                {
                    localQSTimeCST = TimeZoneInfo.ConvertTimeFromUtc(LocalQSSynchTime.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById("America/Chicago"));
                }
                lblQuickLocalDate.Text = localQSTimeCST.ToShortDateString();
                lblQuickLocalTime.Text = localQSTimeCST.ToString("hh:mm:ss tt");
            }

            if (ServerQSSynchTime.Year == 0001)
            {
                lblQuickServerDate.Text = "-- /--/--";
                lblQuickServerTime.Text = "-- /--/--";
                this.segmentQuickSet.SelectedSegment = 0;
               }
            else
            {
               try
                {
                    localQSTimeCST = TimeZoneInfo.ConvertTimeFromUtc(ServerQSSynchTime, TimeZoneInfo.FindSystemTimeZoneById("America/Chicago"));
                }
                catch
                {
                    localQSTimeCST = TimeZoneInfo.ConvertTimeFromUtc(ServerQSSynchTime.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById("America/Chicago"));
                }
                lblQuickServerDate.Text = serverQSTimeCST.ToShortDateString();
                lblQuickServerTime.Text = serverQSTimeCST.ToString("hh:mm:ss tt");
            }
            if(ServerQSSynchTime.Year == 0001 || LocalQSSynchTime.Year == 0001)
            {
                this.segmentQuickSet.UserInteractionEnabled = false;
            }
            else
                this.segmentQuickSet.UserInteractionEnabled = true;
            if (ServerStateSynchTime.Year == 0001 || LocalStateSynchTime.Year == 0001)
            {
                this.segmentState.UserInteractionEnabled = false;
            }
            else
                this.segmentState.UserInteractionEnabled = true;
                
         // Show view with respect to selectedIndex

            if(this.selectedIndex == 1) // View State
            {
                
                this.viewQuickSet.Hidden = true;
                this.stateConstraint.Constant = 0;
                this.mainViewHieghtConstraint.Constant = 422;


            }

            else if(this.selectedIndex == 2)//view Quick set
            {

                this.viewState.Hidden = true;
                this.quicksetConstraint.Constant = 10;
                this.mainViewHieghtConstraint.Constant = 422;

            }

            else if(this.selectedIndex == 3) // View Both
            {

            }




        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }


        // Sync Action

        partial void SyncAction(NSObject sender)
        {


            int segState = (int)this.segmentState.SelectedSegment;
            int segQuick = (int)this.segmentQuickSet.SelectedSegment;
            
             string selectionString = selectedIndex.ToString() + segState.ToString() + segQuick.ToString();

            // Notiifcation Obeserver call SynchSelectionNotif
            this.DismissViewController(true, () =>
            {

                
                NSNotificationCenter.DefaultCenter.PostNotificationName("synchViewNotif", new NSString(selectionString));
            });

        }

        partial void CloseAction(NSObject sender)
        {
            this.DismissViewController(true, null);
        }
       
    }
}

