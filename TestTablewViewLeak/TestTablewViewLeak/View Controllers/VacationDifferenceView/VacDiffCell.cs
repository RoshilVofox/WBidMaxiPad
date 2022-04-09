using System;

using Foundation;
using UIKit;
using WBid.WBidiPad.Model;

namespace TestTablewViewLeak.ViewControllers.VacationDifferenceView
{
    public partial class VacDiffCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("VacDiffCell");
        public static readonly UINib Nib = UINib.FromName("VacDiffDetailsCell", NSBundle.MainBundle);

     
        static VacDiffCell()
        {
            Nib = UINib.FromName("VacDiffCell", NSBundle.MainBundle);
        }

        protected VacDiffCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
        public void LabelValues(FlightDataChangeVacValues flightDataChangeVacValues)
        {
            lblLine.Text = flightDataChangeVacValues.LineNum.ToString();

            lblOldTotPay.Text = flightDataChangeVacValues.OldTotalPay.ToString();
            lblNewTotPay.Text = flightDataChangeVacValues.NewTotalPay.ToString();

            lblOldVpCu.Text = flightDataChangeVacValues.OldVPCu.ToString();
            lblNewVpCu.Text = flightDataChangeVacValues.NewVPCu.ToString();

            lblOldVpNe.Text = flightDataChangeVacValues.OldVPNe.ToString();
            lblNewVpNe.Text = flightDataChangeVacValues.NewVPNe.ToString();

            lblOldVpBo.Text = flightDataChangeVacValues.OldVPBo.ToString();
            lblNewVpBo.Text = flightDataChangeVacValues.NewVPBo.ToString();



        }
    }
}
