using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql").WithLifetime(ContainerLifetime.Persistent);
var db = sql.AddDatabase("database");

var api = builder.AddProject<Projects.KraftMesh_Api>("api")
    .WithHttpHealthCheck("/health")
    .WithReference(db);

builder.AddProject<Projects.KraftMesh_Client>("client")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
