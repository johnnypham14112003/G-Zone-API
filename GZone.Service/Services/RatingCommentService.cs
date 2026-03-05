using DLL.Interfaces;
using GZone.Repository;
using GZone.Repository.Models;
using GZone.Repository.Repositories;

namespace DLL.Services
{
    public class RatingCommentService : IRatingCommentService
    {
        private readonly RatingCommentRepository _repo;

        public RatingCommentService(GZoneDbContext context)
        {
            _repo = new RatingCommentRepository(context);
        }

        public async Task<List<RatingComment>> GetByProduct(Guid productId)
        {
            return (await _repo.GetByProductId(productId)).ToList();
        }

        public async Task Add(RatingComment rating)
        {
            if (rating.RatingScore < 1 || rating.RatingScore > 5)
                throw new Exception("Invalid rating score");

            rating.CreatedAt = DateTime.Now;

            await _repo.AddAsync(rating);
            await _repo.SaveChangeAsync();
        }
    }
}