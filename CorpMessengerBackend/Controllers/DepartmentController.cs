using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorpMessengerBackend.Models;
using CorpMessengerBackend.Services;
using Microsoft.EntityFrameworkCore;

namespace CorpMessengerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly DepartmentContext _db;
        private readonly IAuthService _authService;

        public DepartmentController(DepartmentContext context, IAuthService auth)
        {
            _db = context;
            _authService = auth;

            if (!_db.Departments.Any())
            {
                _db.Departments.Add(new Department
                {
                    DepartmentName = "Based department",
                    Modified = DateTime.Now
                });

                _db.SaveChangesAsync();
            }
        }

        [HttpGet]   // get list of departments
        public async Task<ActionResult<IEnumerable<Department>>> Get(string token)
        {
            if (token != "123456")    // todo check for admin or user token
                return Unauthorized();

            return Ok( await _db.Departments.ToArrayAsync() );
        }

        [HttpGet]   // get department by id
        public async Task<ActionResult<Department>> Get(string token, long depId)
        {
            if (token != "123456")    // todo check for admin or user token
                return Unauthorized();

            if (!_db.Departments.Any(d => d.DepartmentId == depId))
                return BadRequest();

            return Ok( await _db.Departments.FirstAsync(
                d => d.DepartmentId == depId) );
        }

        [HttpPost]  // add department
        public async Task<ActionResult<Department>> Post(string token, Department department)
        {
            if (token != "123456")    // todo check for admin or user token
                return Unauthorized();

            if (department == null)
                return BadRequest();

            if (_db.Departments.Any(d => d.DepartmentId == department.DepartmentId))
                return BadRequest();

            var res = _db.Departments.Add(department);
            await _db.SaveChangesAsync();

            return Ok( res.Entity );
        }

        [HttpPut]  // update department
        public async Task<ActionResult<Department>> Put(string token, Department department)
        {
            if (token != "123456")    // todo check for admin or user token
                return Unauthorized();

            if (department == null)
                return BadRequest();

            if (!_db.Departments.Any(d => d.DepartmentId == department.DepartmentId))
                return BadRequest();


            var res = _db.Update(department);
            await _db.SaveChangesAsync();

            return Ok(res.Entity);
        }

        /*[HttpDelete]  // delete department todo надо ли вообще?
        public async Task<ActionResult<Department>> Delete(string token, long depId)
        {
            if (token != "123456")    // todo check for admin or user token
                return Unauthorized();

            var department = _db.Departments.FirstOrDefault(d => d.DepartmentId == depId);

            if (department == null)
                return BadRequest();

            var res = _db.Departments.Remove(department);
            await _db.SaveChangesAsync();

            return Ok(res.Entity);
        }*/
    }
}
