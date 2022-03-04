using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.SharedLibrary;
using System.Linq;
using WBid.WBidiPad.Core;
using System.Collections.Generic;
using CoreAnimation;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using WBid.WBidiPad.Core.Enum;
using System.Drawing;
using CoreGraphics;
using WBid.WBidiPad.iOS.Utility;

namespace WBid.WBidiPad.iOS
{
    public partial class ModernViewCellClassic : UITableViewCell
    {
        class MyPopDelegate : UIPopoverControllerDelegate
        {
            ModernViewCellClassic _parent;

			public ModernViewControllerSourceClassicScroll _Source;
            public MyPopDelegate(ModernViewCellClassic parent)
            {
                _parent = parent;
            }

            public override bool ShouldDismiss(UIPopoverController popoverController)
            {
                //commented by Roshil on 26-8-2021 to allow the user to set less than 5 properties
                return true;
                //if (CommonClass.modernProperties.Count == 5)
                //{
                //    return true;
                //}
                //else
                //{
                   
                //    UIWindow WindowAlert = new UIWindow(UIScreen.MainScreen.Bounds);
                //    WindowAlert.RootViewController = new UIViewController();
                //    UIAlertController okAlertController = UIAlertController.Create("Additional Columns", "Please select \"5\" columns", UIAlertControllerStyle.Alert);
                //    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                //    WindowAlert.MakeKeyAndVisible();
                //    WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
                //    WindowAlert.Dispose(); 
                //    return false;
                //}
            }
			public override void DidDismiss (UIPopoverController popoverController)
			{
				_parent.popoverController = null;
			}

        }
		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			//DisposeClass.DisposeEx(this);
		}

//		protected override void Dispose (bool disposing)
//		{
//			base.Dispose (disposing);
//			foreach (UIView view in this.Subviews) {
//
//				DisposeClass.DisposeEx(view);
//			}
//		}


		//public static void Fade ( UIView view, bool isIn, double duration = 0.5)
		//{
		//	var minAlpha = (nfloat)0.0f;
		//	var maxAlpha = (nfloat).85f;

		//	Action onFinished = null;
		//	view.Alpha = isIn ? minAlpha : maxAlpha;
		//	view.Transform = CGAffineTransform.MakeIdentity ();
		//	UIView.Animate (duration, 0.0, UIViewAnimationOptions.CurveEaseInOut,
		//		() => {
		//			view.Alpha = isIn ? maxAlpha : minAlpha;

		//		},
		//		onFinished
		//	);
		//}

		class MyPopDelegate3 : UIPopoverControllerDelegate
		{
			ModernViewCellClassic _parent;
			public MyPopDelegate3(ModernViewCellClassic parent)
			{
				_parent = parent;
			}
			public override void DidDismiss (UIPopoverController popoverController)
			{
				_parent.popoverController = null;
			}
		}
UIBezierPath BezierPath;
        public static readonly UINib Nib = UINib.FromName("ModernViewCellClassic", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("ModernViewCellClassic");
NSObject arrObserver,arrShowPopOverNotification;
        UITapGestureRecognizer singleTap;
        UITapGestureRecognizer singleTap2;
        UIPopoverController popoverController;
		UITapGestureRecognizer tipGest;
		UITapGestureRecognizer tipGest2;
		List <BidLineTemplate> template1 = new List<BidLineTemplate> ();
		Line line1;

        public ModernViewCellClassic(IntPtr handle)
            : base(handle)
        {
        }

        public static ModernViewCellClassic Create()
        {
            return (ModernViewCellClassic)Nib.Instantiate(null, null)[0];
        }

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
           
			UILongPressGestureRecognizer longPress1 = new UILongPressGestureRecognizer(handleLongPress);
			this.vwChild1.AddGestureRecognizer(longPress1);
			longPress1.DelaysTouchesBegan = true;

			UILongPressGestureRecognizer longPress2 = new UILongPressGestureRecognizer(handleLongPress);
			this.vwChild2.AddGestureRecognizer(longPress2);
			longPress2.DelaysTouchesBegan = true;

			foreach (UIView vw in vwChild3)
			{
				UILongPressGestureRecognizer longPress3 = new UILongPressGestureRecognizer(handleLongPress);
				vw.AddGestureRecognizer(longPress3);
				longPress3.DelaysTouchesBegan = true;
			}

			UITapGestureRecognizer addColTap = new UITapGestureRecognizer (HandleAdditionalColumn);
			vwChild2.AddGestureRecognizer (addColTap);
			addColTap.NumberOfTapsRequired = 1;

		}

		public void createCellBorder(Line line, UITableViewCell cell)  {
            UIView vWScrollIndication = new UIView();

			viewBorder.BackgroundColor = UIColor.Clear;
			viewBorder.Hidden = true;
			RemoveLayer(line.LineNum.ToString());

				if (GlobalSettings.WBidINIContent.User.IsModernViewShade == false)
				{
                removeallLayer();
					return;
				}
            if (line.ManualScroll == 4)
            {
                DrawBorder(4, UIColor.Red.CGColor, line.LineNum.ToString());
            }
			else if (line.ManualScroll == 1)

			{
				removeallLayer();
				viewBorder.Hidden = false;
				viewBorder.BackgroundColor = UIColor.Blue;


			}
			else if (line.ManualScroll == 2)
			{

				DrawBorder(4,UIColor.Blue.CGColor,line.LineNum.ToString());


			}
			else if (line.ManualScroll == 3)
			{
				DrawBorderFirstLine(8,UIColor.Blue.CGColor,line.LineNum.ToString());
			}
			else
			{

				removeallLayer();

			}


			
		}


		public void removeallLayer()
		{

            if (imgBorder.Layer.Sublayers != null)
			{
                foreach (CAShapeLayer layer in imgBorder.Layer.Sublayers)

				{
					layer.RemoveFromSuperLayer();

				}

			}
		}
		public void RemoveLayer(string linenumber)
		{
            if (imgBorder.Layer.Sublayers != null)
			{
                foreach (CAShapeLayer layer in imgBorder.Layer.Sublayers)

				{
					if (layer.Name != linenumber)
					{
						layer.RemoveFromSuperLayer();
					}


				}
			}
		}
        public void bindData(Line line, NSIndexPath indexpath)
        {
        

            if (indexpath.Row == 0)
            {
                if (arrShowPopOverNotification == null)
                    arrShowPopOverNotification = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ShowPopOverViewModernShadow"), ShowPopOverView);
            }
            line1 = line;
            template1 = line.BidLineTemplates;
            PrepareModernBidLineView prepareModernBidLineView = new PrepareModernBidLineView();
            //obj.SetModernBidLineView();

            if (imgBorder.Tag == 1)
            {
                if (imgBorder.Layer.Sublayers != null)
                {
                    foreach (CALayer lr in imgBorder.Layer.Sublayers)
                    {
                        lr.RemoveFromSuperLayer();
                    }
            }
                imgBorder.Tag = 0;
            }

            if (imgVLayer.Tag == 1)
            {
                if (imgVLayer.Layer.Sublayers != null)
                {
                    foreach (CALayer lr in imgVLayer.Layer.Sublayers)
                    {
                        lr.RemoveFromSuperLayer();
                    }
                }
                imgVLayer.Tag = 0;
            }

            this.ContentView.BackgroundColor = UIColor.Clear;
            this.lblLineSlNo.Text = (indexpath.Row + 1).ToString();
            this.lblLineNum.Text = line.LineDisplay;
            this.lblLinePos.Text = string.Join("", line.FAPositions.ToArray());

            this.imgLockIcon.Image = null;
            this.imgCrossIcon.Image = null;
            this.imgOverlapIcon.Image = null;

            if (line.TopLock)
                this.imgLockIcon.Image = UIImage.FromBundle("lockIconGreen.png");
            if (line.BotLock)
                this.imgLockIcon.Image = UIImage.FromBundle("lockIconRed.png");
            if (line.Constrained)
                this.imgCrossIcon.Image = UIImage.FromBundle("deleteIconBold.png");
            if (line.ShowOverLap)
                this.imgOverlapIcon.Image = UIImage.FromBundle("overlayIconBold.png");

            this.btnLineSelect.Tag = line.LineNum;
            this.btnLineSelect.SetImage(UIImage.FromBundle("roundActive.png"), UIControlState.Selected);
            this.btnLineSelect.SetImage(UIImage.FromBundle("roundNormal.png"), UIControlState.Normal);
            setSelectButton(line.LineNum);
            for (int i = 0; i < 5; i++)
            {
                lblPropName[i].Text = string.Empty;
                lblPropValue[i].Text = string.Empty;
            }
            for (int i = 0; i < CommonClass.modernProperties.Count; i++)
            {
                lblPropName[i].Text = CommonClass.modernProperties[i];
                lblPropValue[i].Text = CommonClass.GetLineProperty(CommonClass.modernProperties[i], line);
            }
            //for (int i = 4; i > CommonClass.modernProperties.Count; i--)
            //{
            //    lblPropName[i].Text = string.Empty;
            //    lblPropValue[i].Text = string.Empty;
            //}
            //            if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
            //            {
            //                lblProperty1[0].Text = "TotPay";
            //                lblProperty1[1].Text = Decimal.Round( line.Tfp,2).ToString();
            //                lblProperty2[0].Text = "VacPay";
            //                lblProperty2[1].Text =  Decimal.Round( line.VacPay,2).ToString();
            //                lblProperty3[0].Text = "FlyPay";
            //                lblProperty3[1].Text =  Decimal.Round( line.FlyPay,2).ToString();
            //                lblProperty4[0].Text = "Off";
            //                lblProperty4[1].Text = line.DaysOff.ToString();
            //                lblProperty5[0].Text = "+Off";
            //                lblProperty5[1].Text = line.LargestBlkOfDaysOff.ToString();
            //            }
            //            else
            //            {
            //                lblProperty1[0].Text = "Pay";
            //                lblProperty1[1].Text =  Decimal.Round( line.Tfp,2).ToString();
            //                lblProperty2[0].Text = "PDiem";
            //                lblProperty2[1].Text = line.TafbInBp.ToString();
            //                lblProperty3[0].Text = "Flt";
            //                lblProperty3[1].Text = line.BlkHrsInBp;
            //                lblProperty4[0].Text = "Off";
            //                lblProperty4[1].Text = line.DaysOff.ToString();
            //                lblProperty5[0].Text = "+Off";
            //                lblProperty5[1].Text = line.LargestBlkOfDaysOff.ToString();
            //            }

            int index = 0;
            int shapeOffset = 0;
            foreach (BidLineTemplate temp in line.BidLineTemplates)
            {
                if (index <= 6)
                    shapeOffset = 195;
                else if (index <= 13)
                    shapeOffset = 349;
                else if (index <= 20)
                    shapeOffset = 503;
                else if (index <= 27)
                    shapeOffset = 657;
                else if (index <= 34)
                    shapeOffset = 811;


                lblTripName[index].Tag = line.LineNum;
                this.lblTripName[index].Lines = 3;

                this.lblTripName[index].Frame = new CGRect(lblTripName[index].Frame.X, 40, 22.5f, 40);
               
                this.lblCalDate[index].Frame = new CGRect(lblCalDate[index].Frame.X, lblCalDate[index].Frame.Y, 22.5f, 40);
                this.lblCalDate[index].Text = temp.Date.Day.ToString()+ "\n \n" + temp.Date.DayOfWeek.ToString().Substring(0, 2).ToUpper();
                this.lblCalDate[index].Lines = 3;
               
                //temporary code to test the label
                if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM || GlobalSettings.MenuBarButtonStatus.IsMIL)
                {
                    if (temp.BidLineType != (int)BidLineType.NormalTrip && temp.BidLineType != (int)BidLineType.VOBSplit && temp.BidLineType != (int)BidLineType.VOBSplitDrop && temp.BidLineType != (int)BidLineType.VOFSplit && temp.BidLineType != (int)BidLineType.VOFSplitDrop)
                    {
                        this.lblTripName[index].Text = temp.ArrStaLastLegDisplay;
                        this.lblTripName[index].Lines = 1;
                        this.lblTripName[index].Frame = new CGRect(lblTripName[index].Frame.X, 40, 22.5f, 40);

                    }
                    else
                    {
                        this.lblTripName[index].Text = temp.TripNum + "\n \n" + temp.ArrStaLastLeg;

                    }
                }
                else
                {
                    this.lblTripName[index].Text = temp.TripNum + "\n \n" + temp.ArrStaLastLeg;

                }


                if (lblCalDate[index].Text.Contains("SA")  || lblCalDate[index].Text.Contains("SU"))
                { // Weekend Days
                  //  lblCalDate[index].BackgroundColor = ColorClass.weekendDayColor;
                    lblCalDate[index].BackgroundColor = ColorClass.weekendDayColor;
                    lblTripName[index].BackgroundColor = ColorClass.weekendTripColor;

                }
                else
                {   // Normal Days
                    // lblCalDate[index].BackgroundColor = ColorClass.normDayColor;
                    lblCalDate[index].BackgroundColor = ColorClass.normDayColor;
                    lblTripName[index].BackgroundColor = ColorClass.normTripColor;

                }



                if (!temp.IsInCurrentMonth)
                { // Not Current Month
                    lblCalDate[index].BackgroundColor = ColorClass.nextMonthDayColor;
                    lblTripName[index].BackgroundColor = ColorClass.nextMonthTripColor;

                }


                if (GlobalSettings.TempOrderedVacationDays != null && (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM) && GlobalSettings.TempOrderedVacationDays.Any(x => x.AbsenceType == "VA" && (x.StartAbsenceDate <= temp.Date) && (x.EndAbsenceDate >= temp.Date)))
                {
                    lblCalDate[index].BackgroundColor = ColorClass.VacationTripDayColor;
                }
                else if (lblCalDate[index].Text.Contains("SA") || lblCalDate[index].Text.Contains("SU"))
                {
                    lblCalDate[index].BackgroundColor = ColorClass.weekendDayColor;
                }
                else
                {
                    lblCalDate[index].BackgroundColor = ColorClass.normDayColor;
                }

                lblTripName[index].UserInteractionEnabled = true;
                tipGest = new UITapGestureRecognizer(HandleToolTip);
                tipGest.NumberOfTapsRequired = 1;
                lblTripName[index].AddGestureRecognizer(tipGest);


                tipGest2 = new UITapGestureRecognizer(HandleToolTip);
                tipGest2.NumberOfTapsRequired = 1;


                if (temp.TripName != null)
                { // Trip Present
                  //this.lblTripName[index].UserInteractionEnabled = true;

                    singleTap = new UITapGestureRecognizer(() =>
                    {
                        CommonClass.selectedTrip = temp.TripName;
                        CommonClass.selectedLine = (int)singleTap.View.Tag;
                        NSNotificationCenter.DefaultCenter.PostNotificationName("TripPopShow", new NSString(temp.TripName));
                        NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                    });
                    singleTap2 = new UITapGestureRecognizer(() =>
                    {
                        CommonClass.selectedTrip = temp.TripName;
                        CommonClass.selectedLine = (int)singleTap2.View.Tag;
                        NSNotificationCenter.DefaultCenter.PostNotificationName("TripPopShow", new NSString(temp.TripName));
                        NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                    });
                    singleTap.NumberOfTapsRequired = 2;
                    singleTap2.NumberOfTapsRequired = 2;
                    this.lblTripName[index].AddGestureRecognizer(singleTap);

                    tipGest.RequireGestureRecognizerToFail(singleTap);
                    tipGest2.RequireGestureRecognizerToFail(singleTap2);


                    if (!line.ReserveLine)
                    { // Not Reserve Line
                        if (temp.AMPMType == "1")
                        { // AM
                            lblTripName[index].BackgroundColor = ColorClass.AMTripColor;

                        }
                        else if (temp.AMPMType == "2")
                        { //PM
                            lblTripName[index].BackgroundColor = ColorClass.PMTripColor;

                        }
                        else if (temp.AMPMType == "3")
                        { //Mix
                            lblTripName[index].BackgroundColor = ColorClass.MixedTripColor;

                        }
                    }
                    else
                    { // Reserve Line
                        if (line.LineDisplay.Contains("RR"))
                        {
                            lblTripName[index].BackgroundColor = ColorClass.ReadyReserveTripColor;
                        }
                        else
                        {
                            if (temp.AMPMType == "1")
                            { // AM
                                lblTripName[index].BackgroundColor = ColorClass.AMReserveTripColor;

                            }
                            else if (temp.AMPMType == "2")
                            { //PM
                                lblTripName[index].BackgroundColor = ColorClass.PMReserveTripColor;

                            }
                            else if (temp.AMPMType == "3")
                            { //Mix
                                lblTripName[index].BackgroundColor = ColorClass.MixedTripColor;

                            }
                        }
                    }

                }
                else
                { // No Trips
                  //this.lblTripName[index].UserInteractionEnabled = false;

                }

                //                if (CommonClass.selectedTrip != null && CommonClass.selectedTrip == temp.TripName)
                //                {
                //                    this.lblTripName[index].BackgroundColor = ColorClass.SummaryHeaderColor;

                //                }

                UIView imgv = new UIView();
                imgv.Frame = new CGRect(lblCalDate[index].Frame.Width - 2, 0, 1, 40);
                imgv.BackgroundColor = UIColor.LightGray;
                if (lblCalDate[index].Subviews.Length == 0)
                    lblCalDate[index].AddSubview(imgv);

            

                UIView imgv2 = new UIView();
                imgv2.Frame = new CGRect(22.5 - 2, 0, 1, 40);
                imgv2.BackgroundColor = UIColor.LightGray;
                if (lblTripName[index].Subviews.Length == 0)
                    lblTripName[index].AddSubview(imgv2);

                UIView imgv3 = new UIView();
                imgv3.Frame = new CGRect(22.5 - 2, 0, 1, 40);
                imgv3.BackgroundColor = UIColor.LightGray;


                // Draws Split Cells if vacation is turned on!
                //======================================================================================================================================
                //set the duty period type for each bid line type.

                // prepareModernBidLineView.SetBidLineViewType(line, temp);
                if (temp.BidLineType == (int)BidLineType.CFV)
                {
                    lblTripName[index].BackgroundColor = ColorClass.CFVVacationColor;

                }
                if (temp.BidLineType == (int)BidLineType.FV)
                {
                    lblTripName[index].BackgroundColor = ColorClass.FVVacationColor;

                }
                if (temp.BidLineType == (int)BidLineType.VA)
                {
                    lblTripName[index].BackgroundColor = ColorClass.VacationTripColor;

                }
                else if (temp.BidLineType == (int)BidLineType.VAP)
                {
                    lblTripName[index].BackgroundColor = ColorClass.VAPColor;

                }
                else if (temp.BidLineType == (int)BidLineType.VO)
                {
                    lblTripName[index].BackgroundColor = ColorClass.VacationOverlapTripColor;

                }
                else if (temp.BidLineType == (int)BidLineType.VD)
                {

                    lblTripName[index].BackgroundColor = ColorClass.VacationDropTripColor;

                }
                else if (temp.BidLineType == (int)BidLineType.VDDrop)
                {
                    lblTripName[index].BackgroundColor = UIColor.Clear;

                }

                else if (temp.BidLineType == (int)BidLineType.VOFSplit)
                {
                    drawSplitShapes(lblTripName[index].Frame, shapeOffset, 1, ColorClass.VacationDropTripColor, ColorClass.VacationOverlapTripColor);
                }
                else if (temp.BidLineType == (int)BidLineType.VOFSplitDrop)
                {
                    drawSplitShapes(lblTripName[index].Frame, shapeOffset, 1, UIColor.White, ColorClass.VacationOverlapTripColor);
                }
                else if (temp.BidLineType == (int)BidLineType.VOBSplit)
                {
                    drawSplitShapes(lblTripName[index].Frame, shapeOffset, 2, ColorClass.VacationOverlapTripColor, ColorClass.VacationDropTripColor);
                }
                else if (temp.BidLineType == (int)BidLineType.VOBSplitDrop)
                {
                    drawSplitShapes(lblTripName[index].Frame, shapeOffset, 2, ColorClass.VacationOverlapTripColor, UIColor.White);
                }


                if (GlobalSettings.MenuBarButtonStatus.IsMIL)
                {
                    if (temp.BidLineType == (int)BidLineType.VA)
                    {
                        lblTripName[index].BackgroundColor = UIColor.Orange;

                    }
                    if (temp.BidLineType == (int)BidLineType.VOFSplit)
                    {
                        drawSplitShapes(lblTripName[index].Frame, shapeOffset, 1, UIColor.White, UIColor.Orange);
                    }
                    if (temp.BidLineType == (int)BidLineType.VOBSplit)
                    {
                        drawSplitShapes(lblTripName[index].Frame, shapeOffset, 2, UIColor.Orange, UIColor.White);
                    }

                }


                if (CommonClass.selectedTrip != null && CommonClass.selectedTrip == temp.TripName)
                {
                    this.lblTripName[index].BackgroundColor = ColorClass.BlankLineColor;

                    //RectangleF frame = new RectangleF (new PointF (shapeOffset + lblTripName [index].Frame.X, lblTripName [index].Frame.Y), lblTripName [index].Frame.Size);
                }

                if (index == 33)
                    break;

                index++;
            }

            this.Tag = indexpath.Row;
            vwChild1.Tag = this.Tag;
            vwChild2.Tag = this.Tag;
            foreach (UIView vw in vwChild3)
            {
                vw.Tag = this.Tag;
            }
            //            UILongPressGestureRecognizer longPress1 = new UILongPressGestureRecognizer(handleLongPress);
            //            this.vwChild1.AddGestureRecognizer(longPress1);
            //            longPress1.DelaysTouchesBegan = true;
            //
            //            UILongPressGestureRecognizer longPress2 = new UILongPressGestureRecognizer(handleLongPress);
            //            this.vwChild2.AddGestureRecognizer(longPress2);
            //            longPress2.DelaysTouchesBegan = true;
            //
            //            foreach (UIView vw in vwChild3)
            //            {
            //                UILongPressGestureRecognizer longPress3 = new UILongPressGestureRecognizer(handleLongPress);
            //                vw.AddGestureRecognizer(longPress3);
            //                longPress3.DelaysTouchesBegan = true;
            //            }
            //
            //          UITapGestureRecognizer addColTap = new UITapGestureRecognizer (HandleAdditionalColumn);
            //          vwChild2.AddGestureRecognizer (addColTap);
            //          addColTap.NumberOfTapsRequired = 1;
              


        }
   

		void HandleToolTip (UITapGestureRecognizer obj)
		{
            string strTip = string.Empty;
            CGSize size = new CGSize(strTip.Length * 7 + 40, 40); ;
            bool isNeedToIncreaseHeight = false;
            if (popoverController == null)
            {
                int index = 0;
                foreach (UILabel lbl in lblTripName)
                {
                    if (lbl == obj.View)
                    {
                        if ((int)template1[index].BidLineType == (int)BidLineType.VOBSplit || (int)template1[index].BidLineType == (int)BidLineType.VOBSplitDrop)
                        {
                            isNeedToIncreaseHeight = true;
                            strTip = template1[index].ToolTipBottom + "\n" + template1[index].ToolTip;
                        }
                        else if ((int)template1[index].BidLineType == (int)BidLineType.VOFSplit || (int)template1[index].BidLineType == (int)BidLineType.VOFSplitDrop)
                        {
                            isNeedToIncreaseHeight = true;
                            strTip = template1[index].ToolTip + "\n" + template1[index].ToolTipBottom;
                        }
                        else
                        {
                            isNeedToIncreaseHeight = false;
                            strTip = template1[index].ToolTip;
                        }
                        //strTip = template1 [index].ToolTip;
                    }
                    index++;
                }
                index = 0;
                //foreach (UILabel lbl in lblTripArrival) {
                //    if (lbl == obj.View) {
                //        if ((int)template1 [index].BidLineType == (int)BidLineType.VOBSplit || (int)template1 [index].BidLineType == (int)BidLineType.VOBSplitDrop || (int)template1 [index].BidLineType == (int)BidLineType.VOFSplit || (int)template1 [index].BidLineType == (int)BidLineType.VOFSplitDrop)
                //            strTip = template1 [index].ToolTipBottom;
                //        else
                //            strTip = template1 [index].ToolTip;
                //    }
                //    index++;
                //}
                if (string.IsNullOrEmpty(strTip))
                {
                    strTip = line1.Pairingdesription;
                }
                UILabel testLabel = new UILabel();
                testLabel.Lines = 0;

                //var size = testLabel.StringSize (strTip, UIFont.BoldSystemFontOfSize (15), new CGSize (800, 30));
                //var size = new CGSize (800, 30);

                TooltipViewController toolTip = new TooltipViewController();
                toolTip.tipMessage = strTip;
                popoverController = new UIPopoverController(toolTip);
                popoverController.Delegate = new MyPopDelegate3(this);
                if (isNeedToIncreaseHeight)
                {
                    toolTip.IsNeedToAlignLeft = true;

                    popoverController.PopoverContentSize = new CGSize(strTip.Length * 6, 55);
                }
                else
                {
                    toolTip.IsNeedToAlignLeft = false;
                    popoverController.PopoverContentSize = new CGSize(strTip.Length * 7 + 40, 40);
                }
                popoverController.PresentFromRect(obj.View.ConvertRectToView(obj.View.Bounds, this), this, UIPopoverArrowDirection.Any, true);
			}
		}


//        private string GetLineProperty(string displayName, Line line)
//		{
//			if (displayName == "$/Day") {
//				return line.TfpPerDay.ToString ();
//			} else if (displayName == "$/DHr") {
//				return line.TfpPerDhr.ToString ();
//			} else if (displayName == "$/Hr") {
//				return line.TfpPerFltHr.ToString ();
//			} else if (displayName == "$/TAFB") {
//				return line.TfpPerTafb.ToString ();
//			} else if (displayName == "+Grd") {
//				return line.LongestGrndTime.ToString ();
//			} else if (displayName == "+Legs") {
//				return line.MostLegs.ToString ();
//			} else if (displayName == "+Off") {
//				return line.LargestBlkOfDaysOff.ToString ();
//			} else if (displayName == "1Dy") {
//				return line.Trips1Day.ToString ();
//			} else if (displayName == "2Dy") {
//				return line.Trips2Day.ToString ();
//			} else if (displayName == "3Dy") {
//				return line.Trips3Day.ToString ();
//			} else if (displayName == "4Dy") {
//				return line.Trips4Day.ToString ();
//			} else if (displayName == "8753") {
//				return line.Equip8753.ToString ();
//			} else if (displayName == "A/P") {
//				return line.AMPM.ToString ();
//			} else if (displayName == "ACChg") {
//				return line.AcftChanges.ToString ();
//			} else if (displayName == "ACDay") {
//				return line.AcftChgDay.ToString ();
//			} else if (displayName == "CO") {
//				return line.CarryOverTfp.ToString ();
//			} else if (displayName == "DP") {
//				return line.TotDutyPds.ToString ();
//			} else if (displayName == "DPinBP") {
//				return line.TotDutyPdsInBp.ToString ();
//			} else if (displayName == "EDomPush") {
//				return line.EDomPush;
//			} else if (displayName == "EPush") {
//				return line.EPush;
//			} else if (displayName == "FA Posn") {
//				return string.Join ("", line.FAPositions.ToArray ());
//			} else if (displayName == "Flt") {
//				return line.BlkHrsInBp;
//			} else if (displayName == "LArr") {
//				return line.LastArrTime.ToString ();
//			} else if (displayName == "LDomArr") {
//				return line.LastDomArrTime.ToString ();
//			} else if (displayName == "Legs") {
//				return line.Legs.ToString ();
//			} else if (displayName == "LgDay") {
//				return line.LegsPerDay.ToString ();
//			} else if (displayName == "LgPair") {
//				return line.LegsPerPair.ToString ();
//			} else if (displayName == "ODrop") {
//				return line.OverlapDrop.ToString ();
//			} else if (displayName == "Off") {
//				return line.DaysOff.ToString ();
//			} else if (displayName == "Pairs") {
//				return line.TotPairings.ToString ();
//			} else if (displayName == "Pay" || displayName == "TotPay") {
//				return Decimal.Round (line.Tfp, 2).ToString ();
//			} else if (displayName == "PDiem") {
//				return line.TafbInBp;
//			} else if (displayName == "MyValue") {
//				return Decimal.Round (line.Points, 2).ToString ();
//			} else if (displayName == "SIPs") {
//				return line.Sips.ToString ();
//			} else if (displayName == "StartDOW") {
//				return line.StartDow;
//			} else if (displayName == "T234") {
//				return line.T234;
//			} else if (displayName == "VDrop") {
//				return line.VacationDrop.ToString ();
//			} else if (displayName == "WkEnd") {
//				if (line.Weekend != null)
//					return line.Weekend.ToLower ();
//				else
//					return "";
//			} else if (displayName == "FltRig") {
//				return line.RigFltInBP.ToString ();
//			} else if (displayName == "MinPayRig") {
//				return line.RigDailyMinInBp.ToString ();
//			} else if (displayName == "DhrRig") {
//				return line.RigDhrInBp.ToString ();
//			} else if (displayName == "AdgRig") {
//				return line.RigAdgInBp.ToString ();
//			} else if (displayName == "TafbRig") {
//				return line.RigTafbInBp.ToString ();
//			} else if (displayName == "TotRig") {
//				return line.RigTotalInBp.ToString ();
//			} else if (displayName == "VacPay") {
//				return Decimal.Round (line.VacPay, 2).ToString ();
//			} else if (displayName == "Vofrnt") {
//				return Decimal.Round (line.VacationOverlapFront, 2).ToString ();
//			} else if (displayName == "Vobk") {
//				return Decimal.Round (line.VacationOverlapBack, 2).ToString ();
//			} else if (displayName == "800legs") {
//				return line.LegsIn800.ToString ();
//			} else if (displayName == "700legs") {
//				return line.LegsIn700.ToString ();
//			} else if (displayName == "500legs") {
//				return line.LegsIn500.ToString ();
//			} else if (displayName == "300legs") {
//				return line.LegsIn300.ToString ();
//			} else if (displayName == "DhrInBp") {
//				return line.DutyHrsInBp;
//			} else if (displayName == "DhrInLine") {
//				return line.DutyHrsInLine;
//			} else if (displayName == "Wts") {
//				return Decimal.Round (line.TotWeight, 2).ToString ();
//			} else if (displayName == "FlyPay") {
//				return Decimal.Round (line.FlyPay, 2).ToString ();
//			} else if (displayName == "LineRig") {
//				return Decimal.Round (line.LineRig, 2).ToString ();
//			} else {
//				return "";
//			}
//		}

        void HandleAdditionalColumn(UITapGestureRecognizer obj)
        {
			


			arrObserver=NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("dismissModernClassicPopover"), dismissPopover);
            PopoverViewController popoverContent = new PopoverViewController();
            popoverContent.PopType = "ModernColumns";
            popoverController = new UIPopoverController(popoverContent);
            popoverController.Delegate = new MyPopDelegate(this);
            popoverController.PopoverContentSize = new CGSize(200, 500);
            popoverController.PresentFromRect(obj.View.Frame, this, UIPopoverArrowDirection.Any, true);
        }



		private void ShowPopOverView(NSNotification n)
		{
			arrObserver = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("dismissModernClassicPopover"), dismissPopover);
			PopoverViewController popoverContent = new PopoverViewController();
			popoverContent.PopType = "ModernColumns";
			popoverController = new UIPopoverController(popoverContent);
			popoverController.Delegate = new MyPopDelegate(this);
			popoverController.PopoverContentSize = new CGSize(200, 500);
			popoverController.PresentFromRect(vwChild2.Frame, this, UIPopoverArrowDirection.Left, true);
		}

private void dismissPopover(NSNotification n)
{
			if (popoverController != null)
			{
				popoverController.Dismiss(true);
			}
			else
			{
				
			}
			if(arrObserver != null)
	NSNotificationCenter.DefaultCenter.RemoveObserver(arrObserver);	 
		}
        public void handleLongPress(UILongPressGestureRecognizer gest)
        {
            NSIndexPath path = NSIndexPath.FromRowSection(gest.View.Tag, 0);
            if (path != null)
            {
                CommonClass.selectedLine = GlobalSettings.Lines[path.Row].LineNum;

                if (gest.State == UIGestureRecognizerState.Began)
                    NSNotificationCenter.DefaultCenter.PostNotificationName("ShowPopover", path);
            }
        }

		private void DrawBorder(int width, CGColor color, string name)
{



			BezierPath = new UIBezierPath();
		
BezierPath.MoveTo(new CGPoint(0, 0));
BezierPath.AddLineTo(new CGPoint(imgVLayer.Frame.Width -2, 0));
BezierPath.AddLineTo(new CGPoint(imgVLayer.Frame.Width-2, imgVLayer.Frame.Height));
                BezierPath.AddLineTo(new CGPoint(1, imgVLayer.Frame.Height));
BezierPath.AddLineTo(new CGPoint(1, imgVLayer.Frame.Height));
                BezierPath.ClosePath();
                CAShapeLayer shLayer = new CAShapeLayer();
shLayer.Path = BezierPath.CGPath;
			shLayer.LineWidth = width;
			shLayer.StrokeColor = color;
			shLayer.FillColor = null;
			shLayer.Name = name;
            imgBorder.Layer.AddSublayer(shLayer);
}

private void DrawBorderFirstLine(int width, CGColor color, string name)
{

BezierPath = new UIBezierPath();
BezierPath.MoveTo(new CGPoint(0, 0));
BezierPath.AddLineTo(new CGPoint(imgVLayer.Frame.Width - 2, 0));

BezierPath.ClosePath();
	CAShapeLayer shLayer = new CAShapeLayer();
	shLayer.Path = BezierPath.CGPath;
			shLayer.LineWidth = width;
			shLayer.StrokeColor = color;
			shLayer.Name = name;
	shLayer.FillColor = null;
            imgBorder.Layer.AddSublayer(shLayer);
}

        private void drawSplitShapes(CGRect rect, int offset, int type, UIColor color1, UIColor color2)
        {

          if (type == 1)
            {
               
                imgVLayer.Tag = 1;

                CGPoint point1 = rect.Location;
                UIBezierPath path = new UIBezierPath();
                path.MoveTo(new CGPoint(offset + point1.X, point1.Y));
                path.AddLineTo(new CGPoint(offset + point1.X + 22, point1.Y));
                path.AddLineTo(new CGPoint(offset + point1.X, point1.Y + 40));
                path.AddLineTo(new CGPoint(offset + point1.X, point1.Y));
                path.ClosePath();
                CAShapeLayer shLayer = new CAShapeLayer();
                shLayer.Path = path.CGPath;
                shLayer.FillColor = color1.CGColor;
                imgVLayer.Layer.AddSublayer(shLayer);


                UIBezierPath path1 = new UIBezierPath();
                path1.MoveTo(new CGPoint(offset + point1.X + 22, point1.Y));
                path1.AddLineTo(new CGPoint(offset + point1.X + 22, point1.Y + 40));
                path1.AddLineTo(new CGPoint(offset + point1.X, point1.Y + 40));
                path1.AddLineTo(new CGPoint(offset + point1.X + 22, point1.Y));
                path1.ClosePath();
                CAShapeLayer shLayer1 = new CAShapeLayer();
                shLayer1.Path = path1.CGPath;
                shLayer1.FillColor = color2.CGColor;
                imgVLayer.Layer.AddSublayer(shLayer1);

            }
            else if (type == 2)
            {
               
                imgVLayer.Tag = 1;

                CGPoint point1 = rect.Location;
                UIBezierPath path = new UIBezierPath();
                path.MoveTo(new CGPoint(offset + point1.X, point1.Y));
                path.AddLineTo(new CGPoint(offset + point1.X, point1.Y + 40));
                path.AddLineTo(new CGPoint(offset + point1.X + 22, point1.Y + 40));
                path.AddLineTo(new CGPoint(offset + point1.X, point1.Y));
                path.ClosePath();
                CAShapeLayer shLayer = new CAShapeLayer();
                shLayer.Path = path.CGPath;
                shLayer.FillColor = color1.CGColor;
                imgVLayer.Layer.AddSublayer(shLayer);

                UIBezierPath path1 = new UIBezierPath();
                path1.MoveTo(new CGPoint(offset + point1.X, point1.Y));
                path1.AddLineTo(new CGPoint(offset + point1.X + 22, point1.Y));
                path1.AddLineTo(new CGPoint(offset + point1.X + 22, point1.Y + 40));
                path1.AddLineTo(new CGPoint(offset + point1.X, point1.Y));
                path1.ClosePath();
                CAShapeLayer shLayer1 = new CAShapeLayer();
                shLayer1.Path = path1.CGPath;
                shLayer1.FillColor = color2.CGColor;
                imgVLayer.Layer.AddSublayer(shLayer1);
            }

             //imgVLayer.Layer.ShouldRasterize = true;
           imgVLayer.Layer.DrawsAsynchronously = true;

        }

        private void setSelectButton(int row)
        {
            if (CommonClass.selectedRows.Contains(row))
            {
                this.btnLineSelect.Selected = true;
            }
            else
            {
                this.btnLineSelect.Selected = false;
            }
        }

        partial void btnLineSelectTapped(UIKit.UIButton sender)
        {
            sender.Selected = !sender.Selected;
            NSNumber row = new NSNumber(sender.Tag);
            NSNotificationCenter.DefaultCenter.PostNotificationName("SumRowSelected", row);
        }

    }
}

