using System;

using Foundation;
using UIKit;
using System.Collections.Generic;
using WBid.WBidiPad.Model;
using CoreGraphics;


namespace WBid.WBidiPad.iOS
{
	public partial class RestCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("RestCell");
		public static readonly UINib Nib;
		ConstraintsChangeViewController _viewController;
		RestCx _cellObject;
		List<int> restValues = new List<int>();
		private NSObject _DayOfcellNotification;
		UIPopoverController popoverController ;

		static RestCell ()
		{
			Nib = UINib.FromName ("RestCell", NSBundle.MainBundle);
		}

		public RestCell (IntPtr handle) : base (handle)
		{
		}
		public static RestCell Create()
		{
			return (RestCell)Nib.Instantiate(null, null)[0];
		}
		public void Filldata(ConstraintsChangeViewController parentVC, RestCx cellData)
		{
			_viewController = parentVC;
			_cellObject = cellData;
//			_cellObject.Dom = "All";
//			_cellObject.LessMore = "Less than";
//			_cellObject.Value = 0;
			UpdateUI ();
		}
		private void UpdateUI (){
			if(_cellObject!=null){
				btnRestDom.SetTitle(_cellObject.Dom, UIControlState.Normal);
				btnLessthan.SetTitle(_cellObject.LessMore, UIControlState.Normal);
				btnRestValue.SetTitle(String.Format("{0}",_cellObject.Value), UIControlState.Normal);
			}
		}

		partial void OnAwayDomEvent (Foundation.NSObject sender){
			if(_viewController!=null){
				
//				UIActionSheet actionSheet = new UIActionSheet ("");
//				actionSheet.AddButton ("All");
//				actionSheet.AddButton ("inDom");
//				actionSheet.AddButton ("AwayDom");
//				actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
//					switch (b.ButtonIndex){
//					case 0:
//						_cellObject.Dom = "All";
//						break;
//					case 1:
//						_cellObject.Dom = "inDom";
//						break;
//					case 2:
//						_cellObject.Dom = "AwayDom";
//						break;
//					}
//					UpdateUI();
//				};
//				actionSheet.ShowInView (_viewController.View);
				UIButton ObjButton= (UIButton)sender;
				BAPopOverViewController ObjPopOver= new BAPopOverViewController();
				ObjPopOver.PopOverType="OnAwayDomEvent";
				popoverController = new UIPopoverController (ObjPopOver);
				popoverController.PopoverContentSize = new CGSize (110, 200);
				popoverController.PresentFromRect(ObjButton.Frame,this.ContentView,UIPopoverArrowDirection.Any,true);
				_DayOfcellNotification=NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("OnAwayDomEvent"),UpdateCell);
			}
		}

	

	public void UpdateCell(NSNotification n)
	{
			_cellObject.Dom = n.Object.ToString ();
		NSNotificationCenter.DefaultCenter.RemoveObserver (_DayOfcellNotification);
		popoverController.Dismiss(true);
		UpdateUI();
	}

		partial void OnDeleteEvent (Foundation.NSObject sender)
		{
			if(_viewController!=null){
				_viewController.DeleteObject(_cellObject);
			}
		}
			
		partial void OnLessThanEvent (Foundation.NSObject sender){
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
			ObjPopOver.PopOverType="LessthanEvent";
			popoverController = new UIPopoverController (ObjPopOver);
			popoverController.PopoverContentSize = new CGSize (200, 100);
			popoverController.PresentFromRect(ObjButton.Frame,this.ContentView,UIPopoverArrowDirection.Any,true);
			_DayOfcellNotification=NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("LessthanEvent"),UpdateOnLessthanEventCell);
		}
		public void UpdateOnLessthanEventCell(NSNotification n)
		{
			_cellObject.LessMore= n.Object.ToString ();
			NSNotificationCenter.DefaultCenter.RemoveObserver (_DayOfcellNotification);
			popoverController.Dismiss(true);

			UpdateUI();
		}

		partial void OnValueEvent (Foundation.NSObject sender){
//			UIActionSheet actionSheet = new UIActionSheet ("");
//			for (int i =0; i<=48;i++){
//				actionSheet.AddButton (String.Format("{0}",i));	
//				actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
//					_cellObject.Value =(int) b.ButtonIndex;
//					UpdateUI();
//				};
//			}
//			actionSheet.ShowInView (_viewController.View);
			UIButton ObjButton= (UIButton)sender;
			BAPopOverViewController ObjPopOver= new BAPopOverViewController();
			ObjPopOver.PopOverType="RestOnValueEvent";
			popoverController = new UIPopoverController (ObjPopOver);
			popoverController.PopoverContentSize = new CGSize (100, 380);
			popoverController.PresentFromRect(ObjButton.Frame,this.ContentView,UIPopoverArrowDirection.Any,true);
			_DayOfcellNotification=NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("RestOnValueEvent"),UpdateOnValueEventCell);
		}
		public void UpdateOnValueEventCell(NSNotification n)
		{
			_cellObject.Value= int.Parse( n.Object.ToString ());
			NSNotificationCenter.DefaultCenter.RemoveObserver (_DayOfcellNotification);
			popoverController.Dismiss(true);
			UpdateUI();
		}
	}
}
