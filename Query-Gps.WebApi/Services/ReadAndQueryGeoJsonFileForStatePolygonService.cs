using NetTopologySuite;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
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
        private Dictionary<string, Geometry> mk = default!;

        private double minx;
        private double maxx;
        private double miny;
        private double maxy;
        private Geometry? reqgeo;
        private Random? r;
        private GeometryFactory? geometryFactory;
        private readonly IRegionRepository _regionRepository;

        public ReadAndQueryGeoJsonFileForStatePolygonService(IRegionRepository regionRepository)
        {
            _regionRepository = regionRepository;
        }

        public async Task ReadGeoJsonDataWithDistrictAsync()
        {
            StringBuilder sb = new();
            geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            Dictionary<string, string> geometrystringdict = new();
            GeometryFactory geometryFactory1 = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            // get the JSON file content
            string filename = @"E:\Downloads\mygeodata_merged\mygeodata_merged.json";
            //filename = @"D:\India_MapData\FinalMap_used\TestingDistrict\mygeodata_merged.json";
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
            var coll = featureCollection.GetEnumerator();
            List<mywktclas> mywktlist = new();
            mk = new Dictionary<string, Geometry>();
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

                dynamic regions = pk.GeometryType == "Polygon"
                    ? new List<double[][]>
                {
                    value.Select(x=> new double[] {x.X, x.Y}).ToArray()
                }
                    : new List<double[][][]>() { new double[][][] { value.Select(x => new double[] { x.X, x.Y }).ToArray() } };

                var regionValue = new
                {
                    Type = pk.GeometryType,
                    Coordinates = regions
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
                mk[key] = pk;
                Console.WriteLine(key);
            }

            File.WriteAllText(@"E:\Downloads\mygeodata_merged\allindiadistrict.txt", sb.ToString());
        }

        public void ReadGeoJsonData(string filename)
        {
            geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
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
            mk = new Dictionary<string, Geometry>();
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

                mk[key] = pk;
            }

            // Log.Information("number of entry " + mk.Count);
        }

        public List<string> getStateList(double latitude, double longitude)
        {
            Point point = geometryFactory!.CreatePoint(new Coordinate(longitude, latitude));
            List<string> result = new();

            foreach (string key in mk.Keys)
            {
                Geometry shape = mk[key];

                if (shape.Contains(point))
                {
                    Console.WriteLine("Found match " + key);
                    result.Add(key);
                }
            }
            foreach (string key in mk.Keys)
            {
                Console.WriteLine(key);
            }
            return result;
        }

        public void setPointBound()
        {
            r = new Random();
            reqgeo = mk["Haryana"];

            var gh = reqgeo.EnvelopeInternal;
            minx = gh.MinX;
            maxx = gh.MaxX;
            miny = gh.MinY;
            maxy = gh.MaxY;
        }

        public List<double> getRandomPoint()
        {
            bool notfoundinside = true;
            List<double> ld = new List<double>();
            int mcount = 0;
            while (notfoundinside)
            {
                double newx = r!.NextDouble() * (maxx - minx) + minx;
                double newy = r!.NextDouble() * (maxy - miny) + miny;

                Point point = geometryFactory!.CreatePoint(new Coordinate(newx, newy));
                List<string> result = new List<string>();
                mcount = mcount + 1;

                if (reqgeo!.Contains(point))
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