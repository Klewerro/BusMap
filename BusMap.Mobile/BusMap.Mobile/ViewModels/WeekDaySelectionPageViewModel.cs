﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using BusMap.Mobile.Helpers;
using BusMap.Mobile.Models;
using Prism.Navigation;

namespace BusMap.Mobile.ViewModels
{
    public class WeekDaySelectionPageViewModel : ViewModelBase
    {
        private ObservableCollection<WeekDaySelectionModel> _days;

        public ObservableCollection<WeekDaySelectionModel> Days
        {
            get => _days;
            set => SetProperty(ref _days, value);
        }


        public WeekDaySelectionPageViewModel(INavigationService navigationService) 
            : base(navigationService)
        {
            Days = new ObservableCollection<WeekDaySelectionModel>();
        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("days"))
            {
                var days = parameters["days"] as List<WeekDaySelectionModel>;
                Days.AddRange(days);
            }
        }

        public override void OnNavigatedFrom(NavigationParameters parameters)
        {
            parameters.Add("days", Days.ToList());
        }

    }
}
