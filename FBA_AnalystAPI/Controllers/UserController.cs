using FBA_AnalystAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FBA_AnalystAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        ApplicationContext db;
        public UserController(ApplicationContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            return await db.Users.OrderBy(x => x.UserId).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<User>> Post(User user)
        {
            if (user == null)
                return BadRequest();

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPut]
        public async Task<ActionResult<User>> Put(User user)
        {
            if (user == null)
                return BadRequest();

            if (!db.Users.Any(x => x.UserId == user.UserId))
                return NotFound();

            db.Update(user);
            await db.SaveChangesAsync();

            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            User? user = db.Users.FirstOrDefault(x => x.UserId == id);

            if (user == null)
                return NotFound();

            db.Users.Remove(user);
            await db.SaveChangesAsync();

            return Ok(user);
        }
    }
}
