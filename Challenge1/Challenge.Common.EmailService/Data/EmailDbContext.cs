using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using challenge1.Common.EmailService.Classes;
using challenge1.Common.EmailService.Models;

namespace challenge1.Common.EmailService.Data
{
    public class EmailDbContext : DbContext
    {
        private readonly string? _schema;
        public EmailDbContext(DbContextOptions<EmailDbContext> options,
            IOptions<EmailServiceSettings> settings)
            : base(options)
        {
            _schema = settings.Value.DBSchema;
        }

        public DbSet<Email> Emails { get; set; }
        public DbSet<EmailAttachment> EmailAttachments { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (!string.IsNullOrEmpty(_schema))
            {
                modelBuilder.HasDefaultSchema(_schema);
            }

            // Constraints & relationships
            modelBuilder.Entity<Email>(entity =>
            {
                entity.ToTable("email", tableBuilder =>
                {
                    tableBuilder.HasCheckConstraint(
                        "batch_job_mandatory_field",
                        "send_type = 'BatchJob' AND target_send_date IS NOT NULL");
                });

                entity.HasKey(e => e.EmailId).HasName("pk_email");

                entity.HasMany(e => e.Attachments)
                      .WithOne(a => a.Email)
                      .HasForeignKey(a => a.EmailId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Logs)
                      .WithOne(l => l.Email)
                      .HasForeignKey(l => l.EmailId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<EmailAttachment>(entity =>
            {
                entity.ToTable("email_attachment");
                entity.HasKey(e => e.EmailAttachmentId).HasName("pk_email_attachment");
            });

            modelBuilder.Entity<EmailLog>(entity =>
            {
                entity.ToTable("email_log");
                entity.HasKey(e => e.EmailLogId).HasName("pk_email_log");
            });
        }
    }
}
