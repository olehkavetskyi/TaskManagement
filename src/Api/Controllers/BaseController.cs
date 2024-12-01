using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// A base controller class that provides common functionality for all API controllers.
/// </summary>
/// <remarks>
/// - Applies the [ApiController] attribute, which enables API-specific behaviors such as automatic model validation.
/// - Sets a default route pattern for derived controllers as "api/[controller]".
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class BaseController : ControllerBase
{
}
