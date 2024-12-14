using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public  class ApplicationDbContext : DbContext, IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Aciklama
            /*Burada herhangi bir DbSet<Webinar> Webinars tanımlaması yapmamamıza rağmen Infrastructure'da bulunan Configurations içerisinde WebinarConfiguration.cs
            dosyası "WebinarConfiguration : IEntityTypeConfiguration<Webinar>" şeklinde implemente edildiğinden dolayı aşağıdaki kodda da Assembly içerisindeki tüm 
            Entity Configuration sınıflarını otomatik olarak bulur ve uygular.
            */
            #endregion

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
