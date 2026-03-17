using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace NewsApp.News
{
    public interface INewsAppService : IApplicationService
    {
        Task<List<NewsDto>> SearchAsync(string query);

    }
}