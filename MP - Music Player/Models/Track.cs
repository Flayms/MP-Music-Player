using System.ComponentModel.DataAnnotations.Schema;

namespace MP_Music_Player.Models;

public class Track {
  private Cover? _cover;

  public int Id { get; set; }
  public string Path { get; set; } = null!;
  public string Title { get; set; } = null!;
  public List<Artist> Artists { get; set; } = null!;
  public string CombinedArtistNames => string.Join($" {Artist.SEPARATOR} ", this.Artists.Select(a => a.Name));
  public string CombinedGenreNames => string.Join($" {Genre.SEPARATOR} ", this.Genres.Select(a => a.Name));

  /// <summary>
  /// Format: "Artists - Title"
  /// </summary>
  public string CombinedName => $"{this.CombinedArtistNames} - {this.Title}";

  public List<Genre> Genres { get; set; } = null!;

  public string? Album { get; set; }
  public TimeSpan Duration { get; set; }

  [NotMapped]
  public Cover Cover => this._cover ??= new Cover(this.Path);

  #region Overrides of Object

  public override string ToString() => this.CombinedName;

  #endregion

}
