using System;

using Foundation;
using UIKit;
using WBid.WBidiPad.Model;

namespace TestTablewViewLeak.ViewControllers.CommuteDifferenceView
{
    public partial class CommutDiffTableViewCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("CommutDiffTableViewCell");
        public static readonly UINib Nib;

        static CommutDiffTableViewCell()
        {
            Nib = UINib.FromName("CommutDiffTableViewCell", NSBundle.MainBundle);
        }

        protected CommutDiffTableViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
        public void LabelValues(CommuteFltChangeValues flightDataChangeCommutValues)
        {
            //lblLine.Text = flightDataChangeVacValues.LineNum.ToString();

            //lblOldTotPay.Text = flightDataChangeVacValues.OldTotalPay.ToString();
            //lblNewTotPay.Text = flightDataChangeVacValues.NewTotalPay.ToString();

            //lblOldVpCu.Text = flightDataChangeVacValues.OldVPCu.ToString();
            //lblNewVpCu.Text = flightDataChangeVacValues.NewVPCu.ToString();

            //lblOldVpNe.Text = flightDataChangeVacValues.OldVPNe.ToString();
            //lblNewVpNe.Text = flightDataChangeVacValues.NewVPNe.ToString();

            //lblOldVpBo.Text = flightDataChangeVacValues.OldVPBo.ToString();
            //lblNewVpBo.Text = flightDataChangeVacValues.NewVPBo.ToString();



        }
    }
}
