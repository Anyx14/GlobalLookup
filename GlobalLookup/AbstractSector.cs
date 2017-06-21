using System.Collections.Generic;

namespace GlobalLookup
{
    public abstract class AbstractSector<T> where T : IGeoPlottable
    {
        protected virtual SuperSector<T> SuperSector { get; set; }

        public double MaxLat { get; set; }
        public double MinLat { get; set; }
        public double MaxLon { get; set; }
        public double MinLon { get; set; }

        protected AbstractSector(double minLat, double maxLat, double minLon, double maxLon)
        {
            MaxLat = maxLat;
            MinLat = minLat;
            MaxLon = maxLon;
            MinLon = minLon;
        }

        public virtual Globe<T> GetGlobe()
        {
            return SuperSector.GetGlobe();
        }

        public abstract void Add(T item);

        public abstract List<T> GetAllItems();

        public abstract List<T> Search(double lat, double lon, double range);

        public abstract void Remove(T item);

        //protected bool FallsWithinRadius(double lat, double lon, double radius)
        //{
        //    
        //}

    }
}
