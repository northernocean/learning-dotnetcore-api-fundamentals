using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository campRepository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;

        public CampsController(
            ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this.campRepository = campRepository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<CampModel[]>> Get(bool includeTalks = false)
        {
            try
            {
                var camps = await campRepository.GetAllCampsAsync(includeTalks);
                return mapper.Map<CampModel[]>(camps);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Error.");
            }
        }

        [HttpGet("{moniker}")]
        [MapToApiVersion("1.0")]
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
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Error.");
            }
        }
        
        [HttpGet("{moniker}")]
        [MapToApiVersion("1.1")]
        public async Task<ActionResult<CampModel>> Get_1_1(string moniker)
        {
            try
            {
                var camp = await campRepository.GetCampAsync(moniker, true);
                if (camp is null)
                    return NotFound();
                return mapper.Map<CampModel>(camp);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Error.");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<CampModel[]>> SearchByDate(DateTime theDate, bool includeTalks = false)
        {
            try
            {
                var result = await campRepository.GetAllCampsByEventDate(theDate, includeTalks);
                if (!result.Any())
                    return NotFound();
                return mapper.Map<CampModel[]>(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Error.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CampModel>> Post(CampModel campModel)
        {

            var x = await campRepository.GetCampAsync(campModel.Moniker);
            if (x != null)
            {
                campRepository.Delete(x);
                await campRepository.SaveChangesAsync();
                //return BadRequest("Monikor is already in use for another camp.");
            }

            try
            {
                var camp = mapper.Map<Camp>(campModel);
                campRepository.Add(camp);
                if (await campRepository.SaveChangesAsync())
                {
                    var location = linkGenerator.GetPathByAction("Get", "Camps",
                                                            new { moniker = campModel.Moniker });
                    var campInDb = mapper.Map<CampModel>(camp);
                    return Created(location, campInDb);

                }
                else
                    return StatusCode(StatusCodes.Status500InternalServerError);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker, CampModel campModel)
        {
            try
            {
                var campInDb = await campRepository.GetCampAsync(moniker, true);
                if (campInDb is null)
                    return NotFound("Camp not found");
                else
                {
                    mapper.Map(campModel, campInDb);
                    await campRepository.SaveChangesAsync();
                    return mapper.Map<CampModel>(campInDb);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{moniker}")]
        public async Task<ActionResult<CampModel>> Delete(string moniker)
        {
            try
            {
                var campInDb = await campRepository.GetCampAsync(moniker, true);
                if (campInDb is null)
                    return NotFound("Camp not found");
                else
                {
                    campRepository.Delete(campInDb);
                    await campRepository.SaveChangesAsync();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
