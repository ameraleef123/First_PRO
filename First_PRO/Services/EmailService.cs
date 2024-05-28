namespace First_PRO.Services
{
    using MailKit.Net.Smtp;

    using MimeKit;

    public static class EmailService
    {
        public static void SendRecipeEmail(string toAddress,string recipeName,string recipeDescription)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse("ameraleef77@gmail.com"));
            message.To.Add(MailboxAddress.Parse(toAddress));
            message.Subject = "Recipe: " + recipeName;


            var builder = new BodyBuilder();
            builder.HtmlBody = $@"
                <div style='background-color: #f5f5f5; padding: 20px;'>
                    <div style='background-color: #fff; border-radius: 5px; padding: 20px;'>
                        <h1 style='color: #333;'>{recipeName}</h1>
                        <p>{recipeDescription}</p>
                        <p>Enjoy!</p>
                    </div>
                    <div style='text-align: center; margin-top: 20px;'>
                        <h3 style='color: #FEA116; font-family: Nunito, sans-serif;'>Recipe Blog</h3>
                    </div>
                </div>
            ";

            message.Body = builder.ToMessageBody();


            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false); 
                client.Authenticate("ameraleef77@gmail.com", "mpdq vete zlsu apeb"); 
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }

}
