namespace DNAPhotoViewer.Sample
{
	using UIKit;

	public partial class ImageViewerFooterView : UIViewController
	{
		public ImageViewerFooterView() : base("ImageViewerFooterView", null)
		{
		}

		public IImageViewerFooterDelegate FooterDelegate { get; set; }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();


		}
	}
}

