using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Music_Player_Maui.Services;

namespace Music_Player_Maui.ViewModels; 

public partial class SearchViewModel : AViewModel {

  [ObservableProperty]
  private IReadOnlyCollection<TrackCellViewModel>? _tracks;

  private readonly MusicService _musicService;

  public SearchViewModel(MusicService musicService) {
    this._musicService = musicService;
  }

  [RelayCommand]
  public async void PerformSearch(string search) {
    var tracks = await this._musicService.GetTracksAsync();
    search = search.Trim().ToLower();

    //todo: just check fileNames, not paths
    var searchResults = tracks
      .Where(t => t.Path.ToLower().Contains(search) 
                  || t.CombinedName.ToLower().Contains(search))
      .Select(t => new TrackCellViewModel(t))
      .ToList();

    this.Tracks = searchResults;
  }

}