using Music_Player_Maui.Services;

namespace Music_Player_Maui.Models; 

public class Cover {

  private const string _DEFAULT_PIC_PATH = "record.png";

  public ImageSource Source {
    get {

      //first time called code
      if (this.hasPicture == null) {
        var source = this._GetImageSource();
        if (source == null) {
          this.hasPicture = false;
          return ImageSource.FromFile(_DEFAULT_PIC_PATH);
        }

        this.hasPicture = true;
        return source;
      }

      return this.hasPicture.Value
        ? this._GetImageSource()!
        : ImageSource.FromFile(_DEFAULT_PIC_PATH);
    }
  }

  private bool? hasPicture;
  private readonly string _filePath;
  //private Color? _dominantColor;

  public Cover(string filePath) {
    this._filePath = filePath;
  }

  private ImageSource? _GetImageSource() {
    var bytes = this._GetBytes();

    return bytes == null //todo: this is maybe not needed at all
      ? null 
      : ImageSource.FromStream(() => new MemoryStream(bytes));
  }

  private byte[]? _GetBytes() => CoverRetriever.GetCover(this._filePath);
}