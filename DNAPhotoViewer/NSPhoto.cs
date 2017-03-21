namespace DevsDNA.DNAPhotoViewer
{
	using Foundation;
	using UIKit;

	public class NSPhoto : NSObject
	{
		public NSAttributedString AttributedCaptionCredit { get; set; }

		public NSAttributedString AttributedCaptionSummary { get; set; }

		public NSAttributedString AttributedCaptionTitle { get; set; }

		public UIImage Image { get; set; }

		public string ImageUrl { get; set; }

		public NSData ImageData { get; set; }

		public UIImage PlaceholderImage { get; set; }
	}
}
