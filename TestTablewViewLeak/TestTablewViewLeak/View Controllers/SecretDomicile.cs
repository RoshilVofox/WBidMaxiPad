using System;

using Foundation;
using UIKit;

namespace TestTablewViewLeak.ViewControllers
{
    public partial class SecretDomicile : UITableViewCell
    {
        public SecretDataDownload SuperParent;
        public static readonly UINib Nib = UINib.FromName("SecretDomicile", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("SecretDomicile");

        public SecretDomicile(IntPtr handle) : base(handle)
        {
        }

        partial void btncheckBoxclicked(NSObject sender)
        {
            if(!((UIButton)sender).Selected) {
                SuperParent.setSelectedBases((int)((UIButton)sender).Tag);
            }
            else {
                SuperParent.removeSelectedBases((int)((UIButton)sender).Tag);
            }
            ((UIButton)sender).Selected = !((UIButton)sender).Selected;
        }

        public static SecretDomicile Create()
        {
            return (SecretDomicile)Nib.Instantiate(null, null)[0];
        }


        public void LabelValues(string Domicile, int indexpath,SecretDataDownload parent)
        {
            lblBase.Text = Domicile;
            btncheckBox.Tag = indexpath + 1;
            SuperParent = parent;

    }
    }
}

