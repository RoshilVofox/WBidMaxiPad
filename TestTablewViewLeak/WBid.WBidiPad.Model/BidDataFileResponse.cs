using System;
using System.Collections.Generic;

namespace WBid.WBidiPad.Model
{
    public class BidDataFileResponse
    {
        public BidDataFileResponse()
        {
        }
        public List<BidDataFiles> bidData { get; set; }
        public List<Vacation> Vacation { get; set; }
        public List<Absense> FVVacation { get; set; }
        public List<BidFileNames> BidFileNames { get; set; }
        public int EmpNum { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }
        public int DomcileSeniority { get; set; }

        public int TotalSenliorityMember { get; set; }

        public bool IsSeniorityExist { get; set; }
        public int paperCount { get; set; }

        public bool ISEBGUser { get; set; }
    }
}
