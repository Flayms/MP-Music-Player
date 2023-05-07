using CommunityToolkit.Mvvm.Input;
using MP_Music_PLayer.Enums;
using MP_Music_Player.Models.EventArgs;
using MP_Music_Player.Services;
using MP_Music_Player.Views.Pages;

namespace MP_Music_Player.ViewModels;

public partial class BigTrackViewModel : ATrackViewModel {

  private const int _PREVIOUS_TIMEOUT_IN_S = 5;

  private readonly TrackOptionsService _trackOptionsService;

  public LoopMode LoopMode {
    get => this.Queue.LoopMode;
    set {
      this.Queue.LoopMode = value;
      this.OnPropertyChanged();
    }
  }

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
  public void Next() => this.Queue.Next();

  [RelayCommand]
  public void GoBack() {
    //Either jump back to last track or repeat current track

    if (this.Player.PositionInS < _PREVIOUS_TIMEOUT_IN_S)
      this.Queue.Previous();
    else
      this.Player.JumpToPercent(0);
  }

  [RelayCommand]
  public void Shuffle() {
    this.Queue.Shuffle();
    //  this.OnPropertyChanged(nameof(this.ShuffleImageSource));
  }

  [RelayCommand]
  public void ChangeLoopMode() {
    //cycle through enum values
    var values = Enum.GetValues(typeof(LoopMode)).Cast<LoopMode>().ToList();
    var pos = values.IndexOf(this.LoopMode);
    var nextMode = values[(pos + 1) % values.Count];
    this.LoopMode = nextMode;
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