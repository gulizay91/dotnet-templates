using Api.Middlewares.MiddlewareModels;
using Api.Settings;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Controllers;

[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
[ApiController]
[Route("[controller]")]
public class ClientController : ControllerBase
{
  private readonly ILogger<ClientController> _logger;
  private readonly IOptions<ClientSettings> _options;

  public ClientController(ILogger<ClientController> logger, IOptions<ClientSettings> options)
  {
    _logger = logger;
    _options = options;
  }
  
  [HttpGet]
  [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
  public IActionResult Get(string clientName)
  {
    _logger.LogInformation($"request: {clientName}");
    if(_options.Value.ApiClient.Url.Contains(clientName))
      return Ok(clientName);
    return BadRequest();
  }
}