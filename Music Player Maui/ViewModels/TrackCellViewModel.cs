using CommunityToolkit.Mvvm.Input;
using Music_Player_Maui.Models;
using Music_Player_Maui.Services;

namespace Music_Player_Maui.ViewModels; 

//todo: multiple code with trackViewModel
public partial class TrackCellViewModel : AViewModel {
  private readonly Track _track;
  private readonly TrackQueue _queue;

  public string Title => this._track?.Title ?? "no song selected";
  public string Producer => this._track?.CombinedArtistNames ?? "/";
  public ImageSource CoverSource => this._track?.Cover.Source ?? ImageSource.FromFile("record.png"); //todo: refac!!

  public TrackCellViewModel(Track track, TrackQueue queue) {
    this._track = track;
    this._queue = queue;
  }

  [RelayCommand]
  public void Play() {
    this._queue.Play(this._track);
  }
}