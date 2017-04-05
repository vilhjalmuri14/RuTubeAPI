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
    public class KingsServiceProvider
    {
        // all the DB tables
        private readonly IUnitOfWork _uow;
        private readonly IRepository<kings> _kings;
        public KingsServiceProvider(IUnitOfWork uow)
        {
            _uow = uow;

            _kings = _uow.GetRepository<kings>();
        }

        public List<kings> GetAllKings()
        {
            var kings = (from u in _kings.All()
                         select u).ToList();

            return kings;
        }
    }
}
