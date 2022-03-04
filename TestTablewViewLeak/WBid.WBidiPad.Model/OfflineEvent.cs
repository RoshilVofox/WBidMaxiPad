using System;
using ProtoBuf;
using System.Collections.Generic;

namespace WBid.WBidiPad.Model
{
	
	public class OfflineEvents
	{
		public OfflineEvents ()
		{
		}

		public  List<LogDatas>  EventLogs{ get; set;}
	}

	public class LogDatas
	{
		
		public DateTime date { get; set;}

		public LogDetails LogDetails { get; set;}
	}
}

