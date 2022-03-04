
using System;

using Foundation;
using UIKit;

namespace WBid.WBidiPad.iOS
{
	public partial class UserAccountDifferenceCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("UserAccountDifferenceCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("UserAccountDifferenceCell");
		public UserDifferenceTableDataSource Objsource;

		public UserAccountDifferenceCell (IntPtr handle) : base (handle)
		{
			
		}

		public void SetDetailCell(string DetailHeader,string localData, string dbData)
		{
			lblEmployeeDetails.Text= DetailHeader;
			SegSelection.SetTitle(localData,0);
			SegSelection.SetTitle(dbData,1);

		}
		partial void SegmentButtonClicked (NSObject sender)
		{
			UISegmentedControl SegControl=(UISegmentedControl)sender;
			Console.WriteLine("Row selected"+this.Tag+",Segment selected "+SegControl.SelectedSegment);
			Objsource.Changesegment((int)this.Tag,(int)SegControl.SelectedSegment);

		}
		public static UserAccountDifferenceCell Create ()
		{
			return (UserAccountDifferenceCell)Nib.Instantiate (null, null) [0];
		}
	}
}

