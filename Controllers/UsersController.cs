using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_API.Data;
using Web_API.Models;

namespace Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly Web_APIContext _context;

        public UsersController(Web_APIContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.User.ToListAsync();
        }

        // GET: api/Users/dasha@mail.ru
        [HttpGet("{email}")]
        public async Task<ActionResult<User>> GetUser(string email)
        {
            var user = await _context.User.FindAsync(email);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // GET: api/User/dasha@mail.ru/order/1    Добавлено
        [Route("/api/user/{email}/order/{id}")]
        [HttpGet]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Order.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // GET: api/Users/dasha@mail.ru/Orders

        [Route("/api/user/{email}/orders")]
        [HttpGet]
        public async Task<ActionResult> GetUserOrders(string email)
        {
            var user = await _context.User.FindAsync(email);

            if (user == null)
            {
                return NotFound();
            }

            var orders = _context.Order.Where(x => x.UserEmail == email).Select(x => new { x.Id, x.UserEmail, x.Name, x.Price, x.UpdatedDate });
            //var orders = "";
            return new OkObjectResult(orders);
        }

        //POST:api/Users/dasha@mail.ru/Orders

        [Route("/api/user/order")]
        [HttpPost]
        public async Task<ActionResult> PostUserOrder(Order order)
        {
            order.CreatedDate = DateTime.Now;
            order.UpdatedDate = DateTime.Now;
            _context.Order.Add(order);
            await _context.SaveChangesAsync();

            outData orders = new();
            orders.Id = order.Id;
            orders.Name = order.Name;
            if (order.Price==null)
            {
                order.Price = 0;
            }
            orders.Price = (int)order.Price;
            orders.UpdatedDate = order.UpdatedDate;
            orders.CreatedDate = order.CreatedDate;
            
            return new OkObjectResult(orders);
            
        }

        //PUT: api/user/dasha@mail.ru/order
        [Route("/api/user/{email}/order/{idOrder}")]
        [HttpPut]
        public async Task<ActionResult> PutUserOrder(string email, int idOrder, Order order)
        {
            //var search = _context.Order.Where(t => t.Name.Contains("искомая строка"));

            var checkUser = await _context.User.FindAsync(email);
            if (checkUser == null)
            {
               return BadRequest(new {email = "Not found" });
            }
            var newOrder = _context.Order.FirstOrDefault(x => x.Id == idOrder && x.UserEmail == email);
            
            if (newOrder != null)
            {
                if (order.Name != null)
                    newOrder.Name = order.Name;
                if(order.Price != null)
                    newOrder.Price = order.Price;
                newOrder.UpdatedDate = DateTime.Now;
                order.CreatedDate = newOrder.CreatedDate;
                await _context.SaveChangesAsync();
            }
            return new OkObjectResult(new {Id = idOrder, Name = newOrder.Name, Price = newOrder.Price, UpdatedDate = DateTime.Now, CreatedDate = order.CreatedDate});
        }



        // PUT: api/Users/dasha@mail.ru
        [HttpPut("{email}")]
        public async Task<IActionResult> PutUser(string email, User user)
        {    
            var ReceivedEmail = _context.User.FirstOrDefault(user => user.Email == email);
            if(ReceivedEmail == null)
            {
                return BadRequest();
            }            
            
            ReceivedEmail.Email = email;
            if(user.Name != null) ReceivedEmail.Name = user.Name;
            ReceivedEmail.Age = user.Age;
            ReceivedEmail.Enabled = user.Enabled;
            _context.Entry(ReceivedEmail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return new BadRequestResult();
            }

            return CreatedAtAction("GetUser", new { email = user.Email });
        }

        // POST: api/Users
        [Route("/api/users")] // Помимо этого роута добавлен "order":[] в Postman
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {            
            string userEmail = user.Email;
            var findEmail = _context.User.FirstOrDefault(t => t.Email == userEmail);

           if (findEmail != null)
            {
                return BadRequest(new {Email = "Данный email уже существует"});
            }
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Email }, user);
        }

        // DELETE: api/Users/dasha@mail.ru
        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            var user = await _context.User.FindAsync(email);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //DELETE: api/user/dasha@mail.ru/order
        [Route("/api/user/{email}/order/{idOrder}")]
        [HttpDelete]
        public async Task<ActionResult> DeleteUserOrder(string email, int idOrder)
        {
            if (UserExists(email))
            {
                if (OrderExist(idOrder))
                {
                    var order = await _context.Order.FindAsync(idOrder);
                    _context.Order.Remove(order);
                    await _context.SaveChangesAsync();
                }
            }
            
            return new NoContentResult();
        }

        [Route ("/api/test")]
        [HttpPost]
        public IActionResult test ([FromForm]string str)//передача данных из формы
        {
            return new OkObjectResult(str);
        }


        private bool UserExists(string email)
        {
            return _context.User.Any(e => e.Email == email);
        }

        private bool OrderExist (int id)
        {
            return _context.Order.Any(x => x.Id == id);
        }
    }
}

class outData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? Price { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}