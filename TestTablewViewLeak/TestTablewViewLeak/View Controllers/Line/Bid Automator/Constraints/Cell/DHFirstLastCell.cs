using System;

using Foundation;
using UIKit;
using WBid.WBidiPad.Model;
using CoreGraphics;

namespace WBid.WBidiPad.iOS
{
	public partial class DHFirstLastCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("DHFirstLastCell");
		public static readonly UINib Nib;
		ConstraintsChangeViewController _viewController;
		DHFristLastConstraint _cellObject;
		UIPopoverController popoverController ;
		private NSObject _DayOfcellNotification;
		static DHFirstLastCell ()
		{
			Nib = UINib.FromName ("DHFirstLastCell", NSBundle.MainBundle);
		}

		public DHFirstLastCell (IntPtr handle) : base (handle)
		{
		}
		public static DHFirstLastCell Create()
		{
			return (DHFirstLastCell)Nib.Instantiate(null, null)[0];
		}
		public void Filldata(ConstraintsChangeViewController parentVC, DHFristLastConstraint cellData)
		{
			_viewController = parentVC;
			_cellObject = cellData;
//			_cellObject.DH = "Both";
//			_cellObject.LessMore = "Less than";
//			_cellObject.Value = 1;
			UpdateUI ();
		}
		private void UpdateUI (){
			btnDHValue.SetTitle(String.Format("{0}",_cellObject.Value), UIControlState.Normal);
			btnDHConstraint.SetTitle(_cellObject.DH, UIControlState.Normal);
			btnLessthan.SetTitle(_cellObject.LessMore, UIControlState.Normal);
		}
				

		partial void OnDeleteEvent (Foundation.NSObject sender){
			if(_viewController!=null){
				_viewController.DeleteObject(_cellObject);
			}
		}

		partial void OnDHConstraintEvent (Foundation.NSObject sender){
//			UIActionSheet actionSheet = new UIActionSheet ("");
//			actionSheet.AddButton ("first");
//			actionSheet.AddButton ("last");
//			actionSheet.AddButton ("both");
//			actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
//				switch (b.ButtonIndex){
//				case 0:
//					_cellObject.DH = "first";
//					break;
//				case 1:
//					_cellObject.DH = "last";
//					break;
//				case 2:
//					_cellObject.DH = "both";
//					break;
//				}
//				UpdateUI();
//			};
//			actionSheet.ShowInView (_viewController.View);
			UIButton ObjButton= (UIButton)sender;
			BAPopOverViewController ObjPopOver= new BAPopOverViewController();
			ObjPopOver.PopOverType="OnDHConstraintEvent";
			popoverController = new UIPopoverController (ObjPopOver);
			popoverController.PopoverContentSize = new CGSize (80, 135);
			popoverController.PresentFromRect(ObjButton.Frame,this.ContentView,UIPopoverArrowDirection.Any,true);
			_DayOfcellNotification=NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("OnDHConstraintEvent"),UpdateDHFirstLast);
		
		}

		public void UpdateDHFirstLast(NSNotification n)
		{
			_cellObject.DH = n.Object.ToString ();
			NSNotificationCenter.DefaultCenter.RemoveObserver (_DayOfcellNotification);
			popoverController.Dismiss(true);
			UpdateUI();
		}
		partial void OnDHValueEvent (Foundation.NSObject sender){
//			UIActionSheet actionSheet = new UIActionSheet ("");
//			for (int i =0; i<=6;i++){
//				actionSheet.AddButton (String.Format("{0}",i));	
//				actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
//					_cellObject.Value =(int) b.ButtonIndex;
//					UpdateUI();
//				};
//			}
//			actionSheet.ShowInView (_viewController.View);
//		
		
			UIButton ObjButton= (UIButton)sender;
			BAPopOverViewController ObjPopOver= new BAPopOverViewController();
			ObjPopOver.PopOverType="DHNumber";
			popoverController = new UIPopoverController (ObjPopOver);
			popoverController.PopoverContentSize = new CGSize (80, 400);
			popoverController.PresentFromRect(ObjButton.Frame,this.ContentView,UIPopoverArrowDirection.Any,true);
			_DayOfcellNotification=NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("DHNumber"),UpdateDaySomeCell);
		}

		public void UpdateDaySomeCell(NSNotification n)
		{
			_cellObject.Value =int.Parse( n.Object.ToString ());
			NSNotificationCenter.DefaultCenter.RemoveObserver (_DayOfcellNotification);
			popoverController.Dismiss(true);
			UpdateUI();
		}


		partial void OnLessthanEvent (Foundation.NSObject sender){
//			UIActionSheet actionSheet = new UIActionSheet ("");
//			actionSheet.AddButton ("More than");
//			actionSheet.AddButton ("Less than");
//			actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
//				switch (b.ButtonIndex){
//				case 0:
//					_cellObject.LessMore = "More than";
//					break;
//				case 1:
//					_cellObject.LessMore = "Less than";
//					break;
//				}
//				UpdateUI();
//			};
//			actionSheet.ShowInView (_viewController.View);
			UIButton ObjButton= (UIButton)sender;
			BAPopOverViewController ObjPopOver= new BAPopOverViewController();
			ObjPopOver.PopOverType="MoreEqualLessListEvent";
			popoverController = new UIPopoverController (ObjPopOver);
			popoverController.PopoverContentSize = new CGSize (100, 130);
			popoverController.PresentFromRect(ObjButton.Frame,this.ContentView,UIPopoverArrowDirection.Any,true);
			_DayOfcellNotification=NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("MoreEqualLessListEvent"),UpdateOnLessthanEventCell);
		}
		public void UpdateOnLessthanEventCell(NSNotification n)
		{
			_cellObject.LessMore= n.Object.ToString ();
			NSNotificationCenter.DefaultCenter.RemoveObserver (_DayOfcellNotification);
			popoverController.Dismiss(true);

			UpdateUI();
		}
	}
}
