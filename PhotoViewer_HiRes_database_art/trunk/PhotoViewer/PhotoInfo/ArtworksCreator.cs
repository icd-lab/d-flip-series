using PhotoViewer.Database.Table;

namespace PhotoViewer.PhotoInfo.PhotoConstructor
{
    class ArtworksCreator: PhotoCreator
    {
        public ArtworksCreator(): base(new ArtworksTable())
        {
 
        }
    }
}
