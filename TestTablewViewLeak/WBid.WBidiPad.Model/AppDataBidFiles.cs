using System;
using System.Collections.Generic;

namespace WBid.WBidiPad.Model
{
    public class AppDataBidFileNames
    {
        public AppDataBidFileNames()
        {
        }
        public string Domicile { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string Round { get; set; }
        public string Position { get; set; }
        public List<BidFileNames> lstBidFileNames { get; set; }
    }
    public class BidFileNames
    {
        public string FileName { get; set; }
        /*
         * NormalLine = 1,
            Trip,
            Vacation,
            VacationDropOFF,
            Eom,
            EomDropOFF,
            VacationEOM,
            VacationEomDropOFF,
         * */
        public int FileType { get; set; }
    }
}
