namespace DevsDNA.DNAPhotoViewer
{
	using System;
	using CoreAnimation;
	using CoreGraphics;
	using Foundation;
	using Interfaces;
	using UIKit;

	public class DNAPhotoCaptionView : UIView, IDNAPhotoCaptionViewLayoutWidthHinting
	{
		static nfloat PhotoCaptionViewHorizontalMargin = 8.0f;
		static nfloat PhotoCaptionViewVerticalMargin = 7.0f;

		nfloat _preferredMaxLayoutWidth;

		NSAttributedString _attributedTitle;
		NSAttributedString _attributedSummary;
		NSAttributedString _attributedCredit;

		UITextView textView;
		CAGradientLayer gradientLayer;


		public DNAPhotoCaptionView(CGRect frame) : base(frame)
		{
			if (this != null)
				Initialize();
		}

		public DNAPhotoCaptionView(NSCoder coder) : base(coder)
		{
			if (this != null)
				Initialize();
		}

		public override void MovedToSuperview()
		{
			base.MovedToSuperview();

			var maxHeightConstraint = NSLayoutConstraint.Create(this, NSLayoutAttribute.Height, NSLayoutRelation.LessThanOrEqual, Superview, NSLayoutAttribute.Height, 0.3f, 0.0f);
			Superview?.AddConstraint(maxHeightConstraint);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			gradientLayer.Frame = Layer.Bounds;
		}

		public override CGSize IntrinsicContentSize
		{
			get
			{
				var contentSize = textView.SizeThatFits(new CGSize(PreferredMaxLayoutWidth, float.MaxValue));
				return new CGSize(PreferredMaxLayoutWidth, Math.Ceiling(contentSize.Height));
			}
		}

		public nfloat PreferredMaxLayoutWidth
		{
			get
			{
				return _preferredMaxLayoutWidth;
			}

			set
			{
				var preferredMaxLayoutWidth = (nfloat)Math.Ceiling(value);

				if (Math.Abs(_preferredMaxLayoutWidth - preferredMaxLayoutWidth) > 0.1)
				{
					_preferredMaxLayoutWidth = preferredMaxLayoutWidth;
					InvalidateIntrinsicContentSize();
				}
			}
		}


		public DNAPhotoCaptionView(NSAttributedString attributedTitle, NSAttributedString attributedSummary, NSAttributedString attributedCredit) : base(CGRect.Empty)
		{
			if (this != null)
			{
				_attributedTitle = attributedTitle;
				_attributedSummary = attributedSummary;
				_attributedCredit = attributedCredit;

				Initialize();
			}
		}

		void Initialize()
		{
			TranslatesAutoresizingMaskIntoConstraints = false;

			SetUpTextView();
			UpdateTextViewAttributedText();
			SetUpGradient();
		}

		void SetUpTextView()
		{
			textView = new UITextView(CGRect.Empty);
			textView.TranslatesAutoresizingMaskIntoConstraints = false;
			textView.Editable = false;
			textView.DataDetectorTypes = UIDataDetectorType.None;
			textView.BackgroundColor = UIColor.Clear;
			textView.TextContainerInset = new UIEdgeInsets(PhotoCaptionViewVerticalMargin, PhotoCaptionViewHorizontalMargin, PhotoCaptionViewVerticalMargin, PhotoCaptionViewHorizontalMargin);

			AddSubview(textView);

			var topConstraint = NSLayoutConstraint.Create(textView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1.0f, 0.0f);
			var bottomConstraint = NSLayoutConstraint.Create(textView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1.0f, 0.0f);
			var widthConstraint = NSLayoutConstraint.Create(textView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, this, NSLayoutAttribute.Width, 1.0f, 0.0f);
			var horizontalPositionConstraint = NSLayoutConstraint.Create(textView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterX, 1.0f, 0.0f);

			AddConstraints(new NSLayoutConstraint[] { topConstraint, bottomConstraint, widthConstraint, horizontalPositionConstraint});
		}

		void SetUpGradient()
		{
			gradientLayer = new CAGradientLayer();
			gradientLayer.Frame = Layer.Bounds;
			gradientLayer.Colors = new CGColor[] { UIColor.Clear.CGColor, UIColor.Black.CGColor };
			Layer.InsertSublayer(gradientLayer, 0);
		}

		void UpdateTextViewAttributedText()
		{
			var attributedLabelText = new NSMutableAttributedString();

			if (_attributedTitle != null)
			{
				attributedLabelText.Append(_attributedTitle);
			}

			if (_attributedSummary != null)
			{
				if (_attributedTitle != null)
				{
					attributedLabelText.Append(new NSAttributedString("\n"));
				}

				attributedLabelText.Append(_attributedSummary);
			}

			if (_attributedCredit != null)
			{
				if (_attributedTitle != null || _attributedSummary != null)
				{
					attributedLabelText.Append(new NSAttributedString("\n"));
				}

				attributedLabelText.Append(_attributedCredit);
			}

			textView.AttributedText = attributedLabelText;
		}
	}
}
