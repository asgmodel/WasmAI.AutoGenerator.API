


namespace AutoGenerator.Notifications;

public  class TemplateEmail
{


    public static string GetConfirmationEmailHtml(string confirmationLink)
    {
        string emailTemplate = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>تأكيد البريد الإلكتروني</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            direction: rtl;
            background-color: #f5f5f5;
            padding: 20px;
        }
        .container {
            background-color: #ffffff;
            padding: 30px;
            border-radius: 10px;
            box-shadow: 0px 0px 10px #cccccc;
        }
        .btn {
            display: inline-block;
            padding: 12px 20px;
            background-color: #28a745;
            color: white;
            text-decoration: none;
            border-radius: 5px;
            margin-top: 20px;
        }
        .footer {
            margin-top: 30px;
            font-size: 12px;
            color: #777777;
        }
    </style>
</head>
<body>
    <div class='container'>
        <h2>مرحباً بك!</h2>
        <p>شكراً لتسجيلك. لتأكيد بريدك الإلكتروني، يرجى الضغط على الزر أدناه:</p>
        <a class='btn' href='{{confirmation_link}}'>تأكيد البريد الإلكتروني</a>
        <p class='footer'>إذا لم تقم بإنشاء هذا الحساب، يمكنك تجاهل هذه الرسالة.</p>
    </div>
</body>
</html>";

        return emailTemplate.Replace("{{confirmation_link}}", confirmationLink);
    }

}
