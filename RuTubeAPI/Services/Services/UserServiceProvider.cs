using System.Collections.Generic;
using System.Linq;
using RuTubeAPI.Models;
using RuTubeAPI.Services.DataAccess;
using RuTubeAPI.Services.Exceptions;
using RuTubeAPI.Services.Models.Entities;
using RuTubeAPI.Services.Utilities;
using System;


namespace RuTubeAPI.Services.Services
{
    // all the Exceptions
    public class LogInException : Exception {}
    public class UserDoesNotExistException : Exception {}
    public class UserAlreadyExistException : Exception {}
    public class VideoDoesNotExistException : Exception {}
    public class VideoAlreadyInFavoritesException : Exception {}   
    public class VideoNotInFavoritesException : Exception {} 
    public class FriendAlreadyInListException : Exception {} 
    public class FriendNotInListException : Exception {}

	public class UserServiceProvider
	{
        // all the DB tables
		private readonly IUnitOfWork _uow;
		private readonly IRepository<User> _users;
        private readonly IRepository<Video> _videos;
        private readonly IRepository<FavoriteVideosByUser> _favoriteVideosByUser;
        private readonly IRepository<UserFriend> _userFriend;

		public UserServiceProvider(IUnitOfWork uow)
		{
			_uow = uow;

			_users                  = _uow.GetRepository<User>();
            _videos                 = _uow.GetRepository<Video>();
            _favoriteVideosByUser   = _uow.GetRepository<FavoriteVideosByUser>(); 
            _userFriend            = _uow.GetRepository<UserFriend>();
		}

        /// <summary>
        /// returns true if a user with that token exist.
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Boolean IsAuthenticated(string userToken)
        {
            var user = (from u in _users.All()
                        where u.Token == userToken
                        select u).SingleOrDefault();

            if(user != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if a user with that token is the user with that id.
        ///  
        /// returns true if user is allowed to perform actions on a user with that id.
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Boolean IsAllowed(string userToken, int userId)
        {
            var user = (from u in _users.All()
                        where u.Token == userToken && u.ID == userId
                        select u).SingleOrDefault();
            
            if(user == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns the Token of the user which he can use to authenticate.
        ///
        /// If userName or Password is wrong then if throws LogInException.
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>        
        public string LogInUser(LoginViewModel model)
        {
            var user = (from u in _users.All()
                        where u.Name == model.Name && u.Password == model.Password
                        select u).SingleOrDefault();

            if(user == null)
            {
                throw new LogInException();
            }

            return user.Token;
        }

		public List<User> GetAllUsers()
		{
			var allUsers = (from c in _users.All()
				select c).ToList();

            return allUsers;
		}

        private Boolean DoesUserExist(int id)
        {
            var result = (from c in _users.All()
                    where c.ID == id
                    select c).SingleOrDefault();

            if(result != null)
            {
                return true;
            }
            return false;
        }

        private Boolean DoesVideoExist(int id)
        {
            var result = (from v in _videos.All()
                    where v.ID == id
                    select v).SingleOrDefault();

            if(result != null)
            {
                return true;
            }
            return false;
        }

        private int nextUserID()
        {
            int maxID =  _users.All().Max(u => u.ID);

            if(maxID == 0)
            {
                return 1;
            }
            return maxID + 1;
        }

        public UserProfileDTO GetUserProfile(int userId)
        {
            if(!DoesUserExist(userId)) 
            {
                throw new UserDoesNotExistException();
            }

            var user = (from u in _users.All()
                where u.ID == userId
                select new UserDTO
                {
                    ID = u.ID,
                    Name = u.Name,
                    Email = u.Email
                }).SingleOrDefault();

            var videos = GetFavoritesVideosByUser(userId);

            var friends = GetFriendsByUser(userId);

            UserProfileDTO profile = new UserProfileDTO
            {
                ID = user.ID,
                Name = user.Name,
                Email = user.Email,
                FavoriteVideos = videos,
                CloseFriends = friends
            };

            return profile;
        }

        public User CreateUser(CreateUserViewModel model)
        {
            // check if a user with same username exists
            var user = (from us in _users.All()
                        where us.Name == model.Name
                        select us).SingleOrDefault();

            if(user != null) 
            {
                throw new UserAlreadyExistException();
            }

            User u = new User {
                ID = nextUserID(),
                Name = model.Name,
                Password = model.Password,
                Token = Guid.NewGuid().ToString(),
                Email = model.Email
            };

            _users.Add(u);

            try
            {
                _uow.Save();
            }
            catch (System.Exception)
            {
                
            }

            return u;
        }

        /// <summary>
        ///  To update the name, password or email of a exsisting user.
        /// </summary>
        /// <param name="UserToken"></param>
        /// <param name="model"> with new username, password or email </param>
        public void UpdateUser(string UserToken, CreateUserViewModel model)
        {
            var user = (from u in _users.All()
                        where u.Token == UserToken
                        select u).SingleOrDefault();

            if(user == null)
            {
                throw new UserDoesNotExistException();
            }

            user.Name = model.Name;
            user.Password = model.Password;
            user.Email = model.Email;

            _users.Update(user);

            try
            {
                _uow.Save();
            }
            catch (System.Exception)
            {
                
            }
        }

        public void DeleteUser(int id)
        {
            if(!DoesUserExist(id))
            {
                throw new UserDoesNotExistException();
            }

            var result = (from u in _users.All()
                        where u.ID == id
                        select u).SingleOrDefault();

            _users.Delete(result);

            try
            {
                _uow.Save();
            }
            catch (System.Exception)
            {
                
            }
        }

        public List<VideoDTO> GetFavoritesVideosByUser(int id)
        {
            if(!DoesUserExist(id))
            {
                throw new UserDoesNotExistException();
            }

            var videos = (from fv in _favoriteVideosByUser.All()
                where fv.UserID == id
                join v in _videos.All() on fv.VideoID equals v.ID
                select new VideoDTO
                {
                    ID = v.ID,
                    Title = v.Title,
                    Description = v.Description
                }).ToList();

            return videos;
        }

        private Boolean IsVideoInFavorite(int UserId, int VideoId)
        {
            var InFavorite = (from i in _favoriteVideosByUser.All()
                    where i.UserID == UserId && i.VideoID == VideoId
                    select i).SingleOrDefault();

            if(InFavorite == null)
            {
                return false;
            }
            return true;
        }

        public void AddVideoToFavorites(int id, int VideoId)
        {
            if(!DoesUserExist(id))
            {
                throw new UserDoesNotExistException();
            }
            if(!DoesVideoExist(VideoId))
            {
                throw new VideoDoesNotExistException();
            }

            if(IsVideoInFavorite(id, VideoId))
            {
                throw new VideoAlreadyInFavoritesException();
            }
            
            FavoriteVideosByUser fv = new FavoriteVideosByUser {
		        UserID = id,
                VideoID = VideoId
            };

            _favoriteVideosByUser.Add(fv);

            try
            {
                _uow.Save();
            }
            catch (System.Exception)
            {
                
            }
        }

        public void DeleteVideoFromFavorites(int id, int videoId)
        {
            if(!DoesUserExist(id))
            {
                throw new UserDoesNotExistException();
            }

            if(!IsVideoInFavorite(id, videoId))
            {
                throw new VideoNotInFavoritesException();
            }

            var result = (from fl in _favoriteVideosByUser.All()
                        where fl.UserID == id && fl.VideoID == videoId
                        select fl).SingleOrDefault();

            _favoriteVideosByUser.Delete(result);

            try
            {
                _uow.Save();
            }
            catch (System.Exception)
            {
                
            }
        }

        public List<UserDTO> GetFriendsByUser(int id)
        {
            if(!DoesUserExist(id))
            {
                throw new UserDoesNotExistException();
            }

            var friends = (from u in _userFriend.All()
                    where u.UserID == id
                    join f in _users.All() on u.FriendID equals f.ID
                    select new UserDTO
                    {
                        ID = f.ID,
                        Name = f.Name,
                        Email = f.Email
                    }).ToList();
            
            return friends;
        }

        private Boolean IsFriendInList(int UserId, int friendId)
        {
            var InFriendList = (from u in _userFriend.All()
                    where u.UserID == UserId && u.FriendID == friendId
                    select u).SingleOrDefault();

            if(InFriendList == null)
            {
                return false;
            }
            return true;
        }

        public void AddFriendToList(int id, int friendId)
        {
            if(!DoesUserExist(id) || !DoesUserExist(friendId))
            {
                throw new UserDoesNotExistException();
            }

            if(IsFriendInList(id, friendId))
            {
                throw new FriendAlreadyInListException();
            }

            UserFriend uf = new UserFriend {
		        UserID = id,
                FriendID = friendId
            };

            _userFriend.Add(uf);

            try
            {
                _uow.Save();
            }
            catch (System.Exception)
            {
                
            }
        }

        public void DeleteFriendFromList(int id, int friendId)
        {
            if(!DoesUserExist(id))
            {
                throw new UserDoesNotExistException();
            }

            if(!IsFriendInList(id, friendId))
            {
                throw new FriendNotInListException();
            }

            var result = (from uf in _userFriend.All()
                        where uf.UserID == id && uf.FriendID == friendId
                        select uf).SingleOrDefault();

            _userFriend.Delete(result);

            try
            {
                _uow.Save();
            }
            catch (System.Exception)
            {
                
            }
        }
    
	}
}
