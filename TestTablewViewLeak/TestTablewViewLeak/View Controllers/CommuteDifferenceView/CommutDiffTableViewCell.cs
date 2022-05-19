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
        public void LabelValues(CommuteFltChangeValues flightDataChangeVacValues)
        {
            lblLine.Text = flightDataChangeVacValues.LineNum.ToString();

            lblOldCmtOv.Text = flightDataChangeVacValues.OldCmtOV.ToString();
            lblNewCmtOv.Text = flightDataChangeVacValues.NewCmtOV.ToString();

            lblOldCmtFr.Text = flightDataChangeVacValues.OldCmtFr.ToString();
            lblNewCmtFr.Text = flightDataChangeVacValues.NewCmtFr.ToString();

            lblOldCmtBa.Text = flightDataChangeVacValues.OldCmtBa.ToString();
            lblNewCmtBa.Text = flightDataChangeVacValues.NewCmtBa.ToString();

            



        }
    }
}
