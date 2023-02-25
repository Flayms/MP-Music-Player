using System.Globalization;

namespace MP_Music_Player.Converters; 

public abstract class AValueConverter<TValue, TTarget> : IValueConverter
  where TValue : notnull
  where TTarget : notnull {

  #region Implementation of IValueConverter

  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
    if (value is not TValue castedValue)
      throw new ArgumentException("Wrong Type!");

    return this.Convert(castedValue, targetType, parameter, culture);
  }

  public abstract TTarget Convert(TValue value, Type targetType, object parameter, CultureInfo culture);

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
    if (value is not TTarget castedValue)
      throw new ArgumentException("Wrong Type!");

    return this.ConvertBack(castedValue, targetType, parameter, culture);
  }

  public abstract TValue ConvertBack(TTarget value, Type targetType, object parameter, CultureInfo culture);

  #endregion
}