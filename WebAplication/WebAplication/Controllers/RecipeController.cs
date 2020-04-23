using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAplication.Models;
using WebAplication.Services;

namespace WebAplication.Controllers
{
    [ApiController]
    [Route("api/prescriptions")]
    public class RecipeController : ControllerBase
    {
        private readonly IDbService _dbService;

        public RecipeController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet("{id}")]
        public IActionResult GetRecipe(int Id)
        {
                return Ok(_dbService.GetPrescriptionById(Id));
        }

        [HttpPost]
        public async Task<IActionResult> RegisterNewPrescription(Presc input)
        {
            var doc = await _dbService.GetDoctor(input.IdPatient);
            var pat = await _dbService.GetPatient(input.IdDoctor);
            if (input.DueDate < input.Date)
            {
                return BadRequest();
            }
            if (doc == null)
            {
                return BadRequest();
            }
            if (pat == null)
            {
                return BadRequest();
            }
            else
            {
                await _dbService.Register(input.Date, input.DueDate, input.IdPatient, input.IdDoctor);
                return StatusCode(201);
            }
        }

    }
}
