
using System;

using Foundation;
using UIKit;
using System.Collections.ObjectModel;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary;

namespace WBid.WBidiPad.iOS
{
	public class BAPopOverViewTableViewSource : UITableViewSource
	{
		public string PopOverType;

		public BAPopOverViewTableViewSource (string OverType)
		{
			PopOverType =OverType;
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			// TODO: return the actual number of items in the section
			if (PopOverType == "DaysInWeek")
				return Constants.DaysInWeek.Count;
			else if (PopOverType == "DaysInNumber")
				return 21;
			else if (PopOverType == "LessthanEvent")
				return Constants.MoreOrLessList.Count;
			else if( PopOverType  =="MoreEqualLessListEvent")
				return Constants.MoreEqualLessList.Count;
			else if (PopOverType == "OnDHConstraintEvent")
				return Constants.DHFirstlastList.Count;
			else if (PopOverType == "OnEquipmentValueEvent")
				return Constants.EquipmentValue.Count;
			else if (PopOverType == "EquipementInNumber")
				return 21;
			else if (PopOverType == "DHNumber")
				return 26;
			else if (PopOverType == "OnAwayDomEvent")
				return Constants.DomEvent.Count;
			else if (PopOverType == "OnValueEvent")
				return 49;
			else if (PopOverType == "RestOnValueEvent")
				return 41;
			else if (PopOverType == "OnPadCheckInEvent")
				return 180 / 5;
			else if (PopOverType == "OnBackToBaseEvent")
				return 60 / 5;
			else if (PopOverType == "AddSort") 
			{
				ObservableCollection<BlockSort> lstblockData = WBidCollection.GetBlockSortListData ();
				return lstblockData.Count;
			}
				
			
			return 1;
		}

//		public override string TitleForHeader (UITableView tableView, nint section)
//		{
//			return "Header";
//		}
//
//		public override string TitleForFooter (UITableView tableView, nint section)
//		{
//			return "Footer";
//		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (BAPopOverViewTableViewCell.Key) as BAPopOverViewTableViewCell;
			if (cell == null)
				cell = new BAPopOverViewTableViewCell ();
			
			// TODO: populate the cell with the appropriate data based on the indexPath
			//cell.DetailTextLabel.Text = Constants.DaysInWeek[indexPath.Row];
			if (PopOverType == "DaysInWeek")
				cell.SetCell (Constants.DaysInWeek [indexPath.Row]);
			else if (PopOverType == "DaysInNumber")
				cell.SetCell (indexPath.Row.ToString ());
			else if (PopOverType == "LessthanEvent")
				cell.SetCell (Constants.MoreOrLessList [indexPath.Row]);
			else if (PopOverType == "MoreEqualLessListEvent")
				cell.SetCell (Constants.MoreEqualLessList [indexPath.Row]);
			
			else if (PopOverType == "OnDHConstraintEvent")
				cell.SetCell (Constants.DHFirstlastList [indexPath.Row]);
			else if (PopOverType == "OnEquipmentValueEvent")
				cell.SetCell (Constants.EquipmentValue [indexPath.Row]);
			else if (PopOverType == "DHNumber")
				cell.SetCell (indexPath.Row.ToString ());
			else if (PopOverType == "EquipementInNumber")
				cell.SetCell (indexPath.Row.ToString ());
			else if (PopOverType == "OnAwayDomEvent")
				cell.SetCell (Constants.DomEvent [indexPath.Row]);
			else if (PopOverType == "OnValueEvent")
				cell.SetCell (indexPath.Row.ToString ());
			else if (PopOverType == "RestOnValueEvent") {
				cell.SetCell ((indexPath.Row +8).ToString ());
			}
			else if (PopOverType == "OnPadCheckInEvent") {
				int i = (indexPath.Row + 1) * 5;
				string title = string.Format("{0}:{1}", (i/60).ToString("00"), (i%60).ToString("00"));
				cell.SetCell (title);
			}
			else if (PopOverType == "OnBackToBaseEvent") {
				int i = (indexPath.Row + 1) * 5;
				string title = string.Format("{0}:{1}", (i/60).ToString("00"), (i%60).ToString("00"));
				cell.SetCell (title);
			}
			else if (PopOverType == "AddSort") {
				ObservableCollection<BlockSort> lstblockData = WBidCollection.GetBlockSortListData ();
				string title = lstblockData[indexPath.Row].Name;
				cell.SetCell (title);
			}
			return cell;
		}
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			if (PopOverType == "DaysInWeek")
				NSNotificationCenter.DefaultCenter.PostNotificationName("DaysInWeek",new Foundation.NSString(Constants.DaysInWeek[indexPath.Row]));
			else if (PopOverType == "DaysInNumber")
				NSNotificationCenter.DefaultCenter.PostNotificationName ("DaysInNumber", new Foundation.NSString (indexPath.Row.ToString ()));
			else if (PopOverType == "DHNumber")
				NSNotificationCenter.DefaultCenter.PostNotificationName ("DHNumber", new Foundation.NSString (indexPath.Row.ToString ()));
			else if (PopOverType == "LessthanEvent")
				NSNotificationCenter.DefaultCenter.PostNotificationName ("LessthanEvent", new Foundation.NSString (Constants.MoreOrLessList [indexPath.Row]));
			else if (PopOverType == "OnDHConstraintEvent")
				NSNotificationCenter.DefaultCenter.PostNotificationName ("OnDHConstraintEvent", new Foundation.NSString (Constants.DHFirstlastList [indexPath.Row]));
			else if (PopOverType == "MoreEqualLessListEvent")
				NSNotificationCenter.DefaultCenter.PostNotificationName ("MoreEqualLessListEvent", new Foundation.NSString (Constants.MoreEqualLessList [indexPath.Row]));
			else if (PopOverType == "OnEquipmentValueEvent")
				NSNotificationCenter.DefaultCenter.PostNotificationName ("OnEquipmentValueEvent", new Foundation.NSString (Constants.EquipmentValue [indexPath.Row]));
			else if (PopOverType == "EquipementInNumber")
				NSNotificationCenter.DefaultCenter.PostNotificationName ("EquipementInNumber", new Foundation.NSString (indexPath.Row.ToString ()));
			else if (PopOverType == "OnAwayDomEvent")
				NSNotificationCenter.DefaultCenter.PostNotificationName ("OnAwayDomEvent", new Foundation.NSString (Constants.DomEvent [indexPath.Row]));
			else if (PopOverType == "OnValueEvent")
				NSNotificationCenter.DefaultCenter.PostNotificationName ("OnValueEvent", new Foundation.NSString (indexPath.Row.ToString ()));
			else if (PopOverType == "RestOnValueEvent")
				NSNotificationCenter.DefaultCenter.PostNotificationName ("RestOnValueEvent", new Foundation.NSString ((indexPath.Row+8).ToString ()));
			
			else if (PopOverType == "OnPadCheckInEvent")
				NSNotificationCenter.DefaultCenter.PostNotificationName ("OnPadCheckInEvent", new Foundation.NSString (indexPath.Row.ToString ()));
			else if (PopOverType == "OnBackToBaseEvent")
				NSNotificationCenter.DefaultCenter.PostNotificationName ("OnBackToBaseEvent", new Foundation.NSString (indexPath.Row.ToString ()));
			else if (PopOverType == "AddSort") {
				ObservableCollection<BlockSort> lstblockData = WBidCollection.GetBlockSortListData ();
				NSNotificationCenter.DefaultCenter.PostNotificationName ("BAAddSort", new Foundation.NSString (lstblockData [indexPath.Row].Id.ToString()));
			}
		}


	}
}

