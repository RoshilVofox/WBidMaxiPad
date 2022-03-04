using System;
using System.Collections.Generic;

namespace WBid.WBidiPad.Model
{
    public class MissedTripResponseModel
    {
        public MissedTripResponseModel()
        {
        }
        public List<Trip> JsonTripData { get; set; }
        public string FileName { get; set; }
        public string Message { get; set; }
    }
}
