using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelListing.Data;
using HotelListing.IRepository;

namespace HotelListing.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _context;

        private IGenericRepository<Country> _countries;

        private IGenericRepository<Hotel> _hotels;

        public UnitOfWork(DatabaseContext context)
        {
            _context = context;
        }

        public IGenericRepository<Country> Countries => _countries ??= new GenericRepository<Country>(_context); // if countries == null then countries = new...
        public IGenericRepository<Hotel> Hotels => _hotels ??= new GenericRepository<Hotel>(_context); // if hotels == null then hotels = new...


        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
        
        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

    }
}
