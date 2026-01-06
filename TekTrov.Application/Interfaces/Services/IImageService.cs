using System.IO;
using System.Threading.Tasks;

namespace TekTrov.Application.Interfaces.Services
{
    public interface IImageService
    {
        Task<string> UploadAsync(
            Stream fileStream,
            string fileName,
            string folder
        );
    }
}
