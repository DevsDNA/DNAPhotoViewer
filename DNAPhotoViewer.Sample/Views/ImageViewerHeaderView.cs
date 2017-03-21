namespace DNAPhotoViewer.Sample
{
	using System;
	using UIKit;

	public partial class ImageViewerHeaderView : UIViewController
	{
		int _photoIndex = 1;
		int _photosTotal = 1;

		public ImageViewerHeaderView() : base("ImageViewerHeaderView", null)
		{
		}

		public IImageViewerHeaderDelegate HeaderDelegate { get; set; }

		public int PhotoIndex
		{
			get
			{
				return _photoIndex;
			}
			set
			{
				_photoIndex = value;
				RefreshLabel();
			}
		}

		public int PhotosTotal
		{
			get
			{
				return _photosTotal;
			}
			set
			{
				_photosTotal = value;
				RefreshLabel();
			}
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			BtnClose.TouchUpInside += BtnClose_TouchUpInside;
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			RefreshLabel();
		}

		void RefreshLabel()
		{
			if (LblPages != null)
				LblPages.Text = (PhotosTotal == 1) ? "" : $"{_photoIndex} of {_photosTotal}";
		}

		void BtnClose_TouchUpInside(object sender, EventArgs e)
		{
			HeaderDelegate?.OnClose();
		}
	}
}

