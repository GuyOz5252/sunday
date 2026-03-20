var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .AddDatabase("sunday-db");

builder.AddProject<Projects.Sunday>("sunday")
    .WithReference(postgres)
    .WaitFor(postgres);

await builder.Build().RunAsync();
