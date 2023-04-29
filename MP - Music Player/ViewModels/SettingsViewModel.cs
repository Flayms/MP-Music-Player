using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using MP_Music_Player.Services;

namespace MP_Music_Player.ViewModels; 

public partial class SettingsViewModel : AViewModel {
 

  [ObservableProperty]
  private string _musicDirectoryPath = null!;

  public string Version => VersionTracking.CurrentVersion;

  private readonly Settings _settings;
  private readonly MusicService _musicLoadingService;

  public SettingsViewModel(Settings settings, MusicService musicLoadingService) {
    this._settings = settings;
    this._musicLoadingService = musicLoadingService;
    this.MusicDirectoryPath = settings.MusicDirectory;
  }

  [RelayCommand]
  public async void SelectMusicDirectory() {
    try {

      //todo: shouldn't be done here
      await Permissions.RequestAsync<Permissions.StorageRead>();
      await Permissions.RequestAsync<Permissions.StorageWrite>();


      var folderPickerResult = await FolderPicker.Default.PickAsync(CancellationToken.None);
      if (!folderPickerResult.IsSuccessful)
        return;

      var folderPath = folderPickerResult.Folder.Path;

      this.MusicDirectoryPath = folderPath;
      this._settings.MusicDirectory = folderPath;

#pragma warning disable CS4014
      Task.Run(this._musicLoadingService.Init);
#pragma warning restore CS4014

    } catch (Exception) {
      await Toast.Make("No folder picked").Show(CancellationToken.None);
    }
  }
}