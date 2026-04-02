using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using NewsApp.Alerts;
using NewsApp.EntityFrameworkCore.Services;

namespace NewsApp.EntityFrameworkCore.BackgroundWorkers
{

    public class AlertNewsBackgroundWorker : AsyncPeriodicBackgroundWorkerBase
    {
        private readonly ILogger<AlertNewsBackgroundWorker> _logger;
        private readonly IRepository<Alert, int> _alertsRepo;
        private readonly INewsApiClient _newsApi;
        private readonly IEmailSender _emailSender;

        public AlertNewsBackgroundWorker(
            AbpAsyncTimer timer,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<AlertNewsBackgroundWorker> logger,
            IRepository<Alert, int> alertsRepo,
            INewsApiClient newsApi,
            IEmailSender emailSender)
            : base(timer, serviceScopeFactory)
        {
            _logger = logger;
            _alertsRepo = alertsRepo;
            _newsApi = newsApi;
            _emailSender = emailSender;

            Timer.Period = 30 * 1000; // 30 segundos
        }

        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            _logger.LogInformation("Worker ejecutándose...");

            try
            {
                var alerts = await _alertsRepo.GetListAsync();

                foreach (var alert in alerts)
                {
                    var articles = await _newsApi.GetArticlesAsync(
                        alert.Keyword,
                        alert.LastSeenPublishedAt);

                    var newArticles = articles
                        .Where(a => a.PublishedAt.HasValue &&
                            (alert.LastSeenPublishedAt == null ||
                             a.PublishedAt > alert.LastSeenPublishedAt))
                        .OrderBy(a => a.PublishedAt)
                        .ToList();

                    _logger.LogInformation($"Noticias encontradas: {newArticles.Count}");

                    if (newArticles.Any())
                    {
                        var sb = new StringBuilder();

                        sb.AppendLine($"Nuevas noticias sobre \"{alert.Keyword}\":");

                        foreach (var a in newArticles)
                        {
                            sb.AppendLine($"{a.PublishedAt:yyyy-MM-dd HH:mm} - {a.Title}");
                            sb.AppendLine(a.Url);
                            sb.AppendLine();
                        }

                        var smtp = new SmtpTestSender();

                        await smtp.SendAsync(
                            alert.UserEmail,
                            $"Noticias sobre {alert.Keyword}",
                            sb.ToString()
                        );

                        alert.LastSeenPublishedAt =
                            newArticles.Max(a => a.PublishedAt);

                        await _alertsRepo.UpdateAsync(alert, autoSave: true);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking alerts");
            }
        }
    }

}