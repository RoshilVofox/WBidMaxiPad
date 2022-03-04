using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.iOS.Utility;
using System.IO;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Essentials;
namespace WBid.WBidiPad.iOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main(string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			//Check the WBidMax directory is created to store the app data for the WBid
			try
			{
				UIApplication.Main(args, null, "AppDelegate");
			}
			catch (Exception exception)
			{
                
				Console.WriteLine("Execption :" + exception);

				Console.WriteLine("Date :" + DateTime.Today.ToString());

                Console.WriteLine("device :" + UIDevice.CurrentDevice.LocalizedModel);

				string currentBid = FileOperations.ReadCurrentBidDetails(WBidHelper.GetAppDataPath() + "/CurrentDetails.txt");


				if (exception != null)
				{
                    Crashes.TrackError(exception);
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

                    submitResult += "\r\n\r\n Device Model: "+ DeviceInfo.Model;
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
}