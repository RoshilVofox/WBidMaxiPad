using System;

using UIKit;
using Foundation;
using WBid.WBidiPad.iOS;

namespace TestTablewViewLeak
{
	public partial class ViewController : UIViewController
	{
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
		partial void GoToTablewView (NSObject sender)
		{
			LeakTablewViewControllerController ObjTable= new LeakTablewViewControllerController();
			this.NavigationController.PushViewController(ObjTable,true);
		}
		partial void PresentView (NSObject sender)
		{

			homeViewController homeVC = new homeViewController ();
			this.NavigationController.PushViewController(homeVC,true);
			this.NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
			this.NavigationController.NavigationBar.Hidden = true;
			return;
			userRegistrationViewController regs = new userRegistrationViewController();
			regs.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
			this.PresentViewController(regs, true, null);
		}
		partial void funGotoUserAccountPage (NSObject sender)
		{
			userRegistrationViewController ObjTable= new userRegistrationViewController();
			this.NavigationController.PushViewController(ObjTable,true);
		}
		   
	}
}

