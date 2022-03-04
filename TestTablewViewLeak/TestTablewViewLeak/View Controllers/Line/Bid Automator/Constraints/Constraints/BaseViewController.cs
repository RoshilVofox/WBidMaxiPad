using System;
using UIKit;

namespace WBid.WBidiPad.iOS
{
	public class BaseViewController: UIViewController
	{
		public BaseViewController (IntPtr handle) : base (handle)
		{		
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.NavigationController.SetNavigationBarHidden (true, false);
		}

		public void ShowNavigationBar()
		{
			this.NavigationController.SetNavigationBarHidden (false, false);
			NavigationController.NavigationBar.BarTintColor = UIColor.White;
			NavigationController.NavigationBar.Translucent = false;
			NavigationItem.BackBarButtonItem = new UIBarButtonItem ("Back", UIBarButtonItemStyle.Plain,null);
		}
		public void ShowPopUpInfo(string title, string message){
		    
            UIAlertController okAlertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            okAlertController.AddAction(UIAlertAction.Create(Constants.OK, UIAlertActionStyle.Default, null));
            this.PresentViewController(okAlertController, true, null);
        }
		public void PushViewController(UIViewController viewController, bool animated){
			NavigationController.PushViewController (viewController, animated);
		}
		public void PopViewController(UIViewController bidViewController, bool anim){
			if (bidViewController != null) {
				NavigationController.PopToViewController (bidViewController, anim);
			} else {
				NavigationController.PopViewController (anim);
			}
		}
		public void NSLogBidValet(string msg){
			Console.WriteLine(msg);
		}
	}
}

