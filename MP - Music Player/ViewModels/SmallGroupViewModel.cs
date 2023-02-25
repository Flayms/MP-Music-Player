using CommunityToolkit.Mvvm.Input;
using Music_Player_Maui.Models;
using Music_Player_Maui.Services;
using Music_Player_Maui.Views.Pages;

namespace Music_Player_Maui.ViewModels;

public partial class SmallGroupViewModel : AViewModel {
  public string Name { get; }
  public List<Track> Tracks { get; }
  public int TrackAmount => this.Tracks.Count;
  public string TrackAmountUi => this.TrackAmount + " Songs"; //todo: maybe converter for this

  public SmallGroupViewModel(string name, List<Track> tracks) {
    this.Name = name;
    this.Tracks = tracks;
  }

  [RelayCommand]
  public void ShowTracks() {
    var trackViewModels = this.Tracks.Select(t => new SmallTrackViewModel(t)).ToList();

    var model = ServiceHelper.GetService<TrackListViewModel>();
    model.Title = this.Name;
    model.TrackViewModels = trackViewModels;

    Shell.Current.Navigation.PushAsync(new TrackListPage(model));
  }
}