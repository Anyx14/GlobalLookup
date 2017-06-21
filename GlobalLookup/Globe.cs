using System;
using System.Collections.Generic;

namespace GlobalLookup
{
    public class Globe<T> : SuperSector<T> where T : IGeoPlottable
    {
        public const double RADIUS_OF_EARTH = 6371000; // meters
        public const double LAT_DEGREES = 180;
        public const double LON_DEGREES = 360;
        
        protected override SuperSector<T> SuperSector
        {
            get { return null; }
        }

        public double Radius { get; set; }

        public Globe()
            : this(RADIUS_OF_EARTH) { }

        public Globe(double radius)
            : this(radius, (int)LAT_DEGREES / 18, (int)LON_DEGREES / 36) { }

        public Globe(double radius, int latDivisions, int lonDivisions)
            : base(-LAT_DEGREES / 2, LAT_DEGREES / 2, -LON_DEGREES / 2, LON_DEGREES / 2, latDivisions, lonDivisions)
        {
            this.Radius = radius;
            this.LatDivisions = latDivisions;
            this.LonDivisions = lonDivisions;
        }

        public override Globe<T> GetGlobe()
        {
            return this;
        }

        /// <summary>
        /// Determine the distance between two points on the globe using the haversine formula. 
        /// Distance returned will be in the same units as the radius of the globe.
        /// </summary>
        /// <param name="latA">Latitude of point A</param>
        /// <param name="lonA">Longitude of point A</param>
        /// <param name="latB">Latitude of point B</param>
        /// <param name="lonB">Longitude of point B</param>
        /// <returns>Distance between the two points</returns>
        public double GeoDistance(double latA, double lonA, double latB, double lonB)
        {
            // Convert lat/lon to radians, get difference
            double dlat = DegreeToRadian(latB) - DegreeToRadian(latA);
            double dlon = DegreeToRadian(lonB) - DegreeToRadian(lonA);

            // Haversine formula stuffs
            double a = Math.Pow(Math.Sin(dlat / 2), 2)
                + Math.Cos(latA)
                * Math.Cos(latB)
                * Math.Pow(Math.Sin(dlon / 2), 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = Radius * c;

            double x = Math.Sqrt(a);
            double y = Math.Sqrt(1 - a);
            double z = Math.Atan2(x, y);

            return d;
        }

        static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public double GeoAngle(double distance)
        {
            return 180.0 / (Math.PI * Radius) * distance;
        }

        public override List<T> Search(double lat, double lon, double range)
        {
            List<T> results = new List<T>();

            double angle = GeoAngle(range);

            double latRange = MaxLat - MinLat;
            double lonRange = MaxLon - MinLon;

            int latIndexStart = (int)Math.Floor(((lat - angle) - MinLat) / (latRange / LatDivisions));
            int latIndexEnd = (int)Math.Ceiling(((lat + angle) - MinLat) / (latRange / LatDivisions));
            int lonIndexStart = (int)Math.Floor(((lon - angle) - MinLon) / (lonRange / LonDivisions));
            int lonIndexEnd = (int)Math.Ceiling(((lon + angle) - MinLon) / (lonRange / LonDivisions));

            for (int x = lonIndexStart; x < lonIndexEnd; x++)
            {
                for (int y = latIndexStart; y < latIndexEnd; y++)
                {
                    results.AddRange(sectors[x % LonDivisions, y % LatDivisions].Search(lat, lon, range));
                }
            }

            return results;
        }

    }
}
