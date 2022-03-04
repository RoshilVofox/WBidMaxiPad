
using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.SharedLibrary.Utility;
using System.Collections.Generic;
using WBid.WBidiPad.iOS.Utility;
//using WBid.WBidiPad.PortableLibrary.Core;
using System.Linq;
using WBid.WBidiPad.Core;
//using WBid.WBidiPad.iOS.Common;


namespace WBid.WBidiPad.iOS
{
	public class CAPTableViewControllerSource : UITableViewSource
	{
		List<CAPOutputParameter> lstCAPOutputParameter;
		public CAPDetailCell obj;
		public CAPTableViewControllerSource (List<CAPOutputParameter> lstParameter)
		{
			lstCAPOutputParameter = lstParameter;
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			// TODO: return the actual number of items in the section
			return lstCAPOutputParameter.Count;
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return "c" ;
		}

//		public override string TitleForFooter (UITableView tableView, nint section)
//		{
//			return;
//		}
		//public override float GetHeightForHeader(UITableView tableView, int section)
		//{
		//	return 10;

		//}
		public override UIView GetViewForHeader(UITableView tableView, nint section)
		{

//			CAPHeaderCell cell = (CAPHeaderCell)tableView.DequeueReusableHeaderFooterView("CAPHeaderCell");

			UIView view = new UIView(new System.Drawing.RectangleF(0,0,443,20));
			view.BackgroundColor = UIColor.White;

			UILabel label = new UILabel();
//			label.Opaque = false;
			label.TextColor = UIColor.Black;
			label.TextAlignment = UITextAlignment.Center;
//			label.Font = UIFont.FromName("Helvetica-Bold", 16f);
			label.Frame = new System.Drawing.RectangleF(27,10,43,20);
			label.Text = "Base";
			view.AddSubview(label);

			UILabel label1 = new UILabel ();
			label1.TextColor = UIColor.Black;
			label1.TextAlignment = UITextAlignment.Center;
			label1.Frame = new System.Drawing.RectangleF(142,10,43,20);
			label1.Text="Seat";
			view.AddSubview (label1);

			UILabel label2 = new UILabel ();
			label2.TextColor = UIColor.Black;
			label2.TextAlignment = UITextAlignment.Center;
			label2.Frame = new System.Drawing.RectangleF(247,10,43,20);
			//label2.Text= MonthValues((GlobalSettings.CurrentBidDetails.Month));
			label2.Text= MonthValues((DateTime.Now.Month));
			view.AddSubview (label2);

			UILabel label3 = new UILabel ();
			label3.TextColor = UIColor.Black;
			label3.TextAlignment = UITextAlignment.Center;
			label3.Frame = new System.Drawing.RectangleF(361,10,43,20);
			//label3.Text= MonthValues((GlobalSettings.CurrentBidDetails.Month+1));
			label3.Text= MonthValues((DateTime.Now.Month+1));
			view.AddSubview (label3);



			return view;


		}
		string MonthValues(int month)
		{
			string objMonth="";
			switch (month)
			{
			case 1:case 13:
				objMonth="Jan";
				break;
			case 2:
				objMonth="Feb";
				break;
			case 3:
				objMonth="Mar";
				break;
			case 4:
				objMonth="Apr";
				break;
			case 5:
				objMonth = "May";
				break;
			case 6:
				objMonth = "Jun";
				break;
			case 7:
				objMonth = "Jul";
				break;
			case 8:
				objMonth = "Aug";
				break;
			case 9:
				objMonth = "Sep";
				break;
			case 10:
				objMonth = "Oct";
				break;
			case 11:
				objMonth = "Nov";
				break;
			case 12:case 0:
				objMonth = "Dec";
				break;
				
			}

			return objMonth;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.RegisterNibForCellReuse(UINib.FromName("CAPDetailCell", NSBundle.MainBundle), "CAPDetailCell");

			CAPDetailCell cell = (CAPDetailCell)tableView.DequeueReusableCell(new NSString("CAPDetailCell"), indexPath);
            string domicile = string.Empty;
            string position = string.Empty;
            string previousmonthcap = string.Empty;
            string currentmonthcap = string.Empty;

            previousmonthcap = (lstCAPOutputParameter[indexPath.Row].PreviousMonthCap == null) ? string.Empty : lstCAPOutputParameter[indexPath.Row].PreviousMonthCap.ToString();
            currentmonthcap = (lstCAPOutputParameter[indexPath.Row].CurrentMonthCap == null) ? string.Empty : lstCAPOutputParameter[indexPath.Row].CurrentMonthCap.ToString();

            cell.LabelValues (lstCAPOutputParameter [indexPath.Row].Domicile, lstCAPOutputParameter [indexPath.Row].Position, previousmonthcap, currentmonthcap);
            return cell;

		}
	}
}

