﻿using Codedberries.Helpers;
using Codedberries.Models.DTOs;
using Codedberries.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codedberries.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly StatusService _statusService;

        public StatusController(StatusService statusService)
        {
            _statusService = statusService;
        }

        [HttpPost("createStatus")]
        public async System.Threading.Tasks.Task<IActionResult> CreateStatus([FromBody] StatusCreationDTO statusDTO)
        {
            try
            {
                await _statusService.CreateStatus(HttpContext, statusDTO);

                return Ok("Status successfully created.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorMsg(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorMsg($"An error occurred while creating the status: {ex.Message}"));
            }
        }

        [HttpGet("allStatuses")]
        public IActionResult GetAllStatuses()
        {
            try
            {
                var statuses = _statusService.GetStatuses(HttpContext);

                return Ok(statuses);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorMsg(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMsg($"An error occurred while fetching statuses: {ex.Message}"));
            }
        }
    }
}