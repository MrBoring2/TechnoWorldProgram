using TechnoWorld_Terminal.ViewModels.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnoWorld_Programm.POCO_Models;

namespace TechnoWorld_Terminal.ViewModels.Windows
{
    public class ElectronicDetailWindowVM : ModalWindowVMBase
    {
        private string model;
        private Manufacturer manfacturer;
        private ElectrnicsType electrnicsType;
        private decimal price;
        private int amountInStorage;
        private string description;
        private byte[] image;
        private string color;
        private string manufacturerCountry;
        private int harantyMonth;
        private double weight;
        public ElectronicDetailWindowVM(Electronic electronic)
        {
            CurrentElectronic = electronic;
        }
        private Electronic CurrentElectronic { get; set; }
        public string Model
        {
            get { return model; }
            set { model = value; OnPropertyChanged(); }
        }

        public Manufacturer Manufacturer
        {
            get { return manfacturer; }
            set { manfacturer = value; OnPropertyChanged(); }
        }
        public ElectrnicsType ElectronicType
        {
            get { return electrnicsType; }
            set { electrnicsType = value; OnPropertyChanged(); }
        }
        public decimal Price
        {
            get { return price; }
            set { price = value; OnPropertyChanged(); }
        }
        public int AmountInStorage
        {
            get { return amountInStorage; }
            set { amountInStorage = value; OnPropertyChanged(); }
        }
        public string Description
        {
            get { return description; }
            set { description = value; OnPropertyChanged(); }
        }
        public byte[] Image
        {
            get => image;
            set
            {
                image = value;
                OnPropertyChanged();
            }
        }
        public string Color
        {
            get => color;
            set
            {
                color = value;
                OnPropertyChanged();
            }
        }

        public string ManufacturerCountry
        {
            get => manufacturerCountry;
            set
            {
                manufacturerCountry = value;
                OnPropertyChanged();
            }
        }

        public int HarantyMonth
        {
            get => harantyMonth;
            set
            {
                harantyMonth = value;
                OnPropertyChanged();
            }
        }

        public double Weight
        {
            get => weight;
            set
            {
                weight = value;
                OnPropertyChanged();
            }
        }

        private void InitializeFields()
        {
            Model = CurrentElectronic.Model;
            Manufacturer = CurrentElectronic.Manufacturer;
            ElectronicType = CurrentElectronic.Type;
            Price = CurrentElectronic.Price;
            AmountInStorage = CurrentElectronic.AmountInStorage;
            Description = CurrentElectronic.Description;
            Image = CurrentElectronic.DisplayImage;
            Color = CurrentElectronic.Color;
            Weight = CurrentElectronic.Weight;
            ManufacturerCountry = CurrentElectronic.ManufacturerCountry;
            HarantyMonth = CurrentElectronic.HarantyMonth;
        }
    }
}
