using System.Globalization;
using Music_Player_Maui.Enums;

namespace Music_Player_Maui.Converters;

public class DisplayStateToContentIsVisibleConverter : AValueConverter<DisplayState, bool> {
  #region Implementation of IValueConverter

  public override bool Convert(DisplayState value, Type targetType, object parameter, CultureInfo culture) => value == DisplayState.DisplayingContent;
  public override DisplayState ConvertBack(bool value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

  #endregion
}
