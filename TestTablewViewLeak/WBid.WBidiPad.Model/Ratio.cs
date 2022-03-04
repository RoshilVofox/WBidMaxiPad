using System;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
	
	public class Ratio
	{
		public Ratio()
		{
		}
		[XmlAttribute("Numerator")]
		public int Numerator { get; set; }

		[XmlAttribute("Denominator")]
		public int Denominator { get; set; }

	}

}
