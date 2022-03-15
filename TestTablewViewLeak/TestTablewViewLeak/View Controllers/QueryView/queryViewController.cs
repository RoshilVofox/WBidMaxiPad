using System;
using CoreGraphics;
using Foundation;
using UIKit;
using CoreGraphics;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.iOS;
using WBid.WBidiPad.Model;
using System.Collections.Generic;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using System.Linq;
using WBid.WBidiPad.iOS.Utility;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using WBid.WBidiPad.SharedLibrary.SWA;
using iOSPasswordStorage;
using WBidDataDownloadAuthorizationService.Model;
using System.ServiceModel;
using WBid.WBidiPad.Core;
using System.Threading.Tasks;
using System.IO;
using Security;
using System.Drawing;
using System.Text;
using System.Json;
using System.Collections.Specialized;
using WBid.WBidiPad.Core.Enum;

namespace WBid.WBidiPad.iOS
{
    public partial class queryViewController : UIViewController,IServiceDelegate
	{

		WBidDataDwonloadAuthServiceClient client;
		LoadingOverlay loadingOverlay;
		public bool isFirstTime;
		int bidIndex = 0;
		List<string> bidrecipt;
		private string _sessionCredentials = string.Empty;
		Guid token;
		bool isnetavailable;
		//NSObject notif;
		public UIPopoverController popoverController;
		public MyPopDelegate objPopDelegate;
		public queryViewController ()
			: base ("queryViewController", null)
		{
		}
public enum queryFromView
{
queryBidEditorFA = 0,
queryBidEditorCPORFO,
querySubmitBid	  }

		public queryFromView isFromView;
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidAppear (bool animated)
		{
			if (!isFirstTime) 
			{
				
				if ((bidIndex < bidrecipt.Count)) 
				{
					string bidString = string.Empty;
					InvokeOnMainThread (() => {
						
							bidString = (bidIndex == 0) ? string.Empty : " Buddy ";
                        UIAlertController okAlertController = UIAlertController.Create("WBidMax", "\n\n Your " + bidString + " Bid Was Successfully Submitted.\n\n A bid Reciept was saved in the " + bidrecipt[bidIndex] + " file .\n\n This Receipt will Display next.Please Review you bid and check for accuracy!", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);
                        SubmitwebPrint fileViewer = new SubmitwebPrint ();
						switch (isFromView)
						{
							case queryFromView.querySubmitBid : fileViewer.isFromView =SubmitwebPrint.FromView.SubmitBid ; break;
							case queryFromView.queryBidEditorFA : fileViewer.isFromView =SubmitwebPrint.FromView.BidEditorFA ; break;
							case queryFromView.queryBidEditorCPORFO : fileViewer.isFromView = SubmitwebPrint.FromView.BidEditorCPORFO ; break;		
						}






							this.PresentViewController(fileViewer, true, () =>
								{
									isFirstTime=false;
								if(Path.GetExtension(bidrecipt[bidIndex]).ToLower()==".rct")
									{
									fileViewer.loadFileFromUrl(bidrecipt[bidIndex]);
									}
									else
									fileViewer.LoadPDFdocument (Path.GetFileName(bidrecipt[bidIndex]));

								bidIndex++;
								});

							

						
						loadingOverlay.RemoveFromSuperview ();
					});

				}

				isFirstTime = true;

			}
			base.ViewDidAppear (animated);
		}
        public void ServiceResponce(JsonValue jsonDoc)
        {
            InvokeOnMainThread(() =>
            {
               // GlobalSettings.WBidStateCollection.SubmittedResult = CommonClass.ConvertJSonToObject<BidSubmittedData>(jsonDoc.ToString()).SubmittedResult;

            });
        }
        public void ResponceError(string Error)
        {
            InvokeOnMainThread(() => {
                //ActivityIndicator.Hide();
                Console.WriteLine("Service Fail");

                UIAlertController okAlertController = UIAlertController.Create("WBidMax", Error, UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
            });
        }
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			var appearance = new UINavigationBarAppearance();
			appearance.ConfigureWithOpaqueBackground();
			appearance.BackgroundColor = ColorClass.TopHeaderColor;
			this.NavigationItem.StandardAppearance = appearance;
			this.NavigationItem.ScrollEdgeAppearance = this.NavigationItem.StandardAppearance;

			BasicHttpBinding binding = ServiceUtils.CreateBasicHttp ();
			client = new WBidDataDwonloadAuthServiceClient (binding, ServiceUtils.EndPoint);
			client.GetAuthorizationforMultiPlatformCompleted += client_GetAuthorizationforMultiPlatformCompleted;
			if (GlobalSettings.CurrentBidDetails.Postion == "CP") {
				//hide buddy bid, show avoidance, avoidance disabled
				vwBuddyBid.Hidden = true;
				vwAvoidance.Hidden = false;
				vwAvoidance.UserInteractionEnabled = false;
				vwAvoidance.Alpha = 0.8f;
			} else if (GlobalSettings.CurrentBidDetails.Postion == "FO") {
				//hide buddy bid, show avoidance
				vwBuddyBid.Hidden = true;
				vwAvoidance.Hidden = false;
				vwAvoidance.UserInteractionEnabled = true;
				vwAvoidance.Alpha = 1.0f;
			} else {
				//hide avoidance bid, show buddy
				vwAvoidance.Hidden = true;
				vwAvoidance.UserInteractionEnabled = false;
				vwAvoidance.Hidden = false;
			}
			LoadDatatoBidQueryWindow ();

		}

		#region WCFEvents


		private void client_GetAuthorizationforMultiPlatformCompleted (object sender, GetAuthorizationforMultiPlatformCompletedEventArgs e)
		{

			ServiceResponseModel serviceResponseModel = new ServiceResponseModel ();
			try {
				if (e.Result != null) {
					serviceResponseModel = e.Result;
				}

			} catch (Exception ex) {
				serviceResponseModel.IsAuthorized = true;
				try {
					client.LogTimeOutDetailsAsync (token);
				} catch (Exception exc) {

				}

			}

			if (serviceResponseModel.IsAuthorized)
			{
				submitBidAllOperations ();

			} else {
				InvokeOnMainThread (() => {

                    UIAlertController okAlertController = UIAlertController.Create("Error", serviceResponseModel.Message, UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                    loadingOverlay.RemoveFromSuperview ();
				});
			}


		}

		#endregion

		#region PrivateMethods
		private void submitBidAllOperations ()
		{
			SubmitBid submitBid = GlobalSettings.SubmitBid;
			SWASubmitBid swaSubmit = new SWASubmitBid ();
			InvokeOnMainThread (() =>  {
				loadingOverlay.updateLoadingText ("Submitting Bid...");
			});
			bidrecipt = Submitbid ();
			if (bidrecipt.Count > 0) {
				string bidString = string.Empty;
				bidIndex = 0;
				InvokeOnMainThread (() =>  {
					foreach(var item in bidrecipt)
                    { 
					bidString = (bidIndex == 0) ? string.Empty : " Buddy ";
                    UIAlertController okAlertController = UIAlertController.Create("WBidMax", "\n\n Your " + bidString + " Bid Was Successfully Submitted.\n\n A bid Reciept was saved in the " + bidrecipt [bidIndex] + " file .\n\n This Receipt will Display next.Please Review you bid and check for accuracy!", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, alert =>
                    {
                    SubmitwebPrint fileViewer = new SubmitwebPrint();                     //fileViewer.isFromView =SubmitwebPrint.FromView.SubmitBid ;                     switch (isFromView)                     {                         case queryFromView.querySubmitBid: fileViewer.isFromView = SubmitwebPrint.FromView.SubmitBid; break;                         case queryFromView.queryBidEditorFA: fileViewer.isFromView = SubmitwebPrint.FromView.BidEditorFA; break;                         case queryFromView.queryBidEditorCPORFO: fileViewer.isFromView = SubmitwebPrint.FromView.BidEditorCPORFO; break;                         default: break;                     }                       this.PresentViewController (fileViewer, true, () =>  {                      isFirstTime = false;                        if (Path.GetExtension (bidrecipt [bidIndex]).ToLower () == ".rct") {                            fileViewer.loadFileFromUrl (bidrecipt [bidIndex]);                      }                       else                            fileViewer.LoadPDFdocument (Path.GetFileName (bidrecipt [bidIndex]));                       bidIndex++;                     } );                    loadingOverlay.RemoveFromSuperview (); 

                    }));
                    this.PresentViewController(okAlertController, true, null);
                   }
				});
			}
		}
		/// <summary>
		/// Submit the bid.it return the List<string> which contains the list of the submitted result.
		/// </summary>
		/// <returns></returns>
		private List<string> Submitbid ()
		{
			List<string> bidrecipt = new List<string> ();
			try {
				SWASubmitBid swasubmitbid = new SWASubmitBid ();

				List<string> bidders = new List<string> ();
				bidders.Add (GlobalSettings.SubmitBid.Bidder);

				//When submitting a  buddy bid, it should submit either 2 or 3 bids, depending upon if there are 2 or 3 buddy bidders.
				if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Round!="S") 
				{
					if (GlobalSettings.SubmitBid.Buddy1 != null && GlobalSettings.SubmitBid.Buddy1 != "0")
						bidders.Add (GlobalSettings.SubmitBid.Buddy1);
					if (GlobalSettings.SubmitBid.Buddy2 != null && GlobalSettings.SubmitBid.Buddy2 != "0")
						bidders.Add (GlobalSettings.SubmitBid.Buddy2);
				}
				int count = 0;
                string submbittingbid = GlobalSettings.SubmitBid.Bid;
				foreach (string buddybidder in bidders) {
					string bidder = buddybidder;
					GlobalSettings.SubmitBid.Bidder = bidder;

					if (GlobalSettings.CurrentBidDetails.Postion == "FA") {
						//first bidder is always the user.other usesr are buddy bidders for that user.so we need to set the 
						if (count != 0) {
							var bid = GlobalSettings.SubmitBid.BuddyBidderBids.FirstOrDefault (x => x.BuddyBidder == bidder);
							if (bid != null)
								GlobalSettings.SubmitBid.Bid = GlobalSettings.SubmitBid.BuddyBidderBids.FirstOrDefault (x => x.BuddyBidder == bidder).BidLines;
						}
						//When submitting a  buddy bid, it should submit either 2 or 3 bids, depending upon if there are 2 or 3 buddy bidders.
						//this will get the buddy bidders other than the bidder.
						List<string> buddybidders = bidders.Where (x => x != bidder).ToList ();
						if (buddybidders.Count () == 1) {
							GlobalSettings.SubmitBid.Buddy1 = buddybidders [0];
						} else if (buddybidders.Count () == 2) {
							GlobalSettings.SubmitBid.Buddy1 = buddybidders [0];
							GlobalSettings.SubmitBid.Buddy2 = buddybidders [1];
						}
						if (GlobalSettings.SubmitBid.Buddy1 == "0")
							GlobalSettings.SubmitBid.Buddy1 = null;
						if (GlobalSettings.SubmitBid.Buddy2 == "0")
							GlobalSettings.SubmitBid.Buddy2 = null;


						count++;

					}
					SubmitBid submitBid = GlobalSettings.SubmitBid;
					SWASubmitBid swaSubmit = new SWASubmitBid ();
					InvokeOnMainThread (() => {
						loadingOverlay.updateLoadingText ("Submitting Bid for " + GlobalSettings.SubmitBid.Bidder);
					});

                    //string submitResult = "45,66,55,33,11,22,44 SUBMITTED BY";
					string submitResult = swaSubmit.SubmitBid (submitBid, _sessionCredentials);

					if (submitResult.Contains ("SUBMITTED BY:")) 
                    {
						string fileName = submitResult.Substring (0, submitResult.IndexOf ("\n")) + "Rct.pdf";
						if (fileName != null) {
                            //System.IO.File.WriteAllText (WBidHelper.GetAppDataPath () + "/" + fileName, submitResult);
                            if (CommonClass.SaveFormatBidReceipt(submitResult))
                            {
                                Task.Factory.StartNew(() =>
                                {
                                    WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
                                    //obgWBidLogEvent.LogBidSubmitDetails (GlobalSettings.SubmitBid, GlobalSettings.SubmitBid.Bidder);
                                    obgWBidLogEvent.LogAllEvents(GlobalSettings.SubmitBid.Bidder, "submitBid", GlobalSettings.SubmitBid.Buddy1, GlobalSettings.SubmitBid.Buddy2,"");
                                });
                            }
                            else
                            {
                                try
                                {
                                    WBidMail mail = new WBidMail();
                                    mail.SendMailtoAdmin("User got the bid receipt, but failed to save it in the app data folder.", GlobalSettings.WbidUserContent.UserInformation.Email, "Bid receipt not saved locally");
                                   
                                }
                                catch(Exception ex)
                                {
                                    
                                }
                            }
							bidrecipt.Add (fileName);
							if (GlobalSettings.WBidINIContent.User.IsNeedBidReceipt) {
								SendEmailBidReceipt (fileName);
							}
						}
						try
						{
							string submittedby = string.Empty;
							string submittedfor = string.Empty;
							string submitteddtg = string.Empty;
							string submittedbid = string.Empty;
							string[] stringSeparators = new string[] { "SUBMITTED BY:" };
							var splittedstring = submitResult.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);

							string[] lastlineseparator = new string[] { "  " };
							var lastsplittedString = splittedstring[1].Split(lastlineseparator, StringSplitOptions.RemoveEmptyEntries);
							if (lastsplittedString.Count() > 2)
							{
								submittedby = Regex.Replace(lastsplittedString[0], @"[^\d]", "");
								submittedfor = lastsplittedString[1];
								submitteddtg = lastsplittedString[2].Replace("\r\n", "");
								submittedbid = splittedstring[0].Substring(splittedstring[0].IndexOf('\n'), splittedstring[0].Length - splittedstring[0].IndexOf('\n')).Replace('\n', ',').Replace("*E,", ",").Trim().TrimEnd(',').TrimStart(',');

							}
							else
							{
								submittedbid = GlobalSettings.SubmitBid.Bid;
							}
							int submitbidder = 0;
							if ((GlobalSettings.SubmitBid.Bidder.Contains('x')) || (GlobalSettings.SubmitBid.Bidder.Contains('X')))
								submitbidder = int.Parse(GlobalSettings.SubmitBid.Bidder.Replace("x", "").Replace("X", ""));
							else
								submitbidder = int.Parse(GlobalSettings.SubmitBid.Bidder.Replace("e", "").Replace("E", ""));
							
							if (submittedfor == string.Empty || submittedfor == null)
							{

								InvokeOnMainThread(() =>
								{

									UIAlertController okAlertController = UIAlertController.Create("WBidMax", " Your bid receipt has been returned with NO employee number.This can occur when you are on a leave of absence.Please contact us if you are not on a leave of absence", UIAlertControllerStyle.Alert);

									okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
									this.PresentViewController(okAlertController, true, null);
									loadingOverlay.RemoveFromSuperview();

								});

								Task.Factory.StartNew(() =>
								{
									WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
									obgWBidLogEvent.LogAllEvents(GlobalSettings.SubmitBid.Bidder, "MissingEmpNumReceipt", GlobalSettings.SubmitBid.Buddy1, GlobalSettings.SubmitBid.Buddy2, "");

								});
							}

							SaveSubmittedDataToDB(submitbidder, submittedbid, submittedby, submittedfor, submitteddtg, submitBid, _sessionCredentials);

							GlobalSettings.WBidStateCollection.SubmittedResult = submbittingbid;
						}
						catch (Exception ex)
						{
						}
					} else if (submitResult == "server failure") {
						WBidLogEvent obgWBidLogEvent = new WBidLogEvent ();

						obgWBidLogEvent.LogAllEvents(GlobalSettings.TemporaryEmployeeNumber,"bidSubmitTimeOut",GlobalSettings.SubmitBid.Buddy1,GlobalSettings.SubmitBid.Buddy2, submitResult);
						SaveSubmittedRawDataToDB(submitBid, _sessionCredentials);
						//obgWBidLogEvent.LogTimeoutBidSubmitDetails (GlobalSettings.SubmitBid, GlobalSettings.TemporaryEmployeeNumber);
						InvokeOnMainThread (() => {
							
							showTimeOutAlert(false);
							loadingOverlay.RemoveFromSuperview ();
						});
					} else {
						WBidLogEvent obgWBidLogEvent = new WBidLogEvent();

						obgWBidLogEvent.LogAllEvents(GlobalSettings.TemporaryEmployeeNumber, "bidSubmitTimeOut", GlobalSettings.SubmitBid.Buddy1, GlobalSettings.SubmitBid.Buddy2, submitResult);
						InvokeOnMainThread (() => {

							string mesasgeAlert = submitResult;
							if (submitResult.Contains("STRINGINDEXOUTOFBOUNDSEXCEPTION"))
							{
								mesasgeAlert = "Your bid receipt has been returned with NO employee number.This can occur when you are on a leave of absence.Please contact us if you are not on a leave of absence";

								WBidLogEvent obgWBidLogEvent1 = new WBidLogEvent();
								obgWBidLogEvent1.LogAllEvents(GlobalSettings.SubmitBid.Bidder, "MissingEmpNumReceipt", GlobalSettings.SubmitBid.Buddy1, GlobalSettings.SubmitBid.Buddy2, "");
							}

							UIAlertController okAlertController = UIAlertController.Create("WBidMax", mesasgeAlert, UIAlertControllerStyle.Alert);

                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                            loadingOverlay.RemoveFromSuperview ();

						});
						SaveSubmittedRawDataToDB(submitBid,_sessionCredentials);
					}
				}
			} catch (Exception ex) {

				InvokeOnMainThread (() => {

					throw ex;
				});
			}
			return bidrecipt;
		}
		private void SaveSubmittedRawDataToDB(SubmitBid submitBid, string sessioncredential)
		{
			try
			{
				bool isConnectionAvailable = Reachability.CheckVPSAvailable();
				if (isConnectionAvailable)
				{
					int employeenumber = 0;
					if ((GlobalSettings.SubmitBid.Bidder.Contains('x')) || (GlobalSettings.SubmitBid.Bidder.Contains('X')))
						employeenumber = int.Parse(GlobalSettings.SubmitBid.Bidder.Replace("x", "").Replace("X", ""));
					else
						employeenumber = int.Parse(GlobalSettings.SubmitBid.Bidder.Replace("e", "").Replace("E", ""));
					OdataBuilder objbuilder = new OdataBuilder();
					objbuilder.RestService.Objdelegate = this;
					SubmittedDataRaw biddetails = new SubmittedDataRaw();
					biddetails.Domicile = GlobalSettings.CurrentBidDetails.Domicile;
					biddetails.Month = GlobalSettings.CurrentBidDetails.Month;
					biddetails.Year = GlobalSettings.CurrentBidDetails.Year;
					biddetails.Position = GlobalSettings.CurrentBidDetails.Postion;
					biddetails.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
					biddetails.EmployeeNumber = employeenumber.ToString();
					biddetails.RawData = GenerateSubmittedRawData(submitBid, sessioncredential);
					biddetails.FromApp = (int)Core.Enum.FromApp.WbidmaxIpad;
					objbuilder.SaveBidSubmittedRawData(biddetails);
				

				}
			}
			catch (Exception ex)
			{
			}
		}
		private string GenerateSubmittedRawData(SubmitBid submitBid, string sessioncredential)
		{
			//set the formdata values
			NameValueCollection formData = new NameValueCollection();
			formData["REQUEST"] = "UPLOAD_BID";
			formData["BASE"] = submitBid.Base;
			formData["BID"] = submitBid.Bid;
			formData["BIDDER"] = submitBid.Bidder;
			formData["BIDROUND"] = submitBid.BidRound;
			//formData["CLOSEDBIDSIM"] = "N";
			formData["CREDENTIALS"] = sessioncredential;
			formData["PACKETID"] = submitBid.PacketId;
			formData["SEAT"] = submitBid.Seat;
			formData["VENDOR"] = "WBidMax";
			// should always be null for CP and FA
			if (submitBid.Pilot1 != null) formData["PILOT1"] = submitBid.Pilot1;
			if (submitBid.Pilot2 != null) formData["PILOT2"] = submitBid.Pilot2;
			if (submitBid.Pilot3 != null) formData["PILOT3"] = submitBid.Pilot3;
			// should always be null for CP and FO
			if (submitBid.Buddy1 != null) formData["BUDDY1"] = submitBid.Buddy1;
			if (submitBid.Buddy2 != null) formData["BUDDY2"] = submitBid.Buddy2;
			string submittedraw = string.Empty;
			foreach (var item in formData.Keys)
			{
				submittedraw += item.ToString() + ":" + formData.GetValues(item.ToString())[0] + ",";
			}
			return submittedraw;
		}
		private void SaveSubmittedDataToDB(int empnum, string bid,string submittedby,string submittedfor,string submitteddtg, SubmitBid submitBid, string sessioncredential)
        {
            try
            {
                if (Reachability.CheckVPSAvailable())
                {
                    OdataBuilder ObjOdata = new OdataBuilder();
                    ObjOdata.RestService.Objdelegate = this;
                    BidSubmittedData biddetails = new BidSubmittedData();
                    biddetails.Domicile = GlobalSettings.CurrentBidDetails.Domicile;
                    biddetails.Month = GlobalSettings.CurrentBidDetails.Month;
                    biddetails.Year = GlobalSettings.CurrentBidDetails.Year;
                    biddetails.Position = GlobalSettings.CurrentBidDetails.Postion;
                    biddetails.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
                    biddetails.EmpNum = empnum;
                    biddetails.SubmittedResult = bid;
                    biddetails.SubmitBy = submittedby;
                    biddetails.SubmitFor = submittedfor;
                    biddetails.SubmitDTG = submitteddtg;
                    biddetails.FromApp = (int)Core.Enum.FromApp.WbidmaxIpad;
					biddetails.RawData = GenerateSubmittedRawData(submitBid,  sessioncredential);
					ObjOdata.SaveBidSubmittedData(biddetails);
                }
            }
            catch (Exception ex)
            {
            }
        }
		void showTimeOutAlert(bool isNeedToDismiss)
		{
			//-----
			TimeOutAlertView regs = new TimeOutAlertView();
			popoverController = new UIPopoverController(regs);
			objPopDelegate = new MyPopDelegate(this);
			objPopDelegate.CanDismiss = false;
			popoverController.Delegate = objPopDelegate;
            regs.objQueryView = this;
            regs.objpopover = popoverController;
            CGRect frame = new CGRect((View.Frame.Size.Width / 2) - 75, (View.Frame.Size.Height / 2) - 175, 150, 350);
			popoverController.PopoverContentSize = new CGSize(regs.View.Frame.Width, regs.View.Frame.Height);
			popoverController.PresentFromRect(frame, View, 0, true);

			//------
		}
		private void SendEmailBidReceipt (string bidFileName)
		{

			WBidMail objMailAgent = new WBidMail ();
			//if (GlobalSettings.WBidINIContent.User != null && GlobalSettings.WBidINIContent.User.IsOn && GlobalSettings.WbidUserContent != null && GlobalSettings.WbidUserContent.UserInformation != null && !string.IsNullOrEmpty(GlobalSettings.WbidUserContent.UserInformation.Email))
			if (GlobalSettings.WbidUserContent != null && GlobalSettings.WbidUserContent.UserInformation != null && !string.IsNullOrEmpty (GlobalSettings.WbidUserContent.UserInformation.Email)) {
				if (File.Exists (WBidHelper.GetAppDataPath () + "/" + bidFileName)) {
					byte[] attachment = System.IO.File.ReadAllBytes (WBidHelper.GetAppDataPath () + "/" + bidFileName);
					objMailAgent.SendMailtoUser ("Hi <Br/> Please find the attached Bid Receipt. <Br/><Br/> WBidMax", GlobalSettings.WbidUserContent.UserInformation.Email, "Bid Receipt", attachment, bidFileName);
				}
			}

		}

		partial void back (UIKit.UIBarButtonItem sender)
		{
			this.DismissViewController (true, null);
		}


		/// <summary>
		/// Set all the Informations to the UI.
		/// </summary>
		private void LoadDatatoBidQueryWindow ()
		{
			string employeeNumber = string.Empty;
			employeeNumber = GlobalSettings.TemporaryEmployeeNumber;
			lblQueryHeader.Text = "Submitting " + GlobalSettings.SubmitBid.TotalBidChoices + " Bid Choices for";
			txtSubmitBid.Text = employeeNumber;
			if (GlobalSettings.CurrentBidDetails.Postion == "FA")
				setUpBuddyBidLabel (GetBuddyBid ());
			if (GlobalSettings.CurrentBidDetails.Postion == "FO")
				setupAvoidanceChoiceLabel (GetAvoidanceBid ());
		}

		/// <summary>
		/// Get Buddy Bid String
		/// </summary>
		/// <returns></returns>
		private string GetBuddyBid ()
		{
			string buddyBidStr = string.Empty;
			if (GlobalSettings.CurrentBidDetails.Round != "S") {
				
				BuddyBids buddyBids = GlobalSettings.WBidINIContent.BuddyBids;
				//disable buddy bid
				buddyBidStr += (buddyBids.Buddy1 != "0") ? buddyBids.Buddy1.ToString () + "," : "";
				buddyBidStr += (buddyBids.Buddy2 != "0") ? buddyBids.Buddy2.ToString () + "," : "";
				buddyBidStr = buddyBidStr.TrimEnd (',');
			}
			return buddyBidStr;

		}

		/// <summary>
		/// Get Avoidance Bid string
		/// </summary>
		/// <returns></returns>
		private string GetAvoidanceBid ()
		{
			string avoidanceBidsStr = string.Empty;
			AvoidanceBids avoidancebids = GlobalSettings.WBidINIContent.AvoidanceBids;
			avoidanceBidsStr += (avoidancebids.Avoidance1 != "0") ? avoidancebids.Avoidance1.ToString () + "," : "";
			avoidanceBidsStr += (avoidancebids.Avoidance2 != "0") ? avoidancebids.Avoidance2.ToString () + "," : "";
			avoidanceBidsStr += (avoidancebids.Avoidance3 != "0") ? avoidancebids.Avoidance3.ToString () : "";
			avoidanceBidsStr = avoidanceBidsStr.TrimEnd (',');
			return avoidanceBidsStr;
		}

		/// <summary>
		/// set the Buddy bid labels
		/// </summary>
		/// <param name="str"></param>
		private void setUpBuddyBidLabel (string str)
		{
			if (str == null || str == string.Empty) {
				txtBuddyBid.Text = string.Empty;
				lblBuddyBidHeader.Text = "No Buddy Bid";
			} else {
				txtBuddyBid.Text = str;
				lblBuddyBidHeader.Text = "Buddy Bid Choices";
			}
		}

		/// <summary>
		/// set the avoidance bid label.
		/// </summary>
		/// <param name="str"></param>
		private void setupAvoidanceChoiceLabel (string str)
		{
			if (str == null) {

				lblAvoidanceHeader.Text = "No Avoidance Bid";
				lblAvoidanceText.Text = "";
			} else {
				lblAvoidanceHeader.Text = "Avoidance Bid: ";
				lblAvoidanceText.Text = str;
			}
		}

		[Export ("AuthenticationCheck")]
		private void AuthenticationCheck ()
		{
			string userName = KeychainHelpers.GetPasswordForUsername ("user", "WBid.WBidiPad.cwa", false);
			string password = KeychainHelpers.GetPasswordForUsername ("pass", "WBid.WBidiPad.cwa", false);

   //         if (WBidHelper.IsSouthWestWifiOr2wire() == false) 
			//{
				//checking  the internet connection available
				//==================================================================================================================
                if (Reachability.CheckVPSAvailable ())
				{
					isnetavailable = true;
					//  NSNotificationCenter.DefaultCenter.PostNotificationName("reachabilityCheckSuccess", null);
					//checking CWA credential
					//==================================================================================================================

					//this.startProgress();
					Authentication authentication = new Authentication ();
					string authResult = authentication.CheckCredential (userName, password);
					if (authResult.Contains ("ERROR: "))
					{
						WBidLogEvent obj = new WBidLogEvent();
						obj.LogBadPasswordUsage(userName, false, authResult);
						KeychainHelpers.SetPasswordForUsername ("pass", "", "WBid.WBidiPad.cwa", SecAccessible.Always, false);

						InvokeOnMainThread (() => {
							CustomAlertView customAlert = new CustomAlertView();
							UINavigationController nav = new UINavigationController(customAlert);
							nav.NavigationBarHidden = true;
							nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
							customAlert.objQueryView = this;
							customAlert.AlertType = "InvalidCredential";
							this.PresentViewController(nav, true, null);
							//UIAlertController okAlertController = UIAlertController.Create("WBidMax", "Invalid Username or Password", UIAlertControllerStyle.Alert);
							//okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
							//this.PresentViewController(okAlertController, true, null);
							loadingOverlay.RemoveFromSuperview ();

						});
					} else if (authResult.Contains ("Exception")) {
						InvokeOnMainThread (() => {
							WBidLogEvent obgWBidLogEvent = new WBidLogEvent ();
							//obgWBidLogEvent.LogTimeoutBidSubmitDetails (GlobalSettings.SubmitBid, GlobalSettings.TemporaryEmployeeNumber);
							obgWBidLogEvent.LogAllEvents(GlobalSettings.TemporaryEmployeeNumber,"bidSubmitTimeOut",GlobalSettings.SubmitBid.Buddy1,GlobalSettings.SubmitBid.Buddy2, authResult);
							
							showTimeOutAlert( false);
						

						});
					} else {
						_sessionCredentials = authResult;

						InvokeOnMainThread (() => {
							loadingOverlay.updateLoadingText ("Authorization Checking...");
						});

						ClientRequestModel clientRequestModel = new ClientRequestModel ();
						clientRequestModel.Base = GlobalSettings.CurrentBidDetails.Domicile;
						clientRequestModel.BidRound = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
						clientRequestModel.Month = new DateTime (GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).ToString ("MMM").ToUpper ();
						clientRequestModel.Postion = GlobalSettings.CurrentBidDetails.Postion;
						clientRequestModel.OperatingSystem = "iPad OS";
						clientRequestModel.Platform = "iPad";
					clientRequestModel.RequestType = (int)RequestTypes.SubmitBid;

					token = new Guid ();
						clientRequestModel.Token = token;
						clientRequestModel.Version = System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
						clientRequestModel.EmployeeNumber = Convert.ToInt32 (Regex.Match (userName, @"\d+").Value);
						client.GetAuthorizationforMultiPlatformAsync (clientRequestModel);
					}
				} 
                else 
                {
					
					//isnetavailable = false;

					//if (WBidHelper.IsSouthWestWifi ())
					//{
					//	if (GlobalSettings.WbidUserContent.UserInformation.PaidUntilDate >= DateTime.Now)
					//	{
					//		Authentication authentication = new Authentication ();
					//		string authResult = authentication.CheckCredential (userName, password);
					//		if (authResult.Contains ("ERROR: "))
					//		{
					//			WBidLogEvent obj = new WBidLogEvent();
					//			obj.LogBadPasswordUsage(userName, false);
					//			KeychainHelpers.SetPasswordForUsername ("pass", "", "WBid.WBidiPad.cwa", SecAccessible.Always, false);

					//			InvokeOnMainThread (() => {
     //                               UIAlertController okAlertController = UIAlertController.Create("WBidMax", "Invalid Username or Password", UIAlertControllerStyle.Alert);
     //                               okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
     //                               this.PresentViewController(okAlertController, true, null);
     //                               loadingOverlay.RemoveFromSuperview ();

					//			});
					//		} else if (authResult.Contains ("Exception")) {
					//			InvokeOnMainThread (() => {
					//				WBidLogEvent obgWBidLogEvent = new WBidLogEvent ();
					//				//obgWBidLogEvent.LogTimeoutBidSubmitDetails (GlobalSettings.SubmitBid, GlobalSettings.TemporaryEmployeeNumber);
					//				obgWBidLogEvent.LogAllEvents (GlobalSettings.TemporaryEmployeeNumber, "bidSubmitTimeOut", GlobalSettings.SubmitBid.Buddy1, GlobalSettings.SubmitBid.Buddy2);
									
					//				showTimeOutAlert(true);
					//				//this.DismissViewController (true, null);
					//				//loadingOverlay.RemoveFromSuperview ();

					//			});
					//		} else {
					//			_sessionCredentials = authResult;
					//			submitBidAllOperations ();
					//		}
					//	} 
					//	else
					//	{

					//		InvokeOnMainThread (() =>  {
     //                           UIAlertController okAlertController = UIAlertController.Create("WBidMax", "Your subscription is expired and you are on the plane using the free company limited internet connection.\\nYou cannot update your subscription using the limited internet connection.Either pay for a full internet connection or wailt until you get on the ground and have a full internet connection", UIAlertControllerStyle.Alert);
     //                           okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
     //                           this.PresentViewController(okAlertController, true, null);

     //                           this.DismissViewController (true, null);
					//			loadingOverlay.RemoveFromSuperview ();
					//		});
						
					//	}

					//}
					//else 
					//{
						InvokeOnMainThread (() => {

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

                            NSNotificationCenter.DefaultCenter.PostNotificationName ("reachabilityCheckFailed", null);
							this.DismissViewController (true, null);
							loadingOverlay.RemoveFromSuperview ();


						});
					}
				//}
			//}
			//else 
			//{
			//	isnetavailable = false;
			//	//wifi test mode.\
			//	if (GlobalSettings.WbidUserContent.UserInformation.PaidUntilDate >= DateTime.Now) {

			//		Authentication authentication = new Authentication ();
			//		string authResult = authentication.CheckCredential (userName, password);
			//		if (authResult.Contains ("ERROR: ")) {
			//			KeychainHelpers.SetPasswordForUsername ("pass", "", "WBid.WBidiPad.cwa", SecAccessible.Always, false);
			//			WBidLogEvent obj = new WBidLogEvent();
			//			obj.LogBadPasswordUsage(userName, false);
			//			InvokeOnMainThread (() => {
   //                         UIAlertController okAlertController = UIAlertController.Create("WBidMax", "Invalid Username or Password", UIAlertControllerStyle.Alert);
   //                         okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
   //                         this.PresentViewController(okAlertController, true, null);

   //                         loadingOverlay.RemoveFromSuperview ();

			//			});
			//		} else if (authResult.Contains ("Exception")) {
			//			InvokeOnMainThread (() => {
			//				WBidLogEvent obgWBidLogEvent = new WBidLogEvent ();
			//				//obgWBidLogEvent.LogTimeoutBidSubmitDetails (GlobalSettings.SubmitBid, GlobalSettings.TemporaryEmployeeNumber);
			//				obgWBidLogEvent.LogAllEvents (GlobalSettings.TemporaryEmployeeNumber, "bidSubmitTimeOut", GlobalSettings.SubmitBid.Buddy1, GlobalSettings.SubmitBid.Buddy2);
						
			//				showTimeOutAlert(true);
			//				//this.DismissViewController (true, null);
			//				//loadingOverlay.RemoveFromSuperview ();

			//			});
			//		} else {
			//			_sessionCredentials = authResult;

			//			submitBidAllOperations ();
			//		}
			//	} else {
			//		InvokeOnMainThread (() =>  {
   //                     UIAlertController okAlertController = UIAlertController.Create("WBidMax", "Your subscription is expired and you are on the plane using the free company limited internet connection.\\nYou cannot update your subscription using the limited internet connection.Either pay for a full internet connection or wailt until you get on the ground and have a full internet connection", UIAlertControllerStyle.Alert);
   //                     okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
   //                     this.PresentViewController(okAlertController, true, null);

   //                     this.DismissViewController (true, null);
			//			loadingOverlay.RemoveFromSuperview ();
			//		});
			//	}

			//}
		}
		public void dismissView()
		{
			this.DismissViewController(true, null);
			loadingOverlay.RemoveFromSuperview();
		}
		#endregion

		#region Events


		partial void btnShowBidChoicesTapped(Foundation.NSObject sender)
		{


			BidChoicesViewController details = new BidChoicesViewController();
			details.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;



			//if (this.NavigationController == null)
			//{
				
			//	UINavigationController nav = new UINavigationController(details);
			//	nav.ModalPresentationStyle =  UIModalPresentationStyle.FormSheet;
			//	//nav.PushViewController(details, true);
			//	this.PresentViewController(nav, true, null);
			//}
			//else
			//{
                this.NavigationController.PushViewController(details, true);
			//}





	}

		partial void btnSubmitTapped (UIKit.UIButton sender)
		{
			
			if (!RegXHandler.EmployeeNumberValidation (txtSubmitBid.Text)) {
                UIAlertController okAlertController = UIAlertController.Create("Error", "Invalid Employee Number.", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);

                return;
			}
			//if (GlobalSettings.CurrentBidDetails.Postion == "FO")
			//{
			//    if(!RegXHandler.EmployeeNumberValidation(txtAvoidance.Text))
			//    {
			
			//        return;
			//    }
			//}
			//if (GlobalSettings.CurrentBidDetails.Postion == "FA")
			//{
			//    if(!RegXHandler.EmployeeNumberValidation(txtBuddyBid.Text))
			//    {
			
			//        return;
			//    }
			//}

			//Submit button action of query view.
			loginViewController login = new loginViewController ();
			login.isFromSubmit = true;
			login.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
			this.PresentViewController (login, true, () => {
				CommonClass.bidObserver = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("loginDetailsEntered"), loginCredetialsEntered);
			});
		}

		private void loginCredetialsEntered (NSNotification obj)
		{
			if (loadingOverlay == null) {
				loadingOverlay = new LoadingOverlay (this.View.Frame, "Authentication Checking..");
			} else {
				loadingOverlay.updateLoadingText ("Authentication Checking..");
			}
			View.Add (loadingOverlay);
			this.PerformSelector (new ObjCRuntime.Selector ("AuthenticationCheck"), null, 0);
			NSNotificationCenter.DefaultCenter.RemoveObserver (CommonClass.bidObserver);
		}

		partial void btnCancelTapped (UIKit.UIButton sender)
		{
			this.DismissViewController (true, null);
			foreach (UIView view in this.View.Subviews) {

				DisposeClass.DisposeEx(view);
			}
		}
		public class MyPopDelegate : UIPopoverControllerDelegate
		{
			queryViewController _parent;
			public bool CanDismiss;
			public MyPopDelegate(queryViewController parent)
			{
				_parent = parent;
			}

			public override bool ShouldDismiss(UIPopoverController popoverController)
			{
				if (CanDismiss)
				{
					return true;
				}
				else {

					return false;
				}
			}
		}
		#endregion
	}
}

