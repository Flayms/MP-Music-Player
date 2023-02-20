namespace Music_Player_Maui.Enums; 

public enum QueuedType {

  /// <summary>
  /// Tracks that already played
  /// </summary>
  History,

  /// <summary>
  /// The current playing track. Always only one item.
  /// </summary>
  Current,

  /// <summary>
  /// tracks the user selected as "play next".
  /// </summary>
  NextUp,

  /// <summary>
  /// all the tracks playing after NextUp tracks.
  /// </summary>
  Queued
}