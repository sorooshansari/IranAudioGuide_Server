using IranAudioGuide_MainServer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IranAudioGuide_MainServer.Models_v2
{
    public class SendEmailForPaymentVM : Microsoft.AspNet.Identity.IdentityMessage
    {

        [Required]
        public string Price { get; set; }

        [Required]
        public string Pac_Name { get; set; }

        [Required]
        public List<string> Cities { get; set; }

        public DateTime Date { get; set; }

        //[Required]
        //public string Lang { get; set; }
        [Required]
        public string ReferenceID { get; set; }
    }


    public class GetUpdateInfoVm
    {
        [Required]
        public string LastUpdateNumber { get; set; }
        [Required]
        public string uuid { get; set; }
    }
    public class ConfirmEmailVM
    {
        public int lang { get; set; }

        public string email { get; set; }
    }
    public class GetAudioUrlRes
    {
        public GetAudioUrlRes(string url, bool isError = false)
        {
            this.url = url;
            if (isError)
                this.status = GetAudioUrlStatus.unauthorizedUser;
            else
                this.status = GetAudioUrlStatus.success;
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


    public class InfoUser
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string uuid { get; set; }
    }
    public class citiesLng
    {
        public int cityID { get; set; }
        public int LangId { get; set; }
        //public string LangTitle { get; set; }
    }

    public class AutorizedPlaceVm
    {
        public Guid PlaceId { get; set; }
        public int LangId { get; set; }
        //public string LangTitle { get; set; }
    }
    public class AutorizedCitiesVM
    {
        public List<citiesLng> cities { get; set; }
        //public string errorMessage { get; set; }
        public getUserStatus status { get; set; }
    }
    public class AutorizedPlacesVM
    {
        public List<AutorizedPlaceVm> places { get; set; }
        public string errorMessage { get; set; }
        public getUserStatus status { get; set; }
    }
    public class GetPackagesByLangVM
    {
        public int CityId { get; set; }
        public int LangId { get; set; }
    }
    public class GetPackagesVM
    {
        public GetPackagesVM()
        {
            errorMessage = null;
        }
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
    public class GetAllPackagesVM
    {
        public GetAllPackagesVM()
        {
            errorMessage = null;
        }
        public List<AllPackageVM> packages { get; set; }
        public List<AllCityVM> packageCities { get; set; }
        public string errorMessage { get; set; }
    }
    public class AllPackageVM
    {
        public Guid PackageId { get; set; }
        public string Name { get; set; }
        public long Price { get; set; }
        public float PriceD { get; set; }
        public int langId { get; set; }
        public int OrderItem { get; set; }
    }
    public class AllCityVM
    {
        public Guid PackageId { get; set; }
        public int CityId { get; set; }
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
        public Guid Id { get; set; }
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
        public List<Guid> TCity { get; set; }
        public List<Guid> TPlace { get; set; }
        public List<Guid> TImage { get; set; }
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
        public List<PlaceVm> Places { get; set; }
        public List<AudioVm> Audios { get; set; }
        public List<AudioVm> Stories { get; set; }
        public List<ImagesVm> Images { get; set; }
        public List<TipsVm> Tips { get; set; }
        public List<CitiesVm> Cities { get; set; }
        public List<TranslateCityVm> TranslateCities { get; set; }
        public List<TranslateImageVm> TranslateImages { get; set; }
        public List<TranslatePlacesVm> TranslatePlaces { get; set; }
        public RemovedEntries RemovedEntries { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class GetAllVm
    {
        public GetAllVm()
        {
            ErrorMessage = string.Empty;
        }
        public GetAllVm(string error)
        {
            ErrorMessage = error;
        }
        public int UpdateNumber { get; set; }
        public List<PlaceVm> Places { get; set; }
        public List<AudioVm> Audios { get; set; }
        public List<AudioVm> Stories { get; set; }
        public List<ImagesVm> Images { get; set; }
        public List<TipsVm> Tips { get; set; }
        public List<CitiesVm> Cities { get; set; }
        public List<TipCategoriesVm> TipCategories { get; set; }
        public List<TranslateCityVm> TranslateCities { get; set; }
        public List<TranslateImageVm> TranslateImages { get; set; }
        public List<TranslatePlacesVm> TranslatePlaces { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class TipCategoriesVm
    {
        public Guid ID { get; set; }
        public string Class { get; set; }
        public string Unicode { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
    }
    public class TipsVm
    {
        public Guid ID { get; set; }
        public string Content { get; set; }
        public Guid CategoryId { get; set; }
        public Guid PlaceId { get; set; }
        public int OrderItem { get; internal set; }
        public int LangId { get; internal set; }
    }
    public class ImagesVm
    {
        public Guid ID { get; set; }
        public string Name { get; internal set; }
        //public string Url { get; set; }
        //public string Desc { get; set; }
        public Guid PlaceId { get; set; }
        public int OrderItem { get; internal set; }
    }
    public class CitiesVm
    {
        public int Id { get; set; }
        //public string Name { get; set; }
        public string ImageUrl { get; set; }
        //public string Desc { get; set; }
        public int OrderItem { get; internal set; }
        //  public int LangId { get; internal set; }
    }
    public class AudioVm
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        // public string Url { get; set; }
        public string Desc { get; set; }
        public Guid PlaceId { get; set; }
        public int OrderItem { get; set; }
        public int LangId { get; set; }
    }
    public class PlaceBasicInfo
    {
        public Guid PlaceId { get; set; }
        public int UpdateNum { get; set; }
    }

    public class PlaceVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImgUrl { get; set; }
        public string TNImgUrl { get; set; }
        public string Desc { get; set; } //Descript
        public double CX { get; set; }
        public double CY { get; set; }
        public string Address { get; set; } //Addres
        public int CityId { get; set; }
        public int UpdateNumber { get; set; }
        public bool isPrimary { get; set; }
        public int OrderItem { get; set; }

    }
    public class TranslateCityVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public int CityId { get; set; }
        public int LangId { get; set; }

    }
    public class TranslateImageVm
    {
        public Guid Id { get; set; }
        // public string Name { get; set; }

        public string Description { get; set; }
        public Guid ImageId { get; set; }
        public int LangId { get; set; }
    }
    public class TranslatePlacesVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Adr { get; set; }
        public int LangId { get; set; }
        public Guid PlaceId { get; set; }

    }
}