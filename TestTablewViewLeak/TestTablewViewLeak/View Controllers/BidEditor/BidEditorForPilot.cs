using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Core;
using CoreGraphics;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.iOS;
using WBid.WBidiPad.Model;
using System.Collections.Generic;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using System.Linq;
using WBid.WBidiPad.iOS.Utility;
using System.Collections.ObjectModel;

namespace WBid.WBidiPad.iOS
{
	public partial class BidEditorForPilot : UIViewController
	{
		public List <string> selectedLines;
		pilotBidEditorCollectionController collectionVw;
		public string highlightedLIneInSelectedLines;
		public BidEditorForPilot () : base ("BidEditorForPilot", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewWillDisappear(bool animated)
		{

			base.ViewWillDisappear(animated);
			foreach (UIView view in this.View.Subviews) {

				DisposeClass.DisposeEx(view);
			}

		}
		partial void btnSubmitTapped (Foundation.NSObject sender)
		{
			tblSelectedLInes.SeparatorInset = new UIEdgeInsets (0, 3, 0, 3);

            //set the properties required to POST the webrequest to SWA server.
            SubmitBid submitBid = new SubmitBid();
            submitBid.Base =GlobalSettings.BidPrepDetails.Domicile;
            submitBid.Bidder = GlobalSettings.TemporaryEmployeeNumber;
            submitBid.BidRound = (GlobalSettings.BidPrepDetails.BidRound == "B") ? "Round 2" : "Round 1";
            submitBid.PacketId = GenaratePacketId();
            submitBid.Seat = GlobalSettings.BidPrepDetails.Position;
            if (GlobalSettings.BidPrepDetails.Position != "CP")
            {
                AvoidanceBids avoidanceBids = GlobalSettings.WBidINIContent.AvoidanceBids;
                if (GlobalSettings.BidPrepDetails.IsChkAvoidanceBid)
                {
                    submitBid.Pilot1 = submitBid.Pilot2 = submitBid.Pilot3 = null;

                }
                else
                {
                    submitBid.Pilot1 = (avoidanceBids.Avoidance1 == "0") ? null : avoidanceBids.Avoidance1;
                    submitBid.Pilot2 = (avoidanceBids.Avoidance2 == "0") ? null : avoidanceBids.Avoidance2;
                    submitBid.Pilot3 = (avoidanceBids.Avoidance3 == "0") ? null : avoidanceBids.Avoidance3;
                }
            }
            //genarate bid line to submit
            submitBid.Bid = GenarateBidLineString();
            submitBid.TotalBidChoices = selectedLines.Count();
            GlobalSettings.SubmitBid = submitBid;
			CommonClass.ObjBidEditorPilot = this;
			queryViewController qv = new queryViewController();
			qv.isFirstTime=true;
			qv.isFromView = queryViewController.queryFromView.queryBidEditorCPORFO;
			qv.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
UINavigationController nav = new UINavigationController(qv);
nav.ModalPresentationStyle =  UIModalPresentationStyle.FormSheet;
				//nav.PushViewController(details, true);
				this.PresentViewController(nav, true, null);

		}

		public void dismissView()
		{
			this.DismissViewController(true, null);

		}
		partial void btnAvoidanceTapped (Foundation.NSObject sender)
		{
			AvoidanceBidVC avBid = new AvoidanceBidVC();
			avBid.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
			this.PresentViewController(avBid,true,null);
			//This should not be done. We should keep three Apple-said sizes. 
			//avBid.View.Superview.BackgroundColor = UIColor.Clear;
			//avBid.View.Frame = new RectangleF (0,130,540,350);
			//avBid.View.Layer.BorderWidth = 1;
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			tblSelectedLInes.SeparatorInset = new UIEdgeInsets (0, 3, 0, 3);

			var layout = new UICollectionViewFlowLayout ();
			layout.SectionInset = new UIEdgeInsets (2,2,2,2);
			layout.MinimumInteritemSpacing = 2;
			layout.MinimumLineSpacing = 2;
			collectionVw = new pilotBidEditorCollectionController (layout, this);
			collectionVw.View.Frame = vwCollectionContainer.Frame;
			this.AddChildViewController (collectionVw);
			this.Add (collectionVw.View);
			if(GlobalSettings.BidPrepDetails.Position == "CP" || GlobalSettings.CurrentBidDetails.Round=="S")
			btnAvoidance.Enabled = false;


			selectedLines = new List<string>();
            GenerateLineList();
			setBidCountLabel ();
			tblSelectedLInes.Source = new pilotBidEditorTableSource (this);
			tblSelectedLInes.SetEditing (true, true);
			tblSelectedLInes.AllowsSelectionDuringEditing = true;
			tblSelectedLInes.Layer.BorderWidth = 1;
            
			// Perform any additional setup after loading the view, typically from a nib.
			btnSubmit.Enabled = false;

			if(GlobalSettings.BidPrepDetails.Position == "CP")
				lblTitle.Text = "Bid Editor - Pilots";
			else if (GlobalSettings.BidPrepDetails.Position == "FA")
				lblTitle.Text = "Bid Editor - FA";
		}
		public void setBidCountLabel ()
		{
			if (selectedLines.Count == 0) {
				btnSubmit.Enabled = false;
				lblBidNumber.Text = "No Bids";
			} else {
				btnSubmit.Enabled = true;
				lblBidNumber.Text = selectedLines.Count + " Bids";
			}

		}
        private void GenerateLineList()
        {
           // BidList = new ObservableCollection<LineDetails>();

            if (GlobalSettings.BidPrepDetails.IsOnStartWithCurrentLine)
            {
                int count =GlobalSettings.BidPrepDetails.LineFrom;

                foreach (Line line in GlobalSettings.Lines)
                {
                    selectedLines.Add(line.LineNum.ToString());
                    count++;
                }
            }
       }

        //private void SetTotalLines()
        //{
        //    TotalLines = LineList.Count.ToString() + ((LineList.Count > 1) ? " Bids" : " Bid");
        //    IsEnableSubmit = (LineList.Count == 0) ? false : true;
        //}
		partial void btnEmployeeNumberTapped (Foundation.NSObject sender)
		{
			ChangeEmpNumberView changeEmp = new ChangeEmpNumberView();
			changeEmp.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
			this.PresentViewController(changeEmp, true, null);
			

		}


	
		partial void btnDeleteTapped (Foundation.NSObject sender)
		{
			//Remove that line reload table and collection view.

			selectedLines.Remove(highlightedLIneInSelectedLines);
			tblSelectedLInes.ReloadData();
			collectionVw.CollectionView.ReloadData();
			highlightedLIneInSelectedLines = null;
			setBidCountLabel();

		}
		partial void btnClearAllTapped (Foundation.NSObject sender)
		{
			selectedLines.Clear();
			tblSelectedLInes.ReloadData();
			highlightedLIneInSelectedLines = null;
			collectionVw.CollectionView.ReloadData();
			setBidCountLabel();

		}
		partial void btnCancelTapped (Foundation.NSObject sender)
		{
			//save selectedlines to file.
			this.DismissViewController(true, null);

        }

        #region PrivateMethods

        /// <summary>
        /// PURPOSE : Generate Bid lines
        /// </summary>
        /// <returns></returns>
        private string GenarateBidLineString()
        {
            string bidLines = string.Empty;
            bidLines = string.Join(",", selectedLines.Select(x => x.ToString()));
            return bidLines;
        }


        /// <summary>
        /// PURPOSE :Genarate Packet Id for Submit Bid Format:
        // Format: BASE || Year || Month || bid-round-number eg(Value=BWI2001032)
        /// </summary>
        /// <param name="bidDetails"></param>
        /// <returns></returns>
        private string GenaratePacketId()
        {
            string packetid = string.Empty;
            packetid = GlobalSettings.BidPrepDetails.Domicile + GlobalSettings.BidPrepDetails.BidYear + GlobalSettings.BidPrepDetails.BidPeriod.ToString("d2");

            //Set-round-numbers:
            //1 - F/A monthly bids
            //2 - F/A supplemental bids
            //3 - reserved
            //4 - Pilot monthly bids
            //5 - Pilot supplemental bids

            //D first Round  B second Round
            if (GlobalSettings.BidPrepDetails.Position == "FA")
            {
                packetid += (GlobalSettings.BidPrepDetails.BidRound == "D") ? "1" : "2";
            }
            else
            {
                packetid += (GlobalSettings.BidPrepDetails.BidRound == "D") ? "4" : "5";
            }

            return packetid;
        }
        #endregion
    }

	public class pilotBidEditorTableSource : UITableViewSource
	{
		//		string[] arrayTitle = {"Ord","ic1","ic2","ic3","Pos","Line","Points","Pay","Wts","WkEnd","A/P","Off","T234","Flt","$/Hr","VacPay","Vof","Vob","VDrop","AcChg","SEL","MOV"};
		//		List<ColumnDefinition> columndefenition = GlobalSettings.columndefinition;
		//		List<DataColumn> datacolumn = GlobalSettings.WBidINIContent.DataColumns;

		BidEditorForPilot parentVC;
		public pilotBidEditorTableSource (BidEditorForPilot parent )
		{
			parentVC = parent;
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			//			return columndefenition.Count; //But only 22 should be shown at a time
			return parentVC.selectedLines.Count;
		}
//		public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
//		{
//
//		}
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			NSString cellIdentifier = new NSString ("cellIdentifier");
			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier) as UITableViewCell;
			if (cell == null)
				cell = new UITableViewCell ();

			cell.TextLabel.Text = parentVC.selectedLines[indexPath.Row].ToString();
            cell.TextLabel.AdjustsFontSizeToFitWidth = true;
//			cell.bindData
			return cell;
		}
		public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath)
		{
			return true;
		}


		public override void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
		{
			UpdateDatacolumnInSourceToIndex (sourceIndexPath.Row, destinationIndexPath.Row);
		}
		public override UITableViewCellEditingStyle EditingStyleForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return UITableViewCellEditingStyle.None;
		}
		public void UpdateDatacolumnInSourceToIndex(int sourceIndex, int destinationIndex)
		{
			string sel = parentVC.selectedLines [sourceIndex];
			parentVC.selectedLines.RemoveAt (sourceIndex);
			parentVC.selectedLines.Insert (destinationIndex, sel);
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			parentVC.highlightedLIneInSelectedLines = parentVC.selectedLines [indexPath.Row];
		}


	}
}

