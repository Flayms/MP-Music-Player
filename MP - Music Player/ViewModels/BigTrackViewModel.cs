using CommunityToolkit.Mvvm.Input;
using MP_Music_Player.Models.EventArgs;
using MP_Music_Player.Services;
using MP_Music_Player.Views.Pages;

namespace MP_Music_Player.ViewModels; 

public partial class BigTrackViewModel : ATrackViewModel {
  private const int _PREVIOUS_TIMEOUT_IN_S = 5;

  private readonly TrackOptionsService _trackOptionsService;

  public BigTrackViewModel(TrackQueue queue, TrackOptionsService trackOptionsService, AudioPlayer player)
    : base(queue, player) {
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
  public void GoBack() {
    //Either jump back to last track or repeat current track

    if (this._player.PositionInS < _PREVIOUS_TIMEOUT_IN_S)
      this.queue.Previous();
    else
      this.queue.JumpToPercent(0);
  }

  [RelayCommand]
  public void Shuffle() {
    this.queue.Shuffle();
    //  this.OnPropertyChanged(nameof(this.ShuffleImageSource));
  }

  [RelayCommand]
  public async void ShowOptions() {
    //todo: what if track is null here?
    await this._trackOptionsService.StartBasicOptionsMenuAsync(this.Track ?? throw new NullReferenceException());
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