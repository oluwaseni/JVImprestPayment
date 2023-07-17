using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using JV_Imprest_Payment.Models;

namespace JV_Imprest_Payment.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<JV_Imprest_Payment.Models.PayRequest>? PayRequest { get; set; }
        public DbSet<JV_Imprest_Payment.Models.AfeStructure>? AfeStructure { get; set; }
        public DbSet<JV_Imprest_Payment.Models.Role>? Role { get; set; }

       
    }


}