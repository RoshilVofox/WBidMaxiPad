using System;
using System.Collections.Generic;
using ProtoBuf;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class FVVacationClass
    {
        public FVVacationClass()
        {
        }
       
    }
    [ProtoContract]
    public class FVVacationData
    {
        [ProtoMember(1)]
        public DateTime StartDate { get; set; }
        [ProtoMember(2)]
        public DateTime EndDate { get; set; }
        [ProtoMember(3)]
        public List<FVVacationTripData> FVVacationTripDatas { get; set; }
    }
    [ProtoContract]
    public class FVVacationTripData
    {
        [ProtoMember(1)]
        public string TripName { get; set; }
        [ProtoMember(2)]
        public decimal TripTfpInLine { get; set; }
        // public decimal TripTfpOutSideBp { get; set; }
        [ProtoMember(3)]
        public string Type { get; set; }
        [ProtoMember(4)]
        public DateTime TripStartDate { get; set; }
        [ProtoMember(5)]
        public DateTime TripEndDate { get; set; }
        [ProtoMember(6)]
        public int TripLegs { get; set; }
    }
    [ProtoContract]
    public class FVvacationLineData
    {
        [ProtoMember(1)]
        public DateTime FVStartDate { get; set; }
        [ProtoMember(2)]
        public DateTime FVEndDate { get; set; }
        [ProtoMember(3)]
        public List<FVVacationTripData> FVVacationTripDatas { get; set; }
        [ProtoMember(4)]
        public DateTime FVVAPStarManetDate { get; set; }
        [ProtoMember(5)]
        public DateTime FVVAPEndDate { get; set; }
        [ProtoMember(6)]
        public decimal FVVacTfp { get; set; }
        [ProtoMember(7)]
        public decimal FVVap { get; set; }

    }
    [ProtoContract]
    public class FVVAP
    {
        [ProtoMember(1)]
        public DateTime StartDate { get; set; }
        [ProtoMember(2)]
        public DateTime EndDate { get; set; }
        [ProtoMember(3)]
        public decimal Vap { get; set; }
    }
}
