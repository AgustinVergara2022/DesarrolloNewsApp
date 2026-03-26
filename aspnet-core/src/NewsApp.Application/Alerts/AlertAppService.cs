using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using NewsApp.Alerts;

public class AlertAppService : ApplicationService
{
    private readonly IRepository<Alert, int> _alertRepository;

    public AlertAppService(IRepository<Alert, int> alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public async Task CreateAsync(string keyword)
    {
        var userId = CurrentUser.Id ?? Guid.Empty;
        var email = CurrentUser.Email;

        var alert = new Alert
        {
            UserId = userId,
            UserEmail = email,
            Keyword = keyword,
            LastSeenPublishedAt = null
        };

        await _alertRepository.InsertAsync(alert, autoSave: true);
    }
}