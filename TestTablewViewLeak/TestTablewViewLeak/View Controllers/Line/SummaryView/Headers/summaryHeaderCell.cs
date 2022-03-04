using System; using CoreGraphics; using Foundation; using UIKit; using CoreGraphics; using WBid.WBidiPad.Model; using WBid.WBidiPad.Core;  namespace WBid.WBidiPad.iOS { 	public partial class summaryHeaderCell : UITableViewCell 	{ 		public static readonly UINib Nib = UINib.FromName ("summaryHeaderCell", NSBundle.MainBundle); 		public static readonly NSString Key = new NSString ("summaryHeaderCell");  		public summaryHeaderCell (IntPtr handle) : base (handle) 		{ 			loadCell (this); 		}  		public void loadCell( summaryHeaderCell cell) 		{ 			if (this != null) { 				const double M_PI = 3.14159265358979323846; 				const float k90DegreesCounterClockwiseAngle = (float) (90 * M_PI / 180.0); 				CGRect frame = this.Frame;  				CGAffineTransform transform = CGAffineTransform.MakeRotation (k90DegreesCounterClockwiseAngle); 				cell.ContentView.Transform = transform; 				cell.ContentView.Frame = frame; 				cell.Layer.BorderWidth = 1; 				cell.Layer.BorderColor = UIColor.FromRGB (158, 179, 131).CGColor; 				cell.addTapGesture (); 			} 		} 		public static summaryHeaderCell Create () 		{ 			return (summaryHeaderCell)Nib.Instantiate (null, null) [0]; 		}  		public void bindData(ColumnDefinition obj ,int row) 		{ 			if (obj.Id == 1) { 				this.lblTitle.Hidden = true; 				this.imgIcon.Hidden = false; 				this.imgIcon.Image = UIImage.FromBundle ("lockIcon.png");  			}  else if (obj.Id == 2) { 				this.lblTitle.Hidden = true; 				this.imgIcon.Hidden = false; 				this.imgIcon.Image = UIImage.FromBundle ("deleteIconBold.png");   			}  else if (obj.Id == 3) { 				this.lblTitle.Hidden = true; 				this.imgIcon.Hidden = false; 				this.imgIcon.Image = UIImage.FromBundle ("overlayIconBold.png");  			}  else { 				this.lblTitle.Hidden = false; 				this.imgIcon.Hidden = true; 				string disp = obj.DisplayName; 				if (disp == "StartDOW") 					disp = "SDOW"; 				if (disp == "EDomPush") 					disp = "EDOM"; 				if (disp == "FA Posn") 					disp = "FaPos"; 				if (disp == "LDomArr") 					disp = "LDOM"; 				if (disp == "AvgLatestDomArrivalTime") 					disp = "ALDA"; 				if (disp == "AvgEarliestDomPush") 					disp = "AEDP"; 				if (disp == "MyValue") 					disp = "MyValue"; 				if (disp == "8Max") 					disp = "8Max"; 				this.lblTitle.Font = UIFont.FromName("HelveticaNeue-Bold", 11f); 				this.lblTitle.Text = disp; 			}  			if (obj.Id == 0 | obj.Id == 1 | obj.Id == 2 | obj.Id == 3 | obj.Id == 4) { 				this.BackgroundColor = UIColor.White; 			}  else { 				this.BackgroundColor = UIColor.FromRGB (207, 226, 183); 				this.lblTitle.TextColor = UIColor.DarkGray; 			}  			if (obj.Id > 4 && obj.Id == CommonClass.columnID) { 				this.BackgroundColor = UIColor.Yellow; 				if (CommonClass.columnAscend) { 					this.lblOrdUp.Text = "∧"; 					this.lblOrdDown.Text = ""; 					this.Tag = 2; 				}  else { 					this.lblOrdUp.Text = ""; 					this.lblOrdDown.Text = "∨"; 					this.Tag = 1; 				} 			}  else { 				this.lblOrdUp.Text = ""; 				this.lblOrdDown.Text = ""; 				this.Tag = 0; 			} 		}  		public void addTapGesture () 		{ 			// single tap for ascending/decending order sorting per column. 			UITapGestureRecognizer singleTap = new UITapGestureRecognizer (handleSingleTap); 			singleTap.NumberOfTapsRequired = 1; 			this.AddGestureRecognizer (singleTap);  			// double tap for showing more column items in popover menu. 			UITapGestureRecognizer doubleTap = new UITapGestureRecognizer (handleDoubleTap); 			doubleTap.NumberOfTapsRequired = 2; 			this.AddGestureRecognizer (doubleTap);  			singleTap.RequireGestureRecognizerToFail (doubleTap);  		} 		public void handleSingleTap(UITapGestureRecognizer gest) 		{ 			switch (this.Tag) { 			case 0: 				this.Tag = 1; 				break; 			case 1: 				this.Tag = 2; 				break; 			case 2: 				this.Tag = 1; 				break; 			}  			NSNotificationCenter.DefaultCenter.PostNotificationName ("SummaryColumnSort", gest.View); 		} 		public void handleDoubleTap(UITapGestureRecognizer gest) 		{ 			UITableViewCell cell = (UITableViewCell)gest.View; 			NSNotificationCenter.DefaultCenter.PostNotificationName ("ColumnPopover", cell); 			NSNotificationCenter.DefaultCenter.PostNotificationName ("CalPopHide", null); 		}  	} }   