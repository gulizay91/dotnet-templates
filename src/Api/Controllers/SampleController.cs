using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[ApiController]
[Route("[controller]")]
public class SampleController : ControllerBase
{
  private readonly ILogger<SampleController> _logger;

  public SampleController(ILogger<SampleController> logger)
  {
    _logger = logger;
  }

  [ApiExplorerSettings(IgnoreApi = true)]
  [HttpGet]
  [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetInitialData()
  {
    _logger.LogInformation("initial values");
    return Ok();
  }
}