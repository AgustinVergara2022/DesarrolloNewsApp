using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewsApp.ReadingLists;

namespace NewsApp.Controllers
{
    [Route("api/reading-lists")]
    public class ReadingListsController : NewsAppController
    {
        private readonly IReadingListAppService _service;

        public ReadingListsController(IReadingListAppService service)
        {
            _service = service;
        }

        // POST /api/reading-lists
        [HttpPost]
        public Task<ReadingListDto> CreateAsync([FromBody] CreateUpdateReadingListDto input)
        {
            return _service.CreateAsync(input);
        }

        // PUT /api/reading-lists/{id}
        [HttpPut("{id}")]
        public Task<ReadingListDto> UpdateAsync(int id, [FromBody] CreateUpdateReadingListDto input)
        {
            return _service.UpdateAsync(id, input);
        }

        // DELETE /api/reading-lists/{id}
        [HttpDelete("{id}")]
        public Task DeleteAsync(int id)
        {
            return _service.DeleteAsync(id);
        }

        // POST /api/reading-lists/{id}/news/{newsId}  -> Añadir artículo específico a la lista
        [HttpPost("{id}/news/{newsId}")]
        public Task<ReadingListDto> AddNewsAsync(int id, int newsId)
        {
            return _service.AddNewsAsync(id, newsId);
        }
    }
}