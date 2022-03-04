using System;
using CoreGraphics;
using Foundation;
using UIKit;

using System.Linq;

using System.Collections.Generic;
using CoreAnimation;

using CoreGraphics;

namespace TestTablewViewLeak
{
    public partial class ModernViewCell : UITableViewCell
    {
        class MyPopDelegate : UIPopoverControllerDelegate
        {
            ModernViewCell _parent;
            public MyPopDelegate(ModernViewCell parent)
            {
                _parent = parent;
            }

            
			public override void DidDismiss (UIPopoverController popoverController)
			{
				_parent.popoverController = null;
			}

        }

		class MyPopDelegate2 : UIPopoverControllerDelegate
		{
			ModernViewCell _parent;
			public MyPopDelegate2(ModernViewCell parent)
			{
				_parent = parent;
			}
			public override void DidDismiss (UIPopoverController popoverController)
			{
				_parent.popoverController = null;
			}
		}

        public static readonly UINib Nib = UINib.FromName("ModernViewCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("ModernViewCell");

        UITapGestureRecognizer singleTap;
        UITapGestureRecognizer singleTap2;
        UIPopoverController popoverController;
		UITapGestureRecognizer tipGest;
		UITapGestureRecognizer tipGest2;
	
        public ModernViewCell(IntPtr handle)
            : base(handle)
        {
        }

        public static ModernViewCell Create()
        {
            return (ModernViewCell)Nib.Instantiate(null, null)[0];
        }

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();


		}
    


    }
}

