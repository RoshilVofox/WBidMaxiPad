using System;

using Foundation;
using UIKit;
using WBid.WBidiPad.Model;
using CoreGraphics;

namespace WBid.WBidiPad.iOS
{
	public partial class EquipmentCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("EquipmentCell");
		public static readonly UINib Nib;

		ConstraintsChangeViewController _viewController;
		EquirementConstraint _cellObject;
		private NSObject _DayOfcellNotification;
		UIPopoverController popoverController ;
		static EquipmentCell ()
		{
			Nib = UINib.FromName ("EquipmentCell", NSBundle.MainBundle);
		}

		public EquipmentCell (IntPtr handle) : base (handle)
		{
		}
		public static EquipmentCell Create()
		{
			return (EquipmentCell)Nib.Instantiate(null, null)[0];
		}
		public void Filldata(ConstraintsChangeViewController parentVC, EquirementConstraint cellData)
		{
			_viewController = parentVC;
			_cellObject = cellData;
//			_cellObject.Equirement = 500;
//			_cellObject.LessMore = "Less than";
//			_cellObject.Value = 11;
			UpdateUI ();
		}
		private void UpdateUI (){
			btnEquipment.SetTitle(String.Format("{0}",_cellObject.Value), UIControlState.Normal);
			btnEquipmentValue.SetTitle(String.Format("{0}",GetEquipmentName( _cellObject.Equipment)), UIControlState.Normal);
			btnLessthan.SetTitle(_cellObject.LessMore, UIControlState.Normal);
		}

		partial void OnEquipmentEvent (UIButton sender)
		{
//			UIActionSheet actionSheet = new UIActionSheet ("");
//			for (int i =0; i<=20;i++){
//				actionSheet.AddButton (String.Format("{0}",i));	
//				actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
//					_cellObject.Value =(int) b.ButtonIndex;
//					UpdateUI();
//				};
//			}
//			actionSheet.ShowInView (_viewController.View);
			UIButton ObjButton= (UIButton)sender;
			BAPopOverViewController ObjPopOver= new BAPopOverViewController();
			ObjPopOver.PopOverType="EquipementInNumber";
			popoverController = new UIPopoverController (ObjPopOver);
			popoverController.PopoverContentSize = new CGSize (50, 380);
			popoverController.PresentFromRect(ObjButton.Frame,this.ContentView,UIPopoverArrowDirection.Any,true);
			_DayOfcellNotification=NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("EquipementInNumber"),UpdateEquipmentCell);
		}

		public void UpdateEquipmentCell(NSNotification n)
		{
			_cellObject.Value =int.Parse( n.Object.ToString ());
			NSNotificationCenter.DefaultCenter.RemoveObserver (_DayOfcellNotification);
			popoverController.Dismiss(true);
			UpdateUI();
		}

		partial void OnEquipmentDeleteEvent (UIButton sender)
		{
			if(_viewController!=null){
				_viewController.DeleteObject(_cellObject);
			}
		}

		partial void OnEquipmentValueEvent (UIButton sender)
		{
//			UIActionSheet actionSheet = new UIActionSheet ("");
//			actionSheet.AddButton ("300");
//			actionSheet.AddButton ("500");
//			actionSheet.AddButton ("700");
//			actionSheet.AddButton ("800");
//			actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
//				switch (b.ButtonIndex){
//				case 0:
//					_cellObject.Equirement = 300;
//					break;
//				case 1:
//					_cellObject.Equirement = 500;
//					break;
//				case 2:
//					_cellObject.Equirement = 700;
//					break;
//				case 3:
//					_cellObject.Equirement = 800;
//					break;
//				}
//				UpdateUI();
//			};
//			actionSheet.ShowInView (_viewController.View);

			UIButton ObjButton= (UIButton)sender;
			BAPopOverViewController ObjPopOver= new BAPopOverViewController();
			ObjPopOver.PopOverType="OnEquipmentValueEvent";
			popoverController = new UIPopoverController (ObjPopOver);
			popoverController.PopoverContentSize = new CGSize (140, 175);
			popoverController.PresentFromRect(ObjButton.Frame,this.ContentView,UIPopoverArrowDirection.Any,true);
			_DayOfcellNotification=NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("OnEquipmentValueEvent"),UpdateCell);
		}

		public void UpdateCell(NSNotification n)
		{
			if (n.Object.ToString () == "300 & 500")
				_cellObject.Equipment = 35;
			if (n.Object.ToString() == "8Max")
				_cellObject.Equipment = 600;
			else if (n.Object.ToString() == "7Max")
				_cellObject.Equipment = 200;
			else				
				_cellObject.Equipment = int.Parse( n.Object.ToString ());
			
			NSNotificationCenter.DefaultCenter.RemoveObserver (_DayOfcellNotification);
			popoverController.Dismiss(true);
			UpdateUI();
		}
		private string GetEquipmentName(int equipment)
		{
			string result = string.Empty;

			switch (equipment)
			{
			case 300:
				result = "300";
				break;

			case 500:
				result = "300";
				break;
			case 35:
				result = "300";
				break;
			case 700:
				result = "700";
				break;
			case 800:
				result = "800";
				break;
			case 600:
				result = "8Max";
					break;
				case 200:
					result = "7Max";
					break;


			}
			return result;


		}
		partial void OnLessthanEvent (UIButton sender)
		{
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
			popoverController.PopoverContentSize = new CGSize (200, 130);
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
