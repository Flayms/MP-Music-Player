using CommunityToolkit.Mvvm.Input;
using Music_Player_Maui.Enums;
using Music_Player_Maui.Models;
using Music_Player_Maui.Services;
using Music_Player_Maui.Views.Pages;
using Music_Player_Maui.Extensions;

namespace Music_Player_Maui.ViewModels; 

public partial class SongsViewModel : AViewModel {
  private readonly MusicService _musicLoadingService;
  private readonly TrackQueue _queue;
  private DisplayState _displayState = DisplayState.Loading;
  private IReadOnlyList<TrackCellViewModel>? _tracks;
  private string? _amountOfTracksRead;

  public IReadOnlyList<TrackCellViewModel>? Tracks {
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
      this.Tracks = this._LoadTrackViewModels();
    }
  }

  //todo: implement for searchPage
  public SongsViewModel(IReadOnlyList<Track> tracks, MusicService musicLoadingService, MusicFileParsingService musicFileParsingService, TrackQueue queue) {
    //this._musicLoadingService = musicLoadingService;
    //this._queue = queue;

    //musicLoadingService.LoadingStateChangedEvent += this._OnLoadingChanged;
    //musicFileParsingService.OnTrackLoaded += this._OnTrackLoaded;

    //this.DisplayState = _GetDisplayState(musicLoadingService.IsLoading, musicLoadingService.HasTracks);

    //if (this.DisplayState == DisplayState.DisplayingContent) { //todo: kinda double code
    //  this.Tracks = this._LoadTrackViewModels();
    //}
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
    this.Tracks = this._LoadTrackViewModels();
  }

  private IReadOnlyList<TrackCellViewModel> _LoadTrackViewModels() {
    var tracks = this._musicLoadingService.GetTracks();

    return tracks.Select(track => {
      var model = new TrackCellViewModel(track);
      model.OnTappedEvent += this._Model_OnTappedEvent;
      return model;
    }).ToList();
  }

  private void _Model_OnTappedEvent(object? sender, TrackEventArgs e) {
    if (sender is not TrackCellViewModel trackModel)
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

}