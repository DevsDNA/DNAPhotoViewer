namespace DevsDNA.DNAPhotoViewer
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Interfaces;

	public class DNAPhotoViewerArrayDataSource : IDNAPhotoViewerDataSource
	{
		public DNAPhotoViewerArrayDataSource(IEnumerable<NSPhoto> photos)
		{
			if (photos == null)
			{
				Photos = new List<NSPhoto>();
			}
			else
			{
				Photos = photos.ToList();
			}
		}

		public List<NSPhoto> Photos { get; set;}

		public nint NumberOfPhotos
		{
			get
			{
				return Photos.Count;
			}
		}

		public nint IndexOfPhoto(NSPhoto photo)
		{
			return Photos.IndexOf(photo);
		}

		public NSPhoto PhotoAtIndex(nint photoIndex)
		{
			if (photoIndex < Photos.Count)
				return Photos[(int)photoIndex];

			return null;
		}
	}
}
