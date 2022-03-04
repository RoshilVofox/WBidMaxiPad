using System;

using UIKit;
using System.Collections.Generic;
using WBid.WBidiPad.PortableLibrary.Parser;
using Foundation;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;

namespace WBid.WBidiPad.iOS
{
	public partial class ContractorEmpScrap : UIViewController
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
		public UIPopoverController objpopover;
		public homeViewController SuperParent;
		string _position = string.Empty;
		Dictionary<string, Trip> parsedDict;
		PraseScrapedTripDetails scraper = new PraseScrapedTripDetails();
		public ContractorEmpScrap(string uname, string pass, List<string> strArr, int mn, int yr, int s1D, int sA1D, string position) : base("ContractorEmpScrap", null)
		{
			_position = position;
			userName = uname;
			password = pass;
			pairingwHasNoDetails = strArr;
			month = mn;
			year = yr;
			show1stDay = s1D;
			showAfter1stDay = sA1D;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			//string url = "https://login.swalife.com/myswa_lifelogin.htm";
			string url = "https://www.swacrew.com";
			WebBrowser1.LoadRequest(new NSUrlRequest(new NSUrl(url)));

			WebBrowser1.ScalesPageToFit = true;
			WebBrowser1.LoadStarted += (object sender, EventArgs e) => loadstarted1(sender, e);
			WebBrowser1.LoadFinished += (object sender, EventArgs e) => browserLoadFinished(sender, e);

			WebBrowser2.ScalesPageToFit = true;

			WebBrowser2.LoadStarted += (object sender, EventArgs e) => loadstarted2(sender, e);
			WebBrowser2.LoadFinished += (object sender, EventArgs e) => browserLoadFinished2(sender, e);

			WebBrowser3.ScalesPageToFit = true;
			WebBrowser3.LoadStarted += (object sender, EventArgs e) => loadstarted3(sender, e);
			WebBrowser3.LoadFinished += (object sender, EventArgs e) => browserLoadFinished3(sender, e);

			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

		public void loadstarted1(object sender, EventArgs e)
		{
			WebBrowser1.EvaluateJavascript("window.alert = function() {};");
		}
		public void loadstarted2(object sender, EventArgs e)
		{
			WebBrowser2.EvaluateJavascript("window.alert = function() {};");
		}
		public void loadstarted3(object sender, EventArgs e)
		{
			WebBrowser3.EvaluateJavascript("window.alert = function() {};");
		}
		public void browserLoadFinished(object sender, EventArgs e)
		{

			WebBrowser1.EvaluateJavascript("window.alert = function() {};");
			finishCount++;
			if (finishCount == 2)
			{
				string setUser = "window.document.getElementById(\"ContentPlaceHolder1_MFALoginControl1_UserIDView_txtUserid\").value =" + "\"" + userName + "\"" + ";";
				string setPass = "document.querySelectorAll('[type=password]')[0].value =" + "\"" + password + "\"" + ";";


				WebBrowser1.EvaluateJavascript(setUser);//Set value to it

				WebBrowser1.EvaluateJavascript(setPass);
				WebBrowser1.EvaluateJavascript("var field = window.document.getElementById(\"ContentPlaceHolder1_MFALoginControl1_UserIDView_btnSubmit\"); field.click();");
				//WebBrowser1.EvaluateJavascript ("goSubmit()");
				//browser.EvaluateJavascript ("window.alert('test');");
			}
			else if (finishCount == 3)
			{
				//WebBrowser1.EvaluateJavascript ("var field = document.getElementsByClassName('eipTabBar')[5].children[0]; field.click();");
				if (_position == "FA")
				{
					//WebBrowser1.EvaluateJavascript("var field = Array.prototype.filter.call(document.getElementsByClassName(\"navLink newPageLink level3Link\"),function(element){ return element.text=='Launch CSS Web IF HDQ';})[0];field.click();");
					string url = "https://www.prod1.swacrew.com/prod1-if/CSSWebClient/eipSessionJsp/EipFrameset.jsp?eipFrameUrl=../loginAsNonScheduler.do?isPortal=truehome&role=webhdq&uid=x21221";
					WebBrowser2.LoadRequest(new NSUrlRequest(new NSUrl(url)));
				}
				else
				{
					string url = "https://www.prod1.swacrew.com/prod1-fo/CSSWebClient/eipSessionJsp/EipFrameset.jsp?eipFrameUrl=../loginAsNonScheduler.do?isPortal=truehome&role=webhdq&uid=x21221";
					WebBrowser2.LoadRequest(new NSUrlRequest(new NSUrl(url)));
					//WebBrowser1.EvaluateJavascript("var field = Array.prototype.filter.call(document.getElementsByClassName(\"navLink newPageLink level3Link\"),function(element){ return element.text=='Launch CSS Web FO HDQ';})[0];field.click();");
				}
				//WebBrowser1.EvaluateJavascript ("var field = document.getElementsByClassName(\"navLink newPageLink level3Link\")[34]; field.click();");
				currentIndex = 0;


			}
			else if (finishCount == 7)
			{

				//string ss=WebBrowser1.EvaluateJavascript ("popUpUrl");
				//WebBrowser2.LoadRequest(new NSUrlRequest(new NSUrl(ss)));

				//WebBrowser1.EvaluateJavascript ("window.alert = function() {}; var field = document.getElementById(\"Link5\"); field.click();");
				//				browser.EvaluateJavascript ("window.alert(window.frames.count);");
				//				browser.EvaluateJavascript ("var field = window.frames[2].document.getElementsByTagName('a')[6]; field.click();");

			}
			//			else if (finishCount == 12) {
			//
			//				WebBrowser1.EvaluateJavascript ("window.alert = function() {}; var field = document.getElementById(\"Menu5\").children[0].getElementsByTagName('a')[0]; field.click();");
			//				//				browser.EvaluateJavascript ("window.alert(window.frames.count);");
			//				//				browser.EvaluateJavascript ("var field = window.frames[2].document.getElementsByTagName('a')[6]; field.click();");
			//
			//			}
			//			else if (finishCount == 16) 
			//			{
			//
			//				WebBrowser1.EvaluateJavascript ("window.alert = function() {};var field = document.getElementsByClassName('eipNavItemMenu eipFlatItem')[1].children[0]; field.click();");
			//
			//			}
			//
			//			else if (finishCount == 18) 
			//			{
			//				string ss=WebBrowser1.EvaluateJavascript ("popUpUrl");
			//				WebBrowser2.LoadRequest(new NSUrlRequest(new NSUrl(ss)));
			//			}

		}


		public void browserLoadFinished2(object sender, EventArgs e)
		{
			finishCount2++;
			if (finishCount2 == 2)
			{
				//get html string here and pass to method
				//pairingwHasNoDetails = new List<string> ();
				//pairingwHasNoDetails.Add("BA1101");
				//pairingwHasNoDetails.Add("BA1201");
				//pairingwHasNoDetails.Add("BA1101");
				//finishCount2 = 0;
				string day = pairingwHasNoDetails[currentIndex].Substring(4, 2).TrimStart(' ');
				string url = string.Empty;
				if (_position == "FA")
				{
					//url = "https://www15.swalife.com/csswa/ea/fa/getPairingDetails.do?crewMemberId=";
					url = "https://www.prod1.swacrew.com/prod1-if/CSSWebClient/getPairingDetails.do?crewMemberId=";
				}
				else
				{
					//url = "https://www15.swalife.com/csswa/ea/plt/getPairingDetails.do?crewMemberId=";
					url = "https://www.prod1.swacrew.com/prod1-fo/CSSWebClient/getPairingDetails.do?crewMemberId=";
				}
				tripDate = month.ToString("d2") + "%2F" + int.Parse(day).ToString("d2") + "%2F" + year.ToString();
				WebBrowser3.LoadRequest(new NSUrlRequest(new NSUrl(url + "&tripDate=" + tripDate + "&tripNumber=" + pairingwHasNoDetails[currentIndex].Substring(0, 4) + "&tripDateInput=" + tripDate)));
				return;
			}




		}

		public void browserLoadFinished3(object sender, EventArgs e)
		{

			try
			{
				Trip aTrip;
				if (_position == "FA")
				{
					aTrip = scraper.ParseTripDetailsForFA(pairingwHasNoDetails[currentIndex], WebBrowser3.EvaluateJavascript("document.body.innerHTML"), show1stDay, showAfter1stDay);
				}
				else
				{
					aTrip = scraper.ParseTripDetails(pairingwHasNoDetails[currentIndex], WebBrowser3.EvaluateJavascript("document.body.innerHTML"), show1stDay, showAfter1stDay);
				}

				if (aTrip != null)
				{
					if (parsedDict == null)
						parsedDict = new Dictionary<string, Trip>();

					parsedDict.Add(pairingwHasNoDetails[currentIndex], aTrip);
				}
				else
				{
					parsedDict = null;
				}
			}
			catch (Exception ex)
			{
				GlobalSettings.parsedDict = null;
				GlobalSettings.IsScrapStart = false;
				NSNotificationCenter.DefaultCenter.PostNotificationName("ScrapingFailed", null);
				Logout();

				return;
			}

			if (parsedDict == null)
			{
				GlobalSettings.parsedDict = null;
				GlobalSettings.IsScrapStart = false;
				NSNotificationCenter.DefaultCenter.PostNotificationName("ScrapingFailed", null);
				Logout();

				return;
			}

			if (currentIndex == pairingwHasNoDetails.Count - 1)
			{
				GlobalSettings.parsedDict = parsedDict;
				GlobalSettings.IsScrapStart = false;
				NSNotificationCenter.DefaultCenter.PostNotificationName("ScrapingSuccess", null);

				//Logout ();

				return;
			}
			else
			{
				finishCount2 = 0;
				currentIndex++;
				Console.WriteLine(pairingwHasNoDetails[currentIndex].Substring(0, 4));
				string day = pairingwHasNoDetails[currentIndex].Substring(4, 2).TrimStart(' ');
				string url = string.Empty;
				if (_position == "FA")
				{
					//url = "https://www15.swalife.com/csswa/ea/fa/getPairingDetails.do?crewMemberId=";
					url = "https://www.prod1.swacrew.com/prod1-if/CSSWebClient/getPairingDetails.do?crewMemberId=";
				}
				else
				{
					//url = "https://www15.swalife.com/csswa/ea/plt/getPairingDetails.do?crewMemberId=";
					url = "https://www.prod1.swacrew.com/prod1-fo/CSSWebClient/getPairingDetails.do?crewMemberId="; ;
				}
				//string url = "https://www15.swalife.com/csswa/ea/plt/getPairingDetails.do?crewMemberId=";
				tripDate = month.ToString("d2") + "%2F" + int.Parse(day).ToString("d2") + "%2F" + year.ToString();
				WebBrowser3.LoadRequest(new NSUrlRequest(new NSUrl(url + "&tripDate=" + tripDate + "&tripNumber=" + pairingwHasNoDetails[currentIndex].Substring(0, 4) + "&tripDateInput=" + tripDate)));
			}
		}
		void Logout()
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

			//WebBrowser2.LoadRequest (new NSUrlRequest (new NSUrl (ex)));
			//browser2.LoadRequest (new N
		}
	}
}


