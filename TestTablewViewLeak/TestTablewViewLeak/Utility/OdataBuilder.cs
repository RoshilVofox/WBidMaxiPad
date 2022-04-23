using System;
using UIKit;
using Foundation;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using System.Text.RegularExpressions;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;

namespace WBid.WBidiPad.iOS
{
	public class OdataBuilder
	{
		public RestServiceUtil RestService;
		public OdataBuilder ()
		{
			RestService = new RestServiceUtil ();
		}
		UIViewController GenericController;

		public void CheckRemoUserAccount(string EmployeeNo)
		{
			//string UrlString = "GetUserAccountDetails/" + EmployeeNo;
            EmployeeNo = Regex.Replace(EmployeeNo, "[^0-9]+", string.Empty);
			string UrlString = "GetEmployeeDetails/" + EmployeeNo+"/4";
           
			RestService.ConstructHttpsURL (UrlString);
			RestService.Get ();

		}
		public void CheckRemoUserAccountTest(string EmployeeNo)
		{
			//string UrlString = "GetUserAccountDetails/" + EmployeeNo;
			EmployeeNo = Regex.Replace(EmployeeNo, "[^0-9]+", string.Empty);
			string UrlString = "GetEmployeeDetails/" + EmployeeNo + "/4";

			RestService.ConstructURL(UrlString);
			RestService.Get();

		}
		public void UpdateUserAccount(RemoteUpdateUserInformation UserInfo)
		{
			string UrlString = "UpdateWBidUserDetails/" ;
			if(UserInfo.Position == 5 || UserInfo.Position==4)
				UserInfo.Position=4;
			string data = SmartSyncLogic.JsonObjectToStringSerializer<RemoteUpdateUserInformation> (UserInfo);
			RestService.ConstructHttpsURL(UrlString);
			RestService.Post (data);

		}
		public void CAPDetails(CAPInputParameter Info)
		{
			string UrlString = "GetCAPData" ;
			string data = SmartSyncLogic.JsonObjectToStringSerializer<CAPInputParameter> (Info);
			RestService.ConstructHttpsURL(UrlString);
			RestService.Post (data);

		}
		public void GetVacationDifferenceDetails(VacationValueDifferenceInputDTO Info)
		{
			string UrlString = "GetVacationDifferenceData";
			string data = SmartSyncLogic.JsonObjectToStringSerializer<VacationValueDifferenceInputDTO>(Info);
			RestService.ConstructHttpsURL(UrlString);
			RestService.Post(data);

		}
		public void GetApplicationLoadData(ApplicationData Info)
		{
			string UrlString = "GetApplicationLoadDatas";
			string data = SmartSyncLogic.JsonObjectToStringSerializer<ApplicationData>(Info);
			RestService.ConstructHttpsURL(UrlString);
			RestService.Post(data);

		}
		public void GetMonthlyAwards(MonthlyBidDetails Info)
        {
            string UrlString = "GetMonthlyAwardData";
            string data = SmartSyncLogic.JsonObjectToStringSerializer<MonthlyBidDetails>(Info);
            RestService.ConstructHttpsURL(UrlString);
            RestService.Post(data);

        }
        public void GetBidSubmittedData(BidSubmittedData Info)
        {
            string UrlString = "GetBidSubmittedData";
            string data = SmartSyncLogic.JsonObjectToStringSerializer<BidSubmittedData>(Info);
            RestService.ConstructHttpsURL(UrlString);
            RestService.Post(data);

        }
        public void SaveBidSubmittedData(BidSubmittedData Info)
        {
            string UrlString = "SaveBidSubmittedData";
            string data = SmartSyncLogic.JsonObjectToStringSerializer<BidSubmittedData>(Info);
            RestService.ConstructHttpsURL(UrlString);
            RestService.Post(data);

        }
		public void SaveBidSubmittedRawData(SubmittedDataRaw Info)
		{
			string url = GlobalSettings.DataDownloadAuthenticationUrl + "AddSubmittedRawDataToServer";
			//string UrlString = "AddSubmittedRawDataToServer";
			string data = SmartSyncLogic.JsonObjectToStringSerializer<SubmittedDataRaw>(Info);
			//RestService.ConstructURL(UrlString);
			RestService.PostData( url,data);

		}
		public void UpdateSubscriptionDate(PaymentUpdateModel SubscriptionDetails)
		{
			
			string UrlString = "UpdateWBidPaidUntilDate/" ;

			string data = SmartSyncLogic.JsonObjectToStringSerializer<PaymentUpdateModel> (SubscriptionDetails);
			RestService.ConstructHttpsURL(UrlString);
			RestService.Post (data);

		}

		public void VerifyPassword(CheckPassword Password)
		{
			string UrlString = "CheckPasswordValidForUser/" ;
			string data = SmartSyncLogic.JsonObjectToStringSerializer<CheckPassword> (Password);
			RestService.ConstructHttpsURL(UrlString);
			RestService.Post (data);

		}

		public void UpdatePassword(CheckPassword Password)
		{
			string UrlString = "UpdateCrewbidUserPassword/" ;
			string data = SmartSyncLogic.JsonObjectToStringSerializer<CheckPassword> (Password);
			RestService.ConstructHttpsURL(UrlString);
			RestService.Post (data);

		}

		public void PasswordRecovery(ForgotPasswordDetails Password)
		{
			string UrlString = "SendPasswordrecoveryDetails/" ;
			string data = SmartSyncLogic.JsonObjectToStringSerializer<ForgotPasswordDetails> (Password);
			RestService.ConstructHttpsURL(UrlString);
			RestService.Post (data);

		}
		public void CreateUserAccount(RemoteUpdateUserInformation UserInfo)
		{
			string UrlString = "CreateWbidMaxUser/" ;
			string data = SmartSyncLogic.JsonObjectToStringSerializer<RemoteUpdateUserInformation> (UserInfo);
			RestService.ConstructHttpsURL(UrlString);
			RestService.Post (data);

		}
public void GetMonthlyBidData(RemoteUpdateUserInformation UserInfo)
		{
			string UrlString = "GetMonthlyBidFiles/";
			string data = SmartSyncLogic.JsonObjectToStringSerializer<RemoteUpdateUserInformation> (UserInfo);
			RestService.ConstructHttpsURL(UrlString);
			RestService.Post (data);

		}
	}
}

