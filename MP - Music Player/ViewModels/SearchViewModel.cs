using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MP_Music_Player.Extensions;
using MP_Music_Player.Models;
using MP_Music_Player.Services;

namespace MP_Music_Player.ViewModels; 

public partial class SearchViewModel : AViewModel {

  [ObservableProperty]
  private IReadOnlyList<SmallTrackViewModel>? _tracks;

  private readonly MusicService _musicService;
  private readonly TrackQueue _queue;

  public SearchViewModel(MusicService musicService, TrackQueue queue) {
    this._musicService = musicService;
    this._queue = queue;
  }

  [RelayCommand]
  public async void PerformSearch(string search) {
    var tracks = await this._musicService.GetTracksAsync();
    search = search.Trim().ToLower();

    //todo: just check fileNames, not paths
    var searchResults = tracks
      .Where(t => t.Path.ToLower().Contains(search) 
                  || t.CombinedName.ToLower().Contains(search))
      .Select(t => {
        var model = new SmallTrackViewModel(t);
        model.OnTappedEvent += this._OnSmallTrackViewTapped;
        return model;
      })
      .ToList();

    this.Tracks = searchResults;
  }

  //todo: duplicate code with songsViewModel
  private void _OnSmallTrackViewTapped(object? sender, TrackEventArgs e) {
    if (sender is not SmallTrackViewModel trackModel)
      return;

    var trackQueue = this._queue;

    var trackViewModels = this.Tracks;
    var index = trackViewModels.IndexOf(trackModel);

    var queue = trackViewModels
      .Skip(index)
      .Take(trackViewModels.Count)
      .Select(t => t.Track);

    trackQueue.ChangeQueue(queue);
    trackQueue.Play();
  }
}