using MP_Music_Player.Models;

namespace MP_Music_Player.ViewModels; 

//todo: add format, length bit-rate, sampling-rate, Album
//todo: add links to discogs, genius, last.fm... like in Hi-Fi Cast
//todo: add option to find similar songs
public class TrackDetailsViewModel {
  private readonly Track _track;

  public string FileName { get; }
  public string FilePath { get; }
  public string Title => this._track.Title;
  public string Artists => this._track.CombinedArtistNames;
  public string Genres => this._track.CombinedGenreNames;

  public TrackDetailsViewModel(Track track) {
    this._track = track;

    var fileInfo = new FileInfo(this._track.Path);
    this.FileName = fileInfo.Name;
    this.FilePath = fileInfo.FullName;
  }
}