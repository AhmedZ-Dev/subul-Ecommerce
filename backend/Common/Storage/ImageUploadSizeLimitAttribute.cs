using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace backend.Common.Storage;

/// <summary>
/// Raises the request body limit for a single upload action.
///
/// Kestrel's <c>MaxRequestBodySize</c> is global, so sizing it for image uploads
/// used to hand every JSON endpoint the same multi-megabyte allowance — including
/// the unauthenticated POST api/orders. The global limit is now small, and the
/// three upload actions opt back up through this attribute.
///
/// The limit is read from <see cref="ImageStorageOptions"/> rather than baked in
/// as a constant, because the framework's own [RequestSizeLimit] needs a
/// compile-time value and would silently disagree with the configured
/// MaxFileSizeBytes the moment someone changed it.
/// </summary>
public sealed class ImageUploadSizeLimitAttribute : Attribute, IFilterFactory
{
    public bool IsReusable => true;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider) =>
        new ImageUploadSizeLimitFilter(
            serviceProvider.GetRequiredService<IOptions<ImageStorageOptions>>());
}

/// <remarks>
/// Implemented as an authorization filter because that stage runs before model
/// binding, which is where the multipart body is actually read. An action filter
/// would run too late to raise the limit.
/// </remarks>
internal sealed class ImageUploadSizeLimitFilter(IOptions<ImageStorageOptions> options)
    : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var feature = context.HttpContext.Features.Get<IHttpMaxRequestBodySizeFeature>();

        // Null on servers that do not support the feature; read-only once the
        // body has been read. Neither is an error here — leaving the ambient
        // limit in place is the safe outcome.
        if (feature is null || feature.IsReadOnly)
            return;

        feature.MaxRequestBodySize =
            options.Value.MaxFileSizeBytes + ImageStorageOptions.MultipartOverheadBytes;
    }
}
