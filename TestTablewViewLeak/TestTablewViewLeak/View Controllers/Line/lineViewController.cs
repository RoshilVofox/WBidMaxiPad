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
using TestTablewViewLeak.Utility;
using System.Threading.Tasks;
using WBid.WBidiPad.SharedLibrary.SWA;
using TestTablewViewLeak.ViewControllers;
using TestTablewViewLeak.ViewControllers.VacationDifferenceView;
using System.Json;
using TestTablewViewLeak.ViewControllers.CommuteDifferenceView;

namespace WBid.WBidiPad.iOS
{
    public partial class lineViewController : UIViewController, IServiceDelegate
    {
        class MyPopDelegate : UIPopoverControllerDelegate
        {

            lineViewController _parent;

            public MyPopDelegate(lineViewController parent)
            {
                _parent = parent;
            }

            public override void DidDismiss(UIPopoverController popoverController)
            {
                _parent.popoverController = null;
                if (_parent.sumList != null && _parent.hTable != null)
                {
                    _parent.sumList.TableView.DeselectRow(_parent.lPath, true);
                    _parent.hTable.TableView.DeselectRow(_parent.hPath, true);
                }
                else if (_parent.bidLineList != null)
                {
                    _parent.bidLineList.TableView.DeselectRow(_parent.lPath, true);
                }
                else if (_parent.modernList != null)
                {
                    _parent.modernList.TableView.DeselectRow(_parent.lPath, true);
                }
            }
        }

        bool FirstTime;
        NSObject confNotif;
        NSObject notif;
        NSObject synchSelectionNotif;
        CGPoint tableListOffset;
        NSIndexPath hPath, lPath;
        public NSIndexPath scrlPath;
        public summaryHeaderListController hTable;
        public SummaryViewController sumList;
        public BidLineViewController bidLineList;
        public ModernContainerViewController modernList;
        UIPopoverController popoverController;
        List<NSObject> arrObserver = new List<NSObject>();
        CalenderPopoverController calCollection;
        TripPopListViewController tripList;
        public string tripNum;
        public string wblFileName;
        ConstraintCalculations constCalc = new ConstraintCalculations();
        WeightCalculation weightCalc = new WeightCalculation();
        SortCalculation sort = new SortCalculation();
        private System.Timers.Timer timer;
        //DateTime defDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1);
        WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
        LoadingOverlay syncOverlay;
        bool isNeedtoCreateMILFile = false;
        bool SynchBtn;
        int DataSynchSelecedValue = 0; //1 for state, 2 for quickset and 3 for both
        bool IsStateFromServer = false;
        bool IsQSFromServer = false;
        bool IskeepLocalQS = false;
        bool IskeepLocalState = false;
        OdataBuilder ObjOdata;
        NSObject ObserverApploadData;
        public lineViewController()
            : base("lineViewController", null)
        {
        }
        public lineViewController(IntPtr handle) : base(handle)
        {
            CommonClass.lineVC = this;
        }
        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        partial void funClearBlueBorder(NSObject sender)
        {
            foreach (var line in GlobalSettings.Lines)
            {
                line.ManualScroll = 0;
            }
            GlobalSettings.isModified = true;
            UpdateSaveButton();
            NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
        }
        public void UpdateSaveButton()
        {
            btnSave.Enabled = GlobalSettings.isModified;
            GlobalSettings.isUndo = false;
            GlobalSettings.isRedo = false;
            UpdateUndoRedoButtons();
        }

        public override void ViewDidLoad()
        {


            base.ViewDidLoad();
            //NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillEnterForegroundNotification, AppCallback);
            ObserverApploadData = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("SetApplicationLoadData"), SetApplicationLoadData);
            CommonClass.lineVC = this;
            this.View.LayoutIfNeeded();
            FirstTime = true;
            GlobalSettings.isModified = false;
            Constants.listCities = GlobalSettings.WBidINIContent.Cities.Select(x => x.Name).ToList();
            //btnSave.Enabled = false;
            UpdateSaveButton();
            lblTitle.AdjustsFontSizeToFitWidth = true;
            lblTitle.Text = WBidCollection.SetTitile();
            vwReparse.Hidden = true;
            setReparseView();

            var wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            if (wBIdStateContent.SortDetails == null)
            {
                wBIdStateContent.SortDetails = new SortDetails();
            }
            if (wBIdStateContent.SortDetails.SortColumn == "SelectedColumn")
            {
                string colName = wBIdStateContent.SortDetails.SortColumnName;
                string direction = wBIdStateContent.SortDetails.SortDirection;
                if (colName == "LineNum")
                    colName = "LineDisplay";
                if (colName == "LastArrivalTime")
                    colName = "LastArrTime";
                if (colName == "StartDowOrder")
                    colName = "StartDow";

                var datapropertyId = GlobalSettings.columndefinition.FirstOrDefault(x => x.DataPropertyName == colName).Id;
                CommonClass.columnID = datapropertyId;
                if (direction == "Asc")
                    CommonClass.columnAscend = true;
                else
                    CommonClass.columnAscend = false;
                Console.WriteLine(datapropertyId);
            }

            this.vwCalPopover.Hidden = true;
            this.vwTripPopover.Hidden = true;

            this.btnPromote.Enabled = false;
            this.btnTrash.Enabled = false;
            //this.btnRemTopLock.Enabled = false;
            // this.btnRemBottomLock.Enabled = false;

            if (wBIdStateContent != null)
            {
                this.btnRemTopLock.Enabled = wBIdStateContent.TopLockCount > 0;
                this.btnRemBottomLock.Enabled = wBIdStateContent.BottomLockCount > 0;
                ;
            }
            else
            {
                this.btnRemTopLock.Enabled = false;
                this.btnRemBottomLock.Enabled = false;
            }

            this.btnGrid.Selected = CommonClass.showGrid;

            if (GlobalSettings.WBidINIContent.ViewType == 1)
            {
                this.sgControlViewType.SelectedSegment = 0;
                //this.loadSummaryListAndHeader();
            }
            else if (GlobalSettings.WBidINIContent.ViewType == 2)
            {
                this.sgControlViewType.SelectedSegment = 1;
            }
            else if (GlobalSettings.WBidINIContent.ViewType == 3)
            {
                this.sgControlViewType.SelectedSegment = 2;
            }
            else
            {
                GlobalSettings.WBidINIContent.ViewType = 1;
                this.sgControlViewType.SelectedSegment = 0;
            }
            loadSummaryListAndHeader();
            Console.WriteLine("Data loaded in summary list");

            this.btnHome.SetBackgroundImage(UIImage.FromBundle("homeIcon.png"), UIControlState.Normal);
            this.btnSave.SetBackgroundImage(UIImage.FromBundle("saveIconRed.png"), UIControlState.Normal);
            this.btnSave.SetBackgroundImage(UIImage.FromBundle("saveIcon.png"), UIControlState.Disabled);
            this.btnPromote.SetBackgroundImage(UIImage.FromBundle("topLockIcon.png"), UIControlState.Normal);
            this.btnTrash.SetBackgroundImage(UIImage.FromBundle("bottomLockIcon.png"), UIControlState.Normal);
            this.btnGrid.SetBackgroundImage(UIImage.FromBundle("gridIcon.png"), UIControlState.Normal);
            this.btnGrid.SetBackgroundImage(UIImage.FromBundle("removeGridIcon.png"), UIControlState.Selected);
            this.btnRemTopLock.SetBackgroundImage(UIImage.FromBundle("removeTopLockIcon.png"), UIControlState.Normal);
            this.btnRemBottomLock.SetBackgroundImage(UIImage.FromBundle("removeBottomLockIcon.png"), UIControlState.Normal);
            this.btnBrightness.SetBackgroundImage(UIImage.FromBundle("brightnessIcon.png"), UIControlState.Normal);

            this.btnOverlap.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            this.btnOverlap.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            this.btnVacCorrect.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            this.btnVacCorrect.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            SetDropButtonTextAndbackground();
            this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            //this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("activeButtonRed.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            //this.btnCSW.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            //this.btnPairing.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            this.btnEOM.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            this.btnEOM.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            this.btnMIL.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            this.btnMIL.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);

            //btnEOM.Enabled = false;
            // observers nsnotificationcenter notifications.
            observeNotification();

            //            // Saving additional header colunmns.
            //            LineSummaryBL.GetAdditionalColumns();
            //
            //            // Saving additional bidline colunmns.
            //            LineSummaryBL.GetBidlineViewAdditionalColumns();
            //
            //            // Saving additional modern colunmns.
            //            LineSummaryBL.GetModernViewAdditionalColumns();
            //
            //            CommonClass.bidLineProperties = new List<string>() {
            //                "Pay",
            //                "PDiem",
            //                "Flt",
            //                "Off",
            //                "+Off"
            //            };

            // this adds pan gestures to custom popovers.
            this.addPanGestures();

            txtGoToLine.Background = UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5));
            txtGoToLine.EditingDidBegin += (object sender, EventArgs e) =>
            {
                vwCalPopover.Hidden = true;
                vwTripPopover.Hidden = true;
                NSNotificationCenter.DefaultCenter.PostNotificationName("disablesummaryheaderediting", null);



            };
            txtGoToLine.ShouldChangeCharacters = (textField, range, replacementString) =>
            {
                string text = textField.Text;
                string newText = text.Substring(0, (int)range.Location) + replacementString + text.Substring((int)range.Location + (int)range.Length);
                int val;
                if (newText == "")
                    return true;
                else
                    return Int32.TryParse(newText, out val);
            };
            txtGoToLine.ShouldReturn = ((textField) =>
            {
                string text = textField.Text;
                GoToLine(text);
                return true;
            });

            btnSynch.TouchUpInside += btnSynchTapped;


            //btnOverlap.Enabled = false;
            //btnVacCorrect.Enabled = false;
            //btnVacDrop.Enabled = false;
            //btnEOM.Enabled = false;

            SetVacButtonStates();
            SingleTapNavigation();
            DoubleTapNavigation();
            LongPressHandling();
            //            if (btnEOM.Selected || btnVacCorrect.Selected || btnVacDrop.Selected)
            //                applyVacation ();
            //            if (btnOverlap.Selected)
            //                applyOverLapCorrection ();
            //            if (btnEOM.Selected || btnVacCorrect.Selected || btnVacDrop.Selected)
            //                applyVacation ();
            //            if (btnOverlap.Selected)
            //                applyOverLapCorrection ();
            //            if (btnMIL.Selected)
            //                applyMIL ();
            AutoSave();
            //            GlobalSettings.WBidINIContent.User.SmartSynch = false;
            //            GlobalSettings.SynchEnable = false;
            btnUndo.TouchUpInside += btnUndoTapped;
            btnRedo.TouchUpInside += btnRedoTapped;
            //            btnUndo.SetTitle (GlobalSettings.UndoStack.Count.ToString (), UIControlState.Normal);
            //            btnRedo.SetTitle (GlobalSettings.RedoStack.Count.ToString (), UIControlState.Normal);
            //            UpdateUndoRedoButtons ();

            btnQuickSet.TouchUpInside += btnQuickSetTapped;


            HandleBlueShadowButton();

           // btnVacDiff.Hidden = !GlobalSettings.IsNeedToEnableVacDiffButton;

            SetFlightDataDiffButton();
        }
        public void SetFlightDataDiffButton()
        {
            bool IsEnableFltDiff;
            if (GlobalSettings.IsNeedToEnableVacDiffButton)
            {
                bool isCommuteAutoAvailable = false;
                if (wBIdStateContent.CxWtState.CLAuto.Cx || wBIdStateContent.CxWtState.CLAuto.Wt || (wBIdStateContent.SortDetails.BlokSort.Contains("33") || wBIdStateContent.SortDetails.BlokSort.Contains("34") || wBIdStateContent.SortDetails.BlokSort.Contains("35")))
                {
                    isCommuteAutoAvailable = true;
                }


                if (GlobalSettings.CurrentBidDetails != null && GlobalSettings.CurrentBidDetails.Postion == "FA")
                {
                    //For FA, the Flt difference button should be display only when user set any commutable line auto in constraints ,weights,sorts or in bid auto
                    IsEnableFltDiff = isCommuteAutoAvailable && (GlobalSettings.ServerFlightDataVersion != GlobalSettings.WBidINIContent.LocalFlightDataVersion);
                }
                else
                {
                    IsEnableFltDiff = ((isCommuteAutoAvailable && (GlobalSettings.ServerFlightDataVersion != GlobalSettings.WBidINIContent.LocalFlightDataVersion)) || (btnVacCorrect.Enabled || btnEOM.Enabled));
                }
            }
            else
            {
                IsEnableFltDiff = false;
            }
            btnVacDiff.Hidden=!IsEnableFltDiff;
        }
        public void SetApplicationLoadData(NSNotification n)
        {
            //  btnVacDiff.Hidden = !GlobalSettings.IsNeedToEnableVacDiffButton;
            SetFlightDataDiffButton();
        }
        public void ServiceResponce(JsonValue jsonDoc)
        {
            try
            {
                ApplicationLoadData appLoadData = CommonClass.ConvertJSonToObject<ApplicationLoadData>(jsonDoc.ToString());
                GlobalSettings.IsNeedToEnableVacDiffButton = appLoadData.IsNeedtoEnableVacationDifference;
                GlobalSettings.ServerFlightDataVersion = appLoadData.FlightDataVersion;
                
                //btnVacDiff.Hidden = !GlobalSettings.IsNeedToEnableVacDiffButton;
            }
            catch (Exception ex)
            {
            }
        }
        public void ResponceError(string Error)
        {
            InvokeOnMainThread(() => {
                //ActivityIndicator.Hide ();
            });
            //			Console.WriteLine ("Service Fail");

        }
        void AppCallback(NSNotification notification)
        {
            if (Reachability.CheckVPSAvailable())
            {
                 GetApplicationLoadDataFromServer();
            }
            NSNotificationCenter.DefaultCenter.RemoveObserver(notification);
        }
        private void GetApplicationLoadDataFromServer()
        {
            InvokeInBackground(() =>
            {
                ObjOdata = new OdataBuilder();
                ApplicationData info = new ApplicationData();
                ObjOdata.RestService.Objdelegate = this;
                info.FromApp = (int)AppNum.WBidMaxApp;
                ObjOdata.GetApplicationLoadData(info);
            });
        }
        partial void btnVacDiffClicked(NSObject sender)
        {
            if (GlobalSettings.CurrentBidDetails.Postion == "FA")
            {
                //only commut diff need to show
                CommuteDifferenceViewController vacdiff = new CommuteDifferenceViewController();
                vacdiff.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                UINavigationController nav = new UINavigationController(vacdiff);
                vacdiff.PreferredContentSize = new CGSize(1020, 700);
                nav.NavigationBarHidden = true;
                nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                this.PresentViewController(nav, true, null);
            }
            else
            {
                bool isCommuteAutoAvailable = false;
                if (wBIdStateContent.CxWtState.CLAuto.Cx || wBIdStateContent.CxWtState.CLAuto.Wt || (wBIdStateContent.SortDetails.BlokSort.Contains("33") || wBIdStateContent.SortDetails.BlokSort.Contains("34") || wBIdStateContent.SortDetails.BlokSort.Contains("35")))
                {
                    isCommuteAutoAvailable = true;
                }
                if (((isCommuteAutoAvailable && (GlobalSettings.ServerFlightDataVersion != GlobalSettings.WBidINIContent.LocalFlightDataVersion)) && (btnVacCorrect.Enabled || btnEOM.Enabled)))
                {
                    //both commut diff and vac diff available
                    
                }
                else if ((isCommuteAutoAvailable && (GlobalSettings.ServerFlightDataVersion != GlobalSettings.WBidINIContent.LocalFlightDataVersion)))
                {
                    //only commut diff need to show
                        CommuteDifferenceViewController vacdiff = new CommuteDifferenceViewController();
                    vacdiff.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                    UINavigationController nav = new UINavigationController(vacdiff);
                    vacdiff.PreferredContentSize = new CGSize(1020, 700);
                    nav.NavigationBarHidden = true;
                    nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                    this.PresentViewController(nav, true, null);
                }
                else
                {
                    //Show vac diff
                    if (Reachability.CheckVPSAvailable())
                    {
                        VacationDifferenceViewController vacdiff = new VacationDifferenceViewController();
                        vacdiff.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                        UINavigationController nav = new UINavigationController(vacdiff);
                        vacdiff.PreferredContentSize = new CGSize(1020, 700);
                        nav.NavigationBarHidden = true;
                        nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                        this.PresentViewController(nav, true, null);
                    }
                    else
                    {
                        if (WBidHelper.IsSouthWestWifiOr2wire())
                        {

                            UIAlertController okAlertController = UIAlertController.Create("WBidMax", Constants.SouthWestConnectionAlert, UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                        }
                        else
                        {
                            UIAlertController okAlertController = UIAlertController.Create("WBidMax", Constants.VPSDownAlert, UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                        }
                    }
                }

            }

            


            
        }
        private void SetDropButtonTextAndbackground()
        {
            if ((GlobalSettings.MenuBarButtonStatus.IsEOM || GlobalSettings.MenuBarButtonStatus.IsVacationCorrection) && GlobalSettings.MenuBarButtonStatus.IsVacationDrop == false)
            {
                btnVacDrop.SetTitle("FLY", UIControlState.Normal);
                btnVacDrop.SetTitleColor(UIColor.White, UIControlState.Normal);
                this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("activeButtonRed.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);

            }
            else
            {
                this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            }
        }

        public void SingleTapNavigation()
        {
            UITapGestureRecognizer DownsingleTap;
            DownsingleTap = new UITapGestureRecognizer(() =>
                {
                    if (GlobalSettings.WBidINIContent.ViewType == 1)
                    {

                        NSIndexPath[] arrVisibleIndex = sumList.TableView.IndexPathsForVisibleRows;

                        if (arrVisibleIndex.Length > 0)
                        {
                            NSIndexPath NextIndex = NSIndexPath.FromRowSection(arrVisibleIndex[0].Row + 50, 0);
                            if (NextIndex.Row >= GlobalSettings.Lines.Count)
                            {
                                NextIndex = NSIndexPath.FromRowSection(sumList.TableView.NumberOfRowsInSection(0) - 1, 0);
                                sumList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            }
                            else
                                sumList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            scrlPath = NextIndex;


                        }
                    }
                    else if (GlobalSettings.WBidINIContent.ViewType == 2)
                    {

                        NSIndexPath[] arrVisibleIndex = bidLineList.TableView.IndexPathsForVisibleRows;
                        if (arrVisibleIndex.Length > 0)
                        {

                            NSIndexPath NextIndex = NSIndexPath.FromRowSection(arrVisibleIndex[0].Row + 50, 0);
                            if (NextIndex.Row >= GlobalSettings.Lines.Count)
                            {
                                NextIndex = NSIndexPath.FromRowSection(bidLineList.TableView.NumberOfRowsInSection(0) - 1, 0);
                                bidLineList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            }
                            else
                                bidLineList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            scrlPath = NextIndex;
                        }
                    }
                    else if (GlobalSettings.WBidINIContent.ViewType == 3)
                    {
                        NSIndexPath[] arrVisibleIndex = modernList.TableView.IndexPathsForVisibleRows;
                        if (arrVisibleIndex.Length > 0)
                        {
                            NSIndexPath NextIndex = NSIndexPath.FromRowSection(arrVisibleIndex[0].Row + 50, 0);
                            if (NextIndex.Row >= GlobalSettings.Lines.Count)
                            {
                                NextIndex = NSIndexPath.FromRowSection(modernList.TableView.NumberOfRowsInSection(0) - 1, 0);
                                modernList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            }
                            else
                                modernList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            scrlPath = NextIndex;

                            ReloadModernViewOverlay();
                        }
                    }
                });

            DownsingleTap.NumberOfTapsRequired = 1;
            btnDownArrow.AddGestureRecognizer(DownsingleTap);


            UITapGestureRecognizer DownDoubleTap;
            DownDoubleTap = new UITapGestureRecognizer(() =>
                {
                    if (GlobalSettings.WBidINIContent.ViewType == 1)
                    {

                        NSIndexPath[] arrVisibleIndex = sumList.TableView.IndexPathsForVisibleRows;

                        if (arrVisibleIndex.Length > 0)
                        {
                            NSIndexPath NextIndex = NSIndexPath.FromRowSection(arrVisibleIndex[0].Row + 100, 0);
                            if (NextIndex.Row >= GlobalSettings.Lines.Count)
                            {
                                NextIndex = NSIndexPath.FromRowSection(sumList.TableView.NumberOfRowsInSection(0) - 1, 0);
                                sumList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            }
                            else
                                sumList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            scrlPath = NextIndex;
                        }
                    }
                    else if (GlobalSettings.WBidINIContent.ViewType == 2)
                    {

                        NSIndexPath[] arrVisibleIndex = bidLineList.TableView.IndexPathsForVisibleRows;
                        if (arrVisibleIndex.Length > 0)
                        {

                            NSIndexPath NextIndex = NSIndexPath.FromRowSection(arrVisibleIndex[0].Row + 100, 0);
                            if (NextIndex.Row >= GlobalSettings.Lines.Count)
                            {
                                NextIndex = NSIndexPath.FromRowSection(bidLineList.TableView.NumberOfRowsInSection(0) - 1, 0);
                                bidLineList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            }
                            else
                                bidLineList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            scrlPath = NextIndex;
                        }
                    }
                    else if (GlobalSettings.WBidINIContent.ViewType == 3)
                    {
                        NSIndexPath[] arrVisibleIndex = modernList.TableView.IndexPathsForVisibleRows;
                        if (arrVisibleIndex.Length > 0)
                        {
                            NSIndexPath NextIndex = NSIndexPath.FromRowSection(arrVisibleIndex[0].Row + 100, 0);
                            if (NextIndex.Row >= GlobalSettings.Lines.Count)
                            {
                                NextIndex = NSIndexPath.FromRowSection(modernList.TableView.NumberOfRowsInSection(0) - 1, 0);
                                modernList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            }
                            else
                                modernList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            scrlPath = NextIndex;
                            ReloadModernViewOverlay();
                        }
                    }
                });

            DownDoubleTap.NumberOfTapsRequired = 2;
            btnDownArrow.AddGestureRecognizer(DownDoubleTap);

            DownsingleTap.RequireGestureRecognizerToFail(DownDoubleTap);
        }

        public void ReloadModernViewOverlay()
        {
            if (GlobalSettings.WBidStateCollection.BidAwards != null && GlobalSettings.WBidStateCollection.BidAwards.Count != 0 && GlobalSettings.WbidUserContent.UserInformation.EmpNo != null)
            {

                //need to show the red line border for the awarded lime.
                var awardedline = GlobalSettings.WBidStateCollection.BidAwards.FirstOrDefault(x => x.EmpNum == Convert.ToInt32(GlobalSettings.WbidUserContent.UserInformation.EmpNo));
                if (awardedline != null)
                {
                    var linedata = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == awardedline.LineNum);
                    if (linedata != null)
                    {
                        linedata.ManualScroll = 4;

                        //NSIndexPath[] arrVisibleIndex = modernList.TableView.IndexPathsForVisibleRows;
                        //ModernViewCellClassic cell = (ModernViewCellClassic)modernList.TableView.CellAt(indexPath);
                        //        cell.createCellBorder(GlobalSettings.Lines[indexPath.Row], cell);


                    }

                }

            }
            modernList.ViewDidLoad();

            if (CommonClass.IsModernScrollClassic != "TRUE" && GlobalSettings.WBidINIContent.ViewType == 3)
            {

                ///foreach (ModernViewCellClassic cell in    modernList.TableView.VisibleCells) {
                /// 
                if (modernList.TableView.Source is ModernViewControllerSource)
                {
                    ModernViewControllerSource Source = (ModernViewControllerSource)modernList.TableView.Source;
                    Source.dragging = false;
                    Source.FastDragging = true;
                    Source.LoadIndex = 0;
                }
                //}
                //Modified the code on Aug 17th 2017-- Commented the table  view reload and add it in the bottom of the function
                modernList.TableView.ReloadData();
                //if (scrlPath != null)
                //modernList.TableView.ScrollToRow (scrlPath, UITableViewScrollPosition.Top, false);
            }
            if (scrlPath != null)
                modernList.TableView.ScrollToRow(scrlPath, UITableViewScrollPosition.Top, false);
            modernList.TableView.ReloadData();

        }
        public void ReloadModernView(NSNotification n)
        {
            if (modernList != null && GlobalSettings.WBidINIContent.ViewType == 3)
            {
                //modernList.ViewDidLoad();

                vwContainerView.Hidden = false;
                vwSummaryContainer.Hidden = true;
                vwBidLineContainer.Hidden = true;

                //modernList.View.RemoveFromSuperview ();
                //modernList.RemoveFromParentViewController ();
                //modernList = null;
                //this.View.LayoutIfNeeded();



                if (scrlPath != null)
                    modernList.TableView.ScrollToRow(scrlPath, UITableViewScrollPosition.Top, false);

            }
            ReloadModernViewOverlay();
            HandleBlueShadowButton();

        }

        public void HandleBlueShadowButton()
        {
            var isBlueLineExists = GlobalSettings.Lines.FirstOrDefault(x => x.ManualScroll == 1 || x.ManualScroll == 2 || x.ManualScroll == 3);
            //if (GlobalSettings.WBidINIContent.User.IsModernViewShade)
            if (isBlueLineExists != null && GlobalSettings.WBidINIContent.User.IsModernViewShade == true)
            {

                btnBlueShade.SetBackgroundImage(new UIImage("blueBorderSelected"), UIControlState.Normal);
            }
            else
            {
                btnBlueShade.SetBackgroundImage(new UIImage("blueBorder"), UIControlState.Normal);
            }

        }

        public void DoubleTapNavigation()
        {

            UITapGestureRecognizer UpsingleTap;
            UpsingleTap = new UITapGestureRecognizer(() =>
                {
                    if (GlobalSettings.WBidINIContent.ViewType == 1)
                    {

                        NSIndexPath[] arrVisibleIndex = sumList.TableView.IndexPathsForVisibleRows;
                        if (arrVisibleIndex.Length > 0)
                        {
                            NSIndexPath NextIndex = NSIndexPath.FromRowSection(arrVisibleIndex[0].Row - 50, 0);
                            if (NextIndex.Row < 0)
                            {
                                NextIndex = NSIndexPath.FromRowSection(0, 0);
                                sumList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            }
                            else
                                sumList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            scrlPath = NextIndex;
                        }
                    }
                    else if (GlobalSettings.WBidINIContent.ViewType == 2)
                    {

                        NSIndexPath[] arrVisibleIndex = bidLineList.TableView.IndexPathsForVisibleRows;
                        if (arrVisibleIndex.Length > 0)
                        {
                            NSIndexPath NextIndex = NSIndexPath.FromRowSection(arrVisibleIndex[0].Row - 50, 0);
                            if (NextIndex.Row < 0)
                            {
                                NextIndex = NSIndexPath.FromRowSection(0, 0);
                                bidLineList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            }
                            else
                                bidLineList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            scrlPath = NextIndex;
                        }
                    }
                    else if (GlobalSettings.WBidINIContent.ViewType == 3)
                    {

                        NSIndexPath[] arrVisibleIndex = modernList.TableView.IndexPathsForVisibleRows;
                        if (arrVisibleIndex.Length > 0)
                        {
                            NSIndexPath NextIndex = NSIndexPath.FromRowSection(arrVisibleIndex[0].Row - 50, 0);
                            if (NextIndex.Row < 0)
                            {
                                NextIndex = NSIndexPath.FromRowSection(0, 0);
                                modernList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            }
                            else
                                modernList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            scrlPath = NextIndex;
                            ReloadModernViewOverlay();
                        }
                    }
                });

            UpsingleTap.NumberOfTapsRequired = 1;
            btnUpArrow.AddGestureRecognizer(UpsingleTap);

            UITapGestureRecognizer UpDoubleTap;
            UpDoubleTap = new UITapGestureRecognizer(() =>
                {
                    if (GlobalSettings.WBidINIContent.ViewType == 1)
                    {

                        NSIndexPath[] arrVisibleIndex = sumList.TableView.IndexPathsForVisibleRows;
                        if (arrVisibleIndex.Length > 0)
                        {
                            NSIndexPath NextIndex = NSIndexPath.FromRowSection(arrVisibleIndex[0].Row - 100, 0);
                            if (NextIndex.Row < 0)
                            {
                                NextIndex = NSIndexPath.FromRowSection(0, 0);
                                sumList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            }
                            else
                                sumList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            scrlPath = NextIndex;
                        }
                    }
                    else if (GlobalSettings.WBidINIContent.ViewType == 2)
                    {

                        NSIndexPath[] arrVisibleIndex = bidLineList.TableView.IndexPathsForVisibleRows;
                        if (arrVisibleIndex.Length > 0)
                        {
                            NSIndexPath NextIndex = NSIndexPath.FromRowSection(arrVisibleIndex[0].Row - 100, 0);
                            if (NextIndex.Row < 0)
                            {
                                NextIndex = NSIndexPath.FromRowSection(0, 0);
                                bidLineList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            }
                            else
                                bidLineList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            scrlPath = NextIndex;
                        }
                    }
                    else if (GlobalSettings.WBidINIContent.ViewType == 3)
                    {

                        NSIndexPath[] arrVisibleIndex = modernList.TableView.IndexPathsForVisibleRows;
                        if (arrVisibleIndex.Length > 0)
                        {
                            NSIndexPath NextIndex = NSIndexPath.FromRowSection(arrVisibleIndex[0].Row - 100, 0);
                            if (NextIndex.Row < 0)
                            {
                                NextIndex = NSIndexPath.FromRowSection(0, 0);
                                modernList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            }
                            else
                                modernList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            scrlPath = NextIndex;
                            ReloadModernViewOverlay();
                        }
                    }
                });

            UpDoubleTap.NumberOfTapsRequired = 2;
            btnUpArrow.AddGestureRecognizer(UpDoubleTap);

            UpsingleTap.RequireGestureRecognizerToFail(UpDoubleTap);
        }

        public void LongPressHandling()
        {
            UILongPressGestureRecognizer longPressUp = new UILongPressGestureRecognizer(() =>
                {
                    if (GlobalSettings.WBidINIContent.ViewType == 1)
                    {


                        if (GlobalSettings.Lines.Count > 0)
                        {
                            NSIndexPath NextIndex = NSIndexPath.FromRowSection(0, 0);
                            sumList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            scrlPath = NextIndex;

                        }

                    }
                    else if (GlobalSettings.WBidINIContent.ViewType == 2)
                    {

                        if (GlobalSettings.Lines.Count > 0)
                        {
                            NSIndexPath NextIndex = NSIndexPath.FromRowSection(0, 0);
                            bidLineList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            scrlPath = NextIndex;

                        }
                    }
                    else if (GlobalSettings.WBidINIContent.ViewType == 3)
                    {

                        if (GlobalSettings.Lines.Count > 0)
                        {

                            NSIndexPath NextIndex = NSIndexPath.FromRowSection(0, 0);
                            modernList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Top, true);
                            scrlPath = NextIndex;
                            ReloadModernViewOverlay();
                        }
                    }
                });

            btnUpArrow.AddGestureRecognizer(longPressUp);
            longPressUp.DelaysTouchesBegan = true;



            UILongPressGestureRecognizer longPressDown = new UILongPressGestureRecognizer(() =>
                {
                    if (GlobalSettings.WBidINIContent.ViewType == 1)
                    {


                        if (GlobalSettings.Lines.Count > 0)
                        {
                            NSIndexPath NextIndex = NSIndexPath.FromRowSection(GlobalSettings.Lines.Count - 1, 0);
                            sumList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Bottom, true);
                            scrlPath = NextIndex;
                        }

                    }
                    else if (GlobalSettings.WBidINIContent.ViewType == 2)
                    {

                        if (GlobalSettings.Lines.Count > 0)
                        {
                            NSIndexPath NextIndex = NSIndexPath.FromRowSection(GlobalSettings.Lines.Count - 1, 0);
                            bidLineList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Bottom, true);
                            scrlPath = NextIndex;
                        }
                    }
                    else if (GlobalSettings.WBidINIContent.ViewType == 3)
                    {

                        if (GlobalSettings.Lines.Count > 0)
                        {

                            NSIndexPath NextIndex = NSIndexPath.FromRowSection(GlobalSettings.Lines.Count - 1, 0);
                            modernList.TableView.ScrollToRow(NextIndex, UITableViewScrollPosition.Bottom, true);
                            scrlPath = NextIndex;
                            ReloadModernViewOverlay();
                        }
                    }
                });

            btnDownArrow.AddGestureRecognizer(longPressDown);
            longPressUp.DelaysTouchesBegan = true;

        }
        void applyMIL()
        {
            MILData milData;
            if (File.Exists(WBidHelper.MILFilePath))
            {
                var loadingOverlay = new LoadingOverlay(View.Bounds, "Applying MIL. Please wait..");
                View.Add(loadingOverlay);

                InvokeInBackground(() =>
                {
                    LineInfo lineInfo = null;
                    using (FileStream milStream = File.OpenRead(WBidHelper.MILFilePath))
                    {

                        MILData milDataobject = new MILData();
                        milData = ProtoSerailizer.DeSerializeObject(WBidHelper.MILFilePath, milDataobject, milStream);
                    }

                    GlobalSettings.MILDates = wBIdStateContent.MILDateList;


                    GlobalSettings.MILDates = GenarateOrderedMILDates(wBIdStateContent.MILDateList);
                    //Apply MIL values (calculate property values including Modern bid line properties
                    //==============================================

                    GlobalSettings.MILData = milData.MILValue;
                    GlobalSettings.MenuBarButtonStatus.IsMIL = true;

                    RecalcalculateLineProperties recalcalculateLineProperties = new RecalcalculateLineProperties();
                    recalcalculateLineProperties.CalcalculateLineProperties();

                    InvokeOnMainThread(() =>
                    {
                        loadingOverlay.Hide();
                        CommonClass.lineVC.SetVacButtonStates();
                        NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);

                    });
                });
            }

        }
        private List<Weekday> GetWeekDays(int year, int month)
        {

            List<Weekday> dates = new List<Weekday>();
            CultureInfo ci = new CultureInfo("en-US");

            for (int i = 1; i <= ci.Calendar.GetDaysInMonth(year, month); i++)
            {

                if (new DateTime(year, month, i).DayOfWeek == DayOfWeek.Saturday)
                {
                    dates.Add(new Weekday { Day = new DateTime(year, month, i).AddDays(-6).Day, StartDate = new DateTime(year, month, i).AddDays(-6).Date, EndDate = new DateTime(year, month, i) });
                }

            }
            //need to add one extra sunday 
            dates.Add(new Weekday { Day = new DateTime(year, month, dates[dates.Count - 1].Day).AddDays(7).Day, StartDate = new DateTime(year, month, dates[dates.Count - 1].Day).AddDays(7).Date, EndDate = new DateTime(year, month, dates[dates.Count - 1].Day).AddDays(13).Date });
            for (int i = 0; i < dates.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        dates[0].Code = "A";
                        break;
                    case 1:
                        dates[1].Code = "B";
                        break;
                    case 2:
                        dates[2].Code = "C";
                        break;
                    case 3:
                        dates[3].Code = "D";
                        break;
                    case 4:
                        dates[4].Code = "E";
                        break;
                    case 5:
                        dates[5].Code = "F";
                        break;
                }
            }
            return dates;

        }
        private List<Absense> GenarateOrderedMILDates(List<Absense> milList)
        {
            List<Absense> absence = new List<Absense>();
            if (milList.Count > 0)
            {
                absence.Add(new Absense
                {
                    StartAbsenceDate = milList.FirstOrDefault().StartAbsenceDate,
                    EndAbsenceDate = milList.FirstOrDefault().EndAbsenceDate,
                    AbsenceType = "VA"
                });

                for (int count = 0; count < milList.Count - 1; count++)
                {
                    if ((milList[count + 1].StartAbsenceDate - milList[count].EndAbsenceDate).Days == 1)
                    {
                        absence[absence.Count - 1].EndAbsenceDate = milList[count + 1].EndAbsenceDate;
                    }
                    else
                    {
                        absence.Add(new Absense
                        {
                            StartAbsenceDate = milList[count + 1].StartAbsenceDate,
                            EndAbsenceDate = milList[count + 1].EndAbsenceDate,
                            AbsenceType = "VA"
                        });
                    }
                }
            }
            return absence;
        }

        void btnQuickSetTapped(object sender, EventArgs e)
        {
            QuickSetViewController quickContent = new QuickSetViewController();
            var navigation = new UINavigationController(quickContent);
            navigation.NavigationBar.BarTintColor = ColorClass.TopHeaderColor;
            navigation.NavigationBar.TitleTextAttributes = new UIStringAttributes() { ForegroundColor = UIColor.White };
            navigation.NavigationBar.TintColor = UIColor.White;
            var popController = new UIPopoverController(navigation);
            popController.BackgroundColor = ColorClass.BottomHeaderColor;
            popController.PopoverContentSize = new CGSize(400, 500);
            CGRect senderframe = btnQuickSet.Frame;
            senderframe.X = btnQuickSet.Frame.GetMidX();
            popController.PresentFromRect(senderframe, tbBottomBar, UIPopoverArrowDirection.Any, true);

        }

        void btnRedoTapped(object sender, EventArgs e)
        {
            if (GlobalSettings.RedoStack.Count > 0)
            {
                var state = GlobalSettings.RedoStack[0];
                bool isNeedtoRecreateMILFile = false;
                if (state.MILDateList != null && wBIdStateContent.MILDateList != null)
                    isNeedtoRecreateMILFile = checkToRecreateMILFile(state.MILDateList, wBIdStateContent.MILDateList);
                StateManagement stateManagement = new StateManagement();
                stateManagement.UpdateWBidStateContent();

                var stateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == state.StateName);

                if (stateContent != null)
                {
                    GlobalSettings.UndoStack.Insert(0, new WBidState(stateContent));
                    GlobalSettings.WBidStateCollection.StateList.Remove(stateContent);
                    GlobalSettings.WBidStateCollection.StateList.Insert(0, new WBidState(state));

                }

                GlobalSettings.RedoStack.RemoveAt(0);
                if (isNeedtoRecreateMILFile)
                {
                    GlobalSettings.MILDates = WBidCollection.GenarateOrderedMILDates(state.MILDateList);
                    GlobalSettings.MILData = CreateNewMILFile().MILValue;

                }
                //   StateManagement stateManagement = new StateManagement();
                //stateManagement.ReloadDataFromStateFile();



                bool isneedtoChangeLineFile = stateManagement.IsneedToChangeLineFile(state);
                state.IsOverlapCorrection = false;
                stateManagement.SetMenuBarButtonStatusFromStateFile(state);
                //Setting  status to Global variables
                stateManagement.SetVacationOrOverlapExists(state);

                SetVacButtonStates();
                if (isneedtoChangeLineFile)
                {
                    SetDropButtonTextAndbackground();
                    var loadingOverlay = new LoadingOverlay(View.Bounds, "Please Wait..");
                    View.Add(loadingOverlay);
                    InvokeInBackground(() =>
                    {
                        string isSuccess = WBidHelper.RetrieveSaveAndSetLineFiles(0, state);
                        bool IsFileAvailable = (isSuccess == "Ok") ? true : false;
                        InvokeOnMainThread(() =>
                        {
                            loadingOverlay.Hide();
                            stateManagement.ReloadStateContent(state);
                            //ReloadLineView ();
                            NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);

                        });
                    });
                }
                else
                {
                    stateManagement.ReloadStateContent(state);
                    //ReloadLineView ();
                    NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                }

                /*         bool isNeedToRecalculateLineProp = stateManagement.CheckLinePropertiesNeedToRecalculate(state);
                 ResetLinePropertiesBackToNormal(stateContent, state);
                 ResetOverlapState(stateContent, state);

                 stateManagement.SetMenuBarButtonStatusFromStateFile(state);
                 //Setting  status to Global variables
                 stateManagement.SetVacationOrOverlapExists(state);

                 SetVacButtonStates();

                 if (isNeedToRecalculateLineProp)
                 {
                     var loadingOverlay = new LoadingOverlay(View.Bounds, "Please Wait..");
                     View.Add(loadingOverlay);
                     InvokeInBackground(() =>
                     {

                         stateManagement.RecalculateLineProperties(state);
                         InvokeOnMainThread(() =>
                         {
                             loadingOverlay.Hide();
                             stateManagement.ReloadStateContent(state);
                             //ReloadLineView ();
                             NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);

                         });
                     });
                 }
                 else {

                     stateManagement.ReloadStateContent(state);
                     //ReloadLineView ();
                     NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                 }
                */
            }

            GlobalSettings.isUndo = false;
            GlobalSettings.isRedo = true;
            UpdateUndoRedoButtons();
            GlobalSettings.isModified = true;
            btnSave.Enabled = GlobalSettings.isModified;

        }

        private bool checkToRecreateMILFile(List<Absense> lstPreviosusMIL, List<Absense> lstCurrentMIL)
        {
            bool isNeedtoReCreateMILFile = false;
            if (lstPreviosusMIL.Count != lstCurrentMIL.Count)
                isNeedtoReCreateMILFile = true;
            else
            {
                for (int count = 0; count < lstPreviosusMIL.Count; count++)
                {
                    if (lstPreviosusMIL[count].StartAbsenceDate != lstCurrentMIL[count].StartAbsenceDate || lstPreviosusMIL[count].EndAbsenceDate != lstCurrentMIL[count].EndAbsenceDate)
                    {
                        isNeedtoReCreateMILFile = true;
                        break;
                    }

                }
            }
            return isNeedtoReCreateMILFile;
        }

        void btnUndoTapped(object sender, EventArgs e)
        {
            if (GlobalSettings.UndoStack.Count > 0)
            {
                WBidState state = GlobalSettings.UndoStack[0];
                bool isNeedtoRecreateMILFile = false;
                if (state.MILDateList != null && wBIdStateContent.MILDateList != null)
                    isNeedtoRecreateMILFile = checkToRecreateMILFile(state.MILDateList, wBIdStateContent.MILDateList);
                StateManagement stateManagement = new StateManagement();
                stateManagement.UpdateWBidStateContent();

                var stateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == state.StateName);
                // GloabalStateList = state;
                // wBIdStateContent = state;

                if (stateContent != null)
                {
                    GlobalSettings.RedoStack.Insert(0, new WBidState(stateContent));
                    GlobalSettings.WBidStateCollection.StateList.Remove(stateContent);
                    GlobalSettings.WBidStateCollection.StateList.Insert(0, new WBidState(state));

                }
                //GlobalSettings.RedoStack.Push(state);

                GlobalSettings.UndoStack.RemoveAt(0);

                if (isNeedtoRecreateMILFile)
                {
                    GlobalSettings.MILDates = WBidCollection.GenarateOrderedMILDates(state.MILDateList);
                    GlobalSettings.MILData = CreateNewMILFile().MILValue;

                }
                StateManagement statemanagement = new StateManagement();
                bool isneedtoChangeLineFile = statemanagement.IsneedToChangeLineFile(state);
                state.IsOverlapCorrection = false;
                //Setting Button status to Global variables
                stateManagement.SetMenuBarButtonStatusFromStateFile(state);
                //Setting  status to Global variables
                stateManagement.SetVacationOrOverlapExists(state);

                SetVacButtonStates();
                if (isneedtoChangeLineFile)
                {
                    SetDropButtonTextAndbackground();
                    var loadingOverlay = new LoadingOverlay(View.Bounds, "Please Wait..");
                    View.Add(loadingOverlay);
                    InvokeInBackground(() =>
                    {
                        string isSuccess = WBidHelper.RetrieveSaveAndSetLineFiles(0, state);
                        bool IsFileAvailable = (isSuccess == "Ok") ? true : false;
                        InvokeOnMainThread(() =>
                        {
                            loadingOverlay.Hide();
                            stateManagement.ReloadStateContent(state);
                            //ReloadLineView ();
                            NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);

                        });
                    });
                }
                else
                {
                    stateManagement.ReloadStateContent(state);

                    NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                }

                /*            bool isNeedToRecalculateLineProp = stateManagement.CheckLinePropertiesNeedToRecalculate(state);
                ResetLinePropertiesBackToNormal(stateContent, state);
                ResetOverlapState(stateContent, state);

                //Setting Button status to Global variables
                stateManagement.SetMenuBarButtonStatusFromStateFile(state);
                //Setting  status to Global variables
                stateManagement.SetVacationOrOverlapExists(state);

                SetVacButtonStates();
                if (isNeedToRecalculateLineProp)
                {
                    var loadingOverlay = new LoadingOverlay(View.Bounds, "Please Wait..");
                    View.Add(loadingOverlay);
                    InvokeInBackground(() =>
                    {

                        stateManagement.RecalculateLineProperties(state);


                        InvokeOnMainThread(() =>
                        {
                            loadingOverlay.Hide();
                            stateManagement.ReloadStateContent(state);
                            //ReloadLineView ();
                            NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);

                        });
                    });
                }
                else {
                    stateManagement.ReloadStateContent(state);

                    // stateManagement.ReloadDataFromStateFile();

                    //ReloadLineView ();
                    NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                }
                */
            }

            GlobalSettings.isUndo = true;
            GlobalSettings.isRedo = false;
            UpdateUndoRedoButtons();
            GlobalSettings.isModified = true;
            btnSave.Enabled = GlobalSettings.isModified;
        }

        public void UpdateUndoRedoButtons()
        {
            btnUndo.SetTitle(GlobalSettings.UndoStack.Count.ToString(), UIControlState.Normal);
            btnRedo.SetTitle(GlobalSettings.RedoStack.Count.ToString(), UIControlState.Normal);

            if (GlobalSettings.isUndo)
                btnUndo.SetBackgroundImage(UIImage.FromBundle("undoOrange.png"), UIControlState.Normal);
            else
                btnUndo.SetBackgroundImage(UIImage.FromBundle("undoGreen.png"), UIControlState.Normal);

            if (GlobalSettings.isRedo)
                btnRedo.SetBackgroundImage(UIImage.FromBundle("redoOrange.png"), UIControlState.Normal);
            else
                btnRedo.SetBackgroundImage(UIImage.FromBundle("redoGreen.png"), UIControlState.Normal);

            if (GlobalSettings.UndoStack.Count == 0)
            {
                btnUndo.SetBackgroundImage(UIImage.FromBundle("undoGreen.png"), UIControlState.Normal);
                btnUndo.SetTitle("", UIControlState.Normal);
                btnUndo.Enabled = false;
            }
            else
            {
                //btnRedo.SetBackgroundImage (UIImage.FromBundle ("undoGreen.png"), UIControlState.Normal);
                btnUndo.SetTitle(GlobalSettings.UndoStack.Count.ToString(), UIControlState.Normal);
                btnUndo.Enabled = true;
            }

            if (GlobalSettings.RedoStack.Count == 0)
            {
                btnRedo.SetBackgroundImage(UIImage.FromBundle("redoGreen.png"), UIControlState.Normal);
                btnRedo.SetTitle("", UIControlState.Normal);
                btnRedo.Enabled = false;
            }
            else
            {
                //btnRedo.SetBackgroundImage (UIImage.FromBundle ("redoGreen.png"), UIControlState.Normal);
                btnRedo.SetTitle(GlobalSettings.RedoStack.Count.ToString(), UIControlState.Normal);
                btnRedo.Enabled = true;
            }

        }

        /// <summary>
        /// This will save the current bid state automatically dependes on the Settings in the Configuration=>user tab
        /// </summary>
        public void AutoSave()
        {
            if (GlobalSettings.WBidINIContent.User.AutoSave)
            {
                //GlobalSettings.WBidINIContent.User.AutoSavevalue=1;
                timer = new System.Timers.Timer(GlobalSettings.WBidINIContent.User.AutoSavevalue * 60000)
                {

                    Interval = GlobalSettings.WBidINIContent.User.AutoSavevalue * 60000,
                    Enabled = true
                };
                timer.Elapsed += timer_Elapsed;
            }
        }


        void btnSynchTapped(object sender, EventArgs e)
        {
            //            StateManagement stateManagement = new StateManagement();
            //            stateManagement.UpdateWBidStateContent();
            ////            CompareState stateObj = new CompareState();
            ////            string fileName = WBidHelper.GenerateFileNameUsingCurrentBidDetails();
            ////            var WbidCollection = XmlHelper.ReadStateFile(WBidHelper.GetAppDataPath() + "/" + fileName + ".WBS");
            ////            wBIdStateContent = WbidCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            ////            bool isNochange = stateObj.CompareStateChange(wBIdStateContent, GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName));
            //
            //            if(GlobalSettings.isModified)
            //            {
            //                GlobalSettings.WBidStateCollection.IsModified = true;
            //                WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);
            //
            //                if (timer != null)
            //                {
            //                    timer.Stop();
            //                    timer.Start();
            //                }
            //                //save the state of the INI File
            //                WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
            //
            //                GlobalSettings.isModified = false;
            //                btnSave.Enabled = false;
            //            }



            //if (GlobalSettings.WBidINIContent.User.IsSouthWestWifiTest == false)
            //{




            if (Reachability.CheckVPSAvailable())
            {
                if (!GlobalSettings.SynchEnable)
                {
                    UIAlertController okAlertController = UIAlertController.Create("Smart Sync", "Please enable Smart synchronisation from Configuration Settings", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                }
                else if (GlobalSettings.isModified)
                {

                    UIAlertController okAlertController = UIAlertController.Create("Smart Sync", "Please save the current state before performing synch.", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                }
                else
                {
                    SynchBtn = true;
                    Synch();


                }
            }
            else
            {


                if (WBidHelper.IsSouthWestWifiOr2wire())
                {
                    UIAlertController okAlertController = UIAlertController.Create("Smart Sync", Constants.SouthWestConnectionAlert, UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);

                }
                else
                {

                    UIAlertController okAlertController = UIAlertController.Create("Smart Sync", Constants.VPSDownAlert, UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                }
            }

            //else
            //{
            //    UIAlertController okAlertController = UIAlertController.Create("Smart Sync", GlobalSettings.SouthWestWifiMessage, UIAlertControllerStyle.Alert);
            //    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            //    this.PresentViewController(okAlertController, true, null);
            //}
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            StateManagement stateManagement = new StateManagement();
            stateManagement.UpdateWBidStateContent();
            GlobalSettings.WBidStateCollection.IsModified = true;
            WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);
            //save the state of the INI File
            WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
            GlobalSettings.isModified = false;
            InvokeOnMainThread(() =>
            {
                if (CommonClass.lineVC != null)
                    CommonClass.lineVC.UpdateSaveButton();
                if (CommonClass.cswVC != null)
                    CommonClass.cswVC.UpdateSaveButton();
            });
        }

        private void setReparseView()
        {
            vwReparse.Layer.BorderWidth = 1;
            vwReparse.Layer.BorderColor = ColorClass.SummaryHeaderBorderColor.CGColor;
            btnReparseCheck.TouchUpInside += (object sender, EventArgs e) =>
            {
                ((UIButton)sender).Selected = !((UIButton)sender).Selected;
                if (btnReparseCheck.Selected)
                {
                    GlobalSettings.IsDifferentUser = true;
                    GlobalSettings.ModifiedEmployeeNumber = txtReparse.Text;
                }
                else
                {
                    GlobalSettings.IsDifferentUser = false;
                }
            };
            txtReparse.ShouldChangeCharacters = (textField, range, replacementString) =>
            {
                string text = textField.Text;
                string newText = text.Substring(0, (int)range.Location) + replacementString + text.Substring((int)range.Location + (int)range.Length);
                int val;
                if (newText == "")
                    return true;
                else
                    return Int32.TryParse(newText, out val);
            };
            btnReparse.TouchUpInside += (object sender, EventArgs e) =>
            {
                txtReparse.ResignFirstResponder();

                UIAlertController alert = UIAlertController.Create("WBidMax", "Do you want to test the vacation correction?", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("NO ", UIAlertActionStyle.Cancel, (actionCancel) =>
                {
                    if (btnReparseCheck.Selected)
                    {
                        GlobalSettings.IsDifferentUser = true;
                        GlobalSettings.ModifiedEmployeeNumber = txtReparse.Text;
                    }
                    var loadingOverlay = new LoadingOverlay(View.Bounds, "Reparsing..Please Wait..");
                    View.Add(loadingOverlay);
                    InvokeInBackground(() =>
                    {
                        string zipFilename = WBidHelper.GenarateZipFileName();
                        ReparseParameters reparseParams = new ReparseParameters() { ZipFileName = zipFilename };
                        ReparseBL.ReparseTripAndLineFiles(reparseParams);
                        InvokeOnMainThread(() =>
                        {
                            loadingOverlay.Hide();
                        });
                    });
                }));

                alert.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, (actionOK) =>
                {
                    if (btnReparseCheck.Selected)
                    {
                        GlobalSettings.IsDifferentUser = true;
                        GlobalSettings.ModifiedEmployeeNumber = txtReparse.Text;
                    }
                    TestVacationViewController testVacVC = new TestVacationViewController();
                    testVacVC.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                    this.PresentViewController(testVacVC, true, null);

                }));

                this.PresentViewController(alert, true, null);


            };
            btnReparseClose.TouchUpInside += (object sender, EventArgs e) =>
            {
                txtReparse.ResignFirstResponder();
                vwReparse.Hidden = true;
            };


        }


        public void SetVacButtonStates()
        {
            //            foreach (var column in GlobalSettings.AdditionalColumns) {
            //                column.IsSelected = false;
            //            }
            //            var selectedColumns = GlobalSettings.AdditionalColumns.Where (x => GlobalSettings.WBidINIContent.DataColumns.Any (y => y.Id == x.Id)).ToList ();
            //            foreach (var selectedColumn in selectedColumns) {
            //                selectedColumn.IsSelected = true;
            //            }
            //
            //            foreach (var column in GlobalSettings.AdditionalvacationColumns) {
            //                column.IsSelected = false;
            //            }
            //            var selectedVColumns = GlobalSettings.AdditionalColumns.Where (x => GlobalSettings.WBidINIContent.SummaryVacationColumns.Any (y => y.Id == x.Id)).ToList ();
            //            foreach (var selectedColumn in selectedVColumns) {
            //                selectedColumn.IsSelected = true;
            //            }

            // Configuring Modern view property lists.
            if (GlobalSettings.MenuBarButtonStatus == null)
                GlobalSettings.MenuBarButtonStatus = new MenuBarButtonStatus();

            CommonClass.bidLineProperties = new List<string>();
            CommonClass.modernProperties = new List<string>();

            if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
            {
                foreach (var item in GlobalSettings.WBidINIContent.BidLineVacationColumns)
                {
                    var col = GlobalSettings.BidlineAdditionalvacationColumns.FirstOrDefault(x => x.Id == item);
                    if (col != null)
                        CommonClass.bidLineProperties.Add(col.DisplayName);
                }
                foreach (var item in GlobalSettings.WBidINIContent.ModernVacationColumns)
                {
                    var col = GlobalSettings.ModernAdditionalvacationColumns.FirstOrDefault(x => x.Id == item);
                    if (col != null)
                        CommonClass.modernProperties.Add(col.DisplayName);
                }

            }
            else
            {
                foreach (var item in GlobalSettings.WBidINIContent.BidLineNormalColumns)
                {
                    var col = GlobalSettings.BidlineAdditionalColumns.FirstOrDefault(x => x.Id == item);
                    if (col != null)
                        CommonClass.bidLineProperties.Add(col.DisplayName);
                }
                foreach (var item in GlobalSettings.WBidINIContent.ModernNormalColumns)
                {
                    var col = GlobalSettings.ModernAdditionalColumns.FirstOrDefault(x => x.Id == item);
                    if (col != null)
                        CommonClass.modernProperties.Add(col.DisplayName);
                }

            }



            if (GlobalSettings.IsOverlapCorrection)
            {
                btnOverlap.Enabled = (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsEOM && !GlobalSettings.MenuBarButtonStatus.IsMIL);

            }
            else
            {
                btnOverlap.Enabled = false;
                GlobalSettings.MenuBarButtonStatus.IsOverlap = false;
            }

            if (GlobalSettings.IsVacationCorrection || GlobalSettings.IsFVVacation)
            {
                btnVacCorrect.Enabled = (!GlobalSettings.MenuBarButtonStatus.IsOverlap && !GlobalSettings.MenuBarButtonStatus.IsMIL);

            }
            else
            {
                btnVacCorrect.Enabled = false;

            }



            btnEOM.Enabled = (!GlobalSettings.MenuBarButtonStatus.IsOverlap && !GlobalSettings.MenuBarButtonStatus.IsMIL && (GlobalSettings.CurrentBidDetails.Postion == "FA" || (int)GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1).DayOfWeek == 0 || (int)GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(2).DayOfWeek == 0 || (int)GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(3).DayOfWeek == 0));


            /// btnVacDrop.Enabled = ((GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && GlobalSettings.OrderedVacationDays!=null&&  GlobalSettings.OrderedVacationDays.Count>0) || GlobalSettings.MenuBarButtonStatus.IsEOM);
            btnVacDrop.Enabled = ((GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && GlobalSettings.IsVacationCorrection) || GlobalSettings.MenuBarButtonStatus.IsEOM);

            btnMIL.Enabled = (!GlobalSettings.MenuBarButtonStatus.IsOverlap && !GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsEOM);

            if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsEOM)
            {
                GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
            }


            btnEOM.Selected = GlobalSettings.MenuBarButtonStatus.IsEOM;
            btnVacCorrect.Selected = GlobalSettings.MenuBarButtonStatus.IsVacationCorrection;
            btnVacDrop.Selected = GlobalSettings.MenuBarButtonStatus.IsVacationDrop;
            btnOverlap.Selected = GlobalSettings.MenuBarButtonStatus.IsOverlap;
            btnMIL.Selected = GlobalSettings.MenuBarButtonStatus.IsMIL;

            //if (GlobalSettings.WBidStateCollection != null && GlobalSettings.WBidStateCollection.DataSource == "HistoricalData") {
            //    btnEOM.Enabled = false;
            //    btnVacDrop.Enabled = false;
            //    btnMIL.Enabled = false;
            //    btnVacCorrect.Enabled = false;
            //    btnOverlap.Enabled = false;
            //}
            if (GlobalSettings.WBidStateCollection != null && GlobalSettings.WBidStateCollection.DataSource == "HistoricalData")
            {
                if (GlobalSettings.WBidStateCollection != null)
                {
                    if (GlobalSettings.WBidStateCollection.DataSource == "HistoricalData")
                    {
                        btnEOM.Enabled = false;
                        btnVacDrop.Enabled = false;
                        btnMIL.Enabled = false;
                        btnVacCorrect.Enabled = false;
                        btnOverlap.Enabled = false;
                    }
                    WBidState WBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                    if (wBIdStateContent.IsMissingTripFailed)
                    {
                        btnMIL.Enabled = false;
                    }
                }

            }
        }
        // Show Cover Letter and news here!
        public override void ViewDidAppear(bool animated)
        {

            base.ViewDidAppear(animated);
            //            GlobalSettings.IsNewsShow = true;

            var IsModernScrollClassic = NSUserDefaults.StandardUserDefaults["IsModernScrollClassic"];
            if (IsModernScrollClassic != null)
            {
                CommonClass.IsModernScrollClassic = IsModernScrollClassic.ToString();
            }
            else
            {
                CommonClass.IsModernScrollClassic = "TRUE";
            }

            if (GlobalSettings.IsNewsShow)
            {
                GlobalSettings.IsNewsShow = false;
                InvokeOnMainThread(() =>
                {
                    webPrint fileViewer = new webPrint();
                    fileViewer.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;

                    this.PresentViewController(fileViewer, true, () =>
                    {
                        fileViewer.LoadPDFdocument("news.pdf");
                    });
                });
            }
            else if (GlobalSettings.IsCoverletterShowFileName != string.Empty)
            {

                string coverLetter = GlobalSettings.IsCoverletterShowFileName;
                InvokeOnMainThread(() =>
                {
                    webPrint fileViewer = new webPrint();
                    fileViewer.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    this.PresentViewController(fileViewer, true, () =>
                    {
                        fileViewer.loadFileFromUrl(coverLetter);
                    });
                });
                GlobalSettings.IsCoverletterShowFileName = string.Empty;
            }
            else if (FirstTime)
            {
                FirstTime = false;
                Synch();
                //commented this because we have implemented this in the application load
                //bool isSubScriptionOnlyFor5Days = CommonClass.isSubScriptionOnlyFor5Days();
                //bool IsUserdataAvailable = CommonClass.isUserInformationAvailable();
                //if (IsUserdataAvailable)
                //{
                //    if (GlobalSettings.WbidUserContent.UserInformation.IsYearlySubscribed || GlobalSettings.WbidUserContent.UserInformation.IsMonthlySubscribed)
                //        return;

                //    DateTime PaidUntilDate = GlobalSettings.WbidUserContent.UserInformation.PaidUntilDate ?? DateTime.Now;
                //    int day = CommonClass.DaysBetween(DateTime.Now, PaidUntilDate);
                //    if (isSubScriptionOnlyFor5Days)
                //    {
                //        string message = "";
                //        if (day == 1)
                //            message = "Your subscription will expire in 1 day";
                //        else
                //            message = "Your subscription will expire in " + day + " days.";

                //        UIAlertController alert = UIAlertController.Create("WBidMax", message, UIAlertControllerStyle.Alert);
                //                    alert.AddAction(UIAlertAction.Create("Go To Subscription", UIAlertActionStyle.Cancel, (actionCancel) => {
                //                        SubscriptionViewController ObjSubscription = new SubscriptionViewController();
                //                        ObjSubscription.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                //                        this.PresentViewController(ObjSubscription, true, null);

                //                    }));

                //                    alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {

                //                    }));

                //                    this.PresentViewController(alert, true, null);
                //                }
                //    else if (day < 1)
                //    {

                //                    UIAlertController alert = UIAlertController.Create("WBidMax", "Your subscription expired", UIAlertControllerStyle.Alert);
                //                    alert.AddAction(UIAlertAction.Create("Go To Subscription", UIAlertActionStyle.Cancel, (actionCancel) => {
                //                        SubscriptionViewController ObjSubscription = new SubscriptionViewController();
                //                        ObjSubscription.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                //                        this.PresentViewController(ObjSubscription, true, null);

                //                    }));

                //                    alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {

                //                    }));

                //                    this.PresentViewController(alert, true, null);
                //                }
                //}
            }
            if (GlobalSettings.IsCoverletterShowFileName == string.Empty && GlobalSettings.IsNewsShow == false)
            {
                // GlobalSettings.iSNeedToShowMonthtoMonthAlert = true;
                //Show Month to Month vacation alert
                //temporary disable for one test flight
                ShowMonthToMonthVacationAlert();
            }
        }

        private void ShowMonthToMonthVacationAlert()
        {
            List<Vacation> uservacation = new List<Vacation>();
            try
            {
                if (GlobalSettings.CurrentBidDetails.Postion != "FA" && GlobalSettings.iSNeedToShowMonthtoMonthAlert == true)
                {
                    

                    List<Weekday> lstweekdays = GetWeekDays(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month);

                   
                     uservacation = GlobalSettings.WBidStateCollection.Vacation;
                    List<Weekday> vacationweeks = lstweekdays.Where(x => uservacation.Any(y => DateTime.Parse(y.StartDate,CultureInfo.InvariantCulture) == x.StartDate && DateTime.Parse(y.EndDate,CultureInfo.InvariantCulture) == x.EndDate)).ToList();
                    bool isneedtoShowAlert = vacationweeks.Any(x => x.Code.Contains("A") || x.Code.Contains("E"));
                    if (isneedtoShowAlert == true)
                    {
                        var startDateA = "";
                        var endDateA = "";
                        var startDateE = "";
                        var endDateE = "";

                        string AlertText = string.Empty;
                        var codeArray = vacationweeks.Select(x => x.Code);

                        if (codeArray.Contains("A") && codeArray.Contains("E"))
                        {
                            //AE Vacation
                            startDateA = vacationweeks.Find(x => x.Code == "A").StartDate.Day + " " + vacationweeks.Find(x => x.Code == "A").StartDate.ToString("MMM");
                            endDateA = vacationweeks.Find(x => x.Code == "A").EndDate.Day + " " + vacationweeks.Find(x => x.Code == "A").EndDate.ToString("MMM");

                            startDateE = vacationweeks.Find(x => x.Code == "E").StartDate.Day + " " + vacationweeks.Find(x => x.Code == "E").StartDate.ToString("MMM");
                            endDateE = vacationweeks.Find(x => x.Code == "E").EndDate.Day + " " + vacationweeks.Find(x => x.Code == "E").EndDate.ToString("MMM");

                            AlertText = "You have  'A & E' week vacation: \n\n" + startDateA + " - " + endDateA + " and " + startDateE + " - " + endDateE;
                            AlertText += "\n\nA weeks generally are the lead out month and E weeks generally are the lead-in month of a month-to-month vacation..";
                            AlertText += "\n\nThere are opportunities with Month-To-Month Vacations, but there are ALSO limitations.";
                        }
                        else if (codeArray.Contains("A"))
                        {
                            //A Vacation
                            startDateA = vacationweeks.Find(x => x.Code == "A").StartDate.Day + " " + vacationweeks.Find(x => x.Code == "A").StartDate.ToString("MMM");
                            endDateA = vacationweeks.Find(x => x.Code == "A").EndDate.Day + " " + vacationweeks.Find(x => x.Code == "A").EndDate.ToString("MMM");
                            AlertText = "You have  'A' week vacation: \n\n" + startDateA + " - " + endDateA;
                            AlertText += "\n\nA weeks generally are the lead out month of a month-to - month vacation.";
                            AlertText += "\n\nThere are opportunities with Month-To-Month Vacations, but there are ALSO limitations.";

                        }
                        else if (codeArray.Contains("E"))
                        {
                            //E Vacation
                            startDateE = vacationweeks.Find(x => x.Code == "E").StartDate.Day + " " + vacationweeks.Find(x => x.Code == "E").StartDate.ToString("MMM");
                            endDateE = vacationweeks.Find(x => x.Code == "E").EndDate.Day + " " + vacationweeks.Find(x => x.Code == "E").EndDate.ToString("MMM");
                            AlertText = "You have  'E' week vacation: \n\n" + startDateE + " - " + endDateE;
                            AlertText += "\n\nE weeks generally are the lead-in month of a month-to-month vacation..";
                            AlertText += "\n\nThere are opportunities with Month-To-Month Vacations, but there are ALSO limitations.";
                        }

                        AlertText += "\n\nWe suggest you read the following documents to improve your bidding knowledge";

                        ShowMonthToMonthAlerView(AlertText);
                    }
                }

            }
            catch (Exception ex)
            {
                List<Vacation> uservacation1 = GlobalSettings.WBidStateCollection.Vacation;
                string message = string.Empty;
                foreach (var item in uservacation1)
                {
                    message += item.StartDate + " " + item.EndDate + "--";
                }
                UIAlertController okAlertController = UIAlertController.Create("WBidMax", message, UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
                
            }
        }

        private void OpenMonthToMonthAlert(NSNotification obj)
        {

        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            btnSave.Enabled = GlobalSettings.isModified;
            UpdateUndoRedoButtons();
            if (GlobalSettings.CurrentBidDetails.Postion != "FA")
                btnMIL.Hidden = !GlobalSettings.WBidINIContent.User.MIL;
            else
                btnMIL.Hidden = true;
            SetVacButtonStates();
        }

        private void applyOverLapCorrection()
        {
            string overlayTxt = string.Empty;
            ReCalculateLinePropertiesForOverlapCorrection reCalculateLinePropertiesForOverlapCorrection = new ReCalculateLinePropertiesForOverlapCorrection();
            overlayTxt = "Applying Overlap Correction";

            SetVacButtonStates();

            LoadingOverlay overlay = new LoadingOverlay(this.View.Frame, overlayTxt);
            this.View.Add(overlay);
            InvokeInBackground(() =>
            {
                try
                {
                    reCalculateLinePropertiesForOverlapCorrection.ReCalculateLinePropertiesOnOverlapCorrection(GlobalSettings.Lines.ToList(), true);
                    SortLineList();
                }
                catch (Exception ex)
                {
                    InvokeOnMainThread(() =>
                    {
                        throw ex;
                    });
                }

                InvokeOnMainThread(() =>
                {
                    overlay.Hide();
                    NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);

                    CommonClass.lineVC.UpdateSaveButton();
                });
            });
        }

        private void applyVacation()
        {
            try
            {
                var str = string.Empty;
                if (btnEOM.Selected)
                    str = "Applying EOM";
                else
                    str = "Applying Vacation Correction";

                LoadingOverlay overlay = new LoadingOverlay(this.View.Frame, str);
                this.View.Add(overlay);
                InvokeInBackground(() =>
                {
                    try
                    {
                        WBidCollection.GenarateTempAbsenceList();
                        PrepareModernBidLineView prepareModernBidLineView = new PrepareModernBidLineView();
                        RecalcalculateLineProperties RecalcalculateLineProperties = new PortableLibrary.BusinessLogic.RecalcalculateLineProperties();
                        prepareModernBidLineView.CalculatebidLinePropertiesforVacation();
                        RecalcalculateLineProperties.CalcalculateLineProperties();
                    }
                    catch (Exception ex)
                    {
                        InvokeOnMainThread(() =>
                        {
                            throw ex;
                        });
                    }

                    InvokeOnMainThread(() =>
                    {
                        NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                        overlay.RemoveFromSuperview();
                    });
                });
            }
            catch (Exception ex)
            {

                throw ex;

            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            //NSNotificationCenter.DefaultCenter.PostNotificationName("CalPopHide", null);
        }

        private void GoToLine(string line)
        {
            this.vwCalPopover.Hidden = true;
            this.vwTripPopover.Hidden = true;
            int lineNo = Convert.ToInt32(line);
            foreach (Line ln in GlobalSettings.Lines)
            {
                int index;
                if (ln.LineNum == lineNo)
                {
                    index = GlobalSettings.Lines.IndexOf(ln);
                    lPath = NSIndexPath.FromRowSection(index, 0);
                }
            }
            if (lPath != null)
            {

                if (sgControlViewType.SelectedSegment == 0 && sumList != null)
                {

                    sumList.TableView.SelectRow(lPath, true, UITableViewScrollPosition.None);
                    sumList.TableView.ScrollToRow(lPath, UITableViewScrollPosition.None, true);
                    scrlPath = lPath;

                }

                else if (sgControlViewType.SelectedSegment == 1 && bidLineList != null)
                {

                    bidLineList.TableView.SelectRow(lPath, true, UITableViewScrollPosition.None);
                    bidLineList.TableView.ScrollToRow(lPath, UITableViewScrollPosition.None, true);
                    scrlPath = lPath;

                }
                else if (sgControlViewType.SelectedSegment == 2 && modernList != null)
                {

                    modernList.TableView.SelectRow(lPath, true, UITableViewScrollPosition.None);
                    modernList.TableView.ScrollToRow(lPath, UITableViewScrollPosition.None, true);
                    scrlPath = lPath;
                    ReloadModernViewOverlay();

                }
            }


            if (scrlPath != null)
            {

                if (sgControlViewType.SelectedSegment == 0 && sumList != null)
                {
                    sumList.TableView.SelectRow(lPath, true, UITableViewScrollPosition.None);
                    sumList.TableView.ScrollToRow(scrlPath, UITableViewScrollPosition.None, true);

                }

                else if (sgControlViewType.SelectedSegment == 1 && bidLineList != null)
                {
                    bidLineList.TableView.SelectRow(lPath, true, UITableViewScrollPosition.None);
                    bidLineList.TableView.ScrollToRow(scrlPath, UITableViewScrollPosition.None, true);

                }
                else if (sgControlViewType.SelectedSegment == 2 && modernList != null)
                {
                    modernList.TableView.SelectRow(lPath, true, UITableViewScrollPosition.None);
                    modernList.TableView.ScrollToRow(scrlPath, UITableViewScrollPosition.None, true);
                    ReloadModernViewOverlay();

                }
            }


        }

        private void addPanGestures()
        {
            var dragCal = new UIPanGestureRecognizer(handleCalPopPan);
            dragCal.MinimumNumberOfTouches = 1;
            dragCal.MaximumNumberOfTouches = 1;
            this.vwCalPopover.AddGestureRecognizer(dragCal);

            var dragTrip = new UIPanGestureRecognizer(handleTripPopPan);
            dragTrip.MinimumNumberOfTouches = 1;
            dragTrip.MaximumNumberOfTouches = 1;
            this.vwTripPopover.AddGestureRecognizer(dragTrip);

            var swipeCal1 = new UISwipeGestureRecognizer(handleCalSwipeDown);
            swipeCal1.NumberOfTouchesRequired = 1;
            swipeCal1.Direction = UISwipeGestureRecognizerDirection.Down;
            this.vwCalChild.AddGestureRecognizer(swipeCal1);

            var swipeCal2 = new UISwipeGestureRecognizer(handleCalSwipeUp);
            swipeCal2.NumberOfTouchesRequired = 1;
            swipeCal2.Direction = UISwipeGestureRecognizerDirection.Up;
            this.vwCalChild.AddGestureRecognizer(swipeCal2);

            dragCal.RequireGestureRecognizerToFail(swipeCal1);
            dragCal.RequireGestureRecognizerToFail(swipeCal2);

        }

        public void handleCalPopPan(UIPanGestureRecognizer gest)
        {
            UIView viewCal = this.sumList.View;
            if (gest.State == UIGestureRecognizerState.Began || gest.State == UIGestureRecognizerState.Changed)
            {
                CGPoint trans = gest.TranslationInView(viewCal);
                CGPoint newCenter = new CGPoint(this.vwCalPopover.Center.X + trans.X, this.vwCalPopover.Center.Y + trans.Y);
                float xMin = (float)viewCal.Bounds.X + (float)vwCalPopover.Frame.Width / 4;
                float xMax = (float)viewCal.Bounds.Width - (float)vwCalPopover.Frame.Width / 4;
                float yMin = (float)viewCal.Bounds.Y + (float)vwCalPopover.Frame.Height / 4;
                float yMax = (float)viewCal.Bounds.Height - (float)vwCalPopover.Frame.Height / 4;
                bool inside = true;
                //    bool inside = (newCenter.Y >= yMin && newCenter.Y <= yMax && newCenter.X >= xMin && newCenter.X <= xMax);
                if (inside)
                    this.vwCalPopover.Center = new CGPoint(this.vwCalPopover.Center.X + trans.X, this.vwCalPopover.Center.Y + trans.Y);
                gest.SetTranslation(CGPoint.Empty, viewCal);
            }

        }

        public void handleTripPopPan(UIPanGestureRecognizer gest)
        {
            UIView viewTrip = this.sumList.View;
            if (gest.State == UIGestureRecognizerState.Began || gest.State == UIGestureRecognizerState.Changed)
            {
                CGPoint trans = gest.TranslationInView(viewTrip);
                CGPoint newCenter = new CGPoint(this.vwTripPopover.Center.X + trans.X, this.vwTripPopover.Center.Y + trans.Y);
                float xMin = (float)viewTrip.Bounds.X + (float)vwTripPopover.Frame.Width / 4;
                float xMax = (float)viewTrip.Bounds.Width - (float)vwTripPopover.Frame.Width / 4;
                float yMin = (float)viewTrip.Bounds.Y + (float)vwTripPopover.Frame.Height / 4;
                float yMax = (float)viewTrip.Bounds.Height - (float)vwTripPopover.Frame.Height / 4;
                bool inside = true;
                //bool inside = (newCenter.Y >= yMin && newCenter.Y <= yMax && newCenter.X >= xMin && newCenter.X <= xMax);
                if (inside)
                    this.vwTripPopover.Center = new CGPoint(this.vwTripPopover.Center.X + trans.X, this.vwTripPopover.Center.Y + trans.Y);
                gest.SetTranslation(CGPoint.Empty, viewTrip);
            }
        }

        public void handleCalSwipeDown(UISwipeGestureRecognizer gest)
        {
            if (gest.State == UIGestureRecognizerState.Ended)
            {
                if (lPath.Row != GlobalSettings.Lines.Count - 1)
                    moveDown();
            }
        }

        public void handleCalSwipeUp(UISwipeGestureRecognizer gest)
        {
            if (gest.State == UIGestureRecognizerState.Ended)
            {
                if (lPath.Row != 0)
                    moveUp();
            }
        }

        partial void btnTripExportTapped(UIKit.UIButton sender)
        {
            if (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO")
            {
                UIActionSheet sheet = new UIActionSheet("Select", null, null, null, new string[] {
                    "Export to Calendar",
                    "Export to FFDO"
                });
                sheet.ShowFrom(((UIButton)sender).Frame, vwTripPopover, true);
                sheet.Clicked += (object senderSheet, UIButtonEventArgs e) =>
                {
                    if (e.ButtonIndex == 0)
                    {
                        ExportTripDetails(tripNum, CommonClass.selectedLine, CommonClass.isLastTrip);
                    }
                    else
                    {
                        //FFDO
                        string ffdoData = GetFlightDataforFFDB(tripNum, CommonClass.isLastTrip);
                        UIPasteboard clipBoard = UIPasteboard.General;
                        clipBoard.String = ffdoData;
                    }
                };
            }
            else
            {
                ExportTripDetails(tripNum, CommonClass.selectedLine, CommonClass.isLastTrip);
            }
        }

        /// <summary>
        /// Genarate FFDO Data for a line
        /// </summary>
        /// <returns></returns>
        private string GenarateFFDOforLine()
        {
            string result = string.Empty;
            Line selectedline = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == CommonClass.selectedLine);
            if (selectedline != null)
            {
                bool isLastTrip = false;
                int paringCount = 0;
                foreach (string pairing in selectedline.Pairings)
                {
                    isLastTrip = ((selectedline.Pairings.Count - 1) == paringCount);
                    paringCount++;
                    result += GetFlightDataforFFDB(pairing, isLastTrip);
                }
            }
            return result;
        }

        /// <summary>
        /// PURPOSE : Get Flight data for FFDB
        /// </summary>
        /// <param name="trip"></param>
        /// <param name="tripName"></param>
        private string GetFlightDataforFFDB(string tripNum, bool isLastTrip)
        {

            Trip trip = GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == tripNum.Substring(0, 4));
            trip = trip ?? GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == tripNum);
            string result = string.Empty;

            // var tripStartDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, int.Parse(SelectedDay.Replace(" ", "")));
            DateTime dutPeriodDate = WBidCollection.SetDate(int.Parse(tripNum.Substring(4, 2)), isLastTrip, GlobalSettings.CurrentBidDetails);
            //DateTime dutPeriodDate = SelectedTripStartDate;

            foreach (var dp in trip.DutyPeriods)
            {
                string datestring = dutPeriodDate.ToString("MM'/'dd'/'yyyy");
                if (trip.ReserveTrip)
                {

                    result += datestring + "  RSRV " + trip.DepSta + " " + Helper.CalcTimeFromMinutesFromMidnight(Convert.ToString(dp.ReserveOut % 1440)).Replace(":", "") + " " + trip.RetSta + " " + Helper.CalcTimeFromMinutesFromMidnight(Convert.ToString(dp.ReserveIn % 1440)).Replace(":", "") + " \n";
                }
                else
                {
                    foreach (var flt in dp.Flights)
                    {
                        result += datestring + " " + flt.FltNum.ToString().PadLeft(4, '0') + " " + flt.DepSta + " " + Helper.CalcTimeFromMinutesFromMidnight(flt.DepTime.ToString()).Replace(":", "") + " " + flt.ArrSta + " " + Helper.CalcTimeFromMinutesFromMidnight(flt.ArrTime.ToString()).Replace(":", "") + " \n";
                    }
                }
                dutPeriodDate = dutPeriodDate.AddDays(1);
            }
            return result;
        }


        private void CalPrint()
        {
            var content = string.Empty;
            content += "---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------\n";
            var line = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == CommonClass.selectedLine);

            content += "Line " + line.LineNum + "\t ";
            content += CommonClass.bidLineProperties[0] + "\t " + GetLineProperty(CommonClass.bidLineProperties[0], line) + "\t";

            foreach (var item in line.BidLineTemplates)
            {
                if (item.Date.Day.ToString().Length == 1)
                    content += item.Date.Day.ToString() + "   ";
                else
                    content += item.Date.Day.ToString() + "  ";
            }
            content += "\n" + "\t ";
            content += CommonClass.bidLineProperties[1] + "\t " + GetLineProperty(CommonClass.bidLineProperties[1], line) + "\t";

            foreach (var item in line.BidLineTemplates)
            {
                content += item.Date.DayOfWeek.ToString().Substring(0, 2).ToUpper() + "  ";
            }
            content += "\n" + "\t ";
            content += CommonClass.bidLineProperties[2] + "\t " + GetLineProperty(CommonClass.bidLineProperties[2], line) + "\t";

            foreach (var item in line.BidLineTemplates)
            {
                if (string.IsNullOrEmpty(item.TripNum))
                    content += "*" + "   ";
                else
                    content += item.TripNum + " ";
            }
            content += "\n" + "\t ";
            content += CommonClass.bidLineProperties[3] + "\t " + GetLineProperty(CommonClass.bidLineProperties[3], line) + "\t";

            foreach (var item in line.BidLineTemplates)
            {
                if (string.IsNullOrEmpty(item.ArrStaLastLeg))
                    content += "*" + "   ";
                else
                    content += item.ArrStaLastLeg + " ";
            }
            content += "\n" + "\t ";
            content += CommonClass.bidLineProperties[4] + "\t " + GetLineProperty(CommonClass.bidLineProperties[4], line) + "\t";

            content += line.Pairingdesription;
            content += "\n";
            content += "---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------\n\n";

            foreach (var item in line.BidLineTemplates)
            {
                if (!string.IsNullOrEmpty(item.TripNum))
                {
                    CorrectionParams correctionParams = new Model.CorrectionParams();
                    correctionParams.selectedLineNum = CommonClass.selectedLine;
                    ObservableCollection<TripData> trip = TripViewBL.GenerateTripDetails(item.TripName, correctionParams, false);
                    foreach (var tr in trip)
                    {
                        content += tr.Content + "\n";
                    }
                    content += "---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------\n\n";
                }
            }
            //            content += CommonClass.bidLineProperties [0] + "\t " + GetLineProperty (CommonClass.bidLineProperties [0], line)+"\n";
            //            content += CommonClass.bidLineProperties [1] + "\t " + GetLineProperty (CommonClass.bidLineProperties [1], line)+"\n";
            //            content += CommonClass.bidLineProperties [2] + "\t " + GetLineProperty (CommonClass.bidLineProperties [2], line)+"\n";
            //            content += CommonClass.bidLineProperties [3] + "\t " + GetLineProperty (CommonClass.bidLineProperties [3], line)+"\n";
            //            content += CommonClass.bidLineProperties [4] + "\t " + GetLineProperty (CommonClass.bidLineProperties [4], line)+"\n";

            var attributes = new UIStringAttributes() { Font = UIFont.FromName("Courier", 5) };
            var printDoc = new NSAttributedString(content, attributes);

            //            var textVW = new UITextView(this.View.Frame);
            //            textVW.AttributedText = printDoc;
            //            this.Add(textVW);
            //
            //            return;

            var printInfo = UIPrintInfo.PrintInfo;
            printInfo.OutputType = UIPrintInfoOutputType.General;
            printInfo.JobName = "My Calendar Print Job";

            var textFormatter = new UISimpleTextPrintFormatter(printDoc)
            {
                StartPage = 0,
                ContentInsets = new UIEdgeInsets(5, 5, 5, 5),
                //MaximumContentWidth = 6 * 72,
            };

            var printer = UIPrintInteractionController.SharedPrintController;
            printer.PrintInfo = printInfo;
            printer.PrintFormatter = textFormatter;
            printer.ShowsPageRange = true;
            printer.PresentFromRectInView(btnCalPrint.Frame, vwCalPopover, true, (handler, completed, err) =>
            {
                if (!completed && err != null)
                {
                    Console.WriteLine("error");
                }
                else if (completed)
                {
                    UIAlertController okAlertController = UIAlertController.Create("WBidMax", "The line has been sent for printout.", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                }
            });
        }

        private void CalFormatPrint()
        {
            var content = string.Empty;
            var line = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == CommonClass.selectedLine);

            string heading = string.Empty;
            //May-2015-BWI CP Line14

            if (GlobalSettings.CurrentBidDetails != null)
            {
                heading = heading +
                    CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(GlobalSettings.CurrentBidDetails.Month) + " " + GlobalSettings.CurrentBidDetails.Domicile + " " + GlobalSettings.CurrentBidDetails.Postion;
            }
            if (line != null)
            {
                heading += " Line " + line.LineNum;
            }

            string InitalContent = "<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">\r\n<html>\r\n<head>\r\n  <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">\r\n  <meta http-equiv=\"Content-Style-Type\" content=\"text/css\">\r\n  <title></title>\r\n  <meta name=\"Generator\" content=\"Cocoa HTML Writer\">\r\n  <meta name=\"CocoaVersion\" content=\"1347.57\">\r\n  <style type=\"text/css\">\r\n    p.p1 {margin: 0.0px 0.0px 1.0px 0.0px; text-align: center; font: 12.0px Arial; color: #2e3e44; -webkit-text-stroke: #2e3e44}\r\n    p.p2 {margin: 0.0px 0.0px 1.0px 0.0px;  font: 12.0px Arial; color: #2e3e44; -webkit-text-stroke: #2e3e44}\r\n  p.p4 {margin: 0.0px 0.0px 1.0px 0.0px;  font: 12.0px Courier; color: #2e3e44; -webkit-text-stroke: #2e3e44}\r\n    p.p3 {margin: 0.0px 0.0px 1.0px 0.0px; text-align: center; font: 12.0px Arial; color: #2e3e44; -webkit-text-stroke: #2e3e44; min-height: 14.0px}\r\n    span.s1 {font-kerning: none; }\r\n    table.t1 {border-style: solid; border-width: 0.5px 0.5px 0.5px 0.5px; border-color: #cbcbcb ; border-collapse: collapse}\r\n    table.t2 {border-style: solid; border-width: 0.5px 0.5px 0.5px 0.5px; border-color: #cbcbcb ; border-collapse: collapse}\r\n    td.td1 {width: 70.0px; border-style: solid; border-width: 1px ; border-color:  #979797; padding:5px; }\r\n    td.td2 {width: 10.0px; border-style: solid; border-width: 0.0px 1.0px 1.0px 1.0px; border-color: #979797;}\r\n\t td.td3 { border:solid 1px #979797; border-right:none; border-top:none; width:20px; text-align:center;}\r\n\t .heading{ font-family:Arial, Helvetica, sans-serif; font-weight:bold; text-align:center;}\r\n  </style>\r\n</head>\r\n<body>\r\n    <table cellspacing=\"0\" cellpadding=\"0\" class=\"t1\" align =\"Center\">\r\n <tr>\r\n      <td valign=\"top\" colspan=\"7\" class=\"td1 heading\">" + heading + "</td>\r\n\t  <tr>\r\n  <tbody>\r\n   \r\n    <tr>\r\n      <td valign=\"top\" class=\"td1\">\r\n        <p class=\"p1\"><span class=\"s1\"><b>SUN</b></span></p>\r\n      </td>\r\n      <td valign=\"top\" class=\"td1\">\r\n        <p class=\"p1\"><span class=\"s1\"><b>MON</b></span></p>\r\n      </td>\r\n      <td valign=\"top\" class=\"td1\">\r\n        <p class=\"p1\"><span class=\"s1\"><b>TUE</b></span></p>\r\n      </td>\r\n      <td valign=\"top\" class=\"td1\">\r\n        <p class=\"p1\"><span class=\"s1\"><b>WED</b></span></p>\r\n      </td>\r\n      <td valign=\"top\" class=\"td1\">\r\n        <p class=\"p1\"><span class=\"s1\"><b>THU</b></span></p>\r\n      </td>\r\n      <td valign=\"top\" class=\"td1\">\r\n        <p class=\"p1\"><span class=\"s1\"><b>FRI</b></span></p>\r\n      </td>\r\n      <td valign=\"top\" class=\"td1\">\r\n        <p class=\"p1\"><span class=\"s1\"><b>SAT</b></span></p>\r\n      </td>\r\n    </tr>\r\n    <tr>";


            string DynamicDays = string.Empty;
            string saparator = " </tr>\n    <tr>";
            for (int i = 0; i < CommonClass.calData.Count; i++)
            {
                if (i % 7 == 0)
                {
                    DynamicDays += saparator;
                }

                string DepTimeFirstLeg = CommonClass.calData[i].DepTimeFirstLeg;
                string ArrStaLastLeg = CommonClass.calData[i].ArrStaLastLeg;
                string LandTimeLastLeg = CommonClass.calData[i].LandTimeLastLeg;
                if (string.IsNullOrEmpty(DepTimeFirstLeg))
                    DepTimeFirstLeg = "    ";

                if (string.IsNullOrEmpty(ArrStaLastLeg))
                    ArrStaLastLeg = "    ";


                if (string.IsNullOrEmpty(LandTimeLastLeg))
                    LandTimeLastLeg = "    ";

                if (string.IsNullOrEmpty(CommonClass.calData[i].Day))
                {
                    DynamicDays += "<td valign=\"top\" class=\"td2\">\r\n       \r\n        <p class=\"p2\"><span class=\"s1\"><br>\r\n</span></p>\r\n<p class=\"p2\"><span class=\"s1\"><br>\r\n</span></p>\r\n        <p class=\"p3\">" + DepTimeFirstLeg + "</p>\r\n        <p class=\"p3\">" + ArrStaLastLeg + "</p>\r\n        <p class=\"p3\">" + LandTimeLastLeg + "</p>\r\n        <p class=\"p2\"><span class=\"s1\"><br>\r\n</span></p>\r\n      </td>";
                }
                else
                    DynamicDays += "<td valign=\"top\" class=\"td2\">\r\n        <table cellspacing=\"0\" cellpadding=\"0\" class=\"t2\" align=\"right\">\r\n          <tbody>\r\n            <tr>\r\n              <td valign=\"middle\" class=\"td3\">\r\n                <p class=\"p2\"><span class=\"s1\"><b>" + CommonClass.calData[i].Day + "</b></span></p>\r\n              </td>\r\n            </tr>\r\n          </tbody>\r\n        </table>\r\n        <p class=\"p2\"><span class=\"s1\"><br>\r\n</span></p>\r\n<p class=\"p2\"><span class=\"s1\"><br>\r\n</span></p>\r\n        <p class=\"p3\">" + DepTimeFirstLeg + "</p>\r\n        <p class=\"p3\">" + ArrStaLastLeg + "</p>\r\n        <p class=\"p3\">" + LandTimeLastLeg + "</p>\r\n        <p class=\"p2\"><span class=\"s1\"><br>\r\n</span></p>\r\n      </td>";
            }

            DynamicDays += " </tr>\n    \r\n  </tbody>";
            string hotels = "<br/>";
            var hotelLst = CalendarViewBL.GenerateCalendarAndHotelDetails(line);
            foreach (var hot in hotelLst)
            {
                hotels += hot + "<br/>";
            }
            string holidayDetails = "<tr>\r\n              <td valign=\"top\" colspan=\"7\" class=\"td4\"> <p class=\"p4\">" + hotels + "</p></td>\r\n              <tr>";
            string FinalContent = "</table>\r\n\r\n</body>\r\n</html>";

            content = InitalContent + DynamicDays + holidayDetails + FinalContent;
            var web = new UIWebView();
            web.Frame = new CGRect(10, 10, 700, 900);

            web.LoadHtmlString(content, null);





            var attributes = new UIStringAttributes() { Font = UIFont.FromName("Courier", 5) };
            var printDoc = new NSAttributedString(content, attributes);

            var printInfo = UIPrintInfo.PrintInfo;

            printInfo.OutputType = UIPrintInfoOutputType.General;
            printInfo.JobName = "CalendarPrint";

            var textFormatter = new UISimpleTextPrintFormatter(printDoc)
            {
                StartPage = 0,

                ContentInsets = new UIEdgeInsets(5, 5, 5, 5),
                MaximumContentWidth = 6 * 72,
            };
            web.ViewPrintFormatter.ContentInsets = new UIEdgeInsets(200, 5, 5, 5);
            var printer = UIPrintInteractionController.SharedPrintController;
            printer.PrintInfo = printInfo;
            printer.PrintFormatter = web.ViewPrintFormatter;

            printer.ShowsPageRange = true;
            printer.PresentFromRectInView(btnCalPrint.Frame, vwCalPopover, true, (handler, completed, err) =>
            {
                if (!completed && err != null)
                {
                    Console.WriteLine("error");
                }
                else if (completed)
                {
                    UIAlertController okAlertController = UIAlertController.Create("WBidMax", "The line has been sent for printout.", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                }
            });

        }

        void TripPrint()
        {
            var content = string.Empty;
            foreach (TripData data in CommonClass.tripData)
            {
                content += "\n";
                content += data.Content;
            }
            var attributes = new UIStringAttributes() { Font = UIFont.FromName("Courier", 10) };
            var printDoc = new NSAttributedString(content, attributes);

            var printInfo = UIPrintInfo.PrintInfo;
            printInfo.OutputType = UIPrintInfoOutputType.General;
            printInfo.JobName = "My Trip Print Job";

            var textFormatter = new UISimpleTextPrintFormatter(printDoc)
            {
                StartPage = 0,
                ContentInsets = new UIEdgeInsets(5, 5, 5, 5),
            };

            var printer = UIPrintInteractionController.SharedPrintController;
            printer.PrintInfo = printInfo;
            printer.PrintFormatter = textFormatter;
            printer.ShowsPageRange = true;
            printer.PresentFromRectInView(btnTripPrint.Frame, vwTripPopover, true, (handler, completed, err) =>
            {
                if (!completed && err != null)
                {
                    Console.WriteLine("error");
                }
                else if (completed)
                {
                    UIAlertController okAlertController = UIAlertController.Create("WBidMax", "The Trip has been sent for printout.", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                }
            });

        }

        private string GetLineProperty(string displayName, Line line)
        {
            if (displayName == "$/Day")
            {
                return line.TfpPerDay.ToString();
            }
            else if (displayName == "$/DHr")
            {
                return line.TfpPerDhr.ToString();
            }
            else if (displayName == "$/Hr")
            {
                return line.TfpPerFltHr.ToString();
            }
            else if (displayName == "$/TAFB")
            {
                return line.TfpPerTafb.ToString();
            }
            else if (displayName == "+Grd")
            {
                return line.LongestGrndTime.ToString(@"hh\:mm");
            }
            else if (displayName == "+Legs")
            {
                return line.MostLegs.ToString();
            }
            else if (displayName == "+Off")
            {
                return line.LargestBlkOfDaysOff.ToString();
            }
            else if (displayName == "1Dy")
            {
                return line.Trips1Day.ToString();
            }
            else if (displayName == "2Dy")
            {
                return line.Trips2Day.ToString();
            }
            else if (displayName == "3Dy")
            {
                return line.Trips3Day.ToString();
            }
            else if (displayName == "4Dy")
            {
                return line.Trips4Day.ToString();
            }
            else if (displayName == "787m8m")
            {
                return line.Equip8753.ToString();
            }
            else if (displayName == "A/P")
            {
                return line.AMPM.ToString();
            }
            else if (displayName == "ACChg")
            {
                return line.AcftChanges.ToString();
            }
            else if (displayName == "ACDay")
            {
                return line.AcftChgDay.ToString();
            }
            else if (displayName == "CO")
            {
                return line.CarryOverTfp.ToString();
            }
            else if (displayName == "DP")
            {
                return line.TotDutyPds.ToString();
            }
            else if (displayName == "DPinBP")
            {
                return line.TotDutyPdsInBp.ToString();
            }
            else if (displayName == "EDomPush")
            {
                return line.EDomPush;
            }
            else if (displayName == "EPush")
            {
                return line.EPush;
            }
            else if (displayName == "FA Posn")
            {
                return string.Join("", line.FAPositions.ToArray());
            }
            else if (displayName == "Flt")
            {
                return line.BlkHrsInBp;
            }
            else if (displayName == "LArr")
            {
                return line.LastArrTime.ToString();
            }
            else if (displayName == "LDomArr")
            {
                return line.LastDomArrTime.ToString();
            }
            else if (displayName == "Legs")
            {
                return line.Legs.ToString();
            }
            else if (displayName == "LgDay")
            {
                return line.LegsPerDay.ToString();
            }
            else if (displayName == "LgPair")
            {
                return line.LegsPerPair.ToString();
            }
            else if (displayName == "ODrop")
            {
                return line.OverlapDrop.ToString();
            }
            else if (displayName == "Off")
            {
                return line.DaysOff.ToString();
            }
            else if (displayName == "Pairs")
            {
                return line.TotPairings.ToString();
            }
            else if (displayName == "Pay")
            {
                return Decimal.Round(line.Tfp, 2).ToString();
            }
            else if (displayName == "PDiem")
            {
                return line.TafbInBp;
            }
            else if (displayName == "MyValue")
            {
                return Decimal.Round(line.Points, 2).ToString();
            }
            else if (displayName == "SIPs")
            {
                return line.Sips.ToString();
            }
            else if (displayName == "StartDOW")
            {
                return line.StartDow;
            }
            else if (displayName == "T234")
            {
                return line.T234;
            }
            else if (displayName == "VDrop")
            {
                return line.VacationDrop.ToString();
            }
            else if (displayName == "WkEnd")
            {
                if (line.Weekend != null)
                    return line.Weekend.ToLower();
                else
                    return "";
            }
            else if (displayName == "FltRig")
            {
                return line.RigFltInBP.ToString();
            }
            else if (displayName == "MinPayRig")
            {
                return line.RigDailyMinInBp.ToString();
            }
            else if (displayName == "DhrRig")
            {
                return line.RigDhrInBp.ToString();
            }
            else if (displayName == "AdgRig")
            {
                return line.RigAdgInBp.ToString();
            }
            else if (displayName == "TafbRig")
            {
                return line.RigTafbInBp.ToString();
            }
            else if (displayName == "TotRig")
            {
                return line.RigTotalInBp.ToString();
            }
            else if (displayName == "VacPayCu")
            {
                return Decimal.Round(line.VacPayCuBp, 2).ToString();
            }
            else if (displayName == "VacPayNe")
            {
                return Decimal.Round(line.VacPayNeBp, 2).ToString();
            }
            else if (displayName == "VacPayBo")
            {
                return Decimal.Round(line.VacPayBothBp, 2).ToString();
            }
            else if (displayName == "Vofrnt")
            {
                return Decimal.Round(line.VacationOverlapFront, 2).ToString();
            }
            else if (displayName == "Vobk")
            {
                return Decimal.Round(line.VacationOverlapBack, 2).ToString();
            }
            else if (displayName == "800legs")
            {
                return line.LegsIn800.ToString();
            }
            else if (displayName == "700legs")
            {
                return line.LegsIn700.ToString();
            }
            //else if (displayName == "500legs")
            //{
            //    return line.LegsIn500.ToString();
            //}
            //else if (displayName == "300legs")
            //{
            //    return line.LegsIn300.ToString();
            //}
            else if (displayName == "8Max")
            {
                return line.LegsIn600.ToString();
            }
            else if (displayName == "7Max")
            {
                return line.LegsIn200.ToString();
            }
            else if (displayName == "DhrInBp")
            {
                return line.DutyHrsInBp;
            }
            else if (displayName == "DhrInLine")
            {
                return line.DutyHrsInLine;
            }
            else if (displayName == "Wts")
            {
                return Decimal.Round(line.TotWeight, 2).ToString();
            }
            else if (displayName == "HolRig")
                return Decimal.Round(line.HolRig, 2).ToString();
            else if (displayName == "nMid")
            {
                return Decimal.Round(line.TotWeight, 2).ToString();


            }
            else if (displayName == "cmts")
            {

                return Decimal.Round(line.TotalCommutes, 2).ToString();

            }
            else if (displayName == "cmtFr")
            {
                return Decimal.Round(line.commutableFronts, 2).ToString();


            }
            else if (displayName == "cmtBa")
            {
                return Decimal.Round(line.CommutableBacks, 2).ToString();


            }
            else if (displayName == "cmt%Fr")
            {
                return Decimal.Round(line.CommutabilityFront, 2).ToString();


            }
            else if (displayName == "cmt%Ba")
            {
                return Decimal.Round(line.CommutabilityBack, 2).ToString();


            }
            else if (displayName == "cmt%Ov")
            {
                return Decimal.Round(line.CommutabilityOverall, 2).ToString();


            }
            else if (displayName == "Ratio")
            {
                return Decimal.Round(line.Ratio, 2).ToString();


            }

            else
            {
                return "";
            }
        }

        partial void btnCalPrintTapped(UIKit.UIButton sender)
        {
            //CalPrint ();

            UIActionSheet sheet = new UIActionSheet("Select", null, null, null, new string[] {
                "BidLine with Pairings",
                "Calendar View"
            });
            sheet.ShowFrom(((UIButton)sender).Frame, vwCalPopover, true);
            sheet.Dismissed += (object senderSheet, UIButtonEventArgs e) =>
            {
                if (e.ButtonIndex == 0)
                {
                    CalPrint();
                }
                else
                {
                    CalFormatPrint();
                }
            };
        }

        partial void btnTripPrintTapped(UIKit.UIButton sender)
        {
            TripPrint();
        }

        partial void btnCalendarExport(Foundation.NSObject sender)
        {
            if (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO")
            {
                UIActionSheet sheet = new UIActionSheet("Select", null, null, null, new string[] {
                    "Export to Calendar",
                    "Export to FFDO"
                });
                sheet.ShowFrom(((UIButton)sender).Frame, vwCalPopover, true);
                sheet.Clicked += (object senderSheet, UIButtonEventArgs e) =>
                {
                    if (e.ButtonIndex == 0)
                    {
                        ExportToCalendar();
                    }
                    else
                    {
                        //FFDO
                        string result = GenarateFFDOforLine();
                        UIPasteboard clipBoard = UIPasteboard.General;
                        clipBoard.String = result;
                        UIAlertController okAlertController = UIAlertController.Create("WBidMax", "The FFDO line details have been copied to the clipboard", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);

                    }
                };
            }
            else
            {
                ExportToCalendar();
            }
        }

        void ExportToCalendar()
        {
            List<ExportCalendar> lstExportCalendar = new List<ExportCalendar>();
            Line exportLine = GlobalSettings.Lines[lPath.Row];
            if (exportLine != null)
            {
                Trip trip = null;
                DateTime tripDate = DateTime.MinValue;
                bool isLastTrip = false;
                int paringCount = 0;
                foreach (string pairing in exportLine.Pairings)
                {
                    trip = GetTrip(pairing);
                    isLastTrip = ((exportLine.Pairings.Count - 1) == paringCount);
                    paringCount++;
                    tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    if (GlobalSettings.WBidINIContent.PairingExport.IsEntirePairing)
                    {
                        lstExportCalendar.Add(ExportTripDetails(trip, tripDate, pairing, exportLine.LineNum));
                    }
                    else
                    {
                        var dutyDetails = ExportDutyPeriodDetails(trip, tripDate, pairing, exportLine.LineNum);
                        if (dutyDetails != null)
                        {
                            foreach (var item in dutyDetails)
                            {
                                lstExportCalendar.Add(item);
                            }
                        }
                    }
                }
            }
            Calendar cal = new Calendar();
            cal.EventStore.RequestAccess(EKEntityType.Event, (bool granted, NSError e) =>
            {
                if (granted)
                {
                    foreach (ExportCalendar exp in lstExportCalendar)
                    {
                        NSError err;
                        try
                        {
                            if (exp.StarDdate > exp.EndDate)
                                exp.EndDate = exp.EndDate.AddDays(1);
                            EKEvent newEvent = EKEvent.FromStore(cal.EventStore);
                            newEvent.StartDate = exp.StarDdate.DateTimeToNSDate();
                            newEvent.EndDate = exp.EndDate.DateTimeToNSDate();
                            newEvent.Title = exp.Title;
                            newEvent.Notes = exp.TripDetails;
                            newEvent.Calendar = cal.EventStore.DefaultCalendarForNewEvents;
                            cal.EventStore.SaveEvent(newEvent, EKSpan.ThisEvent, out err);
                        }
                        catch
                        {
                            err = new NSError();
                        }
                        if (err == null && e == null)
                        {
                            InvokeOnMainThread(() =>
                            {
                                UIAlertController okAlertController = UIAlertController.Create("WBidMax", "The line was added to the calendar.", UIAlertControllerStyle.Alert);
                                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                                this.PresentViewController(okAlertController, true, null);
                            });
                        }
                        else
                        {
                            InvokeOnMainThread(() =>
                            {
                                UIAlertController okAlertController = UIAlertController.Create("WBidMax", "Calendar Export Failed.", UIAlertControllerStyle.Alert);
                                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                                this.PresentViewController(okAlertController, true, null);
                            });
                        }
                    }
                    Console.WriteLine("Calendar access granted.");
                }
                else
                {
                    InvokeOnMainThread(() =>
                    {
                        UIAlertController okAlertController = UIAlertController.Create("Access Denied", "User Denied Access to Calendar \nPlease change the privacy settings for WbidiPad in Settings / Privacy / Calendars / WbidiPad", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);

                    });
                }
            });
        }


        private void ExportTripDetails(string tripName, int lineNum, bool isLastTrip)
        {
            List<ExportCalendar> lstExportCalendar = new List<ExportCalendar>();
            // Line exportLine = GlobalSettings.Lines[lPath.Row];

            //if (exportLine != null)
            //{

            Trip trip = null;
            DateTime tripDate = DateTime.MinValue;


            //  foreach (string pairing in exportLine.Pairings)
            // {
            trip = GetTrip(tripName);

            tripDate = WBidCollection.SetDate(Convert.ToInt16(tripName.Substring(4, 2)), isLastTrip);

            if (GlobalSettings.WBidINIContent.PairingExport.IsEntirePairing)
            {
                lstExportCalendar.Add(ExportTripDetails(trip, tripDate, tripName, lineNum));
            }
            else
            {

                var dutyDetails = ExportDutyPeriodDetails(trip, tripDate, tripName, lineNum);
                if (dutyDetails != null)
                {
                    foreach (var item in dutyDetails)
                    {
                        lstExportCalendar.Add(item);

                    }
                }

            }

            // }
            // }

            Calendar cal = new Calendar();
            cal.EventStore.RequestAccess(EKEntityType.Event,
                (bool granted, NSError e) =>
                {
                    if (granted)
                    {
                        foreach (ExportCalendar exp in lstExportCalendar)
                        {
                            NSError err;
                            try
                            {
                                if (exp.StarDdate > exp.EndDate)
                                    exp.EndDate = exp.EndDate.AddDays(1);
                                EKEvent newEvent = EKEvent.FromStore(cal.EventStore);
                                newEvent.StartDate = exp.StarDdate.DateTimeToNSDate();
                                newEvent.EndDate = exp.EndDate.DateTimeToNSDate();
                                newEvent.Title = exp.Title;
                                newEvent.Notes = exp.TripDetails;
                                newEvent.Calendar = cal.EventStore.DefaultCalendarForNewEvents;
                                cal.EventStore.SaveEvent(newEvent, EKSpan.ThisEvent, out err);
                            }
                            catch
                            {
                                err = new NSError();
                            }
                            if (err == null && e == null)
                            {
                                InvokeOnMainThread(() =>
                                {
                                    UIAlertController okAlertController = UIAlertController.Create("WBidMax", "The trip was added to the calendar.", UIAlertControllerStyle.Alert);
                                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                                    this.PresentViewController(okAlertController, true, null);
                                });
                            }
                            else
                            {
                                InvokeOnMainThread(() =>
                                {
                                    UIAlertController okAlertController = UIAlertController.Create("WBidMax", "Trip Export Failed", UIAlertControllerStyle.Alert);
                                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                                    this.PresentViewController(okAlertController, true, null);
                                });
                            }
                        }
                        Console.WriteLine("Calendar access granted.");
                    }
                    else
                    {
                        InvokeOnMainThread(() =>
                        {
                            UIAlertController okAlertController = UIAlertController.Create("Access Denied", "User Denied Access to Calendar \nPlease change the privacy settings for WbidiPad in Settings / Privacy / Calendars / WbidiPad", UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);

                        });
                    }
                });

        }


        private List<ExportCalendar> ExportDutyPeriodDetails(Trip trip, DateTime tripStartDate, string tripName, int lineNum)
        {
            List<ExportCalendar> lstExportCalendar = new List<ExportCalendar>();


            CorrectionParams correctionParams = new Model.CorrectionParams();
            correctionParams.selectedLineNum = lineNum;
            foreach (DutyPeriod dp in trip.DutyPeriods)
            {
                ExportCalendar exportCalendar = new ExportCalendar();
                int day = 0;
                //string startTime = GetTime(Helper.ConvertMinuteToHHMM(dp.ShowTime - (1440 * (dp.DutPerSeqNum - 1))), out day);
                string startTime = FormatTime((dp.ShowTime - (1440 * (dp.DutPerSeqNum - 1))), out day);
                if (GlobalSettings.WBidINIContent.PairingExport.IsCentralTime)
                {
                    exportCalendar.StarDdate = DateTime.Parse(tripStartDate.AddDays(day).ToShortDateString() + " " + startTime);
                }
                else
                {
                    bool isDayBefore;
                    string modifiedtime;
                    DateTime date = tripStartDate.AddDays(Convert.ToDouble(day));
                    var time = ConvertTimeToDomicile(startTime, tripStartDate.AddDays(Convert.ToDouble(day)), out isDayBefore, out modifiedtime);
                    if (isDayBefore)
                    {
                        date = date.AddDays(-1);
                        time = modifiedtime;
                    }
                    exportCalendar.StarDdate = DateTime.Parse(date.ToShortDateString() + " " + time);
                    //exportCalendar.StarDdate = DateTime.Parse (tripStartDate.AddDays (Convert.ToDouble(day)).ToShortDateString () + " " + ConvertTimeToDomicile (startTime, tripStartDate.AddDays (Convert.ToDouble(day))));
                }
                // string endTime = GetTime(Helper.ConvertMinuteToHHMM(dp.ReleaseTime - (1440 * (dp.DutPerSeqNum - 1))), out day);
                string endTime = FormatTime((dp.LandTimeLastLeg + GlobalSettings.debrief) - ((dp.DutPerSeqNum - 1) * 1440), out day);
                if (GlobalSettings.WBidINIContent.PairingExport.IsCentralTime)
                {
                    exportCalendar.EndDate = DateTime.Parse(tripStartDate.AddDays(day).ToShortDateString() + " " + endTime);
                }
                else
                {

                    bool isDayBefore;
                    string modifiedtime;
                    DateTime date = tripStartDate.AddDays(Convert.ToDouble(day));
                    var time = ConvertTimeToDomicile(endTime, tripStartDate.AddDays(Convert.ToDouble(day)), out isDayBefore, out modifiedtime);
                    if (isDayBefore)
                    {
                        date = date.AddDays(-1);
                        time = modifiedtime;
                    }
                    exportCalendar.EndDate = DateTime.Parse(date.ToShortDateString() + " " + time);
                    //exportCalendar.EndDate = DateTime.Parse (tripStartDate.AddDays (Convert.ToDouble(day)).ToShortDateString () + " " + ConvertTimeToDomicile (endTime, tripStartDate.AddDays (Convert.ToDouble(day))));
                    //exportCalendar.EndDate = DateTime.Parse(tripStartDate.AddDays(Convert.ToDouble(day)).AddMinutes(ConvertTimeToDomicile(endTime, tripStartDate.AddDays(Convert.ToDouble(day)))));
                }
                string subject = dp.ArrStaLastLeg;





                if (GlobalSettings.WBidINIContent.PairingExport.IsCentralTime)
                {
                    subject = dp.ArrStaLastLeg + " " + FormatTime((dp.LandTimeLastLeg + GlobalSettings.debrief) % 1440, out day);
                }
                else
                {

                    string domicileTime = FormatTime((dp.LandTimeLastLeg + GlobalSettings.debrief) - ((dp.DutPerSeqNum - 1) * 1440), out day);
                    bool isDayBefore;
                    string modifiedtime;
                    var startingtime = ConvertTimeToDomicile(domicileTime, tripStartDate.AddDays(day), out isDayBefore, out modifiedtime);
                    if (isDayBefore)
                    {
                        subject = dp.ArrStaLastLeg + " " + modifiedtime;
                    }
                    else
                    {
                        subject = dp.ArrStaLastLeg + " " + startingtime;
                    }
                    //subject = dp.ArrStaLastLeg + " " + ConvertTimeToDomicile (domicileTime, tripStartDate.AddDays (day));

                }

                if (GlobalSettings.WBidINIContent.PairingExport.IsSubjectLineSelected)
                {
                    subject = trip.TripNum.Substring(0, 4) + " " + subject;
                }
                exportCalendar.Title = subject;

                ObservableCollection<TripData> lstDutyperiodDetails = TripViewBL.GenerateDutyPeriodDetails(trip, correctionParams, dp.DutPerSeqNum);


                string body = string.Empty;
                foreach (var item in lstDutyperiodDetails)
                {
                    body += item.Content + Environment.NewLine;

                }

                exportCalendar.TripDetails = "REPORT " + startTime + " CST/CDT";
                body += Environment.NewLine + Environment.NewLine + "(Note: All times are CST/CDT unless otherwise noted.)";
                if (!GlobalSettings.WBidINIContent.PairingExport.IsCentralTime)
                {
                    exportCalendar.TripDetails += " (" + startTime + " Domicile Time)";
                    body += Environment.NewLine + "(Note: AppointmentDetails times  are in Domicile time).";
                }
                exportCalendar.TripDetails += Environment.NewLine + Environment.NewLine + body;



                lstExportCalendar.Add(exportCalendar);
                tripStartDate = tripStartDate.AddDays(1);

            }

            return lstExportCalendar;


        }

        private string FormatTime(int time, out int day)
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
            if (hour >= 24)
            {
                if (minutes > 0)
                {
                    day = 1;
                }

                hour = hour - 24;
                stringTime = hour.ToString().PadLeft(2, '0');
            }
            else
            {
                stringTime = (hour > 12) ? (hour - 12).ToString("d2") : hour.ToString("d2");
            }
            stringTime += ":" + minutes.ToString("d2");

            stringTime += ((hour >= 12) ? " PM" : " AM");

            return stringTime;

        }

        //private string ConvertTimeToDomicile (string time, DateTime date)
        //{




        //    int hours = 0;
        //    int minutes = 0;
        //    int result = 0;
        //    string strTime = string.Empty;

        //    if (time.Substring (6, 2) == "PM") {
        //        hours = 12;
        //    }

        //    hours += int.Parse (time.Substring (0, 2));
        //    minutes = int.Parse (time.Substring (3, 2));

        //    result = hours * 60 + minutes;

        //    result = WBidCollection.DomicileTimeFromHerb (GlobalSettings.CurrentBidDetails.Domicile, date, result);

        //    hours = result / 60;
        //    minutes = result % 60;

        //    if (hours == 24) {
        //        hours = 0;
        //        strTime = "00";
        //    } else {
        //        strTime = (hours > 12) ? (hours - 12).ToString ("d2") : hours.ToString ("d2");
        //    }
        //    strTime += ":" + minutes.ToString ("d2");

        //    strTime += ((hours >= 12) ? " PM" : " AM");

        //    return strTime;
        //}

        private string ConvertTimeToDomicile(string time, DateTime date, out bool isDayBefore, out string modifiedtime)
        {
            modifiedtime = "0";
            int hours = 0;
            int minutes = 0;
            int result = 0;
            string strTime = string.Empty;

            if (time.Substring(6, 2) == "PM" && int.Parse(time.Substring(0, 2)) != 12)
            {
                hours = 12;
            }

            hours += int.Parse(time.Substring(0, 2));
            minutes = int.Parse(time.Substring(3, 2));

            result = hours * 60 + minutes;

            result = WBidCollection.DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, date, result);
            if (result < 0)
            {
                isDayBefore = true;
                modifiedtime = FormatTime(1440 + result);

            }
            else
                isDayBefore = false;
            hours = result / 60;
            minutes = result % 60;

            if (hours == 24)
            {
                hours = 0;
                strTime = "00";
            }
            else
            {
                strTime = (hours > 12) ? (hours - 12).ToString("d2") : hours.ToString("d2");
            }
            strTime += ":" + minutes.ToString("d2");

            strTime += ((hours >= 12) ? " PM" : " AM");

            return strTime;
        }
        private string FormatTime(int time)
        {


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
            if (hour >= 24)
            {
                if (minutes > 0)
                {
                    //  day = 1;
                }

                hour = hour - 24;
                stringTime = hour.ToString().PadLeft(2, '0');
            }
            else
            {
                stringTime = (hour > 12) ? (hour - 12).ToString("d2") : hour.ToString("d2");
            }
            stringTime += ":" + minutes.ToString("d2");

            stringTime += ((hour >= 12) ? " PM" : " AM");

            return stringTime;

        }
        private ExportCalendar ExportTripDetails(Trip trip, DateTime tripStartDate, string tripName, int lineNum)
        {
            ExportCalendar exportCalendar = new ExportCalendar();

            string startTime = CalculateStartTimeBasedOnTime(trip, tripStartDate);
            exportCalendar.StarDdate = DateTime.Parse(tripStartDate.ToShortDateString() + " " + startTime);


            //Generate Subject
            //-----------------------------------------
            string subject = string.Empty;
            foreach (DutyPeriod dutyPeriod in trip.DutyPeriods)
            {

                if (subject != string.Empty)
                {
                    subject += "/";
                }
                subject += dutyPeriod.ArrStaLastLeg;
            }

            if (GlobalSettings.WBidINIContent.PairingExport.IsSubjectLineSelected)
            {
                subject = trip.TripNum.Substring(0, 4) + " " + subject;
            }

            //-----------------------------------------

            exportCalendar.Title = subject;
            int day = 0;

            exportCalendar.EndDate = DateTime.Parse(tripStartDate.AddDays(trip.DutyPeriods.Count - 1).ToShortDateString() + " " + CalculateEndTimeBasedOnTime(trip, tripStartDate, out day));

            exportCalendar.EndDate = exportCalendar.EndDate.AddDays(day);
            CorrectionParams correctionParams = new Model.CorrectionParams();
            correctionParams.selectedLineNum = lineNum;
            var tripDetails = TripViewBL.GenerateTripDetailsFiltered(tripName, correctionParams);

            string body = string.Empty;
            foreach (var item in tripDetails)
            {
                body += item.Content + Environment.NewLine;

            }

            exportCalendar.TripDetails = "REPORT " + CalculateStartTime(trip) + " CST/CDT";
            body += Environment.NewLine + Environment.NewLine + "(Note: All times are CST/CDT unless otherwise noted.)";
            if (!GlobalSettings.WBidINIContent.PairingExport.IsCentralTime)
            {
                exportCalendar.TripDetails += " (" + startTime + " Domicile Time)";
                body += Environment.NewLine + "(Note: AppointmentDetails times  are in Domicile time).";
            }
            exportCalendar.TripDetails += Environment.NewLine + Environment.NewLine + body;




            return exportCalendar;

        }

        private string GetTime(string time, out int day)
        {
            day = 0;
            int hour = 0;
            int minute = 0;
            string result = time;
            hour = int.Parse(result.Substring(0, 2));
            minute = int.Parse(result.Substring(3, 2));
            if (hour >= 24)
            {
                hour = hour - 24;
                day = 1;
            }

            int hourAmpm = hour;
            hourAmpm = (hourAmpm > 12) ? hourAmpm - 12 : hourAmpm;
            result = hourAmpm.ToString().PadLeft(2, '0') + ":" + minute.ToString().PadLeft(2, '0');
            result += ((hour >= 12) ? " PM" : " AM");
            return result;
        }

        private string CalculateStartTime(Trip trip)
        {

            string startTime = string.Empty;
            int hour = 0;
            int minutes = 0;
            int startTimeMinute = 0;
            //int depTimeMinutes =  int.Parse(trip.DepTime);
            int depTimeMinutes = ConvertHhmmToMinutes(trip.DepTime);

            //Int16.Parse(trip.DepTime.Substring(0, 2)) * 60 + Int16.Parse(trip.DepTime.Substring(2, 2));
            //  startTimeMinute = depTimeMinutes - trip.BriefTime;
            startTimeMinute = trip.DutyPeriods[0].ShowTime;
            hour = startTimeMinute / 60;
            minutes = startTimeMinute % 60;

            if (hour == 24)
            {
                hour = 0;
                startTime = "00";
            }
            else
            {
                startTime = (hour > 12) ? (hour - 12).ToString("d2") : hour.ToString("d2");
            }
            startTime += ":" + minutes.ToString("d2");

            startTime += ((hour >= 12) ? " PM" : " AM");

            return startTime;

        }

        private string CalculateStartTimeBasedOnTime(Trip trip, DateTime tripStartDate)
        {

            string startTime = string.Empty;
            int hour = 0;
            int minutes = 0;
            int startTimeMinute = 0;
            startTimeMinute = trip.DutyPeriods[0].ShowTime;
            if (!GlobalSettings.WBidINIContent.PairingExport.IsCentralTime)
            {
                startTimeMinute = WBidCollection.DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, tripStartDate, startTimeMinute);
            }

            hour = startTimeMinute / 60;
            minutes = startTimeMinute % 60;

            if (hour == 24)
            {
                hour = 0;
                startTime = "00";
            }
            else
            {
                startTime = (hour > 12) ? (hour - 12).ToString("d2") : hour.ToString("d2");
            }
            startTime += ":" + minutes.ToString("d2");

            startTime += ((hour >= 12) ? " PM" : " AM");
            return startTime;
        }

        private string CalculateEndTimeBasedOnTime(Trip trip, DateTime tripStartDate, out int day)
        {

            string endtTime = string.Empty;
            int hour = 0;
            int minutes = 0;
            int endTimeMinute = 0;
            day = 0;

            //int repTimeMinutes = int.Parse( trip.RetTime)%1440;
            int repTimeMinutes = ConvertHhmmToMinutes(trip.RetTime);
            //Int16.Parse(trip.RetTime.Substring(0, 2)) * 60 + Int16.Parse(trip.RetTime.Substring(2, 2));
            endTimeMinute = repTimeMinutes + trip.DebriefTime;
            if (!GlobalSettings.WBidINIContent.PairingExport.IsCentralTime)
            {
                endTimeMinute = WBidCollection.DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, tripStartDate, endTimeMinute);
            }

            if (endTimeMinute >= 1440)
            {
                day = 1;
                endTimeMinute = endTimeMinute % 1440;
            }


            if (endTimeMinute == 0)
            {
                hour = 0;
                minutes = 0;

            }
            else
            {

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

            endtTime = (hour > 12) ? (hour - 12).ToString("d2") : hour.ToString("d2");

            endtTime += ":" + minutes.ToString("d2");

            endtTime += ((hour >= 12) ? " PM" : " AM");

            return endtTime;

        }

        private int ConvertHhmmToMinutes(string hhmm)
        {
            hhmm = hhmm.PadLeft(4, '0');
            int hours = Convert.ToInt32(hhmm.Substring(0, 2));
            int minutes = Convert.ToInt32(hhmm.Substring(2, 2));
            return hours * 60 + minutes;
        }

        public void calPopover(NSIndexPath path)
        {
            //            if (calCollection != null)
            //            {
            //                calCollection.View.RemoveFromSuperview();
            //                calCollection = null;
            //            }
            CommonClass.selectedTrip = null;

            CommonClass.selectedLine = GlobalSettings.Lines[path.Row].LineNum;
            CommonClass.calData = CalendarViewBL.GenerateCalendarDetails(GlobalSettings.Lines[path.Row]);

            //this.sumList.TableView.BringSubviewToFront(this.vwCalPopover);
            //below line is commented by Roshil to solve the trip view showing behing calender view. 2022-Feb-23
           // this.View.BringSubviewToFront(this.vwCalPopover);
            this.vwCalPopover.Hidden = true;
            this.vwCalPopover.Layer.BorderWidth = 1;
            this.vwCalPopover.Layer.BorderColor = UIColor.FromRGB(158, 179, 131).CGColor;
            this.vwCalPopover.Layer.CornerRadius = 3.0f;
            this.vwCalPopover.Layer.ShadowColor = UIColor.Black.CGColor;
            this.vwCalPopover.Layer.ShadowOpacity = 0.5f;
            this.vwCalPopover.Layer.ShadowRadius = 2.0f;
            this.vwCalPopover.Layer.ShadowOffset = new CGSize(3f, 3f);


            if (calCollection == null)
            {
                var layout = new UICollectionViewFlowLayout();
                layout.SectionInset = new UIEdgeInsets(0, 0, 0, 0);
                layout.MinimumInteritemSpacing = 0;
                layout.MinimumLineSpacing = 0;
                layout.ItemSize = new CGSize(50, 65);
                calCollection = new CalenderPopoverController(layout);
                this.AddChildViewController(calCollection);
                calCollection.View.Frame = this.vwCalChild.Bounds;
                this.vwCalChild.AddSubview(calCollection.View);
            }
            else
            {
                calCollection.CollectionView.ReloadData();
            }

            Line CalLine = GlobalSettings.Lines[path.Row];
            string title = "Line " + CalLine.LineNum;
            if (GlobalSettings.CurrentBidDetails.Postion == "FA")
            {
                if (CalLine.FAPositions.Count > 0)
                {
                    title += "  - " + string.Join("", CalLine.FAPositions);
                }
            }
            this.lblCalTitle.Text = title;

            if (path.Row == 0)
            {
                this.btnMovUp.Enabled = false;
                this.btnMovDown.Enabled = true;
            }
            else if (path.Row == this.sumList.TableView.NumberOfRowsInSection(0) - 1)
            {
                this.btnMovUp.Enabled = true;
                this.btnMovDown.Enabled = false;
            }
            else
            {
                this.btnMovUp.Enabled = true;
                this.btnMovDown.Enabled = true;
            }

            if (GlobalSettings.Lines[path.Row].TopLock)
                this.btnLineTopLock.Enabled = false;
            else
                this.btnLineTopLock.Enabled = true;
            if (GlobalSettings.Lines[path.Row].BotLock)
                this.btnLineBotLock.Enabled = false;
            else
                this.btnLineBotLock.Enabled = true;

            CommonClass.doubleTapLine.Clear();
            CommonClass.doubleTapLine.Add(GlobalSettings.Lines[path.Row].LineNum);

            this.vwCalPopover.Hidden = false;
            this.vwTripPopover.Hidden = true;

        }

        private void tripPopover()
        {
            //            if (tripList != null)
            //            {
            //                tripList.View.RemoveFromSuperview();
            //                tripList = null;
            //            }

            this.sumList.View.BringSubviewToFront(this.vwTripPopover);
            this.vwTripPopover.Hidden = true;
            this.vwTripPopover.Layer.BorderWidth = 1;
            this.vwTripPopover.Layer.BorderColor = UIColor.FromRGB(158, 179, 131).CGColor;
            this.vwTripPopover.Layer.CornerRadius = 3.0f;
            this.vwTripPopover.Layer.ShadowColor = UIColor.Black.CGColor;
            this.vwTripPopover.Layer.ShadowOpacity = 0.5f;
            this.vwTripPopover.Layer.ShadowRadius = 2.0f;
            this.vwTripPopover.Layer.ShadowOffset = new CGSize(3f, 3f);


            if (tripList == null)
            {
                tripList = new TripPopListViewController();
                this.AddChildViewController(tripList);
                tripList.View.Frame = this.vwTripChild.Bounds;
                this.vwTripChild.AddSubview(tripList.View);
            }
            else
            {
                tripList.TableView.ReloadData();
            }

            if (CommonClass.tripData.Count <= 21)
            {
                int tripCount = CommonClass.tripData.Count;
                CGRect frame = this.vwTripPopover.Frame;
                this.vwTripPopover.Frame = new CGRect(frame.X, frame.Y, frame.Width, tripCount * 25);
            }
            else
            {
                CGRect frame = this.vwTripPopover.Frame;
                this.vwTripPopover.Frame = new CGRect(frame.X, frame.Y, frame.Width, 530);
            }
            this.vwTripPopover.Hidden = false;

        }


        public void observeNotification()
        {
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("Dismisstextfield"), handleTextField));
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("DataReload"), handleDataReload));
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ReloadModernView"), ReloadModernView));
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("DataCulumnsUpdated"), handleDataCulumnsUpdate));
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ButtonEnableDisable"), handleButtonEnable));
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("SumRowSelected"), handleRowSelection));
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ShowPopover"), handlePopoverShow));
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("HidePopover"), handlePopoverHide));
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("CalPopover"), handleCalPopover));
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ColumnPopover"), handleColumnPopover));
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("SummaryColumnSort"), handleColumnSort));
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("CalPopHide"), handleCalPopHide));
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("TripPopShow"), handleTripPopShow));
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("TripPopHide"), handleTripPopHide));
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ResetButtonStates"), handleButtonStates));

            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ShowGroupBidAutomator"), ShowGroupBidAutomator));
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ShowReparseView"), (NSNotification n) =>
            {
                vwReparse.Hidden = false;
            }));
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("AutoSaveTimer"), (NSNotification n) =>
            {
                if (GlobalSettings.WBidINIContent.User.AutoSave)
                {
                    if (timer != null)
                        timer.Stop();
                    AutoSave();
                }
                else
                {
                    if (timer != null)
                        timer.Stop();
                }
            }));

            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ShowRatioScreen"), ShowRatioScreen));
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ClosingRatioScreen"), ClosingRatioScreen));

            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("handleColumnPopoverModernView"), handleColumnPopoverModernView));
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("OpenMonthToMonthAlert"), OpenMonthToMonthAlert));
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("SetApplicationLoadData"), SetApplicationLoadData));
        }



        void ClosingRatioScreen(NSNotification obj)
        {
            var type = obj.Object.ToString();
            if (type == "OK")
            {
                ShowRatioColumn();
            }
            //else
            //{
            if (sgControlViewType.SelectedSegment == 0)
            {

            }
            else if (sgControlViewType.SelectedSegment == 1)
            {



                if (CommonClass.bidLineProperties.Count < 5)
                {
                    this.PerformSelector(new ObjCRuntime.Selector("ViewBidLinePopOverShow"), null, 0.5);
                    UIAlertController okAlertController = UIAlertController.Create("Additional Columns", "Please select \"5\" columns", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                }
            }
            if (sgControlViewType.SelectedSegment == 2)
            {

                if (CommonClass.modernProperties.Count < 5)
                {
                    this.PerformSelector(new ObjCRuntime.Selector("ViewModernPopOverShow"), null, 0.5);
                    UIAlertController okAlertController = UIAlertController.Create("Additional Columns", "Please select \"5\" columns", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                }

            }
            //}
        }


        [Export("ViewBidLinePopOverShow")]
        void ViewBidLinePopOverShow()
        {
            NSNotificationCenter.DefaultCenter.PostNotificationName("ShowPopOverView", null);
        }


        [Export("ViewModernPopOverShow")]
        void ViewModernPopOverShow()
        {

            if (CommonClass.IsModernScrollClassic == "FALSE")
            {
                NSNotificationCenter.DefaultCenter.PostNotificationName("ShowPopOverViewModernShadow", null);
            }
            else
            {
                NSNotificationCenter.DefaultCenter.PostNotificationName("ShowPopOverViewModernClassic", null);
            }


        }
        private void ShowRatioColumn()
        {
            try
            {
                if (sgControlViewType.SelectedSegment == 0)
                {
                    //GlobalSettings.AdditionalvacationColumns.FirstOrDefault(x=>x.DataPropertyName=="Ratio").IsSelected=true;


                    //GlobalSettings.WBidINIContent.DataColumns.Add(new DataColumn()

                    //{
                    //    Id = GlobalSettings.AdditionalvacationColumns.FirstOrDefault(x => x.DataPropertyName == "Ratio").Id,
                    //                Width = 50,
                    //                Order = GlobalSettings.WBidINIContent.DataColumns.Count
                    //            });
                    //            //tappedColumn.IsSelected = true;
                    //            XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());//Save and serialize.
                    //            GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
                    //            NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);
                    //NSNotificationCenter.DefaultCenter.PostNotificationName ("ReloadPopover", null);

                    AddSummaryViewColumns();
                }
                else if (sgControlViewType.SelectedSegment == 1)
                {
                    AddBidlineViewColumns();
                }
                else if (sgControlViewType.SelectedSegment == 2)
                {
                    AddModernBidlineviewcolumns();
                }
            }
            catch (Exception ex)
            {

            }
        }



        void AddSummaryViewColumns()
        {
            if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
            {
                AdditionalColumns tappedColumn = GlobalSettings.AdditionalvacationColumns.FirstOrDefault(x => x.DataPropertyName == "Ratio");
                if (!tappedColumn.IsRequied)
                {
                    if (GlobalSettings.WBidINIContent.SummaryVacationColumns.Count < 18)
                    {
                        if (GlobalSettings.WBidINIContent.SummaryVacationColumns.Count(x => x.Id == tappedColumn.Id) == 0)
                        {
                            GlobalSettings.WBidINIContent.SummaryVacationColumns.Add(new DataColumn()
                            {
                                Id = tappedColumn.Id,
                                Width = 50,
                                Order = GlobalSettings.WBidINIContent.SummaryVacationColumns.Count
                            });
                            tappedColumn.IsSelected = true;
                            XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());//Save and serialize.
                            GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
                            NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);
                            //this.PerformSelector (new MonoTouch.ObjCRuntime.Selector ("reloadTable"), tableView, 0.2);
                        }
                        else
                        {
                            GlobalSettings.WBidINIContent.SummaryVacationColumns.RemoveAll(x => x.Id == tappedColumn.Id);
                            tappedColumn.IsSelected = false;
                            XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());//Save and serialize.
                            GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
                            NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);
                            //this.PerformSelector (new MonoTouch.ObjCRuntime.Selector ("reloadTable"), tableView, 0.2);
                        }
                    }
                    else
                    {
                        if (GlobalSettings.WBidINIContent.SummaryVacationColumns.Count(x => x.Id == tappedColumn.Id) == 1)
                        {
                            GlobalSettings.WBidINIContent.SummaryVacationColumns.RemoveAll(x => x.Id == tappedColumn.Id);
                            tappedColumn.IsSelected = false;
                            XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());//Save and serialize.
                            GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
                            NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);
                            //this.PerformSelector (new MonoTouch.ObjCRuntime.Selector ("reloadTable"), tableView, 0.2);
                        }
                        else
                        {
                            UIAlertController okAlertController = UIAlertController.Create("Additional Columns", "Can not add more columns", UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                        }
                    }
                }


            }
            else
            {
                AdditionalColumns tappedColumn = GlobalSettings.AdditionalColumns.FirstOrDefault(x => x.DataPropertyName == "Ratio");
                if (!tappedColumn.IsRequied)
                {
                    if (GlobalSettings.WBidINIContent.DataColumns.Count < 18)
                    {
                        if (GlobalSettings.WBidINIContent.DataColumns.Count(x => x.Id == tappedColumn.Id) == 0)
                        {
                            GlobalSettings.WBidINIContent.DataColumns.Add(new DataColumn()
                            {
                                Id = tappedColumn.Id,
                                Width = 50,
                                Order = GlobalSettings.WBidINIContent.DataColumns.Count
                            });
                            tappedColumn.IsSelected = true;
                            XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());//Save and serialize.
                            GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
                            NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);
                            //this.PerformSelector (new MonoTouch.ObjCRuntime.Selector ("reloadTable"), tableView, 0.2);
                        }
                        else
                        {
                            GlobalSettings.WBidINIContent.DataColumns.RemoveAll(x => x.Id == tappedColumn.Id);
                            tappedColumn.IsSelected = false;
                            XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());//Save and serialize.
                            GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
                            NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);
                            //this.PerformSelector (new MonoTouch.ObjCRuntime.Selector ("reloadTable"), tableView, 0.2);
                        }
                    }
                    else
                    {
                        if (GlobalSettings.WBidINIContent.DataColumns.Count(x => x.Id == tappedColumn.Id) == 1)
                        {
                            GlobalSettings.WBidINIContent.DataColumns.RemoveAll(x => x.Id == tappedColumn.Id);
                            tappedColumn.IsSelected = false;
                            XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());//Save and serialize.
                            GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
                            NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);

                        }
                        else
                        {
                            UIAlertController okAlertController = UIAlertController.Create("Additional Columns", "Can not add more columns", UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                        }
                    }
                }
            }
        }
        static void AddModernBidlineviewcolumns()
        {
            AdditionalColumns tappedColumn = null;
            if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
            {
                tappedColumn = GlobalSettings.ModernAdditionalvacationColumns.FirstOrDefault(x => x.DataPropertyName == "Ratio");
                tappedColumn.IsSelected = !tappedColumn.IsSelected;
                GlobalSettings.ModernAdditionalvacationColumns = GlobalSettings.ModernAdditionalvacationColumns.OrderByDescending(x => x.IsSelected == true).ThenBy(y => y.DisplayName).ToList();
            }
            else
            {
                tappedColumn = GlobalSettings.ModernAdditionalColumns.FirstOrDefault(x => x.DataPropertyName == "Ratio");
                tappedColumn.IsSelected = !tappedColumn.IsSelected;
                GlobalSettings.ModernAdditionalColumns = GlobalSettings.ModernAdditionalColumns.OrderByDescending(x => x.IsSelected == true).ThenBy(y => y.DisplayName).ToList();
            }

            if (tappedColumn.IsSelected && CommonClass.modernProperties.Count >= 5)
            {
                tappedColumn.IsSelected = false;

                UIWindow WindowAlert = new UIWindow(UIScreen.MainScreen.Bounds);
                WindowAlert.RootViewController = new UIViewController();
                UIAlertController okAlertController = UIAlertController.Create("Additional Columns", "Cannot add more than 5 columns", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {
                    WindowAlert.Dispose();
                }));
                WindowAlert.MakeKeyAndVisible();
                WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
               // WindowAlert.Dispose();

            }
            else
            {
                if (CommonClass.modernProperties.Contains(tappedColumn.DisplayName))
                {
                    CommonClass.modernProperties.Remove(tappedColumn.DisplayName);
                    if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
                    {
                        GlobalSettings.WBidINIContent.ModernVacationColumns.Remove(tappedColumn.Id);

                    }
                    else
                    {
                        GlobalSettings.WBidINIContent.ModernNormalColumns.Remove(tappedColumn.Id);
                    }
                    //tappedColumn.IsSelected = false;
                }
                else
                {
                    CommonClass.modernProperties.Add(tappedColumn.DisplayName);
                    // tappedColumn.IsSelected = true;
                    if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
                    {
                        GlobalSettings.WBidINIContent.ModernVacationColumns.Add(tappedColumn.Id);

                    }
                    else
                    {
                        GlobalSettings.WBidINIContent.ModernNormalColumns.Add(tappedColumn.Id);
                    }

                }
                NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);
            }

            //commented by Roshil on 26-8-2021 to allow the user to set less than 5 properties
            //if (CommonClass.modernProperties.Count == 5)
            //{
            //    NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
            //}
            NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
        }

         void AddBidlineViewColumns()
        {
            AdditionalColumns tappedColumn = null;
            if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
            {
                tappedColumn = GlobalSettings.BidlineAdditionalvacationColumns.FirstOrDefault(x => x.DataPropertyName == "Ratio"); ;
                tappedColumn.IsSelected = !tappedColumn.IsSelected;
                GlobalSettings.BidlineAdditionalvacationColumns = GlobalSettings.BidlineAdditionalvacationColumns.OrderByDescending(x => x.IsSelected == true).ThenBy(y => y.DisplayName).ToList();
            }
            else
            {
                tappedColumn = GlobalSettings.BidlineAdditionalColumns.FirstOrDefault(x => x.DataPropertyName == "Ratio"); ;
                tappedColumn.IsSelected = !tappedColumn.IsSelected;
                GlobalSettings.BidlineAdditionalColumns = GlobalSettings.BidlineAdditionalColumns.OrderByDescending(x => x.IsSelected == true).ThenBy(y => y.DisplayName).ToList();
            }


            if (tappedColumn.IsSelected && CommonClass.bidLineProperties.Count >= 5)
            {
                tappedColumn.IsSelected = false;


                //InvokeOnMainThread(() =>
                //{
                //    UIAlertController okAlertController = UIAlertController.Create("WBidMax", status, UIAlertControllerStyle.Alert);
                //    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                //    WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);

                //});

                InvokeOnMainThread(() =>
                {
                    UIWindow WindowAlert = new UIWindow(UIScreen.MainScreen.Bounds);
                    WindowAlert.RootViewController = new UIViewController();
                    UIAlertController okAlertController = UIAlertController.Create("Additional Columns", "Cannot add more than 5 columns", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {
                        WindowAlert.Dispose();
                    }));
                    WindowAlert.MakeKeyAndVisible();
                    WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
                    WindowAlert.Dispose();
                });


            }
            else
            {
                if (CommonClass.bidLineProperties.Contains(tappedColumn.DisplayName))
                {
                    CommonClass.bidLineProperties.Remove(tappedColumn.DisplayName);
                    if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
                    {
                        GlobalSettings.WBidINIContent.BidLineVacationColumns.Remove(tappedColumn.Id);

                    }
                    else
                    {
                        GlobalSettings.WBidINIContent.BidLineNormalColumns.Remove(tappedColumn.Id);
                    }

                    //tappedColumn.IsSelected = false;
                }
                else
                {
                    CommonClass.bidLineProperties.Add(tappedColumn.DisplayName);
                    // tappedColumn.IsSelected = true;
                    if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
                    {
                        GlobalSettings.WBidINIContent.BidLineVacationColumns.Add(tappedColumn.Id);

                    }
                    else
                    {
                        GlobalSettings.WBidINIContent.BidLineNormalColumns.Add(tappedColumn.Id);
                    }

                }
                NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);
            }



           // if (CommonClass.bidLineProperties.Count == 5)
            //{
                NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
           // }
        }
        //private void CalculateRatioValuesForAllLines()
        //{

        //    foreach (var line in GlobalSettings.Lines)
        //    {
        //        decimal numeratorValue = Convert.ToDecimal(line.GetType().GetProperty(SelectedNumerator.DataPropertyName).GetValue(line, null));
        //        decimal denominatorValue = Convert.ToDecimal(line.GetType().GetProperty(SelectedDenominator.DataPropertyName).GetValue(line, null));
        //        line.Ratio = Math.Round(decimal.Parse(String.Format("{0:0.00}", (denominatorValue == 0) ? 0 : numeratorValue / denominatorValue)), 2);

        //    }

        //}
        void handleButtonStates(NSNotification obj)
        {
            btnVacCorrect.Selected = false;
            btnVacDrop.Selected = false;
            btnVacDrop.Enabled = false;
            btnOverlap.Selected = false;
            btnEOM.Selected = false;


            SetVacButtonStates();
        }

        void ShowRatioScreen(NSNotification obj)
        {

            //error here ..object popoverController is null when using modern view additional property.

            if (sgControlViewType.SelectedSegment == 0)
                popoverController.Dismiss(true);
            else if (sgControlViewType.SelectedSegment == 1)
                NSNotificationCenter.DefaultCenter.PostNotificationName("dismissClassicPopover", null);

            else if (sgControlViewType.SelectedSegment == 2)
            {

                if (CommonClass.IsModernScrollClassic == "FALSE")
                {
                    NSNotificationCenter.DefaultCenter.PostNotificationName("dismissModernPopover", null);
                }
                else
                {
                    NSNotificationCenter.DefaultCenter.PostNotificationName("dismissModernClassicPopover", null);

                }
            }

            if (!IsRatioPropertiesSetFromOtherViews())
            {

                this.PerformSelector(new ObjCRuntime.Selector("ViewShow"), null, 0.5);
            }
            else
            {
                SetRatioValues();
                ShowRatioColumn();
            }



        }
        private void SetRatioValues()
        {
            var numeratorcolumn = GlobalSettings.columndefinition.FirstOrDefault(X => X.Id == GlobalSettings.WBidINIContent.RatioValues.Denominator);
            var denominatorcolumn = GlobalSettings.columndefinition.FirstOrDefault(X => X.Id == GlobalSettings.WBidINIContent.RatioValues.Numerator);
            if (numeratorcolumn != null && denominatorcolumn != null)
            {
                foreach (var line in GlobalSettings.Lines)
                {
                    var numerator = line.GetType().GetProperty(numeratorcolumn.DataPropertyName).GetValue(line, null);
                    if (numeratorcolumn.DataPropertyName == "TafbInBp")
                        numerator = Helper.ConvertHhhColonMmToFractionalHours(line.TafbInBp);
                    decimal numeratorValue = Convert.ToDecimal(numerator);

                    var denominator = line.GetType().GetProperty(denominatorcolumn.DataPropertyName).GetValue(line, null);
                    if (denominatorcolumn.DataPropertyName == "TafbInBp")
                        denominator = Helper.ConvertHhhColonMmToFractionalHours(line.TafbInBp);
                    decimal denominatorValue = Convert.ToDecimal(denominator);

                    //decimal numeratorValue = Convert.ToDecimal(line.GetType().GetProperty(numerator.DataPropertyName).GetValue(line, null));
                    //decimal denominatorValue = Convert.ToDecimal(line.GetType().GetProperty(denominator.DataPropertyName).GetValue(line, null));
                    line.Ratio = Math.Round(((denominatorValue == 0) ? 0 : numeratorValue / denominatorValue), 2);

                }
            }
        }
        private bool IsRatioPropertiesSetFromOtherViews()
        {
            bool isNormalMode = !(GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM);

            if (isNormalMode)
            {
                if (GlobalSettings.WBidINIContent.BidLineNormalColumns.Any(x => x == 75) ||
                GlobalSettings.WBidINIContent.ModernNormalColumns.Any(x => x == 75) ||
                 GlobalSettings.WBidINIContent.DataColumns.Any(x => x.Id == 75) ||
                    wBIdStateContent.SortDetails.BlokSort.Contains("19")
                //wbidSta
                )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (
                GlobalSettings.WBidINIContent.BidLineVacationColumns.Any(x => x == 75) ||
                GlobalSettings.WBidINIContent.ModernVacationColumns.Any(x => x == 75) ||
                 GlobalSettings.WBidINIContent.SummaryVacationColumns.Any(x => x.Id == 75) ||
                    wBIdStateContent.SortDetails.BlokSort.Contains("19")

                )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        [Export("ViewShow")]
        void ViewShow()
        {
            Console.WriteLine("Success");
            UIStoryboard storyboard = UIStoryboard.FromName("Main", null);
            RatioViewController ratioView = storyboard.InstantiateViewController("RatioViewController") as RatioViewController;
            ratioView.isFromLineViewController = true;
            ratioView.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;

            //this.PresentViewController(cmtView, true,null);
            if (this.NavigationController == null)
            {
            }

            this.NavigationController.PresentViewController(ratioView, true, null);
        }

        public void handleTripPopShow(NSNotification n)
        {
            tripNum = n.Object.ToString();
            CorrectionParams correctionParams = new Model.CorrectionParams();
            correctionParams.selectedLineNum = CommonClass.selectedLine;
            CommonClass.tripData = TripViewBL.GenerateTripDetails(tripNum, correctionParams, CommonClass.isLastTrip);
            this.lblTripPopTitle.Text = "Pairing " + tripNum.Substring(0, 4);


            if (GlobalSettings.CurrentBidDetails.Postion == "FA")
            {


                var line = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == correctionParams.selectedLineNum);
                //lines.FirstOrDefault(y => y.Pairings.Any(x => x == tripname));
                if (line != null)
                {
                    if (GlobalSettings.CurrentBidDetails.Round == "M")
                    {
                        this.lblTripPopTitle.Text += "  - " + string.Join("", line.FAPositions);// + " Positions";
                    }
                    else
                    {
                        //this.lblTripPopTitle.Text += "  - " + line.FASecondRoundPositions.FirstOrDefault (x => x.Value.Contains (tripNum.Substring (0, 4))).Value;// + " Position";
                        var fapos=line.FASecondRoundPositions.FirstOrDefault(x => x.key.Contains(tripNum.Substring(0, 4)));
                        if (fapos != null)
                        {
                            this.lblTripPopTitle.Text += "  - " + fapos.Value;// + " Position";
                        }
                    }

                }


            }

            this.tripPopover();
        }

        public void handleTripPopHide(NSNotification n)
        {
            this.vwTripPopover.Hidden = true;
        }

        public void handleCalPopHide(NSNotification n)
        {
            this.vwCalPopover.Hidden = true;
            this.vwTripPopover.Hidden = true;
            if (sumList != null)
                this.sumList.TableView.DeselectRow(lPath, true);
        }

        partial void btnTripCloseTapped(UIKit.UIButton sender)
        {
            this.vwTripPopover.Hidden = true;
            CommonClass.selectedTrip = null;
            NSNotificationCenter.DefaultCenter.PostNotificationName("ReloadCal", null);
            if (bidLineList != null)
                bidLineList.TableView.ReloadData();
            if (modernList != null)
                modernList.TableView.ReloadData();
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


        public void handleColumnSort(NSNotification n)
        {
            if (n == null || n.Object == null)
                return;
            if (sgControlViewType.SelectedSegment != 0)
                return;

            var cell = (UITableViewCell)n.Object;
            if (cell != null && hTable.TableView != null)
                hPath = hTable.TableView.IndexPathForCell(cell);

            if (hPath != null && hPath.Row > 3)
            {
                DataColumn column;
                if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
                    column = GlobalSettings.WBidINIContent.SummaryVacationColumns[hPath.Row];
                else
                    column = GlobalSettings.WBidINIContent.DataColumns[hPath.Row];
                ColumnDefinition definition = GlobalSettings.columndefinition.FirstOrDefault(x => x.Id == column.Id);

                if (column == null || definition == null)
                    return;

                Console.WriteLine(definition.DisplayName);

                if (column.Id > 4)
                {
                    WBidHelper.PushToUndoStack();

                    CommonClass.columnID = column.Id;
                    bool status = false;
                    if (definition.DisplayName == "StartDOW")
                    {
                        status = true;
                    }
                    else if (cell.Tag == 1)
                    {
                        status = false;
                    }
                    else if (cell.Tag == 2)
                    {
                        status = true;
                    }
                    LineOperations.SortColumns(definition.DataPropertyName, status);
                    CommonClass.columnAscend = status;
                    NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);

                    GlobalSettings.isModified = true;
                    CommonClass.lineVC.UpdateSaveButton();
                }




            }
        }

        public void handleDataReload(NSNotification n)
        {
            ReloadLineView();

            if (hTable != null && sgControlViewType.SelectedSegment == 0)
                hTable.TableView.ReloadData();
            if (sumList != null && sgControlViewType.SelectedSegment == 0)
                sumList.TableView.ReloadData();
            if (bidLineList != null && sgControlViewType.SelectedSegment == 1)
                bidLineList.TableView.ReloadData();
            if (modernList != null && sgControlViewType.SelectedSegment == 2)
            {
                ReloadModernViewOverlay();
                modernList.TableView.ReloadData();
            }

            if (GlobalSettings.Lines.Count(x => x.TopLock == true) > 0)
            {
                this.btnRemTopLock.Enabled = true;
            }
            else
            {
                this.btnRemTopLock.Enabled = false;
            }

            if (GlobalSettings.Lines.Count(x => x.BotLock == true) > 0)
            {
                this.btnRemBottomLock.Enabled = true;
            }
            else
            {
                this.btnRemBottomLock.Enabled = false;
            }

            HandleBlueShadowButton();
        }
        public void handleTextField(NSNotification n)
        {
            txtGoToLine.ResignFirstResponder();
        }

        public void handleDataCulumnsUpdate(NSNotification n)
        {
            //            PointF tableOffset = PointF.Empty;
            //            if (sumList != null)
            //                tableOffset = sumList.TableView.ContentOffset;
            //            if (bidLineList != null)
            //                tableOffset = bidLineList.TableView.ContentOffset;


            loadSummaryListAndHeader();

            //            sumList.TableView.ContentOffset = tableOffset;
            //            bidLineList.TableView.ContentOffset = tableOffset;

        }

        public void handlePopoverShow(NSNotification n)
        {
            if (popoverController == null)
            {
                lPath = (NSIndexPath)n.Object;
                PopoverViewController popoverContent = new PopoverViewController();
                popoverContent.PopType = "sumOpt";
                popoverController = new UIPopoverController(popoverContent);
                popoverController.Delegate = new MyPopDelegate(this);
                popoverController.PopoverContentSize = new CGSize(280, 250);
                //if (sumList != null)
                if (sgControlViewType.SelectedSegment == 0 && sumList != null)
                {
                    this.sumList.TableView.SelectRow(lPath, false, UITableViewScrollPosition.None);
                    if (this.View.Window != null)
                        popoverController.PresentFromRect(sumList.TableView.RectForRowAtIndexPath(lPath), sumList.TableView, UIPopoverArrowDirection.Any, true);
                }
                //else if (bidLineList != null) 
                else if (sgControlViewType.SelectedSegment == 1 && bidLineList != null)
                {
                    this.bidLineList.TableView.SelectRow(lPath, false, UITableViewScrollPosition.None);
                    if (this.View.Window != null)
                        popoverController.PresentFromRect(bidLineList.TableView.RectForRowAtIndexPath(lPath), bidLineList.TableView, UIPopoverArrowDirection.Any, true);
                }
                //else if (modernList != null)
                else if (sgControlViewType.SelectedSegment == 2 && modernList != null)
                {
                    this.modernList.TableView.SelectRow(lPath, false, UITableViewScrollPosition.None);
                    if (this.View.Window != null)
                        popoverController.PresentFromRect(modernList.TableView.RectForRowAtIndexPath(lPath), modernList.TableView, UIPopoverArrowDirection.Any, true);
                }
                this.vwCalPopover.Hidden = true;
                this.vwTripPopover.Hidden = true;
            }
        }

        public void handlePopoverHide(NSNotification n)
        {
            if (popoverController != null)
            {
                popoverController.Dismiss(true);
                popoverController = null;
            }
        }

        private void handleCalPopover(NSNotification n)
        {
            UITableViewCell cell = (UITableViewCell)n.Object;
            lPath = sumList.TableView.IndexPathForCell(cell);
            if (cell != null && lPath != null)
            {
                CommonClass.selectedLine = GlobalSettings.Lines[lPath.Row].LineNum;
                sumList.TableView.SelectRow(lPath, false, UITableViewScrollPosition.None);
                this.calPopover(lPath);
            }
        }

        public void handleColumnPopover(NSNotification n)
        {
            if (n == null || n.Object == null)
                return;
            UITableViewCell cell = (UITableViewCell)n.Object;
            if (cell != null && hTable.TableView != null)
                hPath = hTable.TableView.IndexPathForCell(cell);
            //hPath = hTable.TableView.IndexPathForCell (cell);
            if (hPath != null)
            {
                PopoverViewController popoverContent = new PopoverViewController();
                popoverContent.PopType = "sumColumn";
                popoverController = new UIPopoverController(popoverContent);
                popoverController.Delegate = new MyPopDelegate(this);
                popoverController.PopoverContentSize = new CGSize(250, 500);
                this.hTable.TableView.SelectRow(hPath, false, UITableViewScrollPosition.None);
                if (this.View.Window != null)
                    popoverController.PresentFromRect(hTable.TableView.RectForRowAtIndexPath(hPath), hTable.TableView, UIPopoverArrowDirection.Any, true);

            }
        }


        public void handleColumnPopoverModernView(NSNotification n)
        {
            if (n == null || n.Object == null)
                return;
            UIView cell = (UIView)n.Object;
            //if (cell != null && hTable.TableView != null)
            //    hPath = hTable.TableView.IndexPathForCell(cell);
            ////hPath = hTable.TableView.IndexPathForCell (cell);
            //if (hPath != null)
            //{
            PopoverViewController popoverContent = new PopoverViewController();
            popoverContent.PopType = "ModernColumns";
            popoverController = new UIPopoverController(popoverContent);
            popoverController.Delegate = new MyPopDelegate(this);
            popoverController.PopoverContentSize = new CGSize(250, 500);
            this.hTable.TableView.SelectRow(hPath, false, UITableViewScrollPosition.None);
            if (this.View.Window != null)
                popoverController.PresentFromRect(cell.Frame, cell, UIPopoverArrowDirection.Any, true);

            //}
        }




        public void handleRowSelection(NSNotification not)
        {
            NSNumber row = (NSNumber)not.Object;
            GlobalSettings.SelectedLine = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == (int)row);
            if (!CommonClass.selectedRows.Contains(row.Int32Value))
            {
                CommonClass.selectedRows.Add(row.Int32Value);
            }
            else
            {
                CommonClass.selectedRows.Remove(row.Int32Value);
            }
            if (CommonClass.selectedRows.Count != 0)
            {
                NSString str = new NSString("none");
                NSNotificationCenter.DefaultCenter.PostNotificationName("ButtonEnableDisable", str);
                foreach (int num in CommonClass.selectedRows)
                {
                    foreach (Line line in GlobalSettings.Lines)
                    {
                        if (num == line.LineNum)
                        {
                            if (line.TopLock == false)
                            {
                                NSString str1 = new NSString("TL");
                                NSNotificationCenter.DefaultCenter.PostNotificationName("ButtonEnableDisable", str1);
                            }
                            if (line.BotLock == false)
                            {
                                NSString str1 = new NSString("BL");
                                NSNotificationCenter.DefaultCenter.PostNotificationName("ButtonEnableDisable", str1);
                            }
                        }
                    }
                }
            }
            else
            {
                NSString str = new NSString("none");
                NSNotificationCenter.DefaultCenter.PostNotificationName("ButtonEnableDisable", str);
            }
        }

        private void loadSummaryListAndHeader()
        {
            if (hTable != null)
            {
                // DisposeClass.DisposeEx (hTable.View);
                //hTable.View.RemoveFromSuperview ();
                // hTable.RemoveFromParentViewController ();
                //  hTable = null;
            }

            if (GlobalSettings.WBidINIContent.ViewType == 1 || GlobalSettings.WBidINIContent.ViewType == 0)
            {

                NSNotificationCenter.DefaultCenter.PostNotificationName("ReloadTableview", null);


                vwSummaryContainer.Hidden = false;
                vwContainerView.Hidden = true;
                vwBidLineContainer.Hidden = true;
                hTable = new summaryHeaderListController();
                this.AddChildViewController(hTable);
                this.vwHeader.AddSubview(hTable.View);
                hTable.View.BackgroundColor = UIColor.FromRGB(207, 226, 183);

                this.vwHeader.Layer.BorderWidth = 1;
                this.lblSEL.Layer.BorderWidth = 1;
                this.lblMOV.Layer.BorderWidth = 1;
                this.vwHeader.Layer.BorderColor = UIColor.FromRGB(158, 179, 131).CGColor;
                this.lblSEL.Layer.BorderColor = UIColor.FromRGB(158, 179, 131).CGColor;
                this.lblMOV.Layer.BorderColor = UIColor.FromRGB(158, 179, 131).CGColor;
                sumList.reloadData();
                //scrlPath = sumList.TableView.IndexPathForRowAtPoint (sumList.TableView.Bounds.Location);
                if (scrlPath != null)
                {
                    sumList.TableView.ScrollToRow(scrlPath, UITableViewScrollPosition.Top, false);
                }
                //sumList.TableView.ReloadData();
            }
            else if (GlobalSettings.WBidINIContent.ViewType == 2)
            {

                vwBidLineContainer.Hidden = false;
                vwContainerView.Hidden = true;
                vwSummaryContainer.Hidden = true;
                //scrlPath = bidLineList.TableView.IndexPathForRowAtPoint(bidLineList.TableView.Bounds.Location);
                if (scrlPath != null)
                {
                    bidLineList.TableView.ScrollToRow(scrlPath, UITableViewScrollPosition.Top, false);
                }
                bidLineList.TableView.ReloadData();
            }
            else if (GlobalSettings.WBidINIContent.ViewType == 3)
            {
                //    modernList.ViewDidLoad();
                vwContainerView.Hidden = false;
                vwSummaryContainer.Hidden = true;
                vwBidLineContainer.Hidden = true;

                ReloadModernViewOverlay();


            }

        }

        public void handleButtonEnable(NSNotification not)
        {
            string str = not.Object.ToString();
            if (str == "TL")
            {
                this.btnPromote.Enabled = true;
            }
            else if (str == "BL")
            {
                this.btnTrash.Enabled = true;
            }
            else
            {
                this.btnPromote.Enabled = false;
                this.btnTrash.Enabled = false;
            }
        }

        partial void btnHomeTap(UIKit.UIButton sender)
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




            WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());

            StateManagement stateManagement = new StateManagement();
            stateManagement.UpdateWBidStateContent();
            //            WBidStateCollection stateFileContent = XmlHelper.DeserializeFromXml<WBidStateCollection>(WBidHelper.WBidStateFilePath);
            //            WBidState wBIdStateContent = stateFileContent.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            //            
            //            CompareState stateObj = new CompareState();
            //            bool isNochange = stateObj.CompareStateChange(wBIdStateContent, GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName));

            if (GlobalSettings.isModified)
            {
                GlobalSettings.WBidStateCollection.IsModified = true;
                UIActionSheet sheet;
                if (GlobalSettings.WBidINIContent.User.SmartSynch)
                {
                    sheet = new UIActionSheet("You want to save the changes before exit?", null, null, null, new string[] {
                        "Save & Synch",
                        "Save & Exit",
                        "Exit"
                    });
                    CGRect senderframe = sender.Frame;
                    senderframe.X = sender.Frame.GetMidX();
                    sheet.ShowFrom(senderframe, tbTopBar, true);
                    sheet.Dismissed += (object ob, UIButtonEventArgs e) =>
                    {
                        if (e.ButtonIndex == 0)
                        {
                            GlobalSettings.WBidStateCollection.IsModified = true;
                            WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);

                            //save the state of the INI File
                            WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());

                            GlobalSettings.isModified = false;
                            btnSave.Enabled = false;
                            SynchStateAndQQQuickSet();
                            //SynchState ();

                        }
                        else if (e.ButtonIndex == 1)
                        {
                            // StateManagement stateManagement = new StateManagement();
                            //stateManagement.UpdateWBidStateContent();
                            GlobalSettings.WBidStateCollection.IsModified = true;
                            WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);

                            //save the state of the INI File
                            WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());

                            GlobalSettings.isModified = false;
                            btnSave.Enabled = false;

                            CheckSmartSync();

                        }
                        else if (e.ButtonIndex == 2)
                        {
                            //CheckSmartSync();
                            GoToHome();
                        }
                    };
                }

                else
                {
                    sheet = new UIActionSheet("You want to save the changes before exit?", null, null, null, new string[] {
                        "Save & Exit",
                        "Exit"
                    });
                    CGRect senderframe = sender.Frame;
                    senderframe.X = sender.Frame.GetMidX();
                    sheet.ShowFrom(senderframe, tbTopBar, true);
                    sheet.Dismissed += (object ob, UIButtonEventArgs e) =>
                    {
                        if (e.ButtonIndex == 0)
                        {
                            // StateManagement stateManagement = new StateManagement();
                            //stateManagement.UpdateWBidStateContent();
                            GlobalSettings.WBidStateCollection.IsModified = true;
                            WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);

                            //save the state of the INI File
                            WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());

                            GlobalSettings.isModified = false;
                            btnSave.Enabled = false;

                            CheckSmartSync();

                        }
                        else if (e.ButtonIndex == 1)
                        {
                            //CheckSmartSync();
                            GoToHome();
                        }
                    };
                }
            }
            else
            {
                if (GlobalSettings.WBidStateCollection.IsModified || (GlobalSettings.QuickSets != null && GlobalSettings.QuickSets.IsModified))
                {
                    CheckSmartSync();
                }
                else
                    GoToHome();
            }
            //if (timer != null)
            //{
            //    timer.Stop ();
            //    timer.Close();
            //    timer.Dispose();

            //timer.Start ();
            //}
            //save the state of the INI File
            // WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
        }

        public void UpdateWBidStateContent()
        {
            //Save tag details to state file;


            if (GlobalSettings.Lines != null && GlobalSettings.Lines.Count > 0)
            {
                WBidState WBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

                WBidStateContent.TagDetails = new TagDetails();
                WBidStateContent.TagDetails.AddRange(GlobalSettings.Lines.ToList().Where(x => x.Tag != null && x.Tag.Trim() != string.Empty).Select(y => new Tag
                {
                    Line = y.LineNum,
                    Content = y.Tag
                }));

                int toplockcount = GlobalSettings.Lines.Where(x => x.TopLock == true).ToList().Count;
                int bottomlockcount = GlobalSettings.Lines.Where(x => x.BotLock == true).ToList().Count;
                //save the top and bottom lock
                WBidStateContent.TopLockCount = toplockcount;
                WBidStateContent.BottomLockCount = bottomlockcount;

                //Get the line oreder
                List<int> lineorderlist = GlobalSettings.Lines.Select(x => x.LineNum).ToList();
                LineOrders lineOrders = new LineOrders();
                int count = 1;
                lineOrders.Orders = lineorderlist.Select(x => new LineOrder() { LId = x, OId = count++ }).ToList();
                lineOrders.Lines = lineorderlist.Count;
                WBidStateContent.LineOrders = lineOrders;



                WBidStateContent.LineForBlueLine = GlobalSettings.Lines.Where(x => x.ManualScroll == 3 || x.ManualScroll == 1).Select(x => x.LineNum).FirstOrDefault();
                WBidStateContent.LinesForBlueBorder = GlobalSettings.Lines.Where(x => x.ManualScroll == 2).Select(x => x.LineNum).ToList();

                if (GlobalSettings.FAEOMStartDate != null)
                    WBidStateContent.FAEOMStartDate = GlobalSettings.FAEOMStartDate;
                else
                    WBidStateContent.FAEOMStartDate = DateTime.MinValue.ToUniversalTime();
            }


        }


        private void CheckSmartSync()
        {
            if (GlobalSettings.SynchEnable && GlobalSettings.WBidStateCollection != null && GlobalSettings.WBidINIContent != null && GlobalSettings.WBidINIContent.User.SmartSynch)
            {

                UIAlertController alert = UIAlertController.Create("Smart Sync", "Do you want to sync local changes with Server?", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("NO", UIAlertActionStyle.Cancel, (actionCancel) =>
                {
                    GoToHome();
                }));

                alert.AddAction(UIAlertAction.Create("YES", UIAlertActionStyle.Default, (actionOK) =>
                {
                    //SynchState();
                    SynchStateAndQQQuickSet();
                }));

                this.PresentViewController(alert, true, null);

            }
            else
            {
                GoToHome();
            }
        }

        private void GoToHome()
        {
            foreach (NSObject obj in arrObserver)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(obj);
            }
            CommonClass.selectedRows.Clear();
            //UIApplication.SharedApplication.KeyWindow.RootViewController = new homeViewController();
            //            UINavigationController navController = new UINavigationController ();
            //            homeViewController homeVC = new homeViewController ();
            //            UIApplication.SharedApplication.KeyWindow.RootViewController = navController;
            //            navController.NavigationBar.BarStyle = UIBarStyle.Black;
            //            navController.NavigationBar.Hidden = true;
            //            navController.PushViewController (homeVC, false);
            //this.NavigationController.DismissViewController(true,null);

            this.NavigationController.PopViewController(true);
            CommonClass.columnID = 0;

            GlobalSettings.UndoStack.Clear();
            GlobalSettings.RedoStack.Clear();
            if (timer != null)
            {
                timer.Stop();
                timer.Close();
                timer.Dispose();

                //timer.Start ();
            }


            foreach (UIView view in this.View.Subviews)
            {

                DisposeClass.DisposeEx(view);
            }
        }


        partial void btnPairingTapped(UIKit.UIButton sender)
        {
            PairingViewController pairingVC = new PairingViewController();
            pairingVC.ModalPresentationStyle = UIModalPresentationStyle.PageSheet;
            this.PresentViewController(pairingVC, true, null);
        }

        public void ShowGroupBidAutomator(NSNotification n)
        {
            BaPopViewController pairingVC = new BaPopViewController();
            pairingVC.ModalPresentationStyle = UIModalPresentationStyle.PageSheet;
            this.PresentViewController(pairingVC, true, null);
        }
        partial void btnOverlapTap(UIKit.UIButton sender)
        {
            sender.Selected = !sender.Selected;
            WBidHelper.PushToUndoStack();
            UpdateUndoRedoButtons();

            //            ReCalculateLinePropertiesForOverlapCorrection reCalculateLinePropertiesForOverlapCorrection = new ReCalculateLinePropertiesForOverlapCorrection ();
            string overlayTxt = string.Empty;

            if (sender.Selected)
            {
                //  btnVacCorrect.Enabled = false;
                //  btnVacDrop.Enabled = false;
                //  btnEOM.Enabled = false;

                GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = false;
                GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                GlobalSettings.MenuBarButtonStatus.IsEOM = false;
                GlobalSettings.MenuBarButtonStatus.IsOverlap = true;
                overlayTxt = "Applying Overlap Correction";

                SetVacButtonStates();


                //reCalculateLinePropertiesForOverlapCorrection.ReCalculateLinePropertiesOnOverlapCorrection(GlobalSettings.Lines.ToList(), true);
                LoadingOverlay overlay = new LoadingOverlay(this.View.Frame, overlayTxt);
                this.View.Add(overlay);
                InvokeInBackground(() =>
                {
                    try
                    {
                        //                        reCalculateLinePropertiesForOverlapCorrection.ReCalculateLinePropertiesOnOverlapCorrection (GlobalSettings.Lines.ToList (), true);
                        //                        SortLineList ();
                        StateManagement statemanagement = new StateManagement();
                        WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                        statemanagement.RecalculateLineProperties(wBidStateContent);
                        statemanagement.ApplyCSW(wBidStateContent);

                    }
                    catch (Exception ex)
                    {
                        InvokeOnMainThread(() =>
                        {
                            throw ex;
                        });
                    }

                    InvokeOnMainThread(() =>
                    {
                        overlay.Hide();
                        NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                        GlobalSettings.isModified = true;
                        CommonClass.lineVC.UpdateSaveButton();
                    });
                });
            }
            else
            {
                GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = false;
                GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                GlobalSettings.MenuBarButtonStatus.IsEOM = false;
                GlobalSettings.MenuBarButtonStatus.IsOverlap = false;
                overlayTxt = "Removing Overlap Correction";

                SetVacButtonStates();

                //reCalculateLinePropertiesForOverlapCorrection.ReCalculateLinePropertiesOnOverlapCorrection(GlobalSettings.Lines.ToList(), false);
                LoadingOverlay overlay = new LoadingOverlay(this.View.Frame, overlayTxt);
                this.View.Add(overlay);
                InvokeInBackground(() =>
                {
                    //                    reCalculateLinePropertiesForOverlapCorrection.ReCalculateLinePropertiesOnOverlapCorrection (GlobalSettings.Lines.ToList (), false);
                    StateManagement statemanagement = new StateManagement();
                    WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                    statemanagement.RecalculateLineProperties(wBidStateContent);
                    statemanagement.ApplyCSW(wBidStateContent);

                    InvokeOnMainThread(() =>
                    {
                        overlay.Hide();
                        NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                        GlobalSettings.isModified = true;
                        CommonClass.lineVC.UpdateSaveButton();
                    });
                });

            }

            WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            wBIdStateContent.MenuBarButtonState.IsOverlap = GlobalSettings.MenuBarButtonStatus.IsOverlap;

            //NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
        }

        partial void btnVacCorrectTap(UIKit.UIButton sender)
        {
            try
            {

                //==============New Code starts: Roshil

                bool IsFileAvailable = false;
                string status = string.Empty;
                sender.Selected = !sender.Selected;
                WBidHelper.PushToUndoStack();
                UpdateUndoRedoButtons();
                SetVACButton(sender);

                WBidCollection.GenarateTempAbsenceList();
                string overlayTxt = string.Empty;


                if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
                {
                    overlayTxt = "Applying Vacation Correction";

                    //btnVacDrop.Enabled = true;
                }
                else
                {
                    overlayTxt = "Removing Vacation Correction";

                }

                LoadingOverlay overlay = new LoadingOverlay(this.View.Frame, overlayTxt);
                this.View.Add(overlay);

                InvokeInBackground(() =>
                    {
                        try
                        {
                            WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                            status = WBidHelper.RetrieveSaveAndSetLineFiles(3, wBidStateContent);
                            //status = RetrieveAndSaveVACLineFiles(3);
                            IsFileAvailable = (status == "Ok") ? true : false;
                            if (IsFileAvailable)
                            {
                                StateManagement statemanagement = new StateManagement();

                                statemanagement.ApplyCSW(wBidStateContent);
                            }
                            else
                            {
                                InvokeOnMainThread(() =>
                                {
                                    UIAlertController okAlertController = UIAlertController.Create("WBidMax", status, UIAlertControllerStyle.Alert);
                                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                                    this.PresentViewController(okAlertController, true, null);
                                    sender.Selected = !sender.Selected;
                                    SetVACButton(sender);
                                    overlay.Hide();
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            InvokeOnMainThread(() =>
                            {

                                throw ex;
                            });
                        }

                        InvokeOnMainThread(() =>
                        {
                            //NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                            loadSummaryListAndHeader();
                            modernList.reloadModernData();
                            overlay.Hide();
                            GlobalSettings.isModified = true;
                            CommonClass.lineVC.UpdateSaveButton();
                        });
                    });
                WBidState wBidStateCont = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                wBidStateCont.MenuBarButtonState.IsVacationCorrection = GlobalSettings.MenuBarButtonStatus.IsVacationCorrection;

                //============== New Code ends



                //=============== Old code starts
                /*
                 
               GlobalSettings.MenuBarButtonStatus.IsEOM = true;


                sender.Selected = !sender.Selected;
                WBidHelper.PushToUndoStack();
                UpdateUndoRedoButtons();
                if (sender.Selected)
                {
                    //vacation button selected.
                    GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = true;
                    GlobalSettings.MenuBarButtonStatus.IsOverlap = false;
                    GlobalSettings.MenuBarButtonStatus.IsVacationDrop = true;
                    modernList.reloadModernData();
                }
                else
                {


                    //vacation button un selected.
                    GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = false;

                    if (GlobalSettings.MenuBarButtonStatus.IsEOM == false)
                    {
                        btnVacDrop.Selected = false;
                        GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                        modernList.reloadModernData();
                    }

                    //GlobalSettings.MenuBarButtonStatus.IsOverlap = false;
                }
                btnVacDrop.SetTitle("DRP", UIControlState.Selected);
                btnVacDrop.SetTitle("DRP", UIControlState.Normal);
                btnVacDrop.SetTitleColor(UIColor.DarkGray, UIControlState.Normal);
                this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                SetVacButtonStates();

                // GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = sender.Selected;
                //  if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection == false)
                //      GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;


                WBidCollection.GenarateTempAbsenceList();
                string overlayTxt = string.Empty;
                if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
                {
                    overlayTxt = "Applying Vacation Correction";
                    //btnVacDrop.Enabled = true;

                }
                else
                {
                    overlayTxt = "Removing Vacation Correction";
                    //btnVacDrop.Enabled = false;

                }
                //                foreach (var column in GlobalSettings.AdditionalColumns) {
                //                    column.IsSelected = false;
                //                }
                //                var selectedColumns = GlobalSettings.AdditionalColumns.Where (x => GlobalSettings.WBidINIContent.DataColumns.Any (y => y.Id == x.Id)).ToList ();
                //                foreach (var selectedColumn in selectedColumns) {
                //                    selectedColumn.IsSelected = true;
                //                }
                //
                //                foreach (var column in GlobalSettings.AdditionalvacationColumns) {
                //                    column.IsSelected = false;
                //                }
                //                var selectedVColumns = GlobalSettings.AdditionalColumns.Where (x => GlobalSettings.WBidINIContent.SummaryVacationColumns.Any (y => y.Id == x.Id)).ToList ();
                //                foreach (var selectedColumn in selectedVColumns) {
                //                    selectedColumn.IsSelected = true;
                //                }

                LoadingOverlay overlay = new LoadingOverlay(this.View.Frame, overlayTxt);
                this.View.Add(overlay);
                InvokeInBackground(() =>
                {
                    try
                    {

                        //                        PrepareModernBidLineView prepareModernBidLineView = new PrepareModernBidLineView ();
                        //                        RecalcalculateLineProperties RecalcalculateLineProperties = new PortableLibrary.BusinessLogic.RecalcalculateLineProperties ();
                        //                        prepareModernBidLineView.CalculatebidLinePropertiesforVacation ();
                        //                        RecalcalculateLineProperties.CalcalculateLineProperties ();
                        //                        SortLineList ();
                        StateManagement statemanagement = new StateManagement();
                        WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                        statemanagement.RecalculateLineProperties(wBidStateContent);
                        statemanagement.ApplyCSW(wBidStateContent);
                    }
                    catch (Exception ex)
                    {
                        InvokeOnMainThread(() =>
                        {

                            throw ex;
                        });
                    }

                    InvokeOnMainThread(() =>
                    {
                        //NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                        loadSummaryListAndHeader();
                        modernList.reloadModernData();
                        overlay.Hide();
                        GlobalSettings.isModified = true;
                        CommonClass.lineVC.UpdateSaveButton();
                    });
                });
                WBidState wBidStateCont = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                wBidStateCont.MenuBarButtonState.IsVacationCorrection = GlobalSettings.MenuBarButtonStatus.IsVacationCorrection;
                 */
                //================ Old code ends


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void SetVACButton(UIButton sender)
        {
            if (sender.Selected)
            {
                //vacation button selected.
                GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = true;
                GlobalSettings.MenuBarButtonStatus.IsOverlap = false;
                GlobalSettings.MenuBarButtonStatus.IsVacationDrop = true;
                modernList.reloadModernData();
            }

            else
            {

                //vacation button unselected.
                GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = false;

                if (GlobalSettings.MenuBarButtonStatus.IsEOM == false)
                {
                    btnVacDrop.Selected = false;
                    GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                    modernList.reloadModernData();
                }

            }
            if (GlobalSettings.MenuBarButtonStatus.IsEOM == true && GlobalSettings.MenuBarButtonStatus.IsVacationDrop == false)
            {
                btnVacDrop.SetTitle("FLY", UIControlState.Normal);
                btnVacDrop.SetTitleColor(UIColor.White, UIControlState.Normal);
                this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("activeButtonRed.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);


                this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            }
            else
            {
                btnVacDrop.SetTitle("DRP", UIControlState.Selected);
                btnVacDrop.SetTitle("DRP", UIControlState.Normal);
                btnVacDrop.SetTitleColor(UIColor.DarkGray, UIControlState.Normal);
                this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            }
            SetVacButtonStates();
        }


        partial void btnVacDropTap(UIKit.UIButton sender)
        {
            bool IsFileAvailable = false;
            string status = string.Empty;
            sender.Selected = !sender.Selected;
            WBidHelper.PushToUndoStack();
            UpdateUndoRedoButtons();

            GlobalSettings.MenuBarButtonStatus.IsVacationDrop = sender.Selected;
            SetVacButtonStates();


            WBidCollection.GenarateTempAbsenceList();
            string overlayTxt = string.Empty;
            if (GlobalSettings.MenuBarButtonStatus.IsVacationDrop)
            {

                overlayTxt = "Applying Vacation Drop";


                InvokeOnMainThread(() =>
                {
                    btnVacDrop.SetTitle("DRP", UIControlState.Selected);
                    this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);


                });
                VacationDropAction(overlayTxt);
                //}
                //else
                //{
                //    UIAlertController okAlertController = UIAlertController.Create("WBidMax", status, UIAlertControllerStyle.Alert);
                //    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                //    this.PresentViewController(okAlertController, true, null);
                //}
            }
            else
            {
                UIAlertController alert = UIAlertController.Create("WBidMax", "Careful : You have turned OFF the DRP button. The lines will be adjusted after you close this dialog. They will be adjusted to show that you are flying the VDF(red) and VDB(red).\n\nIf you have “drop all” selected as your preference in CWA, then you should turn back on the DRP button to see the lines as they will be after the VDF and VDB are dropped.\n\nIf this does not make sense, please go to Help menu and select Help to read about Vacation Corrections for Pilots and Flight Attendants.", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) =>
                {
                    overlayTxt = "Removing Vacation Drop";
                    //1 for getting DRP files

                    VacationDropAction(overlayTxt);
                    //else
                    //{
                    //    UIAlertController okAlertController = UIAlertController.Create("WBidMax", status, UIAlertControllerStyle.Alert);
                    //    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    //    this.PresentViewController(okAlertController, true, null);
                    //}


                }));

                this.PresentViewController(alert, true, null);

            }
        }
        public void VacationDropAction(string overlayTxt1)
        {
            LoadingOverlay overlay = new LoadingOverlay(this.View.Frame, overlayTxt1);
            this.View.Add(overlay);
            InvokeInBackground(() =>
            {
                try
                {


                    //1 for getting DRP files
                    var status = WBidHelper.RetrieveSaveAndSetLineFiles(1, wBIdStateContent);
                    //status =RetrieveAndSaveVACLineFiles(1);
                    bool IsFileAvailable = (status == "Ok") ? true : false;
                    if (IsFileAvailable)
                    {
                        InvokeOnMainThread(() =>
                        {
                            btnVacDrop.SetTitle("FLY", UIControlState.Normal);
                            btnVacDrop.SetTitleColor(UIColor.White, UIControlState.Normal);
                            this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("activeButtonRed.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);

                        });
                        StateManagement statemanagement = new StateManagement();
                        WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                        //Old Code
                        //statemanagement.RecalculateLineProperties (wBidStateContent);
                        statemanagement.ApplyCSW(wBidStateContent);
                    }
                    else
                    {


                        GlobalSettings.MenuBarButtonStatus.IsVacationDrop = !GlobalSettings.MenuBarButtonStatus.IsVacationDrop;
                        //InvokeOnMainThread(() => {
                        //    btnVacDrop.Selected = !btnVacDrop.Selected;
                        //    btnVacDrop.SetTitle("DRP", UIControlState.Selected);
                        //    this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);

                        //});

                        InvokeOnMainThread(() =>
                        {
                            this.btnVacDrop.Selected = !this.btnVacDrop.Selected;
                            // this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                            UIAlertController okAlertController = UIAlertController.Create("WBidMax", status, UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionCancel) =>
                            {
                                // btnVacDrop.SetTitle("DRP", UIControlState.Selected);
                                //this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                                //btnVacDrop.SetTitle("FLY", UIControlState.Normal);
                                //btnVacDrop.SetTitleColor(UIColor.White, UIControlState.Normal);
                                //this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("activeButtonRed.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                            }));
                            this.PresentViewController(okAlertController, true, null);
                        });
                    }

                }
                catch (Exception ex)
                {
                    InvokeOnMainThread(() =>
                    {
                        throw ex;
                    });
                }

                InvokeOnMainThread(() =>
                {
                    NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                    overlay.Hide();
                    GlobalSettings.isModified = true;
                    CommonClass.lineVC.UpdateSaveButton();
                });
            });
            WBidState wBidStateCont = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            wBidStateCont.MenuBarButtonState.IsVacationDrop = GlobalSettings.MenuBarButtonStatus.IsVacationDrop;
        }






        partial void btnMILTapped(UIKit.UIButton sender)
        {
            if (GlobalSettings.MenuBarButtonStatus.IsMIL)
            {
                sender.Selected = false;
                WBidHelper.PushToUndoStack();
                UpdateUndoRedoButtons();
                LoadingOverlay overlay = new LoadingOverlay(this.View.Frame, "Removing MIL. Please wait.. ");
                this.View.Add(overlay);
                InvokeInBackground(() =>
                {
                    GlobalSettings.MenuBarButtonStatus.IsMIL = false;

                    RecalcalculateLineProperties RecalcalculateLineProperties = new PortableLibrary.BusinessLogic.RecalcalculateLineProperties();
                    RecalcalculateLineProperties.CalcalculateLineProperties();
                    StateManagement statemanagement = new StateManagement();
                    WBidState wBidState = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                    statemanagement.ApplyCSW(wBidState);
                    PrepareModernBidLineView prepareModernBidLineView = new PrepareModernBidLineView();

                    prepareModernBidLineView.CalculatebidLinePropertiesforVacation();

                    SortLineList();
                    InvokeOnMainThread(() =>
                    {
                        GlobalSettings.isModified = true;
                        CommonClass.lineVC.UpdateSaveButton();
                        SetVacButtonStates();
                        NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                        overlay.Hide();
                    });
                });
            }
            else
            {
                var milVC = new MILConfigViewController();
                milVC.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                this.PresentViewController(milVC, true, null);
            }

            var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            wBidStateContent.MenuBarButtonState.IsMIL = GlobalSettings.MenuBarButtonStatus.IsMIL;
        }



        private static void SortLineList()
        {
            SortCalculation sort = new SortCalculation();
            WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            if (wBidStateContent.SortDetails != null && wBidStateContent.SortDetails.SortColumn != null && wBidStateContent.SortDetails.SortColumn != string.Empty)
            {
                sort.SortLines(wBidStateContent.SortDetails.SortColumn);
            }
        }

        partial void btnEOMTapped(UIKit.UIButton sender)
        {
            //=================New code starts Roshil
            try
            {
                WBidHelper.PushToUndoStack();
                UpdateUndoRedoButtons();

                if (!sender.Selected)
                {
                    GlobalSettings.MenuBarButtonStatus.IsEOM = true;
                    GlobalSettings.MenuBarButtonStatus.IsVacationDrop = true;

                    SetVacButtonStates();

                    if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                    {
                        DateTime defDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1);
                        defDate.AddMonths(1);
                        string[] strParams = {
                            String.Format ("{0:m}", GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays (1)),
                            String.Format ("{0:m}", GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays (2)),
                            String.Format ("{0:m}", GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays (3))
                        };
                        UIActionSheet sheet = new UIActionSheet("Where does your vacation start on next month?", null, null, null, strParams);
                        CGRect senderframe = sender.Frame;
                        senderframe.X = sender.Frame.GetMidX();
                        sheet.ShowFrom(senderframe, tbBottomBar, true);
                        sheet.Clicked += handleEOMOptions;

                    }
                    else
                    {
                        WBidCollection.GenarateTempAbsenceList();
                        sender.Selected = true;
                        string overlayTxt = string.Empty;
                        if (GlobalSettings.MenuBarButtonStatus.IsEOM)
                            overlayTxt = "Applying EOM";
                        else
                        {
                            if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
                            {
                                btnVacDrop.Selected = false;
                                btnVacDrop.Enabled = false;
                                GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                            }
                            overlayTxt = "Removing EOM";
                        }



                        LoadingOverlay overlay = new LoadingOverlay(this.View.Frame, overlayTxt);
                        this.View.Add(overlay);
                        InvokeInBackground(() =>
                        {

                            try
                            {
                                WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                                var status = WBidHelper.RetrieveSaveAndSetLineFiles(3, wBidStateContent);
                                if (status == "Ok")
                                {
                                    StateManagement statemanagement = new StateManagement();

                                    statemanagement.ApplyCSW(wBidStateContent);
                                }


                                InvokeOnMainThread(() =>
                                {
                                    if (status == "Ok")
                                    {
                                        loadSummaryListAndHeader();
                                        // NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                                        overlay.Hide();
                                        GlobalSettings.isModified = true;
                                        CommonClass.lineVC.UpdateSaveButton();
                                    }
                                    else
                                    {
                                        overlay.Hide();
                                        UIAlertController okAlertController = UIAlertController.Create("WBidMax", status, UIAlertControllerStyle.Alert);
                                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                                        this.PresentViewController(okAlertController, true, null);
                                        sender.Selected = false;
                                        GlobalSettings.MenuBarButtonStatus.IsEOM = false;
                                        UnselectEOM();
                                    }
                                });
                            }
                            catch (Exception ex)
                            {
                                InvokeOnMainThread(() =>
                                {
                                    throw ex;
                                });
                            }
                        });






                    }
                    if (GlobalSettings.MenuBarButtonStatus.IsEOM && GlobalSettings.CurrentBidDetails.Postion != "FA")
                    {

                        var eomstartdate = GetnextSunday();
                        List<Weekday> vacationweeks = new List<Weekday>();

                        vacationweeks.Add(new Weekday() { StartDate = eomstartdate, EndDate = eomstartdate.AddDays(6), Code = "EOM" });

                        var startDateEOM = "";
                        var endDateEOM = "";
                        string AlertText = string.Empty;

                        //EOM Vacation
                        startDateEOM = vacationweeks.Find(x => x.Code == "EOM").StartDate.Day + " " + vacationweeks.Find(x => x.Code == "EOM").StartDate.ToString("MMM");
                        endDateEOM = vacationweeks.Find(x => x.Code == "EOM").EndDate.Day + " " + vacationweeks.Find(x => x.Code == "EOM").EndDate.ToString("MMM");
                        AlertText = "You have an 'EOM'  vacation: \n\n" + startDateEOM + " - " + endDateEOM;
                        AlertText += "\n\nEOM weeks can affect the vacation pay in the current bid period and also the next month.";
                        AlertText += "\n\nWe have two documents regarding Month-to-Month vacations that also apply to EOM vacation weeks.";
                        AlertText += "\n\nWe suggest you read the following documents to improve your bidding knowledge";

                        ShowMonthToMonthAlerView(AlertText);

                    }
                }
                else
                {

                    sender.Selected = false;
                    GlobalSettings.MenuBarButtonStatus.IsEOM = false;
                    WBidCollection.GenarateTempAbsenceList();
                    UnselectEOM();

                    string overlayTxt = string.Empty;
                    if (GlobalSettings.MenuBarButtonStatus.IsEOM)
                        overlayTxt = "Applying EOM";
                    else
                    {
                        if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
                        {
                            btnVacDrop.Selected = false;
                            btnVacDrop.Enabled = false;
                            GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                        }
                        overlayTxt = "Removing EOM";
                    }

                    LoadingOverlay overlay = new LoadingOverlay(this.View.Frame, overlayTxt);
                    this.View.Add(overlay);

                    InvokeInBackground(() =>
                    {
                        string status = string.Empty;
                        try
                        {
                            WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                            status = WBidHelper.RetrieveSaveAndSetLineFiles(3, wBidStateContent);
                            //status = RetrieveAndSaveVACLineFiles(3);
                            if (status == "Ok")
                            {
                                StateManagement statemanagement = new StateManagement();

                                statemanagement.ApplyCSW(wBidStateContent);
                            }
                        }
                        catch (Exception ex)
                        {
                            InvokeOnMainThread(() =>
                            {
                                throw ex;
                            });
                        }

                        InvokeOnMainThread(() =>
                        {
                            if (status == "Ok")
                            {
                                loadSummaryListAndHeader();
                                //NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                                overlay.Hide();
                                GlobalSettings.isModified = true;
                                CommonClass.lineVC.UpdateSaveButton();
                            }
                            else
                            {
                                overlay.Hide();
                                UIAlertController okAlertController = UIAlertController.Create("WBidMax", status, UIAlertControllerStyle.Alert);
                                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                                this.PresentViewController(okAlertController, true, null);
                                GlobalSettings.MenuBarButtonStatus.IsEOM = true;
                                sender.Selected = true;
                                UnselectEOM();
                            }

                        });
                    });
                }
                WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                wBIdStateContent.MenuBarButtonState.IsEOM = GlobalSettings.MenuBarButtonStatus.IsEOM;





                //=================New code ends


                //================ Old code starts
                /*
                //try
                //{
                    string currentBidName = WBidHelper.GenerateFileNameUsingCurrentBidDetails();

                    //string zipFileName = GenarateZipFileName();
                    string vACFileName = WBidHelper.GetAppDataPath() + "//" + currentBidName + ".VAC";
                    //Cheks the VAC file exists
                    bool vacFileExists = File.Exists(vACFileName);

                    if (GlobalSettings.CurrentBidDetails.Postion != "FA" && !vacFileExists && (GlobalSettings.WBidINIContent.User.IsSouthWestWifiTest || WBidHelper.IsSouthWestWifiOr2wire()))
                    {

                        UIAlertController okAlertController = UIAlertController.Create("WBidMax", "You have a limited Wifi connection using the SouthwestWifi.You are not be able to do the Flight data download for EOM operation using SouthWest Wifi.", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);
                    }
                    else
                    {
                        WBidHelper.PushToUndoStack();
                        UpdateUndoRedoButtons();

                        if (!sender.Selected)
                        {
                            GlobalSettings.MenuBarButtonStatus.IsEOM = true;
                            GlobalSettings.MenuBarButtonStatus.IsVacationDrop = true;
                            //                    WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                            //                    wBIdStateContent.MenuBarButtonState.IsEOM=true;

                            SetVacButtonStates();
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                            {
                                DateTime defDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1);
                                defDate.AddMonths(1);
                                string[] strParams = {
                                String.Format ("{0:m}", GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays (1)),
                                String.Format ("{0:m}", GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays (2)),
                                String.Format ("{0:m}", GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays (3))
                            };
                                UIActionSheet sheet = new UIActionSheet("Where does your vacation start on next month?", null, null, null, strParams);
                                CGRect senderframe = sender.Frame;
                                senderframe.X = sender.Frame.GetMidX();
                                sheet.ShowFrom(senderframe, tbBottomBar, true);
                                //GlobalSettings.FAEOMStartDate = DateTime.MinValue;
                                sheet.Clicked += handleEOMOptions;

                            }
                            else
                            {
                                sender.Selected = true;
                                //btnVacDrop.Enabled = true;

                                //                        string currentBidName = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();
                                //
                                //                        //string zipFileName = GenarateZipFileName();
                                //                        string vACFileName = WBidHelper.GetAppDataPath () + "//" + currentBidName + ".VAC";
                                //                        //Cheks the VAC file exists
                                //                        bool vacFileExists = File.Exists (vACFileName);

                                if (!vacFileExists)
                                {

                                    CreateEOMVacFileForCP(currentBidName);
                                }
                                else
                                {



                                    string overlayTxt = string.Empty;
                                    if (GlobalSettings.MenuBarButtonStatus.IsEOM)
                                        overlayTxt = "Applying EOM";
                                    else
                                    {
                                        if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
                                        {
                                            btnVacDrop.Selected = false;
                                            btnVacDrop.Enabled = false;
                                            GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                                        }
                                        overlayTxt = "Removing EOM";
                                    }

                                    LoadingOverlay overlay = new LoadingOverlay(this.View.Frame, overlayTxt);
                                    this.View.Add(overlay);
                                    InvokeInBackground(() => {

                                        try {

                                            if (GlobalSettings.VacationData == null) {
                                                using (FileStream vacstream = File.OpenRead(vACFileName)) {

                                                    Dictionary<string, TripMultiVacData> objineinfo = new Dictionary<string, TripMultiVacData>();
                                                    GlobalSettings.VacationData = ProtoSerailizer.DeSerializeObject(vACFileName, objineinfo, vacstream);
                                                }
                                            }


                                            GenerateVacationDataView();

                                            InvokeOnMainThread(() => {
                                                loadSummaryListAndHeader();
                                                // NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                                                overlay.Hide();
                                                GlobalSettings.isModified = true;
                                                CommonClass.lineVC.UpdateSaveButton();
                                            });
                                        } catch (Exception ex) {
                                            InvokeOnMainThread(() => {
                                                throw ex;
                                            });
                                        }
                                    });

                                }




                            }
                            if (GlobalSettings.MenuBarButtonStatus.IsEOM && GlobalSettings.CurrentBidDetails.Postion != "FA")
                            {

                                var eomstartdate = GetnextSunday();
                                List<Weekday> vacationweeks = new List<Weekday>();

                                vacationweeks.Add(new Weekday() { StartDate = eomstartdate, EndDate = eomstartdate.AddDays(6), Code = "EOM" });

                                var startDateEOM = "";
                                var endDateEOM = "";
                                string AlertText = string.Empty;

                                //EOM Vacation
                                startDateEOM = vacationweeks.Find(x => x.Code == "EOM").StartDate.Day + " " + vacationweeks.Find(x => x.Code == "EOM").StartDate.ToString("MMM");
                                endDateEOM = vacationweeks.Find(x => x.Code == "EOM").EndDate.Day + " " + vacationweeks.Find(x => x.Code == "EOM").EndDate.ToString("MMM");
                                AlertText = "You have an 'EOM'  vacation: \n\n" + startDateEOM + " - " + endDateEOM;
                                AlertText += "\n\nEOM weeks can affect the vacation pay in the current bid period and also the next month.";
                                AlertText += "\n\nWe have two documents regarding Month-to-Month vacations that also apply to EOM vacation weeks.";
                                AlertText += "\n\nWe suggest you read the following documents to improve your bidding knowledge";

                                ShowMonthToMonthAlerView(AlertText);

                            }
                        }
                        else {

                            GlobalSettings.MenuBarButtonStatus.IsEOM = false;
                            sender.Selected = false;
                            if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection) {
                                btnVacDrop.Selected = false;
                                GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                            }
                            btnVacDrop.SetTitle("DRP", UIControlState.Selected);
                            btnVacDrop.SetTitle("DRP", UIControlState.Normal);
                            btnVacDrop.SetTitleColor(UIColor.DarkGray, UIControlState.Normal);
                            this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                            this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
                            SetVacButtonStates();

                            string overlayTxt = string.Empty;
                            if (GlobalSettings.MenuBarButtonStatus.IsEOM)
                                overlayTxt = "Applying EOM";
                            else
                            {
                                if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
                                {
                                    btnVacDrop.Selected = false;
                                    btnVacDrop.Enabled = false;
                                    GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                                }
                                overlayTxt = "Removing EOM";
                            }

                            LoadingOverlay overlay = new LoadingOverlay(this.View.Frame, overlayTxt);
                            this.View.Add(overlay);
                            InvokeInBackground(() => {
                                try {
                                    GenerateVacationDataView();
                                } catch (Exception ex) {
                                    InvokeOnMainThread(() => {
                                        throw ex;
                                    });
                                }

                                InvokeOnMainThread(() => {
                                    loadSummaryListAndHeader();
                                    //NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                                    overlay.Hide();
                                    GlobalSettings.isModified = true;
                                    CommonClass.lineVC.UpdateSaveButton();
                                });
                            });

                        }

                        WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                        wBIdStateContent.MenuBarButtonState.IsEOM = GlobalSettings.MenuBarButtonStatus.IsEOM;
                    }
                */

            }
            catch (Exception ex)
            {

                throw ex;
            }
            //================Old code ends


        }

        private void UnselectEOM()
        {


            if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
            {
                btnVacDrop.Selected = false;
                GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
            }
            if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && GlobalSettings.MenuBarButtonStatus.IsVacationDrop == false)
            {
                btnVacDrop.SetTitle("FLY", UIControlState.Normal);
                btnVacDrop.SetTitleColor(UIColor.White, UIControlState.Normal);
                this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("activeButtonRed.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);


                this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            }
            else
            {
                btnVacDrop.SetTitle("DRP", UIControlState.Selected);
                btnVacDrop.SetTitle("DRP", UIControlState.Normal);
                btnVacDrop.SetTitleColor(UIColor.DarkGray, UIControlState.Normal);
                this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            }
            SetVacButtonStates();
        }



        ///// <summary>
        ///// Retrive vac file  from local or from server
        ///// </summary>
        ///// <param name="isVacEomDrpBit"></param>
        //private string RetrieveAndSaveVACLineFiles(int isVacEomDrpBit)
        //{
        //    string status = string.Empty;
        //    try
        //    {
        //        string vacFilePath = string.Empty;
        //        string vacFileName = "";
        //        int faEomstartday = 0;
        //        if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.MenuBarButtonStatus.IsEOM)
        //        {

        //            if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1) == wBIdStateContent.FAEOMStartDate)
        //                faEomstartday = 1;
        //            else if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(2) == wBIdStateContent.FAEOMStartDate)
        //                faEomstartday = 2;
        //            else if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(3) == wBIdStateContent.FAEOMStartDate)
        //                faEomstartday = 3;

        //            vacFileName = GetVacationFileName(faEomstartday);
        //        }
        //        else
        //            vacFileName = GetVacationFileName();

        //        vacFilePath = WBidHelper.GetAppDataPath() + "/" + vacFileName;

        //        if (File.Exists(vacFilePath))
        //        {

        //            var compressedData = File.ReadAllText(vacFilePath);
        //            string VAClinefileJsoncontent = LZStringCSharp.LZString.DecompressFromUTF16(compressedData);

        //            //desrialise the Json
        //            LineInfo wblLine = WBidCollection.ConvertJSonStringToObject<LineInfo>(VAClinefileJsoncontent);


        //            GlobalSettings.Lines = new ObservableCollection<Line>(wblLine.Lines.Values);
        //            status = "Ok";
        //            RecalcalculateLineProperties objrecalculate = new RecalcalculateLineProperties();
        //            objrecalculate.CalculateDropTemplateForBidLines(GlobalSettings.Lines);
        //        }
        //        else
        //        {
        //            //checkBit
        //            //1 For DRP
        //            //2 For EOM
        //            //3 For VAC
        //            int chekBit = isVacEomDrpBit;
        //            status= DownloadVacFilesFromServer(chekBit, vacFileName, faEomstartday);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        status = "Something Went Wrong";
        //    }
        //    return status;
        //}


        /// <summary>
        /// Getting vac file name as per  EOM,VAC,DRP button click
        /// </summary>
        /// <param name="FAEOMStartDate">if FA Bid ,we have FA positions  like 1,2,3 </param>
        /// <returns></returns>
        private string GetVacationFileName(int FAEOMStartDate = 0)
        {
            try
            {
                var currentbiddetail = GlobalSettings.CurrentBidDetails;
                AppDataBidFileNames appDataBidFileNames = GlobalSettings.WbidUserContent.AppDataBidFiles.FirstOrDefault(x => x.Domicile == currentbiddetail.Domicile && x.Position == currentbiddetail.Postion && x.Round == currentbiddetail.Round && x.Month == currentbiddetail.Month && x.Year == currentbiddetail.Year);


                //List<string> lstBidFiles = new List<string>();
                //lstBidFiles = GlobalSettings.WBidStateCollection.DownlaodedbidFiles;

                string lineFileName = string.Empty;

                //string vacFileName = lstBidFiles.FirstOrDefault(x => x.Contains(".WBV"));
                //if (vacFileName != null && vacFileName.Length > 3)
                //    vacFileName = vacFileName.Split('.')[0];

                //string normalinefile = lstBidFiles.FirstOrDefault(x => x.Contains(".WBL"));
                //if (normalinefile != null && normalinefile.Length > 3)
                //    normalinefile = normalinefile.Split('.')[0];

                if (appDataBidFileNames != null)
                {
                    MenuBarButtonStatus MenuButton = GlobalSettings.MenuBarButtonStatus;

                    if (!MenuButton.IsVacationCorrection && !MenuButton.IsVacationDrop && !MenuButton.IsEOM)
                    {
                        //No vacation-Set WBL file name
                        BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.NormalLine);
                        if (filename != null)
                            lineFileName = filename.FileName;
                        // lineFileName = normalinefile + ".WBL";
                    }
                    if (MenuButton.IsVacationCorrection && MenuButton.IsVacationDrop && !MenuButton.IsEOM)
                    {
                        //VAC+DRP
                        BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.Vacation);
                        if (filename != null)
                            lineFileName = filename.FileName;
                        //lineFileName = vacFileName + ".WBV";
                    }
                    if (MenuButton.IsVacationCorrection && !MenuButton.IsVacationDrop && !MenuButton.IsEOM)
                    {
                        //VAC
                        BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.VacationDropOFF);
                        if (filename != null)
                            lineFileName = filename.FileName;
                        //lineFileName = vacFileName + ".DRP";
                    }
                    if (MenuButton.IsVacationCorrection && MenuButton.IsVacationDrop && MenuButton.IsEOM)
                    {
                        //VAC+DRP+EOM
                        if (GlobalSettings.CurrentBidDetails.Postion != "FA")
                        {
                            BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.VacationEOM);
                            if (filename != null)
                                lineFileName = filename.FileName;
                        }
                        else
                        {
                            BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileName.Contains("F" + FAEOMStartDate + ".WBE") && x.FileType == (int)BidFileType.VacationEOM);
                            if (filename != null)
                                lineFileName = filename.FileName;
                        }
                        //    lineFileName = vacFileName + "F.WBE";
                        //else
                        //    lineFileName = vacFileName + "F" + FAEOMStartDate + ".WBE";
                    }
                    if (!MenuButton.IsVacationCorrection && !MenuButton.IsVacationDrop && MenuButton.IsEOM)
                    {
                        //EOM
                        if (GlobalSettings.CurrentBidDetails.Postion != "FA")
                        {
                            BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.EomDropOFF);
                            if (filename != null)
                                lineFileName = filename.FileName;
                        }
                        else
                        {
                            BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileName.Contains("F" + FAEOMStartDate + ".DRP") && x.FileType == (int)BidFileType.EomDropOFF);
                            if (filename != null)
                                lineFileName = filename.FileName;
                        }
                        //    if (GlobalSettings.CurrentBidDetails.Postion != "FA")
                        //    lineFileName = vacFileName + "F.DRP";
                        //else
                        //    lineFileName = vacFileName + "F" + FAEOMStartDate + ".DRP";
                    }
                    if (!MenuButton.IsVacationCorrection && MenuButton.IsVacationDrop && MenuButton.IsEOM)
                    {
                        //EOM+DRP

                        if (GlobalSettings.CurrentBidDetails.Postion != "FA")
                        {
                            BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.Eom);
                            if (filename != null)
                                lineFileName = filename.FileName;
                        }
                        else
                        {
                            BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileName.Contains("F" + FAEOMStartDate + ".WBE") && x.FileType == (int)BidFileType.Eom);
                            if (filename != null)
                                lineFileName = filename.FileName;
                        }
                        //if (GlobalSettings.CurrentBidDetails.Postion != "FA")
                        //    lineFileName = normalinefile + "F.WBE";
                        //else
                        //    lineFileName = normalinefile + "F" + FAEOMStartDate + ".WBE";
                    }
                    if (MenuButton.IsVacationCorrection && !MenuButton.IsVacationDrop && MenuButton.IsEOM)
                    {

                        //EOM+VAC
                        if (GlobalSettings.CurrentBidDetails.Postion != "FA")
                        {
                            BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.VacationEomDropOFF);
                            if (filename != null)
                                lineFileName = filename.FileName;
                        }
                        else
                        {
                            BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileName.Contains("F" + FAEOMStartDate + ".WBE") && x.FileType == (int)BidFileType.VacationEomDropOFF);
                            if (filename != null)
                                lineFileName = filename.FileName;
                        }
                    }

                }
                return lineFileName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        /// <summary>
        /// Download VAC,EOM,DRP Files from server
        /// </summary>
        /// <param name="checkBit">for checking whether  EOM,DRP or VAC files to donwload</param>

        private string DownloadVacFilesFromServer(int checkBit, string vacFileName, int faEomstartday)
        {
            string status = string.Empty;
            try
            {
                BidDataFileResponse biddataresponse = new BidDataFileResponse();
                BidDataRequestDTO bidDetails = new BidDataRequestDTO();
                VacFilesRequest vacRequest = new VacFilesRequest();
                bidDetails.EmpNum = int.Parse(GlobalSettings.WbidUserContent.UserInformation.EmpNo);
                bidDetails.Base = GlobalSettings.CurrentBidDetails.Domicile;
                bidDetails.Position = GlobalSettings.CurrentBidDetails.Postion;
                bidDetails.Month = GlobalSettings.CurrentBidDetails.Month;
                bidDetails.Year = GlobalSettings.CurrentBidDetails.Year;
                bidDetails.Round = GlobalSettings.CurrentBidDetails.Round;
                //bidDetails.Round = GlobalSettings.CurrentBidDetails.Round == "D" ? "M" : "S";

                vacRequest.bidDataRequest = bidDetails;
                //1 for DRP Files
                //2 For EOM files
                //3 for vac Files
                vacRequest.checkVACBit = checkBit;
                vacRequest.FAPositions = faEomstartday;
                vacRequest.vacFileName = vacFileName;

                try
                {
                    var jsonData = ServiceUtils.JsonSerializer(vacRequest);
                    StreamReader dr = ServiceUtils.GetRestData("GetVacFilesDRPEOM", jsonData);
                    biddataresponse = WBidCollection.ConvertJSonStringToObject<BidDataFileResponse>(dr.ReadToEnd());
                }
                catch (Exception ex)
                {
                    status = "Server Error";
                }
                if (biddataresponse.Status == true)
                {
                    //Show alert if the  data is not available
                    if (biddataresponse.bidData.Count > 0)
                    {
                        foreach (var item in biddataresponse.bidData)
                        {
                            //Decompress the string using LZ compress.
                            string linefileJsoncontent = LZStringCSharp.LZString.DecompressFromUTF16(item.FileContent);

                            File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + item.FileName, item.FileContent);

                            //desrialise the Json
                            LineInfo wblVACLine = WBidCollection.ConvertJSonStringToObject<LineInfo>(linefileJsoncontent);

                            GlobalSettings.Lines = new ObservableCollection<Line>(wblVACLine.Lines.Values);
                            status = "Ok";
                            RecalcalculateLineProperties objrecalculate = new RecalcalculateLineProperties();
                            objrecalculate.CalculateDropTemplateForBidLines(GlobalSettings.Lines);
                        }
                    }
                }
                else
                {
                    status = biddataresponse.Message;
                    //show alert
                    //InvokeOnMainThread(() =>
                    //{
                    //    AlertController = UIAlertController.Create(biddataresponse.Message, "WBidMax", UIAlertControllerStyle.Alert);
                    //    AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) =>
                    //    {
                    //        DismissCurrentView();
                    //    }));

                    //    this.PresentViewController(AlertController, true, null);

                    //});
                }
            }
            catch (Exception ex)
            {
                status = "ErrorDownload";
            }
            return status;
        }

        private void ShowMonthToMonthAlerView(string AlertText)
        {
            MonthToMonthAlertView monthtomonthalert = new MonthToMonthAlertView();
            monthtomonthalert.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            UINavigationController nav = new UINavigationController(monthtomonthalert);
            monthtomonthalert.alert = AlertText;
            nav.NavigationBarHidden = true;
            nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            this.PresentViewController(nav, true, null);
        }


        private static void GenerateVacationDataView()
        {



            //            WBidCollection.GenarateTempAbsenceList ();
            //            PrepareModernBidLineView prepareModernBidLineView = new PrepareModernBidLineView ();
            //            RecalcalculateLineProperties RecalcalculateLineProperties = new PortableLibrary.BusinessLogic.RecalcalculateLineProperties ();
            //            prepareModernBidLineView.CalculatebidLinePropertiesforVacation ();
            //            RecalcalculateLineProperties.CalcalculateLineProperties ();
            //            SortLineList ();

            StateManagement statemanagement = new StateManagement();
            WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            statemanagement.RecalculateLineProperties(wBidStateContent);
            statemanagement.ApplyCSW(wBidStateContent);

        }


        private void CreateEOMVacFileForCP(string currentBidName)
        {


            InvokeOnMainThread(() =>
            {
                UIAlertController alert = UIAlertController.Create("WBidMax", "WBidMax needs to download vacation data to make the predictions for your end of month trips (EOM VAC).   This could take up to a minute.  Do you want to continue?", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (actionCancel) =>
                {
                    if (!wBIdStateContent.IsVacationOverlapOverlapCorrection)
                        btnVacDrop.Enabled = false;
                    btnEOM.Selected = false;
                    GlobalSettings.MenuBarButtonStatus.IsEOM = false;
                    SetVacButtonStates();
                }));

                alert.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, (actionOK) =>
                {
                    btnVacDrop.Enabled = true;
                    string overlayTxt = string.Empty;
                    if (GlobalSettings.MenuBarButtonStatus.IsEOM)
                        overlayTxt = "Applying EOM";
                    else
                    {
                        if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
                        {
                            btnVacDrop.Selected = false;
                            btnVacDrop.Enabled = false;
                            GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                        }
                        overlayTxt = "Removing EOM";
                    }

                    LoadingOverlay overlay = new LoadingOverlay(this.View.Frame, overlayTxt);
                    this.View.Add(overlay);
                    InvokeInBackground(() =>
                    {
                        CreateEOMVacationforCP();

                        GenerateVacationDataView();

                        InvokeOnMainThread(() =>
                        {
                            loadSummaryListAndHeader();
                            // NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                            overlay.Hide();
                        });
                    });

                }));

                this.PresentViewController(alert, true, null);
            });


        }




        private static void CreateEOMVacationforCP()
        {


            try
            {

                DateTime nextSunday = GetnextSunday();


                GlobalSettings.OrderedVacationDays = new List<Absense>() { new Absense {
                        StartAbsenceDate = nextSunday,
                        EndAbsenceDate = nextSunday.AddDays (6),
                        AbsenceType = "VA"
                    }
                };

                string serverPath = GlobalSettings.WBidDownloadFileUrl + "FlightData.zip";
                string zipLocalFile = Path.Combine(WBidHelper.GetAppDataPath(), "FlightData.zip");
                string networkDataPath = WBidHelper.GetAppDataPath() + "/" + "FlightData.NDA";
                if (File.Exists(networkDataPath))
                    File.Delete(networkDataPath);
                FlightPlan flightPlan = null;
                WebClient wcClient = new WebClient();
                //Downloading networkdat file
                if (File.Exists(networkDataPath))
                    File.Delete(networkDataPath);
                wcClient.DownloadFile(serverPath, zipLocalFile);

                //                //Extracting the zip file
                //                var zip = new ZipArchive();
                //                zip.EasyUnzip(zipLocalFile, WBidHelper.GetAppDataPath(), true, "");

                string target = Path.Combine(WBidHelper.GetAppDataPath(), WBidHelper.GetAppDataPath() + "/");//+ Path.GetFileNameWithoutExtension(zipLocalFile))+ "/";

                if (!File.Exists(networkDataPath))
                {
                    if (File.Exists(zipLocalFile))
                    {

                        ZipFile.ExtractToDirectory(zipLocalFile, target);
                    }
                }
                //ZipStorer.

                // Open an existing zip file for reading
                //                ZipStorer zip = ZipStorer.Open (zipLocalFile, FileAccess.Read);
                //
                //                // Read the central directory collection
                //                List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir ();
                //
                //                // Look for the desired file
                //                foreach (ZipStorer.ZipFileEntry entry in dir) {
                //                    zip.ExtractFile (entry, target + entry);
                //                }
                //                zip.Close ();

                //Deserializing data to FlightPlan object
                FlightPlan fp = new FlightPlan();
                using (FileStream networkDatatream = File.OpenRead(networkDataPath))
                {

                    FlightPlan objineinfo = new FlightPlan();
                    flightPlan = ProtoSerailizer.DeSerializeObject(networkDataPath, fp, networkDatatream);

                }

                if (File.Exists(zipLocalFile))
                {
                    File.Delete(zipLocalFile);
                }
                //                if (File.Exists (networkDataPath)) {
                //                    File.Delete (networkDataPath);
                //                }



                VacationCorrectionParams vacationParams = new VacationCorrectionParams();
                vacationParams.FlightRouteDetails = flightPlan.FlightRoutes.Join(flightPlan.FlightDetails, fr => fr.FlightId, f => f.FlightId,
                    (fr, f) =>
                           new FlightRouteDetails
                           {
                               Flight = f.FlightId,
                               FlightDate = fr.FlightDate,
                               Orig = f.Orig,
                               Dest = f.Dest,
                               Cdep = f.Cdep,
                               Carr = f.Carr,
                               Ldep = f.Ldep,
                               Larr = f.Larr,
                               RouteNum = fr.RouteNum,

                           }).ToList();






                vacationParams.CurrentBidDetails = GlobalSettings.CurrentBidDetails;
                vacationParams.Trips = GlobalSettings.Trip.ToDictionary(x => x.TripNum, x => x);
                vacationParams.Lines = GlobalSettings.Lines.ToDictionary(x => x.LineNum.ToString(), x => x);
                vacationParams.IsEOM = true;
                //  VacationData = new Dictionary<string, TripMultiVacData>();


                //Performing vacation correction algoritham
                VacationCorrectionBL vacationBL = new VacationCorrectionBL();
                GlobalSettings.VacationData = vacationBL.PerformVacationCorrection(vacationParams);


                if (GlobalSettings.VacationData != null)
                {

                    string fileToSave = string.Empty;
                    fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails();


                    // save the VAC file to app data folder

                    var stream = File.Create(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".VAC");
                    ProtoSerailizer.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".VAC", GlobalSettings.VacationData, stream);
                    stream.Dispose();
                    stream.Close();
                }
                else
                {
                    GlobalSettings.IsVacationCorrection = false;
                }
                GlobalSettings.OrderedVacationDays = null;

            }
            catch (Exception ex)
            {
                GlobalSettings.OrderedVacationDays = null;

                throw ex;
            }
        }



        public static DateTime GetnextSunday()
        {
            DateTime date = GlobalSettings.CurrentBidDetails.BidPeriodEndDate;
            for (int count = 1; count <= 3; count++)
            {
                date = date.AddDays(1);
                if (date.DayOfWeek.ToString() == "Sunday")
                    break;
            }


            return date;
        }

        private void CreateEOMforFA()
        {
            if (GlobalSettings.FAEOMStartDate != null && GlobalSettings.FAEOMStartDate != DateTime.MinValue)
            {
                btnVacDrop.Enabled = true;
                string overlayTxt = string.Empty;
                if (GlobalSettings.MenuBarButtonStatus.IsEOM)
                    overlayTxt = "Applying EOM";
                else
                {
                    if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
                    {
                        btnVacDrop.Selected = false;
                        btnVacDrop.Enabled = false;
                        GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                    }
                    overlayTxt = "Removing EOM";
                }

                //RetrieveAndSaveVACLineFiles(3);
                LoadingOverlay overlay = new LoadingOverlay(this.View.Frame, overlayTxt);
                this.View.Add(overlay);
                InvokeInBackground(() =>
                {

                    var status = WBidHelper.RetrieveSaveAndSetLineFiles(3, wBIdStateContent);
                    //Old Code
                    /*
                    CreateEOMVacforFA ();
                    GenerateVacationDataView ();
                    */
                    if (status == "Ok")
                    {
                        StateManagement statemanagement = new StateManagement();
                        WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                        statemanagement.ApplyCSW(wBidStateContent);
                    }
                    InvokeOnMainThread(() =>
                    {
                        if (status == "Ok")
                        {
                            loadSummaryListAndHeader();
                            overlay.Hide();
                            GlobalSettings.isModified = true;
                            CommonClass.lineVC.UpdateSaveButton();
                        }
                        else
                        {
                            overlay.Hide();
                            UIAlertController okAlertController = UIAlertController.Create("WBidMax", status, UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                            btnEOM.Selected = false;
                            GlobalSettings.MenuBarButtonStatus.IsEOM = false;
                            UnselectEOM();
                        }
                    });





                });
            }
        }

        void handleEOMOptions(object sender, UIButtonEventArgs e)
        {
            if (e.ButtonIndex == 0)
            {
                btnEOM.Selected = true;
                GlobalSettings.FAEOMStartDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1);
            }
            else if (e.ButtonIndex == 1)
            {
                btnEOM.Selected = true;
                GlobalSettings.FAEOMStartDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(2);

            }
            else if (e.ButtonIndex == 2)
            {
                btnEOM.Selected = true;
                GlobalSettings.FAEOMStartDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(3);

            }
            else
            {
                btnEOM.Selected = false;
                GlobalSettings.MenuBarButtonStatus.IsEOM = false;
                if (!btnEOM.Selected && !btnVacCorrect.Selected)
                {
                    btnVacDrop.Enabled = false;
                    GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                }

            }
            WBidCollection.GenarateTempAbsenceList();

            //            else if (e.ButtonIndex == 3)
            //            {
            //                btnEOM.Selected = true;
            //                GlobalSettings.FAEOMStartDate = DateTime.MinValue;
            //
            //            }

            CreateEOMforFA();
        }

        private void CreateEOMVacforFA()
        {
            VacationCorrectionParams vacationParams = new VacationCorrectionParams();
            vacationParams.CurrentBidDetails = GlobalSettings.CurrentBidDetails;
            vacationParams.Trips = GlobalSettings.Trip.ToDictionary(x => x.TripNum, x => x);
            vacationParams.Lines = GlobalSettings.Lines.ToDictionary(x => x.LineNum.ToString(), x => x);
            Dictionary<string, TripMultiVacData> allTripsMultiVacData = null;

            string currentBidName = WBidHelper.GenerateFileNameUsingCurrentBidDetails();


            string vACFileName = WBidHelper.GetAppDataPath() + "//" + currentBidName + ".VAC";
            //Cheks the VAC file exists
            bool vacFileExists = File.Exists(vACFileName);

            if (!vacFileExists)
            {
                allTripsMultiVacData = new Dictionary<string, TripMultiVacData>();
            }
            else
            {

                using (FileStream vacstream = File.OpenRead(vACFileName))
                {

                    Dictionary<string, TripMultiVacData> objineinfo = new Dictionary<string, TripMultiVacData>();
                    allTripsMultiVacData = ProtoSerailizer.DeSerializeObject(vACFileName, objineinfo, vacstream);

                }
            }



            //Performing vacation correction algoritham
            VacationCorrectionBL vacationBL = new VacationCorrectionBL();
            GlobalSettings.VacationData = vacationBL.CreateVACfileForEOMFA(vacationParams, allTripsMultiVacData);



            string fileToSave = string.Empty;
            fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails();
            if (GlobalSettings.VacationData != null && GlobalSettings.VacationData.Count > 0)
            {




                // save the VAC file to app data folder

                var stream = File.Create(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".VAC");
                ProtoSerailizer.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".VAC", GlobalSettings.VacationData, stream);
                stream.Dispose();
                stream.Close();


                CaculateVacationDetails calVacationdetails = new CaculateVacationDetails();
                calVacationdetails.CalculateVacationdetailsFromVACfile(vacationParams.Lines, GlobalSettings.VacationData);

                //set the Vacpay,Vdrop,Vofont and VoBack columns in the line summary view 
                ManageVacationColumns managevacationcolumns = new ManageVacationColumns();
                managevacationcolumns.SetVacationColumns();

                LineInfo lineInfo = new LineInfo()
                {
                    LineVersion = GlobalSettings.LineVersion,
                    Lines = vacationParams.Lines

                };




                GlobalSettings.Lines = new System.Collections.ObjectModel.ObservableCollection<Line>(vacationParams.Lines.Select(x => x.Value));


                try
                {
                    var linestream = File.Create(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBL");
                    ProtoSerailizer.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBL", lineInfo, linestream);
                    linestream.Dispose();
                    linestream.Close();
                }
                catch (Exception ex)
                {

                    throw;
                }


                foreach (Line line in GlobalSettings.Lines)
                {
                    if (line.ConstraintPoints == null)
                        line.ConstraintPoints = new ConstraintPoints();
                    if (line.WeightPoints == null)
                        line.WeightPoints = new WeightPoints();
                }

            }






        }

        partial void btnResetTapped(UIKit.UIButton sender)
        {
            UIActionSheet sheet = new UIActionSheet("This action will remove all top locks, bottom locks, constraints, weights, and set the sort to Line Number.  Do you want to continue?", null, null, "YES", null);
            CGRect senderframe = sender.Frame;
            senderframe.X = sender.Frame.GetMidX();
            sheet.ShowFrom(senderframe, this.tbTopBar, true);
            sheet.Clicked += HandleResetAll;
        }

        void HandleResetAll(object sender, UIButtonEventArgs e)
        {
            if (e.ButtonIndex == 0)
            {
                StateManagement stateManagement = new StateManagement();
                stateManagement.UpdateWBidStateContent();
                WBidHelper.PushToUndoStack();

                LineOperations.RemoveAllTopLock();
                LineOperations.RemoveAllBottomLock();
                CommonClass.selectedRows.Clear();
                this.vwCalPopover.Hidden = true;
                this.vwTripPopover.Hidden = true;
                var wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                wBIdStateContent.SortDetails.SortColumn = "Line";
                CommonClass.columnID = 0;

                constCalc.ClearConstraints();
                ConstraintsApplied.clearAll();

                weightCalc.ClearWeights();
                WeightsApplied.clearAll();

                sort.SortLines("Line");
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

                //                if (BidAutomatorViewModelInstance != null)
                //                {
                //                    BidAutomatorViewModelInstance.ResetBidAutomatorDetails();
                //                }


                NSString str = new NSString("none");
                NSNotificationCenter.DefaultCenter.PostNotificationName("ButtonEnableDisable", str);
                NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                //NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);
                GlobalSettings.isModified = true;
                CommonClass.lineVC.UpdateSaveButton();
            }
        }

        partial void btnCSWTap(UIKit.UIButton sender)
        {
            CSWViewController cswController = new CSWViewController();
            CommonClass.cswVC = cswController;
            UINavigationController navController = new UINavigationController(cswController);
            navController.NavigationBar.BarStyle = UIBarStyle.Black;
            navController.NavigationBar.Hidden = true;
            navController.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            navController.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
            this.PresentModalViewController(navController, true);
        }

        partial void BidAutomatorButtonClicked(NSObject sender)
        {
            //BidAutomator button clicked
            //Write code for reseting CSW
            var wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            if (wBIdStateContent != null && GlobalSettings.CurrentBidDetails != null)
            {
                //var anyitemInCSW = CheckIfAnyItemSetInCsw(wBIdStateContent);
                if (CheckIfAnyItemSetInCsw(wBIdStateContent))
                {
                    UIAlertController alert = UIAlertController.Create("Confirmation", "The Constraints, Top lock, Bottom lock etc from the CSW view will Reset. Do you want to continue to open Bid Automator ?", UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (actionCancel) =>
                    {

                    }));

                    alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) =>
                    {
                        StateManagement stateManagement = new StateManagement();
                        stateManagement.UpdateWBidStateContent();
                        WBidHelper.PushToUndoStack();

                        LineOperations.RemoveAllTopLock();
                        LineOperations.RemoveAllBottomLock();
                        CommonClass.selectedRows.Clear();
                        this.vwCalPopover.Hidden = true;
                        this.vwTripPopover.Hidden = true;
                        wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                        wBIdStateContent.SortDetails.SortColumn = "Line";
                        CommonClass.columnID = 0;

                        constCalc.ClearConstraints();
                        ConstraintsApplied.clearAll();

                        //                  weightCalc.ClearWeights ();
                        //                  WeightsApplied.clearAll ();

                        sort.SortLines("Line");

                        NSString str = new NSString("none");
                        NSNotificationCenter.DefaultCenter.PostNotificationName("ButtonEnableDisable", str);
                        NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                        //NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);
                        GlobalSettings.isModified = true;
                        CommonClass.lineVC.UpdateSaveButton();


                        NavigateToBA();
                    }));

                    this.PresentViewController(alert, true, null);

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
            if (wBIdStateContent.Constraints.ETOPS)
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
            else if (wBIdStateContent.CxWtState.MixedHardReserveTrip.Cx)
                return true;
            //            //weights
            //
            //            else if (wBIdStateContent.CxWtState.ACChg.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.AMPM.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.BDO.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.BulkOC.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.CL.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.DHD.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.DHDFoL.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.DOW.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.DP.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.EQUIP.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.FLTMIN.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.GRD.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.InterConus.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.LEGS.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.LegsPerPairing.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.LrgBlkDaysOff.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.MP.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.No3on3off.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.NODO.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.NOL.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.NormalizeDays.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.PDAfter.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.PDBefore.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.PerDiem.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.Position.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.Rest.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.RON.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.SDO.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.SDOW.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.TL.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.TripLength.FourDay)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.TripLength.ThreeDay)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.TripLength.Twoday)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.TripLength.Turns)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.WB.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.WorkDay.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.WtPDOFS.Wt)
            //                return true;
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
            BAViewController baController = new BAViewController();
            CommonClass.BAVC = baController;
            UINavigationController navController = new UINavigationController(baController);
            navController.NavigationBar.BarStyle = UIBarStyle.Black;
            navController.NavigationBar.Hidden = true;
            navController.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            navController.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
            this.PresentViewController(navController, true, null);
        }
        partial void btnGridTap(UIKit.UIButton sender)
        {

            sender.Selected = !sender.Selected;
            CommonClass.showGrid = sender.Selected;
            //NSNotificationCenter.DefaultCenter.PostNotificationName("ReloadTableview", null);
            NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);
        }

        partial void btnPromoteTap(UIKit.UIButton sender)
        {
            StateManagement stateManagement = new StateManagement();
            stateManagement.UpdateWBidStateContent();
            WBidHelper.PushToUndoStack();

            LineOperations.PromoteLines(CommonClass.selectedRows);
            CommonClass.selectedRows.Clear();
            NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
            NSString str = new NSString("none");
            NSNotificationCenter.DefaultCenter.PostNotificationName("ButtonEnableDisable", str);
            GlobalSettings.isModified = true;
            CommonClass.lineVC.UpdateSaveButton();
        }

        partial void btnSaveTap(UIKit.UIButton sender)
        {
            StateManagement stateManagement = new StateManagement();
            stateManagement.UpdateWBidStateContent();
            //            CompareState stateObj = new CompareState();
            //            string fileName = WBidHelper.GenerateFileNameUsingCurrentBidDetails();
            //             var WbidCollection = XmlHelper.ReadStateFile(WBidHelper.GetAppDataPath() + "/" + fileName + ".WBS");
            //             wBIdStateContent = WbidCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            //            bool isNochange = stateObj.CompareStateChange(wBIdStateContent, GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName));


            if (GlobalSettings.isModified)
            {
                GlobalSettings.WBidStateCollection.IsModified = true;
                WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);

                if (timer != null)
                {
                    timer.Stop();
                    timer.Start();
                }
                //save the state of the INI File
                WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());

                GlobalSettings.isModified = false;
                btnSave.Enabled = false;
            }

        }

        partial void btnTrashTap(UIKit.UIButton sender)
        {
            StateManagement stateManagement = new StateManagement();
            stateManagement.UpdateWBidStateContent();
            WBidHelper.PushToUndoStack();

            LineOperations.TrashLines(CommonClass.selectedRows);
            CommonClass.selectedRows.Clear();
            NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
            NSString str = new NSString("none");
            NSNotificationCenter.DefaultCenter.PostNotificationName("ButtonEnableDisable", str);
            GlobalSettings.isModified = true;
            CommonClass.lineVC.UpdateSaveButton();
        }

        partial void btnRemTopLockTap(UIKit.UIButton sender)
        {
            UIActionSheet sheet = new UIActionSheet("This action will remove all top locks, and change the sort to “Manual”.  Do you want to continue?", null, null, "YES", null);
            CGRect senderframe = sender.Frame;
            senderframe.X = sender.Frame.GetMidX();
            sheet.ShowFrom(senderframe, this.tbTopBar, true);
            sheet.Clicked += HandleRemoveToplock;
        }

        void HandleRemoveToplock(object sender, UIButtonEventArgs e)
        {
            if (e.ButtonIndex == 0)
            {
                StateManagement stateManagement = new StateManagement();
                stateManagement.UpdateWBidStateContent();
                WBidHelper.PushToUndoStack();

                LineOperations.RemoveAllTopLock();
                CommonClass.selectedRows.Clear();
                NSString str = new NSString("none");
                NSNotificationCenter.DefaultCenter.PostNotificationName("ButtonEnableDisable", str);
                this.vwCalPopover.Hidden = true;
                this.vwTripPopover.Hidden = true;
                var wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                wBIdStateContent.SortDetails.SortColumn = "Manual";
                CommonClass.columnID = 0;
                NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                //NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);
                GlobalSettings.isModified = true;
                CommonClass.lineVC.UpdateSaveButton();
            }
        }

        partial void btnRemBottomLockTap(UIKit.UIButton sender)
        {
            UIActionSheet sheet = new UIActionSheet("This action will remove all bottom locks, and change the sort to “Manual”.  Do you want to continue?", null, null, "YES", null);
            CGRect senderframe = sender.Frame;
            senderframe.X = sender.Frame.GetMidX();
            sheet.ShowFrom(senderframe, this.tbTopBar, true);
            sheet.Clicked += HandleRemoveBottomlock;
        }

        void HandleRemoveBottomlock(object sender, UIButtonEventArgs e)
        {
            if (e.ButtonIndex == 0)
            {
                StateManagement stateManagement = new StateManagement();
                stateManagement.UpdateWBidStateContent();
                WBidHelper.PushToUndoStack();

                LineOperations.RemoveAllBottomLock();
                CommonClass.selectedRows.Clear();
                NSString str = new NSString("none");
                NSNotificationCenter.DefaultCenter.PostNotificationName("ButtonEnableDisable", str);
                btnRemBottomLock.Enabled = false;
                this.vwCalPopover.Hidden = true;
                this.vwTripPopover.Hidden = true;
                var wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                wBIdStateContent.SortDetails.SortColumn = "Manual";
                CommonClass.columnID = 0;
                NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                //NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);
                GlobalSettings.isModified = true;
                CommonClass.lineVC.UpdateSaveButton();
            }
        }

        partial void btnMiscTap(UIKit.UIButton sender)
        {
            string[] arr = { "Configuration", "Change User Information", "Latest News", "My Subscription" };
            UIActionSheet sheet = new UIActionSheet("Select", null, "Cancel", null, arr);
            CGRect senderframe = sender.Frame;
            senderframe.X = sender.Frame.GetMidX();

            sheet.ShowFrom(senderframe, this.tbTopBar, true);
            sheet.Dismissed += handleMIscTap;
        }

        public void handleMIscTap(object sender, UIButtonEventArgs e)
        {
            if (e.ButtonIndex == 0)
            {
                ConfigTabBarControlller config = new ConfigTabBarControlller(false);
                config.ModalPresentationStyle = UIModalPresentationStyle.PageSheet;
                this.PresentViewController(config, true, null);
            }
            else if (e.ButtonIndex == 1)
            {
                userRegistrationViewController regs = new userRegistrationViewController();
                regs.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                this.PresentViewController(regs, true, null);
            }
            else if (e.ButtonIndex == 2)
            {
                if (File.Exists(WBidHelper.GetAppDataPath() + "/" + "news.pdf"))
                {
                    webPrint fileViewer = new webPrint();
                    fileViewer.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    this.PresentViewController(fileViewer, true, () =>
                    {
                        fileViewer.LoadPDFdocument("news.pdf");
                    });
                }
                else
                {
                    UIAlertController okAlertController = UIAlertController.Create("WBidMax", "No latest News found!", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                }
            }
            else if (e.ButtonIndex == 3)
            {
                SubscriptionViewController ObjSubscription = new SubscriptionViewController();
                ObjSubscription.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                this.PresentViewController(ObjSubscription, true, null);

            }
        }

        partial void btnHelpTap(UIKit.UIButton sender)
        {
            string[] arr = { "Help", "Walkthrough", "Contact Us", "About" };
            UIActionSheet sheet = new UIActionSheet("Select", null, "Cancel", null, arr);
            CGRect senderframe = sender.Frame;
            senderframe.X = sender.Frame.GetMidX();
            sheet.ShowFrom(senderframe, this.tbTopBar, true);
            sheet.Dismissed += handleHelp;
        }

        void handleHelp(object sender, UIButtonEventArgs e)
        {
            if (e.ButtonIndex == 0)
            {
                HelpViewController helpVC = new HelpViewController();
                helpVC.pdfFileName = "Constraints";
                UINavigationController navCont = new UINavigationController(helpVC);
                navCont.NavigationBar.BarStyle = UIBarStyle.Black;
                navCont.NavigationBar.Hidden = true;
                navCont.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                this.PresentViewController(navCont, true, null);
            }
            else if (e.ButtonIndex == 1)
            {
                WalkthroughViewController introV = new WalkthroughViewController();
                introV.home = new homeViewController();
                introV.ModalPresentationStyle = UIModalPresentationStyle.PageSheet;
                this.PresentViewController(introV, true, null);
            }
            else if (e.ButtonIndex == 2)
            {
                ContactUsViewController contactVC = new ContactUsViewController();
                contactVC.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                this.PresentViewController(contactVC, true, null);
            }
            else if (e.ButtonIndex == 3)
            {
                AboutViewController aboutVC = new AboutViewController();
                aboutVC.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                this.PresentViewController(aboutVC, true, null);
            }
        }

        partial void btnBrightnessTapped(UIKit.UIButton sender)
        {
            BrightnessViewController popoverContent = new BrightnessViewController();
            popoverController = new UIPopoverController(popoverContent);
            popoverController.Delegate = new MyPopDelegate(this);
            popoverController.PopoverContentSize = new CGSize(300, 100);
            CGRect senderframe = btnBrightness.Superview.Frame;
            senderframe.X = btnBrightness.Superview.Frame.GetMidX();
            popoverController.PresentFromRect(senderframe, tbTopBar, UIPopoverArrowDirection.Any, true);

        }

        partial void btnBidStuffTap(UIKit.UIButton sender)
        {

            //string[] arr=new string[12];
            if (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO")
            {
                Console.WriteLine(sender.TitleLabel.Text);
                string[] arr = new string[]{
                    "Submit Current Bid Order",
                    "Bid Editor",
                    "View Cover Letter",
                    "View Seniority List",
                    "View Awards",
                    "Get Awards",
                    "Show Bid Reciept",
                    "Print Bid Reciept",
                    //"Redownload Vacation",
                    "Redownload Bid Data",
                    "Show CAP"

                };
                UIActionSheet sheet = new UIActionSheet("Select", null, "Cancel", null, arr);
                CGRect senderframe = sender.Frame;
                senderframe.X = sender.Frame.GetMidX();
                sheet.ShowFrom(senderframe, this.tbTopBar, true);
                sheet.Dismissed += handleBidStuffTap;
            }
            else
            {
                string[] arr = new string[]{
                    "Submit Current Bid Order",
                    "Bid Editor",
                    "View Cover Letter",
                    "View Seniority List",
                    "View Awards",
                    "Get Awards",
                    "Show Bid Reciept",
                    "Print Bid Reciept",
                    //"Redownload Vacation",
                    "Redownload Bid Data",
                };
                UIActionSheet sheet = new UIActionSheet("Select", null, "Cancel", null, arr);
                CGRect senderframe = sender.Frame;
                senderframe.X = sender.Frame.GetMidX();
                sheet.ShowFrom(senderframe, this.tbTopBar, true);
                sheet.Dismissed += handleBidStuffTap;
            }

        }
        private void SouthWestWifiAlert()
        {
            if (WBidHelper.IsSouthWestWifiOr2wire() == false)
            {
                if (Reachability.CheckVPSAvailable())
                {



                }
                else
                {
                    if (WBidHelper.IsSouthWestWifiOr2wire())
                    {
                        UIAlertController okAlertController = UIAlertController.Create("WBidMax", Constants.SouthWestConnectionAlert, UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);


                    }
                    else
                    {

                        UIAlertController okAlertController = UIAlertController.Create("WBidMax", Constants.VPSDownAlert, UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);

                    }
                }
            }
            else
            {
                UIAlertController okAlertController = UIAlertController.Create("WBidMax", Constants.SouthWestConnectionAlert, UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
            }

        }
        public void handleBidStuffTap(object sender, UIButtonEventArgs e)
        {

            if (e.ButtonIndex == 0)
            {
                if (GlobalSettings.WBidStateCollection.DataSource != "HistoricalData")
                {


                    //check blank lines are in low to high order. alert if it not satisfy the condtion
                    List<int> sortedblanklines = GlobalSettings.Lines.Where(x => x.BlankLine).Select(y => y.LineNum).OrderBy(z => z).ToList();
                    List<int> currentblanklines = GlobalSettings.Lines.Where(x => x.BlankLine).Select(y => y.LineNum).ToList();
                    bool isBlankLinesCorrectOrder = true;
                    for (int i = 0; i < sortedblanklines.Count; i++)
                    {
                        if (sortedblanklines[i] != currentblanklines[i])
                        {
                            isBlankLinesCorrectOrder = false;
                            break;
                        }

                    }
                    //if the user is connected to south west wifi, show an alert that user is having limited internet connection
                    SouthWestWifiAlert();

                    if (!isBlankLinesCorrectOrder)
                    {

                        UIAlertController alert = UIAlertController.Create("WbidMax", "Your blank lines are not in order of lowest to highest. Touch Cancel to go back and fix this issue.?", UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (actionCancel) =>
                        {

                        }));

                        alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) =>
                        {
                            SubmitBidViewController submitBid = new SubmitBidViewController();
                            submitBid.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                            UINavigationController nav = new UINavigationController(submitBid);
                            nav.NavigationBar.BarTintColor = ColorClass.TopHeaderColor;
                            nav.NavigationBar.TitleTextAttributes = new UIStringAttributes() { ForegroundColor = UIColor.White };
                            nav.NavigationBar.TintColor = UIColor.White;
                            nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                            nav.InteractivePopGestureRecognizer.Enabled = false;
                            this.PresentViewController(nav, true, null);
                        }));

                        this.PresentViewController(alert, true, null);

                    }
                    else
                    {
                        //Bid view should come up here
                        SubmitBidViewController submitBid = new SubmitBidViewController();
                        submitBid.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                        UINavigationController nav = new UINavigationController(submitBid);
                        nav.NavigationBar.BarTintColor = ColorClass.TopHeaderColor;
                        nav.NavigationBar.TitleTextAttributes = new UIStringAttributes() { ForegroundColor = UIColor.White };
                        nav.NavigationBar.TintColor = UIColor.White;
                        nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;


                        this.PresentViewController(nav, false, null);
                    }
                }
                else
                {
                    UIAlertController okAlertController = UIAlertController.Create("WBidMax", "You can not submit bids for Historical data", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                }
            }
            else if (e.ButtonIndex == 1)
            {




                if (GlobalSettings.WBidStateCollection.DataSource != "HistoricalData")
                {


                    //check blank lines are in low to high order. alert if it not satisfy the condtion
                    List<int> sortedblanklines = GlobalSettings.Lines.Where(x => x.BlankLine).Select(y => y.LineNum).OrderBy(z => z).ToList();
                    List<int> currentblanklines = GlobalSettings.Lines.Where(x => x.BlankLine).Select(y => y.LineNum).ToList();
                    bool isBlankLinesCorrectOrder = true;
                    for (int i = 0; i < sortedblanklines.Count; i++)
                    {
                        if (sortedblanklines[i] != currentblanklines[i])
                        {
                            isBlankLinesCorrectOrder = false;
                            break;
                        }

                    }
                    //if the user is connected to south west wifi, show an alert that user is having limited internet connection
                    SouthWestWifiAlert();
                    if (!isBlankLinesCorrectOrder)
                    {
                        UIAlertController alert = UIAlertController.Create("WbidMax", "Your blank lines are not in order of lowest to highest. Touch Cancel to go back and fix this issue.?", UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (actionCancel) =>
                        {

                        }));

                        alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) =>
                        {
                            bidEditorPrepViewController bidEditor = new bidEditorPrepViewController();
                            //bidEditor.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                            UINavigationController nav = new UINavigationController(bidEditor);
                            nav.NavigationBarHidden = true;
                            nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                            this.PresentViewController(nav, true, () =>
                            {
                                notif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("bidEditorPrepared"), bidEditorPrepared);


                            });
                        }));

                        this.PresentViewController(alert, true, null);

                    }
                    else
                    {
                        bidEditorPrepViewController bidEditor = new bidEditorPrepViewController();
                        bidEditor.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                        UINavigationController nav = new UINavigationController(bidEditor);
                        nav.NavigationBarHidden = true;
                        nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                        this.PresentViewController(nav, true, () =>
                        {
                            notif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("bidEditorPrepared"), bidEditorPrepared);
                        });



                    }

                }
                else
                {
                    UIAlertController okAlertController = UIAlertController.Create("WBidMax", "You can not submit bids for Historical data", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                }
            }
            else if (e.ButtonIndex == 2)
            {
                //cover
                viewFileViewController viewFile = new viewFileViewController();
                UINavigationController nav = new UINavigationController(viewFile);
                nav.NavigationBarHidden = true;
                nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                viewFile.displayType = "cover";
                this.PresentViewController(nav, true, () =>
                {
                });
            }
            else if (e.ButtonIndex == 3)
            {
                //seniority
                viewFileViewController viewFile = new viewFileViewController();
                //viewFile.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                UINavigationController nav = new UINavigationController(viewFile);
                nav.NavigationBarHidden = true;
                nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                viewFile.displayType = "seniority";
                this.PresentViewController(nav, true, () =>
                {
                });
            }
            else if (e.ButtonIndex == 4)
            {
                //Awards
                viewFileViewController viewFile = new viewFileViewController();
                //viewFile.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                UINavigationController nav = new UINavigationController(viewFile);
                nav.NavigationBarHidden = true;
                nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                viewFile.displayType = "awards";
                this.PresentViewController(nav, true, () =>
                {
                });
            }
            else if (e.ButtonIndex == 5)
            {
                RetrieveAwardViewController award = new RetrieveAwardViewController();
                award.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                UINavigationController nav = new UINavigationController(award);
                nav.NavigationBarHidden = true;
                nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                this.PresentViewController(nav, true, null);
            }
            else if (e.ButtonIndex == 6)
            {
                string path = WBidHelper.GetAppDataPath();
                List<string> linefilenames = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).Select(Path.GetFileName).Where(s => s.ToLower().EndsWith(".rct") || s.ToLower().EndsWith(".pdf")).ToList();
                linefilenames.Remove("news.pdf");
                //List<string> linefilenames = Directory.EnumerateFiles (path, "*.RCT", SearchOption.AllDirectories).Select (Path.GetFileName).ToList ();
                if (linefilenames.Count > 1)
                {
                    BidRecieptViewController reciept = new BidRecieptViewController();
                    reciept.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                    reciept.isPrintView = false;
                    this.PresentViewController(reciept, true, null);
                }
                else if (linefilenames.Count == 1)
                {
                    InvokeOnMainThread(() =>
                    {
                        webPrint fileViewer = new webPrint();

                        this.PresentViewController(fileViewer, true, () =>
                            {
                                if (Path.GetExtension(linefilenames[0]).ToLower() == ".rct")
                                {
                                    fileViewer.loadFileFromUrl(linefilenames[0]);
                                }
                                else
                                    fileViewer.LoadPDFdocument(Path.GetFileName(linefilenames[0]));
                            });

                    });
                }
                else if (linefilenames.Count == 0)
                {
                    UIAlertController okAlertController = UIAlertController.Create("WBidMax", "There is no bid reciept available..!", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                }
            }
            else if (e.ButtonIndex == 7)
            {
                string path = WBidHelper.GetAppDataPath();
                List<string> linefilenames = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).Select(Path.GetFileName).Where(s => s.ToLower().EndsWith(".rct") || s.ToLower().EndsWith(".pdf")).ToList();
                linefilenames.Remove("news.pdf");
                //List<string> linefilenames = Directory.EnumerateFiles (path, "*.RCT", SearchOption.AllDirectories).Select (Path.GetFileName).ToList ();
                if (linefilenames.Count > 1)
                {
                    BidRecieptViewController reciept = new BidRecieptViewController();
                    reciept.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                    reciept.isPrintView = true;
                    this.PresentViewController(reciept, true, null);
                }
                else if (linefilenames.Count == 1)
                {
                    InvokeOnMainThread(() =>
                    {
                        CommonClass.PrintReceipt((UIView)btnBidStuff, linefilenames[0]);
                    });
                }
                else if (linefilenames.Count == 0)
                {
                    UIAlertController okAlertController = UIAlertController.Create("WBidMax", "There is no bid reciept available..!", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                }
            }
            //else if (e.ButtonIndex == 8)
            //{
            //    //Redownload vacation
            //    string status = string.Empty;
            //    string overlayTxt = string.Empty;
            //    WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

            //    if (GlobalSettings.MenuBarButtonStatus.IsEOM || GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
            //    {
            //        overlayTxt = "Redownloading vacation...";
            //        LoadingOverlay overlay = new LoadingOverlay(this.View.Frame, overlayTxt);
            //        this.View.Add(overlay);
            //        InvokeInBackground(() =>
            //        {
            //            status = WBidHelper.RedownloadBidDataVacFileFromServer(wBidStateContent);
            //            if (status == "Ok")
            //            {
            //                StateManagement statemanagement = new StateManagement();

            //                statemanagement.ApplyCSW(wBidStateContent);

            //                if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
            //                {
            //                    RecalcalculateLineProperties objrecalculate = new RecalcalculateLineProperties();
            //                    objrecalculate.CalculateDropTemplateForBidLines(GlobalSettings.Lines);
            //                }
            //                InvokeOnMainThread(() =>
            //                {
            //                    WBidHelper.RecalculateAMPMAndWekProperties(false);
            //                    loadSummaryListAndHeader();
            //                    overlay.Hide();
            //                    GlobalSettings.isModified = true;
            //                    CommonClass.lineVC.UpdateSaveButton();
            //                });
            //            }
            //            else
            //            {

            //                InvokeOnMainThread(() =>
            //                {
            //                    overlay.Hide();
            //                    UIAlertController okAlertController = UIAlertController.Create("WBidMax", status, UIAlertControllerStyle.Alert);
            //                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            //                    this.PresentViewController(okAlertController, true, null);
            //                    GlobalSettings.MenuBarButtonStatus.IsEOM = false;
            //                    UnselectEOM();
            //                });
            //            }
            //        });

            //    }
            //    else
            //    {
            //        status = "You have no vacation.. Please check the senioity list!!!";
            //        InvokeOnMainThread(() =>
            //        {
            //            UIAlertController okAlertController = UIAlertController.Create("WBidMax", status, UIAlertControllerStyle.Alert);
            //            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            //            this.PresentViewController(okAlertController, true, null);
            //            GlobalSettings.MenuBarButtonStatus.IsEOM = false;
            //            UnselectEOM();
            //        });
            //    }
            //}
            else if (e.ButtonIndex == 8)
            {
                //Redownload Bid Data
                string appfolderpath = WBidHelper.GetAppDataPath();
                string status = string.Empty;
                string overlayTxt = string.Empty;
                WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

                overlayTxt = "Redownloading Bid Data...";
                LoadingOverlay overlay = new LoadingOverlay(this.View.Frame, overlayTxt);
                this.View.Add(overlay);
                InvokeInBackground(() =>
                {
                    BidDataFileResponse bidDataResponse = new BidDataFileResponse();
                    if (!GlobalSettings.isHistorical)
                    {
                        bidDataResponse = WBidHelper.RedownloadBidDataFromServer();
                        if (bidDataResponse.bidData != null  && bidDataResponse.Status == true)
                        {
                            StateManagement statemanagement = new StateManagement();

                            statemanagement.ApplyCSW(wBidStateContent);
                            InvokeOnMainThread(() =>
                            {
                                overlay.Hide();
                                NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                                GlobalSettings.isModified = true;
                                CommonClass.lineVC.UpdateSaveButton();
                                UIAlertController okAlertController = UIAlertController.Create("Success !", "Bid Data redownloaded. Please check.", UIAlertControllerStyle.Alert);
                                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                                this.PresentViewController(okAlertController, true, null);
                            });
                        }
                        else
                        {
                            string msg = (bidDataResponse != null && bidDataResponse.Message != null) ? bidDataResponse.Message : string.Empty;
                            if (msg == string.Empty)
                                msg = "Something went wrong. please try again or contact Administrator";
                            InvokeOnMainThread(() =>
                            {
                                overlay.Hide();
                                UIAlertController okAlertController = UIAlertController.Create("Error!", msg, UIAlertControllerStyle.Alert);
                                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                                this.PresentViewController(okAlertController, true, null);
                            });
                        }
                    }
                });
            }
            else if (e.ButtonIndex == 9)
            {
                if (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO")
                {
                    if (Reachability.CheckVPSAvailable())
                    {
                        CAPViewController capdetails = new CAPViewController();
                        capdetails.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                        UINavigationController nav = new UINavigationController(capdetails);
                        nav.NavigationBarHidden = true;
                        nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                        this.PresentViewController(nav, true, null);
                    }
                    else
                    {
                        if (WBidHelper.IsSouthWestWifiOr2wire())
                        {

                            UIAlertController okAlertController = UIAlertController.Create("WBidMax", Constants.SouthWestConnectionAlert, UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                        }
                        else
                        {
                            UIAlertController okAlertController = UIAlertController.Create("WBidMax", Constants.VPSDownAlert, UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                        }
                    }
                }
            }
            UIActionSheet obj = (UIActionSheet)sender;
            obj.Dispose();
        }

        public void bidEditorPrepared(NSNotification n)
        {
            string selectedPosition = n.Object.ToString();
            if (selectedPosition == "CP" || selectedPosition == "FO")
            {
                BidEditorForPilot pilotBid = new BidEditorForPilot();
                UINavigationController nav = new UINavigationController(pilotBid);
                // Edited by Francis 12/3/2020
                nav.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;

                nav.NavigationBar.BarStyle = UIBarStyle.Black;
                nav.NavigationBar.Hidden = true;
                this.PresentViewController(nav, true, null);
            }
            else if (selectedPosition == "FA")
            {
                if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                {
                    if (GlobalSettings.CurrentBidDetails.Round == "M")
                    {
                        BidEditorForFA faBid = new BidEditorForFA();
                        faBid.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;

                        UINavigationController nav = new UINavigationController(faBid);
                        nav.NavigationBar.Hidden = true;
                        nav.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;

                        this.PresentViewController(nav, true, null);
                    }
                    else if (GlobalSettings.CurrentBidDetails.Round == "S")
                    {
                        BidEditorForPilot pilotBid = new BidEditorForPilot();
                        UINavigationController nav = new UINavigationController(pilotBid);
                        nav.NavigationBar.BarStyle = UIBarStyle.Black;
                        nav.NavigationBar.Hidden = true;
                        nav.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                        this.PresentViewController(nav, true, null);
                    }

                }

            }
            NSNotificationCenter.DefaultCenter.RemoveObserver(notif);
        }

        partial void btnLineBotLockTap(UIKit.UIButton sender)
        {
            StateManagement stateManagement = new StateManagement();
            stateManagement.UpdateWBidStateContent();
            WBidHelper.PushToUndoStack();

            LineOperations.TrashLines(CommonClass.doubleTapLine);
            sumList.TableView.DeselectRow(lPath, true);
            NSString str = new NSString("none");
            NSNotificationCenter.DefaultCenter.PostNotificationName("ButtonEnableDisable", str);

            //            NSIndexPath path;
            //            if (lPath.Row != this.sumList.TableView.NumberOfRowsInSection(0) - 1)
            //                path = NSIndexPath.FromRowSection(lPath.Row, lPath.Section);
            //            else
            //                path = lPath;
            //            lPath = path;
            NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
            sumList.TableView.SelectRow(lPath, true, UITableViewScrollPosition.None);
            this.calPopover(lPath);

            CGPoint point = sumList.TableView.RectForRowAtIndexPath(lPath).Location;
            point = sumList.TableView.ConvertPointToView(point, this.vwTable);
            CGPoint offset = sumList.TableView.ContentOffset;
            if (point.Y > 580)
                sumList.TableView.ScrollToRow(NSIndexPath.FromRowSection(lPath.Row, lPath.Section), UITableViewScrollPosition.Bottom, true);

            GlobalSettings.isModified = true;
            CommonClass.lineVC.UpdateSaveButton();
        }

        partial void btnLineTopLockTap(UIKit.UIButton sender)
        {
            StateManagement stateManagement = new StateManagement();
            stateManagement.UpdateWBidStateContent();
            WBidHelper.PushToUndoStack();

            LineOperations.PromoteLines(CommonClass.doubleTapLine);
            sumList.TableView.DeselectRow(lPath, true);
            NSString str = new NSString("none");
            NSNotificationCenter.DefaultCenter.PostNotificationName("ButtonEnableDisable", str);

            NSIndexPath path;
            if (lPath.Row != this.sumList.TableView.NumberOfRowsInSection(0) - 1)
                path = NSIndexPath.FromRowSection(lPath.Row + 1, lPath.Section);
            else
                path = lPath;
            lPath = path;
            NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
            sumList.TableView.SelectRow(lPath, true, UITableViewScrollPosition.None);
            this.calPopover(lPath);

            CGPoint point = sumList.TableView.RectForRowAtIndexPath(lPath).Location;
            point = sumList.TableView.ConvertPointToView(point, this.vwTable);
            CGPoint offset = sumList.TableView.ContentOffset;
            if (point.Y > 580)
                sumList.TableView.ScrollToRow(NSIndexPath.FromRowSection(lPath.Row, lPath.Section), UITableViewScrollPosition.Bottom, true);

            GlobalSettings.isModified = true;
            CommonClass.lineVC.UpdateSaveButton();
        }

        partial void btnMovDownTap(UIKit.UIButton sender)
        {
            moveDown();
        }

        private void moveDown()
        {
            sumList.TableView.DeselectRow(lPath, true);
            NSIndexPath path = NSIndexPath.FromRowSection(lPath.Row + 1, lPath.Section);
            lPath = path;
            sumList.TableView.SelectRow(lPath, true, UITableViewScrollPosition.None);
            this.calPopover(lPath);

            CGPoint point = sumList.TableView.RectForRowAtIndexPath(lPath).Location;
            point = sumList.TableView.ConvertPointToView(point, sumList.TableView);
            CGPoint offset = sumList.TableView.ContentOffset;
            if (point.Y > 580)
                sumList.TableView.ScrollToRow(NSIndexPath.FromRowSection(lPath.Row, lPath.Section), UITableViewScrollPosition.Bottom, true);

        }

        partial void btnMovUpTap(UIKit.UIButton sender)
        {
            moveUp();
        }

        private void moveUp()
        {
            sumList.TableView.DeselectRow(lPath, true);
            NSIndexPath path = NSIndexPath.FromRowSection(lPath.Row - 1, lPath.Section);
            lPath = path;
            sumList.TableView.SelectRow(lPath, true, UITableViewScrollPosition.None);
            this.calPopover(lPath);

            CGPoint point = sumList.TableView.RectForRowAtIndexPath(lPath).Location;
            point = sumList.TableView.ConvertPointToView(point, sumList.TableView);
            CGPoint offset = sumList.TableView.ContentOffset;
            if (point.Y < 50)
                sumList.TableView.ScrollToRow(NSIndexPath.FromRowSection(lPath.Row, lPath.Section), UITableViewScrollPosition.Top, true);

        }

        /// <summary>
        /// PURPOSE : Toggles the Line view Type. 
        /// </summary>
        partial void sgControlViewTypeTap(UIKit.UISegmentedControl sender)
        {
            CommonClass.selectedTrip = null;
            // Segmented Control action for view change.
            if (sender.SelectedSegment == 0)
            {
                // Summary view.
                CommonClass.MainViewType = "Summary";
                GlobalSettings.WBidINIContent.ViewType = 1;
                this.loadSummaryListAndHeader();
            }
            else if (sender.SelectedSegment == 1)
            {
                // Bidline View.
                CommonClass.MainViewType = "Bidline";
                GlobalSettings.WBidINIContent.ViewType = 2;
                this.loadSummaryListAndHeader();
            }
            else if (sender.SelectedSegment == 2)
            {
                // Modern View.
                CommonClass.MainViewType = "Modern";
                GlobalSettings.WBidINIContent.ViewType = 3;
                this.loadSummaryListAndHeader();
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
        public int SynchQSVersion { get; set; }
        public DateTime ServerSynchTime { get; set; }

        private void Synch()
        {
            if (GlobalSettings.SynchEnable && GlobalSettings.WBidStateCollection != null && GlobalSettings.WBidINIContent != null && GlobalSettings.WBidINIContent.User.SmartSynch)
            {
                syncOverlay = new LoadingOverlay(View.Bounds, "Smart Synchronisation checking server version..\n Please wait..");
                View.Add(syncOverlay);

                BeginInvokeOnMainThread(() =>
                {
                    //SynchStateForApplicationLoad ();
                    SynchStateAndQQQuickSet();
                });
            }
        }






        private void SynchStateAndQQQuickSet()
        {
            try
            {
                // MessageBoxResult msgResult;
                bool isConnectionAvailable = Reachability.CheckVPSAvailable();

                if (isConnectionAvailable)
                {

                    IsSynchStart = true;
                    string stateFileName = WBidHelper.GenerateFileNameUsingCurrentBidDetails();
                    SynchStateVersion = int.Parse(GlobalSettings.WBidStateCollection.SyncVersion);
                    if (GlobalSettings.QuickSets == null)
                    {
                        if (File.Exists(WBidHelper.GetQuickSetFilePath()))
                        {
                            GlobalSettings.QuickSets = XmlHelper.DeserializeFromXml<QuickSets>(WBidHelper.GetQuickSetFilePath());
                        }
                        else
                        {
                            GlobalSettings.QuickSets = new QuickSets();
                            GlobalSettings.QuickSets.QuickSetColumn = new List<QuickSetColumn>();
                            GlobalSettings.QuickSets.QuickSetCSW = new List<QuickSetCSW>();
                        }

                    }
                    SynchQSVersion = (GlobalSettings.QuickSets != null && GlobalSettings.QuickSets.SyncQuickSetVersion != null) ? int.Parse(GlobalSettings.QuickSets.SyncQuickSetVersion) : 0;
                    //Get server State Version


                    WBGetStateDTO wbStateDTO = new WBGetStateDTO();
                    wbStateDTO.Employeeumber = int.Parse(GlobalSettings.WbidUserContent.UserInformation.EmpNo);
                    wbStateDTO.QuickSetFileName = GlobalSettings.WbidUserContent.UserInformation.EmpNo;
                    wbStateDTO.StateName = stateFileName;
                    wbStateDTO.Year = GlobalSettings.CurrentBidDetails.Year;
                    wbStateDTO.FileType = 2;


                    WBStateInfoDTO versionInfo = GetWBServerStateandquicksetVersionNumber(wbStateDTO);

                    //VersionInfo versionInfo = GetServerVersion(stateFileName);
                    syncOverlay.Hide();

                    if (versionInfo != null)
                    {
                        ServerSynchTime = DateTime.Parse(versionInfo.StateLastUpdatedDate, CultureInfo.InvariantCulture);

                        if (versionInfo.StateVersionNumber != string.Empty || versionInfo.QuickSetVersionNumber != string.Empty)
                        {
                            int serverVersion = Convert.ToInt32(versionInfo.StateVersionNumber);
                            int serverQSversion = Convert.ToInt32(versionInfo.QuickSetVersionNumber);
                            if (SynchStateVersion != serverVersion || GlobalSettings.WBidStateCollection.IsModified || SynchQSVersion != serverQSversion || GlobalSettings.QuickSets.IsModified)
                            {
                                SynchView synchConf = new SynchView();
                                synchConf.ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;


                                //pass  versionInfo 
                                string serverupdated = DateTime.Now.ToString();
                                string localupdated = DateTime.Now.AddDays(-1).ToString();

                                synchConf.serverupdated = serverupdated;
                                synchConf.localupdated = localupdated;
                                this.PresentViewController(synchConf, true, null);
                                int i = 0;

                                // SynchSelection Observer actions
                                synchConf.LocalStateSynchTime = GlobalSettings.WBidStateCollection.StateUpdatedTime;
                                synchConf.LocalQSSynchTime = (GlobalSettings.QuickSets == null) ? DateTime.MinValue : GlobalSettings.QuickSets.QuickSetUpdatedTime;


                                synchConf.ServerStateSynchTime = versionInfo.StateLastUpdate; ;
                                synchConf.ServerQSSynchTime = versionInfo.QuickSetLastUpdated;
                                synchSelectionNotif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("SynchSelectioNotif"), async (NSNotification notification) =>
                                 {


                                     string selectionString = notification.Object.ToString();
                                     NSNotificationCenter.DefaultCenter.RemoveObserver(synchSelectionNotif);

                                     // selectionString = 300 means selectionindex = 3,StateSegement = 0,QuickSegment = 0 ()
                                     // selectionString = 311 means selectionindex = 3,StateSegement = 1,QuickSegment = 1
                                     // selectionString = 301 means selectionindex = 3,StateSegement = 0,QuickSegment = 1
                                     // selectionString = 310 means selectionindex = 3,StateSegement = 1,QuickSegment = 0
                                     // selectionString = 100 means selectionindex = 1,StateSegement = 0
                                     // selectionString = 110 means selectionindex = 1,StateSegement = 1
                                     // selectionString = 200 means selectionindex = 2,QuickSegment = 0
                                     // selectionString = 201 means selectionindex = 2,QuickSegment = 1
                                     if (selectionString != "")
                                     {
                                         switch (selectionString)
                                         {
                                             case "100":
                                                 DataSynchSelecedValue = 1;
                                                 IsStateFromServer = false;
                                                 IskeepLocalState = true;
                                                 IsQSFromServer = false;
                                                 IskeepLocalQS = false;
                                                 break;
                                             case "110":
                                                 DataSynchSelecedValue = 1;
                                                 IsStateFromServer = true;
                                                 IskeepLocalState = false;
                                                 IsQSFromServer = false;
                                                 IskeepLocalQS = false;
                                                 break;
                                             case "200":
                                                 DataSynchSelecedValue = 2;
                                                 IsStateFromServer = false;
                                                 IskeepLocalState = false;
                                                 IsQSFromServer = false;
                                                 IskeepLocalQS = true;
                                                 break;
                                             case "201":
                                                 DataSynchSelecedValue = 2;
                                                 IsStateFromServer = false;
                                                 IskeepLocalState = false;
                                                 IsQSFromServer = true;
                                                 IskeepLocalQS = false;
                                                 break;
                                             case "300":
                                                 DataSynchSelecedValue = 3;
                                                 IsStateFromServer = false;
                                                 IskeepLocalState = true;
                                                 IsQSFromServer = false;
                                                 IskeepLocalQS = true;
                                                 break;
                                             case "301":
                                                 DataSynchSelecedValue = 3;
                                                 IsStateFromServer = false;
                                                 IskeepLocalState = true;
                                                 IsQSFromServer = true;
                                                 IskeepLocalQS = false;
                                                 break;
                                             case "310":
                                                 DataSynchSelecedValue = 3;
                                                 IsStateFromServer = true;
                                                 IskeepLocalState = false;
                                                 IsQSFromServer = false;
                                                 IskeepLocalQS = true;
                                                 break;
                                             case "311":
                                                 DataSynchSelecedValue = 3;
                                                 IsStateFromServer = true;
                                                 IskeepLocalState = false;
                                                 IsQSFromServer = true;
                                                 IskeepLocalQS = false;
                                                 break;
                                         }




                                         // Selected only for state file sync
                                         if (DataSynchSelecedValue == 1)
                                         {
                                             if (SynchStateVersion != serverVersion || GlobalSettings.WBidStateCollection.IsModified)
                                             {
                                                 wbStateDTO.FileType = 0;
                                                 if (IsStateFromServer) // Get Server state to local
                                                 {
                                                     FirstTime = true;
                                                     syncOverlay = new LoadingOverlay(View.Bounds, "Synching current State FROM server \n Please wait..");
                                                     View.Add(syncOverlay);
                                                     WBidHelper.PushToUndoStack();
                                                     CommonClass.lineVC.UpdateUndoRedoButtons();

                                                     bool status = await Task.Run(() => GetWBStateAndquicksetFromServer(wbStateDTO));

                                                     if (status)
                                                     {
                                                         InvokeOnMainThread(() =>
                                                     {
                                                         ShowAlertMessage("Smart Sync", "Successfully Synchronized  your State file from the server.");
                                                         syncOverlay.Hide();

                                                     });
                                                     }
                                                     else
                                                     {
                                                         InvokeOnMainThread(() =>
                                                     {
                                                         ShowAlertMessage("Smart Sync", "An error occured while synchronizing your state to the server.  You can work on this bid, but your state file for this bid may become unsynchronized from a previous state.");
                                                         if (syncOverlay != null)
                                                             syncOverlay.Hide();
                                                     });
                                                     }

                                                 }
                                                 else if (IskeepLocalState) //Save local State to server
                                                 {
                                                     FirstTime = true;
                                                     bool result = false;
                                                     syncOverlay = new LoadingOverlay(View.Bounds, "Synching current State FROM server \n Please wait..");
                                                     View.Add(syncOverlay);
                                                     //syncOverlay.updateLoadingText("Synching current State TO server \n Please wait..");
                                                     //syncOverlay.Show();


                                                     result = await Task.Run(() => SaveWBStateAndQuickSetToServer(stateFileName));

                                                     if (result)
                                                     {
                                                         InvokeOnMainThread(() =>
                                                         {
                                                             ShowAlertMessage("Smart Sync", "Successfully Synchronized  your State file with the server.");
                                                             syncOverlay.Hide();
                                                             if (FirstTime)
                                                                 NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);//loadSummaryListAndHeader();

                                                             FirstTime = false;
                                                         });
                                                     }
                                                     else
                                                     {
                                                         InvokeOnMainThread(() =>
                                                         {
                                                             ShowAlertMessage("Smart Sync", "An error occured while synchronizing your state to the server.  You can work on this bid, but your state file for this bid may become unsynchronized from a previous state.");
                                                             syncOverlay.Hide();
                                                         });
                                                     }
                                                 }
                                             }
                                             else
                                             {
                                                 InvokeOnMainThread(() =>
                                                 {
                                                     ShowAlertMessage("Smart Sync", "Your State file is already synchronized with the server");
                                                     if (syncOverlay != null)
                                                         syncOverlay.Hide();
                                                 });
                                             }
                                         }
                                         else if (DataSynchSelecedValue == 2) // Selected only for Quicksets  sync
                                         {
                                             if (SynchQSVersion != serverQSversion || GlobalSettings.QuickSets.IsModified)
                                             {
                                                 wbStateDTO.FileType = 1;
                                                 FirstTime = true;
                                                 if (IsQSFromServer)
                                                 {
                                                     syncOverlay = new LoadingOverlay(View.Bounds, "Synching Quicksets From server \n Please wait..");
                                                     View.Add(syncOverlay);
                                                     bool result = await Task.Run(() => GetWBStateAndquicksetFromServer(wbStateDTO));
                                                     if (result)
                                                     {
                                                         InvokeOnMainThread(() =>
                                                         {
                                                             ShowAlertMessage("Smart Sync", "Your Quickset is successfully synchronized with server.");
                                                             syncOverlay.Hide();
                                                         });
                                                     }
                                                     else
                                                     {
                                                         InvokeOnMainThread(() =>
                                                         {
                                                             ShowAlertMessage("Smart Sync", "An error occured while synchronizing your quickset from the server.  You can work on this bid");
                                                             syncOverlay.Hide();
                                                         });
                                                     }

                                                 }
                                                 else if (IskeepLocalQS)
                                                 {
                                                     syncOverlay = new LoadingOverlay(View.Bounds, "Synching Quikcsets TO server \n Please wait..");
                                                     View.Add(syncOverlay);
                                                     bool result = await Task.Run(() => SaveWBStateAndQuickSetToServer(stateFileName));
                                                     if (result)
                                                     {
                                                         InvokeOnMainThread(() =>
                                                         {
                                                             ShowAlertMessage("Smart Sync", "Your Quickset is successfully synchronized with server.");
                                                             syncOverlay.Hide();
                                                         });
                                                     }
                                                     else
                                                     {
                                                         InvokeOnMainThread(() =>
                                                         {
                                                             ShowAlertMessage("Smart Sync", "An error occured while synchronizing your quickset to the server.  You can work on this bid");
                                                             syncOverlay.Hide();
                                                         });
                                                     }
                                                 }
                                             }
                                             else
                                             {
                                                 InvokeOnMainThread(() =>
                                             {
                                                 ShowAlertMessage("Smart Sync", "Your Quickset is already synchronized with the server");
                                                 if (syncOverlay != null)
                                                     syncOverlay.Hide();
                                             });
                                             }
                                         }
                                         else if (DataSynchSelecedValue == 3)// bOth State and Quickset sync
                                         {
                                             wbStateDTO.FileType = 2;
                                             bool Stateresult = false;
                                             FirstTime = true;
                                             bool QSresult = false;
                                             syncOverlay = new LoadingOverlay(View.Bounds, "Synching Quicksets and States  \n Please wait..");
                                             View.Add(syncOverlay);
                                             if (IsStateFromServer || IsQSFromServer)
                                             {
                                                 Stateresult = await Task.Run(() => GetWBStateAndquicksetFromServer(wbStateDTO));
                                             }
                                             if (IskeepLocalState || IskeepLocalQS)
                                             {
                                                 QSresult = await Task.Run(() => SaveWBStateAndQuickSetToServer(stateFileName));
                                             }
                                             string alert = string.Empty;
                                             if (QSresult || Stateresult)
                                             {
                                                 alert = "Your Quickset and State  is Successsfully synchronized with server";
                                                 if (FirstTime)
                                                     NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);//loadSummaryListAndHeader();

                                                 FirstTime = false;
                                             }
                                             else
                                                 alert = "Something went wrong on sync operation. Please try again";
                                             ShowAlertMessage("Smart Sync", alert);
                                             if (syncOverlay != null)
                                                 syncOverlay.Hide();
                                         }
                                     }
                                     ////conflict
                                     //confNotif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("SyncConflict"), (NSNotification notification) => {
                                     //    string str = notification.Object.ToString();
                                     //    NSNotificationCenter.DefaultCenter.RemoveObserver(confNotif);
                                     //    BeginInvokeOnMainThread(() => {
                                     //        if (str == "server")
                                     //        {
                                     //            FirstTime = true;
                                     //            syncOverlay = new LoadingOverlay(View.Bounds, "Synching current State FROM server \n Please wait..");
                                     //            View.Add(syncOverlay);
                                     //            WBidHelper.PushToUndoStack();
                                     //            CommonClass.lineVC.UpdateUndoRedoButtons();
                                     //            InvokeInBackground(() => {
                                     //                GetStateFromServer(stateFileName);
                                     //            });
                                     //        }
                                     //        else if (str == "local")
                                     //        {
                                     //            FirstTime = true;
                                     //            syncOverlay = new LoadingOverlay(View.Bounds, "Synching current State TO server \n Please wait..");
                                     //            View.Add(syncOverlay);
                                     //            InvokeInBackground(() => {
                                     //                UploadLocalVersionToServer(stateFileName);
                                     //            });
                                     //        }
                                     //        else
                                     //        {
                                     //            IsSynchStart = false;
                                     //        }
                                     //    });
                                     //});
                                     //SynchConflictViewController synchConf = new SynchConflictViewController();
                                     //synchConf.serverSynchTime = ServerSynchTime;
                                     //if (serverVersion == 0)
                                     //    synchConf.noServer = true;
                                     //synchConf.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                                     //this.PresentViewController(synchConf, true, null);
                                 });


                            }


                            else if (SynchBtn)
                            {
                                SynchBtn = false;
                                UIAlertController okAlertController = UIAlertController.Create("Smart Sync", "Your App is already synchronized with the server..", UIAlertControllerStyle.Alert);
                                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                                this.PresentViewController(okAlertController, true, null);
                            }


                        }
                    }
                    else
                    {
                        InvokeOnMainThread(() =>
                        {

                            UIAlertController okAlertController = UIAlertController.Create("Smart Sync", "The WBid Synch server is not responding.  You can work on this bid and attempt to synch at a later time.", UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                        });
                    }

                }
                else
                {
                    InvokeOnMainThread(() =>
                    {
                        if (WBidHelper.IsSouthWestWifiOr2wire())
                        {
                            UIAlertController okAlertController = UIAlertController.Create("Smart Sync", Constants.SouthWestConnectionAlert, UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                        }
                        else
                        {
                            //UIAlertController okAlertController = UIAlertController.Create("Smart Sync", "You do not have an internet connection.  You can work on this bid offline, but your state file for this bid may become unsynchronized from a previous state.", UIAlertControllerStyle.Alert);
                            //okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            //this.PresentViewController(okAlertController, true, null);
                            UIAlertController okAlertController = UIAlertController.Create("Smart Sync", Constants.VPSDownAlert, UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                        }

                        syncOverlay.Hide();
                    });

                    // base.SendNotificationMessage(WBidMessages.MainVM_Notofication_ShowSmartSyncConfirmationWindow);


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ShowAlertMessage(string caption, string message)
        {
            UIAlertController okAlertController = UIAlertController.Create(caption, message, UIAlertControllerStyle.Alert);
            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            this.PresentViewController(okAlertController, true, null);
        }

        private void SynchStateForApplicationLoad()
        {

            try
            {
                // MessageBoxResult msgResult;
                bool isConnectionAvailable = Reachability.CheckVPSAvailable();

                if (isConnectionAvailable)
                {

                    IsSynchStart = true;
                    string stateFileName = WBidHelper.GenerateFileNameUsingCurrentBidDetails();
                    SynchStateVersion = int.Parse(GlobalSettings.WBidStateCollection.SyncVersion);

                    //Get server State Version
                    VersionInfo versionInfo = GetServerVersion(stateFileName);
                    syncOverlay.Hide();

                    if (versionInfo != null)
                    {
                        ServerSynchTime = DateTime.Parse(versionInfo.LastUpdatedDate, CultureInfo.InvariantCulture);

                        if (versionInfo.Version != string.Empty)
                        {
                            int serverVersion = Convert.ToInt32(versionInfo.Version);

                            if (SynchStateVersion != serverVersion || GlobalSettings.WBidStateCollection.IsModified)
                            {
                                //conflict
                                confNotif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("SyncConflict"), (NSNotification notification) =>
                                {
                                    string str = notification.Object.ToString();
                                    NSNotificationCenter.DefaultCenter.RemoveObserver(confNotif);
                                    BeginInvokeOnMainThread(() =>
                                    {
                                        if (str == "server")
                                        {
                                            FirstTime = true;
                                            syncOverlay = new LoadingOverlay(View.Bounds, "Synching current State FROM server \n Please wait..");
                                            View.Add(syncOverlay);
                                            WBidHelper.PushToUndoStack();
                                            CommonClass.lineVC.UpdateUndoRedoButtons();
                                            InvokeInBackground(() =>
                                            {
                                                GetStateFromServer(stateFileName);
                                            });
                                        }
                                        else if (str == "local")
                                        {
                                            FirstTime = true;
                                            syncOverlay = new LoadingOverlay(View.Bounds, "Synching current State TO server \n Please wait..");
                                            View.Add(syncOverlay);
                                            InvokeInBackground(() =>
                                            {
                                                UploadLocalVersionToServer(stateFileName);
                                            });
                                        }
                                        else
                                        {
                                            IsSynchStart = false;
                                        }
                                    });
                                });
                                SynchConflictViewController synchConf = new SynchConflictViewController();
                                synchConf.serverSynchTime = ServerSynchTime;
                                if (serverVersion == 0)
                                    synchConf.noServer = true;
                                synchConf.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                                this.PresentViewController(synchConf, true, null);
                            }
                            else if (SynchBtn)
                            {
                                SynchBtn = false;
                                UIAlertController okAlertController = UIAlertController.Create("Smart Sync", "Your App is already synchronized with the server..", UIAlertControllerStyle.Alert);
                                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                                this.PresentViewController(okAlertController, true, null);
                            }


                        }
                    }
                    else
                    {
                        InvokeOnMainThread(() =>
                        {

                            UIAlertController okAlertController = UIAlertController.Create("Smart Sync", "The WBid Synch server is not responding.  You can work on this bid and attempt to synch at a later time.", UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                        });
                    }

                }
                else
                {
                    InvokeOnMainThread(() =>
                    {
                        if (WBidHelper.IsSouthWestWifiOr2wire())
                        {
                            UIAlertController okAlertController = UIAlertController.Create("Smart Sync", Constants.SouthWestConnectionAlert, UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                        }
                        else
                        {
                            //UIAlertController okAlertController = UIAlertController.Create("Smart Sync", "You do not have an internet connection.  You can work on this bid offline, but your state file for this bid may become unsynchronized from a previous state.", UIAlertControllerStyle.Alert);
                            //okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            //this.PresentViewController(okAlertController, true, null);
                            UIAlertController okAlertController = UIAlertController.Create("Smart Sync", Constants.VPSDownAlert, UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                        }

                        syncOverlay.Hide();
                    });

                    // base.SendNotificationMessage(WBidMessages.MainVM_Notofication_ShowSmartSyncConfirmationWindow);


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SynchState()
        {
            try
            {
                bool isConnectionAvailable = Reachability.IsHostReachable(GlobalSettings.ServerUrl);
                if (isConnectionAvailable)
                {
                    //new thread?
                    IsSynchStart = true;

                    string stateFileName = WBidHelper.GenerateFileNameUsingCurrentBidDetails();
                    SynchStateVersion = int.Parse(GlobalSettings.WBidStateCollection.SyncVersion);
                    //Get server State Version
                    VersionInfo versionInfo = GetServerVersion(stateFileName);
                    if (versionInfo != null)
                    {
                        ServerSynchTime = DateTime.Parse(versionInfo.LastUpdatedDate, CultureInfo.InvariantCulture);

                        if (versionInfo.Version != string.Empty)
                        {
                            int serverVersion = Convert.ToInt32(versionInfo.Version);

                            if (SynchStateVersion != serverVersion || GlobalSettings.WBidStateCollection.IsModified)
                            {
                                //conflict
                                confNotif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("SyncConflict"), (NSNotification notification) =>
                                {
                                    string str = notification.Object.ToString();
                                    NSNotificationCenter.DefaultCenter.RemoveObserver(confNotif);
                                    BeginInvokeOnMainThread(() =>
                                    {
                                        if (str == "server")
                                        {
                                            syncOverlay = new LoadingOverlay(View.Bounds, "Synching current State FROM server \n Please wait..");
                                            View.Add(syncOverlay);
                                            InvokeInBackground(() =>
                                            {
                                                GetStateFromServer(stateFileName);
                                            }
                                            );
                                        }
                                        else if (str == "local")
                                        {
                                            syncOverlay = new LoadingOverlay(View.Bounds, "Synching current State TO server \n Please wait..");
                                            View.Add(syncOverlay);
                                            InvokeInBackground(() =>
                                            {
                                                UploadLocalVersionToServer(stateFileName);
                                            });
                                        }
                                        else
                                        {
                                            IsSynchStart = false;
                                            GoToHome();
                                        }
                                        //GoToHome ();
                                    });
                                });
                                SynchConflictViewController synchConf = new SynchConflictViewController();
                                synchConf.serverSynchTime = ServerSynchTime;
                                if (serverVersion == 0)
                                    synchConf.noServer = true;
                                synchConf.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                                this.PresentViewController(synchConf, true, null);
                            }


                        }
                    }
                    else
                    {
                        InvokeOnMainThread(() =>
                        {
                            UIAlertController okAlertController = UIAlertController.Create("Smart Sync", "The WBid Synch server is not responding.  You can work on this bid and attempt to synch at a later time.", UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                        });
                    }
                }
                else
                {
                    InvokeOnMainThread(() =>
                    {
                        UIAlertController okAlertController = UIAlertController.Create("Smart Sync", "You do not have an internet connection.  You can work on this bid offline, but your state file for this bid may become unsynchronized from a previous state.", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);
                    });

                    //   base.SendNotificationMessage(WBidMessages.MainVM_Notofication_ShowSmartSyncConfirmationWindow);


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void UploadLocalVersionToServer(string stateFileName)
        {
            bool result = SaveWBStateAndQuickSetToServer(stateFileName);
            if (result)
            {
                InvokeOnMainThread(() =>
                {
                    UIAlertController okAlertController = UIAlertController.Create("Smart Sync", "Successfully Synchronized  your computer with the server.", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                    syncOverlay.Hide();
                    if (FirstTime)
                        NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);//loadSummaryListAndHeader();
                    else
                    {

                    }
                    FirstTime = false;
                });
            }
            else
            {
                InvokeOnMainThread(() =>
                {

                    UIAlertController okAlertController = UIAlertController.Create("Smart Sync", "An error occured while synchronizing your state to the server.  You can work on this bid, but your state file for this bid may become unsynchronized from a previous state.", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                    syncOverlay.Hide();
                });
            }
        }
        //need to remove
        //private void UploadLocalVersionToServer (string stateFileName)
        //{
        //    int version = int.Parse (SaveStateToServer (stateFileName));
        //    if (version != -1) {
        //        GlobalSettings.WBidStateCollection.SyncVersion = version.ToString ();
        //        GlobalSettings.WBidStateCollection.StateUpdatedTime = DateTime.Now.ToUniversalTime ();
        //        GlobalSettings.WBidStateCollection.IsModified = false;
        //        string stateFilePath = Path.Combine (WBidHelper.GetAppDataPath (), stateFileName + ".WBS");
        //        //WBidCollection.SaveStateFile(GlobalSettings.WBidStateCollection, stateFilePath);
        //        WBidHelper.SaveStateFile (WBidHelper.WBidStateFilePath);

        //        IsSynchStart = false;
        //        InvokeOnMainThread (() => {
        //            UIAlertController okAlertController = UIAlertController.Create("Smart Sync", "Successfully Synchronized  your computer with the server.", UIAlertControllerStyle.Alert);
        //            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
        //            this.PresentViewController(okAlertController, true, null);
        //            syncOverlay.Hide ();
        //            if (FirstTime)
        //                NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);//loadSummaryListAndHeader();
        //            else {


        //            }
        //            FirstTime = false;
        //        });
        //    } else {
        //        InvokeOnMainThread (() => {

        //            UIAlertController okAlertController = UIAlertController.Create("Smart Sync", "An error occured while synchronizing your state to the server.  You can work on this bid, but your state file for this bid may become unsynchronized from a previous state.", UIAlertControllerStyle.Alert);
        //            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
        //            this.PresentViewController(okAlertController, true, null);
        //            syncOverlay.Hide ();
        //        });
        //    }
        //}
        //Old Mehod : Remove
        private VersionInfo GetServerVersion(string stateFileName)
        {
            VersionInfo versionInfo = null;
            try
            {
                if (!GlobalSettings.SynchEnable)
                    return versionInfo;
                string url = GlobalSettings.synchServiceUrl + "GetServerStateVersionNumber/" + GlobalSettings.WbidUserContent.UserInformation.EmpNo + "/" + stateFileName + "/" + GlobalSettings.CurrentBidDetails.Year;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 30000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var stream = response.GetResponseStream();
                var reader = new StreamReader(stream);
                versionInfo = ConvertJSonToObject<VersionInfo>(reader.ReadToEnd());
                versionInfo.Version = versionInfo.Version.Trim('"');
                return versionInfo;
            }
            catch (Exception ex)
            {
                versionInfo = null;
                IsSynchStart = false;
                return versionInfo;
                //throw ex;
            }
        }
        public WBStateInfoDTO GetWBServerStateandquicksetVersionNumber(WBGetStateDTO wbStateDTO)
        {
            WBStateInfoDTO versionInfo = null;
            try
            {
                if (!GlobalSettings.SynchEnable)
                    return versionInfo;
                string data = SmartSyncLogic.JsonObjectToStringSerializer<WBGetStateDTO>(wbStateDTO);
                string url = GlobalSettings.synchServiceUrl + "GetWBServerStateandquicksetVersionNumber";
                RestServiceUtil obj = new RestServiceUtil();
                string response = obj.PostData(url, data);
                versionInfo = CommonClass.ConvertJSonToObject<WBStateInfoDTO>(response);
                return versionInfo;
            }
            catch (Exception ex)
            {
                versionInfo = null;
                IsSynchStart = false;
                return versionInfo;
            }

        }
        public bool SaveWBStateAndQuickSetToServer(string stateFileName)
        {
            try
            {
                string url = GlobalSettings.synchServiceUrl + "SaveWBStateAndQuickSetToServer/";
                WBidStateCollection wBidStateCollection = GlobalSettings.WBidStateCollection;

                foreach (var item in wBidStateCollection.StateList)
                {
                    if (item.FAEOMStartDate == DateTime.MinValue)
                    {
                        item.FAEOMStartDate = DateTime.MinValue.ToUniversalTime();
                    }

                }


                StateQuickSetSyncDTO stateSync = new StateQuickSetSyncDTO();
                stateSync.EmployeeNumber = GlobalSettings.WbidUserContent.UserInformation.EmpNo;
                stateSync.StateFileName = stateFileName;
                stateSync.VersionNumber = int.Parse(GlobalSettings.WBidStateCollection.SyncVersion);
                stateSync.Year = GlobalSettings.CurrentBidDetails.Year;
                stateSync.StateContent = "";
                stateSync.LastUpdatedTime = DateTime.MinValue.ToUniversalTime();
                stateSync.QuickSetFileName = GlobalSettings.WbidUserContent.UserInformation.EmpNo;
                stateSync.QuickSetVersionNumber = 0;
                if (GlobalSettings.QuickSets != null)
                {
                    if (GlobalSettings.QuickSets.SyncQuickSetVersion != null)
                        stateSync.QuickSetVersionNumber = int.Parse(GlobalSettings.QuickSets.SyncQuickSetVersion);
                }
                stateSync.QuickSetLastUpdatedTime = DateTime.MinValue.ToUniversalTime();
                stateSync.QuickSetStateContent = "";
                if (DataSynchSelecedValue == 1 || DataSynchSelecedValue == 3)
                {
                    string stateContent = SmartSyncLogic.JsonObjectToStringSerializer<WBidStateCollection>(wBidStateCollection);
                    stateSync.StateContent = stateContent != null ? stateContent : null;
                }
                if (DataSynchSelecedValue == 2 || DataSynchSelecedValue == 3)
                {
                    QuickSets quickset = GlobalSettings.QuickSets;
                    string quickSetContent = SmartSyncLogic.JsonObjectToStringSerializer<QuickSets>(quickset);
                    stateSync.QuickSetStateContent = quickSetContent != null ? quickSetContent : null;
                }
                string data = SmartSyncLogic.JsonObjectToStringSerializer<StateQuickSetSyncDTO>(stateSync);

                RestServiceUtil obj = new RestServiceUtil();
                string response = obj.PostData(url, data);
                response.Trim('"');
                WBStateInfoDTO wbStateInfoDTO = CommonClass.ConvertJSonToObject<WBStateInfoDTO>(response);

                if (DataSynchSelecedValue == 1 || DataSynchSelecedValue == 3)
                {
                    GlobalSettings.WBidStateCollection.SyncVersion = wbStateInfoDTO.StateVersionNumber;
                    GlobalSettings.WBidStateCollection.StateUpdatedTime = wbStateInfoDTO.StateLastUpdate;

                    GlobalSettings.WBidStateCollection.IsModified = false;
                    string stateFilePath = Path.Combine(WBidHelper.GetAppDataPath(), stateFileName + ".WBS");
                    WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);

                    IsSynchStart = false;
                }
                if (DataSynchSelecedValue == 2 || DataSynchSelecedValue == 3)
                {
                    GlobalSettings.QuickSets.SyncQuickSetVersion = wbStateInfoDTO.QuickSetVersionNumber;
                    GlobalSettings.QuickSets.QuickSetUpdatedTime = wbStateInfoDTO.QuickSetLastUpdated;
                    GlobalSettings.QuickSets.IsModified = false;
                    XmlHelper.SerializeToXml(GlobalSettings.QuickSets, WBidHelper.GetQuickSetFilePath());

                    IsSynchStart = false;
                }
                return true;
            }
            catch (Exception ex)
            {
                IsSynchStart = false;
                return false;
            }
        }
        //need to remove
        //private string SaveStateToServer (string stateFileName)
        //{
        //    try {
        //        string url = GlobalSettings.synchServiceUrl + "SaveWBidStateToServer/";
        //        WBidStateCollection wBidStateCollection = GlobalSettings.WBidStateCollection;

        //        foreach (var item in wBidStateCollection.StateList) {
        //            if (item.FAEOMStartDate == DateTime.MinValue) {
        //                item.FAEOMStartDate = DateTime.MinValue.ToUniversalTime ();
        //            }

        //        }

        //        string data = string.Empty;
        //        StateSync stateSync = new StateSync ();
        //        stateSync.EmployeeNumber = GlobalSettings.WbidUserContent.UserInformation.EmpNo;
        //        stateSync.StateFileName = stateFileName;
        //        stateSync.VersionNumber = 0;
        //        stateSync.Year = GlobalSettings.CurrentBidDetails.Year;
        //        stateSync.StateContent = SmartSyncLogic.JsonObjectToStringSerializer<WBidStateCollection> (wBidStateCollection);
        //        stateSync.LastUpdatedTime = DateTime.MinValue.ToUniversalTime ();

        //        var request = (HttpWebRequest)WebRequest.Create (url);
        //        request.Method = "POST";
        //        request.ContentType = "application/x-www-form-urlencoded";
        //        //data = SmartSyncLogic.JsonSerializer(stateSync);

        //        data = SmartSyncLogic.JsonObjectToStringSerializer<StateSync> (stateSync);
        //        var bytes = Encoding.UTF8.GetBytes (data);
        //        request.ContentLength = bytes.Length;
        //        request.GetRequestStream ().Write (bytes, 0, bytes.Length);
        //        request.Timeout = 30000;
        //        //Response
        //        var response = (HttpWebResponse)request.GetResponse ();
        //        var stream = response.GetResponseStream ();
        //        if (stream == null)
        //            return string.Empty;

        //        var reader = new StreamReader (stream);
        //        string result = reader.ReadToEnd ();

        //        return result.Trim ('"');
        //    } catch (Exception ex) {
        //        IsSynchStart = false;
        //        return "-1";
        //    }
        //}

        void ReloadLineView()
        {
            wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            if (wBIdStateContent.SortDetails.SortColumn == "SelectedColumn")
            {
                string colName = wBIdStateContent.SortDetails.SortColumnName;
                string direction = wBIdStateContent.SortDetails.SortDirection;
                if (colName == "LineNum")
                    colName = "LineDisplay";
                if (colName == "LastArrivalTime")
                    colName = "LastArrTime";
                if (colName == "StartDowOrder")
                    colName = "StartDow";
                var datapropertyId = GlobalSettings.columndefinition.FirstOrDefault(x => x.DataPropertyName == colName).Id;
                CommonClass.columnID = datapropertyId;
                if (direction == "Asc")
                    CommonClass.columnAscend = true;
                else
                    CommonClass.columnAscend = false;
            }
            else
            {
                CommonClass.columnID = 0;
                CommonClass.columnAscend = false;
            }
            //            SortCalculation sort = new SortCalculation ();
            //            if (wBIdStateContent.SortDetails != null && wBIdStateContent.SortDetails.SortColumn != null && wBIdStateContent.SortDetails.SortColumn != string.Empty) {
            //                sort.SortLines (wBIdStateContent.SortDetails.SortColumn);
            //            }
        }
        private bool GetWBStateAndquicksetFromServer(WBGetStateDTO wbStateDTO)
        {
            try
            {
                StateQuickSetSyncDTO stateQsSync = null;
                string url = GlobalSettings.synchServiceUrl + "GetWBStateAndquicksetFromServer/";

                string data = SmartSyncLogic.JsonObjectToStringSerializer<WBGetStateDTO>(wbStateDTO);
                RestServiceUtil obj = new RestServiceUtil();
                string response = obj.PostData(url, data);
                response.Trim('"');

                stateQsSync = CommonClass.ConvertJSonToObject<StateQuickSetSyncDTO>(response);
                if (stateQsSync != null)
                {
                    if ((DataSynchSelecedValue != 3) || (DataSynchSelecedValue == 3 && IsStateFromServer && IsQSFromServer))
                    {
                        if (stateQsSync != null && stateQsSync.StateContent != null && stateQsSync.StateContent != string.Empty)
                        {
                            UpdateStateServerToLocal(stateQsSync);
                        }

                        UpdateQuickSetServerToLocal(stateQsSync);

                    }
                    else if (IsStateFromServer && IskeepLocalQS)
                        UpdateStateServerToLocal(stateQsSync);
                    else if (IsQSFromServer && IskeepLocalState)
                        UpdateQuickSetServerToLocal(stateQsSync);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private void UpdateQuickSetServerToLocal(StateQuickSetSyncDTO stateQsSync)
        {
            try
            {
                if (stateQsSync != null && stateQsSync.QuickSetStateContent != null && stateQsSync.QuickSetStateContent != "")
                {
                    QuickSets quickset = null;
                    quickset = SmartSyncLogic.ConvertJsonToObject<QuickSets>(stateQsSync.QuickSetStateContent);
                    GlobalSettings.QuickSets = quickset;
                    GlobalSettings.QuickSets.QuickSetUpdatedTime = DateTime.Parse(stateQsSync.QuickSetLastUpdatedTimeString);//stateQsSync.QuickSetLastUpdatedTime;
                    GlobalSettings.QuickSets.SyncQuickSetVersion = stateQsSync.QuickSetVersionNumber.ToString();
                    GlobalSettings.QuickSets.IsModified = false;
                    //IsQSModified = false;
                    // IsStateModified = false;
                    XmlHelper.SerializeToXml(GlobalSettings.QuickSets, WBidHelper.GetQuickSetFilePath());
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        private void UpdateStateServerToLocal(StateQuickSetSyncDTO stateQsSync)
        {
            bool failed = false;
            bool IsLineFileAvailable = false;
            bool isneedtoChangeLineFile = false;
            try
            {
                WBidStateCollection wBidStateCollection = null;

                //bool isNeedToRecalculateLineProp = false;
                bool isneedTorecalculateForMIL = false;
                if (stateQsSync != null && stateQsSync.StateContent != null && stateQsSync.StateContent != string.Empty)
                {

                    wBidStateCollection = SmartSyncLogic.ConvertJsonToObject<WBidStateCollection>(stateQsSync.StateContent);
                    foreach (WBidState state in wBidStateCollection.StateList)
                    {
                        if (state.CxWtState.CLAuto == null)
                            state.CxWtState.CLAuto = new StateStatus { Cx = false, Wt = false };
                        if (state.CxWtState.StartDay == null)
                            state.CxWtState.StartDay = new StateStatus { Cx = false, Wt = false };
                        if (state.CxWtState.ReportRelease == null)
                            state.CxWtState.ReportRelease = new StateStatus { Cx = false, Wt = false };
                        if (state.CxWtState.CitiesLegs == null)
                        {
                            state.CxWtState.CitiesLegs = new StateStatus { Cx = false, Wt = false };
                            state.Constraints.CitiesLegs = new Cx3Parameters { ThirdcellValue = "1", Type = (int)ConstraintType.LessThan, Value = 1, lstParameters = new List<Cx3Parameter>() };
                            state.Weights.CitiesLegs = new Wt2Parameters
                            {
                                Type = 1,
                                Weight = 0,
                                lstParameters = new List<Wt2Parameter>()
                            };
                        }
                        if (state.CxWtState.Commute == null)
                        {
                            state.CxWtState.Commute = new StateStatus { Cx = false, Wt = false };
                            state.Constraints.Commute = new Commutability { BaseTime = 10, ConnectTime = 30, CheckInTime = 60, SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.MoreThan, Value = 100 };
                            state.Weights.Commute = new Commutability { BaseTime = 10, ConnectTime = 30, CheckInTime = 60, SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.MoreThan, Value = 100 };
                        }
                        state.CxWtState.Commute.Cx = false;
                        state.CxWtState.Commute.Wt = false;

                        if (state.Constraints.StartDayOftheWeek.SecondcellValue == null)
                        {
                            state.Constraints.StartDayOftheWeek.SecondcellValue = "1";
                            if (state.Constraints.StartDayOftheWeek.lstParameters != null)
                            {
                                foreach (var item in state.Constraints.StartDayOftheWeek.lstParameters)
                                {
                                    if (item.SecondcellValue == null)
                                    {
                                        item.SecondcellValue = "1";
                                    }
                                }
                            }
                        }

                        if (state.CxWtState.EQUIP.Cx)
                        {
                            state.Constraints.EQUIP.lstParameters.RemoveAll(x => x.ThirdcellValue == "500" || x.ThirdcellValue == "300");
                            if (state.Constraints.EQUIP.lstParameters.Count == 0)
                                state.CxWtState.EQUIP.Cx = false;
                        }
                        if (state.CxWtState.EQUIP.Wt)
                        {
                            state.Weights.EQUIP.lstParameters.RemoveAll(x => x.SecondlValue == 500 || x.SecondlValue == 300);
                            if (state.Weights.EQUIP.lstParameters.Count == 0)
                                state.CxWtState.EQUIP.Wt = false;
                        }



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


                    var wBidStateContent = wBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);


                    var currentopendState = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                    try
                    {

                        if (wBidStateContent.BidAuto != null && wBidStateContent.BidAuto.BAFilter != null && wBidStateContent.BidAuto.BAFilter.Count > 0)
                        {
                            wBidStateContent.BidAuto.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "500");
                            wBidStateContent.BidAuto.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "300");
                        }
                        if (wBidStateContent.CalculatedBA != null && wBidStateContent.CalculatedBA.BAFilter != null && wBidStateContent.CalculatedBA.BAFilter.Count > 0)
                        {
                            wBidStateContent.CalculatedBA.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "500");
                            wBidStateContent.CalculatedBA.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "300");
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    if (wBidStateContent.MenuBarButtonState.IsOverlap && currentopendState.IsOverlapCorrection == false)
                    {
                        InvokeOnMainThread(() =>
                        {
                            syncOverlay.Hide();
                            UIAlertController okAlertController = UIAlertController.Create("Smart Sync", "Previous state had Overlap Data and You need to re-download the bid data and make an overlap correction", UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                        });

                        return;
                    }


                    StateManagement statemanagement = new StateManagement();
                    isneedtoChangeLineFile = statemanagement.IsneedToChangeLineFile(wBidStateContent);
                    wBidStateContent.IsOverlapCorrection = false;
                    statemanagement.SetMenuBarButtonStatusFromStateFile(wBidStateContent);
                    if (isneedtoChangeLineFile)
                    {
                        string isSuccess = WBidHelper.RetrieveSaveAndSetLineFiles(0, wBidStateContent);
                        IsLineFileAvailable = (isSuccess == "Ok") ? true : false;
                        if (!IsLineFileAvailable)
                        {
                            wBidStateContent.MenuBarButtonState.IsVacationCorrection = currentopendState.MenuBarButtonState.IsVacationCorrection;
                            wBidStateContent.MenuBarButtonState.IsVacationDrop = currentopendState.MenuBarButtonState.IsVacationDrop;
                            wBidStateContent.MenuBarButtonState.IsEOM = currentopendState.MenuBarButtonState.IsEOM;

                        }
                        // RecalculateLineProperties(wBidStateContent);
                    }

                    // isNeedToRecalculateLineProp = statemanagement.CheckLinePropertiesNeedToRecalculate(wBidStateContent);
                    // ResetLinePropertiesBackToNormal(currentopendState, wBidStateContent);
                    // ResetOverlapState(currentopendState, wBidStateContent);

                    GlobalSettings.WBidStateCollection = wBidStateCollection;
                    //GlobalSettings.WBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName); ;
                    GlobalSettings.WBidStateCollection.SyncVersion = stateQsSync.VersionNumber.ToString();
                    GlobalSettings.WBidStateCollection.StateUpdatedTime = stateQsSync.LastUpdatedTime;
                    GlobalSettings.WBidStateCollection.IsModified = false;

                    //string currentBidName = WBidHelper.GenerateFileNameUsingCurrentBidDetails();

                    ////string zipFileName = GenarateZipFileName();
                    //string vACFileName = WBidHelper.GetAppDataPath() + "//" + currentBidName + ".VAC";
                    ////Cheks the VAC file exists
                    //bool vacFileExists = File.Exists(vACFileName);
                    //if (vacFileExists == false)
                    //{
                    //    wBidStateContent.MenuBarButtonState.IsVacationDrop = false;
                    //    wBidStateContent.MenuBarButtonState.IsVacationCorrection = false;
                    //    wBidStateContent.IsVacationOverlapOverlapCorrection = false;
                    //}
                    //if (wBidStateContent.MenuBarButtonState.IsEOM)
                    //{
                    //    SetEOMVacationDataAfterSynch();
                    //}
                    if (wBidStateContent.MenuBarButtonState.IsMIL)
                    {
                        SetMILDataAfterSynch();
                    }
                    if (wBidStateContent.MenuBarButtonState.IsMIL && isneedtoChangeLineFile)
                    {
                        isneedTorecalculateForMIL = true;
                    }
                    wBidStateContent.SortDetails.SortColumn = "Manual";

                    string stateFilePath = Path.Combine(WBidHelper.GetAppDataPath(), stateQsSync.StateFileName + ".WBS");
                    //WBidCollection.SaveStateFile(GlobalSettings.WBidStateCollection, stateFilePath);
                    WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);



                }
                InvokeOnMainThread(() =>
                {
                    // UIAlertController okAlertController = UIAlertController.Create("Smart Sync", "Successfully Synchronized  your computer with the server.", UIAlertControllerStyle.Alert);
                    //  okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    // this.PresentViewController(okAlertController, true, null);
                    syncOverlay.Hide();
                    if (FirstTime)
                    {
                        GlobalSettings.Lines.ToList().ForEach(x =>
                        {
                            x.ConstraintPoints.Reset();
                            x.Constrained = false;
                            x.WeightPoints.Reset();
                            x.TotWeight = 0.0m;
                        });

                        WBidCollection.GenarateTempAbsenceList();
                        StateManagement statemanagement = new StateManagement();
                        statemanagement.ReloadLineDetailsBasedOnPreviousState(isneedTorecalculateForMIL);



                        var wBidStateContent = wBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                        if (wBidStateContent.TagDetails != null)
                        {
                            foreach (var item in GlobalSettings.Lines)
                            {
                                var tagItem = wBidStateContent.TagDetails.FirstOrDefault(x => x.Line == item.LineNum);
                                if (tagItem != null)
                                    item.Tag = tagItem.Content;
                                else
                                    item.Tag = string.Empty;

                            }

                        }

                        if (wBidStateContent.TagDetails != null)
                        {
                            GlobalSettings.TagDetails = new TagDetails();
                            wBidStateContent.TagDetails.ForEach(x => GlobalSettings.TagDetails.Add(new Tag { Line = x.Line, Content = x.Content }));
                        }
                        btnVacDrop.SetTitle("FLY", UIControlState.Normal);
                        btnVacDrop.SetTitleColor(UIColor.White, UIControlState.Normal);
                        this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("activeButtonRed.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);


                        NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);



                        SetVacButtonStates();
                    }
                    else
                    {

                    }
                    FirstTime = false;
                });
            }
            catch (Exception ex)
            {
                FirstTime = false;
                failed = true;

            }

            if (failed)
            {
                InvokeOnMainThread(() =>
                {
                    //syncOverlay.Hide();
                    UIAlertController okAlertController = UIAlertController.Create("Smart Sync", "The server to get the previous state file is not responding.  You can work on this bid, but your state file for this bid may become unsynchronized from a previous state.", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);

                });

            }
            if (isneedtoChangeLineFile == true && !IsLineFileAvailable)
            {
                InvokeOnMainThread(() =>
                {
                    //syncOverlay.Hide();
                    UIAlertController okAlertController = UIAlertController.Create("Smart Sync", "We found there is an error while downloading the Vacation files during sync operation. You can work on this bid, but Please check the VAC, DRP button state ", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);

                });
            }

        }
        //need to remove this
        private void GetStateFromServer(string stateFileName)
        {
            bool failed = false;
            try
            {
                string url = GlobalSettings.synchServiceUrl + "GetWBidStateFromServer/" + GlobalSettings.WbidUserContent.UserInformation.EmpNo + "/" + stateFileName + "/" + GlobalSettings.CurrentBidDetails.Year;


                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 30000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var stream = response.GetResponseStream();
                var reader = new StreamReader(stream);
                StateSync stateSync = SmartSyncLogic.ConvertJsonToObject<StateSync>(reader.ReadToEnd());
                WBidStateCollection wBidStateCollection = null;
                bool isneedtoChangeLineFile = false;
                bool isneedTorecalculateForMIL = false;
                if (stateSync != null)
                {
                    // byte[] byteArr = Convert.FromBase64String(stateSync.StateContent);

                    wBidStateCollection = SmartSyncLogic.ConvertJsonToObject<WBidStateCollection>(stateSync.StateContent);
                    foreach (WBidState state in wBidStateCollection.StateList)
                    {
                        if (state.CxWtState.CLAuto == null)
                            state.CxWtState.CLAuto = new StateStatus { Cx = false, Wt = false };
                        if (state.CxWtState.StartDay == null)
                            state.CxWtState.StartDay = new StateStatus { Cx = false, Wt = false };
                        if (state.CxWtState.ReportRelease == null)
                            state.CxWtState.ReportRelease = new StateStatus { Cx = false, Wt = false };
                        if (state.CxWtState.CitiesLegs == null)
                        {
                            state.CxWtState.CitiesLegs = new StateStatus { Cx = false, Wt = false };
                            state.Constraints.CitiesLegs = new Cx3Parameters { ThirdcellValue = "1", Type = (int)ConstraintType.LessThan, Value = 1, lstParameters = new List<Cx3Parameter>() };
                            state.Weights.CitiesLegs = new Wt2Parameters
                            {
                                Type = 1,
                                Weight = 0,
                                lstParameters = new List<Wt2Parameter>()
                            };
                        }
                        if (state.CxWtState.Commute == null)
                        {
                            state.CxWtState.Commute = new StateStatus { Cx = false, Wt = false };
                            state.Constraints.Commute = new Commutability { BaseTime = 10, ConnectTime = 30, CheckInTime = 60, SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.MoreThan, Value = 100 };
                            state.Weights.Commute = new Commutability { BaseTime = 10, ConnectTime = 30, CheckInTime = 60, SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.MoreThan, Value = 100 };
                        }
                        state.CxWtState.Commute.Cx = false;
                        state.CxWtState.Commute.Wt = false;
                        //once commutability sort implemented, remove this.
                        //state.SortDetails.BlokSort.RemoveAll(x=>x=="30"||x=="31" || x=="32");
                        if (state.Constraints.StartDayOftheWeek.SecondcellValue == null)
                        {
                            state.Constraints.StartDayOftheWeek.SecondcellValue = "1";
                            if (state.Constraints.StartDayOftheWeek.lstParameters != null)
                            {
                                foreach (var item in state.Constraints.StartDayOftheWeek.lstParameters)
                                {
                                    if (item.SecondcellValue == null)
                                    {
                                        item.SecondcellValue = "1";
                                    }
                                }
                            }
                        }

                        if (state.CxWtState.EQUIP.Cx)
                        {
                            state.Constraints.EQUIP.lstParameters.RemoveAll(x => x.ThirdcellValue == "500" || x.ThirdcellValue == "300");
                            if (state.Constraints.EQUIP.lstParameters.Count == 0)
                                state.CxWtState.EQUIP.Cx = false;
                        }
                        if (state.CxWtState.EQUIP.Wt)
                        {
                            state.Weights.EQUIP.lstParameters.RemoveAll(x => x.SecondlValue == 500 || x.SecondlValue == 300);
                            if (state.Weights.EQUIP.lstParameters.Count == 0)
                                state.CxWtState.EQUIP.Wt = false;
                        }



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


                    var wBidStateContent = wBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);


                    var currentopendState = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                    try
                    {
                        //bool isNeedToRecalculateBA= wBidStateContent.BidAuto.BAFilter.Any(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "500");
                        //if (isNeedToRecalculateBA == false)
                        //{
                        //    isNeedToRecalculateBA = wBidStateContent.BidAuto.BAFilter.Any(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "300");
                        //}
                        //remove 300 and 500 equipments
                        if (wBidStateContent.BidAuto != null && wBidStateContent.BidAuto.BAFilter != null && wBidStateContent.BidAuto.BAFilter.Count > 0)
                        {
                            wBidStateContent.BidAuto.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "500");
                            wBidStateContent.BidAuto.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "300");
                        }
                        if (wBidStateContent.CalculatedBA != null && wBidStateContent.CalculatedBA.BAFilter != null && wBidStateContent.CalculatedBA.BAFilter.Count > 0)
                        {
                            wBidStateContent.CalculatedBA.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "500");
                            wBidStateContent.CalculatedBA.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "300");
                        }
                        //if (isNeedToRecalculateBA)
                        //{
                        //    //RecalculateBidAutomatorvalues(wBidStateContent);
                        //}
                    }
                    catch (Exception ex)
                    {

                    }
                    if (wBidStateContent.MenuBarButtonState.IsOverlap && currentopendState.IsOverlapCorrection == false)
                    {
                        InvokeOnMainThread(() =>
                        {
                            syncOverlay.Hide();
                            UIAlertController okAlertController = UIAlertController.Create("Smart Sync", "Previous state had Overlap Data and You need to re-download the bid data and make an overlap correction", UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                        });

                        return;
                    }


                    StateManagement statemanagement = new StateManagement();
                    isneedtoChangeLineFile = statemanagement.IsneedToChangeLineFile(wBidStateContent);
                    wBidStateContent.IsOverlapCorrection = false;
                    statemanagement.SetMenuBarButtonStatusFromStateFile(wBidStateContent);
                    //  isNeedToRecalculateLineProp = statemanagement.CheckLinePropertiesNeedToRecalculate(wBidStateContent);
                    // ResetLinePropertiesBackToNormal(currentopendState, wBidStateContent);

                    //ResetOverlapState(currentopendState, wBidStateContent);

                    GlobalSettings.WBidStateCollection = wBidStateCollection;
                    //GlobalSettings.WBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName); ;
                    GlobalSettings.WBidStateCollection.SyncVersion = stateSync.VersionNumber.ToString();
                    GlobalSettings.WBidStateCollection.StateUpdatedTime = stateSync.LastUpdatedTime;
                    GlobalSettings.WBidStateCollection.IsModified = false;

                    //string currentBidName = WBidHelper.GenerateFileNameUsingCurrentBidDetails();

                    //string zipFileName = GenarateZipFileName();
                    // string vACFileName = WBidHelper.GetAppDataPath() + "//" + currentBidName + ".VAC";
                    //Cheks the VAC file exists
                    // bool vacFileExists = File.Exists(vACFileName);
                    // if (vacFileExists == false)
                    // {
                    //     wBidStateContent.MenuBarButtonState.IsVacationDrop = false;
                    //     wBidStateContent.MenuBarButtonState.IsVacationCorrection = false;
                    //     wBidStateContent.IsVacationOverlapOverlapCorrection = false;
                    // }
                    if (wBidStateContent.MenuBarButtonState.IsEOM)
                    {
                        SetEOMVacationDataAfterSynch();
                    }
                    if (wBidStateContent.MenuBarButtonState.IsMIL)
                    {
                        SetMILDataAfterSynch();
                    }
                    if (wBidStateContent.MenuBarButtonState.IsMIL && isneedtoChangeLineFile)
                    {
                        isneedTorecalculateForMIL = true;

                    }
                    wBidStateContent.SortDetails.SortColumn = "Manual";

                    string stateFilePath = Path.Combine(WBidHelper.GetAppDataPath(), stateFileName + ".WBS");
                    //WBidCollection.SaveStateFile(GlobalSettings.WBidStateCollection, stateFilePath);
                    WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);



                }
                InvokeOnMainThread(() =>
                {
                    UIAlertController okAlertController = UIAlertController.Create("Smart Sync", "Successfully Synchronized  your computer with the server.", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                    syncOverlay.Hide();
                    if (FirstTime)
                    {
                        GlobalSettings.Lines.ToList().ForEach(x =>
                        {
                            x.ConstraintPoints.Reset();
                            x.Constrained = false;
                            x.WeightPoints.Reset();
                            x.TotWeight = 0.0m;
                        });
                        if (isneedtoChangeLineFile)
                            WBidCollection.GenarateTempAbsenceList();
                        StateManagement statemanagement = new StateManagement();
                        statemanagement.ReloadLineDetailsBasedOnPreviousState(isneedTorecalculateForMIL);

                        var wBidStateContent = wBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                        if (wBidStateContent.TagDetails != null)
                        {
                            foreach (var item in GlobalSettings.Lines)
                            {
                                var tagItem = wBidStateContent.TagDetails.FirstOrDefault(x => x.Line == item.LineNum);
                                if (tagItem != null)
                                    item.Tag = tagItem.Content;
                                else
                                    item.Tag = string.Empty;

                            }

                        }

                        if (wBidStateContent.TagDetails != null)
                        {
                            GlobalSettings.TagDetails = new TagDetails();
                            wBidStateContent.TagDetails.ForEach(x => GlobalSettings.TagDetails.Add(new Tag { Line = x.Line, Content = x.Content }));
                        }
                        //statemanagement.ReloadDataFromStateFile();
                        //loadSummaryListAndHeader();
                        //ReloadLineView ();

                        NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                        SetVacButtonStates();
                    }
                    else
                    {
                        //syAlert.Dismissed += (object sender, UIButtonEventArgs e) =>
                        //{
                        //    if (e.ButtonIndex == 0)
                        //        GoToHome();
                        //};
                    }
                    FirstTime = false;
                });
            }
            catch (Exception ex)
            {
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

            if (failed)
            {
                InvokeOnMainThread(() =>
                {
                    syncOverlay.Hide();
                    UIAlertController okAlertController = UIAlertController.Create("Smart Sync", "The server to get the previous state file is not responding.  You can work on this bid, but your state file for this bid may become unsynchronized from a previous state.", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);

                });

            }

        }

        private void RecalculateBidAutomatorvalues(WBidState wBidStateContent)
        {
            //Clear top lock and bottom lock and line property for constraint

            GlobalSettings.Lines.ToList().ForEach(x =>
            {
                x.TopLock = false;
                x.BotLock = false;
                //x.WeightPoints.Reset();
                if (x.BAFilters != null)
                    x.BAFilters.Clear();
                x.BAGroup = string.Empty;
                x.IsGrpColorOn = 0;
            });

            if (wBidStateContent.BidAuto != null && wBidStateContent.BidAuto.BAGroup != null)
            {
                wBidStateContent.BidAuto.BAGroup.Clear();
            }

            // Adding Selected Constraints to State Object
            // AddSelectedConstraintsToStateObject();

            // AddSelectedSortToStateObject();

            // Calculate Line properties value for BA Constraint
            //    InvokeInBackground (() => {
            var bACalculation = new BidAutomatorCalculations();

            bACalculation.CalculateLinePropertiesForBAFilters();



            //Apply COnstrint And Sort
            bACalculation.ApplyBAFilterAndSort();

            //            IsneedToEnableCalculateBid = false;
            //            IsSortChanged = false;

            if (wBidStateContent.BidAuto != null && wBidStateContent.BidAuto.BAFilter != null)
                wBidStateContent.BidAuto.BAFilter.ForEach(x => x.IsApplied = true);


            //Setting Bid Automator settings to CalculatedBA state
            //SetCurrentBADetailsToCalculateBAState();

            GlobalSettings.isModified = true;
        }
        //        partial void ScrollUpButtonClicked (NSObject sender)
        //        {
        //            modernList.ScrollUp();

        //    sumList.TableView.DeselectRow (lPath, true);
        //            NSIndexPath[] visibleRows = sumList.TableView.IndexPathsForVisibleRows;
        //
        //            for (int i=0 ; i < (int)visibleRows.Length ; i++)
        //            {
        //                Console.WriteLine ("Rows-"+ visibleRows[i].Row);
        //
        //            }

        //            NSIndexPath path = NSIndexPath.FromRowSection (lPath.Row - 1, lPath.Section);
        //            lPath = path;
        //            sumList.TableView.SelectRow (lPath, true, UITableViewScrollPosition.None);
        //
        //
        //            CGPoint point = sumList.TableView.RectForRowAtIndexPath (lPath).Location;
        //            point = sumList.TableView.ConvertPointToView (point, this.vwTable);
        //            CGPoint offset = sumList.TableView.ContentOffset;
        //            if (point.Y < 50)
        //                sumList.TableView.ScrollToRow (NSIndexPath.FromRowSection (lPath.Row, lPath.Section), UITableViewScrollPosition.Top, true);

        //        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentState"></param>
        /// <param name="newState"></param>
        private void ResetLinePropertiesBackToNormal(WBidState currentState, WBidState newState)
        {
            if (newState.MenuBarButtonState.IsOverlap == false && currentState.MenuBarButtonState.IsOverlap)
            {
                //remove the  Overlp Calculation from line
                ReCalculateLinePropertiesForOverlapCorrection reCalculateLinePropertiesForOverlapCorrection = new ReCalculateLinePropertiesForOverlapCorrection();
                reCalculateLinePropertiesForOverlapCorrection.ReCalculateLinePropertiesOnOverlapCorrection(GlobalSettings.Lines.ToList(), false);
            }
            else if ((currentState.MenuBarButtonState.IsVacationCorrection || currentState.MenuBarButtonState.IsEOM) && newState.MenuBarButtonState.IsOverlap)
            {
                GlobalSettings.MenuBarButtonStatus.IsEOM = false;
                GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = false;
                GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                //Remove the vacation propertiesfrom Line 
                RecalcalculateLineProperties RecalcalculateLineProperties = new PortableLibrary.BusinessLogic.RecalcalculateLineProperties();
                RecalcalculateLineProperties.CalcalculateLineProperties();
            }

        }


        private void ResetOverlapState(WBidState currentState, WBidState newState)
        {
            if (newState.IsOverlapCorrection == false && currentState.IsOverlapCorrection)
            {
                newState.IsOverlapCorrection = true;
            }
            else if (newState.IsOverlapCorrection && currentState.IsOverlapCorrection == false)
            {
                newState.IsOverlapCorrection = false;
            }
        }

        public MILData CreateNewMILFile()
        {
            MILData milData;
            CalculateMIL calculateMIL = new CalculateMIL();
            MILParams milParams = new MILParams();
            NetworkData networkData = new NetworkData();
            if (System.IO.File.Exists(WBidHelper.GetAppDataPath() + "/FlightData.NDA"))
                networkData.ReadFlightRoutes();
            else
                networkData.GetFlightRoutes();
            //calculate MIL value and create MIL File
            //==============================================
            WBidCollection.GenerateSplitPointCities();
            milParams.Lines = GlobalSettings.Lines.ToList();
            Dictionary<string, TripMultiMILData> milvalue = calculateMIL.CalculateMILValues(milParams);
            milData = new MILData();
            milData.Version = GlobalSettings.MILFileVersion;
            milData.MILValue = milvalue;
            var stream = File.Create(WBidHelper.MILFilePath);
            ProtoSerailizer.SerializeObject(WBidHelper.MILFilePath, milData, stream);
            stream.Dispose();
            stream.Close();
            return milData;
        }

        private void SetMILDataAfterSynch()
        {
            var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

            var MILDates = wBidStateContent.MILDateList;

            if (MILDates.Count > 0)
            {
                isNeedtoCreateMILFile = false;
                if (GlobalSettings.MILDates == null || MILDates.Count != GlobalSettings.MILDates.Count)
                    isNeedtoCreateMILFile = true;
                else
                {
                    for (int count = 0; count < MILDates.Count; count++)
                    {
                        if (GlobalSettings.MILDates[count].StartAbsenceDate != MILDates[count].StartAbsenceDate || GlobalSettings.MILDates[count].EndAbsenceDate != MILDates[count].EndAbsenceDate)
                        {
                            isNeedtoCreateMILFile = true;
                            break;
                        }

                    }
                }
                GlobalSettings.MILDates = GenarateOrderedMILDates(wBidStateContent.MILDateList);
                MILData milData;
                InvokeOnMainThread(() =>
                {
                    syncOverlay.updateLoadingText("Calculating MIL");
                });

                //InvokeInBackground (() => {
                if (System.IO.File.Exists(WBidHelper.MILFilePath) && !isNeedtoCreateMILFile)
                {
                    using (FileStream milStream = File.OpenRead(WBidHelper.MILFilePath))
                    {

                        MILData milDataobject = new MILData();
                        milData = ProtoSerailizer.DeSerializeObject(WBidHelper.MILFilePath, milDataobject, milStream);

                    }
                }
                else
                {

                    milData = CreateNewMILFile();




                }


                //Apply MIL values (calculate property values including Modern bid line properties
                //==============================================

                GlobalSettings.MILData = milData.MILValue;
                GlobalSettings.MenuBarButtonStatus.IsMIL = true;

                RecalcalculateLineProperties recalcalculateLineProperties = new RecalcalculateLineProperties();
                recalcalculateLineProperties.CalcalculateLineProperties();

                InvokeOnMainThread(() =>
                {
                    GlobalSettings.isModified = true;
                    CommonClass.lineVC.UpdateSaveButton();
                    syncOverlay.Hide();
                    CommonClass.lineVC.SetVacButtonStates();
                    NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                    this.DismissViewController(true, null);
                });




                //});

            }
        }


        private void SetEOMVacationDataAfterSynch()
        {
            try
            {
                var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

                if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                {
                    if (GlobalSettings.FAEOMStartDate != null && GlobalSettings.FAEOMStartDate != DateTime.MinValue)
                    {
                        InvokeOnMainThread(() =>
                        {
                            btnVacDrop.Enabled = true;
                            syncOverlay.updateLoadingText("Calculating EOM");
                        });
                        InvokeInBackground(() =>
                        {
                            CreateEOMVacforFA();
                        });
                    }
                }
                else
                {
                    string currentBidName = WBidHelper.GenerateFileNameUsingCurrentBidDetails();

                    //string zipFileName = GenarateZipFileName();
                    string vACFileName = WBidHelper.GetAppDataPath() + "//" + currentBidName + ".VAC";
                    //Cheks the VAC file exists
                    bool vacFileExists = File.Exists(vACFileName);

                    if (!vacFileExists)
                    {
                        InvokeOnMainThread(() =>
                        {
                            //syncOverlay.Hide ();
                            UIAlertController okAlertController = UIAlertController.Create("Smart Sync", "Previous state had EOM selected and we are downloading Vacation Data", UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                            syncOverlay.updateLoadingText("Calculating EOM");
                        });


                        //InvokeOnMainThread (() => {

                        CreateEOMVacationforCP();


                        //});
                    }
                    else
                    {




                        InvokeOnMainThread(() =>
                        {
                            syncOverlay.updateLoadingText("Calculating EOM");

                            if (GlobalSettings.VacationData == null)
                            {
                                using (FileStream vacstream = File.OpenRead(vACFileName))
                                {

                                    Dictionary<string, TripMultiVacData> objineinfo = new Dictionary<string, TripMultiVacData>();
                                    GlobalSettings.VacationData = ProtoSerailizer.DeSerializeObject(vACFileName, objineinfo, vacstream);
                                }
                            }


                        });
                    }
                }
            }
            catch (Exception ex)
            {
                wBIdStateContent.MenuBarButtonState.IsEOM = false;
                throw ex;
            }


        }

        public static T ConvertJSonToObject<T>(string jsonString)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)serializer.ReadObject(ms);
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

        private Trip GetTrip(string pairing)
        {
            Trip trip = null;
            trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing.Substring(0, 4)).FirstOrDefault();
            if (trip == null)
            {
                trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing).FirstOrDefault();
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

