
//using Microsoft.Extensions.Hosting;
//using Quartz;
//using Quartz.Impl.Matchers;

//namespace AutoGenerator.Schedulers;
//public interface ISchedulerJobProvider:IHostedService
//{
//    Task AddJobAsync(JobOptions options);
//    Task RemoveJobAsync(string jobName);
//    Task UpdateJobAsync(string jobName, JobOptions options);
//    Task StartJobAsync(string jobName);
//    Task PauseJobAsync(string jobName);
//    Task ResumeJobAsync(string jobName);
//    Task StopJobAsync(string jobName);
//}


//public class SchedulerJobProvider : ISchedulerJobProvider
//{
//    private readonly ISchedulerFactory _schedulerFactory;
//    private IScheduler _scheduler;
//    private readonly Dictionary<string, JobOptions>? _jobs;



//    public SchedulerJobProvider(ISchedulerFactory schedulerFactory, Dictionary<string,JobOptions>? jobOptions=null)
//    {
//        _schedulerFactory = schedulerFactory;

//        if (jobOptions == null)
//            _jobs = new Dictionary<string, JobOptions>();
//        else
//        {
//            _jobs = jobOptions;
//        }
            

//    }

//    // بدء الـ Scheduler
//    public async Task StartAsync()
//    {
        

//        InitializeJobs();
//    }

//    private  void InitializeJobs()
//    {
        
    
//      foreach (var job in _jobs)
//        {
//            var jobDetail = JobBuilder.Create(job.Value.JobType) // استبدل BaseJob بنوع المهمة الخاصة بك
//                .WithIdentity(job.Value.JobName, job.Value.JobGroup)
//                .UsingJobData("JobData", job.Value.JobData)
//                .Build();
//            var trigger = TriggerBuilder.Create()
//                .WithIdentity(job.Value.TriggerName, job.Value.TriggerGroup)
//                .WithCronSchedule(job.Value.Cron)
//                .Build();
//            _scheduler.ScheduleJob(jobDetail, trigger);
//        }


//    }
//    // إضافة مهمة جديدة
//    public async Task AddJobAsync(JobOptions options) { 
//        if (_jobs.ContainsKey(options.JobName))
//            throw new InvalidOperationException("Job already exists");

//        _jobs[options.JobName] = options;

//        var jobDetail = JobBuilder.Create<BaseJob>()
//            .WithIdentity(options.JobName, options.JobGroup)
//            .UsingJobData("JobData", options.JobData)
//            .Build();

//        var trigger = TriggerBuilder.Create()
//            .WithIdentity(options.TriggerName, options.TriggerGroup)
//            .WithCronSchedule(options.Cron)
//            .Build();

//        await _scheduler.ScheduleJob(jobDetail, trigger);
//    }

//    // إزالة مهمة
//    public async Task RemoveJobAsync(string jobName)
//    {
//        if (!_jobs.ContainsKey(jobName))
//            throw new KeyNotFoundException("Job not found");

//        await _scheduler.DeleteJob(new JobKey(jobName));
//        _jobs.Remove(jobName);
//    }

//    // تحديث مهمة
//    public async Task UpdateJobAsync(string jobName, JobOptions options)
//    {
//        if (!_jobs.ContainsKey(jobName))
//            throw new KeyNotFoundException("Job not found");

//        // إزالة المهمة القديمة أولاً
//        await RemoveJobAsync(jobName);

//        // إضافة المهمة الجديدة
//        await AddJobAsync(options);
//    }

//    // بدء مهمة
//    public async Task StartJobAsync(string jobName)
//    {
//        var jobKey = new JobKey(jobName);
//        await _scheduler.TriggerJob(jobKey);
//    }

//    // إيقاف مهمة
//    public async Task StopJobAsync(string jobName)
//    {
//        var jobKey = new JobKey(jobName);
//        await _scheduler.PauseJob(jobKey);
//    }

//    // استئناف مهمة
//    public async Task ResumeJobAsync(string jobName)
//    {
//        var jobKey = new JobKey(jobName);
//        await _scheduler.ResumeJob(jobKey);
//    }

//    // إيقاف المهمة
//    public async Task PauseJobAsync(string jobName)
//    {
//        var jobKey = new JobKey(jobName);
//        await _scheduler.PauseJob(jobKey);
//    }


//    // إيقاف جميع المهام
//    public async Task StopAllJobsAsync()
//    {
//        await _scheduler.Shutdown();
//    }

//    // بدء جميع المهام
//    public async Task StartAllJobsAsync()
//    {
//        await _scheduler.Start();
//    }

//    // استئناف جميع المهام
//    public async Task ResumeAllJobsAsync()
//    {
//        await _scheduler.ResumeAll();
//    }

//    // إيقاف جميع المهام
//    public async Task PauseAllJobsAsync()
//    {
//        await _scheduler.PauseAll();
//    }


//    // add  list jobs

//    public async Task<List<JobOptions>> ListJobsAsync()
//    {
//        var jobKeys = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
//        var jobs = new List<JobOptions>();
//        foreach (var jobKey in jobKeys)
//        {
//            var jobDetail = await _scheduler.GetJobDetail(jobKey);
//            var trigger = await _scheduler.GetTriggersOfJob(jobKey);
//            var options = new JobOptions
//            {
//                JobName = jobKey.Name,
//                JobGroup = jobKey.Group,
              
//            };
//            jobs.Add(options);
//        }
//        return jobs;
//    }

//    public async Task StartAsync(CancellationToken cancellationToken)
//    {
//        _scheduler = await _schedulerFactory.GetScheduler();

//        InitializeJobs();

//    }

//    public Task StopAsync(CancellationToken cancellationToken)
//    {

//        return StopAllJobsAsync();
//    }
//}
