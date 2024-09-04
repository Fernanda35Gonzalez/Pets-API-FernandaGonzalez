using System;
using System.Collections.Generic;
using System.Linq;
using api.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace api.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {


        }
    
    public DbSet<User> Users {get; set;}
    public DbSet<Pet> Pets {get; set;}
        
    }
}