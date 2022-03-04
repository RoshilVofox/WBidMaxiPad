using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using System.Collections.Generic;
using WBid.WBidiPad.Core;
using System.Linq;
using WBid.WBidiPad.PortableLibrary.Utility;
//namespace TestTablewViewLeak.ViewControllers.Line.CSW.ConstraintsViewController
namespace WBid.WBidiPad.iOS
{
    public partial class ReportReleaseCell : UITableViewCell
    {
        UIPopoverController popoverController;
        public static readonly NSString Key = new NSString("ReportReleaseCell");
        public static readonly UINib Nib;
        public List<ReportRelease> lstReportRelease;
        public ReportRelease ObjreportRelease;
        NSObject reportReleaseNotif;
        string report = "00:00";
        string release = "00:00";
        int contraintRowCount = 0;
        WBidState wBIdStateContent;
        static ReportReleaseCell()
        {
            Nib = UINib.FromName("ReportReleaseCell", NSBundle.MainBundle);
        }

        protected ReportReleaseCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        class MyPopDelegate : UIPopoverControllerDelegate
        {
            ReportReleaseCell _parent;
            public MyPopDelegate(ReportReleaseCell parent)
            {
                _parent = parent;
            }
            public override void DidDismiss(UIPopoverController popoverController)
            {
                
                if (_parent.reportReleaseNotif != null)
                    NSNotificationCenter.DefaultCenter.RemoveObserver(_parent.reportReleaseNotif);
                _parent.btnReport.Selected = false;
                _parent.btnRelease.Selected = false;

            }
        }


        public void ConstrainFilleData(ReportRelease objreportRelease)
        {
            ObjreportRelease = objreportRelease;
            LoadDataFromState();
        }

        public static ReportReleaseCell Create()
        {
            return (ReportReleaseCell)Nib.Instantiate(null, null)[0];
        }
        private void LoadDataFromState()
        {
            btnFirst.Selected = ObjreportRelease.First;
            btnLast.Selected = ObjreportRelease.Last;
            btnNoMid.Selected = ObjreportRelease.NoMid;

        }
        public void bindData(NSIndexPath indexpath,List<ReportRelease> lstReportRelease)
        {
            wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

            contraintRowCount = indexpath.Row;

            ObjreportRelease=wBIdStateContent.Constraints.ReportRelease.lstParameters[indexpath.Row];
            if (ObjreportRelease != null)
            {
                report = Helper.ConvertMinuteToHHMM(ObjreportRelease.Report);
                release=Helper.ConvertMinuteToHHMM(ObjreportRelease.Release);
                btnReport.SetTitle(report, UIControlState.Normal);
                btnRelease.SetTitle(release, UIControlState.Normal);
                btnFirst.Selected = ObjreportRelease.First;
                btnLast.Selected = ObjreportRelease.Last;
                btnNoMid.Selected = ObjreportRelease.NoMid;
                if(ObjreportRelease.AllDays==false)
                    sgRptType.SelectedSegment=1;
            }

            SetCheckBox();
           // lblOutline.Layer.BorderWidth = 1.0f;
           // lblOutline.Layer.BorderColor = UIColor.Black.CGColor;

        }
        partial void btnCalculate(UIButton sender)
        {
            ObjreportRelease.Report = Helper.ConvertHHMMtoMinute(report);
            ObjreportRelease.Release = Helper.ConvertHHMMtoMinute(release);
            ObjreportRelease.AllDays=(sgRptType.SelectedSegment==0);
            ObjreportRelease.First = btnFirst.Selected;
            ObjreportRelease.Last = btnLast.Selected;
            ObjreportRelease.NoMid = btnNoMid.Selected;
            ConstraintCalculations objconstrain = new ConstraintCalculations();
            objconstrain.ApplyReportReleaseConstraint(wBIdStateContent.Constraints.ReportRelease.lstParameters);
            NSNotificationCenter.DefaultCenter.PostNotificationName("LineCountReload", null);

        }
        private void SetCheckBox()
        {
            if(sgRptType.SelectedSegment==1)
            {
                dropView.Hidden = false;
                btnFirst.Enabled = true;
                btnLast.Enabled = true;
                btnNoMid.Enabled = true;
            }
            else
            {
                dropView.Hidden = true;
                btnFirst.Enabled = false;
                btnLast.Enabled = false;
                btnNoMid.Enabled = false;
            }
        }
        partial void sgTypeValueChanged(UISegmentedControl sender)
        {
          
            if(sgRptType.SelectedSegment==1)
            {
                ObjreportRelease.AllDays = false;
            }
            else
            {
                ObjreportRelease.AllDays = true;
            }
            SetCheckBox();
        }
        partial void btnFirstAction(UIButton sender)
        {
            sender.Selected = !sender.Selected;
            ObjreportRelease.First = sender.Selected;
        }
        partial void btnLastAction(UIButton sender)
        {
            sender.Selected = !sender.Selected; 
            ObjreportRelease.Last = sender.Selected;
        }
        partial void btnNoMidAction(UIButton sender)
        {
            sender.Selected = !sender.Selected; 
            ObjreportRelease.NoMid = sender.Selected;
        }
        partial void btnReportAction(UIButton sender)
        {
            sender.Selected = true;
            reportReleaseNotif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ChangeTimeText"), handleChangeTimeText);

            PopoverViewController popoverContent = new PopoverViewController();
            popoverContent.PopType = "timePad";
            popoverContent.timeValue = sender.TitleLabel.Text;
            popoverController = new UIPopoverController(popoverContent);
            popoverController.Delegate = new MyPopDelegate(this);
            popoverController.PopoverContentSize = new CGSize(200, 200);
            popoverController.PresentFromRect(sender.Frame, this, UIPopoverArrowDirection.Any, true);
        }
        partial void btnReleaseAction(UIButton sender)
        {
            sender.Selected = true;
            reportReleaseNotif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ChangeTimeText"), handleChangeTimeText);

            PopoverViewController popoverContent = new PopoverViewController();
            popoverContent.PopType = "timePad";
            popoverContent.timeValue = sender.TitleLabel.Text;
            popoverController = new UIPopoverController(popoverContent);
            popoverController.Delegate = new MyPopDelegate(this);
            popoverController.PopoverContentSize = new CGSize(200, 200);
            popoverController.PresentFromRect(sender.Frame, this, UIPopoverArrowDirection.Any, true);
        }

        public void handleChangeTimeText(NSNotification n)
        {
            if (btnReport.Selected == true)
            {
                report = n.Object.ToString();
                ObjreportRelease.Report = Helper.ConvertHHMMtoMinute(report);
                btnReport.SetTitle(n.Object.ToString(), UIControlState.Normal);
                btnReport.SetTitle(n.Object.ToString(), UIControlState.Selected);

            }
            else if (btnRelease.Selected == true)
            {
                release = n.Object.ToString();
                ObjreportRelease.Release = Helper.ConvertHHMMtoMinute(release);
                btnRelease.SetTitle(n.Object.ToString(), UIControlState.Normal);
                btnRelease.SetTitle(n.Object.ToString(), UIControlState.Selected);
            }
        }

        partial void btnRemoveAction(UIButton sender)
        {
           // wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            wBIdStateContent.Constraints.ReportRelease.lstParameters.RemoveAt(contraintRowCount);

            if(wBIdStateContent.Constraints.ReportRelease.lstParameters.Count==0)
                wBIdStateContent.CxWtState.ReportRelease.Cx = false;
            ConstraintCalculations objconstrain = new ConstraintCalculations();
            objconstrain.ApplyReportReleaseConstraint(wBIdStateContent.Constraints.ReportRelease.lstParameters);
            
           NSNotificationCenter.DefaultCenter.PostNotificationName("AddConstraints", null);
            NSNotificationCenter.DefaultCenter.PostNotificationName("LineCountReload", null);
        }

    }
}
