using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Music_Player_Maui.Models;

public class MusicContext : DbContext {

  public DbSet<Track> Tracks { get; set; } = null!;
  public DbSet<Artist> Artists { get; set; } = null!;
  public DbSet<Genre> Genres { get; set; } = null!;
  public DbSet<QueuedTrack> QueuedTracks { get; set; } = null!;

  private readonly string _filePath;

  public MusicContext(string filePath) {
    this._filePath = filePath;
  }

  #region Overrides of DbContext

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    optionsBuilder.UseSqlite($"FileName={this._filePath}", option => {
      option.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
    });

    base.OnConfiguring(optionsBuilder);
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    var trackBuilder = modelBuilder.Entity<Track>();
    trackBuilder
      .ToTable("Tracks")
      .HasKey(t => t.Id);

    trackBuilder
      .Navigation(t => t.Artists)
      .AutoInclude();

    trackBuilder
      .Navigation(t => t.Genres)
      .AutoInclude();

    modelBuilder.Entity<Artist>()
      .ToTable("Artists")
      .HasKey(a => a.Id);

    modelBuilder.Entity<Genre>()
      .ToTable("Genres")
      .HasKey(g => g.Id);

    var queueBuilder = modelBuilder.Entity<QueuedTrack>(); 
    queueBuilder
      .ToTable("QueuedTracks")
      .HasKey(q => q.Id);

    queueBuilder.Navigation(q => q.Track)
      .AutoInclude();

    base.OnModelCreating(modelBuilder);
  }

  #endregion

  public void ClearAllData() {
    foreach (var track in this.Tracks) {
      this.Tracks.Attach(track);
      this.Tracks.Remove(track);
    }

    foreach (var artist in this.Artists) {
      this.Artists.Attach(artist);
      this.Artists.Remove(artist);
    }

    foreach (var genre in this.Genres) {
      this.Genres.Attach(genre);
      this.Genres.Remove(genre);
    }

    this.SaveChanges();
  }
}