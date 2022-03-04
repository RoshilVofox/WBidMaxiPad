using System;
using CoreGraphics;
using Foundation;
using UIKit;
using System.Collections.Generic;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using System.Linq;
using WBid.WBidiPad.iOS.Utility;

namespace WBid.WBidiPad.iOS
{
	public partial class ConstraintsViewController : UIViewController
	{
		UIPopoverController popoverController;
		ConstraintsTableController constraintsCont;
		List <NSObject> arrObserver = new List <NSObject> ();
		ConstraintCalculations constCalc = new ConstraintCalculations ();
        WBidState wBIdStateContent;
		public ConstraintsViewController () : base ("ContraintsViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
            // Releases the view teamif it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.observeNotification ();
			this.setGraphics ();
			this.loadConstraintsList ();
//			this.observeNotification ();
			setValuesToFixedContraints ();
			listSavedConstraints ();
			//setLineTitle ();
			this.lblLinesNum.Text = constCalc.LinesNotConstrained ();
			vwMainConst.BackgroundColor = ColorClass.ListSeparatorColor;
			btnHelpIcon.TouchUpInside += delegate {
				HelpViewController helpVC = new HelpViewController ();
				helpVC.pdfFileName = "Constraints";
				UINavigationController navCont = new UINavigationController (helpVC);
				navCont.NavigationBar.BarStyle = UIBarStyle.Black;
				navCont.NavigationBar.Hidden = true;
				navCont.ModalPresentationStyle = UIModalPresentationStyle.Custom;
				this.PresentViewController (navCont, true, null);
			};
		}
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

		}
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		
		}
		private void setLineTitle ()
		{
			this.lblLinesNum.Text = constCalc.LinesNotConstrained ();
			GlobalSettings.isModified = true;
			CommonClass.cswVC.UpdateSaveButton ();
		}





		private void setValuesToFixedContraints()
		{
			 wBIdStateContent=GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			if (wBIdStateContent.Constraints.Hard) {
				this.btnHard.Selected = true;
				//this.buttonColor (btnHard);
			} else
				btnHard.Selected = false;
			if (wBIdStateContent.Constraints.Reserve) {
				this.btnReserve.Selected = true;
				//this.buttonColor (btnReserve);
			} else
				btnReserve.Selected = false;
			if (wBIdStateContent.Constraints.Ready||wBIdStateContent.Constraints.Blank) {
				this.btnBlank.Selected = true;
				//this.buttonColor (btnBlank);
			} else
				btnBlank.Selected = false;
			if (wBIdStateContent.Constraints.International) {
				this.btnInternational.Selected = true;
				//this.buttonColor (btnInternational);
			} else
				btnInternational.Selected = false;
			if (wBIdStateContent.Constraints.NonConus) {
				this.btnNonConcus.Selected = true;
				//this.buttonColor (btnNonConcus);
			} else
				btnNonConcus.Selected = false;
            if (wBIdStateContent.Constraints.ETOPS)
            {
                this.btnETOPS.Selected = true;
                //this.buttonColor (btnNonConcus);
            }
            else
                btnETOPS.Selected = false;

            if (wBIdStateContent.Constraints.ReserveETOPS)
            {
                this.btnETOPSRES.Selected = true;
                //this.buttonColor (btnNonConcus);
            }
            else
                btnETOPSRES.Selected = false;
            
			if (wBIdStateContent.CxWtState.AMPMMIX.AM) {
				this.btnAMs.Selected = true;
				//this.buttonColor (btnAMs);
			} else
				btnAMs.Selected = false;
			if (wBIdStateContent.CxWtState.AMPMMIX.PM) {
				this.btnPMs.Selected = true;
				//this.buttonColor (btnPMs);
			} else
				btnPMs.Selected = false;
			if (wBIdStateContent.CxWtState.AMPMMIX.MIX) {
				this.btnMix.Selected = true;
				//this.buttonColor (btnMix);
			} else
				btnMix.Selected = false;
			if (wBIdStateContent.CxWtState.FaPosition.A) {
				this.btnABCD [0].Selected = true;
				//this.buttonColor (btnABCD[0]);
			} else
				btnABCD [0].Selected = false;
			if(wBIdStateContent.CxWtState.FaPosition.B) {
				this.btnABCD [1].Selected = true;
				//this.buttonColor (btnABCD[1]);
			} else
				btnABCD [1].Selected = false;
			if(wBIdStateContent.CxWtState.FaPosition.C) {
				this.btnABCD [2].Selected = true;
				//this.buttonColor (btnABCD[2]);
			} else
				btnABCD [2].Selected = false;
			if(wBIdStateContent.CxWtState.FaPosition.D) {
				this.btnABCD [3].Selected = true;
				//this.buttonColor (btnABCD[3]);
			} else
				btnABCD [3].Selected = false;
			if(wBIdStateContent.CxWtState.TripLength.Turns) {
				this.btnTurns [0].Selected = true;
				//this.buttonColor (btnTurns [0]);
			} else
				btnTurns [0].Selected = false;
			if(wBIdStateContent.CxWtState.TripLength.Twoday) {
				this.btnTurns [1].Selected = true;
				//this.buttonColor (btnTurns [1]);
			} else
				btnTurns [1].Selected = false;
			if(wBIdStateContent.CxWtState.TripLength.ThreeDay) {
				this.btnTurns [2].Selected = true;
				//this.buttonColor (btnTurns [2]);
			} else
				btnTurns [2].Selected = false;
			if(wBIdStateContent.CxWtState.TripLength.FourDay) {
				this.btnTurns [3].Selected = true;
				//this.buttonColor (btnTurns [3]);
			} else
				btnTurns [3].Selected = false;
			if(wBIdStateContent.CxWtState.DaysOfWeek.SUN) {
				this.btnDays [0].Selected = true;
				//this.buttonColor (btnDays [0]);
			} else
				btnDays [0].Selected = false;
			if(wBIdStateContent.CxWtState.DaysOfWeek.MON) {
				this.btnDays [1].Selected = true;
				//this.buttonColor (btnDays [1]);
			} else
				btnDays [1].Selected = false;
			if(wBIdStateContent.CxWtState.DaysOfWeek.TUE) {
				this.btnDays [2].Selected = true;
				//this.buttonColor (btnDays [2]);
			} else
				btnDays [2].Selected = false;
			if(wBIdStateContent.CxWtState.DaysOfWeek.WED) {
				this.btnDays [3].Selected = true;
				//this.buttonColor (btnDays [3]);
			} else
				btnDays [3].Selected = false;
			if(wBIdStateContent.CxWtState.DaysOfWeek.THU) {
				this.btnDays [4].Selected = true;
				//this.buttonColor (btnDays [4]);
			} else
				btnDays [4].Selected = false;
			if(wBIdStateContent.CxWtState.DaysOfWeek.FRI) {
				this.btnDays [5].Selected = true;
				//this.buttonColor (btnDays [5]);
			} else
				btnDays [5].Selected = false;
			if(wBIdStateContent.CxWtState.DaysOfWeek.SAT) {
				this.btnDays [6].Selected = true;
				//this.buttonColor (btnDays [6]);
			} else
				btnDays [6].Selected = false;
		}
		public void observeNotification()
		{
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString("AddConstraints"), handleConstraintsReload));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("dismissPopover"),dismissPopover));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString("LineCountReload"), handleLineCount));

		}
		private void handleLineCount (NSNotification n)
		{
			this.setLineTitle ();
		}
		private void dismissPopover (NSNotification n)
		{
			popoverController.Dismiss (true);

		}
		private void loadConstraintsList ()
		{
			constraintsCont = new ConstraintsTableController ();
			this.AddChildViewController (constraintsCont);
			vwConst.AddSubview (constraintsCont.View);
			//constraintsCont.View.BackgroundColor = UIColor.Red;
			constraintsCont.View.Frame = new CGRect(0, 0, this.View.Frame.Width, vwConst.Frame.Height);
		}
		private void setGraphics ()
		{


			this.btnHard.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnHard.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			this.btnReserve.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnReserve.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			this.btnBlank.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnBlank.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			this.btnInternational.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnInternational.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			this.btnNonConcus.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnNonConcus.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
            this.btnETOPS.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
            this.btnETOPS.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
            this.btnAMs.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnAMs.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			this.btnPMs.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnPMs.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			this.btnMix.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnMix.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
            this.btnETOPSRES.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
            this.btnETOPSRES.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
           
			foreach (UIButton btn in this.btnABCD) {
				btn.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
				btn.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
				btn.Hidden = true;
			}
			foreach (UIButton btn in this.btnTurns) {
				btn.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
				btn.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			}
			foreach (UIButton btn in this.btnDays) {
				btn.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
				btn.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
			}

			if (GlobalSettings.CurrentBidDetails.Postion == "FA") 
            {
				this.btnBlank.SetTitle ("Ready", UIControlState.Normal);
				foreach (UIButton btn in this.btnABCD) {
					btn.Hidden = false;
				}

			}
           

			if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Round == "M")
			{
				btnBlank.Enabled = false;
				btnReserve.Enabled = false;
			}


		}
		public void listSavedConstraints () {
			ConstraintsApplied.clearAll ();
			if (wBIdStateContent.CxWtState.DHD.Cx&&ConstraintsApplied.CmutDHsConstraints.Count==0) {
				foreach (Cx4Parameter para in wBIdStateContent.Constraints.DeadHeads.LstParameter) {
					ConstraintsApplied.CmutDHsConstraints.Add ("Cmut DHs");
				}
			}
			if (wBIdStateContent.CxWtState.DOW.Cx&&ConstraintsApplied.daysOfWeekConstraints.Count==0) {
				foreach (Cx3Parameter para in wBIdStateContent.Constraints.DaysOfWeek.lstParameters) {
					ConstraintsApplied.daysOfWeekConstraints.Add ("Days of the Week");
				}
			}
			if (wBIdStateContent.CxWtState.DHDFoL.Cx&&ConstraintsApplied.DhFirstLastConstraints.Count==0) {
				foreach (Cx3Parameter para in wBIdStateContent.Constraints.DeadHeadFoL.lstParameters) {
					ConstraintsApplied.DhFirstLastConstraints.Add ("DH - first - last");
				}
			}
			if (wBIdStateContent.CxWtState.EQUIP.Cx&&ConstraintsApplied.EQTypeConstraints.Count==0) {
				foreach (Cx3Parameter para in wBIdStateContent.Constraints.EQUIP.lstParameters) {
					ConstraintsApplied.EQTypeConstraints.Add ("Equipment Type");
				}
			}
			if (wBIdStateContent.CxWtState.InterConus.Cx&&ConstraintsApplied.IntlNonConusConstraints.Count==0) {
				foreach (Cx2Parameter para in wBIdStateContent.Constraints.InterConus.lstParameters) {
					ConstraintsApplied.IntlNonConusConstraints.Add ("Intl – NonConus");
				}
			}
			if (wBIdStateContent.CxWtState.RON.Cx&&ConstraintsApplied.OvernightCitiesConstraints.Count==0) 
			{
				foreach (Cx3Parameter para in wBIdStateContent.Constraints.OverNightCities.lstParameters) {
					ConstraintsApplied.OvernightCitiesConstraints.Add ("Overnight Cities");
				}
			}
			if (wBIdStateContent.CxWtState.CitiesLegs.Cx&&ConstraintsApplied.CitiesLegsConstraints.Count==0) 
			{
				foreach (Cx3Parameter para in wBIdStateContent.Constraints.CitiesLegs.lstParameters) {
					ConstraintsApplied.CitiesLegsConstraints.Add ("Cities-Legs");
				}
			}
			if (wBIdStateContent.CxWtState.WtPDOFS.Cx&&ConstraintsApplied.PDOConstraints.Count==0)
			{
				foreach (Cx4Parameter para in wBIdStateContent.Constraints.PDOFS.LstParameter) {
					ConstraintsApplied.PDOConstraints.Add ("PDO");
				}
			}
			if (wBIdStateContent.CxWtState.SDOW.Cx&&ConstraintsApplied.StartDayofWeekConstraints.Count==0) {
				foreach (Cx3Parameter para in wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters) {
					ConstraintsApplied.StartDayofWeekConstraints.Add ("Start Day of Week");
				}
			}
			if (wBIdStateContent.CxWtState.Rest.Cx&&ConstraintsApplied.RestConstraints.Count==0) {
				foreach (Cx3Parameter para in wBIdStateContent.Constraints.Rest.lstParameters) {
					ConstraintsApplied.RestConstraints.Add ("Rest");
				}
			}
			if (wBIdStateContent.CxWtState.TL.Cx&&ConstraintsApplied.TripLengthConstraints.Count==0) {
				foreach (Cx3Parameter para in wBIdStateContent.Constraints.TripLength.lstParameters) {
					ConstraintsApplied.TripLengthConstraints.Add ("Trip Length");
				}
			}
			if (wBIdStateContent.CxWtState.WB.Cx&&ConstraintsApplied.WorkBlockLengthConstraints.Count==0) {
				foreach (Cx3Parameter para in wBIdStateContent.Constraints.WorkBlockLength.lstParameters) {
					ConstraintsApplied.WorkBlockLengthConstraints.Add ("Work Blk Length");
				}
			}
            if (wBIdStateContent.CxWtState.StartDay.Cx && ConstraintsApplied.StartDayConstraints.Count == 0)
            {
                foreach (Cx3Parameter para in wBIdStateContent.Constraints.StartDay.lstParameters)
                {
                    ConstraintsApplied.StartDayConstraints.Add("Start Day");
                }
            }
            if (wBIdStateContent.CxWtState.ReportRelease.Cx && ConstraintsApplied.ReportReleaseConstraints.Count == 0)
            {
                foreach (ReportRelease para in wBIdStateContent.Constraints.ReportRelease.lstParameters)
                {
                    ConstraintsApplied.ReportReleaseConstraints.Add("Report-Release");
                }
            }

			constraintsCont.TableView.ReloadData ();
			//setLineTitle ();
			this.lblLinesNum.Text = constCalc.LinesNotConstrained ();
			adjustScrollHeight ();

		}

		private void adjustScrollHeight ()
		{
            int row50Count = 0, row300Count = 0, row40Count=0,row150Count = 0;

			if (wBIdStateContent.CxWtState.ACChg.Cx) {
				row50Count += 1;//("Aircraft Changes");
			}
			if (wBIdStateContent.CxWtState.MixedHardReserveTrip.Cx)
			{
				row50Count += 1;//("MixedHardReserveTrip");
			}
			if (wBIdStateContent.CxWtState.BDO.Cx) {
				row50Count += 1;//("Blocks of Days Off");
			}
			if (wBIdStateContent.CxWtState.DHD.Cx) {
				foreach (Cx4Parameter para in wBIdStateContent.Constraints.DeadHeads.LstParameter) {
					row50Count += 1;//("Cmut DHs");
				}
			}
			if (wBIdStateContent.CxWtState.CL.Cx) {
				row300Count += 1;//("Commutable Lines");
			}
			if (wBIdStateContent.CxWtState.CLAuto.Cx) {
				row50Count += 1;//("Commutable Lines");
			}
			if (wBIdStateContent.CxWtState.DOW.Cx) {
				foreach (Cx3Parameter para in wBIdStateContent.Constraints.DaysOfWeek.lstParameters) {
					row50Count += 1;//("Days of the Week");
				}
			}
			if (wBIdStateContent.CxWtState.SDO.Cx) {
				row300Count += 1;//("Days of the Month");
			}
			if (wBIdStateContent.CxWtState.DHDFoL.Cx) {
				foreach (Cx3Parameter para in wBIdStateContent.Constraints.DeadHeadFoL.lstParameters) {
					row50Count += 1;//("DH - first - last");
				}
			}
			if (wBIdStateContent.CxWtState.DP.Cx) {
				row50Count += 1;//("Duty period");
			}
			if (wBIdStateContent.CxWtState.EQUIP.Cx) {
				foreach (Cx3Parameter para in wBIdStateContent.Constraints.EQUIP.lstParameters) {
					row50Count += 1;//("Equipment Type");
				}
			}
			if (wBIdStateContent.CxWtState.FLTMIN.Cx) {
				row50Count += 1;//("Flight Time");
			}
			if (wBIdStateContent.CxWtState.GRD.Cx) {
				row50Count += 1;//("Ground Time");
			}
			if (wBIdStateContent.CxWtState.InterConus.Cx) {
				foreach (Cx2Parameter para in wBIdStateContent.Constraints.InterConus.lstParameters) {
					row50Count += 1;//("Intl – NonConus");
				}
			}
			if (wBIdStateContent.CxWtState.LEGS.Cx) {
				row50Count += 1;//("Legs Per Duty Period");
			}
			if (wBIdStateContent.CxWtState.LegsPerPairing.Cx) {
				row50Count += 1;//("Legs Per Pairing");
			}
			if (wBIdStateContent.CxWtState.NODO.Cx) {
				row50Count += 1;//("Number of Days Off");
			}
			if (wBIdStateContent.CxWtState.RON.Cx) {
				foreach (Cx3Parameter para in wBIdStateContent.Constraints.OverNightCities.lstParameters) {
					row50Count += 1;//("Overnight Cities");
				}
			}
			if (wBIdStateContent.CxWtState.CitiesLegs.Cx) {
				foreach (Cx3Parameter para in wBIdStateContent.Constraints.CitiesLegs.lstParameters) {
					row50Count += 1;//("CitiesLegs");
				}
			}
			if (wBIdStateContent.CxWtState.WtPDOFS.Cx) {
				foreach (Cx4Parameter para in wBIdStateContent.Constraints.PDOFS.LstParameter) {
					row50Count += 1;//("PDO");
				}
			}
			if (wBIdStateContent.CxWtState.SDOW.Cx) {
				foreach (Cx3Parameter para in wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters) {
					row50Count += 1;//("Start Day of Week");
				}
			}
			if (wBIdStateContent.CxWtState.Rest.Cx) {
				foreach (Cx3Parameter para in wBIdStateContent.Constraints.Rest.lstParameters) {
					row50Count += 1;//("Rest");
				}
			}
			if (wBIdStateContent.CxWtState.PerDiem.Cx) {
				row50Count += 1;//("Time-Away-From-Base");
			}
			if (wBIdStateContent.CxWtState.TL.Cx) {
				foreach (Cx3Parameter para in wBIdStateContent.Constraints.TripLength.lstParameters) {
					row50Count += 1;//("Trip Length");
				}
			}
			if (wBIdStateContent.CxWtState.WB.Cx) {
				foreach (Cx3Parameter para in wBIdStateContent.Constraints.WorkBlockLength.lstParameters) {
					row50Count += 1;//("Work Blk Length");
				}
			}
			if (wBIdStateContent.CxWtState.WorkDay.Cx) {
				row50Count += 1;//("Work Days");
			}
			if (wBIdStateContent.CxWtState.MP.Cx) {
				row50Count += 1;//("Min Pay");
			}
			if (wBIdStateContent.CxWtState.No3on3off.Cx) {
				row50Count += 1;//("3-on-3-off");
			}
			if (wBIdStateContent.CxWtState.NOL.Cx) {
				row50Count += 1;//("Overlap Days");
			}
			if (wBIdStateContent.CxWtState.Commute.Cx) {
				row50Count += 1;//("Commute");
			}
            if (wBIdStateContent.CxWtState.StartDay.Cx)
            {
                foreach (Cx3Parameter para in wBIdStateContent.Constraints.StartDay.lstParameters)
                {
                    row50Count += 1;//("Start Day");
                }
            }
            if (wBIdStateContent.CxWtState.ReportRelease.Cx)
            {
                foreach (ReportRelease para in wBIdStateContent.Constraints.ReportRelease.lstParameters)
                {
                    row50Count += 3;//("Report-release ");
                }
            }
			if (row300Count == 2) {
				vwConst.Frame = new CGRect (2, 206, vwConst.Frame.Width, (50 * row50Count) + 600);
				constraintsCont.View.Frame = new CGRect (0, 0, vwConst.Bounds.Width, (50 * row50Count) + 600);
				this.scrlConstraints.ContentSize = new CGSize (0, constraintsCont.View.Frame.Height + 207);
				vwMainConst.Frame = new CGRect (0, 0, this.View.Frame.Width, constraintsCont.View.Frame.Height + 207);
			} else if (row300Count == 1) {
				vwConst.Frame = new CGRect (2, 206, this.View.Frame.Width, (50 * row50Count) + 300);
				constraintsCont.View.Frame = new CGRect (0, 0, constraintsCont.View.Bounds.Width, (50 * row50Count) + 300);
				this.scrlConstraints.ContentSize = new CGSize (0, constraintsCont.View.Frame.Height + 207);
				vwMainConst.Frame = new CGRect (0, 0, this.View.Frame.Width, constraintsCont.View.Frame.Height + 207);
			}
            else if (row150Count == 1)
            {

                vwConst.Frame = new CGRect(2, 206, vwConst.Frame.Width, (50 * row150Count) + 150);
                constraintsCont.View.Frame = new CGRect(0, 0, vwConst.Bounds.Width, (50 * row150Count) + 150);
                this.scrlConstraints.ContentSize = new CGSize(0, constraintsCont.View.Frame.Height + 207);
                vwMainConst.Frame = new CGRect(0, 0, this.View.Frame.Width, constraintsCont.View.Frame.Height + 207);
                //vwConst.Frame = new CGRect(2, 206, vwConst.Frame.Width, 150 * row150Count);
                //constraintsCont.View.Frame = new CGRect(0, 0, vwConst.Bounds.Width, 150 * row150Count);
                //this.scrlConstraints.ContentSize = new CGSize(0, constraintsCont.View.Frame.Height + 207);
                //vwMainConst.Frame = new CGRect(0, 0, 482, constraintsCont.View.Frame.Height + 207);
            }
            else if (row150Count == 2)
            {
                vwConst.Frame = new CGRect(2, 206, vwConst.Frame.Width, (50 * row150Count) + 300);
                constraintsCont.View.Frame = new CGRect(0, 0, vwConst.Bounds.Width, (50 * row150Count) + 300);
                this.scrlConstraints.ContentSize = new CGSize(0, constraintsCont.View.Frame.Height + 207);
                vwMainConst.Frame = new CGRect(0, 0, this.View.Frame.Width, constraintsCont.View.Frame.Height + 207);
                //vwConst.Frame = new CGRect(2, 206, vwConst.Frame.Width, 150 * row150Count);
                //constraintsCont.View.Frame = new CGRect(0, 0, vwConst.Bounds.Width, 150 * row150Count);
                //this.scrlConstraints.ContentSize = new CGSize(0, constraintsCont.View.Frame.Height + 207);
                //vwMainConst.Frame = new CGRect(0, 0, 482, constraintsCont.View.Frame.Height + 207);
            }
            else if (row40Count == 1) {
				vwConst.Frame = new CGRect (2, 206, vwConst.Frame.Width, 40 * row40Count);
				constraintsCont.View.Frame = new CGRect (0, 0, vwConst.Bounds.Width, 40 * row40Count);
				this.scrlConstraints.ContentSize = new CGSize (0, constraintsCont.View.Frame.Height + 207);
				vwMainConst.Frame = new CGRect (0, 0, this.View.Frame.Width, constraintsCont.View.Frame.Height + 207);
			}
			else {
				vwConst.Frame = new CGRect (2, 206, vwConst.Frame.Width, 50 * row50Count);
				constraintsCont.View.Frame = new CGRect (0, 0, vwConst.Bounds.Width, 50 * row50Count);
				this.scrlConstraints.ContentSize = new CGSize (0, constraintsCont.View.Frame.Height + 207);
				vwMainConst.Frame = new CGRect (0, 0, this.View.Frame.Width, constraintsCont.View.Frame.Height + 207);
			}

			if (wBIdStateContent.CxWtState.BulkOC.Cx) {
				int count1;
				int count2;

                if(GlobalSettings.OverNightCitiesInBid.Count < 80)
                {
					 count1 = 18;
					 count2 = 0;
				}
                else
                {
					 count1 = GlobalSettings.OverNightCitiesInBid.Count / 5;
					 count2 = GlobalSettings.OverNightCitiesInBid.Count % 5;
				}
				
				if (count2 == 0) {
					vwConst.Frame = new CGRect (2, 206, vwConst.Frame.Width, vwConst.Frame.Height + 50 * count1);
					constraintsCont.View.Frame = new CGRect (0, 0, vwConst.Bounds.Width, vwConst.Frame.Height);
					this.scrlConstraints.ContentSize = new CGSize (0, scrlConstraints.ContentSize.Height + 30 * count1);
					vwMainConst.Frame = new CGRect (0, 0, 482, vwMainConst.Frame.Size.Height + + 30 * count1);

				} else {
					vwConst.Frame = new CGRect (2, 206, vwConst.Frame.Width, vwConst.Frame.Height + 50 * (count1 + 1));
					constraintsCont.View.Frame = new CGRect (0, 0, vwConst.Frame.Width, vwConst.Frame.Height);
					this.scrlConstraints.ContentSize = new CGSize (0, scrlConstraints.ContentSize.Height + 30 * (count1 + 1));
					vwMainConst.Frame = new CGRect (0, 0, 482, vwMainConst.Frame.Size.Height + 30 * (count1 + 1));
				}
			}

//			if(this.scrlConstraints.ContentSize.Height>this.scrlConstraints.Bounds.Height)
//				this.scrlConstraints.ContentOffset = new PointF (0, this.scrlConstraints.ContentSize.Height - this.scrlConstraints.Bounds.Height);

		}
		public void handleConstraintsReload (NSNotification n)
		{
			setValuesToFixedContraints ();
            listSavedConstraints();
			constraintsCont.TableView.ReloadData ();
			adjustScrollHeight ();
		}

		#region IBActions
//		public void buttonColor (UIButton btn)
//		{
//			if (btn.Selected)
//				btn.BackgroundColor = UIColor.FromRGB(150,241,250);
//			else
//				btn.BackgroundColor = UIColor.Clear;
//		}

		partial void btnBlankTapped (UIKit.UIButton sender)
		{
			sender.Selected = !sender.Selected;
			WBidHelper.PushToUndoStack ();
			//this.buttonColor (sender);
            if (GlobalSettings.CurrentBidDetails.Postion == "FA")
            {
                constCalc.ReadyReserveConstraintCalculation(sender.Selected);
                if (sender.Selected)
                    GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints.Ready = true;
                else
                    GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints.Ready = false;
            }
            else
            {
                constCalc.BlankConstraintCalculation(sender.Selected);
                if (sender.Selected)
                    GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints.Blank = true;
                else
                    GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints.Blank = false;
            }
			setLineTitle ();
			//NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
		}

        partial void btnClearTapped(UIKit.UIButton sender)
        {

            UIActionSheet sheet = new UIActionSheet("Really want to clear Contraints?", null, "NO", "YES", null);
            CGRect senderframe = sender.Superview.Frame;
            senderframe.X = sender.Superview.Frame.GetMidX();
            sheet.ShowFrom(senderframe, this.View, true);
            sheet.Clicked += HandleClearContraints; 

        }

        void HandleClearContraints(object sender, UIButtonEventArgs e)
        {
            if (e.ButtonIndex == 0)
            {
                WBidHelper.PushToUndoStack();                 this.btnHard.Selected = false;                 this.btnReserve.Selected = false;                 this.btnBlank.Selected = false;                 this.btnInternational.Selected = false;                 this.btnNonConcus.Selected = false;                 this.btnETOPS.Selected = false;                 this.btnETOPSRES.Selected = false;                 this.btnAMs.Selected = false;                 this.btnPMs.Selected = false;                 this.btnMix.Selected = false;                 foreach (UIButton btn in this.btnABCD)                 {                     btn.Selected = false;                 }                 foreach (UIButton btn in this.btnTurns)                 {                     btn.Selected = false;                 }                 foreach (UIButton btn in this.btnDays)                 {                     btn.Selected = false;                 }                   this.btnHard.BackgroundColor = UIColor.Clear;                 this.btnReserve.BackgroundColor = UIColor.Clear;                 this.btnBlank.BackgroundColor = UIColor.Clear;                 this.btnInternational.BackgroundColor = UIColor.Clear;                 this.btnNonConcus.BackgroundColor = UIColor.Clear;                 this.btnETOPS.BackgroundColor = UIColor.Clear;                 this.btnETOPSRES.BackgroundColor = UIColor.Clear;                 this.btnAMs.BackgroundColor = UIColor.Clear;                 this.btnPMs.BackgroundColor = UIColor.Clear;                 this.btnMix.BackgroundColor = UIColor.Clear;                 foreach (UIButton btn in this.btnABCD)                 {                     btn.BackgroundColor = UIColor.Clear;                 }                 foreach (UIButton btn in this.btnTurns)                 {                     btn.BackgroundColor = UIColor.Clear;                 }                 foreach (UIButton btn in this.btnDays)                 {                     btn.BackgroundColor = UIColor.Clear;                 }                  constCalc.ClearConstraints();                 ConstraintsApplied.clearAll();                  NSNotificationCenter.DefaultCenter.PostNotificationName("AddConstraints", null);                 setLineTitle();
            }
             
             
             
        }


        partial void btnDefineTapped (UIKit.UIButton sender)
		{
			DefineConstraints dc = new DefineConstraints();
			popoverController = new UIPopoverController (dc);
			popoverController.PopoverContentSize = new CGSize (708, 435);
			popoverController.PresentFromRect(sender.Frame,this.vwAMPMcontainer,UIPopoverArrowDirection.Any,true);
		}
		partial void btnHardTapped (UIKit.UIButton sender)
		{
			sender.Selected = !sender.Selected;
			WBidHelper.PushToUndoStack ();
			//this.buttonColor (sender);
			constCalc.HardConstraintCalclculation(sender.Selected);
            if (sender.Selected)
                GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints.Hard = true;
            else
                GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints.Hard = false;
			//NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
			setLineTitle ();

		}
        partial void btnInternationalTapped (UIKit.UIButton sender)
		{
			sender.Selected = !sender.Selected;
			WBidHelper.PushToUndoStack ();
			//this.buttonColor (sender);
			constCalc.InternationalConstraintCalculation(sender.Selected);
            if (sender.Selected)
                GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints.International = true;
            else
                GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints.International = false;

			//NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
			setLineTitle ();

		}
		partial void btnLinesTapped (UIKit.UIButton sender)
		{
            SortCalculation sort = new SortCalculation();
            WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            if (wBidStateContent.SortDetails!=null && wBidStateContent.SortDetails.SortColumn != null && wBidStateContent.SortDetails.SortColumn != string.Empty)
            {
                sort.SortLines(wBidStateContent.SortDetails.SortColumn);
            }
            //added by Roshil Aug 2020
            CommonClass.lineVC.UpdateSaveButton();
			//NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);
			NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
			this.ParentViewController.NavigationController.DismissViewController (true, null);

			memoryRelease();
		}

		public	void memoryRelease()
		{
			foreach (NSObject obj in arrObserver) {
				NSNotificationCenter.DefaultCenter.RemoveObserver (obj);
			}

			CommonClass.cswVC.memoryRelease ();

			foreach (UIView view in this.ParentViewController.View.Subviews) {

				DisposeClass.DisposeEx(view);
			}


			this.View.Dispose ();

		}
		public void removeObservers()
		{
			foreach (NSObject obj in arrObserver) {
				NSNotificationCenter.DefaultCenter.RemoveObserver (obj);
			}
		}
        partial void btnETOPSTapped(UIKit.UIButton sender)
        {
           
            if (sender.Selected==true && GlobalSettings.CurrentBidDetails.Postion != "FA" &&  ChecktheUserIsInEBGGroup() == false)
            {
                UIAlertController alert = UIAlertController.Create("Alert !", "You are NOT in the ETOPS Bid Group.  You should not bid the ETOPS lines as you will not be awarded any ETOPS line.", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Leave ON ", UIAlertActionStyle.Cancel, (actionCancel) =>
                {
                   
                }));

                alert.AddAction(UIAlertAction.Create("Turn OFF", UIAlertActionStyle.Default, (actionOK) =>
                {
                    sender.Selected = !sender.Selected;
                    WBidHelper.PushToUndoStack();
                    //this.buttonColor (sender);
                    constCalc.ETOPSConstraintClaculation(sender.Selected);
                    if (sender.Selected)
                        GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints.ETOPS = true;
                    else
                        GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints.ETOPS = false;

                    //NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                    setLineTitle();
                }));
                this.PresentViewController(alert, true, null);
            }
            else
            {

                sender.Selected = !sender.Selected;
                WBidHelper.PushToUndoStack();
                //this.buttonColor (sender);
                constCalc.ETOPSConstraintClaculation(sender.Selected);
                if (sender.Selected)
                    GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints.ETOPS = true;
                else
                    GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints.ETOPS = false;

                //NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                setLineTitle();
            }
        }
        partial void btnETOPSRESTapped(UIButton sender)
        {


            if ( sender.Selected == true && GlobalSettings.CurrentBidDetails.Postion != "FA" && ChecktheUserIsInEBGGroup()==false)
            {
                UIAlertController alert = UIAlertController.Create("Alert !", "You are NOT in the ETOPS Bid Group.  You should not bid the ETOPS lines as you will not be awarded any ETOPS line.", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Leave ON ", UIAlertActionStyle.Cancel, (actionCancel) =>
                {
                   
                }));

                alert.AddAction(UIAlertAction.Create("Turn OFF", UIAlertActionStyle.Default, (actionOK) =>
                {
                    sender.Selected = !sender.Selected;
                    WBidHelper.PushToUndoStack();
                    //this.buttonColor (sender);
                    constCalc.ETOPSResConstraintClaculation(sender.Selected);
                    if (sender.Selected)
                        GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints.ReserveETOPS = true;
                    else
                        GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints.ReserveETOPS = false;

                    //NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                    setLineTitle();
                }));
                this.PresentViewController(alert, true, null);
            }
            else
            {
                sender.Selected = !sender.Selected;
                WBidHelper.PushToUndoStack();
                //this.buttonColor (sender);
                constCalc.ETOPSResConstraintClaculation(sender.Selected);
                if (sender.Selected)
                    GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints.ReserveETOPS = true;
                else
                    GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints.ReserveETOPS = false;

                //NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                setLineTitle();
            }
        }
        private bool ChecktheUserIsInEBGGroup()
        {

			//return (GlobalSettings.SeniorityListMember != null && GlobalSettings.SeniorityListMember.EBG == "Y") ? true : false;
			return (GlobalSettings.WBidStateCollection.SeniorityListItem != null && GlobalSettings.WBidStateCollection.SeniorityListItem.EBgType == "Y") ? true : false;


		}
        partial void btnNonConusTapped (UIKit.UIButton sender)
		{
			sender.Selected = !sender.Selected;
			WBidHelper.PushToUndoStack ();
			//this.buttonColor (sender);
			constCalc.Non_ConusConstraintClaculation(sender.Selected);
            if (sender.Selected)
                GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints.NonConus = true;
            else
                GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints.NonConus = false;

			//NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
			setLineTitle ();

		}
		partial void btnPlus2Tapped (UIKit.UIButton sender)
		{
			PopoverViewController popoverContent = new PopoverViewController ();
			popoverContent.PopType = "addConst";
			popoverController = new UIPopoverController (popoverContent);
			popoverController.PopoverContentSize = new CGSize (300, 600);
            CGRect senderframe = sender.Superview.Frame;
            senderframe.X = sender.Superview.Frame.GetMidX();
            popoverController.PresentFromRect (senderframe,this.View,UIPopoverArrowDirection.Any,true);
		}
		partial void btnReserveTapped (UIKit.UIButton sender)
		{
			sender.Selected = !sender.Selected;
			WBidHelper.PushToUndoStack ();
			//this.buttonColor (sender);
			constCalc.ReserveConstraintCalculation(sender.Selected);
            if (sender.Selected)
                GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints.Reserve = true;
            else
                GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints.Reserve = false;

			//NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
			setLineTitle ();

		}
		partial void btnWeekDayTapped (UIKit.UIButton sender)
		{
			sender.Selected = !sender.Selected;
			WBidHelper.PushToUndoStack ();
			//this.buttonColor (sender);
			List <int> days = new List<int> ();
			foreach (UIButton btn in this.btnDays) {
                if (btn.Selected)
                {
					days.Add((int)btn.Tag);
                    switch (btn.Tag)
                    {
                        case 0: wBIdStateContent.CxWtState.DaysOfWeek.MON = true;
                            break;
                        case 1: wBIdStateContent.CxWtState.DaysOfWeek.TUE = true;
                            break;
                        case 2: wBIdStateContent.CxWtState.DaysOfWeek.WED = true;
                            break;
                        case 3: wBIdStateContent.CxWtState.DaysOfWeek.THU = true;
                            break;
                        case 4: wBIdStateContent.CxWtState.DaysOfWeek.FRI = true;
                            break;
                        case 5: wBIdStateContent.CxWtState.DaysOfWeek.SAT = true;
                            break;
                        case 6: wBIdStateContent.CxWtState.DaysOfWeek.SUN = true;
                            break;
                    }
                }
                else
                {
                    switch (btn.Tag)
                     {
                            case 0: wBIdStateContent.CxWtState.DaysOfWeek.MON = false;
                                break;
                            case 1: wBIdStateContent.CxWtState.DaysOfWeek.TUE = false;
                                break;
                            case 2: wBIdStateContent.CxWtState.DaysOfWeek.WED = false;
                                break;
                            case 3: wBIdStateContent.CxWtState.DaysOfWeek.THU = false;
                                break;
                            case 4: wBIdStateContent.CxWtState.DaysOfWeek.FRI = false;
                                break;
                            case 5: wBIdStateContent.CxWtState.DaysOfWeek.SAT = false;
                                break;
                            case 6: wBIdStateContent.CxWtState.DaysOfWeek.SUN = false;
                                break;
                        }
                }
			}
			constCalc.ApplyWeekDayConstraint(days);
			//NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
			setLineTitle ();

		}
		partial void btnABCDTapped (UIKit.UIButton sender)
		{
			sender.Selected = !sender.Selected;
			WBidHelper.PushToUndoStack ();
			//this.buttonColor (sender);

			//onstCalc.ABCDPositionsConstraint(sender.TitleLabel.Text,sender.Selected);
			foreach (UIButton btn in this.btnABCD) {
                if (btn.Selected)
                {
                    switch (btn.Tag)
                    {
                        case 1: wBIdStateContent.CxWtState.FaPosition.A = true;
                            break;
                        case 2: wBIdStateContent.CxWtState.FaPosition.B = true;
                            break;
                        case 3: wBIdStateContent.CxWtState.FaPosition.C = true;
                            break;
                        case 4: wBIdStateContent.CxWtState.FaPosition.D = true;
                            break;
                    }
                }
                else
                {
                    switch (btn.Tag)
                    {
                        case 1: wBIdStateContent.CxWtState.FaPosition.A = false;
                            break;
                        case 2: wBIdStateContent.CxWtState.FaPosition.B = false;
                            break;
                        case 3: wBIdStateContent.CxWtState.FaPosition.C = false;
                            break;
                        case 4: wBIdStateContent.CxWtState.FaPosition.D = false;
                            break;
                    }
                }
			}
			var faposition=wBIdStateContent.CxWtState.FaPosition;
			constCalc.ABCDPositionsConstraint(faposition.A,faposition.B,faposition.C,faposition.D);
			setLineTitle ();
			//NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);

		}
		partial void btnAMPMMixTapped (UIKit.UIButton sender)
		{
			sender.Selected = !sender.Selected;
			WBidHelper.PushToUndoStack ();
			//this.buttonColor (sender);
			if(sender.TitleLabel.Text=="AMs")
				constCalc.AMPMMixConstraint("AM",sender.Selected);
			else if(sender.TitleLabel.Text=="PMs")
				constCalc.AMPMMixConstraint(" PM",sender.Selected);
			else
				constCalc.AMPMMixConstraint("Mix",sender.Selected);

            wBIdStateContent.CxWtState.AMPMMIX.AM = this.btnAMs.Selected;
            wBIdStateContent.CxWtState.AMPMMIX.PM = this.btnPMs.Selected;
            wBIdStateContent.CxWtState.AMPMMIX.MIX = this.btnMix.Selected;

			//NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
			setLineTitle ();

		}
		partial void btnTurnsTripsTapped (UIKit.UIButton sender)
		{
			sender.Selected = !sender.Selected;
			WBidHelper.PushToUndoStack ();
			//this.buttonColor (sender);
			string str = string.Empty;
			foreach (UIButton btn in this.btnTurns) {
                if (btn.Selected)
                {
                    if (str != string.Empty)
                        str += ",";
                    str += btn.Tag;

                    switch (btn.Tag)
                    {
                        case 1: wBIdStateContent.CxWtState.TripLength.Turns = true;
                            break;
                        case 2: wBIdStateContent.CxWtState.TripLength.Twoday = true;
                            break;
                        case 3: wBIdStateContent.CxWtState.TripLength.ThreeDay = true;
                            break;
                        case 4: wBIdStateContent.CxWtState.TripLength.FourDay = true;
                            break;
                    }
                }
                else
                {
                    switch (btn.Tag)
                    {
                        case 1: wBIdStateContent.CxWtState.TripLength.Turns = false;
                            break;
                        case 2: wBIdStateContent.CxWtState.TripLength.Twoday = false;
                            break;
                        case 3: wBIdStateContent.CxWtState.TripLength.ThreeDay = false;
                            break;
                        case 4: wBIdStateContent.CxWtState.TripLength.FourDay = false;
                            break;
                    }
                }
			}
			Console.WriteLine(str);
			constCalc.ApplyTripLengthConstraint(str);
			//NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
			setLineTitle ();

		}


		#endregion

	}
}

