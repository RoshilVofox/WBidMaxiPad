using System;

using Foundation;
using UIKit;
using System.Linq;
using WBid.WBidiPad.iOS;
using WBid.WBidiPad.PortableLibrary;
using System.Collections.Generic;

namespace TestTablewViewLeak.ViewControllers
{
    public partial class MonthsViewController : UIViewController
    {

        public string selectedMonth;
        public UIView viewbase = new UIView();
        public UIPopoverController objpopover;
        public SecretDataDownload SuperParent;
        public MonthsViewController() : base("MonthsViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            viewbase = ViewBg;
            pickerView.Model = new pickerViewModel(this);
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        public class pickerViewModel : UIPickerViewModel
        {
            MonthsViewController parent;
            List<string> arr = WBidCollection.GetBidPeriods().Select(x => x.Period).ToList();
            public pickerViewModel(MonthsViewController parentVC)
            {
                parent = parentVC;
            }

            public override nint GetComponentCount(UIPickerView picker)
            {
                return 1;
            }

            public override nint GetRowsInComponent(UIPickerView picker, nint component)
            {
                return arr.Count();
            }

            public override string GetTitle(UIPickerView picker, nint row, nint component)
            {
                return arr[(int)row];
            }
            public override void Selected(UIPickerView picker, nint row, nint component)
            {
                parent.selectedMonth = arr[(int)row];
                parent.SuperParent.setMonthname(parent.selectedMonth);
                parent.objpopover.Dismiss(true);
            }

        }
    }
}

