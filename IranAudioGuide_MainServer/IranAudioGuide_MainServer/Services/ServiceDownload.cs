using IranAudioGuide_MainServer.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


namespace IranAudioGuide_MainServer.Services
{
    public class ServiceDownload
    {

        public static string GetUrl(GetAudioUrlVM model, bool isAdmin = false)
        {

            string FullSource, pathDestination;
            if (model.isAudio)
            {

                FullSource = GlobalPath.PrimaryPathAudios;
                pathDestination = GlobalPath.DownloadPathAudios;
            }
            else
            {
                FullSource = GlobalPath.PrimaryPathStory;
                pathDestination = GlobalPath.DownloadPathStory;

            }
            using (SqlConnection sqlConnection = new SqlConnection(GlobalPath.ConnectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("Download_Link_GetURL", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@IsAudio", model.isAudio));
                    cmd.Parameters.Add(new SqlParameter("@IsAdmin", isAdmin));
                    cmd.Parameters.Add(new SqlParameter("@FileId", model.trackId));
                    cmd.Parameters.Add(new SqlParameter("@UserName", model.email));
                    cmd.Parameters.Add(new SqlParameter("@Path", pathDestination)); ;
                    cmd.Parameters.Add(new SqlParameter("@UserUUID", model.uuid));

                    sqlConnection.Open();
                    var reader = cmd.ExecuteReader();
                    var dt1 = new DataTable();
                    dt1.Load(reader);
                    var links = dt1.AsEnumerable().Select(x => new
                    {
                        pathDestination = x["PathFile"].ToString(),
                        FileName = x["FileName"].ToString(),
                        IsUpdate = x["IsUpdate"].ToString(),
                        IsAccess = x["isAccess"].ToString(),
                        IdDownload = x["IdDownload"].ToString(),
                        langId = x["langId"].ToString()
                    }).FirstOrDefault();

                    if (links.IsAccess == "0")
                        return null;

                    var returnUrl = GlobalPath.host + links.pathDestination;
                    if (links.IsUpdate == "False")
                        return returnUrl;

                    var ftp = new ServiceFtp();

                    var result = ftp.Copy("ftp://lnx1.morvahost.com" + FullSource + links.FileName, GlobalPath.hostFtp + links.pathDestination);

                    if (result)
                        return returnUrl;


                    // if Create Link download eror   DownloadLink  shoud be remove anf return source file
                    SqlCommand cmdRemove = new SqlCommand("Download_Link_Delete", sqlConnection);
                    cmdRemove.CommandType = CommandType.StoredProcedure;
                    cmdRemove.Parameters.Add(new SqlParameter("@id", links.IdDownload));
                    cmdRemove.ExecuteReader();
                    return GlobalPath.host + FullSource + links.FileName;


                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    return string.Format("{0}{1}{2}.mp3", GlobalPath.host, FullSource, model.trackId);
                }
                finally
                {
                    sqlConnection.Close();

                }
            }

        }
    }
}