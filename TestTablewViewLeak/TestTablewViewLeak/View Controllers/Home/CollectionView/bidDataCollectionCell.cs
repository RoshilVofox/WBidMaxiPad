using System;
using CoreGraphics;
using Foundation;
using UIKit;
using CoreGraphics;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS.Utility;
using System.Collections.Generic;

namespace WBid.WBidiPad.iOS
{
	public class bidDataCollectionCell : UICollectionViewCell
	{
		public static readonly NSString Key = new NSString ("bidDataCollectionCell");
		UILabel titlelabel;
		UILabel subTitleLabel;
		UIButton deleteButton;

		[Export ("initWithFrame:")]
		public bidDataCollectionCell (CGRect frame) : base (frame)
		{
			// TODO: add subviews to the ContentView, set various colors, etc.
			titlelabel = new UILabel ();
			titlelabel.Frame = new CGRect (60, 0, 100, 25);
			titlelabel.TextAlignment = UITextAlignment.Center;
			titlelabel.Font = UIFont.SystemFontOfSize (16);
			this.Add (titlelabel);

			subTitleLabel = new UILabel ();
			subTitleLabel.Frame = new CGRect (60, 25, 200, 25);
			subTitleLabel.TextAlignment = UITextAlignment.Left;
			subTitleLabel.Font = UIFont.SystemFontOfSize (14);
			this.Add (subTitleLabel);

			deleteButton = new UIButton ();
			deleteButton.Frame = new CGRect (173, 2, 25, 25);
			deleteButton.BackgroundColor = UIColor.Clear;
			deleteButton.SetBackgroundImage (UIImage.FromBundle ("closeIcon.png"), UIControlState.Normal);
			deleteButton.TouchUpInside += (sender, ea) => {
//				NSString str = new NSString(deleteButton.Tag.ToString());
				NSNotificationCenter.DefaultCenter.PostNotificationName("homeCollectionCellDelateButtonTapped",this);
			};
			deleteButton.Hidden = true;
			this.Add (deleteButton);

			BackgroundColor = UIColor.Orange;
		}

		public NSString title {
			set {
				titlelabel.Text = value;
			}
		}
		public NSString subTitle {
			set{
				subTitleLabel.Text = value;
			}
		}

		public int tag{
			set{
				this.Tag = value;
				deleteButton.Tag = value;
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

//		public const float kAnimationRotateDeg = 0.5;
//		public const float kAnimationTranslateX = 1.0;
//		public const float kAnimationTranslateY = 1.0;
//
		public void startJiggling () 
		{
			Console.WriteLine ("Start Jiggling");


//			#define degreesToRadians(x) (M_PI * (x) / 180.0)
//			#define kAnimationRotateDeg 0.5
//			#define kAnimationTranslateX 1.0
//			#define kAnimationTranslateY 1.0
//
//			//startJiggling
//
//			int count = 1;
//			CGAffineTransform leftWobble = CGAffineTransformMakeRotation(degreesToRadians( kAnimationRotateDeg * (count%2 ? +1 : -1 ) ));
//			CGAffineTransform rightWobble = CGAffineTransformMakeRotation(degreesToRadians( kAnimationRotateDeg * (count%2 ? -1 : +1 ) ));
//			CGAffineTransform moveTransform = CGAffineTransformTranslate(rightWobble, -kAnimationTranslateX, -kAnimationTranslateY);
//			CGAffineTransform conCatTransform = CGAffineTransformConcat(rightWobble, moveTransform);
//
//			self.transform = leftWobble;  // starting point
//
//			[UIView animateWithDuration:0.1
//				delay:(count * 0.08)
//				options:UIViewAnimationOptionAllowUserInteraction | UIViewAnimationOptionRepeat | UIViewAnimationOptionAutoreverse
//				animations:^{ self.transform = conCatTransform; }
//				completion:nil];

			CGAffineTransform leftWobble = CGAffineTransform.MakeRotation (degreesToRadians(2));
			CGAffineTransform rightWobble = CGAffineTransform.MakeRotation (degreesToRadians(-2));
			this.Transform = leftWobble;

			UIView.AnimateKeyframes (0.25, 0,UIViewKeyframeAnimationOptions.AllowUserInteraction| UIViewKeyframeAnimationOptions.Repeat|UIViewKeyframeAnimationOptions.Autoreverse, () => {
				this.Transform = rightWobble;
			},(bool leftStatus)=>{

			});

			deleteButton.Hidden = false;
		}

		public void endJiggling () 
		{
			deleteButton.Hidden = true;
		}

		private float  degreesToRadians( float x) {
			return  (3.14f * x / 180f);
		}
	}
}

