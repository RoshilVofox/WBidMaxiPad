using System;
using UIKit;
using CoreGraphics;

namespace WBid.WBidiPad.iOS
{
	public static class UIHelpers
	{
		public static void StyleForButtons(UIButton[] btn){
			foreach (UIButton button in btn) {
				button.Layer.CornerRadius = 7;
				button.SetTitleColor (UIColor.White, UIControlState.Normal);
			}
		}
		public static void StyleForButtonsWithBorder(UIButton[] btn){
			foreach (UIButton button in btn) {
				button.Layer.CornerRadius = 7;
				button.Layer.BorderColor = UIColor.Black.CGColor;
				button.Layer.BorderWidth = 1.0f;
				button.SetTitleColor (UIColor.Black, UIControlState.Normal);
			}
		}
		public static void StyleForButtonsSortView(UIButton[] btn){
			foreach (UIButton button in btn) {
				button.Layer.CornerRadius = 7;
				button.Layer.BorderColor = UIColor.FromRGB (81, 75, 56).CGColor; 
				button.Layer.BorderWidth = 1.3f;
				button.SetTitleColor (UIColor.Black, UIControlState.Normal);
			}
		}
		public static void StyleForButtonsBorderBlackRectange(UIButton[] btn){
			foreach (UIButton button in btn) {
				button.Layer.BorderColor = UIColor.Black.CGColor; 
				button.Layer.BorderWidth = 1.3f;
				button.SetTitleColor (UIColor.Black, UIControlState.Normal);
			}
		}
		public static void StyleForButtonsBorderBlackRectangeThin(UIButton[] btn){
			foreach (UIButton button in btn) {
				button.Layer.BorderColor = UIColor.Black.CGColor; 
				button.Layer.BorderWidth = 0.5f;
				button.SetTitleColor (UIColor.Black, UIControlState.Normal);
			}
		}
		public static void StyleForButtonsRadiusBorderBlack(UIButton[] btn){
			foreach (UIButton button in btn) {
				button.Layer.CornerRadius = 7;
				button.Layer.BorderColor = UIColor.Black.CGColor; 
				button.Layer.BorderWidth = 1.3f;
				button.SetTitleColor (UIColor.Black, UIControlState.Normal);
			}
		}
		public static void StyleForButtonsDeleteSortView(UIButton[] btn){
			foreach (UIButton button in btn) {
				button.Layer.CornerRadius = 7;
				button.Layer.BorderColor = UIColor.FromRGB (115, 69, 87).CGColor; 
				button.Layer.BorderWidth = 1.3f;
				button.SetTitleColor (Colors.BidRedDelete, UIControlState.Normal);
			}
		}
		public static void StyleForButtonPerMonth(UIButton view){
			view.Layer.CornerRadius = 15;
			view.Layer.BorderColor = UIColor.FromRGB (81, 75, 56).CGColor; 
			view.Layer.BorderWidth = 1.3f;
			view.Layer.MasksToBounds = true;
		}

		public static void StyleForButtonsCreateAccount(UIButton[] btn){
			foreach (UIButton button in btn) {
				button.Layer.CornerRadius = 7;
				button.Layer.BorderColor = UIColor.FromRGB (5, 25, 2).CGColor;
				button.Layer.BorderWidth = 1.3f;
				button.SetTitleColor (UIColor.Black, UIControlState.Normal);
			}
		}
		public static void StyleForButtonCancelCreateAccount(UIButton button){
			button.Layer.CornerRadius = 7;
			button.Layer.BorderColor = UIColor.FromRGB (186,27,22).CGColor;
			button.Layer.BorderWidth = 1.3f;
			button.SetTitleColor (UIColor.White, UIControlState.Normal);
		}


		public static void MoveViewUpDown(UIView view, float delta, Action cb){
			UIView.Animate (0.3, () => {
				CGRect frame = UIScreen.MainScreen.Bounds;
				view.Frame = new CGRect(frame.X, frame.Y, frame.Width, frame.Height + delta);
			}, cb);
		}

		public static bool validatorInputField(UIViewController view, UITextField[] textFields,string[] errorMessages){
			bool status = true;
			int index = 0;
			foreach (UITextField textF in textFields) {
				
				if (String.IsNullOrEmpty (textF.Text)) {
					UIAlertController okAlertController = UIAlertController.Create ("BidValet", errorMessages[index]+" is Empty", UIAlertControllerStyle.Alert);
					okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
					view.PresentViewController (okAlertController, true, null);
					textF.BecomeFirstResponder ();
					status= false;
					break;
				}
				index++;
			}
			return status;
		}
	}
}

