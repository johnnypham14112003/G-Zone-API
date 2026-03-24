using GZone.Repository.Base;
using GZone.Repository.Models;

namespace GZone.Repository.Interfaces
{
    public interface IRatingCommentRepository
        : IGenericRepository<RatingComment>
    {
        Task<IEnumerable<RatingComment>> GetByProductId(Guid productId);
    }
}