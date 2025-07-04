using InvestmentPortfolio.Models.DBContext;
using Microsoft.EntityFrameworkCore;

namespace InvestmentPortfolio.Repository.DBContext
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<TotalPLHoldings> TotalPLHoldings { get; set; }
    public DbSet<DeleteMessage> DeleteMessages { get; set; }

  }

}
