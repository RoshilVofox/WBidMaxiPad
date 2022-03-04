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

namespace WBid.WBidiPad.iOS
{
	public partial class BAViewController : UIViewController
    {
		WBidState wBIdStateContent=GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
        ConstraintCalculations constCalc = new ConstraintCalculations();
        WeightCalculation weightCalc = new WeightCalculation();
        SortCalculation sort = new SortCalculation();



		LoadingOverlay ActivityIndicator;
		//wBIdStateContent.bidaut SortDetails.BlokSort = new List<string>();
		ConstraintView constraintsVc;
		BAsortAndWeightViewController sAndWVC;
		ConstraintViewController constraintsView;
        NSObject notif;

        //		bool undoBtn;
        //		bool redoBtn;

		public BAViewController()
		: base("BAViewController", null)
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }
		public override void ViewWillDisappear (bool animated)
		{
          
            base.ViewWillDisappear (animated);

			//txtCellNumber.Dispose ();


		}

        public void UpdateSaveButton()
        {
			if (GlobalSettings.isModified != null) {
				btnSave.Enabled = GlobalSettings.isModified;
				GlobalSettings.isUndo = false;
				GlobalSettings.isRedo = false;
				//UpdateUndoRedoButtons ();
			}

        }



        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			//wBIdStateContent.BidAuto.BASort.BlokSort = new List<string>();

            btnSave.Enabled = GlobalSettings.isModified;
            //this.btnHome.SetBackgroundImage(UIImage.FromBundle ("homeIcon.png"), UIControlState.Normal);
            this.btnSave.SetBackgroundImage(UIImage.FromBundle("saveIconRed.png"), UIControlState.Normal);
            this.btnSave.SetBackgroundImage(UIImage.FromBundle("saveIcon.png"), UIControlState.Disabled);

            lblTitle.Text = "Bid Automator";
			//lblTitle.Text = WBidCollection.SetTitile();
            this.loadChildViews();
            this.vwSortAndWeights.Layer.BorderWidth = 1;
            vwSortAndWeights.Layer.BorderColor = ColorClass.SummaryHeaderBorderColor.CGColor;
            this.vwConstraints.Layer.BorderWidth = 1;
            vwConstraints.Layer.BorderColor = ColorClass.SummaryHeaderBorderColor.CGColor;
                   
			if (wBIdStateContent.BidAuto.BASort.SortColumn == null)
				wBIdStateContent.BidAuto.BASort.SortColumn = "";
				
			btnBidStuff.Layer.CornerRadius = (nfloat)5.0;
			//NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("ShowHelpView"), ShowHelpView);
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

//
        private void loadChildViews()
        {


			constraintsView = new ConstraintViewController();
			this.AddChildViewController(constraintsView);
			this.vwConstraints.AddSubview(constraintsView.View);


			sAndWVC = new BAsortAndWeightViewController();
            this.AddChildViewController(sAndWVC);
            this.vwSortAndWeights.AddSubview(sAndWVC.View);

        }
        partial void btnHomeTapped(UIKit.UIButton sender)
        {
//            SortCalculation sort = new SortCalculation();
//            WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
//            if (wBidStateContent.SortDetails != null && wBidStateContent.SortDetails.SortColumn != null && wBidStateContent.SortDetails.SortColumn != string.Empty)
//            {
//                sort.SortLines(wBidStateContent.SortDetails.SortColumn);
//            }
//
            //NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);
            NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
            this.NavigationController.DismissViewController(true, null);


			PerformSelector(new ObjCRuntime.Selector("memoryRelease"), null, 0.2);

		}
		[Export("memoryRelease")]
		void memoryRelease()
		{
			sAndWVC.memoryRelease ();
            sAndWVC.removeObservers ();
            constraintsView.constraintsCont.removeObservers ();
			foreach (UIView view in this.View.Subviews) {

				DisposeClass.DisposeEx(view);
			}
			this.View.Dispose ();
			this.View.UserInteractionEnabled = true;
		}
//        partial void btnSaveTapped(UIKit.UIButton sender)
//        {
//            //			CompareState stateObj = new CompareState();
//            //
//            //           string fileName= WBidHelper.GenerateFileNameUsingCurrentBidDetails();
//            //           var WbidCollection = XmlHelper.ReadStateFile(WBidHelper.GetAppDataPath() + "/" + fileName + ".WBS");
//            //           wBIdStateContent = WbidCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
//            //			bool isNochange = stateObj.CompareStateChange(wBIdStateContent, GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName));
//
//            if (GlobalSettings.isModified)
//            {
//                GlobalSettings.WBidStateCollection.IsModified = true;
//                StateManagement stateManagement = new StateManagement();
//                stateManagement.UpdateWBidStateContent();
//                WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);
//                //save the state of the INI File
//                WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
//                GlobalSettings.isModified = false;
//                btnSave.Enabled = false;
//            }
//        }

		partial void BtnHelpClicked (NSObject sender)
		{
			HelpViewController helpVC = new HelpViewController ();
			helpVC.pdfFileName = "Constraints";
			UINavigationController navCont = new UINavigationController (helpVC);
			navCont.NavigationBar.BarStyle = UIBarStyle.Black;
			navCont.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
			navCont.NavigationBar.Hidden = true;
			this.PresentViewController (navCont, true, null);
		}
//		public void ShowHelpView(NSNotification n)
//		{
//
//				HelpViewController helpVC = new HelpViewController ();
//				helpVC.pdfFileName = "Constraints";
//				UINavigationController navCont = new UINavigationController (helpVC);
//				navCont.NavigationBar.BarStyle = UIBarStyle.Black;
//				navCont.NavigationBar.Hidden = true;
//				this.PresentViewController (navCont, true, null);
//
//		}

        partial void btnBidStuffTapped(UIKit.UIButton sender)
        {



			// CalculateLinePropertiesForBAFilters
			try{
				

			
				bool flag1=false;
				bool flag2=false;

				if(SharedObject.Instance.ListConstraint.Count>0)
				{
					flag1=true;
				}

				if ((sAndWVC.sortTempValue.BlokSort.Count>0) || (sAndWVC.sortTempValue.SortColumn.Length >0))
				{
					flag2=true;
				}

				if(!flag1 || !flag2)
				{
                    UIAlertController okAlertController = UIAlertController.Create("WBidMax", "Bid Automator feature required atleast one filter and Sort", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);

                    return;
				}



				ActivityIndicator= new LoadingOverlay(View.Bounds, "Calculating data. \n Please wait..");
				this.View.Add (ActivityIndicator);

				InvokeInBackground(() =>
					{
						
						wBIdStateContent.BidAutoOn = true;
						BidAutoHelper.SetSelectedFiltersToStateObject(SharedObject.Instance.ListConstraint);

						wBIdStateContent.BidAuto.IsBlankBottom=sAndWVC.sortTempValue.IsBlankBottom;
						wBIdStateContent.BidAuto.IsReserveBottom=sAndWVC.sortTempValue.IsReserveBottom;
						wBIdStateContent.BidAuto.IsReserveFirst=sAndWVC.sortTempValue.IsReserveFirst;
						wBIdStateContent.BidAuto.BASort = new SortDetails();
						wBIdStateContent.BidAuto.BASort.SortColumn=sAndWVC.sortTempValue.SortColumn;
						wBIdStateContent.BidAuto.BASort.SortColumnName=sAndWVC.sortTempValue.SortColumnName;
						wBIdStateContent.BidAuto.BASort.SortDirection=sAndWVC.sortTempValue.SortDirection;
						wBIdStateContent.BidAuto.BASort.BlokSort=new List<string>();
						foreach(var item in sAndWVC.sortTempValue.BlokSort)
						{
							wBIdStateContent.BidAuto.BASort.BlokSort.Add(item);
						}

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

				if (wBIdStateContent.BidAuto != null && wBIdStateContent.BidAuto.BAGroup != null)
			{
					wBIdStateContent.BidAuto.BAGroup.Clear();
			}

			// Adding Selected Constraints to State Object
			// AddSelectedConstraintsToStateObject();

			// AddSelectedSortToStateObject();

			// Calculate Line properties value for BA Constraint
			//	InvokeInBackground (() => {
			var bACalculation = new BidAutomatorCalculations();

			bACalculation.CalculateLinePropertiesForBAFilters();
				


			//Apply COnstrint And Sort
			bACalculation.ApplyBAFilterAndSort();
				
//			IsneedToEnableCalculateBid = false;
//			IsSortChanged = false;

				if (wBIdStateContent.BidAuto != null && wBIdStateContent.BidAuto.BAFilter != null)
					wBIdStateContent.BidAuto.BAFilter.ForEach(x => x.IsApplied = true);
				

				//Setting Bid Automator settings to CalculatedBA state
				SetCurrentBADetailsToCalculateBAState();
			
				GlobalSettings.isModified=true;
					
				//NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
						InvokeOnMainThread(() =>
							{
				this.PerformSelector (new ObjCRuntime.Selector ("DismissView"), null, 0.3);
							});
				});
			

			//}
			}
		catch (Exception ex)
		{
			

		}

//            SortCalculation sort = new SortCalculation();
//            WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
//            if (wBidStateContent.SortDetails != null && wBidStateContent.SortDetails.SortColumn != null && wBidStateContent.SortDetails.SortColumn != string.Empty)
//            {
//                sort.SortLines(wBidStateContent.SortDetails.SortColumn);
//            }
//            NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);
//
//			string[] arr = { "Submit Current Bid Order", "Bid Editor", "View Cover Letter", "View Seniority List", "View Awards", "Get Awards", "Show Bid Reciept", "Print Bid Reciept" };
//            UIActionSheet sheet = new UIActionSheet("Select", null, "Cancel", null, arr);
//            sheet.ShowFrom(sender.Frame, this.tbTopBar, true);
//            sheet.Dismissed += handleBidStuffTap;

        }
		[Export ("DismissView")]
		void DismissView ()
		{

			ActivityIndicator.Hide();
			SharedObject.Instance.ListConstraint.Clear();
			NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
			this.NavigationController.DismissViewController(true, null);


			PerformSelector(new ObjCRuntime.Selector("memoryRelease1"), null, 0.2);

		}
		[Export("memoryRelease1")]
		void memoryRelease1()
		{
			foreach (UIView view in this.View.Subviews) {

				DisposeClass.DisposeEx(view);
			}
			this.View.Dispose ();
			this.View.UserInteractionEnabled = true;
		}
		/// <summary>
		/// Setting Bid Automator settings to CalculatedBA state
		/// </summary>
		private void SetCurrentBADetailsToCalculateBAState()
		{
			try
			{
				if (wBIdStateContent.BidAuto != null)
				{
					wBIdStateContent.CalculatedBA = new BidAutomator
					{
						IsBlankBottom = wBIdStateContent.BidAuto.IsBlankBottom,
						IsReserveBottom = wBIdStateContent.BidAuto.IsReserveBottom,
						IsReserveFirst = wBIdStateContent.BidAuto.IsReserveFirst
					};


					//Ba filter
					//---------------------------------------------------------------------------
					if (wBIdStateContent.BidAuto.BAFilter != null)
					{
						wBIdStateContent.CalculatedBA.BAFilter = new List<BidAutoItem>();
						SetCurrentBAFilterDetailsToCalculateBAFilterState();
					}
					//---------------------------------------------------------------------------
					//Ba Group object
					//---------------------------------------------------------------------------
					if (wBIdStateContent.BidAuto.BAGroup != null)
					{
						wBIdStateContent.CalculatedBA.BAGroup = new List<BidAutoGroup>();
						SetCurrentBAGroupDetailsToCalculateBAGroupState();

						// GlobalSettings.WBidStateContent.BidAuto.BAGroup = null;
					}
					//---------------------------------------------------------------------------

					//Sort object
					//---------------------------------------------------------------------------
					if (wBIdStateContent.BidAuto.BASort != null)
					{
						wBIdStateContent.CalculatedBA.BASort = new SortDetails
						{
							SortColumn = wBIdStateContent.BidAuto.BASort.SortColumn,
							SortColumnName = wBIdStateContent.BidAuto.BASort.SortColumnName,
							SortDirection = wBIdStateContent.BidAuto.BASort.SortDirection
						};
						//Block sort list
						// if (GlobalSettings.WBidStateContent.CalculatedBA.BASort.BlokSort != null)
						if (wBIdStateContent.BidAuto.BASort.BlokSort != null)
						{
							wBIdStateContent.CalculatedBA.BASort.BlokSort = new List<string>();
							foreach (var item in wBIdStateContent.BidAuto.BASort.BlokSort)
							{
								wBIdStateContent.CalculatedBA.BASort.BlokSort.Add(item);
							}
						}

					}
					//---------------------------------------------------------------------------




				}
			}
			catch (Exception ex)
			{
				
			}
		}

		private void SetCurrentBAFilterDetailsToCalculateBAFilterState()
		{
			try
			{
				foreach (var item in wBIdStateContent.BidAuto.BAFilter)
				{
					var calculatedItem = new BidAutoItem();
					calculatedItem.Name = item.Name;
					calculatedItem.Priority = item.Priority;
					calculatedItem.IsApplied = item.IsApplied;

					SetAutoObjectValueToCalculateBAFilter(item, calculatedItem);
					wBIdStateContent.CalculatedBA.BAFilter.Add(calculatedItem);
				}

			}
			catch (Exception ex)
			{

			}

		}
		private void SetCurrentBAGroupDetailsToCalculateBAGroupState()
		{

			foreach (var item in wBIdStateContent.BidAuto.BAGroup)
			{
				wBIdStateContent.CalculatedBA.BAGroup.Add(new BidAutoGroup { GroupName = item.GroupName, Lines = item.Lines });
			}

		}
		private void SetAutoObjectValueToCalculateBAFilter(BidAutoItem item, BidAutoItem calculatedItem)
		{
			try
			{
				Cx3Parameter cx3Parameter;
				Cx3Parameter calculateCx3Parameter;
				CxDays cxDay;
				CxDays calculateCxDays;
				switch (calculatedItem.Name)
				{

				//-----------------------------------------------------------------------
				case "AP":
					var amPmConstriants = (AMPMConstriants)item.BidAutoObject;
					var calculateAmPmConstriants = new AMPMConstriants
					{
						AM = amPmConstriants.AM,
						MIX = amPmConstriants.MIX,
						PM = amPmConstriants.PM
					};
					calculatedItem.BidAutoObject = calculateAmPmConstriants;
					break;
					//-----------------------------------------------------------------------
				case "CL":
					var ftCommutableLine = (FtCommutableLine)item.BidAutoObject;
					var calculateFtCommutableLine = new FtCommutableLine
					{
						BaseTime = ftCommutableLine.BaseTime,
						CheckInTime = ftCommutableLine.CheckInTime,
						City = ftCommutableLine.City,
						CommuteCity = ftCommutableLine.CommuteCity,
						ConnectTime = ftCommutableLine.ConnectTime,
						NoNights = ftCommutableLine.NoNights,
						ToHome = ftCommutableLine.NoNights,
						ToWork = ftCommutableLine.ToWork,
						IsNonStopOnly=ftCommutableLine.IsNonStopOnly
					};
					calculatedItem.BidAutoObject = calculateFtCommutableLine;
					break;
					//-----------------------------------------------------------------------
				case "DOM":
					var daysOfMonthCx = (DaysOfMonthCx)item.BidAutoObject;
					var calculateDaysOfMonthCx = new DaysOfMonthCx();
					if (daysOfMonthCx.OFFDays != null)
					{
						calculateDaysOfMonthCx.OFFDays = new List<int>();
						foreach (var offDay in daysOfMonthCx.OFFDays)
						{
							calculateDaysOfMonthCx.OFFDays.Add(offDay);
						}
					}
					if (daysOfMonthCx.WorkDays != null)
					{
						calculateDaysOfMonthCx.WorkDays = new List<int>();
						foreach (var workDay in daysOfMonthCx.WorkDays)
						{
							calculateDaysOfMonthCx.WorkDays.Add(workDay);
						}
					}
					calculatedItem.BidAutoObject = calculateDaysOfMonthCx;
					break;
					//-----------------------------------------------------------------------
				case "DOWA":
					cxDay = (CxDays)item.BidAutoObject;
					calculateCxDays = new CxDays
					{
						IsFri = cxDay.IsFri,
						IsMon = cxDay.IsMon,
						IsSat = cxDay.IsSat,
						IsSun = cxDay.IsSun,
						IsThu = cxDay.IsThu,
						IsTue = cxDay.IsTue,
						IsWed = cxDay.IsWed
					};
					calculatedItem.BidAutoObject = calculateCxDays;
					break;
					//-----------------------------------------------------------------------
				case "DOWS":
					cx3Parameter = (Cx3Parameter)item.BidAutoObject;
					calculateCx3Parameter = new Cx3Parameter
					{
						ThirdcellValue = cx3Parameter.ThirdcellValue,
						Type = cx3Parameter.Type,
						Value = cx3Parameter.Value
					};
					calculatedItem.BidAutoObject = calculateCx3Parameter;
					break;
					//-----------------------------------------------------------------------
				case "DHFL":
					cx3Parameter = (Cx3Parameter)item.BidAutoObject;
					calculateCx3Parameter = new Cx3Parameter
					{
						ThirdcellValue = cx3Parameter.ThirdcellValue,
						Type = cx3Parameter.Type,
						Value = cx3Parameter.Value
					};
					calculatedItem.BidAutoObject = calculateCx3Parameter;
					break;
					//-----------------------------------------------------------------------
				case "ET":
					cx3Parameter = (Cx3Parameter)item.BidAutoObject;
					calculateCx3Parameter = new Cx3Parameter
					{
						ThirdcellValue = cx3Parameter.ThirdcellValue,
						Type = cx3Parameter.Type,
						Value = cx3Parameter.Value
					};
					calculatedItem.BidAutoObject = calculateCx3Parameter;
					break;
					//-----------------------------------------------------------------------
				case "LT":
					var lineTypeItem = (CxLine)item.BidAutoObject;
					var calculateCxLine = new CxLine
					{
						Blank = lineTypeItem.Blank,
						Hard = lineTypeItem.Hard,
						International = lineTypeItem.International,
						NonConus = lineTypeItem.NonConus,
						Ready = lineTypeItem.Ready,
						Reserve = lineTypeItem.Reserve
					};
					calculatedItem.BidAutoObject = calculateCxLine;
					break;
					//-----------------------------------------------------------------------
				case "OC":

					var bulkOvernightCityCx = (BulkOvernightCityCx)item.BidAutoObject;
					var calculateBulkOvernightCityCx = new BulkOvernightCityCx();
					if (bulkOvernightCityCx.OverNightNo != null)
					{
						calculateBulkOvernightCityCx.OverNightNo = new List<int>();
						foreach (var overNightNo in bulkOvernightCityCx.OverNightNo)
						{
							calculateBulkOvernightCityCx.OverNightNo.Add(overNightNo);
						}
					}
					if (bulkOvernightCityCx.OverNightYes != null)
					{
						calculateBulkOvernightCityCx.OverNightYes = new List<int>();
						foreach (var overNightYes in bulkOvernightCityCx.OverNightYes)
						{
							calculateBulkOvernightCityCx.OverNightYes.Add(overNightYes);
						}
					}
					calculatedItem.BidAutoObject = calculateBulkOvernightCityCx;
					break;
					//-----------------------------------------------------------------------
				case "RT":
					cx3Parameter = (Cx3Parameter)item.BidAutoObject;
					calculateCx3Parameter = new Cx3Parameter
					{
						ThirdcellValue = cx3Parameter.ThirdcellValue,
						Type = cx3Parameter.Type,
						Value = cx3Parameter.Value
					};
					calculatedItem.BidAutoObject = calculateCx3Parameter;
					break;
					//-----------------------------------------------------------------------
				case "SDOW":
					cxDay = (CxDays)item.BidAutoObject;
					calculateCxDays = new CxDays
					{
						IsFri = cxDay.IsFri,
						IsMon = cxDay.IsMon,
						IsSat = cxDay.IsSat,
						IsSun = cxDay.IsSun,
						IsThu = cxDay.IsThu,
						IsTue = cxDay.IsTue,
						IsWed = cxDay.IsWed
					};
					calculatedItem.BidAutoObject = calculateCxDays;
					break;
					//-----------------------------------------------------------------------
				case "TBL":
					var tripBlockLengthItem = (CxTripBlockLength)item.BidAutoObject;
					var calculateCxTripBlockLength = new CxTripBlockLength
					{
						FourDay = tripBlockLengthItem.FourDay,
						IsBlock = tripBlockLengthItem.IsBlock,
						ThreeDay = tripBlockLengthItem.ThreeDay,
						Turns = tripBlockLengthItem.Turns,
						Twoday = tripBlockLengthItem.Twoday
					};
					calculatedItem.BidAutoObject = calculateCxTripBlockLength;
					break;
					//-----------------------------------------------------------------------


				}
			}
			catch (Exception ex)
			{
				
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
                    this.PresentViewController(faBid, true, null);
                }

            }
            NSNotificationCenter.DefaultCenter.RemoveObserver(notif);
        }


    }
}

