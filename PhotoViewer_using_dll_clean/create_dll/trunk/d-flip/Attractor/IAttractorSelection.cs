using System.Collections.Generic;
using dflip.Element;
using PhotoInfo;
using dflip.Manager;

namespace Attractor
{
    interface IAttractorSelection
    {
        void select(Dock dock, ScrollBar sBar, AttractorWeight weight, List<Photo> photos, List<Photo> activePhotos, List<Stroke> strokes, SystemState systemState);
    }
}
