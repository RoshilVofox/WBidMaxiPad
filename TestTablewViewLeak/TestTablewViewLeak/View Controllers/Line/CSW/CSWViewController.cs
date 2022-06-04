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
using WBid.WBidiPad.PortableLibrary;
using System.IO;
using EventKit;
using WBid.WBidiPad.SharedLibrary.Utility;
using WBid.WBidiPad.Core.Enum;

namespace WBid.WBidiPad.iOS
{
    public partial class CSWViewController : UIViewController
    {
        WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
        ConstraintCalculations constCalc = new ConstraintCalculations();
        WeightCalculation weightCalc = new WeightCalculation();
        SortCalculation sort = new SortCalculation();
		UIPopoverController popoverController;
        ConstraintsViewController constraintsVc;
        sortAndWeightViewController sAndWVC;
        NSObject notif;
		NSObject ObserverCommute1;
		NSObject ObserverCommute2;
        NSObject ObserverCommuteauto;
        NSObject ObserverCommutemanual;
        //NSObject ObserverCommutability3;

        //		bool undoBtn;
        //		bool redoBtn;

        public CSWViewController()
            : base("CSWViewController", null)
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }


        public void UpdateSaveButton()
        {
			try{
				btnSave.Enabled = GlobalSettings.isModified;
				GlobalSettings.isUndo = false;
				GlobalSettings.isRedo = false;
				UpdateUndoRedoButtons ();

			}
			catch (Exception ex)
			{
				throw ex;
			}
        }


		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

        }
		public override void ViewDidDisappear (bool animated)
		{
//            constraintsVc.removeObservers ();

			base.ViewDidDisappear (animated);
		

		}


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ObserverCommute1 = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ShowCommutableAuto"), PushViewControllView);
            ObserverCommuteauto = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ShowCommutableAutoSortPopUp"), PushViewCommuteAutoSortControllView);
            ObserverCommutemanual = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ShowCommutableManualSortPopUp"), PushViewCommutableManualSortControllerView);
            ObserverCommute2 = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ShowCommutableWeightAuto"), PushWeightAutoViewControllView);

            btnSave.Enabled = GlobalSettings.isModified;
            this.btnSave.SetBackgroundImage(UIImage.FromBundle("saveIconRed.png"), UIControlState.Normal);
            this.btnSave.SetBackgroundImage(UIImage.FromBundle("saveIcon.png"), UIControlState.Disabled);


            this.reduButtonView.Frame = new CGRect(0.0, 30.0, 30.0, 20.0);
            this.undoButtonView.Frame = new CGRect(0.0, 30.0, 30.0, 20.0);
            this.btnSave.Frame = new CGRect(0.0, 30.0, 30.0, 30.0);

            lblTitle.Text = WBidCollection.SetTitile();

            this.loadChildViews();
            this.vwSortAndWeights.Layer.BorderWidth = 1;
            vwSortAndWeights.Layer.BorderColor = ColorClass.SummaryHeaderBorderColor.CGColor;
            this.vwConstraints.Layer.BorderWidth = 1;
            vwConstraints.Layer.BorderColor = ColorClass.SummaryHeaderBorderColor.CGColor;
            btnUndo.TouchUpInside += btnUndoTapped;
            btnRedo.TouchUpInside += btnRedoTapped;
            UpdateUndoRedoButtons();

        }


        private bool checkToRecreateMILFile(List<Absense> lstPreviosusMIL,List<Absense> lstCurrentMIL)
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
        void btnRedoTapped(object sender, EventArgs e)
        {
            if (GlobalSettings.RedoStack.Count > 0)
            {
                var state = GlobalSettings.RedoStack[0];
				bool isNeedtoRecreateMILFile = false;
				if(state.MILDateList!=null && wBIdStateContent.MILDateList!=null)
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
					GlobalSettings.MILDates = WBidCollection.GenarateOrderedMILDates( wBIdStateContent.MILDateList);
					GlobalSettings.MILData = CommonClass.lineVC.CreateNewMILFile().MILValue;

				}
                StateManagement statemanagement = new StateManagement();
                bool isneedtoChangeLineFile = statemanagement.IsneedToChangeLineFile(state);
                state.IsOverlapCorrection = false;
                stateManagement.SetMenuBarButtonStatusFromStateFile(state);
                //Setting  status to Global variables
                stateManagement.SetVacationOrOverlapExists(state);
                if (isneedtoChangeLineFile)
                {
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
                            constraintsVc.View.RemoveFromSuperview();
                            constraintsVc = null;
                            sAndWVC.View.RemoveFromSuperview();
                            sAndWVC = null;
                            loadChildViews();

                        });
                    });
                }
                else
                {
                    stateManagement.ReloadStateContent(state);

                    constraintsVc.removeObservers();
                    sAndWVC.removeObservers();

                    constraintsVc.View.RemoveFromSuperview();
                    constraintsVc = null;
                    sAndWVC.View.RemoveFromSuperview();
                    sAndWVC = null;
                    loadChildViews();
                }
               /* //bool isNeedToRecalculateLineProp = stateManagement.CheckLinePropertiesNeedToRecalculate(state);
                //ResetLinePropertiesBackToNormal(stateContent, state);
                //ResetOverlapState(stateContent, state);

                stateManagement.SetMenuBarButtonStatusFromStateFile(state);
                //Setting  status to Global variables
                stateManagement.SetVacationOrOverlapExists(state);

                // SetVacButtonStates();

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
                            constraintsVc.View.RemoveFromSuperview();
                            constraintsVc = null;
                            sAndWVC.View.RemoveFromSuperview();
                            sAndWVC = null;
                            loadChildViews();

                        });
                    });
                }
                else
                {

                    stateManagement.ReloadStateContent(state);

                    constraintsVc.removeObservers ();
                    sAndWVC.removeObservers ();

                    constraintsVc.View.RemoveFromSuperview();
                    constraintsVc = null;
                    sAndWVC.View.RemoveFromSuperview();
                    sAndWVC = null;
                    loadChildViews();
                }
               */
            }

            GlobalSettings.isUndo = false;
            GlobalSettings.isRedo = true;
            UpdateUndoRedoButtons();
            GlobalSettings.isModified = true;
            btnSave.Enabled = GlobalSettings.isModified;
        }

        private void ResetLinePropertiesBackToNormal(WBidState currentState, WBidState newState)
        {

            //RoshilVofox need to check this
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



        void btnUndoTapped(object sender, EventArgs e)
        {
            if (GlobalSettings.UndoStack.Count > 0)
            {
                WBidState state = GlobalSettings.UndoStack[0];

				bool isNeedtoRecreateMILFile = false;
				if(state.MILDateList!=null && wBIdStateContent.MILDateList!=null)
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

                GlobalSettings.UndoStack.RemoveAt(0);
				if (isNeedtoRecreateMILFile)
				{
					GlobalSettings.MILDates = WBidCollection.GenarateOrderedMILDates( wBIdStateContent.MILDateList);
					GlobalSettings.MILData = CommonClass.lineVC.CreateNewMILFile().MILValue;

				}
                //  StateManagement stateManagement = new StateManagement();
                // stateManagement.ReloadDataFromStateFile();

                StateManagement statemanagement = new StateManagement();
                bool isneedtoChangeLineFile = statemanagement.IsneedToChangeLineFile(state);
                state.IsOverlapCorrection = false;
                stateManagement.SetMenuBarButtonStatusFromStateFile(state);
                //Setting  status to Global variables
                stateManagement.SetVacationOrOverlapExists(state);
                if (isneedtoChangeLineFile)
                {
                    var loadingOverlay = new LoadingOverlay(View.Bounds, "Please Wait..");
                    View.Add(loadingOverlay);
                    InvokeInBackground(() =>
                    {
                        string isSuccess = WBidHelper.RetrieveSaveAndSetLineFiles(0, state);
                        bool IsFileAvailable = (isSuccess == "Ok") ? true : false;
                        
                        loadingOverlay.Hide();
                        stateManagement.ReloadStateContent(state);
                        constraintsVc.View.RemoveFromSuperview();
                        constraintsVc = null;
                        sAndWVC.View.RemoveFromSuperview();
                        sAndWVC = null;
                        loadChildViews();
                    });
                }
                else
                {
                    stateManagement.ReloadStateContent(state);
                    constraintsVc.removeObservers();
                    constraintsVc.View.RemoveFromSuperview();
                    constraintsVc = null;
                    sAndWVC.removeObservers();
                    sAndWVC.View.RemoveFromSuperview();
                    sAndWVC = null;
                    loadChildViews();
                }
               /*             bool isNeedToRecalculateLineProp = stateManagement.CheckLinePropertiesNeedToRecalculate(state);
                ResetLinePropertiesBackToNormal(stateContent, state);
                ResetOverlapState(stateContent, state);

               

                stateManagement.SetMenuBarButtonStatusFromStateFile(state);
                //Setting  status to Global variables
                stateManagement.SetVacationOrOverlapExists(state);

                //SetVacButtonStates();

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
                            constraintsVc.View.RemoveFromSuperview();
                            constraintsVc = null;
                            sAndWVC.View.RemoveFromSuperview();
                            sAndWVC = null;
                            loadChildViews();

                        });
                    });
                }
                else
                {

                    stateManagement.ReloadStateContent(state);
                    //				NSNotificationCenter.DefaultCenter.PostNotificationName ("AddConstraints", null);
                    //				NSNotificationCenter.DefaultCenter.PostNotificationName ("LineCountReload", null);
                    //				NSNotificationCenter.DefaultCenter.PostNotificationName ("AddWeights", null);
                    //				NSNotificationCenter.DefaultCenter.PostNotificationName ("reloadBlockSort", null);
					constraintsVc.removeObservers();
                    constraintsVc.View.RemoveFromSuperview();
                    constraintsVc = null;
					sAndWVC.removeObservers ();
                    sAndWVC.View.RemoveFromSuperview();
                    sAndWVC = null;
                    loadChildViews();
                }
               */
            }

            GlobalSettings.isUndo = true;
            GlobalSettings.isRedo = false;
            UpdateUndoRedoButtons();
            GlobalSettings.isModified = true;
            btnSave.Enabled = GlobalSettings.isModified;
        }
		public void PushViewControllView(NSNotification n)
		{
			this.PerformSelector (new ObjCRuntime.Selector("ViewShow"), null, 0.5);
        }
        public void PushViewCommuteAutoSortControllView(NSNotification n)
        {
            this.PerformSelector(new ObjCRuntime.Selector("CommutableAutoSortViewShow"), null, 0.5);
        }

        public void PushViewCommutableManualSortControllerView(NSNotification notif)
        {
            this.PerformSelector(new ObjCRuntime.Selector("CommutableManualSortViewShow"), null, .5); 
        }
        public void PushWeightAutoViewControllView(NSNotification n)
        {
            this.PerformSelector(new ObjCRuntime.Selector("WeightViewShow"), null, 0.5);
        }
        //public void PushSortControllViewCommutability(NSNotification n)
        //{
        //    this.PerformSelector(new ObjCRuntime.Selector("ViewShowSort"), null, 0.5);
        //}

		[Export("ViewShow")]
		void ViewShow()
		{
			var cl = new FtCommutableLine ();

			if (wBIdStateContent.CxWtState.CLAuto == null)
				wBIdStateContent.CxWtState.CLAuto = new StateStatus{ Wt = false, Cx = false };

			if (!wBIdStateContent.CxWtState.CLAuto.Cx) {
				cl = new FtCommutableLine () {
					ToHome = true,
					ToWork = false,
					NoNights = false,
					BaseTime = 10,
					ConnectTime = 30,
					CheckInTime = 120
				};
			} else
				cl = wBIdStateContent.Constraints.CLAuto;

			UIStoryboard storyboard = UIStoryboard.FromName ("Main", null);
			CommuteLinesViewController cmtView = storyboard.InstantiateViewController ("CommuteLinesViewController")as CommuteLinesViewController;
			cmtView.data1 = cl;
			cmtView.ObjFromView = CommuteFromView.CSWConstraints;
            //cmtView.ObjChangeController = this;
            cmtView.PreferredContentSize = new CGSize(320, 320);
            cmtView.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;

           
				
			this.NavigationController.PresentViewController(cmtView, true,null);

//			NavigationController.PresentViewController (cmtView, true,null);
		}
        /// <summary>
        /// Open the commutability auto pop up when user tries to add commutability auto block sort
        /// </summary>
        [Export("CommutableAutoSortViewShow")]
        void CommutableAutoSortViewShow()
        {
            var cl = new FtCommutableLine();

            if (wBIdStateContent.CxWtState.CLAuto == null)
                wBIdStateContent.CxWtState.CLAuto = new StateStatus { Wt = false, Cx = false };
            
            if (wBIdStateContent.CxWtState.CLAuto.Cx == false && wBIdStateContent.CxWtState.CLAuto.Wt == false && !((wBIdStateContent.SortDetails.BlokSort.Contains("33") || wBIdStateContent.SortDetails.BlokSort.Contains("34") || wBIdStateContent.SortDetails.BlokSort.Contains("35"))))
             {
                cl = new FtCommutableLine()
                {
                    ToHome = true,
                    ToWork = false,
                    NoNights = false,
                    BaseTime = 10,
                    ConnectTime = 30,
                    CheckInTime = 120
                };
            }
            else
                cl = wBIdStateContent.Constraints.CLAuto;

            UIStoryboard storyboard = UIStoryboard.FromName("Main", null);
            CommuteLinesViewController cmtView = storyboard.InstantiateViewController("CommuteLinesViewController") as CommuteLinesViewController;
            if (this.NavigationController != null)
            {
                cmtView.data1 = cl;
                cmtView.ObjFromView = CommuteFromView.CSWCommutableAutoSort;
                cmtView.PreferredContentSize = new CGSize(320, 320);
                cmtView.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                this.NavigationController.PresentViewController(cmtView, true, null);
            }
            else
            {
                
            }


        }
        [Export("CommutableManualSortViewShow")]
        internal void CommutableManualSortView()
        {
            var commuteLine = new CxCommutableLine(); 
            try
            {
                if (wBIdStateContent.CxWtState.CL == null)
                {
                    wBIdStateContent.CxWtState.CL = new StateStatus() { Cx = false, Wt = false };
                }

                if (!wBIdStateContent.CxWtState.CL.Cx && !wBIdStateContent.CxWtState.CL.Wt)
                {
                    commuteLine = new CxCommutableLine()
                    {
                        CommuteToHome = true,
                        CommuteToWork = false,
                        AnyNight = false,
                        RunBoth= false

                    };
                }
                else
                {
                    commuteLine = wBIdStateContent.Constraints.CL; 
                }

                var storyBoard = UIStoryboard.FromName("Main", null);
                var commuteManualSortView = storyBoard.InstantiateViewController("CommutableManualBlockSortViewController") as CommutableManualBlockSortViewController;
                if (this.NavigationController != null)
                {
                    commuteManualSortView.CommutableLine = commuteLine;
                    commuteManualSortView.CommuteType = CommuteFromView.CSWCommutableManualSort;
                    commuteManualSortView.PreferredContentSize = new CGSize(500, 380);
                    commuteManualSortView.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                    this.NavigationController.PresentViewController(commuteManualSortView, true, null);
                }
                else
                {
                    
                }


            }
            catch(Exception e)
            {
                throw e; 
            }
        }

        [Export("WeightViewShow")]
		void WeightViewShow()
		{
			var cl = new WtCommutableLineAuto ();

			if (wBIdStateContent.CxWtState.CLAuto == null)
				wBIdStateContent.CxWtState.CLAuto = new StateStatus{ Wt = false, Cx = false };


			if (!wBIdStateContent.CxWtState.CLAuto.Wt)
            {
                cl = new WtCommutableLineAuto()
                {
                    ToHome = true,
                    ToWork = false,
                    NoNights = false,
                    BaseTime = 10,
                    ConnectTime = 30,
                    CheckInTime = 60
                };
            }
			else cl=wBIdStateContent.Weights.CLAuto;

			UIStoryboard storyboard = UIStoryboard.FromName ("Main", null);
			CommuteLinesViewController cmtView = storyboard.InstantiateViewController ("CommuteLinesViewController")as CommuteLinesViewController;
			cmtView.data1 = cl;
			cmtView.ObjFromView = CommuteFromView.CSWWeight;
            //cmtView.ObjChangeController = this;
            cmtView.PreferredContentSize = new CGSize(320, 320);
            cmtView.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
			//this.PresentViewController(cmtView, true,null);
			this.NavigationController.PresentViewController(cmtView, true,null);
			//NavigationController.PresentViewController (cmtView, true,null);
		}

		

        void UpdateUndoRedoButtons()
        {
			try
			{
			if (GlobalSettings.UndoStack != null && GlobalSettings.RedoStack != null) {
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

			}
			catch(Exception ex) {
				throw ex;
			}
        }

        private void loadChildViews()
        {
            constraintsVc = new ConstraintsViewController();
             //new CGRect(0, 0, 0, 0);

            this.AddChildViewController(constraintsVc);
            this.vwConstraints.AddSubview(constraintsVc.View);
            constraintsVc.View.Frame = new CGRect(0, 0, this.vwConstraints.Frame.Width, this.vwConstraints.Frame.Height);

            sAndWVC = new sortAndWeightViewController();

            this.AddChildViewController(sAndWVC);
            this.vwSortAndWeights.AddSubview(sAndWVC.View);
            sAndWVC.View.Frame = new CGRect(0, 0, this.vwConstraints.Frame.Width, this.vwSortAndWeights.Frame.Height);//this.vwSortAndWeights.Bounds;//new CGRect(0, 0, this.vwSortAndWeights.Frame.Width, this.vwSortAndWeights.Frame.Height);


        }
        partial void btnHomeTapped(UIKit.UIButton sender)
        {
            SortCalculation sort = new SortCalculation();
            WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            if (wBidStateContent.SortDetails != null && wBidStateContent.SortDetails.SortColumn != null && wBidStateContent.SortDetails.SortColumn != string.Empty)
            {
                sort.SortLines(wBidStateContent.SortDetails.SortColumn);
            }
            //added by Roshil Aug 2020
            CommonClass.lineVC.UpdateSaveButton();
            //NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);
            NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
            NSNotificationCenter.DefaultCenter.PostNotificationName("SetFlightDataDifferenceButton", null);
            this.NavigationController.DismissViewController(true, null);

            PerformSelector(new ObjCRuntime.Selector("memoryRelease"), null, 0.2);

        }
        [Export("memoryRelease")]
        public void memoryRelease()
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(ObserverCommute1);
            NSNotificationCenter.DefaultCenter.RemoveObserver(ObserverCommute2);
            NSNotificationCenter.DefaultCenter.RemoveObserver(ObserverCommuteauto);
            NSNotificationCenter.DefaultCenter.RemoveObserver(ObserverCommutemanual);


            constraintsVc.removeObservers();
            sAndWVC.removeObservers();

            foreach (UIView view in this.View.Subviews) {

				DisposeClass.DisposeEx(view);
			}
			this.View.Dispose ();


		}
//		public override void ViewWillUnload ()
//		{
//			base.ViewWillUnload ();
//			memoryRelease();
//		}
        partial void btnSaveTapped(UIKit.UIButton sender)
        {
          
            if (GlobalSettings.isModified)
            {
                GlobalSettings.WBidStateCollection.IsModified = true;
                StateManagement stateManagement = new StateManagement();
                stateManagement.UpdateWBidStateContent();
                WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);
                //save the state of the INI File
                WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
                GlobalSettings.isModified = false;
                btnSave.Enabled = false;
            }
        }
        partial void btnResetTapped(UIKit.UIButton sender)
        {
            UIActionSheet sheet = new UIActionSheet("This action will remove all top locks, bottom locks, constraints, weights, and set the sort to Line Number.  Do you want to continue?", null, null, "YES", null);
            CGRect senderframe = sender.Superview.Frame;
            senderframe.X = sender.Superview.Frame.GetMidX();
            sheet.ShowFrom(senderframe, this.tbTopBar, true);
            sheet.Clicked += HandleResetAll;

        }

        void HandleResetAll(object sender, UIButtonEventArgs e)
        {
            if (e.ButtonIndex == 0)
            {
                StateManagement stateManagement = new StateManagement();                 stateManagement.UpdateWBidStateContent();                 WBidHelper.PushToUndoStack();                  LineOperations.RemoveAllTopLock();                 LineOperations.RemoveAllBottomLock();                 CommonClass.selectedRows.Clear();                 wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);                  wBIdStateContent.SortDetails.SortColumn = "Line";                 wBIdStateContent.SortDetails.BlokSort = new List<string>();                 CommonClass.columnID = 0;                  constCalc.ClearConstraints();                 ConstraintsApplied.clearAll();                  weightCalc.ClearWeights();                 WeightsApplied.clearAll();                  sort.SortLines("Line");                 constraintsVc.removeObservers();                 sAndWVC.removeObservers();                 constraintsVc.View.RemoveFromSuperview();                 constraintsVc = null;                 sAndWVC.View.RemoveFromSuperview();                 sAndWVC = null;                 this.loadChildViews();                  NSString str = new NSString("none");                 NSNotificationCenter.DefaultCenter.PostNotificationName("ButtonEnableDisable", str);                 NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);                 //NSNotificationCenter.DefaultCenter.PostNotificationName ("DataCulumnsUpdated", null);                 GlobalSettings.isModified = true;                 CommonClass.cswVC.UpdateSaveButton();
            }
        }

       
        partial void btnMiscTapped(UIKit.UIButton sender)
        {
			string[] arr = { "Configuration", "Change User Information", "Latest News", "My Subscription" };
            UIActionSheet sheet = new UIActionSheet("Select", null, "Cancel", null, arr);
            CGRect senderframe = sender.Superview.Frame;
            senderframe.X = sender.Superview.Frame.GetMidX();
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
                    this.PresentViewController(fileViewer, true, () =>
                    {
                        fileViewer.LoadPDFdocument("news.pdf");
                    });
                }
                else
                {
               
                    UIAlertController okAlertController = UIAlertController.Create("Error", "No latest News found!", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                }
            }
			else if (e.ButtonIndex == 3) 
			{
				SubscriptionViewController ObjSubscription = new SubscriptionViewController ();
				ObjSubscription.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				this.PresentViewController (ObjSubscription, true, null);

			}
        }

        partial void btnHelpTapped(UIKit.UIButton sender)
        {
            string[] arr = { "Help", "Walkthrough", "Contact Us", "About" };
            UIActionSheet sheet = new UIActionSheet("Select", null, "Cancel", null, arr);
            CGRect senderframe = sender.Superview.Frame;
            senderframe.X = sender.Superview.Frame.GetMidX();
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

        partial void btnBidStuffTapped(UIKit.UIButton sender)
        {
            SortCalculation sort = new SortCalculation();
            WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            if (wBidStateContent.SortDetails != null && wBidStateContent.SortDetails.SortColumn != null && wBidStateContent.SortDetails.SortColumn != string.Empty)
            {
                sort.SortLines(wBidStateContent.SortDetails.SortColumn);
            }
            NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);

            string[] arr;
            if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                arr =new string[] { "Submit Current Bid Order", "Bid Editor", "View Cover Letter", "View Seniority List", "View Awards", "Get Awards", "Show Bid Reciept", "Print Bid Reciept" };
            else
                arr = new string[]{ "Submit Current Bid Order", "Bid Editor", "View Cover Letter", "View Seniority List", "View Awards", "Get Awards", "Show Bid Reciept", "Print Bid Reciept","Show CAP" };
            UIActionSheet sheet = new UIActionSheet("Select", null, "Cancel", null, arr);
            CGRect senderframe = sender.Superview.Frame;
            senderframe.X = sender.Superview.Frame.GetMidX();
            sheet.ShowFrom(senderframe, this.tbTopBar, true);
            sheet.Dismissed += handleBidStuffTap;

        }

        public void handleBidStuffTap(object sender, UIButtonEventArgs e)
        {
            if (e.ButtonIndex == 0)
            {
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
					if (!isBlankLinesCorrectOrder) {
					
                        UIAlertController alert = UIAlertController.Create("WbidMax", "Your blank lines are not in order of lowest to highest. Touch Cancel to go back and fix this issue.?", UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (actionCancel) => {

                        }));

                        alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {
                            SubmitBidViewController submitBid = new SubmitBidViewController();
                            submitBid.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                            UINavigationController nav = new UINavigationController(submitBid);
                            nav.NavigationBar.BarTintColor = ColorClass.TopHeaderColor;
                            nav.NavigationBar.TitleTextAttributes = new UIStringAttributes() { ForegroundColor = UIColor.White };
                            nav.NavigationBar.TintColor = UIColor.White;
                            nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                            this.PresentViewController(nav, true, null);

                        }));

                        this.PresentViewController(alert, true, null);


                    } else {
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
				} else {

                    UIAlertController okAlertController = UIAlertController.Create("WBidMax", "You can not submit bids for Historical data", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                }
            }
            else if (e.ButtonIndex == 1)
            {
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
					if (!isBlankLinesCorrectOrder) {
                        UIAlertController alert = UIAlertController.Create("WbidMax", "Your blank lines are not in order of lowest to highest. Touch Cancel to go back and fix this issue.?", UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (actionCancel) => {

                        }));

                        alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {
                            bidEditorPrepViewController bidEditor = new bidEditorPrepViewController();
                            bidEditor.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                            UINavigationController nav = new UINavigationController(bidEditor);
                            nav.NavigationBarHidden = true;
                            nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                            this.PresentViewController(nav, true, () => {
                                notif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("bidEditorPrepared"), bidEditorPrepared);

                            });

                        }));

                        this.PresentViewController(alert, true, null);
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
				} else {

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
			else if (e.ButtonIndex == 6) {
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
				
                    UIAlertController okAlertController = UIAlertController.Create("WBidMax", "There is no bid receipt available..!", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                }
			}
			else if (e.ButtonIndex == 7) 
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
					
                    UIAlertController okAlertController = UIAlertController.Create("WBidMax", "There is no bid receipt available..!", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                }
			}
            else if (e.ButtonIndex == 8) {
                if (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO") {
                    if (Reachability.CheckVPSAvailable()) {
                        CAPViewController capdetails = new CAPViewController ();
                        capdetails.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                        UINavigationController nav = new UINavigationController (capdetails);
                        nav.NavigationBarHidden = true;
                        nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                        this.PresentViewController (nav, true, null);
                    } else {

                        UIAlertController okAlertController = UIAlertController.Create("WBidMax", Constants.VPSDownAlert, UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);
                    }
                }
            }
        }

        public void bidEditorPrepared(NSNotification n)
        {
            string selectedPosition = n.Object.ToString();
            if (selectedPosition == "CP" || selectedPosition == "FO")
            {
                BidEditorForPilot pilotBid = new BidEditorForPilot();
                this.PresentViewController(pilotBid, true, null);
            }
            else if (selectedPosition == "FA")
            {
                if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                {

                    BidEditorForFA faBid = new BidEditorForFA();
                    faBid.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    this.PresentViewController(faBid, true, null);
                }

            }
            NSNotificationCenter.DefaultCenter.RemoveObserver(notif);
        }


    }
}

