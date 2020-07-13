using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/camps/{moniker}/talks")]
    [ApiController]
    public class TalksController : ControllerBase
    {
        private readonly ICampRepository campRepository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;

        public TalksController(ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this.campRepository = campRepository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> Get(string moniker, bool includeSpeakers = true)
        {
            try
            {
                var result = await campRepository.GetTalksByMonikerAsync(moniker, includeSpeakers);
                if (result is null)
                    return NotFound();
                return mapper.Map<TalkModel[]>(result);
            }
            catch (Exception)
            {

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred.");
            }

        }

        [HttpGet("{talkId:int}")]
        public async Task<ActionResult<TalkModel>> Get(
            string moniker, int talkId, bool includeSpeakers = true)
        {
            try
            {
                var result = await campRepository.GetTalkByMonikerAsync(
                    moniker, talkId, includeSpeakers);
                if (result is null)
                    return NotFound();
                return mapper.Map<TalkModel>(result);
            }
            catch (Exception)
            {

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred.");
            }

        }

        [HttpPost]
        public async Task<ActionResult<TalkModel>> Post(string moniker, TalkModel model)
        {
            try
            {

                var camp = await campRepository.GetCampAsync(moniker);
                if (camp is null)
                    return BadRequest("Camp does not exist for this moniker");

                Talk talk = mapper.Map<Talk>(model);
                talk.Camp = camp;

                Speaker speaker = await campRepository.GetSpeakerAsync(talk.Speaker.SpeakerId);
                if (speaker is null)
                    return BadRequest("Speaker is required.");
                talk.Speaker = speaker;

                campRepository.Add(talk);

                if (await campRepository.SaveChangesAsync())
                {
                    var location = linkGenerator.GetPathByAction(
                        "Get", "Talks", new { moniker, talk.TalkId });
                    return Created(location, mapper.Map<TalkModel>(talk));

                }
                else
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Error saving to database");
            }
            catch (Exception)
            {

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred.");
            }
        }

        [HttpPut("{talkId:int}")]
        public async Task<ActionResult<TalkModel>> Put(string moniker, int talkId, TalkModel model)
        {
            try
            {
                var talk = await campRepository.GetTalkByMonikerAsync(moniker, talkId, true);
                if (talk is null)
                    return NotFound("Could not find this talk.");

                if (model.Speaker != null)
                {
                    var speaker = await campRepository.GetSpeakerAsync(model.Speaker.SpeakerId);
                    if (speaker != null)
                        talk.Speaker = speaker;
                    else
                        return BadRequest("Speaker not found.");
                }

                mapper.Map(model, talk);

                if (await campRepository.SaveChangesAsync())
                {
                    var location = linkGenerator.GetPathByAction(
                        "Get", "Talks", new { moniker, talk.TalkId });
                    return Created(location, mapper.Map<TalkModel>(talk));

                }
                else
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Error saving to database");
            }
            catch (Exception)
            {

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred.");
            }

        }

        [HttpDelete("{talkId:int}")]
        public async Task<IActionResult> Delete(
            string moniker, int talkId)
        {
            try
            {
                var result = await campRepository.GetTalkByMonikerAsync(
                    moniker, talkId, false);
                if (result is null)
                    return NotFound();

                campRepository.Delete(result);

                if (await campRepository.SaveChangesAsync())
                    return Ok();
                else
                    return StatusCode(
                        StatusCodes.Status500InternalServerError, "Error saving changes.");
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred.");
            }

        }

    }
}
