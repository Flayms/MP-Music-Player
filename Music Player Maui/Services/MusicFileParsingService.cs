using System.Diagnostics;
using Music_Player_Maui.Models;

namespace Music_Player_Maui.Services;

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

    var path = this._settings.MusicDirectory;
    //path = "/storage/9C33-6BBD/Music/music4phone";

    var tracks = new List<Track>();
    var artists = new List<Artist>();
    var genres = new List<Genre>();

    //todo: use searchPattern instead
    var files = Directory
      .EnumerateFiles(path, "*", SearchOption.AllDirectories)
      .Select(f => new FileInfo(f))
      .Where(f => _supportedFormats.Contains(f.Extension))
      .ToArray();

    //load all tracks with artists and genres
    try {
      Parallel.ForEach(files, file => {
        cancellationToken.ThrowIfCancellationRequested();

        if (_supportedFormats.All(f => file.Extension != f))
          return;


        if (!this._tagReadingService.TryReadTags(file, ref artists, ref genres, out var dbTrack))
          return;

        tracks.Add(dbTrack);
        this.OnTrackLoaded?.Invoke(this, new TrackLoadedEventArgs(tracks.Count, files.Length));
      });

    } catch (OperationCanceledException ex) {
      result = null!;
      return false;
    }

    //attach tracks to artists
    foreach (var artist in artists) {
      artist.Tracks = tracks
          .Where(t => t.Artists
            .Contains(artist))
          .ToList();
    }

    //attach tracks to genres

    foreach (var genre in genres) {
      genre.Tracks = tracks
        .Where(t => t.Genres
          .Contains(genre))
        .ToList();
    }

    result = new ParseResult(tracks, artists, genres);
    return true;
  }

}