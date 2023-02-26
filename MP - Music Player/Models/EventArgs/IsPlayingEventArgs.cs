namespace MP_Music_Player.Models.EventArgs; 

public class IsPlayingEventArgs : System.EventArgs {

  public bool IsPlaying { get; }

  public IsPlayingEventArgs(bool isPlaying) {
    this.IsPlaying = isPlaying;
  }
}