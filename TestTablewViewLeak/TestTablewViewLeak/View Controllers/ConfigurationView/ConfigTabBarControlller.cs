using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.iOS.Utility;
using CoreAnimation;

namespace WBid.WBidiPad.iOS
{
	public class ConfigTabBarControlller : UITabBarController
	{
		ConfigViewController tab1, tab2, tab3, tab4, tab5, tab6;

		public ConfigTabBarControlller (bool FromHome)
		{
			tab1 = new ConfigViewController();
			tab1.Title = "Misc";
			tab1.View.BackgroundColor = UIColor.White;
			tab1.TabBarItem.Image = UIImage.FromBundle ("miscMenuIcon.png");

			tab2 = new ConfigViewController();
			tab2.Title = "Pairing Export";
			tab2.View.BackgroundColor = UIColor.White;
			tab2.TabBarItem.Image = UIImage.FromBundle ("exportMenuIcon.png");

			tab3 = new ConfigViewController();
			tab3.Title = "AM/PM";
			tab3.View.BackgroundColor = UIColor.White;
			tab3.TabBarItem.Image = UIImage.FromBundle ("timeMenuIcon.png");

			tab4 = new ConfigViewController();
			tab4.Title = "Week";
			tab4.View.BackgroundColor = UIColor.White;
			tab4.TabBarItem.Image = UIImage.FromBundle ("weekMenuIcon.png");

			tab5 = new ConfigViewController();
			tab5.Title = "Hotels";
			tab5.View.BackgroundColor = UIColor.White;
			tab5.TabBarItem.Image = UIImage.FromBundle ("hotelMenuIcon.png");

			tab6 = new ConfigViewController();
			tab6.Title = "User";
			tab6.View.BackgroundColor = UIColor.White;
			tab6.TabBarItem.Image = UIImage.FromBundle ("Contact.png");

			if (!FromHome) {
				var tabs = new ConfigViewController[] {
					tab1, tab2, tab3, tab4, tab5, tab6
				};
				ViewControllers = tabs;
			} else {
				var tabs = new ConfigViewController[] {
					tab1, tab2, tab5, tab6
				};
				ViewControllers = tabs;
			}
			TabBar.Translucent = false;
			TabBar.BarTintColor = ColorClass.BottomHeaderColor;
			TabBar.TintColor = UIColor.Black;
			//TabBar.Frame = new RectangleF (0, 320, 768, 56);
			//TabBar.AutoresizingMask = UIViewAutoresizing.FlexibleWidth ;
		}
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
            //txtCellNumber.Dispose ();

   //         tab1.View.Layer.RemoveAllAnimations();
   //         tab2.View.Layer.RemoveAllAnimations();
   //         tab3.View.Layer.RemoveAllAnimations();
   //         tab4.View.Layer.RemoveAllAnimations();
   //         tab5.View.Layer.RemoveAllAnimations();
   //         tab6.View.Layer.RemoveAllAnimations();

   //         DisposeClass.DisposeEx (tab1.View);
			//DisposeClass.DisposeEx (tab2.View);
			//DisposeClass.DisposeEx (tab3.View);
			//DisposeClass.DisposeEx (tab4.View);
			//DisposeClass.DisposeEx (tab5.View);
			//DisposeClass.DisposeEx (tab6.View);
			//tab1.Dispose();
			//tab2.Dispose();
			//tab3.Dispose();
			//tab4.Dispose();
			//tab5.Dispose();
			//tab6.Dispose();
			//foreach (UIView view in this.View.Subviews) {

			//	DisposeClass.DisposeEx(view);
			//}
			//this.View.Dispose ();
			//this.View.Dispose ();


		}
	}
}

