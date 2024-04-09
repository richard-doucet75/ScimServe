using Microsoft.AspNetCore.Http;
using ScimServe.Endpoints.Presenters.Models;
using ScimServe.Endpoints.Services;
using ScimServe.Services;
using ScimServe.UseCases;
using System;
using System.Threading.Tasks;

public class CreateUserPresenter : CreateUser.IPresenter
{
    private const string ContentType = "application/json+scim";
    
    private readonly IResponseService _responseService;
    private readonly IETagService _eTagService;
    private readonly IUriGeneratorService _uriGeneratorService;

    public CreateUserPresenter(IResponseService responseService, IETagService eTagService, IUriGeneratorService uriGeneratorService)
    {
        _responseService = responseService;
        _eTagService = eTagService;
        _uriGeneratorService = uriGeneratorService;
    }

    public async Task PresentSuccess(PresentableUser user, string id, int version, DateTimeOffset createdDate)
    {
        var etag = _eTagService.GenerateETag("User", id, version);
        _responseService.SetETag(etag);

        await _responseService.SetResponse(StatusCodes.Status201Created, ContentType, user, 
            new MetadataModel 
            {
                ResourceType = "User",
                Location = _uriGeneratorService.GetUserUri(user.Id).ToString(),
                Created = createdDate,
                LastModified = createdDate,
                Version = etag
            });
    }

    // Implement the PresentError method to handle user-friendly error messages
    public async Task PresentError(string message)
    {
        // Consider using a common error response model if your application has one
        await _responseService.SetResponse(StatusCodes.Status400BadRequest, ContentType, new { Error = message });
    }

    // Implement the PresentException method to handle unexpected server errors
    public async Task PresentException(Exception exception)
    {
        // Log the exception details here as needed
        // For security reasons, avoid sending detailed exception information to the client
        await _responseService.SetResponse(StatusCodes.Status500InternalServerError, ContentType, new { Error = "An unexpected error occurred." });
    }

    // Implement the PresentPermissionDenied method to handle access control errors
    public async Task PresentPermissionDenied()
    {
        await _responseService.SetResponse(StatusCodes.Status403Forbidden, ContentType, new { Error = "Permission denied." });
    }
}
