using backend.Common.Results;
using Microsoft.AspNetCore.Http;

namespace backend.Common.Storage;

public interface IImageStorageService
{
    Task<Result<string>> SaveProductImageAsync(
        long productId,
        IFormFile file,
        CancellationToken cancellationToken);

    Task DeleteByRelativePathAsync(
        string relativePath,
        CancellationToken cancellationToken);
}
