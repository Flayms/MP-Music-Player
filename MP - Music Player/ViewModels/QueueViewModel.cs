using MP_Music_PLayer.Models;
using MP_Music_Player.Services;

namespace MP_Music_Player.ViewModels;

public class QueueViewModel : AViewModel {

  //todo: not very efficient
  public IEnumerable<TrackGroup> TrackGroups {
    get {
      var groups = new List<TrackGroup>();
      var current = this._queue.CurrentTrack;
      var nextUp = this._queue.NextUpTracks;
      var queued = this._queue.QueuedTracks;

      if (current != null)
        groups.Add(new TrackGroup("Currently Playing:", new List<SmallTrackViewModel> { new(current) }));

      if (nextUp.Count > 0)
        groups.Add(new TrackGroup("Next Up:", nextUp.Select(n => new SmallTrackViewModel(n)).ToList()));

      if (queued.Count > 0)
        groups.Add(new TrackGroup("Queue:", queued.Select(n => new SmallTrackViewModel(n)).ToList()));

      return groups;
    }
  }

  private readonly TrackQueue _queue;

  public QueueViewModel(TrackQueue queue) {
    this._queue = queue;
    queue.QueueChanged += (_, _) => this.OnPropertyChanged(nameof(this.TrackGroups));
  }

}