using MP_Music_PLayer.Models;
using MP_Music_Player.Services;
using MP_Music_Player.Models.EventArgs;

namespace MP_Music_Player.ViewModels;

public class QueueViewModel : AViewModel {

  public IEnumerable<TrackGroup> TrackGroups { get; private set; }

  private List<SmallTrackViewModel> _trackViewModels;

  private readonly TrackQueue _queue;
  private readonly AudioPlayer _player;

#pragma warning disable CS8618
  public QueueViewModel(TrackQueue queue, AudioPlayer player) {
#pragma warning restore CS8618
    this._queue = queue;
    this._player = player;
    this._UpdateTrackGroups();
    queue.QueueChanged += (_, _) => this._UpdateTrackGroups();
  }

  //todo: not very efficient
  private void _UpdateTrackGroups() {
    var groups = new List<TrackGroup>();
    var current = this._queue.CurrentTrack;
    var nextUp = this._queue.NextUpTracks;
    var queued = this._queue.QueuedTracks;

    if (current != null) {
      var currentViewModel = new SmallTrackViewModel(current);
      currentViewModel.OnTappedEvent += this._CurrentTrackTapped;
      groups.Add(new TrackGroup("Currently Playing:", new List<SmallTrackViewModel> { currentViewModel }));
    }

    if (nextUp.Count > 0) {
      var nextUpViewModels = nextUp.Select(n => new SmallTrackViewModel(n)).ToList();
      nextUpViewModels.ForEach(vm => vm.OnTappedEvent += this._NextUpTrackTapped);
      groups.Add(new TrackGroup("Next Up:", nextUpViewModels));
    }

    if (queued.Count > 0) {
      var queuedViewModels = queued.Select(n => new SmallTrackViewModel(n)).ToList();
      queuedViewModels.ForEach(vm => vm.OnTappedEvent += this._QueueTrackTapped);
      groups.Add(new TrackGroup("Queue:", queuedViewModels));
    }

    this._trackViewModels = groups.SelectMany(g => g.Tracks).ToList();
    this.TrackGroups = groups;
    this.OnPropertyChanged(nameof(this.TrackGroups));
  }

  private void _CurrentTrackTapped(object? _, TrackEventArgs __) {
    if (this._player.IsPlaying)
      this._player.Pause();
    else
      this._player.Play();
  }

  private void _NextUpTrackTapped(object? sender, TrackEventArgs e) {
    if (sender is not SmallTrackViewModel trackModel)
      return;

    var track = trackModel.Track;
    var index = this._queue.NextUpTracks.IndexOf(track);
    var skipped = this._queue.NextUpTracks.Skip(index);
    this._queue.ChangeNextUp(skipped);
  }

  //todo: same code as in searchViewModel
  private void _QueueTrackTapped(object? sender, TrackEventArgs e) {
    if (sender is not SmallTrackViewModel trackModel)
      return;

    var trackQueue = this._queue;
    var trackViewModels = this._trackViewModels;
    var index = trackViewModels.IndexOf(trackModel);

    var queue = trackViewModels
      .Skip(index)
      .Take(trackViewModels.Count)
      .Select(t => t.Track);

    trackQueue.ChangeQueue(queue);
    trackQueue.Play();
  }

}