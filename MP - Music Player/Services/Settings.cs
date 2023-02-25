namespace Music_Player_Maui.Services;

public class Settings {

  public string MusicDirectory {
    get => this._musicDirectory;
    set {
      this._musicDirectory = value;
      Preferences.Default.Set(nameof(this.MusicDirectory), value);
      this.ReadFromCache = false;
    }
  }

  public bool ReadFromCache {
    get => this._readFromCache;
    set {
      Preferences.Default.Set(nameof(this.ReadFromCache), value);
      this._readFromCache = value;
    }
  }

  private string _musicDirectory;
  private bool _readFromCache;

  public Settings() {
    this._ReadSettings();
  }

  private void _ReadSettings() {
    this._musicDirectory = Preferences.Default.Get(nameof(this.MusicDirectory), Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
    this._readFromCache = Preferences.Default.Get(nameof(this.ReadFromCache), false);
  }

}