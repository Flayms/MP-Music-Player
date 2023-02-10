namespace Music_Player_Maui.Models; 

public class TrackEventArgs : EventArgs {
  public Track Track { get; }

  public TrackEventArgs(Track track) {
    this.Track = track;
  }
}