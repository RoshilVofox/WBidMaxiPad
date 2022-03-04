using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Model.SWA;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.SharedLibrary.SWA;
using WBidDataDownloadAuthorizationService.Model;
using System.Text.RegularExpressions;
using System.ServiceModel;
using System.Collections.Generic;
using WBid.WBidiPad.Model;
using System.Linq;
using System.IO;

namespace WBid.WBidiPad.iOS
{
	public partial class viewFileViewController : UIViewController
	{
		public string domicileName;
		public string displayType;
        NSNotification ViewFileNotification;
        NSObject notif;
		public viewFileViewController () : base ("viewFileViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		public override void ViewWillDisappear(bool animated)
		{
			
			base.ViewWillDisappear(animated);


		}
		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
            NSNotificationCenter.DefaultCenter.RemoveObserver(notif);

		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
            notif=NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("RealoadViewFile"), SetAvailabilityofFileStatus);
			pckrDomicileSelect.Model = new pickerViewModel (this);

			int indexToSelect = GlobalSettings.WBidINIContent.Domiciles.IndexOf (GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault (x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile));
			pckrDomicileSelect.Select (indexToSelect, 0, true);
			domicileName = GlobalSettings.CurrentBidDetails.Domicile;
			// Perform any additional setup after loading the view, typically from a nib.
			if (displayType == "cover") {
				if(GlobalSettings.CurrentBidDetails.Round=="S")
					selectReserveFile(0);
				else
					selectMonthlyFile(0);	
			} else if (displayType == "seniority") {
				if(GlobalSettings.CurrentBidDetails.Round=="S")
					selectReserveFile(1);
				else
					selectMonthlyFile(1);	
			} else if (displayType == "awards") {
				if(GlobalSettings.CurrentBidDetails.Round=="S")
					selectReserveFile(2);
				else
					selectMonthlyFile(2);	
			}

		}
		public override void ViewDidAppear (bool animated)
		{
			SetAvailabilityofFileStatus();

		}
		partial void btnSelectDomicileTapped (UIKit.UIButton sender)
		{
			UIButton btn = (UIButton)sender;
			string[] arr = GlobalSettings.WBidINIContent.Domiciles.Select(x=>x.DomicileName).ToArray();
			UIActionSheet sheet = new UIActionSheet("Select Domicile",null,"Cancel",null,arr);
			sheet.ShowFrom(btn.Frame,this.View,true);
			sheet.Clicked += handleDomicleSelect;
		}

		void handleDomicleSelect (object sender, UIButtonEventArgs e)
		{
			List<Domicile> listDomicile = GlobalSettings.WBidINIContent.Domiciles;
			domicileName = listDomicile [(int)e.ButtonIndex].DomicileName;
			SetAvailabilityofFileStatus();
		}
		public void selectMonthlyFile(int n)
		{
			sgMonthlyFile.SelectedSegment = n;
			sgReserveFile.SelectedSegment = -1;
		}
		public void selectReserveFile(int n)
		{
			sgMonthlyFile.SelectedSegment = -1;
			sgReserveFile.SelectedSegment = n;
		}

		partial void btnCancelTapped (Foundation.NSObject sender)
		{
			
			this.DismissViewController(true, null);
			foreach (UIView view in this.View.Subviews) {

				DisposeClass.DisposeEx(view);
			}
		}

        partial void btnViewFileTapped(Foundation.NSObject sender)
        {
            webPrint fileViewer = new webPrint();
			fileViewer.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            this.PresentViewController(fileViewer, true, () =>
            {
                fileViewer.loadFileFromUrl(GetFileNametoOpen());
            });
        }
		partial void sgMonthlyFileTapped (UIKit.UISegmentedControl sender)
		{
			sgReserveFile.SelectedSegment = -1;
			SetAvailabilityofFileStatus();

		}
		partial void sgPositionTapped (UIKit.UISegmentedControl sender)
		{
			SetAvailabilityofFileStatus();

		}
		partial void sgReserveFileTapped (UIKit.UISegmentedControl sender)
		{
			sgMonthlyFile.SelectedSegment = -1;
			SetAvailabilityofFileStatus();

		}
        /// <summary>
        /// Create the filename to open 
        /// </summary>
        /// <returns></returns>
        private string GetFileNametoOpen()
        {
            string filename = string.Empty;
			if (domicileName != "Select Domicile")
            {
				if ((sgMonthlyFile.SelectedSegment == 0) || (sgReserveFile.SelectedSegment == 0))
                {
                    filename = CreateCoverletterFileName();
                }
				else if ((sgMonthlyFile.SelectedSegment == 1) || (sgReserveFile.SelectedSegment == 1))
                {
                    filename = CreateSeniorityLetterFileName();
                }
				else if ((sgMonthlyFile.SelectedSegment == 2) || (sgReserveFile.SelectedSegment == 2))
                {
                    filename = CreateBidAwardFileName();
                }
            }
            return filename;
        }

        /// <summary>
        /// Create the Cover letter name to open the file
        /// </summary>
        /// <returns></returns>
        private string CreateCoverletterFileName()
        {
            string coverletterfileName = string.Empty;
			if (sgMonthlyFile.SelectedSegment == 0)
            {
                //first round cover letter
				coverletterfileName = domicileName + getLongPositionString((int)sgPosition.SelectedSegment) + "C" + ".TXT";
            }
			else if (sgReserveFile.SelectedSegment == 0)
            {
				if (sgPosition.SelectedSegment == 2)
                {
                    // get flight attendant 2nd round cover letter and seniority list
					coverletterfileName = domicileName + getLongPositionString((int)sgPosition.SelectedSegment) + "CR" + ".TXT";

                }
                else
                {
                    // get pilot 2nd round cover letter and seniority list
					coverletterfileName = domicileName + getLongPositionString((int)sgPosition.SelectedSegment) + "R" + ".TXT";
                }
            }
            return coverletterfileName;
        }

        /// <summary>
        /// Create the seniority filename to open the file
        /// </summary>
        /// <returns></returns>
        private string CreateSeniorityLetterFileName()
        {
            string senioritylistfilename = string.Empty;
			if (sgMonthlyFile.SelectedSegment == 1)
            {
                //first round seniority list
				senioritylistfilename = domicileName + getLongPositionString((int)sgPosition.SelectedSegment) + "S" + ".TXT";
            }
			else if (sgReserveFile.SelectedSegment == 1)
            {
				if (sgPosition.SelectedSegment == 2)
                {
                    // get flight attendant 2nd round cover letter and seniority list
					senioritylistfilename = domicileName + getLongPositionString((int)sgPosition.SelectedSegment) + "SR" + ".TXT";

                }
                else
                {
                    // get pilot 2nd round cover letter and seniority list
					senioritylistfilename = domicileName + getLongPositionString((int)sgPosition.SelectedSegment) + "R" + ".TXT";
                }
            }
            return senioritylistfilename;
        }
        /// <summary>
        /// Create Bid Award filename
        /// </summary>
        /// <returns></returns>
        private string CreateBidAwardFileName()
        {
            string bidawardfileName = string.Empty;
			if (sgMonthlyFile.SelectedSegment == 2)
            {
                // get pilot and flight attend first round  award filename.
				bidawardfileName = domicileName + getLongPositionString((int)sgPosition.SelectedSegment) + "M" + ".TXT";
            }
			else if (sgReserveFile.SelectedSegment == 2)
            {
                // get pilot and  flight attendant 2nd round  award filename
				bidawardfileName = domicileName + getLongPositionString((int)sgPosition.SelectedSegment) + "W" + ".TXT";
            }
            return bidawardfileName;
        }
		private string getLongPositionString(int n)
		{
			if (n == 0)
				return "CP";
			else if (n == 1)
				return "FO";
			else if (n == 2)
				return "FA";
			else
				return "";
		}
		/// <summary>
		/// If file exists set the IsFileAvaialable property to true
		/// </summary>
		/// <param name="fileName"></param>
        public void SetAvailabilityofFileStatus(NSNotification n)
		{
			//get the file name to open
			string fileName = GetFileNametoOpen();
			if (fileName != string.Empty) {
				if (File.Exists (WBidHelper.GetAppDataPath () + "/" + fileName)) {
					// enable  the view file button 
					btnViewFile.Enabled = true;
				} else {
					// Disable  the view file button 
					btnViewFile.Enabled = false;

				}
			}
			else
				// Disable  the view file button 
				btnViewFile.Enabled = false;
            
			}
        public void SetAvailabilityofFileStatus()
        {
            //get the file name to open
            string fileName = GetFileNametoOpen();
            if (fileName != string.Empty)
            {
                if (File.Exists(WBidHelper.GetAppDataPath() + "/" + fileName))
                {
                    // enable  the view file button 
                    btnViewFile.Enabled = true;
                }
                else
                {
                    // Disable  the view file button 
                    btnViewFile.Enabled = false;

                }
            }
            else
                // Disable  the view file button 
                btnViewFile.Enabled = false;

        }
	}

	public class pickerViewModel : UIPickerViewModel
	{
		viewFileViewController parent;
		public pickerViewModel(viewFileViewController parentVC)
		{
			parent = parentVC;
		}

		public override nint GetComponentCount (UIPickerView picker)
		{
			return 1;
		}

		public override nint GetRowsInComponent (UIPickerView picker, nint component)
		{
			string[] arr = GlobalSettings.WBidINIContent.Domiciles.Select(x=>x.DomicileName).ToArray();
			return arr.Count();
		}

		public override string GetTitle (UIPickerView picker, nint row, nint component)
		{
			return GlobalSettings.WBidINIContent.Domiciles.Select (x => x.DomicileName).ToArray () [row];
		}
		public override void Selected (UIPickerView picker, nint row, nint component)
		{
			parent.domicileName = GlobalSettings.WBidINIContent.Domiciles.Select (x => x.DomicileName).ToArray () [row];
            NSNotificationCenter.DefaultCenter.PostNotificationName("RealoadViewFile", null);

		}

	}
}

