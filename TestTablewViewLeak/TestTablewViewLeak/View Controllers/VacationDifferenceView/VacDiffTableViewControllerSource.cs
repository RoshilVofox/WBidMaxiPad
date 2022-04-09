using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using WBid.WBidiPad.Model;

namespace TestTablewViewLeak.ViewControllers.VacationDifferenceView
{
    public class VacDiffTableViewControllerSource: UITableViewSource
    {
        private List<FlightDataChangeVacValues> _lstFlightDataChangevalues { get; set; }
        public VacDiffTableViewControllerSource(List<FlightDataChangeVacValues> lstFlightDataChangevalues)
        {
            _lstFlightDataChangevalues = lstFlightDataChangevalues;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.RegisterNibForCellReuse(UINib.FromName("VacDiffCell", NSBundle.MainBundle), "VacDiffCell");
           


            VacDiffCell cell = (VacDiffCell)tableView.DequeueReusableCell(new NSString("VacDiffCell"), indexPath);
            string domicile = string.Empty;
            string position = string.Empty;
            string previousmonthcap = string.Empty;
            string currentmonthcap = string.Empty;

            //previousmonthcap = (lstCAPOutputParameter[indexPath.Row].PreviousMonthCap == null) ? string.Empty : lstCAPOutputParameter[indexPath.Row].PreviousMonthCap.ToString();
            //currentmonthcap = (lstCAPOutputParameter[indexPath.Row].CurrentMonthCap == null) ? string.Empty : lstCAPOutputParameter[indexPath.Row].CurrentMonthCap.ToString();

            cell.LabelValues(_lstFlightDataChangevalues[indexPath.Row]);
            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _lstFlightDataChangevalues.Count;
        }
        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
            //return base.NumberOfSections(tableView);
        }
        public override string TitleForHeader(UITableView tableView, nint section)
        {
            //return base.TitleForHeader(tableView, section);
            return "c";
        }
        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
          

            UIView view = new UIView(new System.Drawing.RectangleF(0, 0, 443, 20));
            view.BackgroundColor = UIColor.White;

            UILabel label = new UILabel();
            label.TextColor = UIColor.Black;
            label.TextAlignment = UITextAlignment.Center;
            label.Frame = new System.Drawing.RectangleF(27, 10, 43, 20);
            label.Text = "Line";
            view.AddSubview(label);

            UILabel label1 = new UILabel();
            label1.TextColor = UIColor.Black;
            label1.TextAlignment = UITextAlignment.Center;
            label1.Frame = new System.Drawing.RectangleF(142, 10, 43, 20);
            label1.Text = "Old Tot Pay";
            view.AddSubview(label1);

            UILabel label2 = new UILabel();
            label2.TextColor = UIColor.Black;
            label2.TextAlignment = UITextAlignment.Center;
            label2.Frame = new System.Drawing.RectangleF(242, 10, 43, 20);
            label2.Text = "New Tot Pay";
            view.AddSubview(label2);

            UILabel label3 = new UILabel();
            label3.TextColor = UIColor.Black;
            label3.TextAlignment = UITextAlignment.Center;
            label3.Frame = new System.Drawing.RectangleF(342, 10, 43, 20);
            label3.Text = "Old VpCu";
            view.AddSubview(label3);

            UILabel label4 = new UILabel();
            label4.TextColor = UIColor.Black;
            label4.TextAlignment = UITextAlignment.Center;
            label4.Frame = new System.Drawing.RectangleF(442, 10, 43, 20);
            label4.Text = "New VpCu";
            view.AddSubview(label4);


            UILabel label5 = new UILabel();
            label5.TextColor = UIColor.Black;
            label5.TextAlignment = UITextAlignment.Center;
            label5.Frame = new System.Drawing.RectangleF(542, 10, 43, 20);
            label5.Text = "Old VpNe";
            view.AddSubview(label5);

            UILabel label6 = new UILabel();
            label6.TextColor = UIColor.Black;
            label6.TextAlignment = UITextAlignment.Center;
            label6.Frame = new System.Drawing.RectangleF(642, 10, 43, 20);
            label6.Text = "New VpNe";
            view.AddSubview(label6);

            UILabel label7 = new UILabel();
            label7.TextColor = UIColor.Black;
            label7.TextAlignment = UITextAlignment.Center;
            label7.Frame = new System.Drawing.RectangleF(742, 10, 43, 20);
            label7.Text = "Old VpBo";
            view.AddSubview(label7);

            UILabel label8 = new UILabel();
            label8.TextColor = UIColor.Black;
            label8.TextAlignment = UITextAlignment.Center;
            label8.Frame = new System.Drawing.RectangleF(842, 10, 43, 20);
            label8.Text = "New VpBo";
            view.AddSubview(label8);



            return view;
        }
    }
}
