using SendGrid;
using SendGrid.Helpers.Mail;
using Settings;

namespace Utils.Email;

public class EmailSender
{
    public async Task SendEmailConfirmationAsync(string email, string pin)
    {
        var defaultContent = "<strong>Seu código de ativação: " + pin + "</strong>";
        var client = new SendGridClient(EmailSettings.Key);
        var from = new EmailAddress(EmailSettings.SenderEmail, EmailSettings.SenderName);
        var subject = "Ativação de conta";
        var to = new EmailAddress(email, email);
        var plainTextContent = defaultContent;
        var htmlContent = defaultContent;
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync(msg);
    }

    public async Task SendForgotPasswordAsync(string email, string pin)
    {
        var client = new SendGridClient(EmailSettings.Key);
        var from = new EmailAddress(EmailSettings.SenderEmail, EmailSettings.SenderName);
        var subject = "Recuperação de senha";
        var to = new EmailAddress(email, email);
        var plainTextContent = "Seu código de redefinição de senha: " + pin;
        var htmlContent = "<strong>Seu código de redefinição de senha: " + pin + "</strong>";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync(msg);
    }
}