namespace DevsDNA.DNAPhotoViewer
{
	using System;
	using CoreGraphics;
	using Foundation;
	using UIKit;

	public class DNAScalingImageView : UIScrollView
	{
		public UIImageView ImageView { get; private set;}


		public DNAScalingImageView(CGRect frame) : base(frame)
		{
			Initialize(new UIImage(), null);
		}

		public DNAScalingImageView(NSCoder coder) : base(coder)
		{
			Initialize(null, null);
		}

		public DNAScalingImageView(UIImage image, CGRect frame) : base(frame)
		{
			Initialize(image, null);
		}

		public DNAScalingImageView(NSData imageData, CGRect frame) : base(frame)
		{
			Initialize(null, imageData);
		}

		public void UpdateImage(UIImage image)
		{
			UpdateImage(image, null);
		}

		public void UpdateImageData(NSData imageData)
		{
			UpdateImage(null, imageData);
		}

		public void CenterScrollViewContents()
		{
			nfloat horizontalInset = 0f;
			nfloat verticalInset = 0f;

			if (ContentSize.Width < Bounds.Width)
				horizontalInset = (Bounds.Width - ContentSize.Width) * 0.5f;

			if (ContentSize.Height < Bounds.Height)
				verticalInset = (Bounds.Height - ContentSize.Height) * 0.5f;

			if (Window != null && Window.Screen.Scale < 2.0)
			{
				horizontalInset = (nfloat) Math.Floor(horizontalInset);
				verticalInset = (nfloat) Math.Floor(verticalInset);
			}

			ContentInset = new UIEdgeInsets(verticalInset, horizontalInset, verticalInset, horizontalInset);
		}

		public override void AddSubview(UIView view)
		{
			base.AddSubview(view);
			CenterScrollViewContents();
		}

		public override CGRect Frame
		{
			get
			{
				return base.Frame;
			}
			set
			{
				base.Frame = value;
				UpdateZoomScale();
				CenterScrollViewContents();
			}
		}

		void Initialize(UIImage image, NSData imageData)
		{
			SetUpInternalImage(image, imageData);
			SetUpImageScrollView();
			UpdateZoomScale();
		}

		void SetUpInternalImage(UIImage image, NSData imageData)
		{
			var imageToUse = (image != null) ? image : UIImage.LoadFromData(imageData);

			ImageView = new UIImageView(imageToUse);
			UpdateImage(imageToUse, imageData);
			AddSubview(ImageView);
		}

		void SetUpImageScrollView()
		{
			AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			ShowsVerticalScrollIndicator = false;
			ShowsHorizontalScrollIndicator = false;
			BouncesZoom = true;
			DecelerationRate = DecelerationRateFast;
		}

		void UpdateZoomScale()
		{
			if (ImageView != null && ImageView.Image != null)
			{
				var scrollViewFrame = Bounds;

				var scaleWidth = scrollViewFrame.Width / ImageView.Image.Size.Width;
				var scaleHeight = scrollViewFrame.Height / ImageView.Image.Size.Height;
				var minScale = Math.Min(scaleWidth, scaleHeight);

				MinimumZoomScale = (nfloat) minScale;
				MaximumZoomScale = (nfloat) Math.Max(minScale, MaximumZoomScale);

				ZoomScale = MinimumZoomScale;

				PanGestureRecognizer.Enabled = false;
			}
		}

		void UpdateImage(UIImage image, NSData imageData)
		{
			var imageToUse = (image != null) ? image : UIImage.LoadFromData(imageData);

			ImageView.Transform = CGAffineTransform.MakeIdentity();
			ImageView.Image = imageToUse;

			ImageView.Frame = new CGRect(0, 0, imageToUse.Size.Width, imageToUse.Size.Height);
			ContentSize = imageToUse.Size;

			UpdateZoomScale();
			CenterScrollViewContents();
		}

	}
}
