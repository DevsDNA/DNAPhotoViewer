namespace DevsDNA.DNAPhotoViewer
{
	using System;
	using CoreGraphics;
	using UIKit;

	public class DNAPhotoDismissalInteractionController : UIViewControllerInteractiveTransitioning
	{
		static nfloat PhotoDismissalInteractionControllerPanDismissDistanceRatio = 50.0f / 667.0f; // distance over iPhone 6 height.
		static nfloat PhotoDismissalInteractionControllerPanDismissMaximumDuration = 0.45f;
		static nfloat PhotoDismissalInteractionControllerReturnToCenterVelocityAnimationRatio = 0.00007f; // Arbitrary value that looked decent.

		IUIViewControllerContextTransitioning _transitionContext;

		public IUIViewControllerAnimatedTransitioning Animator { get; set; }
		public UIView ViewToHideWhenBeginningTransition { get; set; }
		public bool ShouldAnimateUsingAnimator { get; set; }

		public void DidPan(UIPanGestureRecognizer panGestureRecognizer, UIView viewToPan, CGPoint anchorPoint)
		{
			var fromView = _transitionContext?.GetViewFor(UITransitionContext.FromViewKey);
			var translatedPanGesturePoint = panGestureRecognizer.TranslationInView(fromView);
			var newCenterPoint = new CGPoint(anchorPoint.X, anchorPoint.Y + translatedPanGesturePoint.Y);

			viewToPan.Center = newCenterPoint;

			var verticalDelta = newCenterPoint.Y - anchorPoint.Y;

			var backgroundAlpha = BackgroundAlphaForPanning(verticalDelta);

			if(fromView != null)
				fromView.BackgroundColor = fromView.BackgroundColor.ColorWithAlpha(backgroundAlpha);

			if (panGestureRecognizer.State == UIGestureRecognizerState.Ended)
			   FinishPan(panGestureRecognizer, verticalDelta, viewToPan, anchorPoint);
		}

		void FinishPan(UIPanGestureRecognizer panGestureRecognizer, nfloat verticalDelta, UIView viewToPan, CGPoint anchorPoint)
		{
			var fromView = _transitionContext?.GetViewFor(UITransitionContext.FromViewKey);

			var velocityY = panGestureRecognizer.VelocityInView(panGestureRecognizer.View).Y;

			var animationDuration = Math.Abs(velocityY) * PhotoDismissalInteractionControllerReturnToCenterVelocityAnimationRatio + 0.2;
			var animationCurve = UIViewAnimationOptions.CurveEaseOut;
			var finalPageViewCenterPount = anchorPoint;
			var finalBackgroundAlpha = 1.0f;

			var dismissDistance = (fromView == null) ? 0.0f : PhotoDismissalInteractionControllerPanDismissDistanceRatio * fromView.Bounds.Height;

			var isDismissing = Math.Abs(verticalDelta) > dismissDistance;

			var didAnimateUsingAnimator = false;

			if (isDismissing)
			{
				if (ShouldAnimateUsingAnimator)
				{
					Animator.AnimateTransition(_transitionContext);
					didAnimateUsingAnimator = true;
				}
				else
				{
					var isPositiveDelta = verticalDelta >= 0;

					var modifier = isPositiveDelta ? 1 : -1;

					if (fromView == null)
					{
						finalPageViewCenterPount = new CGPoint(0.0f, 0.0f);
					}
					else
					{
						var finalCenterY = fromView.Bounds.GetMidY() + modifier * fromView.Bounds.Height;
						finalPageViewCenterPount = new CGPoint(fromView.Center.X, finalCenterY);
					}

					animationDuration = Math.Abs(finalPageViewCenterPount.Y - viewToPan.Center.Y) / Math.Abs(velocityY);
					animationDuration = Math.Min(animationDuration, PhotoDismissalInteractionControllerPanDismissMaximumDuration);

					animationCurve = UIViewAnimationOptions.CurveEaseOut;
					finalBackgroundAlpha = 0.0f;
				}
			}

			if (!didAnimateUsingAnimator)
			{
				UIView.Animate(animationDuration, 0, 
				               animationCurve, 
				               () => {
								   viewToPan.Center = finalPageViewCenterPount;
								   fromView.BackgroundColor = fromView.BackgroundColor.ColorWithAlpha(finalBackgroundAlpha);
								}, 
				               () =>
							   {
								   if (isDismissing)
								   {
									   _transitionContext.FinishInteractiveTransition();
								   }
								   else
								   {
									   _transitionContext.CancelInteractiveTransition();
									}
									
									if(ViewToHideWhenBeginningTransition != null)
								   		ViewToHideWhenBeginningTransition.Alpha = 1.0f;
								_transitionContext.CompleteTransition(isDismissing && !_transitionContext.TransitionWasCancelled);
								   _transitionContext = null;
								}
					);
			}
			else
			{
				_transitionContext = null;
			}

		}

		nfloat BackgroundAlphaForPanning(nfloat verticalDelta)
		{
			var startingAlpha = 1.0f;
			var finalAlpha = 0.1f;
			var totalAvailableAlpha = startingAlpha - finalAlpha;

			var maximumDelta = (_transitionContext == null) ? 0.0f : _transitionContext.GetViewFor(UITransitionContext.FromViewKey).Bounds.Height / 2.0f;

			var deltaAsPercentageOfMaximum = (nfloat) Math.Min(Math.Abs(verticalDelta) / maximumDelta, 1.0);

			return startingAlpha - (deltaAsPercentageOfMaximum * totalAvailableAlpha);
		}

		public override void StartInteractiveTransition(IUIViewControllerContextTransitioning transitionContext)
		{
			if(ViewToHideWhenBeginningTransition != null)
				ViewToHideWhenBeginningTransition.Alpha = 0.0f;
			_transitionContext = transitionContext;
		}
	}
}
