
using System;

using Foundation;
using UIKit;

namespace TestTablewViewLeak
{
	public partial class Modern : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("Modern", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("Modern");

		public Modern (IntPtr handle) : base (handle)
		{
		}

		public static Modern Create ()
		{
			return (Modern)Nib.Instantiate (null, null) [0];
		}
	}
}

