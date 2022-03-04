
using System;
using System.Drawing;

using Foundation;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using WBid.WBidiPad.Core;
using System.Collections.ObjectModel;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Utility;
using System.Linq;

namespace WBid.WBidiPad.iOS
{
	public partial class QuickSetDetailViewController : UIViewController
	{
		public int Type;
		public string QSName;
		public List <QSDetailData> detailList = new  List<QSDetailData> ();

		public QuickSetDetailViewController () : base ("QuickSetDetailViewController", null)
		{
		}

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
			this.Title = "QuickSet Detail";
			
			tblQuickSetDetail.Layer.BorderWidth = 1;
			tblQuickSetDetail.Layer.BorderColor = ColorClass.SummaryHeaderBorderColor.CGColor;
			tblQuickSetDetail.SeparatorInset = new UIEdgeInsets (0, 3, 0, 3);
			tblQuickSetDetail.TableFooterView = new UIView (CGRect.Empty);
			
			detailList = GetDataList ();
			tblQuickSetDetail.Source = new QuickSetDetailTableSource (this);

		}

		private List<QSDetailData> GetDataList ()
		{
			var AllAppliedStates = new ObservableCollection<AppliedStates> ();
			var lstData = new List<QSDetailData> ();

			try {
				if (Type == 1) {
					var QuickSetColumn = GlobalSettings.QuickSets.QuickSetColumn.FirstOrDefault (x => x.QuickSetColumnName == QSName);
					QuickSetColumn.BidLineNormalColumns.RemoveAll(x => x == 58 || x==59);
					QuickSetColumn.BidLineVacationColumns.RemoveAll(x => x == 58 || x==59);
					QuickSetColumn.SummaryNormalColumns.RemoveAll(x => x.Id == 58|| x.Id==59);
					QuickSetColumn.SummaryNormalColumns.RemoveAll(x => x.Id == 58|| x.Id==59);
					QuickSetColumn.ModernNormalColumns.RemoveAll(x => x == 58|| x==59);
					QuickSetColumn.ModernVacationColumns.RemoveAll(x => x == 58|| x==59);

					QuicksetStateDetails quicksetStateDetails = new QuicksetStateDetails ();
					AllAppliedStates = quicksetStateDetails.GetSelectedColumns (QuickSetColumn);

					foreach (var states in AllAppliedStates) {
						lstData.Add (new QSDetailData (){ Type = 0, Title = states.Key, DataValue = "", RowHeight = 20 });
						foreach (var stateTypes in states.AppliedStateTypes) {
							var datValue = string.Empty;
							if (stateTypes.Value != null) {
								foreach (var value in stateTypes.Value) {
									if (datValue != string.Empty)
										datValue += "\n";
									datValue += value;
								}
							}
							lstData.Add (new QSDetailData () {
								Type = 1,
								Title = stateTypes.Key,
								DataValue = datValue,
								RowHeight = (stateTypes.Value != null && stateTypes.Value.Count > 0) ? stateTypes.Value.Count * 20 : 20
							});
						}
					}

				} else {
					var QuickSetCSW = GlobalSettings.QuickSets.QuickSetCSW.FirstOrDefault (x => x.QuickSetCSWName == QSName);
					QuicksetStateDetails quicksetStateDetails = new QuicksetStateDetails ();
					AllAppliedStates = quicksetStateDetails.GetAppliedState (QuickSetCSW);

					foreach (var states in AllAppliedStates) {
						lstData.Add (new QSDetailData (){ Type = 0, Title = states.Key, DataValue = "", RowHeight = 20 });
						foreach (var stateTypes in states.AppliedStateTypes) {
							var datValue = string.Empty;
							if (stateTypes.Value != null) {
								foreach (var value in stateTypes.Value) {
									if (datValue != string.Empty)
										datValue += "\n";
									datValue += value;
								}
							}
							lstData.Add (new QSDetailData () {
								Type = 1,
								Title = stateTypes.Key,
								DataValue = datValue,
								RowHeight = (stateTypes.Value != null && stateTypes.Value.Count > 0) ? stateTypes.Value.Count * 30 : 30
							});
						}
					}
				}
			} catch (Exception ex) {
				throw ex;
			}

			return lstData;
		}

	}

	public class QuickSetDetailTableSource : UITableViewSource
	{

		QuickSetDetailViewController parentVC;

		public QuickSetDetailTableSource (QuickSetDetailViewController parent)
		{
			parentVC = parent;
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return parentVC.detailList.Count;
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return parentVC.detailList [indexPath.Row].RowHeight;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.RegisterNibForCellReuse (UINib.FromName ("QuickSetDetailCell", NSBundle.MainBundle), "QuickSetDetailCell");
			QuickSetDetailCell cell = tableView.DequeueReusableCell (new NSString ("QuickSetDetailCell")) as QuickSetDetailCell;
			cell.BindData (parentVC.detailList [indexPath.Row], indexPath.Row);
			return cell;
		}

	}

	public class QSDetailData
	{
		public int Type { get; set; }

		public string Title { get; set; }

		public string DataValue { get; set; }

		public float RowHeight { get; set; }
	}


}

