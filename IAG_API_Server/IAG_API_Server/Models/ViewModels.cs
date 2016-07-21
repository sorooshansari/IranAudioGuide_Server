using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IAG_API_Server.Models
{
    public class ViewModels
    {
    }
    public class GetAllVM
    {
        public List<PlaceFullInfo> Places { get; set; }
        public List<AudioFullInfo> Audios { get; set; }
        public List<CitiesFullInfno> Cities { get; set; }
        public List<ImagesFullInfno> Images { get; set; }
        public int UpdateNumber { get; set; }
    }
    public class ImagesFullInfno
    {
        public Guid ID { get; set; }
        public string Url { get; set; }
        public string Desc { get; set; }
        public Guid PlaceId { get; set; }
    }
    public class CitiesFullInfno
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class AudioFullInfo
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Desc { get; set; }
        public Guid PlaceId { get; set; }
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
        public string ImgUrl { get; set; }
        public string TNImgUrl { get; set; }
        public string Desc { get; set; }
        public double CX { get; set; }
        public double CY { get; set; }
        public string Address { get; set; }
        public int CityId { get; set; }
        public int UpdateNumber { get; set; }
    }
}