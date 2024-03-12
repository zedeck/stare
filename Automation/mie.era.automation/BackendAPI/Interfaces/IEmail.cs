using BackendAPI.Models;

namespace BackendAPI.Interfaces
{
    public interface IEmail
    {
        void sendEmail(LEmailModel emailData);
    }
}
