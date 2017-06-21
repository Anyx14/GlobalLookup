using System.Collections.Generic;

namespace GlobalLookup
{
    public class Sector<T> : AbstractSector<T> where T : IGeoPlottable
    {
        private List<T> items = new List<T>();

        public Sector(SuperSector<T> superSector, double minLat, double maxLat, double minLon, double maxLon)
            : base(minLat, maxLat, minLon, maxLon)
        {
            SuperSector = superSector;
        }

        public override void Add(T item)
        {
            items.Add(item);
        }

        public override List<T> GetAllItems()
        {
            List<T> all = new List<T>();
            all.AddRange(items);
            return all;
        }

        public override List<T> Search(double lat, double lon, double range)
        {
            List<T> results = new List<T>();
            foreach (T item in items)
            {
                if (this.GetGlobe().GeoDistance(lat, lon, item.Lat, item.Lon) < range)
                {
                    results.Add(item);
                }
            }
            return results;
        }

        public override void Remove(T item)
        {
            items.Remove(item);
        }

    }
}
