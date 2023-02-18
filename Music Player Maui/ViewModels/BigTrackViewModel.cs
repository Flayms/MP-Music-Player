﻿using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Music_Player_Maui.Models;
using Music_Player_Maui.Services;

namespace Music_Player_Maui.ViewModels; 

public partial class BigTrackViewModel : ATrackViewModel {

  private readonly TrackOptionsService _trackOptionsService;

  public BigTrackViewModel(TrackQueue queue, TrackOptionsService trackOptionsService) : base(queue) {
    this._trackOptionsService = trackOptionsService;
  }

  #region Overrides of ATrackViewModel

  protected override void OnNewSongSelected(object? sender, TrackEventArgs args) {
    base.OnNewSongSelected(sender, args);
    this.OnPropertyChanged(nameof(this.TrackLengthInS));
  }

  #endregion

  [RelayCommand]
  public void Next() => this._queue.Next();

  [RelayCommand]
  public void Previous() => this._queue.Previous();

  [RelayCommand]
  public void Shuffle() {
    this._queue.Shuffle();
    //  this.OnPropertyChanged(nameof(this.ShuffleImageSource));
  }

  [RelayCommand]
  public async void ShowOptions() {
    await this._trackOptionsService.StartBasicOptionsMenuAsync(this.Track);
  }

  [RelayCommand]
  public async void ClosePage() {
    await Shell.Current.Navigation.PopModalAsync();
  }
}