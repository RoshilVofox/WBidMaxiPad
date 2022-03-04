//using System;
//using System.Windows.Forms;
//using System.Collections.Generic;
//using System.Linq;
//using WBid.WBidiPad.Model;
//
//namespace WBid.WBidiPad.iOS
//{
//	public class ScrapTripForContractorUser
//	{
//
//
//
//		#region Private Variables
//		enum PageState { Home = 0, LOGIN = 1, Tool = 2, Operation = 3, CssWeb = 4, PairingView = 5 };
//		private PageState _mainState;
//		private int _loginPageCount = 0;
//		private int _homePageCount = 0;
//		private int _year = 0;
//		private int _month = 0;
//		private string _userName = string.Empty;
//		private string _password = string.Empty;
//		private bool _isParsingCompleted = false;
//
//		private int _operationLinkCount = 0;
//		private int _toolLinkCount = 0;
//		private int _cssWebLinkCount = 0;
//		private int _pairingViewCount = 0;
//
//		SHDocVw.WebBrowser_V1 Web_V1;
//
//		WebBrowser WebBrowser1 = new WebBrowser();
//		WebBrowser WebBrowser2 = new WebBrowser();
//		WebBrowser WebBrowser3 = new WebBrowser();
//		List<string> _pairingHasNoDetails = new List<string>();
//		List<string> _distinctPairingHasNoDetails = new List<string>();
//		Dictionary<string, Trip> _tripdata = new Dictionary<string, Trip>();
//		bool _isErrorHappened = false;
//
//		private int _show1stDay = 0;
//		private int _showAfter1stDay = 0;
//		DateTime starttime;
//		//private Timer _timer;
//		int _time = 0;
//
//		#endregion
//
//
//		public ScrapTripForContractorUser()
//		{
//			WebBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(WebBrowser1_DocumentCompleted);
//			WebBrowser2.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(WebBrowser2_DocumentCompleted);
//
////			WebBrowser1.Navigated += new WebBrowserNavigatedEventHandler(
////				(object sender, WebBrowserNavigatedEventArgs args) =>
////				{
////					Action<HtmlDocument> blockAlerts = (HtmlDocument d) =>
////					{
////						HtmlElement h = d.GetElementsByTagName("head")[0];
////						HtmlElement s = d.CreateElement("script");
////						IHTMLScriptElement e = (IHTMLScriptElement)s.DomElement;
////						e.text = "window.alert=function(){};";
////						h.AppendChild(s);
////					};
////					System.Windows.Forms.WebBrowser b = sender as System.Windows.Forms.WebBrowser;
////					blockAlerts(b.Document);
////					for (int i = 0; i < b.Document.Window.Frames.Count; i++)
////						try
////					{
////						blockAlerts(b.Document.Window.Frames[i].Document);
////					}
////					catch (Exception)
////					{
////					};
////				}
////			);
////			//For remove the pop upmessage box
////			WebBrowser2.Navigated += new WebBrowserNavigatedEventHandler(
////				(object sender, WebBrowserNavigatedEventArgs args) =>
////				{
////					Action<HtmlDocument> blockAlerts = (HtmlDocument d) =>
////					{
////						HtmlElement h = d.GetElementsByTagName("head")[0];
////						HtmlElement s = d.CreateElement("script");
////						IHTMLScriptElement e = (IHTMLScriptElement)s.DomElement;
////						e.text = "window.alert=function(){};";
////						h.AppendChild(s);
////					};
////					System.Windows.Forms.WebBrowser b = sender as System.Windows.Forms.WebBrowser;
////					blockAlerts(b.Document);
////					for (int i = 0; i < b.Document.Window.Frames.Count; i++)
////						try
////					{
////						blockAlerts(b.Document.Window.Frames[i].Document);
////					}
////					catch (Exception)
////					{
////					};
////				}
////			);
//		}
//
//		#region Public Methods
//		/// <summary>
//		/// PURPOSE : Get Trip details
//		/// </summary>
//		/// <param name="userName"></param>
//		/// <param name="password"></param>
//		/// <param name="pairingwHasNoDetails"></param>
//		/// <param name="month"></param>
//		/// <param name="year"></param>
//		/// <returns></returns>
//		public Dictionary<string, Trip> GetCWATripDetails(string userName, string password, List<string> pairingwHasNoDetails, int month, int year, int show1stDay, int showAfter1stDay)
//		{
//
//			try
//			{
//				starttime = DateTime.Now ;
//				_show1stDay = show1stDay;
//				_showAfter1stDay = showAfter1stDay;
//				_isErrorHappened = false;
//				_isParsingCompleted = false;
//				_loginPageCount = 0;
//				_homePageCount = 0;
//				_mainState = PageState.LOGIN;
//				_userName = userName;
//				_password = password;
//				_pairingHasNoDetails = pairingwHasNoDetails;
//
//				_month = month;
//				_year = year;
//
//				WebBrowser1.Url = new Uri("https://login.swalife.com/myswa_lifelogin.htm");
//				Timer timer = new Timer();
//				//Wait for complete the   parsing
//				while (!_isParsingCompleted)
//				{
//					if ((DateTime.Now - starttime).Minutes > 3)
//					{
//						_isParsingCompleted = true;
//						_isErrorHappened = true;
//					}
//					System.Windows.Forms.Application.DoEvents();
//				}
//
//				//LogOut();
//
//				WebBrowser1.Dispose();
//				WebBrowser2.Dispose();
//				WebBrowser3.Dispose();
//
//				//  Web_V1.Stop();
//
//
//
//				if (_isErrorHappened)
//					throw new Exception();
//				return _tripdata;
//
//			}
//			catch (Exception ex)
//			{
//				// VacationHelper.TraceService(GlobalSettings.GetLogPath(), "  ex9" + ex.ToString());
//				throw ex;
//			}
//
//
//		}
//
//		#endregion
//		private void WebBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
//		{
//			switch (_mainState)
//			{
//			case PageState.LOGIN:
//				_loginPageCount++;
//				break;
//			case PageState.Home:
//				_homePageCount++;
//				break;
//			case PageState.Tool:
//				_toolLinkCount++;
//				break;
//			case PageState.Operation:
//				_operationLinkCount++;
//				break;
//			case PageState.CssWeb:
//				_cssWebLinkCount++;
//				break;
//			case PageState.PairingView:
//				_pairingViewCount++;
//				break;
//			}
//
//
//			//WebBrowser1_DocumentCompleted event firing multiple times. That's why we are using count variable 
//
//
//
//			//Login page load complete
//			if (_mainState == PageState.LOGIN)
//			{
//
//				if (LoginProcess(_userName, _password))
//				{
//					//logged in successfully to home page
//					_mainState = PageState.Home;
//
//				}
//			}
//
//			//Home page load completed
//			else if (_mainState == PageState.Home && _homePageCount == 3)
//			{
//
//				HtmlElement toolbutton = WebBrowser1.Document.Window.Document.Body.Document.GetElementsByTagName("a")[9];
//				toolbutton.InvokeMember("click");
//				_mainState = PageState.Tool;
//
//			}
//			else if (_mainState == PageState.Tool && _toolLinkCount == 7)
//			{
//
//				HtmlElement toolbutton = WebBrowser1.Document.Window.Document.Body.Document.GetElementById("Menu5").GetElementsByTagName("a")[0];
//				toolbutton.InvokeMember("click");
//				_mainState = PageState.Operation;
//			}
//			else if (_mainState == PageState.Operation && _operationLinkCount == 4)
//			{
//				Web_V1 = (SHDocVw.WebBrowser_V1)this.WebBrowser1.ActiveXInstance;
//				Web_V1.NewWindow += new SHDocVw.DWebBrowserEvents_NewWindowEventHandler(Web_V1_NewWindow);
//				Web_V1.DownloadComplete += Web_V1_DownloadComplete;
//				HtmlElement toolbutton = WebBrowser1.Document.Window.Document.Body.Document.GetElementsByTagName("a")[48];
//				toolbutton.InvokeMember("click");
//				_mainState = PageState.CssWeb;
//			}
//			else if (_mainState == PageState.PairingView && _pairingViewCount == 4)
//			{
//
//			}
//		}
//
//		void Web_V1_DownloadComplete()
//		{
//		}
//
//		private void WebBrowser2_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
//		{
//			_pairingViewCount++;
//			if (_pairingViewCount == 7)
//			{
//				_mainState = PageState.PairingView;
//				ScrapTripDetails();
//				LogOut(WebBrowser1.Document);
//
//				LogoutPopup();
//			}
//
//		}
//		private void Web_V1_NewWindow(string URL, int Flags, string TargetFrameName, ref object PostData, string Headers, ref bool Processed)
//		{
//			Processed = true; //Stop event from being processed 
//			WebBrowser2.Navigate(URL);
//		}
//
//
//		private void LogoutPopup()
//		{
//			HtmlDocument firstDocument = WebBrowser2.Document;
//			HtmlWindow frameFirst = firstDocument.Window.Frames["bottom"];
//			HtmlDocument secondDocument = frameFirst.Document;
//			HtmlWindow frameSecond = secondDocument.Window.Frames["appMainFrame"];
//			HtmlElementCollection links = frameSecond.Document.Links;
//
//			HtmlElement signout = null;
//			foreach (HtmlElement link in links)
//			{
//				if (link.InnerText.Contains("Exit CSS Web"))
//				{
//					signout = link;
//					break;
//				}
//
//			}
//			if (signout != null)
//			{
//				signout.InvokeMember("click");
//				while (WebBrowser2.ReadyState != WebBrowserReadyState.Complete)
//				{
//					System.Windows.Forms.Application.DoEvents();
//				}
//			}
//		}
//
//		private void WebBrowser3_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
//		{
//
//
//		}
//
//		private bool LoginProcess(string UserName, string Password)
//		{
//			try
//			{
//
//
//				HtmlElement userName = WebBrowser1.Document.Window.Document.Body.Document.GetElementById("useridField");
//
//				HtmlElement password = WebBrowser1.Document.Window.Document.Body.Document.GetElementById("Password");
//
//				userName.SetAttribute("value", UserName);
//				password.SetAttribute("value", Password);
//				WebBrowser1.Document.Window.Document.InvokeScript("goSubmit");
//
//
//				return true;
//			}
//			catch (Exception ex)
//			{
//				return false;
//			}
//		}
//		/// <summary>
//		///PURPOSE: Method to logout from the page
//		/// </summary>
//		/// <param name="logOutDoc"></param>
//		/// <returns>exit link</returns>
//		public void LogOut(HtmlDocument logOutDoc)
//		{
//			try
//			{
//
//				HtmlElement logoutLink = null;
//				foreach (HtmlElement lnkelement in logOutDoc.Links)
//				{
//					if (lnkelement.InnerText != null && lnkelement.InnerText.Contains("log out"))
//					{
//						logoutLink = lnkelement;
//						break;
//					}
//				}
//				if (logoutLink != null)
//				{
//					logoutLink.InvokeMember("click");
//					//_mainState = PageState.DONE;
//
//				}
//
//			}
//			catch
//			{
//
//			}
//
//		}
//
//		private void ScrapTripDetails()
//		{
//			try
//			{
//
//
//				string url = string.Empty;
//				string tripDate = string.Empty;
//
//				url = "https://www15.swalife.com/csswa/ea/plt/getPairingDetails.do?crewMemberId=";
//
//				// _pairingHasNoDetails.Add("BA1102");
//				foreach (string tripName in _pairingHasNoDetails)
//				{
//
//					// string day = _pairingHasNoDetails.FirstOrDefault(x => x.Substring(0, 4) == tripName).Substring(4, 2).TrimStart(' ');
//					string day = tripName.Substring(4, 2).TrimStart(' ');
//
//					tripDate = _month.ToString("d2") + "%2F" + int.Parse(day).ToString("d2") + "%2F" + _year.ToString();
//					WebBrowser3.Url = new Uri(url + "&tripDate=" + tripDate + "&tripNumber=" + tripName.Substring(0, 4) + "&tripDateInput=" + tripDate);
//					//WebBrowser1.Url = new Uri("https://www15.swalife.com/csswa/ea/plt/getPairingDetails.do?crewMemberId=&tripDate=06%2F02%2F2016&tripNumber=ba11&tripDateInput=06%2F02%2F2016");
//
//					//Wait for complete the   webbrowser load
//					while (WebBrowser3.ReadyState != WebBrowserReadyState.Complete)
//					{
//
//						System.Windows.Forms.Application.DoEvents();
//					}
//					ParseTripDetails(tripName);
//				}
//				_isParsingCompleted = true;
//			}
//			catch (Exception ex)
//			{
//				_isParsingCompleted = true;
//				_isErrorHappened = true;
//				throw ex;
//			}
//
//		}
//
//		private void ParseTripDetails(string tripName)
//		{
//			try
//			{
//
//
//				Trip trip = new Trip();
//				int seqenceNumber = 1;
//
//				HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
//				//document.Load(WebBrowser2.Document.Body.InnerHtml);
//				document.Load(WebBrowser3.DocumentStream);
//
//				string startDate = string.Empty;
//				//Read Title Section
//				//------------------------------------
//				HtmlAgilityPack.HtmlNode titleNode = document.DocumentNode.SelectSingleNode("//div//div[@class='printTitle']");
//				startDate = titleNode.InnerText.Substring(titleNode.InnerText.Length - 10, 10);
//
//				HtmlAgilityPack.HtmlNode pairingTable = document.DocumentNode.SelectSingleNode("//div//table[@class='pairingTable']");
//
//
//				trip.StartOp = int.Parse(startDate.Substring(3, 2));
//				trip.TripNum = tripName;//titleNode.InnerText.Substring(5, 4);
//
//				//Get last operational day
//				trip.EndOp = int.Parse(_pairingHasNoDetails.Where(x => x.Substring(0, 4) == trip.TripNum.Substring(0, 4)).Select(y => y.Substring(4, 2)).Last());
//
//
//				int rowsCount = pairingTable.SelectNodes("tr").Count;
//				int colPosition = 0;
//
//
//				// Read the header column order. So that we can use this position while reading the content
//				List<PairingStructure> pairingColumnOrder = new List<PairingStructure>();
//				pairingColumnOrder = GetHeaderColumnDeatails(pairingTable.SelectNodes("tr")[1]);
//				PairingStructure currentColumn = null; ;
//				int leftPanelColumnCount = 11;
//				if (pairingTable.SelectNodes("tr")[rowsCount - 2].SelectNodes("td")[0].Attributes["colSpan"] != null)
//				{
//					leftPanelColumnCount = int.Parse(pairingTable.SelectNodes("tr")[rowsCount - 2].SelectNodes("td")[0].Attributes["colSpan"].Value) - 1;
//				}
//
//
//				//Read trip footer details
//				//------------------------------------
//				//HtmlAgilityPack.HtmlNode footerTable = pairingTable.SelectNodes("tr")[rowsCount - 1].SelectNodes("td")[1].SelectSingleNode("table");
//
//				//Total Block
//				//trip.Block = ConvertHhmmToMinutes(footerTable.SelectNodes("tr")[0].SelectNodes("td")[1].InnerText);
//				currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Block" && x.Position >= 9);
//				colPosition = (currentColumn == null) ? 2 : (currentColumn.ContentPosition - leftPanelColumnCount);
//				trip.Block = ConvertHhmmToMinutes(pairingTable.SelectNodes("tr")[rowsCount - 2].SelectNodes("td")[colPosition].InnerText);
//				//Total dutyTime
//				//trip.DutyTime = ConvertHhmmToMinutes(footerTable.SelectNodes("tr")[0].SelectNodes("td")[2].InnerText);
//				currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Duty");
//				colPosition = (currentColumn == null) ? 4 : (currentColumn.ContentPosition - leftPanelColumnCount); ;
//				trip.DutyTime = ConvertHhmmToMinutes(pairingTable.SelectNodes("tr")[rowsCount - 2].SelectNodes("td")[colPosition].InnerText);
//				//Total credit
//				//trip.Tfp = Math.Round(decimal.Parse(footerTable.SelectNodes("tr")[0].SelectNodes("td")[3].InnerText) / 100, 2);
//				currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Credit");
//				colPosition = (currentColumn == null) ? 5 : (currentColumn.ContentPosition - leftPanelColumnCount);
//				trip.Tfp = Math.Round(decimal.Parse(pairingTable.SelectNodes("tr")[rowsCount - 2].SelectNodes("td")[colPosition].InnerText) / 100, 2);
//				//Carry Block
//				// trip.CarryOverBlock = ConvertHhmmToMinutes(footerTable.SelectNodes("tr")[1].SelectNodes("td")[1].InnerText);
//				currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Block" && x.Position >= 9);
//				colPosition = (currentColumn == null) ? 1 : (currentColumn.ContentPosition - leftPanelColumnCount);
//				trip.CarryOverBlock = ConvertHhmmToMinutes(pairingTable.SelectNodes("tr")[rowsCount - 1].SelectNodes("td")[colPosition - 1].InnerText);
//				//Carry over credit
//				// trip.CarryOverTfp = Math.Round(decimal.Parse(footerTable.SelectNodes("tr")[1].SelectNodes("td")[3].InnerText));
//				currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Credit");
//				colPosition = (currentColumn == null) ? 3 : (currentColumn.ContentPosition - leftPanelColumnCount);
//				trip.CarryOverTfp = Math.Round(decimal.Parse(pairingTable.SelectNodes("tr")[rowsCount - 1].SelectNodes("td")[colPosition - 1].InnerText));
//
//				//------------------------------------
//
//
//				//Parse DutyPeriods and flights
//				//First two rows are Totals and title lines-- so we dont need to consider those lines
//				//Also last two lines not need to consider while paring dutyperiod deatsils 
//				// string  
//
//				ParseStatus status = ParseStatus.NotStarted;
//				DutyPeriod dutyPeriod = null;
//
//
//				for (int count = 2; count < rowsCount - 2; count++)
//				{
//
//					HtmlAgilityPack.HtmlNode trNode = pairingTable.SelectNodes("tr")[count];
//
//					//Rpt line parsing
//					if (status == ParseStatus.NotStarted)
//					{
//						string rpt = string.Empty;
//						dutyPeriod = new DutyPeriod();
//						dutyPeriod.TripNum = tripName;
//						if (trNode.SelectNodes("td").Count > 1)
//						{
//
//							//Read DEPART column
//							//--------------------------------------
//							currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Depart");
//							colPosition = (currentColumn == null) ? 2 : currentColumn.Position;
//
//							rpt = trNode.SelectNodes("td")[colPosition].InnerText;
//							if (rpt.Contains("Rpt"))
//							{
//								//int depTimeFirstLeg = int.Parse(rpt.Replace("Rpt", "").Replace("&nbsp;", "")) + 1440 * trip.DutyPeriods.Count;
//								dutyPeriod.DepTimeFirstLeg = ConvertHhmmToMinutes(rpt.Replace("Rpt", "").Replace("&nbsp;", "")) + 1440 * trip.DutyPeriods.Count;
//								status = ParseStatus.DutyPeriodStarted;
//							}
//						}
//
//					}
//					//Flight line
//					else if (status == ParseStatus.DutyPeriodStarted)
//					{
//
//						if (trNode.Attributes["class"] != null && (trNode.Attributes["class"].Value == "" || trNode.Attributes["class"].Value == "legWarning" || trNode.Attributes["class"].Value == "red"))
//						{
//							Flight flight = new Flight();
//							currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Flight");
//							try
//							{
//								string deadHead = trNode.SelectNodes("td")[1].InnerText.Trim();
//								if (deadHead.Contains("DM") || deadHead.Contains("DH"))
//								{
//									flight.DeadHead = true;
//								}
//							}
//							catch (Exception)
//							{
//
//
//							}
//							colPosition = (currentColumn == null) ? 2 : currentColumn.ContentPosition;
//							flight.FltNum = int.Parse(trNode.SelectNodes("td")[colPosition].SelectSingleNode("a").InnerText);
//
//							currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Depart");
//							colPosition = (currentColumn == null) ? 3 : currentColumn.ContentPosition;
//							flight.DepSta = trNode.SelectNodes("td")[colPosition].InnerText.Substring(0, 3);
//							int depTime = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].SelectSingleNode("span").InnerText);
//							flight.DepTime = depTime + 1440 * trip.DutyPeriods.Count;
//
//							//Ranish--- To solve the Block time issue in trip details view.
//							//if the trip passes the mid night we found that there is an issue in arrTime.
//
//							// flight.DepTime = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].SelectSingleNode("span").InnerText) + 1440 * trip.DutyPeriods.Count;
//
//
//							currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Arrive");
//							colPosition = (currentColumn == null) ? 4 : currentColumn.ContentPosition;
//							flight.ArrSta = trNode.SelectNodes("td")[colPosition].InnerText.Substring(0, 3);
//							// flight.ArrTime = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].SelectSingleNode("span").InnerText) + 1440 * trip.DutyPeriods.Count;
//							int arrTime = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].SelectSingleNode("span").InnerText);
//							if (arrTime < depTime)
//							{
//								arrTime = arrTime + 1440;
//							}
//
//							flight.ArrTime = arrTime + 1440 * trip.DutyPeriods.Count;
//
//							currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Eq");
//							colPosition = (currentColumn == null) ? 5 : currentColumn.ContentPosition;
//							flight.Equip = trNode.SelectNodes("td")[colPosition].InnerText;
//
//							currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Block");
//							colPosition = (currentColumn == null) ? 6 : currentColumn.ContentPosition;
//							flight.Block = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].InnerText.Replace("\t", "").Replace("\n", "").Replace("\r", "").PadLeft(4, '0'));
//							//flight.Reg = trNode.SelectNodes("td")[7].InnerText;
//							currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Reg");
//							colPosition = (currentColumn == null) ? 9 : currentColumn.ContentPosition;
//							flight.Reg = trNode.SelectNodes("td")[colPosition].InnerText;
//
//
//							//flight.TurnTime = ConvertHhmmToMinutes(trNode.SelectNodes("td")[8].InnerText.PadLeft(4,'0'));
//							currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Ground");
//							colPosition = (currentColumn == null) ? 10 : currentColumn.ContentPosition;
//							flight.TurnTime = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].InnerText);
//							//flight.Tfp = Math.Round(decimal.Parse(trNode.SelectNodes("td")[12].InnerText) / 100, 2);
//
//							currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Credit");
//							colPosition = (currentColumn == null) ? 15 : currentColumn.ContentPosition;
//							flight.Tfp = Math.Round(decimal.Parse(trNode.SelectNodes("td")[colPosition].InnerText) / 100, 2);
//
//							flight.FlightSeqNum = dutyPeriod.Flights.Count + 1;
//							dutyPeriod.Flights.Add(flight);
//
//
//						}
//						else
//						{    //Line under the Flights. That is "Rls" line
//							string rls = string.Empty;
//							currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Arrive");
//							colPosition = (currentColumn == null) ? 3 : currentColumn.Position;
//							rls = trNode.SelectNodes("td")[colPosition].InnerText;
//							if (rls.Contains("Rls"))
//							{
//								//dutyPeriod.Block = ConvertHhmmToMinutes(trNode.SelectNodes("td")[9].InnerText.PadLeft(4, '0'));
//								currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Block" && x.Position >= 9);
//								colPosition = (currentColumn == null) ? 11 : currentColumn.Position;
//								dutyPeriod.Block = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].InnerText.PadLeft(4, '0'));
//								//dutyPeriod.DutyTime = ConvertHhmmToMinutes(trNode.SelectNodes("td")[10].InnerText.PadLeft(4, '0'));
//
//								try
//								{
//									currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "FDP");
//									colPosition = (currentColumn == null) ? 12 : currentColumn.Position;
//									dutyPeriod.FDP = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].InnerText);
//
//								}
//								catch (Exception)
//								{
//
//
//								}
//
//
//								currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Duty");
//								colPosition = (currentColumn == null) ? 13 : currentColumn.Position;
//								dutyPeriod.DutyTime = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].InnerText.PadLeft(4, '0'));
//
//								//dutyPeriod.Tfp = decimal.Parse(trNode.SelectNodes("td")[11].InnerText) / 100;
//								currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Credit");
//								colPosition = (currentColumn == null) ? 14 : currentColumn.Position;
//								dutyPeriod.Tfp = decimal.Parse(trNode.SelectNodes("td")[colPosition].InnerText) / 100;
//								// int landTimeLastLeg = int.Parse(rls.Replace("Rls", "").Replace("&nbsp;", "")) + 1440 * trip.DutyPeriods.Count;
//
//								dutyPeriod.LandTimeLastLeg = ConvertHhmmToMinutes(rls.Replace("Rls", "").Replace("&nbsp;", "")) + 1440 * trip.DutyPeriods.Count;
//
//								dutyPeriod.DutPerSeqNum = trip.DutyPeriods.Count + 1;
//
//								dutyPeriod.ArrStaLastLeg = dutyPeriod.Flights[dutyPeriod.Flights.Count - 1].ArrSta;
//
//								dutyPeriod.DutPerSeqNum = seqenceNumber;
//
//								dutyPeriod.ShowTime = dutyPeriod.Flights[0].DepTime - ((dutyPeriod.DutPerSeqNum == 1) ? _show1stDay : _showAfter1stDay);
//								seqenceNumber++;
//								trip.DutyPeriods.Add(dutyPeriod);
//								status = ParseStatus.DutyPeriodEnd;
//							}
//
//						}
//
//					}
//					//Line between two duty periods
//					else if (status == ParseStatus.DutyPeriodEnd)
//					{
//						status = ParseStatus.NotStarted;
//					}
//				}
//
//				trip.DepSta = trip.DutyPeriods[0].Flights[0].DepSta;
//				// trip.DepTime = trip.DutyPeriods[0].DepTimeFirstLeg.ToString();
//				trip.DepTime = ConvertMinuteToHHMM(trip.DutyPeriods[0].Flights[0].DepTime).Replace(":", "");
//				//last dutyperiod
//				DutyPeriod lastDutPeriod = trip.DutyPeriods[trip.DutyPeriods.Count - 1];
//				trip.RetSta = lastDutPeriod.Flights[lastDutPeriod.Flights.Count - 1].ArrSta;
//				// trip.RetTime = Convert.ToString(lastDutPeriod.LandTimeLastLeg + 1440 * (lastDutPeriod.Flights.Count - 1));
//				Flight lastFlight = trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights[trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights.Count - 1];
//				trip.RetTime = ConvertMinuteToHHMM(lastFlight.ArrTime - (1440 * (trip.DutyPeriods.Count - 1))).Replace(":", "");
//				trip.PairLength = trip.DutyPeriods.Count;
//				trip.OpDays = new String(' ', 6);
//				trip.FreqCode = "X";
//				trip.NonOpDays = new String(' ', 8);
//				trip.FDP = trip.DutyPeriods.Sum(x => x.FDP);
//
//				HtmlAgilityPack.HtmlNode tafbNode = document.DocumentNode.SelectNodes("//td[@class='informationTable']").FirstOrDefault(x => x.InnerText.Contains("Time Away From Base:"));
//				trip.Tafb = ConvertHhmmToMinutes(tafbNode.SelectNodes("table/tr/td")[1].InnerHtml.PadLeft(4, '0'));
//				// trip.Tafb = ConvertHhmmToMinutes(document.DocumentNode.SelectNodes("//div//table")[0].SelectNodes("tr")[1].SelectNodes("td")[1].SelectNodes("table//tr//td")[1].InnerHtml.PadLeft(4, '0'));
//
//				trip.BriefTime = _show1stDay;
//				// Add  trip details to trip
//				_tripdata.Add(trip.TripNum, trip);
//
//			}
//			catch (Exception ex)
//			{
//				_isErrorHappened = true;
//				throw ex;
//			}
//
//
//		}
//
//
//
//
//		/// <summary>
//		/// PURPOSE :Convert Hour time to minutes
//		/// </summary>
//		/// <param name="hhmm"></param>
//		/// <returns></returns>
//		private int ConvertHhmmToMinutes(string hhmm)
//		{
//			if (hhmm == string.Empty || int.Parse(hhmm) < 0)
//				return 0;
//
//			hhmm = hhmm.PadLeft(4, '0');
//			int hours = Convert.ToInt32(hhmm.Substring(0, 2));
//			int minutes = Convert.ToInt32(hhmm.Substring(2, 2));
//			return hours * 60 + minutes;
//		}
//
//
//		public string ConvertMinuteToHHMM(int minute)
//		{
//			string result = string.Empty;
//			result = Convert.ToString(minute / 60).PadLeft(2, '0');
//			result += ":";
//			result += Convert.ToString(minute % 60).PadLeft(2, '0'); ;
//			return result;
//
//		}
//		/// <summary>
//		/// PURPOSE: Get header column  order and position. SO that we can read column deatsil based on this position
//		/// (if new column comes, it will nott effect existing parsing
//		/// </summary>
//		/// <param name="htmlNode"></param>
//		/// <returns></returns>
//		private List<PairingStructure> GetHeaderColumnDeatails(HtmlAgilityPack.HtmlNode htmlNode)
//		{
//			List<PairingStructure> lstColuns = new List<PairingStructure>();
//			int count = 0;
//			int incrementer = 0;
//			if (htmlNode.SelectNodes("td").Count > 0)
//			{
//				foreach (HtmlAgilityPack.HtmlNode node in htmlNode.SelectNodes("td"))
//				{
//					if (node.InnerHtml == "Flight")
//					{
//						lstColuns.Add(new PairingStructure() { ColumnName = node.InnerHtml, Position = count, ContentPosition = incrementer + 1 });
//					}
//					else
//					{
//
//						lstColuns.Add(new PairingStructure() { ColumnName = node.InnerHtml, Position = count, ContentPosition = incrementer });
//					}
//					count++;
//					incrementer++;
//					if (node.Attributes["colSpan"] != null)
//					{
//						incrementer += int.Parse(node.Attributes["colSpan"].Value) - 1;
//					}
//				}
//			}
//
//			return lstColuns;
//
//		}
//
//		public enum ParseStatus
//		{
//			NotStarted = 0, DutyPeriodStarted = 1, DutyPeriodEnd = 2
//		}
//
//
//
//		public class PairingStructure
//		{
//			public string ColumnName { get; set; }
//
//			public int Position { get; set; }
//
//			public int ContentPosition { get; set; }
//		}
//	}
//}
//
