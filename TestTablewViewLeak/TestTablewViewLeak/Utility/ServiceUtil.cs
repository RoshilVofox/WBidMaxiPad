using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using System.ServiceModel;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Net;
using WBid.WBidiPad.Core;

namespace WBid.WBidiPad.iOS.Utility
{
	public static class ServiceUtils
	{
		public static  EndpointAddress EndPoint = (CommonClass.isVPSServer=="TRUE")?new EndpointAddress("http://www.wbidmax.com:8000/WBidDataDwonloadAuthService.svc/soap"):new EndpointAddress("http://122.166.23.155/WBidDataDownloadAuthorizationService/WBidDataDwonloadAuthService.svc/soap");
		//public static readonly EndpointAddress EndPoint = new EndpointAddress("https://www.auth.wbidmax.com/WBidDataDwonloadAuthService.svc/");
		public static readonly EndpointAddress NetworkdataEndPoint = new EndpointAddress("http://www.wbidmax.com:8001/NetworkPlanService.svc");
		public static readonly EndpointAddress PushEndPoint = new EndpointAddress("http://push.wbidmax.com:8007/WBidPushSerivce.svc");
		public static BasicHttpBinding CreateBasicHttp()
		{
			BasicHttpBinding binding = new BasicHttpBinding
			{
				Name = "basicHttpBinding",
				MaxBufferSize = 2147483647,
				MaxReceivedMessageSize = 2147483647
			};
			TimeSpan timeout = new TimeSpan(0, 0, 30);
			binding.SendTimeout = timeout;
			binding.OpenTimeout = timeout;
			binding.ReceiveTimeout = timeout;
			EndPoint = (CommonClass.isVPSServer=="TRUE")?new EndpointAddress("http://www.wbidmax.com:8000/WBidDataDwonloadAuthService.svc/soap"):new EndpointAddress("http://122.166.23.155/WBidDataDownloadAuthorizationService/WBidDataDwonloadAuthService.svc/soap");
			return binding;
		}
		public static BasicHttpBinding CreateBasicHttpForOneminuteTimeOut()
		{
			BasicHttpBinding binding = new BasicHttpBinding
			{
				Name = "basicHttpBinding",
				MaxBufferSize = 2147483647,
				MaxReceivedMessageSize = 2147483647
			};
			TimeSpan timeout = new TimeSpan(0, 0, 59);
			binding.SendTimeout = timeout;
			binding.OpenTimeout = timeout;
			binding.ReceiveTimeout = timeout;

			return binding;
		}


		public static BasicHttpBinding CreateBasicHttp(int minute,int second)
		{
			BasicHttpBinding binding = new BasicHttpBinding
			{
				Name = "basicHttpBinding",
				MaxBufferSize = 2147483647,
				MaxReceivedMessageSize = 2147483647
			};

			binding.ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas()
			{
				MaxArrayLength = 2147483646,
				MaxStringContentLength = 5242880,
			};
			TimeSpan timeout = new TimeSpan(0, minute, second);
			binding.SendTimeout = timeout;
			binding.OpenTimeout = timeout;
			binding.ReceiveTimeout = timeout;
			binding.CloseTimeout = timeout;
			return binding;
		}
		public static StreamReader GetRestData(string serviceName,string jsonString)
		{
			string url = GlobalSettings.DataDownloadAuthenticationUrl + serviceName;
			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";

			var bytes = Encoding.UTF8.GetBytes(jsonString);
			request.ContentLength = bytes.Length;
			request.GetRequestStream().Write(bytes, 0, bytes.Length);
			//Response
			var response = (HttpWebResponse)request.GetResponse();
			var streamoutput = response.GetResponseStream();
			var readeroutput = new StreamReader(streamoutput);
			return readeroutput;

			// streamoutput.Dispose();
			// readeroutput.Dispose();
		}

		public static string JsonSerializer<T>(T t)
		{
			string jsonString = string.Empty;
			DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
			MemoryStream ms = new MemoryStream();
			ser.WriteObject(ms, t);
			jsonString = Encoding.UTF8.GetString(ms.ToArray());
			return jsonString;
		}

	}
}