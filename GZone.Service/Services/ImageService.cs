using GZone.Repository.Base;
using GZone.Service.Interfaces;

namespace GZone.Service.Services
{
    public class ImageService : IImageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ImageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
