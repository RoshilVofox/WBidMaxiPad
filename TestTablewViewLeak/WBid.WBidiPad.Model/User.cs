using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{

	[Serializable]
	public class User
	{
		[XmlAttribute("IsOn")]
		public bool IsOn { get; set; }


		[XmlAttribute("SmartSynch")]
		public bool SmartSynch { get; set; }

		[XmlAttribute("AutoSave")]
		public bool AutoSave { get; set; }

		[XmlAttribute("AutoSavevalue")]
		public int AutoSavevalue { get; set; }

		[XmlAttribute("IsNeedCrashMail")]
		public bool IsNeedCrashMail { get; set; }


		[XmlAttribute("MIL")]
		public bool MIL { get; set; }

		[XmlAttribute("IsNeedBidReceipt")]
		public bool IsNeedBidReceipt { get; set; }

		[XmlAttribute("IsSouthWestWifiTest")]
		public bool IsSouthWestWifiTest { get; set; }

		[XmlAttribute("IsSummaryViewShade")]
		public bool IsSummaryViewShade { get; set; }

		[XmlAttribute("IsModernViewShade")]
		public bool IsModernViewShade { get; set; }


	}
} 
