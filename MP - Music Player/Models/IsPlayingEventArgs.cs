namespace Music_Player_Maui.Models; 

public class IsPlayingEventArgs : EventArgs {

  public bool IsPlaying { get; }

  public IsPlayingEventArgs(bool isPlaying) {
    this.IsPlaying = isPlaying;
  }
}