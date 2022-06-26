using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorpMessengerBackend.Models;
using CorpMessengerBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CorpMessengerBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DepartmentController : ControllerBase
{
    private readonly IAuthService _authServiceService;
    private readonly IAppDataContext _db;
    private readonly IDateTimeService _dateTimeService;

    public DepartmentController(IAppDataContext dataContext, IAuthService authService, IDateTimeService dateTimeService)
    {
        _db = dataContext;
        _authServiceService = authService;
        _dateTimeService = dateTimeService;

        if (_db.Departments.Any()) return;
        _db.Departments.Add(new Department
        {
            DepartmentName = "Based department",
            Modified = _dateTimeService.CurrentDateTime
        });

        _db.SaveChangesAsync();
    }

    [HttpGet] // get list of departments
    public async Task<ActionResult<IEnumerable<Department>>> Get(string token)
    {
        if (!_authServiceService.CheckAdminAuth(_db, token)
            && _authServiceService.CheckUserAuth(_db, token) == 0)
            return Unauthorized();

        return Ok(await _db.Departments.ToArrayAsync());
    }

    [HttpGet("id")] // get department by id
    public async Task<ActionResult<Department>> Get(string token, long depId)
    {
        if (!_authServiceService.CheckAdminAuth(_db, token)
            && _authServiceService.CheckUserAuth(_db, token) == 0)
            return Unauthorized();

        if (!_db.Departments.Any(d => d.DepartmentId == depId))
            return NotFound();

        return Ok(await _db.Departments.FirstAsync(
            d => d.DepartmentId == depId));
    }

    [HttpPost] // add department
    public async Task<ActionResult<Department>> Post(string token, Department department)
    {
        if (!_authServiceService.CheckAdminAuth(_db, token))
            return Unauthorized();

        //if (department == null)
        //    return BadRequest();

        if (_db.Departments.Any(d => d.DepartmentId == department.DepartmentId))
            return BadRequest();

        department.Modified = _dateTimeService.CurrentDateTime;

        var newDep = _db.Departments.Add(department);
        await _db.SaveChangesAsync();

        return Ok(newDep.Entity);
    }

    [HttpPut] // update department
    public async Task<ActionResult<Department>> Put(string token, Department department)
    {
        if (!_authServiceService.CheckAdminAuth(_db, token))
            return Unauthorized();

        //if (department == null)
        //    return BadRequest();

        if (!_db.Departments.Any(d => d.DepartmentId == department.DepartmentId))
            return BadRequest();

        department.Modified = _dateTimeService.CurrentDateTime;

        var updatedDep = _db.Update(department);
        await _db.SaveChangesAsync();

        return Ok(updatedDep.Entity);
    }
}