using System;
using CoreGraphics;
using Foundation;
using UIKit;
//using System.Collections.ObjectModel;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core.Enum;

namespace WBid.WBidiPad.iOS
{
	public partial class CalenderPopCell : UICollectionViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("CalenderPopCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("CalenderPopCell");

		public CalenderPopCell (IntPtr handle) : base (handle)
		{
		}

		public static CalenderPopCell Create ()
		{
			return (CalenderPopCell)Nib.Instantiate (null, null) [0];
		}
        public void bindData(CalendarData data)
        {
            this.Layer.BorderWidth = 1;
            this.Layer.BorderColor = UIColor.FromRGB(249, 249, 249).CGColor;

            if (data.DepTimeFirstLeg != null)
                this.BackgroundColor = UIColor.FromRGB(246, 246, 246);
            else
                this.BackgroundColor = UIColor.White;

            if (CommonClass.selectedTrip != null && data.TripNumber == CommonClass.selectedTrip)
                this.BackgroundColor = ColorClass.SummaryHeaderColor;

            this.lblDay.Text = data.Day;
            this.lblDepTime.Text = data.DepTimeFirstLeg;
            this.lblArrLeg.Text = data.ArrStaLastLeg;
            this.lblArrTime.Text = data.LandTimeLastLeg;

			if (data.DepTimeFirstLegDecoration == "Strikethrough") {
				NSAttributedString attString = new NSAttributedString (
					data.DepTimeFirstLeg,
					new UIStringAttributes { StrikethroughStyle = NSUnderlineStyle.Single }
				);
				this.lblDepTime.AttributedText = attString;
			}
            else
            {
				if (data.DepTimeFirstLeg != null)
				{
					NSAttributedString attString = new NSAttributedString(
						 data.DepTimeFirstLeg,
						 new UIStringAttributes { StrikethroughStyle = NSUnderlineStyle.None }
					 );
					this.lblDepTime.AttributedText = attString;
				}
			}

			if (data.LandTimeLastLegDecoration == "Strikethrough") {
				NSAttributedString attString = new NSAttributedString (
					data.LandTimeLastLeg,
					new UIStringAttributes { StrikethroughStyle = NSUnderlineStyle.Single }
				);
				this.lblArrTime.AttributedText = attString;
			}
            else
            {
               if(data.LandTimeLastLeg != null)
                {
					NSAttributedString attString = new NSAttributedString(
					  data.LandTimeLastLeg,
					  new UIStringAttributes { StrikethroughStyle = NSUnderlineStyle.None }
				  );
					this.lblArrTime.AttributedText = attString;
				}
            }

            if (data.ColorTop != null)
            {
                if (data.ColorTop == "Red")
                    imgTop.BackgroundColor = ColorClass.OverlapColor;
                else if (data.ColorTop == "Green")
                    imgTop.BackgroundColor = UIColor.Green;
                else
                    imgTop.BackgroundColor = UIColor.Clear;
            }
            else
            {
                imgTop.BackgroundColor = UIColor.Clear;
            }

            if (data.ColorBottom != null)
            {
                if (data.ColorBottom == "Red")
                    imgBot.BackgroundColor = ColorClass.OverlapColor;
                else if (data.ColorBottom == "Green")
                    imgBot.BackgroundColor = ColorClass.VacationOverlapTripColor;

                else if (data.ColorBottom == CalenderColortype.VA.ToString())
                {
                    imgBot.BackgroundColor = ColorClass.VacationTripColor;
                }
                else if (data.ColorBottom == CalenderColortype.VO.ToString())
                {
                    imgBot.BackgroundColor = ColorClass.VacationOverlapTripColor;
                }
                else if (data.ColorBottom == CalenderColortype.VD.ToString())
                {
                    imgBot.BackgroundColor = ColorClass.VacationDropTripColor;
                }
                else if (data.ColorBottom == CalenderColortype.VD.ToString())
                {
                    imgBot.BackgroundColor = ColorClass.VacationDropTripColor;
                }
                else if (data.ColorBottom == CalenderColortype.VD.ToString())
                {
                    imgBot.BackgroundColor = ColorClass.VacationDropTripColor;
                }
                else if (data.ColorBottom == CalenderColortype.FV.ToString())
                {
                    imgBot.BackgroundColor = ColorClass.FVVacationColor;
                }
                else if (data.ColorBottom == CalenderColortype.Transparaent.ToString())
                    imgBot.BackgroundColor = UIColor.Clear;
                else
                    imgBot.BackgroundColor = UIColor.Clear;

				if (data.ColorTop == CalenderColortype.VA.ToString ()) {
					imgTop.BackgroundColor = ColorClass.VacationTripColor;
				} else if (data.ColorTop == CalenderColortype.VO.ToString ()) {
					imgTop.BackgroundColor = ColorClass.VacationOverlapTripColor;
				} else if (data.ColorTop == CalenderColortype.VD.ToString ()) {
					imgTop.BackgroundColor = ColorClass.VacationDropTripColor;
				} else if (data.ColorTop == CalenderColortype.VD.ToString ()) {
					imgTop.BackgroundColor = ColorClass.VacationDropTripColor;
				} else if (data.ColorTop == CalenderColortype.VD.ToString ()) {
					imgTop.BackgroundColor = ColorClass.VacationDropTripColor;
				} else if (data.ColorTop == CalenderColortype.Transparaent.ToString ()) {
					imgTop.BackgroundColor = UIColor.Clear;
				} 
				else if (data.ColorTop == CalenderColortype.MILVO_No_Work.ToString())
				{
					imgTop.BackgroundColor = UIColor.Orange;

				}
				else if (data.ColorTop==CalenderColortype.MILBackSplitWork.ToString())
				{
					imgTop.BackgroundColor = UIColor.Orange;
				
				}
				else if (data.ColorTop == CalenderColortype.MILFrontSplitWork.ToString())
				{
					imgTop.BackgroundColor = UIColor.Clear;

				}
				else if (data.ColorTop==CalenderColortype.MILBackSplitWithoutStrike.ToString())
				{
					imgTop.BackgroundColor = UIColor.Orange;

				}
				else if (data.ColorTop==CalenderColortype.MILFrontSplitWithoutStrike.ToString())
				{
					imgTop.BackgroundColor = UIColor.Clear;

				}
                else if (data.ColorTop == CalenderColortype.FV.ToString())
                {
                    imgTop.BackgroundColor = ColorClass.FVVacationColor;

                }

				 if (data.ColorBottom == CalenderColortype.MILVO_No_Work.ToString())
				{
				
					imgBot.BackgroundColor = UIColor.Orange;
				}
				else if (data.ColorBottom==CalenderColortype.MILBackSplitWork.ToString())
				{

					imgBot.BackgroundColor = UIColor.Clear;
				}
				else if (data.ColorBottom == CalenderColortype.MILFrontSplitWork.ToString())
				{

					imgBot.BackgroundColor = UIColor.Orange;
				}
				else if (data.ColorBottom==CalenderColortype.MILBackSplitWithoutStrike.ToString())
				{

					imgBot.BackgroundColor = UIColor.Clear;
				}
				else if (data.ColorBottom==CalenderColortype.MILFrontSplitWithoutStrike.ToString())
				{

					imgBot.BackgroundColor = UIColor.Orange;
				}
                else if (data.ColorBottom == CalenderColortype.FV.ToString())
                {

                    imgBot.BackgroundColor = ColorClass.FVVacationColor;
                }
            }
            else
            {
                imgBot.BackgroundColor = UIColor.Clear;
            }
        }
	}
}

