﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Birkesoe_Loebet.ViewModels;

namespace Birkesoe_Loebet.Views
{
    /// <summary>
    /// Interaction logic for AddRunnerWindow.xaml
    /// </summary>
    public partial class AddRunnerWindow : Window
    {
        CreateViewModel CreateVM;
        public AddRunnerWindow()
        {
            CreateVM = new CreateViewModel();
            InitializeComponent();
            DataContext = CreateVM;
        }
    }
}
