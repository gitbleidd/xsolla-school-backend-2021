using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XsollaSchoolBackend.Data;
using XsollaSchoolBackend.Models;

namespace XsollaSchoolBackend.Controllers
{
    [Route("api/merch/{itemId:int}/comments")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepository _repository;

        public CommentsController(ICommentRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{commentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Comment> GetCommentById(int commentId)
        {
            var res = _repository.GetCommentById(commentId);

            if (res == null)
                return NotFound();
            return Ok(res);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<Item>> GetAllComments(int itemId)
        {
            var res = _repository.GetAllComments(itemId);

            return Ok(res);
        }

        [HttpDelete("{commentId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult DeleteComment(int commentId)
        {
            bool isDeleted = _repository.DeleteComment(commentId);
            if (isDeleted)
                return NoContent();
            return BadRequest();
        }

        [HttpPut("{commentId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateComment(int commentId, [FromBody] Comment newComment)
        {
            var isUpdated = _repository.UpdateComment(commentId, newComment);
            if (isUpdated)
                return NoContent();
            return NotFound();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Comment> CreateNewComment(int itemId, [FromBody] Comment newComment)
        {
            var res = _repository.CreateNewComment(itemId, newComment);
            return Created("", newComment);
        }
    }
}
