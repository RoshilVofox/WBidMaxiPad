using System;
using System.Drawing;
using Foundation;
using UIKit;
using WBid.WBidiPad.Model.SWA;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.SharedLibrary.SWA;
using WBidDataDownloadAuthorizationService.Model;
using System.Text.RegularExpressions;
using System.ServiceModel;
using System.Collections.Generic;
using WBid.WBidiPad.Model;
using System.Linq;
using System.IO;
using WBid.WBidiPad.PortableLibrary;
using System.Collections.ObjectModel;

namespace WBid.WBidiPad.iOS
{
	public partial class AvoidanceBidVC : UIViewController
	{
		public AvoidanceBidVC () : base ("AvoidanceBidVC", null)
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

			txtAvoid1.Text = GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance1;
			txtAvoid2.Text = GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance2;
			txtAvoid3.Text = GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance3;

			txtAvoid1.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				string text = textField.Text;
				string newText = text.Substring((int)0, (int)range.Location) + replacementString + text.Substring( (int)(range.Location + range.Length));
				int val;
				validateEmptyField (newText,txtAvoid2.Text,txtAvoid3.Text);
				if (newText == "" && newText.Length<10)
					return true;
				else
					return Int32.TryParse(newText, out val);
			};
			txtAvoid2.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				string text = textField.Text;
				string newText = text.Substring(0,  (int)range.Location) + replacementString + text.Substring( (int)( range.Location + range.Length));
				int val;
				validateEmptyField (txtAvoid1.Text,newText,txtAvoid3.Text);
				if (newText == "" && newText.Length<10)
					return true;
				else
					return Int32.TryParse(newText, out val);
			};
			txtAvoid3.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				string text = textField.Text;
				string newText = text.Substring(0, (int)range.Location) + replacementString + text.Substring((int)(range.Location + range.Length));
				int val;
				validateEmptyField (txtAvoid1.Text,txtAvoid2.Text,newText);
				if (newText == "" && newText.Length<10)
					return true;
				else
					return Int32.TryParse(newText, out val);
			};

		}
		private void validateEmptyField (string str1,string str2,string str3)
		{
			if (str1 == string.Empty || str2 == string.Empty || str3 == string.Empty)
				btnOk.Enabled = false;
			else
				btnOk.Enabled = true;
		}
		partial void brnOkTapped (Foundation.NSObject sender)
		{
			if(txtAvoid1.Text.Length != 0)
			{
				if(!RegXHandler.EmployeeNumberValidation(txtAvoid1.Text))
				{

                    UIAlertController okAlertController = UIAlertController.Create("Error", "Invalid Employee Number in first field.", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                    return;
				}
			}
			if(txtAvoid2.Text.Length != 0)
			{
				if(!RegXHandler.EmployeeNumberValidation(txtAvoid1.Text))
				{

                    UIAlertController okAlertController = UIAlertController.Create("Error", "Invalid Employee Number in second field.", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                    return;
				}
			}
			if(txtAvoid3.Text.Length != 0)
			{
				if(!RegXHandler.EmployeeNumberValidation(txtAvoid2.Text))
				{
                    UIAlertController okAlertController = UIAlertController.Create("Error", "Invalid Employee Number in third field.", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                    return;
				}
			}


            GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance1 = txtAvoid1.Text.Trim();
            GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance2 = txtAvoid2.Text.Trim();
            GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance3 = txtAvoid3.Text.Trim();
            //save the state of the INI File
            WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
          
            NSNotificationCenter.DefaultCenter.PostNotificationName("changeAvoidanceBids", null);
			this.DismissViewController(true,null);
			//Save before doing this
		}
		partial void btnCancelTapped (Foundation.NSObject sender)
		{
			this.DismissViewController(true,null);
		}
	}
}

