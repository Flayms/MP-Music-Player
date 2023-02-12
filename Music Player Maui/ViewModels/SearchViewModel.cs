using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Music_Player_Maui.Extensions;
using Music_Player_Maui.Models;
using Music_Player_Maui.Services;

namespace Music_Player_Maui.ViewModels; 

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
    var queue = new List<Track>();
    var trackViewModels = this.Tracks;
    var index = trackViewModels.IndexOf(trackModel);

    for (var i = index; i < trackViewModels.Count; ++i)
      queue.Add(trackViewModels[i].Track);

    trackQueue.ChangeQueue(queue);
    trackQueue.Play();
  }
}