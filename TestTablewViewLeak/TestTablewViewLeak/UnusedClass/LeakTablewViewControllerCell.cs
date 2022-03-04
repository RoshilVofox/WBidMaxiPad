
using System;

using Foundation;
using UIKit;

namespace TestTablewViewLeak
{
	[Register ("LeakTablewViewControllerCell")]
	public partial class LeakTablewViewControllerCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("LeakTablewViewControllerCell");
	
		public LeakTablewViewControllerCell () : base (UITableViewCellStyle.Value1, Key)
		{
			// TODO: add subviews to the ContentView, set various colors, etc.
			TextLabel.Text = "TextLabel";
			UILabel Testview = new UILabel ();
			UIGestureRecognizer singleTap;
			//singleTap.NumberOfTouches = 1;
			Testview.Frame = new CoreGraphics.CGRect (0, 0, 300, 50);
			for (int i = 0; i < 30; i++) {
				
				this.AddSubview (Testview);
				singleTap = new UITapGestureRecognizer (() => {
					
				});
				//Testview.AddGestureRecognizer (singleTap);

			}
			//this.AddSubview (BaseView);
		}
		partial void btnLineSelectTapped (UIKit.UIButton sender)
		{
			
		}
	}
}

