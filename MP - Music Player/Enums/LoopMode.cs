namespace MP_Music_PLayer.Enums; 

public enum LoopMode {
  /// <summary>
  /// When queue is over, stop playback.
  /// </summary>
  None = 0,

  /// <summary>
  /// When queue is over, repeat from beginning.
  /// </summary>
  Queue,

  /// <summary>
  /// When track is over, repeat same track again.
  /// </summary>
  Current
}