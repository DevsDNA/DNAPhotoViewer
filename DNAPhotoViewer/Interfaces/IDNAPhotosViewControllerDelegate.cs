namespace DevsDNA.DNAPhotoViewer.Interfaces
{
	using System;
	using UIKit;

	public interface IDNAPhotosViewControllerDelegate
	{
		void DidNavigateToPhotoAtIndex(DNAPhotosViewController photosViewController, NSPhoto photo, nint photoIndex);

		void PhotosViewControllerWillDismiss(DNAPhotosViewController photosViewController);

		void PhotosViewControllerDidDismiss(DNAPhotosViewController photosViewController);

		UIView CaptionViewForPhoto(DNAPhotosViewController photosViewController, NSPhoto photo);

		string TitleForPhotoWithTotalPhotoCount(DNAPhotosViewController photosViewController, NSPhoto photo, nint photoIndex, nint totalPhotoCount);

		UIView LoadingViewForPhoto(DNAPhotosViewController photosViewController, NSPhoto photo);

		UIView ReferenceViewForPhoto(DNAPhotosViewController photosViewController, NSPhoto photo);

		nfloat MaximumZoomScaleForPhoto(DNAPhotosViewController photosViewController, NSPhoto photo);

		bool HandleLongPressForPhoto(DNAPhotosViewController photosViewController, NSPhoto photo, UILongPressGestureRecognizer longPressGestureRecognizer);

		bool HandleActionButtonTappedForPhoto(DNAPhotosViewController photosViewController, NSPhoto photo);

		void ActionCompletedWithActivityType(DNAPhotosViewController photosViewController, string activityType);


		UIView HeaderViewForPhoto(DNAPhotosViewController photosViewController, NSPhoto photo);
		nfloat HeaderViewHeightForPhoto(DNAPhotosViewController photosViewController, NSPhoto photo);

		UIView FooterViewForPhoto(DNAPhotosViewController photosViewController, NSPhoto photo);
		nfloat FooterViewHeightForPhoto(DNAPhotosViewController photosViewController, NSPhoto photo);

	}
}
