using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MP_Music_Player.Services;

public class Settings : INotifyPropertyChanged {

  public string MusicDirectory {
    get => this._musicDirectory;
    set {
      this.SetField(ref this._musicDirectory, value);

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

  private string _musicDirectory = null!;
  private bool _readFromCache;

  public Settings() {
    this._ReadSettings();
  }

  private void _ReadSettings() {
    this._musicDirectory = Preferences.Default.Get(nameof(this.MusicDirectory), Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
    this._readFromCache = Preferences.Default.Get(nameof(this.ReadFromCache), false);
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }


  protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null) {
    if (EqualityComparer<T>.Default.Equals(field, value))
      return false;
    field = value;
    this.OnPropertyChanged(propertyName);
    return true;
  }
}