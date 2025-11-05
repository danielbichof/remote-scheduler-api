using Microsoft.AspNetCore.Mvc;

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
