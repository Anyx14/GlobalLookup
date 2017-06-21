using System;
using System.Collections.Generic;

namespace GlobalLookup
{
    public class SuperSector<T> : AbstractSector<T> where T : IGeoPlottable
    {
        protected int LatDivisions { get; set; }
        protected int LonDivisions { get; set; }

        protected AbstractSector<T>[,] sectors;

        protected SuperSector(SuperSector<T> superSector,
            double minLat, double maxLat, double minLon, double maxLon,
            int latDivisions, int lonDivisions)
            : this(minLat, maxLat, minLon, maxLon, latDivisions, lonDivisions)
        {
            SuperSector = superSector;
        }

        protected SuperSector(double minLat, double maxLat, double minLon, double maxLon, int latDivisions, int lonDivisions)
            : base(minLat, maxLat, minLon, maxLon) 
        {
            LatDivisions = latDivisions;
            LonDivisions = lonDivisions;

            sectors = new AbstractSector<T>[LatDivisions, LonDivisions];
            InitSectors();
        }

        private void InitSectors()
        {
            double latRange = MaxLat - MinLat;
            double lonRange = MaxLon - MinLon;

            for (int y = 0; y < LatDivisions; y++)
            {
                for (int x = 0; x < LonDivisions; x++)
                {
                    double minLat = this.MinLat + latRange / this.LatDivisions * y;
                    double maxLat = this.MinLat + latRange / this.LatDivisions * (y + 1);
                    double minLon = this.MinLon + lonRange / this.LonDivisions * x;
                    double maxLon = this.MinLon + lonRange / this.LonDivisions * (x + 1);

                    sectors[x, y] = new Sector<T>(this, minLat, maxLat, minLon, maxLon);
                }
            }
        }

        protected AbstractSector<T> GetSubSector(double lat, double lon)
        {
            int latIndex = (int)((lat - MinLat) / (MaxLat - MinLat) * LatDivisions);
            int lonIndex = (int)((lon - MinLon) / (MaxLon - MinLon) * LonDivisions);

            return sectors[lonIndex, latIndex];
        }

        public override void Add(T item)
        {
            GetSubSector(item.Lat, item.Lon).Add(item);
        }

        public override List<T> GetAllItems()
        {
            List<T> all = new List<T>();
            foreach (AbstractSector<T> sector in sectors)
            {
                all.AddRange(sector.GetAllItems());
            }
            return all;
        }

        public override List<T> Search(double lat, double lon, double range)
        {
            List<T> results = new List<T>();

            double angle = this.GetGlobe().GeoAngle(range);

            double latRange = MaxLat - MinLat;
            double lonRange = MaxLon - MinLon;

            int latIndexStart = (int)Math.Floor(((lat - angle) - MinLat) / (latRange / LatDivisions));
            int latIndexEnd = (int)Math.Ceiling(((lat + angle) - MinLat) / (latRange / LatDivisions));
            int lonIndexStart = (int)Math.Floor(((lon - angle) - MinLon) / (lonRange / LonDivisions));
            int lonIndexEnd = (int)Math.Ceiling(((lon + angle) - MinLon) / (lonRange / LonDivisions));

            for (int x = lonIndexStart >= 0 ? lonIndexStart : 0;
                x < lonIndexEnd && x < LonDivisions;
                x++)
            {
                for (int y = latIndexEnd >= 0 ? latIndexStart : 0;
                    y < latIndexEnd && y < LatDivisions;
                    y++)
                {
                    results.AddRange(sectors[x, y].Search(lat, lon, range));
                }
            }

            return results;
        }

        public override void Remove(T item)
        {
            GetSubSector(item.Lat, item.Lon).Remove(item);
        }

        protected void DivideSubSector(AbstractSector<T> sector, int latDivisions, int lonDivisions)
        {
            List<T> all = sector.GetAllItems();

            int latIndex, lonIndex;
            int w = sectors.GetLength(0);
            int h = sectors.GetLength(1);
            int x = 0;
            int y = 0;
            for (; x < w; x++)
            {
                for (; y < h; y++)
                {
                    if (sectors[x, y] == sector)
                    {
                        latIndex = y;
                        lonIndex = x;
                        break;
                    }
                }
            }
            if (x == w)
                throw new SectorNotFoundException();

            double latRange = MaxLat - MinLat;
            double lonRange = MaxLon - MinLon;
            double minLat = this.MinLat + latRange / this.LatDivisions * y;
            double maxLat = this.MinLat + latRange / this.LatDivisions * (y + 1);
            double minLon = this.MinLon + lonRange / this.LonDivisions * x;
            double maxLon = this.MinLon + lonRange / this.LonDivisions * (x + 1);

            sectors[x, y] = new SuperSector<T>(this, minLat, maxLat, minLon, maxLon, 
                latDivisions, lonDivisions);
        }

    }
}
