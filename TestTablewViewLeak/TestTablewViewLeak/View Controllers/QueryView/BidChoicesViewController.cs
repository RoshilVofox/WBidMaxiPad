using System;

using UIKit;
using WBid.WBidiPad.Core;
using Foundation;

namespace WBid.WBidiPad.iOS
{
	public partial class BidChoicesViewController : UIViewController
	{
		public BidChoicesViewController() : base("BidChoicesViewController", null)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			string submitbidchoice = GlobalSettings.SubmitBid.Bid;
			submitbidchoice = submitbidchoice.Replace(",", ", ");
			txtView.Text = submitbidchoice;
			var textStyle = new NSMutableParagraphStyle();
			textStyle.LineSpacing = 15;
			var textFontAttributes = new UIStringAttributes() { Font = UIFont.SystemFontOfSize(14.0f), ForegroundColor = UIColor.Black, ParagraphStyle = textStyle };
			txtView.AttributedText = new NSAttributedString(submitbidchoice, textFontAttributes);

			this.NavigationItem.Title = "Bid Choices";
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

