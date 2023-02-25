﻿using System.Globalization;
using MP_Music_Player.Enums;

namespace MP_Music_Player.Converters;

public class DisplayStateToIsEmptyConverter : AValueConverter<DisplayState, bool> {
  #region Implementation of IValueConverter

  public override bool Convert(DisplayState value, Type targetType, object parameter, CultureInfo culture) => value == DisplayState.Empty;
  public override DisplayState ConvertBack(bool value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

  #endregion
}
