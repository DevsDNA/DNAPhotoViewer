// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace DNAPhotoViewer.Sample
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UIButton BtnGalleryHeaderAndFooter { get; set; }

		[Outlet]
		UIKit.UIButton BtnGalleryNoHeaderNoFooter { get; set; }

		[Outlet]
		UIKit.UIButton BtnGalleryOnlyHeader { get; set; }

		[Outlet]
		UIKit.UIButton BtnSinglePhotoHeaderAndFooter { get; set; }

		[Outlet]
		UIKit.UIButton BtnSinglePhotoNoHeaderNoFooter { get; set; }

		[Outlet]
		UIKit.UIButton BtnSinglePhotoOnlyHeader { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BtnSinglePhotoNoHeaderNoFooter != null) {
				BtnSinglePhotoNoHeaderNoFooter.Dispose ();
				BtnSinglePhotoNoHeaderNoFooter = null;
			}

			if (BtnSinglePhotoOnlyHeader != null) {
				BtnSinglePhotoOnlyHeader.Dispose ();
				BtnSinglePhotoOnlyHeader = null;
			}

			if (BtnSinglePhotoHeaderAndFooter != null) {
				BtnSinglePhotoHeaderAndFooter.Dispose ();
				BtnSinglePhotoHeaderAndFooter = null;
			}

			if (BtnGalleryNoHeaderNoFooter != null) {
				BtnGalleryNoHeaderNoFooter.Dispose ();
				BtnGalleryNoHeaderNoFooter = null;
			}

			if (BtnGalleryOnlyHeader != null) {
				BtnGalleryOnlyHeader.Dispose ();
				BtnGalleryOnlyHeader = null;
			}

			if (BtnGalleryHeaderAndFooter != null) {
				BtnGalleryHeaderAndFooter.Dispose ();
				BtnGalleryHeaderAndFooter = null;
			}
		}
	}
}
