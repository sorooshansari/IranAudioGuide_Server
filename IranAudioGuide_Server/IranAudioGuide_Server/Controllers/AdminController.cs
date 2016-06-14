using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IranAudioGuide_Server.Models;
using System.IO;
using System.Data.SqlClient;

namespace IranAudioGuide_Server.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private const int pagingLen = 10;

        // GET: Admin
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            ViewBag.View = Views.AdminIndex;

            return View(new AdminIndexVM()
            {
                AdminInfo = GetCurrentUserInfo(),
                Places = GetPlaces(),
                Cities = GetCities()
            });
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Audios(string PlaceId)
        {
            if (PlaceId.Length > 0)
            {
                string imgUrl = PlaceImg(PlaceId);
                if (imgUrl != null)
                {
                    return View(new AudioViewVM()
                    {
                        audios = GetAudios(PlaceId),
                        PlaceImage = imgUrl.Remove(0, 1)
                    });
                }
                return Content("");
            }
            return Content("");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult AddPlace(AdminIndexVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }
            if (model.NewPlace.Image.ContentLength > 0 && IsImage(model.NewPlace.Image))
            {
                var place = new Place()
                {
                    Pla_Name = model.NewPlace.PlaceName,
                    Pla_Discription = model.NewPlace.PlaceDesc,
                    Pla_Address = model.NewPlace.PlaceAddress
                };
                if (model.NewPlace.PlaceCordinates != null)
                {
                    if (!model.NewPlace.PlaceCordinates.Contains(','))
                    {
                        ModelState.AddModelError("", "Enter X and Y cordinates and seprate them whith \",\".");
                        return View("Index", model);
                    }
                    try
                    {
                        List<double> cordinates = (from c in model.NewPlace.PlaceCordinates.Split(',')
                                                   select Convert.ToDouble(c)).ToList();
                        place.Pla_cordinate_X = cordinates[0];
                        place.Pla_cordinate_Y = cordinates[1];
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", "Enter percise cordinates.");
                        return View("Index", model);
                    }
                }

                try
                {
                    using (var dbTran = db.Database.BeginTransaction())
                    {
                        place.Pla_city = db.Cities.Where(c => c.Cit_Id == model.NewPlace.PlaceCityId).FirstOrDefault();
                        db.Places.Add(place);
                        db.SaveChanges(); //Save place and generate Pla_Id
                        string id = Convert.ToString(place.Pla_Id);
                        string extention = Path.GetExtension(model.NewPlace.Image.FileName);
                        string path = string.Format("~/images/Places/{0}{1}", id, extention);
                        model.NewPlace.Image.SaveAs(Server.MapPath(path));
                        place.Pla_ImgUrl = path;
                        db.SaveChanges();
                        dbTran.Commit();
                    }
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            ModelState.AddModelError("", "Only jpg, png, gif, and jpeg are allowed.");
            return View("Index", model);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult AddCity(NewCity model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new OK(false));
            }
            var city = new city() { Cit_Name = model.CityName, Cit_Description = model.CityDesc };
            try
            {
                using (var dbTran = db.Database.BeginTransaction())
                {
                    db.Cities.Add(city);
                    db.SaveChanges();
                    dbTran.Commit();
                }
                return Json(new OK());
            }
            catch (Exception)
            {
                return Json(new OK(false));
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult DelCity(int Id)
        {
            int result = db.Database.SqlQuery<int>("DeleteCity @Id", new SqlParameter("@Id", Id)).Single();
            return Json(new Respond("", result));
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult DelPlace(System.Guid Id)
        {
            int result = db.Database.SqlQuery<int>("DeletePlace @Id", new SqlParameter("@Id", Id)).Single();
            return Json(new Respond("", result));
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult GetPlaces(int PageNum)
        {
            var places = GetPlaces();
            int pagesLen = (places.Count() % 10 == 0) ? places.Count() : places.Count() + 1;
            return Json(new GetPlacesVM(places.GetRange(PageNum * pagingLen, pagingLen), pagesLen));
        }
        private List<PlaceVM> GetPlaces()
        {
            try
            {

                List<PlaceVM> Places = (from place in db.Places
                                        orderby place.Pla_Id descending
                                        select new PlaceVM()
                                        {
                                            PlaceId = place.Pla_Id,
                                            ImgUrl = place.Pla_ImgUrl.Remove(0, 1),
                                            PlaceDesc = place.Pla_Discription,
                                            PlaceName = place.Pla_Name,
                                            Audios = (from audio in db.Audios
                                                      where audio.Pla_Id == place
                                                      select new AudioVM()
                                                      {
                                                          Aud_Discription = audio.Aud_Discription,
                                                          Aud_Id = audio.Aud_Id,
                                                          Aud_Name = audio.Aud_Name,
                                                          Aud_Url = audio.Aud_Url.Remove(0, 1)
                                                      }).ToList(),
                                            CityName = (from c in db.Cities
                                                        where c == place.Pla_city
                                                        select c.Cit_Name).FirstOrDefault()
                                        }).ToList();
                int counter = 0;
                foreach (var item in Places)
                {
                    item.Index = ++counter;
                }
                return Places;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private string PlaceImg(string placeId)
        {
            try
            {
                Place place = db.Places.Where(x => x.Pla_Id.ToString() == placeId).FirstOrDefault();
                if (place != default(Place))
                {
                    return place.Pla_ImgUrl;
                }
                return null;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private UserInfo GetCurrentUserInfo()
        {
            try
            {
                string userName = User.Identity.Name;
                UserInfo Info = (from user in db.Users
                                 where user.UserName == userName
                                 select new UserInfo()
                                 {
                                     Email = user.Email,
                                     FullName = user.FullName,
                                     imgUrl = user.ImgUrl
                                 }).FirstOrDefault();
                return Info;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private List<AudioVM> GetAudios(string PlaceId)
        {
            try
            {
                List<AudioVM> audios = (from a in db.Audios
                                        where a.Pla_Id == db.Places.Where(x => x.Pla_Id.ToString() == PlaceId).FirstOrDefault()
                                        select new AudioVM()
                                        {
                                            Aud_Discription = a.Aud_Discription,
                                            Aud_Name = a.Aud_Name,
                                            Aud_Url = a.Aud_Url,
                                            Aud_Id = a.Aud_Id
                                        }).ToList();
                return audios;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private List<CityVM> GetCities()
        {
            try
            {
                List<CityVM> cities = (from c in db.Cities
                                       orderby c.Cit_Id descending
                                       select new CityVM()
                                       {
                                           CityDesc = c.Cit_Description,
                                           CityID = c.Cit_Id,
                                           CityName = c.Cit_Name
                                       }).ToList();
                int counter = 0;
                foreach (var item in cities)
                {
                    item.Index = ++counter;
                }
                return cities;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private bool IsImage(HttpPostedFileBase file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" };

            // linq from Henrik Stenbæk
            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}