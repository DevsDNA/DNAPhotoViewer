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
	[Register ("ImageViewerHeaderView")]
	partial class ImageViewerHeaderView
	{
		[Outlet]
		UIKit.UIButton BtnClose { get; set; }

		[Outlet]
		UIKit.UILabel LblPages { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BtnClose != null) {
				BtnClose.Dispose ();
				BtnClose = null;
			}

			if (LblPages != null) {
				LblPages.Dispose ();
				LblPages = null;
			}
		}
	}
}
