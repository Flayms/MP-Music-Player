﻿using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Music_Player_Maui.Models;
using CommunityToolkit.Mvvm.Input;
using Music_Player_Maui.Services;
using Microsoft.UI.Xaml;

namespace Music_Player_Maui.ViewModels;

public abstract partial class ATrackViewModel : AViewModel {
  protected readonly TrackQueue _queue;

  [ObservableProperty]
  [NotifyPropertyChangedFor(nameof(Title))]
  [NotifyPropertyChangedFor(nameof(Producer))]
  [NotifyPropertyChangedFor(nameof(CoverSource))]
  [NotifyPropertyChangedFor(nameof(HasTrack))]
  private Track? _track;

  public bool IsPlaying => this._queue.IsPlaying;

  public bool HasTrack => this.Track != null;
  public string Title => this.Track?.Title ?? "no song selected";
  public string Producer => this.Track?.CombinedArtistNames ?? "/";
  public ImageSource CoverSource => this.Track?.Cover.Source ?? ImageSource.FromFile("record.png"); //todo: refac!!

  public double CurrentPositionInS => this._queue.CurrentTrackPositionInS;

  //todo: currently always shows length of last track after switching
  public double TrackLengthInS => this._queue.CurrentTrackDurationInS;

  //public string ShuffleImageSource => this._queue.IsShuffle ? "shuffle_selected.png" : "shuffle.png";

  //colors used for gradient of trackview
  //public Color Color { get; private set; }
  //public Color ColorDark { get; private set; }

  //private readonly TrackQueue _queue;

  private double _progressPercent;

  protected ATrackViewModel(TrackQueue queue) {
    this._queue = queue;


    var timer = new DispatcherTimer {
      Interval = new TimeSpan(0, 0, 1)
    };

    timer.Tick += this._Timer_Tick;
    timer.Start();

    this.Track = queue.CurrentTrack;

    queue.NewSongSelected += this.OnNewSongSelected;
    queue.IsPlayingChanged += this._IsPlayingChanged;

    //this._GetColors();
  }

  private void _IsPlayingChanged(object? sender, IsPlayingEventArgs e) => this.OnPropertyChanged(nameof(this.IsPlaying));

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
    if (this.IsPlaying)
      this._queue.Pause();
    else
      this._queue.Play();
  }

  protected virtual void OnNewSongSelected(object? sender, TrackEventArgs args) {
    this.Track = args.Track;
    //  this._GetColors();
  }

  private void _Timer_Tick(object? sender, object e) {
    this.ProgressPercent = this._queue.GetProgressPercent();
    this.OnPropertyChanged(nameof(this.CurrentPositionInS));
  }

  public double ProgressPercent {
    get => this._progressPercent;
    set => this.SetProperty(ref this._progressPercent, value);
  }

  [RelayCommand]
  public void TrackPositionChanged() {
    Trace.WriteLine($"Jumping to {this.ProgressPercent}");

    this._queue.JumpToPercent(this.ProgressPercent);
    this.OnPropertyChanged(nameof(this.CurrentPositionInS));
  }

}