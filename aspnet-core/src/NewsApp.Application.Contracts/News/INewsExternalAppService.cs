using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace NewsApp.News
{
    public interface INewsExternalAppService : IApplicationService
    {
        Task<List<NewsDto>> SearchExternalAsync(string query, int pageSize = 20);
    }
}