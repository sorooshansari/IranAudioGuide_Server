//using System;
//using Quartz;
//using Quartz.Impl;
//using IranAudioGuide_MainServer.Models;
//using System.Linq;

//namespace IranAudioGuide_MainServer.Services
//{
//    public class JobDownloadLinkDisable : IJob
//    {
//        private bool CheckDate(DateTime startDay)
//        {
//            var duringTimes = 10;

//            var endDay = DateTime.Now;
//            var m = (endDay.Hour * 60 + endDay.Minute) - (startDay.Hour * 60 + startDay.Minute);
//            if (m > duringTimes)
//                return true;
//            else if ((m < 0) & ((60 - endDay.Minute) + startDay.Minute > duringTimes))
//                return true;
//            else
//                return false;
           
//        }
//        public void Execute(IJobExecutionContext context)
//        {
//            //System.Diagnostics.Debug.WriteLine("job Disable:" + DateTime.Now.ToString());
//            using (var db = new ApplicationDbContext())
//            {
//                using (var dbTran = db.Database.BeginTransaction())
//                {
//                    try
//                    {
//                        var links = db.DownloadLinks.AsEnumerable().Where(x => CheckDate(x.TimeToVisit)).ToList();
//                        if (links == null || links.Count == 0)
//                            return;
//                        foreach (var item in links)
//                        {
//                            item.IsDisable = true;
//                        }

//                        db.SaveChanges();
//                        dbTran.Commit();
//                    }
//                    catch (Exception ex)
//                    {
//                        System.Diagnostics.Debug.WriteLine("Exception Job Edite Download link.");
//                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
//                        dbTran.Rollback();
//                    }
//                }
//            }
//        }


//    }
//    public class JobDownloadLinkRemove : IJob
//    {

//        public void Execute(IJobExecutionContext context)
//        {
//            //System.Diagnostics.Debug.WriteLine("job Remove:" + DateTime.Now.ToString());
//            var serviceftp = new ServiceFtp();
//            using (var db = new ApplicationDbContext())
//            {
//                using (var dbTran = db.Database.BeginTransaction())
//                {
//                    try
//                    {
//                        var links = db.DownloadLinks.Where(x => x.IsDisable == true).ToList();
//                        if (links == null || links.Count == 0 )
//                            return;
//                        foreach (var link in links)
//                        {
//                            serviceftp.delete(link.Path);

//                        }
//                        db.DownloadLinks.RemoveRange(links);
//                        db.SaveChanges();
//                        dbTran.Commit();
//                    }
//                    catch (Exception ex)
//                    {
//                        System.Diagnostics.Debug.WriteLine(" Job remove Download link.");
//                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
//                        dbTran.Rollback();
//                    }
//                }
//            }
//        }
//    }
//    public interface ISchedule
//    {
//        void Run();
//    }
//    public class ScheduleDownloadLink : ISchedule
//    {
//        public void Run()
//        {
//            DateTimeOffset startTime = DateBuilder.FutureDate(10, IntervalUnit.Minute);

//            IJobDetail job = JobBuilder.Create<JobDownloadLinkDisable>()
//                                       .WithIdentity("jobDownloadLinkDisable")
//                                       .Build();

//            ITrigger trigger = TriggerBuilder.Create()
//                                             .WithIdentity("triggerDownloadLinkDisable")
//                                             .StartAt(startTime)
//                                             .WithSimpleSchedule(x => x.WithIntervalInMinutes(10).RepeatForever())
//                                             .Build();


//            ISchedulerFactory sfDownloadLinkDisable = new StdSchedulerFactory();
//            IScheduler sc = sfDownloadLinkDisable.GetScheduler();
//            sc.ScheduleJob(job, trigger);
//            sc.Start();



//            DateTimeOffset startTimeRemove = DateBuilder.FutureDate(20, IntervalUnit.Minute);

//            IJobDetail jobRemove = JobBuilder.Create<JobDownloadLinkRemove>()
//                                       .WithIdentity("jobDownloadLinkRemove")
//                                       .Build();

//            ITrigger triggerRemove = TriggerBuilder.Create()
//                                             .WithIdentity("triggerDownloadLinkRemove")
//                                             .StartAt(startTimeRemove)
//                                             .WithSimpleSchedule(x => x.WithIntervalInMinutes(10).RepeatForever())
//                                             .Build();


//            ISchedulerFactory sfDownloadLinkRemove = new StdSchedulerFactory();
//            IScheduler scRemove = sfDownloadLinkRemove.GetScheduler();
//            scRemove.ScheduleJob(jobRemove, triggerRemove);
//            scRemove.Start();


//        }
//    }
//}