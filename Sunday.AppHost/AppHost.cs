var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Sunday>("sunday");

await builder.Build().RunAsync();
