using CommunityToolkit.Mvvm.Input;
using Music_Player_Maui.Enums;
using Music_Player_Maui.Services;
using Music_Player_Maui.Views.Pages;
using Music_Player_Maui.Models;

namespace Music_Player_Maui.ViewModels; 

public partial class SongsViewModel : AViewModel {
  private readonly MusicService _musicLoadingService;
  private readonly TrackQueue _queue;
  private DisplayState _displayState = DisplayState.Loading;
  private IReadOnlyCollection<TrackCellViewModel>? _tracks;
  private string? _amountOfTracksRead;

  public IReadOnlyCollection<TrackCellViewModel>? Tracks {
    get => this._tracks;
    set => this.SetProperty(ref this._tracks, value);
  }

  public DisplayState DisplayState {
    get => this._displayState;
    set => this.SetProperty(ref this._displayState, value);
  }

  public string? AmountOfTracksRead {
    get => this._amountOfTracksRead;
    set => this.SetProperty(ref this._amountOfTracksRead, value);
  }

  public SongsViewModel(MusicService musicLoadingService, MusicFileParsingService musicFileParsingService, TrackQueue queue) {
    this._musicLoadingService = musicLoadingService;
    this._queue = queue;

    musicLoadingService.LoadingStateChangedEvent += this._OnLoadingChanged;
    musicFileParsingService.OnTrackLoaded += this._OnTrackLoaded;

    this.DisplayState = _GetDisplayState(musicLoadingService.IsLoading, musicLoadingService.HasTracks);

    if (this.DisplayState == DisplayState.DisplayingContent) { //todo: kinda double code
      this.Tracks = this.LoadTrackViewModels();
    }
  }

  private void _OnTrackLoaded(object? sender, MusicFileParsingService.TrackLoadedEventArgs e) {
    this.AmountOfTracksRead = $"Amount of tracks read: {e.AmountOfLoadedTracks} / {e.TotalAmountOfTracks}";
  }

  private void _OnLoadingChanged(object? sender, MusicService.LoadingEventArgs e) {
    if (sender is not MusicService musicLoadingService)
      return;

    if (e.IsLoading) {
      this.DisplayState = DisplayState.Loading;
      return;
    }

    if (!musicLoadingService.HasTracks) {
      this.DisplayState = DisplayState.Empty;
      return;
    }

    this.DisplayState = DisplayState.DisplayingContent;
    this.Tracks = this.LoadTrackViewModels();
  }

  private IReadOnlyCollection<TrackCellViewModel> LoadTrackViewModels() {
    var tracks = this._musicLoadingService.GetTracks();

    return tracks.Select(track => new TrackCellViewModel(track, this._queue)).ToList();
  }

  private static DisplayState _GetDisplayState(bool isLoading, bool hasTracks)
    => isLoading
      ? DisplayState.Loading
      : hasTracks
        ? DisplayState.DisplayingContent
        : DisplayState.Empty;

  [RelayCommand]
  public async Task NavigateToSearch() {
    //Shell.Current.CurrentPage.Title = "Search";
    await Shell.Current.GoToAsync($"///{nameof(SearchPage)}");
  }

}