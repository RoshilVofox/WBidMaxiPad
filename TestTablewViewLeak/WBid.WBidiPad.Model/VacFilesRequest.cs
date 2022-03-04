using System;
using System.Collections.Generic;

namespace WBid.WBidiPad.Model
{
    public class VacFilesRequest
    {
       
            public BidDataRequestDTO bidDataRequest { get; set; }
            public int checkVACBit { get; set; }
            public int? FAPositions { get; set; }
        public string vacFileName { get; set; }
        /// Added this for multiple Bid data files download
        /// </summary>
        public List<string> BidFileNames { get; set; }
    }
}
