using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MyCourse.Models.Entities;

namespace MyCourse.Models.Services.Infrastructure
{
    public partial class MyCourseDbContext : DbContext
    {
        public MyCourseDbContext(DbContextOptions<MyCourseDbContext> options)
            : base(options)
        {
        }

        //Proprietà che espongono i DbSet<T>, é il DbContext
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Lesson> Lessons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //esplicita il mapping per la classe di entità
            modelBuilder.Entity<Course>(entity =>
            {
                //su che tabella rimappa la nostra classe di entità
                entity.ToTable("Courses"); //superfluo se la tabella si chiama come la proprietà che espone il DbSet
                entity.HasKey(course => course.Id);    //indica la PK, superfluo se la proprietà si chiama Id oppure CorsesId
                //entity.HasKey(course => new object{ course.Id, course.Author}) //SE abbiamo piu di una PK in tabella

                //PER LE MIGRATION (per gli indici)
                entity.HasIndex(course => course.Title).IsUnique();
                //concorrenza ottimistica
                entity.Property(course => course.RowVersion).IsRowVersion();

                //--------------Mapping per gli owned types

                //Current Price
                entity.OwnsOne(course => course.CurrentPrice, builder =>
                {
                    builder.Property(money => money.Currency)
                    .HasConversion<string>()                  //converte la stringa in enum
                    .HasColumnName("CurrentPrice_Currency");   //permette di fare il mapping delle due proprietà di Money
                    
                    builder.Property(money => money.Amount)
                    .HasConversion<float>()
                    .HasColumnName("CurrentPrice_Amount");
                });  //own type Money (che contiene)

                //Full price
                entity.OwnsOne(course => course.FullPrice, builder =>
                {
                    builder.Property(money => money.Currency).HasConversion<string>();         //converte la stringa in enum
                });

                //--------------Mapping per le relazioni
                entity.HasMany(course => course.Lessons)        //l'entità corsi ha molte lezioni (1 a molti)
                .WithOne(lesson => lesson.Course)               //una lezione ha UN corso a cui fa riferimento
                .HasForeignKey(lesson => lesson.CourseId);      //la proprietà su cui é memorizzata la FK

                #region Mapping generato automaticamente dal tool di reverse engineering
                /*
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Author)
                    .IsRequired()
                    .HasColumnType("TEXT (100)");

                entity.Property(e => e.CurrentPriceAmount)
                    .IsRequired()
                    .HasColumnName("CurrentPrice_Amount")
                    .HasColumnType("NUMERIC")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.CurrentPriceCurrency)
                    .IsRequired()
                    .HasColumnName("CurrentPrice_Currency")
                    .HasColumnType("TEXT (3)")
                    .HasDefaultValueSql("'EUR'");

                entity.Property(e => e.Description).HasColumnType("TEXT (10000)");

                entity.Property(e => e.Email).HasColumnType("TEXT (100)");

                entity.Property(e => e.FullPriceAmount)
                    .IsRequired()
                    .HasColumnName("FullPrice_Amount")
                    .HasColumnType("NUMERIC")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.FullPriceCurrency)
                    .IsRequired()
                    .HasColumnName("FullPrice_Currency")
                    .HasColumnType("TEXT (3)")
                    .HasDefaultValueSql("'EUR'");

                entity.Property(e => e.ImagePath).HasColumnType("TEXT (100)");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnType("TEXT (100)");
                    */
                #endregion
            });

            modelBuilder.Entity<Lesson>(entity =>
            {
                #region Mapping generato automaticamente dal tool di reverse engineering
                /*
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("TEXT (10000)");

                entity.Property(e => e.Duration)
                    .IsRequired()
                    .HasColumnType("TEXT (8)")
                    .HasDefaultValueSql("'00:00:00'");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnType("TEXT (100)");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Lessons)
                    .HasForeignKey(d => d.CourseId);
                    */
                #endregion
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
