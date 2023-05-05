using System.Globalization;
using MP_Music_Player.Converters;
using MP_Music_PLayer.Enums;

namespace MP_Music_PLayer.Converters; 

public class LoopModeToImageSourceConverter : AValueConverter<LoopMode, string> {
  #region Implementation of IValueConverter

  public override string Convert(LoopMode loopMode, Type targetType, object parameter, CultureInfo culture) {
    return loopMode switch {
      LoopMode.None => "loop.png",
      LoopMode.LoopQueue => "loop_selected.png",
      LoopMode.LoopCurrent => "loop_current.png",
      _ => throw new ArgumentOutOfRangeException()
    };
  }

  public override LoopMode ConvertBack(string imageSource, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

  #endregion
}