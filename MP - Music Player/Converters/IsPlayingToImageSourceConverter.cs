using System.Globalization;
using MP_Music_PLayer.Converters;

namespace MP_Music_Player.Converters; 

internal class IsPlayingToImageSourceConverter : AToImageSourceConverter<bool, string> {
  #region Implementation of IValueConverter

  public override string Convert(bool isPlaying, Type targetType, object parameter, CultureInfo culture)
    => isPlaying ? $"pause_{this.ThemeName}.png" : $"play_{this.ThemeName}.png";

  public override bool ConvertBack(string imageSource, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

  #endregion
}