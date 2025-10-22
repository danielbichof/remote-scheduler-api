using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Scheduler.Api.Controllers
{
  [ApiController]
  [Route("v1")] // Rota base: /v1
  public class WelcomeController : ControllerBase
  {
    [HttpGet]
    public IActionResult Get() => Ok(new { message = "Bem vindo" });
  }
}

[ApiController]
[Route("v1/secure")]
public class SecureController : ControllerBase
{
  [HttpGet]
  [Authorize]
  public IActionResult GetSecret()
  {
    return Ok("Segredo protegido por JWT");
  }
}
