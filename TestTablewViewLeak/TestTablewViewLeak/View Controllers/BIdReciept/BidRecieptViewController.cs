using System;
using CoreGraphics;
using Foundation;
using UIKit;
using System.Collections.Generic;
using System.IO;
using WBid.WBidiPad.iOS.Utility;
using System.Linq;
namespace WBid.WBidiPad.iOS
{
	public partial class BidRecieptViewController : UIViewController
	{
		public string selectedFileName;
		public bool isPrintView;
		recieptCollectionController collectionVw;
		public BidRecieptViewController () : base ("BidRecieptViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
            string path = WBidHelper.GetAppDataPath();
			List<string> linefilenames=Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).Select(Path.GetFileName).Where(s => s.ToLower().EndsWith(".rct") || s.ToLower().EndsWith(".pdf")).ToList();
			linefilenames.Remove ("news.pdf");

           // List<string> linefilenames = Directory.EnumerateFiles(path, "*.RCT", SearchOption.AllDirectories).Select(Path.GetFileName).ToList();
			// Perform any additional setup after loading the view, typically from a nib.
			var layout = new UICollectionViewFlowLayout ();
			layout.SectionInset = new UIEdgeInsets (20,20,20,20);
			layout.MinimumInteritemSpacing = 10;
			layout.MinimumLineSpacing = 10;
			layout.ItemSize = new CGSize (110, 25);
			collectionVw = new recieptCollectionController (layout, linefilenames, this);
			collectionVw.View.Frame = this.vwCollectionContainer.Frame;

			this.vwCollectionContainer.RemoveFromSuperview ();
			this.AddChildViewController (collectionVw);
			this.Add (collectionVw.View);
		}
		/// <summary>
		/// Back button tapped. will dismiss the view.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void backButtonTapped (Foundation.NSObject sender)
		{
			this.DismissViewController(true,null);
		}
        /// <summary>
        /// Open the file viewer and show the corresponding receipt.
        /// </summary>
	
		public void ShowReceipt()
        {
            InvokeOnMainThread(() =>
            {
                webPrint fileViewer = new webPrint();
                this.PresentViewController(fileViewer, true, () =>
                {
							if(Path.GetExtension(selectedFileName).ToLower()==".rct")
							{
                    fileViewer.loadFileFromUrl(selectedFileName);
							}
							else
								fileViewer.LoadPDFdocument (Path.GetFileName(selectedFileName));
                });
            });
        }




	}
}

