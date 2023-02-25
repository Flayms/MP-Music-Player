using CommunityToolkit.Mvvm.ComponentModel;

namespace MP_Music_Player.ViewModels; 

public abstract class AViewModel : ObservableObject {
  //public event PropertyChangedEventHandler PropertyChanged;

  //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
  //  this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  //}

  //protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {
  //  if (EqualityComparer<T>.Default.Equals(field, value))
  //    return false;

  //  field = value;
  //  this.OnPropertyChanged(propertyName);
  //  return true;
  //}
}