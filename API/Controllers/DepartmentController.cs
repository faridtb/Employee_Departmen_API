using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class DepartmentController : BaseApiController
    {
        private readonly DataContext _context;
        public DepartmentController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            return await _context.Departments.Include(x =>x.Employees).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            return await _context.Departments.Include(x =>x.Employees).FirstOrDefaultAsync(x=>x.Id==id);
        }


        [HttpPost("crtdep")]
        public async Task<ActionResult<Department>> CreateDepartment(string name)
        {
            var dep = new Department 
            {
                Name = name.ToLower(),
                CreateDate = DateTime.Now,
            };
        
            if(_context.Departments.Any(d=>d.Name ==name.ToLower())) return BadRequest("This department exist");

            _context.Departments.Add(dep);

            await _context.SaveChangesAsync();

            return dep;
        }

        [HttpDelete("del/{id}")]
         public async Task<ActionResult<Department>> RemoveDepartment(int id)
         {
            var dep = _context.Departments.Find(id);

            if(dep == null) return BadRequest("Cant Find Any Department");

            _context.Departments.Remove(dep);

            await _context.SaveChangesAsync();

            return Ok("Department deleted from base");
         }

         [HttpPut("update/{id}")]
          public async Task<ActionResult<Department>> UpdateDepartment(int id,string name)
          {
            var dep = _context.Departments.Find(id);

            if(dep == null) return BadRequest("Cant Find Any Department");
            
            dep.Name = name;

            _context.Departments.Update(dep);

            await _context.SaveChangesAsync();

            return dep;
          }


    }
}