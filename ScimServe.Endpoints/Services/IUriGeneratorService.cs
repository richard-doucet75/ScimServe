namespace ScimServe.Endpoints.Services;

public interface IUriGeneratorService
{
    Uri GetUserUri(string userId);
}