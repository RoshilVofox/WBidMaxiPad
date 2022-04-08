// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WBid.WBidiPad.iOS
{
	[Register ("lineViewController")]
	partial class lineViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITableView BidLineTableView { get; set; }

		[Outlet]
		UIKit.UIButton btnBidAutomator { get; set; }

		[Outlet]
		UIKit.UIButton btnBidStuff { get; set; }

		[Outlet]
		UIKit.UIButton btnBlueShade { get; set; }

		[Outlet]
		UIKit.UIButton btnBrightness { get; set; }

		[Outlet]
		UIKit.UIButton btnCalPrint { get; set; }

		[Outlet]
		UIKit.UIButton btnCSW { get; set; }

		[Outlet]
		UIKit.UIButton btnDownArrow { get; set; }

		[Outlet]
		UIKit.UIButton btnEOM { get; set; }

		[Outlet]
		UIKit.UIButton btnGrid { get; set; }

		[Outlet]
		UIKit.UIButton btnHelp { get; set; }

		[Outlet]
		UIKit.UIButton btnHome { get; set; }

		[Outlet]
		UIKit.UIButton btnLineBotLock { get; set; }

		[Outlet]
		UIKit.UIButton btnLineTopLock { get; set; }

		[Outlet]
		public UIKit.UIButton btnMIL { get; private set; }

		[Outlet]
		UIKit.UIButton btnMisc { get; set; }

		[Outlet]
		UIKit.UIButton btnMovDown { get; set; }

		[Outlet]
		UIKit.UIButton btnMovUp { get; set; }

		[Outlet]
		UIKit.UIButton btnOverlap { get; set; }

		[Outlet]
		UIKit.UIButton btnPairing { get; set; }

		[Outlet]
		UIKit.UIButton btnPromote { get; set; }

		[Outlet]
		UIKit.UIButton btnQuickSet { get; set; }

		[Outlet]
		UIKit.UIButton btnRedo { get; set; }

		[Outlet]
		UIKit.UIButton btnRemBottomLock { get; set; }

		[Outlet]
		UIKit.UIButton btnRemTopLock { get; set; }

		[Outlet]
		UIKit.UIButton btnReparse { get; set; }

		[Outlet]
		UIKit.UIButton btnReparseCheck { get; set; }

		[Outlet]
		UIKit.UIButton btnReparseClose { get; set; }

		[Outlet]
		UIKit.UIButton btnReset { get; set; }

		[Outlet]
		UIKit.UIButton btnSave { get; set; }

		[Outlet]
		UIKit.UIButton btnSynch { get; set; }

		[Outlet]
		UIKit.UIButton btnTrash { get; set; }

		[Outlet]
		UIKit.UIButton btnTripExport { get; set; }

		[Outlet]
		UIKit.UIButton btnTripPrint { get; set; }

		[Outlet]
		UIKit.UIButton btnUndo { get; set; }

		[Outlet]
		UIKit.UIButton btnUpArrow { get; set; }

		[Outlet]
		UIKit.UIButton btnVacCorrect { get; set; }

		[Outlet]
		UIKit.UIButton btnVacDiff { get; set; }

		[Outlet]
		UIKit.UIButton btnVacDrop { get; set; }

		[Outlet]
		UIKit.UILabel lblCalTitle { get; set; }

		[Outlet]
		UIKit.UILabel lblMOV { get; set; }

		[Outlet]
		UIKit.UILabel lblSEL { get; set; }

		[Outlet]
		UIKit.UILabel lblTitle { get; set; }

		[Outlet]
		UIKit.UILabel lblTripPopTitle { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITableView ModernTableView { get; set; }

		[Outlet]
		UIKit.UISegmentedControl sgControlViewType { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITableView SummaryTableView { get; set; }

		[Outlet]
		UIKit.UIView tbBottomBar { get; set; }

		[Outlet]
		UIKit.UIView tbTopBar { get; set; }

		[Outlet]
		UIKit.UITextField txtGoToLine { get; set; }

		[Outlet]
		UIKit.UITextField txtReparse { get; set; }

		[Outlet]
		UIKit.UIView vwBidLineContainer { get; set; }

		[Outlet]
		UIKit.UIView vwCalChild { get; set; }

		[Outlet]
		UIKit.UIView vwCalPopover { get; set; }

		[Outlet]
		UIKit.UIView vwContainerView { get; set; }

		[Outlet]
		UIKit.UIView vwHeader { get; set; }

		[Outlet]
		UIKit.UIView vwReparse { get; set; }

		[Outlet]
		UIKit.UIView vwSummaryContainer { get; set; }

		[Outlet]
		UIKit.UIView vwTable { get; set; }

		[Outlet]
		UIKit.UIView vwTripChild { get; set; }

		[Outlet]
		UIKit.UIView vwTripPopover { get; set; }

		[Action ("BidAutomatorButtonClicked:")]
		partial void BidAutomatorButtonClicked (Foundation.NSObject sender);

		[Action ("btnBidStuffTap:")]
		partial void btnBidStuffTap (UIKit.UIButton sender);

		[Action ("btnBrightnessTapped:")]
		partial void btnBrightnessTapped (UIKit.UIButton sender);

		[Action ("btnCalendarExport:")]
		partial void btnCalendarExport (Foundation.NSObject sender);

		[Action ("btnCalPrintTapped:")]
		partial void btnCalPrintTapped (UIKit.UIButton sender);

		[Action ("btnCSWTap:")]
		partial void btnCSWTap (UIKit.UIButton sender);

		[Action ("btnEOMTapped:")]
		partial void btnEOMTapped (UIKit.UIButton sender);

		[Action ("btnExportTapped:")]
		partial void btnExportTapped (UIKit.UIBarButtonItem sender);

		[Action ("btnGridTap:")]
		partial void btnGridTap (UIKit.UIButton sender);

		[Action ("btnHelpTap:")]
		partial void btnHelpTap (UIKit.UIButton sender);

		[Action ("btnHomeTap:")]
		partial void btnHomeTap (UIKit.UIButton sender);

		[Action ("btnLineBotLockTap:")]
		partial void btnLineBotLockTap (UIKit.UIButton sender);

		[Action ("btnLineTopLockTap:")]
		partial void btnLineTopLockTap (UIKit.UIButton sender);

		[Action ("btnMILTapped:")]
		partial void btnMILTapped (UIKit.UIButton sender);

		[Action ("btnMiscTap:")]
		partial void btnMiscTap (UIKit.UIButton sender);

		[Action ("btnMovDownTap:")]
		partial void btnMovDownTap (UIKit.UIButton sender);

		[Action ("btnMovUpTap:")]
		partial void btnMovUpTap (UIKit.UIButton sender);

		[Action ("btnOverlapTap:")]
		partial void btnOverlapTap (UIKit.UIButton sender);

		[Action ("btnPairingTapped:")]
		partial void btnPairingTapped (UIKit.UIButton sender);

		[Action ("btnPromoteTap:")]
		partial void btnPromoteTap (UIKit.UIButton sender);

		[Action ("btnRemBottomLockTap:")]
		partial void btnRemBottomLockTap (UIKit.UIButton sender);

		[Action ("btnRemTopLockTap:")]
		partial void btnRemTopLockTap (UIKit.UIButton sender);

		[Action ("btnResetTapped:")]
		partial void btnResetTapped (UIKit.UIButton sender);

		[Action ("btnSaveTap:")]
		partial void btnSaveTap (UIKit.UIButton sender);

		[Action ("btnTrashTap:")]
		partial void btnTrashTap (UIKit.UIButton sender);

		[Action ("btnTripCloseTapped:")]
		partial void btnTripCloseTapped (UIKit.UIButton sender);

		[Action ("btnTripExportTapped:")]
		partial void btnTripExportTapped (UIKit.UIButton sender);

		[Action ("btnTripPrintTapped:")]
		partial void btnTripPrintTapped (UIKit.UIButton sender);

		[Action ("btnVacCorrectTap:")]
		partial void btnVacCorrectTap (UIKit.UIButton sender);

		[Action ("btnVacDiffClicked:")]
		partial void btnVacDiffClicked (Foundation.NSObject sender);

		[Action ("btnVacDropTap:")]
		partial void btnVacDropTap (UIKit.UIButton sender);

		[Action ("funClearBlueBorder:")]
		partial void funClearBlueBorder (Foundation.NSObject sender);

		[Action ("sgControlViewTypeTap:")]
		partial void sgControlViewTypeTap (UIKit.UISegmentedControl sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnVacDiff != null) {
				btnVacDiff.Dispose ();
				btnVacDiff = null;
			}

			if (btnBidAutomator != null) {
				btnBidAutomator.Dispose ();
				btnBidAutomator = null;
			}

			if (btnBidStuff != null) {
				btnBidStuff.Dispose ();
				btnBidStuff = null;
			}

			if (btnBlueShade != null) {
				btnBlueShade.Dispose ();
				btnBlueShade = null;
			}

			if (btnBrightness != null) {
				btnBrightness.Dispose ();
				btnBrightness = null;
			}

			if (btnCalPrint != null) {
				btnCalPrint.Dispose ();
				btnCalPrint = null;
			}

			if (btnCSW != null) {
				btnCSW.Dispose ();
				btnCSW = null;
			}

			if (btnDownArrow != null) {
				btnDownArrow.Dispose ();
				btnDownArrow = null;
			}

			if (btnEOM != null) {
				btnEOM.Dispose ();
				btnEOM = null;
			}

			if (btnGrid != null) {
				btnGrid.Dispose ();
				btnGrid = null;
			}

			if (btnHelp != null) {
				btnHelp.Dispose ();
				btnHelp = null;
			}

			if (btnHome != null) {
				btnHome.Dispose ();
				btnHome = null;
			}

			if (btnLineBotLock != null) {
				btnLineBotLock.Dispose ();
				btnLineBotLock = null;
			}

			if (btnLineTopLock != null) {
				btnLineTopLock.Dispose ();
				btnLineTopLock = null;
			}

			if (btnMIL != null) {
				btnMIL.Dispose ();
				btnMIL = null;
			}

			if (btnMisc != null) {
				btnMisc.Dispose ();
				btnMisc = null;
			}

			if (btnMovDown != null) {
				btnMovDown.Dispose ();
				btnMovDown = null;
			}

			if (btnMovUp != null) {
				btnMovUp.Dispose ();
				btnMovUp = null;
			}

			if (btnOverlap != null) {
				btnOverlap.Dispose ();
				btnOverlap = null;
			}

			if (btnPairing != null) {
				btnPairing.Dispose ();
				btnPairing = null;
			}

			if (btnPromote != null) {
				btnPromote.Dispose ();
				btnPromote = null;
			}

			if (btnQuickSet != null) {
				btnQuickSet.Dispose ();
				btnQuickSet = null;
			}

			if (btnRedo != null) {
				btnRedo.Dispose ();
				btnRedo = null;
			}

			if (btnRemBottomLock != null) {
				btnRemBottomLock.Dispose ();
				btnRemBottomLock = null;
			}

			if (btnRemTopLock != null) {
				btnRemTopLock.Dispose ();
				btnRemTopLock = null;
			}

			if (btnReparse != null) {
				btnReparse.Dispose ();
				btnReparse = null;
			}

			if (btnReparseCheck != null) {
				btnReparseCheck.Dispose ();
				btnReparseCheck = null;
			}

			if (btnReparseClose != null) {
				btnReparseClose.Dispose ();
				btnReparseClose = null;
			}

			if (btnReset != null) {
				btnReset.Dispose ();
				btnReset = null;
			}

			if (btnSave != null) {
				btnSave.Dispose ();
				btnSave = null;
			}

			if (btnSynch != null) {
				btnSynch.Dispose ();
				btnSynch = null;
			}

			if (btnTrash != null) {
				btnTrash.Dispose ();
				btnTrash = null;
			}

			if (btnTripExport != null) {
				btnTripExport.Dispose ();
				btnTripExport = null;
			}

			if (btnTripPrint != null) {
				btnTripPrint.Dispose ();
				btnTripPrint = null;
			}

			if (btnUndo != null) {
				btnUndo.Dispose ();
				btnUndo = null;
			}

			if (btnUpArrow != null) {
				btnUpArrow.Dispose ();
				btnUpArrow = null;
			}

			if (btnVacCorrect != null) {
				btnVacCorrect.Dispose ();
				btnVacCorrect = null;
			}

			if (btnVacDrop != null) {
				btnVacDrop.Dispose ();
				btnVacDrop = null;
			}

			if (lblCalTitle != null) {
				lblCalTitle.Dispose ();
				lblCalTitle = null;
			}

			if (lblMOV != null) {
				lblMOV.Dispose ();
				lblMOV = null;
			}

			if (lblSEL != null) {
				lblSEL.Dispose ();
				lblSEL = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (lblTripPopTitle != null) {
				lblTripPopTitle.Dispose ();
				lblTripPopTitle = null;
			}

			if (sgControlViewType != null) {
				sgControlViewType.Dispose ();
				sgControlViewType = null;
			}

			if (tbBottomBar != null) {
				tbBottomBar.Dispose ();
				tbBottomBar = null;
			}

			if (tbTopBar != null) {
				tbTopBar.Dispose ();
				tbTopBar = null;
			}

			if (txtGoToLine != null) {
				txtGoToLine.Dispose ();
				txtGoToLine = null;
			}

			if (txtReparse != null) {
				txtReparse.Dispose ();
				txtReparse = null;
			}

			if (vwBidLineContainer != null) {
				vwBidLineContainer.Dispose ();
				vwBidLineContainer = null;
			}

			if (vwCalChild != null) {
				vwCalChild.Dispose ();
				vwCalChild = null;
			}

			if (vwCalPopover != null) {
				vwCalPopover.Dispose ();
				vwCalPopover = null;
			}

			if (vwContainerView != null) {
				vwContainerView.Dispose ();
				vwContainerView = null;
			}

			if (vwHeader != null) {
				vwHeader.Dispose ();
				vwHeader = null;
			}

			if (vwReparse != null) {
				vwReparse.Dispose ();
				vwReparse = null;
			}

			if (vwSummaryContainer != null) {
				vwSummaryContainer.Dispose ();
				vwSummaryContainer = null;
			}

			if (vwTable != null) {
				vwTable.Dispose ();
				vwTable = null;
			}

			if (vwTripChild != null) {
				vwTripChild.Dispose ();
				vwTripChild = null;
			}

			if (vwTripPopover != null) {
				vwTripPopover.Dispose ();
				vwTripPopover = null;
			}

			if (BidLineTableView != null) {
				BidLineTableView.Dispose ();
				BidLineTableView = null;
			}

			if (ModernTableView != null) {
				ModernTableView.Dispose ();
				ModernTableView = null;
			}

			if (SummaryTableView != null) {
				SummaryTableView.Dispose ();
				SummaryTableView = null;
			}
		}
	}
}
