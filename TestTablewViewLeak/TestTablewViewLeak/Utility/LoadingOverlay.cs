using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreGraphics;

namespace WBid.WBidiPad.iOS.Utility
{
    /// <summary>
    /// A simple modal overlay that shows an activity spinner and loading message. To use, 
    /// instantiate with a RectangleF frame, and add to super view. When finished, call Hide().
    /// </summary>
    public class LoadingOverlay : UIView
    {
        // control declarations
        UIActivityIndicatorView activitySpinner;
        UILabel loadingLabel;
		UIImageView backImage;

        public LoadingOverlay(CGRect frame, string byzyText)
            : base(frame)
        {
            // configurable bits
			BackgroundColor = UIColor.Clear;
			//Alpha = 0.4f;
            AutoresizingMask = UIViewAutoresizing.None;

			backImage = new UIImageView (Bounds);
			backImage.BackgroundColor = UIColor.Black;
			backImage.Alpha = 0.6f;
			AddSubview (backImage);

            float labelHeight = 60;
			float labelWidth = (float)Frame.Width;

            // derive the center x and y
			float centerX = (float)Frame.Width / 2;
			float centerY = (float)Frame.Height / 2;

            // create the activity spinner, center it horizontall and put it 5 points above center x
			activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.White);
			activitySpinner.Color = UIColor.White;
            activitySpinner.Frame = new CGRect(
				centerX + (activitySpinner.Frame.Width/2) - 20,
				centerY - (activitySpinner.Frame.Height/2),
                activitySpinner.Frame.Width,
                activitySpinner.Frame.Height);
            activitySpinner.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
            AddSubview(activitySpinner);
            activitySpinner.StartAnimating();

            // create and configure the "Loading Data" label
            loadingLabel = new UILabel(new CGRect(
				0 ,
				centerY + 25,
                labelWidth,
                labelHeight
                ));
            loadingLabel.BackgroundColor = UIColor.Clear;
			loadingLabel.TextColor = UIColor.White;
            //loadingLabel.Text = "Loading Data...";
            loadingLabel.Text = byzyText;
            loadingLabel.TextAlignment = UITextAlignment.Center;
            loadingLabel.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
			loadingLabel.Lines = 0;
            AddSubview(loadingLabel);
        }

        /// <summary>
        /// Fades out the control and then removes it from the super view
        /// </summary>
        public void Hide()
        {
            UIView.Animate(
                0.5, // duration
                () => { Alpha = 0; }
                //() => { RemoveFromSuperview(); }
            );
        }

        public void Show()
        {
            UIView.Animate(
                0.5, // duration
                () => { Alpha = 1; }
                //() => { RemoveFromSuperview(); }
            );
        }


        public void updateLoadingText(string str)
		{
			loadingLabel.Text = str;
		}
    }
}
