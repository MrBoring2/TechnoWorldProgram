﻿using LiveCharts;
using LiveCharts.Wpf;
using MaterialNotificationLibrary;
using MaterialNotificationLibrary.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnoWorld_WarehouseAccounting.Services;
using TechnoWorld_WarehouseAccounting.ViewModels.Pages.Statistics;
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers.Abstractions;
using WPF_Helpers.Common;
using WPF_VM_Abstractions;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Pages
{
    public class SalesStatisticsPageVM : BasePageVM
    {
        private ObservableCollection<string> statistics;
        private ObservableCollection<string> diagramTypes;
        private string selectedStatistics;
        private string selectedDiagramType;
        private DateTime startDate;
        private DateTime endDate;
        public SalesStatisticsPageVM()
        {
            GenerateStatisticsCommand = new RelayCommand(GenerateStatistics);
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            EndDate = DateTime.Now.Date;
            Statistics = new ObservableCollection<string>
            {
                "Продажи по типам товаров",
                "Продажи по категориям"
            };
            DisagramTypes = new ObservableCollection<string>
            {
                "Линейная диаграмма",
                "Круговая диаграмма"
            };
            SelectedStatistics = Statistics.FirstOrDefault();
            SelectedDiagramType = DisagramTypes.FirstOrDefault();
        }
        public RelayCommand GenerateStatisticsCommand { get; set; }
        public ObservableCollection<string> Statistics { get => statistics; set { statistics = value; OnPropertyChanged(); } }
        public ObservableCollection<string> DisagramTypes { get => diagramTypes; set { diagramTypes = value; OnPropertyChanged(); } }
        public string SelectedStatistics { get => selectedStatistics; set { selectedStatistics = value; OnPropertyChanged(); } }
        public string SelectedDiagramType { get => selectedDiagramType; set { selectedDiagramType = value; OnPropertyChanged(); } }


        public DateTime StartDate
        {
            get => startDate;
            set { startDate = value > endDate && endDate != DateTime.MinValue ? startDate : value; OnPropertyChanged(); }
        }
        public DateTime EndDate
        {
            get => endDate;
            set { endDate = value < startDate && startDate != DateTime.MinValue ? endDate : value; OnPropertyChanged(); }
        }

        private bool PeriodeIsValide()
        {
            if (StartDate.Month - EndDate.Month > 3)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private async void GenerateStatistics(object obj)
        {
            await Task.Run(() => App.Current.Dispatcher.InvokeAsync(() =>
              {
                  if (!PeriodeIsValide() && SelectedDiagramType == "Линейная диаграмма")
                  {
                      MaterialNotification.Show("Внимание", "Для линейной диаграммы период не должен превышать 3 масяца!", MaterialNotificationButton.Ok, MaterialNotificationImage.Warning);
                      return;
                  }

                  if (SelectedStatistics == "Продажи по типам товаров")
                  {
                      if (SelectedDiagramType == "Линейная диаграмма")
                      {
                          StatisticsNavigation.Navigate(new LineChartStatisticPageVM(StartDate, EndDate));
                      }
                      else if (SelectedDiagramType == "Круговая диаграмма")
                      {

                          StatisticsNavigation.Navigate(new PieChartStatisticsPageVM(StartDate, EndDate));
                      }
                  }
                  else if (SelectedStatistics == "Продажи по категориям товаров")
                  {

                  }
              }));
        }
    }
}

