using System;

namespace WBid.WBidiPad.iOS
{
	public class ServiceObjects
	{
		public ServiceObjects ()
		{
		}

	}
	public class RemoteUserInformation
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int EmpNum { get; set; }
		public string Email { get; set; }
		public string CellPhone { get; set; }
		public DateTime UserAccountDateTime { get; set; }
		public string CellCarrier { get; set; }
		public int Position { get; set; }
		public int CarrierNum { get; set; }
		public bool AcceptEmail { get; set; }
		public string Password { get; set; }

		public DateTime? CBExpirationDate { get; set; }

		public DateTime? WBExpirationDate { get; set; }
		public string BidBase { get; set; }
		public string BidSeat { get; set; }
		public bool IsFree { get; set; }


		public bool IsYearlySubscribed { get; set; }


		public bool IsMonthlySubscribed { get; set; }


		public string TopSubscriptionLine { get; set; }


		public string SecondSubscriptionLine { get; set; }


		public string ThirdSubscriptionLine { get; set; }

        public string SubscriptionMessage { get; set; }

	}

	public class RemoteUpdateUserInformation
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int EmpNum { get; set; }
		public string Email { get; set; }
		public string CellPhone { get; set; }

		public string CellCarrier { get; set; }
		public int Position { get; set; }
		public int CarrierNum { get; set; }
		public bool AcceptEmail { get; set; }
		public string Password { get; set; }

		public DateTime? CBExpirationDate { get; set; }

		public DateTime? WBExpirationDate { get; set; }

		public string BidBase { get; set; }
		public string BidSeat { get; set; }

	}
	public class CAPInputParameter
	{
		public int Year { get; set; }
		public int Month { get; set; }
	}
	public class CAPOutputParameter
	{
		public string Domicile { get; set; }
		public string Position { get; set; }
		public decimal? PreviousMonthCap { get; set; }
		public decimal? CurrentMonthCap { get; set; }
	}

	public class PaymentUpdateModel
	{
		public int EmpNum { get; set; }
		public int Month { get; set; }
		public string Message { get; set; }
		public string IpAddress { get; set; }

		public string TransactionNumber { get; set; }

		public int AppNum { get; set; }

	}

	public class ForgotPasswordDetails
	{
		public int EmpNum { get; set; }
		public int Type { get; set; }
	}

	public class CheckPassword
	{
		public string EmpNumber { get; set; }
		public string Password { get; set; }
	}
	public class CustomServiceResponse
	{
		public bool Status { get; set; }
		public string Message { get; set; }

		public DateTime? CBExpirationDate { get; set; }

		public DateTime? WBExpirationDate { get; set; }

		public string TopSubscriptionLine { get; set; }

		public string SecondSubscriptionLine { get; set; }

		public string ThirdSubscriptionLine { get; set; }
	}

}

