using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.SharedLibrary;
using System.Linq;
using WBid.WBidiPad.Core;
using System.Collections.Generic;

namespace WBid.WBidiPad.iOS
{

	public partial class summaryListCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName("summaryListCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString("summaryListCell");

		public summaryListCell(IntPtr handle) : base(handle)
		{
			//SubviewCreation (this);
			laySubViews (this);
			this.addTapGesture ();
		}

		public static summaryListCell Create()
		{
			summaryListCell cell = (summaryListCell)Nib.Instantiate(null, null)[0];
			//laySubViews (cell);

			return cell;
		}


		public void addTapGesture()
		{
			UITapGestureRecognizer doubleTap = new UITapGestureRecognizer (handleDoubleTap);
			doubleTap.NumberOfTapsRequired = 2;
			this.AddGestureRecognizer (doubleTap);

			UITapGestureRecognizer singleTap = new UITapGestureRecognizer (handleSingleTap);
			singleTap.NumberOfTapsRequired = 1;
			this.AddGestureRecognizer (singleTap);

			singleTap.RequireGestureRecognizerToFail (doubleTap);

		}

		public void handleDoubleTap(UITapGestureRecognizer gest)
		{
			UITableViewCell cell = (UITableViewCell)gest.View;
			NSNotificationCenter.DefaultCenter.PostNotificationName ("CalPopover", cell);


		}

		public void handleSingleTap(UITapGestureRecognizer gest)
		{
			NSNotificationCenter.DefaultCenter.PostNotificationName ("CalPopHide", null);
			NSIndexPath path = NSIndexPath.FromRowSection (gest.View.Tag, 0);
			if (path != null) {
				string GroupName = GlobalSettings.Lines [path.Row].BAGroup;
				if (GroupName != null) {
					if (GroupName.Length > 0)
						try
						{
							NSNotificationCenter.DefaultCenter.PostNotificationName("ShowGroupBidAutomator", null);
						}
					catch(Exception ex)
						{ 
					}
				}
			}
		}

		public void SubviewCreation(summaryListCell cell)
		{
			laySubViews (cell);
		}


		public static  void laySubViews(summaryListCell cell)
		{
			Console.WriteLine (cell);

			//foreach (UIView vw in cell.ContentView)
			//{
			//	vw.RemoveFromSuperview();
			//}



			UIImageView imgBack = new UIImageView();
			imgBack.Frame = new CGRect (0, 0, 900, 50);
			imgBack.Tag = 1010;
			cell.ContentView.AddSubview(imgBack);
			UILongPressGestureRecognizer longPress = new UILongPressGestureRecognizer (handleLongPress);
			imgBack.AddGestureRecognizer (longPress);
			imgBack.UserInteractionEnabled = true;
			longPress.DelaysTouchesBegan = true;

			//Here lay subviews as per column defenition(CD) and INI. CD will be already updated in order as per INI. So only care CD.
			//width should be obtained from CD. Set tag as CD property "id". 
			//So that we can access the label back using id and assign value.
			int x = 0;//x position
			int y = 0;//y postion
			const int h = 45;//height, const
			var colCount = 0;
			if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
				colCount = GlobalSettings.WBidINIContent.SummaryVacationColumns.Count;
			else
				colCount = GlobalSettings.WBidINIContent.DataColumns.Count;
			for (int i = 0; i < colCount; i++)
			{//Max 22 
				DataColumn dataColumn = null;
				if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
					dataColumn = GlobalSettings.WBidINIContent.SummaryVacationColumns[i];
				else
					dataColumn = GlobalSettings.WBidINIContent.DataColumns[i];

				ColumnDefinition cd = GlobalSettings.columndefinition.Where(k => k.Id == dataColumn.Id).FirstOrDefault();

				UILabel lbl = new UILabel();
				lbl.Frame = new CGRect(x, y, cd.Width, h);
				lbl.Tag = 3000 + cd.Id;//Adding a const to not to mess things up with something else.
				//				lbl.Text = cd.DataPropertyName;//For testing purpose only.
				x = x + cd.Width;
				//				cell.Add (lbl);
				cell.ContentView.AddSubview(lbl);
				if (cd.DisplayName == "87M" || cd.DisplayName=="+Grd")
				{
					//we need to reduce the font size of 87m column
					lbl.Font = UIFont.SystemFontOfSize(11);
				}
				else
				{
					lbl.Font = UIFont.SystemFontOfSize(14);
				}
				lbl.TextAlignment = UITextAlignment.Center;

				UIImageView imgV = new UIImageView();
				imgV.Frame = new CGRect(12 + lbl.Frame.X, 10, 20, 20);
				imgV.Tag = 2000 + cd.Id;
				cell.ContentView.AddSubview(imgV);
				UITextField txtF = new UITextField ();
				txtF.Frame = new CGRect (lbl.Frame.X, 6, 50, 30);
				txtF.BorderStyle = UITextBorderStyle.RoundedRect;
				txtF.ReturnKeyType = UIReturnKeyType.Done;
				txtF.AutocorrectionType = UITextAutocorrectionType.No;
				txtF.AutocapitalizationType = UITextAutocapitalizationType.None;
				txtF.Font = UIFont.SystemFontOfSize (13);
				txtF.Tag = 4000 + cd.Id;
				txtF.EditingDidBegin += (object sender, EventArgs e) => {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("CalPopHide", null);
				} ;
				txtF.ShouldReturn = ((textField) => {
					string text = textField.Text;


					if (GlobalSettings.Lines[(int)cell.Tag].Tag != text )
					{
						GlobalSettings.Lines[(int)cell.Tag].Tag = text;
						GlobalSettings.isModified = true;
						GlobalSettings.TagDetails = new TagDetails ();
						GlobalSettings.TagDetails.AddRange (GlobalSettings.Lines.ToList ().Where (itm => itm.Tag != null && itm.Tag.Trim () != string.Empty).Select (newitem => new Tag {
							Line = newitem.LineNum,
							Content = newitem.Tag
						}));
						CommonClass.lineVC.UpdateSaveButton ();
					}

					textField.ResignFirstResponder ();
					return true;
				} );
				txtF.EditingDidEnd += (object sender, EventArgs e) => {
					UITextField field = (UITextField)sender;
					string text = field.Text;

					if (GlobalSettings.Lines[(int)cell.Tag].Tag != text )
					{
						GlobalSettings.Lines[(int)cell.Tag].Tag = text;
						GlobalSettings.isModified = true;
						GlobalSettings.TagDetails = new TagDetails ();
						GlobalSettings.TagDetails.AddRange (GlobalSettings.Lines.ToList ().Where (itm => itm.Tag != null && itm.Tag.Trim () != string.Empty).Select (newitem => new Tag {
							Line = newitem.LineNum,
							Content = newitem.Tag
						}));
						CommonClass.lineVC.UpdateSaveButton ();
					}

				} ;
				if (cd.DisplayName == "Tag")
					cell.ContentView.AddSubview (txtF);

				if (CommonClass.showGrid) {
					lbl.Layer.BorderWidth = 1;
					lbl.Layer.BorderColor = ColorClass.SummaryHeaderBorderColor.CGColor;
				}
			}
			if (CommonClass.showGrid) {
				cell.Layer.BorderWidth = 1;
				cell.Layer.BorderColor = ColorClass.SummaryHeaderBorderColor.CGColor;
			}

		}


		partial void btnSelectTapped(UIKit.UIButton sender)
		{
			sender.Selected = !sender.Selected;
			NSNumber row = new NSNumber(sender.Tag);
			NSNotificationCenter.DefaultCenter.PostNotificationName("SumRowSelected", row);
		}

		private void setSelectButton(int row)
		{
			if (CommonClass.selectedRows.Contains(row))
			{
				this.btnSelect.Selected = true;
			}
			else
			{
				this.btnSelect.Selected = false;
			}

		}

		public void RemoveAllViews(summaryListCell cell)
		{
			foreach (UIView vw in cell.ContentView)
			{
				if (vw == btnSelect)
				{
				}
				else
					vw.RemoveFromSuperview();
			}

			cell.SubviewCreation(cell);
		}
		public void bindData(Line line, NSIndexPath indexPath)
		{
			UIView scrlVw = this.ContentView;
			UIView[] subVws = scrlVw.Subviews;
			//laySubViews(this);

		    

			this.Tag = indexPath.Row;

			//			UIImageView imgBack = new UIImageView();
			//			imgBack.Frame = new RectangleF (0, 0, 900, 50);
			//			imgBack.Tag = 1000 + indexPath.Row;
			//			this.AddSubview(imgBack);

			//this.btnSelect.Frame = new CGRect(920, 7, 30, 30);
			this.btnSelect.Tag = line.LineNum;
			this.btnSelect.SetImage(UIImage.FromBundle("roundActive.png"), UIControlState.Selected);
			this.btnSelect.SetImage(UIImage.FromBundle("roundNormal.png"), UIControlState.Normal);

			setSelectButton(line.LineNum);

		//this.Subviews[0];
			//UIScrollView scrlVw = (UIScrollView)this.Subviews[0];


			foreach (UIView vw in subVws) {
				if (vw.Tag >= 3000 && vw.Tag < 4000) {
					UILabel lbl = (UILabel)vw;

						//laySubViews(this);

					if (CommonClass.showGrid)
					{
						lbl.Layer.BorderWidth = 1;
						lbl.Layer.BorderColor = ColorClass.SummaryHeaderBorderColor.CGColor;
						this.Layer.BorderWidth = 1;
						this.Layer.BorderColor = ColorClass.SummaryHeaderBorderColor.CGColor;

					}
					else {
						lbl.Layer.BorderWidth = 0;
						this.Layer.BorderWidth = 0;
					}
					ColumnDefinition cd = GlobalSettings.columndefinition.Where (k => k.Id == lbl.Tag - 3000).FirstOrDefault ();

					if (cd.DisplayName == "Ord") {
						lbl.Text = (indexPath.Row + 1).ToString ();
					}  else {
						lbl.Text = CommonClass.GetLineProperty (cd.DisplayName, line);
						if(cd.DisplayName== "787m8m")
							lbl.Font = UIFont.SystemFontOfSize(9);

					}

					//					if (cd.DisplayName == "Ord") {
					//						lbl.Text = (indexPath.Row + 1).ToString ();
					//					}  else if (cd.DisplayName == "Lock") {
					//						lbl.Text = "";//line.Lock.ToString ();
					//					}  else if (cd.DisplayName == "Constraint") {
					//						lbl.Text = "";//line.Constrained.ToString ();
					//					}  else if (cd.DisplayName == "Overlap") {
					//						lbl.Text = "";//line.ShowOverLap.ToString ();
					//					}  else if (cd.DisplayName == "Line") {
					//						lbl.Text = line.LineDisplay;
					//					}  else if (cd.DisplayName == "$/Day") {
					//						lbl.Text = line.TfpPerDay.ToString ();
					//					}  else if (cd.DisplayName == "$/DHr") {
					//						lbl.Text = line.TfpPerDhr.ToString ();
					//					}  else if (cd.DisplayName == "$/Hr") {
					//						lbl.Text = line.TfpPerFltHr.ToString ();
					//					}  else if (cd.DisplayName == "$/TAFB") {
					//						lbl.Text = line.TfpPerTafb.ToString ();
					//					}  else if (cd.DisplayName == "+Grd") {
					//						lbl.Text = line.LongestGrndTime.ToString (@"hh\:mm");
					//					}  else if (cd.DisplayName == "+Legs") {
					//						lbl.Text = line.MostLegs.ToString ();
					//					}  else if (cd.DisplayName == "+Off") {
					//						lbl.Text = line.LargestBlkOfDaysOff.ToString ();
					//					}  else if (cd.DisplayName == "1Dy") {
					//						lbl.Text = line.Trips1Day.ToString ();
					//					}  else if (cd.DisplayName == "2Dy") {
					//						lbl.Text = line.Trips2Day.ToString ();
					//					}  else if (cd.DisplayName == "3Dy") {
					//						lbl.Text = line.Trips3Day.ToString ();
					//					}  else if (cd.DisplayName == "4Dy") {
					//						lbl.Text = line.Trips4Day.ToString ();
					//					}  else if (cd.DisplayName == "8753") {
					//						lbl.Font = UIFont.SystemFontOfSize(10);
					//						if (line.Equip8753 == null)
					//							lbl.Text = string.Empty;
					//						else
					//							lbl.Text = line.Equip8753.ToString ();
					//					}  else if (cd.DisplayName == "A/P") {
					//						lbl.Text = line.AMPM.ToString ();
					//					}  else if (cd.DisplayName == "ACChg") {
					//						lbl.Text = line.AcftChanges.ToString ();
					//					}  else if (cd.DisplayName == "ACDay") {
					//						lbl.Text = line.AcftChgDay.ToString ();
					//					}  else if (cd.DisplayName == "CO") {
					//						lbl.Text = line.CarryOverTfp.ToString ();
					//					}  else if (cd.DisplayName == "DP") {
					//						lbl.Text = line.TotDutyPds.ToString ();
					//					}  else if (cd.DisplayName == "DPinBP") {
					//						lbl.Text = line.TotDutyPdsInBp.ToString ();
					//					}  else if (cd.DisplayName == "EDomPush") {
					//						lbl.Text = line.EDomPush;
					//					}  else if (cd.DisplayName == "EPush") {
					//						lbl.Text = line.EPush;
					//					}  else if (cd.DisplayName == "FA Posn") {
					//						lbl.Text = string.Join ("", line.FAPositions.ToArray ());
					//					}  else if (cd.DisplayName == "Flt") {
					//						lbl.Text = line.BlkHrsInBp;
					//					}  else if (cd.DisplayName == "LArr") {
					//						lbl.Text = line.LastArrTime.ToString (@"hh\:mm");
					//					}  else if (cd.DisplayName == "LDomArr") {
					//						lbl.Text = line.LastDomArrTime.ToString (@"hh\:mm");
					//					}  else if (cd.DisplayName == "Legs") {
					//						lbl.Text = line.Legs.ToString ();
					//					}  else if (cd.DisplayName == "LgDay") {
					//						lbl.Text = line.LegsPerDay.ToString ();
					//					}  else if (cd.DisplayName == "LgPair") {
					//						lbl.Text = line.LegsPerPair.ToString ();
					//					}  else if (cd.DisplayName == "ODrop") {
					//						lbl.Text = line.OverlapDrop.ToString ();
					//					}  else if (cd.DisplayName == "Off") {
					//						lbl.Text = line.DaysOff.ToString ();
					//					}  else if (cd.DisplayName == "Pairs") {
					//						lbl.Text = line.TotPairings.ToString ();
					//					}  else if (cd.DisplayName == "Pay") {
					//						lbl.Text =  Decimal.Round (line.Tfp, 2).ToString ();
					//					}  else if (cd.DisplayName == "PDiem") {
					//						lbl.Text = line.TafbInBp;
					//					}  else if (cd.DisplayName == "MyValue") {
					//
					//                        lbl.Text = Decimal.Round(line.Points, 2).ToString();
					//                            //line.Points.ToString();
					//                         
					//					}  else if (cd.DisplayName == "SIPs") {
					//						lbl.Text = line.Sips.ToString ();
					//					}  else if (cd.DisplayName == "StartDOW") {
					//						lbl.Text = line.StartDow;
					//					}  else if (cd.DisplayName == "T234") {
					//						lbl.Text = line.T234;
					////					}  else if (cd.DisplayName == "Tag") {
					//
					//					}  else if (cd.DisplayName == "VDrop") {
					//                        lbl.Text = line.VacationDrop.ToString();
					//					}  else if (cd.DisplayName == "WkEnd") {
					//						if (line.Weekend != null)
					//							lbl.Text = line.Weekend.ToLower ();
					//						else
					//							lbl.Text = "";
					//					}  else if (cd.DisplayName == "FltRig") {
					//						lbl.Text = line.RigFltInBP.ToString ();
					//					}  else if (cd.DisplayName == "MinPayRig") {
					//						lbl.Text = line.RigDailyMinInBp.ToString ();
					//					}  else if (cd.DisplayName == "DhrRig") {
					//						lbl.Text = line.RigDhrInBp.ToString ();
					//					}  else if (cd.DisplayName == "AdgRig") {
					//						lbl.Text = line.RigAdgInBp.ToString ();
					//					}  else if (cd.DisplayName == "TafbRig") {
					//						lbl.Text = line.RigTafbInBp.ToString ();
					//					}  else if (cd.DisplayName == "TotRig") {
					//						lbl.Text = line.RigTotalInBp.ToString ();
					////                    else if (cd.DisplayName == "TopLock")
					////                    {
					////
					////                    }
					////                    else if (cd.DisplayName == "BotLock")
					////                    {
					////
					////                    }
					//					}  else if (cd.DisplayName == "VacPay") {
					//						lbl.Text = Decimal.Round (line.VacPay, 2).ToString ();
					//					}  else if (cd.DisplayName == "Vofrnt") {
					//						lbl.Text = Decimal.Round (line.VacationOverlapFront, 2).ToString ();
					//					}  else if (cd.DisplayName == "Vobk") {
					//						lbl.Text = Decimal.Round (line.VacationOverlapBack, 2).ToString ();
					//					}  else if (cd.DisplayName == "800legs") {
					//						lbl.Text = line.LegsIn800.ToString ();
					//					}  else if (cd.DisplayName == "700legs") {
					//						lbl.Text = line.LegsIn700.ToString ();
					//					}  else if (cd.DisplayName == "500legs") {
					//						lbl.Text = line.LegsIn500.ToString ();
					//					}  else if (cd.DisplayName == "300legs") {
					//						lbl.Text = line.LegsIn300.ToString ();
					//					}  else if (cd.DisplayName == "DhrInBp") {
					//						lbl.Text = line.DutyHrsInBp;
					//					}  else if (cd.DisplayName == "DhrInLine") {
					//						lbl.Text = line.DutyHrsInLine;
					//					}  else if (cd.DisplayName == "Wts") {
					//                        lbl.Text = Math.Round( line.TotWeight,2,MidpointRounding.AwayFromZero).ToString();
					//                            //Decimal.Round(line.TotWeight, 2).ToString ();
					//					}  else if (cd.DisplayName == "LineRig") {
					//						lbl.Text = Decimal.Round (line.LineRig, 2).ToString ();
					//					}  else {
					//						lbl.Text = "";
					//					}

				}  else if (vw.Tag >= 2000 && vw.Tag < 3000) {
					UIImageView imgV = (UIImageView)vw;
					ColumnDefinition cd = GlobalSettings.columndefinition.Where (k => k.Id == imgV.Tag - 2000).FirstOrDefault ();
					if (cd.DisplayName == "Lock") {
						if (line.TopLock) {
							imgV.Image = UIImage.FromBundle ("lockIconGreen.png");
						}  else if (line.BotLock) {
							imgV.Image = UIImage.FromBundle ("lockIconRed.png");
						}  else {
							imgV.Image = null;
						}
					}  else if (cd.DisplayName == "Constraint") {
						if (line.Constrained) {
							imgV.Image = UIImage.FromBundle ("deleteIconBold.png");
						}  else {
							imgV.Image = null;
						}
					}
					else if (cd.DisplayName == "Overlap")
					{
						//string str = line.ShowOverLap.ToString ();
						if (line.ShowOverLap)
						{
							imgV.Image = UIImage.FromBundle("overlayIconBold.png");
						}
						else
						{
							imgV.Image = null;
						}

					}
				}  else if (vw.Tag >= 1000 && vw.Tag < 2000) {
					//					UIImageView imgV = (UIImageView)vw;
					//					UILongPressGestureRecognizer longPress = new UILongPressGestureRecognizer (handleLongPress);
					//					imgV.AddGestureRecognizer (longPress);
					//					imgV.UserInteractionEnabled = true;
					//					longPress.DelaysTouchesBegan = true;
					vw.Tag = 1010 + indexPath.Row;
				}  else if (vw.Tag >= 4000) {
					UITextField txtF = (UITextField)vw;

					this.BringSubviewToFront (vw);
					if (line.Tag == null)
						line.Tag = "";

					txtF.Text = line.Tag;
				}
			}

		}
		static void handleLongPress (UILongPressGestureRecognizer gest) 
		{
			NSIndexPath path = NSIndexPath.FromRowSection (gest.View.Tag-1010, 0);
			if (path != null) {
				CommonClass.selectedLine = GlobalSettings.Lines [path.Row].LineNum;
				if (gest.State == UIGestureRecognizerState.Began)
					NSNotificationCenter.DefaultCenter.PostNotificationName ("ShowPopover", path);
			}
		}


	}
}


