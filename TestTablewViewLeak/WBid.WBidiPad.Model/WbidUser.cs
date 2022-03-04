using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    public class WbidUser
    {
        /// <summary>
        /// Store all the user information like Emp no,Domicile,postion,and Seniority number
        /// </summary>
        public UserInformation UserInformation { get; set; }

        /// <summary>
        /// Holds the recent file
        /// </summary>
        public RecentFiles RecentFiles { get; set; }
        /// <summary>
        /// Holds the Bid data file names
        /// </summary>
        public List<AppDataBidFileNames> AppDataBidFiles { get; set; }
    }
}
