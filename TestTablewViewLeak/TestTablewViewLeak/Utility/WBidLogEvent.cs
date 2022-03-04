using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using WBidDataDownloadAuthorizationService.Model;
using System.ServiceModel;
using SystemConfiguration;
using System.IO;
using System.Text.RegularExpressions;

namespace WBid.WBidiPad.iOS.Utility
{
   public  class WBidLogEvent
    {
       WBidDataDwonloadAuthServiceClient DwonloadAuthServiceClient;

//        public void LogBidSubmitDetails(SubmitBid submitBid, string employeeNumber)
//        {
//            try
//            {
//				
//					
//
//               // DwonloadAuthServiceClient = new WBidDataDwonloadAuthServiceClient("BasicHttpBinding_IWBidDataDwonloadAuthServiceForNormalTimout");
//                BasicHttpBinding binding = ServiceUtils.CreateBasicHttpForOneminuteTimeOut();
//                DwonloadAuthServiceClient = new WBidDataDwonloadAuthServiceClient(binding, ServiceUtils.EndPoint);
//                SubmitBidModel submitBidModel = new SubmitBidModel();
//                submitBid.Buddy1 = submitBid.Buddy1 ?? "0";
//                submitBid.Buddy2 = submitBid.Buddy2 ?? "0";
//                submitBidModel.Base = GlobalSettings.CurrentBidDetails.Domicile;
//                submitBidModel.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
//                submitBidModel.Month = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).ToString("MMM").ToUpper();
//                submitBidModel.Position = GlobalSettings.CurrentBidDetails.Postion;
//                submitBidModel.OperatingSystemNum = UIDevice.CurrentDevice.SystemVersion;
//                submitBidModel.PlatformNumber = "iPad";
//                submitBidModel.EmployeeNumber = int.Parse(GlobalSettings.WbidUserContent.UserInformation.EmpNo.Replace("e", "").Replace("E", ""));
//                submitBidModel.VersionNumber = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
//                submitBidModel.Event = "submitBid";
//                submitBidModel.Message = "Submit Bid";
//                submitBidModel.BidForEmpNum = int.Parse(employeeNumber.Replace("e", "").Replace("E", ""));
//                submitBidModel.BuddyBid1 = int.Parse(submitBid.Buddy1.Replace("e", "").Replace("E", ""));
//                submitBidModel.BuddyBid2 = int.Parse(submitBid.Buddy2.Replace("e", "").Replace("E", ""));
//
//
//				string wifiSSID=WBidHelper.CurrentSSID();
//				if ((wifiSSID.ToLower() != "southwestwifi")) 
//				{
//                DwonloadAuthServiceClient.SubmitBidDetailsCompleted += DwonloadAuthServiceClient_SubmitBidDetailsCompleted;
//                DwonloadAuthServiceClient.SubmitBidDetailsAsync(submitBidModel);
//				}
//				else
//				{
////					if(GlobalSettings.OfflineEvents==null)
////					   GlobalSettings.OfflineEvents=new OfflineEvents();
////					
////					if(GlobalSettings.OfflineEvents.EventLogs==null)
////						GlobalSettings.OfflineEvents.EventLogs=new List<LogData>();
////						
////					GlobalSettings.OfflineEvents.SubmitBidLogs.Add(new SubmitBidLog{date=DateTime.Now,SubmitBid= submitBidModel});
////							
////
////					WBidHelper.SaveOfflineEventFile(GlobalSettings.OfflineEvents, WBidHelper.WBidOfflineEventFilePath);				
//				}
//                
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//
//            }
//
//        }
		public void LogAllEvents(string employeeNumber, string eventName, string buddy1, string buddy2,string swaMessage)
		{
			try {


				string buddy3=string.Empty;
				WBidDataDwonloadAuthServiceClient client;
				BasicHttpBinding binding = ServiceUtils.CreateBasicHttp ();
				client = new WBidDataDwonloadAuthServiceClient (binding, ServiceUtils.EndPoint);
				client.InnerChannel.OperationTimeout = new TimeSpan (0, 0, 30);
				client.LogOperationCompleted += Client_LogOperationCompleted;


				string baseStr = GlobalSettings.WbidUserContent.UserInformation.Domicile;
				string roundStr = "M";
				string monthStr = new DateTime (DateTime.Now.Year, DateTime.Now.Month, 1).ToString ("MMM").ToUpper ();
				string positionStr = GlobalSettings.WbidUserContent.UserInformation.Position;

				if (GlobalSettings.CurrentBidDetails != null) {
					baseStr = GlobalSettings.CurrentBidDetails.Domicile;
					roundStr = GlobalSettings.CurrentBidDetails.Round;
					monthStr = new DateTime (GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).ToString ("MMM").ToUpper ();
					positionStr = GlobalSettings.CurrentBidDetails.Postion;

				}



				WBidDataDownloadAuthorizationService.Model.LogDetails logDetails = new WBidDataDownloadAuthorizationService.Model.LogDetails ();
				buddy1 = buddy1 ?? "0";
				buddy2 = buddy2 ?? "0";
				buddy3="0";
				if(eventName=="submitBid" && GlobalSettings.CurrentBidDetails.Postion=="FO")
				{
					buddy1=GlobalSettings.SubmitBid.Pilot1??"0";
					buddy2=GlobalSettings.SubmitBid.Pilot2??"0";
					buddy3=GlobalSettings.SubmitBid.Pilot3??"0";

				}
				logDetails.Base = baseStr;
				logDetails.Round = (roundStr == "M") ? 1 : 2;
				logDetails.Month = monthStr;
				logDetails.Position = positionStr;
				logDetails.OperatingSystemNum = UIDevice.CurrentDevice.SystemVersion;
				;
				logDetails.PlatformNumber = "iPad";
				logDetails.EmployeeNumber = int.Parse (GlobalSettings.WbidUserContent.UserInformation.EmpNo.Replace ("e", "").Replace ("E", ""));
				logDetails.VersionNumber = System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
				if (eventName == "RequestFailed")
				{
					logDetails.Event = "inAppPmntFail";
					logDetails.Message = "Request Failed";
				}
				else if (eventName == "MissingEmpNumReceipt")
				{
					logDetails.Event = eventName;
					logDetails.Message = "Missing EmpNum in Bid Receipt";
				}
				else
				{
					logDetails.Event = eventName;
					logDetails.Message = eventName;
				}
				logDetails.BidForEmpNum = int.Parse (employeeNumber.Replace ("e", "").Replace ("E", ""));
				logDetails.BuddyBid1 = int.Parse (buddy1.Replace ("e", "").Replace ("E", ""));
				logDetails.BuddyBid2 = int.Parse (buddy2.Replace ("e", "").Replace ("E", ""));
				logDetails.BuddyBid3 = int.Parse (buddy3.Replace ("e", "").Replace ("E", ""));

				logDetails.SWAMessage = swaMessage;



				if (GlobalSettings.WBidINIContent.User.IsSouthWestWifiTest == false) {
                    if (Reachability.CheckVPSAvailable()) {


						client.LogOperationAsync (logDetails);
					} else {
						AddOfflineEvents (employeeNumber, eventName,  buddy1,  buddy2);
						
					}
				} else {
					AddOfflineEvents (employeeNumber, eventName,  buddy1,  buddy2);

				}

			} catch (Exception ex) {
				throw ex;

			}

		}

		static void AddOfflineEvents (string employeeNumber, string eventName,  string buddy1,  string buddy2)
		{
			string baseStr = GlobalSettings.WbidUserContent.UserInformation.Domicile;
			string roundStr = "M";
			string monthStr = new DateTime (DateTime.Now.Year, DateTime.Now.Month, 1).ToString ("MMM").ToUpper ();
			string positionStr = GlobalSettings.WbidUserContent.UserInformation.Position;

			if (GlobalSettings.OfflineEvents == null)
				GlobalSettings.OfflineEvents = new OfflineEvents ();
			if (GlobalSettings.OfflineEvents.EventLogs == null)
				GlobalSettings.OfflineEvents.EventLogs = new List<LogDatas> ();
			WBid.WBidiPad.Model.LogDetails logdetails = new WBid.WBidiPad.Model.LogDetails ();
			buddy1 = buddy1 ?? "0";
			buddy2 = buddy2 ?? "0";
			logdetails.Base = baseStr;
			logdetails.Round = (roundStr == "M") ? 1 : 2;
			logdetails.Month = monthStr;
			logdetails.Position = positionStr;
			logdetails.OperatingSystemNum = UIDevice.CurrentDevice.SystemVersion;
			;
			logdetails.PlatformNumber = "iPad";
			logdetails.EmployeeNumber = int.Parse (GlobalSettings.WbidUserContent.UserInformation.EmpNo.Replace ("e", "").Replace ("E", ""));
			logdetails.VersionNumber = System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
			logdetails.Event = eventName;
			logdetails.Message = "SouthWestWifi " + eventName;
			logdetails.BidForEmpNum = int.Parse (employeeNumber.Replace ("e", "").Replace ("E", ""));
			logdetails.BuddyBid1 = int.Parse (buddy1.Replace ("e", "").Replace ("E", ""));
			logdetails.BuddyBid2 = int.Parse (buddy2.Replace ("e", "").Replace ("E", ""));
			GlobalSettings.OfflineEvents.EventLogs.Add (new LogDatas {
				date = DateTime.Now,
				LogDetails = logdetails
			});
			WBidHelper.SaveOfflineEventFile (GlobalSettings.OfflineEvents, WBidHelper.WBidOfflineEventFilePath);
		}

		 void Client_LogOperationCompleted (object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{

		}

        void DwonloadAuthServiceClient_SubmitBidDetailsCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
        }
		public void LogBadPasswordUsage(string userId, bool isDownload,string swamessage)
		{
			try
			{
				WBidDataDwonloadAuthServiceClient client;
				BasicHttpBinding binding = ServiceUtils.CreateBasicHttp();
				client = new WBidDataDwonloadAuthServiceClient(binding, ServiceUtils.EndPoint);
				WBidDataDownloadAuthorizationService.Model.LogDetails objLog = new WBidDataDownloadAuthorizationService.Model.LogDetails();

				if (isDownload)
				{
					if (GlobalSettings.DownloadBidDetails != null)
					{
						objLog.Base = GlobalSettings.DownloadBidDetails.Domicile;
						objLog.Round = (GlobalSettings.DownloadBidDetails.Round == "D") ? 1 : 2;
						objLog.Month = new DateTime(GlobalSettings.DownloadBidDetails.Year, GlobalSettings.DownloadBidDetails.Month, 1).ToString("MMM").ToUpper();
						objLog.Position = GlobalSettings.DownloadBidDetails.Postion;
					}
				}
				else
				{
					if (GlobalSettings.CurrentBidDetails != null)
					{
						objLog.Base = GlobalSettings.CurrentBidDetails.Domicile;
						objLog.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
						objLog.Month = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).ToString("MMM").ToUpper();
						objLog.Position = GlobalSettings.CurrentBidDetails.Postion;
					}
				}
				objLog.OperatingSystemNum = UIDevice.CurrentDevice.SystemVersion;
				objLog.PlatformNumber = "iPad";
				objLog.Event = "bad password";
				objLog.Message = "bad password";
				objLog.VersionNumber = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
				objLog.SWAMessage = swamessage;
				objLog.EmployeeNumber = Convert.ToInt32(Regex.Match(userId, @"\d+").Value);
				client.LogOperationAsync(objLog);
			}
			catch (Exception ex)
			{
				
			}
		}
//        public void LogTimeoutBidSubmitDetails(SubmitBid submitBid, string employeeNumber)
//        {
//            try
//            {
//
//
//                //DwonloadAuthServiceClient = new WBidDataDwonloadAuthServiceClient("BasicHttpBinding_IWBidDataDwonloadAuthServiceForNormalTimout");
//                BasicHttpBinding binding = ServiceUtils.CreateBasicHttpForOneminuteTimeOut();
//                DwonloadAuthServiceClient = new WBidDataDwonloadAuthServiceClient(binding, ServiceUtils.EndPoint);
//                SubmitBidModel submitBidModel = new SubmitBidModel();
//                submitBid.Buddy1 = submitBid.Buddy1 ?? "0";
//                submitBid.Buddy2 = submitBid.Buddy2 ?? "0";
//                submitBidModel.Base = GlobalSettings.CurrentBidDetails.Domicile;
//                submitBidModel.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
//                submitBidModel.Month = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).ToString("MMM").ToUpper();
//                submitBidModel.Position = GlobalSettings.CurrentBidDetails.Postion;
//                submitBidModel.OperatingSystemNum = UIDevice.CurrentDevice.SystemVersion;
//                submitBidModel.PlatformNumber = "iPad";
//                submitBidModel.EmployeeNumber = int.Parse(GlobalSettings.WbidUserContent.UserInformation.EmpNo.Replace("e", "").Replace("E", ""));
//                submitBidModel.VersionNumber = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
//                submitBidModel.Event = "bidSubmitTimeOut";
//                submitBidModel.Message = "bidSubmitTimeOut";
//                submitBidModel.BidForEmpNum = int.Parse(employeeNumber.Replace("e", "").Replace("E", ""));
//                submitBidModel.BuddyBid1 = int.Parse(submitBid.Buddy1.Replace("e", "").Replace("E", ""));
//                submitBidModel.BuddyBid2 = int.Parse(submitBid.Buddy2.Replace("e", "").Replace("E", ""));
//				string wifiSSID=WBidHelper.CurrentSSID();
//				if ((wifiSSID.ToLower() != "southwestwifi")) 
//				{
//                DwonloadAuthServiceClient.SubmitBidDetailsAsync(submitBidModel);
//				}
//				else
//					{
////						if(GlobalSettings.OfflineEvents==null)
////							GlobalSettings.OfflineEvents=new OfflineEvents();
////
////						if(GlobalSettings.OfflineEvents.SubmitBidLogs==null)
////							GlobalSettings.OfflineEvents.SubmitBidLogs=new List<SubmitBidLog>();
////
////						//GlobalSettings.OfflineEvents.SubmitBidLogs.Add(new SubmitBidLog{date=DateTime.Now,SubmitBid= submitBidModel});
////
////
////						WBidHelper.SaveOfflineEventFile(GlobalSettings.OfflineEvents, WBidHelper.WBidOfflineEventFilePath);				
//					}
//				}
//            catch (Exception ex)
//            {
//                throw ex;
//
//
//            }

        //}
    }
}