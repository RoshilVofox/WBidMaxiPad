using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class WorkBlockDetails
    {
//        [ProtoMember(1)]
//        public int StartTime { get; set; }
//
//        [ProtoMember(2)]
//        public int EndTime { get; set; }
//        [ProtoMember(3)]
//        public int StartDay { get; set; }
//
//        [ProtoMember(4)]
//        public int EndDay { get; set; }
//        [ProtoMember(5)]
//		public int BackToBackCount { get; set; }try
//
		//Start time of the Work Block
		[ProtoMember(1)]
		public int StartTime { get; set; }

		//End time of the Work Block
		[ProtoMember(2)]
		public int EndTime { get; set; }

		//Day number of Start Date of the Work Block (Sunday 0,Monday 1 etc
		[ProtoMember(3)]
		public int StartDay { get; set; }

		//Day number of End Date of the Work Block (Sunday 0,Monday 1 etc
		[ProtoMember(4)]
		public int EndDay { get; set; }

		//Back to back trip count
		[ProtoMember(6)]
		public int BackToBackCount { get; set; }

		[ProtoMember(7)]
		public DateTime StartDateTime { get; set; }


		[ProtoMember(8)]
		public DateTime EndDateTime { get; set; }

		// this property is needed because of the companies irregular method of keeping times
		// for example, a trip may end on the 4th, but after midnight.  Which means the trip ends on the 5th 
		// but we need to compare the EndDateTime against the last departure time for the trip end date
		// if the last if the pairing finishes after midnight on the 5th, we need to find the last departure time on the 4th
		[ProtoMember(9)]
		public DateTime EndDate { get; set; }

		[ProtoMember(10)]
		public int nightINDomicile;

        [ProtoMember(11)]
        public int BriefTime;
	}
}
