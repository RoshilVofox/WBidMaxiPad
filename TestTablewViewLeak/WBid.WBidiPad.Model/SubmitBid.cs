using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace WBid.WBidiPad.Model
{
	[ProtoContract]
    public class SubmitBid
    {
		[ProtoMember(1)]
        public string Base { get; set; }
		[ProtoMember(2)]
        public string Bid { get; set; }
		[ProtoMember(3)]
        public string BidRound { get; set; }
		[ProtoMember(4)]
        public string Bidder { get; set; }
		[ProtoMember(5)]
        public string PacketId { get; set; }
		[ProtoMember(6)]
        public string Seat { get; set; }
		[ProtoMember(7)]
        public string Pilot1 { get; set; }
		[ProtoMember(8)]
        public string Pilot2 { get; set; }
		[ProtoMember(9)]
        public string Pilot3 { get; set; }
		[ProtoMember(10)]
        public int SeniorityNumber { get; set; }
		[ProtoMember(11)]
        public string Buddy1 { get; set; }
		[ProtoMember(12)]
        public string Buddy2 { get; set; }
		[ProtoMember(13)]
        public bool IsSubmitAllChoices { get; set; }
		[ProtoMember(14)]
        public List<BuddyBidderBid> BuddyBidderBids { get; set; }
		[ProtoMember(15)]
        public int TotalBidChoices { get; set; }
    }
}
