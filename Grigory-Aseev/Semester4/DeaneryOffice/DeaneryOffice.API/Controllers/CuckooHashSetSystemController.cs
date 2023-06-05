using Microsoft.AspNetCore.Mvc;

namespace DeaneryOffice.API.Controllers;


using DeaneryOffice.ExamSystems;

[ApiController]
[Route("Cuckoo/api/[action]")]
public class CuckooHashSetSystemController: ControllerBase
{
    private static readonly StripedCuckooExamSystem CuckooSystem = new();

    private readonly ILogger<CuckooHashSetSystemController> _logger;

    public CuckooHashSetSystemController(ILogger<CuckooHashSetSystemController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "countCuckoo")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    public ActionResult<int> Count() => CuckooSystem.Count;

    [HttpGet(Name = "addCuckoo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult Add(long studentId, long courseId)
    {
        CuckooSystem.Add(studentId, courseId);

        return Ok();
    }

    [HttpGet(Name = "removeCuckoo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult Remove(long studentId, long courseId)
    {
        CuckooSystem.Remove(studentId, courseId);

        return Ok();
    }

    [HttpGet(Name = "containsCuckoo")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public ActionResult<bool> Contains(long studentId, long courseId)
    {
        var contains = CuckooSystem.Contains(studentId, courseId);

        return Ok(contains);
    }
}