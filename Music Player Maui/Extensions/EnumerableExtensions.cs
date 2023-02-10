namespace Music_Player_Maui.Extensions; 

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

  public static T Dequeue<T>(this IList<T> @this) {
    var item = @this.First();
    @this.RemoveAt(0);
    return item;
  }

  public static int IndexOf<T>(this IReadOnlyList<T> list, T item) where T : class {
    for (var i = 0; i < list.Count; ++i)
      if (list[i].Equals(item))
        return i;

    return -1;
  }
}