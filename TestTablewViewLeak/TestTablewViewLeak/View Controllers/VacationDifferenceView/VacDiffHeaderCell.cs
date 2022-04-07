using System;

using Foundation;
using UIKit;

namespace TestTablewViewLeak.ViewControllers.VacationDifferenceView
{
    public partial class VacDiffHeaderCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("VacDiffHeaderCell");
        public static readonly UINib Nib;

        static VacDiffHeaderCell()
        {
            Nib = UINib.FromName("VacDiffHeaderCell", NSBundle.MainBundle);
        }

        protected VacDiffHeaderCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}
