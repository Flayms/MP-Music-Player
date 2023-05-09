using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using MP_Music_Player.Services;

namespace MP_Music_PLayer.Services; 

public class MusicDirectoryService {

  private readonly Settings _settings;
  private readonly MusicService _musicService;

  public MusicDirectoryService(Settings settings, MusicService musicService) {
    this._settings = settings;
    this._musicService = musicService;
  }

  public async Task<string?> LetUserChangeMusicDirectory() {
    try {

      //todo: shouldn't be done here
      await Permissions.RequestAsync<Permissions.StorageRead>();
      await Permissions.RequestAsync<Permissions.StorageWrite>();

      var folderPickerResult = await FolderPicker.Default.PickAsync(CancellationToken.None);
      if (!folderPickerResult.IsSuccessful)
        return null;

      var folderPath = folderPickerResult.Folder.Path;

      //this.MusicDirectoryPath = folderPath;
      this._settings.MusicDirectory = folderPath;

#pragma warning disable CS4014
      Task.Run(this._musicService.Init);
#pragma warning restore CS4014

      return folderPath;
    } catch (Exception) {
      await Toast.Make("No folder picked").Show(CancellationToken.None);
    }

    return null;
  }

}