namespace MP_Music_Player.Models; 

public class IsPlayingEventArgs : EventArgs {

  public bool IsPlaying { get; }

  public IsPlayingEventArgs(bool isPlaying) {
    this.IsPlaying = isPlaying;
  }
}