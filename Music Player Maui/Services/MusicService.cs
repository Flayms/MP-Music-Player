using Microsoft.EntityFrameworkCore;
using Music_Player_Maui.Models;

namespace Music_Player_Maui.Services;

/// <summary>
/// A service giving access to all the tracks, genres, artists, etc
/// </summary>
public class MusicService {
  private readonly Settings _settings;
  private readonly MusicContext _context;
  private readonly MusicFileParsingService _musicFileParsingService;
  private readonly TrackQueue _queue;

  public IReadOnlyCollection<Track>? _tracks;

  public bool HasTracks => this._tracks?.Count > 0;

  public bool IsLoading {
    get => this._isLoading;
    private set {
      this._isLoading = value;
      this.LoadingStateChangedEvent?.Invoke(this, new LoadingEventArgs(value));
    }
  }

  public event EventHandler<LoadingEventArgs>? LoadingStateChangedEvent;

  public class LoadingEventArgs : EventArgs {

    public bool IsLoading { get; }

    public LoadingEventArgs(bool isLoading) {
      this.IsLoading = isLoading;
    }

  }

  private readonly ManualResetEvent _finishedLoadingEvent = new(true);
  private CancellationTokenSource? _initializationCancellationToken;
  private bool _isLoading;

  public MusicService(Settings settings, MusicContext context, MusicFileParsingService musicFileParsingService, TrackQueue queue) {
    this._settings = settings;
    this._context = context;
    this._musicFileParsingService = musicFileParsingService;
    this._queue = queue;
  }

  /// <summary>
  /// Needs to be called to load data before it can be accessed.
  /// If it gets called while it's running, cancels running init and runs again.
  /// If it gets called if data is already loaded, reloads data.
  /// </summary>
  public void Init() {
    //todo: extra project for all the loading stuff
    this._initializationCancellationToken?.Cancel();
    this._initializationCancellationToken?.Token.WaitHandle.WaitOne();
    this._finishedLoadingEvent.Set();
    this.IsLoading = true;

    if (!this._settings.ReadFromCache)
      this._InitDatabase();

    this._tracks = this._context.Tracks
      .OrderBy(t => t.Title)
      .ToArray();

    this.IsLoading = false;
    this._settings.ReadFromCache = true;
    this._finishedLoadingEvent.Reset();
  }

  private void _InitDatabase() {
    this._initializationCancellationToken = new CancellationTokenSource();

    if (!this._musicFileParsingService.TryParseAllLocalSongsFromMusicDirectory(this._initializationCancellationToken.Token, out var result))
      return;

    var context = this._context;

    context.ClearAllData();

    context.Tracks.AddRange(result.Tracks);
    context.Artists.AddRange(result.Artists);
    context.Genres.AddRange(result.Genres );

    context.SaveChanges();
  }

  public IReadOnlyCollection<Track> GetTracks() {

    if (this._finishedLoadingEvent.WaitOne(0)) //if is blocking
      this._finishedLoadingEvent.WaitOne();

    if (this._tracks == null)
      this.Init();

    return this._tracks!;
  }

  public async Task<IReadOnlyCollection<Track>> GetTracksAsync() => await Task.Run(this.GetTracks);
}