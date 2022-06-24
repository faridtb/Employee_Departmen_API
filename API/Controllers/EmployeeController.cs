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
    public class EmployeeController : BaseApiController
    {
        private readonly DataContext _context;
        public EmployeeController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            return await _context.Employees.FindAsync(id);
        }

        [HttpPost("crt")]
        public async Task<ActionResult<Employee>> CreateEmployee(string name,string surname,DateTime birth,int depid)
        {
            var emp = new Employee 
            {
                Name = name,
                Surname = surname,
                BirthDate = birth,
                CreateDate = DateTime.Now,
                DepartmentId =depid
            };

            if(_context.Departments.Find(depid) == null) return BadRequest("Cant Find any department");

            if(_context.Employees.Any(e => e.Name ==name && e.Surname ==surname && e.BirthDate ==birth)) return BadRequest("That employee exist");

            _context.Add(emp);

            await _context.SaveChangesAsync();

            return emp;
        }

         [HttpDelete("del/{id}")]
         public async Task<ActionResult<Employee>> RemoveEmployee(int id)
         {
            var emp = _context.Employees.Find(id);

            if(emp == null) return BadRequest("Cant Find Any Employee");

            _context.Employees.Remove(emp);

            await _context.SaveChangesAsync();

            return Ok("Employee deleted from base");
         }

          [HttpPut("update/{id}")]

          public async Task<ActionResult<Employee>> UpdateEmployee(int id,[FromBody]Employee employee)
          {
            var emp = _context.Employees.Find(id);

            if(emp == null) return BadRequest("Cant Find Any Employee");
            
            emp.Name = employee.Name;
            emp.Surname = employee.Surname;
            emp.BirthDate =employee.BirthDate;
            emp.DepartmentId = employee.DepartmentId;
            emp.CreateDate =DateTime.Now;

            _context.Employees.Update(emp);

            await _context.SaveChangesAsync();

            return emp;
          }

          [HttpGet("filter")]
          public async Task<ActionResult<ICollection<Employee>>> GetFilteredEmployees(string ? name=null,string? surname=null,int? depid=0)
          {
            ICollection<Employee> employees = _context.Employees.ToList();

            if(name != null)
            {
                employees = employees.Where(e =>e.Name.ToLower().Contains(name.ToLower())).ToList();
            }
            if(surname != null)
            {
                employees = employees.Where(e =>e.Surname.ToLower().Contains(surname.ToLower())).ToList();
            }
            if(depid != 0)
            {
                employees = employees.Where(e =>e.DepartmentId==depid).ToList();
            } 

            return employees.ToList();
          }

    }
}