using MP_Music_Player.Services;
using MP_Music_Player.ViewModels;

namespace MP_Music_Player.Views.Pages;

public partial class SongsPage : ContentPage {
  public SongsPage() {
    this.InitializeComponent();

    var viewModel = ServiceHelper.GetService<SongsViewModel>(); //todo: find better way

    //workaround bc dependency injection for controls inside contentPages currently doesn't work yet
    this.BindingContext = viewModel;
  }
}
