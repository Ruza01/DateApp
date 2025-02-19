using System;
using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ServiceFilter(typeof(LogUserActivity))]
[ApiController]
[Route("api/[controller]")]  //ovo [controller] uzima prvi deo imena klase
public class BaseApiController : ControllerBase
{

}
