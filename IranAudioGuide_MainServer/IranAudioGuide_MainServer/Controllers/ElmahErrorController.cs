using IranAudioGuide_MainServer.Services;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;

namespace IranAudioGuide_MainServer.Controllers
{
    public class ElmahErrorController : ApiController
    {
        // GET: api/ElmahError
        public IHttpActionResult Get()
        {
           return GetError();
        }

        // GET: api/ElmahError/5
        public IHttpActionResult Get(int id)
        {
            return GetError(id);
        }
        private IHttpActionResult GetError(int number = 0)
        {
            if (number == 0)
                number = -1;
            if (number > 0)
                number = (-1) * number;

            using (SqlConnection sqlConnection = new SqlConnection(GlobalPath.ConnectionStringElmah))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("GetError", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@BeforDay", number));
                    sqlConnection.Open();
                    var reader = cmd.ExecuteReader();
                    var dt1 = new DataTable();
                    dt1.Load(reader);
                    var list = dt1.AsEnumerable().Select(x => new
                    {
                        ErrorId = x["ErrorId"].convertToGuid(),
                        Application = x["Application"].ToString(),
                        Host = x["Host"].ToString(),
                        Type = x["Type"].ToString(),
                        Source = x["Source"].ToString(),
                        Message = x["Message"].ToString(),
                        User = x["User"].ToString(),
                        StatusCode = x["StatusCode"].ToString(),
                        TimeUtc = x["TimeUtc"].convertToString(),
                        Sequence = x["Sequence"].ToString(),
                    //    AllXml = x["AllXml"].ToString()
                    }).ToList();
                    return Ok(list);

                }
                catch (System.Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                finally
                {
                    sqlConnection.Close();

                }
            }


        }
    }
}
