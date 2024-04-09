namespace ScimServe.Services;

public interface IETagService
{
    string GenerateETag(string scope, string id, int version);
    (string Scope, string Id, int Version) ParseETag(string etag);
}