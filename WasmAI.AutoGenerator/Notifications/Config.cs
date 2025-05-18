




using AutoNotificationService.Notifications;
using AutoNotificationService.Services.Email;
using AutoNotificationService.Services.Sms;
using Microsoft.Extensions.DependencyInjection;


namespace AutoGenerator.Notifications.Config;

public class MailConfig: MailConfiguration
{

}
public class SmsConfig : BaseSmsConfiguration
{

}
public class OptionNotifier
{
    public MailConfig? MailConfiguration { get; set; }

    public SmsConfig? SmsConfiguration { get; set; }



}

public static class AutoNotifierConfigall
{

    public static IServiceCollection AddAutoNotifier(this IServiceCollection serviceCollection, OptionNotifier option)
    {


        

        if (option != null)
        {
           
            if (option.MailConfiguration != null)
            {
                serviceCollection.AddTransient<IBaseEmailService>(pro =>
                {
                    return new BaseEmailService(option.MailConfiguration);
                });
                serviceCollection.AddTransient<IEmailNotifier>(pro =>
                {
                    var email = pro.GetRequiredService<IBaseEmailService>();
                    var emiler = new EmailNotifier(email);
                   
                    return emiler;
                }
                );
            }
            if (option.SmsConfiguration != null)
            {

                serviceCollection.AddTransient<IBaseSmsService>(pro =>
                {
                    return new BaseSmsService(option.SmsConfiguration);
                });
                serviceCollection.AddTransient<ISmsNotifier>(pro =>
                {
                    var sms = pro.GetRequiredService<IBaseSmsService>();
                    var smser = new SmsNotifier(sms);
                    return smser;
                }

                );

            }

          

                serviceCollection.AddSingleton<IAutoNotifier>(pro =>
                {
                    var notifications = new List<IProviderNotifier>();
                    if (option.MailConfiguration != null)
                    {
                        var emailer = pro.GetRequiredService<IEmailNotifier>();
                        notifications.Add(emailer);
                    }
                    if (option.SmsConfiguration != null)
                    {
                        var smser = pro.GetRequiredService<ISmsNotifier>();
                        notifications.Add(smser);
                    }

                    
                    return new AutoNotifier(notifications);
                });
            




        }

        return serviceCollection;


    }
}