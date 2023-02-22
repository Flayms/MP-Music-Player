using Microsoft.EntityFrameworkCore;

namespace Music_Player_Maui.Extensions; 

public static class DbSetExtensions {

  public static void Clear<T>(this DbSet<T> @this) where T : class {
    foreach (var item in @this) {
      @this.Attach(item);
      @this.Remove(item);
    }
  }

}