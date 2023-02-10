using System.Diagnostics;
using System.Timers;
using Music_Player_Maui.Models;
using CommunityToolkit.Mvvm.Input;
using Music_Player_Maui.Services;
using Timer = System.Threading.Timer;

namespace Music_Player_Maui.ViewModels;

public partial class TrackPlayerViewModel : AViewModel {
  private readonly TrackQueue _queue;

  public Track? Track {
    get => this._track;
    set {
      if (!this.SetProperty(ref this._track!, value))
        return;

      this.OnPropertyChanged(nameof(this.Title));
      this.OnPropertyChanged(nameof(this.Producer));
      this.OnPropertyChanged(nameof(this.CoverSource));
      this.OnPropertyChanged(nameof(this.PlayPauseImageSource));
    }
  }

  //todo: only temp!!
  //public ICollection<Track> Tracks => TrackQueue.Instance.AllTracks;

  public string Title => this.Track?.Title ?? "no song selected";
  public string Producer => this.Track?.CombinedArtistNames ?? "/";
  public ImageSource CoverSource => this.Track?.Cover.Source ?? ImageSource.FromFile("record.png"); //todo: refac!!
  public string PlayPauseImageSource => this._isPlaying ? "pause.png" : "play.png";
  //public string ShuffleImageSource => this._queue.IsShuffle ? "shuffle_selected.png" : "shuffle.png";

  //colors used for gradient of trackview
  //public Color Color { get; private set; }
  //public Color ColorDark { get; private set; }

  private bool _isPlaying;

  //private readonly TrackQueue _queue;
  private Track _track = null!;
  private Timer _timer;
  private double _progressPercent;

  public TrackPlayerViewModel(TrackQueue queue) {
    this._queue = queue;

    this._timer = new Timer(this._UpdateProgress, null, 0, 500);

    //this.Track = musicService.GetTracks().First();
    //var queue = TrackQueue.Instance;
    //
    //this._queue = queue;
    //this.Track = queue.CurrentTrack;

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
    this._isPlaying = !this._isPlaying;
    this.OnPropertyChanged(nameof(this.PlayPauseImageSource));

    if (this._isPlaying)
      this._queue.Play();
    else
      this._queue.Pause();
  }

  [RelayCommand]
  public void NextTapped() => this._queue.Next();

  [RelayCommand]
  public void PreviousTapped() => this._queue.Previous();

  [RelayCommand]
  public void ShuffleTapped() {
    this._queue.Shuffle();
    //  this.OnPropertyChanged(nameof(this.ShuffleImageSource));
  }

  private void _OnNewSongSelected(object? sender, TrackEventArgs args) {
    this.Track = args.Track;
    this._isPlaying = true;
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
}