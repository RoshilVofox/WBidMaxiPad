
using System;

using Foundation;
using UIKit;
using System.Collections.ObjectModel;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using System.Collections.Generic;
using System.Linq;
using WBid.WBidiPad.iOS.Utility;

namespace WBid.WBidiPad.iOS
{
	public partial class BAPopOverViewController : UIViewController
	{
		
		public BAPopOverViewController () : base ("BAPopOverViewController", null)
		{
		}
		public string PopOverType;

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
	

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			TableView.Source=new BAPopOverViewTableViewSource(PopOverType);

			
			// Perform any additional setup after loading the view, typically from a nib.
		}








	}
}

