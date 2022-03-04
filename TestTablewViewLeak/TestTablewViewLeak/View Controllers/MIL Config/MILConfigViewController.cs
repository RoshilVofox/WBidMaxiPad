
using System;
using System.Drawing;

using Foundation;
using UIKit;
using CoreGraphics;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using System.Collections.Generic;
using System.Linq;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.SharedLibrary.Utility;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;

//using WBid.WBidClient.Main.BusinessLogic;
using WBid.WBidiPad.PortableLibrary;
using System.IO;

namespace WBid.WBidiPad.iOS
{
	public partial class MILConfigViewController : UIViewController
	{
		MILData milData;

		public MILConfigViewController () : base ("MILConfigViewController", null)
		{
		}

		NSObject obj;
		//		public List<Absense> MILList;

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			obj = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("MILReload"), delegate {
				ReloadView ();
			});
			foreach (var lbl in lblStart) {
				lbl.Text = string.Empty;
			}
			foreach (var lbl in lblEnd) {
				lbl.Text = string.Empty;
			}

			//open apply view
			if (File.Exists (WBidHelper.MILFilePath)) {
				LineInfo lineInfo = null;
				using (FileStream milStream = File.OpenRead (WBidHelper.MILFilePath)) {

					MILData milDataobject = new MILData ();
					milData = ProtoSerailizer.DeSerializeObject (WBidHelper.MILFilePath, milDataobject, milStream);
				}
				var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
				GlobalSettings.MILDates = wBidStateContent.MILDateList;

//				navBar.Items [0].Title = "MIL Dates View";
//				this.View.AddSubview (vwMILInfo);
//				vwMILInfo.Center = this.View.Center;
//				for (int i = 0; i < GlobalSettings.MILDates.Count; i++) {
//					lblStart [i].Text = GlobalSettings.MILDates [i].StartAbsenceDate.ToString ("MM/dd/yyyy HH:mm");
//					lblEnd [i].Text = GlobalSettings.MILDates [i].EndAbsenceDate.ToString ("MM/dd/yyyy HH:mm");
//				}

				if (GlobalSettings.MILDates != null && GlobalSettings.MILDates.Count > 0) {
					navBar.Items [0].Title = "MIL Dates View";
					
					vwMILInfo.Center = this.View.Center;
                    this.View.AddSubview(vwMILInfo);
					for (int i = 0; i < GlobalSettings.MILDates.Count; i++) {
						lblStart [i].Text = GlobalSettings.MILDates [i].StartAbsenceDate.ToString ("MM/dd/yyyy HH:mm");
						lblEnd [i].Text = GlobalSettings.MILDates [i].EndAbsenceDate.ToString ("MM/dd/yyyy HH:mm");
					}
					CommonClass.MILList = GlobalSettings.MILDates;
				} else {
					navBar.Items [0].Title = "MIL Dates Selection View";
					CommonClass.MILList = new System.Collections.Generic.List<Absense> ();
					CommonClass.MILList.Add (new Absense () {
						StartAbsenceDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate,
						EndAbsenceDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate
					});
				}

			} else {
				navBar.Items [0].Title = "MIL Dates Selection View";
				CommonClass.MILList = new System.Collections.Generic.List<Absense> ();
				CommonClass.MILList.Add (new Absense () {
					StartAbsenceDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate,
					EndAbsenceDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate
				});
			}
			//

//			if (GlobalSettings.MILDates != null) {
//				CommonClass.MILList = GlobalSettings.MILDates;
//			} else {
//				CommonClass.MILList = new System.Collections.Generic.List<Absense> ();
//				CommonClass.MILList.Add (new Absense () {
//					StartAbsenceDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate,
//					EndAbsenceDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate
//				});
//			}

			tblMILDates.TableFooterView = new UIView (CGRect.Empty);
			tblMILDates.Source = new MILDatesTableSource (this);
			ReloadView ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			NSNotificationCenter.DefaultCenter.RemoveObserver (obj);

		}

		partial void btnDoneTapped (UIKit.UIBarButtonItem sender)
		{
			this.DismissViewController (true, null);
		}

		partial void btnAddTapped (UIKit.UIButton sender)
		{
			CommonClass.MILList.Add (new Absense () {
				StartAbsenceDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate,
				EndAbsenceDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate
			});
			ReloadView ();
		}

		partial void btnApplyTapped (UIKit.UIButton sender)
		{
			WBidHelper.PushToUndoStack ();
			CommonClass.lineVC.UpdateUndoRedoButtons ();
			var loadingOverlay = new LoadingOverlay (View.Bounds, "Applying MIL. Please wait..");
			View.Add (loadingOverlay);
			var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			InvokeInBackground (() => {
				GlobalSettings.MILDates = GenarateOrderedMILDates (wBidStateContent.MILDateList);
				//Apply MIL values (calculate property values including Modern bid line properties
				//==============================================

				GlobalSettings.MILData = milData.MILValue;
				GlobalSettings.MenuBarButtonStatus.IsMIL = true;

				RecalcalculateLineProperties recalcalculateLineProperties = new RecalcalculateLineProperties ();
				recalcalculateLineProperties.CalcalculateLineProperties ();
				StateManagement statemanage=new StateManagement();

				statemanage.ApplyCSW(wBidStateContent);

				InvokeOnMainThread (() => {
					GlobalSettings.isModified = true;
					CommonClass.lineVC.UpdateSaveButton ();
					loadingOverlay.Hide ();
					CommonClass.lineVC.SetVacButtonStates ();
					NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
					this.DismissViewController (true, null);
				});
			});

//			MainViewModel mainviewmodel = ServiceLocator.Current.GetInstance<MainViewModel>();
//			mainviewmodel.IsMIL = true;
//			mainviewmodel.ISEnableOverlapCorrection = false;
//			mainviewmodel.ISEnableVacationOverlapCorrection = false;
//			mainviewmodel.IsEOMVacation = false;
		}

		partial void btnCalculateNewTapped (UIKit.UIButton sender)
		{
			vwMILInfo.RemoveFromSuperview ();

		}

		private List<Absense> GenarateOrderedMILDates (List<Absense> milList)
		{
			List<Absense> absence = new List<Absense> ();
			if (milList.Count > 0) {
				absence.Add (new Absense {
					StartAbsenceDate = milList.FirstOrDefault ().StartAbsenceDate,
					EndAbsenceDate = milList.FirstOrDefault ().EndAbsenceDate,
					AbsenceType = "VA"
				});

				for (int count = 0; count < milList.Count - 1; count++) {
					if ((milList [count + 1].StartAbsenceDate - milList [count].EndAbsenceDate).Days == 1) {
						absence [absence.Count - 1].EndAbsenceDate = milList [count + 1].EndAbsenceDate;
					} else {
						absence.Add (new Absense {
							StartAbsenceDate = milList [count + 1].StartAbsenceDate,
							EndAbsenceDate = milList [count + 1].EndAbsenceDate,
							AbsenceType = "VA"
						});
					}
				}
			}
			return absence;
		}

		private bool ValidateDates ()
		{
			var valid = true;
			var mildates = CommonClass.MILList.ToList ();

			mildates.ForEach (item => {
				mildates.Where (y => y != item).ToList ().ForEach (z => {
					if (item.StartAbsenceDate > item.EndAbsenceDate
					    || item.StartAbsenceDate < z.StartAbsenceDate && z.StartAbsenceDate < item.EndAbsenceDate
					    || item.StartAbsenceDate < z.EndAbsenceDate && z.EndAbsenceDate < item.EndAbsenceDate) {
						valid = false;
						return;
					}

				});
			});

			return valid;
		}

		partial void btnCancel1Tapped (UIKit.UIButton sender)
		{
			this.DismissViewController (true, null);
		}

		partial void btnCalculateTapped (UIKit.UIButton sender)
		{
			try{


				if (!(System.IO.File.Exists (WBidHelper.GetAppDataPath () + "/FlightData.NDA")) && (GlobalSettings.WBidINIContent.User.IsSouthWestWifiTest || WBidHelper.IsSouthWestWifiOr2wire()))
					{
				
                    UIAlertController okAlertController = UIAlertController.Create("WBidMax", "You have a limited Wifi connection using the SouthwestWifi.You are not be able to do the Flight data download for MIL operation using SouthWest Wifi.", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                }
					else
					{
			WBidHelper.PushToUndoStack ();
			CommonClass.lineVC.UpdateUndoRedoButtons ();
			var milList = CommonClass.MILList.OrderBy (x => x.StartAbsenceDate).ToList ();
			bool isValid = ValidateDates ();
			if (isValid) {
				var loadingOverlay = new LoadingOverlay (View.Bounds, "Calculating MIL data. Please wait..");
				View.Add (loadingOverlay);

				InvokeInBackground (() => {
					GlobalSettings.MILDates = GenarateOrderedMILDates (milList);
					CalculateMIL calculateMIL = new CalculateMIL ();
					MILParams milParams = new MILParams ();

					NetworkData networkData = new NetworkData ();
					if (System.IO.File.Exists (WBidHelper.GetAppDataPath () + "/FlightData.NDA"))
						networkData.ReadFlightRoutes ();
					else
						networkData.GetFlightRoutes ();

					//calculate MIL value and create MIL File
					//==============================================
					WBidCollection.GenerateSplitPointCities ();
					milParams.Lines = GlobalSettings.Lines.ToList ();
					Dictionary<string, TripMultiMILData> milvalue = calculateMIL.CalculateMILValues (milParams);
					var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
					wBidStateContent.MILDateList = milList;

					MILData milData = new MILData ();
					milData.Version = GlobalSettings.MILFileVersion;
					milData.MILValue = milvalue;

					var stream = File.Create (WBidHelper.MILFilePath);
					ProtoSerailizer.SerializeObject (WBidHelper.MILFilePath, milData, stream);
					stream.Dispose ();
					stream.Close ();

					//==============================================

					//Apply MIL values (calculate property values including Modern bid line properties
					//==============================================
//					InvokeOnMainThread (() => {
//					WBidHelper.PushToUndoStack ();
//					CommonClass.lineVC.UpdateUndoRedoButtons ();
//					});

					GlobalSettings.MILData = milvalue;
					GlobalSettings.MenuBarButtonStatus.IsMIL = true;

						try{
					RecalcalculateLineProperties recalcalculateLineProperties = new RecalcalculateLineProperties ();
					recalcalculateLineProperties.CalcalculateLineProperties ();
								StateManagement statemanage=new StateManagement();

								statemanage.ApplyCSW(wBidStateContent);
						}catch(Exception exc)
						{
							InvokeOnMainThread (() => { throw exc;});
						}

					InvokeOnMainThread (() => {
						GlobalSettings.isModified = true;
						CommonClass.lineVC.UpdateSaveButton ();
						loadingOverlay.Hide ();
						CommonClass.lineVC.SetVacButtonStates ();
						NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
						this.DismissViewController (true, null);
					});
				});

			} else {
			
                 UIAlertController okAlertController = UIAlertController.Create("WBidMax", "Please enter valid dates", UIAlertControllerStyle.Alert);
                 okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                 this.PresentViewController(okAlertController, true, null);

                    }
					}
			}catch(Exception ex)
						{

				throw ex;
			
			}
		}

		partial void btnCancel2Tapped (UIKit.UIButton sender)
		{
			this.DismissViewController (true, null);
		}

		public void ReloadView ()
		{
			if (CommonClass.MILList.Count == 4)
				btnAdd.Enabled = false;
			else
				btnAdd.Enabled = true;
			tblMILDates.ReloadData ();
		}
	}

	public class MILDatesTableSource : UITableViewSource
	{

		MILConfigViewController parentVC;

		public MILDatesTableSource (MILConfigViewController parent)
		{
			parentVC = parent;
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return CommonClass.MILList.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.RegisterNibForCellReuse (UINib.FromName ("MILConfigCell", NSBundle.MainBundle), "MILConfigCell");
			MILConfigCell cell = tableView.DequeueReusableCell (new NSString ("MILConfigCell")) as MILConfigCell;
			cell.BindData (CommonClass.MILList [indexPath.Row], indexPath.Row);
			return cell;
		}

	}

}

