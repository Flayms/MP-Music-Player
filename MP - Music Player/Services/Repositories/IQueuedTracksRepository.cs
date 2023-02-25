using MP_Music_Player.Models;

namespace MP_Music_Player.Services.Repositories;

public interface IQueuedTracksRepository {
  IReadOnlyCollection<Track> NextUpTracks { get; }
  IReadOnlyCollection<Track> QueuedTracks { get; }

  /// <summary>
  /// Removes all queued tracks in the repository and adds the new list.
  /// </summary>
  /// <param name="tracks">The tracks to be added.</param>
  void ChangeQueue(IReadOnlyCollection<Track> tracks);
}
