using System.Collections.Generic;
using PhotoViewer.Element;
using PhotoInfo;
using PhotoViewer.Manager;

namespace Attractor
{
    interface IAttractorSelection
    {
        void select(Dock dock, ScrollBar sBar, AttractorWeight weight, List<Photo> photos, List<Photo> activePhotos, List<Stroke> strokes, SystemState systemState);
    }
}
