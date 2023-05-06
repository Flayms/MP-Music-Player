using MP_Music_Player.Services;

namespace MP_Music_Player;

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

#if ANDROID
  //android app lifecycle works different than windows
  protected override void OnSleep() {
    this._queue.SaveToDb();
    base.OnSleep();
  }
#endif

  protected override void OnStart() {
    Task.Run(this._SetupAsync);
    base.OnStart();
  }

  private async void _SetupAsync() {
    await this._musicLoadingService.GetTracksAsync();
    this._queue.LoadTracksFromDb();
  }

#endregion

}