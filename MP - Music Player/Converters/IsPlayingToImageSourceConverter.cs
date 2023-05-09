using System.Globalization;

namespace MP_Music_Player.Converters; 

internal class IsPlayingToImageSourceConverter : AValueConverter<bool, string> {
  #region Implementation of IValueConverter

  public override string Convert(bool isPlaying, Type targetType, object parameter, CultureInfo culture) {
    var themeName = Application.Current!.RequestedTheme == AppTheme.Light ? "light" : "dark";

    return isPlaying ? $"pause_{themeName}.png" : $"play_{themeName}.png";
  }

  public override bool ConvertBack(string imageSource, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

  #endregion
}