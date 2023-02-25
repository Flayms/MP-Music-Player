using MP_Music_Player.Services;

namespace MP_Music_Player.Models; 

public class Cover {

  private const string _DEFAULT_PIC_PATH = "record.png";

  //todo: makes view slow, make this lazy, put logic into viewModel(s)
  public ImageSource Source {
    get {
      if (this.hasPicture != null)
        return this.hasPicture.Value
          ? this._GetImageSource()!
          : ImageSource.FromFile(_DEFAULT_PIC_PATH); //todo: don't reload every time


      //first time called code
      var source = this._GetImageSource();
      if (source == null) {
        this.hasPicture = false;
        return ImageSource.FromFile(_DEFAULT_PIC_PATH);
      }

      this.hasPicture = true;
      return source;
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