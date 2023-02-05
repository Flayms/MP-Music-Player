namespace Music_Player_Maui.Models;

public class ParseResult {

  public IReadOnlyCollection<Track> Tracks { get; }
  public IReadOnlyCollection<Artist> Artists { get; }
  public IReadOnlyCollection<Genre> Genres { get; }

  public ParseResult(IEnumerable<Track> tracks, IEnumerable<Artist> artists, IEnumerable<Genre> genres) {
    this.Tracks = tracks.ToList();
    this.Artists = artists.ToList();
    this.Genres = genres.ToList();
  }

}