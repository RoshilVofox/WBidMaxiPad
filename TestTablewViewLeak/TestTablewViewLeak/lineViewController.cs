using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Core;
using CoreGraphics;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.iOS;
using WBid.WBidiPad.Model;
using System.Collections.Generic;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using System.Linq;
using WBid.WBidiPad.iOS.Utility;
using System.Collections.ObjectModel;
using System.IO;
using EventKit;

//using System.Collections.ObjectModel;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.SharedLibrary.Utility;
using System.Net;

//using MiniZip.ZipArchive;

using VacationCorrection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Globalization;
using System.IO.Compression;
using WBid.WBidiPad.Core.Enum;


namespace WBid.WBidiPad.iOS
{
	public partial class lineViewController : UIViewController
	{
		class MyPopDelegate : UIPopoverControllerDelegate
		{

			lineViewController _parent;

			public MyPopDelegate (lineViewController parent)
			{
				_parent = parent;
			}

			public override void DidDismiss (UIPopoverController popoverController)
			{
				_parent.popoverController = null;
				if (_parent.sumList != null && _parent.hTable != null) {
					_parent.sumList.TableView.DeselectRow (_parent.lPath, true);
					_parent.hTable.TableView.DeselectRow (_parent.hPath, true);
				} else if (_parent.bidLineList != null) {
					_parent.bidLineList.TableView.DeselectRow (_parent.lPath, true);
				} else if (_parent.modernList != null) {
					_parent.modernList.TableView.DeselectRow (_parent.lPath, true);
				}
			}
		}

		bool FirstTime;
		NSObject confNotif;
		NSObject notif;
		CGPoint tableListOffset;
		NSIndexPath hPath, lPath;
		public NSIndexPath scrlPath;
		public summaryHeaderListController hTable;
		public SummaryViewController sumList;
		public BidLineViewController bidLineList;
		public ModernContainerViewController modernList;
		UIPopoverController popoverController;
		List<NSObject> arrObserver = new List<NSObject> ();
		CalenderPopoverController calCollection;
		TripPopListViewController tripList;
		public string tripNum;
		public string wblFileName;
		ConstraintCalculations constCalc = new ConstraintCalculations ();
		WeightCalculation weightCalc = new WeightCalculation ();
		SortCalculation sort = new SortCalculation ();
		private System.Timers.Timer timer;
		//DateTime defDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1);
		WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		LoadingOverlay syncOverlay;
		bool isNeedtoCreateMILFile = false;
		bool SynchBtn;
		//		bool undoBtn;
		//		bool redoBtn;

		public lineViewController ()
			: base ("lineViewController", null)
		{
		}
		public lineViewController(IntPtr handle) : base (handle)
		{
			CommonClass.lineVC = this;
		}
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public void UpdateSaveButton ()
		{
			btnSave.Enabled = GlobalSettings.isModified;
			GlobalSettings.isUndo = false;
			GlobalSettings.isRedo = false;
			UpdateUndoRedoButtons ();
		}
		void HandleAlertClicked (object sender, UIButtonEventArgs e)
		{

			if (e.ButtonIndex == 0) {

			}
			else if (e.ButtonIndex ==1)
			{
				SubscriptionViewController ObjSubscription = new SubscriptionViewController ();
				ObjSubscription.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				this.PresentViewController (ObjSubscription, true, null);
			}
		}
		public override void ViewDidLoad ()
		{
			

			base.ViewDidLoad ();
			CommonClass.lineVC = this;

			//UIStoryboard storyboard = UIStoryboard.FromName("LineStoryboardView", null);
		 //   sumList = storyboard.InstantiateViewController("SummaryViewController") as SummaryViewController;


			//this.vwContainerView.Hidden = true;
			this.View.LayoutIfNeeded();
			//this.vwTable.LayoutIfNeeded();
			FirstTime = true;
			GlobalSettings.isModified = false;
			Constants.listCities = GlobalSettings.WBidINIContent.Cities.Select(x => x.Name).ToList();
			//btnSave.Enabled = false;
			UpdateSaveButton ();
			lblTitle.Text = WBidCollection.SetTitile ();
			vwReparse.Hidden = true;
			setReparseView ();

//			if (GlobalSettings.WBidStateCollection != null && GlobalSettings.WBidStateCollection.DataSource == "HistoricalData")
//				btnBidStuff.Enabled = false;
			
			var wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			if (wBIdStateContent.SortDetails == null) {
				wBIdStateContent.SortDetails = new SortDetails ();
			}
			if (wBIdStateContent.SortDetails.SortColumn == "SelectedColumn") {
				string colName = wBIdStateContent.SortDetails.SortColumnName;
				string direction = wBIdStateContent.SortDetails.SortDirection;
				if (colName == "LineNum")
					colName = "LineDisplay";
				if (colName == "LastArrivalTime")
					colName = "LastArrTime";
				if (colName == "StartDowOrder")
					colName = "StartDow";

				var datapropertyId = GlobalSettings.columndefinition.FirstOrDefault (x => x.DataPropertyName == colName).Id;
				CommonClass.columnID = datapropertyId;
				if (direction == "Asc")
					CommonClass.columnAscend = true;
				else
					CommonClass.columnAscend = false;
				Console.WriteLine (datapropertyId);
			}

			this.vwCalPopover.Hidden = true;
			this.vwTripPopover.Hidden = true;

			this.btnPromote.Enabled = false;
			this.btnTrash.Enabled = false;
			//this.btnRemTopLock.Enabled = false;
			// this.btnRemBottomLock.Enabled = false;

			if (wBIdStateContent != null) {
				this.btnRemTopLock.Enabled = wBIdStateContent.TopLockCount > 0;
				this.btnRemBottomLock.Enabled = wBIdStateContent.BottomLockCount > 0;
				;
			} else {
				this.btnRemTopLock.Enabled = false;
				this.btnRemBottomLock.Enabled = false;
			}

			this.btnGrid.Selected = CommonClass.showGrid;
			/*
                        if (CommonClass.MainViewType == "Summary")
                            this.sgControlViewType.SelectedSegment = 0;
                        else if (CommonClass.MainViewType == "Bidline")
                            this.sgControlViewType.SelectedSegment = 1;
                        else if (CommonClass.MainViewType == "Modern")
                            this.sgControlViewType.SelectedSegment = 2;
                        */
			if (GlobalSettings.WBidINIContent.ViewType == 1) {
				this.sgControlViewType.SelectedSegment = 0;
				//this.loadSummaryListAndHeader();
			} else if (GlobalSettings.WBidINIContent.ViewType == 2) {
				this.sgControlViewType.SelectedSegment = 1;
			} else if (GlobalSettings.WBidINIContent.ViewType == 3) {
				this.sgControlViewType.SelectedSegment = 2;
			} else {
				GlobalSettings.WBidINIContent.ViewType = 1;
				this.sgControlViewType.SelectedSegment = 0;
			}
			loadSummaryListAndHeader ();
			Console.WriteLine ("Data loaded in summary list");

			this.btnHome.SetBackgroundImage (UIImage.FromBundle ("homeIcon.png"), UIControlState.Normal);
			this.btnSave.SetBackgroundImage (UIImage.FromBundle ("saveIconRed.png"), UIControlState.Normal);
			this.btnSave.SetBackgroundImage (UIImage.FromBundle ("saveIcon.png"), UIControlState.Disabled);
			this.btnPromote.SetBackgroundImage (UIImage.FromBundle ("topLockIcon.png"), UIControlState.Normal);
			this.btnTrash.SetBackgroundImage (UIImage.FromBundle ("bottomLockIcon.png"), UIControlState.Normal);
			this.btnGrid.SetBackgroundImage (UIImage.FromBundle ("gridIcon.png"), UIControlState.Normal);
			this.btnGrid.SetBackgroundImage (UIImage.FromBundle ("removeGridIcon.png"), UIControlState.Selected);
			this.btnRemTopLock.SetBackgroundImage (UIImage.FromBundle ("removeTopLockIcon.png"), UIControlState.Normal);
			this.btnRemBottomLock.SetBackgroundImage (UIImage.FromBundle ("removeBottomLockIcon.png"), UIControlState.Normal);
			this.btnBrightness.SetBackgroundImage (UIImage.FromBundle ("brightnessIcon.png"), UIControlState.Normal);

			this.btnOverlap.SetBackgroundImage (UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Normal);
			this.btnOverlap.SetBackgroundImage (UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Selected);
			this.btnVacCorrect.SetBackgroundImage (UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Normal);
			this.btnVacCorrect.SetBackgroundImage (UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Selected);
			this.btnVacDrop.SetBackgroundImage (UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Normal);
			this.btnVacDrop.SetBackgroundImage (UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Selected);
			//this.btnCSW.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
			//this.btnPairing.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
			this.btnEOM.SetBackgroundImage (UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Normal);
			this.btnEOM.SetBackgroundImage (UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Selected);
			this.btnMIL.SetBackgroundImage (UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Normal);
			this.btnMIL.SetBackgroundImage (UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Selected);

			//btnEOM.Enabled = false;
			// observers nsnotificationcenter notifications.
			observeNotification ();

			//            // Saving additional header colunmns.
			//            LineSummaryBL.GetAdditionalColumns();
			//
			//            // Saving additional bidline colunmns.
			//			LineSummaryBL.GetBidlineViewAdditionalColumns();
			//
			//			// Saving additional modern colunmns.
			//			LineSummaryBL.GetModernViewAdditionalColumns();
			//
			//            CommonClass.bidLineProperties = new List<string>() {
			//				"Pay",
			//				"PDiem",
			//				"Flt",
			//				"Off",
			//				"+Off"
			//			};

			// this adds pan gestures to custom popovers.
			this.addPanGestures ();

			txtGoToLine.Background = UIImage.FromBundle ("textField.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5));
			txtGoToLine.EditingDidBegin += (object sender, EventArgs e) => {
				vwCalPopover.Hidden = true;
				vwTripPopover.Hidden = true;
			};
			txtGoToLine.ShouldChangeCharacters = (textField, range, replacementString) => {
				string text = textField.Text;
				string newText = text.Substring (0, (int)range.Location) + replacementString + text.Substring ((int)range.Location + (int)range.Length);
				int val;
				if (newText == "")
					return true;
				else
					return Int32.TryParse (newText, out val);
			};
			txtGoToLine.ShouldReturn = ((textField) => {
				string text = textField.Text;
				GoToLine (text);
				return true;
			});
			btnSynch.TouchUpInside += btnSynchTapped;


			//btnOverlap.Enabled = false;
			//btnVacCorrect.Enabled = false;
			//btnVacDrop.Enabled = false;
			//btnEOM.Enabled = false;

			SetVacButtonStates ();
			SingleTapNavigation ();
			DoubleTapNavigation ();
			LongPressHandling ();
//			if (btnEOM.Selected || btnVacCorrect.Selected || btnVacDrop.Selected)
//				applyVacation ();
//			if (btnOverlap.Selected)
//				applyOverLapCorrection ();
//			if (btnEOM.Selected || btnVacCorrect.Selected || btnVacDrop.Selected)
//				applyVacation ();
//			if (btnOverlap.Selected)
//				applyOverLapCorrection ();
//			if (btnMIL.Selected)
//				applyMIL ();
			AutoSave ();
			//			GlobalSettings.WBidINIContent.User.SmartSynch = false;
			//			GlobalSettings.SynchEnable = false;
			btnUndo.TouchUpInside += btnUndoTapped;
			btnRedo.TouchUpInside += btnRedoTapped;
			//			btnUndo.SetTitle (GlobalSettings.UndoStack.Count.ToString (), UIControlState.Normal);
			//			btnRedo.SetTitle (GlobalSettings.RedoStack.Count.ToString (), UIControlState.Normal);
			//			UpdateUndoRedoButtons ();

			btnQuickSet.TouchUpInside += btnQuickSetTapped;




		}

		public void SingleTapNavigation()
		{
			UITapGestureRecognizer DownsingleTap;
			DownsingleTap = new UITapGestureRecognizer(() =>
				{
					if (GlobalSettings.WBidINIContent.ViewType == 1) {

						NSIndexPath[] arrVisibleIndex = sumList.TableView.IndexPathsForVisibleRows  ;

						if(arrVisibleIndex.Length>0)
						{
							NSIndexPath NextIndex= NSIndexPath.FromRowSection(arrVisibleIndex[0].Row +50, 0);
							if (NextIndex.Row >= GlobalSettings.Lines.Count) 
							{
								NextIndex= NSIndexPath.FromRowSection(sumList.TableView.NumberOfRowsInSection(0) -1, 0);
								sumList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
							}
							else
								sumList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);


						}
					} else if (GlobalSettings.WBidINIContent.ViewType == 2) {

						NSIndexPath[] arrVisibleIndex = bidLineList.TableView.IndexPathsForVisibleRows  ;
						if(arrVisibleIndex.Length>0)
						{

							NSIndexPath NextIndex= NSIndexPath.FromRowSection(arrVisibleIndex[0].Row +50, 0);
							if (NextIndex.Row >= GlobalSettings.Lines.Count) 
							{
								NextIndex= NSIndexPath.FromRowSection(bidLineList.TableView.NumberOfRowsInSection(0) -1, 0);
								bidLineList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
							}
							else
								bidLineList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
						}
					} else if (GlobalSettings.WBidINIContent.ViewType == 3) {
						NSIndexPath[] arrVisibleIndex = modernList.TableView.IndexPathsForVisibleRows  ;
						if(arrVisibleIndex.Length>0)
						{
							NSIndexPath NextIndex= NSIndexPath.FromRowSection(arrVisibleIndex[0].Row +50, 0);
							if (NextIndex.Row >= GlobalSettings.Lines.Count) 
							{
								NextIndex= NSIndexPath.FromRowSection(modernList.TableView.NumberOfRowsInSection(0) -1, 0);
								modernList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
							}
							else
								modernList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);

							ReloadModernViewOverlay();
						}
					}
				});

			DownsingleTap.NumberOfTapsRequired = 1;
			btnDownArrow.AddGestureRecognizer (DownsingleTap);


			UITapGestureRecognizer DownDoubleTap;
			DownDoubleTap = new UITapGestureRecognizer(() =>
				{
					if (GlobalSettings.WBidINIContent.ViewType == 1) {

						NSIndexPath[] arrVisibleIndex = sumList.TableView.IndexPathsForVisibleRows  ;

						if(arrVisibleIndex.Length>0)
						{
							NSIndexPath NextIndex= NSIndexPath.FromRowSection(arrVisibleIndex[0].Row +100, 0);
							if (NextIndex.Row >= GlobalSettings.Lines.Count) 
							{
								NextIndex= NSIndexPath.FromRowSection(sumList.TableView.NumberOfRowsInSection(0) -1, 0);
								sumList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
							}
							else
								sumList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
						}
					} else if (GlobalSettings.WBidINIContent.ViewType == 2) {

						NSIndexPath[] arrVisibleIndex = bidLineList.TableView.IndexPathsForVisibleRows  ;
						if(arrVisibleIndex.Length>0)
						{

							NSIndexPath NextIndex= NSIndexPath.FromRowSection(arrVisibleIndex[0].Row +100, 0);
							if (NextIndex.Row >= GlobalSettings.Lines.Count) 
							{
								NextIndex= NSIndexPath.FromRowSection(bidLineList.TableView.NumberOfRowsInSection(0) -1, 0);
								bidLineList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
							}
							else
								bidLineList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
						}
					} else if (GlobalSettings.WBidINIContent.ViewType == 3) {
						NSIndexPath[] arrVisibleIndex = modernList.TableView.IndexPathsForVisibleRows  ;
						if(arrVisibleIndex.Length>0)
						{
							NSIndexPath NextIndex= NSIndexPath.FromRowSection(arrVisibleIndex[0].Row +100, 0);
							if (NextIndex.Row >= GlobalSettings.Lines.Count) 
							{
								NextIndex= NSIndexPath.FromRowSection(modernList.TableView.NumberOfRowsInSection(0) -1, 0);
								modernList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
							}
							else
								modernList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
							ReloadModernViewOverlay();
						}
					}
				});

			DownDoubleTap.NumberOfTapsRequired = 2;
			btnDownArrow.AddGestureRecognizer (DownDoubleTap);

			DownsingleTap.RequireGestureRecognizerToFail (DownDoubleTap);
			}

		public void ReloadModernViewOverlay()
		{


			if (CommonClass.IsModernScrollClassic != "TRUE" && GlobalSettings.WBidINIContent.ViewType == 3) {
				
				foreach (ModernViewCell cell in	modernList.TableView.VisibleCells) {
					ModernViewControllerSource Source = (ModernViewControllerSource)modernList.TableView.Source;
					Source.dragging = false;
					Source.FastDragging = true;
					Source.LoadIndex = 0;

				}
			}  
		}
		public void ReloadModernView(NSNotification n)
		{
			if (modernList != null && GlobalSettings.WBidINIContent.ViewType == 3)
			{
				modernList.ViewDidLoad();

				vwContainerView.Hidden = false;
				vwSummaryContainer.Hidden = true;
				vwBidLineContainer.Hidden = true;

				//modernList.View.RemoveFromSuperview ();
				//modernList.RemoveFromParentViewController ();
				//modernList = null;
				//this.View.LayoutIfNeeded();



				if (scrlPath != null)
					modernList.TableView.ScrollToRow (scrlPath, UITableViewScrollPosition.Top, false);

			}
			ReloadModernViewOverlay ();
		}
		public void DoubleTapNavigation()
		{

			UITapGestureRecognizer UpsingleTap;
			UpsingleTap = new UITapGestureRecognizer(() =>
				{
					if (GlobalSettings.WBidINIContent.ViewType == 1) {

					NSIndexPath[] arrVisibleIndex = sumList.TableView.IndexPathsForVisibleRows  ;
						if(arrVisibleIndex.Length>0)
						{
							NSIndexPath NextIndex= NSIndexPath.FromRowSection(arrVisibleIndex[0].Row -50, 0);
							if (NextIndex.Row < 0) 
							{
								NextIndex= NSIndexPath.FromRowSection(0, 0);
							sumList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
							}
							else
							sumList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
						}
					} else if (GlobalSettings.WBidINIContent.ViewType == 2) {

						NSIndexPath[] arrVisibleIndex = bidLineList.TableView.IndexPathsForVisibleRows  ;
						if(arrVisibleIndex.Length>0)
						{
							NSIndexPath NextIndex= NSIndexPath.FromRowSection(arrVisibleIndex[0].Row -50, 0);
							if (NextIndex.Row < 0) 
							{
								NextIndex= NSIndexPath.FromRowSection(0, 0);
								bidLineList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
							}
							else
								bidLineList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
						}
					} else if (GlobalSettings.WBidINIContent.ViewType == 3) {

						NSIndexPath[] arrVisibleIndex = modernList.TableView.IndexPathsForVisibleRows  ;
						if(arrVisibleIndex.Length>0)
						{
							NSIndexPath NextIndex= NSIndexPath.FromRowSection(arrVisibleIndex[0].Row -50, 0);
							if (NextIndex.Row < 0) 
							{
								NextIndex= NSIndexPath.FromRowSection(0, 0);
								modernList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
							}
							else
								modernList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
							ReloadModernViewOverlay();
						}
					}
				});

			UpsingleTap.NumberOfTapsRequired = 1;
			btnUpArrow.AddGestureRecognizer (UpsingleTap);

			UITapGestureRecognizer UpDoubleTap;
			UpDoubleTap = new UITapGestureRecognizer(() =>
				{
					if (GlobalSettings.WBidINIContent.ViewType == 1) {

					NSIndexPath[] arrVisibleIndex = sumList.TableView.IndexPathsForVisibleRows  ;
						if(arrVisibleIndex.Length>0)
						{
							NSIndexPath NextIndex= NSIndexPath.FromRowSection(arrVisibleIndex[0].Row -100, 0);
							if (NextIndex.Row < 0) 
							{
								NextIndex= NSIndexPath.FromRowSection(0, 0);
								sumList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
							}
							else
								sumList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
						}
					} else if (GlobalSettings.WBidINIContent.ViewType == 2) {

						NSIndexPath[] arrVisibleIndex = bidLineList.TableView.IndexPathsForVisibleRows  ;
						if(arrVisibleIndex.Length>0)
						{
							NSIndexPath NextIndex= NSIndexPath.FromRowSection(arrVisibleIndex[0].Row -100, 0);
							if (NextIndex.Row < 0) 
							{
								NextIndex= NSIndexPath.FromRowSection(0, 0);
								bidLineList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
							}
							else
								bidLineList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
						}
					} else if (GlobalSettings.WBidINIContent.ViewType == 3) {

						NSIndexPath[] arrVisibleIndex = modernList.TableView.IndexPathsForVisibleRows  ;
						if(arrVisibleIndex.Length>0)
						{
							NSIndexPath NextIndex= NSIndexPath.FromRowSection(arrVisibleIndex[0].Row -100, 0);
							if (NextIndex.Row < 0) 
							{
								NextIndex= NSIndexPath.FromRowSection(0, 0);
								modernList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
							}
							else
								modernList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
							ReloadModernViewOverlay();
						}
					}
				});

			UpDoubleTap.NumberOfTapsRequired = 2;
			btnUpArrow.AddGestureRecognizer (UpDoubleTap);

			UpsingleTap.RequireGestureRecognizerToFail (UpDoubleTap);
		}

		public void LongPressHandling()
		{
			UILongPressGestureRecognizer longPressUp = new UILongPressGestureRecognizer(() =>
				{
					if (GlobalSettings.WBidINIContent.ViewType == 1) {


						if(GlobalSettings.Lines.Count>0)
						{
							NSIndexPath NextIndex= NSIndexPath.FromRowSection(0, 0);
							sumList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);

						}

					} else if (GlobalSettings.WBidINIContent.ViewType == 2) {

						if(GlobalSettings.Lines.Count>0)
						{
							NSIndexPath NextIndex= NSIndexPath.FromRowSection(0, 0);
							bidLineList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);

						}
					} else if (GlobalSettings.WBidINIContent.ViewType == 3) {

						if(GlobalSettings.Lines.Count>0)
						{
							
							NSIndexPath NextIndex= NSIndexPath.FromRowSection(0, 0);
							modernList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
							ReloadModernViewOverlay();
						}
					}
			});

			btnUpArrow.AddGestureRecognizer(longPressUp);
			longPressUp.DelaysTouchesBegan = true;



			UILongPressGestureRecognizer longPressDown = new UILongPressGestureRecognizer(() =>
				{
					if (GlobalSettings.WBidINIContent.ViewType == 1) {


						if(GlobalSettings.Lines.Count>0)
						{
							NSIndexPath NextIndex= NSIndexPath.FromRowSection(GlobalSettings.Lines.Count -1 , 0);
							sumList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Bottom, true);

						}

					} else if (GlobalSettings.WBidINIContent.ViewType == 2) {

						if(GlobalSettings.Lines.Count>0)
						{
							NSIndexPath NextIndex= NSIndexPath.FromRowSection(GlobalSettings.Lines.Count -1, 0);
							bidLineList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Bottom, true);

						}
					} else if (GlobalSettings.WBidINIContent.ViewType == 3) {

						if(GlobalSettings.Lines.Count>0)
						{

							NSIndexPath NextIndex= NSIndexPath.FromRowSection(GlobalSettings.Lines.Count -1, 0);
							modernList.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Bottom, true);
							ReloadModernViewOverlay();
						}
					}
				});

			btnDownArrow.AddGestureRecognizer(longPressDown);
			longPressUp.DelaysTouchesBegan = true;

		}
		void applyMIL ()
		{
			MILData milData;
			if (File.Exists (WBidHelper.MILFilePath)) {
				var loadingOverlay = new LoadingOverlay (View.Bounds, "Applying MIL. Please wait..");
				View.Add (loadingOverlay);

				InvokeInBackground (() => {
					LineInfo lineInfo = null;
					using (FileStream milStream = File.OpenRead (WBidHelper.MILFilePath)) {

						MILData milDataobject = new MILData ();
						milData = ProtoSerailizer.DeSerializeObject (WBidHelper.MILFilePath, milDataobject, milStream);
					}

					GlobalSettings.MILDates = wBIdStateContent.MILDateList;


					GlobalSettings.MILDates = GenarateOrderedMILDates (wBIdStateContent.MILDateList);
					//Apply MIL values (calculate property values including Modern bid line properties
					//==============================================

					GlobalSettings.MILData = milData.MILValue;
					GlobalSettings.MenuBarButtonStatus.IsMIL = true;

					RecalcalculateLineProperties recalcalculateLineProperties = new RecalcalculateLineProperties ();
					recalcalculateLineProperties.CalcalculateLineProperties ();

					InvokeOnMainThread (() => {
						loadingOverlay.Hide ();
						CommonClass.lineVC.SetVacButtonStates ();
						NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);

					});
				});
			}

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

		void btnQuickSetTapped (object sender, EventArgs e)
		{
			QuickSetViewController quickContent = new QuickSetViewController ();
			var navigation = new UINavigationController (quickContent);
			navigation.NavigationBar.BarTintColor = ColorClass.TopHeaderColor;
			navigation.NavigationBar.TitleTextAttributes = new UIStringAttributes (){ ForegroundColor = UIColor.White };
			navigation.NavigationBar.TintColor = UIColor.White;
			var popController = new UIPopoverController (navigation);
			popController.BackgroundColor = ColorClass.BottomHeaderColor;
			popController.PopoverContentSize = new CGSize (400, 500);
			popController.PresentFromRect (btnQuickSet.Frame, tbBottomBar, UIPopoverArrowDirection.Any, true);

		}

		void btnRedoTapped (object sender, EventArgs e)
		{
			if (GlobalSettings.RedoStack.Count > 0) {
				var state = GlobalSettings.RedoStack [0];
				bool isNeedtoRecreateMILFile = false;
				if (state.MILDateList != null && wBIdStateContent.MILDateList != null)
					isNeedtoRecreateMILFile = checkToRecreateMILFile (state.MILDateList, wBIdStateContent.MILDateList);
				StateManagement stateManagement = new StateManagement ();
				stateManagement.UpdateWBidStateContent ();

				var stateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == state.StateName);

				if (stateContent != null) {
					GlobalSettings.UndoStack.Insert (0, new WBidState (stateContent));
					GlobalSettings.WBidStateCollection.StateList.Remove (stateContent);
					GlobalSettings.WBidStateCollection.StateList.Insert (0, new WBidState (state));

				}

				GlobalSettings.RedoStack.RemoveAt (0);
				if (isNeedtoRecreateMILFile) {
					GlobalSettings.MILDates = WBidCollection.GenarateOrderedMILDates (state.MILDateList);
					GlobalSettings.MILData = CreateNewMILFile ().MILValue;

				}
				//   StateManagement stateManagement = new StateManagement();
				//stateManagement.ReloadDataFromStateFile();
			
				bool isNeedToRecalculateLineProp = stateManagement.CheckLinePropertiesNeedToRecalculate (state);
				ResetLinePropertiesBackToNormal (stateContent, state);
				ResetOverlapState (stateContent, state);

				stateManagement.SetMenuBarButtonStatusFromStateFile (state);
				//Setting  status to Global variables
				stateManagement.SetVacationOrOverlapExists (state);

				SetVacButtonStates ();

				if (isNeedToRecalculateLineProp) {
					var loadingOverlay = new LoadingOverlay (View.Bounds, "Please Wait..");
					View.Add (loadingOverlay);
					InvokeInBackground (() => {

						stateManagement.RecalculateLineProperties (state);
						InvokeOnMainThread (() => {
							loadingOverlay.Hide ();
							stateManagement.ReloadStateContent (state);
							//ReloadLineView ();
							NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);

						});
					});
				} else {

					stateManagement.ReloadStateContent (state);
					//ReloadLineView ();
					NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
				}
			}

			GlobalSettings.isUndo = false;
			GlobalSettings.isRedo = true;
			UpdateUndoRedoButtons ();
			GlobalSettings.isModified = true;
			btnSave.Enabled = GlobalSettings.isModified;

		}

		private bool checkToRecreateMILFile (List<Absense> lstPreviosusMIL, List<Absense> lstCurrentMIL)
		{
			bool isNeedtoReCreateMILFile = false;
			if (lstPreviosusMIL.Count != lstCurrentMIL.Count)
				isNeedtoReCreateMILFile = true;
			else {
				for (int count = 0; count < lstPreviosusMIL.Count; count++) {
					if (lstPreviosusMIL [count].StartAbsenceDate != lstCurrentMIL [count].StartAbsenceDate || lstPreviosusMIL [count].EndAbsenceDate != lstCurrentMIL [count].EndAbsenceDate) {
						isNeedtoReCreateMILFile = true;
						break;
					}

				}
			}
			return isNeedtoReCreateMILFile;
		}

		void btnUndoTapped (object sender, EventArgs e)
		{
			if (GlobalSettings.UndoStack.Count > 0) {
				WBidState state = GlobalSettings.UndoStack [0];
				bool isNeedtoRecreateMILFile = false;
				if (state.MILDateList != null && wBIdStateContent.MILDateList != null)
					isNeedtoRecreateMILFile = checkToRecreateMILFile (state.MILDateList, wBIdStateContent.MILDateList);
				StateManagement stateManagement = new StateManagement ();
				stateManagement.UpdateWBidStateContent ();

				var stateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == state.StateName);
				// GloabalStateList = state;
				// wBIdStateContent = state;

				if (stateContent != null) {
					GlobalSettings.RedoStack.Insert (0, new WBidState (stateContent));
					GlobalSettings.WBidStateCollection.StateList.Remove (stateContent);
					GlobalSettings.WBidStateCollection.StateList.Insert (0, new WBidState (state));

				}
				//GlobalSettings.RedoStack.Push(state);

				GlobalSettings.UndoStack.RemoveAt (0);

				if (isNeedtoRecreateMILFile) {
					GlobalSettings.MILDates = WBidCollection.GenarateOrderedMILDates (state.MILDateList);
					GlobalSettings.MILData = CreateNewMILFile ().MILValue;

				}
				bool isNeedToRecalculateLineProp = stateManagement.CheckLinePropertiesNeedToRecalculate (state);
				ResetLinePropertiesBackToNormal (stateContent, state);
				ResetOverlapState (stateContent, state);

				//Setting Button status to Global variables
				stateManagement.SetMenuBarButtonStatusFromStateFile (state);
				//Setting  status to Global variables
				stateManagement.SetVacationOrOverlapExists (state);
               
				SetVacButtonStates ();
				if (isNeedToRecalculateLineProp) {
					var loadingOverlay = new LoadingOverlay (View.Bounds, "Please Wait..");
					View.Add (loadingOverlay);
					InvokeInBackground (() => {
					
						stateManagement.RecalculateLineProperties (state);


						InvokeOnMainThread (() => {
							loadingOverlay.Hide ();
							stateManagement.ReloadStateContent (state);
							//ReloadLineView ();
							NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);

						});
					});
				} else {
					stateManagement.ReloadStateContent (state);

					// stateManagement.ReloadDataFromStateFile();

					//ReloadLineView ();
					NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
				}
			}

			GlobalSettings.isUndo = true;
			GlobalSettings.isRedo = false;
			UpdateUndoRedoButtons ();
			GlobalSettings.isModified = true;
			btnSave.Enabled = GlobalSettings.isModified;
		}

		public void UpdateUndoRedoButtons ()
		{
			btnUndo.SetTitle (GlobalSettings.UndoStack.Count.ToString (), UIControlState.Normal);
			btnRedo.SetTitle (GlobalSettings.RedoStack.Count.ToString (), UIControlState.Normal);

			if (GlobalSettings.isUndo)
				btnUndo.SetBackgroundImage (UIImage.FromBundle ("undoOrange.png"), UIControlState.Normal);
			else
				btnUndo.SetBackgroundImage (UIImage.FromBundle ("undoGreen.png"), UIControlState.Normal);

			if (GlobalSettings.isRedo)
				btnRedo.SetBackgroundImage (UIImage.FromBundle ("redoOrange.png"), UIControlState.Normal);
			else
				btnRedo.SetBackgroundImage (UIImage.FromBundle ("redoGreen.png"), UIControlState.Normal);

			if (GlobalSettings.UndoStack.Count == 0) {
				btnUndo.SetBackgroundImage (UIImage.FromBundle ("undoGreen.png"), UIControlState.Normal);
				btnUndo.SetTitle ("", UIControlState.Normal);
				btnUndo.Enabled = false;
			} else {
				//btnRedo.SetBackgroundImage (UIImage.FromBundle ("undoGreen.png"), UIControlState.Normal);
				btnUndo.SetTitle (GlobalSettings.UndoStack.Count.ToString (), UIControlState.Normal);
				btnUndo.Enabled = true;
			}

			if (GlobalSettings.RedoStack.Count == 0) {
				btnRedo.SetBackgroundImage (UIImage.FromBundle ("redoGreen.png"), UIControlState.Normal);
				btnRedo.SetTitle ("", UIControlState.Normal);
				btnRedo.Enabled = false;
			} else {
				//btnRedo.SetBackgroundImage (UIImage.FromBundle ("redoGreen.png"), UIControlState.Normal);
				btnRedo.SetTitle (GlobalSettings.RedoStack.Count.ToString (), UIControlState.Normal);
				btnRedo.Enabled = true;
			}

		}

		/// <summary>
		/// This will save the current bid state automatically dependes on the Settings in the Configuration=>user tab
		/// </summary>
		public void AutoSave ()
		{
			if (GlobalSettings.WBidINIContent.User.AutoSave)
			{
				//GlobalSettings.WBidINIContent.User.AutoSavevalue=1;
				timer = new System.Timers.Timer (GlobalSettings.WBidINIContent.User.AutoSavevalue * 60000)
				{
					
					Interval = GlobalSettings.WBidINIContent.User.AutoSavevalue * 60000,
					Enabled = true
				};
				timer.Elapsed += timer_Elapsed;
			}
		}
	

		void btnSynchTapped (object sender, EventArgs e)
		{
			//			StateManagement stateManagement = new StateManagement();
			//			stateManagement.UpdateWBidStateContent();
			////			CompareState stateObj = new CompareState();
			////			string fileName = WBidHelper.GenerateFileNameUsingCurrentBidDetails();
			////			var WbidCollection = XmlHelper.ReadStateFile(WBidHelper.GetAppDataPath() + "/" + fileName + ".WBS");
			////			wBIdStateContent = WbidCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			////			bool isNochange = stateObj.CompareStateChange(wBIdStateContent, GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName));
			//
			//			if(GlobalSettings.isModified)
			//			{
			//				GlobalSettings.WBidStateCollection.IsModified = true;
			//				WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);
			//
			//				if (timer != null)
			//				{
			//					timer.Stop();
			//					timer.Start();
			//				}
			//				//save the state of the INI File
			//				WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
			//
			//				GlobalSettings.isModified = false;
			//				btnSave.Enabled = false;
			//			}



			if (GlobalSettings.WBidINIContent.User.IsSouthWestWifiTest == false )
			{
			if(Reachability.IsHostReachable (GlobalSettings.ServerUrl))
			{
				if (!GlobalSettings.SynchEnable) 
				{
					UIAlertView syAlert = new UIAlertView ("Smart Sync", "Please enable Smart synchronisation from Configuration Settings", null, "OK", null);
					syAlert.Show ();
				} 
				else if (GlobalSettings.isModified) {
					UIAlertView syAlert = new UIAlertView ("Smart Sync", "Please save the current state before performing synch.", null, "OK", null);
					syAlert.Show ();
				} 
				else 
				{
					SynchBtn = true;
					Synch ();
				
				}
				}
				else
				{


				if (WBidHelper.IsSouthWestWifi()) {
						UIAlertView syAlert = new UIAlertView ("Smart Sync", GlobalSettings.SouthWestWifiMessage, null, "OK", null);
						syAlert.Show ();

					} else {

						UIAlertView alertVW = new UIAlertView ("Smart Sync", "Connectivity not available", null, "OK", null);
						alertVW.Show ();
					}
				}
			} 
			else
			{

				UIAlertView syAlert = new UIAlertView ("Smart Sync", GlobalSettings.SouthWestWifiMessage, null, "OK", null);
				syAlert.Show ();
			}
		}

		private void timer_Elapsed (object sender, System.Timers.ElapsedEventArgs e)
		{
			StateManagement stateManagement = new StateManagement ();
			stateManagement.UpdateWBidStateContent ();
			GlobalSettings.WBidStateCollection.IsModified = true;
			WBidHelper.SaveStateFile (WBidHelper.WBidStateFilePath);
			//save the state of the INI File
			WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());
			GlobalSettings.isModified = false;
			InvokeOnMainThread (() => {
				if (CommonClass.lineVC != null)
					CommonClass.lineVC.UpdateSaveButton ();
				if (CommonClass.cswVC != null)
					CommonClass.cswVC.UpdateSaveButton ();
			});
		}

		private void setReparseView ()
		{
			vwReparse.Layer.BorderWidth = 1;
			vwReparse.Layer.BorderColor = ColorClass.SummaryHeaderBorderColor.CGColor;
			btnReparseCheck.TouchUpInside += (object sender, EventArgs e) => {
				((UIButton)sender).Selected = !((UIButton)sender).Selected;
				if (btnReparseCheck.Selected) {
					GlobalSettings.IsDifferentUser = true;
					GlobalSettings.ModifiedEmployeeNumber = txtReparse.Text;
				} else {
					GlobalSettings.IsDifferentUser = false;
				}
			};
			txtReparse.ShouldChangeCharacters = (textField, range, replacementString) => {
				string text = textField.Text;
				string newText = text.Substring (0, (int)range.Location) + replacementString + text.Substring ((int)range.Location + (int)range.Length);
				int val;
				if (newText == "")
					return true;
				else
					return Int32.TryParse (newText, out val);
			};
			btnReparse.TouchUpInside += (object sender, EventArgs e) => {
				txtReparse.ResignFirstResponder ();
				UIAlertView alert = new UIAlertView ("WBidMax", "Do you want to test the vacation correction?", null, "NO", new string[] { "Yes" });
				alert.Show ();
				alert.Clicked += handleReparse;
			};
			btnReparseClose.TouchUpInside += (object sender, EventArgs e) => {
				txtReparse.ResignFirstResponder ();
				vwReparse.Hidden = true;
			};


		}

		void handleReparse (object sender, UIButtonEventArgs e)
		{

			if (btnReparseCheck.Selected) {
				GlobalSettings.IsDifferentUser = true;
				GlobalSettings.ModifiedEmployeeNumber = txtReparse.Text;
			}
			if (e.ButtonIndex == 0) {
				var loadingOverlay = new LoadingOverlay (View.Bounds, "Reparsing..Please Wait..");
				View.Add (loadingOverlay);
				InvokeInBackground (() => {
					string zipFilename = WBidHelper.GenarateZipFileName ();
					ReparseParameters reparseParams = new ReparseParameters () { ZipFileName = zipFilename };
					ReparseBL.ReparseTripAndLineFiles (reparseParams);
					InvokeOnMainThread (() => {
						loadingOverlay.Hide ();
					});
				});
			} else if (e.ButtonIndex == 1) {
				TestVacationViewController testVacVC = new TestVacationViewController ();
				testVacVC.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				this.PresentViewController (testVacVC, true, null);
				//testVacVC.View.Superview.BackgroundColor = UIColor.Clear;
				//testVacVC.View.Frame = new RectangleF(0, 100, 540, 420);
				//testVacVC.View.Layer.BorderWidth = 1;
			}
		}

		public void SetVacButtonStates ()
		{
//			foreach (var column in GlobalSettings.AdditionalColumns) {
//				column.IsSelected = false;
//			}
//			var selectedColumns = GlobalSettings.AdditionalColumns.Where (x => GlobalSettings.WBidINIContent.DataColumns.Any (y => y.Id == x.Id)).ToList ();
//			foreach (var selectedColumn in selectedColumns) {
//				selectedColumn.IsSelected = true;
//			}
//
//			foreach (var column in GlobalSettings.AdditionalvacationColumns) {
//				column.IsSelected = false;
//			}
//			var selectedVColumns = GlobalSettings.AdditionalColumns.Where (x => GlobalSettings.WBidINIContent.SummaryVacationColumns.Any (y => y.Id == x.Id)).ToList ();
//			foreach (var selectedColumn in selectedVColumns) {
//				selectedColumn.IsSelected = true;
//			}

			// Configuring Modern view property lists.
			if (GlobalSettings.MenuBarButtonStatus == null)
				GlobalSettings.MenuBarButtonStatus = new MenuBarButtonStatus ();
             
			CommonClass.bidLineProperties = new List<string> ();
			CommonClass.modernProperties = new List<string> ();

			if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM) {
				foreach (var item in GlobalSettings.WBidINIContent.BidLineVacationColumns) {
					var col = GlobalSettings.BidlineAdditionalvacationColumns.FirstOrDefault (x => x.Id == item);
					if (col != null)
						CommonClass.bidLineProperties.Add (col.DisplayName);
				}
				foreach (var item in GlobalSettings.WBidINIContent.ModernVacationColumns) {
					var col = GlobalSettings.ModernAdditionalvacationColumns.FirstOrDefault (x => x.Id == item);
					if (col != null)
						CommonClass.modernProperties.Add (col.DisplayName);
				}

			} else {
				foreach (var item in GlobalSettings.WBidINIContent.BidLineNormalColumns) {
					var col = GlobalSettings.BidlineAdditionalColumns.FirstOrDefault (x => x.Id == item);
					if (col != null)
						CommonClass.bidLineProperties.Add (col.DisplayName);
				}
				foreach (var item in GlobalSettings.WBidINIContent.ModernNormalColumns) {
					var col = GlobalSettings.ModernAdditionalColumns.FirstOrDefault (x => x.Id == item);
					if (col != null)
						CommonClass.modernProperties.Add (col.DisplayName);
				}

			}



			if (GlobalSettings.IsOverlapCorrection) {
				btnOverlap.Enabled = (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsEOM && !GlobalSettings.MenuBarButtonStatus.IsMIL);

			} else {
				btnOverlap.Enabled = false;
				GlobalSettings.MenuBarButtonStatus.IsOverlap = false;
			}

			if (GlobalSettings.IsVacationCorrection) {
				btnVacCorrect.Enabled = (!GlobalSettings.MenuBarButtonStatus.IsOverlap && !GlobalSettings.MenuBarButtonStatus.IsMIL);

			} else {
				btnVacCorrect.Enabled = false;

			}



			btnEOM.Enabled = (!GlobalSettings.MenuBarButtonStatus.IsOverlap && !GlobalSettings.MenuBarButtonStatus.IsMIL && (GlobalSettings.CurrentBidDetails.Postion == "FA" || (int)GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays (1).DayOfWeek == 0 || (int)GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays (2).DayOfWeek == 0 || (int)GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays (3).DayOfWeek == 0));


			btnVacDrop.Enabled = (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM);

			btnMIL.Enabled = (!GlobalSettings.MenuBarButtonStatus.IsOverlap && !GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsEOM);

			if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsEOM) {
				GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
			}


			btnEOM.Selected = GlobalSettings.MenuBarButtonStatus.IsEOM;
			btnVacCorrect.Selected = GlobalSettings.MenuBarButtonStatus.IsVacationCorrection;
			btnVacDrop.Selected = GlobalSettings.MenuBarButtonStatus.IsVacationDrop;
			btnOverlap.Selected = GlobalSettings.MenuBarButtonStatus.IsOverlap;
			btnMIL.Selected = GlobalSettings.MenuBarButtonStatus.IsMIL;

			//if (GlobalSettings.WBidStateCollection != null && GlobalSettings.WBidStateCollection.DataSource == "HistoricalData") {
			//	btnEOM.Enabled = false;
			//	btnVacDrop.Enabled = false;
			//	btnMIL.Enabled = false;
			//	btnVacCorrect.Enabled = false;
			//	btnOverlap.Enabled = false;
			//}
			if (GlobalSettings.WBidStateCollection != null && GlobalSettings.WBidStateCollection.DataSource == "HistoricalData") {
				if (GlobalSettings.WBidStateCollection != null) {
					if (GlobalSettings.WBidStateCollection.DataSource == "HistoricalData") {
						btnEOM.Enabled = false;
						btnVacDrop.Enabled = false;
						btnMIL.Enabled = false;
						btnVacCorrect.Enabled = false;
						btnOverlap.Enabled = false;
					}
					WBidState WBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
					if (wBIdStateContent.IsMissingTripFailed) {
						btnMIL.Enabled = false;
					}
				}

			}
		}
		// Show Cover Letter and news here!
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
//			GlobalSettings.IsNewsShow = true;
			if (GlobalSettings.IsNewsShow) {
				GlobalSettings.IsNewsShow = false;
				InvokeOnMainThread (() => {
					webPrint fileViewer = new webPrint ();
					this.PresentViewController (fileViewer, true, () => {
						fileViewer.LoadPDFdocument ("news.pdf");
					});
				});
			} else if (GlobalSettings.IsCoverletterShowFileName != string.Empty) {

				string coverLetter = GlobalSettings.IsCoverletterShowFileName;
				InvokeOnMainThread (() => {
					webPrint fileViewer = new webPrint ();
					this.PresentViewController (fileViewer, true, () => {
						fileViewer.loadFileFromUrl (coverLetter);
					});
				});
				GlobalSettings.IsCoverletterShowFileName = string.Empty;
			} else if (FirstTime) {
				FirstTime = false;
				Synch ();
				bool isSubScriptionOnlyFor5Days=	CommonClass.isSubScriptionOnlyFor5Days ();
				bool IsUserdataAvailable=	CommonClass.isUserInformationAvailable ();
				if (IsUserdataAvailable) {
					if (GlobalSettings.WbidUserContent.UserInformation.IsYearlySubscribed || GlobalSettings.WbidUserContent.UserInformation.IsMonthlySubscribed)
						return;
					
					DateTime PaidUntilDate = GlobalSettings.WbidUserContent.UserInformation.PaidUntilDate ?? DateTime.Now;
					int day = CommonClass.DaysBetween (DateTime.Now, PaidUntilDate);
					if (isSubScriptionOnlyFor5Days) {
						string message = "";
						if (day == 1)
							message = "Your subscription will expire in 1 day";
						else
							message = "Your subscription will expire in " + day + " days.";

						UIAlertView alertSubscription = new UIAlertView ("WBidMax", message, null, null, new string[] {
							"OK",
							"Go To Subscription"
						});
						alertSubscription.Show ();
						alertSubscription.Dismissed += HandleAlertClicked;
					} else if (day < 1) {
						UIAlertView alertSubscription = new UIAlertView ("WBidMax", "Your subscription expired", null, null, new string[] {
							"OK",
							"Go To Subscription"
						});
						alertSubscription.Show ();
						alertSubscription.Dismissed += HandleAlertClicked;
					}
				}
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			btnSave.Enabled = GlobalSettings.isModified;
			UpdateUndoRedoButtons ();
			if (GlobalSettings.CurrentBidDetails.Postion != "FA")
				btnMIL.Hidden = !GlobalSettings.WBidINIContent.User.MIL;
			else
				btnMIL.Hidden = true;
			SetVacButtonStates ();
		}

		private void applyOverLapCorrection ()
		{
			string overlayTxt = string.Empty;
			ReCalculateLinePropertiesForOverlapCorrection reCalculateLinePropertiesForOverlapCorrection = new ReCalculateLinePropertiesForOverlapCorrection ();
			overlayTxt = "Applying Overlap Correction";

			SetVacButtonStates ();

			LoadingOverlay overlay = new LoadingOverlay (this.View.Frame, overlayTxt);
			this.View.Add (overlay);
			InvokeInBackground (() => {
				try {
					reCalculateLinePropertiesForOverlapCorrection.ReCalculateLinePropertiesOnOverlapCorrection (GlobalSettings.Lines.ToList (), true);
					SortLineList ();
				} catch (Exception ex) {
					InvokeOnMainThread (() => {
						throw ex;
					});
				}

				InvokeOnMainThread (() => {
					overlay.Hide ();
					NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);

					CommonClass.lineVC.UpdateSaveButton ();
				});
			});
		}

		private void applyVacation ()
		{
			try {
				var str = string.Empty;
				if (btnEOM.Selected)
					str = "Applying EOM";
				else
					str = "Applying Vacation Correction";

				LoadingOverlay overlay = new LoadingOverlay (this.View.Frame, str);
				this.View.Add (overlay);
				InvokeInBackground (() => {
					try {
						WBidCollection.GenarateTempAbsenceList ();
						PrepareModernBidLineView prepareModernBidLineView = new PrepareModernBidLineView ();
						RecalcalculateLineProperties RecalcalculateLineProperties = new PortableLibrary.BusinessLogic.RecalcalculateLineProperties ();
						prepareModernBidLineView.CalculatebidLinePropertiesforVacation ();
						RecalcalculateLineProperties.CalcalculateLineProperties ();
					} catch (Exception ex) {
						InvokeOnMainThread (() => {
							throw ex;
						});
					}

					InvokeOnMainThread (() => {
						NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
						overlay.RemoveFromSuperview ();
					});
				});
			} catch (Exception ex) {

				throw ex;

			}
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			NSNotificationCenter.DefaultCenter.PostNotificationName ("CalPopHide", null);
		}

		private void GoToLine (string line)
		{
			this.vwCalPopover.Hidden = true;
			this.vwTripPopover.Hidden = true;

			int lineNo = Convert.ToInt32 (line);
			foreach (Line ln in GlobalSettings.Lines) {
				int index;
				if (ln.LineNum == lineNo) {
					index = GlobalSettings.Lines.IndexOf (ln);
					lPath = NSIndexPath.FromRowSection (index, 0);
				}
			}
			if (lPath != null) {
				if (sumList != null) {
					sumList.TableView.SelectRow (lPath, true, UITableViewScrollPosition.None);
					sumList.TableView.ScrollToRow (lPath, UITableViewScrollPosition.None, true);
				} else if (bidLineList != null) {
					bidLineList.TableView.SelectRow (lPath, true, UITableViewScrollPosition.None);
					bidLineList.TableView.ScrollToRow (lPath, UITableViewScrollPosition.None, true);
				} else if (modernList != null) {
					modernList.TableView.SelectRow (lPath, true, UITableViewScrollPosition.None);
					modernList.TableView.ScrollToRow (lPath, UITableViewScrollPosition.None, true);
					ReloadModernViewOverlay ();
				}
			}
		}

		private void addPanGestures ()
		{
			var dragCal = new UIPanGestureRecognizer (handleCalPopPan);
			dragCal.MinimumNumberOfTouches = 1;
			dragCal.MaximumNumberOfTouches = 1;
			this.vwCalPopover.AddGestureRecognizer (dragCal);

			var dragTrip = new UIPanGestureRecognizer (handleTripPopPan);
			dragTrip.MinimumNumberOfTouches = 1;
			dragTrip.MaximumNumberOfTouches = 1;
			this.vwTripPopover.AddGestureRecognizer (dragTrip);

			var swipeCal1 = new UISwipeGestureRecognizer (handleCalSwipeDown);
			swipeCal1.NumberOfTouchesRequired = 1;
			swipeCal1.Direction = UISwipeGestureRecognizerDirection.Down;
			this.vwCalChild.AddGestureRecognizer (swipeCal1);

			var swipeCal2 = new UISwipeGestureRecognizer (handleCalSwipeUp);
			swipeCal2.NumberOfTouchesRequired = 1;
			swipeCal2.Direction = UISwipeGestureRecognizerDirection.Up;
			this.vwCalChild.AddGestureRecognizer (swipeCal2);

			dragCal.RequireGestureRecognizerToFail (swipeCal1);
			dragCal.RequireGestureRecognizerToFail (swipeCal2);

		}

		public void handleCalPopPan (UIPanGestureRecognizer gest)
		{
			if (gest.State == UIGestureRecognizerState.Began || gest.State == UIGestureRecognizerState.Changed) {
				CGPoint trans = gest.TranslationInView (this.vwTable);
				CGPoint newCenter = new CGPoint (this.vwCalPopover.Center.X + trans.X, this.vwCalPopover.Center.Y + trans.Y);
				float xMin = (float)vwTable.Bounds.X + (float)vwCalPopover.Frame.Width / 2;
				float xMax = (float)vwTable.Bounds.Width - (float)vwCalPopover.Frame.Width / 2;
				float yMin = (float)vwTable.Bounds.Y + (float)vwCalPopover.Frame.Height / 2;
				float yMax = (float)vwTable.Bounds.Height - (float)vwCalPopover.Frame.Height / 2;
				bool inside = (newCenter.Y >= yMin && newCenter.Y <= yMax && newCenter.X >= xMin && newCenter.X <= xMax);
				if (inside)
					this.vwCalPopover.Center = new CGPoint (this.vwCalPopover.Center.X + trans.X, this.vwCalPopover.Center.Y + trans.Y);
				gest.SetTranslation (CGPoint.Empty, this.vwTable);
			}
		}

		public void handleTripPopPan (UIPanGestureRecognizer gest)
		{
			if (gest.State == UIGestureRecognizerState.Began || gest.State == UIGestureRecognizerState.Changed) {
				CGPoint trans = gest.TranslationInView (this.vwTable);
				CGPoint newCenter = new CGPoint (this.vwTripPopover.Center.X + trans.X, this.vwTripPopover.Center.Y + trans.Y);
				float xMin = (float)vwTable.Bounds.X + (float)vwTripPopover.Frame.Width / 2;
				float xMax = (float)vwTable.Bounds.Width - (float)vwTripPopover.Frame.Width / 2;
				float yMin = (float)vwTable.Bounds.Y + (float)vwTripPopover.Frame.Height / 2;
				float yMax = (float)vwTable.Bounds.Height - (float)vwTripPopover.Frame.Height / 2;
				bool inside = (newCenter.Y >= yMin && newCenter.Y <= yMax && newCenter.X >= xMin && newCenter.X <= xMax);
				if (inside)
					this.vwTripPopover.Center = new CGPoint (this.vwTripPopover.Center.X + trans.X, this.vwTripPopover.Center.Y + trans.Y);
				gest.SetTranslation (CGPoint.Empty, this.vwTable);
			}
		}

		public void handleCalSwipeDown (UISwipeGestureRecognizer gest)
		{
			if (gest.State == UIGestureRecognizerState.Ended) {
				if (lPath.Row != GlobalSettings.Lines.Count - 1)
					moveDown ();
			}
		}

		public void handleCalSwipeUp (UISwipeGestureRecognizer gest)
		{
			if (gest.State == UIGestureRecognizerState.Ended) {
				if (lPath.Row != 0)
					moveUp ();
			}
		}

		partial void btnTripExportTapped (UIKit.UIButton sender)
		{
			if (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO") {
				UIActionSheet sheet = new UIActionSheet ("Select", null, null, null, new string[] {
					"Export to Calendar",
					"Export to FFDO"
				});
				sheet.ShowFrom (((UIButton)sender).Frame, vwTripPopover, true);
				sheet.Clicked += (object senderSheet, UIButtonEventArgs e) => {
					if (e.ButtonIndex == 0) {
						ExportTripDetails (tripNum, CommonClass.selectedLine, CommonClass.isLastTrip);
					} else {
						//FFDO
						string ffdoData = GetFlightDataforFFDB (tripNum, CommonClass.isLastTrip);
						UIPasteboard clipBoard = UIPasteboard.General;
						clipBoard.String = ffdoData;
					}
				};
			} else {
				ExportTripDetails (tripNum, CommonClass.selectedLine, CommonClass.isLastTrip);
			}
		}

		/// <summary>
		/// Genarate FFDO Data for a line
		/// </summary>
		/// <returns></returns>
		private string GenarateFFDOforLine ()
		{
			string result = string.Empty;
			Line selectedline = GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == CommonClass.selectedLine);
			if (selectedline != null) {
				bool isLastTrip = false;
				int paringCount = 0;
				foreach (string pairing in selectedline.Pairings) {
					isLastTrip = ((selectedline.Pairings.Count - 1) == paringCount);
					paringCount++;
					result += GetFlightDataforFFDB (pairing, isLastTrip);
				}
			}
			return result;
		}

		/// <summary>
		/// PURPOSE : Get Flight data for FFDB
		/// </summary>
		/// <param name="trip"></param>
		/// <param name="tripName"></param>
		private string GetFlightDataforFFDB (string tripNum, bool isLastTrip)
		{

			Trip trip = GlobalSettings.Trip.FirstOrDefault (x => x.TripNum == tripNum.Substring (0, 4));
			trip = trip ?? GlobalSettings.Trip.FirstOrDefault (x => x.TripNum == tripNum);
			string result = string.Empty;

			// var tripStartDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, int.Parse(SelectedDay.Replace(" ", "")));
			DateTime dutPeriodDate = WBidCollection.SetDate (int.Parse (tripNum.Substring (4, 2)), isLastTrip, GlobalSettings.CurrentBidDetails);
			//DateTime dutPeriodDate = SelectedTripStartDate;

			foreach (var dp in trip.DutyPeriods) {
				string datestring = dutPeriodDate.ToString ("MM'/'dd'/'yyyy");
				if (trip.ReserveTrip) {

					result += datestring + "  RSRV " + trip.DepSta + " " + Helper.CalcTimeFromMinutesFromMidnight (Convert.ToString (dp.ReserveOut % 1440)).Replace (":", "") + " " + trip.RetSta + " " + Helper.CalcTimeFromMinutesFromMidnight (Convert.ToString (dp.ReserveIn % 1440)).Replace (":", "") + " \n";
				} else {
					foreach (var flt in dp.Flights) {
						result += datestring + " " + flt.FltNum.ToString ().PadLeft (4, '0') + " " + flt.DepSta + " " + Helper.CalcTimeFromMinutesFromMidnight (flt.DepTime.ToString ()).Replace (":", "") + " " + flt.ArrSta + " " + Helper.CalcTimeFromMinutesFromMidnight (flt.ArrTime.ToString ()).Replace (":", "") + " \n";
					}
				}
				dutPeriodDate = dutPeriodDate.AddDays (1);
			}
			return result;
		}


		private void CalPrint ()
		{
			var content = string.Empty;
			content += "---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------\n";
			var line = GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == CommonClass.selectedLine);

			content += "Line " + line.LineNum + "\t ";
			content += CommonClass.bidLineProperties [0] + "\t " + GetLineProperty (CommonClass.bidLineProperties [0], line) + "\t";

			foreach (var item in line.BidLineTemplates) {
				if (item.Date.Day.ToString ().Length == 1)
					content += item.Date.Day.ToString () + "   ";
				else
					content += item.Date.Day.ToString () + "  ";
			}
			content += "\n" + "\t ";
			content += CommonClass.bidLineProperties [1] + "\t " + GetLineProperty (CommonClass.bidLineProperties [1], line) + "\t";

			foreach (var item in line.BidLineTemplates) {
				content += item.Date.DayOfWeek.ToString ().Substring (0, 2).ToUpper () + "  ";
			}
			content += "\n" + "\t ";
			content += CommonClass.bidLineProperties [2] + "\t " + GetLineProperty (CommonClass.bidLineProperties [2], line) + "\t";

			foreach (var item in line.BidLineTemplates) {
				if (string.IsNullOrEmpty (item.TripNum))
					content += "*" + "   ";
				else
					content += item.TripNum + " ";
			}
			content += "\n" + "\t ";
			content += CommonClass.bidLineProperties [3] + "\t " + GetLineProperty (CommonClass.bidLineProperties [3], line) + "\t";

			foreach (var item in line.BidLineTemplates) {
				if (string.IsNullOrEmpty (item.ArrStaLastLeg))
					content += "*" + "   ";
				else
					content += item.ArrStaLastLeg + " ";
			}
			content += "\n" + "\t ";
			content += CommonClass.bidLineProperties [4] + "\t " + GetLineProperty (CommonClass.bidLineProperties [4], line) + "\t";

			content += line.Pairingdesription;
			content += "\n";
			content += "---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------\n\n";

			foreach (var item in line.BidLineTemplates) {
				if (!string.IsNullOrEmpty (item.TripNum)) {
					CorrectionParams correctionParams = new Model.CorrectionParams ();
					correctionParams.selectedLineNum = CommonClass.selectedLine;
					ObservableCollection<TripData> trip = TripViewBL.GenerateTripDetails (item.TripName, correctionParams, false);
					foreach (var tr in trip) {
						content += tr.Content + "\n";
					}
					content += "---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------\n\n";
				}
			}
			//			content += CommonClass.bidLineProperties [0] + "\t " + GetLineProperty (CommonClass.bidLineProperties [0], line)+"\n";
			//			content += CommonClass.bidLineProperties [1] + "\t " + GetLineProperty (CommonClass.bidLineProperties [1], line)+"\n";
			//			content += CommonClass.bidLineProperties [2] + "\t " + GetLineProperty (CommonClass.bidLineProperties [2], line)+"\n";
			//			content += CommonClass.bidLineProperties [3] + "\t " + GetLineProperty (CommonClass.bidLineProperties [3], line)+"\n";
			//			content += CommonClass.bidLineProperties [4] + "\t " + GetLineProperty (CommonClass.bidLineProperties [4], line)+"\n";

			var attributes = new UIStringAttributes () { Font = UIFont.FromName ("Courier", 5) };
			var printDoc = new NSAttributedString (content, attributes);

			//			var textVW = new UITextView(this.View.Frame);
			//			textVW.AttributedText = printDoc;
			//			this.Add(textVW);
			//
			//			return;

			var printInfo = UIPrintInfo.PrintInfo;
			printInfo.OutputType = UIPrintInfoOutputType.General;
			printInfo.JobName = "My Calendar Print Job";

			var textFormatter = new UISimpleTextPrintFormatter (printDoc) {
				StartPage = 0,
				ContentInsets = new UIEdgeInsets (5, 5, 5, 5),
				//MaximumContentWidth = 6 * 72,
			};

			var printer = UIPrintInteractionController.SharedPrintController;
			printer.PrintInfo = printInfo;
			printer.PrintFormatter = textFormatter;
			printer.ShowsPageRange = true;
			printer.PresentFromRectInView (btnCalPrint.Frame, vwCalPopover, true, (handler, completed, err) => {
				if (!completed && err != null) {
					Console.WriteLine ("error");
				} else if (completed) {
					new UIAlertView ("WBidMax",
						"The line has been sent for printout.", null,
						"OK", null).Show ();
				}
			});
		}

		private void CalFormatPrint ()
		{
			var content = string.Empty;
			var line = GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == CommonClass.selectedLine);

			string heading = string.Empty;
			//May-2015-BWI CP Line14

			if (GlobalSettings.CurrentBidDetails != null) {
				heading = heading + 
					CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(GlobalSettings.CurrentBidDetails.Month) + " " + GlobalSettings.CurrentBidDetails.Domicile + " " + GlobalSettings.CurrentBidDetails.Postion;
			}
			if (line != null) {
				heading += " Line " + line.LineNum;
			}

			string InitalContent = "<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">\r\n<html>\r\n<head>\r\n  <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">\r\n  <meta http-equiv=\"Content-Style-Type\" content=\"text/css\">\r\n  <title></title>\r\n  <meta name=\"Generator\" content=\"Cocoa HTML Writer\">\r\n  <meta name=\"CocoaVersion\" content=\"1347.57\">\r\n  <style type=\"text/css\">\r\n    p.p1 {margin: 0.0px 0.0px 1.0px 0.0px; text-align: center; font: 12.0px Arial; color: #2e3e44; -webkit-text-stroke: #2e3e44}\r\n    p.p2 {margin: 0.0px 0.0px 1.0px 0.0px;  font: 12.0px Arial; color: #2e3e44; -webkit-text-stroke: #2e3e44}\r\n  p.p4 {margin: 0.0px 0.0px 1.0px 0.0px;  font: 12.0px Courier; color: #2e3e44; -webkit-text-stroke: #2e3e44}\r\n    p.p3 {margin: 0.0px 0.0px 1.0px 0.0px; text-align: center; font: 12.0px Arial; color: #2e3e44; -webkit-text-stroke: #2e3e44; min-height: 14.0px}\r\n    span.s1 {font-kerning: none; }\r\n    table.t1 {border-style: solid; border-width: 0.5px 0.5px 0.5px 0.5px; border-color: #cbcbcb ; border-collapse: collapse}\r\n    table.t2 {border-style: solid; border-width: 0.5px 0.5px 0.5px 0.5px; border-color: #cbcbcb ; border-collapse: collapse}\r\n    td.td1 {width: 70.0px; border-style: solid; border-width: 1px ; border-color:  #979797; padding:5px; }\r\n    td.td2 {width: 10.0px; border-style: solid; border-width: 0.0px 1.0px 1.0px 1.0px; border-color: #979797;}\r\n\t td.td3 { border:solid 1px #979797; border-right:none; border-top:none; width:20px; text-align:center;}\r\n\t .heading{ font-family:Arial, Helvetica, sans-serif; font-weight:bold; text-align:center;}\r\n  </style>\r\n</head>\r\n<body>\r\n    <table cellspacing=\"0\" cellpadding=\"0\" class=\"t1\" align =\"Center\">\r\n <tr>\r\n      <td valign=\"top\" colspan=\"7\" class=\"td1 heading\">"+heading+"</td>\r\n\t  <tr>\r\n  <tbody>\r\n   \r\n    <tr>\r\n      <td valign=\"top\" class=\"td1\">\r\n        <p class=\"p1\"><span class=\"s1\"><b>SUN</b></span></p>\r\n      </td>\r\n      <td valign=\"top\" class=\"td1\">\r\n        <p class=\"p1\"><span class=\"s1\"><b>MON</b></span></p>\r\n      </td>\r\n      <td valign=\"top\" class=\"td1\">\r\n        <p class=\"p1\"><span class=\"s1\"><b>TUE</b></span></p>\r\n      </td>\r\n      <td valign=\"top\" class=\"td1\">\r\n        <p class=\"p1\"><span class=\"s1\"><b>WED</b></span></p>\r\n      </td>\r\n      <td valign=\"top\" class=\"td1\">\r\n        <p class=\"p1\"><span class=\"s1\"><b>THU</b></span></p>\r\n      </td>\r\n      <td valign=\"top\" class=\"td1\">\r\n        <p class=\"p1\"><span class=\"s1\"><b>FRI</b></span></p>\r\n      </td>\r\n      <td valign=\"top\" class=\"td1\">\r\n        <p class=\"p1\"><span class=\"s1\"><b>SAT</b></span></p>\r\n      </td>\r\n    </tr>\r\n    <tr>";


			string DynamicDays = string.Empty;
			string saparator =	" </tr>\n    <tr>";
			for (int i = 0; i < CommonClass.calData.Count; i++) {
				if (i % 7 == 0) {
					DynamicDays += saparator;
				}

				string DepTimeFirstLeg = CommonClass.calData [i].DepTimeFirstLeg;
				string ArrStaLastLeg = CommonClass.calData [i].ArrStaLastLeg;
				string LandTimeLastLeg = CommonClass.calData [i].LandTimeLastLeg;
				if (string.IsNullOrEmpty (DepTimeFirstLeg))
					DepTimeFirstLeg = "    ";

				if (string.IsNullOrEmpty (ArrStaLastLeg))
					ArrStaLastLeg = "    ";


				if (string.IsNullOrEmpty (LandTimeLastLeg))
					LandTimeLastLeg = "    ";

				if (string.IsNullOrEmpty (CommonClass.calData [i].Day)) {
					DynamicDays += "<td valign=\"top\" class=\"td2\">\r\n       \r\n        <p class=\"p2\"><span class=\"s1\"><br>\r\n</span></p>\r\n<p class=\"p2\"><span class=\"s1\"><br>\r\n</span></p>\r\n        <p class=\"p3\">" + DepTimeFirstLeg + "</p>\r\n        <p class=\"p3\">" + ArrStaLastLeg + "</p>\r\n        <p class=\"p3\">" + LandTimeLastLeg + "</p>\r\n        <p class=\"p2\"><span class=\"s1\"><br>\r\n</span></p>\r\n      </td>";
				} else
					DynamicDays += "<td valign=\"top\" class=\"td2\">\r\n        <table cellspacing=\"0\" cellpadding=\"0\" class=\"t2\" align=\"right\">\r\n          <tbody>\r\n            <tr>\r\n              <td valign=\"middle\" class=\"td3\">\r\n                <p class=\"p2\"><span class=\"s1\"><b>" + CommonClass.calData [i].Day + "</b></span></p>\r\n              </td>\r\n            </tr>\r\n          </tbody>\r\n        </table>\r\n        <p class=\"p2\"><span class=\"s1\"><br>\r\n</span></p>\r\n<p class=\"p2\"><span class=\"s1\"><br>\r\n</span></p>\r\n        <p class=\"p3\">" + DepTimeFirstLeg + "</p>\r\n        <p class=\"p3\">" + ArrStaLastLeg + "</p>\r\n        <p class=\"p3\">" + LandTimeLastLeg + "</p>\r\n        <p class=\"p2\"><span class=\"s1\"><br>\r\n</span></p>\r\n      </td>";
			}

			DynamicDays += " </tr>\n    \r\n  </tbody>";
			string hotels = "<br/>";
			var hotelLst = 	CalendarViewBL.GenerateCalendarAndHotelDetails (line);
			foreach (var hot in hotelLst) {
				hotels += hot+"<br/>";
			}
			string holidayDetails = "<tr>\r\n              <td valign=\"top\" colspan=\"7\" class=\"td4\"> <p class=\"p4\">"+hotels+"</p></td>\r\n              <tr>";
			string FinalContent = "</table>\r\n\r\n</body>\r\n</html>";

			content = InitalContent + DynamicDays + holidayDetails + FinalContent;
			var web = new UIWebView ();
			web.Frame=new CGRect(10,10,700,900);

			web.LoadHtmlString (content, null);





			var attributes = new UIStringAttributes () { Font = UIFont.FromName ("Courier", 5) };
			var printDoc = new NSAttributedString (content, attributes);

			var printInfo = UIPrintInfo.PrintInfo;

			printInfo.OutputType = UIPrintInfoOutputType.General;
			printInfo.JobName = "CalendarPrint";

			var textFormatter = new UISimpleTextPrintFormatter (printDoc) {
				StartPage = 0,

			ContentInsets = new UIEdgeInsets( 5,5,5,5),
				MaximumContentWidth = 6 * 72,
			};
			web.ViewPrintFormatter.ContentInsets = new UIEdgeInsets (200, 5, 5, 5);
			var printer = UIPrintInteractionController.SharedPrintController;
			printer.PrintInfo = printInfo;
			printer.PrintFormatter = web.ViewPrintFormatter;

			printer.ShowsPageRange = true;
			printer.PresentFromRectInView (btnCalPrint.Frame, vwCalPopover, true, (handler, completed, err) => {
				if (!completed && err != null) {
					Console.WriteLine ("error");
				} else if (completed) {
					new UIAlertView ("WBidMax",
						"The line has been sent for printout.", null,
						"OK", null).Show ();
				}
			});

		}

		void TripPrint ()
		{
			var content = string.Empty;
			foreach (TripData data in CommonClass.tripData) {
				content += "\n";
				content += data.Content;
			}
			var attributes = new UIStringAttributes () { Font = UIFont.FromName ("Courier", 10) };
			var printDoc = new NSAttributedString (content, attributes);

			var printInfo = UIPrintInfo.PrintInfo;
			printInfo.OutputType = UIPrintInfoOutputType.General;
			printInfo.JobName = "My Trip Print Job";

			var textFormatter = new UISimpleTextPrintFormatter (printDoc) {
				StartPage = 0,
				ContentInsets = new UIEdgeInsets (5, 5, 5, 5),
			};

			var printer = UIPrintInteractionController.SharedPrintController;
			printer.PrintInfo = printInfo;
			printer.PrintFormatter = textFormatter;
			printer.ShowsPageRange = true;
			printer.PresentFromRectInView (btnTripPrint.Frame, vwTripPopover, true, (handler, completed, err) => {
				if (!completed && err != null) {
					Console.WriteLine ("error");
				} else if (completed) {
					new UIAlertView ("WBidMax",
						"The Trip has been sent for printout.", null,
						"OK", null).Show ();
				}
			});

		}

		private string GetLineProperty (string displayName, Line line)
		{
			if (displayName == "$/Day") {
				return line.TfpPerDay.ToString ();
			} else if (displayName == "$/DHr") {
				return line.TfpPerDhr.ToString ();
			} else if (displayName == "$/Hr") {
				return line.TfpPerFltHr.ToString ();
			} else if (displayName == "$/TAFB") {
				return line.TfpPerTafb.ToString ();
			} else if (displayName == "+Grd") {
				return line.LongestGrndTime.ToString (@"hh\:mm");
			} else if (displayName == "+Legs") {
				return line.MostLegs.ToString ();
			} else if (displayName == "+Off") {
				return line.LargestBlkOfDaysOff.ToString ();
			} else if (displayName == "1Dy") {
				return line.Trips1Day.ToString ();
			} else if (displayName == "2Dy") {
				return line.Trips2Day.ToString ();
			} else if (displayName == "3Dy") {
				return line.Trips3Day.ToString ();
			} else if (displayName == "4Dy") {
				return line.Trips4Day.ToString ();
			} else if (displayName == "8753") {
				return line.Equip8753.ToString ();
			} else if (displayName == "A/P") {
				return line.AMPM.ToString ();
			} else if (displayName == "ACChg") {
				return line.AcftChanges.ToString ();
			} else if (displayName == "ACDay") {
				return line.AcftChgDay.ToString ();
			} else if (displayName == "CO") {
				return line.CarryOverTfp.ToString ();
			} else if (displayName == "DP") {
				return line.TotDutyPds.ToString ();
			} else if (displayName == "DPinBP") {
				return line.TotDutyPdsInBp.ToString ();
			} else if (displayName == "EDomPush") {
				return line.EDomPush;
			} else if (displayName == "EPush") {
				return line.EPush;
			} else if (displayName == "FA Posn") {
				return string.Join ("", line.FAPositions.ToArray ());
			} else if (displayName == "Flt") {
				return line.BlkHrsInBp;
			} else if (displayName == "LArr") {
				return line.LastArrTime.ToString ();
			} else if (displayName == "LDomArr") {
				return line.LastDomArrTime.ToString ();
			} else if (displayName == "Legs") {
				return line.Legs.ToString ();
			} else if (displayName == "LgDay") {
				return line.LegsPerDay.ToString ();
			} else if (displayName == "LgPair") {
				return line.LegsPerPair.ToString ();
			} else if (displayName == "ODrop") {
				return line.OverlapDrop.ToString ();
			} else if (displayName == "Off") {
				return line.DaysOff.ToString ();
			} else if (displayName == "Pairs") {
				return line.TotPairings.ToString ();
			} else if (displayName == "Pay") {
				return Decimal.Round (line.Tfp, 2).ToString ();
			} else if (displayName == "PDiem") {
				return line.TafbInBp;
			} else if (displayName == "MyValue") {
				return Decimal.Round (line.Points, 2).ToString ();
			} else if (displayName == "SIPs") {
				return line.Sips.ToString ();
			} else if (displayName == "StartDOW") {
				return line.StartDow;
			} else if (displayName == "T234") {
				return line.T234;
			} else if (displayName == "VDrop") {
				return line.VacationDrop.ToString ();
			} else if (displayName == "WkEnd") {
				if (line.Weekend != null)
					return line.Weekend.ToLower ();
				else
					return "";
			} else if (displayName == "FltRig") {
				return line.RigFltInBP.ToString ();
			} else if (displayName == "MinPayRig") {
				return line.RigDailyMinInBp.ToString ();
			} else if (displayName == "DhrRig") {
				return line.RigDhrInBp.ToString ();
			} else if (displayName == "AdgRig") {
				return line.RigAdgInBp.ToString ();
			} else if (displayName == "TafbRig") {
				return line.RigTafbInBp.ToString ();
			} else if (displayName == "TotRig") {
				return line.RigTotalInBp.ToString ();
			} else if (displayName == "VacPay") {
				return Decimal.Round (line.VacPay, 2).ToString ();
			} else if (displayName == "Vofrnt") {
				return Decimal.Round (line.VacationOverlapFront, 2).ToString ();
			} else if (displayName == "Vobk") {
				return Decimal.Round (line.VacationOverlapBack, 2).ToString ();
			} else if (displayName == "800legs") {
				return line.LegsIn800.ToString ();
			} else if (displayName == "700legs") {
				return line.LegsIn700.ToString ();
			} else if (displayName == "500legs") {
				return line.LegsIn500.ToString ();
			} else if (displayName == "300legs") {
				return line.LegsIn300.ToString ();
			} else if (displayName == "DhrInBp") {
				return line.DutyHrsInBp;
			} else if (displayName == "DhrInLine") {
				return line.DutyHrsInLine;
			} else if (displayName == "Wts") {
				return Decimal.Round (line.TotWeight, 2).ToString ();
			}  
			else if (displayName == "HolRig")
				return Decimal.Round (line.HolRig, 2).ToString ();
			else if (displayName == "nMid") {
				return Decimal.Round (line.TotWeight, 2).ToString ();


			}
			else if (displayName == "cmts") {

				return Decimal.Round (line.TotalCommutes, 2).ToString ();

			}
			else if (displayName == "cmtFr") {
				return Decimal.Round (line.commutableFronts, 2).ToString ();


			}
			else if (displayName == "cmtBa") {
				return Decimal.Round (line.CommutableBacks, 2).ToString ();


			}
			else if (displayName == "cmt%Fr") {
				return Decimal.Round (line.CommutabilityFront, 2).ToString ();


			}
			else if (displayName == "cmt%Ba") {
				return Decimal.Round (line.CommutabilityBack, 2).ToString ();


			}
			else if (displayName == "cmt%Ov") {
				return Decimal.Round (line.CommutabilityOverall, 2).ToString ();


			}
			else {
				return "";
			}
		}

		partial void btnCalPrintTapped (UIKit.UIButton sender)
		{
			//CalPrint ();

			UIActionSheet sheet = new UIActionSheet ("Select", null, null, null, new string[] {
				"BidLine with Pairings",
				"Calendar View"
			});
			sheet.ShowFrom (((UIButton)sender).Frame, vwCalPopover, true);
			sheet.Dismissed += (object senderSheet, UIButtonEventArgs e) => {
				if (e.ButtonIndex == 0) {
					CalPrint ();
				} else {
					CalFormatPrint ();
				}
			};
		}

		partial void btnTripPrintTapped (UIKit.UIButton sender)
		{
			TripPrint ();
		}

		partial void btnCalendarExport (Foundation.NSObject sender)
		{
			if (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO") {
				UIActionSheet sheet = new UIActionSheet ("Select", null, null, null, new string[] {
					"Export to Calendar",
					"Export to FFDO"
				});
				sheet.ShowFrom (((UIButton)sender).Frame, vwCalPopover, true);
				sheet.Clicked += (object senderSheet, UIButtonEventArgs e) => {
					if (e.ButtonIndex == 0) {
						ExportToCalendar ();
					} else {
						//FFDO
						string result = GenarateFFDOforLine ();
						UIPasteboard clipBoard = UIPasteboard.General;
						clipBoard.String = result;
                        new UIAlertView ("WBidMax", "The FFDO line details have been copied to the clipboard", null, "OK", null).Show ();
					}
				};
			} else {
				ExportToCalendar ();
			}
		}

		void ExportToCalendar ()
		{
			List<ExportCalendar> lstExportCalendar = new List<ExportCalendar> ();
			Line exportLine = GlobalSettings.Lines [lPath.Row];
			if (exportLine != null) {
				Trip trip = null;
				DateTime tripDate = DateTime.MinValue;
				bool isLastTrip = false;
				int paringCount = 0;
				foreach (string pairing in exportLine.Pairings) {
					trip = GetTrip (pairing);
					isLastTrip = ((exportLine.Pairings.Count - 1) == paringCount);
					paringCount++;
					tripDate = WBidCollection.SetDate (Convert.ToInt16 (pairing.Substring (4, 2)), isLastTrip);
					if (GlobalSettings.WBidINIContent.PairingExport.IsEntirePairing) {
						lstExportCalendar.Add (ExportTripDetails (trip, tripDate, pairing, exportLine.LineNum));
					} else {
						var dutyDetails = ExportDutyPeriodDetails (trip, tripDate, pairing, exportLine.LineNum);
						if (dutyDetails != null) {
							foreach (var item in dutyDetails) {
								lstExportCalendar.Add (item);
							}
						}
					}
				}
			}
			Calendar cal = new Calendar ();
			cal.EventStore.RequestAccess (EKEntityType.Event, (bool granted, NSError e) => {
				if (granted) {
					foreach (ExportCalendar exp in lstExportCalendar) {
						NSError err;
						try {
							if (exp.StarDdate > exp.EndDate)
								exp.EndDate = exp.EndDate.AddDays (1);
							EKEvent newEvent = EKEvent.FromStore (cal.EventStore);
							newEvent.StartDate = exp.StarDdate.DateTimeToNSDate ();
							newEvent.EndDate = exp.EndDate.DateTimeToNSDate ();
							newEvent.Title = exp.Title;
							newEvent.Notes = exp.TripDetails;
							newEvent.Calendar = cal.EventStore.DefaultCalendarForNewEvents;
							cal.EventStore.SaveEvent (newEvent, EKSpan.ThisEvent, out err);
						} catch {
							err = new NSError ();
						}
						if (err == null && e == null) {
							InvokeOnMainThread (() => {
								new UIAlertView ("WBidMax", "The line was added to the calendar.", null, "OK", null).Show ();
							});
						} else {
							InvokeOnMainThread (() => {
								new UIAlertView ("WBidMax", "Calendar Export Failed.", null, "OK", null).Show ();
							});
						}
					}
					Console.WriteLine ("Calendar access granted.");
				} else {
					InvokeOnMainThread (() => {
						new UIAlertView ("Access Denied", "User Denied Access to Calendar \nPlease change the privacy settings for WbidiPad in Settings/Privacy/Calendars/WbidiPad", null, "OK", null).Show ();
					});
				}
			});
		}


		private void ExportTripDetails (string tripName, int lineNum, bool isLastTrip)
		{
			List<ExportCalendar> lstExportCalendar = new List<ExportCalendar> ();
			// Line exportLine = GlobalSettings.Lines[lPath.Row];

			//if (exportLine != null)
			//{

			Trip trip = null;
			DateTime tripDate = DateTime.MinValue;


			//  foreach (string pairing in exportLine.Pairings)
			// {
			trip = GetTrip (tripName);

			tripDate = WBidCollection.SetDate (Convert.ToInt16 (tripName.Substring (4, 2)), isLastTrip);

			if (GlobalSettings.WBidINIContent.PairingExport.IsEntirePairing) {
				lstExportCalendar.Add (ExportTripDetails (trip, tripDate, tripName, lineNum));
			} else {

				var dutyDetails = ExportDutyPeriodDetails (trip, tripDate, tripName, lineNum);
				if (dutyDetails != null) {
					foreach (var item in dutyDetails) {
						lstExportCalendar.Add (item);

					}
				}

			}

			// }
			// }

			Calendar cal = new Calendar ();
			cal.EventStore.RequestAccess (EKEntityType.Event,
				(bool granted, NSError e) => {
					if (granted) {
						foreach (ExportCalendar exp in lstExportCalendar) {
							NSError err;
							try {
								if (exp.StarDdate > exp.EndDate)
									exp.EndDate = exp.EndDate.AddDays (1);
								EKEvent newEvent = EKEvent.FromStore (cal.EventStore);
								newEvent.StartDate = exp.StarDdate.DateTimeToNSDate ();
								newEvent.EndDate = exp.EndDate.DateTimeToNSDate ();
								newEvent.Title = exp.Title;
								newEvent.Notes = exp.TripDetails;
								newEvent.Calendar = cal.EventStore.DefaultCalendarForNewEvents;
								cal.EventStore.SaveEvent (newEvent, EKSpan.ThisEvent, out err);
							} catch {
								err = new NSError ();
							}
							if (err == null && e == null) {
								InvokeOnMainThread (() => {
									new UIAlertView ("WBidMax",
										"The trip was added to the calendar.", null,
										"OK", null).Show ();
								});
							} else {
								InvokeOnMainThread (() => {
									new UIAlertView ("WBidMax",
										"Trip Export Failed", null,
										"OK", null).Show ();
								});
							}
						}
						Console.WriteLine ("Calendar access granted.");
					} else {
						InvokeOnMainThread (() => {
							new UIAlertView ("Access Denied",
								"User Denied Access to Calendar \nPlease change the privacy settings for WbidiPad in Settings/Privacy/Calendars/WbidiPad", null,
								"OK", null).Show ();
						});
					}
				});

		}


		private List<ExportCalendar> ExportDutyPeriodDetails (Trip trip, DateTime tripStartDate, string tripName, int lineNum)
		{
			List<ExportCalendar> lstExportCalendar = new List<ExportCalendar> ();


			CorrectionParams correctionParams = new Model.CorrectionParams ();
			correctionParams.selectedLineNum = lineNum;
			foreach (DutyPeriod dp in trip.DutyPeriods) {
				ExportCalendar exportCalendar = new ExportCalendar ();
				int day = 0;
				//string startTime = GetTime(Helper.ConvertMinuteToHHMM(dp.ShowTime - (1440 * (dp.DutPerSeqNum - 1))), out day);
				string startTime = FormatTime ((dp.ShowTime - (1440 * (dp.DutPerSeqNum - 1))), out day);
				if (GlobalSettings.WBidINIContent.PairingExport.IsCentralTime) {
					exportCalendar.StarDdate = DateTime.Parse (tripStartDate.AddDays (day).ToShortDateString () + " " + startTime);
				} else {
					exportCalendar.StarDdate = DateTime.Parse (tripStartDate.AddDays (day).ToShortDateString () + " " + ConvertTimeToDomicile (startTime, tripStartDate.AddDays (day)));
				}
				// string endTime = GetTime(Helper.ConvertMinuteToHHMM(dp.ReleaseTime - (1440 * (dp.DutPerSeqNum - 1))), out day);
				string endTime = FormatTime ((dp.LandTimeLastLeg + GlobalSettings.debrief) - ((dp.DutPerSeqNum - 1) * 1440), out day);
				if (GlobalSettings.WBidINIContent.PairingExport.IsCentralTime) {
					exportCalendar.EndDate = DateTime.Parse (tripStartDate.AddDays (day).ToShortDateString () + " " + endTime);
				} else {
					exportCalendar.EndDate = DateTime.Parse (tripStartDate.AddDays (day).ToShortDateString () + " " + ConvertTimeToDomicile (endTime, tripStartDate.AddDays (day)));
				}
				string subject = dp.ArrStaLastLeg;





				if (GlobalSettings.WBidINIContent.PairingExport.IsCentralTime) {
					subject = dp.ArrStaLastLeg + " " + FormatTime ((dp.LandTimeLastLeg + GlobalSettings.debrief) % 1440, out day);
				} else {

					string domicileTime = FormatTime ((dp.LandTimeLastLeg + GlobalSettings.debrief) - ((dp.DutPerSeqNum - 1) * 1440), out day);
					subject = dp.ArrStaLastLeg + " " + ConvertTimeToDomicile (domicileTime, tripStartDate.AddDays (day));

				}

				if (GlobalSettings.WBidINIContent.PairingExport.IsSubjectLineSelected) {
					subject = trip.TripNum.Substring (0, 4) + " " + subject;
				}
				exportCalendar.Title = subject;

				ObservableCollection<TripData> lstDutyperiodDetails = TripViewBL.GenerateDutyPeriodDetails (trip, correctionParams, dp.DutPerSeqNum);


				string body = string.Empty;
				foreach (var item in lstDutyperiodDetails) {
					body += item.Content + Environment.NewLine;

				}

				exportCalendar.TripDetails = "REPORT " + startTime + " CST/CDT";
				body += Environment.NewLine + Environment.NewLine + "(Note: All times are CST/CDT unless otherwise noted.)";
				if (!GlobalSettings.WBidINIContent.PairingExport.IsCentralTime) {
					exportCalendar.TripDetails += " (" + startTime + " Domicile Time)";
					body += Environment.NewLine + "(Note: AppointmentDetails times  are in Domicile time).";
				}
				exportCalendar.TripDetails += Environment.NewLine + Environment.NewLine + body;



				lstExportCalendar.Add (exportCalendar);
				tripStartDate = tripStartDate.AddDays (1);

			}

			return lstExportCalendar;


		}

		private string FormatTime (int time, out int day)
		{
			day = 0;

			string stringTime = string.Empty;
			int hour = 0;
			int minutes = 0;


			hour = time / 60;
			minutes = time % 60;

			//if (hour > 24)
			//{
			//    day = 1;
			//    hour = hour - 24;
			//}
			if (hour >= 24) {
				if (minutes > 0) {
					day = 1;
				}

				hour = hour - 24;
				stringTime = hour.ToString ().PadLeft (2, '0');
			} else {
				stringTime = (hour > 12) ? (hour - 12).ToString ("d2") : hour.ToString ("d2");
			}
			stringTime += ":" + minutes.ToString ("d2");

			stringTime += ((hour >= 12) ? " PM" : " AM");

			return stringTime;

		}

		private string ConvertTimeToDomicile (string time, DateTime date)
		{




			int hours = 0;
			int minutes = 0;
			int result = 0;
			string strTime = string.Empty;

			if (time.Substring (6, 2) == "PM") {
				hours = 12;
			}

			hours += int.Parse (time.Substring (0, 2));
			minutes = int.Parse (time.Substring (3, 2));

			result = hours * 60 + minutes;

			result = WBidCollection.DomicileTimeFromHerb (GlobalSettings.CurrentBidDetails.Domicile, date, result);

			hours = result / 60;
			minutes = result % 60;

			if (hours == 24) {
				hours = 0;
				strTime = "00";
			} else {
				strTime = (hours > 12) ? (hours - 12).ToString ("d2") : hours.ToString ("d2");
			}
			strTime += ":" + minutes.ToString ("d2");

			strTime += ((hours >= 12) ? " PM" : " AM");

			return strTime;
		}


		private ExportCalendar ExportTripDetails (Trip trip, DateTime tripStartDate, string tripName, int lineNum)
		{
			ExportCalendar exportCalendar = new ExportCalendar ();

			string startTime = CalculateStartTimeBasedOnTime (trip, tripStartDate);
			exportCalendar.StarDdate = DateTime.Parse (tripStartDate.ToShortDateString () + " " + startTime);


			//Generate Subject
			//-----------------------------------------
			string subject = string.Empty;
			foreach (DutyPeriod dutyPeriod in trip.DutyPeriods) {

				if (subject != string.Empty) {
					subject += "/";
				}
				subject += dutyPeriod.ArrStaLastLeg;
			}

			if (GlobalSettings.WBidINIContent.PairingExport.IsSubjectLineSelected) {
				subject = trip.TripNum.Substring (0, 4) + " " + subject;
			}

			//-----------------------------------------

			exportCalendar.Title = subject;
			int day = 0;

			exportCalendar.EndDate = DateTime.Parse (tripStartDate.AddDays (trip.DutyPeriods.Count - 1).ToShortDateString () + " " + CalculateEndTimeBasedOnTime (trip, tripStartDate, out day));

			exportCalendar.EndDate = exportCalendar.EndDate.AddDays (day);
			CorrectionParams correctionParams = new Model.CorrectionParams ();
			correctionParams.selectedLineNum = lineNum;
			var tripDetails = TripViewBL.GenerateTripDetailsFiltered (tripName, correctionParams);

			string body = string.Empty;
			foreach (var item in tripDetails) {
				body += item.Content + Environment.NewLine;

			}

			exportCalendar.TripDetails = "REPORT " + CalculateStartTime (trip) + " CST/CDT";
			body += Environment.NewLine + Environment.NewLine + "(Note: All times are CST/CDT unless otherwise noted.)";
			if (!GlobalSettings.WBidINIContent.PairingExport.IsCentralTime) {
				exportCalendar.TripDetails += " (" + startTime + " Domicile Time)";
				body += Environment.NewLine + "(Note: AppointmentDetails times  are in Domicile time).";
			}
			exportCalendar.TripDetails += Environment.NewLine + Environment.NewLine + body;




			return exportCalendar;

		}

		private string GetTime (string time, out int day)
		{
			day = 0;
			int hour = 0;
			int minute = 0;
			string result = time;
			hour = int.Parse (result.Substring (0, 2));
			minute = int.Parse (result.Substring (3, 2));
			if (hour >= 24) {
				hour = hour - 24;
				day = 1;
			}

			int hourAmpm = hour;
			hourAmpm = (hourAmpm > 12) ? hourAmpm - 12 : hourAmpm;
			result = hourAmpm.ToString ().PadLeft (2, '0') + ":" + minute.ToString ().PadLeft (2, '0');
			result += ((hour >= 12) ? " PM" : " AM");
			return result;
		}

		private string CalculateStartTime (Trip trip)
		{

			string startTime = string.Empty;
			int hour = 0;
			int minutes = 0;
			int startTimeMinute = 0;
			//int depTimeMinutes =  int.Parse(trip.DepTime);
			int depTimeMinutes = ConvertHhmmToMinutes (trip.DepTime);

			//Int16.Parse(trip.DepTime.Substring(0, 2)) * 60 + Int16.Parse(trip.DepTime.Substring(2, 2));
			//  startTimeMinute = depTimeMinutes - trip.BriefTime;
			startTimeMinute = trip.DutyPeriods [0].ShowTime;
			hour = startTimeMinute / 60;
			minutes = startTimeMinute % 60;

			if (hour == 24) {
				hour = 0;
				startTime = "00";
			} else {
				startTime = (hour > 12) ? (hour - 12).ToString ("d2") : hour.ToString ("d2");
			}
			startTime += ":" + minutes.ToString ("d2");

			startTime += ((hour >= 12) ? " PM" : " AM");

			return startTime;

		}

		private string CalculateStartTimeBasedOnTime (Trip trip, DateTime tripStartDate)
		{

			string startTime = string.Empty;
			int hour = 0;
			int minutes = 0;
			int startTimeMinute = 0;
			startTimeMinute = trip.DutyPeriods [0].ShowTime;
			if (!GlobalSettings.WBidINIContent.PairingExport.IsCentralTime) {
				startTimeMinute = WBidCollection.DomicileTimeFromHerb (GlobalSettings.CurrentBidDetails.Domicile, tripStartDate, startTimeMinute);
			}

			hour = startTimeMinute / 60;
			minutes = startTimeMinute % 60;

			if (hour == 24) {
				hour = 0;
				startTime = "00";
			} else {
				startTime = (hour > 12) ? (hour - 12).ToString ("d2") : hour.ToString ("d2");
			}
			startTime += ":" + minutes.ToString ("d2");

			startTime += ((hour >= 12) ? " PM" : " AM");
			return startTime;
		}

		private string CalculateEndTimeBasedOnTime (Trip trip, DateTime tripStartDate, out int day)
		{

			string endtTime = string.Empty;
			int hour = 0;
			int minutes = 0;
			int endTimeMinute = 0;
			day = 0;

			//int repTimeMinutes = int.Parse( trip.RetTime)%1440;
			int repTimeMinutes = ConvertHhmmToMinutes (trip.RetTime);
			//Int16.Parse(trip.RetTime.Substring(0, 2)) * 60 + Int16.Parse(trip.RetTime.Substring(2, 2));
			endTimeMinute = repTimeMinutes + trip.DebriefTime;
			if (!GlobalSettings.WBidINIContent.PairingExport.IsCentralTime) {
				endTimeMinute = WBidCollection.DomicileTimeFromHerb (GlobalSettings.CurrentBidDetails.Domicile, tripStartDate, endTimeMinute);
			}

			if (endTimeMinute >= 1440) {
				day = 1;
				endTimeMinute = endTimeMinute % 1440;
			}


			if (endTimeMinute == 0) {
				hour = 0;
				minutes = 0;

			} else {

				hour = endTimeMinute / 60;
				minutes = endTimeMinute % 60;
			}

			//if (hour == 24)
			//{
			//    hour = 0;
			//    endtTime = "00";
			//}
			//else
			//{
			//    endtTime = (hour > 12) ? (hour - 12).ToString("d2") : hour.ToString("d2");
			//}

			endtTime = (hour > 12) ? (hour - 12).ToString ("d2") : hour.ToString ("d2");

			endtTime += ":" + minutes.ToString ("d2");

			endtTime += ((hour >= 12) ? " PM" : " AM");

			return endtTime;

		}

		private int ConvertHhmmToMinutes (string hhmm)
		{
			hhmm = hhmm.PadLeft (4, '0');
			int hours = Convert.ToInt32 (hhmm.Substring (0, 2));
			int minutes = Convert.ToInt32 (hhmm.Substring (2, 2));
			return hours * 60 + minutes;
		}

		public void calPopover (NSIndexPath path)
		{
			//            if (calCollection != null)
			//            {
			//                calCollection.View.RemoveFromSuperview();
			//                calCollection = null;
			//            }
			CommonClass.selectedTrip = null;

			CommonClass.selectedLine = GlobalSettings.Lines [path.Row].LineNum;
			CommonClass.calData = CalendarViewBL.GenerateCalendarDetails (GlobalSettings.Lines [path.Row]);

			this.vwTable.BringSubviewToFront (this.vwCalPopover);
			this.vwCalPopover.Hidden = true;
			this.vwCalPopover.Layer.BorderWidth = 1;
			this.vwCalPopover.Layer.BorderColor = UIColor.FromRGB (158, 179, 131).CGColor;
			this.vwCalPopover.Layer.CornerRadius = 3.0f;
			this.vwCalPopover.Layer.ShadowColor = UIColor.Black.CGColor;
			this.vwCalPopover.Layer.ShadowOpacity = 0.5f;
			this.vwCalPopover.Layer.ShadowRadius = 2.0f;
			this.vwCalPopover.Layer.ShadowOffset = new CGSize (3f, 3f);

			if (calCollection == null) {
				var layout = new UICollectionViewFlowLayout ();
				layout.SectionInset = new UIEdgeInsets (0, 0, 0, 0);
				layout.MinimumInteritemSpacing = 0;
				layout.MinimumLineSpacing = 0;
				layout.ItemSize = new CGSize (50, 65);
				calCollection = new CalenderPopoverController (layout);
				this.AddChildViewController (calCollection);
				calCollection.View.Frame = this.vwCalChild.Bounds;
				this.vwCalChild.AddSubview (calCollection.View);
			} else {
				calCollection.CollectionView.ReloadData ();
			}

			Line CalLine = GlobalSettings.Lines [path.Row];
			string title = "Line " + CalLine.LineNum;
			if (GlobalSettings.CurrentBidDetails.Postion == "FA") {
				if (CalLine.FAPositions.Count > 0) {
					title += "  - " + string.Join ("", CalLine.FAPositions);
				}
			}
			this.lblCalTitle.Text = title;

			if (path.Row == 0) {
				this.btnMovUp.Enabled = false;
				this.btnMovDown.Enabled = true;
			} else if (path.Row == this.sumList.TableView.NumberOfRowsInSection (0) - 1) {
				this.btnMovUp.Enabled = true;
				this.btnMovDown.Enabled = false;
			} else {
				this.btnMovUp.Enabled = true;
				this.btnMovDown.Enabled = true;
			}

			if (GlobalSettings.Lines [path.Row].TopLock)
				this.btnLineTopLock.Enabled = false;
			else
				this.btnLineTopLock.Enabled = true;
			if (GlobalSettings.Lines [path.Row].BotLock)
				this.btnLineBotLock.Enabled = false;
			else
				this.btnLineBotLock.Enabled = true;

			CommonClass.doubleTapLine.Clear ();
			CommonClass.doubleTapLine.Add (GlobalSettings.Lines [path.Row].LineNum);

			this.vwCalPopover.Hidden = false;
			this.vwTripPopover.Hidden = true;
		}

		private void tripPopover ()
		{
			//            if (tripList != null)
			//            {
			//                tripList.View.RemoveFromSuperview();
			//                tripList = null;
			//            }

			this.vwTable.BringSubviewToFront (this.vwTripPopover);
			this.vwTripPopover.Hidden = true;
			this.vwTripPopover.Layer.BorderWidth = 1;
			this.vwTripPopover.Layer.BorderColor = UIColor.FromRGB (158, 179, 131).CGColor;
			this.vwTripPopover.Layer.CornerRadius = 3.0f;
			this.vwTripPopover.Layer.ShadowColor = UIColor.Black.CGColor;
			this.vwTripPopover.Layer.ShadowOpacity = 0.5f;
			this.vwTripPopover.Layer.ShadowRadius = 2.0f;
			this.vwTripPopover.Layer.ShadowOffset = new CGSize (3f, 3f);


			if (tripList == null) {
				tripList = new TripPopListViewController ();
				this.AddChildViewController (tripList);
				tripList.View.Frame = this.vwTripChild.Bounds;
				this.vwTripChild.AddSubview (tripList.View);
			} else {
				tripList.TableView.ReloadData ();
			}

			if (CommonClass.tripData.Count <= 21) {
				int tripCount = CommonClass.tripData.Count;
				CGRect frame = this.vwTripPopover.Frame;
				this.vwTripPopover.Frame = new CGRect (frame.X, frame.Y, frame.Width, tripCount * 25);
			} else {
				CGRect frame = this.vwTripPopover.Frame;
				this.vwTripPopover.Frame = new CGRect (frame.X, frame.Y, frame.Width, 530);
			}
			this.vwTripPopover.Hidden = false;

		}


		public void observeNotification ()
		{
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("DataReload"), handleDataReload));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("ReloadModernView"), ReloadModernView));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("DataCulumnsUpdated"), handleDataCulumnsUpdate));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("ButtonEnableDisable"), handleButtonEnable));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("SumRowSelected"), handleRowSelection));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("ShowPopover"), handlePopoverShow));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("HidePopover"), handlePopoverHide));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("CalPopover"), handleCalPopover));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("ColumnPopover"), handleColumnPopover));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("SummaryColumnSort"), handleColumnSort));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("CalPopHide"), handleCalPopHide));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("TripPopShow"), handleTripPopShow));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("TripPopHide"), handleTripPopHide));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("ResetButtonStates"), handleButtonStates));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("ShowGroupBidAutomator"), ShowGroupBidAutomator));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("ShowReparseView"), (NSNotification n) => {
				vwReparse.Hidden = false;
			}));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("AutoSaveTimer"), (NSNotification n) => {
				if (GlobalSettings.WBidINIContent.User.AutoSave) {
					if (timer != null)
						timer.Stop ();
					AutoSave ();
				} else {
					if (timer != null)
						timer.Stop ();
				}
			}));


		}

		void handleButtonStates (NSNotification obj)
		{
			btnVacCorrect.Selected = false;
			btnVacDrop.Selected = false;
			btnVacDrop.Enabled = false;
			btnOverlap.Selected = false;
			btnEOM.Selected = false;


			SetVacButtonStates ();
		}

		public void handleTripPopShow (NSNotification n)
		{
			tripNum = n.Object.ToString ();
			CorrectionParams correctionParams = new Model.CorrectionParams ();
			correctionParams.selectedLineNum = CommonClass.selectedLine;
			CommonClass.tripData = TripViewBL.GenerateTripDetails (tripNum, correctionParams, CommonClass.isLastTrip);
			this.lblTripPopTitle.Text = "Pairing " + tripNum.Substring (0, 4);


			if (GlobalSettings.CurrentBidDetails.Postion == "FA") {


				var line = GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == correctionParams.selectedLineNum);
				//lines.FirstOrDefault(y => y.Pairings.Any(x => x == tripname));
				if (line != null) {
					if (GlobalSettings.CurrentBidDetails.Round == "M") {
						this.lblTripPopTitle.Text += "  - " + string.Join ("", line.FAPositions);// + " Positions";
					} else {
						this.lblTripPopTitle.Text += "  - " + line.FASecondRoundPositions.FirstOrDefault (x => x.Key.Contains (tripNum.Substring (0, 4))).Value;// + " Position";
					}

				}


			}

			this.tripPopover ();
		}

		public void handleTripPopHide (NSNotification n)
		{
			this.vwTripPopover.Hidden = true;
		}

		public void handleCalPopHide (NSNotification n)
		{
			this.vwCalPopover.Hidden = true;
			this.vwTripPopover.Hidden = true;
			if (sumList != null)
				this.sumList.TableView.DeselectRow (lPath, true);
		}

		partial void btnTripCloseTapped (UIKit.UIButton sender)
		{
			this.vwTripPopover.Hidden = true;
			CommonClass.selectedTrip = null;
			NSNotificationCenter.DefaultCenter.PostNotificationName ("ReloadCal", null);
			if (bidLineList != null)
				bidLineList.TableView.ReloadData ();
			if (modernList != null)
				modernList.TableView.ReloadData ();
		}
		//public void handleColumnSort(NSNotification n)
		//{
		//    UITableViewCell cell = (UITableViewCell)n.Object;
		//    hPath = hTable.TableView.IndexPathForCell(cell);

		//    if (hPath != null && hPath.Row > 3)
		//    {
		//        DataColumn column = GlobalSettings.WBidINIContent.DataColumns[hPath.Row];
		//        ColumnDefinition definition = GlobalSettings.columndefinition.Where(x => x.Id == column.Id).FirstOrDefault();
		//        Console.WriteLine(definition.DisplayName);
		//        //string[] exclude = {"ODrop","Tag","VDrop","VacPay","Vofront","Vobk"};
		//        if (definition.DisplayName == "StartDOW")
		//        {
		//            CommonClass.columnID = GlobalSettings.WBidINIContent.DataColumns[hPath.Row].Id;
		//            LineOperations.SortColumns(definition.DataPropertyName, true);
		//            CommonClass.columnAscend = true;
		//            NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
		//        }
		//        else
		//        {
		//            if (column.Id > 4)
		//            { //&& !exclude.Contains (definition.DisplayName)) {
		//                CommonClass.columnID = GlobalSettings.WBidINIContent.DataColumns[hPath.Row].Id;
		//                if (cell.Tag == 1)
		//                {
		//                    LineOperations.SortColumns(definition.DataPropertyName, false);
		//                    CommonClass.columnAscend = false;
		//                }
		//                else if (cell.Tag == 2)
		//                {
		//                    LineOperations.SortColumns(definition.DataPropertyName, true);
		//                    CommonClass.columnAscend = true;
		//                }
		//            }
		//            NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
		//        }
		//    }
		//}


		public void handleColumnSort (NSNotification n)
		{
			if (n==null || n.Object == null)
				return;
			if (sgControlViewType.SelectedSegment != 0)
				return;

			var cell = (UITableViewCell)n.Object;
			if (cell != null && hTable.TableView!=null)
				hPath = hTable.TableView.IndexPathForCell (cell);

			if (hPath != null && hPath.Row > 3) {
				DataColumn column;
				if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
					column = GlobalSettings.WBidINIContent.SummaryVacationColumns [hPath.Row];
				else
					column = GlobalSettings.WBidINIContent.DataColumns [hPath.Row];
				ColumnDefinition definition = GlobalSettings.columndefinition.FirstOrDefault (x => x.Id == column.Id);

				if (column == null || definition == null)
					return;

				Console.WriteLine (definition.DisplayName);

				if (column.Id > 4) {
					WBidHelper.PushToUndoStack ();

					CommonClass.columnID = column.Id;
					bool status = false;
					if (definition.DisplayName == "StartDOW") {
						status = true;
					} else if (cell.Tag == 1) {
						status = false;
					} else if (cell.Tag == 2) {
						status = true;
					}
					LineOperations.SortColumns (definition.DataPropertyName, status);
					CommonClass.columnAscend = status;
					NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);

					GlobalSettings.isModified = true;
					CommonClass.lineVC.UpdateSaveButton ();
				}




			}
		}

		public void handleDataReload (NSNotification n)
		{
			ReloadLineView ();

			if (hTable != null)
				hTable.TableView.ReloadData ();
			if (sumList != null)
				sumList.TableView.ReloadData ();
			if (bidLineList != null)
				bidLineList.TableView.ReloadData ();
			if (modernList != null)
			{
				ReloadModernViewOverlay ();
				modernList.TableView.ReloadData ();
			}

			if (GlobalSettings.Lines.Count (x => x.TopLock == true) > 0) {
				this.btnRemTopLock.Enabled = true;
			} else {
				this.btnRemTopLock.Enabled = false;
			}

			if (GlobalSettings.Lines.Count (x => x.BotLock == true) > 0) {
				this.btnRemBottomLock.Enabled = true;
			} else {
				this.btnRemBottomLock.Enabled = false;
			}
		

		}

		public void handleDataCulumnsUpdate (NSNotification n)
		{
			//			PointF tableOffset = PointF.Empty;
			//			if (sumList != null)
			//				tableOffset = sumList.TableView.ContentOffset;
			//			if (bidLineList != null)
			//				tableOffset = bidLineList.TableView.ContentOffset;


			loadSummaryListAndHeader ();
            
			//			sumList.TableView.ContentOffset = tableOffset;
			//			bidLineList.TableView.ContentOffset = tableOffset;

		}

		public void handlePopoverShow (NSNotification n)
		{
			if (popoverController == null) {
				lPath = (NSIndexPath)n.Object;
				PopoverViewController popoverContent = new PopoverViewController ();
				popoverContent.PopType = "sumOpt";
				popoverController = new UIPopoverController (popoverContent);
				popoverController.Delegate = new MyPopDelegate (this);
				popoverController.PopoverContentSize = new CGSize (280, 250);
				if (sumList != null) {
					this.sumList.TableView.SelectRow (lPath, false, UITableViewScrollPosition.None);
					if (this.View.Window != null)
						popoverController.PresentFromRect (sumList.TableView.RectForRowAtIndexPath (lPath), sumList.TableView, UIPopoverArrowDirection.Any, true);
				} else if (bidLineList != null) {
					this.bidLineList.TableView.SelectRow (lPath, false, UITableViewScrollPosition.None);
					if (this.View.Window != null)
						popoverController.PresentFromRect (bidLineList.TableView.RectForRowAtIndexPath (lPath), bidLineList.TableView, UIPopoverArrowDirection.Any, true);
				} else if (modernList != null) {
					
					this.modernList.TableView.SelectRow (lPath, false, UITableViewScrollPosition.None);
					if (this.View.Window != null)
						popoverController.PresentFromRect (modernList.TableView.RectForRowAtIndexPath (lPath), modernList.TableView, UIPopoverArrowDirection.Any, true);
				}
				this.vwCalPopover.Hidden = true;
				this.vwTripPopover.Hidden = true;
			}
		}

		public void handlePopoverHide (NSNotification n)
		{
			if (popoverController != null) {
				popoverController.Dismiss (true);
				popoverController = null;
			}
		}

		private void handleCalPopover (NSNotification n)
		{
			UITableViewCell cell = (UITableViewCell)n.Object;
			lPath = sumList.TableView.IndexPathForCell (cell);
			if (cell != null && lPath != null) {
				CommonClass.selectedLine = GlobalSettings.Lines [lPath.Row].LineNum;
				sumList.TableView.SelectRow (lPath, false, UITableViewScrollPosition.None);
				this.calPopover (lPath);
			}
		}

		public void handleColumnPopover (NSNotification n)
		{
			if (n==null || n.Object == null)
				return;
			UITableViewCell cell = (UITableViewCell)n.Object;
			if (cell != null && hTable.TableView!=null)
				hPath = hTable.TableView.IndexPathForCell (cell);
			//hPath = hTable.TableView.IndexPathForCell (cell);
			if (hPath != null) {
				PopoverViewController popoverContent = new PopoverViewController ();
				popoverContent.PopType = "sumColumn";
				popoverController = new UIPopoverController (popoverContent);
				popoverController.Delegate = new MyPopDelegate (this);
				popoverController.PopoverContentSize = new CGSize (250, 500);
				this.hTable.TableView.SelectRow (hPath, false, UITableViewScrollPosition.None);
				if (this.View.Window != null)
					popoverController.PresentFromRect (hTable.TableView.RectForRowAtIndexPath (hPath), hTable.TableView, UIPopoverArrowDirection.Any, true);
			}
		}

		public void handleRowSelection (NSNotification not)
		{
			NSNumber row = (NSNumber)not.Object;
			GlobalSettings.SelectedLine = GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == (int)row);
			if (!CommonClass.selectedRows.Contains (row.Int32Value)) {
				CommonClass.selectedRows.Add (row.Int32Value);
			} else {
				CommonClass.selectedRows.Remove (row.Int32Value);
			}
			if (CommonClass.selectedRows.Count != 0) {
				NSString str = new NSString ("none");
				NSNotificationCenter.DefaultCenter.PostNotificationName ("ButtonEnableDisable", str);
				foreach (int num in CommonClass.selectedRows) {
					foreach (Line line in GlobalSettings.Lines) {
						if (num == line.LineNum) {
							if (line.TopLock == false) {
								NSString str1 = new NSString ("TL");
								NSNotificationCenter.DefaultCenter.PostNotificationName ("ButtonEnableDisable", str1);
							}
							if (line.BotLock == false) {
								NSString str1 = new NSString ("BL");
								NSNotificationCenter.DefaultCenter.PostNotificationName ("ButtonEnableDisable", str1);
							}
						}
					}
				}
			} else {
				NSString str = new NSString ("none");
				NSNotificationCenter.DefaultCenter.PostNotificationName ("ButtonEnableDisable", str);
			}
		}

		private void loadSummaryListAndHeader ()
		{
			if (hTable != null) {
				DisposeClass.DisposeEx (hTable.View);
				//hTable.View.RemoveFromSuperview ();
				hTable.RemoveFromParentViewController ();
				hTable = null;
			}

			//if (sumList != null) {
			//	scrlPath = sumList.TableView.IndexPathForRowAtPoint (sumList.TableView.Bounds.Location);
				//DisposeClass.DisposeEx (sumList.View);

				////sumList.View.RemoveFromSuperview ();
				//sumList.RemoveFromParentViewController ();
			//sumList = null;
			//}
			//if (bidLineList != null) {
			//	scrlPath = bidLineList.TableView.IndexPathForRowAtPoint (bidLineList.TableView.Bounds.Location);
				//bidLineList.View.RemoveFromSuperview ();.
				//DisposeClass.DisposeEx (bidLineList.View);
				//bidLineList.RemoveFromParentViewController ();
				//bidLineList = null;
			//}
			//if (modernList != null) {
			//	scrlPath = modernList.TableView.IndexPathForRowAtPoint (modernList.TableView.Bounds.Location);
				//DisposeClass.DisposeEx (modernList.View);
				//modernList.View.RemoveFromSuperview ();
				//modernList.RemoveFromParentViewController ();
				//modernList = null;
			//}

			if (GlobalSettings.WBidINIContent.ViewType == 1 || GlobalSettings.WBidINIContent.ViewType == 0) {

				NSNotificationCenter.DefaultCenter.PostNotificationName("ReloadTableview", null);???

				vwSummaryContainer.Hidden = false;
				vwContainerView.Hidden = true;
				vwBidLineContainer.Hidden = true;

				//this.vwTable.Frame = new CGRect (0, 153, WBidHelper.ScreenWidth() , 660);

				hTable = new summaryHeaderListController ();
				this.AddChildViewController (hTable);
				this.vwHeader.AddSubview (hTable.View);
				hTable.View.BackgroundColor = UIColor.FromRGB (207, 226, 183);

				this.vwHeader.Layer.BorderWidth = 1;
				this.lblSEL.Layer.BorderWidth = 1;
				this.lblMOV.Layer.BorderWidth = 1;
				this.vwHeader.Layer.BorderColor = UIColor.FromRGB (158, 179, 131).CGColor;
				this.lblSEL.Layer.BorderColor = UIColor.FromRGB (158, 179, 131).CGColor;
				this.lblMOV.Layer.BorderColor = UIColor.FromRGB (158, 179, 131).CGColor;
				//sumList.TableView.ReloadData();
				sumList.reloadData();
               
				//sumList.ReloadTable();
				//scrlPath = sumList.TableView.IndexPathForRowAtPoint (sumList.TableView.Bounds.Location);
				       if (scrlPath != null)
					sumList.TableView.ScrollToRow (scrlPath, UITableViewScrollPosition.Top, false);
			

			}  else if (GlobalSettings.WBidINIContent.ViewType == 2) {

				vwBidLineContainer.Hidden = false;
				vwContainerView.Hidden = true;
				vwSummaryContainer.Hidden = true;
				//scrlPath = bidLineList.TableView.IndexPathForRowAtPoint(bidLineList.TableView.Bounds.Location);
				if (scrlPath != null)
					bidLineList.TableView.ScrollToRow (scrlPath, UITableViewScrollPosition.Top, false);

			}  else if (GlobalSettings.WBidINIContent.ViewType == 3) {

				vwContainerView.Hidden = false;
				vwSummaryContainer.Hidden = true;
				vwBidLineContainer.Hidden = true;
				//scrlPath = modernList.TableView.IndexPathForRowAtPoint(modernList.TableView.Bounds.Location);
				if (scrlPath != null)
					modernList.TableView.ScrollToRow (scrlPath, UITableViewScrollPosition.Top, false);
			}

		}

		public void handleButtonEnable (NSNotification not)
		{
			string str = not.Object.ToString ();
			if (str == "TL") {
				this.btnPromote.Enabled = true;
			} else if (str == "BL") {
				this.btnTrash.Enabled = true;
			} else {
				this.btnPromote.Enabled = false;
				this.btnTrash.Enabled = false;
			}
		}

		partial void btnHomeTap (UIKit.UIButton sender)
		{
			//foreach (NSObject obj in arrObserver)
			//{
			//    NSNotificationCenter.DefaultCenter.RemoveObserver(obj);
			//}
			//CommonClass.selectedRows.Clear();
			////UIApplication.SharedApplication.KeyWindow.RootViewController = new homeViewController();
			//UINavigationController navController = new UINavigationController();
			//homeViewController homeVC = new homeViewController();
			//UIApplication.SharedApplication.KeyWindow.RootViewController = navController;
			//navController.NavigationBar.BarStyle = UIBarStyle.Black;
			//navController.NavigationBar.Hidden = true;
			//navController.PushViewController(homeVC, false);
			//CommonClass.columnID = 0;

			//UpdateWBidStateContent();
			//save the state of the INI File



//            GlobalSettings.WBidINIContent.BidLineNormalColumns = GlobalSettings.BidlineAdditionalColumns.Where(x => x.IsSelected).Select(y => y.Id).ToList(); ;
//            GlobalSettings.WBidINIContent.BidLineVacationColumns = GlobalSettings.BidlineAdditionalvacationColumns.Where(x => x.IsSelected).Select(y => y.Id).ToList(); ;
//            GlobalSettings.WBidINIContent.ModernNormalColumns = GlobalSettings.ModernAdditionalColumns.Where(x => x.IsSelected).Select(y => y.Id).ToList(); ;
//            GlobalSettings.WBidINIContent.ModernVacationColumns = GlobalSettings.ModernAdditionalvacationColumns.Where(x => x.IsSelected).Select(y => y.Id).ToList(); ;
//           
           



			WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());

			StateManagement stateManagement = new StateManagement ();
			stateManagement.UpdateWBidStateContent ();
			//            WBidStateCollection stateFileContent = XmlHelper.DeserializeFromXml<WBidStateCollection>(WBidHelper.WBidStateFilePath);
			//            WBidState wBIdStateContent = stateFileContent.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			//            
			//			CompareState stateObj = new CompareState();
			//			bool isNochange = stateObj.CompareStateChange(wBIdStateContent, GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName));

			if (GlobalSettings.isModified) {
				GlobalSettings.WBidStateCollection.IsModified = true;
				UIActionSheet sheet;
				if (GlobalSettings.WBidINIContent.User.SmartSynch) {
					sheet = new UIActionSheet ("You want to save the changes before exit?", null, null, null, new string[] {
						"Save & Synch",
						"Save & Exit",
						"Exit"
					});
					sheet.ShowFrom (sender.Frame, tbTopBar, true);
					sheet.Dismissed += (object ob, UIButtonEventArgs e) => {
						if (e.ButtonIndex == 0) {
							GlobalSettings.WBidStateCollection.IsModified = true;
							WBidHelper.SaveStateFile (WBidHelper.WBidStateFilePath);

							//save the state of the INI File
							WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());

							GlobalSettings.isModified = false;
							btnSave.Enabled = false;

							SynchState ();

						} else if (e.ButtonIndex == 1) {
							// StateManagement stateManagement = new StateManagement();
							//stateManagement.UpdateWBidStateContent();
							GlobalSettings.WBidStateCollection.IsModified = true;
							WBidHelper.SaveStateFile (WBidHelper.WBidStateFilePath);

							//save the state of the INI File
							WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());

							GlobalSettings.isModified = false;
							btnSave.Enabled = false;

							CheckSmartSync ();

						} else if (e.ButtonIndex == 2) {
							//CheckSmartSync();
							GoToHome ();
						}
					};
				} else {
					sheet = new UIActionSheet ("You want to save the changes before exit?", null, null, null, new string[] {
						"Save & Exit",
						"Exit"
					});
					sheet.ShowFrom (sender.Frame, tbTopBar, true);
					sheet.Dismissed += (object ob, UIButtonEventArgs e) => {
						if (e.ButtonIndex == 0) {
							// StateManagement stateManagement = new StateManagement();
							//stateManagement.UpdateWBidStateContent();
							GlobalSettings.WBidStateCollection.IsModified = true;
							WBidHelper.SaveStateFile (WBidHelper.WBidStateFilePath);

							//save the state of the INI File
							WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());

							GlobalSettings.isModified = false;
							btnSave.Enabled = false;

							CheckSmartSync ();

						} else if (e.ButtonIndex == 1) {
							//CheckSmartSync();
							GoToHome ();
						}
					};
				}
			} else {
				if (GlobalSettings.WBidStateCollection.IsModified)
					CheckSmartSync ();
				else
					GoToHome ();
			}
			//if (timer != null)
			//{
			//	timer.Stop ();
			//	timer.Close();
			//	timer.Dispose();

				//timer.Start ();
			//}
			//save the state of the INI File
			// WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
		}

		public void UpdateWBidStateContent ()
		{
			//Save tag details to state file;


			if (GlobalSettings.Lines != null && GlobalSettings.Lines.Count > 0) {
				WBidState WBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

				WBidStateContent.TagDetails = new TagDetails ();
				WBidStateContent.TagDetails.AddRange (GlobalSettings.Lines.ToList ().Where (x => x.Tag != null && x.Tag.Trim () != string.Empty).Select (y => new Tag {
					Line = y.LineNum,
					Content = y.Tag
				}));

				int toplockcount = GlobalSettings.Lines.Where (x => x.TopLock == true).ToList ().Count;
				int bottomlockcount = GlobalSettings.Lines.Where (x => x.BotLock == true).ToList ().Count;
				//save the top and bottom lock
				WBidStateContent.TopLockCount = toplockcount;
				WBidStateContent.BottomLockCount = bottomlockcount;

				//Get the line oreder
				List<int> lineorderlist = GlobalSettings.Lines.Select (x => x.LineNum).ToList ();
				LineOrders lineOrders = new LineOrders ();
				int count = 1;
				lineOrders.Orders = lineorderlist.Select (x => new LineOrder () { LId = x, OId = count++ }).ToList ();
				lineOrders.Lines = lineorderlist.Count;
				WBidStateContent.LineOrders = lineOrders;




				if (GlobalSettings.FAEOMStartDate != null)
					WBidStateContent.FAEOMStartDate = GlobalSettings.FAEOMStartDate;
				else
					WBidStateContent.FAEOMStartDate = DateTime.MinValue.ToUniversalTime ();
			}


		}


		private void CheckSmartSync ()
		{
			if (GlobalSettings.SynchEnable && GlobalSettings.WBidStateCollection != null && GlobalSettings.WBidINIContent != null && GlobalSettings.WBidINIContent.User.SmartSynch) {
				UIAlertView syAlert = new UIAlertView ("Smart Sync", "Do you want to sync local changes with Server?", null, "NO", new string[] { "YES" });
				syAlert.Show ();
				syAlert.Dismissed += (object sender, UIButtonEventArgs e) => {
					if (e.ButtonIndex == 0) {
						GoToHome ();
					} else {
						SynchState ();
					}
				};
			} else {
				GoToHome ();
			}
		}

		private void GoToHome ()
		{
			foreach (NSObject obj in arrObserver) {
				NSNotificationCenter.DefaultCenter.RemoveObserver (obj);
			}
			CommonClass.selectedRows.Clear ();
			//UIApplication.SharedApplication.KeyWindow.RootViewController = new homeViewController();
//			UINavigationController navController = new UINavigationController ();
//			homeViewController homeVC = new homeViewController ();
//			UIApplication.SharedApplication.KeyWindow.RootViewController = navController;
//			navController.NavigationBar.BarStyle = UIBarStyle.Black;
//			navController.NavigationBar.Hidden = true;
//			navController.PushViewController (homeVC, false);
			//this.NavigationController.DismissViewController(true,null);

			this.NavigationController.PopViewController (true);
			CommonClass.columnID = 0;

			GlobalSettings.UndoStack.Clear ();
			GlobalSettings.RedoStack.Clear ();
			if (timer != null)
			{
				timer.Stop ();
				timer.Close();
				timer.Dispose();

				//timer.Start ();
			}


			foreach (UIView view in this.View.Subviews) {

				DisposeClass.DisposeEx(view);
			}
		}


		partial void btnPairingTapped (UIKit.UIButton sender)
		{
			PairingViewController pairingVC = new PairingViewController ();
			pairingVC.ModalPresentationStyle = UIModalPresentationStyle.PageSheet;
			this.PresentViewController (pairingVC, true, null);
		}

		public void ShowGroupBidAutomator(NSNotification n)
		{
			BaPopViewController pairingVC = new BaPopViewController ();
			pairingVC.ModalPresentationStyle = UIModalPresentationStyle.PageSheet;
			this.PresentViewController (pairingVC, true, null);
		}
		partial void btnOverlapTap (UIKit.UIButton sender)
		{
			sender.Selected = !sender.Selected;
			WBidHelper.PushToUndoStack ();
			UpdateUndoRedoButtons ();

//			ReCalculateLinePropertiesForOverlapCorrection reCalculateLinePropertiesForOverlapCorrection = new ReCalculateLinePropertiesForOverlapCorrection ();
			string overlayTxt = string.Empty;

			if (sender.Selected) {
				//  btnVacCorrect.Enabled = false;
				//  btnVacDrop.Enabled = false;
				//  btnEOM.Enabled = false;

				GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = false;
				GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
				GlobalSettings.MenuBarButtonStatus.IsEOM = false;
				GlobalSettings.MenuBarButtonStatus.IsOverlap = true;
				overlayTxt = "Applying Overlap Correction";

				SetVacButtonStates ();


				//reCalculateLinePropertiesForOverlapCorrection.ReCalculateLinePropertiesOnOverlapCorrection(GlobalSettings.Lines.ToList(), true);
				LoadingOverlay overlay = new LoadingOverlay (this.View.Frame, overlayTxt);
				this.View.Add (overlay);
				InvokeInBackground (() => {
					try {
//						reCalculateLinePropertiesForOverlapCorrection.ReCalculateLinePropertiesOnOverlapCorrection (GlobalSettings.Lines.ToList (), true);
//						SortLineList ();
						StateManagement statemanagement = new StateManagement ();
						WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
						statemanagement.RecalculateLineProperties (wBidStateContent);
						statemanagement.ApplyCSW (wBidStateContent);

					} catch (Exception ex) {
						InvokeOnMainThread (() => {
							throw ex;
						});
					}

					InvokeOnMainThread (() => {
						overlay.Hide ();
						NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
						GlobalSettings.isModified = true;
						CommonClass.lineVC.UpdateSaveButton ();
					});
				});
			} else {
				GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = false;
				GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
				GlobalSettings.MenuBarButtonStatus.IsEOM = false;
				GlobalSettings.MenuBarButtonStatus.IsOverlap = false;
				overlayTxt = "Removing Overlap Correction";

				SetVacButtonStates ();

				//reCalculateLinePropertiesForOverlapCorrection.ReCalculateLinePropertiesOnOverlapCorrection(GlobalSettings.Lines.ToList(), false);
				LoadingOverlay overlay = new LoadingOverlay (this.View.Frame, overlayTxt);
				this.View.Add (overlay);
				InvokeInBackground (() => {
//					reCalculateLinePropertiesForOverlapCorrection.ReCalculateLinePropertiesOnOverlapCorrection (GlobalSettings.Lines.ToList (), false);
					StateManagement statemanagement = new StateManagement ();
					WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
					statemanagement.RecalculateLineProperties (wBidStateContent);
					statemanagement.ApplyCSW (wBidStateContent);

					InvokeOnMainThread (() => {
						overlay.Hide ();
						NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
						GlobalSettings.isModified = true;
						CommonClass.lineVC.UpdateSaveButton ();
					});
				});

			}

			WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			wBIdStateContent.MenuBarButtonState.IsOverlap = GlobalSettings.MenuBarButtonStatus.IsOverlap;

			//NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
		}

		partial void btnVacCorrectTap (UIKit.UIButton sender)
		{
			try {
				// GlobalSettings.MenuBarButtonStatus.IsEOM = true;


				sender.Selected = !sender.Selected;
				WBidHelper.PushToUndoStack ();
				UpdateUndoRedoButtons ();
				if (sender.Selected) {
					//vacation button selected.
					GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = true;
					GlobalSettings.MenuBarButtonStatus.IsOverlap = false;
					modernList.reloadModernData();
				} else {


					//vacation button un selected.
					GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = false;
					if (GlobalSettings.MenuBarButtonStatus.IsEOM == false) {
						btnVacDrop.Selected = false;
						GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
						modernList.reloadModernData();
					}
					//GlobalSettings.MenuBarButtonStatus.IsOverlap = false;
				}

				SetVacButtonStates ();

				// GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = sender.Selected;
				//  if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection == false)
				//      GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;


				WBidCollection.GenarateTempAbsenceList ();
				string overlayTxt = string.Empty;
				if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection) {
					overlayTxt = "Applying Vacation Correction";
					//btnVacDrop.Enabled = true;

				} else {
					overlayTxt = "Removing Vacation Correction";
					//btnVacDrop.Enabled = false;

				}
//				foreach (var column in GlobalSettings.AdditionalColumns) {
//					column.IsSelected = false;
//				}
//				var selectedColumns = GlobalSettings.AdditionalColumns.Where (x => GlobalSettings.WBidINIContent.DataColumns.Any (y => y.Id == x.Id)).ToList ();
//				foreach (var selectedColumn in selectedColumns) {
//					selectedColumn.IsSelected = true;
//				}
//
//				foreach (var column in GlobalSettings.AdditionalvacationColumns) {
//					column.IsSelected = false;
//				}
//				var selectedVColumns = GlobalSettings.AdditionalColumns.Where (x => GlobalSettings.WBidINIContent.SummaryVacationColumns.Any (y => y.Id == x.Id)).ToList ();
//				foreach (var selectedColumn in selectedVColumns) {
//					selectedColumn.IsSelected = true;
//				}

				LoadingOverlay overlay = new LoadingOverlay (this.View.Frame, overlayTxt);
				this.View.Add (overlay);
				InvokeInBackground (() => {
					try {

//						PrepareModernBidLineView prepareModernBidLineView = new PrepareModernBidLineView ();
//						RecalcalculateLineProperties RecalcalculateLineProperties = new PortableLibrary.BusinessLogic.RecalcalculateLineProperties ();
//						prepareModernBidLineView.CalculatebidLinePropertiesforVacation ();
//						RecalcalculateLineProperties.CalcalculateLineProperties ();
//						SortLineList ();
						StateManagement statemanagement = new StateManagement ();
						WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
						statemanagement.RecalculateLineProperties (wBidStateContent);
						statemanagement.ApplyCSW (wBidStateContent);
					} catch (Exception ex) {
						InvokeOnMainThread (() => {

							throw ex;
						});
					}

					InvokeOnMainThread (() => {
						//NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
						loadSummaryListAndHeader ();
						modernList.reloadModernData();
						overlay.Hide ();
						GlobalSettings.isModified = true;
						CommonClass.lineVC.UpdateSaveButton ();
					});
				});
				WBidState wBidStateCont = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
				wBidStateCont.MenuBarButtonState.IsVacationCorrection = GlobalSettings.MenuBarButtonStatus.IsVacationCorrection;
			
			} catch (Exception ex) {

				throw ex;
			}
		}





		partial void btnVacDropTap (UIKit.UIButton sender)
		{

			sender.Selected = !sender.Selected;
			WBidHelper.PushToUndoStack ();
			UpdateUndoRedoButtons ();

			GlobalSettings.MenuBarButtonStatus.IsVacationDrop = sender.Selected;
			SetVacButtonStates ();


			WBidCollection.GenarateTempAbsenceList ();
			string overlayTxt = string.Empty;
			if (GlobalSettings.MenuBarButtonStatus.IsVacationDrop)
			{
				overlayTxt = "Applying Vacation Drop";
				VacationDropAction(overlayTxt);
			}
			else
			{
				

				UIAlertView alert = new UIAlertView ("WBidMax", "Careful : You have turned OFF the DRP button. The lines will be adjusted after you close this dialog. They will be adjusted to show that you are flying the VDF(red) and VDB(red).\n\nIf you have ???drop all??? selected as your preference in CWA, then you should turn back on the DRP button to see the lines as they will be after the VDF and VDB are dropped.\n\nIf this does not make sense, please go to Help menu and select Help to read about Vacation Corrections for Pilots and Flight Attendants.", null, "OK", null);
				alert.Show ();
				alert.Dismissed += (object sender1, UIButtonEventArgs e) => {
					if (e.ButtonIndex == 0) {
						overlayTxt = "Removing Vacation Drop";
						VacationDropAction(overlayTxt);
					} 


			
				};
			
			}
		}
		public void VacationDropAction(string overlayTxt1)
			{
				LoadingOverlay overlay = new LoadingOverlay (this.View.Frame, overlayTxt1);
				this.View.Add (overlay);
				InvokeInBackground (() => {
					try {
						//					PrepareModernBidLineView prepareModernBidLineView = new PrepareModernBidLineView ();
						//					RecalcalculateLineProperties RecalcalculateLineProperties = new PortableLibrary.BusinessLogic.RecalcalculateLineProperties ();
						//					prepareModernBidLineView.CalculatebidLinePropertiesforVacation ();
						//					RecalcalculateLineProperties.CalcalculateLineProperties ();
						//					SortLineList ();
						StateManagement statemanagement = new StateManagement ();
						WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
						statemanagement.RecalculateLineProperties (wBidStateContent);
						statemanagement.ApplyCSW (wBidStateContent);

					} catch (Exception ex) {
						InvokeOnMainThread (() => {
							throw ex;
						});
					}

					InvokeOnMainThread (() => {
						NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
						overlay.Hide ();
						GlobalSettings.isModified = true;
						CommonClass.lineVC.UpdateSaveButton ();
					});
				});
				WBidState wBidStateCont = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
				wBidStateCont.MenuBarButtonState.IsVacationDrop = GlobalSettings.MenuBarButtonStatus.IsVacationDrop;
			}
		partial void btnMILTapped (UIKit.UIButton sender)
		{
			if (GlobalSettings.MenuBarButtonStatus.IsMIL) {
				sender.Selected = false;
				WBidHelper.PushToUndoStack ();
				UpdateUndoRedoButtons ();
				LoadingOverlay overlay = new LoadingOverlay (this.View.Frame, "Removing MIL. Please wait.. ");
				this.View.Add (overlay);
				InvokeInBackground (() => {
					GlobalSettings.MenuBarButtonStatus.IsMIL = false;

					RecalcalculateLineProperties RecalcalculateLineProperties = new PortableLibrary.BusinessLogic.RecalcalculateLineProperties ();
					RecalcalculateLineProperties.CalcalculateLineProperties ();
					StateManagement statemanagement = new StateManagement ();
					WBidState wBidState = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
					statemanagement.ApplyCSW (wBidState);
					PrepareModernBidLineView prepareModernBidLineView = new PrepareModernBidLineView ();

					prepareModernBidLineView.CalculatebidLinePropertiesforVacation ();

					SortLineList ();
					InvokeOnMainThread (() => {
						GlobalSettings.isModified = true;
						CommonClass.lineVC.UpdateSaveButton ();
						SetVacButtonStates ();
						NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
						overlay.Hide ();
					});
				});
			} else {
				var milVC = new MILConfigViewController ();
				milVC.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				this.PresentViewController (milVC, true, null);
			}

			var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			wBidStateContent.MenuBarButtonState.IsMIL = GlobalSettings.MenuBarButtonStatus.IsMIL;
		}

		private static void SortLineList ()
		{
			SortCalculation sort = new SortCalculation ();
			WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			if (wBidStateContent.SortDetails != null && wBidStateContent.SortDetails.SortColumn != null && wBidStateContent.SortDetails.SortColumn != string.Empty) {
				sort.SortLines (wBidStateContent.SortDetails.SortColumn);
			}
		}

		partial void btnEOMTapped (UIKit.UIButton sender)
		{
			try 
			{
				string currentBidName = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();

				//string zipFileName = GenarateZipFileName();
				string vACFileName = WBidHelper.GetAppDataPath () + "//" + currentBidName + ".VAC";
				//Cheks the VAC file exists
				bool vacFileExists = File.Exists (vACFileName);

				if(GlobalSettings.CurrentBidDetails.Postion!="FA" && !vacFileExists && (GlobalSettings.WBidINIContent.User.IsSouthWestWifiTest || WBidHelper.IsSouthWestWifi()))
					{
					UIAlertView syAlert = new UIAlertView ("WBidMax", "You have a limited Wifi connection using the SouthwestWifi.You are not be able to do the Flight data download for EOM operation using SouthWest Wifi.", null, "OK", null);
					syAlert.Show ();
					}
					else
					{
				WBidHelper.PushToUndoStack ();
				UpdateUndoRedoButtons ();

				if (!sender.Selected) 
				{
					GlobalSettings.MenuBarButtonStatus.IsEOM = true;
//					WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
//					wBIdStateContent.MenuBarButtonState.IsEOM=true;

					SetVacButtonStates ();
					if (GlobalSettings.CurrentBidDetails.Postion == "FA") 
					{
						DateTime defDate = new DateTime (GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1);
						defDate.AddMonths (1);
						string[] strParams = {
							String.Format ("{0:m}", GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays (1)),
							String.Format ("{0:m}", GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays (2)),
							String.Format ("{0:m}", GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays (3))
						};
						UIActionSheet sheet = new UIActionSheet ("Where does your vacation start on next month?", null, null, null, strParams);
						sheet.ShowFrom (sender.Frame, tbBottomBar, true);
						//GlobalSettings.FAEOMStartDate = DateTime.MinValue;
						sheet.Clicked += handleEOMOptions;

					} 
					else
					{
						sender.Selected = true;
						//btnVacDrop.Enabled = true;

//						string currentBidName = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();
//
//						//string zipFileName = GenarateZipFileName();
//						string vACFileName = WBidHelper.GetAppDataPath () + "//" + currentBidName + ".VAC";
//						//Cheks the VAC file exists
//						bool vacFileExists = File.Exists (vACFileName);

						if (!vacFileExists) 
						{

							CreateEOMVacFileForCP (currentBidName);
						} 
						else 
						{



							string overlayTxt = string.Empty;
							if (GlobalSettings.MenuBarButtonStatus.IsEOM)
								overlayTxt = "Applying EOM";
							else
								overlayTxt = "Removing EOM";

							LoadingOverlay overlay = new LoadingOverlay (this.View.Frame, overlayTxt);
							this.View.Add (overlay);
							InvokeInBackground (() => {

								try {

									if (GlobalSettings.VacationData == null) {
										using (FileStream vacstream = File.OpenRead (vACFileName)) {

											Dictionary<string, TripMultiVacData> objineinfo = new Dictionary<string, TripMultiVacData> ();
											GlobalSettings.VacationData = ProtoSerailizer.DeSerializeObject (vACFileName, objineinfo, vacstream);
										}
									}


									GenerateVacationDataView ();

									InvokeOnMainThread (() => {
										loadSummaryListAndHeader ();
										// NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
										overlay.Hide ();
										GlobalSettings.isModified = true;
										CommonClass.lineVC.UpdateSaveButton ();
									});
								} catch (Exception ex) {
									InvokeOnMainThread (() => {
										throw ex;
									});
								}
							});

						}




					}
				} else {
					GlobalSettings.MenuBarButtonStatus.IsEOM = false;
					sender.Selected = false;
					if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection) {
						btnVacDrop.Selected = false;
						GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
					}

					SetVacButtonStates ();

					string overlayTxt = string.Empty;
					if (GlobalSettings.MenuBarButtonStatus.IsEOM)
						overlayTxt = "Applying EOM";
					else
						overlayTxt = "Removing EOM";

					LoadingOverlay overlay = new LoadingOverlay (this.View.Frame, overlayTxt);
					this.View.Add (overlay);
					InvokeInBackground (() => {
						try {
							GenerateVacationDataView ();
						} catch (Exception ex) {
							InvokeOnMainThread (() => {
								throw ex;
							});
						}

						InvokeOnMainThread (() => {
							loadSummaryListAndHeader ();
							//NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
							overlay.Hide ();
							GlobalSettings.isModified = true;
							CommonClass.lineVC.UpdateSaveButton ();
						});
					});

				}

				WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
				wBIdStateContent.MenuBarButtonState.IsEOM = GlobalSettings.MenuBarButtonStatus.IsEOM;
					}


			} catch (Exception ex) {

				throw ex;
			}

		}


		private static void GenerateVacationDataView ()
		{



//			WBidCollection.GenarateTempAbsenceList ();
//			PrepareModernBidLineView prepareModernBidLineView = new PrepareModernBidLineView ();
//			RecalcalculateLineProperties RecalcalculateLineProperties = new PortableLibrary.BusinessLogic.RecalcalculateLineProperties ();
//			prepareModernBidLineView.CalculatebidLinePropertiesforVacation ();
//			RecalcalculateLineProperties.CalcalculateLineProperties ();
//			SortLineList ();

			StateManagement statemanagement = new StateManagement ();
			WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			statemanagement.RecalculateLineProperties (wBidStateContent);
			statemanagement.ApplyCSW (wBidStateContent);

		}


		private void CreateEOMVacFileForCP (string currentBidName)
		{


			InvokeOnMainThread (() => {

				UIAlertView alert = new UIAlertView ("WBidMax", "WBidMax needs to download vacation data to make the predictions for your end of month trips (EOM VAC).   This could take up to a minute.  Do you want to continue?", null, "Cancel", new string[] { "Yes" });

				alert.Clicked += alert_Clicked;

				alert.Show ();
			});


		}


		void alert_Clicked (object sender, UIButtonEventArgs e)
		{

			try {

				if (e.ButtonIndex == 1) {
					btnVacDrop.Enabled = true;
					string overlayTxt = string.Empty;
					if (GlobalSettings.MenuBarButtonStatus.IsEOM)
						overlayTxt = "Applying EOM";
					else
						overlayTxt = "Removing EOM";

					LoadingOverlay overlay = new LoadingOverlay (this.View.Frame, overlayTxt);
					this.View.Add (overlay);
					InvokeInBackground (() => {
						CreateEOMVacationforCP ();

						GenerateVacationDataView ();

						InvokeOnMainThread (() => {
							loadSummaryListAndHeader ();
							// NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
							overlay.Hide ();
						});
					});
				} else if (e.ButtonIndex == 0) {
					if (!wBIdStateContent.IsVacationOverlapOverlapCorrection)
						btnVacDrop.Enabled = false;
					btnEOM.Selected = false;
					GlobalSettings.MenuBarButtonStatus.IsEOM = false;
					SetVacButtonStates ();
				}

			} catch (Exception ex) {

				throw ex;
			}
		}

		private static void CreateEOMVacationforCP ()
		{


			try {

				DateTime nextSunday = GetnextSunday ();


				GlobalSettings.OrderedVacationDays = new List<Absense> () { new Absense {
						StartAbsenceDate = nextSunday,
						EndAbsenceDate = nextSunday.AddDays (6),
						AbsenceType = "VA"
					}
				};

				string serverPath = GlobalSettings.WBidDownloadFileUrl + "FlightData.zip";
				string zipLocalFile = Path.Combine (WBidHelper.GetAppDataPath (), "FlightData.zip");
				string networkDataPath = WBidHelper.GetAppDataPath () + "/" + "FlightData.NDA";
				if(File.Exists(networkDataPath))
					File.Delete(networkDataPath);
				FlightPlan flightPlan = null;
				WebClient wcClient = new WebClient ();
				//Downloading networkdat file
				if(File.Exists(networkDataPath))
					File.Delete(networkDataPath);
				wcClient.DownloadFile (serverPath, zipLocalFile);

//                //Extracting the zip file
//                var zip = new ZipArchive();
//                zip.EasyUnzip(zipLocalFile, WBidHelper.GetAppDataPath(), true, "");

				string target = Path.Combine (WBidHelper.GetAppDataPath (), WBidHelper.GetAppDataPath () + "/");//+ Path.GetFileNameWithoutExtension(zipLocalFile))+ "/";

				if (!File.Exists(networkDataPath))
				{
					if (File.Exists(zipLocalFile))
					{
						
				ZipFile.ExtractToDirectory(zipLocalFile,target);
					}
				}
				//ZipStorer.

				// Open an existing zip file for reading
//				ZipStorer zip = ZipStorer.Open (zipLocalFile, FileAccess.Read);
//
//				// Read the central directory collection
//				List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir ();
//
//				// Look for the desired file
//				foreach (ZipStorer.ZipFileEntry entry in dir) {
//					zip.ExtractFile (entry, target + entry);
//				}
//				zip.Close ();

				//Deserializing data to FlightPlan object
				FlightPlan fp = new FlightPlan ();
				using (FileStream networkDatatream = File.OpenRead (networkDataPath)) {

					FlightPlan objineinfo = new FlightPlan ();
					flightPlan = ProtoSerailizer.DeSerializeObject (networkDataPath, fp, networkDatatream);

				}

				if (File.Exists (zipLocalFile)) {
					File.Delete (zipLocalFile);
				}
//				if (File.Exists (networkDataPath)) {
//					File.Delete (networkDataPath);
//				}



				VacationCorrectionParams vacationParams = new VacationCorrectionParams ();
				vacationParams.FlightRouteDetails = flightPlan.FlightRoutes.Join (flightPlan.FlightDetails, fr => fr.FlightId, f => f.FlightId,
					(fr, f) =>
                           new FlightRouteDetails {
						Flight = f.FlightId,
						FlightDate = fr.FlightDate,
						Orig = f.Orig,
						Dest = f.Dest,
						Cdep = f.Cdep,
						Carr = f.Carr,
						Ldep = f.Ldep,
						Larr = f.Larr,
						RouteNum = fr.RouteNum,

					}).ToList ();






				vacationParams.CurrentBidDetails = GlobalSettings.CurrentBidDetails;
				vacationParams.Trips = GlobalSettings.Trip.ToDictionary (x => x.TripNum, x => x);
				vacationParams.Lines = GlobalSettings.Lines.ToDictionary (x => x.LineNum.ToString (), x => x);
				vacationParams.IsEOM = true;
				//  VacationData = new Dictionary<string, TripMultiVacData>();


				//Performing vacation correction algoritham
				VacationCorrectionBL vacationBL = new VacationCorrectionBL ();
				GlobalSettings.VacationData = vacationBL.PerformVacationCorrection (vacationParams);


				if (GlobalSettings.VacationData != null) {

					string fileToSave = string.Empty;
					fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();


					// save the VAC file to app data folder

					var stream = File.Create (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".VAC");
					ProtoSerailizer.SerializeObject (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".VAC", GlobalSettings.VacationData, stream);
					stream.Dispose ();
					stream.Close ();
				} else {
					GlobalSettings.IsVacationCorrection = false;
				}
				GlobalSettings.OrderedVacationDays = null;

			} catch (Exception ex) {
				GlobalSettings.OrderedVacationDays = null;

				throw ex;
			}
		}



		public static DateTime GetnextSunday ()
		{
			DateTime date = GlobalSettings.CurrentBidDetails.BidPeriodEndDate;
			for (int count = 1; count <= 3; count++) {
				date = date.AddDays (1);
				if (date.DayOfWeek.ToString () == "Sunday")
					break;
			}


			return date;
		}

		private void CreateEOMforFA ()
		{
			if (GlobalSettings.FAEOMStartDate != null && GlobalSettings.FAEOMStartDate != DateTime.MinValue) {
				btnVacDrop.Enabled = true;
				string overlayTxt = string.Empty;
				if (GlobalSettings.MenuBarButtonStatus.IsEOM)
					overlayTxt = "Applying EOM";
				else
					overlayTxt = "Removing EOM";
				LoadingOverlay overlay = new LoadingOverlay (this.View.Frame, overlayTxt);
				this.View.Add (overlay);
				InvokeInBackground (() => {
					CreateEOMVacforFA ();
					GenerateVacationDataView ();
					InvokeOnMainThread (() => {
						loadSummaryListAndHeader ();
						//NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
						overlay.Hide ();
						GlobalSettings.isModified = true;
						CommonClass.lineVC.UpdateSaveButton ();
					});
				});
			}
		}

		void handleEOMOptions (object sender, UIButtonEventArgs e)
		{
			if (e.ButtonIndex == 0) {
				btnEOM.Selected = true;
				GlobalSettings.FAEOMStartDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays (1);
			} else if (e.ButtonIndex == 1) {
				btnEOM.Selected = true;
				GlobalSettings.FAEOMStartDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays (2);

			} else if (e.ButtonIndex == 2) {
				btnEOM.Selected = true;
				GlobalSettings.FAEOMStartDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays (3);

			} else {
				btnEOM.Selected = false;
				GlobalSettings.MenuBarButtonStatus.IsEOM = false;
				if (!btnEOM.Selected && !btnVacCorrect.Selected)
					btnVacDrop.Enabled = false;

			}
			//            else if (e.ButtonIndex == 3)
			//            {
			//                btnEOM.Selected = true;
			//                GlobalSettings.FAEOMStartDate = DateTime.MinValue;
			//
			//            }

			CreateEOMforFA ();
		}

		private void CreateEOMVacforFA ()
		{
			VacationCorrectionParams vacationParams = new VacationCorrectionParams ();
			vacationParams.CurrentBidDetails = GlobalSettings.CurrentBidDetails;
			vacationParams.Trips = GlobalSettings.Trip.ToDictionary (x => x.TripNum, x => x);
			vacationParams.Lines = GlobalSettings.Lines.ToDictionary (x => x.LineNum.ToString (), x => x);
			Dictionary<string, TripMultiVacData> allTripsMultiVacData = null;

			string currentBidName = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();


			string vACFileName = WBidHelper.GetAppDataPath () + "//" + currentBidName + ".VAC";
			//Cheks the VAC file exists
			bool vacFileExists = File.Exists (vACFileName);

			if (!vacFileExists) {
				allTripsMultiVacData = new Dictionary<string, TripMultiVacData> ();
			} else {

				using (FileStream vacstream = File.OpenRead (vACFileName)) {

					Dictionary<string, TripMultiVacData> objineinfo = new Dictionary<string, TripMultiVacData> ();
					allTripsMultiVacData = ProtoSerailizer.DeSerializeObject (vACFileName, objineinfo, vacstream);

				}
			}



			//Performing vacation correction algoritham
			VacationCorrectionBL vacationBL = new VacationCorrectionBL ();
			GlobalSettings.VacationData = vacationBL.CreateVACfileForEOMFA (vacationParams, allTripsMultiVacData);



			string fileToSave = string.Empty;
			fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();
			if (GlobalSettings.VacationData != null && GlobalSettings.VacationData.Count > 0) {




				// save the VAC file to app data folder

				var stream = File.Create (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".VAC");
				ProtoSerailizer.SerializeObject (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".VAC", GlobalSettings.VacationData, stream);
				stream.Dispose ();
				stream.Close ();


				CaculateVacationDetails calVacationdetails = new CaculateVacationDetails ();
				calVacationdetails.CalculateVacationdetailsFromVACfile (vacationParams.Lines, GlobalSettings.VacationData);

				//set the Vacpay,Vdrop,Vofont and VoBack columns in the line summary view 
				ManageVacationColumns managevacationcolumns = new ManageVacationColumns ();
				managevacationcolumns.SetVacationColumns ();

				LineInfo lineInfo = new LineInfo () {
					LineVersion = GlobalSettings.LineVersion,
					Lines = vacationParams.Lines

				};




				GlobalSettings.Lines = new System.Collections.ObjectModel.ObservableCollection<Line> (vacationParams.Lines.Select (x => x.Value));


				try {
					var linestream = File.Create (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".WBL");
					ProtoSerailizer.SerializeObject (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".WBL", lineInfo, linestream);
					linestream.Dispose ();
					linestream.Close ();
				} catch (Exception ex) {

					throw;
				}


				foreach (Line line in GlobalSettings.Lines) {
					if (line.ConstraintPoints == null)
						line.ConstraintPoints = new ConstraintPoints ();
					if (line.WeightPoints == null)
						line.WeightPoints = new WeightPoints ();
				}

			}






		}

		partial void btnResetTapped (UIKit.UIButton sender)
		{
			UIActionSheet sheet = new UIActionSheet ("This action will remove all top locks, bottom locks, constraints, weights, and set the sort to Line Number.  Do you want to continue?", null, null, "YES", null);
			sheet.ShowFrom (sender.Frame, this.tbTopBar, true);
			sheet.Clicked += HandleResetAll;
		}

		void HandleResetAll (object sender, UIButtonEventArgs e)
		{
			if (e.ButtonIndex == 0) {
				StateManagement stateManagement = new StateManagement ();
				stateManagement.UpdateWBidStateContent ();
				WBidHelper.PushToUndoStack ();

				LineOperations.RemoveAllTopLock ();
				LineOperations.RemoveAllBottomLock ();
				CommonClass.selectedRows.Clear ();
				this.vwCalPopover.Hidden = true;
				this.vwTripPopover.Hidden = true;
				var wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
				wBIdStateContent.SortDetails.SortColumn = "Line";
				CommonClass.columnID = 0;

				constCalc.ClearConstraints ();
				ConstraintsApplied.clearAll ();

				weightCalc.ClearWeights ();
				WeightsApplied.clearAll ();

				sort.SortLines ("Line");
				//clear group number
				GlobalSettings.Lines.ToList().ForEach(x => { x.BAGroup = string.Empty; x.IsGrpColorOn = 0; });

				if (wBIdStateContent.BidAuto != null)
				{
					wBIdStateContent.BidAuto.BAGroup = new List<BidAutoGroup>();

					//Reset Bid Automator settings.
					wBIdStateContent.BidAuto.BAFilter = new List<BidAutoItem>();

					wBIdStateContent.BidAuto.BASort = new SortDetails();
					//GlobalSettings.WBidStateContent.BidAuto.BASort=
				}

//				if (BidAutomatorViewModelInstance != null)
//				{
//					BidAutomatorViewModelInstance.ResetBidAutomatorDetails();
//				}


				NSString str = new NSString ("none");
				NSNotificationCenter.DefaultCenter.PostNotificationName ("ButtonEnableDisable", str);
				NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
				//NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);
				GlobalSettings.isModified = true;
				CommonClass.lineVC.UpdateSaveButton ();
			}
		}

		partial void btnCSWTap (UIKit.UIButton sender)
		{
			CSWViewController cswController = new CSWViewController ();
			CommonClass.cswVC = cswController;
			UINavigationController navController = new UINavigationController (cswController);
			navController.NavigationBar.BarStyle = UIBarStyle.Black;
			navController.NavigationBar.Hidden = true;
			this.PresentViewController (navController, true, null);
		}

		partial void BidAutomatorButtonClicked (NSObject sender)
		{
			//BidAutomator button clicked
			//Write code for reseting CSW
			var wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			if (wBIdStateContent!= null && GlobalSettings.CurrentBidDetails != null)
			{
				//var anyitemInCSW = CheckIfAnyItemSetInCsw(wBIdStateContent);
				if (CheckIfAnyItemSetInCsw(wBIdStateContent))
				{
			UIAlertView syAlert = new UIAlertView("Confirmation", "The Constraints, Top lock, Bottom lock etc from the CSW view will Reset. Do you want to continue to open Bid Automator ?", null, "Cancel", new string[] { "Ok" });
			syAlert.Show();
			syAlert.Dismissed += (object sender1, UIButtonEventArgs e1) =>
			{
				if (e1.ButtonIndex == 0)
				{

				}
				else
				{
					StateManagement stateManagement = new StateManagement ();
					stateManagement.UpdateWBidStateContent ();
					WBidHelper.PushToUndoStack ();

					LineOperations.RemoveAllTopLock ();
					LineOperations.RemoveAllBottomLock ();
					CommonClass.selectedRows.Clear ();
					this.vwCalPopover.Hidden = true;
					this.vwTripPopover.Hidden = true;
					wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
					wBIdStateContent.SortDetails.SortColumn = "Line";
					CommonClass.columnID = 0;

					constCalc.ClearConstraints ();
					ConstraintsApplied.clearAll ();

//					weightCalc.ClearWeights ();
//					WeightsApplied.clearAll ();

					sort.SortLines ("Line");

					NSString str = new NSString ("none");
					NSNotificationCenter.DefaultCenter.PostNotificationName ("ButtonEnableDisable", str);
					NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
					//NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);
					GlobalSettings.isModified = true;
					CommonClass.lineVC.UpdateSaveButton ();
				

					NavigateToBA();
				}
			};
				}
				else
				{
					NavigateToBA();
				}
			}


		}
		/// <summary>
		/// this will return true,If anyof the weights,contraints or sorts set
		/// </summary>
		/// <returns></returns>
		private bool CheckIfAnyItemSetInCsw(WBidState wBIdStateContent)
		{
			
			if (wBIdStateContent.Constraints.Hard)
				return true;
			if (wBIdStateContent.Constraints.Ready)
				return true;
			if (wBIdStateContent.Constraints.Reserve)
				return true;
			if (wBIdStateContent.Constraints.Blank)
				return true;
			if (wBIdStateContent.Constraints.International)
				return true;
			if (wBIdStateContent.Constraints.NonConus)
				return true;
			if (wBIdStateContent.CxWtState.ACChg.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.AMPM.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.AMPMMIX.AM)
				return true;
			else if (wBIdStateContent.CxWtState.AMPMMIX.PM)
				return true;
			else if (wBIdStateContent.CxWtState.AMPMMIX.MIX)
				return true;
			else if (wBIdStateContent.CxWtState.BDO.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.BulkOC.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.CL.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.CLAuto.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.DaysOfWeek.SUN)
				return true;
			else if (wBIdStateContent.CxWtState.DaysOfWeek.MON)
				return true;
			else if (wBIdStateContent.CxWtState.DaysOfWeek.TUE)
				return true;
			else if (wBIdStateContent.CxWtState.DaysOfWeek.WED)
				return true;
			else if (wBIdStateContent.CxWtState.DaysOfWeek.THU)
				return true;
			else if (wBIdStateContent.CxWtState.DaysOfWeek.FRI)
				return true;
			else if (wBIdStateContent.CxWtState.DaysOfWeek.SAT)
				return true;
			else if (wBIdStateContent.CxWtState.FaPosition.A)
				return true;
			else if (wBIdStateContent.CxWtState.FaPosition.B)
				return true;
			else if (wBIdStateContent.CxWtState.FaPosition.C)
				return true;
			else if (wBIdStateContent.CxWtState.FaPosition.D)
				return true;
			else if (wBIdStateContent.CxWtState.DHD.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.DHDFoL.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.DOW.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.DP.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.EQUIP.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.FLTMIN.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.GRD.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.InterConus.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.LEGS.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.LegsPerPairing.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.LrgBlkDaysOff.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.MP.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.No3on3off.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.NODO.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.NOL.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.NormalizeDays.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.PDAfter.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.PDBefore.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.PerDiem.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.Position.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.Rest.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.RON.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.SDO.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.SDOW.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.TL.Cx)
				return true;

			else if (wBIdStateContent.CxWtState.WB.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.WorkDay.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.WtPDOFS.Cx)
				return true;
			else if (wBIdStateContent.CxWtState.Commute.Cx)
				return true;
//			//weights
//
//			else if (wBIdStateContent.CxWtState.ACChg.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.AMPM.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.BDO.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.BulkOC.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.CL.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.DHD.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.DHDFoL.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.DOW.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.DP.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.EQUIP.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.FLTMIN.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.GRD.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.InterConus.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.LEGS.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.LegsPerPairing.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.LrgBlkDaysOff.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.MP.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.No3on3off.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.NODO.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.NOL.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.NormalizeDays.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.PDAfter.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.PDBefore.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.PerDiem.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.Position.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.Rest.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.RON.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.SDO.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.SDOW.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.TL.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.TripLength.FourDay)
//				return true;
//			else if (wBIdStateContent.CxWtState.TripLength.ThreeDay)
//				return true;
//			else if (wBIdStateContent.CxWtState.TripLength.Twoday)
//				return true;
//			else if (wBIdStateContent.CxWtState.TripLength.Turns)
//				return true;
//			else if (wBIdStateContent.CxWtState.WB.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.WorkDay.Wt)
//				return true;
//			else if (wBIdStateContent.CxWtState.WtPDOFS.Wt)
//				return true;
			//sorts
			else if (wBIdStateContent.SortDetails.SortColumn != "Line")
				return true;
			else if (GlobalSettings.Lines.Any(x => x.TopLock))
				return true;
			else if (GlobalSettings.Lines.Any(x => x.BotLock))
				return true;
			else
				return false;
		}
		public void NavigateToBA()
		{
			BAViewController baController = new BAViewController ();
			CommonClass.BAVC = baController;
			UINavigationController navController = new UINavigationController (baController);
			navController.NavigationBar.BarStyle = UIBarStyle.Black;
			navController.NavigationBar.Hidden = true;
			this.PresentViewController (navController, true, null);
		}
		partial void btnGridTap (UIKit.UIButton sender)
		{
			
			sender.Selected = !sender.Selected;
			CommonClass.showGrid = sender.Selected;
			//NSNotificationCenter.DefaultCenter.PostNotificationName("ReloadTableview", null);
			NSNotificationCenter.DefaultCenter.PostNotificationName ("DataCulumnsUpdated", null);
		}

		partial void btnPromoteTap (UIKit.UIButton sender)
		{
			StateManagement stateManagement = new StateManagement ();
			stateManagement.UpdateWBidStateContent ();
			WBidHelper.PushToUndoStack ();

			LineOperations.PromoteLines (CommonClass.selectedRows);
			CommonClass.selectedRows.Clear ();
			NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
			NSString str = new NSString ("none");
			NSNotificationCenter.DefaultCenter.PostNotificationName ("ButtonEnableDisable", str);
			GlobalSettings.isModified = true;
			CommonClass.lineVC.UpdateSaveButton ();
		}

		partial void btnSaveTap (UIKit.UIButton sender)
		{
			StateManagement stateManagement = new StateManagement ();
			stateManagement.UpdateWBidStateContent ();
			//			CompareState stateObj = new CompareState();
			//            string fileName = WBidHelper.GenerateFileNameUsingCurrentBidDetails();
			//             var WbidCollection = XmlHelper.ReadStateFile(WBidHelper.GetAppDataPath() + "/" + fileName + ".WBS");
			//             wBIdStateContent = WbidCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			//			bool isNochange = stateObj.CompareStateChange(wBIdStateContent, GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName));


			if (GlobalSettings.isModified) {
				GlobalSettings.WBidStateCollection.IsModified = true;
				WBidHelper.SaveStateFile (WBidHelper.WBidStateFilePath);
			
				if (timer != null) {
					timer.Stop ();
					timer.Start ();
				}
				//save the state of the INI File
				WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());

				GlobalSettings.isModified = false;
				btnSave.Enabled = false;
			}

		}

		partial void btnTrashTap (UIKit.UIButton sender)
		{
			StateManagement stateManagement = new StateManagement ();
			stateManagement.UpdateWBidStateContent ();
			WBidHelper.PushToUndoStack ();

			LineOperations.TrashLines (CommonClass.selectedRows);
			CommonClass.selectedRows.Clear ();
			NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
			NSString str = new NSString ("none");
			NSNotificationCenter.DefaultCenter.PostNotificationName ("ButtonEnableDisable", str);
			GlobalSettings.isModified = true;
			CommonClass.lineVC.UpdateSaveButton ();
		}

		partial void btnRemTopLockTap (UIKit.UIButton sender)
		{
			UIActionSheet sheet = new UIActionSheet ("This action will remove all top locks, and change the sort to ???Manual???.  Do you want to continue?", null, null, "YES", null);
			sheet.ShowFrom (sender.Frame, this.tbTopBar, true);
			sheet.Clicked += HandleRemoveToplock;
		}

		void HandleRemoveToplock (object sender, UIButtonEventArgs e)
		{
			if (e.ButtonIndex == 0) {
				StateManagement stateManagement = new StateManagement ();
				stateManagement.UpdateWBidStateContent ();
				WBidHelper.PushToUndoStack ();

				LineOperations.RemoveAllTopLock ();
				CommonClass.selectedRows.Clear ();
				NSString str = new NSString ("none");
				NSNotificationCenter.DefaultCenter.PostNotificationName ("ButtonEnableDisable", str);
				this.vwCalPopover.Hidden = true;
				this.vwTripPopover.Hidden = true;
				var wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
				wBIdStateContent.SortDetails.SortColumn = "Manual";
				CommonClass.columnID = 0;
				NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
				//NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);
				GlobalSettings.isModified = true;
				CommonClass.lineVC.UpdateSaveButton ();
			}
		}

		partial void btnRemBottomLockTap (UIKit.UIButton sender)
		{
			UIActionSheet sheet = new UIActionSheet ("This action will remove all bottom locks, and change the sort to ???Manual???.  Do you want to continue?", null, null, "YES", null);
			sheet.ShowFrom (sender.Frame, this.tbTopBar, true);
			sheet.Clicked += HandleRemoveBottomlock;
		}

		void HandleRemoveBottomlock (object sender, UIButtonEventArgs e)
		{
			if (e.ButtonIndex == 0) {
				StateManagement stateManagement = new StateManagement ();
				stateManagement.UpdateWBidStateContent ();
				WBidHelper.PushToUndoStack ();

				LineOperations.RemoveAllBottomLock ();
				CommonClass.selectedRows.Clear ();
				NSString str = new NSString ("none");
				NSNotificationCenter.DefaultCenter.PostNotificationName ("ButtonEnableDisable", str);
				btnRemBottomLock.Enabled = false;
				this.vwCalPopover.Hidden = true;
				this.vwTripPopover.Hidden = true;
				var wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
				wBIdStateContent.SortDetails.SortColumn = "Manual";
				CommonClass.columnID = 0;
				NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
				//NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);
				GlobalSettings.isModified = true;
				CommonClass.lineVC.UpdateSaveButton ();
			}
		}

		partial void btnMiscTap (UIKit.UIButton sender)
		{
			string[] arr = { "Configuration", "Change User Information", "Latest News","My Subscription" };
			UIActionSheet sheet = new UIActionSheet ("Select", null, "Cancel", null, arr);
			sheet.ShowFrom (sender.Frame, this.tbTopBar, true);
			sheet.Dismissed += handleMIscTap;
		}

		public void handleMIscTap (object sender, UIButtonEventArgs e)
		{
			if (e.ButtonIndex == 0) {
				ConfigTabBarControlller config = new ConfigTabBarControlller (false);
				config.ModalPresentationStyle = UIModalPresentationStyle.PageSheet;
				this.PresentViewController (config, true, null);
			} else if (e.ButtonIndex == 1) {
				userRegistrationViewController regs = new userRegistrationViewController ();
				regs.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				this.PresentViewController (regs, true, null);
			} else if (e.ButtonIndex == 2) {
				if (File.Exists (WBidHelper.GetAppDataPath () + "/" + "news.pdf")) {
					webPrint fileViewer = new webPrint ();
					this.PresentViewController (fileViewer, true, () => {
						fileViewer.LoadPDFdocument ("news.pdf");
					});
				} else {
					UIAlertView alert = new UIAlertView ("WBidMax", "No latest News found!", null, "OK", null);
					alert.Show ();
				}
			}
			else if (e.ButtonIndex == 3) 
			{
				SubscriptionViewController ObjSubscription = new SubscriptionViewController ();
				ObjSubscription.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				this.PresentViewController (ObjSubscription, true, null);

			}
		}

		partial void btnHelpTap (UIKit.UIButton sender)
		{
			string[] arr = { "Help", "Walkthrough", "Contact Us", "About" };
			UIActionSheet sheet = new UIActionSheet ("Select", null, "Cancel", null, arr);
			sheet.ShowFrom (sender.Frame, this.tbTopBar, true);
			sheet.Dismissed += handleHelp;
		}

		void handleHelp (object sender, UIButtonEventArgs e)
		{
			if (e.ButtonIndex == 0) {
				HelpViewController helpVC = new HelpViewController ();
				helpVC.pdfFileName = "Constraints";
				UINavigationController navCont = new UINavigationController (helpVC);
				navCont.NavigationBar.BarStyle = UIBarStyle.Black;
				navCont.NavigationBar.Hidden = true;
				this.PresentViewController (navCont, true, null);
			} else if (e.ButtonIndex == 1) {
				WalkthroughViewController introV = new WalkthroughViewController ();
				introV.home = new homeViewController ();
				introV.ModalPresentationStyle = UIModalPresentationStyle.PageSheet;
				this.PresentViewController (introV, true, null);
			} else if (e.ButtonIndex == 2) {
				ContactUsViewController contactVC = new ContactUsViewController ();
				contactVC.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				this.PresentViewController (contactVC, true, null);
			} else if (e.ButtonIndex == 3) {
				AboutViewController aboutVC = new AboutViewController ();
				aboutVC.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				this.PresentViewController (aboutVC, true, null);
			}
		}

		partial void btnBrightnessTapped (UIKit.UIButton sender)
		{
			BrightnessViewController popoverContent = new BrightnessViewController ();
			popoverController = new UIPopoverController (popoverContent);
			popoverController.Delegate = new MyPopDelegate (this);
			popoverController.PopoverContentSize = new CGSize (300, 100);
			popoverController.PresentFromRect (btnBrightness.Frame, tbTopBar, UIPopoverArrowDirection.Any, true);

		}

		partial void btnBidStuffTap (UIKit.UIButton sender)
		{

			//string[] arr=new string[12];
			if(GlobalSettings.CurrentBidDetails.Postion=="CP" || GlobalSettings.CurrentBidDetails.Postion=="FO")
			{
			Console.WriteLine (sender.TitleLabel.Text);
				string[] arr = new string[]{
					"Submit Current Bid Order",
					"Bid Editor",
					"View Cover Letter",
					"View Seniority List",
					"View Awards",
					"Get Awards",
					"Show Bid Reciept",
					"Print Bid Reciept",
					"Show CAP"

				};
				UIActionSheet sheet = new UIActionSheet ("Select", null, "Cancel", null, arr);
				sheet.ShowFrom (sender.Frame, this.tbTopBar, true);
				sheet.Dismissed += handleBidStuffTap;
			}
			else
			{
				string[] arr= new string[]{
					"Submit Current Bid Order",
					"Bid Editor",
					"View Cover Letter",
					"View Seniority List",
					"View Awards",
					"Get Awards",
					"Show Bid Reciept",
					"Print Bid Reciept"
				};
				UIActionSheet sheet = new UIActionSheet ("Select", null, "Cancel", null, arr);
				sheet.ShowFrom (sender.Frame, this.tbTopBar, true);
				sheet.Dismissed += handleBidStuffTap;
			}

		}
		private void SouthWestWifiAlert()
		{
			if (GlobalSettings.WBidINIContent.User.IsSouthWestWifiTest == false) {
				if (Reachability.IsHostReachable (GlobalSettings.ServerUrl)) {



				} else 
				{
					if (WBidHelper.IsSouthWestWifi()) {
						UIAlertView syAlert = new UIAlertView ("WBidMax", GlobalSettings.SouthWestWifiMessage, null, "OK", null);
						syAlert.Show ();

					} else {

						UIAlertView alertVW = new UIAlertView ("WBidMax", "Connectivity not available", null, "OK", null);
						alertVW.Show ();
					}
				}
			}
			else
			{
				UIAlertView alertVW = new UIAlertView ("WBidMax", GlobalSettings.SouthWestWifiMessage, null, "OK", null);
				alertVW.Show ();
			}

		}
		public void handleBidStuffTap (object sender, UIButtonEventArgs e)
		{
			if (e.ButtonIndex == 0) {
				if (GlobalSettings.WBidStateCollection.DataSource != "HistoricalData") 
				{


					//check blank lines are in low to high order. alert if it not satisfy the condtion
					List<int> sortedblanklines=GlobalSettings.Lines.Where(x=>x.BlankLine).Select(y=>y.LineNum).OrderBy(z=>z).ToList();
					List<int> currentblanklines=GlobalSettings.Lines.Where(x=>x.BlankLine).Select(y=>y.LineNum).ToList();
					bool isBlankLinesCorrectOrder = true;
					for (int i = 0; i < sortedblanklines.Count; i++) {
						if (sortedblanklines [i] != currentblanklines [i]) {
							isBlankLinesCorrectOrder = false;
							break;
						}

					}
					//if the user is connected to south west wifi, show an alert that user is having limited internet connection
					SouthWestWifiAlert ();

					if (!isBlankLinesCorrectOrder) 
					{
						UIAlertView syAlert = new UIAlertView("WbidMax", "Your blank lines are not in order of lowest to highest. Touch Cancel to go back and fix this issue.?", null, "Cancel", new string[] { "Ok" });
						syAlert.Show();
						syAlert.Dismissed += (object sender1, UIButtonEventArgs e1) =>
							{
							if (e1.ButtonIndex == 0)
							{
								
							}
							else
							{
								SubmitBidViewController submitBid = new SubmitBidViewController ();
								submitBid.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
								UINavigationController nav = new UINavigationController (submitBid);
								nav.NavigationBar.BarTintColor = ColorClass.TopHeaderColor;
								nav.NavigationBar.TitleTextAttributes = new UIStringAttributes () { ForegroundColor = UIColor.White };
								nav.NavigationBar.TintColor = UIColor.White;
								nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
								this.PresentViewController (nav, true, null);
							}
						};
					}
					else
					{
					//Bid view should come up here
					SubmitBidViewController submitBid = new SubmitBidViewController ();
					submitBid.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
					UINavigationController nav = new UINavigationController (submitBid);
					nav.NavigationBar.BarTintColor = ColorClass.TopHeaderColor;
					nav.NavigationBar.TitleTextAttributes = new UIStringAttributes () { ForegroundColor = UIColor.White };
					nav.NavigationBar.TintColor = UIColor.White;
					nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
					this.PresentViewController (nav, true, null);
					}
				} 
				else 
				{
					UIAlertView alert = new UIAlertView ("WBidMax", "You can not submit bids for Historical data", null, "OK", null);
					alert.Show ();
				}
				} 
				else if (e.ButtonIndex == 1) {




				if (GlobalSettings.WBidStateCollection.DataSource != "HistoricalData") {


					//check blank lines are in low to high order. alert if it not satisfy the condtion
					List<int> sortedblanklines=GlobalSettings.Lines.Where(x=>x.BlankLine).Select(y=>y.LineNum).OrderBy(z=>z).ToList();
					List<int> currentblanklines=GlobalSettings.Lines.Where(x=>x.BlankLine).Select(y=>y.LineNum).ToList();
					bool isBlankLinesCorrectOrder = true;
					for (int i = 0; i < sortedblanklines.Count; i++) {
						if (sortedblanklines [i] != currentblanklines [i]) {
							isBlankLinesCorrectOrder = false;
							break;
						}

					}
					//if the user is connected to south west wifi, show an alert that user is having limited internet connection
					SouthWestWifiAlert ();
					if (!isBlankLinesCorrectOrder) {
						UIAlertView syAlert = new UIAlertView ("WbidMax", "Your blank lines are not in order of lowest to highest. Touch Cancel to go back and fix this issue.?", null, "Cancel", new string[] { "Ok" });
						syAlert.Show ();
						syAlert.Dismissed += (object sender1, UIButtonEventArgs e1) => {
							if (e1.ButtonIndex == 0) {

							} else {
								bidEditorPrepViewController bidEditor = new bidEditorPrepViewController ();
								bidEditor.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
								UINavigationController nav = new UINavigationController (bidEditor);
								nav.NavigationBarHidden = true;
								nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
								this.PresentViewController (nav, true, () => {
									notif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("bidEditorPrepared"), bidEditorPrepared);


								});
							}
						};
					} else {
						bidEditorPrepViewController bidEditor = new bidEditorPrepViewController ();
						bidEditor.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
						UINavigationController nav = new UINavigationController (bidEditor);
						nav.NavigationBarHidden = true;
						nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
						this.PresentViewController (nav, true, () => {
							notif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("bidEditorPrepared"), bidEditorPrepared);
						});
					}

				} 
				else 
				{
					UIAlertView alert = new UIAlertView ("WBidMax", "You can not submit bids for Historical data", null, "OK", null);
					alert.Show ();
				}
			} else if (e.ButtonIndex == 2) {
				//cover
				viewFileViewController viewFile = new viewFileViewController ();
				UINavigationController nav = new UINavigationController (viewFile);
				nav.NavigationBarHidden = true;
				nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				viewFile.displayType = "cover";
				this.PresentViewController (nav, true, () => {
				});
			} else if (e.ButtonIndex == 3) {
				//seniority
				viewFileViewController viewFile = new viewFileViewController ();
				//viewFile.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				UINavigationController nav = new UINavigationController (viewFile);
				nav.NavigationBarHidden = true;
				nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				viewFile.displayType = "seniority";
				this.PresentViewController (nav, true, () => {
				});
			} else if (e.ButtonIndex == 4) {
				//Awards
				viewFileViewController viewFile = new viewFileViewController ();
				//viewFile.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				UINavigationController nav = new UINavigationController (viewFile);
				nav.NavigationBarHidden = true;
				nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				viewFile.displayType = "awards";
				this.PresentViewController (nav, true, () => {
				});
			} else if (e.ButtonIndex == 5) {
				RetrieveAwardViewController award = new RetrieveAwardViewController ();
				award.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				UINavigationController nav = new UINavigationController (award);
				nav.NavigationBarHidden = true;
				nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				this.PresentViewController (nav, true, null);
			} else if (e.ButtonIndex == 6) {
				string path = WBidHelper.GetAppDataPath ();
				List<string> linefilenames = Directory.EnumerateFiles (path, "*.*", SearchOption.AllDirectories).Select (Path.GetFileName).Where (s => s.ToLower ().EndsWith (".rct") || s.ToLower ().EndsWith (".pdf")).ToList ();
				linefilenames.Remove ("news.pdf");
				//List<string> linefilenames = Directory.EnumerateFiles (path, "*.RCT", SearchOption.AllDirectories).Select (Path.GetFileName).ToList ();
				if (linefilenames.Count > 1) {
					BidRecieptViewController reciept = new BidRecieptViewController ();
					reciept.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
					reciept.isPrintView = false;
					this.PresentViewController (reciept, true, null);
				} else if (linefilenames.Count == 1) {
					InvokeOnMainThread (() => {
						webPrint fileViewer = new webPrint ();

						this.PresentViewController(fileViewer, true, () =>
							{
								if(Path.GetExtension(linefilenames [0]).ToLower()==".rct")
								{
									fileViewer.loadFileFromUrl(linefilenames [0]);
								}
								else
									fileViewer.LoadPDFdocument (Path.GetFileName(linefilenames [0]));
							});

					});
				} else if (linefilenames.Count == 0) {
					UIAlertView alert = new UIAlertView ("WBidMax", "There is no bid reciept available..!", null, "OK", null);
					alert.Show ();
				}
			} else if (e.ButtonIndex == 7) 
			{
                string path = WBidHelper.GetAppDataPath ();
				List<string> linefilenames = Directory.EnumerateFiles (path, "*.*", SearchOption.AllDirectories).Select (Path.GetFileName).Where (s => s.ToLower ().EndsWith (".rct") || s.ToLower ().EndsWith (".pdf")).ToList ();
				linefilenames.Remove ("news.pdf");
				//List<string> linefilenames = Directory.EnumerateFiles (path, "*.RCT", SearchOption.AllDirectories).Select (Path.GetFileName).ToList ();
				if (linefilenames.Count > 1) {
					BidRecieptViewController reciept = new BidRecieptViewController ();
					reciept.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
					reciept.isPrintView = true;
					this.PresentViewController (reciept, true, null);
				} else if (linefilenames.Count == 1) {
					InvokeOnMainThread (() => {
						CommonClass.PrintReceipt ((UIView)btnBidStuff,linefilenames [0]);	
					});
				} else if (linefilenames.Count == 0) {
					UIAlertView alert = new UIAlertView ("WBidMax", "There is no bid reciept available..!", null, "OK", null);
					alert.Show ();
				}
			}
            else if (e.ButtonIndex == 8) {
				if (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO") {
					if (Reachability.IsHostReachable (GlobalSettings.ServerUrl)) {
						CAPViewController capdetails = new CAPViewController ();
						capdetails.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
						UINavigationController nav = new UINavigationController (capdetails);
						nav.NavigationBarHidden = true;
						nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
						this.PresentViewController (nav, true, null);
					} else {
						UIAlertView alert = new UIAlertView ("WBidMax", "Please check your Internet connection.", null, "OK", null);
						alert.Show ();
					}
				}              
            }
			UIActionSheet obj = (UIActionSheet)sender;
			obj.Dispose ();
		}

		public void bidEditorPrepared (NSNotification n)
		{
			string selectedPosition = n.Object.ToString ();
			if (selectedPosition == "CP" || selectedPosition == "FO") {
				BidEditorForPilot pilotBid = new BidEditorForPilot ();
				UINavigationController nav = new UINavigationController (pilotBid);
				nav.NavigationBar.BarStyle = UIBarStyle.Black;
				nav.NavigationBar.Hidden = true;
				this.PresentViewController (nav, true, null);
			} else if (selectedPosition == "FA") {
				if (GlobalSettings.CurrentBidDetails.Postion == "FA") {
					if (GlobalSettings.CurrentBidDetails.Round == "M") {
						BidEditorForFA faBid = new BidEditorForFA ();
						UINavigationController nav = new UINavigationController (faBid);
						nav.NavigationBar.BarStyle = UIBarStyle.Black;
						nav.NavigationBar.Hidden = true;
						this.PresentViewController (nav, true, null);
					} else if (GlobalSettings.CurrentBidDetails.Round == "S") {
						BidEditorForPilot pilotBid = new BidEditorForPilot ();
						UINavigationController nav = new UINavigationController (pilotBid);
						nav.NavigationBar.BarStyle = UIBarStyle.Black;
						nav.NavigationBar.Hidden = true;
						this.PresentViewController (nav, true, null);
					}

				}
				//else
				//{
				//    UIAlertView alert = new UIAlertView("FA Bid Editor", "Invalid Bid Editor Initailization", null,"OK",null);
				//    alert.Show();
				//}
			}
			NSNotificationCenter.DefaultCenter.RemoveObserver (notif);
		}

		partial void btnLineBotLockTap (UIKit.UIButton sender)
		{
			StateManagement stateManagement = new StateManagement ();
			stateManagement.UpdateWBidStateContent ();
			WBidHelper.PushToUndoStack ();

			LineOperations.TrashLines (CommonClass.doubleTapLine);
			sumList.TableView.DeselectRow (lPath, true);
			NSString str = new NSString ("none");
			NSNotificationCenter.DefaultCenter.PostNotificationName ("ButtonEnableDisable", str);

			//            NSIndexPath path;
			//            if (lPath.Row != this.sumList.TableView.NumberOfRowsInSection(0) - 1)
			//                path = NSIndexPath.FromRowSection(lPath.Row, lPath.Section);
			//            else
			//                path = lPath;
			//            lPath = path;
			NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
			sumList.TableView.SelectRow (lPath, true, UITableViewScrollPosition.None);
			this.calPopover (lPath);

			CGPoint point = sumList.TableView.RectForRowAtIndexPath (lPath).Location;
			point = sumList.TableView.ConvertPointToView (point, this.vwTable);
			CGPoint offset = sumList.TableView.ContentOffset;
			if (point.Y > 580)
				sumList.TableView.ScrollToRow (NSIndexPath.FromRowSection (lPath.Row, lPath.Section), UITableViewScrollPosition.Bottom, true);

			GlobalSettings.isModified = true;
			CommonClass.lineVC.UpdateSaveButton ();
		}

		partial void btnLineTopLockTap (UIKit.UIButton sender)
		{
			StateManagement stateManagement = new StateManagement ();
			stateManagement.UpdateWBidStateContent ();
			WBidHelper.PushToUndoStack ();

			LineOperations.PromoteLines (CommonClass.doubleTapLine);
			sumList.TableView.DeselectRow (lPath, true);
			NSString str = new NSString ("none");
			NSNotificationCenter.DefaultCenter.PostNotificationName ("ButtonEnableDisable", str);

			NSIndexPath path;
			if (lPath.Row != this.sumList.TableView.NumberOfRowsInSection (0) - 1)
				path = NSIndexPath.FromRowSection (lPath.Row + 1, lPath.Section);
			else
				path = lPath;
			lPath = path;
			NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
			sumList.TableView.SelectRow (lPath, true, UITableViewScrollPosition.None);
			this.calPopover (lPath);

			CGPoint point = sumList.TableView.RectForRowAtIndexPath (lPath).Location;
			point = sumList.TableView.ConvertPointToView (point, this.vwTable);
			CGPoint offset = sumList.TableView.ContentOffset;
			if (point.Y > 580)
				sumList.TableView.ScrollToRow (NSIndexPath.FromRowSection (lPath.Row, lPath.Section), UITableViewScrollPosition.Bottom, true);

			GlobalSettings.isModified = true;
			CommonClass.lineVC.UpdateSaveButton ();
		}

		partial void btnMovDownTap (UIKit.UIButton sender)
		{
			moveDown ();
		}

		private void moveDown ()
		{
			sumList.TableView.DeselectRow (lPath, true);
			NSIndexPath path = NSIndexPath.FromRowSection (lPath.Row + 1, lPath.Section);
			lPath = path;
			sumList.TableView.SelectRow (lPath, true, UITableViewScrollPosition.None);
			this.calPopover (lPath);

			CGPoint point = sumList.TableView.RectForRowAtIndexPath (lPath).Location;
			point = sumList.TableView.ConvertPointToView (point, this.vwTable);
			CGPoint offset = sumList.TableView.ContentOffset;
			if (point.Y > 580)
				sumList.TableView.ScrollToRow (NSIndexPath.FromRowSection (lPath.Row, lPath.Section), UITableViewScrollPosition.Bottom, true);

		}

		partial void btnMovUpTap (UIKit.UIButton sender)
		{
			moveUp ();
		}

		private void moveUp ()
		{
			sumList.TableView.DeselectRow (lPath, true);
			NSIndexPath path = NSIndexPath.FromRowSection (lPath.Row - 1, lPath.Section);
			lPath = path;
			sumList.TableView.SelectRow (lPath, true, UITableViewScrollPosition.None);
			this.calPopover (lPath);

			CGPoint point = sumList.TableView.RectForRowAtIndexPath (lPath).Location;
			point = sumList.TableView.ConvertPointToView (point, this.vwTable);
			CGPoint offset = sumList.TableView.ContentOffset;
			if (point.Y < 50)
				sumList.TableView.ScrollToRow (NSIndexPath.FromRowSection (lPath.Row, lPath.Section), UITableViewScrollPosition.Top, true);

		}

		/// <summary>
		/// PURPOSE : Toggles the Line view Type. 
		/// </summary>
		partial void sgControlViewTypeTap (UIKit.UISegmentedControl sender)
		{
			CommonClass.selectedTrip = null;
			// Segmented Control action for view change.
			if (sender.SelectedSegment == 0) {
				// Summary view.
				CommonClass.MainViewType = "Summary";
				GlobalSettings.WBidINIContent.ViewType = 1;
				this.loadSummaryListAndHeader ();
			} else if (sender.SelectedSegment == 1) {
				// Bidline View.
				CommonClass.MainViewType = "Bidline";
				GlobalSettings.WBidINIContent.ViewType = 2;
				this.loadSummaryListAndHeader ();
			} else if (sender.SelectedSegment == 2) {
				// Modern View.
				CommonClass.MainViewType = "Modern";
				GlobalSettings.WBidINIContent.ViewType = 3;
				this.loadSummaryListAndHeader ();
			}
			this.vwCalPopover.Hidden = true;
			this.vwTripPopover.Hidden = true;

		}

		///// <summary>
		///// PURPOSE : Set Application Title 
		///// </summary>
		//public void SetTitile()
		//{

		//    string domicile = GlobalSettings.CurrentBidDetails.Domicile;
		//    string position = GlobalSettings.CurrentBidDetails.Postion;
		//    System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
		//    string strMonthName = mfi.GetMonthName(GlobalSettings.CurrentBidDetails.Month).ToString();
		//    string round = GlobalSettings.CurrentBidDetails.Round == "M" ? "1st Round" : "2nd Round";
		//    string dataSource = string.Empty;
		//    string stateName = string.Empty;
		//    string seniority = string.Empty;
		//    string wBidiTitle;
		//    //will need to enable after the state file implemntation
		//    if (GlobalSettings.WBidStateCollection != null && GlobalSettings.WBidStateCollection.DataSource != "Original")
		//    {
		//        dataSource = " (Mock - Data)";

		//    }

		//    //if (GlobalSettings.WBidStateContent != null && GlobalSettings.WBidStateContent.StateName != "Default")
		//    //{
		//    //    stateName = " State : " + GlobalSettings.WBidStateContent.StateName;

		//    //}
		//    string equipment = string.Empty;
		//    if (GlobalSettings.WBidINIContent != null && GlobalSettings.WBidINIContent.Equipments != null && GlobalSettings.WBidINIContent.Equipments.Count > 0)
		//        equipment = GlobalSettings.WBidINIContent.Equipments[0].EquipmentNumber.ToString();
		//    //if (IsOverlapCorrection)
		//    //{
		//    //    WBidiTitle = domicile + "/" + position + "/" + equipment + " " + round + "  Line for " + strMonthName + " " + GlobalSettings.CurrentBidDetails.Year + "( Corrected for Overlap )";
		//    //}
		//    //else
		//    //{
		//        wBidiTitle = domicile + " - " + position + " - "  + round + " - " + strMonthName + " - " + GlobalSettings.CurrentBidDetails.Year;
		//    //}
		//   var qamode = string.Empty;
		//   if (GlobalSettings.buddyBidTest)
		//   {
		//       qamode = "  ( QA Mode )";
		//   }
		//   if (GlobalSettings.WBidStateCollection.SeniorityListItem != null)
		//   {

		//       if (GlobalSettings.WBidStateCollection.SeniorityListItem.SeniorityNumber == 0)
		//           seniority = " (Not in Domicile) ";
		//       else
		//           seniority = " ( " + GlobalSettings.WBidStateCollection.SeniorityListItem.SeniorityNumber + " of " + GlobalSettings.WBidStateCollection.SeniorityListItem.TotalCount + " )";
		//   }
		//   lblTitle.Text = wBidiTitle += dataSource + stateName + seniority+qamode;

		//}



		#region Synchronization
        public bool IsSynchStart;

		public int SynchStateVersion { get; set; }

		public DateTime ServerSynchTime { get; set; }

		private void Synch ()
		{
			if (GlobalSettings.SynchEnable && GlobalSettings.WBidStateCollection != null && GlobalSettings.WBidINIContent != null && GlobalSettings.WBidINIContent.User.SmartSynch) {
				syncOverlay = new LoadingOverlay (View.Bounds, "Smart Synchronisation checking server version..\n Please wait..");
				View.Add (syncOverlay);

				BeginInvokeOnMainThread (() => {
					SynchStateForApplicationLoad ();
				});
			}
		}

		private void SynchStateForApplicationLoad ()
		{

			try {
				// MessageBoxResult msgResult;
				bool isConnectionAvailable = Reachability.IsHostReachable (GlobalSettings.ServerUrl);

				if (isConnectionAvailable) {

					IsSynchStart = true;
					string stateFileName = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();
					SynchStateVersion = int.Parse (GlobalSettings.WBidStateCollection.SyncVersion);

					//Get server State Version
					VersionInfo versionInfo = GetServerVersion (stateFileName);
					syncOverlay.Hide ();

					if (versionInfo != null) {
						ServerSynchTime = DateTime.Parse (versionInfo.LastUpdatedDate, CultureInfo.InvariantCulture);

						if (versionInfo.Version != string.Empty) {
							int serverVersion = Convert.ToInt32 (versionInfo.Version);

							if (SynchStateVersion != serverVersion || GlobalSettings.WBidStateCollection.IsModified) {
								//conflict
								confNotif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("SyncConflict"), (NSNotification notification) => {
									string str = notification.Object.ToString ();
									NSNotificationCenter.DefaultCenter.RemoveObserver (confNotif);
									BeginInvokeOnMainThread (() => {
										if (str == "server") {
											FirstTime = true;
											syncOverlay = new LoadingOverlay (View.Bounds, "Synching current State FROM server \n Please wait..");
											View.Add (syncOverlay);
											WBidHelper.PushToUndoStack ();
											CommonClass.lineVC.UpdateUndoRedoButtons ();
											InvokeInBackground (() => {
												GetStateFromServer (stateFileName);
											});
										} else if (str == "local") {
											FirstTime = true;
											syncOverlay = new LoadingOverlay (View.Bounds, "Synching current State TO server \n Please wait..");
											View.Add (syncOverlay);
											InvokeInBackground (() => {
												UploadLocalVersionToServer (stateFileName);
											});
										} else {
											IsSynchStart = false;
										}
									});
								});
								SynchConflictViewController synchConf = new SynchConflictViewController ();
								synchConf.serverSynchTime = ServerSynchTime;
								if (serverVersion == 0)
									synchConf.noServer = true;
								synchConf.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
								this.PresentViewController (synchConf, true, null);
							} else if (SynchBtn) {
								SynchBtn = false;
								UIAlertView syAlert = new UIAlertView ("Smart Sync", "Your App is already synchronized with the server..", null, "OK", null);
								syAlert.Show ();
							}

							/* if (SynchStateVersion == serverVersion)
                         {
                             if (GlobalSettings.WBidStateCollection.IsModified)
                             {
                                 //Upload to server
                                 UIAlertView syAlert = new UIAlertView("Smart Sync", "Do you want to Synchronize the current State TO server?", null, "NO", new string[] { "YES" });
                                 syAlert.Show();
                                 syAlert.Dismissed += (object sender, UIButtonEventArgs e) =>
                                 {
                                     if (e.ButtonIndex == 0)
                                     {
                                         //no
                                         IsSynchStart = false;
                                     }
                                     else
                                     {
                                         //yes
            FirstTime=true;
            syncOverlay = new LoadingOverlay(View.Bounds, "Synching current State TO server \n Please wait..");
            View.Add(syncOverlay);
            InvokeInBackground(()=>{
            UploadLocalVersionToServer(stateFileName);
            });
           	
                                     }
                                 };
           	
                             }
                             else
                             {
            //                                UIAlertView syAlert = new UIAlertView("Smart Sync", "Your App is already synchronized with the server..", null, "OK", null);
            //                                syAlert.Show();
                                 IsSynchStart = false;
                             }
                         }
                         else if (SynchStateVersion < serverVersion && !GlobalSettings.WBidStateCollection.IsModified)
                         {
                             UIAlertView syAlert = new UIAlertView("Smart Sync", "Do you want to Synchronize the current State FROM server?", null, "NO", new string[] { "YES" });
                             syAlert.Show();
                             syAlert.Dismissed += (object sender, UIButtonEventArgs e) =>
                             {
                                 if (e.ButtonIndex == 0)
                                 {
                                     //no
                                     IsSynchStart = false;
                                 }
                                 else
                                 {
                                     //yes
            FirstTime = true;
            syncOverlay = new LoadingOverlay(View.Bounds, "Synching current State FROM server \n Please wait..");
            View.Add(syncOverlay);
            InvokeInBackground(()=>{
                                             GetStateFromServer(stateFileName);
            });
           	
                                 }
                             };
                         }
                         else
                         {
            //conflict
            confNotif = NSNotificationCenter.DefaultCenter.AddObserver ("SyncConflict", (NSNotification notification) => {
            string str = notification.Object.ToString ();
            NSNotificationCenter.DefaultCenter.RemoveObserver(confNotif);
            BeginInvokeOnMainThread(()=>{
            if(str=="server") {
            FirstTime=true;
            syncOverlay = new LoadingOverlay(View.Bounds, "Synching current State FROM server \n Please wait..");
            View.Add(syncOverlay);
            InvokeInBackground(()=>{
            GetStateFromServer(stateFileName);
            });
            }
            else if (str=="local") {
            FirstTime=true;
            syncOverlay = new LoadingOverlay(View.Bounds, "Synching current State TO server \n Please wait..");
            View.Add(syncOverlay);
            InvokeInBackground(()=>{
            UploadLocalVersionToServer(stateFileName);
            });
            }
            else {
            IsSynchStart = false;
            }
            });
            });
            SynchConflictViewController synchConf = new SynchConflictViewController();
            synchConf.serverSynchTime = ServerSynchTime;
            synchConf.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            this.PresentViewController (synchConf, true, null);
           	
                             ////System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                             ////{
                             ////    base.SendNotificationMessageAction(WBidMessages.MainVM_Notofication_ShowSynchConflictWindow, (resultVal) =>
                             ////    {
           	
                             ////        //get the server version
                             ////        if ((int)resultVal == 1)
                             ////        {
                             ////            GetStateFromServer(stateFileName);
                             ////        }
                             ////        else if ((int)resultVal == 2)//keep local version
                             ////        {
                             ////            UploadLocalVersionToServer(stateFileName);
                             ////        }
                             ////        else //Cancel
                             ////        {
                             ////            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                             ////            {
                             ////                IsSynchStart = false;
           	
                             ////            }));
                             ////        }
           	
                             ////    });
                             //}));
                         } */
						}
					} else {
						InvokeOnMainThread (() => {
							UIAlertView syAlert = new UIAlertView ("Smart Sync", "The WBid Synch server is not responding.  You can work on this bid and attempt to synch at a later time.", null, "OK", null);
							syAlert.Show ();
						});
					}

				} else {
					InvokeOnMainThread (() => {
						UIAlertView syAlert = new UIAlertView ("Smart Sync", "You do not have an internet connection.  You can work on this bid offline, but your state file for this bid may become unsynchronized from a previous state.", null, "OK", null);
						syAlert.Show ();
						syncOverlay.Hide ();
					});

					// base.SendNotificationMessage(WBidMessages.MainVM_Notofication_ShowSmartSyncConfirmationWindow);


				}
			} catch (Exception ex) {
				throw ex;
			}
		}

		private void SynchState ()
		{
			try {
				bool isConnectionAvailable = Reachability.IsHostReachable (GlobalSettings.ServerUrl);
				if (isConnectionAvailable) {
					//new thread?
					IsSynchStart = true;

					string stateFileName = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();
					SynchStateVersion = int.Parse (GlobalSettings.WBidStateCollection.SyncVersion);
					//Get server State Version
					VersionInfo versionInfo = GetServerVersion (stateFileName);
					if (versionInfo != null) {
						ServerSynchTime = DateTime.Parse (versionInfo.LastUpdatedDate, CultureInfo.InvariantCulture);

						if (versionInfo.Version != string.Empty) {
							int serverVersion = Convert.ToInt32 (versionInfo.Version);

							if (SynchStateVersion != serverVersion || GlobalSettings.WBidStateCollection.IsModified) {
								//conflict
								confNotif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("SyncConflict"), (NSNotification notification) => {
									string str = notification.Object.ToString ();
									NSNotificationCenter.DefaultCenter.RemoveObserver (confNotif);
									BeginInvokeOnMainThread (() => {
										if (str == "server") {
											syncOverlay = new LoadingOverlay (View.Bounds, "Synching current State FROM server \n Please wait..");
											View.Add (syncOverlay);
											InvokeInBackground (() => {
												GetStateFromServer (stateFileName);
											}
											);
										} else if (str == "local") {
											syncOverlay = new LoadingOverlay (View.Bounds, "Synching current State TO server \n Please wait..");
											View.Add (syncOverlay);
											InvokeInBackground (() => {
												UploadLocalVersionToServer (stateFileName);
											});
										} else {
											IsSynchStart = false;
											GoToHome ();
										}
										//GoToHome ();
									});
								});
								SynchConflictViewController synchConf = new SynchConflictViewController ();
								synchConf.serverSynchTime = ServerSynchTime;
								if (serverVersion == 0)
									synchConf.noServer = true;
								synchConf.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
								this.PresentViewController (synchConf, true, null);
							}

							/*  if (SynchStateVersion == serverVersion)
                            {
                                if (GlobalSettings.WBidStateCollection.IsModified)
                                {
                                    //Upload to server
                syncOverlay = new LoadingOverlay(View.Bounds, "Synching current State TO server \n Please wait..");
                View.Add(syncOverlay);
                InvokeInBackground (() => {
                UploadLocalVersionToServer (stateFileName);
                });
                //GoToHome ();
            	
                                }
                                else
                                {
                IsSynchStart = false;
                InvokeOnMainThread (() => {
                UIAlertView syAlert = new UIAlertView("Smart Sync", "Your App is already synchronized with the server..", null, "OK", null);
                syAlert.Dismissed += (object sender, UIButtonEventArgs e) => {
                if(e.ButtonIndex==0){
                GoToHome ();
                }
                };
                syAlert.Show();
                });
                                }
                            }
                            else if (SynchStateVersion < serverVersion && !GlobalSettings.WBidStateCollection.IsModified)
                            {
                syncOverlay = new LoadingOverlay(View.Bounds, "Synching current State FROM server \n Please wait..");
                View.Add(syncOverlay);
                InvokeInBackground (() => {
                GetStateFromServer (stateFileName);
                });
                //GoToHome ();
            	
                            }
                            else
                            {
                //conflict
                confNotif = NSNotificationCenter.DefaultCenter.AddObserver ("SyncConflict", (NSNotification notification) => {
                string str = notification.Object.ToString ();
                NSNotificationCenter.DefaultCenter.RemoveObserver(confNotif);
                BeginInvokeOnMainThread(()=>{
                if(str=="server") {
                syncOverlay = new LoadingOverlay(View.Bounds, "Synching current State FROM server \n Please wait..");
                View.Add(syncOverlay);
                InvokeInBackground (() => {
                GetStateFromServer(stateFileName);}
                );
                }
                else if (str=="local") {
                syncOverlay = new LoadingOverlay(View.Bounds, "Synching current State TO server \n Please wait..");
                View.Add(syncOverlay);
                InvokeInBackground (() => {
                UploadLocalVersionToServer(stateFileName);
                });
                }
                else {
                IsSynchStart = false;
                GoToHome ();
                }
                //GoToHome ();
                });
                });
                SynchConflictViewController synchConf = new SynchConflictViewController();
                synchConf.serverSynchTime = ServerSynchTime;
                synchConf.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                this.PresentViewController (synchConf, true, null);
            	
            	
            	
                                //System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                                //{
                                //    base.SendNotificationMessageAction(WBidMessages.MainVM_Notofication_ShowSynchConflictWindow, (resultVal) =>
                                //    {
            	
                                //        //get the server version
                                //        if ((int)resultVal == 1)
                                //        {
                                //            GetStateFromServer(stateFileName);
                                //        }
                                //        else if ((int)resultVal == 2)//keep local version
                                //        {
                                //            UploadLocalVersionToServer(stateFileName);
                                //        }
                                //        else //Cancel
                                //        {
                                //            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                //            {
                                //                IsSynchStart = false;
            	
                                //            }));
                                //        }
            	
                                //    });
                                //}));
                            } */
						}
					} else {
						InvokeOnMainThread (() => {
							UIAlertView syAlert = new UIAlertView ("Smart Sync", "The WBid Synch server is not responding.  You can work on this bid and attempt to synch at a later time.", null, "OK", null);
							syAlert.Show ();
						});
					}
				} else {
					InvokeOnMainThread (() => {
						UIAlertView syAlert = new UIAlertView ("Smart Sync", "You do not have an internet connection.  You can work on this bid offline, but your state file for this bid may become unsynchronized from a previous state.", null, "OK", null);
						syAlert.Show ();
					});

					//   base.SendNotificationMessage(WBidMessages.MainVM_Notofication_ShowSmartSyncConfirmationWindow);


				}
			} catch (Exception ex) {
				throw ex;
			}

		}

		private void UploadLocalVersionToServer (string stateFileName)
		{
			int version = int.Parse (SaveStateToServer (stateFileName));
			if (version != -1) {
				GlobalSettings.WBidStateCollection.SyncVersion = version.ToString ();
				GlobalSettings.WBidStateCollection.StateUpdatedTime = DateTime.Now.ToUniversalTime ();
				GlobalSettings.WBidStateCollection.IsModified = false;
				string stateFilePath = Path.Combine (WBidHelper.GetAppDataPath (), stateFileName + ".WBS");
				//WBidCollection.SaveStateFile(GlobalSettings.WBidStateCollection, stateFilePath);
				WBidHelper.SaveStateFile (WBidHelper.WBidStateFilePath);

				IsSynchStart = false;
				InvokeOnMainThread (() => {
					UIAlertView syAlert = new UIAlertView ("Smart Sync", "Successfully Synchronized  your computer with the server.", null, "OK", null);
					syAlert.Show ();
					syncOverlay.Hide ();
					if (FirstTime)
						NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);//loadSummaryListAndHeader();
                    else {
						syAlert.Dismissed += (object sender, UIButtonEventArgs e) => {
							if (e.ButtonIndex == 0)
								GoToHome ();
						};
					}
					FirstTime = false;
				});
			} else {
				InvokeOnMainThread (() => {
					UIAlertView syAlert = new UIAlertView ("Smart Sync", "An error occured while synchronizing your state to the server.  You can work on this bid, but your state file for this bid may become unsynchronized from a previous state.", null, "OK", null);
					syAlert.Show ();
					syncOverlay.Hide ();
				});
			}
		}

		private VersionInfo GetServerVersion (string stateFileName)
		{
			VersionInfo versionInfo = null;
			try {
				if (!GlobalSettings.SynchEnable)
					return versionInfo;
				string url = GlobalSettings.synchServiceUrl + "GetServerStateVersionNumber/" + GlobalSettings.WbidUserContent.UserInformation.EmpNo + "/" + stateFileName + "/" + GlobalSettings.CurrentBidDetails.Year;
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);
				request.Timeout = 30000;
				HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
				var stream = response.GetResponseStream ();
				var reader = new StreamReader (stream);
				versionInfo = ConvertJSonToObject<VersionInfo> (reader.ReadToEnd ());
				versionInfo.Version = versionInfo.Version.Trim ('"');
				return versionInfo;
			} catch (Exception ex) {
				versionInfo = null;
				IsSynchStart = false;
				return versionInfo;
				//throw ex;
			}
		}

		private string SaveStateToServer (string stateFileName)
		{
			try {
				string url = GlobalSettings.synchServiceUrl + "SaveWBidStateToServer/";
				WBidStateCollection wBidStateCollection = GlobalSettings.WBidStateCollection;

				foreach (var item in wBidStateCollection.StateList) {
					if (item.FAEOMStartDate == DateTime.MinValue) {
						item.FAEOMStartDate = DateTime.MinValue.ToUniversalTime ();
					}

				}

				string data = string.Empty;
				StateSync stateSync = new StateSync ();
				stateSync.EmployeeNumber = GlobalSettings.WbidUserContent.UserInformation.EmpNo;
				stateSync.StateFileName = stateFileName;
				stateSync.VersionNumber = 0;
				stateSync.Year = GlobalSettings.CurrentBidDetails.Year;
				stateSync.StateContent = SmartSyncLogic.JsonObjectToStringSerializer<WBidStateCollection> (wBidStateCollection);
				stateSync.LastUpdatedTime = DateTime.MinValue.ToUniversalTime ();

				var request = (HttpWebRequest)WebRequest.Create (url);
				request.Method = "POST";
				request.ContentType = "application/x-www-form-urlencoded";
				//data = SmartSyncLogic.JsonSerializer(stateSync);

				data = SmartSyncLogic.JsonObjectToStringSerializer<StateSync> (stateSync);
				var bytes = Encoding.UTF8.GetBytes (data);
				request.ContentLength = bytes.Length;
				request.GetRequestStream ().Write (bytes, 0, bytes.Length);
				request.Timeout = 30000;
				//Response
				var response = (HttpWebResponse)request.GetResponse ();
				var stream = response.GetResponseStream ();
				if (stream == null)
					return string.Empty;

				var reader = new StreamReader (stream);
				string result = reader.ReadToEnd ();

				return result.Trim ('"');
			} catch (Exception ex) {
				IsSynchStart = false;
				return "-1";
			}
		}

		void ReloadLineView ()
		{
			wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			if (wBIdStateContent.SortDetails.SortColumn == "SelectedColumn") {
				string colName = wBIdStateContent.SortDetails.SortColumnName;
				string direction = wBIdStateContent.SortDetails.SortDirection;
				if (colName == "LineNum")
					colName = "LineDisplay";
				if (colName == "LastArrivalTime")
					colName = "LastArrTime";
				if (colName == "StartDowOrder")
					colName = "StartDow";
				var datapropertyId = GlobalSettings.columndefinition.FirstOrDefault (x => x.DataPropertyName == colName).Id;
				CommonClass.columnID = datapropertyId;
				if (direction == "Asc")
					CommonClass.columnAscend = true;
				else
					CommonClass.columnAscend = false;
			} else {
				CommonClass.columnID = 0;
				CommonClass.columnAscend = false;
			}
			//			SortCalculation sort = new SortCalculation ();
			//			if (wBIdStateContent.SortDetails != null && wBIdStateContent.SortDetails.SortColumn != null && wBIdStateContent.SortDetails.SortColumn != string.Empty) {
			//				sort.SortLines (wBIdStateContent.SortDetails.SortColumn);
			//			}
		}

		private void GetStateFromServer (string stateFileName)
		{
			bool failed = false;
			try {
				string url = GlobalSettings.synchServiceUrl + "GetWBidStateFromServer/" + GlobalSettings.WbidUserContent.UserInformation.EmpNo + "/" + stateFileName + "/" + GlobalSettings.CurrentBidDetails.Year;


				HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);
				request.Timeout = 30000;
				HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
				var stream = response.GetResponseStream ();
				var reader = new StreamReader (stream);
				StateSync stateSync = SmartSyncLogic.ConvertJsonToObject<StateSync> (reader.ReadToEnd ());
				WBidStateCollection wBidStateCollection = null;
				bool isNeedToRecalculateLineProp = false;
				if (stateSync != null) {
					// byte[] byteArr = Convert.FromBase64String(stateSync.StateContent);

					wBidStateCollection = SmartSyncLogic.ConvertJsonToObject<WBidStateCollection> (stateSync.StateContent);
					foreach (WBidState state in wBidStateCollection.StateList)
					{
						if (state.CxWtState.CLAuto == null)
							state.CxWtState.CLAuto = new StateStatus { Cx = false, Wt = false };
						if (state.CxWtState.CitiesLegs == null)
						{
							state.CxWtState.CitiesLegs = new StateStatus { Cx = false, Wt = false };
							state.Constraints.CitiesLegs=new Cx3Parameters { ThirdcellValue = "1", Type = (int)ConstraintType.LessThan, Value = 1 ,lstParameters=new List<Cx3Parameter>()};
							state.Weights.CitiesLegs=new Wt2Parameters
							{
								Type = 1,
								Weight = 0   ,
								lstParameters=new List<Wt2Parameter>()
							};
						}
						if(state.CxWtState.Commute==null)
						{
						   state.CxWtState.Commute = new StateStatus { Cx = false, Wt = false };
							state.Constraints.Commute=new Commutability {BaseTime=10, ConnectTime=30,CheckInTime=60, SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.LessThan, Value = 100 };
							state.Weights.Commute=new Commutability {BaseTime=10, ConnectTime=30,CheckInTime=60, SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.LessThan, Value = 100 };
						}
						//once commutability sort implemented, remove this.
						state.SortDetails.BlokSort.RemoveAll(x=>x=="30"||x=="31" || x=="32");
							
					}
					foreach (var item in wBidStateCollection.StateList)
					{
						if (item.BidAuto != null && item.BidAuto.BAFilter != null && item.BidAuto.BAFilter.Count > 0)
						{
							WBidCollection.HandleTypeOfBidAutoObject(item.BidAuto.BAFilter);
						}
						if (item.CalculatedBA != null && item.CalculatedBA.BAFilter != null && item.CalculatedBA.BAFilter.Count > 0)
						{
							WBidCollection.HandleTypeOfBidAutoObject(item.CalculatedBA.BAFilter);
						}

					}

					var wBidStateContent = wBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
					var currentopendState = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
					if (wBidStateContent.MenuBarButtonState.IsOverlap && currentopendState.IsOverlapCorrection == false) {
						InvokeOnMainThread (() => {
							syncOverlay.Hide ();
							UIAlertView syAlert = new UIAlertView ("Smart Sync", "Previous state had Overlap Data and You need to re-download the bid data and make an overlap correction", null, "OK", null);
							syAlert.Show ();
						});

						return;
					}


					StateManagement statemanagement = new StateManagement ();
                
					isNeedToRecalculateLineProp = statemanagement.CheckLinePropertiesNeedToRecalculate (wBidStateContent);
					ResetLinePropertiesBackToNormal (currentopendState, wBidStateContent);
					ResetOverlapState (currentopendState, wBidStateContent);

					GlobalSettings.WBidStateCollection = wBidStateCollection;
					//GlobalSettings.WBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName); ;
					GlobalSettings.WBidStateCollection.SyncVersion = stateSync.VersionNumber.ToString ();
					GlobalSettings.WBidStateCollection.StateUpdatedTime = stateSync.LastUpdatedTime;
					GlobalSettings.WBidStateCollection.IsModified = false;

					if (wBidStateContent.MenuBarButtonState.IsEOM) {
						SetEOMVacationDataAfterSynch ();
					}
					if (wBidStateContent.MenuBarButtonState.IsMIL) {
						SetMILDataAfterSynch ();
					}
					if (wBidStateContent.MenuBarButtonState.IsMIL && isNeedtoCreateMILFile) {
						isNeedToRecalculateLineProp = true;
					}
					wBidStateContent.SortDetails.SortColumn = "Manual";

					string stateFilePath = Path.Combine (WBidHelper.GetAppDataPath (), stateFileName + ".WBS");
					//WBidCollection.SaveStateFile(GlobalSettings.WBidStateCollection, stateFilePath);
					WBidHelper.SaveStateFile (WBidHelper.WBidStateFilePath);

                

				}
				InvokeOnMainThread (() => {
					UIAlertView syAlert = new UIAlertView ("Smart Sync", "Successfully Synchronized  your computer with the server.", null, "OK", null);
					syAlert.Show ();
					syncOverlay.Hide ();
					if (FirstTime) {
						GlobalSettings.Lines.ToList ().ForEach (x => {
							x.ConstraintPoints.Reset ();
							x.Constrained = false;
							x.WeightPoints.Reset ();
							x.TotWeight = 0.0m;
						});
						StateManagement statemanagement = new StateManagement ();
						statemanagement.ReloadLineDetailsBasedOnPreviousState (isNeedToRecalculateLineProp);

						var wBidStateContent = wBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
						if (wBidStateContent.TagDetails != null) {
							foreach (var item in GlobalSettings.Lines) {
								var tagItem = wBidStateContent.TagDetails.FirstOrDefault (x => x.Line == item.LineNum);
								if (tagItem != null)
									item.Tag = tagItem.Content;
								else
									item.Tag = string.Empty;

							}

						}

						if(wBIdStateContent.TagDetails!=null)
						{
							GlobalSettings.TagDetails=new TagDetails();
							wBIdStateContent.TagDetails.ForEach(x => GlobalSettings.TagDetails.Add(new Tag{Line=x.Line,Content=x.Content}));
						}
						//statemanagement.ReloadDataFromStateFile();
						//loadSummaryListAndHeader();
						//ReloadLineView ();

						NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
						SetVacButtonStates ();
					} else {
						syAlert.Dismissed += (object sender, UIButtonEventArgs e) => {
							if (e.ButtonIndex == 0)
								GoToHome ();
						};
					}
					FirstTime = false;
				});
			} catch (Exception ex) {
				//System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
				//{
				//    SmartSynchMessage = "The server to get the previous state file is not responding.  You can work on this bid, but your state file for this bid may become unsynchronized from a previous state.";
				//    base.SendNotificationMessage(WBidMessages.MainVM_Notofication_ShowSmartSyncConfirmationWindow);
				//    IsSynchStart = false;
				//}));

				FirstTime = false;
				failed = true;
				//throw  ex;
			}

			if (failed) {
				InvokeOnMainThread (() => {
					syncOverlay.Hide ();
					UIAlertView syAlert = new UIAlertView ("Smart Sync", "The server to get the previous state file is not responding.  You can work on this bid, but your state file for this bid may become unsynchronized from a previous state.", null, "OK", null);
					syAlert.Show ();
				});

			}

		}


//		partial void ScrollUpButtonClicked (NSObject sender)
//		{
//			modernList.ScrollUp();

		//	sumList.TableView.DeselectRow (lPath, true);
//			NSIndexPath[] visibleRows = sumList.TableView.IndexPathsForVisibleRows;
//
//			for (int i=0 ; i < (int)visibleRows.Length ; i++)
//			{
//				Console.WriteLine ("Rows-"+ visibleRows[i].Row);
//
//			}

//			NSIndexPath path = NSIndexPath.FromRowSection (lPath.Row - 1, lPath.Section);
//			lPath = path;
//			sumList.TableView.SelectRow (lPath, true, UITableViewScrollPosition.None);
//
//
//			CGPoint point = sumList.TableView.RectForRowAtIndexPath (lPath).Location;
//			point = sumList.TableView.ConvertPointToView (point, this.vwTable);
//			CGPoint offset = sumList.TableView.ContentOffset;
//			if (point.Y < 50)
//				sumList.TableView.ScrollToRow (NSIndexPath.FromRowSection (lPath.Row, lPath.Section), UITableViewScrollPosition.Top, true);
			
//		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="currentState"></param>
		/// <param name="newState"></param>
		private void ResetLinePropertiesBackToNormal (WBidState currentState, WBidState newState)
		{
			if (newState.MenuBarButtonState.IsOverlap == false && currentState.MenuBarButtonState.IsOverlap) {
				//remove the  Overlp Calculation from line
				ReCalculateLinePropertiesForOverlapCorrection reCalculateLinePropertiesForOverlapCorrection = new ReCalculateLinePropertiesForOverlapCorrection ();
				reCalculateLinePropertiesForOverlapCorrection.ReCalculateLinePropertiesOnOverlapCorrection (GlobalSettings.Lines.ToList (), false);
			} else if ((currentState.MenuBarButtonState.IsVacationCorrection || currentState.MenuBarButtonState.IsEOM) && newState.MenuBarButtonState.IsOverlap) {
				GlobalSettings.MenuBarButtonStatus.IsEOM = false;
				GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = false;
				GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
				//Remove the vacation propertiesfrom Line 
				RecalcalculateLineProperties RecalcalculateLineProperties = new PortableLibrary.BusinessLogic.RecalcalculateLineProperties ();
				RecalcalculateLineProperties.CalcalculateLineProperties ();
			}

		}


		private void ResetOverlapState (WBidState currentState, WBidState newState)
		{
			if (newState.IsOverlapCorrection == false && currentState.IsOverlapCorrection) {
				newState.IsOverlapCorrection = true;
			} else if (newState.IsOverlapCorrection && currentState.IsOverlapCorrection == false) {
				newState.IsOverlapCorrection = false;
			}
		}

		public MILData CreateNewMILFile ()
		{
			MILData milData;
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
			milData = new MILData ();
			milData.Version = GlobalSettings.MILFileVersion;
			milData.MILValue = milvalue;
			var stream = File.Create (WBidHelper.MILFilePath);
			ProtoSerailizer.SerializeObject (WBidHelper.MILFilePath, milData, stream);
			stream.Dispose ();
			stream.Close ();
			return milData;
		}

		private void SetMILDataAfterSynch ()
		{
			var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

			var MILDates = wBidStateContent.MILDateList;

			if (MILDates.Count > 0) {
				isNeedtoCreateMILFile = false;
				if (GlobalSettings.MILDates == null || MILDates.Count != GlobalSettings.MILDates.Count)
					isNeedtoCreateMILFile = true;
				else {
					for (int count = 0; count < MILDates.Count; count++) {
						if (GlobalSettings.MILDates [count].StartAbsenceDate != MILDates [count].StartAbsenceDate || GlobalSettings.MILDates [count].EndAbsenceDate != MILDates [count].EndAbsenceDate) {
							isNeedtoCreateMILFile = true;
							break;
						}

					}
				}
				GlobalSettings.MILDates = GenarateOrderedMILDates (wBidStateContent.MILDateList);
				MILData milData;
				InvokeOnMainThread (() => {
					syncOverlay.updateLoadingText ("Calculating MIL");
				});

				//InvokeInBackground (() => {
				if (System.IO.File.Exists (WBidHelper.MILFilePath) && !isNeedtoCreateMILFile) {
					using (FileStream milStream = File.OpenRead (WBidHelper.MILFilePath)) {

						MILData milDataobject = new MILData ();
						milData = ProtoSerailizer.DeSerializeObject (WBidHelper.MILFilePath, milDataobject, milStream);
						
					}
				} else {

					milData = CreateNewMILFile ();




				}

				
				//Apply MIL values (calculate property values including Modern bid line properties
				//==============================================

				GlobalSettings.MILData = milData.MILValue;
				GlobalSettings.MenuBarButtonStatus.IsMIL = true;

				RecalcalculateLineProperties recalcalculateLineProperties = new RecalcalculateLineProperties ();
				recalcalculateLineProperties.CalcalculateLineProperties ();

				InvokeOnMainThread (() => {
					GlobalSettings.isModified = true;
					CommonClass.lineVC.UpdateSaveButton ();
					syncOverlay.Hide ();
					CommonClass.lineVC.SetVacButtonStates ();
					NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
					this.DismissViewController (true, null);
				});




				//});

			}
		}


		private void SetEOMVacationDataAfterSynch ()
		{
			try {
				var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

				if (GlobalSettings.CurrentBidDetails.Postion == "FA") {
					if (GlobalSettings.FAEOMStartDate != null && GlobalSettings.FAEOMStartDate != DateTime.MinValue) {
						btnVacDrop.Enabled = true;
						syncOverlay.updateLoadingText ("Calculating EOM");
						InvokeInBackground (() => {
							CreateEOMVacforFA ();
						});
					}
				} else {
					string currentBidName = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();

					//string zipFileName = GenarateZipFileName();
					string vACFileName = WBidHelper.GetAppDataPath () + "//" + currentBidName + ".VAC";
					//Cheks the VAC file exists
					bool vacFileExists = File.Exists (vACFileName);

					if (!vacFileExists) {
						InvokeOnMainThread (() => {
							//syncOverlay.Hide ();
							UIAlertView syAlert = new UIAlertView ("Smart Sync", "Previous state had EOM selected and we are downloading Vacation Data", null, "OK", null);
							syAlert.Show ();
							syncOverlay.updateLoadingText ("Calculating EOM");
						});


						//InvokeOnMainThread (() => {

						CreateEOMVacationforCP ();


						//});
					} else {




						InvokeOnMainThread (() => {
							syncOverlay.updateLoadingText ("Calculating EOM");

							if (GlobalSettings.VacationData == null) {
								using (FileStream vacstream = File.OpenRead (vACFileName)) {

									Dictionary<string, TripMultiVacData> objineinfo = new Dictionary<string, TripMultiVacData> ();
									GlobalSettings.VacationData = ProtoSerailizer.DeSerializeObject (vACFileName, objineinfo, vacstream);
								}
							}


						});
					}
				}
			} catch (Exception ex) {
				wBIdStateContent.MenuBarButtonState.IsEOM = false;
				throw ex;
			}


		}

		public static T ConvertJSonToObject<T> (string jsonString)
		{
			DataContractJsonSerializer serializer = new DataContractJsonSerializer (typeof(T));
			MemoryStream ms = new MemoryStream (Encoding.UTF8.GetBytes (jsonString));
			T obj = (T)serializer.ReadObject (ms);
			return obj;
		}

		//private static string JsonSerializer<T>(T t)
		//{
		//    string jsonString = string.Empty;
		//    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
		//    MemoryStream ms = new MemoryStream();
		//    ser.WriteObject(ms, t);
		//    jsonString = Encoding.UTF8.GetString(ms.ToArray());
		//    return jsonString;
		//}

		//private static string ObjectToBase64(WBidStateCollection wBidStateCollection)
		//{
		//    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(WBidStateCollection));
		//    string base64String = "";
		//    using (MemoryStream memStream = new MemoryStream())
		//    {
		//        if (wBidStateCollection.StateUpdatedTime == DateTime.MinValue) ;
		//        wBidStateCollection.StateUpdatedTime = wBidStateCollection.StateUpdatedTime.ToUniversalTime();
		//        ser.WriteObject(memStream, wBidStateCollection);

		//        byte[] byteArray = memStream.ToArray();

		//        base64String = Convert.ToBase64String(byteArray);


		//    }
		//    return base64String;
		//  }

		#endregion

		private Trip GetTrip (string pairing)
		{
			Trip trip = null;
			trip = GlobalSettings.Trip.Where (x => x.TripNum == pairing.Substring (0, 4)).FirstOrDefault ();
			if (trip == null) {
				trip = GlobalSettings.Trip.Where (x => x.TripNum == pairing).FirstOrDefault ();
			}

			return trip;

		}

		public class ExportCalendar
		{
			public string Title { get; set; }

			public DateTime StarDdate { get; set; }

			public DateTime EndDate { get; set; }

			public String TripDetails { get; set; }
		}


	}

}

