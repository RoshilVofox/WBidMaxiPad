using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace WBid.WBidiPad.Model
{
	[Serializable]
	public class BidAutoGroup
	{
		[XmlAttribute("GrpName")]
		public string GroupName { get; set; }

		//[XmlAttribute("Lines")]
		[XmlElement("Lines")]
		public List<int> Lines { get; set; }
	}
}

