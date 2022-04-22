using System;
using UIKit;
using Foundation;
using System.Net;
using System.IO;
using System.Json;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Text;


namespace WBid.WBidiPad.iOS
{
	public class RestServiceUtil
	{
		public RestServiceUtil ()
		{
		}

		string ServiceURL;
		public IServiceDelegate Objdelegate;
		string url=CommonClass.isVPSServer;
		public static string BaseUrl="";
		public static string HttpsBaseUrl = "";

		//public static string BaseUrl="http://192.168.10.100/WBidDataDownloadAuthorizationService/WBidDataDwonloadAuthService.svc/";

		public void SetURl ()
		{
			if (url == "TRUE")
			{
				HttpsBaseUrl = "https://www.auth.wbidmax.com/WBidDataDwonloadAuthService.svc/Rest/";
				
				//BaseUrl="http://108.60.201.50:8000/WBidDataDwonloadAuthService.svc/";
				BaseUrl="http://www.auth.wbidmax.com/WBidDataDwonloadAuthService.svc/";
				//BaseUrl="http://192.168.10.100/WBidDataDownloadAuthorizationService/WBidDataDwonloadAuthService.svc/";

			}
			else
			{
				//BaseUrl="http://192.168.10.100/WBidDataDownloadAuthorizationService/WBidDataDwonloadAuthService.svc/";
				//BaseUrl="http://122.166.23.155/WBidDataDownloadAuthorizationService/WBidDataDwonloadAuthService.svc/";
				BaseUrl = "http://108.60.201.50/VofoxWbidAuth/WBidDataDwonloadAuthService.svc/";
			}
		}
		public void ConstructURL(string ServiceName)
		{
			SetURl ();
			ServiceURL = BaseUrl + ServiceName;
			ServiceURL = ServiceURL.Replace (" ", "%20");

		}
		public void ConstructHttpsURL(string ServiceName)
		{
			SetURl();
			ServiceURL = HttpsBaseUrl + ServiceName;
			ServiceURL = ServiceURL.Replace(" ", "%20");

		}
		public  StreamReader GetRestData(string serviceNameandParameter)
        {
            SetURl();
            string url = BaseUrl + serviceNameandParameter;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 30000;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var stream = response.GetResponseStream();
            var reader = new StreamReader(stream);
            return reader;

        }
		public void Get()
		{
			try {
				
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create (ServiceURL);
				request.Timeout = 30000;
				request.ContentType = "application/json";
				request.Method = "GET";
				HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
				var stream = response.GetResponseStream ();
				JsonValue jsonDoc =JsonObject.Load (stream);
				RequestResponce(response.StatusCode,jsonDoc);

			} catch (Exception ex) {
				
				Console.WriteLine (ex);
				Objdelegate.ResponceError ("Something went wrong.\nPlease try again or contact us. We would love to help.");
			}

		}

		public void Post(string data)
		{
			try {

				HttpWebRequest request = (HttpWebRequest)WebRequest.Create (ServiceURL);
				request.Timeout = 30000;
				request.ContentType = "application/x-www-form-urlencoded";

				//request.ContentType = "text/xml; charset=utf-8";
				request.Method = "POST";

				var bytes = Encoding.UTF8.GetBytes (data);
				request.ContentLength = bytes.Length;
				request.GetRequestStream ().Write (bytes, 0, bytes.Length);

				//Response
				var response = (HttpWebResponse)request.GetResponse ();
				var stream = response.GetResponseStream ();

				JsonValue jsonDoc =JsonObject.Load (stream);
				RequestResponce(response.StatusCode,jsonDoc);

			} catch (Exception ex) {

				Console.WriteLine (ex);
				Objdelegate.ResponceError ("Something went wrong.\nPlease try again or contact us. We would love to help.");
			}

		}
        public string PostData(string url,string data)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            var bytes = Encoding.UTF8.GetBytes(data);
            request.ContentLength = bytes.Length;
            request.GetRequestStream().Write(bytes, 0, bytes.Length);
            request.Timeout = 30000;
            //Response
            var response = (HttpWebResponse)request.GetResponse();
            var stream = response.GetResponseStream();
            if (stream == null)
                return string.Empty;

            var reader = new StreamReader(stream);
            string result = reader.ReadToEnd();

            return result.Trim('"');
        }
		public void RequestResponce(HttpStatusCode Status_Code,JsonValue jsonDoc)
		{
			
			if ((int)Status_Code == 200 || (int)Status_Code == 201) 
			{
				Objdelegate.ServiceResponce (jsonDoc);


			} else 
			{

				if ((int)Status_Code == 408) 
				{
					Objdelegate.ResponceError ("Something went wrong.\nPlease try again or contact us. We would love to help.");
				} else 
				{
					Objdelegate.ResponceError ("Something went wrong.\nPlease try again or contact us. We would love to help.");
				}
			}

		}
       
	}

	public interface IServiceDelegate
	{
		void ServiceResponce (JsonValue jsonDoc);
		void ResponceError (string Error);
	}
}

