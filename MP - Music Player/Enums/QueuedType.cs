namespace MP_Music_Player.Enums; 

public enum QueuedType {

  /// <summary>
  /// Tracks that already played
  /// </summary>
  History,

  /// <summary>
  /// tracks the user selected as "play next".
  /// </summary>
  NextUp,

  /// <summary>
  /// all the tracks playing after NextUp tracks.
  /// </summary>
  Queued
}