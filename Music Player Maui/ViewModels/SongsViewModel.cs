using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Music_Player_Maui.Enums;
using Music_Player_Maui.Models;
using Music_Player_Maui.Services;
using Music_Player_Maui.Views.Pages;
using Music_Player_Maui.Extensions;

namespace Music_Player_Maui.ViewModels;

public partial class SongsViewModel : AViewModel {

  [ObservableProperty]
  private DisplayState _displayState = DisplayState.Loading;

  [ObservableProperty]
  private IReadOnlyList<SmallTrackViewModel>? _tracks;

  [ObservableProperty]
  private string? _amountOfTracksRead;

  private readonly MusicService _musicService;
  private readonly TrackQueue _queue;

  public SongsViewModel(MusicService musicService, MusicFileParsingService musicFileParsingService, TrackQueue queue) {
    this._musicService = musicService;
    this._queue = queue;

    musicService.LoadingStateChangedEvent += this._OnLoadingChanged;
    musicFileParsingService.OnTrackLoaded += this._OnTrackLoaded;

    this.DisplayState = _GetDisplayState(musicService.IsLoading, musicService.HasTracks);

    if (this.DisplayState == DisplayState.DisplayingContent) { //todo: kinda double code
      this.Tracks = this._LoadTrackViewModels();
    }
  }

  private void _OnTrackLoaded(object? sender, MusicFileParsingService.TrackLoadedEventArgs e) {
    this.AmountOfTracksRead = $"Amount of tracks read: {e.AmountOfLoadedTracks} / {e.TotalAmountOfTracks}";
  }

  private void _OnLoadingChanged(object? sender, MusicService.LoadingEventArgs e) {
    if (sender is not MusicService musicService)
      return;

    if (e.IsLoading) {
      this.DisplayState = DisplayState.Loading;
      return;
    }

    if (!musicService.HasTracks) {
      this.DisplayState = DisplayState.Empty;
      return;
    }

    this.DisplayState = DisplayState.DisplayingContent;
    this.Tracks = this._LoadTrackViewModels();
  }

  private IReadOnlyList<SmallTrackViewModel> _LoadTrackViewModels() {
    var tracks = this._musicService.GetTracks();

    return tracks.Select(track => {
      var model = new SmallTrackViewModel(track);
      model.OnTappedEvent += this._OnSmallTrackViewTapped;
      return model;
    }).ToList();
  }

  private void _OnSmallTrackViewTapped(object? sender, TrackEventArgs e) {
    if (sender is not SmallTrackViewModel trackModel)
      return;

    var trackQueue = this._queue;
    var queue = new List<Track>();
    var trackViewModels = this.Tracks;
    var index = trackViewModels.IndexOf(trackModel);

    for (var i = index; i < trackViewModels.Count; ++i)
      queue.Add(trackViewModels[i].Track);

    trackQueue.ChangeQueue(queue);
    trackQueue.Play();
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

  [RelayCommand]
  public void ShuffleAll() { //todo: also kinda double code?
    var trackQueue = this._queue;

    var trackViewModels = this.Tracks;
    var newQueue = new List<SmallTrackViewModel>(trackViewModels); //todo: possible null!
    newQueue.Shuffle();

    trackQueue.ChangeQueue(newQueue.Select(vm => vm.Track).ToList());
    trackQueue.Play();
  }

}