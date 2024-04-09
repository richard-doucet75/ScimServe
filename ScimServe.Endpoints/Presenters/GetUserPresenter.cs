using ScimServe.Endpoints.Services;
using ScimServe.UseCases;
using Microsoft.AspNetCore.Http;
using ScimServe.Endpoints.Presenters.Models;
using ScimServe.Services;

namespace ScimServe.Endpoints.Presenters;

public class GetUserPresenter : GetUser.IPresenter
{
    private const string ContentType = "application/json+scim";
    
    private readonly IResponseService _responseService;
    private readonly IETagService _etagService;
    private readonly IUriGeneratorService _uriGeneratorService;

    public GetUserPresenter(IResponseService responseService, IETagService etagService, IUriGeneratorService uriGeneratorService)
    {
        _uriGeneratorService = uriGeneratorService;
        _responseService = responseService;
        _etagService = etagService;
    }

    public async Task PresentSuccess(PresentableUser user, string id, int version, DateTimeOffset created, DateTimeOffset lastModified)
    {
        await _responseService.SetResponse(StatusCodes.Status200OK, ContentType, user, new MetadataModel
        {
            Location = _uriGeneratorService.GetUserUri(id).ToString(),
            Created = created,
            LastModified = lastModified,
            ResourceType = "User",
            Version = _etagService.GenerateETag("User", id, version)
        });
    }

    public async Task PresentUserNotFound(string identifier)
    {
        await _responseService.SetResponse(StatusCodes.Status404NotFound, ContentType, new { Message = $"User {identifier} not found." });
    }

    public async Task PresentError(string message)
    {
        await _responseService.SetResponse(StatusCodes.Status400BadRequest, ContentType, new { Error = message });
    }

    public async Task PresentException(Exception exception)
    {
        await _responseService.SetResponse(StatusCodes.Status500InternalServerError, ContentType, new { Error = "An internal error occurred." });
    }

    public async Task PresentPermissionDenied()
    {
        await _responseService.SetResponse(StatusCodes.Status403Forbidden, ContentType,new { Error = "Permission denied." });
    }
}