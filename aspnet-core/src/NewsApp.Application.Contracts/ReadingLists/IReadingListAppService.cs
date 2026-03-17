using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace NewsApp.ReadingLists
{
    public interface IReadingListAppService : IApplicationService
    {
        Task<ReadingListDto> CreateAsync(CreateUpdateReadingListDto input);
        Task<ReadingListDto> UpdateAsync(int id, CreateUpdateReadingListDto input);
        Task DeleteAsync(int id);
        Task<ReadingListDto> AddNewsAsync(int id, int newsId);
    }
}
