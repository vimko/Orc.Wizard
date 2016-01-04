﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewProjectWizardWindow.xaml.cs" company="Wild Gums">
//   Copyright (c) 2013 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Wizard.Views
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Catel.Threading;
    using Catel.Windows;
    using Catel.Windows.Threading;
    using ViewModels;

    /// <summary>
    /// Interaction logic for WizardWindow.xaml
    /// </summary>
    public partial class WizardWindow
    {
        #region Constructors
        public WizardWindow()
            : this(null)
        {
        }

        public WizardWindow(WizardViewModel viewModel)
            : base(viewModel, DataWindowMode.Custom, infoBarMessageControlGenerationMode: InfoBarMessageControlGenerationMode.Overlay)
        {
            InitializeComponent();
        }

        protected override void OnLoaded(EventArgs e)
        {
            base.OnLoaded(e);

            Dispatcher.BeginInvoke(async () =>
            {
                UpdateOpacityMask();
            });
        }

        protected override void OnViewModelPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnViewModelPropertyChanged(e);

            if (e.HasPropertyChanged("CurrentPage"))
            {
                Dispatcher.BeginInvoke(async () =>
                {
                    breadcrumb.CenterSelectedItem();

                    // We need to await the animation
                    await TaskShim.Delay(WizardConfiguration.AnimationDuration);

                    UpdateOpacityMask();
                });
            }
        }

        private void UpdateOpacityMask()
        {
            var scrollViewer = breadcrumb.FindVisualDescendantByType<ScrollViewer>();
            if (scrollViewer == null)
            {
                return;
            }

            var opacityMask = new LinearGradientBrush();
            if (scrollViewer.HorizontalOffset > 0d)
            {
                opacityMask.GradientStops.Add(new GradientStop(Colors.Transparent, 0d));
                opacityMask.GradientStops.Add(new GradientStop(Colors.Black, 0.05d));
            }

            var scrollableWidth = scrollViewer.ScrollableWidth;
            if (scrollableWidth > scrollViewer.HorizontalOffset)
            {
                opacityMask.GradientStops.Add(new GradientStop(Colors.Black, 0.95d));
                opacityMask.GradientStops.Add(new GradientStop(Colors.Transparent, 1d));
            }

            breadcrumb.OpacityMask = opacityMask.GradientStops.Count > 0 ? opacityMask : null;
        }
        #endregion
    }
}