using System.Diagnostics;
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

  //todo: same code as in searchViewModel
  private void _OnSmallTrackViewTapped(object? sender, TrackEventArgs e) {
    if (sender is not SmallTrackViewModel trackModel)
      return;

    var trackQueue = this._queue;

    var sw = new Stopwatch();
    sw.Start();

    var trackViewModels = this.TrackViewModels;
    var index = trackViewModels.IndexOf(trackModel);

    var queue = trackViewModels
      .Skip(index)
      .Take(trackViewModels.Count)
      .Select(t => t.Track);

    trackQueue.ChangeQueue(queue);

    Trace.WriteLine($"tvm: change queue: {sw.ElapsedMilliseconds}");
    trackQueue.Play();
  }
}