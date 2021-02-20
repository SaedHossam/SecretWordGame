using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace SecretWordGame
{
    public partial class CFDB : DbContext
    {
        public CFDB() : base("name=CFDB")
        {
        }

        public virtual DbSet<WordsGame> WordsGames { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WordsGame>()
                .Property(e => e.Word)
                .IsUnicode(false);

            modelBuilder.Entity<WordsGame>()
                .Property(e => e.Category)
                .IsUnicode(false);

            modelBuilder.Entity<WordsGame>()
                .Property(e => e.Difficulty)
                .IsUnicode(false);

            modelBuilder.Entity<WordsGame>()
                .Property(e => e.Photo)
                .IsUnicode(false);
        }
    }
}
