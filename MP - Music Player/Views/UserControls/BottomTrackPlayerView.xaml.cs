using MP_Music_Player.Services;
using MP_Music_Player.ViewModels;

namespace MP_Music_Player.Views.UserControls;

public partial class BottomTrackPlayerView : ContentView {
  public BottomTrackPlayerView() {
    this.InitializeComponent();

    var viewModel = ServiceHelper.GetService<BottomTrackViewModel>(); //todo: find better way

    //workaround bc dependency injection for controls inside contentPages currently doesn't work yet
    this.BindingContext = viewModel;
  }
}
