using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsApp.ReadingLists;
using NewsApp.EntityFrameworkCore;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Users;
using Volo.Abp.Authorization;
using Volo.Abp;

namespace NewsApp.ReadingLists
{
    public class ReadingListAppService : ApplicationService, IReadingListAppService
    {
        private readonly IRepository<ReadingList, int> _readingListRepository;
        private readonly IDbContextProvider<NewsAppDbContext> _dbContextProvider;
        private readonly ILogger<ReadingListAppService> _logger;

        public ReadingListAppService(
            IRepository<ReadingList, int> readingListRepository,
            IDbContextProvider<NewsAppDbContext> dbContextProvider,
            ILogger<ReadingListAppService> logger)
        {
            _readingListRepository = readingListRepository;
            _dbContextProvider = dbContextProvider;
            _logger = logger;
        }

        public async Task<ReadingListDto> CreateAsync(CreateUpdateReadingListDto input)
        {
            var ownerId = CurrentUser.Id ?? Guid.Empty;

            var list = new ReadingList
            {
                Name = input.Name,
                OwnerUserId = ownerId
            };

            int order = 0;
            foreach (var newsId in input.NewsIds)
            {
                list.Items.Add(new ReadingListItem
                {
                    NewsId = newsId,
                    Order = order++
                });
            }

            var created = await _readingListRepository.InsertAsync(list, autoSave: true);

            return new ReadingListDto
            {
                Id = created.Id,
                Name = created.Name,
                OwnerUserId = created.OwnerUserId,
                NewsIds = created.Items.OrderBy(i => i.Order).Select(i => i.NewsId).ToList()
            };
        }

        public async Task<ReadingListDto> UpdateAsync(int id, CreateUpdateReadingListDto input)
        {
            var db = await _dbContextProvider.GetDbContextAsync();

            var entity = await db.ReadingLists.Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (entity == null)
            {
                throw new UserFriendlyException($"ReadingList with id {id} not found.");
            }

            // Ownership check: only owner may update
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            if (entity.OwnerUserId != currentUserId)
            {
                throw new AbpAuthorizationException("You are not allowed to update this reading list.");
            }

            entity.Name = input.Name;

            // Replace items: clear and re-add preserving order
            entity.Items.Clear();
            int order = 0;
            foreach (var newsId in input.NewsIds)
            {
                entity.Items.Add(new ReadingListItem
                {
                    NewsId = newsId,
                    Order = order++
                });
            }

            await db.SaveChangesAsync();

            return new ReadingListDto
            {
                Id = entity.Id,
                Name = entity.Name,
                OwnerUserId = entity.OwnerUserId,
                NewsIds = entity.Items.OrderBy(i => i.Order).Select(i => i.NewsId).ToList()
            };
        }

        public async Task DeleteAsync(int id)
        {
            var db = await _dbContextProvider.GetDbContextAsync();

            var entity = await db.ReadingLists.Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (entity == null)
            {
                throw new UserFriendlyException($"ReadingList with id {id} not found.");
            }

            // Ownership check: only owner may delete
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            if (entity.OwnerUserId != currentUserId)
            {
                throw new AbpAuthorizationException("You are not allowed to delete this reading list.");
            }

            // Remove associated items explicitly, then the list
            if (entity.Items != null && entity.Items.Any())
            {
                db.ReadingListItems.RemoveRange(entity.Items);
            }

            db.ReadingLists.Remove(entity);

            await db.SaveChangesAsync();
        }

        // Añadido: agrega un artículo específico a la lista
        public async Task<ReadingListDto> AddNewsAsync(int id, int newsId)
        {
            var db = await _dbContextProvider.GetDbContextAsync();

            var entity = await db.ReadingLists
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (entity == null)
            {
                throw new UserFriendlyException($"ReadingList with id {id} not found.");
            }

            if (CurrentUser.Id == null)
            {
                throw new AbpAuthorizationException("Authentication is required.");
            }

            var currentUserId = CurrentUser.Id.Value;
            if (entity.OwnerUserId != currentUserId)
            {
                throw new AbpAuthorizationException("You are not allowed to modify this reading list.");
            }

            // Verificar que la noticia exista
            var newsExists = await db.News.AnyAsync(n => n.Id == newsId);
            if (!newsExists)
            {
                throw new UserFriendlyException($"News with id {newsId} not found.");
            }

            // Evitar duplicados: si ya existe, devolvemos la lista tal cual
            if (entity.Items.Any(i => i.NewsId == newsId))
            {
                return new ReadingListDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    OwnerUserId = entity.OwnerUserId,
                    NewsIds = entity.Items.OrderBy(i => i.Order).Select(i => i.NewsId).ToList()
                };
            }

            var nextOrder = entity.Items.Any() ? entity.Items.Max(i => i.Order) + 1 : 0;

            entity.Items.Add(new ReadingListItem
            {
                NewsId = newsId,
                Order = nextOrder
            });

            await db.SaveChangesAsync();

            return new ReadingListDto
            {
                Id = entity.Id,
                Name = entity.Name,
                OwnerUserId = entity.OwnerUserId,
                NewsIds = entity.Items.OrderBy(i => i.Order).Select(i => i.NewsId).ToList()
            };
        }
    }
}