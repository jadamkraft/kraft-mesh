var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.KraftMesh_Api>("api")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.KraftMesh_Client>("client")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
