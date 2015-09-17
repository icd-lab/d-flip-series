using System.Collections.Generic;
using PhotoViewer.PhotoInfo.Tag;

namespace PhotoViewer.Database.Table
{
    interface TableProcessor
    {
        Dictionary<string, PhotoTag> select(List<string> filename);
    }
}
