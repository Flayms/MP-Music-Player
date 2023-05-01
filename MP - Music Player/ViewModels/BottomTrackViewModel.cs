using CommunityToolkit.Mvvm.Input;
using MP_Music_Player.Services;
using MP_Music_Player.Views.Pages;

namespace MP_Music_Player.ViewModels; 

public partial class BottomTrackViewModel : ATrackViewModel {
  public BottomTrackViewModel(TrackQueue queue, AudioPlayer player) : base(queue, player) { }

  [RelayCommand]
  public void OpenBigTrackPage() {
    //todo: solve without getting over ServiceHelper
    var viewModel = ServiceHelper.GetService<BigTrackViewModel>();

    //todo: rather implement as PushModalAsync but needs to look better for windows then
    Shell.Current.Navigation.PushAsync(new BigTrackPage(viewModel));
  }
}