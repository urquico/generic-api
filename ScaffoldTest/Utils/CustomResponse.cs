using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ScaffoldTest.Utils
{
    public class CustomSuccess : ControllerBase
    {
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
                Console.WriteLine($"Activity: {activity}");
                Console.WriteLine($"IP: {ip}");
                Console.WriteLine($"StatusCode: {statusCode}");
            }

            return StatusCode(statusCode, response);
        }

        public ObjectResult Error(int statusCode, Exception e, bool saveLog = false)
        {
            var response = new { StatusCode = statusCode, Error = e.Message };

            if (saveLog)
            {
                // TODO: store it in actual database using stored procedures; Log the error
                Console.WriteLine($"Error: {e.Message}");
                Console.WriteLine($"StackTrace: {e.StackTrace}");
                Console.WriteLine($"StatusCode: {statusCode}");
            }

            var result = new ObjectResult(response) { StatusCode = statusCode };
            return result;
        }
    }
}
