using Android.Media;

namespace Music_Player_Maui.Services; 

public partial class CoverRetriever {

  public static partial byte[]? GetCover(string filePath) {
    var reader = new MediaMetadataRetriever();
    reader.SetDataSource(filePath);
    return reader.GetEmbeddedPicture();
  }
}