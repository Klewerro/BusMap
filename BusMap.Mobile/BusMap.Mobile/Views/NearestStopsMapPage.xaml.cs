﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusMap.Mobile.Helpers;
using BusMap.Mobile.Models;
using BusMap.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BusMap.Mobile.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NearestStopsMapPage : ContentPage
	{
		public NearestStopsMapPage ()
		{
			InitializeComponent ();
		}

	    public NearestStopsMapPage(Route route)
	    {
            InitializeComponent();
	        ((NearestStopsMapPageViewModel)this.BindingContext).Pins = route.BusStops.ConvertToMapPins();
            
        }
	}
}