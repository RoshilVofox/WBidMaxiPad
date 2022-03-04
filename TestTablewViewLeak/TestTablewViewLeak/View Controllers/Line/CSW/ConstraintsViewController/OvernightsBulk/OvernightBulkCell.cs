using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using System.Linq;
using WBid.WBidiPad.SharedLibrary.Utility;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using System.Collections.Generic;
using WBid.WBidiPad.Core.Enum;
using CoreGraphics;
using System.IO;

namespace WBid.WBidiPad.iOS
{
	public partial class OvernightBulkCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("OvernightBulkCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("OvernightBulkCell");

		public string DisplayMode;
		OvernightBulkCollectionController overnightCollection;
		WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		WeightCalculation weightCalc = new WeightCalculation();
		ConstraintCalculations constCalc = new ConstraintCalculations ();

		public OvernightBulkCell (IntPtr handle) : base (handle)
		{
		}

		public static OvernightBulkCell Create ()
		{
			return (OvernightBulkCell)Nib.Instantiate (null, null) [0];
		}

		public void bindData (NSIndexPath indexpath)
		{
			vwLabel.Transform = CGAffineTransform.MakeRotation((float)(-90 * 3.14 / 180.0));
			lblTitle.Text = "Overnight Cities";
			vwNoOvernights.Transform = CGAffineTransform.MakeRotation((float)(-90 * 3.14 / 180.0));
			lblNoOvernights.Text = "No Overnight exists";
			if (DisplayMode == "Constraints")
			{
				lblTitle.Hidden = false;
				//vwNoOvernights.Transform = CGAffineTransform.MakeRotation((float)(-90 * 3.14 / 180.0));
				//lblNoOvernights.Text = "No Overnight exists";

			}
			else
			{
               //vwNoOvernights.Layer.Frame = new CGRect(10, 200, 30, 30);
				//btnNone.Layer.Frame = new CGRect(10, 250, 30, 30);
				lblTitle.Hidden = true;
				//vwNoOvernights.Hidden = true;
				//lblNoOvernights.Hidden = true;
				//btnNone.Hidden = true;
				 lblNo.Hidden = true;
				lblYes.Hidden = true; 
			
			}

			if (overnightCollection != null)
			{
				overnightCollection.View.RemoveFromSuperview();
				overnightCollection = null;
			}
			var layout = new UICollectionViewFlowLayout();
			layout.SectionInset = new UIEdgeInsets(0, 0, 0, 0);
			layout.MinimumInteritemSpacing = 0;
			layout.MinimumLineSpacing = 0;
			layout.ItemSize = new CGSize(82, 30);
			overnightCollection = new OvernightBulkCollectionController(layout,DisplayMode);
			overnightCollection.View.Frame = vwCities.Bounds;
			overnightCollection.DisplayMode = DisplayMode;
			vwCities.AddSubview(overnightCollection.View);


		}

		partial void btnHelpIconTapped (UIKit.UIButton sender)
		{
			if (DisplayMode == "Constraints") {
				HelpViewController helpVC = new HelpViewController ();
				helpVC.pdfFileName = "Constraints";
				helpVC.pdfOffset = ConstraintsApplied.HelpPageOffset ["Overnight Cities - Bulk"];
				UINavigationController navCont = new UINavigationController (helpVC);
				navCont.NavigationBar.BarStyle = UIBarStyle.Black;
				navCont.NavigationBar.Hidden = true;
				navCont.ModalPresentationStyle = UIModalPresentationStyle.Custom;
				CommonClass.cswVC.PresentViewController (navCont, true, null);
			} else {
				HelpViewController helpVC = new HelpViewController ();
				helpVC.pdfFileName = "Weights";
				helpVC.selectRow = 1;
				helpVC.pdfOffset = WeightsApplied.HelpPageOffset ["Overnight Cities - Bulk"];
				UINavigationController navCont = new UINavigationController (helpVC);
				navCont.NavigationBar.BarStyle = UIBarStyle.Black;
				navCont.NavigationBar.Hidden = true;
				navCont.ModalPresentationStyle = UIModalPresentationStyle.Custom;
				CommonClass.cswVC.PresentViewController (navCont, true, null);
			}
		}

		partial void btnRemoveTapped (UIKit.UIButton sender)
		{
			WBidHelper.PushToUndoStack ();
			if (DisplayMode == "Constraints") {
				wBIdStateContent.CxWtState.BulkOC.Cx = false;
				wBIdStateContent.Constraints.BulkOvernightCity.OverNightNo.Clear();
				wBIdStateContent.Constraints.BulkOvernightCity.OverNightYes.Clear();
				constCalc.ApplyOvernightBulkConstraint (wBIdStateContent.Constraints.BulkOvernightCity);
				NSNotificationCenter.DefaultCenter.PostNotificationName ("AddConstraints", null);
				NSNotificationCenter.DefaultCenter.PostNotificationName ("LineCountReload", null);
			} else {
				wBIdStateContent.CxWtState.BulkOC.Wt = false;
				wBIdStateContent.Weights.OvernightCitybulk.Clear();
				weightCalc.ApplyOvernightCityBulkWeight(wBIdStateContent.Weights.OvernightCitybulk);
				NSNotificationCenter.DefaultCenter.PostNotificationName ("AddWeights", null);
			}
		}

	}
}

