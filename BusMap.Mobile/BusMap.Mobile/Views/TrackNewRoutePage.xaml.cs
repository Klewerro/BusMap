﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusMap.Mobile.Helpers;
using BusMap.Mobile.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BusMap.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrackNewRoutePage : ContentPage
    {
        public TrackNewRoutePage()
        {
            InitializeComponent();
        }

        private void ShowEditRoutePopup(object obj, EventArgs e)
        {

        }

        private void EditMenuItem_OnClicked(object sender, EventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            StackLayout stackItem = menuItem.Parent as StackLayout;
            var busStop = stackItem.BindingContext as BusStop;
            //Navigation.Pu


            //Button button = sender as Button;
            //Grid gridItem = button.Parent as Grid;
            //var route = gridItem.BindingContext as Route;
            //Navigation.PushAsync(new BusStopsMap(new BusStopsMapViewModel(route)));
        }
    }
}