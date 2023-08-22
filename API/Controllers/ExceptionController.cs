
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ExceptionController : ControllerBase
{
    [HttpGet(Name = "GetException")]
    public IEnumerable<IActionResult> Get()
    {
        throw new Exception("someething went wrong");
    }
}