﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    public class BlockSort
    {
        /// <summary>
        /// Block Sort Id
        /// </summary>
        public int Id { get; set; }


        /// <summary>
        /// Block Sort Name
        /// </summary>
        public string Name { get; set; }

		public bool IsVisibleOptions { get; set; }

		public int SelectedBlockSort { get; set; }

		public string DisplayName { get; set; }



    }
}
