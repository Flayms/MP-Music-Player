using MP_Music_Player.ViewModels;

namespace MP_Music_PLayer.Models; 

public class TrackGroup : List<SmallTrackViewModel> {

  public string Name { get; }
  public IEnumerable<SmallTrackViewModel> Tracks { get; }

  public TrackGroup(string name, IList<SmallTrackViewModel> tracks) : base(tracks){
    this.Name = name;
    this.Tracks = tracks;
  }

  #region Overrides of Object

  public override string ToString() => this.Name;

  #endregion
}