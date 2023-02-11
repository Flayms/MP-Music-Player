using Music_Player_Maui.Services;

namespace Music_Player_Maui.ViewModels; 

public partial class QueueViewModel : AViewModel {

  public TrackCellViewModel? CurrentTrack => this._queue.CurrentTrack != null ? new TrackCellViewModel(this._queue.CurrentTrack) : null;

  //todo: need to find better way than just copying the lists
  public List<TrackCellViewModel> NextUpTracks => this._queue.NextUpTracks.Select(t => new TrackCellViewModel(t)).ToList();

  public List<TrackCellViewModel> QueuedTracks {
    get {
      return this._queue.QueuedTracks.Count <= 20
        ? this._queue.QueuedTracks.Select(t => new TrackCellViewModel(t)).ToList()
        : this._queue.QueuedTracks.GetRange(0, 20)
          .Select(t => new TrackCellViewModel(t))
          .ToList();
    }
  }

  //todo: implement
  public bool NextUpsVisible => this.NextUpTracks.Any();

  //todo: implement
  public bool QueuedVisible => this.QueuedTracks.Any();


  private readonly TrackQueue _queue;

  public QueueViewModel(TrackQueue queue) {
    this._queue = queue;
    queue.NewSongSelected += (_, _) => this.Refresh();
  }

  public void Refresh() {
    this.OnPropertyChanged(nameof(this.CurrentTrack));
    this.OnPropertyChanged(nameof(this.NextUpTracks));
    this.OnPropertyChanged(nameof(this.QueuedTracks));
    this.OnPropertyChanged(nameof(this.NextUpsVisible));
    this.OnPropertyChanged(nameof(this.QueuedVisible));
  }
}