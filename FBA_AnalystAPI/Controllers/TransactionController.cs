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
        ApplicationContext db;
        public TransactionController(ApplicationContext context)
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

            db.Transactions.Add(transaction);

            await db.SaveChangesAsync();
            await SendNewBalance(transaction.UserId);

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

            await SendNewBalance(transaction.UserId);

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

            await SendNewBalance(transaction.UserId);

            return Ok(transaction);
        }

        private async Task<ActionResult> SendNewBalance(int userId)
        {
            using var client = new Client("https://localhost:7095");

            await client.PostAsync<Balance, Balance>(CalculateBalance(userId).Result, "api/balance");

            return Ok();
        }

        private async Task<Balance> CalculateBalance(int userId)
        {
            decimal currentBalance = 0;

            var trList = await db.Transactions.Where(x=>x.UserId == userId).ToListAsync();
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
