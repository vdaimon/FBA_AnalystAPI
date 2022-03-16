using FBA_AnalystAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestClient.Net;

namespace FBA_AnalystAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        DataBaseContext db;
        public TransactionController(DataBaseContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> Get()
        {
            var transactions = await db.Transactions.OrderBy(x=>x.TransactionId).ToListAsync();

            if (transactions == null)
                return NotFound();

            return Ok(transactions);
        }

        [HttpPost]
        public async Task<ActionResult<Transaction>> Post(Transaction transaction)
        {
            if (transaction == null)
                return BadRequest();

            User user = await db.Users.FirstOrDefaultAsync(x=>x.UserId==transaction.User.UserId);

            if (user != null)
                transaction.User = user;
            else return NotFound($"User whith id = {transaction.User.UserId} not found");

            db.Transactions.Add(transaction);
            await db.SaveChangesAsync();

            await SendNewBalance();

            return Ok(transaction);
        }

        [HttpPut]
        public async Task<ActionResult<User>> Put(Transaction transaction)
        {
            if (transaction == null)
                return BadRequest();

            if (!db.Transactions.Any(x => x.TransactionId == transaction.TransactionId))
                return NotFound();

            db.Update(transaction);
            await db.SaveChangesAsync();

            await SendNewBalance();

            return Ok(transaction);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            Transaction? transaction = db.Transactions.FirstOrDefault(x => x.TransactionId == id);

            if (transaction == null)
                return NotFound();

            db.Transactions.Remove(transaction);
            await db.SaveChangesAsync();

            await SendNewBalance();

            return Ok(transaction);
        }

        private async Task<ActionResult> SendNewBalance()
        {
            using var client = new Client("https://localhost:7095");

            await client.PostAsync<Balance, Balance>(CalculateBalance().Result, "api/balance");

            return Ok();
        }

        private async Task<Balance> CalculateBalance()
        {
            decimal currentBalance = 0;

            var trList = await db.Transactions.ToListAsync();
            foreach (Transaction tr in trList)
            {
                if (tr.IsIncome)
                    currentBalance += tr.Amount;
                else currentBalance -= tr.Amount;
            }

            Balance balance = new Balance();
            balance.DateTime = DateTime.UtcNow;
            balance.Amount = currentBalance;

            return balance;
        }

    }
}
