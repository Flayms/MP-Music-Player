using System.Globalization;
using Music_Player_Maui.Enums;

namespace Music_Player_Maui.Converters;

public class DisplayStateToIsLoadingConverter : IValueConverter {
  #region Implementation of IValueConverter

  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
    var displayState = (DisplayState)value;

    return displayState == DisplayState.Loading;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
    throw new NotImplementedException();
  }

  #endregion
}
