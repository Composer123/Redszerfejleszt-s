﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Raktar.DataContext.DataTransferObjects;
using Raktar.Services;

namespace Raktar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpGet("reviews/{id})")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFeedbackById(int id)
        {
            var feedback = await _feedbackService.GetFeedbackIdAsync(id);
            return Ok(feedback);
        }


        [HttpPost("reviews")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> PostReview([FromBody] FeedbackCreateDTO feedbackDTO)
        {
            var feedback = await _feedbackService.CreateFeedbackAsync(feedbackDTO);
            return CreatedAtAction(nameof(GetFeedbackById), new { id = feedback.FeedbackId }, feedback);


        }



    }
}
