
using System;

using Foundation;
using UIKit;

using System.Linq;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Core.Enum;

namespace  WBid.WBidiPad.iOS
{
    public partial class CommutableTimeViewController : UIViewController
    {
		//public	enum CommuteFromView
		//{
		//	BidAutomator,
		//	CSWConstraints,
		//	CSWWeight,
		//	CSWCommutabilityConstraints,
		//	CSWCommutabilityWeights,
		//} ;
		public CommuteFromView ObjCommuteTimeFromView;
        private CommutablePickerView _view;
		commutabilityCommon data;
		public Object data1 {
			get;
			set;
		}


		public UIPopoverController popOver;
        public CommutableTimeViewController()
            : base("CommutableTimeViewController", null)
        {
        }
        public CommutableTimeViewController(IntPtr handle)
            : base(handle)
        {
        }

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			NSNotificationCenter.DefaultCenter.PostNotificationName ("CalPopHide", null);
			//txtCellNumber.Dispose ();

			foreach (UIView view in this.View.Subviews) {

				DisposeClass.DisposeEx(view);
			}
			this.View.Dispose ();
			this.View.UserInteractionEnabled = true;

		}

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }
        public override void ViewDidAppear(bool animated)
        {
			
			data = new commutabilityCommon();
			if (data1.GetType().Name == "FtCommutableLine")
			{
				var objdata = (FtCommutableLine)data1;
				data.BaseTime = objdata.BaseTime;
				data.CheckInTime = objdata.CheckInTime;
				data.City = objdata.City;
				data.CommuteCity = objdata.CommuteCity;
				data.ConnectTime = objdata.ConnectTime;
				data.NoNights = objdata.NoNights;
				data.ToHome = objdata.ToHome;
				data.ToWork = objdata.ToWork;
				data.isNonStop = objdata.IsNonStopOnly;
			}
			else if (data1.GetType().Name == "Commutability")
			{
				var objdata = (Commutability)data1;
				data.BaseTime = objdata.BaseTime;
				data.CheckInTime = objdata.CheckInTime;
				data.City = objdata.City;
				data.CommuteCity = objdata.CommuteCity;
				data.ConnectTime = objdata.ConnectTime;
				data.SecondcellValue = objdata.SecondcellValue;
				data.ThirdcellValue = objdata.ThirdcellValue;
				data.Value = objdata.Value;
				data.Weight = objdata.Weight;
				
			}
			else if (data1.GetType().Name == "WtCommutableLineAuto")
			{
				var objdata = (WtCommutableLineAuto)data1;
				data.BaseTime = objdata.BaseTime;
				data.CheckInTime = objdata.CheckInTime;
				data.City = objdata.City;
				data.CommuteCity = objdata.CommuteCity;
				data.ConnectTime = objdata.ConnectTime;
				data.NoNights = objdata.NoNights;
				data.ToHome = objdata.ToHome;
				data.ToWork = objdata.ToWork;
				data.Weight = objdata.Weight;
				data.isNonStop = objdata.IsNonStopOnly;
			}
			else
			{
				data =(commutabilityCommon)data1;
			}

            _view = new CommutablePickerView(data);
			_view.ObjCommutePickerFromView=(CommuteFromView)ObjCommuteTimeFromView;
            _view.BackgroundColor = UIColor.FromRGB((nfloat)(234.0 / 255.0), (nfloat)(237.0 / 255.0), (nfloat)(180.0 / 255.0));
            _view.Frame = Viewcalender.Bounds;


            Viewcalender.AddSubview(_view);
			
			base.ViewDidAppear(animated);
			
			//this.NavigationController.Title = "Arrival";
		}
        partial void DismissCalendarView(NSObject sender)
        {
			popOver.Dismiss(true);
        }
        public override void ViewDidLoad()
        {
			
			base.ViewDidLoad();
			
			
			data = new commutabilityCommon();
			if (data1.GetType().Name == "FtCommutableLine")
			{
				var objdata = (FtCommutableLine)data1;
				data.BaseTime = objdata.BaseTime;
				data.CheckInTime = objdata.CheckInTime;
				data.City = objdata.City;
				data.CommuteCity = objdata.CommuteCity;
				data.ConnectTime = objdata.ConnectTime;
				data.NoNights = objdata.NoNights;
				data.ToHome = objdata.ToHome;
				data.ToWork = objdata.ToWork;
				data.isNonStop = objdata.IsNonStopOnly;
			}
			else if (data1.GetType().Name == "Commutability")
			{
				var objdata = (Commutability)data1;
				data.BaseTime = objdata.BaseTime;
				data.CheckInTime = objdata.CheckInTime;
				data.City = objdata.City;
				data.CommuteCity = objdata.CommuteCity;
				data.ConnectTime = objdata.ConnectTime;
				data.SecondcellValue = objdata.SecondcellValue;
				data.ThirdcellValue = objdata.ThirdcellValue;
				data.Value = objdata.Value;
				data.Weight = objdata.Weight;
				
			}
			else if (data1.GetType().Name == "WtCommutableLineAuto")
			{
				var objdata = (WtCommutableLineAuto)data1;
				data.BaseTime = objdata.BaseTime;
				data.CheckInTime = objdata.CheckInTime;
				data.City = objdata.City;
				data.CommuteCity = objdata.CommuteCity;
				data.ConnectTime = objdata.ConnectTime;
				data.NoNights = objdata.NoNights;
				data.ToHome = objdata.ToHome;
				data.ToWork = objdata.ToWork;
				data.Weight = objdata.Weight;
				data.isNonStop = objdata.IsNonStopOnly;
			}
			else
			{
				data = (commutabilityCommon)data1;
			}

			lblNavTitle.TopItem.Title = data.isNonStop ? "Arrival & Depart Times (Non Stop)" : "Arrival & Depart Times";
			lblBase.Text = GlobalSettings.CurrentBidDetails.Domicile;

           // WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            //  wBIdStateContent.BidAuto = null;
//			if (data.City != null && wBIdStateContent.BidAuto.BAFilter != null)
//            {
//                var sl = wBIdStateContent.BidAuto.BAFilter.FirstOrDefault(x => x.Name == "CL");
//                lblCommuteCity.Text = ((FtCommutableLine)sl.BidAutoObject).City;
//            }
//            else
//            {
//                lblCommuteCity.Text = string.Empty;
//            }
			if (data!= null) lblCommuteCity.Text= data.City;

            // Perform any additional setup after loading the view, typically from a nib.
        }
    }
}

