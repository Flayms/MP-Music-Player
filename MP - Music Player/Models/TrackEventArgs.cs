namespace MP_Music_Player.Models; 

public class TrackEventArgs : EventArgs {
  public Track Track { get; }

  public TrackEventArgs(Track track) {
    this.Track = track;
  }
}