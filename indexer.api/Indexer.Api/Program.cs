using System.Text.Json.Serialization;

using Indexer.Api.Options;
using Indexer.Api.Middlewares;
using Indexer.Api.Repositories.Abstractions;
using Indexer.Api.Repositories;
using Indexer.Api.Services.Abstractions;
using Indexer.Api.Services;
using Indexer.Api.Data;
using Indexer.Api.Helpers;
using Indexer.Api.Handlers;
using Indexer.Api.Hubs;
using Indexer.Api.Extensions;
using Indexer.Api.Strategies.Abstractions;
using Indexer.Api.Strategies;


var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;


builder.Services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var corsConfig = builder.Services.AddAndGetConfiguration<CorsConfig>(config);

var dbConfig = builder.Services.AddAndGetConfiguration<DbConfig>(config);
builder.Services.AddSqlServerDbContext<IndexerDbContext>(dbConfig);

builder.Services.AddConfiguration<TorProxyConfig>(config);
builder.Services.AddScoped<TorProxyHandler>();

builder.Services.AddConfiguration<InvalidCidrRangesConfig>(config);


var indexerConfig = builder.Services.AddAndGetConfiguration<IndexerConfig>(config);

if (indexerConfig.UseTorProxy)
    builder.Services.AddScoped<IHttpRequestStrategy, TorHttpRequestStrategy>();
else
    builder.Services.AddScoped<IHttpRequestStrategy, StandardHttpRequestStrategy>();



builder.Services.AddScoped<HttpRequestHelper>();

builder.Services.AddScoped<IWebsitesRepository, WebsitesRepository>();
builder.Services.AddScoped<IIndexerService, IndexerService>();

builder.Services.AddSignalR();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        options.DisplayRequestDuration();
    });
}

app.UseCors(builder =>
{
    builder.WithOrigins(corsConfig.AllowedOrigins)
           .AllowAnyHeader()
           .AllowAnyMethod()
           .AllowCredentials();
});

app.UseHttpsRedirection();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ConsoleHub>("/consoleOutputHub");

    endpoints.MapControllers();
});

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.Run();