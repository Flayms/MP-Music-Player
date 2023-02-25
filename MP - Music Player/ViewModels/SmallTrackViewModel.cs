using CommunityToolkit.Mvvm.Input;
using MP_Music_Player.Models;
using MP_Music_Player.Services;

namespace MP_Music_Player.ViewModels; 

//todo: multiple code with trackViewModel
public partial class SmallTrackViewModel : AViewModel {
  public Track Track { get; }

  public event EventHandler<TrackEventArgs>? OnTappedEvent;

  public string Title => this.Track.Title;
  public string Producer => this.Track.CombinedArtistNames;
  public ImageSource CoverSource => this.Track.Cover.Source; //todo: refac!!

  public SmallTrackViewModel(Track track) {
    this.Track = track;
  }

  [RelayCommand]
  public void Play() {
    this.OnTappedEvent?.Invoke(this, new TrackEventArgs(this.Track));
  }

  [RelayCommand]
  public async void ShowOptions() {
    //todo: would be better to use DI over constructor
    await ServiceHelper.Current.GetService<TrackOptionsService>()!.StartBasicOptionsMenuAsync(this.Track);
  }
}