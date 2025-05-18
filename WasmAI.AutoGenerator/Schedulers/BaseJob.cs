
using Hangfire;
using Quartz;

namespace AutoGenerator.Schedulers;
public readonly struct CronSchedule
{
    public static string EveryMinute => "0 0/1 * 1/1 * ? *";
    public static string EveryHour => "0 0 0/1 1/1 * ? *";
    public static string Every4Hours => "0 0 0/4 1/1 * ? *";
    public static string Every12Hours => "0 0 0/12 1/1 * ? *";
    public static string EveryDay => "0 0 0 1/1 * ? *";
    public static string Every2Days => "0 0 0 1/2 * ? *";
    public static string EveryWeek => "0 0 0 ? * 1 *";
    public static string EveryMonth => "0 0 0 1 1/1 ? *";
    public static string Every2Months => "0 0 0 1 1/2 ? *";
    public static string EveryYear => "0 0 0 1 1 ? *";
}

public class JobOptions
{

    public Type? JobType { get; set; }
    public JobOptions()
    {
    }
    public JobOptions(string cron)
    {
        Cron = cron;
    }
    public JobOptions(string cron, string jobName)
    {
        Cron = cron;
        JobName = jobName;
    }
    public string? Cron { get; set; } = CronSchedule.EveryMinute; // كل دقيقة
    public string JobName { get; set; } = "job1";
    public string JobGroup { get; set; } = "group"; // مجموعة المهمة
    public string TriggerName { get; set; } = "trigger ";
    public string TriggerGroup { get; set; } = "group1"; // مجموعة الـ Trigger
    public string JobData { get; set; } = ""; // بيانات إضافية للمهمة
    public string JobDataType { get; set; } = ""; // نوع البيانات الإضافية
    public string JobDataValue { get; set; } = ""; // قيمة البيانات الإضافية

}



public class JobEventArgs : EventArgs
{
    public  JobOptions?  Options { get; set; }
    public string? Message { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Status { get; set; }
    public object? AdditionalData { get; set; }

    public  object?  Injector { get; set; }
}

public class CJober : IJob
{

    public virtual Task Execute(IJobExecutionContext context)
    {
        throw new NotImplementedException();
    }
}

public interface ITJob {
    JobOptions Options { get; }
}
public abstract class BaseJob : CJober, ITJob
{
    protected readonly JobOptions _options;

    private readonly string? _id;



    public BaseJob()
    {
        _options = new JobOptions();
        _id = Guid.NewGuid().ToString();
        initialize();
    }
    
    public  JobOptions Options
    {
        get { return _options; }
    }
    
    private  void initialize()
    {
        InitializeJobOptions();
        _options.TriggerGroup += _id;

        _options.JobGroup +=_id;
        _options.JobName += _id;
        _options.TriggerName +=_id;





    }
    abstract  protected void InitializeJobOptions();

    

    public override Task Execute(IJobExecutionContext context)
    {
        var jobeventArgs = new JobEventArgs
        {
            Options = _options,
            Message = "Job executed",
            Timestamp = DateTime.Now,
            Status = "Success",
            AdditionalData = null
        };
    //    RecurringJob.AddOrUpdate(
    //_options.JobName,
    //() => Console.WriteLine(jobeventArgs),
    //Cron.Daily);
        return Execute(jobeventArgs);
    }
    abstract public Task Execute(JobEventArgs context);





}

