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
        Task<IList<FeedbackDTO>> GetAllFeedbacksAsync();
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
            // Map the DTO to a Feedback entity.
            var feedback = _mapper.Map<Feedback>(feedbackCreateDTO);

            // Load the order including its OrderItems using the specified OrderId.
            var order = await _context.Orders
                          .Include(o => o.OrderItems)
                          .ThenInclude(oi => oi.Product) // if you plan to map product-related info later
                          .Include(o => o.User)         // if you plan to map UserId via OrderItem.Order.UserId
                          .FirstOrDefaultAsync(o => o.OrderId == feedbackCreateDTO.OrderId);

            if (order == null)
                throw new KeyNotFoundException("Order not found for feedback.");

            // Attach the order's items to the feedback.
            // Now feedback.OrderItems is not empty, so our mapping can extract the OrderId.
            feedback.OrderItems = order.OrderItems.ToList();

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
            // Load both OrderItems and, via them, the associated Order and Product.
            var feedbacks = await _context.Feedbacks
                .Include(f => f.OrderItems)
                    .ThenInclude(oi => oi.Order)
                .Include(f => f.OrderItems)
                    .ThenInclude(oi => oi.Product)  // This ensures Product is loaded.
                .ToListAsync();

            return _mapper.Map<IList<FeedbackDTO>>(feedbacks);
        }




    }
}
