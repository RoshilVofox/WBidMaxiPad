using System; using CoreGraphics; using Foundation; using UIKit; using WBid.WBidiPad.Model; using WBid.WBidiPad.PortableLibrary; using WBid.WBidiPad.SharedLibrary.Utility; using System.Collections.Generic; using WBid.WBidiPad.iOS.Utility; //using WBid.WBidiPad.PortableLibrary.Core; using System.Linq; using WBid.WBidiPad.Core; //using WBid.WBidiPad.iOS.Common;  namespace WBid.WBidiPad.iOS { 	public class summaryHeaderListSource : UITableViewSource 	{ 		//		string[] arrayTitle = {"Ord","ic1","ic2","ic3","Pos","Line","Points","Pay","Wts","WkEnd","A/P","Off","T234","Flt","$/Hr","VacPay","Vof","Vob","VDrop","AcChg","SEL","MOV"}; 		//		List<ColumnDefinition> columndefenition = GlobalSettings.columndefinition; 		//		List<DataColumn> datacolumn = GlobalSettings.WBidINIContent.DataColumns;  		public summaryHeaderListSource () 		{ 			if (GlobalSettings.WBidINIContent.DataColumns.Count > 18) 				GlobalSettings.WBidINIContent.DataColumns.RemoveRange (18, GlobalSettings.WBidINIContent.DataColumns.Count - 18); 			//			if (GlobalSettings.WBidINIContent.SummaryVacationColumns.Count > 18) 			//				GlobalSettings.WBidINIContent.SummaryVacationColumns.RemoveRange (18, GlobalSettings.WBidINIContent.SummaryVacationColumns.Count - 18); 		}  		public override nint NumberOfSections (UITableView tableView) 		{ 			// TODO: return the actual number of sections 			return 1; 		}  		public override nint RowsInSection (UITableView tableview, nint section) 		{ 			if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM) 				return GlobalSettings.WBidINIContent.SummaryVacationColumns.Count; 			else 				return GlobalSettings.WBidINIContent.DataColumns.Count; 		} 		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath) 		{ 			//			DataColumn dataColumn = GlobalSettings.WBidINIContent.DataColumns [indexPath.Row]; 			//			return dataColumn.Width; 			return 50; 		} 		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath) 		{ 		tableView.RegisterNibForCellReuse(UINib.FromName("summaryHeaderCell", NSBundle.MainBundle), "summaryHeaderCell");  			summaryHeaderCell cell = (summaryHeaderCell)tableView.DequeueReusableCell(new NSString("summaryHeaderCell"), indexPath); 			//cell.loadCell (cell); 			if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM) { 				DataColumn dataColumn = GlobalSettings.WBidINIContent.SummaryVacationColumns [indexPath.Row]; 				ColumnDefinition columDefinition = GlobalSettings.columndefinition.Where (x => x.Id == dataColumn.Id).FirstOrDefault (); 				cell.bindData (columDefinition, indexPath.Row); 			}  else { 				DataColumn dataColumn = GlobalSettings.WBidINIContent.DataColumns [indexPath.Row]; 				ColumnDefinition columDefinition = GlobalSettings.columndefinition.Where (x => x.Id == dataColumn.Id).FirstOrDefault (); 				cell.bindData (columDefinition, indexPath.Row); 			} 			return cell; 		} 		public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath) 		{ 			if(indexPath.Row <= 3) 				return false; 			return true; 		} 		public override NSIndexPath CustomizeMoveTarget (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath proposedIndexPath) 		{ 			if (proposedIndexPath.Row <= 3) { 				return NSIndexPath.FromRowSection (4, 0); 			}  else { 				return proposedIndexPath; 			} 		} 		public override void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath) 		{ 			if (sourceIndexPath.Row == destinationIndexPath.Row) 				UpdateDatacolumnInSourceToIndex (sourceIndexPath.Row, destinationIndexPath.Row, false); 			else 				UpdateDatacolumnInSourceToIndex (sourceIndexPath.Row, destinationIndexPath.Row, true); 			tableView.SetEditing (false, false);  		} 		public override UITableViewCellEditingStyle EditingStyleForRow (UITableView tableView, NSIndexPath indexPath) 		{ 			return UITableViewCellEditingStyle.None; 		} 		public void UpdateDatacolumnInSourceToIndex(int sourceIndex, int destinationIndex, bool reload) 		{ 			//TODO need to check why cells are not reloading perfectly. 			if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM) { 				DataColumn backupColumn = GlobalSettings.WBidINIContent.SummaryVacationColumns [sourceIndex]; 				GlobalSettings.WBidINIContent.SummaryVacationColumns.RemoveAt (sourceIndex);  				List<DataColumn> newDataColumns = new List<DataColumn> (); 				bool addedBackUpColumn = false; 				for (int i = 0; i < GlobalSettings.WBidINIContent.SummaryVacationColumns.Count; i++) { 					if (i == destinationIndex) { 						newDataColumns.Add (backupColumn); 						addedBackUpColumn = true; 					} 					newDataColumns.Add (GlobalSettings.WBidINIContent.SummaryVacationColumns [i]); 				} 				if (!addedBackUpColumn) 					newDataColumns.Add (backupColumn);  				int index = 0; 				foreach (DataColumn aDataColmn in newDataColumns) { 					aDataColmn.Order = index; 					index++; 				}  				GlobalSettings.WBidINIContent.SummaryVacationColumns = newDataColumns; 				XmlHelper.SerializeToXml (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());//Save and serialize. 				GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI> (WBidHelper.GetWBidINIFilePath ());  			}  else { 				DataColumn backupColumn = GlobalSettings.WBidINIContent.DataColumns [sourceIndex]; 				GlobalSettings.WBidINIContent.DataColumns.RemoveAt (sourceIndex);  				List<DataColumn> newDataColumns = new List<DataColumn> (); 				bool addedBackUpColumn = false; 				for (int i = 0; i < GlobalSettings.WBidINIContent.DataColumns.Count; i++) { 					if (i == destinationIndex) { 						newDataColumns.Add (backupColumn); 						addedBackUpColumn = true; 					} 					newDataColumns.Add (GlobalSettings.WBidINIContent.DataColumns [i]); 				} 				if (!addedBackUpColumn) 					newDataColumns.Add (backupColumn);  				int index = 0; 				foreach (DataColumn aDataColmn in newDataColumns) { 					aDataColmn.Order = index; 					index++; 				}  				GlobalSettings.WBidINIContent.DataColumns = newDataColumns; 				XmlHelper.SerializeToXml (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());//Save and serialize. 				GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI> (WBidHelper.GetWBidINIFilePath ());  			}  			if (reload) 				this.PerformSelector (new ObjCRuntime.Selector ("ReloadSummaryData"), null, 0.2);  		}  		public override void RowSelected (UITableView tableView, NSIndexPath indexPath) 		{ 			tableView.DeselectRow (indexPath, true); 		}  		[Export("ReloadSummaryData")] 		private void reload () 		{ 			NSNotificationCenter.DefaultCenter.PostNotificationName ("DataCulumnsUpdated", null); 		}  	} }   