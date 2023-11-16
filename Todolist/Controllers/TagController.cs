using Microsoft.AspNetCore.Mvc;
using Todolist.Data;
using Todolist.Models;

namespace Todolist.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : Controller
    {
        private readonly TodoListDBContext _todoListDBContext;

        public TagController(TodoListDBContext todoListDBContext)
        {
            _todoListDBContext = todoListDBContext;
        }
        [HttpGet]
        public IActionResult GetTag()
        {
            var tag = _todoListDBContext.Tags.ToList();
            return Ok(tag);
        }

        [HttpGet("{id}")]
        public IActionResult GetTagById(int id)
        {
            var tag = _todoListDBContext.Tags.SingleOrDefault(t => t.TagID == id);
            if (tag != null)
            {
                return Ok(tag);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult AddTag([FromBody] Tags tags)
        {
            try
            {
                if (tags == null)
                {
                    return BadRequest("Dữ liệu không hợp lệ.");
                }

                if (string.IsNullOrEmpty(tags.TagName))
                {
                    return BadRequest("Tên danh mục không được để trống.");
                }

                var tag = new Tags
                {
                    TagName = tags.TagName
                };

                _todoListDBContext.Tags.Add(tag);
                _todoListDBContext.SaveChanges();

                return new JsonResult("Thêm mới thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi không xác định: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public IActionResult EditTag(int id, Tags tags)
        {
            var tag = _todoListDBContext.Tags.SingleOrDefault(t => t.TagID == id);
            if (tag != null)
            {
                tag.TagName = tags.TagName;
                _todoListDBContext.SaveChanges();

                return new JsonResult("Sửa thành công");
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTag(int id)
        {
            try
            {
                var tag = _todoListDBContext.Tags.SingleOrDefault(t => t.TagID == id);
                if (tag == null)
                {
                    return NotFound();
                }
                _todoListDBContext.Remove(tag);
                _todoListDBContext.SaveChanges();
                return new JsonResult("Xoá thành công");
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetAllTagName")]
        public IActionResult GetAllTags()
        {
            var tagNames = _todoListDBContext.Tags.Select(t => t.TagName).ToList();
            return Ok(tagNames);
        }
    }
}
