using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RobotCleaner.Db;
using RobotCleaner.Models.Request;
using RobotCleaner.Models.Response;
using RobotCleaner.Shared;

namespace RobotCleaner.Controllers;

[ApiController]
[Route("[controller]")]
public class RobotController : ControllerBase
{
    private readonly ILogger<RobotController> _logger;
    private readonly IRobot _robot;
    private readonly IDbHandler _postgresService;

    public RobotController(ILogger<RobotController> logger, IRobot robot, IDbHandler postgresService)
    {
        _logger = logger;
        _robot = robot;
        _postgresService = postgresService;
    }

    /// <summary>
    /// Executes the supplied path and adds execution report to database
    /// </summary>
    /// <param name="request">Contains the robot's starting position and a list of commands the robot will execute</param>
    /// <returns>An execution report indicating the robot's path and time taken to traverse it</returns>
    [HttpPost("/tibber-developer-test/enter-path")]
    public async Task<ActionResult<PathResponse>> PostPath(PathRequest request)
    {
        _logger.LogInformation("Received request: {request}", JsonConvert.SerializeObject(request));
        // Execute provided commands
        var executionReport = _robot.ExecuteCommands(request.Start, request.Commands);
        // Write execution output to DB
        var id = await _postgresService.InsertExecutionReport(executionReport);
        if (id is null) return StatusCode(500);
        
        return PathResponse.FromExecutionReport(executionReport with { Id = id });
    }

    /// <summary>
    /// Retrieves the execution report with the supplied ID, if it exists
    /// </summary>
    /// <param name="id">The ID of the execution report to fetch</param>
    /// <returns>The execution report with the supplied ID, if it exists</returns>
    [HttpGet("/tibber-developer-test/enter-path/{id:int}")]
    public async Task<ActionResult<PathResponse>> GetPath(int id)
    {
        _logger.LogInformation("Received request for execution report with ID: {Id}", id);
        var executionReport = await _postgresService.GetExecutionReport(id);
        if (executionReport is null)
        {
            _logger.LogInformation("Execution report with Id {Id} not found", id);
            return NotFound();
        }
        
        _logger.LogInformation("Found execution report: {Report}", JsonConvert.SerializeObject(executionReport));
        return PathResponse.FromExecutionReport(executionReport);
    }
}