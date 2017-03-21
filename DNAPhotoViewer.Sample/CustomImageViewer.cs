namespace DNAPhotoViewer.Sample
{
	using System.Collections.Generic;
	using DevsDNA.DNAPhotoViewer;
	using DevsDNA.DNAPhotoViewer.Interfaces;
	using UIKit;

	public class CustomImageViewer
	{
		DNAPhotoViewerArrayDataSource _dataSource;
		DNAPhotosViewController _openViewController;

		public void OpenDNAPhotoViewer(UIImage imageToOpen, List<UIImage> images, IDNAPhotosViewControllerDelegate photosViewControllerDelegate, UIViewController viewController)
		{
			var photos = new List<NSPhoto>();

			foreach (var image in images)
			{
				photos.Add(new NSPhoto
				{
					Image = image
				});
			}

			_dataSource = new DNAPhotoViewerArrayDataSource(photos);

			_openViewController = new DNAPhotosViewController(_dataSource, images.IndexOf(imageToOpen), photosViewControllerDelegate);
			_openViewController.Delegate = photosViewControllerDelegate;

			viewController.PresentViewController(_openViewController, true, null);
		}

		public void CloseDNAPhotoViewer()
		{
			if (_openViewController != null)
				_openViewController.DismissViewController(true, null);
		}
	}
}
