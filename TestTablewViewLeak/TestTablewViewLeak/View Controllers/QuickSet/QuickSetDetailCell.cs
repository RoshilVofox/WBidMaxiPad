

using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;

namespace WBid.WBidiPad.iOS
{
	public partial class QuickSetDetailCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("QuickSetDetailCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("QuickSetDetailCell");

		public QuickSetDetailCell (IntPtr handle) : base (handle)
		{
		}

		public static QuickSetDetailCell Create ()
		{
			return (QuickSetDetailCell)Nib.Instantiate (null, null) [0];
		}

		public void BindData (QSDetailData data, int index)
		{
			vwLine.Frame = new CGRect (197, 0, 1, data.RowHeight);
			vwLine.BackgroundColor = ColorClass.SummaryHeaderBorderColor;
			lblData.Frame = new CGRect (202, 0, 190, data.RowHeight);

			if (data.Type == 0) {
				lblName.Frame = new CGRect (8, 0, 384, 20);
				this.BackgroundColor = ColorClass.SummaryHeaderColor;
				lblName.Font = UIFont.BoldSystemFontOfSize (14);
			} else {
				lblName.Frame = new CGRect (8, 0, 384, 30);
				this.BackgroundColor = UIColor.Clear;
				lblName.Font = UIFont.SystemFontOfSize (13);
			}
			lblName.Text = data.Title;
			lblData.Font = UIFont.SystemFontOfSize (13);
			lblData.Text = data.DataValue;

		}

	}
}

