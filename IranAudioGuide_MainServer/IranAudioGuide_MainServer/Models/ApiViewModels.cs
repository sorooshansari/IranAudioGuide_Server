using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IranAudioGuide_MainServer.Models
{
    public class ApiViewModels
    {
    }
    public class RemovedEntries
    {
        public List<Guid> Places { get; set; }
        public List<Guid> Audios { get; set; }
        public List<Guid> Stories { get; set; }
        public List<Guid> Images { get; set; }
        public List<Guid> Tips { get; set; }
        public List<int> Cities { get; set; }
    }
    public class GetUpdateVM
    {

        public int UpdateNumber { get; set; }
        public List<PlaceFullInfo> Places { get; set; }
        public List<AudioFullInfo> Audios { get; set; }
        public List<AudioFullInfo> Stories { get; set; }
        public List<ImagesFullInfno> Images { get; set; }
        public List<TipsFullInfo> Tips { get; set; }
        public List<CitiesFullInfno> Cities { get; set; }
        public RemovedEntries RemovedEntries { get; set; }
    }
    public class GetAllVM
    {
        public int UpdateNumber { get; set; }
        public List<PlaceFullInfo> Places { get; set; }
        public List<AudioFullInfo> Audios { get; set; }
        public List<AudioFullInfo> Stories { get; set; }
        public List<ImagesFullInfno> Images { get; set; }
        public List<TipsFullInfo> Tips { get; set; }
        public List<CitiesFullInfno> Cities { get; set; }
        public List<TipCategoriesFullInfo> TipCategories { get; set; }
    }
    public class TipCategoriesFullInfo
    {
        public Guid ID { get; set; }
        public string Class { get; set; }
        public string Unicode { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
    }
    public class TipsFullInfo
    {
        public Guid ID { get; set; }
        public string Content { get; set; }
        public Guid CategoryId { get; set; }
        public Guid PlaceId { get; set; }
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
        public bool isPrimary { get; set; }
    }
}