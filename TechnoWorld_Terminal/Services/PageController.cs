﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TechnoWorld_Terminal.ViewModels.Pages;
using TechnoWorld_Terminal.ViewModels.Windows;
using TechnoWorld_Terminal.Views.Windows;
using WPF_Helpers.Abstractions;

namespace TechnoWorld_Terminal.Services
{
    public class PageController
    {
        private readonly Dictionary<Type, Type> viewModelsToPagesMapping = new Dictionary<Type, Type>();
        private readonly Dictionary<BasePageVM, Page> createdPages = new Dictionary<BasePageVM, Page>();

        public IEnumerable<BasePageVM> Pages => createdPages.Keys;

        public void RegisterPageType<VM, P>()
            where P : Page
            where VM : BasePageVM
        {
            var vmType = typeof(VM);
            viewModelsToPagesMapping[vmType] = typeof(P);
        }

        //public void CreatePage(Type vmType)
        //{
        //    var vm = (BasePageVM)Activator.CreateInstance(vmType);
        //    var page = CreatePageInstanceWithVM(vm);
        //    createdPages[vm] = page;
        //}
        public void ClearPages()
        {
            if (createdPages != null)
            {
                createdPages.Clear();
            }
        }
        public bool IsPageCreated(Type type)
        {
            return createdPages.Any(p => p.Key.GetType() == type);
        }
        public void UnregisterPageType<VM>()
        {
            var vmType = typeof(VM);
            if (!viewModelsToPagesMapping.ContainsKey(vmType))
                throw new InvalidOperationException(
                    $"Тип {vmType.FullName} не зарегистрирован");
            viewModelsToPagesMapping.Remove(vmType);
        }

        public Page CreatePageInstanceWithVM(BasePageVM vm)
        {
            if (vm == null)
                throw new ArgumentNullException("ViewModel is null");
            Type pageType = null;

            var vmType = vm.GetType();
            while (vmType != null && !viewModelsToPagesMapping.TryGetValue(vmType, out pageType))
                vmType = vmType.BaseType;

            if (pageType == null)
                throw new ArgumentException(
                    $"Нет зарегистрированого окна с таким ViewModel: {vm.GetType().FullName}");

            var page = (Page)Activator.CreateInstance(pageType);
            page.DataContext = vm;
            return page;
        }
        public Page GetNewPage(BasePageVM vmPage)
        {
            //if (vm is null)
            //    throw new ArgumentNullException(nameof(vm));

            Page page;

            page = CreatePageInstanceWithVM(vmPage);
            return page;

        }
        public Page GetPage(Type vmPageType)
        {
            Page page;
            if (createdPages.Keys.FirstOrDefault(p => p.GetType() == vmPageType) != null)
            {
                createdPages.TryGetValue(createdPages.Keys.FirstOrDefault(p => p.GetType() == vmPageType), out page);
                return page;
            }
            else
            {
                return CreatePage(vmPageType);
            }
        }
        public Page GetPage(Type vmPageType, params object[] args)
        {
            Page page;
            if (createdPages.Keys.FirstOrDefault(p => p.GetType() == vmPageType) != null)
            {
                createdPages.TryGetValue(createdPages.Keys.FirstOrDefault(p => p.GetType() == vmPageType), out page);
                return page;
            }
            else
            {
                return CreatePage(vmPageType, args);
            }
        }

        public Page CreatePage(Type vmPageType)
        {
            var pageVM = (BasePageVM)Activator.CreateInstance(vmPageType);
            var page = CreatePageInstanceWithVM(pageVM);
            createdPages[pageVM] = page;
            return page;
        }
        public Page CreatePage(Type vmPageType, params object[] args)
        {
            var pageVM = (BasePageVM)Activator.CreateInstance(vmPageType, args);
            var page = CreatePageInstanceWithVM(pageVM);
            createdPages[pageVM] = page;
            return page;
        }

        public void HidePage(Type vmPageType)
        {
            var vm = createdPages.FirstOrDefault(p => p.Key.GetType() == vmPageType).Key;
            if (vm == null)
                return;

            createdPages.Remove(vm);
        }
    }
}