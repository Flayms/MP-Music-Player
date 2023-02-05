namespace Music_Player_Maui.Services;

public partial class CoverRetriever {

  public static partial byte[]? GetCover(string filePath) {
    var file = TagLib.File.Create(filePath);
    var picture = file.Tag.Pictures.FirstOrDefault();

    return picture?.Data.Data;
  }
}