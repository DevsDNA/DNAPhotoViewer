namespace DevsDNA.DNAPhotoViewer.Interfaces
{
	using System;

	public interface IDNAPhotoViewerDataSource
	{
		nint NumberOfPhotos { get; }

		NSPhoto PhotoAtIndex(nint photoIndex);

		nint IndexOfPhoto(NSPhoto photo);
	}
}
