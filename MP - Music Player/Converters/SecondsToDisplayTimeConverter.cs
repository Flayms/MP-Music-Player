using System.Globalization;

namespace MP_Music_Player.Converters; 

public class SecondsToDisplayTimeConverter : AValueConverter<double, string> {
  #region Overrides of AValueConverter<double,string>

  public override string Convert(double value, Type targetType, object parameter, CultureInfo culture) {
    var timeSpan = new TimeSpan(0, 0, (int)value);
    return timeSpan.ToString("g");
  }

  public override double ConvertBack(string value, Type targetType, object parameter, CultureInfo culture) {
    throw new NotImplementedException();
  }

  #endregion
}