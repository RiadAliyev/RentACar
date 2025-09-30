using Microsoft.AspNetCore.Http;
using RentACar.Application.Abstracts.Services;
using System.Net.Http;


namespace RentACar.Application.Shared;

public class JwtBlacklistMiddleware
{
    private readonly RequestDelegate _next;

    public JwtBlacklistMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IRedisService redisService)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

        if (!string.IsNullOrEmpty(token))
        {
            var isBlacklisted = await redisService.IsTokenBlacklistedAsync(token);
            if (isBlacklisted)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token is blacklisted. Please login again.");
                return;
            }
        }

        await _next(context);
    }
}
