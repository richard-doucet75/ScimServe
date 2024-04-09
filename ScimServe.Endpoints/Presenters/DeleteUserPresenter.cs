using Microsoft.AspNetCore.Http;
using ScimServe.Endpoints.Services;
using ScimServe.UseCases;

public class DeleteUserPresenter : DeleteUser.IPresenter
{
    private readonly IResponseService _responseService;

    public DeleteUserPresenter(IResponseService responseService)
    {
        _responseService = responseService;
    }

    public async Task PresentSuccess()
    {
        await _responseService.SetNoContentResponse();
    }

    public async Task PresentUserNotFound(string identifier)
    {
        await _responseService.SetResponse(StatusCodes.Status404NotFound, "application/json", new { Message = $"User {identifier} not found." });
    }

    public async Task PresentVersionConflict(string identifier, int expectedVersion, int actualVersion)
    {
        await _responseService.SetResponse(StatusCodes.Status409Conflict, "application/json", new { Message = $"Version conflict for user {identifier}. Expected: {expectedVersion}, Actual: {actualVersion}." });
    }

    public async Task PresentError(string message)
    {
        await _responseService.SetResponse(StatusCodes.Status400BadRequest, "application/json", new { Error = message });
    }

    public async Task PresentException(Exception exception)
    {
        // Log the exception here if not already logged
        await _responseService.SetResponse(StatusCodes.Status500InternalServerError, "application/json", new { Error = "An internal error occurred." });
    }

    public async Task PresentSystemUserDeletionAttempt()
    {
        await _responseService.SetResponse(StatusCodes.Status403Forbidden, "application/json", new { Message = "System user deletion is not allowed." });
    }
}