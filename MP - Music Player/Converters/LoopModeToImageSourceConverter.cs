using System.Globalization;
using MP_Music_PLayer.Enums;
namespace MP_Music_PLayer.Converters;

public class LoopModeToImageSourceConverter : AToImageSourceConverter<LoopMode, string> {
  #region Implementation of IValueConverter

  public override string Convert(LoopMode loopMode, Type targetType, object parameter, CultureInfo culture) 
    => loopMode switch { 
      LoopMode.None => $"loop_{this.ThemeName}.png", 
      LoopMode.Queue => "loop_selected.png",
      LoopMode.Current => "loop_current.png", 
      _ => throw new ArgumentOutOfRangeException()
    };

  public override LoopMode ConvertBack(string imageSource, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

  #endregion
}