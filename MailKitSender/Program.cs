// See https://aka.ms/new-console-template for more information

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

var email = new MimeMessage();

//email.Sender = new MailboxAddress("Sender Name", "info@baittechnology.com");
email.From.Add(new MailboxAddress("Sender Name", "rastgele@okanmio.com"));
email.To.Add(new MailboxAddress("Receiver Name", "s.okankilic@gmail.com"));

email.Subject = "Testing out email sending";
email.Body = new TextPart(MimeKit.Text.TextFormat.Plain) { 
    Text = "Bok bok bok"
};

try
{
    using (var smtp = new SmtpClient())
    {
        smtp.Connect("mail.bait.technology", 587, SecureSocketOptions.StartTls);

        // Note: only needed if the SMTP server requires authentication
        smtp.Authenticate("ilkem@baittechnology.co", "Sazan2499");

        await smtp.SendAsync(email);
        
        smtp.Disconnect(true);
    }
    
    Console.WriteLine("Sent");
}
catch (Exception e)
{
    Console.WriteLine(e);
}

