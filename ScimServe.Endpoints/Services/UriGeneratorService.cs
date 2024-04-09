using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ScimServe.Endpoints.Services;

public class UriGeneratorService : IUriGeneratorService
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UriGeneratorService(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
    {
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    public Uri GetUserUri(string userId)
    {
        var uriString = _linkGenerator.GetUriByRouteValues(
            _httpContextAccessor.HttpContext,
            "GetUser",
            new { id = userId },
            _httpContextAccessor.HttpContext.Request.Scheme,
            _httpContextAccessor.HttpContext.Request.Host);
        

        // Construct the full URI using the request's host information
        var uri = new Uri(uriString);
        return uri;
    }
}