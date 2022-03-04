using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using System.IO;
using WBid.WBidiPad.SharedLibrary.Utility;
using System.Runtime.Serialization.Formatters.Binary;
//using WBid.WBidiPad.iOS.Common;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.SharedLibrary.Parser;
using System.ServiceModel;
using SystemConfiguration;
using WBid.WBidiPad.PortableLibrary;
using System.Globalization;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using System.Collections.ObjectModel;
using WBid.WBidiPad.Core.Enum;
using Newtonsoft.Json;
using Xamarin.Essentials;
using System.Text.RegularExpressions;
using WBid.WBidiPad.Model.SWA;

namespace WBid.WBidiPad.iOS.Utility
{
    public class WBidHelper
    {
        /// <summary>
        /// PURPOSE : Get App Data path
        /// </summary>
        /// <returns></returns>
        public static string GetAppDataPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/WBidMax";
        }

        /// <summary>
        /// PURPOSE : Get the path of the INI file
        /// </summary>
        /// <returns></returns>
        public static string GetWBidINIFilePath()
        {
            return GetAppDataPath() + "/WBidINI.XML";
        }

        public static string GetWBidDWCFilePath()
        {
            return GetAppDataPath() + "/WBidDWC.XML";
        }

        public static string WBidUserFilePath
        {
            get
            {
                return GetAppDataPath() + "/User.xml";
            }
        }
        public static string WBidOfflineEventFilePath
        {
            get
            {
                return GetAppDataPath() + "/OfflineEvents.xml";
            }
        }
        public static string WBidStateFilePath
        {
            get
            {
                return WBidHelper.GetAppDataPath() + "/" + GenerateFileNameUsingCurrentBidDetails() + ".WBS";
            }
        }
        public static string WBidUpdateFilePath
        {
            get
            {
                return WBidHelper.GetAppDataPath() + "/WBUPDATE.DAT";
            }
        }
        public static string HistoricalFilesInfoPath
        {
            get
            {
                return GetAppDataPath() + "/History" + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Year.ToString().PadLeft(2, '0') + ".HST";
            }
        }
        public static string GetQuickSetFilePath()
        {
            return GetAppDataPath() + "/WBidQuicksets.qs";
        }
        public static string MILFilePath
        {
            get
            {
                return GetAppDataPath() + "/" + GenerateFileNameUsingCurrentBidDetails() + ".MIL";
            }
        }
        public static string GetWBidOfflinePaymentFilePath()
        {
            return GetAppDataPath() + "/Payment.log";
        }
        /// <summary>
        /// Get the "Column defenition" path 
        /// </summary>
        /// <returns></returns>
        public static string GetWBidColumnDefinitionFilePath()
        {
            //return GetAppDataPath() + "/ColumnDefinitions.xml";
            return NSBundle.MainBundle.ResourcePath + "/ColumnDefinitions.xml";
        }
        /// <summary>
        /// create the directory to store the data
        /// </summary>
        public static void CreateAppDataDirectory()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var directoryname = Path.Combine(documents, "WBidMax");
            Directory.CreateDirectory(directoryname);
        }

        /// <summary>
        /// Serialize the parsed trip file and saved it into the root folder.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="objectToSerialize"></param>
        public static void SerializeObject(string filename, Object objectToSerialize)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memStream, objectToSerialize);
            byte[] byteArray = memStream.ToArray();
            FileStream fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write);
            fileStream.Write(byteArray.ToArray(), 0, byteArray.Length);
            fileStream.Close();
            memStream.Close();
            memStream.Dispose();
        }
        /// <summary>
        /// Derialize the file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Object DeSerializeObject(string filename)
        {
            object obj = new object();
            Stream stream = File.Open(filename, FileMode.Open);
            BinaryFormatter bFormatter = new BinaryFormatter();
            obj = (object)bFormatter.Deserialize(stream);
            stream.Close();
            return obj;
        }




        /// <summary>
        /// PURPOSE : Set current Bid details
        /// </summary>
        /// <param name="fileName"></param>
        public static void SetCurrentBidInformationfromZipFileName(string fileName, bool isHistoricalData)
        {
            try
            {

                string domicile = fileName.Substring(2, 3);
                string position = fileName.Substring(0, 1);
                position = (position == "C" ? "CP" : (position == "F" ? "FO" : "FA"));
                int month = int.Parse(fileName.Substring(5, 1), System.Globalization.NumberStyles.HexNumber);
                string round = fileName.Substring(1, 1) == "D" ? "M" : "S";
                int equipment = Convert.ToInt32(fileName.Substring(7, 3));
                int year = 0;
                if (isHistoricalData)
                    year = GlobalSettings.DownloadBidDetails.Year;
                else
                    year = GetZipFolderCreationTime(fileName.Replace(".737", ""), month);
                DateTime bpStartDay = CalculateBpStartDayWithYear(position, month, year);
                GlobalSettings.CurrentBidDetails = new BidDetails
                {
                    Domicile = domicile,
                    Postion = position,
                    Round = round,
                    Month = month,
                    Equipment = equipment,
                    BidPeriodStartDate = bpStartDay,
                    BidPeriodEndDate = CalculateBPEndDateWithYear(position, month, year),
                    Year = bpStartDay.Year,
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetApplicationBidData()
        {
            try

            {
                string currentbidDetails = string.Empty;
                string domicile = GlobalSettings.CurrentBidDetails.Domicile ?? string.Empty;
                string position = GlobalSettings.CurrentBidDetails.Postion ?? string.Empty;
                System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                string strMonthName = mfi.GetMonthName(GlobalSettings.CurrentBidDetails.Month).ToString();
                string round = GlobalSettings.CurrentBidDetails.Round == "M" ? "Monthly" : "2nd Round";
                currentbidDetails = domicile + "/" + position + "/" + " " + round + "  Line for " + strMonthName + " " + GlobalSettings.CurrentBidDetails.Year;

                var sb = new StringBuilder();
                if (GlobalSettings.WbidUserContent.UserInformation != null)
                {
                    sb.Append("<br/>" + "Base            :" + GlobalSettings.WbidUserContent.UserInformation.Domicile);
                    sb.Append("<br/>" + "Seat            :" + GlobalSettings.WbidUserContent.UserInformation.Position);
                    sb.Append("<br/>" + "Employee Number :" + GlobalSettings.WbidUserContent.UserInformation.EmpNo);
                    sb.Append("<br/>" + "App Email  :" + GlobalSettings.WbidUserContent.UserInformation.Email);
                }
                currentbidDetails = currentbidDetails + sb.ToString();
                return currentbidDetails;


            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// PURPOSE : Set current Bid details from State fileName,   
        /// </summary>
        /// <param name="fileName"></param>
        public static void SetCurrentBidInformationfromStateFileName(string fileName)
        {

            string domicile = fileName.Substring(0, 3);
            string position = fileName.Substring(3, 2);
            int month = Convert.ToInt32(fileName.Substring(5, 2));
            //string round = fileName.Substring(7, 1);
            //modified the file structure
            string round = fileName.Substring(9, 1);
            string linefilename = domicile + position + month.ToString("d2") + round + "737";
            int year = Convert.ToInt16(fileName.Substring(7, 2)) + 2000;
            DateTime bpStartDay = CalculateBpStartDayWithYear(position, month, year);

            GlobalSettings.CurrentBidDetails = new BidDetails
            {
                Domicile = domicile,
                Postion = position,
                Round = round,
                Month = month,
                BidPeriodStartDate = bpStartDay,
                BidPeriodEndDate = CalculateBPEndDateWithYear(position, month, year),
                Year = bpStartDay.Year,
            };
        }
        public static DateTime CalculateBpStartDayWithYear(string position, int month, int year)
        {
            DateTime startDay;
            if (position == "FA")
            {
                if (month == 2) startDay = new DateTime(year, month - 1, 31);                  // Jan 31
                else if (month == 3) startDay = new DateTime(year, month, 2);                  // Mar 2
                else startDay = new DateTime(year, month, 1);                                  // all other months, start day is the 1st
            }
            else
            {
                startDay = new DateTime(year, month, 1);
            }
            return startDay;
        }

        public static DateTime CalculateBPEndDateWithYear(string position, int month, int year)
        {
            DateTime endDay;
            int numberOfDays = DateTime.DaysInMonth(year, month);

            if (position == "FA")
            {
                if (month == 1) endDay = new DateTime(year, month, 30);
                else if (month == 2) endDay = new DateTime(year, month + 1, 1);                  // Mar 1
                else endDay = new DateTime(year, month, numberOfDays);
            }
            else
            {
                if (month == 1) endDay = new DateTime(year, month, 31);
                else endDay = new DateTime(year, month, numberOfDays);
            }
            return endDay;

        }

        private static int GetlineFileCreationTime(string linefilename, int month)
        {

            string lineFilename = Path.Combine(WBidHelper.GetAppDataPath(), linefilename + ".WBL");
            //get the file created time for the line file 
            DateTime filecreationTime = File.GetCreationTime(lineFilename);

            int year = 2013;
            //if the user donwloads the decemeber month data from the decemeber month ,we have to use the  file created year for the bid year.
            if (filecreationTime.Month == 12 && filecreationTime.Month == month)
                year = filecreationTime.Year;
            else
                //(Since the January month data is downloaded from the December month we need to add one month to the createddatetime of the line file to get the exact bid period year.)
                year = filecreationTime.AddMonths(1).Year;
            return year;
        }
        private static int GetZipFolderCreationTime(string zipfoldername, int month)
        {

            string zipFilename = Path.Combine(WBidHelper.GetAppDataPath(), zipfoldername);
            //get the folder created time for the zip file 
            DateTime foldercreationTime = Directory.GetCreationTime(zipFilename);

            int year = 2013;
            //if the user donwloads the decemeber month data from the decemeber month ,we have to use the  file created year for the bid year.
            if (foldercreationTime.Month == 12 && foldercreationTime.Month == month)
                year = foldercreationTime.Year;
            else
                //(Since the January month data is downloaded from the December month we need to add one month to the createddatetime of the line file to get the exact bid period year.)
                year = foldercreationTime.AddMonths(1).Year;
            return year;
        }

        /// <summary>
        /// PURPOSE : Generate Filename using current bid details
        /// </summary>
        /// <returns></returns>
        public static string GenerateFileNameUsingCurrentBidDetails()
        {
            return (GlobalSettings.CurrentBidDetails == null) ? string.Empty : GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + GlobalSettings.CurrentBidDetails.Month.ToString("d2") + (GlobalSettings.CurrentBidDetails.Year - 2000).ToString() + GlobalSettings.CurrentBidDetails.Round + "737";
        }

        /// <summary>
        /// save the state file to app data folder.
        /// </summary>
        /// <param name="stateFileName"></param>
        public static void SaveStateFile(string stateFileName)
        {
            GlobalSettings.WBidStateCollection.StateUpdatedTime = DateTime.Now.ToUniversalTime();
            XmlHelper.SerializeToXml(GlobalSettings.WBidStateCollection, stateFileName);
        }
        /// <summary>
        /// PURPOSE : Save INI File
        /// </summary>
        /// <param name="wBidINI"></param>
        /// <param name="fileName"></param>
        public static bool SaveINIFile(WBidINI wBidINI, string fileName)
        {
            try
            {
                return XmlHelper.SerializeToXml<WBidINI>(wBidINI, fileName);
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        ///  Save the user information(recent file) to the user.xml file
        /// </summary>
        /// <param name="wbidUser"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool SaveUserFile(WbidUser wbidUser, string fileName)
        {
            try
            {
                //		tested code for sharing path violation		if(wbidUser.UserInformation.UserAccountDateTime==DateTime.MinValue)
                //					wbidUser.UserInformation.UserAccountDateTime=wbidUser.UserInformation.UserAccountDateTime.AddYears(1);
                return XmlHelper.SerializeToXmlForUserFile<WbidUser>(wbidUser, fileName);
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        ///  Save the offline events such as submit log events 
        /// </summary>
        /// <param name="wbidUser"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool SaveOfflineEventFile(OfflineEvents offlineEvent, string fileName)
        {
            try
            {
                return XmlHelper.SerializeToXml<OfflineEvents>(offlineEvent, fileName);
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// PURPOSE : Read  file content ferom WBid Updated (WBUPDATE.DAT ) file
        /// </summary>
        /// <param name="Filename"></param>
        /// <returns></returns>
        public static WBidUpdate ReadValuesfromWBUpdateFile(string fileName)
        {

            WBidUpdateParser parser = new WBidUpdateParser();
            return parser.ParseWBidUpdateFile(fileName);


        }
        /// <summary>
        /// Read Coulmn Defenition Data from XML file
        /// </summary>
        /// <param name="Filename"></param>
        public static ColumnDefinitions ReadCoulumnDefenitionData(string Filename)
        {
            //Read Coulmn Defenition Data from XML file
            return XmlHelper.DeserializeFromXml<ColumnDefinitions>(Filename);
        }

        public static string GenarateZipFileName()
        {
            string filename = (GlobalSettings.CurrentBidDetails.Postion == "CP") ? "C" : (GlobalSettings.CurrentBidDetails.Postion == "FO") ? "F" : "A";
            filename += (GlobalSettings.CurrentBidDetails.Round == "M") ? "D" : "B";
            filename += GlobalSettings.CurrentBidDetails.Domicile;
            filename += GlobalSettings.CurrentBidDetails.Month.ToString("X");
            return filename;
        }


        public static void GenerateDynamicOverNightCitiesList()
        {
            GlobalSettings.OverNightCitiesInBid = new List<City>();
            foreach (Line line in GlobalSettings.Lines)
            {
                // bool isLastTrip = false; int paringCount = 0;
                Trip trip = null;
                DateTime tripDate = DateTime.MinValue;
                foreach (var pairing in line.Pairings)
                {                 //Get trip
                    trip = GetTrip(pairing);

                    // isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    // tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);

                    List<string> overNightCities = trip.DutyPeriods.Select(x => x.ArrStaLastLeg).Where(y => y.ToString() != GlobalSettings.CurrentBidDetails.Domicile).ToList();

                    foreach (string city in overNightCities)
                    {
                        if (!GlobalSettings.OverNightCitiesInBid.Any(x => x.Name == city))
                        {
                            var inicity = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Name == city);
                            if (inicity == null)
                            {
                                var cityid = GlobalSettings.WBidINIContent.Cities.Max(x => x.Id) + 1;
                                WBidMail mail = new WBidMail();
                                mail.SendMailtoAdmin("Below city is added into the INI file for this user .Check the Wbupdate file.+\nc City= " + city + "Id =" + cityid, GlobalSettings.WbidUserContent.UserInformation.Email, "New City Has been added to the INI File");

                                GlobalSettings.WBidINIContent.Cities.Add(new City { Id = cityid, Name = city, Code = 6 });
                                inicity = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Name == city);
                            }



                            GlobalSettings.OverNightCitiesInBid.Add(new City()
                            {
                                Name = city,
                                Id = inicity.Id
                            });
                        }

                    }

                }
            }

            GlobalSettings.OverNightCitiesInBid = GlobalSettings.OverNightCitiesInBid.OrderBy(x => x.Name).ToList();
        }

        private static Trip GetTrip(string pairing)
        {
            Trip trip = null;
            trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing.Substring(0, 4)).FirstOrDefault();
            if (trip == null)
            {
                trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing).FirstOrDefault();
            }

            return trip;

        }
        /// <summary>
        /// push the Current state file to the Undo  stack and clear the Redo statck
        /// </summary>
        public static void PushToUndoStack()
        {
            WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            if (GlobalSettings.UndoStack.Count == 99)
            {
                GlobalSettings.UndoStack.RemoveAt(98);
            }
            GlobalSettings.UndoStack.Insert(0, new WBidState(wBIdStateContent));
            GlobalSettings.RedoStack.Clear();
        }

        //		public static void LogDetails(string employeeNumber, string eventName, string buddy1, string buddy2)
        //		{  	try
        //			{
        //				
        //			WBidDataDwonloadAuthServiceClient client;
        //			BasicHttpBinding binding = ServiceUtils.CreateBasicHttp();
        //			client = new WBidDataDwonloadAuthServiceClient(binding, ServiceUtils.EndPoint);
        //			client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 30);
        //			client.LogOperationCompleted += Client_LogOperationCompleted;
        //
        //
        //					string baseStr = GlobalSettings.WbidUserContent.UserInformation.Domicile;
        //					string roundStr = "M";
        //					string monthStr = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("MMM").ToUpper();
        //					string positionStr = GlobalSettings.WbidUserContent.UserInformation.Position;
        //
        //					if (GlobalSettings.CurrentBidDetails != null)
        //					{
        //						baseStr = GlobalSettings.CurrentBidDetails.Domicile;
        //						roundStr = GlobalSettings.CurrentBidDetails.Round;
        //						monthStr = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).ToString("MMM").ToUpper();
        //						positionStr = GlobalSettings.CurrentBidDetails.Postion;
        //
        //					}
        //
        //
        //					//DwonloadAuthServiceClient = new WBidDataDwonloadAuthServiceClient("BasicHttpBinding_IWBidDataDwonloadAuthServiceForNormalTimout");
        //				WBidDataDownloadAuthorizationService.Model.LogDetails logDetails=new WBidDataDownloadAuthorizationService.Model.LogDetails();
        //					buddy1 = buddy1 ?? "0";
        //					buddy2 = buddy2 ?? "0";
        //
        //					logDetails.Base = baseStr;
        //					logDetails.Round = (roundStr == "M") ? 1 : 2;
        //					logDetails.Month = monthStr;
        //					logDetails.Position = positionStr;
        //				logDetails.OperatingSystemNum = UIDevice.CurrentDevice.SystemVersion;;
        //				logDetails.PlatformNumber = "iPad";
        //					logDetails.EmployeeNumber = int.Parse(GlobalSettings.WbidUserContent.UserInformation.EmpNo.Replace("e", "").Replace("E", ""));
        //				logDetails.VersionNumber =  System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        //					logDetails.Event = eventName;
        //					logDetails.Message = eventName;
        //					logDetails.BidForEmpNum = int.Parse(employeeNumber.Replace("e", "").Replace("E", ""));
        //					logDetails.BuddyBid1 = int.Parse(buddy1.Replace("e", "").Replace("E", ""));
        //					logDetails.BuddyBid2 = int.Parse(buddy2.Replace("e", "").Replace("E", ""));
        //				client.LogOperationAsync(logDetails);
        //
        //
        //			
        //
        //
        //			//client.LogOperationAsync(
        //					}
        //					catch (Exception ex)
        //					{
        //
        //
        //					}
        //		}

        static void Client_LogOperationCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

        }

        /// <summary>
        /// Gets the current SSID.
        /// </summary>
        /// <value>The current SSID.</value>
        public static string CurrentSSID()
        {

            NSDictionary dict;
            var status = CaptiveNetwork.TryCopyCurrentNetworkInfo("en0", out dict);
            if (dict != null)
            {
                if (status == StatusCode.NoKey)
                {
                    return string.Empty;
                }

                var bssid = dict[CaptiveNetwork.NetworkInfoKeyBSSID];
                var ssid = dict[CaptiveNetwork.NetworkInfoKeySSID];
                var ssiddata = dict[CaptiveNetwork.NetworkInfoKeySSIDData];

                return ssid.ToString();
            }
            else
                return string.Empty;

        }
        /// <summary>
        /// Return true if the current wifi is southWestWifi
        /// </summary>
        /// <returns><c>true</c> if is south west wifi; otherwise, <c>false</c>.</returns>
        public static bool IsSouthWestWifi()
        {
            if (CurrentSSID().ToLower() == "southwestwifi")
                return true;
            else
                return false;
        }
        public static bool IsSouthWestWifiOr2wire()
        {
            var currentwifi = CurrentSSID().ToLower();
            if (currentwifi == "southwestwifi" || currentwifi == "2wire" || GlobalSettings.WBidINIContent.User.IsSouthWestWifiTest == true)
                return true;
            else
                return false;
        }
        public static int ScreenWidth()
        {
            int width = 1024;
            float version;
            try
            {
                version = float.Parse(UIDevice.CurrentDevice.SystemVersion);
            }
            catch (Exception ex)
            {
                return width;
            }

            if (version < 9.4)
            {
                width = 1024;
            }
            else
            {
                width = 1370;
            }
            return width;
        }
        /// <summary> /// get the name of the flt equipment. flt string contains the value parsed from the trip records (parse 6 record) /// </summary> /// <param name="fltstring"></param> /// <returns></returns> 
        public static string GetEquipmentName(string fltstring)
        {
            string equip = string.Empty;
            if (GlobalSettings.CurrentBidDetails.Postion == "FA")
            {
                equip = fltstring.Substring(0, 3);
            }
            else
            {
                equip = (fltstring.Substring(1, 1) == "6") ? "MAX" : fltstring.Substring(1, 1) + "00";
            }
            return equip;
        }
        public static int ConvertHHMMToMinute(string hhmm)
        {

            int result = 0;

            if (hhmm == string.Empty || hhmm == null) return result;

            string[] split = hhmm.Split(':');
            result = int.Parse(split[0]) * 60 + int.Parse(split[1]);
            return result;

        }
        public static Dictionary<string, Trip> GetMissingtripFromVPS(MonthlyBidDetails bidDetails)
        {

            var jsonData = ServiceUtils.JsonSerializer(bidDetails);
            StreamReader dr = ServiceUtils.GetRestData("GetScrappedMissedTrips", jsonData);
            MissedTripResponseModel tripdata = WBidCollection.ConvertJSonStringToObject<MissedTripResponseModel>(dr.ReadToEnd());
            if (tripdata.JsonTripData == null || tripdata.Message == "No such missed trips available")
            {
                return new Dictionary<string, Trip>();
            }

            Dictionary<string, Trip> tripdatas = tripdata.JsonTripData.ToDictionary(x => x.TripNum, x => x);
            return tripdatas;


        }
        public static string GetAwardAlert(UserBidDetails userbiddetails)
        {
            string alert = string.Empty;

            string data = SmartSyncLogic.JsonObjectToStringSerializer<UserBidDetails>(userbiddetails);
            string url = GlobalSettings.DataDownloadAuthenticationUrl + "GetCurrentMonthAwardData";
            RestServiceUtil obj = new RestServiceUtil();
            string response = obj.PostData(url, data);
            AwardDetails awarddata = CommonClass.ConvertJSonToObject<AwardDetails>(response);

            string monthName = new DateTime(2010, userbiddetails.Month, 1).ToString("MMM", CultureInfo.InvariantCulture);
            if (awarddata.AwardedLine == 10000)//code for just to avoid alert
                return "";
            if (awarddata.AwardedLine != 0)
            {

                if (awarddata.IsPaperbid)
                {
                    alert = "You are a paper bid for the month and you were awarded line " + awarddata.AwardedLine;
                }
                else if (awarddata.ReserveLine)
                {
                    alert = "You were awarded line " + awarddata.AwardedLine + " for " + monthName + " " + userbiddetails.Year + " .\n\n";
                    alert += "Line " + awarddata.AwardedLine + " is a Reserve Line";
                }
                else if (awarddata.BlankLine)
                {
                    alert = "You were awarded line " + awarddata.AwardedLine + " for " + monthName + " " + userbiddetails.Year + " .\n\n";
                    alert += "Line " + awarddata.AwardedLine + " is a Blank Line";
                }
                else
                {
                    if (userbiddetails.Position == "FA")
                    {
                        //You were awarded line 214 B for Jan 2020.  You will be flying with Wonder Woman (22222) postion A and SuperMan (11111) position C
                        //You are a paper bid for the month and you were awarded line 176
                        //You were awarded line 213 for Jan 2020.  You will be flying with Capt Sky King (22028)".  Then a simple OK button.  When the OK button is pushed, the Awards list will display

                        alert = "You were awarded line " + awarddata.AwardedLine + " " + awarddata.Position + " for " + monthName + " " + userbiddetails.Year + " .";
                        if (awarddata.BuddyAwards.Count > 0)
                        {
                            alert += "\n\nYou will be flying with ";
                        }
                        foreach (var item in awarddata.BuddyAwards)
                        {
                            alert += item.BuddyName.TrimEnd() + " ( " + item.BuddyEmpNum + " ) position " + item.BuddyPosition + " and ";
                        }
                        if (alert.Length > 3 && alert.Substring(alert.Length - 4, 4) == "and ")
                        {
                            alert = alert.Substring(0, alert.Length - 4);
                        }
                    }
                    else if (userbiddetails.Position == "CP")
                    {
                        alert = "You were awarded line " + awarddata.AwardedLine + " " + awarddata.Position + " for " + monthName + " " + userbiddetails.Year + " .\n\n";
                        if (awarddata.BuddyAwards.Count > 0)
                        {
                            alert += "You will be flying with " + awarddata.BuddyAwards[0].BuddyName + " ( " + awarddata.BuddyAwards[0].BuddyEmpNum + " )";
                        }
                    }
                    else if (userbiddetails.Position == "FO")
                    {
                        alert = "You were awarded line " + awarddata.AwardedLine + " " + awarddata.Position + " for " + monthName + " " + userbiddetails.Year + " .\n\n";
                        if (awarddata.BuddyAwards.Count > 0)
                        {
                            alert += "You will be flying with Capt " + awarddata.BuddyAwards[0].BuddyName + " ( " + awarddata.BuddyAwards[0].BuddyEmpNum + " )";
                        }
                    }
                }
            }
            else
            {
                // We could not find any awarded line for 22020
                alert = "We could not find any awarded line for " + userbiddetails.EmployeeNumber;
            }
            return alert;
        }
        /// <summary>
        /// Retrive line file  from server(redownload data from server)
        /// </summary>
        /// <param name="wBIdStateContent"></param>
        public static string RedownloadBidDataVacFileFromServer(WBidState wBIdStateContent)
        {
            string status = string.Empty;
            try
            {
                string vacFileName = "";
                int faEomstartday = 0;
                if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.MenuBarButtonStatus.IsEOM)
                {

                    if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1) == GlobalSettings.FAEOMStartDate)
                        faEomstartday = 1;
                    else if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(2) == GlobalSettings.FAEOMStartDate)
                        faEomstartday = 2;
                    else if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(3) == GlobalSettings.FAEOMStartDate)
                        faEomstartday = 3;

                    vacFileName = GetRequiredLineFileName(faEomstartday);
                }
                else
                    vacFileName = GetRequiredLineFileName();

                if (vacFileName != "NoAppDataFileName")
                {
                    status = DownloadVacFilesFromServer(0, vacFileName, faEomstartday);
                }
                else
                {
                    // if the users.XML dont have Filenames available for the selected Bid.
                    try
                    {
                        Model.OldLine.LineInfo oldlineInfo = null;
                        var currentBid = GlobalSettings.CurrentBidDetails;
                        string filename = currentBid.Domicile + currentBid.Postion + currentBid.Month.ToString().PadLeft(2, '0') + (Convert.ToInt16(currentBid.Year) - 2000) + currentBid.Round + "737";
                        var newFileName = filename.Substring(0, 10);
                        using (FileStream lineStream = File.OpenRead(WBidHelper.GetAppDataPath() + "/" + filename + ".WBL"))
                        {
                            Model.OldLine.LineInfo ObjlineInfo = new Model.OldLine.LineInfo();
                            oldlineInfo = ProtoSerailizer.DeSerializeObject(WBidHelper.GetAppDataPath() + "/" + filename + ".WBL", ObjlineInfo, lineStream);

                        }
                        LineInfo lineInfo = new LineInfo();
                        lineInfo.Lines = new Dictionary<string, Line>();

                        foreach (var item in oldlineInfo.Lines.Values)
                        {
                            Line lst = new Line();
                            var parentProperties = item.GetType().GetProperties();


                            var childProperties = lst.GetType().GetProperties();
                            foreach (var parentProperty in parentProperties)
                            {

                                foreach (var childProperty in childProperties)
                                {
                                    if (parentProperty.Name == childProperty.Name && parentProperty.PropertyType == childProperty.PropertyType)
                                    {
                                        if (parentProperty.Name == "BlankLine")
                                        {
                                        }
                                        childProperty.SetValue(lst, parentProperty.GetValue(item));
                                        break;
                                    }
                                }
                            }
                            lineInfo.Lines.Add(lst.LineNum.ToString(), lst);
                        }


                        var jsonString = JsonConvert.SerializeObject(lineInfo);

                        //Lz compression
                        var compressedData = LZStringCSharp.LZString.CompressToUTF16(jsonString);

                        File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + newFileName + ".WBL", compressedData);

                        //desrialise the Json
                        LineInfo wblLine = WBidCollection.ConvertJSonStringToObject<LineInfo>(jsonString);



                        GlobalSettings.Lines = new ObservableCollection<Line>(wblLine.Lines.Values);

                        wBIdStateContent.MenuBarButtonState.IsVacationCorrection = false;
                        wBIdStateContent.MenuBarButtonState.IsEOM = false;
                        wBIdStateContent.MenuBarButtonState.IsVacationDrop = false;
                        wBIdStateContent.IsVacationOverlapOverlapCorrection = false;

                        GlobalSettings.WbidUserContent.AppDataBidFiles.Add(new AppDataBidFileNames
                        {
                            Domicile = currentBid.Domicile,
                            Month = currentBid.Month,
                            Position = currentBid.Postion,
                            Round = currentBid.Round,
                            Year = currentBid.Year,
                            lstBidFileNames = new List<BidFileNames>() { new BidFileNames { FileName = newFileName + ".WBL", FileType = (int)BidFileType.NormalLine } }
                        });

                        WBidHelper.SaveUserFile(GlobalSettings.WbidUserContent, WBidHelper.WBidUserFilePath);

                        File.Delete(filename);
                    }
                    catch (Exception ex1)
                    {
                        //throw ex1;
                        status = "Something Went Wrong";
                    }
                }
            }
            catch (Exception ex)
            {
                status = "Something Went Wrong";
            }
            return status;

        }
        private static void SaveTopLockandBottomLockToLineObject()
        {
            if (GlobalSettings.Lines != null)
            {

            }
        }
        /// <summary>
        /// Retrive line file  from local or from server
        /// </summary>
        /// <param name="isVacEomDrpBit"></param>
        public static string RetrieveSaveAndSetLineFiles(int isVacEomDrpBit, WBidState wBIdStateContent)
        {
            string status = string.Empty; 
            try
            {
                string vacFilePath = string.Empty;
                string vacFileName = "";
                int faEomstartday = 0;
                if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.MenuBarButtonStatus.IsEOM)
                {

                    if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1) == GlobalSettings.FAEOMStartDate)
                        faEomstartday = 1;
                    else if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(2) == GlobalSettings.FAEOMStartDate)
                        faEomstartday = 2;
                    else if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(3) == GlobalSettings.FAEOMStartDate)
                        faEomstartday = 3;

                    vacFileName = GetRequiredLineFileName(faEomstartday);
                }
                else
                    vacFileName = GetRequiredLineFileName();

                if (vacFileName != "NoAppDataFileName")
                {
                    vacFilePath = WBidHelper.GetAppDataPath() + "/" + vacFileName;

                    if (File.Exists(vacFilePath))
                    {

                        var compressedData = File.ReadAllText(vacFilePath);
                        string VAClinefileJsoncontent = LZStringCSharp.LZString.DecompressFromUTF16(compressedData);

                        //desrialise the Json
                        LineInfo wblLine = WBidCollection.ConvertJSonStringToObject<LineInfo>(VAClinefileJsoncontent);
                        List<Line> toplockllist = new List<Line>();
                        List<Line> bottomlockllist = new List<Line>();

                        if (GlobalSettings.Lines != null)
                        {
                            toplockllist = GlobalSettings.Lines.Where(x => x.TopLock).ToList();
                            bottomlockllist = GlobalSettings.Lines.Where(x => x.BotLock).ToList();
                        }
                        GlobalSettings.Lines = new ObservableCollection<Line>(wblLine.Lines.Values);

                        SetTopLoclAndBottomLock(toplockllist, bottomlockllist);
                        status = "Ok";
                        RecalculateAMPMAndWekProperties(false);
                        RecalcalculateLineProperties objrecalculate = new RecalcalculateLineProperties();
                        objrecalculate.CalculateDropTemplateForBidLines(GlobalSettings.Lines);
                    }
                    else
                    {
                        //checkBit
                        //1 For DRP
                        //2 For EOM
                        //3 For VAC
                        int chekBit = isVacEomDrpBit;
                        status = DownloadVacFilesFromServer(chekBit, vacFileName, faEomstartday);

                    }
                }
                else
                {
                    // if the users.XML dont have Filenames available for the selected Bid.
                    try
                    {
                        Model.OldLine.LineInfo oldlineInfo = null;
                        var currentBid = GlobalSettings.CurrentBidDetails;
                        string filename = currentBid.Domicile + currentBid.Postion + currentBid.Month.ToString().PadLeft(2, '0') + (Convert.ToInt16(currentBid.Year) - 2000) + currentBid.Round + "737";
                        var newFileName = filename.Substring(0, 10);
                        using (FileStream lineStream = File.OpenRead(WBidHelper.GetAppDataPath() + "/" + filename + ".WBL"))
                        {
                            Model.OldLine.LineInfo ObjlineInfo = new Model.OldLine.LineInfo();
                            oldlineInfo = ProtoSerailizer.DeSerializeObject(WBidHelper.GetAppDataPath() + "/" + filename + ".WBL", ObjlineInfo, lineStream);

                        }
                        LineInfo lineInfo = new LineInfo();
                        lineInfo.Lines = new Dictionary<string, Line>();

                        foreach (var item in oldlineInfo.Lines.Values)
                        {
                            Line lst = new Line();
                            var parentProperties = item.GetType().GetProperties();


                            var childProperties = lst.GetType().GetProperties();
                            foreach (var parentProperty in parentProperties)
                            {

                                foreach (var childProperty in childProperties)
                                {
                                    if (parentProperty.Name == childProperty.Name && parentProperty.PropertyType == childProperty.PropertyType)
                                    {
                                        if (parentProperty.Name == "BlankLine")
                                        {
                                        }
                                        childProperty.SetValue(lst, parentProperty.GetValue(item));
                                        break;
                                    }
                                }
                            }
                            lineInfo.Lines.Add(lst.LineNum.ToString(), lst);
                        }


                        var jsonString = JsonConvert.SerializeObject(lineInfo);

                        //Lz compression
                        var compressedData = LZStringCSharp.LZString.CompressToUTF16(jsonString);

                        File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + newFileName + ".WBL", compressedData);

                        //desrialise the Json
                        LineInfo wblLine = WBidCollection.ConvertJSonStringToObject<LineInfo>(jsonString);



                        GlobalSettings.Lines = new ObservableCollection<Line>(wblLine.Lines.Values);

                        wBIdStateContent.MenuBarButtonState.IsVacationCorrection = false;
                        wBIdStateContent.MenuBarButtonState.IsEOM = false;
                        wBIdStateContent.MenuBarButtonState.IsVacationDrop = false;
                        wBIdStateContent.IsVacationOverlapOverlapCorrection = false;

                        GlobalSettings.WbidUserContent.AppDataBidFiles.Add(new AppDataBidFileNames
                        {
                            Domicile = currentBid.Domicile,
                            Month = currentBid.Month,
                            Position = currentBid.Postion,
                            Round = currentBid.Round,
                            Year = currentBid.Year,
                            lstBidFileNames = new List<BidFileNames>() { new BidFileNames { FileName = newFileName + ".WBL", FileType = (int)BidFileType.NormalLine } }
                        });

                        WBidHelper.SaveUserFile(GlobalSettings.WbidUserContent, WBidHelper.WBidUserFilePath);

                        File.Delete(filename);
                    }
                    catch (Exception ex1)
                    {
                        //throw ex1;
                        status = "Something Went Wrong";
                        AddDetailsToMailCrashLogs(ex1);
                    }
                }
            }
            catch (Exception ex)
            {
                status = "Something Went Wrong";
                AddDetailsToMailCrashLogs(ex);
            }
            return status;
        }
        /// <summary>
        /// Set Top and Bottom lock from vac, normal or emon line objects. This method is calling when user tick or untick VAC,DRp and EOM button click
        /// </summary>
        /// <param name="toplockllist"></param>
        /// <param name="bottomlockllist"></param>
        public static void SetTopLoclAndBottomLock(List<Line> toplockllist, List<Line> bottomlockllist)
        {
            if (toplockllist.Count > 0)
            {

                var toplockedlines = GlobalSettings.Lines.Where(x => toplockllist.Any(y => y.LineNum == x.LineNum)).OrderBy(z => toplockllist.Select(s => s.LineNum).ToList().IndexOf(z.LineNum)).ToList();
                foreach (var item in toplockedlines)
                {
                    if (GlobalSettings.Lines.Contains(item))
                    {
                        GlobalSettings.Lines.Remove(item);
                    }

                }
                toplockedlines.Reverse();

                foreach (var item in toplockedlines)
                {
                    GlobalSettings.Lines.Insert(0, item);
                }

                toplockedlines.ForEach(x => x.TopLock = true);
            }
            if (bottomlockllist.Count > 0)
            {

                var bottomlockedlines = GlobalSettings.Lines.Where(x => bottomlockllist.Any(y => y.LineNum == x.LineNum)).OrderBy(z => bottomlockllist.Select(s => s.LineNum).ToList().IndexOf(z.LineNum)).ToList();
                foreach (var item in bottomlockedlines)
                {
                    if (GlobalSettings.Lines.Contains(item))
                    {
                        GlobalSettings.Lines.Remove(item);
                    }

                }


                foreach (var item in bottomlockedlines)
                {
                    GlobalSettings.Lines.Insert(GlobalSettings.Lines.Count, item);
                }

                bottomlockedlines.ForEach(x => x.BotLock = true);
            }
        }

        public static string GetCurrentlyLoadedLinefileName()
        {

            string filename = string.Empty;
            if (GlobalSettings.CurrentBidDetails != null)
            {
                int faEomstartday = 0;
                if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.MenuBarButtonStatus.IsEOM)
                {

                    if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1) == GlobalSettings.FAEOMStartDate)
                        faEomstartday = 1;
                    else if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(2) == GlobalSettings.FAEOMStartDate)
                        faEomstartday = 2;
                    else if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(3) == GlobalSettings.FAEOMStartDate)
                        faEomstartday = 3;
                    filename = GetRequiredLineFileName(faEomstartday);
                }
                else
                {
                    filename = GetRequiredLineFileName();
                }
            }
            return filename;
        }
        /// <summary>
        /// Getting Line file name as per  EOM,VAC,DRP button click
        /// </summary>
        /// <param name="FAEOMStartDate">if FA Bid ,we have FA positions  like 1,2,3 </param>
        /// <returns></returns>
        private static string GetRequiredLineFileName(int FAEOMStartDate = 0)
        {
            try
            {
                var currentbiddetail = GlobalSettings.CurrentBidDetails;
                AppDataBidFileNames appDataBidFileNames = GlobalSettings.WbidUserContent.AppDataBidFiles.FirstOrDefault(x => x.Domicile == currentbiddetail.Domicile && x.Position == currentbiddetail.Postion && x.Round == currentbiddetail.Round && x.Month == currentbiddetail.Month && x.Year == currentbiddetail.Year);


                //List<string> lstBidFiles = new List<string>();
                //lstBidFiles = GlobalSettings.WBidStateCollection.DownlaodedbidFiles;

                string lineFileName = string.Empty;

                //string vacFileName = lstBidFiles.FirstOrDefault(x => x.Contains(".WBV"));
                //if (vacFileName != null && vacFileName.Length > 3)
                //    vacFileName = vacFileName.Split('.')[0];

                //string normalinefile = lstBidFiles.FirstOrDefault(x => x.Contains(".WBL"));
                //if (normalinefile != null && normalinefile.Length > 3)
                //    normalinefile = normalinefile.Split('.')[0];

                if (appDataBidFileNames != null)
                {
                    MenuBarButtonStatus MenuButton = GlobalSettings.MenuBarButtonStatus;

                    if (!MenuButton.IsVacationCorrection && !MenuButton.IsVacationDrop && !MenuButton.IsEOM)
                    {
                        //No vacation-Set WBL file name
                        BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.NormalLine);
                        if (filename != null)
                            lineFileName = filename.FileName;
                        // lineFileName = normalinefile + ".WBL";
                    }
                    if (MenuButton.IsVacationCorrection && MenuButton.IsVacationDrop && !MenuButton.IsEOM)
                    {
                        //VAC+DRP
                        BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.Vacation);
                        if (filename != null)
                            lineFileName = filename.FileName;
                        //lineFileName = vacFileName + ".WBV";
                    }
                    if (MenuButton.IsVacationCorrection && !MenuButton.IsVacationDrop && !MenuButton.IsEOM)
                    {
                        //VAC
                        BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.VacationDropOFF);
                        if (filename != null)
                            lineFileName = filename.FileName;
                        //lineFileName = vacFileName + ".DRP";
                    }
                    if (MenuButton.IsVacationCorrection && MenuButton.IsVacationDrop && MenuButton.IsEOM)
                    {
                        //VAC+DRP+EOM
                        if (GlobalSettings.CurrentBidDetails.Postion != "FA")
                        {
                            BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.VacationEOM);
                            if (filename != null)
                                lineFileName = filename.FileName;
                        }
                        else
                        {
                            BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileName.Contains("F" + FAEOMStartDate + ".WBE") && x.FileType == (int)BidFileType.VacationEOM);
                            if (filename != null)
                                lineFileName = filename.FileName;
                        }
                        //    lineFileName = vacFileName + "F.WBE";
                        //else
                        //    lineFileName = vacFileName + "F" + FAEOMStartDate + ".WBE";
                    }
                    if (!MenuButton.IsVacationCorrection && !MenuButton.IsVacationDrop && MenuButton.IsEOM)
                    {
                        //EOM
                        if (GlobalSettings.CurrentBidDetails.Postion != "FA")
                        {
                            BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.EomDropOFF);
                            if (filename != null)
                                lineFileName = filename.FileName;
                        }
                        else
                        {
                            BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileName.Contains("F" + FAEOMStartDate + ".DRP") && x.FileType == (int)BidFileType.EomDropOFF);
                            if (filename != null)
                                lineFileName = filename.FileName;
                        }
                        //    if (GlobalSettings.CurrentBidDetails.Postion != "FA")
                        //    lineFileName = vacFileName + "F.DRP";
                        //else
                        //    lineFileName = vacFileName + "F" + FAEOMStartDate + ".DRP";
                    }
                    if (!MenuButton.IsVacationCorrection && MenuButton.IsVacationDrop && MenuButton.IsEOM)
                    {
                        //EOM+DRP

                        if (GlobalSettings.CurrentBidDetails.Postion != "FA")
                        {
                            BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.Eom);
                            if (filename != null)
                                lineFileName = filename.FileName;
                        }
                        else
                        {
                            BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileName.Contains("F" + FAEOMStartDate + ".WBE") && x.FileType == (int)BidFileType.Eom);
                            if (filename != null)
                                lineFileName = filename.FileName;
                        }
                        //if (GlobalSettings.CurrentBidDetails.Postion != "FA")
                        //    lineFileName = normalinefile + "F.WBE";
                        //else
                        //    lineFileName = normalinefile + "F" + FAEOMStartDate + ".WBE";
                    }
                    if (MenuButton.IsVacationCorrection && !MenuButton.IsVacationDrop && MenuButton.IsEOM)
                    {

                        //EOM+VAC
                        if (GlobalSettings.CurrentBidDetails.Postion != "FA")
                        {
                            BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.VacationEomDropOFF);
                            if (filename != null)
                                lineFileName = filename.FileName;
                        }
                        else
                        {
                            BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileName.Contains("F" + FAEOMStartDate + ".DRP") && x.FileType == (int)BidFileType.VacationEomDropOFF);
                            if (filename != null)
                                lineFileName = filename.FileName;
                        }
                    }

                }
                else
                {
                    lineFileName = "NoAppDataFileName";
                }
                return lineFileName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        /// <summary>
        /// Download VAC,EOM,DRP Files from server
        /// </summary>
        /// <param name="checkBit">for checking whether  EOM,DRP or VAC files to donwload</param>

        private static string DownloadVacFilesFromServer(int checkBit, string vacFileName, int faEomstartday)
        {
            string status = string.Empty;
            try
            {
                BidDataFileResponse biddataresponse = new BidDataFileResponse();
                BidDataRequestDTO bidDetails = new BidDataRequestDTO();
                VacFilesRequest vacRequest = new VacFilesRequest();
                bidDetails.EmpNum = int.Parse(GlobalSettings.WbidUserContent.UserInformation.EmpNo);
                bidDetails.Base = GlobalSettings.CurrentBidDetails.Domicile;
                bidDetails.Position = GlobalSettings.CurrentBidDetails.Postion;
                bidDetails.Month = GlobalSettings.CurrentBidDetails.Month;
                bidDetails.Year = GlobalSettings.CurrentBidDetails.Year;
                bidDetails.Round = GlobalSettings.CurrentBidDetails.Round;
                //bidDetails.Round = GlobalSettings.CurrentBidDetails.Round == "D" ? "M" : "S";

                vacRequest.bidDataRequest = bidDetails;
                //1 for DRP Files
                //2 For EOM files
                //3 for vac Files
                vacRequest.checkVACBit = checkBit;
                vacRequest.FAPositions = faEomstartday;
                vacRequest.vacFileName = vacFileName;

                try
                {
                    var jsonData = ServiceUtils.JsonSerializer(vacRequest);
                    StreamReader dr = ServiceUtils.GetRestData("GetVacFilesDRPEOM", jsonData);
                    biddataresponse = WBidCollection.ConvertJSonStringToObject<BidDataFileResponse>(dr.ReadToEnd());
                }
                catch (Exception ex)
                {
                    status = "Server Error";
                }
                if (biddataresponse.Status == true)
                {
                    //Show alert if the  data is not available
                    if (biddataresponse.bidData.Count > 0)
                    {
                        foreach (var item in biddataresponse.bidData)
                        {
                            List<Line> toplockllist = new List<Line>();
                            List<Line> bottomlockllist = new List<Line>();
                            //Decompress the string using LZ compress.
                            string linefileJsoncontent = LZStringCSharp.LZString.DecompressFromUTF16(item.FileContent);

                            File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + item.FileName, item.FileContent);

                            //desrialise the Json
                            LineInfo wblVACLine = WBidCollection.ConvertJSonStringToObject<LineInfo>(linefileJsoncontent);

                            if (GlobalSettings.Lines != null)
                            {
                                toplockllist = GlobalSettings.Lines.Where(x => x.TopLock).ToList();
                                bottomlockllist = GlobalSettings.Lines.Where(x => x.BotLock).ToList();
                            }
                            GlobalSettings.Lines = new ObservableCollection<Line>(wblVACLine.Lines.Values);
                            SetTopLoclAndBottomLock(toplockllist, bottomlockllist);
                            status = "Ok";
                            RecalculateAMPMAndWekProperties(false);
                            RecalcalculateLineProperties objrecalculate = new RecalcalculateLineProperties();
                            objrecalculate.CalculateDropTemplateForBidLines(GlobalSettings.Lines);
                        }
                    }
                }
                else
                {
                    status = biddataresponse.Message;
                    //show alert
                    //InvokeOnMainThread(() =>
                    //{
                    //    AlertController = UIAlertController.Create(biddataresponse.Message, "WBidMax", UIAlertControllerStyle.Alert);
                    //    AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) =>
                    //    {
                    //        DismissCurrentView();
                    //    }));

                    //    this.PresentViewController(AlertController, true, null);

                    //});
                }
            }
            catch (Exception ex)
            {
                status = "ErrorDownload";
            }
            return status;
        }
        public static void RecalculateAMPMAndWekProperties(bool isneedToSaveLineFile)
        {
            try
            {
                if (GlobalSettings.Lines != null && GlobalSettings.Trip != null)
                {
                    Dictionary<string, Line> lines = new Dictionary<string, Line>();
                    RecalcalculateLineProperties calculateLineProperties = new RecalcalculateLineProperties();
                    foreach (Line line in GlobalSettings.Lines)
                    {
                        calculateLineProperties.RecalculateAMPMPropertiesAfterAMPMDefenitionChanges(line);
                        calculateLineProperties.RecalculateWeekProperties(line);
                        lines.Add(line.LineNum.ToString(), line);
                    }

                    LineInfo lineInfo = new LineInfo()
                    {
                        LineVersion = GlobalSettings.LineVersion,
                        Lines = lines

                    };
                    if (isneedToSaveLineFile)
                    {
                        var filename = WBidHelper.GetCurrentlyLoadedLinefileName();
                        if (filename != string.Empty)
                        {
                            var jsonLineString = JsonConvert.SerializeObject(lineInfo);
                            File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + filename, jsonLineString);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
            }
        }

        public static BidDataFileResponse RedownloadBidDataFromServer()
        {
            //Get Bid File Name

            BidDetails bidDetails = GlobalSettings.CurrentBidDetails;
            if (GlobalSettings.WbidUserContent.AppDataBidFiles == null)
                GlobalSettings.WbidUserContent.AppDataBidFiles = new List<AppDataBidFileNames>();

            AppDataBidFileNames appDataBidFileNames = GlobalSettings.WbidUserContent.AppDataBidFiles.FirstOrDefault(x => x.Domicile == bidDetails.Domicile && x.Position == bidDetails.Postion && x.Round == bidDetails.Round && x.Month == bidDetails.Month && x.Year == bidDetails.Year);
            WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

            List<string> DownloadFiles = new List<string>();
            if (appDataBidFileNames != null)
            {
                DownloadFiles = GenerateRedownloadFileNames(appDataBidFileNames, DownloadFiles);

            }

            // Download multiple Bid file. Need to check multiple file downloaded service created in the service or not.
            var biddataresponse = new BidDataFileResponse();
            VacFilesRequest vacRequest = new VacFilesRequest();
            BidDataRequestDTO bidData = new BidDataRequestDTO
            {
                EmpNum = int.Parse(GlobalSettings.WbidUserContent.UserInformation.EmpNo),
                Base = bidDetails.Domicile,
                Position = bidDetails.Postion,
                Month = bidDetails.Month,
                Year = bidDetails.Year,
                Round = bidDetails.Round
            };


            vacRequest.bidDataRequest = bidData;
            vacRequest.BidFileNames = DownloadFiles;
            try
            {
                var jsonData = ServiceUtils.JsonSerializer(vacRequest);
                StreamReader dr = ServiceUtils.GetRestData("GetMultipleBidDataFiles", jsonData);
                biddataresponse = WBidCollection.ConvertJSonStringToObject<BidDataFileResponse>(dr.ReadToEnd());

                //set line and trip objects
                if (biddataresponse.bidData.Count > 0)
                {

                    foreach (var item in biddataresponse.bidData)
                    {
                        if (!item.IsError)
                        {
                            var fileExtension = item.FileName.Split('.')[1].ToString().ToLower();
                            if (fileExtension == "wbp")
                            {
                                //Decompress the string using LZ compress.
                                string tripfileJsoncontent = LZStringCSharp.LZString.DecompressFromUTF16(item.FileContent);

                                File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + item.FileName, item.FileContent);
                                //desrialise the Json
                                Dictionary<string, Trip> wbpLine = WBidCollection.ConvertJSonStringToObject<Dictionary<string, Trip>>(tripfileJsoncontent);
                                GlobalSettings.Trip = new ObservableCollection<Trip>(wbpLine.Values);
                            }
                            else
                            {
                                //Decompress the string using LZ compress.
                                string linefileJsoncontent = LZStringCSharp.LZString.DecompressFromUTF16(item.FileContent);

                                File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + item.FileName, item.FileContent); //desrialise the Json
                              //  break; Commented by Roshil on 2021 novmeber 04. Redownloading the WBL files not saving all the files.
                            }
                        }
                    }
                    string status = WBidHelper.RetrieveSaveAndSetLineFiles(3, wBIdStateContent);
                    if (status != "Ok")
                    {
                        biddataresponse.Status = false;
                    }

                    // Delete other files from local app data folder
                    IEnumerable<string> deletedFiles = new List<string>();
                    List<string> currentFiles = new List<string>();
                    foreach (var appDataFiLe in appDataBidFileNames.lstBidFileNames)
                    {
                        currentFiles.Add(appDataFiLe.FileName);
                    }
                    deletedFiles = currentFiles.Except(DownloadFiles);
                    foreach (var deleteFile in deletedFiles)
                    {
                        if (File.Exists(WBidHelper.GetAppDataPath() + "\\" + deleteFile))
                        {
                            File.Delete(WBidHelper.GetAppDataPath() + "\\" + deleteFile);

                        }
                    }
                }
                return biddataresponse;
            }
            catch (Exception ex)
            {
                AddDetailsToMailCrashLogs(ex);
                return biddataresponse;
            }


        }

        private static List<string> GenerateRedownloadFileNames(AppDataBidFileNames appDataBidFileNames, List<string> DownloadFiles)
        {
            var normalline = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.NormalLine);
            if (normalline != null)
                DownloadFiles.Add(normalline.FileName);
            var tripdata = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.Trip);
            if (tripdata != null)
                DownloadFiles.Add(tripdata.FileName);
            var vacfile = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.Vacation);
            if (vacfile != null)
                DownloadFiles.Add(vacfile.FileName);

            //Set currently used line file
            int faEomstartday = 0;
            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.MenuBarButtonStatus.IsEOM)
            {

                if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1) == GlobalSettings.FAEOMStartDate)
                    faEomstartday = 1;
                else if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(2) == GlobalSettings.FAEOMStartDate)
                    faEomstartday = 2;
                else if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(3) == GlobalSettings.FAEOMStartDate)
                    faEomstartday = 3;
            }

            var currentlyLoadedLineFile = GetRequiredLineFileName(faEomstartday);
            if (currentlyLoadedLineFile != null && !DownloadFiles.Contains(currentlyLoadedLineFile))
                DownloadFiles.Add(currentlyLoadedLineFile);

            return DownloadFiles;
        }

        public static BidDataFileResponse RedownloadBidDatFromServer(bool isHistorical, bool isFromMultipleBidDownload)
        {
            var biddataresponse = new BidDataFileResponse();

            var vacationlinesObject = new BidDataFiles();
            bool isVacationFileDownloaded = false;
            var _stateFileName = "";
            try
            {
                //IMplement the service
                //Set vacation,senlist and FV vacation from the service

                BidDataRequestDTO bidDetails = new BidDataRequestDTO();
                bidDetails.EmpNum = GlobalSettings.IsDifferentUser ? Convert.ToInt32(GlobalSettings.ModifiedEmployeeNumber) : Convert.ToInt32(Regex.Match(GlobalSettings.WbidUserContent.UserInformation.EmpNo, @"\d+").Value);
                bidDetails.Base = GlobalSettings.DownloadBidDetails.Domicile;
                bidDetails.Position = GlobalSettings.DownloadBidDetails.Postion;
                bidDetails.Month = GlobalSettings.DownloadBidDetails.Month;
                bidDetails.Year = GlobalSettings.DownloadBidDetails.Year;
                bidDetails.Round = GlobalSettings.DownloadBidDetails.Round == "D" ? "M" : "S";
                bidDetails.IsHistoricBid = isHistorical;


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

                if (biddataresponse.Status == true)
                {
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
                    }
                    //Need to check
                    //if (!isFromMultipleBidDownload)
                    //{
                    //    ////SeniorityList
                    //    _seniorityListItem.SeniorityNumber = biddataresponse.DomcileSeniority;
                    //    _seniorityListItem.TotalCount = biddataresponse.TotalSenliorityMember;
                    //    if (biddataresponse.ISEBGUser)
                    //        _seniorityListItem.EBgType = "Y";
                    //    _paperCount = biddataresponse.paperCount;
                    //    _isInSeriority = biddataresponse.IsSeniorityExist;

                    //    ////Vacation
                    //    if (biddataresponse.Vacation.Count > 0)
                    //        _vacation = biddataresponse.Vacation;

                    //    ////FV Vacation
                    //    if (biddataresponse.FVVacation.Count > 0)
                    //        _fVVacation = biddataresponse.FVVacation;

                    //    return true;
                    //}


                    return biddataresponse;
                }
                else
                {

                    //Show alert. Set the alet from server itself.
                    //Show the server message from response.
                    return biddataresponse;
                }
            }
            catch (Exception ex)
            {
                AddDetailsToMailCrashLogs(ex);
                return biddataresponse;
            }

        }

        public static void AddDetailsToMailCrashLogs(Exception exception)
        {
            Console.WriteLine("Execption :" + exception);

            Console.WriteLine("Date :" + DateTime.Today.ToString());

            Console.WriteLine("device :" + UIDevice.CurrentDevice.LocalizedModel);

            string currentBid = FileOperations.ReadCurrentBidDetails(WBidHelper.GetAppDataPath() + "/CurrentDetails.txt");


            if (exception != null)
            {
                // Crashes.TrackError(exception);
                Exception InnerException = exception.InnerException;
                string message = exception.Message;
                string where = exception.StackTrace.Split(new string[] { " at " }, 2, StringSplitOptions.None)[1];
                string source = exception.Source;

                if (InnerException != null)
                {
                    if (InnerException.Message != null)
                    {
                        message = InnerException.Message;
                    }

                    if (InnerException.StackTrace != null)
                    {
                        where = InnerException.StackTrace.Split(new string[] { " at " }, 2, StringSplitOptions.None)[1];
                    }

                    source = InnerException.Source;

                    if (InnerException.InnerException != null)
                    {
                        if (InnerException.InnerException.Message != null)
                        {
                            message += " -> " + InnerException.InnerException.Message;
                        }

                        if (InnerException.InnerException.StackTrace != null)
                        {
                            where += "\r\n\r\n -> " + InnerException.InnerException.StackTrace.Split(new string[] { " at " },
                                2, StringSplitOptions.None)[1];
                        }

                        if (InnerException.InnerException.Source != null)
                        {
                            source += " -> " + InnerException.InnerException.Source;
                        }
                    }
                }

                if (where.Length > 1024)
                {
                    where = where.Substring(0, 1024);
                }


                var submitResult = "\r\n WbidiPad Error Details.\r\n\r\n Error  :  " + message + "\r\n\r\n Where  :  " + where + "\r\n\r\n Source   :  " + source + "\r\n\r\n Version : " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();


                submitResult += "\r\n\r\n Data :" + currentBid;

                submitResult += "\r\n\r\n Device Model: " + DeviceInfo.Model;
                submitResult += "\r\n iOS Version: " + UIDevice.CurrentDevice.SystemVersion;


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
                submitResult += "\r\n Internet Connectivity Via : " + internetType;

                // string submitResult = "\r\n\r\n\r\n Crash Report : \r\n\r\n\r\n" + "\r\n Date: " + DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss") + "\r\n\r\n Device: " + UIDevice.CurrentDevice.LocalizedModel + "\r\n\r\n Crash Details: " + ex + "\r\n\r\n Data: " + currentBid + "\r\n\r\n" + " ******************************* \r\n";


                if (!Directory.Exists(WBidHelper.GetAppDataPath() + "/" + "Crash"))
                {
                    Directory.CreateDirectory(WBidHelper.GetAppDataPath() + "/" + "Crash");
                }

                System.IO.File.AppendAllText(WBidHelper.GetAppDataPath() + "/Crash/" + "Crash.log", submitResult);
            }
        }

    }
}
