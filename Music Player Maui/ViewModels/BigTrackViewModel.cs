using CommunityToolkit.Mvvm.Input;
using Music_Player_Maui.Services;

namespace Music_Player_Maui.ViewModels; 

public partial class BigTrackViewModel : TrackPlayerViewModel {
  public BigTrackViewModel(TrackQueue queue) : base(queue) { }


  [RelayCommand]
  public void Next() => this._queue.Next();

  [RelayCommand]
  public void Previous() => this._queue.Previous();

  [RelayCommand]
  public void Shuffle() {
    this._queue.Shuffle();
    //  this.OnPropertyChanged(nameof(this.ShuffleImageSource));
  }
}