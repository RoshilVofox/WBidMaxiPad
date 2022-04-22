using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using System.IO;
using WBid.WBidiPad.iOS.Utility;

//using WBid.WBidiPad.PortableLibrary.Core;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.SharedLibrary.Utility;
using WBid.WBidiPad.SharedLibrary.Parser;

//using WBid.WBidiPad.SharedLibrary.Model;
using WBid.WBidiPad.SharedLibrary;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.SharedLibrary.Serialization;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WBid.WBidiPad.SharedLibrary.SWA;
using System.Net;

//using MiniZip.ZipArchive;
using System.ServiceModel;
using WBidPushService.Model;

//using WBid.WBidiPad.iOS.Common;
using iOSPasswordStorage;
using Security;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using WBid.WBidiPad.iOS;
using UserNotifications;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Essentials;

using CoreLocation;
using System.Json;

namespace WBidMax
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate, ICLLocationManagerDelegate
	{
		// class-level declarations
		private readonly CLLocationManager locationManager = new CLLocationManager();


		public override UIWindow Window
		{
			get;
			set;
		}
		UIWindow window;

		string tok = string.Empty;
		public string devID = string.Empty;
		public static UIStoryboard storyboard = UIStoryboard.FromName("Main", null);
		public static AppDelegate Self { get; private set; }
		
		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			// Override point for customization after application launch.
			// If not required for your application you can safely delete this method

			// Location access checking

			locationManager.Delegate = this;
			locationManager.RequestWhenInUseAuthorization();

			if (CLLocationManager.LocationServicesEnabled)
			{

			}

			try
			{
				AppCenter.Start("9cac26b2-7c71-4313-b92f-b02ffa1e1bdf", typeof(Analytics), typeof(Crashes));
			}
			catch (Exception ex)
			{
				throw ex;
			}

			// Code to start the Xamarin Test Cloud Agent
#if ENABLE_TEST_CLOUD
			//Xamarin.Calabash.Start();
#endif

			AppDelegate.Self = this;
			// class-level declarations

			// This method is invoked when the application has loaded and is ready to run. In this
			// method you should instantiate the window, load the UI into it and then make the window
			// visible.
			//
			// You have 17 seconds to return from this method, or iOS will terminate your application.
			//

			///CommonClass.SaveFormattedBidReceiptForPilot("80348\n409\n410\n411\n412\n413\n414\n415\n416\n417\n418\n419\n420\n421\n422\n423\n424\n425\n426\n427\n428\n429\n430\n431\n432\n433\n434\n435\n436\n437\n438\n439\n440\n441\n442\n443\n444\n445\n446\n447\n448\n449\n450\n451\n452\n453\n454\n455\n456\n457\n458\n459\n460\n461\n462\n463\n464\n465\n466\n467\n468\n469\n470\n471\n472\n473\n474\n475\n476\n477\n478\n479\n480\n481\n482\n483\n484\n409\n410\n411\n412\n413\n414\n415\n416\n417\n418\n419\n420\n421\n422\n423\n424\n425\n426\n427\n428\n429\n430\n431\n432\n433\n434\n435\n436\n437\n438\n439\n440\n441\n442\n443\n444\n445\n446\n447\n448\n449\n450\n451\n452\n453\n454\n455\n456\n457\n458\n459\n460\n461\n462\n463\n464\n465\n466\n467\n468\n469\n470\n471\n472\n473\n474\n475\n476\n477\n478\n479\n480\n481\n482\n483\n484\n409\n410\n411\n412\n413\n414\n415\n416\n417\n418\n419\n420\n421\n422\n423\n424\n425\n426\n427\n428\n429\n430\n431\n432\n433\n434\n435\n436\n437\n438\n439\n440\n441\n442\n443\n444\n445\n446\n447\n448\n449\n450\n451\n452\n453\n454\n455\n456\n457\n458\n459\n460\n461\n462\n463\n464\n465\n466\n467\n468\n469\n470\n471\n472\n473\n474\n475\n476\n477\n478\n479\n480\n481\n482\n483\n484\n409\n410\n411\n412\n413\n414\n415\n416\n417\n418\n419\n420\n421\n422\n423\n424\n425\n426\n427\n428\n429\n430\n431\n432\n433\n434\n435\n436\n437\n438\n439\n440\n441\n442\n443\n444\n445\n446\n447\n448\n449\n450\n451\n452\n453\n454\n455\n456\n457\n458\n459\n460\n461\n462\n463\n464\n465\n466\n467\n468\n469\n470\n471\n472\n473\n474\n475\n476\n477\n478\n479\n480\n481\n482\n483\n484\n409\n410\n411\n412\n413\n414\n415\n416\n417\n418\n419\n420\n421\n422\n423\n424\n425\n426\n427\n428\n429\n430\n431\n432\n433\n434\n435\n436\n437\n438\n439\n440\n441\n442\n443\n444\n445\n446\n447\n448\n449\n450\n451\n452\n453\n454\n455\n456\n457\n458\n459\n460\n461\n462\n463\n464\n465\n466\n467\n468\n469\n470\n471\n472\n473\n474\n475\n476\n477\n478\n479\n480\n481\n482\n483\n484\n409\n410\n411\n412\n413\n414\n415\n416\n417\n418\n419\n420\n421\n422\n423\n424\n425\n426\n427\n428\n429\n430\n431\n432\n433\n434\n435\n436\n437\n438\n439\n440\n441\n442\n443\n444\n445\n446\n447\n448\n449\n450\n451\n452\n453\n454\n455\n456\n457\n458\n459\n460\n461\n462\n463\n464\n465\n466\n467\n468\n469\n470\n471\n472\n473\n474\n475\n476\n477\n478\n479\n480\n481\n482\n483\n484\n409\n410\n411\n412\n413\n414\n415\n416\n417\n418\n419\n420\n421\n422\n423\n424\n425\n426\n427\n428\n429\n430\n431\n432\n433\n434\n435\n436\n437\n438\n439\n440\n441\n442\n443\n444\n445\n446\n447\n448\n449\n450\n451\n452\n453\n454\n455\n456\n457\n458\n459\n460\n461\n462\n463\n464\n465\n466\n467\n468\n469\n470\n471\n472\n473\n474\n475\n476\n477\n478\n479\n480\n481\n482\n483\n484\n*E\n*E\n SUBMITTED BY: [e80348]     80348    04/17/22 07:27:56\r\n");
			Console.WriteLine("App Launch");
			Console.WriteLine(UIDevice.CurrentDevice.IdentifierForVendor.AsString());
			//IS App using VPS server or is null value in NsDefaults .Deafult is VPS
			var server = NSUserDefaults.StandardUserDefaults["isVPS"];
			if (server != null)
			{
				CommonClass.isVPSServer = server.ToString();
			}
			else
			{
				CommonClass.isVPSServer = "TRUE";
			}



			var IsModernScrollClassic = NSUserDefaults.StandardUserDefaults["IsModernScrollClassic"];
			if (IsModernScrollClassic != null)
			{
				CommonClass.IsModernScrollClassic = IsModernScrollClassic.ToString();
			}
			else
			{
				CommonClass.IsModernScrollClassic = "TRUE";
			}

			//Get Username and Password in Key Chain Or Create Defaults values if it is not exists in key chain

			devID = KeychainHelpers.GetPasswordForUsername("DeviceID", "WBid.ID", false);
			if (string.IsNullOrEmpty(devID))
			{

				KeychainHelpers.SetPasswordForUsername("DeviceID", UIDevice.CurrentDevice.IdentifierForVendor.AsString(), "WBid.ID", SecAccessible.Always, false);
				devID = KeychainHelpers.GetPasswordForUsername("DeviceID", "WBid.ID", false);
			}
			//	NSThread.SleepFor (3);


			try
			{

				//============ Check App Data folder exists or not.

				if (!Directory.Exists(WBidHelper.GetAppDataPath()))
				{
					//create app data folder
					WBidHelper.CreateAppDataDirectory();
				}

				//=============Read User Account Details

				ReadUserData(application);

				//=============cheCk the INI file is ceated or not.If not,create it.
				if (!File.Exists(WBidHelper.GetWBidINIFilePath()))
				{
					WBidINI wbidINI = WBidCollection.CreateINIFile();
					XmlHelper.SerializeToXml(wbidINI, WBidHelper.GetWBidINIFilePath());
				}

				//==============cheCk the DWC file is ceated or not.If not,create it.

				if (!File.Exists(WBidHelper.GetWBidDWCFilePath()))
				{
					WBidIntialState WBidIntialState = WBidCollection.CreateDWCFile(GlobalSettings.DwcVersion);
					XmlHelper.SerializeToXml(WBidIntialState, WBidHelper.GetWBidDWCFilePath());
				}

				ReadINIData();

				ReadDWCData();

				ManageINIDataafterINIRead();
				//LoadINIFileData ();

				var exURL = NSUrl.FromFilename(WBidHelper.GetAppDataPath() + "/");
				exURL.SetResource(new NSString("NSURLIsExcludedFromBackupKey"), NSNumber.FromBoolean(true));

				if (File.Exists(WBidHelper.GetAppDataPath() + "/Crash/" + "Crash.log") && GlobalSettings.WBidINIContent.User.IsNeedCrashMail)
				{

					//Check internet wavailable
					if (GlobalSettings.WBidINIContent.User != null && GlobalSettings.WBidINIContent.User.IsSouthWestWifiTest == false)
					{
						if (Reachability.CheckVPSAvailable())
						{
							string content = System.IO.File.ReadAllText(WBidHelper.GetAppDataPath() + "/Crash/" + "Crash.log");
							WBidMail wbidMail = new WBidMail();

							if (GlobalSettings.WbidUserContent != null && GlobalSettings.WbidUserContent.UserInformation != null)
							{
								wbidMail.SendCrashMail(content);

								File.Delete(WBidHelper.GetAppDataPath() + "/Crash/" + "Crash.log");
							}
						}
					}
				}


				//copyBundledFileToAppdata ("ColumnDefinitions.xml");
				LoadColumnDefenitionData();

				Console.WriteLine("Loading INI and AppData done");



				GlobalSettings.MenuBarButtonStatus = new MenuBarButtonStatus();


				GlobalSettings.SynchEnable = GlobalSettings.WBidINIContent.User.SmartSynch;

				

			}
			catch (Exception ex)
			{
				//throw ex;
			}

			try
			{
				Self.GetAppStoreAppVersion();
			}
			catch (Exception ex)
			{

			}
			Console.WriteLine("Launch method finished.");
			CommonClass.MainViewType = "Summary";
			UINavigationController navController = new UINavigationController();
			homeViewController homeVC = new homeViewController();
			Window.RootViewController = navController;
			navController.NavigationBar.BarStyle = UIBarStyle.Black;
			navController.NavigationBar.Hidden = true;
			navController.PushViewController(homeVC, false);
			Window.MakeKeyAndVisible();
			return true;
		}


		// Check for the new version in App Store.
		private void GetAppStoreAppVersion()
		{
			var IsUpdateRequired = false;
			string appStoreAppVersion = string.Empty;
			string currentVersion = string.Empty;

			try
			{
				if (Reachability.IsHostReachable("www.google.com"))
				{
					var dictionary = NSBundle.MainBundle.InfoDictionary;
					string applicationID = dictionary[@"CFBundleIdentifier"].ToString();
					currentVersion = dictionary[@"CFBundleShortVersionString"].ToString();

					NSUrl url = new NSUrl($"http://itunes.apple.com/lookup?bundleId={applicationID}");
					NSData data = NSData.FromUrl(url);
					NSError error = new NSError();
					NSDictionary lookup = NSJsonSerialization.Deserialize(data, 0, out error) as NSDictionary;

					if (error == null
						&& lookup != null
						&& lookup.Keys.Length >= 1
						&& lookup["resultCount"] != null
						&& Convert.ToInt32(lookup["resultCount"].ToString()) > 0)
					{

						var results = lookup[@"results"] as NSArray;

						if (results != null && results.Count > 0)
						{
							appStoreAppVersion = results.GetItem<NSDictionary>(0)["version"].ToString();
							var storeVersion = new Version(appStoreAppVersion);
							var localVersion = new Version(currentVersion);
							var result = storeVersion.CompareTo(localVersion);
							if ((appStoreAppVersion.ToString() != currentVersion.ToString() && (result > 0)))
							{
								IsUpdateRequired = true;
							}

						}
					}
				}
			}
			catch (Exception ex)
			{
				//No need to throw an exception if version check fails 
			}
			if (IsUpdateRequired)
			{
				InvokeOnMainThread(() =>
				{

					var msg = string.Format("You do not have newest version of WbidMax.You are using version {0} and the latest version is {1}", currentVersion, appStoreAppVersion);
					var pushView = UIAlertController.Create("Update Available", msg, UIAlertControllerStyle.Alert);
					pushView.AddAction(UIAlertAction.Create("Go to AppStore", UIAlertActionStyle.Default, alert =>
					{

						var nsurl = new NSUrl("https://itunes.apple.com/us/app/wbidmax/id892320623?mt=8");
						UIApplication.SharedApplication.OpenUrl(nsurl);
					}));
					pushView.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Default, null));
					Window.MakeKeyAndVisible();
					this.Window.RootViewController.PresentViewController(pushView, true, null);
				});
			}
		}



		private void AddExceptionDetailToCrashLog(string MailContent)
		{

			try
			{
				var submitResult = "\r\n WbidiPad Launch Error.\r\n\r\n Error  :  " + MailContent + "\r\n\r\n Version : " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();


				submitResult += "\r\n\r\n Device Model:" + DeviceInfo.Model;
				submitResult += "\r\n iOS Version:" + UIDevice.CurrentDevice.SystemVersion;


				var profiles = Connectivity.ConnectionProfiles;
				string internetType = string.Empty;
				if (profiles.Contains(ConnectionProfile.WiFi))
				{
					internetType = "Wifi";
				}
				if (profiles.Contains(ConnectionProfile.Cellular))
				{
					internetType = "Cellular";
				}
				if (profiles.Contains(ConnectionProfile.Ethernet))
				{
					internetType = "BlueTooth";
				}
				submitResult += "r\n Internet Connectivity Via:" + UIDevice.CurrentDevice.SystemVersion;

				if (!Directory.Exists(WBidHelper.GetAppDataPath() + "/" + "Crash"))
				{
					Directory.CreateDirectory(WBidHelper.GetAppDataPath() + "/" + "Crash");
				}

				System.IO.File.AppendAllText(WBidHelper.GetAppDataPath() + "/Crash/" + "Crash.log", submitResult);
			}
			catch (Exception Exception)
			{
			}

		}
		public override void DidEnterBackground(UIApplication application)
		{
			application.IgnoreSnapshotOnNextApplicationLaunch();
			AppStateSave();

		}
		private void ReadUserData(UIApplication app)
		{
			if (File.Exists(WBidHelper.WBidUserFilePath))
			{

				try
				{
					GlobalSettings.WbidUserContent = (WbidUser)XmlHelper.DeserializeFromXmlForUserFile<WbidUser>(WBidHelper.WBidUserFilePath);

					if (GlobalSettings.WbidUserContent != null)
					{
						var version = UIDevice.CurrentDevice.SystemVersion.Split('.');
						string compVersion = version[0] + "." + version[1];
						if (float.Parse(compVersion) < 8.0)
						{
							UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
							app.RegisterForRemoteNotificationTypes(notificationTypes);
						}
						else
						{
							//UIUserNotificationSettings notificationSettings = UIUserNotificationSettings.GetSettingsForTypes (UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null);
							//app.RegisterUserNotificationSettings (notificationSettings);
							//app.RegisterForRemoteNotifications ();


							UNUserNotificationCenter notification = UNUserNotificationCenter.Current;
							notification.Delegate = new UserNotificationCenterDelegate();

							notification.RequestAuthorization(UNAuthorizationOptions.Alert, (approved, err) =>
							{
								// Handle approval

								if (err == null)
								{
									InvokeOnMainThread(() =>
							{
								UIApplication.SharedApplication.RegisterForRemoteNotifications();
							});

								}
								else
								{

								}
							});

							////Get current notification settings.
							//notification.GetNotificationSettings((settings) =>
							//{
							//                         //Notification not allowed
							//                         if(settings.AuthorizationStatus != UNAuthorizationStatus.Authorized)
							//                         {

							//                         }
							//                         else
							//                         {

							//                         }

							//	var alertsAllowed = (settings.AlertSetting == UNNotificationSetting.Enabled);
							//});


							//----------------------


						}
					}
				}
				catch (Exception exception)
				{

					File.Delete(WBidHelper.WBidUserFilePath);
					GlobalSettings.WbidUserContent = null;
					AddExceptionDetailToCrashLog("User account file corrupted");
				}


			}
		}






		/// <summary>
		/// Reads the INI data.
		/// </summary>
		private void ReadINIData()
		{

			try
			{
				//read the values of the INI file.
				GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
				GlobalSettings.WBidINIContent.Cities = GlobalSettings.WBidINIContent.Cities.OrderBy(x => x.Name).ToList();
			}
			catch
			{
				//if any error happened , again create  INI file
				WBidINI wbidINI = WBidCollection.CreateINIFile();
				XmlHelper.SerializeToXml(wbidINI, WBidHelper.GetWBidINIFilePath());
				GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
				GlobalSettings.WBidINIContent.Cities.OrderBy(x => x.Name);
				AddExceptionDetailToCrashLog("INI file corrupted and Recreated");
			}
		}
		/// <summary>
		/// Reads the DWC data.
		/// </summary>
		private void ReadDWCData()
		{

			WBidIntialState wbidintialState = null;
			try
			{
				wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
			}
			catch (Exception ex)
			{
				wbidintialState = WBidCollection.CreateDWCFile(GlobalSettings.DwcVersion);
				XmlHelper.SerializeToXml(wbidintialState, WBidHelper.GetWBidDWCFilePath());
				AddExceptionDetailToCrashLog("DWC file corrupted and Recreated");
				//WBidHelper.LogDetails(GlobalSettings.WbidUserContent.UserInformation.EmpNo,"dwcRecreate","0","0");
				if (GlobalSettings.WbidUserContent == null)
				{
					if (File.Exists(WBidHelper.WBidUserFilePath))
					{
						GlobalSettings.WbidUserContent = (WbidUser)XmlHelper.DeserializeFromXmlForUserFile<WbidUser>(WBidHelper.WBidUserFilePath);
					}
				}

				try
				{
					WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
					obgWBidLogEvent.LogAllEvents(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "dwcRecreate", "0", "0", "");
				}
				catch
				{
				}


			}
			//Manage DWC data after read DWC data
			if (decimal.Parse(wbidintialState.Version) < 2.9m)
			{
				File.Delete(WBidHelper.GetWBidDWCFilePath());
				WBidIntialState WBidIntialState = WBidCollection.CreateDWCFile(GlobalSettings.DwcVersion);
				XmlHelper.SerializeToXml(WBidIntialState, WBidHelper.GetWBidDWCFilePath());
			}

		}


		private void ManageINIDataafterINIRead()
		{
			if (GlobalSettings.WBidINIContent.BidLineVacationColumns == null || GlobalSettings.WBidINIContent.BidLineVacationColumns.Count == 0 || GlobalSettings.WBidINIContent.ModernVacationColumns == null || GlobalSettings.WBidINIContent.ModernVacationColumns.Count == 0 || GlobalSettings.WBidINIContent.SummaryVacationColumns == null || GlobalSettings.WBidINIContent.SummaryVacationColumns.Count == 0)
			{
				GlobalSettings.WBidINIContent.BidLineNormalColumns = new List<int>() { 36, 37, 27, 34, 12 };
				GlobalSettings.WBidINIContent.BidLineVacationColumns = new List<int>() { 36, 53, 200, 34, 12 };

				GlobalSettings.WBidINIContent.ModernNormalColumns = new List<int>() { 36, 37, 27, 34, 12 };
				GlobalSettings.WBidINIContent.ModernNormalColumns = new List<int>() { 36, 53, 200, 34, 12 };

				GlobalSettings.WBidINIContent.SummaryVacationColumns = WBidCollection.GenerateDefaultVacationColumns();
				GlobalSettings.WBidINIContent.Version = GlobalSettings.IniFileVersion;
				XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());


			}
			//remove the legs in 500 and legs in 300 columns
			GlobalSettings.WBidINIContent.SummaryVacationColumns.RemoveAll(x => x.Id == 58 || x.Id == 59);
			GlobalSettings.WBidINIContent.SummaryVacationColumns.RemoveAll(x => x.Id == 58 || x.Id == 59);

			GlobalSettings.WBidINIContent.ModernNormalColumns.RemoveAll(x => x == 58 || x == 59);
			GlobalSettings.WBidINIContent.ModernNormalColumns.RemoveAll(x => x == 58 || x == 59);

			GlobalSettings.WBidINIContent.BidLineNormalColumns.RemoveAll(x => x == 58 || x == 59);
			GlobalSettings.WBidINIContent.BidLineVacationColumns.RemoveAll(x => x == 58 || x == 59);

			GlobalSettings.WBidINIContent.DataColumns.RemoveAll(x => x.Id == 58 || x.Id == 59);



			if (GlobalSettings.WBidINIContent.AmPmConfigure == null)
			{

				GlobalSettings.WBidINIContent.AmPmConfigure = new AmPmConfigure()
				{
					HowCalcAmPm = 1,
					AmPush = TimeSpan.FromHours(4),
					AmLand = TimeSpan.FromHours(19),
					PmPush = TimeSpan.FromHours(11),
					PmLand = TimeSpan.FromHours(2),
					NitePush = TimeSpan.FromHours(22),
					NiteLand = TimeSpan.FromHours(7),
					NumberOrPercentageCalc = 1,
					NumOpposites = 3,
					PctOpposities = 20

				};
			}
			if (GlobalSettings.WBidINIContent.Version == null)
			{
				GlobalSettings.WBidINIContent.Version = "1.0";
				GlobalSettings.WBidINIContent.Updates = new INIUpdates
				{
					Trips = 0,
					News = 0,
					Cities = 0,
					Hotels = 0,
					Domiciles = 0,
					Equipment = 0,
					EquipTypes = 0,

				};
				XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
			}
			if (GlobalSettings.WBidINIContent.Data == null)
			{
				GlobalSettings.WBidINIContent.Data = new Data();
				GlobalSettings.WBidINIContent.Data.IsCompanyData = true;
			}
			if (GlobalSettings.WBidINIContent.User == null)
			{
				GlobalSettings.WBidINIContent.User = new User()
				{
					IsNeedBidReceipt = true,
					SmartSynch = false,
					AutoSave = false,
					IsNeedCrashMail = true
				};
			}
			GlobalSettings.UndoStack = new List<WBidState>();
			GlobalSettings.RedoStack = new List<WBidState>();

			if (decimal.Parse(GlobalSettings.WBidINIContent.Version ?? "0.0") < 2.1m)
			{
				if (!GlobalSettings.WBidINIContent.DataColumns.Any(x => x.Id == 66))
				{
					if (GlobalSettings.WBidINIContent.DataColumns.Count > 4)
					{
						for (int count = 4; count < GlobalSettings.WBidINIContent.DataColumns.Count; count++)
						{
							GlobalSettings.WBidINIContent.DataColumns[count].Order = GlobalSettings.WBidINIContent.DataColumns[count].Order + 1;
						}

						GlobalSettings.WBidINIContent.DataColumns.Insert(4, new DataColumn() { Id = 66, Order = 4, Width = 60 });
					}
				}
				if (!GlobalSettings.WBidINIContent.SummaryVacationColumns.Any(x => x.Id == 66))
				{
					if (GlobalSettings.WBidINIContent.SummaryVacationColumns.Count > 4)
					{
						for (int count = 4; count < GlobalSettings.WBidINIContent.SummaryVacationColumns.Count; count++)
						{
							GlobalSettings.WBidINIContent.SummaryVacationColumns[count].Order = GlobalSettings.WBidINIContent.SummaryVacationColumns[count].Order + 1;
						}

						GlobalSettings.WBidINIContent.SummaryVacationColumns.Insert(4, new DataColumn() { Id = 66, Order = 4, Width = 60 });
					}
				}
			}
			if (!GlobalSettings.WBidINIContent.Domiciles.Any(x => x.DomicileName == "AUS"))
			{
				GlobalSettings.WBidINIContent.Domiciles.Add(new Domicile { DomicileName = "AUS", DomicileId = 11, Code = "P", Number = 11 });
			}
			if (!GlobalSettings.WBidINIContent.Domiciles.Any(x => x.DomicileName == "FLL"))
			{
				GlobalSettings.WBidINIContent.Domiciles.Add(new Domicile { DomicileName = "FLL", DomicileId = 12, Code = "A", Number = 12 });
			}
			if (decimal.Parse(GlobalSettings.WBidINIContent.Version ?? "0.0") < 2.8m)
			{
				GlobalSettings.WBidINIContent.User.IsModernViewShade = true;
			}
			if (!GlobalSettings.WBidINIContent.Domiciles.Any(x => x.DomicileName == "LAX"))
			{
				GlobalSettings.WBidINIContent.Domiciles.Add(new Domicile { DomicileName = "LAX", DomicileId = 13, Code = "X", Number = 13 });
			}
			if (GlobalSettings.WBidINIContent.SenioritylistFormat.Count == 0)
			{
				GlobalSettings.WBidINIContent.SenioritylistFormat = WBidCollection.getDefaultSenlistFormatValue();
			}
			//
		}


		/// <summary>
		/// Copies the bundled XMLs to appdata.
		/// </summary>
		private static void copyBundledFileToAppdata(string fileName)
		{
			var sourcePath =
				Path.Combine(NSBundle.MainBundle.BundlePath,
					fileName);
			var destinationPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/WBidMax/" + fileName;
			try
			{
				//---copy only if file does not exist---
				if (!File.Exists(destinationPath))
				{
					File.Copy(sourcePath, destinationPath);

					if (File.Exists(destinationPath))
					{
						LoadColumnDefenitionData();
					}

				}
				else
				{
					LoadColumnDefenitionData();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				throw e;
			}

		}

		/// <summary>
		/// Load/Read the INI file data from the app data folder.or Create the INI file if the INI file is not present in the app data folder.
		/// </summary>
		private static void LoadINIFileData()
		{


			//cheCk the INI file is ceated or not.If not,create it.
			if (!File.Exists(WBidHelper.GetWBidINIFilePath()))
			{
				WBidINI wbidINI = WBidCollection.CreateINIFile();
				XmlHelper.SerializeToXml(wbidINI, WBidHelper.GetWBidINIFilePath());
			}


			//Read INI file 
			try
			{
				//read the values of the INI file.
				GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
			}
			catch
			{
				//if any error happened , again create  INI file
				WBidINI wbidINI = WBidCollection.CreateINIFile();
				XmlHelper.SerializeToXml(wbidINI, WBidHelper.GetWBidINIFilePath());
				GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
			}


			//Read DWC file
			if (!File.Exists(WBidHelper.GetWBidDWCFilePath()))
			{
				WBidIntialState WBidIntialState = WBidCollection.CreateDWCFile(GlobalSettings.DwcVersion);
				XmlHelper.SerializeToXml(WBidIntialState, WBidHelper.GetWBidDWCFilePath());
			}
			else
			{
				WBidIntialState wbidintialState = null;
				try
				{
					wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
				}
				catch (Exception ex)
				{
					wbidintialState = WBidCollection.CreateDWCFile(GlobalSettings.DwcVersion);
					XmlHelper.SerializeToXml(wbidintialState, WBidHelper.GetWBidDWCFilePath());
					//WBidHelper.LogDetails(GlobalSettings.WbidUserContent.UserInformation.EmpNo,"dwcRecreate","0","0");
					if (GlobalSettings.WbidUserContent == null)
					{
						if (File.Exists(WBidHelper.WBidUserFilePath))
						{
							GlobalSettings.WbidUserContent = (WbidUser)XmlHelper.DeserializeFromXmlForUserFile<WbidUser>(WBidHelper.WBidUserFilePath);
						}
					}

					try
					{
						WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
						obgWBidLogEvent.LogAllEvents(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "dwcRecreate", "0", "0", "");
					}
					catch
					{
					}

				}

				if (decimal.Parse(wbidintialState.Version) < 2.1m)
				{
					File.Delete(WBidHelper.GetWBidDWCFilePath());
					WBidIntialState WBidIntialState = WBidCollection.CreateDWCFile(GlobalSettings.DwcVersion);
					XmlHelper.SerializeToXml(WBidIntialState, WBidHelper.GetWBidDWCFilePath());
					// wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
				}
			}




			//            if ((decimal.Parse(GlobalSettings.WBidINIContent.Version??"0.0")) < decimal.Parse( GlobalSettings.IniFileVersion))
			if (GlobalSettings.WBidINIContent.BidLineVacationColumns == null || GlobalSettings.WBidINIContent.BidLineVacationColumns.Count == 0 || GlobalSettings.WBidINIContent.ModernVacationColumns == null || GlobalSettings.WBidINIContent.ModernVacationColumns.Count == 0 || GlobalSettings.WBidINIContent.SummaryVacationColumns == null || GlobalSettings.WBidINIContent.SummaryVacationColumns.Count == 0)
			{
				GlobalSettings.WBidINIContent.BidLineNormalColumns = new List<int>() { 36, 37, 27, 34, 12 };
				GlobalSettings.WBidINIContent.BidLineVacationColumns = new List<int>() { 36, 53, 200, 34, 12 };

				GlobalSettings.WBidINIContent.ModernNormalColumns = new List<int>() { 36, 37, 27, 34, 12 };
				GlobalSettings.WBidINIContent.ModernVacationColumns = new List<int>() { 36, 53, 200, 34, 12 };

				GlobalSettings.WBidINIContent.SummaryVacationColumns = WBidCollection.GenerateDefaultVacationColumns();
				GlobalSettings.WBidINIContent.Version = GlobalSettings.IniFileVersion;
				XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());


			}

			if (GlobalSettings.WBidINIContent.AmPmConfigure == null)
			{

				GlobalSettings.WBidINIContent.AmPmConfigure = new AmPmConfigure()
				{
					HowCalcAmPm = 1,
					AmPush = TimeSpan.FromHours(4),
					AmLand = TimeSpan.FromHours(19),
					PmPush = TimeSpan.FromHours(11),
					PmLand = TimeSpan.FromHours(2),
					NitePush = TimeSpan.FromHours(22),
					NiteLand = TimeSpan.FromHours(7),
					NumberOrPercentageCalc = 1,
					NumOpposites = 3,
					PctOpposities = 20

				};
			}
			if (GlobalSettings.WBidINIContent.Version == null)
			{
				GlobalSettings.WBidINIContent.Version = "1.0";
				GlobalSettings.WBidINIContent.Updates = new INIUpdates
				{
					Trips = 0,
					News = 0,
					Cities = 0,
					Hotels = 0,
					Domiciles = 0,
					Equipment = 0,
					EquipTypes = 0
				};
				XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
			}
			if (GlobalSettings.WBidINIContent.Data == null)
			{
				GlobalSettings.WBidINIContent.Data = new Data();
				GlobalSettings.WBidINIContent.Data.IsCompanyData = true;
			}
			if (GlobalSettings.WBidINIContent.User == null)
			{
				GlobalSettings.WBidINIContent.User = new User()
				{
					IsNeedBidReceipt = true,
					SmartSynch = false,
					AutoSave = false,
					IsNeedCrashMail = true
				};
			}
			GlobalSettings.UndoStack = new List<WBidState>();
			GlobalSettings.RedoStack = new List<WBidState>();

			if (decimal.Parse(GlobalSettings.WBidINIContent.Version ?? "0.0") < 2.1m)
			{
				if (!GlobalSettings.WBidINIContent.DataColumns.Any(x => x.Id == 66))
				{
					if (GlobalSettings.WBidINIContent.DataColumns.Count > 4)
					{
						for (int count = 4; count < GlobalSettings.WBidINIContent.DataColumns.Count; count++)
						{
							GlobalSettings.WBidINIContent.DataColumns[count].Order = GlobalSettings.WBidINIContent.DataColumns[count].Order + 1;
						}

						GlobalSettings.WBidINIContent.DataColumns.Insert(4, new DataColumn() { Id = 66, Order = 4, Width = 60 });
					}
				}
				if (!GlobalSettings.WBidINIContent.SummaryVacationColumns.Any(x => x.Id == 66))
				{
					if (GlobalSettings.WBidINIContent.SummaryVacationColumns.Count > 4)
					{
						for (int count = 4; count < GlobalSettings.WBidINIContent.SummaryVacationColumns.Count; count++)
						{
							GlobalSettings.WBidINIContent.SummaryVacationColumns[count].Order = GlobalSettings.WBidINIContent.SummaryVacationColumns[count].Order + 1;
						}

						GlobalSettings.WBidINIContent.SummaryVacationColumns.Insert(4, new DataColumn() { Id = 66, Order = 4, Width = 60 });
					}
				}
			}
			// var ss= System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
			// //BWICPS.TXT
			// SeniorityListParser ss1=new SeniorityListParser();
			//List<SeniorityListMember> list= ss1.ParseSeniorityListForFirstRoundPilot(WBidHelper.GetAppDataPath()+"/BWICPS.TXT","CP",2014,2);45
			//WBidHelper.SerializeObject(WBidHelper.GetAppDataPath()+"/BWICPS.SL", list);
			//  var sss=(List<SeniorityListMember>)WBidHelper.DeSerializeObject(WBidHelper.GetAppDataPath() + "/BWICPS.SL");

			// GlobalSettings.DownloadBidDetails.Year = 201;

			// GetExistingDataInAppData();
			// DownloadWBidFiles();
			//string filename = "PHXFA04M737";
			//LineInfo objineinfo = new LineInfo();
			//if (File.Exists(WBidHelper.GetAppDataPath() + "/" + filename + ".WBL"))
			//{
			//    Task taskA = Task.Run(() =>
			//    {
			//        using (FileStream linestream = File.OpenRead(WBidHelper.GetAppDataPath() + "/" + filename + ".WBL"))
			//        {
			//            //int a = 1;
			//            var result = ProtoSerailizer.DeSerializeObject(WBidHelper.GetAppDataPath() + "/" + filename + ".WBL", objineinfo, linestream);
			//            GlobalSettings.Lines = new ObservableCollection<Line>(result.Lines.Values);
			//        }
			//    });
			//}
			//else
			//{

			//    //}

			//    //Read Coulmn Defenition Data from XML file
			//    //           List<ColumnDefinition> columndefenition = (List<ColumnDefinition>)XmlHelper.DeserializeFromXml<ColumnDefinitions>(WBidHelper.GetWBidColumnDefinitionFilePath());
			//    //           List<DataColumn> datacolumn = GlobalSettings.WBidINIContent.DataColumns;
			//}

		}

		private static void LoadColumnDefenitionData()
		{
			GlobalSettings.columndefinition = (List<ColumnDefinition>)XmlHelper.DeserializeFromXml<ColumnDefinitions>(WBidHelper.GetWBidColumnDefinitionFilePath());
		}

		/// <summary>
		/// get the existing bid data infromation from the app data path.it checks whether WBP,WBS and WBL files.
		/// </summary>
		//private static List<RecentFile> GetExistingDataInAppData()
		//{

		//    //List<RecentFile> lstRecentFiles = new List<RecentFile>(){
		//    //    new RecentFile{Domcile="BWI",Position="CP",Round="2nd Round", Month=2,MonthDisplay="FEB",Year="2014"},
		//    //    new RecentFile{Domcile="BWI",Position="CP",Round="1st Round", Month=4, MonthDisplay="APR",Year="2014"},
		//    //    new RecentFile{Domcile="PHX",Position="CP",Round="1st Round", Month=4, MonthDisplay="APR",Year="2014"},
		//    //    new RecentFile{Domcile="PHX",Position="CP",Round="2nd Round", Month=4, MonthDisplay="APR",Year="2014"},
		//    //    new RecentFile{Domcile="BWI",Position="CP",Round="1st Round", Month=1,MonthDisplay="JAN",Year="2014"},
		//    //   new RecentFile{Domcile="BWI",Position="CP",Round="1st Round", Month=12,MonthDisplay="DEC",Year="2013"},
		//    //   new RecentFile{Domcile="BWI",Position="CP",Round="2nd Round", Month=4, MonthDisplay="APR",Year="2014"},
		//    //    new RecentFile{Domcile="BWI",Position="CP",Round="1st Round", Month=3,MonthDisplay="MAR",Year="2014"},
		//    //    new RecentFile{Domcile="BWI",Position="CP",Round="1st Round", Month=2,MonthDisplay="FEB",Year="2014"},
		//    //   new RecentFile{Domcile="BWI",Position="CP",Round="2nd Round", Month=1,MonthDisplay="JAN",Year="2014"},
		//    //  new RecentFile{Domcile="BWI",Position="CP",Round="2nd Round", Month=3, MonthDisplay="MAR",Year="2014"}

		//    //};
		//    //lstRecentFiles = lstRecentFiles.OrderByDescending(x => x.Year).ThenByDescending(y => y.Month).ThenByDescending(z => z.Round).ThenBy(a => a.Domcile).ToList();


		//    //string path = WBidHelper.GetAppDataPath();
		//    //List<RecentFile> lstRecentFiles = new RecentFiles();
		//    ////get all the  files in the root folder(look for wbl)
		//    //List<string> linefilenames = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).Select(Path.GetFileName)
		//    //    .Where(s => s.ToLower().EndsWith(".wbl")).ToList();
		//    //foreach (string filenames in linefilenames)
		//    //{

		//    //    string filename = filenames.Substring(0, filenames.Length - 3);
		//    //    // if (File.Exists(path + "/" + filenames + ".WBP") && File.Exists(path + "/" + filenames + ".WBS"))
		//    //    //temporary code.In future we need to check the WBS file also
		//    //    if (File.Exists(path + "/" + filename + "WBP"))
		//    //    {
		//    //        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
		//    //        RecentFile recentfile = new RecentFile();
		//    //        recentfile.Domcile = filename.Substring(0, 3);
		//    //        recentfile.Position = filename.Substring(3, 2);
		//    //        recentfile.Month = Convert.ToInt32(filename.Substring(5, 2));
		//    //        recentfile.MonthDisplay = mfi.GetMonthName(Convert.ToInt32(filename.Substring(5, 2))).Substring(0, 3).ToUpper();
		//    //        recentfile.Year = (Convert.ToInt16(filename.Substring(7, 2)) + 2000).ToString();
		//    //        recentfile.Round = (filename.Substring(9, 1) == "M") ? "1st Round" : "2nd Round";
		//    //        lstRecentFiles.Add(recentfile);
		//    //    }
		//    //}
		//    //lstRecentFiles = lstRecentFiles.OrderByDescending(x => x.Year).ThenByDescending(y => y.Month).ThenByDescending(z => z.Round).ThenBy(a => a.Domcile).ToList();
		//    //return lstRecentFiles;

		//}



		//private static bool DownloadWBidFiles()
		//{
		//    bool status = true;
		//    int previousnewsverion = 0;
		//    if (GlobalSettings.WBidINIContent != null && GlobalSettings.WBidINIContent.Updates != null)
		//    {
		//        previousnewsverion = GlobalSettings.WBidINIContent.Updates.News;
		//    }

		//    WBidHelper.GetAppDataPath();
		//    try
		//    {

		//        List<string> lstWBidFiles = new List<string>() { "WBUPDATE.DAT", "news.rtf", "trips.ttp", "falistwb4.dat" };

		//        var tasks = new Task[4];

		//        int count = 0;

		//        foreach (var bidFile in lstWBidFiles)
		//        {

		//            tasks[count] = Task.Factory.StartNew(() => DownloadBid.DownloadWBidFile(WBidHelper.GetAppDataPath(), bidFile));
		//            //DownloadBid.DownloadWBidFile(WBidHelper.GetAppDataPath(), bidFile);
		//            count++;
		//        }

		//        Task.WaitAll(tasks);


		//    }
		//    catch (Exception)
		//    {


		//    }
		//    return status;
		//}

		public override void OnActivated(UIApplication application)
		{
			application.ApplicationIconBadgeNumber = -1;
			if (Reachability.CheckVPSAvailable() && NSUserDefaults.StandardUserDefaults["isRegistered"] != null)
			{
				WBidPushSerivceClient client = null;
				BasicHttpBinding binding = ServiceUtils.CreateBasicHttp();
				client = new WBidPushSerivceClient(binding, ServiceUtils.PushEndPoint);
				client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 30);
				client.ResetBadgeAsync(new Guid(devID));
			}
		}

		public override void WillEnterForeground(UIApplication application)
		{
			//Check for App Update
			Self.GetAppStoreAppVersion();
		}

		public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
		{
			WBidPushSerivceClient client = null;
			BasicHttpBinding binding = ServiceUtils.CreateBasicHttp();
			client = new WBidPushSerivceClient(binding, ServiceUtils.PushEndPoint);
			client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 30);
			// client.RegistorDevicesCompleted += client_RegistorDevicesCompleted;
			client.RegisterDevicesForPushNotiifcationCompleted += Client_RegisterDevicesForPushNotiifcationCompleted;

			if (deviceToken != null)
			{
				byte[] result = new byte[deviceToken.Length];
				System.Runtime.InteropServices.Marshal.Copy(deviceToken.Bytes, result, 0, (int)deviceToken.Length);
				tok = BitConverter.ToString(result).Replace("-", "");
				//tok = deviceToken.ToString();
				//            tok = tok.Replace("<", "");
				//            tok = tok.Replace(">", "");
				//            tok = tok.Replace(" ", "");
				//            Console.WriteLine("Device Token: " + tok);

				PushDeviceDetails pushDetails = new PushDeviceDetails();
				pushDetails.BadgeCount = 0;
				pushDetails.DeviceTocken = tok;
				pushDetails.DeviceId = new Guid(devID);
				pushDetails.DeviceType = "iPad";
				pushDetails.EmpNo = int.Parse(GlobalSettings.WbidUserContent.UserInformation.EmpNo.Replace("e", ""));
				pushDetails.IsActive = true;
				pushDetails.FromAppNumber = 4;

				if (NSUserDefaults.StandardUserDefaults["Token"] == null && NSUserDefaults.StandardUserDefaults["isRegistered"] == null)
				{
					NSUserDefaults.StandardUserDefaults.SetString(tok, "Token");
					// Call reg service here
					//if (Reachability.IsHostReachable(GlobalSettings.ServerUrl))
					if (Reachability.CheckVPSAvailable())
					{
						//client.RegistorDevicesAsync(pushDetails);
						client.RegisterDevicesForPushNotiifcationAsync(pushDetails);

					}
				}
				else if (NSUserDefaults.StandardUserDefaults["Token"] != null && NSUserDefaults.StandardUserDefaults["isRegistered"] == null)
				{
					// Call reg service here 
					if (Reachability.CheckVPSAvailable())
					{
						//client.RegistorDevicesAsync(pushDetails);
						client.RegisterDevicesForPushNotiifcationAsync(pushDetails);
					}
				}
				else if (NSUserDefaults.StandardUserDefaults["isRegistered"] != null && NSUserDefaults.StandardUserDefaults["Token"].ToString() != tok)
				{
					// Call reg service here 
					if (Reachability.CheckVPSAvailable())
					{
						//client.RegistorDevicesAsync(pushDetails);
						client.RegisterDevicesForPushNotiifcationAsync(pushDetails);
					}
				}

				Console.WriteLine(pushDetails.DeviceId.ToString());
			}
		}

		private void Client_RegisterDevicesForPushNotiifcationCompleted(object sender, RegisterDevicesForPushNotiifcationCompletedEventArgs e)
		{
			if (e.Result > 0)
			{
				NSUserDefaults.StandardUserDefaults.SetString(tok, "Token");
				NSUserDefaults.StandardUserDefaults.SetString("YES", "isRegistered");
				NSUserDefaults.StandardUserDefaults.Synchronize();
			}
		}

		//     void client_RegistorDevicesCompleted (object sender, RegistorDevicesCompletedEventArgs e)
		//{
		//	if (e.Result > 0) {
		//		NSUserDefaults.StandardUserDefaults.SetString (tok, "Token");
		//		NSUserDefaults.StandardUserDefaults.SetString ("YES", "isRegistered");
		//		NSUserDefaults.StandardUserDefaults.Synchronize ();
		//	}
		//}

		public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
		{
			Console.WriteLine(error.ToString());
		}

		public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
		{
			NSObject dict = userInfo.ValueForKey(new NSString("aps"));
			dict = dict.ValueForKey(new NSString("alert"));

			UIWindow WindowAlert = new UIWindow(UIScreen.MainScreen.Bounds);
			WindowAlert.RootViewController = new UIViewController();
			UIAlertController okAlertController = UIAlertController.Create("WBidMax Notification", dict.ToString(), UIAlertControllerStyle.Alert);
			okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
			WindowAlert.MakeKeyAndVisible();
			WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
			WindowAlert.Dispose();

			application.ApplicationIconBadgeNumber = -1;
			if (Reachability.CheckVPSAvailable() && NSUserDefaults.StandardUserDefaults["isRegistered"] != null)
			{
				WBidPushSerivceClient client = null;
				BasicHttpBinding binding = ServiceUtils.CreateBasicHttp();
				client = new WBidPushSerivceClient(binding, ServiceUtils.PushEndPoint);
				client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 30);
				//client.ResetBadgeAsync (new Guid (devID));
				ResetBadgeParam resetbadge = new ResetBadgeParam();
				resetbadge.deviceId = new Guid(devID);
				resetbadge.FromAppNumber = 4;
				client.ResetPushBadgeAsync(resetbadge);
			}
		}

		private void AppStateSave()
		{
			if (CommonClass.lineVC != null && GlobalSettings.isModified == true)
			{
				StateManagement stateManagement = new StateManagement();
				stateManagement.UpdateWBidStateContent();
				GlobalSettings.WBidStateCollection.IsModified = true;
				WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);

				//save the state of the INI File
				WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());

				GlobalSettings.isModified = false;

				CommonClass.lineVC.UpdateSaveButton();

				if (CommonClass.cswVC != null)
					CommonClass.cswVC.UpdateSaveButton();
			}
		}

		public override void OnResignActivation(UIApplication application)
		{
			AppStateSave();

		}

		public override void WillTerminate(UIApplication application)
		{
			AppStateSave();
		}

		public override void DidRegisterUserNotificationSettings(UIApplication application, UIUserNotificationSettings notificationSettings)
		{

		}

		

       
    }


	public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
	{

        public string devID;
        #region Override Methods
        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
		{


			NSObject dict = response.Notification.Request.Content.UserInfo.ValueForKey(new NSString("aps"));
			dict = dict.ValueForKey(new NSString("alert"));
            UIWindow WindowAlert = new UIWindow(UIScreen.MainScreen.Bounds);
            WindowAlert.RootViewController = new UIViewController();
            UIAlertController okAlertController = UIAlertController.Create("WBidMax Notification", dict.ToString(), UIAlertControllerStyle.Alert);
            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            WindowAlert.MakeKeyAndVisible();
            WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
            WindowAlert.Dispose(); 

            UIApplication.SharedApplication.ApplicationIconBadgeNumber = -1;
           
            if (Reachability.CheckVPSAvailable() && NSUserDefaults.StandardUserDefaults["isRegistered"] != null)
			{
				WBidPushSerivceClient client = null;
				BasicHttpBinding binding = ServiceUtils.CreateBasicHttp();
				client = new WBidPushSerivceClient(binding, ServiceUtils.PushEndPoint);
				client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 30);
				ResetBadgeParam resetbadge = new ResetBadgeParam();
				resetbadge.deviceId = new Guid(AppDelegate.Self.devID);
				resetbadge.FromAppNumber = 4;
				client.ResetPushBadgeAsync(resetbadge);
				//client.ResetBadgeAsync(new Guid(AppDelegate.Self.devID));
			}
            // Inform caller it has been handled
			completionHandler();
		}


		public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
		{
		completionHandler(UNNotificationPresentationOptions.Alert);
			
		}
		#endregion
	}


	}

