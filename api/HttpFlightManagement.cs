using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace fhhagenberg
{
    public class HttpFlightManagement
    {
        private const string HttpFunctionTagName = "Flight Management";
        private readonly ILogger<HttpFlightManagement> _logger;

        public HttpFlightManagement(ILogger<HttpFlightManagement> log)
        {
            _logger = log;
        }

        [FunctionName("HttpFlightManagement")]
        [OpenApiParameter(name: "from", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "From where to start.")]
        [OpenApiParameter(name: "to", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "Where to go.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        [OpenApiOperation(operationId: nameof(HttpGetAllFlights), tags: new[] { HttpFunctionTagName }, Description = "This endpoint gets all flights.")]
        public IActionResult HttpGetAllFlights(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "flights")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var flights = Data.Flights;

            string from = req.Query["from"];
            if (!string.IsNullOrEmpty(from))
            {
                flights = flights.Where(f => f.From.Equals(from.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
            }

            string to = req.Query["to"];
            if (!string.IsNullOrEmpty(to))
            {
                flights = flights.Where(f => f.To.Equals(to.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return new OkObjectResult(flights);
        }

        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Flight), Required = true, Description = $"This {nameof(Flight)} to create a flight.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(Flight), Description = "Returns the created version of the asset.")] //, IsNullable = false)]
        [OpenApiOperation(operationId: nameof(HttpPostSingleFlight), tags: new[] { HttpFunctionTagName }, Description = "This endpoint allows to create a new flight.")]
        [FunctionName(nameof(HttpPostSingleFlight))]
        public static async Task<IActionResult> HttpPostSingleFlight(
              [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "flights")] HttpRequest req,
              ILogger log)
        {

            Flight input = null;
            // log.LogInformation("Creating a new collection list item");
            using (var streamReader = new StreamReader(req.Body))
            {
                var requestBody = await streamReader.ReadToEndAsync().ConfigureAwait(false);

                try
                {
                    input = JsonConvert.DeserializeObject<Flight>(requestBody);
                }
                catch (JsonException e)
                {
                    var message = "Object is not a valid Flight.";
                    log.LogError(message);
                    return new BadRequestObjectResult(new { message, exception = e.Message });
                }
            }

            var newId = Data.Flights.Max(f => f.Id);
            input.Id = newId + 1;
            Data.Flights.Add(input);

            return new ObjectResult(input) { StatusCode = StatusCodes.Status201Created };
        }

        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "The ID of the flight.")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Flight), Required = true, Description = $"This {nameof(Flight)} to replaces a flight.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Flight), Description = "Returns the created version of the asset.")] //, IsNullable = false)]
        [OpenApiOperation(operationId: nameof(HttpPostSingleFlight), tags: new[] { HttpFunctionTagName }, Description = "This endpoint replaces a flight.")]
        [FunctionName(nameof(HttpPutSingleFlight))]
        public static async Task<IActionResult> HttpPutSingleFlight(
                  [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "flights/{id}")] HttpRequest req,
                  int id,
                  ILogger log)
        {

            Flight input = null;
            // log.LogInformation("Creating a new collection list item");
            using (var streamReader = new StreamReader(req.Body))
            {
                var requestBody = await streamReader.ReadToEndAsync().ConfigureAwait(false);

                try
                {
                    input = JsonConvert.DeserializeObject<Flight>(requestBody);
                }
                catch (JsonException e)
                {
                    var message = "Object is not a valid Flight.";
                    log.LogError(message);
                    return new BadRequestObjectResult(new { message, exception = e.Message });
                }
            }

            input.Id = id;

            Data.Flights[Data.Flights.FindIndex(ind => ind.Id == id)] = input;

            return new ObjectResult(input);
        }

        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "The ID of the flight.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NoContent, contentType: "application/json", bodyType: typeof(Flight), Description = "Returns no content.")] //, IsNullable = false)]
        [OpenApiOperation(operationId: nameof(HttpPostSingleFlight), tags: new[] { HttpFunctionTagName }, Description = "This endpoint deletes a flight.")]
        [FunctionName(nameof(HttpDeleteSingleFlight))]
        public static IActionResult HttpDeleteSingleFlight(
                  [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "flights/{id}")] HttpRequest req,
                  int id,
                  ILogger log)
        {

            var flight = Data.Flights.FirstOrDefault(f => f.Id == id);
            if (flight != null)
            {
                Data.Flights.Remove(flight);
            }

            return new NoContentResult();
        }
    }
}

