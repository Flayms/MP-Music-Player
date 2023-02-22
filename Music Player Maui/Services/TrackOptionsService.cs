﻿using Music_Player_Maui.Enums;
using Music_Player_Maui.Models;
using Music_Player_Maui.ViewModels;
using Music_Player_Maui.Views.Pages;
using Track = Music_Player_Maui.Models.Track;

namespace Music_Player_Maui.Services;

public class TrackOptionsService {
  private const string _CANCEL_TEXT = "Cancel";
  private readonly TrackQueue _queue;

  //todo: BiDictionary
  public static readonly IReadOnlyDictionary<TrackOption, string> OptionTexts = new Dictionary<TrackOption, string> {
    {TrackOption.PlayNext, "Play next"},
    {TrackOption.AddToQueue, "Add to queue" },
    {TrackOption.AddToEndOfQueue, "Add to end of queue"},
    {TrackOption.RemoveFromQueue, "Remove from queue"},
    {TrackOption.AddToPlaylist, "Add to playlist" },
    {TrackOption.RemoveFromPlaylist, "Remove from playlist" },
    {TrackOption.GoToArtist, "Go to artist"},
    {TrackOption.GoToAlbum, "Go to album"},
    {TrackOption.Details, "Details"}
  };

  /// <summary>
  /// The options that are always visible
  /// </summary>
  public static readonly TrackOption[] BasicOptions = {
    TrackOption.PlayNext,
    TrackOption.AddToQueue,
    TrackOption.AddToEndOfQueue,
    //TrackOption.AddToPlaylist,
    TrackOption.GoToArtist,
    TrackOption.Details,
  };

  public TrackOptionsService(TrackQueue queue) {
    this._queue = queue;
  }

  public async Task StartBasicOptionsMenuAsync(Track track)
    => await this._StartOptionsAsync(track, BasicOptions);

  private async Task _StartOptionsAsync(Track track, params TrackOption[] options) {
    var dic = OptionTexts;
    var texts = new string[options.Length];

    for (var i = 0; i < texts.Length; ++i)
      texts[i] = dic[options[i]];

    var selectText = await Shell.Current.DisplayActionSheet(track.ToString(), _CANCEL_TEXT, null, texts);

    if (selectText is null or _CANCEL_TEXT)
      return;

    //todo: handle this with BiDic instead
    await this._ExecuteOption(dic.First(p => p.Value == selectText).Key, track);
  }

  //todo: implement better than giant case chain
  private async Task _ExecuteOption(TrackOption option, Track track) {
    var queue = this._queue;

    switch (option) {
      case TrackOption.PlayNext:
        queue.InsertNextUp(track);
        break;

      case TrackOption.AddToQueue:
        queue.AddToNextUp(track);
        break;

      case TrackOption.AddToEndOfQueue:
        queue.AddToEndOfQueue(track);
        break;

      case TrackOption.RemoveFromQueue:
        queue.Remove(track);
        break;

      case TrackOption.AddToPlaylist:
        throw new NotImplementedException();
      //playlist.DisplayAddMenuAsync(track);
      //break;

      case TrackOption.RemoveFromPlaylist:
        throw new NotImplementedException();

      //todo: give option to select specific artist
      case TrackOption.GoToArtist: //todo: error handling when no artist available
        await _GoToArtist(track);
        break;

      case TrackOption.GoToAlbum:
        throw new NotImplementedException();

      case TrackOption.Details:
        var model = new TrackDetailsViewModel(track);
        await Shell.Current.Navigation.PushAsync(new TrackDetailsPage(model));
        break;

      default:
        throw new ArgumentOutOfRangeException();
    }
  }

  private static async Task _GoToArtist(Track track) {
    if (track.Artists.Count <= 0) {
      await Shell.Current.DisplayAlert("No Artist", $"Couldn't find any Artists for '{track.CombinedName}'", "OK");
      return;
    }

    Artist artist;

    if (track.Artists.Count == 1)
      artist = track.Artists.First();
    else {
      var artistNames = track.Artists.Select(a => a.Name).ToArray();
      var selectText = await Shell.Current.DisplayActionSheet("Select Artist", _CANCEL_TEXT, null, artistNames);

      if (selectText is null or _CANCEL_TEXT)
        return;

      artist = track.Artists.First(a => a.Name == selectText);
    }

    var model = ServiceHelper.GetService<TrackListViewModel>();
    model.TrackViewModels = artist.Tracks.Select(t => new SmallTrackViewModel(t)).ToList();
    model.Title = artist.Name;

    await Shell.Current.Navigation.PushAsync(new TrackListPage(model));
  }

}