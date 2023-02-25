namespace MP_Music_Player.Models;

public class Genre {

  public const string SEPARATOR = "/";
  public int Id { get; set; }
  public string Name { get; set; } = null!;
  public List<Track> Tracks { get; set; } = null!;

  #region Overrides of Object

  public override string ToString() => this.Name;

  #endregion
}
