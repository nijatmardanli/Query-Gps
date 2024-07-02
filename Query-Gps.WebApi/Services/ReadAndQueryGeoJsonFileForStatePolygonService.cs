using NetTopologySuite;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Query_Gps.WebApi.Domain;
using Query_Gps.WebApi.Repositories.Abstract;
using System.Text;

namespace Query_Gps.WebApi.Services
{
    public class mywktclas
    {
        [JsonProperty]
        public string? stname;
        [JsonProperty]
        public string? wktstr;
    }

    public class ReadAndQueryGeoJsonFileForStatePolygonService
    {
        private Dictionary<string, Geometry> _mk = default!;

        private double _minX;
        private double _maxX;
        private double _minY;
        private double _maxY;
        private Geometry? _geometry;
        private Random? _random;
        private GeometryFactory? _geometryFactory;
        private readonly IRegionRepository _regionRepository;

        public ReadAndQueryGeoJsonFileForStatePolygonService(IRegionRepository regionRepository)
        {
            _regionRepository = regionRepository;
        }

        public async Task ReadGeoJsonDataWithDistrictAsync()
        {
            StringBuilder sb = new();
            _geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            Dictionary<string, string> geometrystringdict = new();
            GeometryFactory geometryFactory1 = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            // get the JSON file content
            string filename = @"E:\Downloads\mygeodata_merged\mygeodata_merged.json";
            //filename = @"D:\India_MapData\FinalMap_used\TestingDistrict\mygeodata_merged.json";
            var jsonString = File.ReadAllText(filename);
            var jsonData = JObject.Parse(jsonString)!;
            var features = jsonData["features"]!;

            // create NetTopology JSON reader
            var reader = new GeoJsonReader();

            // pass geoJson's FeatureCollection to read all the features
            var featureCollection = reader.Read<FeatureCollection>(jsonString);

            // if feature collection is null then return
            if (featureCollection == null)
            {
                return;
            }

            var coll = featureCollection.GetEnumerator();
            List<mywktclas> mywktlist = new();
            _mk = new Dictionary<string, Geometry>();
            while (coll.MoveNext())
            {
                var item = coll.Current;
                var pk = item.Geometry;

                string? state = item.Attributes["st_nm"].ToString();
                string? district = item.Attributes["district"].ToString();

                if (state == null || district == null || state.Trim().Length == 0 || district.Trim().Length == 0)
                {
                    Console.WriteLine("issue");
                }

                string key = district + "@" + state;
                var value = pk.Coordinates;
                var feature = features.FirstOrDefault(x => x["properties"]!["st_nm"]!.ToString() == state 
                                && x["properties"]!["district"]!.ToString() == district);

                var coordinates = feature!["geometry"]!["coordinates"]!.ToArray();


                var regionValue = new
                {
                    Type = "Feature",
                    Geometry = new
                    {
                        Type = pk.GeometryType,
                        Coordinates = coordinates
                    }
                };

                Region region = new()
                {
                    Key = key,
                    Value = JsonConvert.SerializeObject(regionValue, new JsonSerializerSettings()
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    })
                };

                try
                {
                    await _regionRepository.UpsertAsync(region);
                }
                catch (Exception ex)
                {
                    throw;
                }

                sb.AppendLine(key);
                _mk[key] = pk;
                Console.WriteLine(key);
            }

            File.WriteAllText(@"E:\Downloads\mygeodata_merged\allindiadistrict.txt", sb.ToString());
        }

        public void ReadGeoJsonData(string filename)
        {
            _geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            // get the JSON file content
            // Log.Information("file name " + filename);
            var josnData = File.ReadAllText(filename);

            // create NetTopology JSON reader
            var reader = new GeoJsonReader();

            // pass geoJson's FeatureCollection to read all the features
            var featureCollection = reader.Read<FeatureCollection>(josnData);

            // if feature collection is null then return
            if (featureCollection == null)
            {
                return;
            }
            //  Log.Information("found features ");

            var coll = featureCollection.GetEnumerator();
            _mk = new Dictionary<string, Geometry>();
            while (coll.MoveNext())
            {
                var item = coll.Current;
                var pk = item.Geometry;
                string[] attributelist = item.Attributes.GetNames();

                string key = "NOTKNOWN";
                foreach (string s1 in attributelist)
                {
                    if (s1 == "st_nm")
                    {
                        key = item.Attributes["st_nm"].ToString()!;
                        break;
                    }
                    else if (s1 == "NAME_1")
                    {
                        key = item.Attributes["NAME_1"].ToString()!;
                        break;
                    }
                    else if (s1 == "shapeName")
                    {
                        key = item.Attributes["shapeName"].ToString()!;

                        break;
                    }
                    else if (s1 == "name")
                    {
                        key = item.Attributes["name"].ToString()!;

                        break;
                    }
                }

                _mk[key] = pk;
            }

            // Log.Information("number of entry " + mk.Count);
        }

        public List<string> getStateList(double latitude, double longitude)
        {
            Point point = _geometryFactory!.CreatePoint(new Coordinate(longitude, latitude));
            List<string> result = new();

            foreach (string key in _mk.Keys)
            {
                Geometry shape = _mk[key];

                if (shape.Contains(point))
                {
                    Console.WriteLine("Found match " + key);
                    result.Add(key);
                }
            }
            foreach (string key in _mk.Keys)
            {
                Console.WriteLine(key);
            }
            return result;
        }

        public void setPointBound()
        {
            _random = new Random();
            _geometry = _mk["Haryana"];

            var gh = _geometry.EnvelopeInternal;
            _minX = gh.MinX;
            _maxX = gh.MaxX;
            _minY = gh.MinY;
            _maxY = gh.MaxY;
        }

        public List<double> getRandomPoint()
        {
            bool notfoundinside = true;
            List<double> ld = new List<double>();
            int mcount = 0;
            while (notfoundinside)
            {
                double newx = _random!.NextDouble() * (_maxX - _minX) + _minX;
                double newy = _random!.NextDouble() * (_maxY - _minY) + _minY;

                Point point = _geometryFactory!.CreatePoint(new Coordinate(newx, newy));
                List<string> result = new List<string>();
                mcount = mcount + 1;

                if (_geometry!.Contains(point))
                {
                    ld.Add(newx);
                    ld.Add(newy);
                    notfoundinside = false;
                }
                if (mcount == 100000)
                {
                    notfoundinside = false;
                }
            }

            return ld;
        }
    }
}