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
using System.Json;


namespace WBid.WBidiPad.iOS
{
	public partial class CAPViewController : UIViewController,IServiceDelegate
    {
		
        public CAPViewController () : base ("CAPViewController", null)
        {
        }
		LoadingOverlay ActivityIndicator;
		CAPInputParameter objCAPInputParameter;
		OdataBuilder ObjOdata;
		List<CAPOutputParameter> lstCAPOutputParameter;
        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            // Perform any additional setup after loading the view, typically from a nib.

			ObjOdata=new OdataBuilder();
			objCAPInputParameter=new CAPInputParameter();
			ObjOdata.RestService.Objdelegate = this;
			objCAPInputParameter.Year = DateTime.Now.AddMonths(1).Year;
			objCAPInputParameter.Month = DateTime.Now.AddMonths(1).Month;
			//objCAPInputParameter.Year = GlobalSettings.CurrentBidDetails.Year;
			//objCAPInputParameter.Month = GlobalSettings.CurrentBidDetails.Month+1;

			ObjOdata.CAPDetails (objCAPInputParameter);


        }

		public void ServiceResponce(JsonValue jsonDoc)
		{
			InvokeOnMainThread (() => {
				Console.WriteLine ("Service Success");
				//ActivityIndicator.Hide ();
				lstCAPOutputParameter = CommonClass.ConvertJSonToObject<List<CAPOutputParameter>> (jsonDoc.ToString ());
                lstCAPOutputParameter = lstCAPOutputParameter.Where(x => x.CurrentMonthCap != null).ToList();
				tblCAPDetails.Source = new CAPTableViewControllerSource (lstCAPOutputParameter);


			

			});
		}

		public void ResponceError(string Error)
		{
			InvokeOnMainThread (() => {
				ActivityIndicator.Hide ();
				Console.WriteLine ("Service Fail");
				
                UIAlertController okAlertController = UIAlertController.Create("WBidMax", Error, UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
            });
		}

		partial void funDismissViewAction (NSObject sender)
		{
			
			this.DismissViewController(true,null);

		}
        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();


            // Release any cached data, images, etc that aren't in use.
        }
    }
}


