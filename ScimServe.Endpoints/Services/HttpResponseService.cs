using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using ScimServe.Endpoints.Presenters.Models;

namespace ScimServe.Endpoints.Services;

public class HttpResponseService : IResponseService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpResponseService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task SetNoContentResponse()
    {
        var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is null.");
        httpContext.Response.StatusCode = StatusCodes.Status204NoContent;
    
        // No need to set a content type or write to the response body for a 204 No Content response
        await Task.CompletedTask;
    }

    public async Task SetResponse<TContent>(int statusCode, string contentType, TContent content)
    {
        var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is null.");
        httpContext.Response.ContentType = contentType;
        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(content, JsonSerializerOptionsFactory.Create()));
    }
    
    public async Task SetResponse<TContent>(int statusCode, string contentType, TContent content, MetadataModel? metadata)
    {
        var httpContext = _httpContextAccessor.HttpContext 
                          ?? throw new InvalidOperationException("HttpContext is null.");
        
        httpContext.Response.ContentType = contentType;
        httpContext.Response.StatusCode = statusCode;

        var responseObject = new JsonObject
        {
            ["data"] = JsonNode.Parse(JsonSerializer.Serialize(content, JsonSerializerOptionsFactory.Create())) ?? new JsonObject()
        };

        if (metadata != null)
        {
            responseObject["meta"] = JsonNode.Parse(JsonSerializer.Serialize(metadata, JsonSerializerOptionsFactory.Create())) ?? new JsonObject();
        }

        await httpContext.Response.WriteAsync(responseObject.ToJsonString(JsonSerializerOptionsFactory.Create()));
    }

    public void SetETag(string etag)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) throw new InvalidOperationException("HTTP context is not available.");

        httpContext.Response.Headers["ETag"] = etag;
    }

    public void SetLocation(string location)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) throw new InvalidOperationException("HTTP context is not available.");

        httpContext.Response.Headers["Location"] = location;
    }
}