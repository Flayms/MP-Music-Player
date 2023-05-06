using MP_Music_Player.Models;

namespace MP_Music_PLayer.Models; 

public abstract class ACategory {

  public int Id { get; set; }
  public string Name { get; set; } = null!;
  public List<Track> Tracks { get; set; } = null!;

  #region Overrides of Object

  public override string ToString() => this.Name;

  #endregion
}