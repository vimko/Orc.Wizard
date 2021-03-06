﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BreadcrumbItem.xaml.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Wizard.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using Catel.Collections;

    public sealed partial class BreadcrumbItem
    {
        public BreadcrumbItem()
        {
            InitializeComponent();
        }

        public IWizardPage Page
        {
            get { return (IWizardPage)GetValue(PageProperty); }
            set { SetValue(PageProperty, value); }
        }

        public static readonly DependencyProperty PageProperty = DependencyProperty.Register(nameof(Page), typeof(IWizardPage),
            typeof(BreadcrumbItem), new PropertyMetadata(null, (sender, e) => ((BreadcrumbItem)sender).OnPageChanged()));


        public IWizardPage CurrentPage
        {
            get { return (IWizardPage)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }

        public static readonly DependencyProperty CurrentPageProperty = DependencyProperty.Register(nameof(CurrentPage), typeof(IWizardPage),
            typeof(BreadcrumbItem), new PropertyMetadata(null, (sender, e) => ((BreadcrumbItem)sender).OnCurrentPageChanged()));


        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string),
            typeof(BreadcrumbItem), new PropertyMetadata(string.Empty));


        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof(Description), typeof(string),
            typeof(BreadcrumbItem), new PropertyMetadata(string.Empty));


        public int Number
        {
            get { return (int)GetValue(NumberProperty); }
            set { SetValue(NumberProperty, value); }
        }

        public static readonly DependencyProperty NumberProperty = DependencyProperty.Register(nameof(Number), typeof(int),
            typeof(BreadcrumbItem), new PropertyMetadata(0));

        private void OnPageChanged()
        {
            var page = Page;
            if (page != null)
            {
                SetCurrentValue(NumberProperty, page.Number);
                SetCurrentValue(TitleProperty, page.BreadcrumbTitle ?? page.Title);
                SetCurrentValue(DescriptionProperty, page.Description);

                pathline.SetCurrentValue(VisibilityProperty, page.Wizard.IsLastPage(page) ? Visibility.Collapsed : Visibility.Visible);
            }
        }

        private void OnCurrentPageChanged()
        {
            var isSelected = ReferenceEquals(CurrentPage, Page);
            var isCompleted = Page.Number < CurrentPage.Number;

            UpdateContent(isCompleted);
            UpdateSelection(isSelected, isCompleted);
        }

        private void UpdateSelection(bool isSelected, bool isCompleted)
        {
            UpdateShapeColor(pathline, isCompleted && !isSelected);
            UpdateShapeColor(ellipse, isSelected || isCompleted);

            txtTitle.SetCurrentValue(System.Windows.Controls.TextBlock.ForegroundProperty, isSelected ? Brushes.Black : Brushes.DimGray);
        }

        private void UpdateContent(bool isCompleted)
        {
            ellipseText.SetCurrentValue(VisibilityProperty, isCompleted ? Visibility.Hidden : Visibility.Visible);
            ellipseCheck.SetCurrentValue(VisibilityProperty, isCompleted ? Visibility.Visible : Visibility.Hidden);
        }

        private void UpdateShapeColor(Shape shape, bool isSelected)
        {
            var storyboard = new Storyboard();

            if (shape != null && shape.Fill is null)
            {
#pragma warning disable WPF0041 // Set mutable dependency properties using SetCurrentValue.
                shape.Fill = (SolidColorBrush)TryFindResource(DefaultColorNames.AccentColorBrush4) ?? new SolidColorBrush(DefaultColors.AccentColor4);
#pragma warning restore WPF0041 // Set mutable dependency properties using SetCurrentValue.
            }

            var fromColor = ((SolidColorBrush)shape?.Fill)?.Color ?? DefaultColors.AccentColor4;
            var targetColor = this.GetAccentColorBrush(isSelected).Color;

            var colorAnimation = new ColorAnimation(fromColor, (Color)targetColor, WizardConfiguration.AnimationDuration);
            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("Fill.(SolidColorBrush.Color)", ArrayShim.Empty<object>()));

            storyboard.Children.Add(colorAnimation);

            storyboard.Begin(shape);
        }
    }
}
