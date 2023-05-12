using Android.Media;

namespace MP_Music_Player.Services; 

public partial class CoverRetriever {

  public static partial byte[]? GetCover(string filePath) {
    var reader = new MediaMetadataRetriever();
    reader.SetDataSource(filePath);
    return reader.GetEmbeddedPicture();
  }
}