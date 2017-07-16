using Elmah;
using IranAudioGuide_MainServer;
using System.Collections;

namespace DbUpdate
{
    public class CustomSqlErrorLog : SqlErrorLog

    {
        //private string _con = "Data Source=.;Initial Catalog=iranaudi_test52;Integrated Security=True";
        protected string connectionStringName;

        public CustomSqlErrorLog(IDictionary config) : base(config)
        {
            if (config == null) return;
            if (config.Count == 0) return;
            connectionStringName = GlobalPath.ConnectionStringElmah;

        }

        public override string ConnectionString
        {
            get { return GlobalPath.ConnectionStringElmah; }
        }

    }
}