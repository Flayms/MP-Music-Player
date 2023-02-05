using CommunityToolkit.Mvvm.Input;
using Music_Player_Maui.Enums;
using Music_Player_Maui.Services;
using Music_Player_Maui.Views.Pages;
using Music_Player_Maui.Models;

namespace Music_Player_Maui.ViewModels; 

public partial class SongsViewModel : AViewModel {
  private DisplayState _displayState = DisplayState.Loading;
  private IReadOnlyCollection<Track>? _tracks;
  private string? _amountOfTracksRead;

  public IReadOnlyCollection<Track>? Tracks {
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

  public SongsViewModel(MusicService musicLoadingService, MusicFileParsingService musicFileParsingService) {
    musicLoadingService.LoadingStateChangedEvent += this._OnLoadingChanged;
    musicFileParsingService.OnTrackLoaded += this._OnTrackLoaded;

    this.DisplayState = _GetDisplayState(musicLoadingService.IsLoading, musicLoadingService.HasTracks);
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
    this.Tracks = musicLoadingService.GetTracks();
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