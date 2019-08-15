using Microsoft.EntityFrameworkCore;

namespace loginReg.Models {
    public class  UserContext : DbContext {
        public UserContext(DbContextOptions options) : base(options) {}
        public DbSet<User> users {get;set;}
    }
}