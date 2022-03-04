using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using ObjCRuntime;

namespace WBid.WBidiPad.iOS
{
	partial class ConstraintItemsView : UIView
	{
		public ConstraintItemsView (IntPtr handle) : base (handle)
		{
		}

		public static ConstraintItemsView Create()
		{
			var arr = NSBundle.MainBundle.LoadNib ("ConstraintItemsView", null, null);
			var v = Runtime.GetNSObject<ConstraintItemsView> (arr.ValueAt(0));
			return v;
		}

		public override void AwakeFromNib(){
			//MyLabel.Text = "hello from the SomeView class";
			//ButtonSelected.SetImage(UIImage.FromFile("marked@2x.png"),UIControlState.Selected);
		}
	}
}
