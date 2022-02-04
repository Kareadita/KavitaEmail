using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Skeleton.Entities;

namespace Skeleton.Data
{
    public interface IUnitOfWork
    {
        
    }
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public UnitOfWork(DataContext context, IMapper mapper, UserManager<AppUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }
        

        /// <summary>
        /// Commits changes to the DB. Completes the open transaction.
        /// </summary>
        /// <returns></returns>
        public bool Commit()
        {
            return _context.SaveChanges() > 0;
        }
        /// <summary>
        /// Commits changes to the DB. Completes the open transaction.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CommitAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Is the DB Context aware of Changes in loaded entities
        /// </summary>
        /// <returns></returns>
        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }

        /// <summary>
        /// Rollback transaction
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RollbackAsync()
        {
            await _context.DisposeAsync();
            return true;
        }
        /// <summary>
        /// Rollback transaction
        /// </summary>
        /// <returns></returns>
        public bool Rollback()
        {
            _context.Dispose();
            return true;
        }
    }
}
