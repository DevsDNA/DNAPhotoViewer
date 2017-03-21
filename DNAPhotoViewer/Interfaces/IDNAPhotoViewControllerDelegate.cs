namespace DevsDNA.DNAPhotoViewer.Interfaces
{
	using Foundation;
	using UIKit;

	public interface IDNAPhotoViewControllerDelegate
	{
		[Export("photoViewController:didLongPressWithGestureRecognizer:")]
		void DidLongPressWithGestureRecognizer(DNAPhotoViewController photoViewController, UILongPressGestureRecognizer longPressGestureRecognizer);
	}
}
