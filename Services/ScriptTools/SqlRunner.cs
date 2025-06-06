using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GenericApi.Models;
using GenericApi.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GenericApi.Services.ScriptTools
{
    public class SqlRunner
    {
        private readonly AppDbContext _context = new();
        private readonly ApiResponse _response = new(new HttpContextAccessor()); // Update to your actual response helper

        public async Task<IActionResult> RunStoredProcedureAsync<T>(
            string sqlQuery,
            string? activity = null,
            params SqlParameter[] parameters
        )
        {
            // Find or create output parameters
            var statusCode =
                parameters.FirstOrDefault(p => p.ParameterName == "@StatusCode")
                ?? new SqlParameter("@StatusCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output,
                };
            var message =
                parameters.FirstOrDefault(p => p.ParameterName == "@Message")
                ?? new SqlParameter("@Message", SqlDbType.NVarChar, 255)
                {
                    Direction = ParameterDirection.Output,
                };
            var data =
                parameters.FirstOrDefault(p => p.ParameterName == "@Data")
                ?? new SqlParameter("@Data", SqlDbType.NVarChar, -1)
                {
                    Direction = ParameterDirection.Output,
                };

            // Ensure output parameters are included
            var paramList = parameters.ToList();
            if (!paramList.Contains(statusCode))
                paramList.Add(statusCode);
            if (!paramList.Contains(message))
                paramList.Add(message);
            if (!paramList.Contains(data))
                paramList.Add(data);

            await _context.Database.ExecuteSqlRawAsync(sqlQuery, [.. paramList]);

            var dataJson = data.Value?.ToString();
            T? resultObj = default;
            if (!string.IsNullOrWhiteSpace(dataJson))
            {
                resultObj = JsonSerializer.Deserialize<T>(dataJson);
                // Convert all DateTime properties from UTC to Asia/Manila
                if (resultObj != null)
                {
                    var dateProps = typeof(T)
                        .GetProperties()
                        .Where(p => p.PropertyType == typeof(DateTime));
                    foreach (var prop in dateProps)
                    {
                        var valueObj = prop.GetValue(resultObj);
                        if (valueObj is DateTime value)
                        {
                            prop.SetValue(
                                resultObj,
                                TimeZoneInfo.ConvertTimeFromUtc(
                                    value,
                                    TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila")
                                )
                            );
                        }
                    }
                }
            }

            var _statusCode = (int)statusCode.Value;
            var _message = message.Value?.ToString();

            if (_statusCode >= StatusCodes.Status400BadRequest)
            {
                return _response.Error(
                    statusCode: _statusCode,
                    e: new Exception(_message),
                    saveLog: true
                );
            }

            return _response.Success(
                statusCode: _statusCode,
                activity: activity ?? string.Empty,
                message: _message,
                data: resultObj
            );
        }

        public class StoredProcedureRawResult<T>
        {
            public int StatusCode { get; set; }
            public string? Message { get; set; }
            public T? Data { get; set; }
        }

        public async Task<StoredProcedureRawResult<T>> RunStoredProcedureRaw<T>(
            string sqlQuery,
            params SqlParameter[] parameters
        )
        {
            // Find or create output parameters
            var statusCode =
                parameters.FirstOrDefault(p => p.ParameterName == "@StatusCode")
                ?? new SqlParameter("@StatusCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output,
                };
            var message =
                parameters.FirstOrDefault(p => p.ParameterName == "@Message")
                ?? new SqlParameter("@Message", SqlDbType.NVarChar, 255)
                {
                    Direction = ParameterDirection.Output,
                };
            var data =
                parameters.FirstOrDefault(p => p.ParameterName == "@Data")
                ?? new SqlParameter("@Data", SqlDbType.NVarChar, -1)
                {
                    Direction = ParameterDirection.Output,
                };

            // Ensure output parameters are included
            var paramList = parameters.ToList();
            if (!paramList.Contains(statusCode))
                paramList.Add(statusCode);
            if (!paramList.Contains(message))
                paramList.Add(message);
            if (!paramList.Contains(data))
                paramList.Add(data);

            await _context.Database.ExecuteSqlRawAsync(sqlQuery, [.. paramList]);

            var dataJson = data.Value?.ToString();
            T? resultObj = default;
            if (!string.IsNullOrWhiteSpace(dataJson))
            {
                resultObj = JsonSerializer.Deserialize<T>(dataJson);
                // Convert all DateTime properties from UTC to Asia/Manila
                if (resultObj != null)
                {
                    var dateProps = typeof(T)
                        .GetProperties()
                        .Where(p => p.PropertyType == typeof(DateTime));
                    foreach (var prop in dateProps)
                    {
                        var valueObj = prop.GetValue(resultObj);
                        if (valueObj is DateTime value)
                        {
                            prop.SetValue(
                                resultObj,
                                TimeZoneInfo.ConvertTimeFromUtc(
                                    value,
                                    TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila")
                                )
                            );
                        }
                    }
                }
            }

            var _statusCode = (int)statusCode.Value;
            var _message = message.Value?.ToString();

            return new StoredProcedureRawResult<T>
            {
                StatusCode = _statusCode,
                Message = _message,
                Data = resultObj,
            };
        }
    }
}
