using System.Diagnostics;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using Music_Player_Maui.Models;
using CommunityToolkit.Mvvm.Input;
using Music_Player_Maui.Services;
using Music_Player_Maui.Views.Pages;
using Timer = System.Threading.Timer;

namespace Music_Player_Maui.ViewModels;

public partial class TrackPlayerViewModel : AViewModel {
  protected readonly TrackQueue _queue;

  [ObservableProperty]
  [NotifyPropertyChangedFor(nameof(Title))]
  [NotifyPropertyChangedFor(nameof(Producer))]
  [NotifyPropertyChangedFor(nameof(CoverSource))]
  [NotifyPropertyChangedFor(nameof(IsPlaying))]
  [NotifyPropertyChangedFor(nameof(HasTrack))]
  private Track? _track;

  [ObservableProperty]
  private bool _isPlaying;

  public bool HasTrack => this.Track != null;
  public string Title => this.Track?.Title ?? "no song selected";
  public string Producer => this.Track?.CombinedArtistNames ?? "/";
  public ImageSource CoverSource => this.Track?.Cover.Source ?? ImageSource.FromFile("record.png"); //todo: refac!!

  //public string ShuffleImageSource => this._queue.IsShuffle ? "shuffle_selected.png" : "shuffle.png";

  //colors used for gradient of trackview
  //public Color Color { get; private set; }
  //public Color ColorDark { get; private set; }

  //private readonly TrackQueue _queue;

  private double _progressPercent;

  public TrackPlayerViewModel(TrackQueue queue) {
    this._queue = queue;

    var _ = new Timer(this._UpdateProgress, null, 0, 500);

    this.Track = queue.CurrentTrack;

    queue.NewSongSelected += this._OnNewSongSelected;

    //this._GetColors();
  }

  //private void _GetColors() {
  //  var color = this._track.Cover.GetDominantColor();
  //
  //  if (color.R == 0 && color.G == 0 && color.B == 0) {
  //    this.Color = Color.DimGray;
  //    this.ColorDark = color;
  //  } else {
  //    this.Color = color;
  //    this.ColorDark = new Color(color.R / 3, color.G / 3, color.B / 3, color.A);
  //  }
  //
  //  this.OnPropertyChanged(nameof(this.Color));
  //  this.OnPropertyChanged(nameof(this.ColorDark));
  //}

  [RelayCommand]
  public void PlayTapped() {
    this.IsPlaying = !this.IsPlaying;

    if (this.IsPlaying)
      this._queue.Play();
    else
      this._queue.Pause();
  }

  private void _OnNewSongSelected(object? sender, TrackEventArgs args) {
    this.Track = args.Track;
    this.IsPlaying = true;
    //  this._GetColors();
  }

  private void _UpdateProgress(object? _) => this.ProgressPercent = this._queue.GetProgressPercent();

  public double ProgressPercent {
    get => this._progressPercent;
    set => this.SetProperty(ref this._progressPercent, value);
  }

  [RelayCommand]
  public void TrackPositionChanged() {
    Trace.WriteLine($"Jumping to {this.ProgressPercent}");

    this._queue.JumpToPercent(this.ProgressPercent);
  }

  [RelayCommand]
  public void OpenBigTrackPage() {
    //todo: solve without getting over ServiceHelper
    var viewModel = ServiceHelper.GetService<BigTrackViewModel>();

    //todo: rather implement as PushModalAsync but needs to look better for windows then
    Shell.Current.Navigation.PushAsync(new BigTrackPage(viewModel));
  }
}