using GZone.Repository.Models;

namespace DLL.Interfaces
{
    public interface IRatingCommentService
    {
        Task<List<RatingComment>> GetByProduct(Guid productId);

        Task Add(RatingComment rating);
    }
}