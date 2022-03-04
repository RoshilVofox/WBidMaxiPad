using System;
using UIKit;
using System.Collections.Generic;
using System.Linq;

namespace WBid.WBidiPad.iOS.Utility
{
	public class DisposeClass
	{
		public DisposeClass ()
		{
		}

		public static void DisposeEx(UIView view) {
			const bool enableLogging = false;
			try {
				if (view == null)
					return;

				var viewDescription = string.Empty;

				if (enableLogging) {
					viewDescription = view.Description;
					Console.WriteLine("Destroying " + viewDescription);
				}

				var disposeView = true;
				var disconnectFromSuperView = true;
				var disposeSubviews = true;
				var removeGestureRecognizers = false; // WARNING: enable at your own risk, may causes crashes
				var removeConstraints = true;
				var removeLayerAnimations = true;
				var associatedViewsToDispose = new List<UIView>();
				var otherDisposables = new List<IDisposable>();

				if (view is UIActivityIndicatorView) {
					var aiv = (UIActivityIndicatorView)view;
					if (aiv.IsAnimating) {
						aiv.StopAnimating();
					}
				} else if (view is UITableView) {
					var tableView = (UITableView)view;

					if (tableView.DataSource != null) {
						otherDisposables.Add(tableView.DataSource);
					}
					if (tableView.BackgroundView != null) {
						associatedViewsToDispose.Add(tableView.BackgroundView);
					}

					tableView.Source = null;
					tableView.Delegate = null;
					tableView.DataSource = null;
					tableView.WeakDelegate = null;
					tableView.WeakDataSource = null;
					associatedViewsToDispose.AddRange(tableView.VisibleCells ?? new UITableViewCell[0]);
				} else if (view is UITableViewCell) {
					var tableViewCell = (UITableViewCell)view;
					disposeView = false;
					disconnectFromSuperView = false;
					if (tableViewCell.ImageView != null) {
						associatedViewsToDispose.Add(tableViewCell.ImageView);
					}

				} else if (view is UICollectionView) {
					var collectionView = (UICollectionView)view;
					disposeView = false; 
					if (collectionView.DataSource != null) {
						otherDisposables.Add(collectionView.DataSource);
					}
					if (collectionView == null) {
						associatedViewsToDispose.Add(collectionView.BackgroundView);
					}
					//associatedViewsToDispose.AddRange(collectionView.VisibleCells ?? new UICollectionViewCell[0]);
					collectionView.Source = null;
					collectionView.Delegate = null;
					collectionView.DataSource = null;
					collectionView.WeakDelegate = null;
					collectionView.WeakDataSource = null;
				} else if (view is UICollectionViewCell) {
					var collectionViewCell = (UICollectionViewCell)view;
					disposeView = false;
					disconnectFromSuperView = false;
					if (collectionViewCell.BackgroundView != null) {
						associatedViewsToDispose.Add(collectionViewCell.BackgroundView);
					}
				} else if (view is UIWebView) {
					var webView = (UIWebView)view;
					if (webView.IsLoading)
						webView.StopLoading();
					webView.LoadHtmlString(string.Empty, null); // clear display
					webView.Delegate = null;
					webView.WeakDelegate = null;
				} else if (view is UIImageView) {
					var imageView = (UIImageView)view;
					if (imageView.Image != null) {
						otherDisposables.Add(imageView.Image);
						imageView.Image = null;
					}
				} else if (view is UIScrollView) {
					var scroll=(UIScrollView)view;
					scroll=null;
					//scrollView.UnsetZoomableContentView();
				}
				else if (view is UISegmentedControl) {
					var Seg=(UISegmentedControl)view;
					Seg=null;
					//scrollView.UnsetZoomableContentView();
				}
				else if (view is UISwitch) {
					var Switch=(UISwitch)view;
					Switch=null;
					//scrollView.UnsetZoomableContentView();
				}
				else if (view is UIButton) {
					var button=(UIButton)view;
					button=null;

					//scrollView.UnsetZoomableContentView();
				}
				else if (view is UILabel) {
					var label=(UILabel)view;
					label=null;
					//scrollView.UnsetZoomableContentView();
				}
				var gestures = view.GestureRecognizers;
				if (removeGestureRecognizers && gestures != null) {
					foreach(var gr in gestures) {
						view.RemoveGestureRecognizer(gr);
						gr.Dispose();
					}
				}

				if (removeLayerAnimations && view.Layer != null) {
					view.Layer.RemoveAllAnimations();
				}

				if (disconnectFromSuperView && view.Superview != null) {
					view.RemoveFromSuperview();
				}

				var constraints = view.Constraints;
				if (constraints != null && constraints.Any() && constraints.All(c => c.Handle != IntPtr.Zero)) {
					view.RemoveConstraints(constraints);
					foreach(var constraint in constraints) {
						constraint.Dispose();
					}
				}

				foreach(var otherDisposable in otherDisposables) {
					otherDisposable.Dispose();
				}

				foreach(var otherView in associatedViewsToDispose) {
					DisposeEx(otherView);
				}

				var subViews = view.Subviews;
				if (disposeSubviews && subViews != null) {

					foreach(UIView v in subViews)
						DisposeEx(v);
				}                   

				if (view is ISpecialDisposable) {
					((ISpecialDisposable)view).SpecialDispose();
				} else if (disposeView) {
					if (view.Handle != IntPtr.Zero)
						view.Dispose();
				}

				if (enableLogging) {
					Console.WriteLine("Destroyed {0}", viewDescription);
				}

			} catch (Exception error) {
				Console.WriteLine(error);
			}
		}

		public void RemoveAndDisposeChildSubViews(UIView view) {
			if (view == null)
				return;
			if (view.Handle == IntPtr.Zero)
				return;
			if (view.Subviews == null)
				return;

			foreach(UIView subViews in view)
				RemoveFromSuperviewAndDispose(subViews);
		}

		public  void RemoveFromSuperviewAndDispose( UIView view) {
			view.RemoveFromSuperview();
			DisposeEx(view);
		}

		public  bool IsDisposedOrNull( UIView view) {
			if (view == null)
				return true;

			if (view.Handle == IntPtr.Zero)
				return true;;

			return false;
		}

		public interface ISpecialDisposable {
			void SpecialDispose();
		}


	}
}

