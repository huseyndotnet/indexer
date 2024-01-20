using System.Net.Mime;

using Ardalis.GuardClauses;

using Microsoft.AspNetCore.Mvc;

using Indexer.Api.Services.Abstractions;
using Indexer.Api.Models.Responses;
using Indexer.Api.Helpers;
using System.Text.Json;


namespace Indexer.Api.Controllers;

[ApiController]
[Route("api/[controller]/")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class IndexerController : ControllerBase
{
    private readonly IIndexerService _indexerService;
    private readonly HttpRequestHelper _httpRequestHelper;

    public IndexerController(IIndexerService indexerService, HttpRequestHelper httpRequestHelper)
    {
        _indexerService = Guard.Against.Null(indexerService);
        _httpRequestHelper = Guard.Against.Null(httpRequestHelper);
    }


    [HttpGet("start")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ApiResponse> Search()
    {
        await Task.Factory.StartNew(() => _indexerService.StartAsync());

        return new ApiResponse()
        {
            Result = true,
            Payload = null
        };
    }

    [HttpGet("stop")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ApiResponse> Stop()
    {
        await _indexerService.Stop();

        return new ApiResponse()
        {
            Result = true,
            Payload = null
        };
    }

    [HttpGet("status")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ApiResponse> Status()
    {
        var result = await _indexerService.Status();

        return new ApiResponse()
        {
            Result = true,
            Payload = result
        };
    }

    [HttpGet("check-ip")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<ApiResponse<string>> CheckIp()
    {
        try
        {
            string targetUrl = "http://ip.me";

            var response = await _httpRequestHelper.SendHttpRequestAsync(targetUrl);

            if (response.IsSuccessStatusCode)
            {
                var serverIpAddress = await response.Content.ReadAsStringAsync();


                return new ApiResponse<string>()
                {
                    Result = true,
                    Payload = serverIpAddress
                };
            }
            else
            {
                return new ApiResponse<string>()
                {
                    Result = false,
                    Payload = null,
                    Message = $"HTTP request failed with status code: {response.StatusCode}"
                };
            }
        }
        catch (Exception ex)
        {
            return new ApiResponse<string>()
            {
                Result = false,
                Payload = null,
                Message = $"An error occurred: {ex.Message}"
            };
        }
    }
}