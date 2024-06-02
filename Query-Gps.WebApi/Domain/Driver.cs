using GeoJSON.Net.Geometry;
using System.Text.Json.Serialization;

namespace Query_Gps.WebApi.Domain
{
    public class Driver
    {
        public Guid Id { get; set; }

        //[JsonIgnore]
        //public Point CoordinatesPoint { get; set; } = default!;

        public Coordinates Coordinates { get; set; } = default!;

        public Vehicle Vehicle { get; set; } = default!;
    }
}
