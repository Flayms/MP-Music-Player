namespace MP_Music_Player.Extensions; 

public static class EnumerableExtensions {

  private static readonly Random _rnd = new();

  public static void Shuffle<T>(this IList<T> @this) {
    var i = @this.Count;

    while (i > 1) {
      i--;
      var k = _rnd.Next(i + 1);
      (@this[k], @this[i]) = (@this[i], @this[k]);
    }
  }

  /// <summary>
  /// Removes and returns the item at the beginning of the <see cref="IList{T}"/>.
  /// </summary>
  /// <typeparam name="T">The type of the item.</typeparam>
  /// <param name="this">This list.</param>
  /// <returns>The dequeued item.</returns>
  public static T Dequeue<T>(this IList<T> @this) {
    var item = @this.First();
    @this.RemoveAt(0);
    return item;
  }

  /// <summary>
  /// Removes and returns the item at the end of the <see cref="IList{T}"/>
  /// </summary>
  /// <typeparam name="T">The type of the item.</typeparam>
  /// <param name="this">This list.</param>
  /// <returns>The popped item.</returns>
  public static T Pop<T>(this IList<T> @this) {
    var item = @this.Last();
    @this.RemoveAt(@this.Count - 1);
    return item;
  }

  public static int IndexOf<T>(this IReadOnlyList<T> list, T item) where T : class {
    for (var i = 0; i < list.Count; ++i)
      if (list[i].Equals(item))
        return i;

    return -1;
  }
}