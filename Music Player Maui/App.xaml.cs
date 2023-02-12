using Music_Player_Maui.Services;

namespace Music_Player_Maui;

public partial class App : Application {
  private readonly MusicService _musicLoadingService;

  public App(MusicService musicLoadingService) {
    this._musicLoadingService = musicLoadingService;
    this.InitializeComponent();

    this.MainPage = new NavigationPage(new AppShell());
    this.MainPage = new AppShell();
  }

  #region Overrides of Application

  protected override void OnStart() {
    Task.Run(this._musicLoadingService.GetTracksAsync);

    base.OnStart();
  }

  #endregion
}
