// Create Novino Application Builder

using Novin;
using Novino.Demo.Web.Endpoints;

var builder = NovinoWebApplication.CreateBuilder();
builder.Services.AddScoped<HealthEndpoint>();
builder.AddMvc();
// Build And Start Application
await builder
    .Build()
    .UseMvc()
    .RunAsync();