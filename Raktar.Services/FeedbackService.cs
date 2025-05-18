using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Raktar.DataContext;
using Raktar.DataContext.DataTransferObjects;
using Raktar.DataContext.Entities;

namespace Raktar.Services
{
    public interface IFeedbackService
    {
        Task<FeedbackDTO> CreateFeedbackAsync(FeedbackCreateDTO feedbackCreate);
        Task<FeedbackDTO> GetFeedbackIdAsync(int id);
    }

    public class FeedbackService : IFeedbackService
    {
        private readonly WarehouseDbContext _context;
        private readonly IMapper _mapper;

        public FeedbackService(WarehouseDbContext warehouseDbContext, IMapper mapper)
        {
            this._context = warehouseDbContext;
            _mapper = mapper;
        }

        public async Task<FeedbackDTO> CreateFeedbackAsync(FeedbackCreateDTO feedbackCreateDTO)
        {
            var feedback = _mapper.Map<Feedback>(feedbackCreateDTO);

            await _context.Feedbacks.AddAsync(feedback);
            await _context.SaveChangesAsync();
            return _mapper.Map<FeedbackDTO>(feedback);

        }

        public async Task<FeedbackDTO> GetFeedbackIdAsync(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback==null)
            {
                throw new KeyNotFoundException("Feedback not found.");
            }
            return _mapper.Map<FeedbackDTO>(feedback);
        }
        public async Task<IList<FeedbackDTO>> GetAllFeedbacksAsync()
        {
            var feedbacks = await _context.Feedbacks.ToListAsync();
            return _mapper.Map<IList<FeedbackDTO>>(feedbacks);
        }
    }
}
