﻿using Music_Player_Maui.Services;

namespace Music_Player_Maui;

public partial class App : Application {
  private readonly MusicService _musicLoadingService;
  private readonly TrackQueue _queue;

  public App(MusicService musicLoadingService, TrackQueue queue) {
    this._musicLoadingService = musicLoadingService;
    this._queue = queue;
    this.InitializeComponent();

    this.MainPage = new NavigationPage(new AppShell());
    this.MainPage = new AppShell();
  }

  #region Overrides of Application

  protected override void OnStart() {
    Task.Run(this._musicLoadingService.GetTracksAsync);

    base.OnStart();
  }

  protected override void OnSleep() {
    this._queue.SaveToDb();

    base.OnSleep();
  }

  #endregion
}
