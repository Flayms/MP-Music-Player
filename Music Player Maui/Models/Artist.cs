namespace Music_Player_Maui.Models;

public class Artist {

  public const string SEPARATOR = "&";

  public int Id { get; set; }
  public string Name { get; set; } = null!;
  public List<Track> Tracks { get; set; } = null!;

  #region Overrides of Object

  public override string ToString() => this.Name;

  #endregion
}
