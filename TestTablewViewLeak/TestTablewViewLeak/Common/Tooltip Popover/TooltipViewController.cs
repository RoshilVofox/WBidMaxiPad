
using System;
using CoreGraphics;

using Foundation;
using UIKit;

namespace WBid.WBidiPad.iOS
{
	public partial class TooltipViewController : UIViewController
	{
public string tipMessage;
public bool IsNeedToAlignLeft;
public TooltipViewController() : base ("TooltipViewController", null)
		{
}

public override void DidReceiveMemoryWarning()
{
	// Releases the view if it doesn't have a superview.
	base.DidReceiveMemoryWarning();

	// Release any cached data, images, etc that aren't in use.
}

public override void ViewDidLoad()
{
	base.ViewDidLoad();

	// Perform any additional setup after loading the view, typically from a nib.
	if (IsNeedToAlignLeft)
		lblMessage.TextAlignment = UITextAlignment.Left;
	else
		lblMessage.TextAlignment = UITextAlignment.Center;
	lblMessage.Text = tipMessage;

}
	}
}

