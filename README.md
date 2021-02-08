# Spdy.AspNetCore
An ASP.NET Core middleware integration for [Spdy](https://github.com/Fresa/Spdy).

![Continuous Delivery](https://github.com/Fresa/Spdy.AspNetCore/workflows/Continuous%20Delivery/badge.svg)

## Installation
```bash
dotnet add package Spdy.AspNetCore
```

## Usage
[SpdyMiddleware](https://github.com/Fresa/Spdy.AspNetCore/blob/main/src/Spdy.AspNetCore/SpdyMiddleware.cs) works similar to the [WebSocketMiddleware](https://github.com/dotnet/aspnetcore/blob/main/src/Middleware/WebSockets/src/WebSocketMiddleware.cs).

Register the middleware with the `IApplicationBuilder` in the `StartUp.Configure` method.
```c#
public void Configure(IApplicationBuilder app)
{
    app.UseSpdy();    
}
```

Spdy requests can now be detected and implemented within a controller using extensions methods to `HttpContext`.
```c#
[ApiController]
[Route("api/v1/[Controller]")]
public class MyController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Spdy(
        CancellationToken cancellationToken)
    {
        if (HttpContext.Spdy()
                       .IsSpdyRequest)
        {
            await using var spdySession = 
                await HttpContext.Spdy()
                                 .AcceptSpdyAsync();

            return Ok();
        }

        return BadRequest();
    }
}
```

## Requesting a Spdy Upgrade
In order to request an upgrade of an HTTP request to Spdy, set the Connection and Upgrade headers.
```
Connection: Upgrade
Upgrade: SPDY/3.1
```

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://github.com/Fresa/Spdy.AspNetCore/blob/master/LICENSE)
