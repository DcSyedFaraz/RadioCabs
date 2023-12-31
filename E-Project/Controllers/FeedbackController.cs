﻿using E_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public FeedbackController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/<FeedbackController>
        [HttpGet]
        public IActionResult Get()
        {
            var Feedbackss = _context.Feedbacks.ToList();

            return Ok(Feedbackss);
        }

        // GET api/<FeedbackController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<FeedbackController>
        [HttpPost]
        public  IActionResult Post([FromBody] Feedback value)
        {
            _context.Feedbacks.Add(value);
             _context.SaveChanges();

            return Ok(new Responce
            {
                Status = "Success",
                Message = "Successfully Submitted Feedback"
            });
        }

        // PUT api/<FeedbackController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        [HttpGet("{id}")]
        public async Task<ActionResult<Feedback>> GetFeedback(int id)
        {
            var advertisement = await _context.Feedbacks.FirstOrDefaultAsync(a => a.Id == id);

            if (advertisement == null)
            {
                return NotFound();
            }
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
            };

            var jsonString = JsonSerializer.Serialize(advertisement, options);

            return Ok(jsonString);
        }

        // DELETE api/<FeedbackController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var feed =  _context.Feedbacks.Find(id);
            if (feed == null)
            {
                return NotFound();
            }

            _context.Feedbacks.Remove(feed);
             _context.SaveChanges();

            return Ok(new Responce
            {
                Status = "Success",
                Message = "Successfully Deleted Feedback's Data"
            });
        }
    }
}
