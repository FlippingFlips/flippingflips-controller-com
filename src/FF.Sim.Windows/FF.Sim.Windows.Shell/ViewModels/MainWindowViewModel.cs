using GongSolutions.Wpf.DragDrop;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace FF.Sim.Windows.Shell.ViewModels
{
    public class MainWindowViewModel : BindableBase, IDropTarget
    {
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private string _title = "Flipping Flips - Helper";

        public MainWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            NavigateCommand = new DelegateCommand<string>(Navigate);
            ShowFlyoutCommand = new DelegateCommand<string>(ShowFlyout);
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public DelegateCommand<string> NavigateCommand { get; private set; }
        public DelegateCommand<string> ShowFlyoutCommand { get; private set; }

        private void ShowFlyout(string showFlyout)
        {
            if (!_isAboutOpen)
            {
                IsAboutOpen = true;
            }                
        }

        private bool _isAboutOpen;
        public bool IsAboutOpen
        {
            get { return _isAboutOpen; }
            set { SetProperty(ref _isAboutOpen, value); }
        }

        #region Methods
        private void Navigate(string uri)
        {
            switch (uri)
            {
                case "":                    
                default:
                    break;
            }

            //_regionManager.RequestNavigate(RegionNames.ContentRegion, uri);

            //if (uri == "MasterDatabaseView")
            //{                
            //    _eventAggregator.GetEvent<DisableControlsEvent>().Publish(false);                
            //}
            //else
            //    _eventAggregator.GetEvent<DisableControlsEvent>().Publish(true);

            //
        }
        #endregion


        public void DragOver(IDropInfo dropInfo)
        {
            var dataObject = dropInfo.Data as IDataObject;
            // look for drag&drop new files
            if (dataObject != null && dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                dropInfo.Effects = DragDropEffects.Copy;
            }
            else
            {
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        private string tableDropA;
        public string TableDropA
        {
            get { return tableDropA; }
            set { SetProperty(ref tableDropA, value); }
        }

        private string tableDropB;
        public string TableDropB
        {
            get { return tableDropB; }
            set { SetProperty(ref tableDropB, value); }
        }

        public void Drop(IDropInfo dropInfo)
        {
            var dataObject = dropInfo.Data as DataObject;
            // look for drag&drop new files
            if (dataObject != null && dataObject.ContainsFileDropList())
            {
                this.HandleDropActionAsync(dropInfo, dataObject.GetFileDropList());
            }
            else
            {
                GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
                var data = dropInfo.Data;
                // do something with the data
            }
        }

        private void HandleDropActionAsync(IDropInfo dropInfo, StringCollection stringCollection)
        {
            var groupBoxDrop = dropInfo.VisualTarget as GroupBox;
            if(groupBoxDrop != null)
            {
                if (groupBoxDrop.Name == "TableDropA")
                {
                    TableDropA = stringCollection[0] ?? String.Empty;
                }
                else
                {
                    TableDropB = stringCollection[0] ?? String.Empty;
                }
            }            
        }
    }
}
