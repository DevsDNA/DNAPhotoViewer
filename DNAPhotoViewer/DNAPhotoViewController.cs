namespace DevsDNA.DNAPhotoViewer
{
	using System;
	using CoreGraphics;
	using Foundation;
	using ObjCRuntime;
	using Interfaces;
	using UIKit;

	public class DNAPhotoViewController : UIViewController, IDNAPhotoContainer, IUIScrollViewDelegate
	{
		string PhotoViewControllerPhotoImageUpdatedNotification = @"NYTPhotoViewControllerPhotoImageUpdatedNotification";

		NSNotificationCenter _notificationCenter;
		UILongPressGestureRecognizer _longPressGestureRecognizer;

		NSPhoto _photo;

		public DNAScalingImageView ScalingImageView { get; internal set; }

		public UIView LoadingView { get; internal set;}

		public UITapGestureRecognizer DoubleTapGestureRecognizer { get; internal set; }

		public IDNAPhotoViewControllerDelegate Delegate { get; set; }

		public NSPhoto Photo
		{
			get
			{
				return _photo;
			}
		}


		protected override void Dispose(bool disposing)
		{
			ScalingImageView.Delegate = null;
			_notificationCenter.RemoveObserver(this);
			
			base.Dispose(disposing);
		}


		public DNAPhotoViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
		{
			Initialize(null, null, null);
		}

		public DNAPhotoViewController(NSCoder coder) : base(coder)
		{
			Initialize(null, null, null);
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			_notificationCenter.AddObserver(this, new Selector("photoImageUpdatedWithNotification:"), new NSString(PhotoViewControllerPhotoImageUpdatedNotification), null);

			ScalingImageView.Frame = View.Bounds;
			View.AddSubview(ScalingImageView);

			if (LoadingView != null)
			{
				View.AddSubview(LoadingView);
				LoadingView.SizeToFit();
			}

			View.AddGestureRecognizer(DoubleTapGestureRecognizer);
			View.AddGestureRecognizer(_longPressGestureRecognizer);
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();

			ScalingImageView.Frame = View.Bounds;

			if (LoadingView != null)
			{
				LoadingView.SizeToFit();
				LoadingView.Center = new CGPoint(View.Bounds.GetMidX(), View.Bounds.GetMidY());
			}
		}


		public DNAPhotoViewController(NSPhoto photo, UIView loadingView, NSNotificationCenter notificationCenter) : base(null, null)
		{
			Initialize(photo, loadingView, notificationCenter);
		}

		void Initialize(NSPhoto photo, UIView loadingView, NSNotificationCenter notificationCenter)
		{
			_photo = photo;

			if (photo.ImageData != null)
			{
				ScalingImageView = new DNAScalingImageView(photo.ImageData, CGRect.Empty);
			}
			else
			{
				var photoImage = (photo.Image != null) ? photo.Image : photo.PlaceholderImage;
				ScalingImageView = new DNAScalingImageView(photoImage, CGRect.Empty);

				if (photoImage == null)
					SetUpLoadingView(loadingView);
			}

			ScalingImageView.Delegate = this;
			_notificationCenter = notificationCenter;

			SetUpGestureRecognizers();
		}

		void SetUpLoadingView(UIView loadingView)
		{
			LoadingView = loadingView;

			if (loadingView == null)
			{
				var activityIndicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.White);
				activityIndicator.StartAnimating();
				loadingView = activityIndicator;
			}
		}

		void PhotoImageUpdated(NSNotification notification)
		{
			var photo = notification.Object as NSPhoto;

			if (photo != null && photo.Equals(Photo))
				UpdateImage(photo.Image, photo.ImageData);

		}

		void UpdateImage(UIImage image, NSData imageData)
		{
			if (imageData != null)
				ScalingImageView.UpdateImageData(imageData);
			else
				ScalingImageView.UpdateImage(image);

			if (imageData != null || image != null)
				LoadingView.RemoveFromSuperview();
			else
				View.AddSubview(LoadingView);
		}

		void SetUpGestureRecognizers()
		{
			DoubleTapGestureRecognizer = new UITapGestureRecognizer(this, new Selector("didDoubleTapWithGestureRecognizer:"));
			DoubleTapGestureRecognizer.NumberOfTapsRequired = 2;

			_longPressGestureRecognizer = new UILongPressGestureRecognizer(this, new Selector("didLongPressWithGestureRecognizer:"));
		}

		[Export("didDoubleTapWithGestureRecognizer:")]
		public void DidDoubleTapWithGestureRecognizer(UITapGestureRecognizer recognizer)
		{
			var pointInView = recognizer.LocationInView(ScalingImageView.ImageView);

			var newZoomScale = ScalingImageView.MaximumZoomScale;

			if ((ScalingImageView.ZoomScale >= ScalingImageView.MinimumZoomScale) || (Math.Abs(ScalingImageView.ZoomScale - ScalingImageView.MaximumZoomScale) <= 0.01))
				newZoomScale = ScalingImageView.MinimumZoomScale;

			var scrollViewSize = ScalingImageView.Bounds.Size;

			var width = scrollViewSize.Width / newZoomScale;
			var height = scrollViewSize.Height / newZoomScale;
			var originX = pointInView.X - (width / 2.0);
			var originY = pointInView.Y - (height / 2.0);

			var rectToZoomTo = new CGRect(originX, originY, width, height);

			ScalingImageView.ZoomToRect(rectToZoomTo, true);
		}

		[Export("didLongPressWithGestureRecognizer:")]
		public void DidLongPressWithGestureRecognizer(UILongPressGestureRecognizer recognizer)
		{
			if (((NSObject)Delegate).RespondsToSelector(new Selector("photoViewController:didLongPressWithGestureRecognizer:")))
			{
				if (recognizer.State == UIGestureRecognizerState.Began)
					Delegate.DidLongPressWithGestureRecognizer(this, recognizer);
			}
		}

		[Export("viewForZoomingInScrollView:")]
		public UIView ViewForZoomingInScrollView(UIScrollView scrollView)
		{
			return ScalingImageView.ImageView;
		}

		[Export("scrollViewWillBeginZooming:withView:")]
		public void ZoomingStarted(UIScrollView scrollView, UIView view)
		{
			scrollView.PanGestureRecognizer.Enabled = true;
		}

		[Export("scrollViewDidEndZooming:withView:atScale:")]
		public void ZoomingEnded(UIScrollView scrollView, UIView withView, nfloat atScale)
		{
			if (scrollView.ZoomScale == scrollView.MinimumZoomScale)
				scrollView.PanGestureRecognizer.Enabled = false;
		}

	}
}
