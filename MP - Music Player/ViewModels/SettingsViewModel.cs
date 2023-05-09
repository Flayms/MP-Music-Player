using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using MP_Music_Player.Services;
using MP_Music_PLayer.Services;

namespace MP_Music_Player.ViewModels; 

public partial class SettingsViewModel : AViewModel {

  [ObservableProperty]
  private string _musicDirectoryPath = null!;

  public string Version => VersionTracking.CurrentVersion;

  private readonly Settings _settings;
  private readonly MusicDirectoryService _musicDirectoryService;

  public SettingsViewModel(Settings settings, MusicDirectoryService musicDirectoryService) {
    this._settings = settings;
    this._musicDirectoryService = musicDirectoryService;
    this.MusicDirectoryPath = settings.MusicDirectory;

    settings.PropertyChanged += this._Settings_PropertyChanged;
  }

  private void _Settings_PropertyChanged(object? _, System.ComponentModel.PropertyChangedEventArgs e) {
    if (e.PropertyName != nameof(Settings.MusicDirectory))
      return;

    this.MusicDirectoryPath = this._settings.MusicDirectory;
  }

  [RelayCommand]
  public async void SelectMusicDirectory() {
    var result = await this._musicDirectoryService.LetUserChangeMusicDirectory();
    if (result == null)
      return;

    this.MusicDirectoryPath = result;
  }
}