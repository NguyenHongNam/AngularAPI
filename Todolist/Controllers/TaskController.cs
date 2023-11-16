using Microsoft.AspNetCore.Mvc;
using Todolist.Data;
using Todolist.Models;

namespace Todolist.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : Controller
    {
        private readonly TodoListDBContext _todoListDBContext;
        public TaskController(TodoListDBContext todoListDBContext)
        {
            _todoListDBContext = todoListDBContext;
        }
        [HttpGet]
        public IActionResult GetTask()
        {
            var task = _todoListDBContext.Tasks.ToList();
            return Ok(task);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = _todoListDBContext.Tasks.SingleOrDefault(t => t.TaskID == id);
            if (task != null)
            {
                return Ok(task);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult AddTask(Tasks tasks)
        {
            {
                var task = new Tasks
                {
                    Title = tasks.Title,
                    Description = tasks.Description,
                    TagName = tasks.TagName,
                };

                _todoListDBContext.Add(task);
                _todoListDBContext.SaveChanges();

                return Ok(task);
            }
        }
        [HttpPut("{id}")]
        public IActionResult EditTask(int id, Tasks tasks)
        {
            var task = _todoListDBContext.Tasks.SingleOrDefault(t => t.TaskID == id);
            if (task != null)
            {
                task.Title = tasks.Title;
                task.Description = tasks.Description;
                task.TagName = tasks.TagName;
                _todoListDBContext.SaveChanges();

                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            try
            {
                var task = _todoListDBContext.Tasks.SingleOrDefault(t => t.TaskID == id);
                if (task == null)
                {
                    return NotFound();
                }
                _todoListDBContext.Remove(task);
                _todoListDBContext.SaveChanges();
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
