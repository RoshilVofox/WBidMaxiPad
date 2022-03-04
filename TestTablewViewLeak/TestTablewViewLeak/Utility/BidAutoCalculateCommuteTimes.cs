
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using  System.Linq;
using System.Collections.ObjectModel;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.PortableLibrary;
using System.IO.Compression;
using WBid.WBidiPad.Core.Enum;

namespace WBid.WBidiPad.iOS.Utility
{
    public class BidAutoCalculateCommuteTimes
    {
        public List<FlightRouteDetails> _flightRouteDetails;
		//public enum CommuteFromView
		//{
		//	BidAutomator,
		//	CSWConstraints,
		//	CSWWeight,
		//	CSWCommutabilityConstraints,
		//	CSWCommutabilityWeights,
  //          CSWCommutabilitySort,
  //          CSWCommutableAutoSort,
  //          CSWCommutableManualSort,
		//};
		public CommuteFromView ObjFromView;
        public string ErrorMessage;
        WBidState _wBIdStateContent;
        private int _depTime;
        private int _arrTime;
        private ObservableCollection<City> _listCommuteCity = new ObservableCollection<City>();
        public ObservableCollection<City> ListCommuteCity
        {
            get
            {
                return _listCommuteCity;
            }
            set
            {
                _listCommuteCity = value;
             
            }
        }

        private ObservableCollection<string> _listConnectTime = new ObservableCollection<string>();
        public ObservableCollection<string> ListConnectTime
        {
            get
            {
                return _listConnectTime;
            }
            set
            {
                _listConnectTime = value;
            
            }
        }

       

        private ObservableCollection<string> _istCheckInTime = new ObservableCollection<string>();
        public ObservableCollection<string> ListCheckInTime
        {
            get
            {
                return _istCheckInTime;
            }
            set
            {
                _istCheckInTime = value;
                
            }
        }

     

        private ObservableCollection<string> _listBaseTime = new ObservableCollection<string>();
        public ObservableCollection<string> ListBaseTime
        {
            get
            {
                return _listBaseTime;
            }
            set
            {
                _listBaseTime = value;
               
            }
        }

        public  void CalculateDailyCommutableTimes(string cityName,bool iNonStop,int connectTime)
        {
            try
            {
                ErrorMessage = string.Empty;

                _wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

                ReadFlightRoutes();

                if (_flightRouteDetails != null)
                {
					switch(ObjFromView)
					{
					case CommuteFromView.BidAutomator:	
						CalculateCommutableTimes(cityName, iNonStop, connectTime);
						break;
                        case CommuteFromView.CSWConstraints:
                        case CommuteFromView.CSWWeight:
                        case CommuteFromView.CSWCommutableAutoSort:
                            CalculateDailyCommutableTimesForCommuteAuto(cityName, iNonStop, connectTime);
                            break;
					//case CommuteFromView.CSWConstraints:
						//CalculateCSWCommutableTimes(cityName);
						//break;
					//case CommuteFromView.CSWWeight:
						//CalculateWeightCommutableTimes(cityName);
						//break;
					//case CommuteFromView.CSWCommutabilityConstraints:
					//	CalculateCSWCommutability(cityName);
					//	break;
					//case CommuteFromView.CSWCommutabilityWeights:
						//CalculateCSWCommutability(cityName);
						//break;
                       
					}
                    

                }
                else
                {
                    ErrorMessage = "Commutable Filter is NOT available  this time";
                    //System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    //{
                    //    SendNotificationMessage(WBidMessages.CommutableLineBAWindow_Notofication_ShowFilterNotAvilableMessageBox);
                    //    SendNotificationMessage(WBidMessages.CommuteLineBAWindow_Notofication_CloseCommuteLineBAWindow);
                    //}));

                }
            }
            catch {
                ErrorMessage = "Commutable Filter is NOT available  this time";
            }

        }

        public void ReadFlightRoutes()
        {

            _flightRouteDetails = null;

            string serverPath = GlobalSettings.WBidDownloadFileUrl + "FlightData.zip";
            string zipLocalFile = Path.Combine(WBidHelper.GetAppDataPath(), "FlightData.zip");
            string networkDataPath = WBidHelper.GetAppDataPath() + "/" + "FlightData.NDA";


            if (!File.Exists(networkDataPath))
            {

                int typeOfInternetConnection;
//                typeOfInternetConnection = InternetHelper.CheckInterNetConnection();
//                //typeOfInternetConnection = (int)InternetType.Air;
//                //No internet connection
//                if (typeOfInternetConnection == (int)InternetType.Ground || typeOfInternetConnection == (int)InternetType.AirPaid)
//                {
				if (Reachability.CheckVPSAvailable()) {
						//----------------------------------------------------------------------------------------
						WebClient wcClient = new WebClient ();
						wcClient.DownloadFile (serverPath, WBidHelper.GetAppDataPath () + "/" + "FlightData.zip");
						//----------------------------------------------------------------------------------------
						string target = Path.Combine (WBidHelper.GetAppDataPath (), WBidHelper.GetAppDataPath () + "/");
						if (!File.Exists (networkDataPath)) {
							if (File.Exists (zipLocalFile))
								ZipFile.ExtractToDirectory (zipLocalFile, target);
						}


						if (File.Exists (zipLocalFile)) {
							File.Delete (zipLocalFile);
						}

					}
					else
					{

                    ErrorMessage = "Commutable Filter is NOT available  this time "+Constants.VPSDownAlert;
						return ;

					}
				//}
                //else
                //{

                //    ErrorMessage = "Commutable Filter is NOT available  this time";
                //    return ;
 
                //}

            }



            //Reading NDA file
            //----------------------------------------------------------------------------------------
            if (File.Exists(networkDataPath))
            {




                //Deserializing data to FlightPlan object
                FlightPlan fp = new FlightPlan();
                using (FileStream networkDatatream = File.OpenRead(networkDataPath))
                {

                    FlightPlan flightPlan = new FlightPlan();
                    flightPlan = ProtoSerailizer.DeSerializeObject(networkDataPath, fp, networkDatatream);



                    _flightRouteDetails = flightPlan.FlightRoutes.Join(flightPlan.FlightDetails, fr => fr.FlightId, f => f.FlightId,
                                  (fr, f) =>
                                 new FlightRouteDetails
                                 {
                                     Flight = f.FlightId,
                                     FlightDate = fr.FlightDate,
                                     Orig = f.Orig,
                                     Dest = f.Dest,
                                     Cdep = f.Cdep,
                                     Carr = f.Carr,
                                     Ldep = f.Ldep,
                                     Larr = f.Larr,
                                     RouteNum = fr.RouteNum,

                                 }).ToList();
                }
            }
            //----------------------------------------------------------------------------------------
        }

        private void CalculateCommutableTimes(string commuteCity,bool IsNonStopOnly,int connectTime)
        {
			
            string domicile = GlobalSettings.CurrentBidDetails.Domicile;

            if (_wBIdStateContent.BidAuto == null)
            {
                _wBIdStateContent.BidAuto = new BidAutomator();
            }
            if (_wBIdStateContent.BidAuto.DailyCommuteTimes == null)
            {
                _wBIdStateContent.BidAuto.DailyCommuteTimes = new List<CommuteTime>();
            }
            _wBIdStateContent.BidAuto.DailyCommuteTimes.Clear();

            DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate.Date;
            DateTime endDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.Date;
            endDate = endDate.AddDays(3);
            int depTimeHhMm = 0301;  /* format is hhmm -- min time is 0301 (3:01AM) */
            int arrTimeHhMm = 2700; /* format is hhmm -- max time is 2700 (3:00AM)   */
            _depTime = depTimeHhMm / 100 * 60 + depTimeHhMm % 100;
            _arrTime = arrTimeHhMm / 100 * 60 + arrTimeHhMm % 100;
			int arrivalcount = 0;
			int depaturecount = 0;
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {

                CommuteTime commuteTime = new CommuteTime();
                commuteTime.BidDay = date;
                commuteTime.EarliestArrivel = DateTime.MinValue;
                commuteTime.LatestDeparture = DateTime.MinValue;
                //Calculating earliest arrivel time
                //------------------------------------------------------------------------
                var nonConnectFlights = GetNonConnectFlights(commuteCity, domicile, date);

                var oneAndZeroConnectFlights = nonConnectFlights;
                if (IsNonStopOnly != true)
                {
                    var oneConnectFlights = GetOneConnectFlights(commuteCity, domicile, date, connectTime);
                    oneAndZeroConnectFlights = nonConnectFlights.Union(oneConnectFlights).ToList();
                }


                if (oneAndZeroConnectFlights != null && oneAndZeroConnectFlights.Count > 0)
                {
                    double earliestArrivelTime = oneAndZeroConnectFlights.OrderBy(x => x.RtArr).FirstOrDefault().RtArr;
                    commuteTime.EarliestArrivel = date.Date.AddMinutes(earliestArrivelTime);
                }
                else
                {
					arrivalcount++;
					commuteTime.EarliestArrivel = DateTime.MinValue;
                    //ErrorMessage = "There are NO possible connections between your commute city and your base.";
                    //System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    //{
                    //    SendNotificationMessage(WBidMessages.CommutableLineBAWindow_Notofication_ShowMessageBox);
                    //    IsCommuteCitySelected = false;
                    //}));

                    //break;
                }
                //----------------------------------------------------------------------------

                //Calculating earliest arrivel time
                //------------------------------------------------------------------------
                nonConnectFlights = GetNonConnectFlights(domicile, commuteCity, date);

                
                 oneAndZeroConnectFlights = nonConnectFlights;
                if (IsNonStopOnly != true)
                {
                    var oneConnectFlights = GetOneConnectFlights(domicile, commuteCity, date, connectTime);
                    oneAndZeroConnectFlights = nonConnectFlights.Union(oneConnectFlights).ToList();
                }
                


                if (oneAndZeroConnectFlights != null && oneAndZeroConnectFlights.Count > 0)
                {
                    double latestDepartureTime = oneAndZeroConnectFlights.OrderByDescending(x => x.RtDep).FirstOrDefault().RtDep;
                    commuteTime.LatestDeparture = date.Date.AddMinutes(latestDepartureTime);


                }
                else
                {
					depaturecount++;
					commuteTime.LatestDeparture = DateTime.MinValue;
                   // ErrorMessage = "There are NO possible connections between your commute city and your base.";
                    //System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    //{
                    //    SendNotificationMessage(WBidMessages.CommutableLineBAWindow_Notofication_ShowMessageBox);
                    //    IsCommuteCitySelected = false;
                    //}));

                    //break;
                }

                //----------------------------------------------------------------------------
                _wBIdStateContent.BidAuto.DailyCommuteTimes.Add(commuteTime);
            }

			int totaldays = (endDate - startDate).Days+1;
			if (totaldays == depaturecount && totaldays == arrivalcount)
			{
				ErrorMessage = "There are NO possible connections between your commute city and your base.";

			}


        }


		private void CalculateCSWCommutableTimes(string commuteCity,int connectTime)
		{

			string domicile = GlobalSettings.CurrentBidDetails.Domicile;

			if (_wBIdStateContent.Constraints == null)
			{
				_wBIdStateContent.Constraints = new Constraints();
			}
			if (_wBIdStateContent.Constraints.DailyCommuteTimes == null)
			{
				_wBIdStateContent.Constraints.DailyCommuteTimes = new List<CommuteTime>();
			}
            if (_wBIdStateContent.Constraints.DailyCommuteTimesCmmutability == null)
            {
                _wBIdStateContent.Constraints.DailyCommuteTimesCmmutability = new List<CommuteTime>();
            }
			_wBIdStateContent.Constraints.DailyCommuteTimes.Clear();
            _wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.Clear();

			DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate.Date;
			DateTime endDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.Date;
			endDate = endDate.AddDays(3);
			int depTimeHhMm = 0301;  /* format is hhmm -- min time is 0301 (3:01AM) */
			int arrTimeHhMm = 2700; /* format is hhmm -- max time is 2700 (3:00AM)   */
			_depTime = depTimeHhMm / 100 * 60 + depTimeHhMm % 100;
			_arrTime = arrTimeHhMm / 100 * 60 + arrTimeHhMm % 100;
			int arrivalcount = 0;
			int depaturecount = 0;
			for (var date = startDate; date <= endDate; date = date.AddDays(1))
			{

				CommuteTime commuteTime = new CommuteTime();
				commuteTime.BidDay = date;
				commuteTime.EarliestArrivel = DateTime.MinValue;
				commuteTime.LatestDeparture = DateTime.MinValue;
				//Calculating earliest arrivel time
				//------------------------------------------------------------------------
				var nonConnectFlights = GetNonConnectFlights(commuteCity, domicile, date);

				var oneConnectFlights = GetOneConnectFlights(commuteCity, domicile, date, connectTime);

				var oneAndZeroConnectFlights = nonConnectFlights.Union(oneConnectFlights).ToList();


				if (oneAndZeroConnectFlights != null && oneAndZeroConnectFlights.Count > 0)
				{
					double earliestArrivelTime = oneAndZeroConnectFlights.OrderBy(x => x.RtArr).FirstOrDefault().RtArr;
					commuteTime.EarliestArrivel = date.Date.AddMinutes(earliestArrivelTime);
				}
				else
				{ 
					arrivalcount++;
					commuteTime.EarliestArrivel = DateTime.MinValue;
					//ErrorMessage = "There are NO possible connections between your commute city and your base.";
					//System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
					//{
					//    SendNotificationMessage(WBidMessages.CommutableLineBAWindow_Notofication_ShowMessageBox);
					//    IsCommuteCitySelected = false;
					//}));

					//break;
				}
				//----------------------------------------------------------------------------

				//Calculating earliest arrivel time
				//------------------------------------------------------------------------
				nonConnectFlights = GetNonConnectFlights(domicile, commuteCity, date);

				oneConnectFlights = GetOneConnectFlights(domicile, commuteCity, date, connectTime);

				oneAndZeroConnectFlights = nonConnectFlights.Union(oneConnectFlights).ToList();


				if (oneAndZeroConnectFlights != null && oneAndZeroConnectFlights.Count > 0)
				{
					double latestDepartureTime = oneAndZeroConnectFlights.OrderByDescending(x => x.RtDep).FirstOrDefault().RtDep;
					commuteTime.LatestDeparture = date.Date.AddMinutes(latestDepartureTime);


				}
				else
				{
					depaturecount++;
					commuteTime.LatestDeparture = DateTime.MinValue;
					//ErrorMessage = "There are NO possible connections between your commute city and your base.";
					//System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
					//{
					//    SendNotificationMessage(WBidMessages.CommutableLineBAWindow_Notofication_ShowMessageBox);
					//    IsCommuteCitySelected = false;
					//}));

					//break;
				}

				//----------------------------------------------------------------------------
				_wBIdStateContent.Constraints.DailyCommuteTimes.Add(commuteTime);
                _wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.Add(commuteTime);

			}

			int totaldays = (endDate - startDate).Days+1;
			if (totaldays == depaturecount && totaldays == arrivalcount)
			{
				ErrorMessage = "There are NO possible connections between your commute city and your base.";

			}


		}

		//private void CalculateCSWCommutability(string commuteCity)
		//{

		//	string domicile = GlobalSettings.CurrentBidDetails.Domicile;

		//	if (_wBIdStateContent.Constraints == null)
		//	{
		//		_wBIdStateContent.Constraints = new Constraints();
		//	}
		//	if (_wBIdStateContent.Constraints.DailyCommuteTimesCmmutability == null)
		//	{
		//		_wBIdStateContent.Constraints.DailyCommuteTimesCmmutability = new List<CommuteTime>();
		//	}
		//	_wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.Clear();

		//	DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate.Date;
		//	DateTime endDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.Date;
		//	endDate = endDate.AddDays(3);
		//	int depTimeHhMm = 0301;  /* format is hhmm -- min time is 0301 (3:01AM) */
		//	int arrTimeHhMm = 2700; /* format is hhmm -- max time is 2700 (3:00AM)   */
		//	_depTime = depTimeHhMm / 100 * 60 + depTimeHhMm % 100;
		//	_arrTime = arrTimeHhMm / 100 * 60 + arrTimeHhMm % 100;
		//	int arrivalcount = 0;
		//	int depaturecount = 0;
		//	for (var date = startDate; date <= endDate; date = date.AddDays(1))
		//	{

		//		CommuteTime commuteTime = new CommuteTime();
		//		commuteTime.BidDay = date;
		//		commuteTime.EarliestArrivel = DateTime.MinValue;
		//		commuteTime.LatestDeparture = DateTime.MinValue;
		//		//Calculating earliest arrivel time
		//		//------------------------------------------------------------------------
		//		var nonConnectFlights = GetNonConnectFlights(commuteCity, domicile, date);

		//		var oneConnectFlights = GetOneConnectFlights(commuteCity, domicile, date);

		//		var oneAndZeroConnectFlights = nonConnectFlights.Union(oneConnectFlights).ToList();


		//		if (oneAndZeroConnectFlights != null && oneAndZeroConnectFlights.Count > 0)
		//		{
		//			double earliestArrivelTime = oneAndZeroConnectFlights.OrderBy(x => x.RtArr).FirstOrDefault().RtArr;
		//			commuteTime.EarliestArrivel = date.Date.AddMinutes(earliestArrivelTime);
		//		}
		//		else
		//		{ 
		//			arrivalcount++;
		//			commuteTime.EarliestArrivel = DateTime.MinValue;
				
		//		}
		//		//----------------------------------------------------------------------------

		//		//Calculating earliest arrivel time
		//		//------------------------------------------------------------------------
		//		nonConnectFlights = GetNonConnectFlights(domicile, commuteCity, date);

		//		oneConnectFlights = GetOneConnectFlights(domicile, commuteCity, date);

		//		oneAndZeroConnectFlights = nonConnectFlights.Union(oneConnectFlights).ToList();


		//		if (oneAndZeroConnectFlights != null && oneAndZeroConnectFlights.Count > 0)
		//		{
		//			double latestDepartureTime = oneAndZeroConnectFlights.OrderByDescending(x => x.RtDep).FirstOrDefault().RtDep;
		//			commuteTime.LatestDeparture = date.Date.AddMinutes(latestDepartureTime);


		//		}
		//		else
		//		{
		//			depaturecount++;
		//			commuteTime.LatestDeparture = DateTime.MinValue;

		//		}

		//		//----------------------------------------------------------------------------
		//		_wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.Add(commuteTime);
		//	}

		//	int totaldays = (endDate - startDate).Days+1;
		//	if (totaldays == depaturecount && totaldays == arrivalcount)
		//	{
		//		ErrorMessage = "There are NO possible connections between your commute city and your base.";

		//	}


		//}
        /// <summary>
        /// Calculates the commutable times for commutable line auto constraints,weights and sorts
        /// </summary>
        /// <param name="commuteCity">Commute city.</param>
		private void CalculateDailyCommutableTimesForCommuteAuto(string commuteCity,bool IsNonStopOnly,int connectTime)
		{

			string domicile = GlobalSettings.CurrentBidDetails.Domicile;

			if (_wBIdStateContent.Weights == null)
			{
				_wBIdStateContent.Weights = new Weights();
			}
			if (_wBIdStateContent.Weights.DailyCommuteTimes == null)
			{
				_wBIdStateContent.Weights.DailyCommuteTimes = new List<CommuteTime>();
			}
			_wBIdStateContent.Weights.DailyCommuteTimes.Clear();
            if (_wBIdStateContent.Constraints.DailyCommuteTimes == null)
            {
                _wBIdStateContent.Constraints.DailyCommuteTimes = new List<CommuteTime>();
            }
            if (_wBIdStateContent.Constraints.DailyCommuteTimesCmmutability == null)
            {
                _wBIdStateContent.Constraints.DailyCommuteTimesCmmutability = new List<CommuteTime>();
            }
            _wBIdStateContent.Constraints.DailyCommuteTimes.Clear();
            _wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.Clear();

			DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate.Date;
			DateTime endDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.Date;
			endDate = endDate.AddDays(3);
			int depTimeHhMm = 0301;  /* format is hhmm -- min time is 0301 (3:01AM) */
			int arrTimeHhMm = 2700; /* format is hhmm -- max time is 2700 (3:00AM)   */
			_depTime = depTimeHhMm / 100 * 60 + depTimeHhMm % 100;
			_arrTime = arrTimeHhMm / 100 * 60 + arrTimeHhMm % 100;
			int arrivalcount = 0;
			int depaturecount = 0;
			for (var date = startDate; date <= endDate; date = date.AddDays(1))
			{

				CommuteTime commuteTime = new CommuteTime();
				commuteTime.BidDay = date;
				commuteTime.EarliestArrivel = DateTime.MinValue;
				commuteTime.LatestDeparture = DateTime.MinValue;
				//Calculating earliest arrivel time
				//------------------------------------------------------------------------
				var nonConnectFlights = GetNonConnectFlights(commuteCity, domicile, date);

                var oneAndZeroConnectFlights = nonConnectFlights;
                if (IsNonStopOnly != true)
                {
                    var oneConnectFlights = GetOneConnectFlights(commuteCity, domicile, date,connectTime);
                   oneAndZeroConnectFlights = nonConnectFlights.Union(oneConnectFlights).ToList();
                }
                


				if (oneAndZeroConnectFlights != null && oneAndZeroConnectFlights.Count > 0)
				{
					double earliestArrivelTime = oneAndZeroConnectFlights.OrderBy(x => x.RtArr).FirstOrDefault().RtArr;
					commuteTime.EarliestArrivel = date.Date.AddMinutes(earliestArrivelTime);
				}
				else
				{
					arrivalcount++;
					commuteTime.EarliestArrivel = DateTime.MinValue;
					
				}
				//----------------------------------------------------------------------------

				//Calculating earliest arrivel time
				//------------------------------------------------------------------------
				nonConnectFlights = GetNonConnectFlights(domicile, commuteCity, date);

                oneAndZeroConnectFlights = nonConnectFlights;
                if (IsNonStopOnly != true)
                {
                    var oneConnectFlights = GetOneConnectFlights(domicile, commuteCity, date,connectTime);
                 oneAndZeroConnectFlights = nonConnectFlights.Union(oneConnectFlights).ToList();
                }


                if (oneAndZeroConnectFlights != null && oneAndZeroConnectFlights.Count > 0)
				{
					double latestDepartureTime = oneAndZeroConnectFlights.OrderByDescending(x => x.RtDep).FirstOrDefault().RtDep;
					commuteTime.LatestDeparture = date.Date.AddMinutes(latestDepartureTime);

				}
				else
				{
					depaturecount++;
					commuteTime.LatestDeparture = DateTime.MinValue;
				}

				//----------------------------------------------------------------------------
				_wBIdStateContent.Weights.DailyCommuteTimes.Add(commuteTime);
                _wBIdStateContent.Constraints.DailyCommuteTimes.Add(commuteTime);
                _wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.Add(commuteTime);
			}
			int totaldays = (endDate - startDate).Days+1;
			if (totaldays == depaturecount && totaldays == arrivalcount)
			{
				ErrorMessage = "There are NO possible connections between your commute city and your base.";

			}

		}
        private List<RouteDomain> GetNonConnectFlights(string depSta, string arrSta, DateTime dateTime)
        {

            List<RouteDomain> nonConnectFlights = null;

            nonConnectFlights = _flightRouteDetails
                               .Where(x => x.Orig == depSta && x.Dest == arrSta && x.FlightDate == dateTime && x.Cdep >= _depTime && x.Carr <= _arrTime)

                            .Select(y =>
                              new RouteDomain
                              {
                                  Date = y.FlightDate,
                                  Route = y.Orig + '-' + y.Dest,
                                  RtDep = y.Cdep,
                                  RtArr = y.Carr,
                                  RtTime = y.Carr - y.Cdep,
                                  Rt1 = y.RouteNum,
                                  Rt2 = 0,
                                  Rt3 = 0,
                                  Rt1Dep = y.Cdep,
                                  Rt2Dep = 0,
                                  Rt3Dep = 0,
                                  Rt1Arr = y.Carr,
                                  Rt2Arr = 0,
                                  Rt3Arr = 0,
                                  Con1 = 0,
                                  Con2 = 0,
                                  Rt1Orig = y.Orig,
                                  Rt2Orig = "",
                                  Rt3Orig = "",
                                  Rt1Dest = y.Dest,
                                  Rt2Dest = "",
                                  Rt3Dest = "",
                                  Rt1FltNum = y.Flight,
                                  Rt2FltNum = 0,
                                  Rt3FltNum = 0


                              }).ToList()
                              .OrderBy(z => z.Route).ThenBy(z1 => z1.RtTime).ToList();
            //.ThenBy(z1 => z1.RtTime).ToList();
            return nonConnectFlights;
        }

        private List<RouteDomain> GetOneConnectFlights(string depSta, string arrSta, DateTime dateTime,int connectTime)
        {
            //int connectTime = 30;
            List<RouteDomain> oneConnectFlights = null;

            oneConnectFlights = _flightRouteDetails.Where(frd1 => frd1.Orig == depSta && frd1.FlightDate == dateTime).Join(_flightRouteDetails.Where(frd2 => frd2.Dest == arrSta && frd2.FlightDate == dateTime), f1 => f1.Dest, f2 => f2.Orig,
                                                      (f1, f2) => new { ff1 = f1, ff2 = f2 }).ToList()
                                                      .Where(x =>
                                                        x.ff1.Dest != arrSta
                                                        && x.ff1.Cdep >= _depTime && x.ff2.Carr <= _arrTime
                                                         && (x.ff1.Carr + connectTime <= x.ff2.Cdep || x.ff1.RouteNum == x.ff2.RouteNum) && x.ff2.Cdep > x.ff1.Cdep
                                                      )
                                                    .Select(y =>
                                                    new RouteDomain
                                                    {
                                                        Date = y.ff1.FlightDate,
                                                        Route = y.ff1.Orig + '-' + y.ff1.Dest + '-' + y.ff2.Dest,
                                                        RtDep = y.ff1.Cdep,
                                                        RtArr = y.ff2.Carr,
                                                        RtTime = y.ff2.Carr - y.ff1.Cdep,
                                                        Rt1 = y.ff1.RouteNum,
                                                        Rt2 = y.ff2.RouteNum,
                                                        Rt3 = 0,
                                                        Rt1Dep = y.ff1.Cdep,
                                                        Rt2Dep = y.ff2.Cdep,
                                                        Rt3Dep = 0,
                                                        Rt1Arr = y.ff1.Carr,
                                                        Rt2Arr = y.ff2.Carr,
                                                        Rt3Arr = 0,
                                                        Con1 = y.ff2.Cdep - y.ff1.Carr,
                                                        Con2 = 0,
                                                        Rt1Orig = y.ff1.Orig,
                                                        Rt2Orig = y.ff2.Orig,
                                                        Rt3Orig = "",
                                                        Rt1Dest = y.ff1.Dest,
                                                        Rt2Dest = y.ff2.Dest,
                                                        Rt3Dest = "",
                                                        Rt1FltNum = y.ff1.Flight,
                                                        Rt2FltNum = y.ff2.Flight,
                                                        Rt3FltNum = 0

                                                    }).ToList()
                                                    .OrderBy(z => z.Route).ThenBy(z1 => z1.RtTime).ToList();
            return oneConnectFlights;
        }

        private void GenerateCommuteCities()
        {
            ListCommuteCity = new ObservableCollection<City>();
            foreach (var city in GlobalSettings.WBidINIContent.Cities)
            {
                ListCommuteCity.Add(new City() { Id = city.Id, Name = city.Name });
            }

            ListCommuteCity.Insert(0, new City() { Id = 0, Name = "Not Set" });

        }

        private void GenarateConnectTime()
        {
            ListConnectTime = new ObservableCollection<string>();
            for (int i = 5; i < 60; i = i + 5)
            {
                ListConnectTime.Add("00:" + i.ToString().PadLeft(2, '0'));
            }
            ListConnectTime.Add("01:00");
        }

        private void GenarateCheckInTime()
        {
            ListCheckInTime = new ObservableCollection<string>();
            for (int i = 5; i <= 120; i = i + 5)
            {
                ListCheckInTime.Add(ConvertMinuteToHHMM(i));
            }
            //ListCheckInTime.Add("01:00");
        }

        private void GenarateBackToBaseTime()
        {
            ListBaseTime = new ObservableCollection<string>();
            for (int i = 5; i < 60; i = i + 5)
            {
                ListBaseTime.Add("00:" + i.ToString().PadLeft(2, '0'));
            }
            ListBaseTime.Add("01:00");
        }

        private string ConvertMinuteToHHMM(int minute)
        {
            string result = string.Empty;
            result = Convert.ToString(minute / 60).PadLeft(2, '0');
            result += ":";
            result += Convert.ToString(minute % 60).PadLeft(2, '0');
            return result;


        }

        private int ConvertHHMMToMinute(string hhmm)
        {

            int result = 0;

            if (hhmm == string.Empty || hhmm == null) return result;

            string[] split = hhmm.Split(':');
            result = int.Parse(split[0]) * 60 + int.Parse(split[1]);
            return result;

        }

    }
}
