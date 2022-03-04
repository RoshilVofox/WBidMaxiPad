using System;
using UIKit;
using WBid.WBidiPad.Model;
using Foundation;
using CoreGraphics;
using WBid.WBidiPad.Core;
using System.Linq;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using Microsoft.CSharp;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.PortableLibrary.Utility;

namespace WBid.WBidiPad.iOS
{
	public partial class RatioViewController : UIViewController
	{
		UIPopoverController popoverController;
		NSObject NumeratorNotification,DenominatorNotification;
		string[]  ObjNumerator;
		string[]  ObjDenominator;
		public Boolean isFromLineViewController;
		public RatioViewController(IntPtr handle) : base(handle)
		{
		}
		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);


		}

		partial void OkButtonClicked(NSObject sender)
		{
			if (btnNumerator.TitleLabel.Text != "Select" && btnDenominator.TitleLabel.Text != "Select")
			{
				CalculateRatioValuesForAllLines();
				if (GlobalSettings.WBidINIContent.RatioValues == null)
					GlobalSettings.WBidINIContent.RatioValues = new Ratio();
				GlobalSettings.WBidINIContent.RatioValues.Denominator = int.Parse( ObjDenominator[2].ToString());
				GlobalSettings.WBidINIContent.RatioValues.Numerator = int.Parse(ObjNumerator[2].ToString());

				if(isFromLineViewController)
				NSNotificationCenter.DefaultCenter.PostNotificationName("ClosingRatioScreen", new Foundation.NSString("OK"));
				else 
					NSNotificationCenter.DefaultCenter.PostNotificationName("ClosingRatioScreenSort", new Foundation.NSString("OK"));
				this.DismissModalViewController(true);
				
			}
			else
			{
            
                UIAlertController okAlertController = UIAlertController.Create("WBidMax", "Either select numerator or denominator values - or click Cancel.", UIAlertControllerStyle.Alert);                 okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));                 this.PresentViewController(okAlertController, true, null);

            }
		}

		private void CalculateRatioValuesForAllLines()
		{
			
			foreach (var line in GlobalSettings.Lines)
			{
				var numerator = line.GetType().GetProperty(ObjNumerator[1]).GetValue(line, null);
				if (ObjNumerator[1] == "TafbInBp")
					numerator = Helper.ConvertHhhColonMmToFractionalHours(line.TafbInBp);
				decimal numeratorValue = Convert.ToDecimal(numerator);

				var denominator = line.GetType().GetProperty(ObjDenominator[1]).GetValue(line, null);
				if (ObjDenominator[1] == "TafbInBp")
					denominator = Helper.ConvertHhhColonMmToFractionalHours(line.TafbInBp);
				decimal denominatorValue = Convert.ToDecimal(denominator);

				//decimal denominatorValue = Convert.ToDecimal(line.GetType().GetProperty(ObjDenominator[1]).GetValue(line, null));

				line.Ratio = Math.Round(decimal.Parse(String.Format("{0:0.00}", (denominatorValue == 0) ? 0 : numeratorValue / denominatorValue)), 2, MidpointRounding.AwayFromZero);

			}

		}
		partial void DenominatorClicked(NSObject sender)
		{
			UIButton ObjButton = (UIButton)sender;
			PopoverViewController popoverContent = new PopoverViewController();
			popoverContent.PopType = "NumeratorData";
			popoverContent.SubPopType = "Denominator";
			popoverContent.index = (int)ObjButton.Tag;
			popoverController = new UIPopoverController(popoverContent);
			popoverController.PopoverContentSize = new CGSize(135, 300);
			popoverController.PresentFromRect(ObjButton.Frame, this.View, UIPopoverArrowDirection.Any, true);
			DenominatorNotification = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("DenominatorSelection"), DenominatorSelection);

		}
		partial void NumeratorClicked(NSObject sender)
		{
			UIButton ObjButton = (UIButton)sender;
			PopoverViewController popoverContent = new PopoverViewController();
			popoverContent.PopType = "NumeratorData";
			popoverContent.SubPopType = "Numerator";
			popoverContent.index = (int)ObjButton.Tag;
			popoverController = new UIPopoverController(popoverContent);
			popoverController.PopoverContentSize = new CGSize(135, 300);
			popoverController.PresentFromRect(ObjButton.Frame, this.View, UIPopoverArrowDirection.Any, true);
			NumeratorNotification = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("NumeratorSelection"), NumeratorSelection);

		}

		public void NumeratorSelection(NSNotification n)
		{


			string numeratorValue = n.Object.ToString() ;

			                                       
			ObjNumerator = numeratorValue.Split(',');
			btnNumerator.SetTitle(ObjNumerator[0].ToString(), UIControlState.Normal);

			NSNotificationCenter.DefaultCenter.RemoveObserver(NumeratorNotification);
			popoverController.Dismiss(true);


		}

		public void DenominatorSelection(NSNotification n)
		{


			string numeratorValue = n.Object.ToString();
			ObjDenominator = numeratorValue.Split(',');
			btnDenominator.SetTitle(ObjDenominator[0].ToString(), UIControlState.Normal);

			NSNotificationCenter.DefaultCenter.RemoveObserver(DenominatorNotification);
			popoverController.Dismiss(true);


		}

		partial void CancelButtonClicked(NSObject sender)
		{
			if (isFromLineViewController)
				NSNotificationCenter.DefaultCenter.PostNotificationName("ClosingRatioScreen", new Foundation.NSString("CANCEL"));
			else
				NSNotificationCenter.DefaultCenter.PostNotificationName("ClosingRatioScreenSort", new Foundation.NSString("CANCEL"));
			this.DismissModalViewController(true);
		}
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

		}



	}
}


