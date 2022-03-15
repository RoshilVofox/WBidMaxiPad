
using System;
using System.Drawing;

using Foundation;
using UIKit;
using CoreGraphics;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using System.Linq;
using System.IO;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.SharedLibrary.Utility;
using System.Collections.Generic;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using WBid.WBidiPad.Core.Enum;

namespace WBid.WBidiPad.iOS
{
	public partial class QuickSetViewController : UIViewController
	{
		WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		public string selectedCSWQset;
		public string selectedColumnsQset;
		//		public NSIndexPath selectedIndex;

		public QuickSetViewController () : base ("QuickSetViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			//txtCellNumber.Dispose ();

//			foreach (UIView view in this.View.Subviews) {
//
//				DisposeClass.DisposeEx(view);
//			}
//			this.View.Dispose ();
			this.View.UserInteractionEnabled = true;

		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
			this.Title = "QuickSets";
			this.NavigationItem.BackBarButtonItem = new UIBarButtonItem ("Back", UIBarButtonItemStyle.Plain, (object sender, EventArgs e) => {
				this.NavigationController.PopViewController (true);
			});

			if (GlobalSettings.QuickSets == null) {
				LoadQuickSetFile ();
			}
			EnableDisableUseDelete ();

			tblQuickSets.Layer.BorderWidth = 1;
			tblQuickSets.Layer.BorderColor = ColorClass.SummaryHeaderBorderColor.CGColor;
			tblQuickSets.SeparatorInset = new UIEdgeInsets (0, 3, 0, 3);
			tblQuickSets.TableFooterView = new UIView (CGRect.Empty);
			tblQuickSets.Source = new QuickSetTableSource (this);

			sqQuickSetToggle.ValueChanged += sgQuickSetToggleChanged;
			btnNew.TouchUpInside += btnNewTapped;
			btnUse.TouchUpInside += btnUseTapped;
			btnDelete.TouchUpInside += btnDeleteTapped;

			//in ios 15, the header coming as transparent and below code solved the issue
			var appearance = new UINavigationBarAppearance();
			appearance.ConfigureWithOpaqueBackground();
			appearance.BackgroundColor = ColorClass.TopHeaderColor;
			this.NavigationItem.StandardAppearance = appearance;
			this.NavigationItem.ScrollEdgeAppearance = this.NavigationItem.StandardAppearance;


		}

		void sgQuickSetToggleChanged (object sender, EventArgs e)
		{
			selectedCSWQset = string.Empty;
			selectedColumnsQset = string.Empty;
			EnableDisableUseDelete ();
			tblQuickSets.ReloadData ();
		}

		public void EnableDisableUseDelete ()
		{
			btnUse.Enabled = (!string.IsNullOrEmpty (selectedCSWQset) || !string.IsNullOrEmpty (selectedColumnsQset));
			btnDelete.Enabled = (!string.IsNullOrEmpty (selectedCSWQset) || !string.IsNullOrEmpty (selectedColumnsQset));
		}

		private void LoadQuickSetFile ()
		{
                        			if (File.Exists (WBidHelper.GetQuickSetFilePath ())) {
				GlobalSettings.QuickSets = XmlHelper.DeserializeFromXml<QuickSets> (WBidHelper.GetQuickSetFilePath ());
			} else {
				GlobalSettings.QuickSets = new QuickSets ();
				GlobalSettings.QuickSets.QuickSetColumn = new List<QuickSetColumn> ();
				GlobalSettings.QuickSets.QuickSetCSW = new List<QuickSetCSW> ();
			}
		}

		void btnDeleteTapped (object sender, EventArgs e)
		{
			var msg = string.Empty;
			if (sqQuickSetToggle.SelectedSegment == 0)
				msg = "Are you sure you want to delete \"" + selectedCSWQset + "\" from QuickSet?";
			else
				msg = "Are you sure you want to delete \"" + selectedColumnsQset + "\" from QuickSet?";

			UIAlertController alert = UIAlertController.Create("QuickSet", msg, UIAlertControllerStyle.Alert);

            alert.AddAction(UIAlertAction.Create("NO", UIAlertActionStyle.Default, (actionOK) => {

            }));             alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) => {
                if (sqQuickSetToggle.SelectedSegment == 0)
                {
                    GlobalSettings.QuickSets.QuickSetCSW.RemoveAll(x => x.QuickSetCSWName == selectedCSWQset);
                    GlobalSettings.QuickSets.QuickSetUpdatedTime = DateTime.Now.ToUniversalTime();
                    GlobalSettings.QuickSets.IsModified = true;
                    XmlHelper.SerializeToXml(GlobalSettings.QuickSets, WBidHelper.GetQuickSetFilePath());
                    tblQuickSets.ReloadData();
                    selectedCSWQset = string.Empty;
                    EnableDisableUseDelete();
                   


                }
                else
                {
                    GlobalSettings.QuickSets.QuickSetColumn.RemoveAll(x => x.QuickSetColumnName == selectedColumnsQset);
                    GlobalSettings.QuickSets.QuickSetUpdatedTime = DateTime.Now.ToUniversalTime();
                    GlobalSettings.QuickSets.IsModified = true;
                    XmlHelper.SerializeToXml(GlobalSettings.QuickSets, WBidHelper.GetQuickSetFilePath());
                    tblQuickSets.ReloadData();
                    selectedColumnsQset = string.Empty;
                    EnableDisableUseDelete();
                  
                }

            }));             this.PresentViewController(alert, true, null);




        }

        void btnUseTapped (object sender, EventArgs e)
		{
			if (sqQuickSetToggle.SelectedSegment == 0) {
				WBidHelper.PushToUndoStack ();
				ApplyCSWDataFromQuickSets (selectedCSWQset);
				GlobalSettings.isModified = true;
				CommonClass.lineVC.UpdateSaveButton ();
				NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
			} else {
				ApplyColumnsDataFromQuickSets (selectedColumnsQset);
				CommonClass.bidLineProperties.Clear ();
				CommonClass.modernProperties.Clear ();
				if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM) {
					foreach (var item in GlobalSettings.WBidINIContent.BidLineVacationColumns) {
						var col = GlobalSettings.BidlineAdditionalvacationColumns.FirstOrDefault (x => x.Id == item);
						if (col != null) {
							CommonClass.bidLineProperties.Add (col.DisplayName);
						}
					}
					foreach (var item in GlobalSettings.WBidINIContent.ModernVacationColumns) {
						var col = GlobalSettings.ModernAdditionalvacationColumns.FirstOrDefault (x => x.Id == item);
						if (col != null) {
							CommonClass.modernProperties.Add (col.DisplayName);
						}
					}
					LineSummaryBL.GetBidlineViewAdditionalVacationColumns ();
					LineSummaryBL.SetSelectedBidLineVacationColumnstoGlobalList ();
					LineSummaryBL.GetModernViewAdditionalVacationalColumns ();
					LineSummaryBL.SetSelectedModernBidLineVacationColumnstoGlobalList ();
				} else {
					foreach (var item in GlobalSettings.WBidINIContent.BidLineNormalColumns) {
						var col = GlobalSettings.BidlineAdditionalColumns.FirstOrDefault (x => x.Id == item);
						if (col != null) {
							CommonClass.bidLineProperties.Add (col.DisplayName);
						}
					}
					foreach (var item in GlobalSettings.WBidINIContent.ModernNormalColumns) {
						var col = GlobalSettings.ModernAdditionalColumns.FirstOrDefault (x => x.Id == item);
						if (col != null) {
							CommonClass.modernProperties.Add (col.DisplayName);
						}
					}
					LineSummaryBL.GetBidlineViewAdditionalColumns ();
					LineSummaryBL.SetSelectedBidLineColumnstoGlobalList ();
					LineSummaryBL.GetModernViewAdditionalColumns ();
					LineSummaryBL.SetSelectedModernBidLineColumnstoGlobalList ();
					NSNotificationCenter.DefaultCenter.PostNotificationName ("DataCulumnsUpdated", null);
				}
			}
		}

		private void ApplyCSWDataFromQuickSets (string cSWName)
		{
			try {
				QuickSetCSW quickSetCSW = GlobalSettings.QuickSets.QuickSetCSW.FirstOrDefault (X => X.QuickSetCSWName == cSWName);
                quickSetCSW.CxWtState.Commute.Cx = false;
                quickSetCSW.CxWtState.Commute.Wt = false;
                               
				if (quickSetCSW.CxWtState.EQUIP.Cx)
				{
					quickSetCSW.Constraints.EQUIP.lstParameters.RemoveAll(x => x.ThirdcellValue == "500" || x.ThirdcellValue == "300");
					if (quickSetCSW.Constraints.EQUIP.lstParameters.Count == 0)
						quickSetCSW.CxWtState.EQUIP.Cx = false;
				}
				if (quickSetCSW.CxWtState.EQUIP.Wt)
				{
					quickSetCSW.Weights.EQUIP.lstParameters.RemoveAll(x => x.SecondlValue == 500 || x.SecondlValue == 300);
					if (quickSetCSW.Weights.EQUIP.lstParameters.Count == 0)
						quickSetCSW.CxWtState.EQUIP.Wt = false;
				}
				if (quickSetCSW.Constraints.CLAuto == null) {
					quickSetCSW.Constraints.CLAuto = new FtCommutableLine () {ToHome = true,
						ToWork = false,
						NoNights = false,
						BaseTime = 10,
						ConnectTime = 30,
						CheckInTime = 120
					};
				}
				if (quickSetCSW.Weights.CLAuto == null) {
					quickSetCSW.Weights.CLAuto = new WtCommutableLineAuto (){
						ToHome = true,
						ToWork = false ,
						NoNights = false,
						BaseTime = 10,
						ConnectTime = 30,
						CheckInTime = 120
					};
				}
				if(quickSetCSW.Constraints.Commute==null)
				{
					quickSetCSW.Constraints.Commute=new Commutability {BaseTime=10, ConnectTime=30,CheckInTime=60, SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.MoreThan, Value = 100 };
				}
				if(quickSetCSW.Weights.Commute==null)
				{
					quickSetCSW.Weights.Commute=new Commutability {BaseTime=10, ConnectTime=30,CheckInTime=60, SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.MoreThan, Value = 100,Weight=0 };
				}
				wBIdStateContent.Constraints = new Constraints (quickSetCSW.Constraints);
				wBIdStateContent.Weights = new Weights (quickSetCSW.Weights);
				if(quickSetCSW.CxWtState.CLAuto==null)
					quickSetCSW.CxWtState.CLAuto=new StateStatus{Wt=false,Cx=false};
				if(quickSetCSW.CxWtState.CitiesLegs==null)
					quickSetCSW.CxWtState.CitiesLegs=new StateStatus{Wt=false,Cx=false};
				if(quickSetCSW.CxWtState.Commute==null)
					quickSetCSW.CxWtState.Commute=new StateStatus{Wt=false,Cx=false};
				if(quickSetCSW.Constraints.CLAuto.City==null)
				{
					quickSetCSW.CxWtState.CLAuto.Cx=false;
				}
				if(quickSetCSW.Weights.CLAuto.City==null)
				{
					quickSetCSW.CxWtState.CLAuto.Wt=false;
				}
				if(quickSetCSW.Constraints.Commute.City==null)
				{
					quickSetCSW.CxWtState.Commute.Cx=false;
				}
				if(quickSetCSW.Weights.Commute.City==null)
				{
					quickSetCSW.CxWtState.Commute.Wt=false;
				}

				wBIdStateContent.CxWtState = new CxWtState (quickSetCSW.CxWtState);
				wBIdStateContent.SortDetails = new SortDetails (quickSetCSW.SortDetails);

				if (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO") {
					wBIdStateContent.CxWtState.FaPosition.A = false;
					wBIdStateContent.CxWtState.FaPosition.B = false;
					wBIdStateContent.CxWtState.FaPosition.C = false;
					wBIdStateContent.CxWtState.FaPosition.D = false;
					wBIdStateContent.CxWtState.Position.Wt = false;
					wBIdStateContent.Weights.POS.lstParameters = null;
				}
				if (wBIdStateContent.Constraints.StartDayOftheWeek.SecondcellValue == null)
				{
					wBIdStateContent.Constraints.StartDayOftheWeek.SecondcellValue = "1";
					foreach (var item in wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters)
					{
						if (item.SecondcellValue == null)
						{
							item.SecondcellValue = "1";
						}
					}
				}
				StateManagement stateManagement = new StateManagement ();
				stateManagement.ApplyCSW (wBIdStateContent);
			} catch (Exception ex) {
				throw ex;
			}
		}

		private void ApplyColumnsDataFromQuickSets (string colName)
		{
			try {
				QuickSetColumn quickSetCol = GlobalSettings.QuickSets.QuickSetColumn.FirstOrDefault (X => X.QuickSetColumnName == colName);
				GlobalSettings.WBidINIContent.DataColumns = new List<DataColumn> (quickSetCol.SummaryNormalColumns);
				GlobalSettings.WBidINIContent.SummaryVacationColumns = new List<DataColumn> (quickSetCol.SummaryVacationColumns);
				GlobalSettings.WBidINIContent.BidLineNormalColumns = new List<int> (quickSetCol.BidLineNormalColumns);
				GlobalSettings.WBidINIContent.BidLineVacationColumns = new List<int> (quickSetCol.BidLineVacationColumns);
				GlobalSettings.WBidINIContent.ModernNormalColumns = new List<int> (quickSetCol.ModernNormalColumns);
				GlobalSettings.WBidINIContent.ModernVacationColumns = new List<int> (quickSetCol.ModernVacationColumns);

				//remove the legs in 500 and legs in 300 columns
				GlobalSettings.WBidINIContent.SummaryVacationColumns.RemoveAll(x => x.Id == 58 || x.Id == 59);
				GlobalSettings.WBidINIContent.SummaryVacationColumns.RemoveAll(x => x.Id == 58 || x.Id == 59);

				GlobalSettings.WBidINIContent.ModernNormalColumns.RemoveAll(x => x == 58 || x == 59);
				GlobalSettings.WBidINIContent.ModernNormalColumns.RemoveAll(x => x == 58 || x == 59);

				GlobalSettings.WBidINIContent.BidLineNormalColumns.RemoveAll(x => x == 58 || x == 59);
				GlobalSettings.WBidINIContent.BidLineVacationColumns.RemoveAll(x => x == 58 || x == 59);

				GlobalSettings.WBidINIContent.DataColumns.RemoveAll(x => x.Id == 58 || x.Id == 59);
				GlobalSettings.WBidINIContent.DataColumns.RemoveAll(x => x.Id == 58 || x.Id == 59);

				WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());
			} catch (Exception ex) {
				throw ex;
			}

		}

		void btnNewTapped (object sender, EventArgs e)
		{
			var msg = string.Empty;
			if (sqQuickSetToggle.SelectedSegment == 0)
				msg = "Enter a name for CSW QuickSet";
			else
				msg = "Enter a name for Columns QuickSet";


            UIAlertController alert = UIAlertController.Create("QuickSet", msg, UIAlertControllerStyle.Alert);

            alert.AddTextField(delegate (UITextField textField)
            {
               
            });             alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (actionCancel) => {

            }));              alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {
                string tCode = alert.TextFields[0].Text;
                NewQuickSetNameEntererd(tCode);

            }));              this.PresentViewController(alert, true, null);



        }

        void NewQuickSetNameEntererd (string name)
		{
			if (!string.IsNullOrEmpty (name)) {
				var msg = string.Empty;
				if (sqQuickSetToggle.SelectedSegment == 0) {
					// CSW
					if (GlobalSettings.QuickSets.QuickSetCSW.Count (x => x.QuickSetCSWName == name) == 0) {
                        GlobalSettings.QuickSets.QuickSetUpdatedTime = DateTime.Now.ToUniversalTime();
                        GlobalSettings.QuickSets.IsModified = true;
						AddCswDatatoQuickSets (name);
						tblQuickSets.ReloadData ();

                       
					} else {
						msg = name + " already exists. Please choose another name!";
                        UIAlertController okAlertController = UIAlertController.Create("QuickSet", msg, UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);
                    }
				} else {
					// Columns
					if (GlobalSettings.QuickSets.QuickSetColumn.Count (x => x.QuickSetColumnName == name) == 0) {
                        GlobalSettings.QuickSets.QuickSetUpdatedTime = DateTime.Now.ToUniversalTime();
                        GlobalSettings.QuickSets.IsModified = true;
						AddColumnDatatoQuickSets (name);
						tblQuickSets.ReloadData ();


					} else {
						msg = name + " already exists. Please choose another name!";
                        UIAlertController okAlertController = UIAlertController.Create("QuickSet", msg, UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);
                    }
				}
			}
		}

		private void AddCswDatatoQuickSets (string cSwName)
		{
			try {
				var quickSetCsw = new QuickSetCSW {
					QuickSetCSWName = cSwName,
					Constraints = new Constraints (wBIdStateContent.Constraints),
					Weights = new Weights (wBIdStateContent.Weights),
					SortDetails = new SortDetails (wBIdStateContent.SortDetails),
					CxWtState = new CxWtState (wBIdStateContent.CxWtState)
				};

				if (quickSetCsw.CxWtState.SDO.Cx || quickSetCsw.CxWtState.SDO.Wt) {
					var message = "The Following constraints and weights are not included";

					if (quickSetCsw.CxWtState.SDO.Cx)
						message += "\r\n Days of month Constraints ";
					if (quickSetCsw.CxWtState.WtPDOFS.Cx)
						message += "\r\n Partial Days Off Constraints ";
					if (quickSetCsw.CxWtState.SDO.Wt)
						message += "\r\n  Days of month Weights ";
					if (quickSetCsw.CxWtState.PDAfter.Wt)
						message += "\r\n  PDO After ";
					if (quickSetCsw.CxWtState.PDBefore.Wt)
						message += "\r\n  PDO Before ";
                        
                    UIAlertController okAlertController = UIAlertController.Create("WBidMax", message, UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);

                    quickSetCsw.CxWtState.PDBefore.Wt = false;
					quickSetCsw.CxWtState.PDAfter.Wt = false;
					quickSetCsw.CxWtState.WtPDOFS.Cx = false;
					quickSetCsw.CxWtState.SDO.Cx = false;
					quickSetCsw.CxWtState.SDO.Wt = false;
					quickSetCsw.Constraints.DaysOfMonth.OFFDays = new List<int> ();
					quickSetCsw.Constraints.DaysOfMonth.WorkDays = new List<int> ();
					quickSetCsw.Weights.SDO.isWork = false;
					quickSetCsw.Weights.SDO.Weights = new List<Wt> ();
					quickSetCsw.Weights.PDAfter = new Wt4Parameters {
						FirstValue = 1,
						SecondlValue = 180,
						ThrirdCellValue = 1,
						Weight = 0,
						lstParameters = new List<Wt4Parameter> ()
					};
					quickSetCsw.Weights.PDBefore = new Wt4Parameters {
						FirstValue = 1,
						SecondlValue = 180,
						ThrirdCellValue = 1,
						Weight = 0,
						lstParameters = new List<Wt4Parameter> ()
					};
				}

				GlobalSettings.QuickSets.QuickSetCSW = GlobalSettings.QuickSets.QuickSetCSW ?? new List<QuickSetCSW> ();
				GlobalSettings.QuickSets.QuickSetCSW.Add (quickSetCsw);
				XmlHelper.SerializeToXml (GlobalSettings.QuickSets, WBidHelper.GetQuickSetFilePath ());
			} catch (Exception ex) {
				throw ex;
			}
		}

		private void AddColumnDatatoQuickSets (string colName)
		{
			try {
				var quickSetCol = new QuickSetColumn {
					QuickSetColumnName = colName,
					SummaryNormalColumns = new List<DataColumn> (GlobalSettings.WBidINIContent.DataColumns),
					SummaryVacationColumns = new List<DataColumn> (GlobalSettings.WBidINIContent.SummaryVacationColumns),
					BidLineNormalColumns = new List<int> (GlobalSettings.WBidINIContent.BidLineNormalColumns),
					BidLineVacationColumns = new List<int> (GlobalSettings.WBidINIContent.BidLineVacationColumns),
					ModernNormalColumns = new List<int> (GlobalSettings.WBidINIContent.ModernNormalColumns),
					ModernVacationColumns = new List<int> (GlobalSettings.WBidINIContent.ModernVacationColumns)
				};
				GlobalSettings.QuickSets.QuickSetColumn = GlobalSettings.QuickSets.QuickSetColumn ?? new List<QuickSetColumn> ();
				GlobalSettings.QuickSets.QuickSetColumn.Add (quickSetCol);
				XmlHelper.SerializeToXml (GlobalSettings.QuickSets, WBidHelper.GetQuickSetFilePath ());
			} catch (Exception ex) {
				throw ex;
			}
		}

	}

	public class QuickSetTableSource : UITableViewSource
	{

		QuickSetViewController parentVC;

		public QuickSetTableSource (QuickSetViewController parent)
		{
			parentVC = parent;
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			if (parentVC.sqQuickSetToggle.SelectedSegment == 0)
				return GlobalSettings.QuickSets.QuickSetCSW.Count;
			else
				return GlobalSettings.QuickSets.QuickSetColumn.Count;
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return 40;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			NSString cellIdentifier = new NSString ("cellIdentifier");
			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier) as UITableViewCell;
			if (cell == null)
				cell = new UITableViewCell ();

			cell.Accessory = UITableViewCellAccessory.DetailButton;
			cell.SelectedBackgroundView = new UIImageView (UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)));
			cell.TextLabel.TextColor = UIColor.DarkGray;
			cell.TextLabel.HighlightedTextColor = UIColor.DarkGray;
			cell.TextLabel.Font = UIFont.SystemFontOfSize (13);

			if (parentVC.sqQuickSetToggle.SelectedSegment == 0)
				cell.TextLabel.Text = GlobalSettings.QuickSets.QuickSetCSW [indexPath.Row].QuickSetCSWName;
			else
				cell.TextLabel.Text = GlobalSettings.QuickSets.QuickSetColumn [indexPath.Row].QuickSetColumnName;

			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			if (parentVC.sqQuickSetToggle.SelectedSegment == 0) {
				parentVC.selectedCSWQset = GlobalSettings.QuickSets.QuickSetCSW [indexPath.Row].QuickSetCSWName;
			} else {
				parentVC.selectedColumnsQset = GlobalSettings.QuickSets.QuickSetColumn [indexPath.Row].QuickSetColumnName;
			}
			parentVC.EnableDisableUseDelete ();
		}

		public override void AccessoryButtonTapped (UITableView tableView, NSIndexPath indexPath)
		{
			var qsDetail = new QuickSetDetailViewController ();
			qsDetail.Type = (int)parentVC.sqQuickSetToggle.SelectedSegment;
			if (parentVC.sqQuickSetToggle.SelectedSegment == 0)
				qsDetail.QSName = GlobalSettings.QuickSets.QuickSetCSW[indexPath.Row].QuickSetCSWName;
			else
				qsDetail.QSName = GlobalSettings.QuickSets.QuickSetColumn[indexPath.Row].QuickSetColumnName;
			parentVC.NavigationController.PushViewController (qsDetail, true);
		}

	}

}

