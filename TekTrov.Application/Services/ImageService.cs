using TekTrov.Application.Interfaces.Services;

namespace TekTrov.Infrastructure.Services
{
    public class ImageService : IImageService
    {
        public async Task<string> UploadAsync(
            Stream imageStream,
            string fileName,
            string contentType)
        {
            await Task.CompletedTask;

            return $"https://dummyimage.com/{Guid.NewGuid()}";
        }
    }
}
