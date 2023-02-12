using Music_Player_Maui.Models;

namespace Music_Player_Maui.ViewModels;

public class SmallGroupViewModel {
  public string Name { get; }
  public List<Track> Tracks { get; }
  public int TrackAmount => this.Tracks.Count;
  public string TrackAmountUi => this.TrackAmount + " Songs"; //todo: maybe converter for this

  public SmallGroupViewModel(string name, List<Track> tracks) {
    this.Name = name;
    this.Tracks = tracks;
  }
}