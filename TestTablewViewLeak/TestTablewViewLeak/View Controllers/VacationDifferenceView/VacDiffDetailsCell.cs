using System;
using Foundation;
using UIKit;
using WBid.WBidiPad.Model;

namespace TestTablewViewLeak.ViewControllers.VacationDifferenceView
{
    public partial class VacDiffDetailsCell : UITableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("VacDiffDetailsCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("VacDiffDetailsCell");
        //public VacDiffDetailsCell() : base("VacDiffDetailsCell", null)
        //{
        //}

        //public override void ViewDidLoad()
        //{
        //    base.ViewDidLoad();
        //    // Perform any additional setup after loading the view, typically from a nib.
        //}

        //public override void DidReceiveMemoryWarning()
        //{
        //    base.DidReceiveMemoryWarning();
        //    // Release any cached data, images, etc that aren't in use.
        //}
        public VacDiffDetailsCell(IntPtr handle) : base(handle)
        {
        }

        public static VacDiffDetailsCell Create()
        {
            return (VacDiffDetailsCell)Nib.Instantiate(null, null)[0];
        }

        public void LabelValues(FlightDataChangeVacValues flightDataChangeVacValues)
        {
            lblLine.Text = flightDataChangeVacValues.LineNum.ToString();
            lblOldTotPay.Text = flightDataChangeVacValues.OldTotalPay.ToString();
            lblNewTotPay.Text = flightDataChangeVacValues.NewTotalPay.ToString();
            //lblLine.Text = flightDataChangeVacValues.LineNum.ToString();
           // lblLine.Text = flightDataChangeVacValues.LineNum.ToString();
           // lblLine.Text = flightDataChangeVacValues.LineNum.ToString();


        }
    }
}

