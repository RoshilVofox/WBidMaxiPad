using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using WBid.WBidiPad.Model;

namespace WBid.WBidiPad.iOS.Common
{
    public class GlobalSettings
    {


        public static string ServerUrl = "108.60.201.50";


        public const decimal FAReserveDayPay = 6.0m;

        public static WBidINI WBidINIContent { get; set; }
        /// <summary>
        /// Store Current Bid Information
        /// </summary>
        public static BidDetails DownloadBidDetails { get; set; }
        /// <summary>
        /// Gets or sets the columndefenition.
        /// </summary>
        /// <value>The columndefenition.</value>
        public static List<ColumnDefinition> columndefinition { get; set; }


        /// <summary>
        /// Store Current Bid Information
        /// </summary>
        public static BidDetails CurrentBidDetails { get; set; }

        /// <summary>
        /// second Sunday in March.  Used to calculate herb time for FA takeoff and land times from raw data
        /// </summary>
        public static DateTime FirstDayOfDST { get; set; }

        /// <summary>
        /// first Sunday in November.  Used to calculate herb time for FA takeoff and land times from raw data
        /// </summary>
        public static DateTime LastDayOfDST { get; set; }
    }
}