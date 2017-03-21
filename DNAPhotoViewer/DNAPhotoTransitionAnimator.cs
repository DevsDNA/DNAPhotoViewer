namespace DevsDNA.DNAPhotoViewer
{
	using System;
	using CoreGraphics;
	using Foundation;
	using ObjCRuntime;
	using UIKit;

	public class DNAPhotoTransitionAnimator : NSObject, IUIViewControllerAnimatedTransitioning
	{
		static nfloat PhotoTransitionAnimatorDurationWithZooming = 0.5f;
		static nfloat PhotoTransitionAnimatorDurationWithoutZooming = 0.3f;
		static nfloat PhotoTransitionAnimatorBackgroundFadeDurationRatio = 4.0f / 9.0f;
		static nfloat PhotoTransitionAnimatorEndingViewFadeInDurationRatio = 0.1f;
		static nfloat PhotoTransitionAnimatorStartingViewFadeOutDurationRatio = 0.05f;
		static nfloat PhotoTransitionAnimatorSpringDamping = 0.9f;

		nfloat _animationDurationFadeRatio;
		nfloat _animationDurationEndingViewFadeInRatio;
		nfloat _animationDurationStartingViewFadeOutRatio;

		public DNAPhotoTransitionAnimator()
		{
			AnimationDurationWithZooming = PhotoTransitionAnimatorDurationWithZooming;
			AnimationDurationWithoutZooming = PhotoTransitionAnimatorDurationWithoutZooming;
			AnimationDurationFadeRatio = PhotoTransitionAnimatorBackgroundFadeDurationRatio;
			AnimationDurationEndingViewFadeInRatio = PhotoTransitionAnimatorEndingViewFadeInDurationRatio;
			AnimationDurationStartingViewFadeOutRatio = PhotoTransitionAnimatorStartingViewFadeOutDurationRatio;
			ZoomingAnimationSpringDamping = PhotoTransitionAnimatorSpringDamping;
		}

		public UIView StartingView { get; set; }

		public UIView EndingView { get; set; }

		public UIView StartingViewForAnimation { get; set; }

		public UIView EndingViewForAnimation { get; set; }

		public bool IsDismissing { get; set; }

		public nfloat AnimationDurationWithZooming { get; set; }

		public nfloat AnimationDurationWithoutZooming { get; set; }

		public nfloat ZoomingAnimationSpringDamping { get; set; }

		void SetUpTransitionContainerHierarchy(IUIViewControllerContextTransitioning transitionContext)
		{
			var fromView = transitionContext.GetViewFor(UITransitionContext.FromViewKey);
			var toView = transitionContext.GetViewFor(UITransitionContext.ToViewKey);

			var toViewController = transitionContext.GetViewControllerForKey(UITransitionContext.ToViewControllerKey);

			if(toView != null)
				toView.Frame = transitionContext.GetFinalFrameForViewController(toViewController);

			if (toView != null && !toView.IsDescendantOfView(transitionContext.ContainerView))
				transitionContext.ContainerView.AddSubview(toView);

			if (IsDismissing)
				transitionContext.ContainerView.BringSubviewToFront(fromView);
		}

		public nfloat AnimationDurationFadeRatio
		{
			get
			{
				return _animationDurationFadeRatio;
			}
			set
			{
				_animationDurationFadeRatio = (nfloat)Math.Min(value, 1.0);
			}
		}

		public nfloat AnimationDurationEndingViewFadeInRatio
		{
			get
			{
				return _animationDurationEndingViewFadeInRatio;
			}
			set
			{
				_animationDurationEndingViewFadeInRatio = (nfloat)Math.Min(value, 1.0);
			}
		}

		public nfloat AnimationDurationStartingViewFadeOutRatio
		{
			get
			{
				return _animationDurationStartingViewFadeOutRatio;
			}
			set
			{
				_animationDurationStartingViewFadeOutRatio = (nfloat)Math.Min(value, 1.0);
			}
		}


		void PerformFadeAnimation(IUIViewControllerContextTransitioning transitionContext)
		{
			var fromView = transitionContext.GetViewFor(UITransitionContext.FromViewKey);
			var toView = transitionContext.GetViewFor(UITransitionContext.ToViewKey);

			var viewToFade = toView;
			var beginningAlpha = 0.0f;
			var endingAlpha = 1.0f;

			if (IsDismissing)
			{
				viewToFade = fromView;
				beginningAlpha = 1.0f;
				endingAlpha = 0.0f;
			}

			viewToFade.Alpha = beginningAlpha;

			UIView.Animate(FadeDuration(transitionContext), () => { viewToFade.Alpha = endingAlpha; }, 
			               () => {
							   if (!ShouldPerformZoomingAnimation)
								   CompleteTransition(transitionContext);
			});
		}

		nfloat FadeDuration(IUIViewControllerContextTransitioning transitionContext)
		{
			if (ShouldPerformZoomingAnimation)
				return (nfloat) TransitionDuration(transitionContext) * AnimationDurationFadeRatio;

			return (nfloat) TransitionDuration(transitionContext);
		}

		void PerformZoomingAnimation(IUIViewControllerContextTransitioning transitionContext)
		{
			var containerView = transitionContext.ContainerView;

			var startingViewForAnimation = StartingViewForAnimation;
			if (startingViewForAnimation == null)
				startingViewForAnimation = NewAnimationViewFromView(StartingView);

			var endingViewForAnimation = EndingViewForAnimation;
			if (endingViewForAnimation == null)
				endingViewForAnimation = NewAnimationViewFromView(EndingView);

			var finalEndingViewTransform = EndingView.Transform;

			var endingViewInitialTransform = startingViewForAnimation.Frame.Height / endingViewForAnimation.Frame.Height;

			var translatedStartingViewCenter = CenterPointTranslatedTo(StartingView, containerView);

			startingViewForAnimation.Center = translatedStartingViewCenter;

			endingViewForAnimation.Transform = CGAffineTransform.Scale(endingViewForAnimation.Transform, endingViewInitialTransform, endingViewInitialTransform);
			endingViewForAnimation.Center = translatedStartingViewCenter;
			endingViewForAnimation.Alpha = 0.0f;

			transitionContext.ContainerView.AddSubview(startingViewForAnimation);
			transitionContext.ContainerView.AddSubview(endingViewForAnimation);

			EndingView.Alpha = 0.0f;
			StartingView.Alpha = 0.0f;

			var fadeInDuration = TransitionDuration(transitionContext) * AnimationDurationEndingViewFadeInRatio;
			var fadeOutDuration = TransitionDuration(transitionContext) * AnimationDurationStartingViewFadeOutRatio;

			UIView.Animate(fadeInDuration, 0.0f, 
			               UIViewAnimationOptions.AllowAnimatedContent | UIViewAnimationOptions.BeginFromCurrentState, 
			               () => {
							   endingViewForAnimation.Alpha = 1.0f;
			}, 
			               () => {
								UIView.Animate(fadeOutDuration, 0.0f, 
				                               UIViewAnimationOptions.AllowAnimatedContent | UIViewAnimationOptions.BeginFromCurrentState, 
				                               () => { startingViewForAnimation.Alpha = 0.0f; }, 
				                               () => { startingViewForAnimation.RemoveFromSuperview(); });
			});

			var startingViewFinalTransform = 1.0f / endingViewInitialTransform;
			var translatedEndingViewFinalCenter = CenterPointTranslatedTo(EndingView, containerView);

			UIView.AnimateNotify(TransitionDuration(transitionContext), 0.0f,
						   ZoomingAnimationSpringDamping, 0.0f,
						   UIViewAnimationOptions.AllowAnimatedContent | UIViewAnimationOptions.BeginFromCurrentState,
						   () =>
						   {
							   endingViewForAnimation.Transform = finalEndingViewTransform;
							   endingViewForAnimation.Center = translatedEndingViewFinalCenter;
							   startingViewForAnimation.Transform = CGAffineTransform.Scale(startingViewForAnimation.Transform, startingViewFinalTransform, startingViewFinalTransform);
							   startingViewForAnimation.Center = translatedEndingViewFinalCenter;
						   }, (finished) => { 
							endingViewForAnimation.RemoveFromSuperview();
							   EndingView.Alpha = 1.0f;
							   StartingView.Alpha = 1.0f;
							   CompleteTransition(transitionContext);
			
							}
			              );


		}

		bool ShouldPerformZoomingAnimation
		{
			get
			{
				return StartingView != null && EndingView != null;
			}
		}


		void CompleteTransition(IUIViewControllerContextTransitioning transitionContext)
		{
			if (transitionContext.IsInteractive)
			{
				if (transitionContext.TransitionWasCancelled)
					transitionContext.CancelInteractiveTransition();
				else
					transitionContext.FinishInteractiveTransition();
			}

			transitionContext.CompleteTransition(!transitionContext.TransitionWasCancelled);
		}

		CGPoint CenterPointTranslatedTo(UIView view, UIView containerView)
		{
			var centerPoint = view.Center;

			if (view.Superview.IsKindOfClass(new Class(typeof(UIScrollView)))){
				var scrollView = (UIScrollView) view.Superview;

				if (scrollView.ZoomScale != 1.0f)
				{
					centerPoint.X += (nfloat) ((scrollView.Bounds.Width - scrollView.ContentSize.Width) / 2.0 + scrollView.ContentOffset.X);
					centerPoint.Y += (nfloat) ((scrollView.Bounds.Height - scrollView.ContentSize.Height) / 2.0 + scrollView.ContentOffset.Y);
				}
			}

			return view.Superview.ConvertPointToView(centerPoint, containerView);
		}

		public UIView NewAnimationViewFromView(UIView view)
		{
			if (view == null)
				return null;

			UIView animationView;
			if (view.Layer.Contents != null)
			{
				if (view.IsKindOfClass(new Class(typeof(UIImageView))))
				{
					animationView = new UIImageView(((UIImageView)view).Image);
					animationView.Bounds = view.Bounds;
				}
				else
				{
					animationView = new UIView(view.Frame);
					animationView.Layer.Contents = view.Layer.Contents;
					animationView.Layer.Bounds = view.Layer.Bounds;
				}

				animationView.Layer.CornerRadius = view.Layer.CornerRadius;
				animationView.Layer.MasksToBounds = view.Layer.MasksToBounds;
				animationView.ContentMode = view.ContentMode;
				animationView.Transform = view.Transform;
			}
			else
			{
				animationView = view.SnapshotView(true);
			}

			return animationView;
		}

		public double TransitionDuration(IUIViewControllerContextTransitioning transitionContext)
		{
			if (ShouldPerformZoomingAnimation)
				return AnimationDurationWithZooming;

			return AnimationDurationWithoutZooming;
		}

		public void AnimateTransition(IUIViewControllerContextTransitioning transitionContext)
		{
			SetUpTransitionContainerHierarchy(transitionContext);

			PerformFadeAnimation(transitionContext);

			if (ShouldPerformZoomingAnimation)
				PerformZoomingAnimation(transitionContext);
		}
	}
}
