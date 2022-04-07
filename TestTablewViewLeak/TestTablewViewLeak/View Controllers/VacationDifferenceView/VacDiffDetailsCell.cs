using System;

using UIKit;

namespace TestTablewViewLeak.ViewControllers.VacationDifferenceView
{
    public partial class VacDiffDetailsCell : UIViewController
    {
        public VacDiffDetailsCell() : base("VacDiffDetailsCell", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

