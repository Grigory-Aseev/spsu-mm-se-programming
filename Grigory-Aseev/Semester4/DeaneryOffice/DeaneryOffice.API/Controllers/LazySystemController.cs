using Microsoft.AspNetCore.Mvc;

namespace DeaneryOffice.API.Controllers;

using DeaneryOffice.ExamSystems;


[ApiController]
[Route("lazy/api/[action]")]
public class LazyController : ControllerBase
{
    private static readonly LazyExamSystem _lazySystem = new();

    private readonly ILogger<LazyController> _logger;

    public LazyController(ILogger<LazyController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "countLazy")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    public ActionResult<int> Count() => _lazySystem.Count;

    [HttpGet(Name = "addLazy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult Add(long studentId, long courseId)
    {
        _lazySystem.Add(studentId, courseId);

        return Ok();
    }

    [HttpGet(Name = "removeLazy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult Remove(long studentId, long courseId)
    {
        _lazySystem.Remove(studentId, courseId);

        return Ok();
    }

    [HttpGet(Name = "containsLazy")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public ActionResult<bool> Contains(long studentId, long courseId)
    {
        var contains = _lazySystem.Contains(studentId, courseId);

        return Ok(contains);
    }
}