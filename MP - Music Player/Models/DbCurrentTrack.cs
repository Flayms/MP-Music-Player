namespace MP_Music_Player.Models; 

/// <summary>
/// The last track that was currently playing.
/// </summary>
public class DbCurrentTrack {
  public int Id { get; set; }
  public Track Track { get; set; } = null!;
  public double ProgressInSeconds { get; set; }
}