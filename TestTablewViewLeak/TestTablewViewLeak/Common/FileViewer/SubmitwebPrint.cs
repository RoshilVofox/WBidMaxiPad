using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.iOS.Utility;
using System.Text;
using System.IO;


namespace WBid.WBidiPad.iOS
{
	public partial class SubmitwebPrint : UIViewController
	{
public SubmitwebPrint () : base ("SubmitwebPrint", null)
		{
		}

		public enum FromView
		{
			BidEditorFA = 0,
			BidEditorCPORFO,
			SubmitBid
	  }

		public FromView isFromView;
		public Boolean isFromSubmitBid;
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		string fileName;
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			searchBar.SearchButtonClicked += (sender, e) => {
				highlightAllOccurancesOfString (searchBar.Text);
				searchBar.ResignFirstResponder();
			};
			searchBar.CancelButtonClicked += (object sender, EventArgs e) =>{
				searchBar.Hidden = true;
				searchBar.ResignFirstResponder();
				removeAllHighLights();
			};
		}
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			//txtCellNumber.Dispose ();

			foreach (UIView view in this.View.Subviews) {

				DisposeClass.DisposeEx(view);
			}
			this.View.Dispose ();


		}
		partial void btnBackTapped (UIKit.UIBarButtonItem sender)
		{
			
			this.DismissViewController(true, null);

			if (isFromView == FromView.SubmitBid)
			{
				CommonClass.ObjSubmitView.DismissViewController(true, null);
			}
			else if (isFromView == FromView.BidEditorFA)
			{
				CommonClass.ObjBidEditorFA.DismissViewController(true, null);
                PerformSelector(new ObjCRuntime.Selector("dismissBidEditorFA"), null, 1.00);
			}
			else if (isFromView == FromView.BidEditorCPORFO)
			{
                
				CommonClass.ObjBidEditorPilot.DismissViewController(true, null);
				PerformSelector(new ObjCRuntime.Selector("dismissBidEditor"), null, 1.00);
			}



		}
[Export("dismissBidEditor")]
		public void dismissBidEditor()
		{
			CommonClass.ObjBidEditorPilot.dismissView();
		}

		[Export("dismissBidEditorFA")]
		public void dismissBidEditorFA()
		{
			CommonClass.ObjBidEditorFA.dismissView();
		}

		public void loadFileFromUrl(string strUrl)
		{
			try{
                var preText = "<html><body style='font-family:Courier;color:#000000;'> ";
				var postText = "</body></html>";
                string text = System.IO.File.ReadAllText(WBidHelper.GetAppDataPath() + "/" + strUrl, Encoding.Default);
                text = text.Replace("\n", "<br/>").Replace(" ", "&nbsp;").Replace("\t","&nbsp;&nbsp;");
				this.webView.LoadHtmlString (preText + text + postText, null);
			}catch{
				this.webView.LoadHtmlString ("Oops!! File does not exist.", null);	
			}
		}

        public void LoadPDFdocument(string filename)
        {
			fileName = filename;
            webView.LoadRequest(new NSUrlRequest(new NSUrl(WBidHelper.GetAppDataPath() + "/" + filename, false)));
         
        }
		public void loadHTMTLFromUrl(string url)
		{
			webView.LoadRequest(new NSUrlRequest(new NSUrl(url)));
		}
		partial void btnReloadTapped (UIKit.UIBarButtonItem sender)
		{
			webView.LoadRequest(new NSUrlRequest(new NSUrl(WBidHelper.GetAppDataPath() + "/" + fileName, false)));
			//this.webView.Reload();
		}
		public int highlightAllOccurancesOfString(string searchStr)
		{
			//string fileName = "Common/FileViewer/search.js"; // remember case-sensitive
			//string localFileUrl = Path.Combine (NSBundle.MainBundle.BundlePath, "search.js");
			//string jsCode = System.IO.File.ReadAllText(localFileUrl);
			this.webView.EvaluateJavascript ("// We're using a global variable to store the number of occurrences\nvar MyApp_SearchResultCount = 0;\n\n// helper function, recursively searches in elements and their child nodes\nfunction MyApp_HighlightAllOccurencesOfStringForElement(element,keyword) {\n  if (element) {\n    if (element.nodeType == 3) {        // Text node\n      while (true) {\n        var value = element.nodeValue;  // Search for keyword in text node\n        var idx = value.toLowerCase().indexOf(keyword);\n\n        if (idx < 0) break;             // not found, abort\n\n        var span = document.createElement(\"span\");\n        var text = document.createTextNode(value.substr(idx,keyword.length));\n        span.appendChild(text);\n        span.setAttribute(\"class\",\"MyAppHighlight\");\n        span.style.backgroundColor=\"yellow\";\n        span.style.color=\"black\";\n        text = document.createTextNode(value.substr(idx+keyword.length));\n        element.deleteData(idx, value.length - idx);\n        var next = element.nextSibling;\n        element.parentNode.insertBefore(span, next);\n        element.parentNode.insertBefore(text, next);\n        element = text;\n        MyApp_SearchResultCount++;\t// update the counter\n      }\n    } else if (element.nodeType == 1) { // Element node\n      if (element.style.display != \"none\" && element.nodeName.toLowerCase() != 'select') {\n        for (var i=element.childNodes.length-1; i>=0; i--) {\n          MyApp_HighlightAllOccurencesOfStringForElement(element.childNodes[i],keyword);\n        }\n      }\n    }\n  }\n}\n\n// the main entry point to start the search\nfunction MyApp_HighlightAllOccurencesOfString(keyword) {\n  MyApp_RemoveAllHighlights();\n  MyApp_HighlightAllOccurencesOfStringForElement(document.body, keyword.toLowerCase());\n}\n\n// helper function, recursively removes the highlights in elements and their childs\nfunction MyApp_RemoveAllHighlightsForElement(element) {\n  if (element) {\n    if (element.nodeType == 1) {\n      if (element.getAttribute(\"class\") == \"MyAppHighlight\") {\n        var text = element.removeChild(element.firstChild);\n        element.parentNode.insertBefore(text,element);\n        element.parentNode.removeChild(element);\n        return true;\n      } else {\n        var normalize = false;\n        for (var i=element.childNodes.length-1; i>=0; i--) {\n          if (MyApp_RemoveAllHighlightsForElement(element.childNodes[i])) {\n            normalize = true;\n          }\n        }\n        if (normalize) {\n          element.normalize();\n        }\n      }\n    }\n  }\n  return false;\n}\n\n// the main entry point to remove the highlights\nfunction MyApp_RemoveAllHighlights() {\n  MyApp_SearchResultCount = 0;\n  MyApp_RemoveAllHighlightsForElement(document.body);\n}");

			string startSearch = "MyApp_HighlightAllOccurencesOfString('" + searchStr + "')";
			this.webView.EvaluateJavascript (startSearch);
		
			string searchCount = this.webView.EvaluateJavascript ("MyApp_SearchResultCount");
			return System.Convert.ToInt32(searchCount);;

		}
		public void removeAllHighLights()
		{
			this.webView.EvaluateJavascript("MyApp_RemoveAllHighlights()");
		}

		partial void btnSearchTapped (UIKit.UIBarButtonItem sender)
		{
			searchBar.Hidden = false;
		}


	}
}

