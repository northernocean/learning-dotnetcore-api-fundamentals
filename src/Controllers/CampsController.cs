using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository campRepository;
        private readonly IMapper mapper;

        public CampsController(ICampRepository campRepository, IMapper mapper)
        {
            this.campRepository = campRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<CampModel[]>> Get()
        {
            try
            {
                var camps = await campRepository.GetAllCampsAsync();
                return mapper.Map<CampModel[]>(camps);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure.");
            }
        }

        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModel>> Get(string moniker)
        {
            try
            {
                var camp = await campRepository.GetCampAsync(moniker);
                if (camp is null)
                    return NotFound();
                return mapper.Map<CampModel>(camp);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure.");
            }
            

        }

    }
}
