using HGV.Basilius.Contants;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HGV.Basilius
{
    public class Region
    {
        public int Id { get; set; } 
        public string Key { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<int> Clusters { get; set; } = new List<int>();
    }
}
