using System;
using CoreGraphics;
using Foundation;
using UIKit;
using System.IO;
using WBid.WBidiPad.iOS.Utility;
using System.Collections.Generic;
using System.Linq;
using WBid.WBidiPad.Model;
using System.Threading.Tasks;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.PortableLibrary;
using System.Collections.ObjectModel;
using CoreGraphics;
using WBid.WBidiPad.SharedLibrary.Utility;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;

namespace WBid.WBidiPad.iOS
{
	public partial class BidCollectionCell : UICollectionViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("BidCollectionCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("BidCollectionCell");

		public BidCollectionCell (IntPtr handle) : base (handle)
		{
		}

		public static BidCollectionCell Create ()
		{
			return (BidCollectionCell)Nib.Instantiate (null, null) [0];
		}
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			UILongPressGestureRecognizer lngPress = new UILongPressGestureRecognizer(handleLngPress);
			this.AddGestureRecognizer(lngPress);
			lngPress.DelaysTouchesBegan = true;
		}
		public void BindData (RecentFile aFile, NSIndexPath index, bool jiggle)
		{
			btnDelete.Tag = index.Row;
			if (jiggle)
				btnDelete.Hidden = false;
			else
				btnDelete.Hidden = true;
			lblTitle.Text = aFile.MonthDisplay + " " + aFile.Year;
			lblSubTitle.Text = aFile.Domcile + "-" + aFile.Position + "-" + aFile.Round;

		}

		private void handleLngPress(UILongPressGestureRecognizer lngPress)
		{
			if (lngPress.State == UIGestureRecognizerState.Began)
			{
				//Console.WriteLine("Long press on cell detected");
				NSNotificationCenter.DefaultCenter.PostNotificationName ("StartJiggle", null);
			}
		}

		public void setBackImage (string str,bool isFemale)
		{
			if (isFemale) {
				if (str == "CP")
					this.BackgroundView = new UIImageView (UIImage.FromBundle ("cpBgWomen.png"));
				else if (str == "FO")
					this.BackgroundView = new UIImageView (UIImage.FromBundle ("foBgWomen.png"));
				else if (str == "FA")
					this.BackgroundView = new UIImageView (UIImage.FromBundle ("faBgWomen.png"));
			} else {
				if (str == "CP")
					this.BackgroundView = new UIImageView (UIImage.FromBundle ("cpBgMen.png"));
				else if (str == "FO")
					this.BackgroundView = new UIImageView (UIImage.FromBundle ("foBgMen.png"));
				else if (str == "FA")
					this.BackgroundView = new UIImageView (UIImage.FromBundle ("faBgMen.png"));
			}
		}

		private float  degreesToRadians( float x) {
			return  (3.14f * x / 180f);
		}

		public void DoJiggle (bool jiggle)
		{
			if (jiggle) {
				CGAffineTransform leftWobble = CGAffineTransform.MakeRotation (degreesToRadians(2));
				CGAffineTransform rightWobble = CGAffineTransform.MakeRotation (degreesToRadians(-2));
				this.Transform = leftWobble;

				UIView.AnimateKeyframes (0.20, 0,UIViewKeyframeAnimationOptions.AllowUserInteraction| UIViewKeyframeAnimationOptions.Repeat|UIViewKeyframeAnimationOptions.Autoreverse, () => {
					this.Transform = rightWobble;
				},(bool leftStatus)=>{

				});
			} else {
				CGAffineTransform back = CGAffineTransform.MakeRotation (degreesToRadians (0));
				this.Transform = back;
			}
		}

		partial void btnDeleteTapped (UIKit.UIButton sender)
		{
			NSNumber num = new NSNumber(sender.Tag);
			NSNotificationCenter.DefaultCenter.PostNotificationName("HandleBidDelete",num);
		}
	}
}

