using Microsoft.AspNetCore.Mvc;
using NewsApp.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewsApp.News; 

namespace NewsApp.Controllers
{
    [Route("api/news")]
    public class NewsController : NewsAppController
    {
        private readonly INewsAppService _service;
        private readonly INewsExternalAppService _externalService;

        public NewsController(INewsAppService service, INewsExternalAppService externalService)
        {
            _service = service;
            _externalService = externalService;
        }

        // GET /api/news/search?q=termino
        [HttpGet("search")]
        public Task<List<NewsDto>> SearchAsync([FromQuery] string q)
        {
            return _service.SearchAsync(q);
        }

        // GET /api/news/external-search?q=termino
        [HttpGet("external-search")]
        public Task<List<NewsDto>> ExternalSearchAsync([FromQuery] string q, [FromQuery] int pageSize = 20)
        {
            return _externalService.SearchExternalAsync(q, pageSize);
        }
    }
}