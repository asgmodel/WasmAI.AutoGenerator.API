using AutoNotificationService.Notifications;


namespace AutoGenerator.Notifications
{

   


    public interface IAutoNotifier: INotifierManager
    {


    }
    public class AutoNotifier : NotifierManager, IAutoNotifier
    {
        public AutoNotifier(IEnumerable<IProviderNotifier> notifiers) : base(notifiers)
        {
        }
    }

}
