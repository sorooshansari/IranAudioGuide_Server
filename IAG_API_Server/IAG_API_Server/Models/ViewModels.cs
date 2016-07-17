using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IAG_API_Server.Models
{
    public class ViewModels
    {
    }
    public class PlaceBasicInfo
    {
        public Guid PlaceId { get; set; }
        public int UpdateNum { get; set; }
    }
    public class PlaceFullInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string imgUrl { get; set; }
        public string desc { get; set; }
        public float c_x { get; set; }
        public float C_y { get; set; }
        public string address { get; set; }
        public string CityName { get; set; }
        public int UpdateNumber { get; set; }
    }
}