// Create Novino Application Builder

using Novin;
using Novino;
using Novino.Demo.Web;
using Novino.Demo.Web.Endpoints;

await NovinoWebApplication
  .CreateBuilder()
  .AddEndpoints()
  .AddQuartz(quartz =>
  {
    quartz.AddCronJob<TestCronJob>("1 * * * *");
  })
  .Build()
  .UseEndpoints(c =>
  {
    c.MapEndpoint<TestGetEndpoint>();
    c.MapEndpoint<TestPostEndpoint>();
  })
  .RunAsync();
