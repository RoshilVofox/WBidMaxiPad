using System;

using Foundation;
using UIKit;
using WBid.WBidiPad.Model;
using CoreGraphics;


namespace WBid.WBidiPad.iOS
{
	public partial class DaysOfWeekSomeCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("DaysOfWeekSomeCell");
		public static readonly UINib Nib;
		ConstraintsChangeViewController _viewController;
		DaysOfWeekSome _cellObject;
		private NSObject _DayOfcellNotification;
		UIPopoverController popoverController ;
		static DaysOfWeekSomeCell ()
		{
			Nib = UINib.FromName ("DaysOfWeekSomeCell", NSBundle.MainBundle);
		}

		public DaysOfWeekSomeCell (IntPtr handle) : base (handle)
		{
		}
		public static DaysOfWeekSomeCell Create()
		{
			return (DaysOfWeekSomeCell)Nib.Instantiate(null, null)[0];
		}
		public void Filldata(ConstraintsChangeViewController parentVC, DaysOfWeekSome cellData)
		{
			_viewController = parentVC;
			_cellObject = cellData;
//			_cellObject.Date = "Mon";
//			_cellObject.LessOrMore = "Less than";
//			_cellObject.Value = 1;
			UpdateUI ();
		}
		private void UpdateUI (){
			
			btnDaySomeValue.SetTitle(String.Format("{0}",_cellObject.Value), UIControlState.Normal);
			btnDaySomeConstraint.SetTitle(_cellObject.Date, UIControlState.Normal);
			btnLessthan.SetTitle(_cellObject.LessOrMore, UIControlState.Normal);
		}

		partial void OnDaySomeConstraintEvent (Foundation.NSObject sender){
			
//			UIActionSheet actionSheet = new UIActionSheet (new CoreGraphics.CGRect (ObjButton.Frame.X,ObjButton.Frame.Y,50,50));
//		
//			//actionSheet.Frame= new CoreGraphics.CGRect (ObjButton.Frame.X,ObjButton.Frame.Y,100,250);
//			actionSheet.AddButton ("Sun");
//			actionSheet.AddButton ("Mon");
//			actionSheet.AddButton ("Tue");
//			actionSheet.AddButton ("Wed");
//			actionSheet.AddButton ("Thu");
//			actionSheet.AddButton ("Fri");
//			actionSheet.AddButton ("Sat");
//			actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
//				switch (b.ButtonIndex){
//				case 0:
//					_cellObject.Date = "Sun";
//					break;
//				case 1:
//					_cellObject.Date = "Mon";
//					break;
//				case 2:
//					_cellObject.Date = "Tue";
//					break;
//				case 3:
//					_cellObject.Date = "Wed";
//					break;
//				case 4:
//					_cellObject.Date = "Thu";
//					break;
//				case 5:
//					_cellObject.Date = "Fri";
//					break;
//				case 6:
//					_cellObject.Date = "Sat";
//					break;
//				}
//				UpdateUI();
//			};
//
		//actionSheet.ShowInView (this.ContentView);
			UIButton ObjButton= (UIButton)sender;
			BAPopOverViewController ObjPopOver= new BAPopOverViewController();
			ObjPopOver.PopOverType="DaysInWeek";
			popoverController = new UIPopoverController (ObjPopOver);
			popoverController.PopoverContentSize = new CGSize (80, 300);
			popoverController.PresentFromRect(ObjButton.Frame,this.ContentView,UIPopoverArrowDirection.Any,true);
			_DayOfcellNotification=NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("DaysInWeek"),UpdateCell);
		}

		public void UpdateCell(NSNotification n)
		{
				_cellObject.Date = n.Object.ToString ();
			NSNotificationCenter.DefaultCenter.RemoveObserver (_DayOfcellNotification);
			popoverController.Dismiss(true);
			UpdateUI();
		}

		partial void OnDaySomeValueEvent (Foundation.NSObject sender){
			
			//UIActionSheet actionSheet = new UIActionSheet ("");
//			for (int i =0; i<=6;i++){
//				actionSheet.AddButton (String.Format("{0}",i));	
//				actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
//					_cellObject.Value =(int) b.ButtonIndex;
//					UpdateUI();
//				};
//			}
			//actionSheet.ShowInView (this.ContentView);
			UIButton ObjButton= (UIButton)sender;
			BAPopOverViewController ObjPopOver= new BAPopOverViewController();
			ObjPopOver.PopOverType="DaysInNumber";
			popoverController = new UIPopoverController (ObjPopOver);
			popoverController.PopoverContentSize = new CGSize (80, 400);
			popoverController.PresentFromRect(ObjButton.Frame,this.ContentView,UIPopoverArrowDirection.Any,true);
			_DayOfcellNotification=NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("DaysInNumber"),UpdateDaySomeCell);
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
//					_cellObject.LessOrMore = "More than";
//					break;
//				case 1:
//					_cellObject.LessOrMore = "Less than";
//					break;
//				}
//				UpdateUI();
//			};
//			actionSheet.ShowInView (this.ContentView);


			UIButton ObjButton= (UIButton)sender;
			BAPopOverViewController ObjPopOver= new BAPopOverViewController();
			ObjPopOver.PopOverType="MoreEqualLessListEvent";
			popoverController = new UIPopoverController (ObjPopOver);
			popoverController.PopoverContentSize = new CGSize (130, 130);
			popoverController.PresentFromRect(ObjButton.Frame,this.ContentView,UIPopoverArrowDirection.Any,true);
			_DayOfcellNotification=NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("MoreEqualLessListEvent"),UpdateOnLessthanEventCell);
		}
		public void UpdateOnLessthanEventCell(NSNotification n)
		{
			_cellObject.LessOrMore= n.Object.ToString ();
			NSNotificationCenter.DefaultCenter.RemoveObserver (_DayOfcellNotification);
			popoverController.Dismiss(true);

			UpdateUI();
		}
		partial void OnDeleteEvent (Foundation.NSObject sender){
			if(_viewController!=null){
				_viewController.DeleteObject(_cellObject);
			}
		}
	}
}
