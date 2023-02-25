using Music_Player_Maui.Enums;

namespace Music_Player_Maui.Models; 

public class DbQueuedTrack {

  //allows bigger values since id-incrementation of queue could become quite high
  public ulong Id { get; set; }
  public Track Track { get; set; } = null!;
  public QueuedType Type { get; set; }
}