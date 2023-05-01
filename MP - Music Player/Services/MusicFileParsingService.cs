using MP_Music_Player.Models;
using MP_Music_PLayer.Models;

namespace MP_Music_Player.Services;

public class MusicFileParsingService {
  public class TrackLoadedEventArgs : EventArgs {
    public int AmountOfLoadedTracks { get; }
    public int TotalAmountOfTracks { get; }

    public TrackLoadedEventArgs(int amountOfLoadedTracks, int totalAmountOfTracks) {
      this.AmountOfLoadedTracks = amountOfLoadedTracks;
      this.TotalAmountOfTracks = totalAmountOfTracks;
    }
  }

  private readonly TagReadingService _tagReadingService;
  private readonly Settings _settings;

  public static readonly string[] _supportedFormats
    = { ".mp3", ".aac", ".ogg", ".wma", ".alac", ".pcm", ".flac", ".wav" };


  public event EventHandler<TrackLoadedEventArgs>? OnTrackLoaded;

  public MusicFileParsingService(TagReadingService tagReadingService, Settings settings) {
    this._tagReadingService = tagReadingService;
    this._settings = settings;
  }

  public bool TryParseAllLocalSongsFromMusicDirectory(CancellationToken cancellationToken, out ParseResult result) {

    var files = this._GetMusicFiles();

    //load all tracks with artists and genres
    if (!this._LoadTracks(cancellationToken, files,
          out var tracks,
          out var artists,
          out var genres,
          out var albums)) {
      result = null!;
      return false;
    }

    _AttachTracksToArtists(artists, tracks);
    _AttachTracksToGenres(genres, tracks);
    _AttachTracksToAlbums(albums, tracks);

    result = new ParseResult(tracks, artists, genres);
    return true;
  }

  private FileInfo[] _GetMusicFiles() {
    var path = this._settings.MusicDirectory;
    //path = "/storage/9C33-6BBD/Music/music4phone";

    var directory = new DirectoryInfo(path);

    if (!directory.Exists)
      return Array.Empty<FileInfo>();

    try {

      //todo: use searchPattern instead
      var files = directory
        .EnumerateFiles("*", SearchOption.AllDirectories)
        //.Select(f => new FileInfo(f))
        .Where(f => _supportedFormats.Contains(f.Extension))
        .ToArray();

      return files;
    } catch (UnauthorizedAccessException) {
      return Array.Empty<FileInfo>();
    }
  }

  private bool _LoadTracks(CancellationToken cancellationToken, IReadOnlyCollection<FileInfo> files,
    out List<Track> tracks, out List<Artist> artists, out List<Genre> genres, out List<Album> albums) {
    tracks = new List<Track>();
    artists = new List<Artist>();
    genres = new List<Genre>();
    albums = new List<Album>();

    foreach (var file in files) {
      if (cancellationToken.IsCancellationRequested)
        return false;

      if (_supportedFormats.All(f => file.Extension != f))
        continue;

      if (!this._tagReadingService.TryReadTags(file, ref artists, ref genres, ref albums, out var dbTrack))
        continue;

      tracks.Add(dbTrack);
      this.OnTrackLoaded?.Invoke(this, new TrackLoadedEventArgs(tracks.Count, files.Count));
    }

    tracks = tracks.OrderBy(t => t.Title).ToList();
    artists = artists.OrderBy(a => a.Name).ToList();
    genres = genres.OrderBy(g => g.Name).ToList();
    return true;
  }

  private static void _AttachTracksToGenres(IEnumerable<Genre> genres, IList<Track> tracks) {
    foreach (var genre in genres) {
      genre.Tracks = tracks
        .Where(t => t.Genres
          .Contains(genre))
        .ToList();
    }
  }

  private static void _AttachTracksToArtists(IEnumerable<Artist> artists, IList<Track> tracks) {
    foreach (var artist in artists) {
      artist.Tracks = tracks
        .Where(t => t.Artists
          .Contains(artist))
        .ToList();
    }
  }

  private static void _AttachTracksToAlbums(IEnumerable<Album> albums, IList<Track> tracks) {
    foreach (var album in albums)
      album.Tracks = tracks.Where(t => t.Album == album).ToList();
  }

}