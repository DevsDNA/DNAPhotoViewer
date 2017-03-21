namespace DevsDNA.DNAPhotoViewer
{
	using CoreGraphics;
	using ObjCRuntime;
	using Interfaces;
	using UIKit;
	using System;

	public class DNAPhotosOverlayView : UIView
	{
		UINavigationItem _navigationItem;
		UINavigationBar _navigationBar;

		UIView _captionView;

		UIView _headerView;
		UIView _footerView;

		public DNAPhotosOverlayView(CGRect frame) : base(frame)
		{
			if (this != null)
			{
				SetUpNavigationBar();
			}
		}

		public override UIView HitTest(CGPoint point, UIEvent uievent)
		{
			var hitView = base.HitTest(point, uievent);

			if (hitView == this)
				return null;

			return hitView;
		}

		public override void LayoutSubviews()
		{
			PerformWithoutAnimation(() =>
			{
				NavigationBar.InvalidateIntrinsicContentSize();
				NavigationBar.LayoutIfNeeded();
			});

			base.LayoutSubviews();

			if (_captionView != null && _captionView.ConformsToProtocol(Runtime.GetProtocol("IPhotoCaptionViewLayoutWidthHinting")))
			{
				((IDNAPhotoCaptionViewLayoutWidthHinting)_captionView).PreferredMaxLayoutWidth = Bounds.Width;
			}

		}
        
		void SetUpNavigationBar()
		{
			NavigationBar = new UINavigationBar();
			NavigationBar.TranslatesAutoresizingMaskIntoConstraints = false;

			NavigationBar.BackgroundColor = UIColor.Clear;
			NavigationBar.BarTintColor = null;
			NavigationBar.Translucent = true;
			NavigationBar.ShadowImage = new UIImage();
			NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);

			NavigationItem = new UINavigationItem("");
			NavigationBar.Items = new[] { NavigationItem };

			AddSubview(NavigationBar);

			var topConstraint = NSLayoutConstraint.Create(NavigationBar, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1.0f, 0.0f);
			var widthConstraint = NSLayoutConstraint.Create(NavigationBar, NSLayoutAttribute.Width, NSLayoutRelation.Equal, this, NSLayoutAttribute.Width, 1.0f, 0.0f);
			var horizontalPositionConstraint = NSLayoutConstraint.Create(NavigationBar, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterX, 1.0f, 0.0f);

			AddConstraints(new[] { topConstraint, widthConstraint, horizontalPositionConstraint });

		}

		public UIView CaptionView
		{
			get { return _captionView; }
			set
			{
				if (_captionView == value)
					return;

				_captionView?.RemoveFromSuperview();

				_captionView = value;
				_captionView.TranslatesAutoresizingMaskIntoConstraints = false;

				AddSubview(_captionView);

				var bottomConstraint = NSLayoutConstraint.Create(_captionView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1.0f, 0.0f);
				var widthConstraint = NSLayoutConstraint.Create(_captionView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, this, NSLayoutAttribute.Width, 1.0f, 0.0f);
				var horizontalPositionConstraint = NSLayoutConstraint.Create(_captionView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterX, 1.0f, 0.0f);

				AddConstraints(new[] { bottomConstraint, widthConstraint, horizontalPositionConstraint });
			}
		}

		public UIView HeaderView
		{
			get { return _headerView; }
			set
			{
				if (_headerView == value)
					return;

				_headerView?.RemoveFromSuperview();

				_headerView = value;
				_headerView.TranslatesAutoresizingMaskIntoConstraints = false;

				AddSubview(_headerView);

				var topConstraint = NSLayoutConstraint.Create(_headerView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1.0f, 0.0f);
				var widthConstraint = NSLayoutConstraint.Create(_headerView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, this, NSLayoutAttribute.Width, 1.0f, 0.0f);
				var heightConstraint = NSLayoutConstraint.Create(_headerView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1.0f, HeaderViewHeight);
				var horizontalPositionConstraint = NSLayoutConstraint.Create(_headerView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterX, 1.0f, 0.0f);

				AddConstraints(new[] { topConstraint, widthConstraint, heightConstraint, horizontalPositionConstraint });
			}
		}

		public UIView FooterView
		{
			get { return _footerView; }
			set
			{
				if (_footerView == value)
					return;

				_footerView?.RemoveFromSuperview();

				_footerView = value;
				_footerView.TranslatesAutoresizingMaskIntoConstraints = false;

				AddSubview(_footerView);

				var bottomConstraint = NSLayoutConstraint.Create(_footerView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1.0f, 0.0f);
				var widthConstraint = NSLayoutConstraint.Create(_footerView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, this, NSLayoutAttribute.Width, 1.0f, 0.0f);
				var heightConstraint = NSLayoutConstraint.Create(_footerView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1.0f, FooterViewHeight);
				var horizontalPositionConstraint = NSLayoutConstraint.Create(_footerView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterX, 1.0f, 0.0f);

				AddConstraints(new[] { bottomConstraint, widthConstraint, heightConstraint, horizontalPositionConstraint });
			}
		}


		public nfloat HeaderViewHeight { get; set; }
		public nfloat FooterViewHeight { get; set; }

		public UIBarButtonItem LeftBarButtonItem
		{
			get
			{
				return NavigationItem.LeftBarButtonItem;
			} 
			set
			{
				NavigationItem.SetLeftBarButtonItem(value, false);
			}
		}
		public UIBarButtonItem[] LeftBarButtonItems
		{
			get
			{
				return NavigationItem.LeftBarButtonItems;
			}
			set
			{
				NavigationItem.SetLeftBarButtonItems(value, false);
			}
		}

		public UIBarButtonItem RightBarButtonItem
		{
			get
			{
				return NavigationItem.RightBarButtonItem;
			}
			set
			{
				NavigationItem.SetRightBarButtonItem(value, false);
			}
		}
		public UIBarButtonItem[] RightBarButtonItems
		{
			get
			{
				return NavigationItem.RightBarButtonItems;
			}
			set
			{
				NavigationItem.SetRightBarButtonItems(value, false);
			}
		}

		public string Title
		{
			get
			{
				return NavigationItem.Title;
			}
			set
			{
				NavigationItem.Title = value;
			}
		}

		public UINavigationBar NavigationBar
		{
			get { return _navigationBar; }
			set { _navigationBar = value; }
		}

		public UINavigationItem NavigationItem
		{
			get { return _navigationItem; }
			set { _navigationItem = value; }
		}

		public UIStringAttributes TitleTextAttributes
		{
			get
			{
				return NavigationBar.TitleTextAttributes;
			}
			set
			{
				NavigationBar.TitleTextAttributes = value;
			}
		}
	}
}
