using MP_Music_Player.Models.EventArgs;
using MP_Music_Player.Services;
using MP_Music_Player.ViewModels;

namespace MP_Music_PLayer.ViewModels; 

//todo: this ain't the fanciest way to implement this but it works, maybe refac and use TrackListPage
public partial class HistoryViewModel : AViewModel {

  public TrackListViewModel TrackListViewModel { get; set; }

  private readonly TrackQueue _queue;

  public HistoryViewModel(TrackQueue queue) {
    this._queue = queue;

    var model = ServiceHelper.GetService<TrackListViewModel>();
    this.TrackListViewModel = model;
    this._queue.NewSongSelected += this._Queue_NewSongSelected;
  }

  private void _Queue_NewSongSelected(object? _, TrackEventArgs __) {
    this.TrackListViewModel.TrackViewModels = this._queue.HistoryTracks.Reverse().Select(t => new SmallTrackViewModel(t)).ToList();
  }
}