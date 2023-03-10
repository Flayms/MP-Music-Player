using CommunityToolkit.Mvvm.Input;
using MP_Music_Player.Models;
using MP_Music_Player.Services;
using MP_Music_Player.Views.Pages;

namespace MP_Music_Player.ViewModels; 

public partial class BigTrackViewModel : ATrackViewModel {

  private readonly TrackOptionsService _trackOptionsService;

  public BigTrackViewModel(TrackQueue queue, TrackOptionsService trackOptionsService) : base(queue) {
    this._trackOptionsService = trackOptionsService;
  }

  #region Overrides of ATrackViewModel

  protected override void OnNewSongSelected(object? sender, TrackEventArgs args) {
    base.OnNewSongSelected(sender, args);
    this.OnPropertyChanged(nameof(this.TrackLengthInS));
  }

  #endregion

  [RelayCommand]
  public void Next() => this.queue.Next();

  [RelayCommand]
  public void Previous() => this.queue.Previous();

  [RelayCommand]
  public void Shuffle() {
    this.queue.Shuffle();
    //  this.OnPropertyChanged(nameof(this.ShuffleImageSource));
  }

  [RelayCommand]
  public async void ShowOptions() {
    await this._trackOptionsService.StartBasicOptionsMenuAsync(this.Track);
  }

  [RelayCommand]
  public async void ClosePage() {
    await Shell.Current.Navigation.PopModalAsync();
  }

  [RelayCommand]
  public async void OpenQueuePage() {
    await Shell.Current.GoToAsync($"///{nameof(QueuePage)}");
  }
}