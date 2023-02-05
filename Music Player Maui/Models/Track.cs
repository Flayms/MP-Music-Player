using System.ComponentModel.DataAnnotations.Schema;

namespace Music_Player_Maui.Models;

public class Track {
  private Cover? _cover;

  public int Id { get; set; }
  public string Path { get; set; } = null!;
  public string Title { get; set; } = null!;
  public List<Artist> Artists { get; set; } = null!;
  public string CombinedArtistNames => string.Join($" {Artist.SEPARATOR} ", Artists.Select(a => a.Name));

  public List<Genre> Genres { get; set; } = null!;

  public string? Album { get; set; }
  public TimeSpan Duration { get; set; }

  [NotMapped]
  public Cover Cover => this._cover ??= new Cover(this.Path);

  #region Overrides of Object

  public override string ToString() => this.Title;

  #endregion

}
