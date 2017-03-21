namespace DevsDNA.DNAPhotoViewer
{
	using CoreGraphics;
	using Foundation;
	using UIKit;

	public class DNAPhotoTransitionController : NSObject, IUIViewControllerTransitioningDelegate
	{
		DNAPhotoTransitionAnimator _animator;
		DNAPhotoDismissalInteractionController _interactionController;

		public DNAPhotoTransitionController()
		{
			_animator = new DNAPhotoTransitionAnimator();
			_interactionController = new DNAPhotoDismissalInteractionController();
			ForcesNonInteractiveDismissal = true;
		}

		public UIView StartingView
		{
			get
			{
				return _animator.StartingView;
			}
			set
			{
				_animator.StartingView = value;
			}
		}

		public UIView EndingView
		{
			get
			{
				return _animator.EndingView;
			}
			set
			{
				_animator.EndingView = value;
			}
		}

		public bool ForcesNonInteractiveDismissal { get; set; }

		public void DidPanWithPanGestureRecognizer(UIPanGestureRecognizer panGestureRecognizer, UIView viewToPan, CGPoint anchorPoint)
		{
			_interactionController.DidPan(panGestureRecognizer, viewToPan, anchorPoint);
		}

		[Export("animationControllerForPresentedController:presentingController:sourceController:")]
		public IUIViewControllerAnimatedTransitioning GetAnimationControllerForPresentedController(UIViewController presented, UIViewController presenting, UIViewController source)
		{
			_animator.IsDismissing = false;
			return _animator;
		}

		[Export("animationControllerForDismissedController:")]
		public IUIViewControllerAnimatedTransitioning GetAnimationControllerForDismissedController(UIViewController dismissed)
		{
			_animator.IsDismissing = true;
			return _animator;
		}

		[Export("interactionControllerForDismissal:")]
		public IUIViewControllerInteractiveTransitioning GetInteractionControllerForDismissal(IUIViewControllerAnimatedTransitioning animator)
		{
			if (ForcesNonInteractiveDismissal)
				return null;

			_animator.EndingViewForAnimation = _animator.NewAnimationViewFromView(EndingView);

			_interactionController.Animator = _animator;
			_interactionController.ShouldAnimateUsingAnimator = EndingView != null;
			_interactionController.ViewToHideWhenBeginningTransition = (StartingView != null) ? EndingView : null;

			return _interactionController;
		}
	}
}
