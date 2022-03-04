
using System;

using Foundation;
using UIKit;
using System.Drawing;

namespace  WBid.WBidiPad.iOS
{  
	//[Register("ConnectTimeView")]
	public partial class ConnectTimeView : UIView
	{

		public ConnectTimeView() {}
		public ConnectTimeView(IntPtr handle) : base(handle) {}

		public static readonly UINib Nib = UINib.FromName ("ConnectTimeView", NSBundle.MainBundle);
	

		public void SetValues(string day,string Departure, string Arrival,bool isEnabled)
		{
			lblDay.Text=day;

			lblArrivalTime.Text=Arrival;
			lblDepartureTime.Text=Departure;

            if (!isEnabled)
                lblDay.BackgroundColor = UIColor.Gray;
			//if()lblDay.BackgroundColor=UIColor.Gray;
		}
		public static ConnectTimeView Create()
		{
			return (ConnectTimeView)Nib.Instantiate(null, null)[0];
		}

	}
}

