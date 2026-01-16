using MiniCRM.Services;
using Microsoft.Extensions.Logging;

namespace MiniCRM.Services
{
    public class FakeEmailService : IEmailService
    {
        private readonly ILogger<FakeEmailService> _logger;

        public FakeEmailService(ILogger<FakeEmailService> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
        {
            _logger.LogInformation("FAKE EMAIL -> To: {To}, Subject: {Subject}, Body: {Body}", to, subject, body);
            return Task.CompletedTask;
        }
    }
}