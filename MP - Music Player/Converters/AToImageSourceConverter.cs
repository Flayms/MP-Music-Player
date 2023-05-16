using MP_Music_Player.Converters;

namespace MP_Music_PLayer.Converters;

public abstract class AToImageSourceConverter<TValue, TTarget> : AValueConverter<TValue, TTarget>
  where TValue : notnull
  where TTarget : notnull {
 public string ThemeName => Application.Current!.RequestedTheme == AppTheme.Light? "light" : "dark";
}