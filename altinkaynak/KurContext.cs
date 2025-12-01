using Microsoft.EntityFrameworkCore;
using AltinKaynak.GetCurrency.ASMX.Model;

namespace AltinKaynak.GetCurrency.ASMX
{
    public class KurContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=127.0.0.1,1433;Database=AltinKaynak1;User Id=sa;Password=Passw0rd;TrustServerCertificate=True;Encrypt=True;");
        }

        public DbSet<Kur> Kurlar { get; set; }
        public DbSet<Altin> Altinlar { get; set; }
    }
}
