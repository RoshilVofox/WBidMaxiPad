using System;
using System.Collections.Generic;

namespace WBid.WBidiPad.iOS
{
	public static class Constants
	{
		public static string AppName = "BidValet";
		public static string SubScriptionExpired = "Your Subscription expired on:";
		public static string SubScriptionExpires = "Your Subscription expires:";
		public static List<string> SelectSeat = new List<string> () {
			"Captian",
			"First Officer",
			"Flight Attendant"
		};
		public static List<string> SelectRound = new List<string> () {
			"1st Round",
			"2nd Round"
		};
		public static List<string> MoreEqualLessList = new List<string> () {
			"Less than",
			"Equal",
			"More than"
		};

		public static List<string> MoreOrLessList = new List<string> () {
			"More than",
			"Less than"
		};

		public static List<string> DomEvent = new List<string> () {
			"All",
			"inDom",
			"AwayDom"
		};
		public static List<string> DHFirstlastList = new List<string> () {
			"first",
			"last",
			"both"
		};
		public static List<string> EquipmentValue = new List<string> () {
			//"300",
			//"500",
			//"300 & 500",
			"700",
			"800",
			"7Max",
			"8Max"
		};

		public static List<string> SelectBase = new List<string> () {
			"ATL",
			"BWI",
			"DAL",
			"DEN",
			"HOU",
			"LAS",
            "LAX",
			"MCO",
			"MDW",
			"OAK",
			"PHX"
		};
		public static string[]arrPositions = new string[]{ "Captain", "First Officer", "Flight Attendant" };
		public static List<string> Seat = new List<string>{ "Captain", "First Officer", "Flight Attendant" };
		public static List<string> Round = new List<string> { "1st Round", "2nd Round" };
		public static string LABEL_CREATE_ACCOUNT = "You do not have an account.\nFill in the following form.";
		public static string LABEL_EDIT_ACCOUNT = "Edit and change account details as needed.";
		public static string No_internet = "No internet";
		public static string NO_INTERNET_MSG = "Sorry!\n\nYou do not have an Internet connection.\n\nYou will need to establish an internet connection to continue.\n";
		public static string TO_3rd_pty_free_internet = "T/O 3rd pty free internet";
		public static string FREE_INTERNET_MSG = "Darn it!\n\nThe company 3rd Party server is not responding.\n\nThis is not uncommon.\n\nYour only courses of action are to wait a while and try again, or try another internet connection.\n\nSome time internet signal on the plane is just too weak.";
		public static string Subscr_later_no_itunes = "Subscr later - no itunes";
		public static string SUBSCR_NO_ITUNES_MSG = "Darn it!\n\nThe company 3rd Party server is not responding.\n\nThis is not uncommon.\n\nYour only courses of action are to wait a while and try again, or try another internet connection.\n\nSome time internet signal on the plane is just too weak.";
		public static string TO_3rd_pty_paid_air = "T/O 3rd pty - paid - air";
		public static string TIME_OUT_PLANE_MSG = "Your subscription is expired and you are on the plane using the free company limited internet connection.\n\nYou cannot update your subscription using the limited internet connection.\n\nEither pay for a full internet connection or wait until you get on the ground and have a full internet connection.";
		public static string TO_BVDB_paid_air = "T/O BVDB paid -air";
		public static string TO_3rd_pty_ground = "T/O 3rd pty ground";
		public static string TO_BVDB_ground = "T/O BVDB ground";
		public static string Invalid_Login = "Invalid Login";
		public static string Version_not_supported = "Version not supported";
		public static string Found_account_edit = "Found account - edit";
		public static string Create_account = "Create account";
		public static string New_CB_WB_user = "New CB/WB user";
		public static string Expired_subscription = "Expired subscription";
		public static string Valid_Subscription = "Valid Subscription";
		public static List<string> TitleActionSheet = new List<string> {
			No_internet,
			TO_3rd_pty_free_internet,
			Subscr_later_no_itunes,
			TO_3rd_pty_paid_air,
			TO_BVDB_paid_air,
			TO_3rd_pty_ground,
			TO_BVDB_ground,
			Invalid_Login,
			Version_not_supported,
			Found_account_edit,
			Create_account,
			New_CB_WB_user,
			Expired_subscription,
			Valid_Subscription
		};
		public static List<string> ErrorMessages = new List<string> {
			"Sorry!\n\nYou do not have an Internet connection.\n\nYou will need to establish an internet connection to continue.",
			"Darn it!\n\nThe company 3rd Party server is not responding.\n\nThis is not uncommon.\n\nYour only courses of action are to wait a while and try again, or try another internet connection.\n\nSome time internet signal on the plane is just too weak.",
			"Your subscription is expired and you are on the plane using the free company limited internet connection.\n\nYou cannot update your subscription using the limited internet connection.\n\nEither pay for a full internet connection or wait until you get on the ground and have a full internet connection.\n",
			"Darn it!\n\nThe company 3rd Party server is not responding. This is different than the Swalife server.\n\nThis is not uncommon.\n\nYour only course of action is to wait a while and try again, or try another internet connection.\n\nSometimes the internet signal on the plane is just too weak.",
			"Oops!\n\nThe Bid Valet database is not responding.\n\nA text has been sent to Bid Valet Admin. They will investigate the problem.\n\nYour only courses of action are to wait a while and try again, or try another internet connection.\n\nSometimes the internet signal is just too weak.",
			"Darn it!\n\nThe company 3rd Party server is not responding.\n\nThis is not uncommon.\n\nYour only courses of action are to wait a while and try again, or try another internet connection.",
			"Oops!\n\nThe Bid Valet database is not responding.\n\nA text has been sent to Bid Valet Admin. They will investigate the problem.\nYour only courses of action are to wait a while and try again, or try another internet connection.\n\nSometimes the internet signal is just too weak.",
			"Oops!\n\nThe combination of your employee number and password was not recognized by the company.\n\nOnly pilots and flight attendants of Southwest Airlines will be authorized to use this app.\n\nCheck your employee number and CWA password and try again.",
			"Oops!\n\nThis version of Bid Valet is no longer supported.\n\nGo to the App Store and update to the latest version.",
			"",
			"",
			"Welcome! Your account is free for the month.\n\nNext we are going to have you set your CONSTRAINT (filter) criteria.\n\nYou can set things like AMs before PMs, no international, Sat & Sun off, 23rd and 29th off, etc.\n\nIf you are a Fit Att don’t worry about position just yet",
			"Your Subscription expired on 17 Sep 2015\n\nYou can subscribe here for $19.99 a one month usage.\n\nOr you can go to www.swabidvalet.com for additional account options.",
			"Great! You have a valid subscription!\n\nNext we are going to have you set your CONSTRAINT (filter) criteria.\n\nYou can set things like AMs before PMs, no international, Sat & Sun off, 23rd and 29th off, etc.\n\nDon’t worry about position just yet"
		};
		public static List<string> ListCarrier = new List<string> {
			"AT&T",
			"Cingular",
			"Metro-PCS",
			"Nextel",
			"Other",
			"Sprint",
			"T-Mobile",
			"Verizon",
			"Virgin-Mobile"
		};
		public static List<string> listTitleTopBar = new List<string> {
			"No Internet",
			"3rd Party Timeout - Plane",
			"Subscribe Later",
			"3rd Party Timeout - Plane",
			"Authorization Timeout",
			"3rd Party Timeout - Ground",
			"Authorization Timeout",
			"Invalid Login",
			"Version Not Supported",
			"",
			"",
			"Prepare",
			"Subscription Expired",
			"Valid Account"
		};
		public static List<string> listConstraints = new List<string> {
			"Am - Pm",
//			"Blank-Reserve",
			"Commutable Lines",
			"Days of the Month",
			"Days of the Week - All",
			"Days of the Week - Some",
			"DH First-Last",
			"Equipment",
			"Line Type",
			"Overnight Cities",
			"Rest",
			"Start Day Of Week",
			"Trip-Work-Block Length"
		};

		public static List<string> listUserConstraints = new List<string> {
			"No PMs, No Mix",
			"Commutable Lines - Manual",
			"Intl-none",
			"Rest<14"
		};
		public static List<string> lsFirstLast = new List<string> (){ "First", "Last", "Both" };
		public static List<string> lsLessMore = new List<string> (){ "Less than", "More than"};
		public static List<string> lsRest = new List<string> (){ "All", "inDom", "AwayDom"};
		public static List<string> listBlockSorts = new List<string> {
			"1 - Days Off", "2 - Weekday", "3 - Per Diem", "4 - Pay"
		};

		public static List<string> listSubmittals = new List<string> {
			"Captain", "First Officer", "Flight Attendant"
		};

		public static List<string> listCityInternationals = new List<string> { 
			"AUA", "BDA", "BZE", "CUN", "LIR", "MBJ", "MEX", "PUJ", "PVR", "SJD", "SJO", "SJU"
		};

		public static List<int> listEquipments = new List<int> {
			300, 500, 700, 800
		};
		public static List<string> DaysInWeek = new List<string> {
			"Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"
		};
		public static List<string> listCities;
//		public static List<string> listCities = new List<string> {
//			"ABQ", "ALB", "AMA", "ATL", "AUA", "AUS", "BDA", "BDL", "BHM", "BKG", "BNA", "BOI",
//			"BOS", "BUF", "BUR", "BWI", "BZE", "CAK", "CHS", "CLE", "CLT", "CMH", "CRP", "CUN",
//			"DAL", "DAY", "DCA", "DEN", "DSM", "DTW", "ECP", "ELP", "EWR", "EYW", "FLL", "FNT",
//			"GEG", "GRR", "GSP", "HOU", "HRL", "IAD", "ICT", "IND", "ISP", "JAN", "JAX", "LAS",
//			"LAX", "LBB", "LGA", "LIR", "LIT", "MAF", "MBJ", "MCI", "MCO", "MDW", "MEM", "MEX",
//			"MHT", "MKE", "MSP", "MSY", "NAS", "OAK", "OKC", "OMA", "ONT", "ORF", "PBI", "PDX",
//			"PHL", "PHX", "PIT", "PNS", "PUJ", "PVD", "PVR", "PWM", "RDU", "RIC", "RNO", "ROC",
//			"RSW", "SAN", "SAT", "SDF", "SEA", "SFO", "SJC", "SJD", "SJO", "SJU", "SLC", "SMF",
//			"SNA", "STL", "TPA", "TUL", "TUS"
//		};
		public static string Set_Commute_City = "Set Commute City";
		public static string Set_Commute_City_Message = "You can select your commute city, and we can retrieve the takeoff and landing times between your commute city and domicile.\n \nWith these times, plus your selected connect time, we can determine if the line is commutable on the front, back, or both.\n\nNights in domicile between turns and 2-days are not considered.\n\nThe connect time is used tof connecting fights and for checkin/landing at the beginning or end of your trip.";

		public static string Set_Connect_Times = "Set Connect Times";
		public static string Set_Connect_Times_Message = "The connect time is used for connecting fights and for checkin/landing at the beginning or end of your trips.";

		public static string Get_Commute_Times = "Get Commute Times";
		public static string Get_Commute_Times_Message = "By providing your commute city, we can retrieve the takeoff and landing times between your commute city and your domicile.\n\nThese times plus your connect time, allow us to determine if a line is commutable on the front, back or both ends of a work block.";

		public static string Pad_For_Checkin = "Pad for Check-in";
		public static string Pad_For_Checkin_Msg = "The commuter policy says you need to have a scheduled flight arriving 1 hour prior to check-in.  Therefore, we recommend setting a check-in pad of 1 hour.  Thus, if a trip reports at least 1 hour after your commuter fight arrives, the this trip will be commutable on the front end";

		public static string Pad_For_Back_To_Base = "Pad for Back-to-Base";
		public static string Pad_For_Back_To_Base_Msg = "If the last flight home from base leaves at 2100, we will look for trips that end prior to that time.  If the trip land time + the Pad for Back-to-Base (typically 10 minutes minimum) is less that the last flight to home takeoff, then the Trip is considered commutable on the back end";

        public static string SouthWestConnectionAlert = "You cannot get needed access via SouthwestWifi or 2Wire.  \n\nTry again later when you are safely on the ground and have another internet access.";
        //public static string VPSDownAlert = "We are not able to connect to the WBidMax server.  We need to do so to download important files.  Please try again later.  If this persists, you can try to change internet connections (like use a hotspot).  Finally, if this persists for more than 4 hours, let us know.";
        public static string VPSDownAlert = "We are not able to connect to the WBidMax server.Please check your Internet connection and  try again later.  \n\nIf this persists, you can try to change internet connections (like use a hotspot). \n\nFinally, if this persists for more than 4 hours, let us know.";
        //public static string VPSDownCommonAlert = "We are not able to connect to the WBidMax server.Please check your Internet connection and  try again later.  \n\nIf this persists, you can try to change internet connections (like use a hotspot). \n\nFinally, if this persists for more than 4 hours, let us know.";public static string VPSDownCommonAlert = "We are not able to connect to the WBidMax server.Please check your Internet connection and  try again later.  \n\nIf this persists, you can try to change internet connections (like use a hotspot). \n\nFinally, if this persists for more than 4 hours, let us know.";
		public static string CONSTRAINTS = "Constraints";
		public static string GO_TO_CONSTRAINTS = "Go to CONSTRAINTS";
		public static string CANCEL = "Cancel";
		public static string DONE = "Done";
		public static string OK = "OK";
		public static string AUTHORIZATION = "Authorization";
		public static string LABEL_SAVE_CHANGE = "Save Change";
		public static string LABEL_CREATE = "Create";
		public static string TITLE_SENT_EMAIL = "Email Sent";
		public static string TITLE_SENT_EMAIL_TEXT = "Your bid receipt was emailed to:";
		public static string TITLE_EMAIL = "phamthangnd@gmail.com";
	
		public static string TITLE_SENT_PRINT_TEXT = "Your bid receipt was sent to:";
		public static string TITLE_SENT_PRINT = "Print Sent";
		public static string TITLE_PRINT = "HP Printer";
		public static string NOT_FUNCTIONAL = "Not Functional";
		public static string TITLE_AVOID_INTERNATIONAL = "AVOID INTERNATIONAL - MULT OK";
		public static string AVOID_CERTAIN_EQUIPMENT = "AVOID CERTAIN EQUIPMENT - MULT OK";
		public static string CHOOSE_UP_CONSTRAINTS = "Choose up to 6 constraints";
		public static string FUNCTION_COMMING_SOON = "This future feature will blow you away. Coming soon!";
		public const int CMUT_LINE_REQUEST = 1111;
		public const int CMUT_OVER_NIGHT_REQUEST = 3333;
		public const int CMUT_PICK_CITY_REQUEST = 2222;
		public const int CMUT_DAY_OF_MONTH_REQUEST = 4444;
		public const int NO_INTERNET = 1;
		public const int FREE_INTERNET = 2;
		public const int NO_ITUNES = 3;
		public const int TO_PAID_AIR = 4;
		public const int TO_BVDB_PAID_AIR = 5;
		public const int TO_GROUND = 6;
		public const int TO_BVDB_GROUND = 7;
		public const int INVALID_LOGIN = 8;
		public const int VERSION_NOT_SUPPORTED = 9;
		public const int FOUND_ACCOUNT = 10;
		public const int CREATE_ACCOUNT = 11;
		public const int NEW_CB_WB_USER = 12;
		public const int EXPIRED_SUBSCRIPTION = 13;
		public const int VALID_SUBSCRIPTION = 14;

		public const int AmPmView = 0;
		public const int BlankReserveView = 1;
		public const int CommutableLinesView = 2;
		public const int DaysMonthView = 3;
		public const int DaysOfWeekView = 4;
		public const int DHFirstLastView = 5;
		public const int EquipmentView = 6;
		public const int InternationalView = 7;
		public const int OvernightCitiesView = 8;
		public const int TripWorkBlockLengthView = 9;
		public const int RestView = 10;

		public const int  Am_Pm_Constraint = 0;
//		public const int  Blank_Reserve_Constraint=1;
		public const int  Commutable_Lines_Constraint=1;
		public const int  Days_of_Month_Constraint=2;
		public const int  Days_Week_All_Constraint=3;
		public const int  Days_of_Week_Some_Constraint=4;
		public const int  DH_First_Last_Constraint=5;
		public const int  Equipment_Constraint=6;
		public const int  Line_Type_Constraint=7;
		public const int  Overnight_Cities_Constraint=8;
		public const int  Rest_Constraint=9;
		public const int  Start_Day_Of_Week_Constraint=10;
		public const int  Trip_Work_Block_Length_Constraint=11;
		// keys intent android
		public static readonly string START_STATE = "start_state";
		public static readonly string POSITION = "position";
		public static readonly string USER_EXIST = "user_exist";
		public static readonly string SHOW_PURCHASE = "is_show_purchase";
		public static readonly string MESSAGE_ERROR = "message_error";
		public static readonly string BUTTON_TITLE = "button_title";
		public static readonly string STATE_SERVICES = "state_services";
		public static readonly string DATE_EXPIRED = "date_expired";
		public static readonly string BASE_SELECTED = "selected_base";
		public static readonly string SEAT_SELECTED = "selected_seat";
		public static readonly string ROUND_SELECTED = "selected_round";
		public static readonly string BLOCK_SORT = "is_block_sort";
		public static readonly string SORT_KEYS = "sort_keys";
		public static readonly string KEY_SENT = "key_sent";
		public static readonly string KEY_CITY_NAME = "cmt_city_name";
		public static readonly string KEY_DAY_OFF_MONTH = "days_off_month";
		public static readonly string KEY_DAY_WORK_MONTH = "days_work_month";

		public static readonly string KEY_OVERNIGHT_YES = "overnight_yes";
		public static readonly string KEY_OVERNIGHT_NO = "overnight_no";

		public const int STARE_NEW_DOWNLOAD = 0;
		public const int STARE_VALID_SUBSCRIPTION = 1;
		public const int STARE_EXPIRED_SUBSCRIPTION = 2;
//		public const int EDIT_ACCOUNT = 9;
//		public const int CREATE_ACCOUNT = 10;
//		public const int EXPIRED_VALUE = 12;
//		public const int VALID_ACCOUNT = 13;
		public const int CAPITAIN = 0;
		public const int FIRST_OFFICER = 1;
		public const int FLIGHT_ATTENDANT = 2;


	}

	public enum AuthStaus
	{
		No_internet=1,
		TO_3rd_pty_free_internet,
		Subscr_later_no_itunes,
		TO_3rd_pty_paid_air,
		TO_BVDB_paid_air,
		TO_3rd_pty_ground,
		TO_BVDB_ground,
		Invalid_Login,
		Version_not_supported,
		Found_account_edit,
		Create_account,
		New_CB_WB_user,
		Expired_subscription,
		Valid_Subscription
	}

	public enum AppNum
	{
		WBidMax=1,
		CrewBidMax,
		BidValet,
		WBidMaxApp,
		CrewBidApp
	}

}

