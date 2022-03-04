using UIKit;
using WBid.WBidiPad.Model;
using System.Collections.Generic;
using System.Linq;
using WBid.WBidiPad.Core;
using System;
using Foundation;
using System.Reflection;

namespace WBid.WBidiPad.iOS
{
	public class UserDifferenceTableDataSource : UITableViewSource
	{
		public RemoteUpdateUserInformation ObjremoteUserInfo = new RemoteUpdateUserInformation ();

		public UserDifferenceTableDataSource ( List<KeyValuePair<string, string>> DifferenceListt)
		{
			DifferenceList = DifferenceListt;
			LoadUserDetails ();

		}
		public List<KeyValuePair<string, string>> DifferenceList = new List<KeyValuePair<string, string>> ();
		public List<KeyValuePair<string, string>> SelectedList = new List<KeyValuePair<string, string>> ();
		public override nint NumberOfSections (UITableView tableView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			
			return DifferenceList.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			
			tableView.RegisterNibForCellReuse (UINib.FromName ("UserAccountDifferenceCell", NSBundle.MainBundle), "UserAccountDifferenceCell");
			UserAccountDifferenceCell cell = (UserAccountDifferenceCell)tableView.DequeueReusableCell (new NSString ("UserAccountDifferenceCell"), indexPath);
			cell.Tag = indexPath.Row;
			cell.Objsource = this;
			string[] values = DifferenceList[indexPath.Row].Value.Split(',');
			for(int i = 0; i < values.Length; i++)
			{
				values[i] = values[i].Trim();
				if(!(values[i].Length >0)) values[i]="Not available";



			}

		//	SelectedList.Add (KeyValuePair<string,string> (DifferenceList [indexPath.Row].Key.ToString (), values [0]));

			//value 0 - local value
			//Value 1- Remote value
			if (DifferenceList [indexPath.Row].Key.ToString () == "CarrierNum") {
				int IntLocalvalue = int.Parse (values [0]);
				int IntRemoteValue = int.Parse (values [1]);
				PropertyInfo propertyInfo = ObjremoteUserInfo.GetType().GetProperty(DifferenceList [indexPath.Row].Key);
				propertyInfo.SetValue(ObjremoteUserInfo, Convert.ChangeType(IntLocalvalue, propertyInfo.PropertyType), null);
				string local = CommonClass.CellCarrier [IntLocalvalue];
				string Remote = CommonClass.CellCarrier [IntRemoteValue];
				cell.SetDetailCell (DifferenceList [indexPath.Row].Key.ToString (), local, Remote);
			} 
			else if (DifferenceList [indexPath.Row].Key.ToString () == "Position")
			{
				string localValue="";
//				if (values [0] == "Captain") {
//					localValue = "5";
//				} else if (values [0] == "First Officer") {
//					localValue = "4";
//				} else if (values [0] == "Flight Attendant") {
//					localValue = "3";
//				}
				if (values [0] == "Pilot") {
					localValue = "4";
				}
				else if (values [0] == "Flight Attendant") {
					localValue = "3";
				}

				PropertyInfo propertyInfo = ObjremoteUserInfo.GetType().GetProperty(DifferenceList [indexPath.Row].Key);
				propertyInfo.SetValue(ObjremoteUserInfo, Convert.ChangeType(localValue, propertyInfo.PropertyType), null);
				cell.SetDetailCell (DifferenceList [indexPath.Row].Key.ToString (), values [0], values [1]);	
			}
			else {
				cell.SetDetailCell (DifferenceList [indexPath.Row].Key.ToString (), values [0], values [1]);	
				PropertyInfo propertyInfo = ObjremoteUserInfo.GetType().GetProperty(DifferenceList [indexPath.Row].Key);
				propertyInfo.SetValue(ObjremoteUserInfo, Convert.ChangeType(values[0], propertyInfo.PropertyType), null);
			}



			return cell;

		}

		void LoadUserDetails()
		{
			ObjremoteUserInfo.AcceptEmail = GlobalSettings.WbidUserContent.UserInformation.isAcceptMail;
			ObjremoteUserInfo.FirstName = GlobalSettings.WbidUserContent.UserInformation.FirstName;
			ObjremoteUserInfo.LastName = GlobalSettings.WbidUserContent.UserInformation.LastName;
			ObjremoteUserInfo.Email = GlobalSettings.WbidUserContent.UserInformation.Email;
			ObjremoteUserInfo.CellPhone = GlobalSettings.WbidUserContent.UserInformation.CellNumber;
			ObjremoteUserInfo.AcceptEmail = GlobalSettings.WbidUserContent.UserInformation.isAcceptMail;
			ObjremoteUserInfo.EmpNum =int.Parse( GlobalSettings.WbidUserContent.UserInformation.EmpNo.ToString());
			ObjremoteUserInfo.CarrierNum = GlobalSettings.WbidUserContent.UserInformation.CellCarrier;

			//ObjremoteUserInfo.Position =GlobalSettings.WbidUserContent.UserInformation.Position;
			if (GlobalSettings.WbidUserContent.UserInformation.Position == "CP" || GlobalSettings.WbidUserContent.UserInformation.Position == "FO"|| GlobalSettings.WbidUserContent.UserInformation.Position == "Pilot")
			{
				ObjremoteUserInfo.Position = 4;
			}

			else if (GlobalSettings.WbidUserContent.UserInformation.Position == "FA")
			{
				ObjremoteUserInfo.Position = 3;
			}
			ObjremoteUserInfo.BidBase = GlobalSettings.WbidUserContent.UserInformation.Domicile;

			//ObjremoteUserInfo.AcceptEmail = GlobalSettings.WbidUserContent.UserInformation.isAcceptMail;
		} 
//		void Test(string s, string v)
//		{
//			RemoteUserInformation remoteUserdetails = new RemoteUserInformation ();
//			PropertyInfo propertyInfo = remoteUserdetails.GetType().GetProperty(s);
//			propertyInfo.SetValue(remoteUserdetails, Convert.ChangeType(v, propertyInfo.PropertyType), null);
//		}

		public void Changesegment(int row, int SegValue)
		{

			string[] values = DifferenceList[row].Value.Split(',');
			for(int i = 0; i < values.Length; i++)
			{
				values[i] = values[i].Trim();
				if(!(values[i].Length >0)) values[i]="Not available";



			}

			if (DifferenceList [row].Key.ToString () == "Position")
			{
				string localValue="";
				if (values[SegValue] == "Pilot")
					localValue = "4";
				else if (values [SegValue] == "Captain") {
					localValue = "5";
				} else if (values [SegValue] == "First Officer") {
					localValue = "4";
				} else if (values [SegValue] == "Flight Attendant") {
					localValue = "3";
				}
				values [SegValue] = localValue;


			}
//			else if (DifferenceList [row].Key.ToString () == "CarrierNum")
//				{
//				int index1 = int.Parse( values[SegValue].ToString());
//				values [SegValue] = index1.ToString ();
//				}

			PropertyInfo propertyInfo = ObjremoteUserInfo.GetType().GetProperty(DifferenceList [row].Key);
			propertyInfo.SetValue(ObjremoteUserInfo, Convert.ChangeType(values[SegValue], propertyInfo.PropertyType), null);



		}
		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return 40;
		}
	}
}
