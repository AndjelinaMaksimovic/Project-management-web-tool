using Microsoft.EntityFrameworkCore;

namespace MarkoNenadovicAPI.data
{
    public class dataContext :DbContext
    {
        public dataContext(DbContextOptions<dataContext> options): base(options) { }

        public DbSet<product> products => Set<product>();
    }
}
