using Microsoft.EntityFrameworkCore;
using MP_Music_Player.Enums;
using MP_Music_Player.Models;

namespace MP_Music_Player.Services.Repositories;

//todo: helper method for getting tracks of specific type
public class QueuedTracksRepository : IQueuedTracksRepository {

  private readonly MusicContext _context;
  private DbSet<DbQueuedTrack> _queuedTracks => this._context.QueuedTracks;

  //todo: decide if its better for these to be properties or methods
  public IReadOnlyCollection<Track> HistoryTracks => this._queuedTracks
    .Where(qt => qt.Type == QueuedType.History)
    .Select(qt => qt.Track)
    .ToList();

  public IReadOnlyCollection<Track> NextUpTracks => this._queuedTracks
    .Where(qt => qt.Type == QueuedType.NextUp)
    .Select(qt => qt.Track)
    .ToList();

  public IReadOnlyCollection<Track> QueuedTracks => this._queuedTracks
    .Where(qt => qt.Type == QueuedType.Queued)
    .Select(qt => qt.Track)
    .ToList();

  public QueuedTracksRepository(MusicContext context) {
    this._context = context;
  }

  /// <inheritdoc cref="IQueuedTracksRepository.ChangeQueue"/>
  public void ChangeQueue(IReadOnlyCollection<Track> tracks) {
    //cleanup db
    var currentlyQueued = this._context.QueuedTracks
      .Where(qt => qt.Type == QueuedType.Queued);
    this._queuedTracks.RemoveRange(currentlyQueued);

    //now add the items
    this._queuedTracks.AddRange(tracks.Select(t => new DbQueuedTrack(t, QueuedType.Queued)));
    this._context.SaveChanges();
  }

}