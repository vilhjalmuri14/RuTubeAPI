using System;
using System.Collections.Generic;
using System.Linq;
using RuTubeAPI.Models;
using RuTubeAPI.Services.Exceptions;
using RuTubeAPI.Services.Models.Entities;
using RuTubeAPI.Services.Services;
using RuTubeAPI.Tests.MockObjects;
using Xunit;

namespace RuTubeAPI.Tests.Services
{
	public class ServicesTest
	{
		private MockUnitOfWork<MockDataContext> _mockUnitOfWork;
		private UserServiceProvider _service;
        private VideoServiceProvider _vidoeservice;

        private List<User> _users;
        private List<Video> _videos;
        private List<FavoriteVideosByUser> _favoriteVideosByUser;
        private List<UserFriend> _userFriend;
        private List<Channel> _Channel;
        private List<VideosByChannel> _videosByChannel;
        

        private const int USER1_ID = 1;
        private const string USER1_NAME = "John Johnsson";
        private const string USER1_PASS = "123johnsson";
        private const string USER1_INVALID_PASS = "wrongPass";
        private const string USER1_EMAIL = "john54@gmail.com";

        private const int USER2_ID = 2;
        private const string USER2_NAME = "Anna Simpsson";
        private const string USER2_PASS = "kass45jk";
        private const string USER2_TOKEN = "lkwje3kj4l2lk1æ";
        private const string USER2_EMAIL = "homer33@gmail.com";

        private const int USER3_ID = 3;
        private const string USER3_NAME = "Homer Smith";
        private const string USER3_PASS = "smith34KL";

        private const int VIDEO1_ID = 1;
        private const string VIDEO1_TITLE = "One great goal";
        private const string VIDEO1_DESCRIPTION = "Michael Owen scores for liverpool.";  

        private const int VIDEO2_ID = 2;
        private const string VIDEO2_TITLE = "Charley singing in the rain";
        private const string VIDEO2_DESCRIPTION = "Very funny video.";     

        private const int CHANNEL1_ID = 1;
        private const string CHANNEL1_TITLE = "Funny videos";
        private const string CHANNEL1_DESCRIPTION = "try not to laugh.";

		public ServicesTest()
		{
			_mockUnitOfWork = new MockUnitOfWork<MockDataContext>();

            #region Users
			_users = new List<User>
			{
				new User
				{
					ID          = USER1_ID,
					Name        = USER1_NAME,
					Password    = USER1_PASS,
					Token       = "j2j3jh4lkdljl234",
                    Email       = USER1_EMAIL
				},
                new User
                {
                    ID          = USER2_ID,
					Name        = USER2_NAME,
					Password    = USER2_PASS,
					Token       = USER2_TOKEN,
                    Email       = "anna@jbc.org"
                },
                new User
                {
                    ID          = USER3_ID,
					Name        = USER3_NAME,
					Password    = USER3_PASS,
					Token       = "sdlkfjiunvi324k1j",
                    Email       = "smith@klo.org"
                }
			};
			#endregion

            #region Videos
			_videos = new List<Video>
			{
				new Video
				{
					ID          = VIDEO1_ID,
					Title       = VIDEO1_TITLE,
					Description = VIDEO1_DESCRIPTION
				},
                new Video
                {
                    ID          = VIDEO2_ID,
					Title       = VIDEO2_TITLE,
					Description = VIDEO2_DESCRIPTION
                }
			};
			#endregion

            _favoriteVideosByUser  = new List<FavoriteVideosByUser>
            {
                new FavoriteVideosByUser
                {
                    ID = 1,
                    UserID = USER1_ID,
                    VideoID = VIDEO1_ID
                }
            };

            _userFriend = new List<UserFriend>
            {
                new UserFriend
                {
                    ID = 1,
                    UserID = USER1_ID,
                    FriendID = USER2_ID
                }
            };

            _Channel = new List<Channel>
            {
                new Channel
                {
                    ID = CHANNEL1_ID,
                    Title = CHANNEL1_TITLE,
                    Description = CHANNEL1_DESCRIPTION
                }
            };

            _videosByChannel = new List<VideosByChannel>
            {
                new VideosByChannel
                {
                    ID = 1,
                    VideoID = VIDEO1_ID,
                    ChannelID = CHANNEL1_ID
                }
            };

            _mockUnitOfWork.SetRepositoryData(_users);
            _mockUnitOfWork.SetRepositoryData(_videos);
            _mockUnitOfWork.SetRepositoryData(_favoriteVideosByUser);
            _mockUnitOfWork.SetRepositoryData(_userFriend);
            _mockUnitOfWork.SetRepositoryData(_Channel);
            _mockUnitOfWork.SetRepositoryData(_videosByChannel);

			_service = new UserServiceProvider(_mockUnitOfWork);
            _vidoeservice = new VideoServiceProvider(_mockUnitOfWork);
		}

        #region Account Service
        [Fact]
		public void CreateUserAndAuthenticate()
		{
            // Arrange:
            CreateUserViewModel NEW_USER = new CreateUserViewModel
            {
                Name        = "John Newman",
                Password    = "new78kiss",
                Email       = "john@jbk.com"
            };

			// Act:
            var user = _service.CreateUser(NEW_USER);

			// Assert:
			Assert.Equal(true, _service.IsAuthenticated(user.Token));
        }

		[Fact]
		public void CreateUserThatAlreadyExsists()
		{
			// Arrange:
            CreateUserViewModel EXSISTING_USER = new CreateUserViewModel
            {
                Name        = USER1_NAME,
                Password    = USER1_PASS,
                Email       = "john54@gmail.com"
            };

			// Act:

			// Assert:
			Assert.Throws<UserAlreadyExistException>( () => _service.CreateUser(EXSISTING_USER) );
		}

        [Fact]
		public void AuthenticateUserWithWrongPassword()
		{
			// Arrange:
            LoginViewModel INVALID_PASSWORD = new LoginViewModel
            {
                Name        = USER1_NAME,
                Password    = USER1_INVALID_PASS
            };

			// Act:

			// Assert:
			Assert.Throws<LogInException>( () => _service.LogInUser(INVALID_PASSWORD) );
		}
        #endregion

        #region User Service

        
        //View the profile of a user and confirm it correctly matches the expected profile
        [Fact]
		public void ViewUserProfile()
		{
            // Act:
            var user = _service.GetUserProfile(USER1_ID);

            // Assert:
            Assert.Equal(user.ID, USER1_ID);
            Assert.Equal(user.Name, USER1_NAME);
            Assert.Equal(user.Email, USER1_EMAIL);

            Assert.Equal(user.FavoriteVideos[0].Title, VIDEO1_TITLE);

            Assert.Equal(user.CloseFriends[0].Name, USER2_NAME);
        }

        //Add favorite videos to a user, read the profile back and confirm the list matches
        [Fact]
		public void AddVideosToUser()
		{
            _service.AddVideoToFavorites(USER1_ID, VIDEO2_ID);

            var user = _service.GetUserProfile(USER1_ID);

            Assert.Equal(user.FavoriteVideos[0].Title, VIDEO1_TITLE);
            Assert.Equal(user.FavoriteVideos[1].Title, VIDEO2_TITLE);
        }

        //Update the username of a user, read the profile and confirm it changed
        [Fact]
		public void UpdateUserNameOfUser()
		{
            // Arrange:
            CreateUserViewModel UPDATED_USER = new CreateUserViewModel
            {
                Name        = "newPassword345",
                Password    = USER2_PASS,
                Email       = USER2_EMAIL
            };

            // Act:
            _service.UpdateUser(USER2_TOKEN, UPDATED_USER);

            var user = _service.GetUserProfile(USER2_ID);

            // Assert:
            Assert.Equal(user.Name, UPDATED_USER.Name);
        }

        //Add a list of close friends to a profile and read it back to confirm it was stored properly
        [Fact]
		public void AddToCloseFriendsList()
		{
            _service.AddFriendToList(USER2_ID, USER1_ID);
            _service.AddFriendToList(USER2_ID, USER3_ID);

            var user = _service.GetUserProfile(USER2_ID);

            // Assert:
            Assert.Equal(user.CloseFriends[0].Name, USER1_NAME);
            Assert.Equal(user.CloseFriends[1].Name, USER3_NAME);
        }

        #endregion

        #region Video Service
        
        //Add a video to a channel and confirm it’s listed in “all videos”
        [Fact]
		public void AddVideoToChannelAndListedInAllVideos()
		{
            CreateVideoViewModel NEW_VIDEO = new CreateVideoViewModel
            {
                Title = "my baby laughing",
                Description = "my little sophie is so funny."
            };

            _vidoeservice.AddVideoToChannel(CHANNEL1_ID, NEW_VIDEO);

            var videos = _vidoeservice.GetAllVideos();

            // Assert:
            Assert.Equal(videos[0].Title, VIDEO1_TITLE);
            Assert.Equal(videos[1].Title, VIDEO2_TITLE);
            Assert.Equal(videos[2].Title, NEW_VIDEO.Title);
        }

        //Add a video to a channel and confirm it’s listed in that channel
        [Fact]
		public void AddVideoToChannelAndListedInChannel()
		{
            CreateVideoViewModel NEW_VIDEO = new CreateVideoViewModel
            {
                Title = "man falls into a hole",
                Description = "Try not to laugh."
            };

            _vidoeservice.AddVideoToChannel(CHANNEL1_ID, NEW_VIDEO);

            var videos = _vidoeservice.GetVideosFromChannel(CHANNEL1_ID);

            // Assert:
            Assert.Equal(videos[0].Title, VIDEO1_TITLE);
            Assert.Equal(videos[1].Title, NEW_VIDEO.Title);
            Assert.Equal(videos.Count, 2);
        }

        //Remove a video and confirm it’s not listed in all videos or the (previous) channel list
        [Fact]
		public void RemoveVideoAndNotListed()
		{
            var NumberOfVideosInChannel = _vidoeservice.GetVideosFromChannel(CHANNEL1_ID).Count;
            var NumberOfVideos = _vidoeservice.GetAllVideos().Count;

            _vidoeservice.DeleteVideoFromChannel(CHANNEL1_ID, VIDEO1_ID);

            // Assert:
            Assert.Equal(NumberOfVideosInChannel - 1, _vidoeservice.GetVideosFromChannel(CHANNEL1_ID).Count);
            Assert.Equal(NumberOfVideos - 1, _vidoeservice.GetAllVideos().Count);
        }

        #endregion
	}
    
}
