namespace JV_Imprest_Payment.Services
{
    public interface IMailSender
    {
        void SendMail(string to, string subject, string message, string cc = null);
    }
}
