using System;

using Foundation;
using UIKit;

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
    }
}
