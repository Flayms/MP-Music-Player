using Music_Player_Maui.Services;
using Music_Player_Maui.ViewModels;

namespace Music_Player_Maui.Views.Pages;

public partial class SongsPage : ContentPage {
  public SongsPage() {
    this.InitializeComponent();

    var viewModel = ServiceHelper.GetService<SongsViewModel>(); //todo: find better way

    //workaround bc dependency injection for controls inside contentPages currently doesn't work yet
    this.BindingContext = viewModel;
  }
}
