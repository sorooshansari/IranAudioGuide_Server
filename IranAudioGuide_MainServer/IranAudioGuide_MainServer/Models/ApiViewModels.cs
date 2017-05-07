using IranAudioGuide_MainServer.Models;
using System;
using System.Collections.Generic;


//use in mobail

namespace IranAudioGuide_MainServer.Models_v1
{
    public class ApiViewModels
    {
    }
    public class ConfirmEmailVM
    {
        public string email { get; set; }
    }
    public class GetAudioUrlRes
    {
        public GetAudioUrlRes(string url, bool isError = false)
        {
            this.url = url;
            if (isError)
                status = GetAudioUrlStatus.unauthorizedUser;
            else
                status = GetAudioUrlStatus.success;
        }
        public GetAudioUrlRes(GetAudioUrlStatus status, string error = "")
        {
            this.status = status;
            this.errorMessage = error;
        }
        public GetAudioUrlStatus status { get; set; }
        public string url { get; set; }
        public string errorMessage { get; set; }
    }
    public class GetAutorizedCitiesVM
    {
        public string username { get; set; }
        public string uuid { get; set; }
    }
    public class AutorizedCitiesVM
    {
        public List<int> cities { get; set; }
        public string errorMessage { get; set; }
        public getUserStatus status { get; set; }
    }
    //public class GetPackagesVM
    //{
    //    public List<PackageVM> packages { get; set; }
    //    public string errorMessage { get; set; }
    //}
    public class GetPackagesVM
    {
        public List<ApiPackageVM> packages { get; set; }
        public List<ApiCitInfoVM> cities { get; set; }
        public string errorMessage { get; set; }
        public bool IsForeign { get; set; }
    }
    public class ApiPackageVM
    {
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public long Price { get; set; }
        public float PriceD { get; set; }
    }
    public class ApiCitInfoVM
    {
        public Guid PackageId { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int PlacesCount { get; set; }
        public int AudiosCount { get; set; }
        public int StoriesCount { get; set; }
    }
    public class ApiCityVM
    {
        public System.Guid Id { get; set; }
        public string CityName { get; set; }
        public int PlacesCount { get; set; }
        public int AudiosCount { get; set; }
        public int StoriesCount { get; set; }
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
    //public class GetUpdateVM_v2
    //{
    //    public GetUpdateVM_v2()
    //    {
    //        ErrorMessage = string.Empty;
    //    }
    //    public GetUpdateVM_v2(string error)
    //    {
    //        ErrorMessage = error;
    //    }
    //    public int UpdateNumber { get; set; }
    //    public List<PlaceFullInfo> Places { get; set; }
    //    public List<AudioFullInfo> Audios { get; set; }
    //    public List<AudioFullInfo> Stories { get; set; }
    //    public List<ImagesFullInfno> Images { get; set; }
    //    public List<TipsFullInfo> Tips { get; set; }
    //    public List<CitiesFullInfno> Cities { get; set; }
    //    public RemovedEntries RemovedEntries { get; set; }
    //    public string ErrorMessage { get; set; }
    //}
 
    public class GetUpdateVM
    {
        public GetUpdateVM()
        {
            ErrorMessage = string.Empty;
        }
        public GetUpdateVM(string error)
        {
            ErrorMessage = error;
        }
        public int UpdateNumber { get; set; }
        public List<PlaceFullInfo> Places { get; set; }
        public List<AudioFullInfo> Audios { get; set; }
        public List<AudioFullInfo> Stories { get; set; }
        public List<ImagesFullInfno> Images { get; set; }
        public List<TipsFullInfo> Tips { get; set; }
        public List<CitiesFullInfno> Cities { get; set; }
        public RemovedEntries RemovedEntries { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class GetAllVM
    {
        public GetAllVM()
        {
            ErrorMessage = string.Empty;
        }
        public GetAllVM(string error)
        {
            ErrorMessage = error;
        }
        public int UpdateNumber { get; set; }
        public List<PlaceFullInfo> Places { get; set; }
        public List<AudioFullInfo> Audios { get; set; }
        public List<AudioFullInfo> Stories { get; set; }
        public List<ImagesFullInfno> Images { get; set; }
        public List<TipsFullInfo> Tips { get; set; }
        public List<CitiesFullInfno> Cities { get; set; }
        public List<TipCategoriesFullInfo> TipCategories { get; set; }
        public string ErrorMessage { get; set; }
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
        public string ImageUrl { get; set; }
        public string Desc { get; set; }
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