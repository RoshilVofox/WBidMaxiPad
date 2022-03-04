using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace WBid.WBidiPad.iOS
{
	public class pilotBidEditorCollectionCell : UICollectionViewCell
	{
		public static readonly NSString Key = new NSString ("pilotBidEditorCollectionCell");
		UILabel titlelabel;


		[Export ("initWithFrame:")]
		public pilotBidEditorCollectionCell (CGRect frame) : base (frame)
		{
			// TODO: add subviews to the ContentView, set various colors, etc.
			Layer.BorderWidth = 1.0f;
			BackgroundColor = UIColor.White;

			titlelabel = new UILabel ();
			titlelabel.Frame = new CGRect (0, 15, 50, 25);
			titlelabel.TextAlignment = UITextAlignment.Center;
			this.Add (titlelabel);

		}

		public NSString title {
			set {
				titlelabel.Text = value;
			}
		}
	}
}

