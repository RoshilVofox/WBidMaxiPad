using System;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Core;

namespace WBid.WBidiPad.iOS
{
	public partial class ModernContainerViewController : UITableViewController
	{
		public ModernContainerViewController() : base (UITableViewStyle.Plain)
		{
		}

		public override void DidReceiveMemoryWarning()
		{
			// Releases the view if it doesn't have a superview.
			//			base.View.DangerousRelease();

			base.DidReceiveMemoryWarning();

			// Release any cached data, images, etc that aren't in use.
		}

		//		public override void ViewWillDisappear (bool animated)
		//		{
		//			TableView.RemoveFromSuperview ();
		//		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);

			//			foreach (UIView view in this.View.Subviews) {
			//
			//				DisposeClass.DisposeEx(view);
			//			}
		}
		public ModernContainerViewController(IntPtr handle) : base (handle)
		{
		}
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			//this.View.BackgroundColor = UIColor.Red;

			//TableView.BackgroundColor = UIColor.Blue;
			// Register the TableView's data source
			//CommonClass.IsModernScrollClassic="TRUE";


			if (CommonClass.IsModernScrollClassic == "TRUE")
			{
				ModernViewControllerSourceClassicScroll DataSource = new ModernViewControllerSourceClassicScroll();
				TableView.Source = DataSource;
			}
			else {
				ModernViewControllerSource DataSource = new ModernViewControllerSource();
				TableView.Source = DataSource;
				DataSource.tablView = TableView;
			}

			//classic
			//DataSource.tablView = TableView;
			TableView.SeparatorInset = new UIEdgeInsets(0, 3, 0, 3);
			TableView.SeparatorColor = UIColor.FromRGB(220, 220, 220);
			TableView.ContentInset = new UIEdgeInsets(0, 0, 5, 0);
			TableView.ScrollIndicatorInsets = new UIEdgeInsets(0, 0, 5, 0);
			this.Editing = true;

			TableView.AllowsSelectionDuringEditing = true;
			TableView.BackgroundColor = ColorClass.normDayColor;
			CommonClass.lineVC.modernList = (ModernContainerViewController)this;
		}
		public void reloadModernData()
		{

			TableView.ReloadData();

		}

		public void ScrollUp()
		{

			NSIndexPath[] arrVisibleIndex = TableView.IndexPathsForVisibleRows;
			Console.WriteLine("indexes " + TableView.IndexPathsForVisibleRows);

			for (int i = 0; i < (int)arrVisibleIndex.Length; i++)
			{
				Console.WriteLine("Rows-" + arrVisibleIndex[i].Row);

			}
			int count = arrVisibleIndex.Length / 2;
			NSIndexPath NextIndex = NSIndexPath.FromRowSection(arrVisibleIndex[count].Row + 50, 0);
			if (NextIndex.Row > GlobalSettings.Lines.Count)
			{
				NextIndex = NSIndexPath.FromRowSection(TableView.NumberOfRowsInSection(0) - 1, 0);
				TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
			}
			else
				TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);

		}
	}
}

