using System;
using Quartz;
using Quartz.Impl;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using IranAudioGuide_MainServer.Services;

namespace DbUpdate
{

    public class JobDownloadLinkDisable : IJob
    {

        public void Execute(IJobExecutionContext context)
        {
            //System.Diagnostics.Debug.WriteLine("job Disable:" + DateTime.Now.ToString());
            try
            {
                ServiceSqlServer.RunStoredProc("Download_LinkDisable");
            }
            catch (Exception ex)
            {
                EmailServiceForJob.SendEmail(ex.Message);
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
            }
        }
    }
    public class JobDownloadLinkRemove : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            //System.Diagnostics.Debug.WriteLine("job Remove:" + DateTime.Now.ToString());
            var serviceftp = new ServiceFtpForJob();
            try
            {
                var dt = ServiceSqlServer.RunStoredProc("Download_GetPathForDelete", true);
                var links = dt.AsEnumerable().Select(x => new
                {
                    Path = x["Path"].ConvertToString(),
                    DowId = x["DowId"].ConvertToGuid(),
                }).ToList();
                foreach (var item in links)
                {
                    var isremove = serviceftp.delete(item.Path);
                    if (isremove && item.DowId != Guid.Empty)
                    {
                        var SP = new SqlParameter("@Id", item.DowId);
                        SP.SqlDbType = SqlDbType.UniqueIdentifier;
                        ServiceSqlServer.RunStoredProc("Download_Link_Remove_ById", false, SP);
                    }
                }
            }
            catch (Exception ex)
            {
                EmailServiceForJob.SendEmail(ex.Message);
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
            }
        }
    }
    public interface ISchedule
    {
        void Run();
    }
    public class ScheduleDownloadLink : ISchedule
    {
        public void Run()
        {

            DateTimeOffset startTime = DateBuilder.FutureDate(1, IntervalUnit.Minute);

            IJobDetail job = JobBuilder.Create<JobDownloadLinkDisable>()
                                       .WithIdentity("jobDownloadLinkDisable")
                                       .Build();

            ITrigger trigger = TriggerBuilder.Create()
                                             .WithIdentity("triggerDownloadLinkDisable")
                                             .StartAt(startTime)
                                             .WithSimpleSchedule(x => x.WithIntervalInMinutes(11).RepeatForever())
                                             .Build();


            ISchedulerFactory sfDownloadLinkDisable = new StdSchedulerFactory();
            IScheduler sc = sfDownloadLinkDisable.GetScheduler();
            sc.ScheduleJob(job, trigger);
            sc.Start();



            DateTimeOffset startTimeRemove = DateBuilder.FutureDate(8, IntervalUnit.Minute);

            IJobDetail jobRemove = JobBuilder.Create<JobDownloadLinkRemove>()
                                       .WithIdentity("jobDownloadLinkRemove")
                                       .Build();

            ITrigger triggerRemove = TriggerBuilder.Create()
                                             .WithIdentity("triggerDownloadLinkRemove")
                                             .StartAt(startTimeRemove)
                                             .WithSimpleSchedule(x => x.WithIntervalInMinutes(14).RepeatForever())
                                             .Build();


            ISchedulerFactory sfDownloadLinkRemove = new StdSchedulerFactory();
            IScheduler scRemove = sfDownloadLinkRemove.GetScheduler();
            scRemove.ScheduleJob(jobRemove, triggerRemove);
            scRemove.Start();


        }
    }


}