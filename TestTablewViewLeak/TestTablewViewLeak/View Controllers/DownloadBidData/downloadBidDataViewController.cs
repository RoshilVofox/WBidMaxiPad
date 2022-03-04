using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UIKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Model.SWA;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.SharedLibrary.SWA;
using WBid.WBidiPad.SharedLibrary.Utility;
using WBidDataDownloadAuthorizationService.Model;

namespace TestTablewViewLeak.ViewControllers.DownloadBidData
{
	public partial class downloadBidDataViewController1 : UIViewController
	{

		#region Properties
		public bool _isCompanyServerData { get; set; }
        private List<Vacation> _vacation { get; set; }
		private SeniorityListItem _seniorityListItem { get; set; }
		private List<Absense> _fVVacation { get; set; }
		private string _stateFileName = string.Empty;
		private WBidState wBIdStateContent;
		public int _paperCount = 0;
		public bool _isInSeriority {get; set; }
		#endregion

		#region Controller code
		public downloadBidDataViewController1()
			: base("downloadBidDataViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning();

			// Release any cached data, images, etc that aren't in use.
		}
		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			this.View.Dispose();
		}

		public override void ViewDidLoad()
		{

		}
        #endregion

        #region Public method
        public void DownloadBidData()
		{
			if (!GlobalSettings.isHistorical)
			{

				//Download and save Bid data//
				bool isBidDataAvailable = DownloadAndSaveBidDataFromWBid();

				if (isBidDataAvailable == true)
				{

					WBidHelper.SetCurrentBidInformationfromStateFileName(_stateFileName);

					DownloadWbidFiles();

					GenerateStateFile(_stateFileName);

					SetValuesToStateFile();

					ChecktheUserIsInEBGGroup();

					ShowSeniorityListInformation();
				}

			}
			else
				DownloadHistoricalData();


		}
		#endregion

		#region Private method
		/// <summary>
		/// Download and save WBL,WBP ,Vacation Files, Seniority and Cover letter from Wbid VPS
		/// </summary>
		private bool DownloadAndSaveBidDataFromWBid()
		{
			try
			{
				//IMplement the service
				//Set vacation,senlist and FV vacation from the service
				BidDataFileResponse biddataresponse;
				BidDataRequestDTO bidDetails = new BidDataRequestDTO();
				bidDetails.EmpNum = 21221;
				bidDetails.Base = "ATL";
				bidDetails.Position = "CP";
				bidDetails.Month = 4;
				bidDetails.Year = 2021;
				bidDetails.Round = "M";


				try
				{
					var jsonData = ServiceUtils.JsonSerializer(bidDetails);
					StreamReader dr = ServiceUtils.GetRestData("GetMonthlyBidFiles", jsonData);
					biddataresponse = WBidCollection.ConvertJSonStringToObject<BidDataFileResponse>(dr.ReadToEnd());
				}
				catch (Exception ex)
				{
					return false;
				}

				//Show alert if the bid data is not available
				if (biddataresponse.bidData.Count > 0)
				{

					//Ierate through all Bid data files and save the file
					foreach (var item in biddataresponse.bidData)
					{

						var fileExtension = item.FileName.Split('.')[1].ToString().ToLower();
						switch (fileExtension)
						{

							case "wbl":

								_stateFileName = item.FileName;
								//Decompress the string using LZ compress.
								string linefileJsoncontent = LZStringCSharp.LZString.DecompressFromUTF16(item.FileContent);

								File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + item.FileName + ".WBL", item.FileContent);
								//desrialise the Json
								LineInfo wblLine = WBidCollection.ConvertJSonStringToObject<LineInfo>(linefileJsoncontent);

								GlobalSettings.Lines = new ObservableCollection<Line>(wblLine.Lines.Values);

								break;
							case "wbp":

								//Decompress the string using LZ compress.
								string tripfileJsoncontent = LZStringCSharp.LZString.DecompressFromUTF16(item.FileContent);

								File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + item.FileName + ".WBP", item.FileContent);
								//desrialise the Json
								Dictionary<string, Trip> wbpLine = WBidCollection.ConvertJSonStringToObject<Dictionary<string, Trip>>(tripfileJsoncontent);

								GlobalSettings.Trip = new ObservableCollection<Trip>(wbpLine.Values);
								break;
							case "json":

								break;
							case "wbv":
								//Decompress the string using LZ compress.
								string vacationlinefileJsoncontent = LZStringCSharp.LZString.DecompressFromUTF16(item.FileContent);

								File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + item.FileName + ".WBV", item.FileContent);
								break;

							default:
								File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + item.FileName, item.FileContent);
								break;


						}
					}

					////SeniorityList
					_seniorityListItem.SeniorityNumber = biddataresponse.DomcileSeniority;
					_seniorityListItem.TotalCount = biddataresponse.TotalSenliorityMember;
					if (biddataresponse.ISEBGUser)
						_seniorityListItem.EBgType = "Y";
					_paperCount = biddataresponse.paperCount;
					_isInSeriority = biddataresponse.IsSeniorityExist;

					////Vacation
					if (biddataresponse.Vacation.Count > 0)
						_vacation = biddataresponse.Vacation;

					////FV Vacation
					if (biddataresponse.FVVacation.Count > 0)
						_fVVacation = GlobalSettings.FVVacation;

					//Cover Letter
					if (GlobalSettings.WBidINIContent.MiscellaneousTab.Coverletter)
					{
						BidDataFiles coverLetter = biddataresponse.bidData.FirstOrDefault(x => (x.FileName.Contains("C.TXT") || x.FileName.Contains("CR.TXT")) && !x.IsError);
						if (coverLetter != null)
							GlobalSettings.IsCoverletterShowFileName = coverLetter.FileName;
					}



					return true;
				}
                else {
					//           {
					//InvokeOnMainThread(() =>
					//{
					//	AlertController = UIAlertController.Create("Data Transfer Failed", "Error", UIAlertControllerStyle.Alert);
					//	AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) => {
					//		DismissCurrentView();
					//	}));

					//	this.PresentViewController(AlertController, true, null);

					//});

					//InvokeOnMainThread(() =>
					//{
					//	AlertController = UIAlertController.Create("Error", "The internet connection has been lost.  Try again later.", UIAlertControllerStyle.Alert);
					//	AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) => {
					//		DismissCurrentView();
					//	}));
					//	this.PresentViewController(AlertController, true, null);
					//});

					//if (zipFile.Message.Contains("BIDINFO DATA NOT AVAILABLE"))
					//{

					//	errorMessage += "The Requested data doesnot exist on  the SWA servers . make sure proper month is selected and you are within the normal timeframe for the request\r\n";
					//	errorMessage += zipFile.Message.Replace("ERROR: ", " ");

					//}
					//else
					//{
					//	errorMessage += zipFile.Message.Replace("ERROR: ", " ");

					//}


					//InvokeOnMainThread(() =>
					//{
					//	AlertController = UIAlertController.Create("Data Transfer Failed", errorMessage, UIAlertControllerStyle.Alert);
					//	AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) => {
					//		DismissCurrentView();
					//	}));

					//	this.PresentViewController(AlertController, true, null);
					//});


					return false;
                }
			}
			catch(Exception ex)
            {
				return false;
            }
		}
		/// <summary>
		/// Download Wbidupdate.dat, News letter, flight data
		/// </summary>
		private void DownloadWbidFiles()
		{
			int previousnewsverion = 0;
			if (GlobalSettings.WBidINIContent != null && GlobalSettings.WBidINIContent.Updates != null)
			{
				previousnewsverion = GlobalSettings.WBidINIContent.Updates.News;
			}
			try
			{
				List<string> lstWBidFiles = new List<string>() { "WBUPDATE.DAT", "news.pdf", "trips.ttp" };
				
				foreach (var bidFile in lstWBidFiles)
				{

					var success = DownloadBid.DownloadWBidFile(WBidHelper.GetAppDataPath(), bidFile);
				}

				if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Round == "M")
				{
					DownloadBid.DownloadWBidFile(WBidHelper.GetAppDataPath(), "falistwb4.dat");
				}

			}
			catch (Exception ex)
			{
				throw ex;
			}
			
		}
		/// <summary>
		/// Generate state file if the file is not available
		/// </summary>
		private void GenerateStateFile(string StateFileName)
		{
			//Read the intial state file value from DWC file and create state file
			if (!File.Exists(WBidHelper.GetAppDataPath() + "/" + StateFileName + ".WBS"))
			{
				try
				{
					WBidIntialState wbidintialState = null;
					try
					{
						wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
					}
					catch (Exception)
					{
						WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
						try
						{
							wbidintialState = WBidCollection.CreateDWCFile(GlobalSettings.DwcVersion);
						}
						catch (Exception)
						{

							wbidintialState = WBidCollection.CreateDWCFile(GlobalSettings.DwcVersion);
							XmlHelper.SerializeToXml(wbidintialState, WBidHelper.GetWBidDWCFilePath());
							obgWBidLogEvent.LogAllEvents(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "dwcRecreate", "0", "0", "");

						}
						XmlHelper.SerializeToXml(wbidintialState, WBidHelper.GetWBidDWCFilePath());
						obgWBidLogEvent.LogAllEvents(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "dwcRecreate", "0", "0", "");

					}
					GlobalSettings.WBidStateCollection = WBidCollection.CreateStateFile(WBidHelper.GetAppDataPath() + "/" + StateFileName + ".WBS", GlobalSettings.Lines.Count, GlobalSettings.Lines.First().LineNum, wbidintialState);
					if (GlobalSettings.isHistorical)
					{
						GlobalSettings.WBidStateCollection.DataSource = "HistoricalData";
					}
					else
						GlobalSettings.WBidStateCollection.DataSource = (_isCompanyServerData) ? "Original" : "Mock";
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
			else
			{
				try
				{
					//Read the state file object and store it to global settings.
					GlobalSettings.WBidStateCollection = XmlHelper.ReadStateFile(WBidHelper.GetAppDataPath() + "/" + StateFileName + ".WBS");
				}
				catch (Exception)
				{

					//Recreate state file
					//--------------------------------------------------------------------------------
					WBidIntialState wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
					GlobalSettings.WBidStateCollection = WBidCollection.CreateStateFile(WBidHelper.GetAppDataPath() + "/" + StateFileName + ".WBS", GlobalSettings.Lines.Count, GlobalSettings.Lines.First().LineNum, wbidintialState);
					WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
					obgWBidLogEvent.LogAllEvents(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "dwcRecreate", "0", "0", "");
					if (GlobalSettings.isHistorical)
					{
						GlobalSettings.WBidStateCollection.DataSource = "HistoricalData";
					}
					else
						GlobalSettings.WBidStateCollection.DataSource = (_isCompanyServerData) ? "Original" : "Mock";
				}

				if (GlobalSettings.isHistorical)
				{
					GlobalSettings.WBidStateCollection.DataSource = "HistoricalData";
				}
				else if (GlobalSettings.WBidStateCollection.DataSource == "Original" && _isCompanyServerData == false)
				{
					GlobalSettings.WBidStateCollection.DataSource = "Mock";

				}
				else if (GlobalSettings.WBidStateCollection.DataSource == "Mock" && _isCompanyServerData == true)
				{
					GlobalSettings.WBidStateCollection.DataSource = "Original";
				}

			}
			wBIdStateContent= GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		}
		/// <summary>
		/// Set VA,FC,CFV vacation details to the state file. We are getting this information from VPS.
		/// </summary>
		private void SetValuesToStateFile()
		{
			GlobalSettings.WBidStateCollection.Vacation = _vacation;
			GlobalSettings.WBidStateCollection.SeniorityListItem = _seniorityListItem;
			GlobalSettings.WBidStateCollection.FVVacation = _fVVacation;

			//Set Menu bar button state
			//wBIdStateContent.MenuBarButtonState.is
		}
	
		private void ChecktheUserIsInEBGGroup()
		{
			if (GlobalSettings.CurrentBidDetails.Postion != "FA")
			{
				//After the Seniority LIst is parsed, if the USER is NOT in the EBG, we should filter/ constrain for ETOPS.However, some users may want to look at the ETOPS bid data, so in the Alert we give them the chance to "Turn OFF"
				bool isEBG= (GlobalSettings.WBidStateCollection.SeniorityListItem != null && GlobalSettings.WBidStateCollection.SeniorityListItem.EBgType == "Y") ? true : false;
				if (isEBG == false)
				{
					wBIdStateContent.Constraints.ETOPS = true;
					wBIdStateContent.Constraints.ReserveETOPS = true;
				}
			}
			 
		}
		/// <summary>
		/// Alert to user when user is in seniority list or not. Also Display the paper bid information ///Differnt alert for Pilots and FA
		/// </summary>
		private void ShowSeniorityListInformation()
        {
			
			string message = string.Empty;
			if (GlobalSettings.CurrentBidDetails.Postion == "FA" || ((GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO") && GlobalSettings.CurrentBidDetails.Round == "S"))
			{
                
                if (_isInSeriority)
                {
                    message = "WBidMax found you in the Seniority List !! ";
                }
                else
                {
                    message = "WBidMax DID NOT find you in the Seniority List." +
                                                "\nYou may want to check your assigned Domicile for next month." +
                                                "\n\nDO NOT BID THESE LINES!!!!!";
                }
            }
			else
			{
				
				if (_isInSeriority)
				{
					int actualSeniority = _seniorityListItem.SeniorityNumber - _paperCount;
				
					message = "WBidMax found you in the Seniority List !! . You are number " + _seniorityListItem.SeniorityNumber + " out of " + _seniorityListItem.TotalCount + "\n\n There are " + _paperCount + " paper bids and ExTO/ETO above you, making you " + actualSeniority + " on the bid list";
				}
				else
				{
					
					message = "WBidMax DID NOT find you in the Seniority List." +
												"\nYou may want to check your assigned Domicile for next month." +
												"\n\nDO NOT BID THESE LINES!!!!!";
				}
			}
		}

		/// <summary>
        /// Download History Data from SWA
        /// </summary>
		private void DownloadHistoricalData()
		{
			if (GlobalSettings.DownloadBidDetails.Round != "D")
			{
				//bidDetails.FileName=
				WBidDataDownloadAuthorizationService.Model.BidDetails bidDetails = new WBidDataDownloadAuthorizationService.Model.BidDetails();
				bidDetails.Month = GlobalSettings.DownloadBidDetails.Month;
				bidDetails.Year = GlobalSettings.DownloadBidDetails.Year;
				bidDetails.Round = GlobalSettings.DownloadBidDetails.Round == "D" ? 1 : 2;
				bidDetails.Domicile = GlobalSettings.DownloadBidDetails.Domicile;
				bidDetails.Position = GlobalSettings.DownloadBidDetails.Postion;
				bidDetails.FileName = bidDetails.Domicile + bidDetails.Position + "N" + ".TXT";
				var jsonData = ServiceUtils.JsonSerializer(bidDetails);
				StreamReader dr = ServiceUtils.GetRestData("DownloadHistoricalBidLineAll", jsonData);
				HistoricalFileInfo historicalFileInfo = WBidCollection.ConvertJSonStringToObject<HistoricalFileInfo>(dr.ReadToEnd());
				//sWAFileInfo = new DownloadedFileInfo();
				//sWAFileInfo.byteArray = Convert.FromBase64String(historicalFileInfo.DataString);
				//sWAFileInfo.FileName = historicalFileInfo.Title;

				////  sWAFileInfo.IsError = (historicalFileInfo.Data == null) ? true : false;
				//lstDownloadedFiles.Add(sWAFileInfo);
			}
		}
		#endregion

	}
}

