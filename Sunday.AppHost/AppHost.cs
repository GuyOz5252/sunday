var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Sunday>("booking");

await builder.Build().RunAsync();
