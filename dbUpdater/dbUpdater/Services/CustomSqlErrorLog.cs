using System.Collections;

namespace dbUpdater.Services
{
    public class CustomSqlErrorLog : Elmah.SqlErrorLog

    {
        //private string _con = "Data Source=.;Initial Catalog=iranaudi_test52;Integrated Security=True";
        protected string connectionStringName;

        public CustomSqlErrorLog(IDictionary config) : base(config)
        {
            connectionStringName = (string)config["connectionStringName"];

        }

        public override string ConnectionString
        {
            get { return GlobalPath.ConnectionStringElmah; }
        }

    }
}