using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MathHelper.Services;

public class PathAwareNavigationManager
{
    private readonly NavigationManager _navigationManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<PathAwareNavigationManager> _logger;

    public PathAwareNavigationManager(
        NavigationManager navigationManager, 
        IHttpContextAccessor httpContextAccessor,
        ILogger<PathAwareNavigationManager> logger)
    {
        _navigationManager = navigationManager;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public void NavigateTo(string uri, bool forceLoad = false)
    {
        var pathBase = _httpContextAccessor.HttpContext?.Request.PathBase.Value ?? "";
        var originalUri = uri;
        
        // If uri is relative and doesn't already start with pathBase, prepend it
        if (!string.IsNullOrEmpty(pathBase) && 
            !uri.StartsWith("http", StringComparison.OrdinalIgnoreCase) &&
            !uri.StartsWith(pathBase))
        {
            uri = $"{pathBase}/{uri.TrimStart('/')}";
        }
        
        _logger.LogInformation("PathAwareNavigationManager: Navigating from '{OriginalUri}' to '{FinalUri}' (PathBase: '{PathBase}')", 
            originalUri, uri, pathBase);
        
        _navigationManager.NavigateTo(uri, forceLoad);
    }

    public string BaseUri => _navigationManager.BaseUri;
    public string Uri => _navigationManager.Uri;
}
