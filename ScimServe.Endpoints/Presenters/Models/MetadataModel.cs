namespace ScimServe.Endpoints.Presenters.Models;

public class MetadataModel
{
    public string ResourceType { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset LastModified { get; set; }
    public string Location { get; set; }
    public string Version { get; set; }
}