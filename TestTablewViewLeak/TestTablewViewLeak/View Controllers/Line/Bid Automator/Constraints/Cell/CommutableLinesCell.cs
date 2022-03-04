using System;

using Foundation;
using UIKit;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using System.Linq;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using CoreGraphics;
using WBid.WBidiPad.iOS.Utility;


namespace WBid.WBidiPad.iOS
{
	public partial class CommutableLinesCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("CommutableLinesCell");
		public static readonly UINib Nib;
		ConstraintsChangeViewController _parentVC;
		DaysOfMonthCollectionController calCollection;
		ConstraintCalculations constCalc = new ConstraintCalculations ();
		WeightCalculation weightCalc= new WeightCalculation();
		NSObject changeContraintParamNotif;
        UIPopoverController popoverController;
		WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		UIButton btnPoint;
		commutabilityCommon _cellData;
		public Object data1 {
			get;
			set;
		}
		enum FromView
		{
			BidAutomator,
			CSWConstraints,
			CSWWeight,
		};
		FromView ObjFromView;
		static CommutableLinesCell ()
		{
			Nib = UINib.FromName ("CommutableLinesCell", NSBundle.MainBundle);
		}

		public CommutableLinesCell (IntPtr handle) : base (handle)
		{
		}

		public static CommutableLinesCell Create()
		{
			return (CommutableLinesCell)Nib.Instantiate(null, null)[0];
		}

		public void Filldata (ConstraintsChangeViewController constraintsChangeViewController, FtCommutableLine cxCommutableLine)
		{
			ObjFromView = FromView.BidAutomator;
			_parentVC = constraintsChangeViewController;
			data1 = cxCommutableLine;
		
				//_cellData = (FtCommutableLine)data1;
	

			



			UpdateUI ();
		}




		public void CSWConstraintFilldata (FtCommutableLine cxCommutableLine)
		{

			ObjFromView = FromView.CSWConstraints;
			data1 = cxCommutableLine;
			//_cellData = (FtCommutableLine)cxCommutableLine;
			var objdata = (FtCommutableLine)cxCommutableLine;
			_cellData = new commutabilityCommon();
			_cellData.BaseTime = objdata.BaseTime;
			_cellData.CheckInTime = objdata.CheckInTime;
			_cellData.City = objdata.City;
			_cellData.CommuteCity = objdata.CommuteCity;
			_cellData.ConnectTime = objdata.ConnectTime;
			_cellData.NoNights = objdata.NoNights;
			_cellData.ToHome = objdata.ToHome;
			_cellData.ToWork = objdata.ToWork;
			_cellData.isNonStop = objdata.IsNonStopOnly;
			//
			if (calCollection != null)
			{
				calCollection.View.RemoveFromSuperview();
				calCollection = null;
			}

			UpdateUI ();
		}
		public void CSWWeightFilldata (WtCommutableLineAuto cxCommutableLine)
		{
		ObjFromView = FromView.CSWWeight;
			data1 = cxCommutableLine;
		var objdata = (WtCommutableLineAuto)cxCommutableLine;
			_cellData = new commutabilityCommon();
			_cellData.BaseTime = objdata.BaseTime;
			_cellData.CheckInTime = objdata.CheckInTime;
			_cellData.City = objdata.City;
			_cellData.CommuteCity = objdata.CommuteCity;
			_cellData.ConnectTime = objdata.ConnectTime;
			_cellData.NoNights = objdata.NoNights;
			_cellData.ToHome = objdata.ToHome;
			_cellData.ToWork = objdata.ToWork;
			_cellData.Weight = objdata.Weight;
			_cellData.isNonStop = objdata.IsNonStopOnly;

			UpdateUI ();
		}

		void UpdateUI()
		{
			
			this.BackgroundColor = UIColor.White;
			//if (_cellData == null)
			//{
				_cellData = new commutabilityCommon();
				if (data1.GetType().Name == "FtCommutableLine")
				{
					var objdata = (FtCommutableLine)data1;
					_cellData.BaseTime = objdata.BaseTime;
					_cellData.CheckInTime = objdata.CheckInTime;
					_cellData.City = objdata.City;
					_cellData.CommuteCity = objdata.CommuteCity;
					_cellData.ConnectTime = objdata.ConnectTime;
					_cellData.NoNights = objdata.NoNights;
					_cellData.ToHome = objdata.ToHome;
					_cellData.ToWork = objdata.ToWork;
				_cellData.isNonStop = objdata.IsNonStopOnly;
				}
				else
				{
					var objdata = (WtCommutableLineAuto)data1;
					_cellData.BaseTime = objdata.BaseTime;
					_cellData.CheckInTime = objdata.CheckInTime;
					_cellData.City = objdata.City;
					_cellData.CommuteCity = objdata.CommuteCity;
					_cellData.ConnectTime = objdata.ConnectTime;
					_cellData.NoNights = objdata.NoNights;
					_cellData.ToHome = objdata.ToHome;
					_cellData.ToWork = objdata.ToWork;
					_cellData.Weight = objdata.Weight;
				_cellData.isNonStop = objdata.IsNonStopOnly;
			}
			//}
			//UIHelpers.StyleForButtonsBorderBlackRectangeThin (new UIButton[]{btnAny});
			lbCommuteName.Text = string.Format("Cmut Lines ({0})", _cellData.City);
			if (_cellData.NoNights) 
			{
				btnAny.BackgroundColor = Colors.BidRowGreen;
				btnAny.SetTitleColor (UIColor.Black, UIControlState.Normal);
			} 
			else  btnAny.BackgroundColor = Colors.BidOrange;
			if (_cellData.ToWork) {
				btnWork.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnWork.BackgroundColor = Colors.BidOrange;
			}
			if (_cellData.ToHome) {
				btnHome.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnHome.BackgroundColor = Colors.BidOrange;
			}

			if (ObjFromView == FromView.CSWWeight) 
			{
				btnPoint = new UIButton (new CoreGraphics.CGRect (this.Frame.Width - 40, btnHome.Frame.Y +6, 40, btnHome.Frame.Height));
				btnPoint.SetTitle (wBIdStateContent.Weights.CLAuto.Weight.ToString(), UIControlState.Normal);
				btnPoint.BackgroundColor = UIColor.Clear;
				btnPoint.SetTitleColor (UIColor.Black, UIControlState.Normal);
				btnPoint.TitleLabel.Font = UIFont.BoldSystemFontOfSize((nfloat)11.0);

				this.AddSubview (btnPoint);
				btnPoint.TouchUpInside += delegate {


					changeContraintParamNotif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString("changeWeightParamInWeightsCell"), handleParamTermChange);
					string supPopType = "Commutable Line - Auto";
					PopoverViewController popoverContent = new PopoverViewController ();
					popoverContent.PopType = "changeWeightParamInWeightsCell";
					popoverContent.SubPopType = supPopType;
					popoverContent.index = (int)this.Tag;
					popoverController = new UIPopoverController (popoverContent);
					popoverController.PopoverContentSize = new CGSize (210, 300);
					popoverController.PresentFromRect(btnPoint.Frame,this,UIPopoverArrowDirection.Any,true);

				};

			}
		}
		public void handleParamTermChange (NSNotification n)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (changeContraintParamNotif); 
			popoverController.Dismiss (true);
			btnPoint.SetTitle(n.Object.ToString(), UIControlState.Normal);
		}

		void Updatevalues ()
		{
			switch (ObjFromView) {
			case FromView.BidAutomator:
				break;
			case FromView.CSWConstraints:
				GlobalSettings.isModified = true;
				CommonClass.cswVC.UpdateSaveButton ();
				ConstraintCalculations constCalc = new ConstraintCalculations ();
				constCalc.ApplyCommutableLinesAutoConstraint (wBIdStateContent.Constraints.CLAuto);
				NSNotificationCenter.DefaultCenter.PostNotificationName ("AddConstraints", null);
				NSNotificationCenter.DefaultCenter.PostNotificationName ("LineCountReload", null);
				break;
			case FromView.CSWWeight:
				GlobalSettings.isModified = true;
				CommonClass.cswVC.UpdateSaveButton ();
				WeightCalculation constCalc1 = new WeightCalculation ();
				constCalc1.ApplyCommutableLineAuto (wBIdStateContent.Weights.CLAuto);
				break;
			}
		}

		partial void OnAnyEvent (Foundation.NSObject sender)
		{
			
			_cellData.NoNights = !_cellData.NoNights;
			if (data1.GetType().Name == "FtCommutableLine")
			{
				((FtCommutableLine)data1).NoNights = _cellData.NoNights;
			}
			else
			{
				((WtCommutableLineAuto)data1).NoNights = _cellData.NoNights;
			}
			Updatevalues ();


			UpdateUI();
		}

		partial void OnDeleteEvent (Foundation.NSObject sender){

			switch(ObjFromView)
			{
			case FromView.BidAutomator:	
				if (_parentVC != null) 
				{
						var objdata = new FtCommutableLine();
						objdata.BaseTime = _cellData.BaseTime;
						objdata.CheckInTime = _cellData.CheckInTime;
						objdata.City = _cellData.City;
						objdata.CommuteCity = _cellData.CommuteCity;
						objdata.ConnectTime = _cellData.ConnectTime;
						objdata.NoNights = _cellData.NoNights;
						objdata.ToHome = _cellData.ToHome;
						objdata.ToWork = _cellData.ToWork;
						objdata.IsNonStopOnly = _cellData.isNonStop;
						_parentVC.DeleteObject(objdata);

				}
				break;
			case FromView.CSWConstraints:
				DeleteObject(_cellData);
				break;
			case FromView.CSWWeight:
				DeleteObject(_cellData);
				break;
				
			}

		}
		public void DeleteObject(object obj)
		{
			
			var wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

			switch (ObjFromView)
			{
								case FromView.BidAutomator:
					break;
				case FromView.CSWConstraints:
					
     //               if (wBIdStateContent != null && wBIdStateContent.Constraints != null && wBIdStateContent.Constraints.DailyCommuteTimesCmmutability != null)
					//{
     //                   wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.Clear();
					//}
					wBIdStateContent.CxWtState.CLAuto.Cx = false;
					constCalc.RemoveAutoCommutableLinesConstraint();
					NSNotificationCenter.DefaultCenter.PostNotificationName("AddConstraints", null);
					NSNotificationCenter.DefaultCenter.PostNotificationName("LineCountReload", null);
					break;
					case FromView.CSWWeight:
					//if (wBIdStateContent != null && wBIdStateContent.Constraints != null && wBIdStateContent.Constraints.DailyCommuteTimesCmmutability != null)
					//{
     //                   wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.Clear();
					//}
					wBIdStateContent.CxWtState.CLAuto.Wt = false;
					weightCalc.RemoveAutoCommutableLineWeight();
					NSNotificationCenter.DefaultCenter.PostNotificationName("AddWeights", null);
					break;
			}
			
		}
		partial void OnHomeEvent (Foundation.NSObject sender){

			if(ObjFromView == FromView.BidAutomator)
			{
			if(_cellData.ToWork == true) 
			{
				_cellData.ToHome = !_cellData.ToHome;
	if (data1.GetType().Name == "FtCommutableLine")
					{
						((FtCommutableLine)data1).ToHome = _cellData.ToHome;
					}
					else
					{
						((WtCommutableLineAuto)data1).ToHome = _cellData.ToHome;
					}
			Updatevalues ();
			UpdateUI();
			}
			}
			else 
			{
				_cellData.ToHome = !_cellData.ToHome;
				if (data1.GetType().Name == "FtCommutableLine")
				{
					((FtCommutableLine)data1).ToHome = _cellData.ToHome;
				}
				else
				{
					((WtCommutableLineAuto)data1).ToHome = _cellData.ToHome;
				}
				Updatevalues ();
				UpdateUI();
			}
		}

		partial void OnRonBothEvent (Foundation.NSObject sender){
			//_cellData.IsRonBoth = true;
			_cellData.NoNights = false;
			if (data1.GetType().Name == "FtCommutableLine")
			{
				((FtCommutableLine)data1).NoNights = _cellData.NoNights;
			}
			else
			{
				((WtCommutableLineAuto)data1).NoNights = _cellData.NoNights;
			}
			Updatevalues ();

			UpdateUI();
		}

		partial void OnWorkEvent(Foundation.NSObject sender)
		{
			if (ObjFromView == FromView.BidAutomator)
			{
				if (_cellData.ToHome == true)
				{
					_cellData.ToWork = !_cellData.ToWork;
					if (data1.GetType().Name == "FtCommutableLine")
					{
						((FtCommutableLine)data1).ToWork = _cellData.ToWork;
					}
					else
					{
						((WtCommutableLineAuto)data1).ToWork = _cellData.ToWork;
					}
					Updatevalues();
					UpdateUI();
				}
			}
			else
			{
				_cellData.ToWork = !_cellData.ToWork;
				if (data1.GetType().Name == "FtCommutableLine")
				{
					((FtCommutableLine)data1).ToWork = _cellData.ToWork;
				}
				else
				{
					((WtCommutableLineAuto)data1).ToWork = _cellData.ToWork;
				}
				Updatevalues();
				UpdateUI();
			}
		}

		partial void OnCommuteLineEvent (Foundation.NSObject sender){
			switch(ObjFromView)
			{
			case FromView.BidAutomator:	
				if(_parentVC!=null)
				{
						var objdata = new FtCommutableLine();
						objdata.BaseTime = _cellData.BaseTime;
						objdata.CheckInTime = _cellData.CheckInTime;
						objdata.City = _cellData.City;
						objdata.CommuteCity = _cellData.CommuteCity;
						objdata.ConnectTime = _cellData.ConnectTime;
						objdata.NoNights = _cellData.NoNights;
						objdata.ToHome = _cellData.ToHome;
						objdata.ToWork = _cellData.ToWork;
						objdata.IsNonStopOnly = _cellData.isNonStop;
					_parentVC.PushViewControllView(objdata);
				}
				break;
			case FromView.CSWConstraints:

                    NSNotificationCenter.DefaultCenter.PostNotificationName ("ShowCommutableAuto", null);
				break;
			case FromView.CSWWeight:
                   
                    NSNotificationCenter.DefaultCenter.PostNotificationName ("ShowCommutableWeightAuto", null);
				break;

			}

		}



    }
}
