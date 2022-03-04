using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace WBid.WBidiPad.iOS
{
	public class recieptCollectionCell : UICollectionViewCell
	{
		public static readonly NSString Key = new NSString ("recieptCollectionCell");
		UILabel titlelabel;
		[Export ("initWithFrame:")]
		public recieptCollectionCell (CGRect frame) : base (frame)
		{
			this.BackgroundView = new UIImageView (UIImage.FromBundle ("menuGreenNormal.png"));
			// TODO: add subviews to the ContentView, set various colors, etc.
			titlelabel = new UILabel ();
			titlelabel.Frame = new CGRect (5, 0, 100, 25);
			titlelabel.TextAlignment = UITextAlignment.Center;
			titlelabel.Font = UIFont.SystemFontOfSize (14);
			this.Add (titlelabel);
		}

		public NSString title {
			set {
				titlelabel.Text = value;
			}
		}
	}
}

