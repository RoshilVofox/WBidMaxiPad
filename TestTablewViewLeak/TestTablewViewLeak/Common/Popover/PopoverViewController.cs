using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using System.Linq;
using WBid.WBidiPad.SharedLibrary.Utility;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using System.Collections.Generic;
using WBid.WBidiPad.Core.Enum;

namespace WBid.WBidiPad.iOS
{
	public partial class PopoverViewController : UIViewController
	{
		public string PopType;
		public string SubPopType;
		public int index;
		public string timeValue;
		public string numValue;
		public string dateValue;


		public PopoverViewController () : base ("PopoverViewController", null)
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

            //foreach (var column in GlobalSettings.BidlineAdditionalColumns) {
            //    if (CommonClass.bidLineProperties.Contains (column.DisplayName))
            //        column.IsSelected = true;
            //    else
            //        column.IsSelected = false;
            //}
            //GlobalSettings.BidlineAdditionalColumns = GlobalSettings.BidlineAdditionalColumns.OrderByDescending (x => x.IsSelected == true).ThenBy(y=>y.DisplayName).ToList ();
		
            //foreach (var column in GlobalSettings.ModernAdditionalColumns) {
            //    if (CommonClass.modernProperties.Contains (column.DisplayName))
            //        column.IsSelected = true;
            //    else
            //        column.IsSelected = false;
            //}
            //GlobalSettings.ModernAdditionalColumns = GlobalSettings.ModernAdditionalColumns.OrderByDescending(x => x.IsSelected == true).ThenBy(y => y.DisplayName).ToList();
		
		}
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear(animated);
            if (PopType == "refreshBtnHelp")
            {
				UILabel helpLabel = new UILabel();
				CGRect frame1 = new CGRect(10.0, 20.0, this.View.Frame.Width - 20, this.View.Frame.Height - 30);
				helpLabel.Frame = frame1;
				helpLabel.Text = "If historical bid data does not appear to be available, click the Refresh button and we may be able to upload the missing historical data to our server, so you can then download the historical bid data.";
				helpLabel.Lines = 0;
				helpLabel.TextAlignment = UITextAlignment.Left;
				this.View.AddSubview(helpLabel);
			}
            else if (PopType == "sumOpt") {
				PopoverTableViewControllerController poptable = new PopoverTableViewControllerController ();
				poptable.PopType = PopType;
				//Changed by Francis 6/3/2020
				CGRect frame1 = new CGRect(10.0, 5.0, this.View.Frame.Width, this.View.Frame.Height);
				//poptable.View.Frame = this.View.Frame;
				poptable.View.Frame = frame1;
				this.View.AddSubview (poptable.View);
			} else if (PopType == "addConst") {
				PopoverTableViewControllerController poptable = new PopoverTableViewControllerController ();
				poptable.PopType = PopType;
				//Changed by Francis 6/3/2020
				CGRect frame1 = new CGRect(10.0, 5.0, this.View.Frame.Width, this.View.Frame.Height);
				//poptable.View.Frame = this.View.Frame;
				poptable.View.Frame = frame1;
				this.View.AddSubview (poptable.View);
			} else if (PopType == "sumColumn") {
				PopoverTableViewControllerController poptable = new PopoverTableViewControllerController ();
				poptable.PopType = PopType;
				//Changed by Francis 6/3/2020
				CGRect frame1 = new CGRect(10.0, 5.0, this.View.Frame.Width, this.View.Frame.Height);
				//poptable.View.Frame = this.View.Frame;
				poptable.View.Frame = frame1;
				this.View.AddSubview (poptable.View);
			} else if (PopType == "changeCompareTermInConstraintsCell") {
				SmallPopoverTableController poptable = new SmallPopoverTableController ();
				poptable.PopType = PopType;
				poptable.SubPopType = SubPopType;
				poptable.index = index;
				//Changed by Francis 6/3/2020
				CGRect frame1 = new CGRect(10.0, 5.0, this.View.Frame.Width, this.View.Frame.Height);
				//poptable.View.Frame = this.View.Frame;
				poptable.View.Frame = frame1;
				this.View.AddSubview (poptable.View);
			} else if (PopType == "changeContraintParam") {
				SmallPopoverTableController poptable = new SmallPopoverTableController ();
				poptable.PopType = PopType;
				poptable.SubPopType = SubPopType;
				poptable.index = index;
				//Changed by Francis 6/3/2020
				CGRect frame1 = new CGRect(10.0, 5.0, this.View.Frame.Width, this.View.Frame.Height-15);
				//poptable.View.Frame = this.View.Frame;
				poptable.View.Frame = frame1;
				this.View.AddSubview (poptable.View);
			} 
			else if (PopType == "changefourthcellparamWeightCell") {
				SmallPopoverTableController poptable = new SmallPopoverTableController ();
				poptable.PopType = PopType;
				poptable.SubPopType = SubPopType;
				poptable.index = index;
				//Changed by Francis 6/3/2020
				CGRect frame1 = new CGRect(10.0, 5.0, this.View.Frame.Width, this.View.Frame.Height);
				//poptable.View.Frame = this.View.Frame;
				poptable.View.Frame = frame1;
				this.View.AddSubview (poptable.View);
			} 
			else if (PopType == "changeThirdCellParam") {
				SmallPopoverTableController poptable = new SmallPopoverTableController ();
				poptable.PopType = PopType;
				poptable.SubPopType = SubPopType;
				poptable.index = index;

                //Changed by Francis 6/3/2020
				CGRect frame1 = new CGRect(10.0,5.0,this.View.Frame.Width,this.View.Frame.Height);
				//poptable.View.Frame = this.View.Frame;
				poptable.View.Frame = frame1;
				this.View.AddSubview (poptable.View);
			} else if (PopType == "changeSecondCellParam") {
				SmallPopoverTableController poptable = new SmallPopoverTableController ();
				poptable.PopType = PopType;
				poptable.SubPopType = SubPopType;
				poptable.index = index;
				//Changed by Francis 6/3/2020
				CGRect frame1 = new CGRect(10.0, 5.0, this.View.Frame.Width, this.View.Frame.Height);
				//poptable.View.Frame = this.View.Frame;
				poptable.View.Frame = frame1;
				this.View.AddSubview (poptable.View);
			} else if (PopType == "blockSort") {
				PopoverTableViewControllerController poptable = new PopoverTableViewControllerController ();
				poptable.PopType = PopType;
				poptable.View.Frame = this.View.Frame;
				this.View.AddSubview (poptable.View);
			} else if (PopType == "timePad") {
				UIDatePicker picker = new UIDatePicker (this.View.Frame);
				picker.TimeZone = NSTimeZone.SystemTimeZone;
				picker.Mode = UIDatePickerMode.Time;
				int SystemVersion = Convert.ToInt16(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]);
				if (SystemVersion == 14)
				{
					picker.PreferredDatePickerStyle = UIDatePickerStyle.Wheels;
				}
				picker.Locale = new NSLocale ("NL");
				picker.MinuteInterval = 5;
				this.View.AddSubview (picker);
				picker.Frame = this.View.Frame;
				NSDateFormatter formatter = new NSDateFormatter ();
				formatter.DateFormat = "HH:mm";
				formatter.Locale = new NSLocale ("NL");
				NSDate date = formatter.Parse (timeValue);
				picker.Date = date;
				picker.ValueChanged += HandleTimeValueChanged;
			} else if (PopType == "addWeights") {
				PopoverTableViewControllerController poptable = new PopoverTableViewControllerController ();
				poptable.PopType = PopType;
				//Changed by Francis 6/3/2020
				CGRect frame1 = new CGRect(10.0, 10.0, this.View.Frame.Width, this.View.Frame.Height);
				poptable.View.Frame = frame1;
				this.View.AddSubview (poptable.View);
			} else if (PopType == "changeThirdCellParamInWeightCell") {
				SmallPopoverTableController poptable = new SmallPopoverTableController ();
				poptable.PopType = PopType;
				poptable.SubPopType = SubPopType;
				poptable.index = index;
				//Changed by Francis 6/3/2020
				CGRect frame1 = new CGRect(10.0, 10.0, this.View.Frame.Width, this.View.Frame.Height);
				//poptable.View.Frame = this.View.Frame;
				poptable.View.Frame = frame1;
				this.View.AddSubview (poptable.View);
			} else if (PopType == "changeSecondCellParamInWeightCell") {
				SmallPopoverTableController poptable = new SmallPopoverTableController ();
				poptable.PopType = PopType;
				poptable.SubPopType = SubPopType;
				poptable.index = index;
				//Changed by Francis 6/3/2020
				CGRect frame1 = new CGRect(10.0, 5.0, this.View.Frame.Width, this.View.Frame.Height);
				//poptable.View.Frame = this.View.Frame;
				poptable.View.Frame = frame1;
				this.View.AddSubview (poptable.View);
			} else if (PopType == "changeFirstCellParamInWeightCell") {
				SmallPopoverTableController poptable = new SmallPopoverTableController ();
				poptable.PopType = PopType;
				poptable.SubPopType = SubPopType;
				poptable.index = index;
				//Changed by Francis 6/3/2020
				CGRect frame1 = new CGRect(10.0, 5.0, this.View.Frame.Width, this.View.Frame.Height);
				//poptable.View.Frame = this.View.Frame;
				poptable.View.Frame = frame1;
				this.View.AddSubview (poptable.View);
			} else if (PopType == "changeWeightParamInWeightsCell") {
				NumberPadView numpadView = new NumberPadView ();
				numpadView.PopType = PopType;
				numpadView.SubPopType = SubPopType;
				numpadView.index = index;
				this.AddChildViewController (numpadView);
				//Changed by Francis 7/3/2020

				int SystemVersion = Convert.ToInt16(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]);

				if (SystemVersion > 12)
				{
					CGRect frame1 = new CGRect(0, 0, this.View.Frame.Width, this.View.Frame.Height);
					if (this.PopoverPresentationController.ArrowDirection == UIPopoverArrowDirection.Left)
					{

						frame1 = new CGRect(13.0, 0, this.View.Frame.Width, this.View.Frame.Height);

					}


					if (this.PopoverPresentationController.ArrowDirection == UIPopoverArrowDirection.Up)
					{ frame1 = new CGRect(0, 5.0, this.View.Frame.Width, this.View.Frame.Height); }

					numpadView.View.Frame = frame1;
				}
				else
				{
					numpadView.View.Frame = this.View.Frame;
				}
				this.View.AddSubview(numpadView.View);
			} else if (PopType == "changeWeightParamInDOWCell") {
				NumberPadView numpadView = new NumberPadView ();
				numpadView.PopType = PopType;
				numpadView.SubPopType = SubPopType;
				numpadView.numValue = numValue;
				numpadView.index = index;
				Console.WriteLine(this.PopoverPresentationController.ArrowDirection);
				this.AddChildViewController (numpadView);
				//Changed by Francis 7/3/2020

				int SystemVersion = Convert.ToInt16(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]);

				if (SystemVersion > 12)
				{
					CGRect frame1 = new CGRect(0, 0, this.View.Frame.Width, this.View.Frame.Height);
					if (this.PopoverPresentationController.ArrowDirection == UIPopoverArrowDirection.Left)
					{

						frame1 = new CGRect(13.0, 0, this.View.Frame.Width, this.View.Frame.Height);

					}


					if (this.PopoverPresentationController.ArrowDirection == UIPopoverArrowDirection.Up)
					{ frame1 = new CGRect(0, 5.0, this.View.Frame.Width, this.View.Frame.Height); }

					numpadView.View.Frame = frame1;
				}
				else
				{
					numpadView.View.Frame = this.View.Frame;
				}
				this.View.AddSubview (numpadView.View);
			} else if (PopType == "changeWeightParamInDOMCell") {
				NumberPadView numpadView = new NumberPadView ();
				numpadView.PopType = PopType;
				numpadView.SubPopType = SubPopType;
				numpadView.numValue = numValue;
				numpadView.index = index;
				//Changed by Francis 7/3/2020

				int SystemVersion = Convert.ToInt16(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]);

				if (SystemVersion > 12)
				{
					CGRect frame1 = new CGRect(0, 0, this.View.Frame.Width, this.View.Frame.Height);
					if (this.PopoverPresentationController.ArrowDirection == UIPopoverArrowDirection.Left)
					{

						frame1 = new CGRect(13.0, 0, this.View.Frame.Width, this.View.Frame.Height);

					}


					if (this.PopoverPresentationController.ArrowDirection == UIPopoverArrowDirection.Up)
					{ frame1 = new CGRect(0, 5.0, this.View.Frame.Width, this.View.Frame.Height); }

					numpadView.View.Frame = frame1;
				}
				else
				{
					numpadView.View.Frame = this.View.Frame;
				}
				this.View.AddSubview(numpadView.View);
			} else if (PopType == "changeWeightParamInCommutableLine") {
				NumberPadView numpadView = new NumberPadView ();
				numpadView.PopType = PopType;
				numpadView.SubPopType = SubPopType;
				numpadView.numValue = numValue;
				numpadView.index = index;
				//Changed by Francis 7/3/2020

				int SystemVersion = Convert.ToInt16(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]);

				if (SystemVersion > 12)
				{
					CGRect frame1 = new CGRect(0, 0, this.View.Frame.Width, this.View.Frame.Height);
					if (this.PopoverPresentationController.ArrowDirection == UIPopoverArrowDirection.Left)
					{

						frame1 = new CGRect(13.0, 0, this.View.Frame.Width, this.View.Frame.Height);

					}


					if (this.PopoverPresentationController.ArrowDirection == UIPopoverArrowDirection.Up)
					{ frame1 = new CGRect(0, 5.0, this.View.Frame.Width, this.View.Frame.Height); }

					numpadView.View.Frame = frame1;
				}
				else
				{
					numpadView.View.Frame = this.View.Frame;
				}
				this.View.AddSubview(numpadView.View);
			} else if (PopType == "changeOvernightBulkWeight") {
				NumberPadView numpadView = new NumberPadView ();
				numpadView.PopType = PopType;
				numpadView.SubPopType = SubPopType;
				numpadView.numValue = numValue;
				numpadView.index = index;
				//this.AddChildViewController (numpadView);
				//Changed by Francis 7/3/2020

				int SystemVersion = Convert.ToInt16(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]);

				if (SystemVersion > 12)
				{
					CGRect frame1 = new CGRect(0, 0, this.View.Frame.Width, this.View.Frame.Height);
					if (this.PopoverPresentationController.ArrowDirection == UIPopoverArrowDirection.Left)
					{

						frame1 = new CGRect(13.0, 0, this.View.Frame.Width, this.View.Frame.Height);

					}


					if (this.PopoverPresentationController.ArrowDirection == UIPopoverArrowDirection.Up)
					{ frame1 = new CGRect(0, 5.0, this.View.Frame.Width, this.View.Frame.Height); }

					numpadView.View.Frame = frame1;
				}
				else
				{
					numpadView.View.Frame = this.View.Frame;
				}
				this.View.AddSubview(numpadView.View);
			} else if (PopType == "numberPad") {
				NumberPadView numpadView = new NumberPadView ();
				numpadView.PopType = PopType;
				numpadView.SubPopType = SubPopType;
				numpadView.numValue = numValue;
				numpadView.index = index;
				//Changed by Francis 7/3/2020


				int SystemVersion = Convert.ToInt16(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]);

				if (SystemVersion > 12)
				{
					CGRect frame1 = new CGRect(0, 0, this.View.Frame.Width, this.View.Frame.Height);
					if (this.PopoverPresentationController.ArrowDirection == UIPopoverArrowDirection.Left)
					{

						frame1 = new CGRect(13.0, 0, this.View.Frame.Width, this.View.Frame.Height);

					}


					if (this.PopoverPresentationController.ArrowDirection == UIPopoverArrowDirection.Up)
					{ frame1 = new CGRect(0, 5.0, this.View.Frame.Width, this.View.Frame.Height); }

					numpadView.View.Frame = frame1;
				}
                else
                {
					numpadView.View.Frame = this.View.Frame;
				}


				
				this.View.AddSubview(numpadView.View);
			} else if (PopType == "datePad") {
				UIDatePicker picker = new UIDatePicker (this.View.Frame);
				picker.TimeZone = NSTimeZone.SystemTimeZone;
				picker.Mode = UIDatePickerMode.Date;
				this.View.AddSubview (picker);
				List<DateHelper> lstDates = ConstraintBL.GetPartialDayList ();
				string stDate = lstDates [0].Date.ToString ("MM-dd-yyyy");
				string enDate = lstDates [lstDates.Count - 1].Date.ToString ("MM-dd-yyyy");
				NSDateFormatter formatter = new NSDateFormatter ();
				formatter.DateFormat = "MM-dd-yyyy";
				picker.MinimumDate = formatter.Parse (stDate);
				picker.MaximumDate = formatter.Parse (enDate);
				NSDate date = formatter.Parse (dateValue);
				picker.Date = date;
				picker.ValueChanged += HandleDateChange;
			} else if (PopType == "BidlineColumns" || PopType == "ModernColumns") {
				PopoverTableViewControllerController poptable = new PopoverTableViewControllerController ();
				poptable.PopType = PopType;
				//Changed by Francis 6/3/2020
				CGRect frame1 = new CGRect(10.0, 5.0, this.View.Frame.Width, this.View.Frame.Height);
				//poptable.View.Frame = this.View.Frame;
				poptable.View.Frame = frame1;
				this.View.AddSubview (poptable.View);
			}else if (PopType == "changeValueCommutabilitySort") {
                PopoverTableViewControllerController poptable = new PopoverTableViewControllerController ();
                poptable.PopType = PopType;
				//Changed by Francis 6/3/2020
				CGRect frame1 = new CGRect(10.0, 5.0, this.View.Frame.Width, this.View.Frame.Height);
				//poptable.View.Frame = this.View.Frame;
				poptable.View.Frame = frame1;
				this.View.AddSubview (poptable.View);
            }
			else if (PopType == "NumeratorData")
			{
				SmallPopoverTableController poptable = new SmallPopoverTableController();
				poptable.PopType = PopType;
				poptable.SubPopType = SubPopType;
				poptable.index = index;
				//Changed by Francis 6/3/2020
				CGRect frame1 = new CGRect(10.0, 5.0, this.View.Frame.Width, this.View.Frame.Height);
				//poptable.View.Frame = this.View.Frame;
				poptable.View.Frame = frame1;
				this.View.AddSubview(poptable.View);
			}


		}

		void HandleDateChange (object sender, EventArgs e)
		{
			NSDate date = (sender as UIDatePicker).Date;
			NSDateFormatter dateformat = new NSDateFormatter ();
			dateformat.TimeZone = NSTimeZone.SystemTimeZone;
			dateformat.DateFormat = "MM-dd-yyyy";
			string dateStr = dateformat.ToString (date);
			NSNotificationCenter.DefaultCenter.PostNotificationName("ChangeDateText",new NSString(dateStr));

		}
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		void HandleTimeValueChanged (object sender, EventArgs e)
		{
			NSDate date = (sender as UIDatePicker).Date;
			NSDateFormatter dateformat = new NSDateFormatter ();
			dateformat.TimeZone = NSTimeZone.SystemTimeZone;
			dateformat.DateFormat = "HH:mm";
			dateformat.Locale = new NSLocale ("NL");
			string time = dateformat.ToString (date);
			if(SubPopType=="Define")
				NSNotificationCenter.DefaultCenter.PostNotificationName("ChangeDefineTimeText",new NSString(time));
			else
				NSNotificationCenter.DefaultCenter.PostNotificationName("ChangeTimeText",new NSString(time));
		}
	}
}

