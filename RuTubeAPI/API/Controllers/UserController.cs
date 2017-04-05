using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using RuTubeAPI.Models;
using RuTubeAPI.Services.DataAccess;
using RuTubeAPI.Services.Services;

namespace RuTubeAPI.Controllers
{
	[Route("api/users")]
	public class UserController : Controller
	{
		private readonly UserServiceProvider _service;

		public UserController(IUnitOfWork uow)
		{
			_service = new UserServiceProvider(uow);
		}

        /// <summary>
        /// Returns the profile of a user. id, name, email and list of favorite videos and list of close friends.
        /// User has to be authenticated to use it. (has to send "Authorization" in http header)
        ///	 
        /// returns status code 401 if user is not authenticated.
        /// returns status code 404 if no user with that id is found.
        /// </summary>
        /// <param name=""{id:int}""></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
		[Route("{id:int}", Name="GetUserProfile")]
		public IActionResult GetUserProfile(int id)
		{
			if(!_service.IsAuthenticated(Request.Headers["Authorization"]))
			{
				return StatusCode(401);
			}

            try
			{
				return new ObjectResult(_service.GetUserProfile(id));
			}
			catch(UserDoesNotExistException)
			{
				return StatusCode(404); 
			}
		}

		/// <summary>
        /// Returns a list favorite videos by user.
		/// User has to be authenticated to use it. (has to send "Authorization" in http header)
        ///	 
		/// returns status code 401 if user is not authenticated.
		/// returns status code 404 if no user with that id is found.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		[HttpGet]
		[Route("{id:int}/favorites")]
		public IActionResult GetFavoritesVideosByUser(int id)
		{
			if(!_service.IsAuthenticated(Request.Headers["Authorization"]))
			{
				return StatusCode(401);
			}

            try
			{
				return new ObjectResult(_service.GetFavoritesVideosByUser(id));
			}
            catch( UserDoesNotExistException )
			{
				return StatusCode(404);
            }
		}

		/// <summary>
        /// Add videos to the favorite list.
		/// User has to be authenticated and each user is only allowed to add to his list.
        /// 
		/// returns status code 403 if user is not authorized.
		/// returns status code 404 if no user with that id is found.
		/// returns status code 412 if video is already in the list.
		/// </summary>
        /// <param name="id"></param>
        /// <param name="VideoId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id:int}/favorites")]
		public IActionResult AddVideoToFavorites(int id, int VideoId)
		{
			// check if this useToken is allowed to perform actions on user with this id.
			if(!_service.IsAllowed(Request.Headers["Authorization"], id))
			{
				return StatusCode(403);
			}

			try
			{
				_service.AddVideoToFavorites(id, VideoId);
			}
			catch(Exception ex)
			{
				if(ex is UserDoesNotExistException || ex is VideoDoesNotExistException)
				{
					return StatusCode(404);
				}
				if(ex is VideoAlreadyInFavoritesException)
				{
					return StatusCode(412);
				}
				throw;
			}

			return StatusCode(201);
		}

		/// <summary>
        /// Removes a video from the favorite list.
		/// User has to be authenticated and each user is only allowed to remove from his list.
        /// 
		/// returns status code 403 if user is not authorized.
		/// returns status code 404 if no user with that id is found.
		/// returns status code 412 if video is already in the list.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="videoId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:int}/favorites/{videoId:int}")]
		public IActionResult DeleteVideoFromFavorites(int id, int videoId)
		{
			// check if this useToken is allowed to perform actions on user with this id.
			if(!_service.IsAllowed(Request.Headers["Authorization"], id))
			{
				return StatusCode(403);
			}

			try
			{
				_service.DeleteVideoFromFavorites(id, videoId);
			}
			catch( Exception ex )
			{
				if(ex is UserDoesNotExistException)
				{
					return StatusCode(404);
				}
				if(ex is VideoNotInFavoritesException)
				{
					return StatusCode(412);
				}
				throw;
			}

			return StatusCode(202);
		}

		/// <summary>
        /// Retuns a list of close friends by user id.
		/// User has to be authenticated.
        /// 
		/// returns status code 401 if user is not authenticated.
		/// returns status code 404 if no user with that id is found.
		/// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
		[Route("{id:int}/friends")]
		public IActionResult GetFriendsByUser(int id)
		{
			if(!_service.IsAuthenticated(Request.Headers["Authorization"]))
			{
				return StatusCode(401);
			}

            try
			{
				return new ObjectResult(_service.GetFriendsByUser(id));
			}
            catch(UserDoesNotExistException)
			{
				return StatusCode(404);
            }
		}

		/// <summary>
    	/// Adds a user to the close friends list
		/// User has to be authenticated and each user is only allowed to add to his list.
        /// 
		/// returns status code 403 if user is not authorized.
		/// returns status code 404 if no user with that id is found.
		/// returns status code 412 if user is already in the list.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="FriendId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id:int}/friends")]
		public IActionResult AddFriendToList(int id, int FriendId)
		{
			// check if this useToken is allowed to perform actions on user with this id.
			if(!_service.IsAllowed(Request.Headers["Authorization"], id))
			{
				return StatusCode(403);
			}

			try
			{
				_service.AddFriendToList(id, FriendId);
			}
			catch(Exception ex)
			{
				if(ex is UserDoesNotExistException)
				{
					return StatusCode(404);
				}
				if(ex is FriendAlreadyInListException)
				{
					return StatusCode(412);
				}
				throw;
			}

			return StatusCode(201);
		}

		/// <summary>
        /// Removes a user from the close friends list
		/// User has to be authenticated and each user is only allowed to remove from his list.
        /// 
		/// returns status code 403 if user is not authorized.
		/// returns status code 404 if no user with that id is found.
		/// returns status code 412 if friend is not in the list.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="friendId"></param>
        /// <returns></returns>
		[HttpDelete]
        [Route("{id:int}/friends/{friendId:int}")]
		public IActionResult DeleteFriendFromList(int id, int friendId)
		{
			// check if this useToken is allowed to perform actions on user with this id.
			if(!_service.IsAllowed(Request.Headers["Authorization"], id))
			{
				return StatusCode(403);
			}

			try
			{
				_service.DeleteFriendFromList(id, friendId);
			}
			catch( Exception ex )
			{
				if(ex is UserDoesNotExistException)
				{
					return StatusCode(404);
				}
				if(ex is FriendNotInListException)
				{
					return StatusCode(412);
				}
				throw;
			}

			return StatusCode(202);
		}
	}
}
