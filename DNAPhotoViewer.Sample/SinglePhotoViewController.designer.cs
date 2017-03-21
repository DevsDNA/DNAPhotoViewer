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
	[Register ("SinglePhotoViewController")]
	partial class SinglePhotoViewController
	{
		[Outlet]
		UIKit.UIButton BtnHover { get; set; }

		[Outlet]
		UIKit.UIImageView ImgView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ImgView != null) {
				ImgView.Dispose ();
				ImgView = null;
			}

			if (BtnHover != null) {
				BtnHover.Dispose ();
				BtnHover = null;
			}
		}
	}
}
