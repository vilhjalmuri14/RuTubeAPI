using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using RuTubeAPI.Models;
using RuTubeAPI.Services.DataAccess;
using RuTubeAPI.Services.Services;

namespace RuTubeAPI.Controllers
{
	[Route("api/")]
	public class VideoController : Controller
	{
		private readonly VideoServiceProvider _service;
		private readonly UserServiceProvider _userservice;

		// the admin token is just hardcoded in this assignment.
		private string adminToken = "theAdminToken";

		public VideoController(IUnitOfWork uow)
		{
			_service = new VideoServiceProvider(uow);
			_userservice = new UserServiceProvider(uow);
		}

		/// <summary>
        /// Returns a list of all videos.
		/// User does not have to be authenticated to use it.
        ///
		/// </summary>
        /// <param name="GetAllVideos("></param>
        /// <returns></returns>
        [HttpGet]
        [Route("videos")]
		public IActionResult GetAllVideos()
		{
			return Ok(_service.GetAllVideos());
		}

		/// <summary>
        /// Returns a list of video by channel id.
		///
        /// Returns status code 404 if no channel with that id is found.
		/// </summary>
        /// <param name=""channels/{id:int}/videos""></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("channels/{id:int}/videos", Name="GetVideosByChannel")]
		public IActionResult GetVideosFromChannel(int id)
		{
            try
			{
				return new ObjectResult(_service.GetVideosFromChannel(id));
			}
			catch(ChannelDoesNotExistException)
			{
				return StatusCode(404); 
			}
		}

		/// <summary>
        /// Adds a new video to the channel with that id.
		/// User has to be authenticated to use it.
        ///  
		/// Returns status code 412 if a video with same title exists.
		/// Returns status code 401 if user is not authenticated.
		/// Returns status code 404 if no user with that id is found.
		/// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("channels/{id:int}/videos")]
		public IActionResult AddVideoToChannel(int id, CreateVideoViewModel model)
		{
			if(!_userservice.IsAuthenticated(Request.Headers["Authorization"]))
			{
				return StatusCode(401);
			}


            if(model.Title == null || model.Description == null)
            {
                return BadRequest();
            }

            try
			{
				var result = _service.AddVideoToChannel(id, model);

            	var location = Url.Link("GetVideosByChannel", new { id = id } );
				return Created(location, result);
			}
            catch( Exception ex )
			{
				if(ex is ChannelDoesNotExistException)
				{
					return StatusCode(404);
				}
				if(ex is VideoAlreadyExistException)
				{
					return StatusCode(412);
				}
				throw;
			}
        }

		/// <summary>
        /// Removes the video from the channel and the database.
		/// Only admin is allowed to use it.
        /// 
		/// Returns status code 412 if a video with that id is not in the channel.
		/// Returns status code 403 if user is not authorized.
		/// Returns status code 404 if no channel with that id is found.
		/// </summary>
        /// <param name="id"></param>
        /// <param name="videoId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("channels/{id:int}/videos/{videoId:int}")]
		public IActionResult DeleteVideoFromChannel(int id, int videoId)
		{
			// only the admin is allowed to delete videos
			if(Request.Headers["Authorization"] == adminToken)
			{
				return StatusCode(403);
			}

			try
			{
				_service.DeleteVideoFromChannel(id, videoId);
			}
			catch( Exception ex )
			{
				if(ex is ChannelDoesNotExistException || ex is VideoDoesNotExistException)
				{
					return StatusCode(404);
				}
				if(ex is VideoNotInChannelException)
				{
					return StatusCode(412);
				}
				throw;
			}

			return StatusCode(202);
		}
    }
}