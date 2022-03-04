using System;
using CoreGraphics;
using Foundation;
using UIKit;
using System.IO;
using WBid.WBidiPad.iOS.Utility;
using System.Collections.Generic;
using System.Linq;
using WBid.WBidiPad.Model;
using System.Threading.Tasks;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.PortableLibrary;
using System.Collections.ObjectModel;
using CoreGraphics;
using WBid.WBidiPad.SharedLibrary.Utility;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using System.Text.RegularExpressions;
using WBid.WBidiPad.PortableLibrary.Utility;
using System.Json;
using Newtonsoft.Json;
using System.Reflection;
using WBid.WBidiPad.Core.Enum;

namespace WBid.WBidiPad.iOS
{
    public class bidDataCollectionController : UICollectionViewController,IServiceDelegate
	{
		List<RecentFile> recentFiles;
		LoadingOverlay loadingOverlay;
		public bool shouldJiggle = false;
		WbidUser User;
		//NSObject notif;
		NSObject newNotif;
        NSObject notif1;

        public bidDataCollectionController (UICollectionViewLayout layout)
			: base (layout)
		{
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
//            if (File.Exists(WBidHelper.WBidUserFilePath))
//            {
//                User = (WbidUser)XmlHelper.DeserializeFromXml<WbidUser>(WBidHelper.WBidUserFilePath);
//            }

			recentFiles = GetExistingDataInAppData ();
			CollectionView.RegisterClassForCell (typeof(BidCollectionCell), BidCollectionCell.Key);
            observeNotifications();
			this.CollectionView.BackgroundColor = UIColor.White;
			this.View.Layer.BorderWidth = 1;
			this.View.Layer.BorderColor = ColorClass.SummaryHeaderBorderColor.CGColor;
			this.View.Layer.CornerRadius = 3;
		}
        public void ResponceError(string Error)
        {
            InvokeOnMainThread(() => {
                //ActivityIndicator.Hide();
                Console.WriteLine("Service Fail");

                UIAlertController okAlertController = UIAlertController.Create("WBidMax", Error, UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
            });
        }
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		

		}

		public override void ViewDidDisappear (bool animated)
		{

            base.ViewDidDisappear (animated);
			if (newNotif != null) {
				NSNotificationCenter.DefaultCenter.RemoveObserver (newNotif);
                NSNotificationCenter.DefaultCenter.RemoveObserver(notif1);
                newNotif = null;
			}
		}

		private void observeNotifications ()
		{
			//notif = NSNotificationCenter.DefaultCenter.AddObserver("homeCollectionCellDelateButtonTapped", cellDeleteButtonTapped);
			newNotif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("HandleBidDelete"), HandleDelete);
            notif1 = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("HandleReload"), HandleReload);

        }
        void HandleReload(NSNotification obj) {
            recentFiles = GetExistingDataInAppData();
            this.CollectionView.ReloadData();
        }


        void HandleDelete (NSNotification obj)
		{
			recentFiles = GetExistingDataInAppData ();
			int strBid = Convert.ToInt32 (obj.Object.ToString ());
			if(recentFiles.Count>strBid)
				{
			RecentFile fileTodelete = recentFiles [strBid];
			string message = fileTodelete.MonthDisplay + " " + fileTodelete.Year + "\n" + fileTodelete.Domcile + "-" + fileTodelete.Position + "-" + fileTodelete.Round + "\n Do you want to delete this Bid?";
			
                UIAlertController alert = UIAlertController.Create("WBidMax", message, UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("NO", UIAlertActionStyle.Cancel, (actionCancel) => {

                }));

                alert.AddAction(UIAlertAction.Create("YES", UIAlertActionStyle.Default, (actionOK) => {
                    DeleteBidPeriod(fileTodelete.Domcile, fileTodelete.Position, fileTodelete.Round, fileTodelete.Month, Convert.ToInt32(fileTodelete.Year));
                    recentFiles.Remove(fileTodelete);
					WBidHelper.SaveUserFile(GlobalSettings.WbidUserContent, WBidHelper.WBidUserFilePath);
					CollectionView.ReloadData();

                }));

                this.PresentViewController(alert, true, null);

            }
        }

		//        private void cellDeleteButtonTapped(NSNotification n)
		//        {
		//            bidDataCollectionCell cell = (bidDataCollectionCell)n.Object;
		//            recentFiles = GetExistingDataInAppData();
		//            //			string strIndex = index.ToString ();
		//            RecentFile fileTodelete = recentFiles[cell.Tag];
		//
		//            //Delete this RecentFile, update recent files array above this line.
		//
		//            DeleteBidPeriod(fileTodelete.Domcile, fileTodelete.Position, fileTodelete.Round, fileTodelete.Month, Convert.ToInt32(fileTodelete.Year));
		//            recentFiles.Remove(fileTodelete);
		//
		//            CGAffineTransform back = CGAffineTransform.MakeRotation(degreesToRadians(0));
		//            cell.Transform = back;
		//            shouldJiggle = false;
		//			cell.endJiggling ();
		//            this.CollectionView.ReloadData();
		//
		//        }
		//        private float degreesToRadians(float x)
		//        {
		//            return (3.14f * x / 180f);
		//        }
		public override nint NumberOfSections (UICollectionView collectionView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			// TODO: return the actual number of items in the section
			if (recentFiles.Count == 0)
				NSNotificationCenter.DefaultCenter.PostNotificationName ("EditEnableDisable", new NSString ("disable"));
			else
				NSNotificationCenter.DefaultCenter.PostNotificationName ("EditEnableDisable", new NSString ("enable"));
			return recentFiles.Count;
			//return 10;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			collectionView.RegisterNibForCell (UINib.FromName ("BidCollectionCell", NSBundle.MainBundle), new NSString ("BidCollectionCell"));
			var cell = collectionView.DequeueReusableCell (BidCollectionCell.Key, indexPath) as BidCollectionCell;
			RecentFile aFile = recentFiles [indexPath.Row];
			cell.BindData (aFile, indexPath, shouldJiggle);
			cell.DoJiggle (shouldJiggle);
			if (shouldJiggle)
				collectionView.AllowsSelection = false;
			else
				collectionView.AllowsSelection = true;
			if (File.Exists (WBidHelper.WBidUserFilePath)) {
				User = (WbidUser)XmlHelper.DeserializeFromXml<WbidUser> (WBidHelper.WBidUserFilePath);
				cell.setBackImage (aFile.Position, User.UserInformation.IsFemale);
			} else {
				cell.setBackImage (aFile.Position, true);
			}
		

			//UILongPressGestureRecognizer lngPress = new UILongPressGestureRecognizer(handleLngPress);
			//cell.AddGestureRecognizer(lngPress);

			//			cell.title = new NSString(aFile.MonthDisplay + " " + aFile.Year);
			//            cell.subTitle = new NSString(aFile.Domcile + "-" + aFile.Position + "-" + aFile.Round);
			//            cell.tag = indexPath.Row;
			//            // TODO: populate the cell with the appropriate data based on the indexPath
			//addLongPressToCell(cell);
			//
			//			if (shouldJiggle) {
			//				cell.startJiggling ();
			//				collectionView.AllowsSelection = false;
			//			} else {
			//				CGAffineTransform back = CGAffineTransform.MakeRotation (degreesToRadians (0));
			//				cell.Transform = back;
			//				cell.endJiggling ();
			//				collectionView.AllowsSelection = true;
			//			}
			//
			return cell;
		}
		//        private void addLongPressToCell(bidDataCollectionCell cell)
		//        {
		//            UILongPressGestureRecognizer lngPress = new UILongPressGestureRecognizer(handleLngPress);
		//            cell.AddGestureRecognizer(lngPress);
		//        }
		//
		//        private void handleLngPress(UILongPressGestureRecognizer lngPress)
		//        {
		//            if (lngPress.State == UIGestureRecognizerState.Began)
		//            {
		//                Console.WriteLine("Long press on cell detected");
		//                shouldJiggle = true;
		//                this.CollectionView.ReloadData();
		//
		//            }
		//        }


		public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
		{
			try
			{
				this.ParentViewController.View.UserInteractionEnabled = false;
				//collectionView.AllowsSelection = false;
				Console.WriteLine("Collection ItemSelected");
				RecentFile aFile = recentFiles[indexPath.Row];

				string round = (aFile.Round == "1st Round") ? "M" : "S";
				//genarate the filename
				string filename = aFile.Domcile + aFile.Position + aFile.Month.ToString().PadLeft(2, '0') + (Convert.ToInt16(aFile.Year) - 2000) + round + "737";
				WBidHelper.SetCurrentBidInformationfromStateFileName(filename);

				//Write to currentBidDetailsfile for Error log
				FileOperations.WriteCurrentBidDetails(WBidHelper.GetAppDataPath() + "/CurrentDetails.txt", WBidHelper.GetApplicationBidData());

				string zipFilename = WBidHelper.GenarateZipFileName();


				UICollectionViewCell selectedCell = collectionView.CellForItem(indexPath);
				loadingOverlay = new LoadingOverlay(selectedCell.Frame, "");
				View.Add(loadingOverlay);

				//load the line file data
				Task task = Task.Run(() =>
				{

					if (!File.Exists(WBidHelper.GetAppDataPath() + "/" + filename + ".WBS"))
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
							WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
							obgWBidLogEvent.LogAllEvents(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "dwcRecreate", "0", "0", "");


						}
						GlobalSettings.WBidStateCollection = WBidCollection.CreateStateFile(WBidHelper.GetAppDataPath() + "/" + filename + ".WBS", 400, 1, wbidintialState);
						WBidHelper.SaveStateFile(filename);

					}
					else
					{

						try
						{
							//Read State file content and Stored it into an object
							GlobalSettings.WBidStateCollection = XmlHelper.ReadStateFile(WBidHelper.GetAppDataPath() + "/" + filename + ".WBS");

							//XmlHelper.DeserializeFromXml<WBidStateCollection>(WBidHelper.GetAppDataPath() + "/" + filename + ".WBS");
						}
						catch (Exception ex)
						{
							//Recreate state file
							//--------------------------------------------------------------------------------
							WBidIntialState wbidintialState = null;
							WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
							try
							{

								wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
							}
							catch (Exception exx)
							{
								wbidintialState = WBidCollection.CreateDWCFile(GlobalSettings.DwcVersion);
								XmlHelper.SerializeToXml(wbidintialState, WBidHelper.GetWBidDWCFilePath());
								//WBidHelper.LogDetails(GlobalSettings.WbidUserContent.UserInformation.EmpNo,"dwcRecreate","0","0");

								obgWBidLogEvent.LogAllEvents(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "dwcRecreate", "0", "0", "");

							}
							GlobalSettings.WBidStateCollection = WBidCollection.CreateStateFile(WBidHelper.GetAppDataPath() + "/" + filename + ".WBS", 400, 1, wbidintialState);
							WBidHelper.SaveStateFile(filename);
							//WBidHelper.LogDetails(GlobalSettings.WbidUserContent.UserInformation.EmpNo,"wbsRecreate","0","0");


							//obgWBidLogEvent.LogAllEvents(GlobalSettings.WbidUserContent.UserInformation.EmpNo,"wbsRecreate","0","0");
							//GlobalSettings.WBidStateCollection =XmlHelper.ReCreateStateFile(filename,GlobalSettings.Lines.Count,GlobalSettings.Lines.First ().LineNum);
						}
					}

					if (GlobalSettings.WBidStateCollection.SubmittedResult != null && GlobalSettings.WBidStateCollection.SubmittedResult != string.Empty)
					{


						//update submit result
						if (Reachability.CheckVPSAvailable())
						{
							OdataBuilder ObjOdata = new OdataBuilder();
							ObjOdata.RestService.Objdelegate = this;
							BidSubmittedData objbiddetails = new BidSubmittedData();
							objbiddetails.Domicile = GlobalSettings.CurrentBidDetails.Domicile;
							objbiddetails.Month = GlobalSettings.CurrentBidDetails.Month;
							objbiddetails.Year = GlobalSettings.CurrentBidDetails.Year;
							objbiddetails.Position = GlobalSettings.CurrentBidDetails.Postion;
							objbiddetails.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
							objbiddetails.EmpNum = Convert.ToInt32(GlobalSettings.WbidUserContent.UserInformation.EmpNo);
							ObjOdata.GetBidSubmittedData(objbiddetails);
						}

					}
					//Check the WBL file contains 737
					//if it contrains 737, desealize using protbug
					//seralize using lz compression. remove 737 file.
					//load WBL and Load WBP using LZ compression.

					TripInfo tripInfo = null;
					LineInfo lineInfo = null;
					Model.OldLine.LineInfo oldlineInfo = null;
					var newFileName = filename.Substring(0, 10);
					string filePathOldWBP = WBidHelper.GetAppDataPath() + "/" + filename + ".WBP";
					string filePathNewWBP = WBidHelper.GetAppDataPath() + "/" + newFileName + ".WBP";
					string filePathOldWBL = WBidHelper.GetAppDataPath() + "/" + filename + ".WBL";
					string filePathNewWBL = WBidHelper.GetAppDataPath() + "/" + newFileName + ".WBL";

					if (File.Exists(filePathOldWBP) || File.Exists(filePathNewWBP))
					{
						try
						{
							var compressedData = File.ReadAllText(WBidHelper.GetAppDataPath() + "/" + newFileName + ".WBP");
							string tripfileJsoncontent = LZStringCSharp.LZString.DecompressFromUTF16(compressedData);

							//desrialise the Json
							Dictionary<string, Trip> wbpLine = WBidCollection.ConvertJSonStringToObject<Dictionary<string, Trip>>(tripfileJsoncontent);

							GlobalSettings.Trip = new ObservableCollection<Trip>(wbpLine.Values);
						}
						catch (Exception ex)
						{
							try
							{
								using (FileStream tripStream = File.OpenRead(WBidHelper.GetAppDataPath() + "/" + filename + ".WBP"))
								{
									TripInfo objTripInfo = new TripInfo();
									tripInfo = ProtoSerailizer.DeSerializeObject(WBidHelper.GetAppDataPath() + "/" + filename + ".WBP", objTripInfo, tripStream);

								}
								var jsonString = JsonConvert.SerializeObject(tripInfo.Trips);

								//Lz compression
								var compressedData = LZStringCSharp.LZString.CompressToUTF16(jsonString);

								File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + newFileName + ".WBP", compressedData);

								//desrialise the Json
								Dictionary<string, Trip> wbpLine = WBidCollection.ConvertJSonStringToObject<Dictionary<string, Trip>>(jsonString);

								GlobalSettings.Trip = new ObservableCollection<Trip>(wbpLine.Values);

								File.Delete(filePathOldWBP);
							}
							catch (Exception e)
							{
								throw ex;
							}

						}

					}



					if (File.Exists(filePathOldWBL) || File.Exists(filePathNewWBL))
					{
						try
						{

							var compressedData = File.ReadAllText(WBidHelper.GetAppDataPath() + "/" + newFileName + ".WBL");
							string linefileJsoncontent = LZStringCSharp.LZString.DecompressFromUTF16(compressedData);

							//desrialise the Json
							LineInfo wblLine = WBidCollection.ConvertJSonStringToObject<LineInfo>(linefileJsoncontent);


							GlobalSettings.Lines = new ObservableCollection<Line>(wblLine.Lines.Values);
						}
						catch (Exception ex)
						{
							try
							{
								using (FileStream lineStream = File.OpenRead(WBidHelper.GetAppDataPath() + "/" + filename + ".WBL"))
								{
									Model.OldLine.LineInfo ObjlineInfo = new Model.OldLine.LineInfo();
									oldlineInfo = ProtoSerailizer.DeSerializeObject(WBidHelper.GetAppDataPath() + "/" + filename + ".WBL", ObjlineInfo, lineStream);

								}
								lineInfo = new LineInfo();
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
												if (parentProperty.Name == "FASecondRoundPositions")
												{

												}
												childProperty.SetValue(lst, parentProperty.GetValue(item));



												break;
											}
										}
									}

									if (item.FASecondRoundPositions != null || item.FASecondRoundPositions.Count > 0)
									{
										var fasecondroundpositionsOld = item.FASecondRoundPositions;
										List<FASecondRoundPositions> lstnewFaseconroudnposition = new List<FASecondRoundPositions>();
										FASecondRoundPositions objFaposition;
										foreach (var singleitem in fasecondroundpositionsOld)
										{
											objFaposition = new FASecondRoundPositions();
											objFaposition.key = singleitem.Key;
											objFaposition.Value = singleitem.Value;
											lstnewFaseconroudnposition.Add(objFaposition);
										}
										lst.FASecondRoundPositions = lstnewFaseconroudnposition;
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
								var currentbiddetail = GlobalSettings.CurrentBidDetails;
								AppDataBidFileNames appDataBidFileNames = GlobalSettings.WbidUserContent.AppDataBidFiles.FirstOrDefault(x => x.Domicile == currentbiddetail.Domicile && x.Position == currentbiddetail.Postion && x.Round == currentbiddetail.Round && x.Month == currentbiddetail.Month && x.Year == currentbiddetail.Year);
								if (appDataBidFileNames == null)
								{
                                    GlobalSettings.WbidUserContent.AppDataBidFiles.Add(new AppDataBidFileNames
                                    {
                                        Domicile = currentbiddetail.Domicile,
                                        Month = currentbiddetail.Month,
                                        Position = currentbiddetail.Postion,
                                        Round = currentbiddetail.Round,
                                        Year = currentbiddetail.Year,
                                        lstBidFileNames = new List<BidFileNames>() { new BidFileNames { FileName = newFileName + ".WBL", FileType = (int)BidFileType.NormalLine } }
                                    });

                                    WBidHelper.SaveUserFile(GlobalSettings.WbidUserContent, WBidHelper.WBidUserFilePath);
                                }
								File.Delete(filePathOldWBL);
							}
							catch (Exception ex1)
							{
								throw ex1;
							}

						}

					}


					////======== Old code before server WBL change
					//TripInfo tripInfo = null;
					//LineInfo lineInfo = null;
					//using (FileStream tripStream = File.OpenRead (WBidHelper.GetAppDataPath () + "/" + filename + ".WBP")) {
					//	//int a = 1;
					//	TripInfo objTripInfo = new TripInfo ();
					//	tripInfo = ProtoSerailizer.DeSerializeObject (WBidHelper.GetAppDataPath () + "/" + filename + ".WBP", objTripInfo, tripStream);
					//}

					//GlobalSettings.Trip = new ObservableCollection<Trip> (tripInfo.Trips.Values);
					//if (tripInfo.TripVersion == GlobalSettings.TripVersion) {

					//	using (FileStream linestream = File.OpenRead (WBidHelper.GetAppDataPath () + "/" + filename + ".WBL")) {
					//		//int a = 1;


					//		LineInfo objineinfo = new LineInfo ();
					//		lineInfo = ProtoSerailizer.DeSerializeObject (WBidHelper.GetAppDataPath () + "/" + filename + ".WBL", objineinfo, linestream);

					//	}

					//	if (lineInfo.LineVersion == GlobalSettings.LineVersion) {
					//		GlobalSettings.Lines = new ObservableCollection<Line> (lineInfo.Lines.Values);
					//	} else {
					//		ReparseParameters reparseParams = new ReparseParameters () { ZipFileName = zipFilename };
					//		ReparseBL.ReparseLineFile (reparseParams);
					//	}
					//} else {
					//	ReparseParameters reparseParams = new ReparseParameters () { ZipFileName = zipFilename };
					//	tripInfo.Trips = ReparseBL.ReparseTripAndLineFiles (reparseParams);
					//}

					////------
					// NetworkPlanData networkPlanData = new NetworkPlanData();
					//networkPlanData.GetFlightRoutes(new DateTime(2015, 06, 1), new DateTime(2015, 06, 30));

					//if (tripInfo.TripVersion == GlobalSettings.TripVersion)
					//{

					//    using (FileStream linestream = File.OpenRead(WBidHelper.GetAppDataPath() + "/" + filename + ".WBL"))
					//    {
					//        //int a = 1;
					//        LineInfo objineinfo = new LineInfo();
					//         lineInfo = ProtoSerailizer.DeSerializeObject(WBidHelper.GetAppDataPath() + "/" + filename + ".WBL", objineinfo, linestream);

					//    }
					//    if (lineInfo.LineVersion == GlobalSettings.LineVersion)
					//    {
					//        GlobalSettings.Lines = new ObservableCollection<Line>(lineInfo.Lines.Values);
					//    }
					//    else
					//    {
					//        ReparseParameters reparseParams = new ReparseParameters() { Trips = tripInfo.Trips, ZipFileName = zipFilename };
					//        ReparseBL.ReparseLineFile(reparseParams);
					//    }

					//    foreach (Line line in GlobalSettings.Lines)
					//    {
					//        line.ConstraintPoints = new ConstraintPoints();
					//        line.WeightPoints = new WeightPoints();
					//    }
					//}





					//if (!File.Exists(WBidHelper.GetAppDataPath() + "/" + filename + ".WBS"))
					//{
					//    WBidIntialState wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
					//    GlobalSettings.WBidStateCollection = WBidCollection.CreateStateFile(WBidHelper.GetAppDataPath() + "/" + filename + ".WBS", GlobalSettings.Lines.Count, GlobalSettings.Lines.First().LineNum, wbidintialState);
					//    WBidHelper.SaveStateFile(filename);

					//}
					//else
					//{
					//    //Read State file content and Stored it into an object
					//    GlobalSettings.WBidStateCollection = XmlHelper.ReadStateFile(WBidHelper.GetAppDataPath() + "/" + filename + ".WBS");
					//    //XmlHelper.DeserializeFromXml<WBidStateCollection>(WBidHelper.GetAppDataPath() + "/" + filename + ".WBS");

					//}

					GlobalSettings.CompanyVA = GlobalSettings.WBidStateCollection.CompanyVA;
					if (GlobalSettings.WBidStateCollection.SeniorityListItem != null)
					{
						if (GlobalSettings.WBidStateCollection.SeniorityListItem.SeniorityNumber == 0)
							GlobalSettings.WbidUserContent.UserInformation.SeniorityNumber = GlobalSettings.WBidStateCollection.SeniorityListItem.TotalCount;
						else
							GlobalSettings.WbidUserContent.UserInformation.SeniorityNumber = GlobalSettings.WBidStateCollection.SeniorityListItem.SeniorityNumber;
					}

					WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

					if (wBIdStateContent.Weights.NormalizeDaysOff == null)
					{
						wBIdStateContent.Weights.NormalizeDaysOff = new Wt2Parameter() { Type = 1, Weight = 0 };

					}
					if (wBIdStateContent.CxWtState.NormalizeDays == null)
					{
						wBIdStateContent.CxWtState.NormalizeDays = new StateStatus() { Cx = false, Wt = false };

					}
					if (wBIdStateContent.Constraints.Rest.ThirdcellValue == "")
						wBIdStateContent.Constraints.Rest.ThirdcellValue = "1";

					if (wBIdStateContent.Constraints.PerDiem.Value == 300)
						wBIdStateContent.Constraints.PerDiem.Value = 18000;
					if (wBIdStateContent.CxWtState.CLAuto == null)
						wBIdStateContent.CxWtState.CLAuto = new StateStatus { Cx = false, Wt = false };
					if (wBIdStateContent.CxWtState.CitiesLegs == null)
						wBIdStateContent.CxWtState.CitiesLegs = new StateStatus { Cx = false, Wt = false };
					if (wBIdStateContent.Constraints.StartDayOftheWeek.SecondcellValue == null)
					{
						wBIdStateContent.Constraints.StartDayOftheWeek.SecondcellValue = "1";
						foreach (var item in wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters)
						{
							if (item.SecondcellValue == null)
							{
								item.SecondcellValue = "1";
							}
						}
					}
					WBidState state = new WBidState(wBIdStateContent);


					//read the VAC file contents if the Vacation exits.
					// if (wBIdStateContent.IsVacationOverlapOverlapCorrection || wBIdStateContent.MenuBarButtonState.IsEOM)
					// {
					//if (File.Exists (WBidHelper.GetAppDataPath () + "/" + filename + ".VAC")) {

					//	using (
					//		FileStream vacstream =
					//			File.OpenRead (WBidHelper.GetAppDataPath () + "/" + filename + ".VAC")) {

					//		Dictionary<string, TripMultiVacData> objineinfo =
					//			new Dictionary<string, TripMultiVacData> ();
					//		GlobalSettings.VacationData =
					//                          ProtoSerailizer.DeSerializeObject (
					//			WBidHelper.GetAppDataPath () + "/" + filename + ".VAC", objineinfo, vacstream);

					//	}
					//} else {
					//	if (wBIdStateContent.MenuBarButtonState != null) {
					//		wBIdStateContent.MenuBarButtonState.IsEOM = false;
					//		wBIdStateContent.MenuBarButtonState.IsVacationCorrection = false;
					//		wBIdStateContent.MenuBarButtonState.IsVacationDrop = false;
					//	}
					//}

					if (wBIdStateContent.MenuBarButtonState.IsMIL && GlobalSettings.WBidINIContent.User.MIL)
					{
						MILData milData;
						if (File.Exists(WBidHelper.MILFilePath))
						{
							using (FileStream milStream = File.OpenRead(WBidHelper.MILFilePath))
							{
								MILData milDataobject = new MILData();
								milData = ProtoSerailizer.DeSerializeObject(WBidHelper.MILFilePath, milDataobject, milStream);
							}

							GlobalSettings.MILData = milData.MILValue;
							GlobalSettings.MILDates = WBidCollection.GenarateOrderedMILDates(wBIdStateContent.MILDateList);
						}

					}
					if (wBIdStateContent.TagDetails != null)
					{
						GlobalSettings.TagDetails = new TagDetails();
						wBIdStateContent.TagDetails.ForEach(x => GlobalSettings.TagDetails.Add(new Tag { Line = x.Line, Content = x.Content }));
					}
					//}
					GlobalSettings.MenuBarButtonStatus = wBIdStateContent.MenuBarButtonState;
					GlobalSettings.IsVacationCorrection = wBIdStateContent.IsVacationOverlapOverlapCorrection;
					//GlobalSettings.OverNightCitiesInBid = GlobalSettings.Lines.SelectMany(x => x.OvernightCities).Distinct().OrderBy(x => x).ToList();
					GlobalSettings.OrderedVacationDays = new List<Absense>();
					GlobalSettings.TempOrderedVacationDays = new List<Absense>();

					WBidHelper.GenerateDynamicOverNightCitiesList();
					GlobalSettings.AllCitiesInBid = GlobalSettings.WBidINIContent.Cities.Select(y => y.Name).ToList();
					if (GlobalSettings.WBidStateCollection.Vacation != null)
					{

						if (GlobalSettings.WBidStateCollection.Vacation.Count > 0)
						{

							GlobalSettings.SeniorityListMember = GlobalSettings.SeniorityListMember ?? new SeniorityListMember();
							GlobalSettings.SeniorityListMember.Absences = new List<Absense>();
							GlobalSettings.WBidStateCollection.Vacation.ForEach(x => GlobalSettings.SeniorityListMember.Absences.Add(new Absense
							{
								AbsenceType = "VA",
								StartAbsenceDate = Convert.ToDateTime(x.StartDate),
								EndAbsenceDate = Convert.ToDateTime(x.EndDate)
							}));
							GlobalSettings.OrderedVacationDays = WBidCollection.GetOrderedAbsenceDates();
							GlobalSettings.TempOrderedVacationDays = WBidCollection.GetOrderedAbsenceDates();
						}
					}




					// var sabu = GlobalSettings.Trip.Where(x => linePairing.Contains(x.TripNum)).SelectMany(z => z.DutyPeriods.SelectMany(r => r.Flights.Select(t => new { arrival = t.ArrSta, depart = t.DepSta }))).ToList();

					List<int> lstint = new List<int>() { 1, 2, 3 };

					StateManagement statemanagement = new StateManagement();
					// statemanagement.ReloadDataFromStateFile();


					//SortCalculation sort = new SortCalculation();
					WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);


					GlobalSettings.IsFVVacation = (GlobalSettings.WBidStateCollection.FVVacation != null && GlobalSettings.WBidStateCollection.FVVacation.Count > 0 && (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO"));

					GlobalSettings.FVVacation = GlobalSettings.WBidStateCollection.FVVacation;


					if (wBidStateContent != null)
					{
						if (GlobalSettings.CurrentBidDetails.Postion == "FA" && (wBidStateContent.FAEOMStartDate != null && wBidStateContent.FAEOMStartDate != DateTime.MinValue && wBidStateContent.FAEOMStartDate != DateTime.MinValue.ToUniversalTime()))
							GlobalSettings.FAEOMStartDate = wBidStateContent.FAEOMStartDate;
						else
							GlobalSettings.FAEOMStartDate = DateTime.MinValue.ToUniversalTime();

						try
						{
							WBidHelper.RetrieveSaveAndSetLineFiles(3, wBidStateContent);
						}
						catch (Exception ex)
						{
							WBidHelper.AddDetailsToMailCrashLogs(ex);
						}

						foreach (Line line in GlobalSettings.Lines)
						{
							line.ConstraintPoints = new ConstraintPoints();
							line.WeightPoints = new WeightPoints();
						}
						if (GlobalSettings.IsFVVacation)
						{
							FVVacation objvac = new FVVacation();
							GlobalSettings.Lines = new ObservableCollection<Line>(objvac.SetFVVacationValuesForAllLines(GlobalSettings.Lines.ToList()));

							//wBidStateContent.MenuBarButtonState.IsVacationCorrection = true;
						}
						//statemanagement.RecalculateLineProperties(wBidStateContent);
						if (GlobalSettings.WBidINIContent.RatioValues != null)
						{
							SetRatioValues(wBidStateContent);
						}
						//Setting Button status to Global variables
						statemanagement.SetMenuBarButtonStatusFromStateFile(wBidStateContent);
						//Setting  status to Global variables
						statemanagement.SetVacationOrOverlapExists(wBidStateContent);
						//St the line order based on the state file.
						statemanagement.ReloadStateContent(wBidStateContent);

						SetManualMovementShadowForLines(wBidStateContent);
						//SetBidAutoGroupNumberFromStateFile(wBidStateContent);
					}

					//InvokeInBackground(() =>
					//{
					//	RecalculateBidAutomatorvalues(wBidStateContent);
					//});


					//if (wBidStateContent.SortDetails != null && wBidStateContent.SortDetails.SortColumn != null && wBidStateContent.SortDetails.SortColumn != string.Empty)
					//{
					//    sort.SortLines(wBidStateContent.SortDetails.SortColumn);
					//}

					InvokeOnMainThread(() =>
					{
						//WBidHelper.SetCurrentBidInformationfromStateFileName(filename);
						loadingOverlay.Hide();



						//NSNotificationCenter.DefaultCenter.RemoveObserver(notif);
						NSNotificationCenter.DefaultCenter.RemoveObserver(newNotif);
						newNotif = null;
						NSNotificationCenter.DefaultCenter.PostNotificationName("NavigateToLineView", null);
						//UIApplication.SharedApplication.KeyWindow.RootViewController = new lineViewController();
						//						lineViewController lineController = new lineViewController ();
						//						CommonClass.lineVC = lineController;
						//						UINavigationController navController = new UINavigationController (lineController);
						//						navController.NavigationBar.BarStyle = UIBarStyle.Black;
						//						navController.NavigationBar.Hidden = true;
						//						//navController.PushViewController(lineController,false);
						//						UIApplication.SharedApplication.KeyWindow.RootViewController = navController;

					});

				});
			}
			catch (Exception ex)
			{


				InvokeOnMainThread(() =>
				{
					loadingOverlay.Hide();
					throw ex;
				});

			}



		}

        public void ServiceResponce(JsonValue jsonDoc)
        {
            InvokeOnMainThread(() =>
            {
                GlobalSettings.WBidStateCollection.SubmittedResult = CommonClass.ConvertJSonToObject<BidSubmittedData>(jsonDoc.ToString()).SubmittedResult;

            });
        }
        private void SetManualMovementShadowForLines(WBidState wBidStateContent)
        {
            try
            {
                if (GlobalSettings.WBidINIContent.User.IsModernViewShade)
                {
                    int blueLine = wBidStateContent.LineForBlueLine;
                    List<int> shadowLines = wBidStateContent.LinesForBlueBorder;

                    var objBlueLine = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == blueLine);
                    if (objBlueLine != null)
                    {
                        if (GlobalSettings.Lines[0].LineNum == blueLine)
                            objBlueLine.ManualScroll = 3;
                        else
                            objBlueLine.ManualScroll = 1;
                    }
                    if (shadowLines!=null&& shadowLines.Count > 0)
                    {
                        var objBorderLines = GlobalSettings.Lines.Where(x => shadowLines.Any(y => y == x.LineNum));
                        foreach (var item in objBorderLines)
                        {
                            item.ManualScroll = 2;
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
		private void RecalculateBidAutomatorvalues(WBidState wBidStateContent)
		{
			//Clear top lock and bottom lock and line property for constraint

			GlobalSettings.Lines.ToList().ForEach(x =>
			{
				x.TopLock = false;
				x.BotLock = false;
		//x.WeightPoints.Reset();
		if (x.BAFilters != null)
					x.BAFilters.Clear();
				x.BAGroup = string.Empty;
				x.IsGrpColorOn = 0;
			});

			if (wBidStateContent.BidAuto != null && wBidStateContent.BidAuto.BAGroup != null)
			{
				wBidStateContent.BidAuto.BAGroup.Clear();
			}

			// Adding Selected Constraints to State Object
			// AddSelectedConstraintsToStateObject();

			// AddSelectedSortToStateObject();

			// Calculate Line properties value for BA Constraint
			//	InvokeInBackground (() => {
			var bACalculation = new BidAutomatorCalculations();

			bACalculation.CalculateLinePropertiesForBAFilters();



			//Apply COnstrint And Sort
			bACalculation.ApplyBAFilterAndSort();

			//			IsneedToEnableCalculateBid = false;
			//			IsSortChanged = false;

			if (wBidStateContent.BidAuto != null && wBidStateContent.BidAuto.BAFilter != null)
wBidStateContent.BidAuto.BAFilter.ForEach(x => x.IsApplied = true);


			//Setting Bid Automator settings to CalculatedBA state
			//SetCurrentBADetailsToCalculateBAState();

			GlobalSettings.isModified = true;
		}
		[Export ("collectionView:layout:sizeForItemAtIndexPath:")]
		public virtual CGSize SizeForItemAtIndexPath (UICollectionView collectionView,
		                                                   UICollectionViewLayout layout, NSIndexPath indexPath)
		{
			return new CGSize (200, 50);
		}

		private void SetRatioValues(WBidState wBidStateContent)
		{
			if (GlobalSettings.WBidINIContent.RatioValues.Denominator != 0 && GlobalSettings.WBidINIContent.RatioValues.Denominator != 0)
			{
				if (GlobalSettings.WBidINIContent.SummaryVacationColumns.Any(x => x.Id == 75) ||
					GlobalSettings.WBidINIContent.ModernNormalColumns.Any(x => x == 75) ||
					GlobalSettings.WBidINIContent.ModernVacationColumns.Any(x => x == 75) ||
					GlobalSettings.WBidINIContent.BidLineNormalColumns.Any(x => x == 75) ||
					GlobalSettings.WBidINIContent.BidLineVacationColumns.Any(x => x == 75) ||
					GlobalSettings.WBidINIContent.DataColumns.Any(x => x.Id == 75) ||
					wBidStateContent.SortDetails.BlokSort.Contains("19")
					)
				{
					var denominatorcolumn = GlobalSettings.columndefinition.FirstOrDefault(X => X.Id == GlobalSettings.WBidINIContent.RatioValues.Denominator);

					var numeratorcolumn = GlobalSettings.columndefinition.FirstOrDefault(X => X.Id == GlobalSettings.WBidINIContent.RatioValues.Numerator);

					if (denominatorcolumn == null || numeratorcolumn == null)
					{
						GlobalSettings.WBidINIContent.SummaryVacationColumns.RemoveAll(x => x.Id == 75);
						GlobalSettings.WBidINIContent.ModernNormalColumns.RemoveAll(x => x == 75);
						GlobalSettings.WBidINIContent.ModernVacationColumns.RemoveAll(x => x == 75);
						GlobalSettings.WBidINIContent.BidLineNormalColumns.RemoveAll(x => x == 75);
						GlobalSettings.WBidINIContent.BidLineVacationColumns.RemoveAll(x => x == 75);
						GlobalSettings.WBidINIContent.DataColumns.RemoveAll(x => x.Id == 75);
						wBidStateContent.SortDetails.BlokSort.RemoveAll(x => x == "19");
						GlobalSettings.WBidINIContent.RatioValues.Denominator = 0;
						GlobalSettings.WBidINIContent.RatioValues.Denominator = 0;
						return;
					}
					//if (denominator != null && numerator != null)
					//{
					//	foreach (var line in GlobalSettings.Lines)
					//	{
					//		decimal numeratorValue = Convert.ToDecimal(line.GetType().GetProperty(numerator.DataPropertyName).GetValue(line, null));
					//		decimal denominatorValue = Convert.ToDecimal(line.GetType().GetProperty(denominator.DataPropertyName).GetValue(line, null));
					//		line.Ratio = Math.Round(((denominatorValue == 0) ? 0 : numeratorValue / denominatorValue), 2);

					//	}
					//}

					foreach (var line in GlobalSettings.Lines)
					{
						var numerator = line.GetType().GetProperty(numeratorcolumn.DataPropertyName).GetValue(line, null);
						if (numeratorcolumn.DataPropertyName == "TafbInBp")
							numerator = Helper.ConvertHhhColonMmToFractionalHours(line.TafbInBp);
						decimal numeratorValue = Convert.ToDecimal(numerator);

						var denominator = line.GetType().GetProperty(denominatorcolumn.DataPropertyName).GetValue(line, null);
						if (denominatorcolumn.DataPropertyName == "TafbInBp")
							denominator = Helper.ConvertHhhColonMmToFractionalHours(line.TafbInBp);
						decimal denominatorValue = Convert.ToDecimal(denominator);

						//decimal denominatorValue = Convert.ToDecimal(line.GetType().GetProperty(ObjDenominator[1]).GetValue(line, null));

						line.Ratio = Math.Round(decimal.Parse(String.Format("{0:0.00}", (denominatorValue == 0) ? 0 : numeratorValue / denominatorValue)), 2, MidpointRounding.AwayFromZero);

					}
				}
			}
		}
		private static List<RecentFile> GetExistingDataInAppData ()
		{
			string path = WBidHelper.GetAppDataPath ();
			List<RecentFile> lstRecentFiles = new RecentFiles ();
			//get all the  files in the root folder(look for wbl)
			List<string> linefilenames = Directory.EnumerateFiles (path, "*.*", SearchOption.AllDirectories).Select (Path.GetFileName)
                .Where (s => s.ToLower ().EndsWith (".wbl")).ToList ();
			foreach (string filenames in linefilenames) {
				string filename = filenames.Substring (0, filenames.Length - 3);
				if (filenames.Length < 14)
					continue;
				// if (File.Exists(path + "/" + filenames + ".WBP") && File.Exists(path + "/" + filenames + ".WBS"))
				//temporary code.In future we need to check the WBS file also
				if (File.Exists (path + "/" + filename + "WBP")) {
					System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo ();
					RecentFile recentfile = new RecentFile ();
					recentfile.Domcile = filename.Substring (0, 3);
					recentfile.Position = filename.Substring (3, 2);
					recentfile.Month = Convert.ToInt32 (filename.Substring (5, 2));
					recentfile.MonthDisplay = mfi.GetMonthName (Convert.ToInt32 (filename.Substring (5, 2))).Substring (0, 3).ToUpper ();
					recentfile.Year = (Convert.ToInt16 (filename.Substring (7, 2)) + 2000).ToString ();
					recentfile.Round = (filename.Substring (9, 1) == "M") ? "1st Round" : "2nd Round";
					lstRecentFiles.Add (recentfile);
				}
			}
			lstRecentFiles = lstRecentFiles.OrderByDescending (x => x.Year).ThenByDescending (y => y.Month).ThenByDescending (z => z.Round).ThenBy (a => a.Domcile).ToList ();
			return lstRecentFiles;

		}

		private void DeleteBidPeriod (string domcile, string position, string round, int bidperiod, int year)
		{
			try {
				//domicle==bwi
				//position==Cp
				//round=D
				//bidperiod=1

				//string message = "Are you sure you want to delete the" + currentOpenBid + " WBid data files \nfor " + SelectedDomicile.DomicileName + " " + SelectedPosition.LongStr + " " + SelectedEquipment.EquipmentNumber.ToString() + " ";
				// message += SelectedBidRound.RoundDescription + " for " + SelectedBidPeriod.Period + " ?";

				//if (MessageBox.Show(message, "WBidMax", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Information) == System.Windows.MessageBoxResult.Yes)
				// {

				string bidround = (round == "1st Round" ? "M" : "S");
				AppDataBidFileNames appbiddata = GlobalSettings.WbidUserContent.AppDataBidFiles.FirstOrDefault(x => x.Year == year && x.Month == bidperiod && x.Domicile == domcile && x.Position == position && x.Round == bidround);
				if (appbiddata != null)
				{
					var bidfiles= appbiddata.lstBidFileNames;
					if (bidfiles != null && bidfiles.Count > 0)
					{
						foreach (var item in bidfiles)
						{
							var file = WBidHelper.GetAppDataPath() + "/" + item.FileName;
							if (File.Exists(file))
							{
								File.Delete(file);
							}
						}
					}
					GlobalSettings.WbidUserContent.AppDataBidFiles.Remove(appbiddata);

				}

				//Delete Old formatted files
				string fileName = domcile + position + bidperiod.ToString ("d2") + (year - 2000) + (round == "1st Round" ? "M" : "S") + "737";

				string folderName = WBidCollection.GetPositions ().FirstOrDefault (x => x.LongStr == fileName.Substring (3, 2)).ShortStr + (round == "1st Round" ? "D" : "B") + fileName.Substring (0, 3) + WBidCollection.GetBidPeriods ().FirstOrDefault (x => x.BidPeriodId == bidperiod).HexaValue;
				//Delete WBL file
				if (File.Exists (WBidHelper.GetAppDataPath () + "/" + fileName + ".WBL")) {
					File.Delete (WBidHelper.GetAppDataPath () + "/" + fileName + ".WBL");

				}

				//Delete WBP file
				if (File.Exists (WBidHelper.GetAppDataPath () + "/" + fileName + ".WBP")) {
					File.Delete (WBidHelper.GetAppDataPath () + "/" + fileName + ".WBP");

				}

				//Delete WBS file
				if (File.Exists (WBidHelper.GetAppDataPath () + "/" + fileName + ".WBS")) {
					File.Delete (WBidHelper.GetAppDataPath () + "/" + fileName + ".WBS");

				}

				//Delete VAC file
				if (File.Exists (WBidHelper.GetAppDataPath () + "/" + fileName + ".VAC")) {
					File.Delete (WBidHelper.GetAppDataPath () + "/" + fileName + ".VAC");

				}
				//delete the folder.
				if (Directory.Exists (WBidHelper.GetAppDataPath () + "/" + folderName)) {
					Directory.Delete (WBidHelper.GetAppDataPath () + "/" + folderName, true);
				}
				

			} catch (Exception ex) {
				throw ex;
			}
		}
	}
}

