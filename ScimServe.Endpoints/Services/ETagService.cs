using System;
using System.Security.Cryptography;
using System.Text;

namespace ScimServe.Services;

public class ETagService : IETagService
{
    public string GenerateETag(string scope, string id, int version)
    {
        var rawData = $"{scope}:{id}:{version}";
        var bytes = Encoding.UTF8.GetBytes(rawData);
        return Convert.ToBase64String(bytes);
    }

    public (string Scope, string Id, int Version) ParseETag(string etag)
    {
        try
        {
            var bytes = Convert.FromBase64String(etag);
            var decodedString = Encoding.UTF8.GetString(bytes);
            var parts = decodedString.Split(':');
            if (parts.Length == 3)
            {
                return (parts[0], parts[1], int.Parse(parts[2]));
            }
        }
        catch (Exception ex)
        {
            // Handle errors, potentially logging them or throwing a more specific exception
            throw new InvalidOperationException("Failed to parse ETag.", ex);
        }

        throw new ArgumentException("Invalid ETag format.");
    }

}