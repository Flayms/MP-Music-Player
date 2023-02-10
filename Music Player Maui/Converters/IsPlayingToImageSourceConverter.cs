﻿using System.Globalization;

namespace Music_Player_Maui.Converters; 

internal class IsPlayingToImageSourceConverter : AValueConverter<bool, string> {
  #region Implementation of IValueConverter

  public override string Convert(bool isPlaying, Type targetType, object parameter, CultureInfo culture) => isPlaying ? "pause.png" : "play.png";
  public override bool ConvertBack(string imageSource, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

  #endregion
}