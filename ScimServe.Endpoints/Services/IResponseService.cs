using System.Text.Json.Nodes;
using ScimServe.Endpoints.Presenters.Models;

namespace ScimServe.Endpoints.Services;

public interface IResponseService
{
    Task SetNoContentResponse();
    Task SetResponse<TContent>(int statusCode, string contentType, TContent content);
    Task SetResponse<TContent>(int statusCode, string contentType, TContent content, MetadataModel? metadata);
    void SetETag(string etag);
    void SetLocation(string location);
}