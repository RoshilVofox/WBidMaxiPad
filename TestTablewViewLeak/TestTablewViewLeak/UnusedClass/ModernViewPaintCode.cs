
using System;

using Foundation;
using UIKit;
using CoreGraphics;

namespace TestTablewViewLeak
{
	public partial class ModernViewPaintCode : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("ModernViewPaintCode", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("ModernViewPaintCode");

		public ModernViewPaintCode (IntPtr handle) : base (handle)
		{
		}

		public static ModernViewPaintCode Create ()
		{
			return (ModernViewPaintCode)Nib.Instantiate (null, null) [0];
		}

	}
}

