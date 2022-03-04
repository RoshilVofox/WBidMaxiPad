
using System;

using Foundation;
using UIKit;
using CoreGraphics;

namespace TestTablewViewLeak
{
	public class sampleCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("sampleCell");

		public sampleCell () : base (UITableViewCellStyle.Value1, Key)
		{
			// TODO: add subviews to the ContentView, set various colors, etc.
			//TextLabel.Text = "TextLabel";
		}


		public override void Draw (CGRect rect)
		{
			
			base.Draw (rect);
			Console.WriteLine ("Cell no:" + this.Tag);
			//// General Declarations
			var context = UIGraphics.GetCurrentContext();

			//// Color Declarations
			var color = UIColor.FromRGBA(1.000f, 0.993f, 0.993f, 1.000f);
			var color3 = UIColor.FromRGBA(0.996f, 0.965f, 0.690f, 1.000f);
			var color4 = UIColor.FromRGBA(0.855f, 0.420f, 0.573f, 1.000f);
			//// Image Declarations
			var image = UIImage.FromFile("image001.jpg");

			//// Text Drawing
			CGRect textRect = new CGRect(4.0f, 4.0f, 16.0f, 8.0f);
			{
				var textContent = this.Tag.ToString();
				UIColor.Black.SetFill();
				var textStyle = new NSMutableParagraphStyle ();
				textStyle.Alignment = UITextAlignment.Left;

				var textFontAttributes = new UIStringAttributes () {Font = UIFont.SystemFontOfSize(7.0f), ForegroundColor = UIColor.Black, ParagraphStyle = textStyle};
				var textTextHeight = new NSString(textContent).GetBoundingRect(new CGSize(textRect.Width, nfloat.MaxValue), NSStringDrawingOptions.UsesLineFragmentOrigin, textFontAttributes, null).Height;
				context.SaveState();
				context.ClipToRect(textRect);
				new NSString(textContent).DrawString(new CGRect(textRect.GetMinX(), textRect.GetMinY() + (textRect.Height - textTextHeight) / 2.0f, textRect.Width, textTextHeight), UIFont.SystemFontOfSize(7.0f), UILineBreakMode.WordWrap, UITextAlignment.Left);
				context.RestoreState();
			}


			//// Text 2 Drawing
			CGRect text2Rect = new CGRect(26.0f, 4.0f, 16.0f, 8.0f);
			{
				var textContent = "ABC";
				UIColor.Black.SetFill();
				var text2Style = new NSMutableParagraphStyle ();
				text2Style.Alignment = UITextAlignment.Left;

				var text2FontAttributes = new UIStringAttributes () {Font = UIFont.SystemFontOfSize(7.0f), ForegroundColor = UIColor.Black, ParagraphStyle = text2Style};
				var text2TextHeight = new NSString(textContent).GetBoundingRect(new CGSize(text2Rect.Width, nfloat.MaxValue), NSStringDrawingOptions.UsesLineFragmentOrigin, text2FontAttributes, null).Height;
				context.SaveState();
				context.ClipToRect(text2Rect);
				new NSString(textContent).DrawString(new CGRect(text2Rect.GetMinX(), text2Rect.GetMinY() + (text2Rect.Height - text2TextHeight) / 2.0f, text2Rect.Width, text2TextHeight), UIFont.SystemFontOfSize(7.0f), UILineBreakMode.WordWrap, UITextAlignment.Left);
				context.RestoreState();
			}


			//// Text 3 Drawing
			CGRect text3Rect = new CGRect(23.0f, 15.0f, 21.0f, 8.0f);
			{
				var textContent = "999";
				UIColor.Black.SetFill();
				var text3Style = new NSMutableParagraphStyle ();
				text3Style.Alignment = UITextAlignment.Center;

				var text3FontAttributes = new UIStringAttributes () {Font = UIFont.BoldSystemFontOfSize(8.0f), ForegroundColor = UIColor.Black, ParagraphStyle = text3Style};
				var text3TextHeight = new NSString(textContent).GetBoundingRect(new CGSize(text3Rect.Width, nfloat.MaxValue), NSStringDrawingOptions.UsesLineFragmentOrigin, text3FontAttributes, null).Height;
				context.SaveState();
				context.ClipToRect(text3Rect);
				new NSString(textContent).DrawString(new CGRect(text3Rect.GetMinX(), text3Rect.GetMinY() + (text3Rect.Height - text3TextHeight) / 2.0f, text3Rect.Width, text3TextHeight), UIFont.BoldSystemFontOfSize(8.0f), UILineBreakMode.WordWrap, UITextAlignment.Center);
				context.RestoreState();
			}


			//// Rectangle Drawing
			var rectanglePath = UIBezierPath.FromRect(new CGRect(4.0f, 59.0f, 8.0f, 9.0f));
			context.SaveState();
			rectanglePath.AddClip();
			context.ScaleCTM(1.0f, -1.0f);
			context.DrawTiledImage(new CGRect(4.0f, -59.0f, image.Size.Width, image.Size.Height), image.CGImage);
			context.RestoreState();


			//// Rectangle 2 Drawing
			var rectangle2Path = UIBezierPath.FromRect(new CGRect(16.0f, 59.0f, 8.0f, 9.0f));
			context.SaveState();
			rectangle2Path.AddClip();
			context.ScaleCTM(1.0f, -1.0f);
			context.DrawTiledImage(new CGRect(16.0f, -59.0f, image.Size.Width, image.Size.Height), image.CGImage);
			context.RestoreState();


			//// Rectangle 3 Drawing
			var rectangle3Path = UIBezierPath.FromRect(new CGRect(29.0f, 59.0f, 8.0f, 9.0f));
			context.SaveState();
			rectangle3Path.AddClip();
			context.ScaleCTM(1.0f, -1.0f);
			context.DrawTiledImage(new CGRect(29.0f, -59.0f, image.Size.Width, image.Size.Height), image.CGImage);
			context.RestoreState();


			//// Text 4 Drawing
			CGRect text4Rect = new CGRect(46.0f, 3.0f, 30.0f, 76.0f);
			UIColor.Black.SetFill();
			new NSString("Pay CR  \n\nPay CR \n\nPay CR\n\nPay CR\n\n Pay CR ").DrawString(text4Rect, UIFont.SystemFontOfSize(7.0f), UILineBreakMode.WordWrap, UITextAlignment.Left);


			//// Text 5 Drawing
			CGRect text5Rect = new CGRect(74.0f, 3.0f, 30.0f, 76.0f);
			UIColor.Black.SetFill();
			new NSString("1000\n\n10\n\n20\n\n2000\n\n100").DrawString(text5Rect, UIFont.SystemFontOfSize(7.0f), UILineBreakMode.WordWrap, UITextAlignment.Right);
			int Padding = 0;


		for (int dayCell = 0; dayCell < 34; dayCell++) {

				//// Rectangle 4 Drawing
				var rectangle4Path = UIBezierPath.FromRect(new CGRect(108.0f+Padding, -1.0f, 25.0f, 80.0f));
				color.SetFill();
				rectangle4Path.Fill();
				UIColor.Black.SetStroke();
				rectangle4Path.LineWidth = 1.0f;
				rectangle4Path.Stroke();


				//// Text 6 Drawing
				CGRect text6Rect = new CGRect(108.0f+Padding, 0.0f, 25.0f, 20.0f);
				var text6Path = UIBezierPath.FromRect(text6Rect);
				color3.SetFill();
				text6Path.Fill();
				{
					var textContent = "1";
					UIColor.Black.SetFill();
					var text6Style = new NSMutableParagraphStyle ();
					text6Style.Alignment = UITextAlignment.Center;

					var text6FontAttributes = new UIStringAttributes () {Font = UIFont.SystemFontOfSize(15.0f), ForegroundColor = UIColor.Black, ParagraphStyle = text6Style};
					var text6TextHeight = new NSString(textContent).GetBoundingRect(new CGSize(text6Rect.Width, nfloat.MaxValue), NSStringDrawingOptions.UsesLineFragmentOrigin, text6FontAttributes, null).Height;
					context.SaveState();
					context.ClipToRect(text6Rect);
					new NSString(textContent).DrawString(new CGRect(text6Rect.GetMinX(), text6Rect.GetMinY() + (text6Rect.Height - text6TextHeight) / 2.0f, text6Rect.Width, text6TextHeight), UIFont.SystemFontOfSize(15.0f), UILineBreakMode.WordWrap, UITextAlignment.Center);
					context.RestoreState();
				}


				//// Text 7 Drawing
				CGRect text7Rect = new CGRect(108.0f+Padding, 20.0f, 25.0f, 20.0f);
				var text7Path = UIBezierPath.FromRect(text7Rect);
				color3.SetFill();
				text7Path.Fill();
				{
					var textContent = "SU";
					UIColor.Black.SetFill();
					var text7Style = new NSMutableParagraphStyle ();
					text7Style.Alignment = UITextAlignment.Center;

					var text7FontAttributes = new UIStringAttributes () {Font = UIFont.SystemFontOfSize(13.0f), ForegroundColor = UIColor.Black, ParagraphStyle = text7Style};
					var text7TextHeight = new NSString(textContent).GetBoundingRect(new CGSize(text7Rect.Width, nfloat.MaxValue), NSStringDrawingOptions.UsesLineFragmentOrigin, text7FontAttributes, null).Height;
					context.SaveState();
					context.ClipToRect(text7Rect);
					new NSString(textContent).DrawString(new CGRect(text7Rect.GetMinX(), text7Rect.GetMinY() + (text7Rect.Height - text7TextHeight) / 2.0f, text7Rect.Width, text7TextHeight), UIFont.SystemFontOfSize(UIFont.SmallSystemFontSize), UILineBreakMode.WordWrap, UITextAlignment.Center);
					context.RestoreState();
				}


				//// Text 8 Drawing
				CGRect text8Rect = new CGRect(108.0f+Padding, 40.0f, 25.0f, 20.0f);
				var text8Path = UIBezierPath.FromRect(text8Rect);
				color4.SetFill();
				text8Path.Fill();
				{
					var textContent = "ABC";
					UIColor.Black.SetFill();
					var text8Style = new NSMutableParagraphStyle ();
					text8Style.Alignment = UITextAlignment.Center;

					var text8FontAttributes = new UIStringAttributes () {Font = UIFont.BoldSystemFontOfSize(10.0f), ForegroundColor = UIColor.Black, ParagraphStyle = text8Style};
					var text8TextHeight = new NSString(textContent).GetBoundingRect(new CGSize(text8Rect.Width, nfloat.MaxValue), NSStringDrawingOptions.UsesLineFragmentOrigin, text8FontAttributes, null).Height;
					context.SaveState();
					context.ClipToRect(text8Rect);
					new NSString(textContent).DrawString(new CGRect(text8Rect.GetMinX(), text8Rect.GetMinY() + (text8Rect.Height - text8TextHeight) / 2.0f, text8Rect.Width, text8TextHeight), UIFont.BoldSystemFontOfSize(10.0f), UILineBreakMode.WordWrap, UITextAlignment.Center);
					context.RestoreState();
				}


				//// Text 9 Drawing
				CGRect text9Rect = new CGRect(108.0f+Padding, 60.0f, 25.0f, 20.0f);
				var text9Path = UIBezierPath.FromRect(text9Rect);
				color4.SetFill();
				text9Path.Fill();
				{
					var textContent = "ABC";
					UIColor.Black.SetFill();
					var text9Style = new NSMutableParagraphStyle ();
					text9Style.Alignment = UITextAlignment.Center;

					var text9FontAttributes = new UIStringAttributes () {Font = UIFont.BoldSystemFontOfSize(10.0f), ForegroundColor = UIColor.Black, ParagraphStyle = text9Style};
					var text9TextHeight = new NSString(textContent).GetBoundingRect(new CGSize(text9Rect.Width, nfloat.MaxValue), NSStringDrawingOptions.UsesLineFragmentOrigin, text9FontAttributes, null).Height;
					context.SaveState();
					context.ClipToRect(text9Rect);
					new NSString(textContent).DrawString(new CGRect(text9Rect.GetMinX(), text9Rect.GetMinY() + (text9Rect.Height - text9TextHeight) / 2.0f, text9Rect.Width, text9TextHeight), UIFont.BoldSystemFontOfSize(10.0f), UILineBreakMode.WordWrap, UITextAlignment.Center);
					context.RestoreState();
				}

				Padding += 25;
			}
		}
	}
}

