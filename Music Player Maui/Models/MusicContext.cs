using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Music_Player_Maui.Extensions;

namespace Music_Player_Maui.Models;

public class MusicContext : DbContext {

  public DbSet<Track> Tracks { get; set; } = null!;
  public DbSet<Artist> Artists { get; set; } = null!;
  public DbSet<Genre> Genres { get; set; } = null!;
  public DbSet<DbQueuedTrack> QueuedTracks { get; set; } = null!;
  public DbSet<DbCurrentTrack> CurrentTracks { get; set; } = null!; //always only one entry

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

    var queueBuilder = modelBuilder.Entity<DbQueuedTrack>(); 
    queueBuilder
      .ToTable("QueuedTracks")
      .HasKey(q => q.Id);

    queueBuilder.Navigation(qt => qt.Track)
      .AutoInclude();

    var currentTrackBuilder = modelBuilder.Entity<DbCurrentTrack>();
    currentTrackBuilder.ToTable("CurrentTrack")
    .HasKey(t => t.Id);

    currentTrackBuilder
      .Navigation(ct => ct.Track)
      .AutoInclude();

    base.OnModelCreating(modelBuilder);
  }

  #endregion

  public void ClearAllData() {
    this.Tracks.Clear();
    this.Artists.Clear();
    this.QueuedTracks.Clear();

    this.SaveChanges();
  }
}