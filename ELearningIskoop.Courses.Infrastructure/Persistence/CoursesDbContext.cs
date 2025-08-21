using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Courses.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Courses.Infrastructure.Persistence.Configurations;

namespace ELearningIskoop.Courses.Infrastructure.Persistence
{
    // Courses modülü için EF Core DbContext
    public class CoursesDbContext : DbContext
    {

        private readonly IMediator? _mediator;

        public CoursesDbContext(DbContextOptions<CoursesDbContext> options,IMediator? mediator) : base(options)
        {
            _mediator = mediator;
        }
        
        //dbset
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Lesson> Lessons { get; set; } = null!;
        public DbSet<Category> categories { get; set; } = null!;
        public DbSet<CourseCategory> CourseCategories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //schema ayarları
            modelBuilder.HasDefaultSchema("courses");
            //Config
            modelBuilder.ApplyConfiguration(new CourseConfiguration());
            modelBuilder.ApplyConfiguration(new LessonConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new CourseCategoryConfiguration());


            // Global query filters (Soft delete)
            modelBuilder.Entity<Course>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Lesson>().HasQueryFilter(l => !l.IsDeleted);
            modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);


        }

        // SaveChanges override - Domain events dispatch eder
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // 1. Audit fields'ları güncelle
            UpdateAuditFields();

            //2. Domain Eventsleri topla
            var domainEvents = ChangeTracker.Entries<BaseEntity>().Where(x => x.Entity.DomainEvents.Any())
                .SelectMany(x => x.Entity.DomainEvents).ToList();

            // 3. Değişiklikleri kaydet
            var result = await base.SaveChangesAsync(cancellationToken);


            // 4. Domain events'leri publish et
            if (domainEvents.Any() && _mediator != null)
            {
                foreach (var domainEvent in domainEvents)
                {
                    await _mediator.Publish(domainEvent, cancellationToken);
                }

                //Eventleri temizle
                var entities = ChangeTracker.Entries<BaseEntity>().Where(x => x.Entity.DomainEvents.Any()).ToList();

                foreach (var entity in entities)
                {
                    entity.Entity.ClearDomainEvents();
                }
            }

            return result;

        }

        private async Task DispatchDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents)
        {
            // Bu kısım Infrastructure'da IMediator ile implement edilecek
            // Şimdilik placeholder
            foreach (var domainEvent in domainEvents)
            {
                // TODO: Publish domain event
                await Task.CompletedTask;
            }
        }

        // Audit fields'ları otomatik set eder
        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker.Entries<BaseEntity>()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    // CreatedAt otomatik set edildi BaseEntity'de
                }
                else if (entry.State == EntityState.Modified)
                {
                    // UpdatedAt manuel set edilmeli - domain logic'de
                    entry.Property(x => x.CreatedAt).IsModified = false;
                    entry.Property(x => x.CreatedBy).IsModified = false;
                }
            }
        }
    }
}
