using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using RuTubeAPI.Models;
using RuTubeAPI.Services.DataAccess;
using RuTubeAPI.Services.Services;

namespace RuTubeAPI.Controllers
{
	[Route("api/users")]
	public class AccountController : Controller
	{
		private readonly UserServiceProvider _service;

		public AccountController(IUnitOfWork uow)
		{
			_service = new UserServiceProvider(uow);
		}

		[HttpPost]
		public IActionResult CreateUser(CreateUserViewModel model)
		{
            if(model.Name == null || model.Password == null)
            {
                return BadRequest();
            }

			try
			{
				var result = _service.CreateUser(model);

            	var location = Url.Link("GetUserProfile", new { id = result.ID } );
				return Created(location, result);
			}
			catch(UserAlreadyExistException)
			{
				return StatusCode(412);
			}
		}

        [HttpPost]
		[Route("login")]
		public IActionResult LogInUser(LoginViewModel model)
		{
			try
			{
				return Ok(_service.LogInUser(model));
			}
			catch(LogInException)
			{
				return StatusCode(404); 
			}
		}

        [HttpPut]
		public IActionResult UpdateUser(CreateUserViewModel model)
		{
			//TODO: get token from header
			var token = "sdfsd";

			if(model.Name == null || model.Password == null)
            {
                return BadRequest();
            }

			try
			{
				_service.UpdateUser(token, model);
			}
			catch(UserDoesNotExistException)
			{
				return StatusCode(412);
			}
			return StatusCode(202);
		}

        [HttpDelete]
        [Route("{id:int}")]
		public IActionResult DeleteUser(int id)
		{
			try
			{
				_service.DeleteUser(id);
			}
			catch( UserDoesNotExistException )
			{
				return StatusCode(404); 
			}

			return StatusCode(202);
		}
    }
}
