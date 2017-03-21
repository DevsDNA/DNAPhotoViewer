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
	[Register ("GalleryViewController")]
	partial class GalleryViewController
	{
		[Outlet]
		UIKit.UIButton Btn1 { get; set; }

		[Outlet]
		UIKit.UIButton Btn2 { get; set; }

		[Outlet]
		UIKit.UIButton Btn3 { get; set; }

		[Outlet]
		UIKit.UIButton Btn4 { get; set; }

		[Outlet]
		UIKit.UIImageView Img1 { get; set; }

		[Outlet]
		UIKit.UIImageView Img2 { get; set; }

		[Outlet]
		UIKit.UIImageView Img3 { get; set; }

		[Outlet]
		UIKit.UIImageView Img4 { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Img1 != null) {
				Img1.Dispose ();
				Img1 = null;
			}

			if (Btn1 != null) {
				Btn1.Dispose ();
				Btn1 = null;
			}

			if (Img3 != null) {
				Img3.Dispose ();
				Img3 = null;
			}

			if (Btn3 != null) {
				Btn3.Dispose ();
				Btn3 = null;
			}

			if (Img2 != null) {
				Img2.Dispose ();
				Img2 = null;
			}

			if (Btn2 != null) {
				Btn2.Dispose ();
				Btn2 = null;
			}

			if (Img4 != null) {
				Img4.Dispose ();
				Img4 = null;
			}

			if (Btn4 != null) {
				Btn4.Dispose ();
				Btn4 = null;
			}
		}
	}
}
