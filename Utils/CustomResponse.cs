using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ScaffoldTest.Utils
{
    public class CustomSuccess : ControllerBase
    {
        private readonly Serilog.ILogger _logger = Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(
                "logs/error_log.txt",
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error
            )
            .CreateLogger();

        public ObjectResult Success(
            int statusCode,
            string activity,
            string ip,
            string? message = null,
            object? data = null
        )
        {
            var response = new
            {
                StatusCode = statusCode,
                Message = message,
                Data = data,
            };

            if (
                !string.IsNullOrEmpty(ip)
                && !string.IsNullOrEmpty(activity)
                && !string.IsNullOrEmpty(message)
            )
            {
                // TODO: store it in actual database using stored procedures; Log the activity
                _logger.Information($"Activity: {activity}");
                _logger.Information($"IP: {ip}");
                _logger.Information($"StatusCode: {statusCode}");
            }

            return StatusCode(statusCode, response);
        }

        public ObjectResult Error(int statusCode, Exception e, bool saveLog = false)
        {
            var response = new { StatusCode = statusCode, Error = e.Message };

            if (saveLog)
            {
                // TODO: store it in actual database using stored procedures; Log the error
                _logger.Error($"Error: {e.Message}");
                _logger.Error($"StackTrace: {e.StackTrace}");
                _logger.Error($"StatusCode: {statusCode}");
            }

            var result = new ObjectResult(response) { StatusCode = statusCode };
            return result;
        }
    }
}
