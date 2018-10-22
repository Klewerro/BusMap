﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BusMap.Mobile.Annotations;
using BusMap.Mobile.Helpers;
using BusMap.Mobile.Models;
using BusMap.Mobile.Services;
using BusMap.Mobile.Views;
using Microsoft.EntityFrameworkCore.Internal;
using Prism;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace BusMap.Mobile.ViewModels
{
    public class TrackNewRouteViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly IPageDialogService _pageDialogService;

        private ObservableCollection<Pin> _mapPins;
        private MapSpan _mapPosition;
        private ObservableCollection<BusStop> _busStops;
        private Carrier _carrier;

        private int _editingElementIndex = -1;
        private bool _saveButtonEnabled;

        public ObservableCollection<Pin> MapPins
        {
            get => _mapPins;
            set => SetProperty(ref _mapPins, value);
        }

        public MapSpan MapPosition
        {
            get => _mapPosition;
            set => SetProperty(ref _mapPosition, value);
        }

        public ObservableCollection<BusStop> BusStops
        {
            get => _busStops;
            set => SetProperty(ref _busStops, value);
        }

        public Carrier Carrier
        {
            get => _carrier;
            set => SetProperty(ref _carrier, value);
        }

        public bool SaveButtonEnabled
        {
            get => _saveButtonEnabled;
            set => SetProperty(ref _saveButtonEnabled, value);
        }


        public TrackNewRouteViewModel(IDataService dataService, INavigationService navigationService,
            IPageDialogService pageDialogService)
            : base (navigationService)
        {
            _dataService = dataService;
            _pageDialogService = pageDialogService;
            Title = "Add route";
            MapPins = new ObservableCollection<Pin>();
            BusStops = new ObservableCollection<BusStop>();

            Carrier = new Carrier
            {
                Name = "Placeholder carrier",
                Id = 2  //Todo: get id carrier checked on list
            };
        }


        public ICommand PopupCommand => new DelegateCommand(async () =>
        {            
            await NavigationService.NavigateAsync(nameof(AddNewBusStopPage));
        });

        public ICommand MapAppearingCommand => new DelegateCommand(async () =>
        {
            try
            {
                var currentPosition = await LocalizationHelpers.GetCurrentUserPositionAsync(true);
                MapPosition = null;
                MapPosition = MapSpan.FromCenterAndRadius(currentPosition.ToGoogleMapsPosition(), Distance.FromKilometers(10));
                if (MapPins == null || MapPins.Count < 1)
                    MapPins.AddRange(BusStops.ToGoogleMapsPins());
            }
            catch (TaskCanceledException)
            {
                MessagingHelper.Toast("Unable to get position.", ToastTime.ShortTime);
            }

        });

        public ICommand EditBusStopCommand => new DelegateCommand<BusStop>(async busStop =>
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("busStopToEdit", busStop);
            _editingElementIndex = BusStops.IndexOf(busStop);

            await NavigationService.NavigateAsync(nameof(EditBusStopPage), navigationParameters);
        });

        public ICommand SaveButtonCommand => new DelegateCommand(async () =>
        {
            var busStopsReversed = BusStops.Reverse().ToList();
            //foreach (var stop in busStopsReversed)
            //{
            //    stop.Id = 0;
            //}

            var route = new Route
            {
                BusStops = busStopsReversed,
                CarrierId = Carrier.Id,
                Name = "Test" //TODO: From Entry
            };

            var dialogAnswer = await _pageDialogService
                .DisplayAlertAsync("Are you sure?", "You would not edit route after it.", "Yes", "No");
            if (dialogAnswer)
            {
                await PostDataAsync(route);
            }

            
        });

        private async Task PostDataAsync(Route route)
        {
            MessagingHelper.Toast("Uploading new route...", ToastTime.ShortTime);
            var result = await _dataService.PostRouteAsync(route);
            if (result)
            {
                //MessagingHelper.Toast("Upload successful!", ToastTime.LongTime);
                await _pageDialogService.DisplayAlertAsync("Success!",
                    "Route added successfully.\nYou can find it in new routes queue.", "Ok");
                await NavigationService.GoBackAsync();
            }
            else
            {
                MessagingHelper.Toast("Upload failed!", ToastTime.ShortTime);
            }
        }



        public override async void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("newBusStop"))
            {
                AddBusStopToLists(parameters["newBusStop"] as BusStop);
                if (BusStops.Count > 1)
                {
                    SaveButtonEnabled = true;
                }
            }
                
            if (parameters.ContainsKey("busStopFromEdit"))
            {
                var busStopFromEdit = parameters["busStopFromEdit"] as BusStop;
                AddEditedBusStopToLists(busStopFromEdit, ref _editingElementIndex);
            }

            if (parameters.ContainsKey("removeBusStopAddress") && parameters.ContainsKey("removeBusStopLabel"))
            {
                var busStopToRemoveLabel = parameters["removeBusStopLabel"] as string;
                var busStopToRemoveAddress = parameters["removeBusStopAddress"] as string;
                await RemoveBusStop(busStopToRemoveAddress, busStopToRemoveLabel);
                if (BusStops.Count < 2)
                {
                    SaveButtonEnabled = false;
                }
            }
        }

        private void AddBusStopToLists(BusStop busStop)
        {
            BusStops.Insert(0, busStop);
            MapPins.Insert(0, busStop.ToGoogleMapsPin());
        }

        private void AddEditedBusStopToLists(BusStop busStop, ref int index)
        {
            BusStops[index] = busStop;
            MapPins.RemoveAt(index);
            MapPins.Insert(index, busStop.ToGoogleMapsPin());
            index = -1;
        }

        private async Task RemoveBusStop(string address, string label)
        {
            var busStopToRemove = BusStops
                .Where(b => b.Address.Equals(address))
                .SingleOrDefault(b => b.Label.Equals(label));

            if (busStopToRemove != null)
            {
                BusStops.Remove(busStopToRemove);
                MapPins.Remove(busStopToRemove.ToGoogleMapsPin());
            }
            else
            {
                await _pageDialogService.DisplayAlertAsync("Alert!", "Could not remove busStop.\nPlease try again.", "Ok");
            }
        }

    }
}
