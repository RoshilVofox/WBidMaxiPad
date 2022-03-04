using System;
using CoreGraphics;
using Foundation;
using UIKit;
using MessageUI;
using WBid.WBidiPad.iOS.Utility;

namespace WBid.WBidiPad.iOS
{
	public partial class AboutViewController : UIViewController
	{
		public AboutViewController () : base ("AboutViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
			lblVersion.Text = System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
			lblContact.AttributedText = new NSAttributedString (
				"Contact Us", 
				underlineStyle: NSUnderlineStyle.Single
			);
			lblContact.UserInteractionEnabled = true;
			lblContact.AddGestureRecognizer (new UITapGestureRecognizer (HandleContact));
		}

		void HandleContact (UITapGestureRecognizer obj)
		{
			if (MFMailComposeViewController.CanSendMail) {
				var mail = new MFMailComposeViewController ();
				mail.SetToRecipients (new string[]{ "support@wbidmax.com" });
				mail.SetSubject ("WBidMax Enquiry");
				//mail.SetMessageBody ("", false);
				mail.Finished += HandleFinished;
				this.PresentViewController (mail, true, null);
			} else {
               
                 UIAlertController okAlertController = UIAlertController.Create("WBidMax", "Mail is not configured in this device", UIAlertControllerStyle.Alert);                 okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));                 this.PresentViewController(okAlertController, true, null);

            }
        }

		void HandleFinished (object sender, MFComposeResultEventArgs e)
		{
			Console.WriteLine (e.Result);
			e.Controller.DismissViewController (true, null);
		}

		partial void btnDoneTapped (UIKit.UIBarButtonItem sender)
		{
			this.DismissViewController(true,null);
			foreach (UIView view in this.View.Subviews) {

				DisposeClass.DisposeEx(view);
			}
		}

	}
}

