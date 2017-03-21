namespace DevsDNA.DNAPhotoViewer
{
	using System;
	using Interfaces;

	public class DNAPhotoViewerSinglePhotoDataSource : IDNAPhotoViewerDataSource
	{
		public DNAPhotoViewerSinglePhotoDataSource(NSPhoto photo)
		{
			Photo = photo;
		}

		public NSPhoto Photo { get; set;}

		public nint NumberOfPhotos
		{
			get
			{
				return 1;
			}
		}

		public nint IndexOfPhoto(NSPhoto photo)
		{
			return 0;
		}

		public NSPhoto PhotoAtIndex(nint photoIndex)
		{
			return Photo;
		}
	}
}
