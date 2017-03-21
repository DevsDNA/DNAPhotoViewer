namespace DNAPhotoViewer.Sample
{
	using System;
	using UIKit;

	public partial class ViewController : UIViewController
	{
		string _singlePhotoUrl = "https://images-na.ssl-images-amazon.com/images/I/81NbHKbl%2BpL.jpg";

		string[] _photosUrl;

		protected ViewController(IntPtr handle) : base(handle)
		{
			_photosUrl = new string[]
			{
				"https://images-na.ssl-images-amazon.com/images/I/81NbHKbl%2BpL.jpg",
				"https://media.gettyimages.com/photos/bill-gates-ceo-of-microsoft-reclines-on-his-desk-in-his-office-soon-picture-id533081086",
				"https://blog.xamarin.com/wp-content/uploads/2014/04/Screen-Shot-2014-04-03-at-13.31.34.png",
				"https://blogdoiphone.com/wp-content/uploads/2015/07/steve-wozniak.jpg"
			};
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			BtnSinglePhotoNoHeaderNoFooter.TouchUpInside += BtnSinglePhotoNoHeaderNoFooter_TouchUpInside;
			BtnSinglePhotoOnlyHeader.TouchUpInside += BtnSinglePhotoOnlyHeader_TouchUpInside;
			BtnSinglePhotoHeaderAndFooter.TouchUpInside += BtnSinglePhotoHeaderAndFooter_TouchUpInside;

			BtnGalleryNoHeaderNoFooter.TouchUpInside += BtnGalleryNoHeaderNoFooter_TouchUpInside;
			BtnGalleryOnlyHeader.TouchUpInside += BtnGalleryOnlyHeader_TouchUpInside;
			BtnGalleryHeaderAndFooter.TouchUpInside += BtnGalleryHeaderAndFooter_TouchUpInside;

			Title = "Sample";

		}

		void BtnSinglePhotoNoHeaderNoFooter_TouchUpInside(object sender, EventArgs e)
		{
			OpenSinglePhotoView(false, false);
		}

		void BtnSinglePhotoOnlyHeader_TouchUpInside(object sender, EventArgs e)
		{
			OpenSinglePhotoView(true, false);
		}

		void BtnSinglePhotoHeaderAndFooter_TouchUpInside(object sender, EventArgs e)
		{
			OpenSinglePhotoView(true, true);
		}

		void OpenSinglePhotoView(bool hasHeader, bool hasFooter)
		{
			var viewController = Storyboard.InstantiateViewController("SinglePhotoViewController") as SinglePhotoViewController;
			viewController.Photo = _singlePhotoUrl;
			viewController.HasHeader = hasHeader;
			viewController.HasFooter = hasFooter;

			NavigationController.PushViewController(viewController, true);
		}

		void BtnGalleryNoHeaderNoFooter_TouchUpInside(object sender, EventArgs e)
		{
			OpenGalleryView(false, false);
		}

		void BtnGalleryOnlyHeader_TouchUpInside(object sender, EventArgs e)
		{
			OpenGalleryView(true, false);
		}

		void BtnGalleryHeaderAndFooter_TouchUpInside(object sender, EventArgs e)
		{
			OpenGalleryView(true, true);
		}

		void OpenGalleryView(bool hasHeader, bool hasFooter)
		{
			var viewController = Storyboard.InstantiateViewController("GalleryViewController") as GalleryViewController;
			viewController.Photos = _photosUrl;
			viewController.HasHeader = hasHeader;
			viewController.HasFooter = hasFooter;

			NavigationController.PushViewController(viewController, true);
		}
	}
}
