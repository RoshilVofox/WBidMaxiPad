
using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.SharedLibrary.Utility;
using System.Collections.Generic;
using WBid.WBidiPad.iOS.Utility;
//using WBid.WBidiPad.PortableLibrary.Core;
using System.Linq;
using WBid.WBidiPad.Core;
//using WBid.WBidiPad.iOS.Common;

namespace TestTablewViewLeak.ViewControllers
{
    public class SecretDataTableViewSource : UITableViewSource
    {

        List<Domicile> listDomicile = GlobalSettings.WBidINIContent.Domiciles.OrderBy(x => x.DomicileName).ToList();

        SecretDataDownload parentVC;
        public SecretDataTableViewSource(SecretDataDownload parent)
        {
            parentVC = parent;
        }

        public SecretDomicile obj;
       // public List<string> DomicileList = GlobalSettings.WBidINIContent.Domiciles;
                                                                     

        public override nint NumberOfSections(UITableView tableView)
        {
            // TODO: return the actual number of sections
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            // TODO: return the actual number of items in the section
            //return DomicileList.Count;
            return listDomicile.Count;
        }


  
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.RegisterNibForCellReuse(UINib.FromName("SecretDomicile", NSBundle.MainBundle), "SecretDomicile");
            SecretDomicile cell = (SecretDomicile)tableView.DequeueReusableCell(new NSString("SecretDomicile"), indexPath);
            cell.LabelValues(listDomicile.Select(x=>x.DomicileName).ToList()[indexPath.Row],indexPath.Row,parentVC);

                return cell;

        }
    }
}



