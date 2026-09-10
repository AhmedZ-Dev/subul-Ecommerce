using Microsoft.AspNetCore.Mvc;

namespace backend.Common.Extensions;

/// <summary>
/// Helpers for the catalog routes that serve both the storefront and the admin
/// panel through one endpoint.
/// </summary>
public static class CatalogVisibilityExtensions
{
    /// <summary>The status a product must carry to be visible to the public.</summary>
    public const string PublicProductStatus = "active";

    /// <summary>
    /// True when the request carries no authenticated identity.
    ///
    /// The catalog reads are [AllowAnonymous] so the storefront can call them,
    /// which also means an anonymous caller can set any filter the admin panel
    /// can — `?status=draft` or `?isActive=false` would enumerate unpublished
    /// rows. Endpoints use this to pin the visibility filter for such callers
    /// instead of trusting the query string.
    /// </summary>
    public static bool IsAnonymousCaller(this ControllerBase controller) =>
        controller.User.Identity?.IsAuthenticated != true;
}
