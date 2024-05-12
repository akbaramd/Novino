// Create Novino Application Builder

using Novin;
using Novino.Demo.Web.Endpoints;

await NovinoWebApplication
  .CreateBuilder()
  .AddEndpoints()
  .Build()
  .UseEndpoints(c =>
  {
    c.MapEndpoint<TestGetEndpoint>();
    c.MapEndpoint<TestPostEndpoint>();
  })
  .RunAsync();
