

namespace Infrastructure.Services
{
    public class NoOpEmailSender 
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Do nothing, just return completed task
            return Task.CompletedTask;
        }
    }
}
