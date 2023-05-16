using System.Globalization;
using MP_Music_Player.Converters;

namespace MP_Music_PLayer.Converters; 

public class IsShuffleToImageSourceConverter : AToImageSourceConverter<bool, string> {
  #region Overrides of AValueConverter<bool,string>

  public override string Convert(bool value, Type targetType, object parameter, CultureInfo culture)
    => value ? "shuffle_selected.png" : $"shuffle_{this.ThemeName}.png";

  public override bool ConvertBack(string value, Type targetType, object parameter, CultureInfo culture) {
    throw new NotImplementedException();
  }

  #endregion
}