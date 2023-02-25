using CommunityToolkit.Mvvm.Input;
using MP_Music_Player.Views.Pages;

namespace MP_Music_Player.ViewModels; 

public partial class TabLibraryViewModel : AViewModel {


  public TabLibraryViewModel() { }


  [RelayCommand]
  public async Task NavigateToSearch() {
    //Shell.Current.CurrentPage.Title = "Search";
    await Shell.Current.GoToAsync($"///{nameof(SearchPage)}");
  }

}