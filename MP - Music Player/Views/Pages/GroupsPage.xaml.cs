using MP_Music_Player.Enums;
using MP_Music_Player.ViewModels;

namespace MP_Music_Player.Views.Pages;

//todo: display image for groups

/// <summary>
/// A page displaying the tracks grouped by a category.
/// All possible categories are in the <see cref="GroupType"/> enum.
/// </summary>
public partial class GroupsPage : ContentPage {

  private readonly GroupsViewModel _viewModel;

  public GroupsPage(GroupsViewModel viewModel) {
    this._viewModel = viewModel;
    this.InitializeComponent();
    this.BindingContext = this._viewModel = viewModel;
  }

  protected override void OnNavigatedTo(NavigatedToEventArgs args) {
    // Hack: Get the GroupType
    var groupType = this.GetGroupTypeFromRoute();

    this._viewModel.SetGroupType(groupType);
    base.OnNavigatedTo(args);
  }


  private GroupType GetGroupTypeFromRoute() {
    // Hack: As the shell can't define query parameters
    // in XAML, we have to parse the route. 
    // as a convention the last route section defines the GroupType.
    var route = Shell.Current.CurrentState.Location
      .OriginalString.Split("-").LastOrDefault();

    return Enum.Parse<GroupType>(route!);
  }
}