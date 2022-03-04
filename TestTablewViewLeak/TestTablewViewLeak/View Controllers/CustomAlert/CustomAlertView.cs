using System;
using Foundation;
using UIKit;

namespace WBid.WBidiPad.iOS
{
    public partial class CustomAlertView : UIViewController
    {

        public NSMutableAttributedString CustomAttributedMessage { get; set; }
        public string AlertType = string.Empty;
        public RetrieveAwardViewController objRetriveAwards;
        public SubmitBidViewController objSubmitBid;
        public downloadBidDataViewController ObjDownload;
        public queryViewController objQueryView;
       
        public CustomAlertView() : base("CustomAlertView", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            if (AlertType == "InvalidCredential")
            {
                SetInvalidCredentialAlertMessage();
                navigationTitle.Title = "Invalid Credential";

            }

        }
        public override void ViewWillDisappear(bool animated)
        {
            DismissView();
        }
       
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        partial void btnOkTapped(NSObject sender)
        {
            DismissView();
        }

        private void DismissView()
        {
            this.DismissViewController(true, null);
            if (objRetriveAwards != null)
            {
                objRetriveAwards.DismissView();
            }
            else if (ObjDownload != null)
            {
                ObjDownload.DismissCurrentView();
            }
            else if (objQueryView != null)
            {
                objQueryView.dismissView();
            }
        }

        public void SetInvalidCredentialAlertMessage()
        {
            string showmessage1 = "\n\n\n\nTo LOGIN, you need to use your ";
            string showmessage2 = " password! \n\nMost likely, your ";
            string showmessage3 = " password has expired. \n\nBTW, it is possible your password to LOGIN on ";
            string showmessage4 = " is valid and your ";
            string showmessage5 = " password is expired. \n\n\nTo verify your ";
            string showmessage6 = " password is still good, open a browser and try to login to ";
            string showmessage7 = " (NOT swacrew.com).";
            string Boldsletter1 = "SwaLife"; //Defines the bold field
            string Boldsletter2 = "swacrew.com"; //Defines the bold field
            string Boldsletter3 = "swalife.com"; //Defines the bold field

            
            nfloat size = 18.0f;
            var msg = new NSMutableAttributedString(
    showmessage1,
    font: UIFont.SystemFontOfSize(size)
   
);
            var msg2 = new NSMutableAttributedString(
    showmessage2,
    font: UIFont.SystemFontOfSize(size)
    
);
            var msg3 = new NSMutableAttributedString(
    showmessage3,
    font: UIFont.SystemFontOfSize(size)
    
);
            var msg4 = new NSMutableAttributedString(
    showmessage4,
    font: UIFont.SystemFontOfSize(size)

);
            var msg5 = new NSMutableAttributedString(
    showmessage5,
    font: UIFont.SystemFontOfSize(size)

);
            var msg6 = new NSMutableAttributedString(
    showmessage6,
    font: UIFont.SystemFontOfSize(size)

);
            var msg7 = new NSMutableAttributedString(
    showmessage7,
    font: UIFont.SystemFontOfSize(size)

);
            var swaBold = (new NSAttributedString(
    Boldsletter1,
 font: UIFont.BoldSystemFontOfSize(size)

));
            var swacrew = (new NSAttributedString(
   Boldsletter2,
 font: UIFont.BoldSystemFontOfSize(size)

));
            var swalife = (new NSAttributedString(
   Boldsletter3,
 font: UIFont.BoldSystemFontOfSize(size)
    
));
            // CustomAttributedMessage.Append(msg);
            msg.Append(swaBold);
            msg.Append(msg2);
            msg.Append(swaBold);
            msg.Append(msg3);
            msg.Append(swacrew);
            msg.Append(msg4);
            msg.Append(swaBold);
            msg.Append(msg5);
            msg.Append(swaBold);
            msg.Append(msg6);
            msg.Append(swalife);
            msg.Append(msg7);
            lblMessageText.AttributedText = msg;

        }

    }
}

