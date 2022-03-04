using System;
using CoreGraphics;
using Foundation;
using UIKit;
using System.Collections.Generic;
using WBid.WBidiPad.PortableLibrary.Parser;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;


namespace WBid.WBidiPad.iOS
{
	public partial class webView : UIViewController
	{

		int finishCount = 0;
		int finishCount2 = 0;
		List<string> listOfLines;
		string userName;
		string password;
		List<string> pairingwHasNoDetails;
		int month;
		int year;
		int show1stDay;
		int showAfter1stDay;
		string tripDate;
		int currentIndex;
		string _position;
		Dictionary<string, Trip> parsedDict;
		PraseScrapedTripDetails scraper = new PraseScrapedTripDetails ();

		public webView ( string uname, string pass, List<string> strArr, int mn, int yr, int s1D, int sA1D,string position) : base ("webView", null)
		{
			userName = uname;
			password = pass;
			pairingwHasNoDetails = strArr;
			month = mn;
			year = yr;
			show1stDay = s1D;
			showAfter1stDay = sA1D;
			_position = position;
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
			string url = "https://www15.swalife.com/PortalWeb/cwa.jsp?test=test";
			browser.LoadRequest(new NSUrlRequest(new NSUrl(url)));
			//browser.Hidden = true;
			browser.ScalesPageToFit = true;
			browser.LoadFinished += (object sender, EventArgs e) => browserLoadFinished(sender,e);

			browser2.ScalesPageToFit = true;
			browser2.LoadFinished += (object sender, EventArgs e) => browserLoadFinished2(sender,e);

		}

		public void browserLoadFinished(object sender, EventArgs e)
		{
			finishCount++;
			if (finishCount == 3) {
				string setUser = "window.frames[1].document.getElementById(\"useridField\").value =" + "\"" + userName + "\"" + ";";
				string setPass = "window.frames[1].document.forms[0].elements[2].value =" + "\"" + password + "\"" + ";";
				browser.EvaluateJavascript (setUser);//Set value to it
				browser.EvaluateJavascript (setPass);
				browser.EvaluateJavascript ("var field = window.frames[1].document.getElementsByTagName('a')[1]; field.click();");
				//browser.EvaluateJavascript ("window.alert('test');");
			} else if (finishCount == 7) {
				//finishCount = 0;
				//browser.Tag = 3;
				currentIndex = 0;

				string day = pairingwHasNoDetails [currentIndex].Substring (4, 2).TrimStart (' ');
				tripDate = month.ToString ("d2") + "%2F" + int.Parse (day).ToString ("d2") + "%2F" + year.ToString ();
				Console.WriteLine (pairingwHasNoDetails [currentIndex].Substring (0, 4));
				if (_position == "FA")
				{
					browser2.LoadRequest(new NSUrlRequest(new NSUrl("https://www15.swalife.com/csswa/ea/fa/getPairingDetails.do?crewMemberId=" + userName + "&tripDate=" + tripDate + "&tripNumber=" + pairingwHasNoDetails[currentIndex].Substring(0, 4) + "&tripDateInput=" + tripDate)));
				}
				else
				{ 
					browser2.LoadRequest(new NSUrlRequest(new NSUrl("https://www15.swalife.com/csswa/ea/plt/getPairingDetails.do?crewMemberId=" + userName + "&tripDate=" + tripDate + "&tripNumber=" + pairingwHasNoDetails[currentIndex].Substring(0, 4) + "&tripDateInput=" + tripDate)));
				}


			} else if (finishCount == 10) {
//				browser.EvaluateJavascript ("window.alert(window.frames.count);");
//				browser.EvaluateJavascript ("var field = window.frames[2].document.getElementsByTagName('a')[6]; field.click();");

			}
		}

		public void browserLoadFinished2(object sender, EventArgs e)
		{
			finishCount2++;
			if (finishCount2 == 1) {
				//get html string here and pass to method

				string html = browser2.EvaluateJavascript ("document.body.innerHTML");
				if (html == string.Empty) {
					finishCount2 = 0;
					if (_position == "FA")
					{
						browser2.LoadRequest(new NSUrlRequest(new NSUrl("https://www15.swalife.com/csswa/ea/fa/getPairingDetails.do?crewMemberId=" + userName + "&tripDate=" + tripDate + "&tripNumber=" + pairingwHasNoDetails[currentIndex].Substring(0, 4) + "&tripDateInput=" + tripDate)));
					}
					else
					{
						browser2.LoadRequest(new NSUrlRequest(new NSUrl("https://www15.swalife.com/csswa/ea/plt/getPairingDetails.do?crewMemberId=" + userName + "&tripDate=" + tripDate + "&tripNumber=" + pairingwHasNoDetails[currentIndex].Substring(0, 4) + "&tripDateInput=" + tripDate)));
					}
					return;
				}
				try {

					Trip aTrip;
					if (GlobalSettings.CurrentBidDetails.Postion == "FA")
					{
						aTrip = scraper.ParseTripDetailsForFA(pairingwHasNoDetails[currentIndex], browser2.EvaluateJavascript("document.body.innerHTML"), show1stDay, showAfter1stDay);
					}
					else
					{
						aTrip = scraper.ParseTripDetails(pairingwHasNoDetails[currentIndex], browser2.EvaluateJavascript("document.body.innerHTML"), show1stDay, showAfter1stDay);
					}
					//Trip aTrip = scraper.ParseTripDetails (pairingwHasNoDetails [currentIndex], browser2.EvaluateJavascript ("document.body.innerHTML"), show1stDay, showAfter1stDay);

                 

					if (aTrip != null) {
						if (parsedDict == null)
							parsedDict = new Dictionary<string, Trip> ();
					
						parsedDict.Add (pairingwHasNoDetails [currentIndex], aTrip);
					} else {
						parsedDict = null;
					}
				} catch (Exception ex) {
                    GlobalSettings.parsedDict = null;
                    GlobalSettings.IsScrapStart = false;
                    NSNotificationCenter.DefaultCenter.PostNotificationName("ScrapingFailed", null);
					Logout ();

					return;
				}

				if (parsedDict == null) {
					GlobalSettings.parsedDict = null;
                    GlobalSettings.IsScrapStart = false;
					NSNotificationCenter.DefaultCenter.PostNotificationName ("ScrapingFailed", null);
					Logout ();

					return;
				}


				if (currentIndex == pairingwHasNoDetails.Count - 1) {
					//call method and return
					GlobalSettings.parsedDict = parsedDict;
                    GlobalSettings.IsScrapStart = false;
					NSNotificationCenter.DefaultCenter.PostNotificationName ("ScrapingSuccess", null);

					// Log Out //
					//browser.EvaluateJavascript ("var field = window.frames[2].document.getElementsByTagName('a')[1]; field.click();");

					//browser.EvaluateJavascript("var frm1 = window.frames['portalMainFrame']; var frm2 = frm1[1].window.frames['appMainFrame']; var clk = frm2[1].document.getElementsByTagName('Exit CWA'); alert(clk.length);");
					//browser.EvaluateJavascript ("var a = document.getElementByName('portalMainFrame'); alert(a.length); ");

					Logout ();

					return;
				} else {
					finishCount2 = 0;
					currentIndex++;
					Console.WriteLine (pairingwHasNoDetails [currentIndex].Substring (0, 4));

					string day = pairingwHasNoDetails [currentIndex].Substring (4, 2).TrimStart (' ');
					tripDate = month.ToString ("d2") + "%2F" + int.Parse (day).ToString ("d2") + "%2F" + year.ToString ();
					if (_position == "FA")
					{
						browser2.LoadRequest(new NSUrlRequest(new NSUrl("https://www15.swalife.com/csswa/ea/fa/getPairingDetails.do?crewMemberId=" + userName + "&tripDate=" + tripDate + "&tripNumber=" + pairingwHasNoDetails[currentIndex].Substring(0, 4) + "&tripDateInput=" + tripDate)));
					}
					else
					{
						browser2.LoadRequest(new NSUrlRequest(new NSUrl("https://www15.swalife.com/csswa/ea/plt/getPairingDetails.do?crewMemberId=" + userName + "&tripDate=" + tripDate + "&tripNumber=" + pairingwHasNoDetails[currentIndex].Substring(0, 4) + "&tripDateInput=" + tripDate)));
					}
				}	
			}
		}

		void Logout ()
		{
			string ex = string.Empty;
			if (_position == "FA")
			{
				ex = "https://www15.swalife.com/csswa/ea/fa/logout.do";
			}
			else
			{
				 ex = "https://www15.swalife.com/csswa/ea/plt/logout.do";
			}
			browser.LoadRequest (new NSUrlRequest (new NSUrl (ex)));
			browser2.LoadRequest (new NSUrlRequest (new NSUrl (ex)));
		}
	}
}

