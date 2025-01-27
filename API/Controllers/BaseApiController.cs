using System;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]  //ovo [controller] uzima prvi deo imena klase
public class BaseApiController : ControllerBase
{

}
