using CommunityToolkit.Mvvm.Input;
using Music_Player_Maui.Services;
using Music_Player_Maui.Views.Pages;

namespace Music_Player_Maui.ViewModels; 

public partial class BottomTrackViewModel : ATrackViewModel {
  public BottomTrackViewModel(TrackQueue queue) : base(queue) { }

  [RelayCommand]
  public void OpenBigTrackPage() {
    //todo: solve without getting over ServiceHelper
    var viewModel = ServiceHelper.GetService<BigTrackViewModel>();

    //todo: rather implement as PushModalAsync but needs to look better for windows then
    Shell.Current.Navigation.PushAsync(new BigTrackPage(viewModel));
  }
}