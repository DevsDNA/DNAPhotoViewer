namespace DNAPhotoViewer.Sample
{
	using System;
	using System.Linq;
	using DevsDNA.DNAPhotoViewer;
	using DevsDNA.DNAPhotoViewer.Interfaces;
	using Foundation;
	using SDWebImage;
	using UIKit;

	public partial class SinglePhotoViewController : UIViewController, IDNAPhotosViewControllerDelegate, IImageViewerHeaderDelegate, IImageViewerFooterDelegate
	{
		CustomImageViewer _imageViewer;
		ImageViewerHeaderView _headerView;
		ImageViewerFooterView _footerView;

		public SinglePhotoViewController (IntPtr handle) : base (handle)
		{
		}

		public string Photo { get; set; }
		public bool HasHeader { get; set; }
		public bool HasFooter { get; set; }

		CustomImageViewer ImageViewer
		{
			get
			{
				if (_imageViewer == null)
					_imageViewer = new CustomImageViewer();
				return _imageViewer;
			}
		}

		ImageViewerHeaderView HeaderView
		{
			get
			{
				if (_headerView == null)
					_headerView = new ImageViewerHeaderView
					{
						HeaderDelegate = this
					};
				return _headerView;
			}
		}

		ImageViewerFooterView FooterView
		{
			get
			{
				if (_footerView == null)
					_footerView = new ImageViewerFooterView
					{
						FooterDelegate = this
					};
				return _footerView;
			}
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			ImgView.SetImage(new NSUrl(Photo));

			BtnHover.TouchUpInside += BtnHover_TouchUpInside;
		}

		void BtnHover_TouchUpInside(object sender, EventArgs e)
		{
			ImageViewer.OpenDNAPhotoViewer(ImgView.Image, new[] { ImgView.Image }.ToList(), this, this);
		}

		[Export("photosViewController:didNavigateToPhoto:atIndex:")]
		public virtual void DidNavigateToPhotoAtIndex(DNAPhotosViewController photosViewController, NSPhoto photo, nint photoIndex)
		{
		}

		[Export("photosViewControllerWillDismiss:")]
		public virtual void PhotosViewControllerWillDismiss(DNAPhotosViewController photosViewController)
		{
		}

		[Export("photosViewControllerDidDismiss:")]
		public virtual void PhotosViewControllerDidDismiss(DNAPhotosViewController photosViewController)
		{
		}

		[Export("photosViewController:captionViewForPhoto:")]
		public virtual UIView CaptionViewForPhoto(DNAPhotosViewController photosViewController, NSPhoto photo)
		{
			return null;
		}

		[Export("photosViewController:titleForPhoto:atIndex:totalPhotoCount:")]
		public virtual string TitleForPhotoWithTotalPhotoCount(DNAPhotosViewController photosViewController, NSPhoto photo, nint photoIndex, nint totalPhotoCount)
		{
			return "";
		}

		[Export("photosViewController:loadingViewForPhoto:")]
		public virtual UIView LoadingViewForPhoto(DNAPhotosViewController photosViewController, NSPhoto photo)
		{
			return null;
		}

		[Export("photosViewController:referenceViewForPhoto:")]
		public virtual UIView ReferenceViewForPhoto(DNAPhotosViewController photosViewController, NSPhoto photo)
		{
			return ImgView;
		}

		[Export("photosViewController:maximumZoomScaleForPhoto:")]
		public virtual nfloat MaximumZoomScaleForPhoto(DNAPhotosViewController photosViewController, NSPhoto photo)
		{
			return 5.0f;
		}

		[Export("photosViewController:handleLongPressForPhoto:withGestureRecognizer:")]
		public virtual bool HandleLongPressForPhoto(DNAPhotosViewController photosViewController, NSPhoto photo, UILongPressGestureRecognizer longPressGestureRecognizer)
		{
			return false;
		}

		[Export("photosViewController:handleActionButtonTappedForPhoto:")]
		public virtual bool HandleActionButtonTappedForPhoto(DNAPhotosViewController photosViewController, NSPhoto photo)
		{
			return false;
		}

		[Export("photosViewController:actionCompletedWithActivityType:")]
		public virtual void ActionCompletedWithActivityType(DNAPhotosViewController photosViewController, string activityType)
		{
		}

		[Export("photosViewController:headerViewForPhoto:")]
		public UIView HeaderViewForPhoto(DNAPhotosViewController photosViewController, NSPhoto photo)
		{
			if (HasHeader)
				return HeaderView.View;
			
			return null;
		}

		[Export("photosViewController:headerViewHeightForPhoto:")]
		public nfloat HeaderViewHeightForPhoto(DNAPhotosViewController photosViewController, NSPhoto photo)
		{
			return 54.0f;
		}

		[Export("photosViewController:footerViewForPhoto:")]
		public UIView FooterViewForPhoto(DNAPhotosViewController photosViewController, NSPhoto photo)
		{
			if (HasFooter)
				return FooterView.View;
			
			return null;
		}

		[Export("photosViewController:footerViewHeightForPhoto:")]
		public nfloat FooterViewHeightForPhoto(DNAPhotosViewController photosViewController, NSPhoto photo)
		{
			return 52.0f;
		}

		void IImageViewerHeaderDelegate.OnClose()
		{
			ImageViewer.CloseDNAPhotoViewer();
		}
	}
}
