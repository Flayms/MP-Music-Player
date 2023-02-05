using Music_Player_Maui.Services;

namespace Music_Player_Maui;

public partial class App : Application {
  private readonly MusicService _musicLoadingServiceX;

  public App(MusicService musicLoadingServiceX) {
    this._musicLoadingServiceX = musicLoadingServiceX;
    this.InitializeComponent();

    this.MainPage = new NavigationPage(new AppShell());
    this.MainPage = new AppShell();
  }

  #region Overrides of Application

  protected override void OnStart() {
    Task.Run(this._musicLoadingServiceX.GetTracksAsync);

    base.OnStart();
  }

  #endregion
}
