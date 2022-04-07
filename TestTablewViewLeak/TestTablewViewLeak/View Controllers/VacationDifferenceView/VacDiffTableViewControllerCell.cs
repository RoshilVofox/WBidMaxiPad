using System;
using Foundation;
using UIKit;

namespace TestTablewViewLeak.ViewControllers.VacationDifferenceView
{
	public class VacDiffTableViewControllerCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString("VacDiffTableViewControllerCell");

		public VacDiffTableViewControllerCell() : base(UITableViewCellStyle.Value1, Key)
		{
			// TODO: add subviews to the ContentView, set various colors, etc.
			//TextLabel.Text = "TextLabel";
		}
	}
}

