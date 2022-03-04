using System;
using System.Collections.Generic;
using Foundation;
using System.Collections.ObjectModel;
using WBid.WBidiPad.Model;

//using WBid.WBidiPad.PortableLibrary.Utility;
using System.Text;
using System.IO;

//using iTextSharp.text;
//using iTextSharp.text.pdf;
using WBid.WBidiPad.iOS.Utility;
using System.Linq;
using System.Drawing;
using CoreGraphics;
using UIKit;
using WBid.WBidiPad.Core;
using System.Runtime.Serialization.Json;

namespace WBid.WBidiPad.iOS
{
	public class CommonClass
	{
		public CommonClass ()
		{
		}
		public static BidLineViewController ObjBidLineList;
		public static SummaryViewController ObjSummaryList;
		public static SubmitBidViewController ObjSubmitView;
		public static BidEditorForFA ObjBidEditorFA;
		public static BidEditorForPilot ObjBidEditorPilot;
		
		public static string IsModernScrollClassic = "";
		public static ModernContainerViewController ObjModernList;
		public static string isVPSServer="";
		public static List<int> selectedRows = new List<int> ();

		public static int selectedLine { get; set; }

		public static bool showGrid { get; set; }

		public static int columnID { get; set; }

		public static bool columnAscend { get; set; }

		public static NSObject bidObserver { get; set; }

		public static ObservableCollection<CalendarData> calData = new ObservableCollection<CalendarData> ();

		public static List<int> doubleTapLine = new List<int> ();

		public static ObservableCollection<TripData> tripData = new ObservableCollection<TripData> ();

		public static bool isLastTrip { get; set; }

		public static bool weightSelected { get; set; }

		public static string MainViewType { get; set; }

		public static string selectedTrip { get; set; }

		public static List<string> bidLineProperties = new List<string> ();

		public static List<string> modernProperties = new List<string> ();

		public static lineViewController lineVC { get; set; }

		public static CSWViewController cswVC { get; set; }
		public static BAViewController BAVC{ get; set; }
		public static List<Absense> MILList = new List<Absense> ();

		public static string[] CellCarrier = {
			"",
			"ATandT",
			"Cingular",
			"Metro_PCS",
			"Nextel",
			"Other",
			"Sprint",
			"Tmobile",
			"Verizon",
			"Virgin_Mobile"
		};
        public static string[] Months = {
            "",
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"
        };
        public static string GetLineProperty (string displayName, Line line)
		{
			if (displayName == "Line")
			{
				return line.LineDisplay;
			}
			else if (displayName == "$/Day")
			{
				return string.Format("{0:0.00}", line.TfpPerDay);
			}
			else if (displayName == "$/DHr")
			{
				return string.Format("{0:0.00}", line.TfpPerDhr);
			}
			else if (displayName == "$/Hr")
			{
				return string.Format("{0:0.00}", line.TfpPerFltHr);
			}
			else if (displayName == "$/TAFB")
			{
				return line.TfpPerTafb.ToString();
			}
			else if (displayName == "+Grd")
			{
				return line.LongestGrndTime.ToString();
			}
			else if (displayName == "+Legs")
			{
				return line.MostLegs.ToString();
			}
			else if (displayName == "+Off")
			{
				return line.LargestBlkOfDaysOff.ToString();
			}
			else if (displayName == "1Dy")
			{
				return line.Trips1Day.ToString();
			}
			else if (displayName == "2Dy")
			{
				return line.Trips2Day.ToString();
			}
			else if (displayName == "3Dy")
			{
				return line.Trips3Day.ToString();
			}
			else if (displayName == "4Dy")
			{
				return line.Trips4Day.ToString();
			}
			else if (displayName == "787m8m")
			{
				return line.Equip8753.ToString();
			}
			else if (displayName == "A/P")
			{
				return line.AMPM.ToString();
			}
			else if (displayName == "ACChg")
			{
				return line.AcftChanges.ToString();
			}
			else if (displayName == "ACDay")
			{
				return line.AcftChgDay.ToString();
			}
			else if (displayName == "CO")
			{
				return line.CarryOverTfp.ToString();
			}
			else if (displayName == "DP")
			{
				return line.TotDutyPds.ToString();
			}
			else if (displayName == "DPinBP")
			{
				return line.TotDutyPdsInBp.ToString();
			}
			else if (displayName == "EDomPush")
			{
				return (line.EDomPush != null) ? line.EDomPush : string.Empty;
			}
			else if (displayName == "EPush")
			{
				return (line.EPush != null) ? line.EPush : string.Empty;
			}
			else if (displayName == "FA Posn")
			{
				return string.Join("", line.FAPositions.ToArray());
			}
			else if (displayName == "Flt")
			{
				return (line.BlkHrsInBp != null) ? line.BlkHrsInBp : string.Empty;
			}
			else if (displayName == "LArr")
			{
				return line.LastArrTime.ToString(@"hh\:mm");
			}
			else if (displayName == "AEDP")
			{
				return line.AvgEarliestDomPush.ToString(@"hh\:mm");
			}
			else if (displayName == "ALDA")
			{
				return line.AvgLatestDomArrivalTime.ToString(@"hh\:mm");

			}
			else if (displayName == "LDomArr")
			{
				return line.LastDomArrTime.ToString(@"hh\:mm");
			}
			else if (displayName == "Legs")
			{
				return line.Legs.ToString();
			}
			else if (displayName == "LgDay")
			{
				return line.LegsPerDay.ToString();
			}
			else if (displayName == "LgPair")
			{
				return line.LegsPerPair.ToString();
			}
			else if (displayName == "ODrop")
			{
				return line.OverlapDrop.ToString();
			}
			else if (displayName == "Off")
			{
				return line.DaysOff.ToString();
			}
			else if (displayName == "Pairs")
			{
				return line.TotPairings.ToString();
			}
			else if (displayName == "Pay" || displayName == "TotPay")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.Tfp, 2));
			}
			else if (displayName == "PDiem")
			{
				return (line.TafbInBp != null) ? line.TafbInBp : string.Empty;
			}
			else if (displayName == "MyValue")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.Points, 2));
			}
			else if (displayName == "SIPs")
			{
				return line.Sips.ToString();
			}
			else if (displayName == "StartDOW")
			{
				return (line.StartDow != null) ? line.StartDow : string.Empty;
			}
			else if (displayName == "T234")
			{
				return (line.T234 != null) ? line.T234 : string.Empty;
			}
			else if (displayName == "VDrop")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.VacationDrop, 2));
			}
			else if (displayName == "WkEnd")
			{
				return (line.Weekend != null) ? line.Weekend.ToLower() : string.Empty;
			}
			else if (displayName == "FltRig")
			{
				return line.RigFltInBP.ToString();
			}
			else if (displayName == "MinPayRig")
			{
				return line.RigDailyMinInBp.ToString();
			}
			else if (displayName == "DhrRig")
			{
				return line.RigDhrInBp.ToString();
			}
			else if (displayName == "AdgRig")
			{
				return line.RigAdgInBp.ToString();
			}
			else if (displayName == "TafbRig")
			{
				return line.RigTafbInBp.ToString();
			}
			else if (displayName == "TotRig")
			{
				return line.RigTotalInBp.ToString();
			}
            else if (displayName == "VacPayCu")
            {
                return string.Format("{0:0.00}", Decimal.Round(line.VacPayCuBp, 2));
            }
            else if (displayName == "VacPayNe")
            {
                return string.Format("{0:0.00}", Decimal.Round(line.VacPayNeBp, 2));
            }
            else if (displayName == "VacPayBo")
            {
                return string.Format("{0:0.00}", Decimal.Round(line.VacPayBothBp, 2));
            }
			else if (displayName == "Vofrnt")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.VacationOverlapFront, 2));
			}
			else if (displayName == "Vobk")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.VacationOverlapBack, 2));
			}
			else if (displayName == "8Max")
			{
				return line.LegsIn600.ToString();
			}
			else if (displayName == "7Max")
			{
				return line.LegsIn200.ToString();
			}
			else if (displayName == "800legs")
			{
				return line.LegsIn800.ToString();
			}
			else if (displayName == "700legs")
			{
				return line.LegsIn700.ToString();
			}
			//else if (displayName == "500legs")
			//{
			//	return line.LegsIn500.ToString();
			//}
			//else if (displayName == "300legs")
			//{
			//	return line.LegsIn300.ToString();
			//}
			else if (displayName == "DhrInBp")
			{
				return (line.DutyHrsInBp != null) ? line.DutyHrsInBp : string.Empty;
			}
			else if (displayName == "DhrInLine")
			{
				return (line.DutyHrsInLine != null) ? line.DutyHrsInLine : string.Empty;
			}
			else if (displayName == "Wts")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.TotWeight, 2));
			}
			else if (displayName == "LineRig")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.LineRig, 2));
			}
			else if (displayName == "FlyPay")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.FlyPay, 2));
			}
			else if (displayName == "Tag")
			{
				return (line.Tag != null) ? line.Tag : string.Empty;

			}
			else if (displayName == "HolRig")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.HolRig, 2));
			}
			else if (displayName == "Grp")
			{
				return (line.BAGroup != null) ? line.BAGroup : string.Empty;
			}
			else if (displayName == "nMid")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.NightsInMid, 2));

			}
			else if (displayName == "cmts")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.TotalCommutes, 2));
			}
			else if (displayName == "cmtFr")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.commutableFronts, 2));

			}
			else if (displayName == "cmtBa")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.CommutableBacks, 2));

			}
			else if (displayName == "cmt%Fr")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.CommutabilityFront, 2));

			}
			else if (displayName == "cmt%Ba")
			{
				return string.Format("{0:0.00}", Decimal.Round (line.CommutabilityBack, 2));

			}
			else if (displayName == "cmt%Ov") {
				return string.Format ("{0:0.00}", Decimal.Round (line.CommutabilityOverall, 2));

			}
			else if (displayName == "Ratio")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.Ratio, 2));
			}
            else if (displayName == "Vac+LG")
            {
                return string.Format("{0:0.00}", Decimal.Round(line.VacPlusRig, 2));
            }
			else if (displayName == "VAbo")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.VAbo, 2));
			}
			else if (displayName == "VAbp")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.VAbp, 2));
			}
			else if (displayName == "VAne")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.VAne, 2));
			}
			else if (displayName == "VAPbo")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.VAPbo, 2));
			}
			else if (displayName == "VAPbp")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.VAPbp, 2));
			}
			else if (displayName == "VAPne")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.VAPne, 2));
			}
			else if (displayName == "Etrips")
			{
				return line.ETOPSTripsCount.ToString();
			}
			else if (displayName == "DHFirst")
			{
				return line.DhFirstTotal.ToString();
			}
			else if (displayName == "DHLast")
			{
				return line.DhLastTotal.ToString();
			}
			else if (displayName == "DHTotal")
			{
				return line.DhTotal.ToString();
			}
			else {
				return "";
			}
		}
		public static bool isUserInformationAvailable()
		{
			//return true;

			if (GlobalSettings.WbidUserContent != null && GlobalSettings.WbidUserContent.UserInformation != null) {
				if (GlobalSettings.WbidUserContent.UserInformation.PaidUntilDate!=null) {
					return true;
				} 
			}
			else
				return false;
		return false;
		}

		public static bool isSubScriptionOnlyFor5Days()
		{
			bool IsUserdataAvailable=	CommonClass.isUserInformationAvailable ();
		
			if (IsUserdataAvailable) 
			{
				DateTime PaidUntilDate =  GlobalSettings.WbidUserContent.UserInformation.PaidUntilDate ?? DateTime.Now ;
				int days = DaysBetween (DateTime.Now,PaidUntilDate );
				//days = 2;
				if (days > 0 && days < 5) {
					return true;
				}

			}
			return false;

		}
		public static int DaysBetween(DateTime d1, DateTime d2) {
			TimeSpan span = d2.Subtract(d1);
			return (int)span.TotalDays;
		}
		public static T ConvertJSonToObject<T> (string jsonString)
		{
			DataContractJsonSerializer serializer = new DataContractJsonSerializer (typeof(T));
			MemoryStream ms = new MemoryStream (Encoding.UTF8.GetBytes (jsonString));
			T obj = (T)serializer.ReadObject (ms);
			return obj;
		}
		public static string FormatBidReceipt (string receipt)
		{
			StringBuilder resultHeadingString = new StringBuilder ();
			List<StringBuilder> content = new List<StringBuilder> ();

			List<PrintBidReceipt> lstPrintBidReceipt = new List<PrintBidReceipt> ();
			string lineString = string.Empty;
			string lineType = "header";

			resultHeadingString.Append ("WBidMax Formatted Bid Receipt" + Environment.NewLine);
			resultHeadingString.Append ("Employee Number :");

			int count = 1;

			using (StreamReader reader = new StreamReader (receipt)) {
				while ((lineString = reader.ReadLine ()) != null) {
					// lineString = reader.ReadLine();
					if (lineType == "header") {
						resultHeadingString.Append (lineString + Environment.NewLine);
						resultHeadingString.Append ("Bid Receipt File :" + receipt + Environment.NewLine);
						resultHeadingString.Append ("Receipt File Dated :" + DateTime.Now.ToLongDateString () + "(Local)" + Environment.NewLine + Environment.NewLine);
						lineType = "body";
					} else if (lineType == "body") {
						int num;
						if (int.TryParse (lineString.Substring (0, 1), out num)) {
							lstPrintBidReceipt.Add (new PrintBidReceipt () { LineOrder = count++, LineNum = lineString });

						} else {
							lineType = "footer";
							resultHeadingString.Append (lineString + Environment.NewLine);

						}

					} else if (lineType == "footer") {
						resultHeadingString.Append (lineString + Environment.NewLine + Environment.NewLine);

					}

				}

			}

			int startvalu = 0;
			int index = 0;
			int itemPercolumn = 67;

			StringBuilder singlePageContent = new StringBuilder ();
			string lineStr = string.Empty;
			int bidReceiptIndex;
			while (index + startvalu < lstPrintBidReceipt.Count) {
				for (int cnt = 0; cnt < 9; cnt++) {
					bidReceiptIndex = startvalu + index + (cnt * itemPercolumn);

					if (bidReceiptIndex < lstPrintBidReceipt.Count) {
						lineStr = lstPrintBidReceipt [bidReceiptIndex].LineOrder.ToString ().PadLeft (3, ' ') + ". " + lstPrintBidReceipt [bidReceiptIndex].LineNum;
						singlePageContent.Append (lineStr.PadRight (13, ' '));
					} else {
						break;
					}
				}
				singlePageContent.Append (Environment.NewLine);

				index++;
				if (index == itemPercolumn) {
					index = 0;
					startvalu = startvalu + index + (9 * itemPercolumn);
					itemPercolumn = 72;
					content.Add (singlePageContent);
					singlePageContent = new StringBuilder ();
				}
			}

			if (singlePageContent.ToString ().Trim () != string.Empty) {
				content.Add (singlePageContent);

			}

			PDfParams pdfParams = new PDfParams
			{
				Author = "WBidMax",
				Creator = "WBidmax",
				FileName = WBidHelper.GetAppDataPath() + "\\" + "test.pdf",
				Subject = "Bid Receipt",
				Title = "Bid Receipt"
			};

			var str = "\n\n" + resultHeadingString.ToString () + "\n\n";
			str += string.Join ("", content.ToList ().ConvertAll (x => x.ToString ()));
			return str;
		}

        public static bool SaveFormatBidReceipt (string result)
		{
            try
            {
                //			result = string.Empty;
                //			result = "22028\n";
                //			
                //			for (int i = 10; i < 1000; i++) {
                //				result += i + "\n";
                //			}
                string lineString = string.Empty;
                string employeename = result.Substring(0, result.IndexOf("\n"));
                string fileName = employeename + "Rct";
                string footer = string.Empty;

                List<string> lists = result.Split('*').ToList();
                lists.RemoveAt(0);
                foreach (var item in lists)
                {
                    footer += item;
                }
                StringBuilder resultHeadingString = new StringBuilder();

                List<StringBuilder> content = new List<StringBuilder>();

                List<PrintBidReceipt> lstPrintBidReceipt = new List<PrintBidReceipt>();
                List<string> submit = result.Split('\n').ToList();
                submit.RemoveAt(0);
                int count = 1;
                foreach (string item in submit)
                {
                    if (item.Contains('*'))
                        break;
                    lstPrintBidReceipt.Add(new PrintBidReceipt() { LineOrder = count++, LineNum = item });

                }
                resultHeadingString.Append(lineString + Environment.NewLine + Environment.NewLine);

                int startvalu = 0;
                int index = 0;
                int itemPercolumn = 65;

                StringBuilder singlePageContent = new StringBuilder();
                singlePageContent.Append("WBidMax Formatted Bid Receipt" + Environment.NewLine);
                singlePageContent.Append("Employee Number : " + employeename + Environment.NewLine);
                //singlePageContent.Append ("Bid Receipt File :" + WBidHelper.GetAppDataPath () + "/" + fileName + Environment.NewLine);
                singlePageContent.Append("Receipt File Dated :" + DateTime.Now.ToLongDateString() + "(Local)" + Environment.NewLine);
                singlePageContent.Append(footer + Environment.NewLine);
                string lineStr = string.Empty;
                int bidReceiptIndex;
                while (index + startvalu < lstPrintBidReceipt.Count)
                {
                    for (int cnt = 0; cnt < 7; cnt++)
                    {
                        bidReceiptIndex = startvalu + index + (cnt * itemPercolumn);

                        if (bidReceiptIndex < lstPrintBidReceipt.Count)
                        {
                            lineStr = lstPrintBidReceipt[bidReceiptIndex].LineOrder.ToString().PadLeft(3, ' ') + ". " + lstPrintBidReceipt[bidReceiptIndex].LineNum.PadRight(3, ' ');
                            singlePageContent.Append(lineStr.PadRight(15, ' '));
                        }
                        else
                        {
                            break;
                        }
                    }
                    singlePageContent.Append(Environment.NewLine);

                    index++;
                    if (index == itemPercolumn)
                    {
                        index = 0;
                        startvalu = startvalu + index + (7 * itemPercolumn);
                        itemPercolumn = 62;
                        content.Add(singlePageContent);
                        singlePageContent = new StringBuilder();
                    }
                }

                if (singlePageContent.ToString().Trim() != string.Empty)
                {
                    content.Add(singlePageContent);

                }

                PDfParams pdfParams = new PDfParams
                {
                    Author = "WBidMax",
                    Creator = "WBidmax",
                    FileName = WBidHelper.GetAppDataPath() + "/" + employeename + "Rct.pdf",
                    Subject = "Bid Receipt",
                    Title = "Bid Receipt"
                };

                //var str = "\n\n" + resultHeadingString.ToString () + "\n\n";
                //str += string.Join ("", content.ToList ().ConvertAll (x => x.ToString ()));
                //System.IO.File.WriteAllText (WBidHelper.GetAppDataPath() + "/" + employeename+".RCT", str);
                CreatePDF(pdfParams, content);
            }catch(Exception ex)
            {
                return false;
            }
            return true;
		}

		public static void CreatePDF (PDfParams PDfParams, List<StringBuilder> ls)
		{
//			FileStream fileStream = new FileStream(PDfParams.FileName, FileMode.Create, FileAccess.Write, FileShare.None);
//			Document document = new Document();
//
//			if (!String.IsNullOrEmpty(PDfParams.Title))
//			{
//				document.AddTitle(PDfParams.Title);
//			}
//			if (!String.IsNullOrEmpty(PDfParams.Subject))
//			{
//				document.AddSubject(PDfParams.Subject);
//			}
//			if (!String.IsNullOrEmpty(PDfParams.Creator))
//			{
//				document.AddCreator(PDfParams.Creator);
//			}
//			if (!String.IsNullOrEmpty(PDfParams.Author))
//			{
//				document.AddAuthor(PDfParams.Author);
//			}
//			document.AddHeader("Nothing", "No Header");
//			PdfWriter writer = PdfWriter.GetInstance(document, fileStream);
//
//			document.Open();
//
//			iTextSharp.text.Font font = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.COURIER,8f,0);
//
//
//			foreach (var item in ls)
//			{
//				document.Add(new Paragraph(item.ToString(), font));
//				document.NewPage();
//			}
//
//			document.Close();
			string pdfPath = PDfParams.FileName;
			UIGraphics.BeginPDFContext (pdfPath, CGRect.Empty, new Foundation.NSDictionary ());

			foreach (var pg in ls) {
				var text = pg.ToString ();
				UIGraphics.BeginPDFPage (new CGRect (0, 0, 612, 792), new Foundation.NSDictionary ());
				UIFont font = UIFont.FromName ("Courier", 9);
				CGSize stringSize = text.StringSize (font, new CGSize (550, 700), UILineBreakMode.WordWrap);
				CGRect renderingRect = new CGRect (20, 20, 550, stringSize.Height);
				text.DrawString (renderingRect, font, UILineBreakMode.WordWrap);
			}

			UIGraphics.EndPDFContent ();

		}

		/// <summary>
		/// Print the pdf File
		/// </summary>

		public static void PrintReceipt(UIView Cell, string selectedFileName)
		{

			if (Path.GetExtension (selectedFileName).ToLower () == ".pdf") {
				var printInfo = UIPrintInfo.PrintInfo;

				printInfo.Duplex = UIPrintInfoDuplex.LongEdge;

				printInfo.OutputType = UIPrintInfoOutputType.General;

				printInfo.JobName = "WBidMax Receipt Print job";

				var printer = UIPrintInteractionController.SharedPrintController;

				printer.PrintInfo = printInfo;
				Console.WriteLine ("Printer path" + Path.GetFileName (selectedFileName));
				printer.PrintingItem = NSData.FromFile (WBidHelper.GetAppDataPath () + "/" + selectedFileName);
				printer.ShowsPageRange = true;
				printer.PresentFromRectInView (Cell.Bounds, Cell, true, (handler, completed, err) => {
					if (!completed && err != null) {
						Console.WriteLine ("error");
					} else if (completed) {
                        UIWindow WindowAlert = new UIWindow(UIScreen.MainScreen.Bounds);
                        WindowAlert.RootViewController = new UIViewController();
                        UIAlertController okAlertController = UIAlertController.Create("WBidMax", "The line has been sent for printout.", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        WindowAlert.MakeKeyAndVisible();
                        WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
                        WindowAlert.Dispose();
                    }
				});
			} else 
			{
				
				string content = FormatBidReceipt (WBidHelper.GetAppDataPath () + "/" + selectedFileName);
				var attributes = new UIStringAttributes () { Font = UIFont.FromName ("Courier", 7) };
				var printDoc = new NSAttributedString (content, attributes);

				//			var textVW = new UITextView(this.View.Frame);
				//			textVW.AttributedText = printDoc;
				//			this.Add(textVW);
				//
				//			return;

				var printInfo = UIPrintInfo.PrintInfo;
				printInfo.OutputType = UIPrintInfoOutputType.General;
				printInfo.JobName = "WBidMax Receipt Print job";

				var textFormatter = new UISimpleTextPrintFormatter (printDoc) {
					StartPage = 0,
					ContentInsets = new UIEdgeInsets (5, 5, 5, 5),
					//MaximumContentWidth = 6 * 72,
				};

				var printer = UIPrintInteractionController.SharedPrintController;
				printer.PrintInfo = printInfo;
				printer.PrintFormatter = textFormatter;
				printer.ShowsPageRange = true;
				printer.PresentFromRectInView (Cell.Bounds, Cell, true, (handler, completed, err) => {
					if (!completed && err != null) {
						Console.WriteLine ("error");
					} else if (completed) {
					    UIWindow WindowAlert = new UIWindow(UIScreen.MainScreen.Bounds);
                        WindowAlert.RootViewController = new UIViewController();
                        UIAlertController okAlertController = UIAlertController.Create("WBidMax", "The line has been sent for printout.", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        WindowAlert.MakeKeyAndVisible();
                        WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
                        WindowAlert.Dispose();
                    }
				});
			}
		}


	
		public class PrintBidReceipt
		{
			public int LineOrder { get; set; }

			public string LineNum { get; set; }

		}

		public class PDfParams
		{

			public string FileName { get; set; }

			public StringBuilder FileContent { get; set; }

			public string Title { get; set; }

			public string Subject { get; set; }

			public string Creator { get; set; }

			public string Author { get; set; }




		}
	}
}

