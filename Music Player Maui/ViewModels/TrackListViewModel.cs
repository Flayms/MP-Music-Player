using CommunityToolkit.Mvvm.ComponentModel;
using Music_Player_Maui.Extensions;
using Music_Player_Maui.Models;
using Music_Player_Maui.Services;

namespace Music_Player_Maui.ViewModels;

public partial class TrackListViewModel : AViewModel {

  [ObservableProperty]
  private string _title;

  public IReadOnlyList<SmallTrackViewModel> TrackViewModels {
    get => this._trackViewModels;
    set {
      if (!this.SetProperty(ref this._trackViewModels, value))
        return;

      foreach (var smallTrackViewModel in value) {
        smallTrackViewModel.OnTappedEvent += this._OnSmallTrackViewTapped;
      }
    }
  }

  private readonly TrackQueue _queue;
  private IReadOnlyList<SmallTrackViewModel> _trackViewModels;

  public TrackListViewModel(TrackQueue queue) {
    this._queue = queue;
  }

  private void _OnSmallTrackViewTapped(object? sender, TrackEventArgs e) {
    if (sender is not SmallTrackViewModel trackModel)
      return;

    var trackQueue = this._queue;
    var queue = new List<Track>();
    var trackViewModels = this.TrackViewModels;
    var index = trackViewModels.IndexOf(trackModel);

    for (var i = index; i < trackViewModels.Count; ++i)
      queue.Add(trackViewModels[i].Track);

    trackQueue.ChangeQueue(queue);
    trackQueue.Play();
  }
}