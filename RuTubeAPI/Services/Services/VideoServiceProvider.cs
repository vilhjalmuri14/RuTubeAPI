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
    public class ChannelDoesNotExistException : Exception {}
    public class VideoAlreadyExistException : Exception {} 
    public class VideoNotInChannelException : Exception {}

	public class VideoServiceProvider
	{
        // all the DB tables
		private readonly IUnitOfWork _uow;
        private readonly IRepository<Video> _videos;
        private readonly IRepository<VideosByChannel> _videosByChannel;
        private readonly IRepository<Channel> _Channel;

		public VideoServiceProvider(IUnitOfWork uow)
		{
			_uow = uow;
            _videos                 = _uow.GetRepository<Video>();
            _videosByChannel        = _uow.GetRepository<VideosByChannel>(); 
            _Channel                = _uow.GetRepository<Channel>(); 
		}

        public List<VideoDTO> GetAllVideos()
        {
            var allVideos = (from v in _videos.All()
				select new VideoDTO{
                    ID = v.ID,
                    Title = v.Title,
                    Description = v.Description
                }).ToList();

            return allVideos;
        }

        private Boolean DoesChannelExist(int id)
        {
            var result = (from c in _Channel.All()
                    where c.ID == id
                    select c).SingleOrDefault();

            if(result != null)
            {
                return true;
            }
            return false;
        }

        public List<VideoDTO> GetVideosFromChannel(int id)
        {
            if(!DoesChannelExist(id))
            {
                throw new ChannelDoesNotExistException();
            }

            var videos = (from c in _videosByChannel.All()
                        where c.ChannelID == id
                        join f in _videos.All() on c.VideoID equals f.ID
                        select new VideoDTO{
                            ID = f.ID,
                            Title = f.Title,
                            Description = f.Description
                        }).ToList();
            
            return videos;
        }

        private int nextVideoID()
        {
            int maxID =  _videos.All().Max(p => p.ID);

            if(maxID == 0)
            {
                return 1;
            }
            return maxID + 1;
        }

        public Video AddVideoToChannel(int channel,CreateVideoViewModel model)
        {
            if(!DoesChannelExist(channel))
            {
                throw new ChannelDoesNotExistException();
            }

            // check if a video with same title exists
            var video = (from v in _videos.All()
                        where v.Title == model.Title
                        select v).SingleOrDefault();

            if(video != null) 
            {
                throw new VideoAlreadyExistException();
            }

            Video newVideo = new Video {
                ID = nextVideoID(),
                Title = model.Title,
                Description = model.Description
            };

            _videos.Add(newVideo);

            VideosByChannel vc = new VideosByChannel {
                 VideoID = newVideo.ID,
		         ChannelID = channel
            };

            _videosByChannel.Add(vc);

            try
            {
                _uow.Save();
            }
            catch (System.Exception)
            {
                
            }

            return newVideo;
        }

        public void DeleteVideoFromChannel(int channel, int videoId)
        {
            if(!DoesChannelExist(channel))
            {
                throw new ChannelDoesNotExistException();
            }

            var video = (from v in _videos.All()
                        where v.ID == videoId
                        select v).SingleOrDefault();

            if(video == null)
            {
                throw new VideoDoesNotExistException();
            }

             var videoInChannel = (from vc in _videosByChannel.All()
                        where vc.ChannelID == channel && vc.VideoID == videoId
                        select vc).SingleOrDefault();

            if(videoInChannel == null)
            {
                throw new VideoNotInChannelException();
            }

            _videos.Delete(video);
            _videosByChannel.Delete(videoInChannel);

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