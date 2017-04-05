using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using RuTubeAPI.Models;
using RuTubeAPI.Services.DataAccess;
using RuTubeAPI.Services.Services;
namespace RuTubeAPI.Controllers
{
    [Route("api/kings")]
    public class KingsController : Controller
    {
        private readonly KingsServiceProvider _service;

        public KingsController(IUnitOfWork uow)
        {
            _service = new KingsServiceProvider(uow);
        }

        [HttpGet]
        [Route("", Name = "GetAllKings")]
        public IActionResult GetAllKings()
        {
            try
            {
                return new ObjectResult(_service.GetAllKings());
            }
            catch (UserDoesNotExistException)
            {
                return StatusCode(404);
            }
        }

        [HttpGet]
        [Route("ja")]
        public String GetTest()
        {
            return "ja marr";
        }
    }
}