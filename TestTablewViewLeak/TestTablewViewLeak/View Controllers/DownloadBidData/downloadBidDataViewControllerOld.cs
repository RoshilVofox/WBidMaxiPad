#region NameSpace
using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBidDataDownloadAuthorizationService.Model;
using WBid.WBidiPad.iOS.Utility;
using System.ServiceModel;
using WBid.WBidiPad.SharedLibrary.SWA;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using WBid.WBidiPad.Model.SWA;
using iOSPasswordStorage;
using Security;
using System.Linq;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.SharedLibrary.Parser;
using System.IO;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using WBid.WBidiPad.PortableLibrary.Parser;
using WBid.WBidiPad.Core;
using System.Collections.ObjectModel;
using WBid.WBidiPad.SharedLibrary.Serialization;
using WBid.WBidiPad.PortableLibrary;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using WBid.WBidiPad.SharedLibrary.Utility;
using VacationCorrection;
//using MiniZip.ZipArchive;
using System.IO.Compression;
using WBid.WBidiPad.SharedLibrary;

using SystemConfiguration;
using WBid.WBidiPad.Core.Enum;
using TestTablewViewLeak.Utility;


#endregion

namespace WBid.WBidiPad.iOS
{
	public partial class downloadBidDataViewController : UIViewController
	{
		#region Properties & Variables

		public bool _isCompanyServerData { get; set; }
		private List<Vacation> _vacation { get; set; }
		private SeniorityListItem _seniorityListItem = new SeniorityListItem();
		private List<Absense> _fVVacation { get; set; }
		private string _stateFileName = string.Empty;
		private WBidState wBIdStateContent;
		public int _paperCount = 0;
		public bool _isInSeriority { get; set; }
		BidDataFileResponse biddataresponse;
		BidDataFiles vacationlinesObject;
		bool isVacationFileDownloaded = false;
		public Dictionary<string, TripMultiVacData> VacationData { get; set; }
		public UIPopoverController popoverController;
		public MyPopDelegate objPopDelegate;
		private bool _waitCompanyVADialog;
		private static ObservableCollection<Trip> _trip;
		public static ObservableCollection<Trip> Trip
		{
			get
			{
				return _trip ?? (_trip = new ObservableCollection<Trip>());
			}
			set
			{
				_trip = value;
			}
		}

		WBidDataDwonloadAuthServiceClient client;

		private Guid token;
		

		private string _sessionCredentials = string.Empty;

		public DownloadInfo _downloadFileDetails;

		private DownloadBid _downloadBidObject = new DownloadBid();

		//Hold the totla number of seniority list item and domcile seniority
		

		private List<SeniorityListMember> seniorityListMembers;

		List<NSObject> arrObserver = new List<NSObject>();
        UIAlertController AlertController;
        Dictionary<string, Trip> trips = null;
		Dictionary<string, Line> lines = null;


		/// <summary>
		/// create single instance of TripTtpParser class
		/// </summary>
		private TripTtpParser _tripTtpParser;
		public TripTtpParser TripTtpParser
		{
			get
			{
				return _tripTtpParser ?? (_tripTtpParser = new TripTtpParser());
			}
		}

		private CalculateTripProperties _calculateTripProperties;
		public CalculateTripProperties CalculateTripProperties
		{
			get
			{
				return _calculateTripProperties ?? (_calculateTripProperties = new CalculateTripProperties());
			}
		}


		private CalculateLineProperties _calculateLineProperties;
		public CalculateLineProperties CalculateLineProperties
		{
			get
			{
				return _calculateLineProperties ?? (_calculateLineProperties = new CalculateLineProperties());
			}
		}

		#endregion


		public bool IsMissingTripFailed { get; set; }

		public downloadBidDataViewController()
			: base("downloadBidDataViewController", null)
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
		
			this.View.Dispose ();

		}

		public override void ViewDidLoad()
		{
			Console.WriteLine("Download view init");
			int SystemVersion = Convert.ToInt16(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]);

			if (SystemVersion > 12)
			{
                ModalInPresentation = true;
			}
			base.ViewDidLoad();
			GlobalSettings.MenuBarButtonStatus = new MenuBarButtonStatus();
			GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
			GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = false;
			GlobalSettings.MenuBarButtonStatus.IsEOM = false;
			GlobalSettings.MenuBarButtonStatus.IsOverlap = false;
			GlobalSettings.MenuBarButtonStatus.IsMIL = false;
			IsMissingTripFailed = false;

			lblTitle.Text = SetTitle();


			BasicHttpBinding binding = ServiceUtils.CreateBasicHttp();
			client = new WBidDataDwonloadAuthServiceClient(binding, ServiceUtils.EndPoint);
			client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 30);
			client.GetAuthorizationforMultiPlatformCompleted += client_GetAuthorizationforMultiPlatformCompleted;
			client.UpdateUserBidBaseAndSeatCompleted += client_UpdateUserbaseAndPosotionCompleted;
			//client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 30);
			client.LogTimeOutDetailsCompleted += client_LogTimeOutDetailsCompleted;
			// Perform any additional setup after loading the view, typically from a nib.
			this.prgrsVw.SetProgress(0.0f, false);
			this.observeNotifications();

			_isCompanyServerData = GlobalSettings.WBidINIContent.Data.IsCompanyData;



			InvokeInBackground(() =>
				{
					this.initiateDownloadProcess();
				});

		}

		void client_LogTimeOutDetailsCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{

		}

		public override void ViewDidAppear(bool animated)
		{
			
		}

		#region Public method
		public void DownloadBidData()
		{
			try
			{
				NSNotificationCenter.DefaultCenter.PostNotificationName("authCheckSuccess", null);
				if (!GlobalSettings.isHistorical)
				{

					//Download and save Bid data//
					bool isBidDataAvailable = DownloadAndSaveBidDataFromWBid(false,false);

					if (isBidDataAvailable == true)
					{

						

						WBidHelper.SetCurrentBidInformationfromStateFileName(_stateFileName);


						GenerateStateFile(_stateFileName);

						

						SetValuesToStateFile();
											
						ReadWbUpdateAndUpdateINIFile();
					
						
						WBidHelper.GenerateDynamicOverNightCitiesList();
						GlobalSettings.AllCitiesInBid = GlobalSettings.WBidINIContent.Cities.Select(y => y.Name).ToList(); var linePairing = GlobalSettings.Lines.SelectMany(y => y.Pairings);

						SetDRPColoring();

						WBidHelper.RecalculateAMPMAndWekProperties(false);

						ChecktheUserIsInEBGGroup();

						NSNotificationCenter.DefaultCenter.PostNotificationName("saveDataSuccess", null);

						StateManagement statemanagement = new StateManagement();
						statemanagement.ApplyCSW(wBIdStateContent);

						ShowSeniorityListInformation();

						FlightDataDownload();

						CalculateDailyCommutes();
					}
					else
					{
						showAlertAndSimissView("Data Transfer Failed", "Error");
						//DismisscCurrentView();
					}
				}
				else
				{
					//hisotrical data download process
					bool isBidDataAvailable = DownloadAndSaveBidDataFromWBid(true,false);

					if (isBidDataAvailable == true)
					{

						WBidHelper.SetCurrentBidInformationfromStateFileName(_stateFileName);
						GenerateStateFile(_stateFileName);

						SetValuesToStateFile();
						NSNotificationCenter.DefaultCenter.PostNotificationName("saveDataSuccess", null);
						//ShowSeniorityListInformation();
						//NSNotificationCenter.DefaultCenter.PostNotificationName("getDataFilesSuccess", null);

						DismisscAndRedirectToLineView();
					}
					else
					{
						showAlertAndSimissView("Data Transfer Failed", "Error");
						//DismisscCurrentView();
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			


		}
		#endregion

		#region Private method

		private void DismisscCurrentView()
		{
			InvokeOnMainThread(() =>
			{
				RemoveAllObserver();
				this.DismissViewController(false, null);
				
			});
		}
		private void DismisscAndRedirectToLineView()
		{
			InvokeOnMainThread(() =>
			{
				RemoveAllObserver();
				this.DismissViewController(false, null);
				NSNotificationCenter.DefaultCenter.PostNotificationName("NavigateToLineView", null);
			});
		}
		private void RemoveAllObserver()
		{

			foreach (NSObject obj in arrObserver)
			{
				NSNotificationCenter.DefaultCenter.RemoveObserver(obj);
			}

			foreach (NSObject obj in arrObserver)
			{
				NSNotificationCenter.DefaultCenter.RemoveObserver(obj);
			}
		}
		private void CalculateDailyCommutes()
		{
			if (wBIdStateContent.CxWtState.CLAuto.Cx || wBIdStateContent.CxWtState.CLAuto.Wt || (wBIdStateContent.SortDetails.BlokSort.Contains("33") || wBIdStateContent.SortDetails.BlokSort.Contains("34") || wBIdStateContent.SortDetails.BlokSort.Contains("35")))
			{
				if (GlobalSettings.FlightRouteDetails == null)
				{
					NetworkData networkplandata = new NetworkData();
					networkplandata.ReadFlightRoutes();
				}
				CommuteCalculations objCommuteCalculations = new CommuteCalculations();
				objCommuteCalculations.CalculateDailyCommutableTimes(wBIdStateContent.Constraints.CLAuto, GlobalSettings.FlightRouteDetails);
			}
		}
		private void SetDRPColoring()
		{
			if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
			{
				RecalcalculateLineProperties objrecalculate = new RecalcalculateLineProperties();
				objrecalculate.CalculateDropTemplateForBidLines(GlobalSettings.Lines);
			}
		}
		private void FlightDataDownload()
		{
			try
			{
				string serverPath = GlobalSettings.WBidDownloadFileUrl + "FlightData.zip";
				string zipLocalFile = Path.Combine(WBidHelper.GetAppDataPath(), "FlightData.zip");
				string networkDataPath = WBidHelper.GetAppDataPath() + "/" + "FlightData.NDA";

				CustomWebClient wcClient = new CustomWebClient();
				//Downloading networkdat file
				wcClient.DownloadFile(serverPath, zipLocalFile);

				this.setButtonState(this.btnVacationData, true);


				string target = Path.Combine(WBidHelper.GetAppDataPath(), WBidHelper.GetAppDataPath() + "/");

				if (File.Exists(networkDataPath))
				{
					File.Delete(networkDataPath);
				}

				if (File.Exists(zipLocalFile))
				{
					ZipFile.ExtractToDirectory(zipLocalFile, target);

					GlobalSettings.WBidINIContent.LocalFlightDataVersion = GlobalSettings.ServerFlightDataVersion;
					//Save Ini file

					WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
				}

			}
			catch (Exception ex)
			{
				GlobalSettings.ExtraErrorInfo += "Flight data download error : \n<br>" + ex.StackTrace + "\n<br>";
				throw ex;
			}
		}
		/// <summary>
		/// Read the WBPUpdate.Dat and Update the INI file.
		/// </summary>
		private void ReadWbUpdateAndUpdateINIFile()
		{
			if (!File.Exists(WBidHelper.WBidUpdateFilePath))
				return;


			int previousnewsverion = GlobalSettings.WBidINIContent.Updates.News;
			WBidUpdate WBidUpdate = WBidHelper.ReadValuesfromWBUpdateFile(WBidHelper.WBidUpdateFilePath);


			if (WBidUpdate != null)
			{
				bool isUpdateFound = WBidCollection.UpdateINIFile(WBidUpdate);
				//Save the INI file
				if (isUpdateFound)
				{
					WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
				}

				if (GlobalSettings.WBidINIContent.Updates.News > previousnewsverion)
				{
					GlobalSettings.IsNewsShow = true;
				}
			}

		}
		/// <summary>
		/// Download and save WBL,WBP ,Vacation Files, Seniority and Cover letter from Wbid VPS, For historic bid data, it will give only WBL and WBP file
		/// </summary>
		public bool DownloadAndSaveBidDataFromWBid(bool isHistorical, bool isFromMultipleBidDownload)
		{
			try
			{
				vacationlinesObject = new BidDataFiles();
				//IMplement the service
				//Set vacation,senlist and FV vacation from the service
				biddataresponse = new BidDataFileResponse();
				BidDataRequestDTO bidDetails = new BidDataRequestDTO();
				bidDetails.EmpNum = GlobalSettings.IsDifferentUser ? Convert.ToInt32(GlobalSettings.ModifiedEmployeeNumber) : Convert.ToInt32(Regex.Match(_downloadFileDetails.UserId, @"\d+").Value);
				bidDetails.Base = GlobalSettings.DownloadBidDetails.Domicile;
				bidDetails.Position = GlobalSettings.DownloadBidDetails.Postion;
				bidDetails.Month = GlobalSettings.DownloadBidDetails.Month;
				bidDetails.Year = GlobalSettings.DownloadBidDetails.Year;
				bidDetails.Round = GlobalSettings.DownloadBidDetails.Round == "D" ? "M" : "S";
				bidDetails.IsHistoricBid = isHistorical;
				bidDetails.IsQATest = GlobalSettings.buddyBidTest;
				try
				{
					var jsonData = ServiceUtils.JsonSerializer(bidDetails);
					StreamReader dr = ServiceUtils.GetRestData("GetMonthlyBidFiles", jsonData);
					biddataresponse = WBidCollection.ConvertJSonStringToObject<BidDataFileResponse>(dr.ReadToEnd());


					if (GlobalSettings.WbidUserContent.AppDataBidFiles == null)
						GlobalSettings.WbidUserContent.AppDataBidFiles = new List<AppDataBidFileNames>();
					AppDataBidFileNames appDataBidFileNames = GlobalSettings.WbidUserContent.AppDataBidFiles.FirstOrDefault(x => x.Domicile == bidDetails.Base && x.Position == bidDetails.Position && x.Round == bidDetails.Round && x.Month == bidDetails.Month && x.Year == bidDetails.Year);
					if (appDataBidFileNames == null)
					{
						//it means file is not already downlaoded. Then we need to add app bid data file names into the users file
						GlobalSettings.WbidUserContent.AppDataBidFiles.Add(new AppDataBidFileNames
						{
							lstBidFileNames = biddataresponse.BidFileNames,
							Domicile = bidDetails.Base,
							Month = bidDetails.Month,
							Position = bidDetails.Position,
							Round = bidDetails.Round,
							Year = bidDetails.Year
						});
					}
					else
					{
						if (appDataBidFileNames.lstBidFileNames.Count() <= biddataresponse.BidFileNames.Count())
						{
							appDataBidFileNames.lstBidFileNames = biddataresponse.BidFileNames;
						}
					}
					if (!isFromMultipleBidDownload)
					{
						UpdateUserDomicileAndPosition();
						WBidHelper.SaveUserFile(GlobalSettings.WbidUserContent, WBidHelper.WBidUserFilePath);
					}




				}
				catch (Exception ex)
				{
					WBidHelper.AddDetailsToMailCrashLogs(ex);
					return false;
				}
				if (biddataresponse.Status == true)
				{


					bool status = false;
					if (!isFromMultipleBidDownload)
					{
						//Downlaod news.pdf, wbidupdate.dat etc
						DownloadWbidFiles();
						NSNotificationCenter.DefaultCenter.PostNotificationName("getDataFilesSuccess", null);
					}
					//Show alert if the bid data is not available
					if (biddataresponse.bidData.Count > 0)
					{
						List<Line> toplockllist = new List<Line>();
						List<Line> bottomlockllist = new List<Line>();
						vacationlinesObject = biddataresponse.bidData.FirstOrDefault((x => x.FileName.Contains(".WBV")));
						if (vacationlinesObject != null && vacationlinesObject.IsError == false && !isFromMultipleBidDownload)
						{
							isVacationFileDownloaded = true;
							//vacation exists.
						}
						//Ierate through all Bid data files and save the file
						foreach (var item in biddataresponse.bidData)
						{
							if (!item.IsError)
							{
								var fileExtension = item.FileName.Split('.')[1].ToString().ToLower();

								switch (fileExtension)
								{

									case "wbl":

										_stateFileName = item.FileName.Substring(0, 10) + "737";
										//Decompress the string using LZ compress.
										string linefileJsoncontent = LZStringCSharp.LZString.DecompressFromUTF16(item.FileContent);

										File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + item.FileName, item.FileContent);
										if (isVacationFileDownloaded == false && !isFromMultipleBidDownload)
										{
											//desrialise the Json
											LineInfo wblLine = WBidCollection.ConvertJSonStringToObject<LineInfo>(linefileJsoncontent);
											if (GlobalSettings.Lines != null)
											{
												toplockllist = GlobalSettings.Lines.Where(x => x.TopLock).ToList();
												bottomlockllist = GlobalSettings.Lines.Where(x => x.BotLock).ToList();
											}
											GlobalSettings.Lines = new ObservableCollection<Line>(wblLine.Lines.Values);
											WBidHelper.SetTopLoclAndBottomLock(toplockllist, bottomlockllist);

										}


										break;
									case "wbp":

										//Decompress the string using LZ compress.
										string tripfileJsoncontent = LZStringCSharp.LZString.DecompressFromUTF16(item.FileContent);

										File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + item.FileName, item.FileContent);
										if (!isFromMultipleBidDownload)
										{
											//desrialise the Json
											Dictionary<string, Trip> wbpLine = WBidCollection.ConvertJSonStringToObject<Dictionary<string, Trip>>(tripfileJsoncontent);

											GlobalSettings.Trip = new ObservableCollection<Trip>(wbpLine.Values);
										}

										break;
									case "json":

										break;
									case "wbv":
										//Decompress the string using LZ compress.
										string vacationlinefileJsoncontent = LZStringCSharp.LZString.DecompressFromUTF16(item.FileContent);

										File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + item.FileName, item.FileContent);

										if (isVacationFileDownloaded)
										{
											//desrialise the Json
											LineInfo wblLine = WBidCollection.ConvertJSonStringToObject<LineInfo>(vacationlinefileJsoncontent);
											if (GlobalSettings.Lines != null)
											{
												toplockllist = GlobalSettings.Lines.Where(x => x.TopLock).ToList();
												bottomlockllist = GlobalSettings.Lines.Where(x => x.BotLock).ToList();
											}
											GlobalSettings.Lines = new ObservableCollection<Line>(wblLine.Lines.Values);
											WBidHelper.SetTopLoclAndBottomLock(toplockllist, bottomlockllist);
											//WBidHelper.RecalculateAMPMAndWekProperties(false);
										}
										break;

									default:
										if (!isHistorical && !item.IsError)
										{
											File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + item.FileName, item.FileContent);
										}
										break;

								}
							}



						}
						//WBidHelper.RecalculateAMPMAndWekProperties(false);
						//Cover Letter
						if (GlobalSettings.WBidINIContent.MiscellaneousTab.Coverletter && !isHistorical)
						{
							BidDataFiles coverLetter = biddataresponse.bidData.FirstOrDefault(x => (x.FileName.Contains("C.TXT") || x.FileName.Contains("CR.TXT")) && !x.IsError);
							if (coverLetter != null)
								GlobalSettings.IsCoverletterShowFileName = coverLetter.FileName;
						}
						status = true;
					}
					if (!isFromMultipleBidDownload)
					{
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
							_fVVacation = biddataresponse.FVVacation;
					}
					//if ((biddataresponse.Vacation.Count > 0 || biddataresponse.FVVacation.Count > 0) && (vacationlinesObject != null && vacationlinesObject.IsError==false))
					//{
					//	//isVacationFileDownloaded
					//}

					//	GlobalSettings.IsVacationCorrection = false;
					//	InvokeOnMainThread(() =>
					//	{
					//		AlertController = UIAlertController.Create("WBidMax", biddataresponse.Message, UIAlertControllerStyle.Alert);
					//		AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
					//		this.PresentViewController(AlertController, true, null);
					//	});
					//	//showAlertView(biddataresponse.Message, "Error!!");

					//               }

					return status;
				}
				else
				{

					showAlertView(biddataresponse.Message, "Error!!");

					//Show alert. Set the alet from server itself.
					//Show the server message from response.
					return false;
				}

			}
			catch (Exception ex)
			{
				WBidHelper.AddDetailsToMailCrashLogs(ex);
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

				if (GlobalSettings.DownloadBidDetails.Postion == "FA" && GlobalSettings.DownloadBidDetails.Round == "D")
				{
					DownloadBid.DownloadWBidFile(WBidHelper.GetAppDataPath(), "falistwb4.dat");
				}

			}
			catch (Exception ex)
			{
				WBidHelper.AddDetailsToMailCrashLogs(ex);
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

			//GlobalSettings.WBidStateCollection.DownlaodedbidFiles = downloadedBidList;
			wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			ManageStateFile();
			

		}
		private void ManageStateFile()
		{
			if (wBIdStateContent.CxWtState.MixedHardReserveTrip == null)
				wBIdStateContent.CxWtState.MixedHardReserveTrip = new StateStatus { Cx = false, Wt = false };
			if (wBIdStateContent.CxWtState.StartDay == null)
				wBIdStateContent.CxWtState.StartDay = new StateStatus { Cx = false, Wt = false };
			if (wBIdStateContent.CxWtState.ReportRelease == null)
				wBIdStateContent.CxWtState.ReportRelease = new StateStatus { Cx = false, Wt = false };


		}
		/// <summary>
		/// Set VA,FC,CFV vacation details to the state file. We are getting this information from VPS.
		/// </summary>
		private void SetValuesToStateFile()
		{
			try
			{
				GlobalSettings.WBidStateCollection.Vacation = _vacation;
				GlobalSettings.WBidStateCollection.SeniorityListItem = _seniorityListItem;

				GlobalSettings.WBidStateCollection.FVVacation = _fVVacation;
				GlobalSettings.FVVacation = _fVVacation;
				//GlobalSettings.SeniorityListMember = _seniorityListItem;
				GlobalSettings.IsVacationCorrection = false;

				if (!GlobalSettings.isHistorical)
				{
					if (GlobalSettings.WBidStateCollection.Vacation != null && GlobalSettings.WBidStateCollection.Vacation.Count > 0)
					{
						GlobalSettings.IsVacationCorrection = (GlobalSettings.CurrentBidDetails.Round == "M" || (GlobalSettings.CurrentBidDetails.Round == "S" && GlobalSettings.CurrentBidDetails.Postion != "FA"));
					}

					GlobalSettings.IsFVVacation = (GlobalSettings.FVVacation != null && GlobalSettings.FVVacation.Count > 0 && (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO"));

					//In case if the vacation file is not downloaded , we dont need to ON VAC and DRP button. We need to show alert to the user in this case
					if (GlobalSettings.IsVacationCorrection && isVacationFileDownloaded)
					{
						GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = true;
						GlobalSettings.MenuBarButtonStatus.IsVacationDrop = true;
						GlobalSettings.iSNeedToShowMonthtoMonthAlert = true;

						wBIdStateContent.IsVacationOverlapOverlapCorrection = true;
					}
					if (GlobalSettings.IsFVVacation)
					{
						GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = true;
					}
					wBIdStateContent.MenuBarButtonState.IsVacationCorrection = GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.IsFVVacation;
					wBIdStateContent.MenuBarButtonState.IsVacationDrop = GlobalSettings.MenuBarButtonStatus.IsVacationDrop;
				}
				WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);
			}
			catch (Exception ex)
			{
				WBidHelper.AddDetailsToMailCrashLogs(ex);
				throw ex;
			}
			//Set Menu bar button state
			//wBIdStateContent.MenuBarButtonState.is
		}

		private void ChecktheUserIsInEBGGroup()
		{
			if (GlobalSettings.CurrentBidDetails.Postion != "FA")
			{
				//After the Seniority LIst is parsed, if the USER is NOT in the EBG, we should filter/ constrain for ETOPS.However, some users may want to look at the ETOPS bid data, so in the Alert we give them the chance to "Turn OFF"
				bool isEBG = (GlobalSettings.WBidStateCollection.SeniorityListItem != null && GlobalSettings.WBidStateCollection.SeniorityListItem.EBgType == "Y") ? true : false;
				if (isEBG == false)
				{
					wBIdStateContent.Constraints.ETOPS = true;
					wBIdStateContent.Constraints.ReserveETOPS = true;
					WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);
				}
			}

		}
		/// <summary>
		/// Alert to user when user is in seniority list or not. Also Display the paper bid information ///Differnt alert for Pilots and FA
		/// </summary>
		private void ShowSeniorityListInformation()
		{
			try
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
				string seniorityFileName = string.Empty;
				if (GlobalSettings.CurrentBidDetails.Round == "M")
				{
					seniorityFileName = WBidHelper.GetAppDataPath() + "/" + GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "S";
				}
				else if (GlobalSettings.CurrentBidDetails.Round == "S" && GlobalSettings.CurrentBidDetails.Postion != "FA")
				{
					seniorityFileName = WBidHelper.GetAppDataPath() + "/" + GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "R";
				}
				else if (GlobalSettings.CurrentBidDetails.Round == "S" && GlobalSettings.CurrentBidDetails.Postion == "FA")
				{
					seniorityFileName = WBidHelper.GetAppDataPath() + "/" + GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "SR";
				}
				var sencheckempnum = (GlobalSettings.IsDifferentUser) ? GlobalSettings.ModifiedEmployeeNumber : _downloadFileDetails.UserId;
				int intEmpNum = Convert.ToInt32(Regex.Match(sencheckempnum, @"\d+").Value);
				Convert.ToInt32(Regex.Match(_downloadFileDetails.UserId, @"\d+").Value);
				InvokeOnMainThread(() =>
				{
					
					AlertController = UIAlertController.Create("WBidMax", message, UIAlertControllerStyle.Alert);

					AlertController.AddAction(UIAlertAction.Create("View Seniority List", UIAlertActionStyle.Default, (actionViewSen) =>
					{
						//NSNotificationCenter.DefaultCenter.PostNotificationName("DismissDownloadAndRedirectoLineView", null);

						//DismisscAndRedirectToLineView();
						webPrint fileViewer = new webPrint();

						fileViewer.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
						this.PresentViewController(fileViewer, true, () =>
						{
							fileViewer.loadFileFromUrlFromDownload(seniorityFileName + ".TXT", intEmpNum.ToString());
						//	DismisscAndRedirectToLineView();
						});
						
					}));
					AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionOK) =>
					{

						if ((biddataresponse.Vacation.Count > 0 || biddataresponse.FVVacation.Count > 0) && (vacationlinesObject != null && vacationlinesObject.IsError))
						{


							InvokeOnMainThread(() =>
							{
								AlertController = UIAlertController.Create("WbidMax", biddataresponse.Message, UIAlertControllerStyle.Alert);
								AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) =>
								{
									InvokeInBackground(() =>
									{
										if (!GlobalSettings.isHistorical)
										{
											GlobalSettings.ExtraErrorInfo += "Parse ParseSeniorityList Finished\n<br>";
											GlobalSettings.ExtraErrorInfo += "Parse CheckAndPerformVacationCorrection Started\n<br>";
											DismisscAndRedirectToLineView();
											GlobalSettings.ExtraErrorInfo += "Parse CheckAndPerformVacationCorrection Finished\n<br>";
										}

									});
								}));

								this.PresentViewController(AlertController, true, null);

							});
							//showAlertView(biddataresponse.Message, "Error!!");

						}
						else
						{
							InvokeInBackground(() =>
							{
								if (!GlobalSettings.isHistorical)
								{
									GlobalSettings.ExtraErrorInfo += "Parse ParseSeniorityList Finished\n<br>";
									GlobalSettings.ExtraErrorInfo += "Parse CheckAndPerformVacationCorrection Started\n<br>";
									DismisscAndRedirectToLineView();
									GlobalSettings.ExtraErrorInfo += "Parse CheckAndPerformVacationCorrection Finished\n<br>";
								}

							});
						}
					}));

					this.PresentViewController(AlertController, true, null);
					
				});
			}
			catch (Exception ex)
			{
				WBidHelper.AddDetailsToMailCrashLogs(ex);
			}
		}



		/// <summary>
		/// update the users domicile and position into the local user information
		/// </summary>
		private void UpdateUserDomicileAndPosition()
		{			
			GlobalSettings.WbidUserContent.UserInformation.Domicile = GlobalSettings.DownloadBidDetails.Domicile;
			GlobalSettings.WbidUserContent.UserInformation.BidSeat = GlobalSettings.DownloadBidDetails.Postion;
			//WBidHelper.SaveUserFile(GlobalSettings.WbidUserContent, WBidHelper.WBidUserFilePath);
		}
		/// <summary>
		/// Showing alert with  OK/Cancel message box
		/// </summary>
		/// <param name="message">Error mesage to be displayed</param>
		/// <param name="subject">Heading of the alert message eg:Error,WBid</param>
		private void showAlertAndSimissView(string message, string subject)
        {
			try
			{
				InvokeOnMainThread(() =>
				{
					AlertController = UIAlertController.Create(subject,message , UIAlertControllerStyle.Alert);
					AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) =>
					{
						DismissCurrentView();
					}));

					this.PresentViewController(AlertController, true, null);

				});
			}
			catch(Exception ex)
            {
				throw ex;
            }
        }
		/// <summary>
		/// Showing alert with  OK/Cancel message box
		/// </summary>
		/// <param name="message">Error mesage to be displayed</param>
		/// <param name="subject">Heading of the alert message eg:Error,WBid</param>
		private void showAlertView(string message, string subject)
		{
			try
			{
				if (message == string.Empty)
					message = "Something went wrong. please try again or contact Administrator"; 
				InvokeOnMainThread(() =>
				{
					AlertController = UIAlertController.Create(subject, message, UIAlertControllerStyle.Alert);
					AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) =>
					{
						DismissCurrentView();
					}));

					this.PresentViewController(AlertController, true, null);

				});
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}


		#endregion
		/// <summary>
		/// set the title of this page.
		/// </summary>
		private string SetTitle()
		{
			string domicile = GlobalSettings.DownloadBidDetails.Domicile;
			string position = GlobalSettings.DownloadBidDetails.Postion;
			System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
			string strMonthName = mfi.GetMonthName(GlobalSettings.DownloadBidDetails.Month).ToString();
			string round = GlobalSettings.DownloadBidDetails.Round == "D" ? "1st Round" : "2nd Round";
			string downloadTitle = domicile + " - " + position + " - " + round + " - " + strMonthName + " - " + GlobalSettings.DownloadBidDetails.Year;
			return downloadTitle;
		}
		#region Download Process



		#endregion

		#region Observe Notifications



		public void reachabilityCheck(NSNotification n)
		{
			this.setButtonState(this.btnCheckInternetConnection, true);
			Console.WriteLine("reachabilityCheck");

		}
		public void cwaCredentialsCheck(NSNotification n)
		{
			this.setButtonState(this.btnCheckCWACredentials, true);
			Console.WriteLine("cwaCredentialsCheck");

		}
		public void authCheckSuccess(NSNotification n)
		{
			this.setButtonState(this.btnCheckAuthorization, true);
			Console.WriteLine("authCheckSuccess");
			this.startProgress();
		}
		public void getDataFilesSuccess(NSNotification n)
		{
			this.setButtonState(this.btnGetDataFiles, true);
			Console.WriteLine("getDataFilesSuccess");

			this.startProgress();

		}
		public void parseDataSuccess(NSNotification n)
		{


		}
		public void saveDataSuccess(NSNotification n)
		{
				this.setButtonState(this.btnParseData, true);
				Console.WriteLine("SaveDataFilesSuccess");

				this.startProgress();
			
		}

	



		public void calcVACCorrection(NSNotification n)
		{
			this.setButtonState(this.btnCalculateVACCorrection, true);
		}
		public void setButtonState(UIButton theButton, bool status)
		{
			InvokeOnMainThread(() =>
				{
					theButton.Selected = status;
					//                this.prgrsVw.Progress = 1.0f;
					this.moveProgressToEnd();
				});
		}

		#endregion

		#region Progress Bar
		public void startProgress()
		{
			InvokeOnMainThread(() =>
				{
					this.prgrsVw.Progress = 0.0f;
					PerformSelector(new ObjCRuntime.Selector("incrementProgress"), null, 1);
				});
		}
		[Export("incrementProgress")]
		void incrementProgress()
		{
			if (this.prgrsVw.Progress >= 0.7f)
				return;
			InvokeOnMainThread(() =>
				{
					
					prgrsVw.SetProgress(prgrsVw.Progress + 0.1f, true);
				});
			PerformSelector(new ObjCRuntime.Selector("incrementProgress"), null, 1);

		}
		[Export("moveProgressToEnd")]
		void moveProgressToEnd()
		{
			if (this.prgrsVw.Progress >= 1.0f)
				return;
			InvokeOnMainThread(() =>
				{
					//this.prgrsVw.Progress += this.prgrsVw.Progress + 0.1f;
					prgrsVw.SetProgress(prgrsVw.Progress + 0.1f, true);

				});
			PerformSelector(new ObjCRuntime.Selector("moveProgressToEnd"), null, 0.5);

		}
		#endregion

		

		#region Private Methods

		
		

		/// <summary>
		/// PURPOSE :Download Mock Award details
		/// </summary>
		/// <param name="downloadInfo"></param>
		/// <returns></returns>
		private void DownloadHistoryData(DownloadInfo downloadInfo)
		{
			List<DownloadedFileInfo> downloadedFileDetails = new List<DownloadedFileInfo>();


			WBidDataDownloadAuthorizationService.Model.BidDetails bidDetails = new WBidDataDownloadAuthorizationService.Model.BidDetails ();
			bidDetails.Month = GlobalSettings.DownloadBidDetails.Month;
			bidDetails.Year = GlobalSettings.DownloadBidDetails.Year;
			bidDetails.Round = GlobalSettings.DownloadBidDetails.Round == "D" ? 1 : 2;
			bidDetails.Domicile = GlobalSettings.DownloadBidDetails.Domicile;
			bidDetails.Position = GlobalSettings.DownloadBidDetails.Postion;
			bidDetails.FileName = downloadInfo.DownloadList.FirstOrDefault(x => x.Length == 10 && x.Substring(7, 3) == "737");
			client.DownloadHistoricalDataCompleted+= HandleDownloadHistoricalDataCompleted;
			client.DownloadHistoricalDataAsync(bidDetails);


		}


		void HandleDownloadHistoricalDataCompleted(object sender, DownloadHistoricalDataCompletedEventArgs e)
		{
			if (e.Result != null)
			{
				List<DownloadedFileInfo> lstDownloadedFiles = new List<DownloadedFileInfo>();
				DownloadedFileInfo sWAFileInfo = new DownloadedFileInfo();
				sWAFileInfo.byteArray = e.Result.Data;
				sWAFileInfo.FileName = e.Result.Title;

				sWAFileInfo.IsError = (e.Result.Data == null) ? true : false;

				lstDownloadedFiles.Add(sWAFileInfo);


				if (lstDownloadedFiles == null)
				{
					InvokeOnMainThread(() => {
						AlertController = UIAlertController.Create("Data Transfer Failed", "Error", UIAlertControllerStyle.Alert);
						AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) => {
							DismissCurrentView();
						}));
						this.PresentViewController(AlertController, true, null);
					});
					return;
				}
				//Download Bid line file.
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
					sWAFileInfo = new DownloadedFileInfo();
					sWAFileInfo.byteArray = Convert.FromBase64String(historicalFileInfo.DataString);
					sWAFileInfo.FileName = historicalFileInfo.Title;

					//  sWAFileInfo.IsError = (historicalFileInfo.Data == null) ? true : false;
					lstDownloadedFiles.Add(sWAFileInfo);
				}

				_downloadBidObject.SaveDownloadedBidFiles(lstDownloadedFiles, WBidHelper.GetAppDataPath());
				string zipFileName = _downloadFileDetails.DownloadList.Where(x => x.Contains(".737")).FirstOrDefault();
				DownloadedFileInfo zipFile = lstDownloadedFiles.FirstOrDefault(x => x.FileName == zipFileName);

				if (zipFile.IsError)
				{

					string errorMessage = string.Empty;
					if (zipFile.Message.Contains("BIDINFO DATA NOT AVAILABLE"))
					{

						errorMessage += "The Requested data doesnot exist on  the SWA servers . make sure proper month is selected and you are within the normal timeframe for the request\r\n";
						errorMessage += zipFile.Message.Replace("ERROR: ", " ");

					}
					else
					{
						errorMessage += zipFile.Message.Replace("ERROR: ", " ");

					}


					InvokeOnMainThread(() => {
						AlertController = UIAlertController.Create("Data Transfer Failed", errorMessage, UIAlertControllerStyle.Alert);
						AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) => {
							DismissCurrentView();
						}));
						this.PresentViewController(AlertController, true, null);
					});
				}
				else
				{

					string path = WBidHelper.GetAppDataPath() + "/" + Path.GetFileNameWithoutExtension(zipFileName);

					if (!(File.Exists(path + "/" + "TRIPS") && File.Exists(path + "/" + "PS")))
					{

						InvokeOnMainThread(() => {
							AlertController = UIAlertController.Create("Well Darn it!", "We were downloading the bid data, but your internet connection stopped working just long enough that we could not download all the files completely.\nPlease attempt to download the bid data again.", UIAlertControllerStyle.Alert);
							AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) => {
								DismissCurrentView();
							}));
							this.PresentViewController(AlertController, true, null);
						});
						return;
					}
					//complted the download process
					WBidHelper.SetCurrentBidInformationfromZipFileName(zipFileName, true);

					//Write to currentBidDetailsfile for Error log
					FileOperations.WriteCurrentBidDetails(WBidHelper.GetAppDataPath() + "/CurrentDetails.txt", WBidHelper.GetApplicationBidData());

					//  Set Cover letter file
					if (GlobalSettings.WBidINIContent.MiscellaneousTab.Coverletter)
					{
						DownloadedFileInfo coverLetter = lstDownloadedFiles.FirstOrDefault(x => x.FileName.Contains("C.TXT") && !x.IsError);
						if (coverLetter != null)
						{

							GlobalSettings.IsCoverletterShowFileName = coverLetter.FileName;

						}
					}



					NSNotificationCenter.DefaultCenter.PostNotificationName("getDataFilesSuccess", null);
					//ParseData(zipFileName);
				}
			}
		}




		void alertVW_Clicked(object sender, UIButtonEventArgs e)
		{
			//NSNotificationCenter.DefaultCenter.PostNotificationName("saveDataSuccess", null);
		}



       
       
		
		
		


		
		
		

		private void observeNotifications()
		{
			arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("reachabilityCheckSuccess"), reachabilityCheck));
			arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("cwaCheckSuccess"), cwaCredentialsCheck));
			arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("authCheckSuccess"), authCheckSuccess));
			arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("getDataFilesSuccess"), getDataFilesSuccess));
			arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("parseDataSuccess"), parseDataSuccess));
			arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("saveDataSuccess"), saveDataSuccess));
			arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("calcVACCorrection"), calcVACCorrection));
			arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("DismissDownloadAndRedirectoLineView"), DismissDownloadAndRedirectoLineView));
		}

        private void DismissDownloadAndRedirectoLineView(NSNotification obj)
        {
			
			DismisscAndRedirectToLineView();

		}

        void SouthWestWiFiDownload()
		{
			if (GlobalSettings.WbidUserContent.UserInformation.PaidUntilDate >= DateTime.Now)
			{
				//download bid data
				this.startProgress();
				Authentication authentication = new Authentication();
				string authResult = authentication.CheckCredential(_downloadFileDetails.UserId, _downloadFileDetails.Password);
				if (authResult.Contains("ERROR: "))
				{
					WBidLogEvent obj = new WBidLogEvent();
					obj.LogBadPasswordUsage(_downloadFileDetails.UserId, true, authResult);
					InvokeOnMainThread(() =>
					{
						KeychainHelpers.SetPasswordForUsername("pass", "", "WBid.WBidiPad.cwa", SecAccessible.Always, false);
												
						CustomAlertView customAlert = new CustomAlertView();
						UINavigationController nav = new UINavigationController(customAlert);
						nav.NavigationBarHidden = true;
						nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
						customAlert.ObjDownload = this;
						customAlert.AlertType = "InvalidCredential";
						this.PresentViewController(nav, true, null);
						//this.PresentViewController(AlertController, true, null);

					});
				}
				else if (authResult.Contains("Exception"))
				{
					InvokeOnMainThread(() =>
					{

						showTimeOutAlert();
						DismissCurrentView();
					});
				}
				else
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("cwaCheckSuccess", null);
					this.startProgress();
					_sessionCredentials = authResult;
				}
				//UpdateUserDomicileAndPosition();
				//DownloadBidData();
				WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
				obgWBidLogEvent.LogAllEvents(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "dataDownloadAuth", "0", "0","");
			}
			else
			{
				//show Invalid Subscription message
				InvokeOnMainThread(() =>
				{

					AlertController = UIAlertController.Create("WBidMax", "Your subscription is expired and you are on the plane using the free company limited internet connection.\nYou cannot update your subscription using the limited internet connection.Either pay for a full internet connection or wailt until you get on the ground and have a full internet connection", UIAlertControllerStyle.Alert);
					AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) =>
					{
						DismissCurrentView();
					}));

					this.PresentViewController(AlertController, true, null);
					NSNotificationCenter.DefaultCenter.PostNotificationName("reachabilityCheckFailed", null);
				});
			}
		}
		void showTimeOutAlert()
		{
			//-----
			TimeOutAlertView regs = new TimeOutAlertView();
			UIKit.UIPopoverController popoverController = new UIPopoverController(regs);
			objPopDelegate = new MyPopDelegate(this);
			objPopDelegate.CanDismiss = false;
			popoverController.Delegate = objPopDelegate;
			regs.ObjDownload = this;
			regs.objpopover = popoverController;

			CGRect frame = new CGRect((View.Frame.Size.Width / 2) - 75, (View.Frame.Size.Height / 2) - 175, 150, 350);
			popoverController.PopoverContentSize = new CGSize(regs.View.Frame.Width, regs.View.Frame.Height);
			popoverController.PresentFromRect(frame, View, 0, true);

			//------
		}
		private void initiateDownloadProcess()
		{

			_downloadFileDetails = new DownloadInfo();
			_downloadFileDetails.UserId = KeychainHelpers.GetPasswordForUsername("user", "WBid.WBidiPad.cwa", false);
			_downloadFileDetails.Password = KeychainHelpers.GetPasswordForUsername("pass", "WBid.WBidiPad.cwa", false);

            if (WBidHelper.IsSouthWestWifiOr2wire()==false) 
			{
				//checking  the internet connection available
				//==================================================================================================================
				if (Reachability.IsHostReachable (GlobalSettings.ServerUrl)) {


					NSNotificationCenter.DefaultCenter.PostNotificationName ("reachabilityCheckSuccess", null);
					//checking CWA credential
					//==================================================================================================================

					this.startProgress ();


					Authentication authentication = new Authentication ();
					string authResult = authentication.CheckCredential (_downloadFileDetails.UserId, _downloadFileDetails.Password);
					if (authResult.Contains ("ERROR: ")) 
					{
						WBidLogEvent obj = new WBidLogEvent();
						obj.LogBadPasswordUsage(_downloadFileDetails.UserId,true, authResult);

						InvokeOnMainThread (() => {
							KeychainHelpers.SetPasswordForUsername ("pass", "", "WBid.WBidiPad.cwa", SecAccessible.Always, false);

							CustomAlertView customAlert = new CustomAlertView();
							UINavigationController nav = new UINavigationController(customAlert);
							nav.NavigationBarHidden = true;
							nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
							customAlert.ObjDownload = this;
							customAlert.AlertType = "InvalidCredential";
							this.PresentViewController(nav, true, null);

                        });
					} else if (authResult.Contains ("Exception")) {


						InvokeOnMainThread (() => {
							
							showTimeOutAlert();
							//DismissCurrentView();
						});
					} else {
						NSNotificationCenter.DefaultCenter.PostNotificationName ("cwaCheckSuccess", null);
						this.startProgress ();

						_sessionCredentials = authResult;
						//checking authorization
						//==================================================================================================================

						ClientRequestModel clientRequestModel = new ClientRequestModel ();
						clientRequestModel.Base = GlobalSettings.DownloadBidDetails.Domicile;
						clientRequestModel.BidRound = (GlobalSettings.DownloadBidDetails.Round == "D") ? 1 : 2;
						clientRequestModel.Month = new DateTime (GlobalSettings.DownloadBidDetails.Year, GlobalSettings.DownloadBidDetails.Month, 1).ToString ("MMM").ToUpper ();
						clientRequestModel.Postion = GlobalSettings.DownloadBidDetails.Postion;
						clientRequestModel.OperatingSystem = "iPad OS";
						clientRequestModel.Platform = "iPad";
						if (GlobalSettings.isHistorical)
						{

							clientRequestModel.RequestType = (int)RequestTypes.DownnloadHostoricalBid;
						}
						else
						{
							clientRequestModel.RequestType = (int)RequestTypes.DownnloadBid;
						}
						token = Guid.NewGuid ();
						clientRequestModel.Token = token;
						clientRequestModel.Version = System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
						//clientRequestModel.Version = "4.0.31.2";
						clientRequestModel.EmployeeNumber = Convert.ToInt32 (Regex.Match (_downloadFileDetails.UserId, @"\d+").Value);

						// client.GetAuthorizationDetailsAsync(clientRequestModel);
						client.GetAuthorizationforMultiPlatformAsync (clientRequestModel);
					}
				} else {
					//check if it is Sotuhewesit Wifi

					//"southwestwifi"
					//string wifiSSID=CurrentSSID();

                    if (WBidHelper.IsSouthWestWifiOr2wire()) 
					{
						InvokeOnMainThread (() => {
                            AlertController = UIAlertController.Create("WBidMax", Constants.SouthWestConnectionAlert, UIAlertControllerStyle.Alert);
                            AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {
                                DismissCurrentView();
                            }));
                            this.PresentViewController(AlertController, true, null);
                            NSNotificationCenter.DefaultCenter.PostNotificationName("reachabilityCheckFailed", null);
                           

                        });
						

					} else {

						InvokeOnMainThread (() => {
						
                            AlertController = UIAlertController.Create("WBidMax", Constants.VPSDownAlert, UIAlertControllerStyle.Alert);
                            AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {
                                DismissCurrentView();
                            }));
                            this.PresentViewController(AlertController, true, null);
                            NSNotificationCenter.DefaultCenter.PostNotificationName ("reachabilityCheckFailed", null);
						});
					}
				}
			} else {

				InvokeOnMainThread (() => {
                   
                    AlertController = UIAlertController.Create("WBidMax", Constants.SouthWestConnectionAlert, UIAlertControllerStyle.Alert);
                    AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {
                        DismissCurrentView();
                    }));
                    this.PresentViewController(AlertController, true, null);
                    NSNotificationCenter.DefaultCenter.PostNotificationName("reachabilityCheckFailed", null);
                });
                // we dont need to allow the bid dowbload if the user using south west or 2 wire 
				//SouthWestWiFiDownload ();

			}
		}
	
		/// <summary>
		/// Gets the current SSID.
		/// </summary>
		/// <value>The current SSID.</value>
		/// 
		public string CurrentSSID ()
		{

			NSDictionary dict;
			var status = CaptiveNetwork.TryCopyCurrentNetworkInfo ("en0", out dict);
			if (dict != null) {
				if (status == StatusCode.NoKey) {
					return string.Empty;
				}

				var bssid = dict [CaptiveNetwork.NetworkInfoKeyBSSID];
				var ssid = dict [CaptiveNetwork.NetworkInfoKeySSID];
				var ssiddata = dict [CaptiveNetwork.NetworkInfoKeySSIDData];

				return ssid.ToString ();
			} else
				return string.Empty;

		}
		public void DismissCurrentView()
		{

			foreach (NSObject obj in arrObserver)
			{
				NSNotificationCenter.DefaultCenter.RemoveObserver(obj);
			}

			

			this.DismissViewController(true, null);

		}

		
		void ShowVacationOverlapView ()
		{
			InvokeOnMainThread (() => {
				string dynamicdate = string.Empty;
				var leadinvacation = GlobalSettings.SeniorityListMember.Absences.FirstOrDefault (x => x.AbsenceType == "VA" && x.StartAbsenceDate < GlobalSettings.CurrentBidDetails.BidPeriodStartDate);
				if (leadinvacation != null) {
					if (leadinvacation.EndAbsenceDate == GlobalSettings.CurrentBidDetails.BidPeriodStartDate)
						dynamicdate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate.ToString ("MMM") + " - " + GlobalSettings.CurrentBidDetails.BidPeriodStartDate.Day.ToString ();
					else
						dynamicdate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate.Month.ToString () + "/" + GlobalSettings.CurrentBidDetails.BidPeriodStartDate.Day + " - " + leadinvacation.EndAbsenceDate.Month.ToString () + "/" + leadinvacation.EndAbsenceDate.Day.ToString ();
				}
				string Message1 = "You have a vacation that overlaps the begining of the month.  WBidMax needs to know how much VA the company awarded for  " + dynamicdate;
				string Message2 = "Log into CWA and go to your crewboard.  Hover over the green bar for  " + dynamicdate + "  and copy the VA credit  for those vacation days.";
				this.View.AddSubview (vwVacOverLap);
				vwVacOverLap.Center = new CGPoint (this.View.Center.X, this.View.Center.Y - 50);
				vwVacOverLap.Layer.BorderWidth = 1;
				vwVacOverLap.Layer.BorderColor = UIColor.FromRGB (158, 179, 131).CGColor;
				vwVacOverLap.Layer.CornerRadius = 3.0f;
				vwVacOverLap.Layer.ShadowColor = UIColor.Black.CGColor;
				vwVacOverLap.Layer.ShadowOpacity = 0.5f;
				vwVacOverLap.Layer.ShadowRadius = 2.0f;
				vwVacOverLap.Layer.ShadowOffset = new CGSize (3f, 3f);
				lblMessage1.Text = Message1;
				lblMessage2.Text = Message2;
				//txtVANumber.BecomeFirstResponder ();
				txtVANumber.ShouldChangeCharacters = (textField, range, replacementString) => {
					string text = textField.Text;
					string newText = text.Substring (0, (int)range.Location) + replacementString + text.Substring ((int)range.Location + (int)range.Length);
					decimal val;
					if (newText == "")
						return true;
					else
						return decimal.TryParse (newText, out val);
				};
				btnVacDone.TouchUpInside += (object sender, EventArgs e) => {
					txtVANumber.ResignFirstResponder ();
					var value = leadinvacation.EndAbsenceDate.Date.Day * GlobalSettings.DailyVacPay;
					if (!string.IsNullOrEmpty (txtVANumber.Text)) {
						if (decimal.Parse (txtVANumber.Text) >= 0 && decimal.Parse (txtVANumber.Text) <= value) {
							GlobalSettings.CompanyVA = txtVANumber.Text;
							_waitCompanyVADialog = false;
							vwVacOverLap.RemoveFromSuperview ();
						} else {

                            UIAlertController okAlertController = UIAlertController.Create("WBidMax", "Please enter a value below or equal to " + value.ToString(), UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);

                        }
                    }
				};
				btnVacLater.TouchUpInside += (object sender, EventArgs e) => {
					txtVANumber.ResignFirstResponder ();
					GlobalSettings.IsVacationCorrection = false;
					_waitCompanyVADialog = false;
					vwVacOverLap.RemoveFromSuperview ();
                    AlertController = UIAlertController.Create("WBidMax", "You can do vacation corrections later, when you have retrieved the company awared VA, by simply re-downloading the bid data.", UIAlertControllerStyle.Alert);
                    AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(AlertController, true, null);
                };
			});
		}

		


		#endregion
		

		#region WCF Events 

		void client_UpdateUserbaseAndPosotionCompleted(object sender,UpdateUserBidBaseAndSeatCompletedEventArgs e)
		{
		}
		void client_GetAuthorizationforMultiPlatformCompleted(object sender, GetAuthorizationforMultiPlatformCompletedEventArgs e)
		{
			try
			{
				GlobalSettings.ExtraErrorInfo=string.Empty;
				bool timeout = false;
				if (e.Error != null && e.Error.Message != "")
				{
					client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 60);
					client.LogTimeOutDetailsAsync(token);
					timeout = true;
				}

				if (timeout || e.Result != null)
				{

					ServiceResponseModel serviceResponseModel = new ServiceResponseModel();
					if (timeout)
						serviceResponseModel.IsAuthorized = true;
					else
						serviceResponseModel = e.Result;

					if (serviceResponseModel.FlightDataVersion != string.Empty)
						GlobalSettings.ServerFlightDataVersion = serviceResponseModel.FlightDataVersion;

					if (serviceResponseModel.IsAuthorized)
					{
						GlobalSettings.IsNeedToDownloadSeniorityUser= serviceResponseModel.IsNeedToDownloadSeniorityFromServer;
												
					    
						DownloadBidData();
					}
					else
					{
						GlobalSettings.ExtraErrorInfo += "serviceResponseModel.IsAuthorized =false\n<br>";
						if (serviceResponseModel.Type != null && serviceResponseModel.Type == "Invalid Account")
							// if (serviceResponseModel.Message != null && serviceResponseModel.Message == "Invalid Account")
						{

							var bodyContent = GetInvalidAccountMessage();
							WBidMail objMailAgent = new WBidMail();
							objMailAgent.SendMailtoAdmin(bodyContent.ToString(), GlobalSettings.WbidUserContent.UserInformation.Email, "User has Invalid Account");
						}

						InvokeOnMainThread(() =>
							{
							    AlertController = UIAlertController.Create("Error", serviceResponseModel.Message, UIAlertControllerStyle.Alert);
                                AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) => {
                                    DismissCurrentView();
                                }));
                                this.PresentViewController(AlertController, true, null);
                            });
					}
				}
				GlobalSettings.ExtraErrorInfo=string.Empty;
			}
			catch (Exception ex)
			{

				FileOperations.WriteCurrentBidDetails(WBidHelper.GetAppDataPath() + "/CurrentDetails.txt", WBidHelper.GetApplicationBidData()+"\n"+GlobalSettings.ExtraErrorInfo);
				InvokeOnMainThread(() =>
					{

						throw ex;
					});
			}
		}
		
		
		
		private StringBuilder GetInvalidAccountMessage()
		{
			var bodyContent = new StringBuilder();
			bodyContent.Append("<table width=\"700\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style='font-family:Arial;font-size:12'>");
			bodyContent.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\"> Hi Admin, </td></tr>");
			bodyContent.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style='height:15px'> </td></tr>");
			bodyContent.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\"> I am getting \"Invalid Account\" message while Downloading bid(authenticating)  </td></tr>");
			bodyContent.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style='height:15px'> </td></tr>");

			bodyContent.Append("<tr><td style='Width:180px'><B>First Name</B></td><td>" + GlobalSettings.WbidUserContent.UserInformation.FirstName + " </td><td></td></tr>");
			bodyContent.Append("<tr><td ><B>Last Name</B></td><td>" + GlobalSettings.WbidUserContent.UserInformation.LastName + " </td><td></td></tr>");
			bodyContent.Append("<tr><td ><B>Email</B></td><td>" + GlobalSettings.WbidUserContent.UserInformation.Email + " </td><td></td></tr>");
			try
			{
				bodyContent.Append("<tr><td ><B>Employee Number</B></td><td>" + Regex.Match(_downloadFileDetails.UserId, @"\d+").Value + " </td><td></td></tr>");
			}
			catch (Exception)
			{


			}

			bodyContent.Append("<tr><td ><B>Domicile</B></td><td>" + GlobalSettings.WbidUserContent.UserInformation.Domicile + " </td><td></td></tr>");
			bodyContent.Append("<tr><td ><B>Gender</B></td><td>" + (GlobalSettings.WbidUserContent.UserInformation.IsFemale ? "Female" : "Male") + " </td><td></td></tr>");
			bodyContent.Append("<tr><td ><B>Position</B></td><td>" + GlobalSettings.WbidUserContent.UserInformation.Position + " </td><td></td></tr>");
			bodyContent.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style='height:15px'> </td></tr>");
			bodyContent.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style=>" + GlobalSettings.WbidUserContent.UserInformation.FirstName + " " + GlobalSettings.WbidUserContent.UserInformation.LastName + " </td></tr>");

			bodyContent.Append("</table>");
			return bodyContent;
		}
		public class MyPopDelegate : UIPopoverControllerDelegate
		{
			downloadBidDataViewController _parent;
			public bool CanDismiss;
			public MyPopDelegate(downloadBidDataViewController parent)
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
		private class CustomWebClient : WebClient
		{
			public CustomWebClient()
			{

			}

			protected override WebRequest GetWebRequest(Uri uri)
			{
				WebRequest w = base.GetWebRequest(uri);
				w.Timeout = 90000;
				return w;
			}



		}
	
	}
}

