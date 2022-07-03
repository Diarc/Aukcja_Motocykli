using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aukcja_Motocykli.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aukcja_Motocykli.Database_Context
{
    public class DbContext_Aukcji:IdentityDbContext<IdentityUser>
    {
        public DbContext_Aukcji(DbContextOptions<DbContext_Aukcji> options) :
            base(options)
            {

            }
        public DbSet<Make> Makes { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Bike> Bikes { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
