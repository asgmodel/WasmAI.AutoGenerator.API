using AutoGenerator.ApiFolder;

namespace AutoGenerator.TM
{

    public class TmSchedulers
    {

    

        public static string GetTmScheduler(string classNameSchedulerTM, TmOptions options = null)
        {
            return @$"
  public class {classNameSchedulerTM}Job : BaseJob
    {{
        public override Task Execute(JobEventArgs context)
        {{


            Console.WriteLine($""Executing job: {{_options.JobName}} with cron: {{_options.Cron}}"");

            return Task.CompletedTask;
        }}

        protected override void InitializeJobOptions()
        {{

            // _options.
            _options.JobName = ""{classNameSchedulerTM}1"";
           

        }}
    }}
    
";



        }



    }

}