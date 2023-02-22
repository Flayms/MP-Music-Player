using Music_Player_Maui.Models;

namespace Music_Player_Maui.Services.Repositories;

public interface IQueuedTracksRepository {
  IReadOnlyCollection<Track> NextUpTracks { get; }
  IReadOnlyCollection<Track> QueuedTracks { get; }

  /// <summary>
  /// Removes all queued tracks in the repository and adds the new list.
  /// </summary>
  /// <param name="tracks">The tracks to be added.</param>
  void ChangeQueue(IReadOnlyCollection<Track> tracks);

  void InsertNextUp(Track track);
  void AddToNextUp(Track track);
  void AddToEndOfQueue(Track track);
  void RemoveFromQueue(Track track);
  bool TryDequeueTrack(out Track track);
}
