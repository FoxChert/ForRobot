using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Interop;

using HelixToolkit.Wpf;

namespace ForRobot.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private variables        
        
        #endregion

        #region Public variables      
        
        #endregion

        #region Constructr

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Private functions

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_SHOWME)
            {
                Show();
            }
            return IntPtr.Zero;
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            lock (App.Current)
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(new Libr.Messages.SaveLayoutMessage());

                if (App.Current.RobotsCollection.Where(robot => robot.IsConnection).Count() > 0)
                {
                    if (MessageBox.Show($"Закрыть соединение?", "Закрытие приложения", MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
                    {
                        e.Cancel = true;
                        return;
                    }
                    else
                    {
                        foreach (var robot in App.Current.RobotsCollection)
                        {
                            robot.Dispose();
                        }
                    }
                }
            }
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
                }
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
                }
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var viewport = sender as HelixViewport3D;
            var firstHit = viewport?.Viewport.FindHits(e.GetPosition(viewport))?.FirstOrDefault();
            //if (firstHit != null)
            //    this.ViewModel.Select(firstHit.Visual);
            //else
            //    this.ViewModel.Select(null);
        }

        #endregion

        #region Protected function
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }

        #endregion

        #region Public functions
        public new void Show()
        {
            base.Show();
            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }
            this.Activate();
        }

        #endregion
    }
}
