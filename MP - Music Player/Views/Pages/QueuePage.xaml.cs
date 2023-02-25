using MP_Music_Player.ViewModels;

namespace MP_Music_Player.Views.Pages;

public partial class QueuePage : ContentPage {
  public QueuePage(QueueViewModel viewModel) {
    this.InitializeComponent();
    this.BindingContext = viewModel;
  }
}
