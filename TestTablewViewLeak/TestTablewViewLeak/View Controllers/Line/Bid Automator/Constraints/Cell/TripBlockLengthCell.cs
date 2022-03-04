using System;

using Foundation;
using UIKit;
using WBid.WBidiPad.Model;
namespace WBid.WBidiPad.iOS
{
	public partial class TripBlockLengthCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("TripBlockLengthCell");
		public static readonly UINib Nib;

		ConstraintsChangeViewController _viewController;
		CxTripBlockLength _cellObject;

		static TripBlockLengthCell ()
		{
			Nib = UINib.FromName ("TripBlockLengthCell", NSBundle.MainBundle);
		}

		public TripBlockLengthCell (IntPtr handle) : base (handle)
		{
		}
		public static TripBlockLengthCell Create()
		{
			return (TripBlockLengthCell)Nib.Instantiate(null, null)[0];
		}

		public void Filldata(ConstraintsChangeViewController parentVC, CxTripBlockLength cellData)
		{
			_viewController = parentVC;
			_cellObject = cellData;
////			_cellObject.FourDay = true;
////			_cellObject.IsTrip = true;
//			_cellObject.IsBlock = !_cellObject.IsBlock;
			lbTripBlockLenght.Text = "TrpBlkLen"; 
			UIHelpers.StyleForButtonsBorderBlackRectangeThin (new UIButton[]{btnTrip, btnBlock});
			System.Threading.Timer timer = null; 
			timer = new System.Threading.Timer((obj) =>
				{
					UpdateUI();
					timer.Dispose();
				}, 
				null, 50, System.Threading.Timeout.Infinite);
//			UpdateUI ();
		}

		/// <summary>
		/// the Trip click event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void OnTripClickEvent (UIButton sender)
		{
			if (_cellObject != null) {
				_cellObject.IsBlock = false;
				//_cellObject.IsBlock = !_cellObject.IsTrip;
				UpdateUI();
			}
		}

		/// <summary>
		/// the delete button event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void OnDeleteEvent (UIButton sender)
		{
			Console.WriteLine("ondelete");
			if(_viewController!=null){
				_viewController.DeleteObject(_cellObject);
			}
		}

		partial void OnFouthButtonEvent (UIButton sender)
		{
			if (_cellObject != null) {
				_cellObject.FourDay = !_cellObject.FourDay;
				UpdateUI();
			}
		}

		partial void OnThirdButtonEvent (UIButton sender)
		{
			if (_cellObject != null) {
				_cellObject.ThreeDay = !_cellObject.ThreeDay;
				UpdateUI();
			}
		}

		partial void OnSecondButtonEvent (UIButton sender)
		{
			if (_cellObject != null) {
				_cellObject.Twoday = !_cellObject.Twoday;
				UpdateUI();
			}
		}

		partial void OnTurnButtonEvent (UIButton sender)
		{
			if (_cellObject != null) {
				_cellObject.Turns = !_cellObject.Turns;
				UpdateUI();
			}
		}
		private void UpdateUI(){
			InvokeOnMainThread(()=>{
				if (!_cellObject.IsBlock) {
					btnTrip.BackgroundColor = Colors.BidDarkGreen;
					btnTrip.SetTitleColor (UIColor.White, UIControlState.Normal);
					btnBlock.BackgroundColor = UIColor.White;
					btnBlock.SetTitleColor (UIColor.Black, UIControlState.Normal);
				} else {
					btnBlock.BackgroundColor = Colors.BidDarkGreen;
					btnBlock.SetTitleColor (UIColor.White, UIControlState.Normal);
					btnTrip.BackgroundColor = UIColor.White;
					btnTrip.SetTitleColor (UIColor.Black,UIControlState.Normal);
				}

				if (_cellObject.Turns) {
					btnTurn.BackgroundColor = Colors.BidRowGreen;
				} else {
					btnTurn.BackgroundColor = Colors.BidOrange;
				}
				if (_cellObject.Twoday) {
					btn2Day.BackgroundColor = Colors.BidRowGreen;
				} else {
					btn2Day.BackgroundColor = Colors.BidOrange;
				}
				if (_cellObject.ThreeDay) {
					btn3Day.BackgroundColor = Colors.BidRowGreen;
				} else {
					btn3Day.BackgroundColor = Colors.BidOrange;
				}
				if (_cellObject.FourDay) {
					btn4Day.BackgroundColor = Colors.BidRowGreen;
				} else {
					btn4Day.BackgroundColor = Colors.BidOrange;
				}
			});
		}

		partial void OnBlockEvent (UIButton sender)
		{
			if (_cellObject != null) {
				_cellObject.IsBlock = true;
				UpdateUI();
			}
		}
	}
}
