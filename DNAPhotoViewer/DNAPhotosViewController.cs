namespace DevsDNA.DNAPhotoViewer
{
	using System;
	using CoreGraphics;
	using Foundation;
	using Interfaces;
	using ObjCRuntime;
	using UIKit;

	public class DNAPhotosViewController : UIViewController, IUIPageViewControllerDataSource, IUIPageViewControllerDelegate, IDNAPhotoViewControllerDelegate
	{
		string PhotosViewControllerDidNavigateToPhotoNotification = @"NYTPhotosViewControllerDidNavigateToPhotoNotification";
		string PhotosViewControllerWillDismissNotification = @"NYTPhotosViewControllerWillDismissNotification";
		string PhotosViewControllerDidDismissNotification = @"NYTPhotosViewControllerDidDismissNotification";

		nfloat PhotosViewControllerOverlayAnimationDuration = 0.2f;

		UIPageViewController PageViewController;
		DNAPhotoTransitionController TransitionController;

		DNAPhotosOverlayView OverlayView;

		NSNotificationCenter _notificationCenter;

		bool ShouldHandleLongPress { get; set;}
		bool OverlayWasHiddenBeforeTransition { get; set;}

		UIPanGestureRecognizer PanGestureRecognizer { get; set; }
		UITapGestureRecognizer SingleTapGestureRecognizer { get; set; }

		IDNAPhotoViewerDataSource DataSource { get; set;}

		NSPhoto InitialPhoto { get; set;}


		public IDNAPhotosViewControllerDelegate Delegate { get; set; }


		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (PageViewController != null)
			{
				PageViewController.DataSource = null;
				PageViewController.Delegate = null;
			}
		}

		public override void Copy(NSObject sender)
		{
		}

		public override bool CanBecomeFirstResponder
		{
			get
			{
				return true;
			}
		}

		public override bool CanPerform(Selector action, NSObject withSender)
		{
			if (ShouldHandleLongPress && action == new Selector("copy:") && CurrentlyDisplayedPhoto.Image != null)
				return true;

			return false;
		}


		public DNAPhotosViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
		{
			Initialize(null, null, null);
		}

		public DNAPhotosViewController(NSCoder coder) : base(coder)
		{
			Initialize(null, null, null);
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			ConfigurePageViewController();

			View.TintColor = UIColor.White;
			View.BackgroundColor = UIColor.Black;
			PageViewController.View.BackgroundColor = UIColor.Clear;

			PageViewController.View.AddGestureRecognizer(PanGestureRecognizer);
			PageViewController.View.AddGestureRecognizer(SingleTapGestureRecognizer);

			AddChildViewController(PageViewController);
			View.AddSubview(PageViewController.View);
			PageViewController.DidMoveToParentViewController(this);

			AddOverlayView();

			TransitionController.StartingView = ReferenceViewForCurrentPhoto;

			UIView endingView = null;
			if (CurrentlyDisplayedPhoto.Image != null || CurrentlyDisplayedPhoto.PlaceholderImage != null)
				endingView = CurrentPhotoViewController.ScalingImageView.ImageView;

			TransitionController.EndingView = endingView;
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			if (!OverlayWasHiddenBeforeTransition)
				SetOverlayViewHidden(false, true);
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();

			PageViewController.View.Frame = View.Bounds;
			OverlayView.Frame = View.Bounds;
		}

		public override bool PrefersStatusBarHidden()
		{
			return true;
		}

		public override UIStatusBarAnimation PreferredStatusBarUpdateAnimation
		{
			get
			{
				return UIStatusBarAnimation.Fade;
			}
		}

		public override void DismissViewController(bool animated, Action completionHandler)
		{
			DismissViewController(animated, false, completionHandler);
		}


		public DNAPhotosViewController(IDNAPhotoViewerDataSource dataSource, NSPhoto initialPhoto = null, IDNAPhotosViewControllerDelegate _delegate = null)
		{
			if(initialPhoto == null)
				initialPhoto = dataSource.PhotoAtIndex(0);
			
			Initialize(dataSource, initialPhoto, _delegate);
		}

		public DNAPhotosViewController(IDNAPhotoViewerDataSource dataSource, int initialPhotoIndex, IDNAPhotosViewControllerDelegate _delegate = null)
		{
			var initialPhoto = dataSource.PhotoAtIndex(initialPhotoIndex);

			Initialize(dataSource, initialPhoto, _delegate);
		}

		void Initialize(IDNAPhotoViewerDataSource dataSource, NSPhoto initialPhoto, IDNAPhotosViewControllerDelegate _delegate = null)
		{
			DataSource = dataSource;
			Delegate = _delegate;
			InitialPhoto = initialPhoto;

			PanGestureRecognizer = new UIPanGestureRecognizer(this, new Selector("didPanWithGestureRecognizer:"));
			SingleTapGestureRecognizer = new UITapGestureRecognizer(this, new Selector("didSingleTapWithGestureRecognizer:"));

			TransitionController = new DNAPhotoTransitionController();
			ModalPresentationStyle = UIModalPresentationStyle.Custom;
			TransitioningDelegate = TransitionController;
			ModalPresentationCapturesStatusBarAppearance = true;

			OverlayView = SetUpOverlayView();

			_notificationCenter = new NSNotificationCenter();
			PageViewController = new UIPageViewController(UIPageViewControllerTransitionStyle.Scroll, UIPageViewControllerNavigationOrientation.Horizontal, new NSDictionary());
			PageViewController.Delegate = this;
			PageViewController.DataSource = this;

		}

		DNAPhotosOverlayView SetUpOverlayView()
		{
			var photosOverlayView = new DNAPhotosOverlayView(CGRect.Empty);

			return photosOverlayView;
		}

		void ConfigurePageViewController()
		{
			DNAPhotoViewController initialPhotoViewController;

			if (InitialPhoto != null && DataSource.IndexOfPhoto(InitialPhoto) != -1)
				initialPhotoViewController = NewPhotoViewController(InitialPhoto);
			else
				initialPhotoViewController = NewPhotoViewController(DataSource.PhotoAtIndex(0));

			SetCurrentlyDisplayedViewController(initialPhotoViewController, false);
		}

		void AddOverlayView()
		{
			var textColor = View.TintColor != null ? View.TintColor : UIColor.White;
			OverlayView.TitleTextAttributes = new UIStringAttributes { ForegroundColor = textColor };

			UpdateOverlayInformation();
			View.AddSubview(OverlayView);

			SetOverlayViewHidden(true, false);
		}

		void UpdateOverlayInformation()
		{

			if (((NSObject)Delegate).RespondsToSelector(new Selector("photosViewController:headerViewHeightForPhoto:")))
				OverlayView.HeaderViewHeight = Delegate.HeaderViewHeightForPhoto(this, CurrentlyDisplayedPhoto);

			if (((NSObject)Delegate).RespondsToSelector(new Selector("photosViewController:footerViewHeightForPhoto:")))
				OverlayView.FooterViewHeight = Delegate.FooterViewHeightForPhoto(this, CurrentlyDisplayedPhoto);
			

			if (((NSObject)Delegate).RespondsToSelector(new Selector("photosViewController:headerViewForPhoto:")))
				OverlayView.HeaderView = Delegate.HeaderViewForPhoto(this, CurrentlyDisplayedPhoto);

			if (((NSObject)Delegate).RespondsToSelector(new Selector("photosViewController:footerViewForPhoto:")))
				OverlayView.FooterView = Delegate.FooterViewForPhoto(this, CurrentlyDisplayedPhoto);

		}

		void DisplayActivityViewController(UIActivityViewController controller, bool animated)
		{
			PresentViewController(controller, animated, null);
		}

		public UIBarButtonItem LeftBarButtonItem
		{
			get
			{
				return OverlayView.LeftBarButtonItem;
			}
			set
			{
				OverlayView.LeftBarButtonItem = value;
			}
		}
		public UIBarButtonItem[] LeftBarButtonItems
		{
			get
			{
				return OverlayView.LeftBarButtonItems;
			}
			set
			{
				OverlayView.LeftBarButtonItems = value;
			}
		}

		public UIBarButtonItem RightBarButtonItem
		{
			get
			{
				return OverlayView.RightBarButtonItem;
			}
			set
			{
				OverlayView.RightBarButtonItem = value;
			}
		}
		public UIBarButtonItem[] RightBarButtonItems
		{
			get
			{
				return OverlayView.RightBarButtonItems;
			}
			set
			{
				OverlayView.RightBarButtonItems = value;
			}
		}

		void DisplayPhoto(NSPhoto photo, bool animated)
		{
			if (DataSource.IndexOfPhoto(photo) == -1)
				return;

			var photoViewController = NewPhotoViewController(photo);
			SetCurrentlyDisplayedViewController(photoViewController, animated);
			UpdateOverlayInformation();
		}

		void UpdatePhoto(int photoIndex)
		{
			var photo = DataSource.PhotoAtIndex(photoIndex);
			if (photo == null)
				return;

			UpdatePhoto(photo);
		}

		void UpdatePhoto(NSPhoto photo)
		{
			if (DataSource.IndexOfPhoto(photo) == -1)
				return;

			_notificationCenter.PostNotificationName(Constants.DNAPhotoViewControllerPhotoImageUpdatedNotification, photo);

			if (CurrentlyDisplayedPhoto.Equals(photo))
				UpdateOverlayInformation();
		}

		void ReloadPhotos(bool animated)
		{
			NSPhoto newCurrentPhoto = null;

			if (DataSource.IndexOfPhoto(CurrentlyDisplayedPhoto) != -1)
				newCurrentPhoto = CurrentlyDisplayedPhoto;
			else
				newCurrentPhoto = DataSource.PhotoAtIndex(0);

			DisplayPhoto(newCurrentPhoto, animated);

			if (OverlayView.Hidden)
				SetOverlayViewHidden(false, animated);
		}


		[Export("didSingleTapWithGestureRecognizer:")]
		public void DidSingleTapWithGestureRecognizer(UITapGestureRecognizer recognizer)
		{
			SetOverlayViewHidden(!OverlayView.Hidden, true);
		}

		[Export("didPanWithGestureRecognizer:")]
		public void DidPanWithGestureRecognizer(UIPanGestureRecognizer recognizer)
		{
			if (recognizer.State == UIGestureRecognizerState.Began)
			{
				TransitionController.ForcesNonInteractiveDismissal = false;
				DismissViewController(true, true, null);
			}
			else
			{
				TransitionController.ForcesNonInteractiveDismissal = true;
				TransitionController.DidPanWithPanGestureRecognizer(recognizer, PageViewController.View, BoundsCenterPoint);
			}
		}


		public void DismissViewController(bool animated, bool isUserInitiated, Action completionHandler)
		{
			if (PresentedViewController != null)
			{
				base.DismissViewController(animated, completionHandler);
				return;
			}

			UIView startingView = null;
			if (CurrentlyDisplayedPhoto.Image != null || CurrentlyDisplayedPhoto.PlaceholderImage != null || CurrentlyDisplayedPhoto.ImageData != null)
				startingView = CurrentPhotoViewController.ScalingImageView.ImageView;

			TransitionController.StartingView = startingView;
			TransitionController.EndingView = ReferenceViewForCurrentPhoto;

			OverlayWasHiddenBeforeTransition = OverlayView.Hidden;
			SetOverlayViewHidden(true, animated);

			bool shouldSendDelegateMessages = isUserInitiated;

			if (shouldSendDelegateMessages && ((NSObject)Delegate).RespondsToSelector(new Selector("photosViewControllerWillDismiss:")))
			   Delegate.PhotosViewControllerWillDismiss(this);

			NSNotificationCenter.DefaultCenter.PostNotificationName(PhotosViewControllerWillDismissNotification, this);

			base.DismissViewController(animated, () =>
			{
				bool isStillOnscreen = View.Window != null;

				if (isStillOnscreen && !OverlayWasHiddenBeforeTransition)
					SetOverlayViewHidden(false, true);

				if (!isStillOnscreen)
				{
					if (shouldSendDelegateMessages && ((NSObject)Delegate).RespondsToSelector(new Selector("photosViewControllerDidDismiss:")))
						Delegate.PhotosViewControllerDidDismiss(this);

					NSNotificationCenter.DefaultCenter.PostNotificationName(PhotosViewControllerDidDismissNotification, this);
				}

				if(completionHandler != null)
					completionHandler();

			});

		}

		void SetCurrentlyDisplayedViewController(UIViewController viewController, bool animated)
		{
			if (viewController == null)
				return;

			if (((IDNAPhotoContainer)viewController).Photo.Equals(CurrentlyDisplayedPhoto))
				animated = false;

			var currentIdx = DataSource.IndexOfPhoto(CurrentlyDisplayedPhoto);
			var newIdx = DataSource.IndexOfPhoto(((IDNAPhotoContainer)viewController).Photo);
			var direction = (newIdx < currentIdx) ? UIPageViewControllerNavigationDirection.Reverse : UIPageViewControllerNavigationDirection.Forward;

			PageViewController.SetViewControllers(new[] { viewController }, direction, animated, null);
		}

		void SetOverlayViewHidden(bool hidden, bool animated)
		{
			if (hidden == OverlayView.Hidden)
				return;

			if (animated)
			{
				OverlayView.Hidden = false;
				OverlayView.Alpha = hidden ? 1.0f : 0.0f;

				UIView.Animate(PhotosViewControllerOverlayAnimationDuration, 0.0f,
							   UIViewAnimationOptions.CurveEaseInOut | UIViewAnimationOptions.AllowAnimatedContent | UIViewAnimationOptions.AllowUserInteraction,
							   () =>
				{
					OverlayView.Alpha = hidden ? 0.0f : 1.0f;
				}, () =>
				{
					OverlayView.Alpha = 1.0f;
					OverlayView.Hidden = hidden;
				});

			}
			else
			{
				OverlayView.Hidden = hidden;
			}
		}

		DNAPhotoViewController NewPhotoViewController(NSPhoto photo)
		{
			if (photo != null)
			{
				UIView loadingView = null;

				if (((NSObject)Delegate).RespondsToSelector(new Selector("photosViewController:loadingViewForPhoto:")))
					loadingView = Delegate.LoadingViewForPhoto(this, photo);

				var photoViewController = new DNAPhotoViewController(photo, loadingView, _notificationCenter);
				photoViewController.Delegate = this;

				SingleTapGestureRecognizer.RequireGestureRecognizerToFail(photoViewController.DoubleTapGestureRecognizer);

				if (((NSObject)Delegate).RespondsToSelector(new Selector("photosViewController:maximumZoomScaleForPhoto:")))
				{
					var maximumZoomScale = Delegate.MaximumZoomScaleForPhoto(this, photo);
					photoViewController.ScalingImageView.MaximumZoomScale = maximumZoomScale;
				}
				return photoViewController;
					
			}
			return null;
		}

		void DidNavigateToPhoto(NSPhoto photo)
		{
			if (((NSObject)Delegate).RespondsToSelector(new Selector("photosViewController:didNavigateToPhoto:atIndex:")))
				Delegate.DidNavigateToPhotoAtIndex(this, photo, DataSource.IndexOfPhoto(photo));

			NSNotificationCenter.DefaultCenter.PostNotificationName(PhotosViewControllerDidNavigateToPhotoNotification, this);
		}

		NSPhoto CurrentlyDisplayedPhoto
		{
			get
			{
				return CurrentPhotoViewController?.Photo;
			}
		}

		DNAPhotoViewController CurrentPhotoViewController
		{
			get
			{
				return (PageViewController.ViewControllers.Length > 0) ? (DNAPhotoViewController)PageViewController.ViewControllers[0] : null;
			}
		}

		UIView ReferenceViewForCurrentPhoto
		{
			get
			{
				if (((NSObject)Delegate).RespondsToSelector(new Selector("photosViewController:referenceViewForPhoto:")))
					return Delegate.ReferenceViewForPhoto(this, CurrentlyDisplayedPhoto);
					
				return null;
			}
		}

		CGPoint BoundsCenterPoint
		{ 
			get
			{
				return new CGPoint(View.Bounds.GetMidX(), View.Bounds.GetMidY());
			}
		}

		public void DidLongPressWithGestureRecognizer(DNAPhotoViewController photoViewController, UILongPressGestureRecognizer longPressGestureRecognizer)
		{
			ShouldHandleLongPress = false;

			bool clientDidHandle = false;
			if (((NSObject)Delegate).RespondsToSelector(new Selector("photosViewController:handleLongPressForPhoto:")))
				clientDidHandle = Delegate.HandleLongPressForPhoto(this, photoViewController.Photo, longPressGestureRecognizer);

			ShouldHandleLongPress = !clientDidHandle;

			if (ShouldHandleLongPress)
			{
				var menuController = UIMenuController.SharedMenuController;
				var targetRect = CGRect.Empty;
				targetRect.Location = longPressGestureRecognizer.LocationInView(longPressGestureRecognizer.View);
				menuController.SetTargetRect(targetRect, longPressGestureRecognizer.View);
				menuController.SetMenuVisible(true, true);
			}
		}

		public UIViewController GetPreviousViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			var photoIndex = DataSource.IndexOfPhoto(((IDNAPhotoContainer)referenceViewController).Photo);
			if (photoIndex == 0 || photoIndex == -1)
				return null;

			return NewPhotoViewController(DataSource.PhotoAtIndex(photoIndex - 1));
		}

		public UIViewController GetNextViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			var photoIndex = DataSource.IndexOfPhoto(((IDNAPhotoContainer)referenceViewController).Photo);
			if (photoIndex == -1)
				return null;

			return NewPhotoViewController(DataSource.PhotoAtIndex(photoIndex + 1));
		}

		[Export("pageViewController:didFinishAnimating:previousViewControllers:transitionCompleted:")]
		public void DidFinishAnimating(UIPageViewController pageViewController, bool finished, UIViewController[] previousViewControllers, bool completed)
		{
			if (completed)
			{
				UpdateOverlayInformation();

				var photoViewController = pageViewController.ViewControllers[0];
				DidNavigateToPhoto(((IDNAPhotoContainer)photoViewController).Photo);
			}
		}
	}
}
