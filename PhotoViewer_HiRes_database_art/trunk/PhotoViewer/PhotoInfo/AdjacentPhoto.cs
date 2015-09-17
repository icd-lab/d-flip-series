using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhotoViewer.PhotoInfo
{
    class AdjacentPhoto : IDisposable
    {
        static List<AdjacentPhoto> pool_ = new List<AdjacentPhoto>();

        private Photo photo_;
        private Vector2 dir_;
        private float dira_;

        private AdjacentPhoto()
        {
        }

        public static AdjacentPhoto newInstance(Photo photo, Vector2 dir, float dira)
        {
            AdjacentPhoto ap;

            if (pool_.Count > 0)
            {
                ap = pool_[pool_.Count - 1];
                pool_.RemoveAt(pool_.Count - 1);
            }
            else
            {
                ap = new AdjacentPhoto();
            }

            ap.photo_ = photo;
            ap.dir_ = dir;
            ap.dira_ = dira;

            return ap;
        }

        public void Dispose()
        {
            pool_.Add(this);
        }

        public Photo Photo
        {
            get
            {
                return photo_;
            }
        }

        public Vector2 Direction
        {
            get
            {
                return dir_;
            }
        }

        public float AngleDirection
        {
            get
            {
                return dira_;
            }
        }
    }
}
