// Create Novino Application Builder

using Novin;
using Novino;
using Novino.Demo.Web;
using Novino.Demo.Web.Endpoints;

await NovinoWebApplication
  .CreateBuilder("title","Title")
  .AddEndpoints()
  .AddSwagger(sw =>
  {
    sw.AddEndpointsSwagger();
  })
  .AddQuartz(quartz =>
  {
    quartz.AddCronJob<TestCronJob>("0 * * ? * *");
    quartz.AddIntervalJob<TestCronJob>(TimeSpan.FromSeconds(5));
  })
  .Build()
  .UseEndpoints(c =>
  {
    c.MapEndpointFromAssembly(typeof(TestGetEndpoint));

  })
  .UseSwagger()
  .RunAsync();
