namespace MP_Music_Player.Models.EventArgs; 

public class TrackEventArgs : System.EventArgs {
  public Track Track { get; }

  public TrackEventArgs(Track track) {
    this.Track = track;
  }
}