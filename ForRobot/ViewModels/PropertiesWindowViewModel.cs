using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

using AvalonDock.Themes;
using AvalonDock.Layout;

using ForRobot.Models.Settings;

namespace ForRobot.ViewModels
{
    public class PropertiesWindowViewModel : BaseClass
    {
        private Settings _settings;

        private ICommand _editPathForUpdateCommand;
        private ICommand _standartSettingsCommand;
        private ICommand _selectClosedControlCommand;

        #region Public variables

        public Settings Settings { get => this._settings; set => Set(ref this._settings, value); }

        /// <summary>
        /// Коллекция панелей макета интерфейса
        /// </summary>
        public List<LayoutAnchorable> Anchorables
        {
            get
            {
                var dockingManager = (App.Current.MainWindow as ForRobot.Views.Windows.MainWindow).DockingManeger;
                return dockingManager.Layout.Descendents().OfType<LayoutAnchorable>().ToList();
            }
        }

        public ObservableCollection<HorizontalAlignment> HorizontalAlignments { get; } = new ObservableCollection<HorizontalAlignment>(Enum.GetValues(typeof(HorizontalAlignment)).Cast<HorizontalAlignment>().ToList<HorizontalAlignment>());
        public ObservableCollection<VerticalAlignment> VerticalAlignments { get; } = new ObservableCollection<VerticalAlignment>(Enum.GetValues(typeof(VerticalAlignment)).Cast<VerticalAlignment>().ToList<VerticalAlignment>());

        #region Commands

        /// <summary>
        /// Команда изменения директивы каталога с новой версией программы
        /// </summary>
        public ICommand EditPathForUpdateCommand
        {
            get => this._editPathForUpdateCommand ?? (this._editPathForUpdateCommand = new RelayCommand(_ => 
            {
                if (!ForRobot.Libr.AppWindowManager.PinCode(ForRobot.Properties.Settings.Default.PinCode))
                    return;

                using (var fbd = new System.Windows.Forms.FolderBrowserDialog() { SelectedPath = Properties.Settings.Default.UpdatePath })
                {
                    System.Windows.Forms.DialogResult result = fbd.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        Properties.Settings.Default.UpdatePath = fbd.SelectedPath;
                        Properties.Settings.Default.Save();
                    }
                }
            }));
        }

        /// <summary>
        /// Команда возвращения к стандартным настройкам
        /// </summary>
        public ICommand DefaultSettingsCommand
        {
            get => this._standartSettingsCommand ?? (this._standartSettingsCommand = new RelayCommand(_ =>
            {
                ForRobot.Themes.Colors.DefaultColors();
                this.Settings = new Settings();
            }));
        }

        /// <summary>
        /// Команда сохранения настроек
        /// </summary>
        public ICommand SaveSettingsCommand { get; }

        /// <summary>
        /// Выбор закрытого элемента управления
        /// </summary>
        public ICommand SelectClosedControlCommand
        {
            get => this._selectClosedControlCommand ?? (this._selectClosedControlCommand = new RelayCommand(obj =>
            {
                if (!ForRobot.Libr.AppWindowManager.PinCode(ForRobot.Properties.Settings.Default.PinCode))
                    return;

                ForRobot.Libr.ControlExtensions.FocusCancel(((System.Windows.RoutedEventArgs)obj).Source as System.Windows.Controls.Control);

                //switch (obj)
                //{
                //    case AvalonDock.Layout.LayoutAnchorable layoutContent:
                //        layoutContent.IsSelected = false;
                //        layoutContent.IsActive = false;
                //        layoutContent.Hide();
                //        break;



                //    default:
                //        if (obj == null) return;

                //        var control = obj as System.Windows.Controls.Control;

                //        // Сброс фокуса для всех областей фокуса
                //        var focusScope = FocusManager.GetFocusScope(control);
                //        FocusManager.SetFocusedElement(focusScope, null);

                //        // Дополнительно: поиск и сброс фокуса во всех дочерних элементах
                //        var children = ForRobot.Libr.DependencyObjectExtensions.FindVisualChildren<UIElement>(control);
                //        foreach (var child in children)
                //        {
                //            if (child.IsKeyboardFocused)
                //            {
                //                Keyboard.ClearFocus();
                //                break;
                //            }
                //        }
                //        break;
                //}
            }));
        }

        #endregion Commands

        #endregion Public variables

        public PropertiesWindowViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            this.Settings = App.Current.Settings.Clone() as Settings;
        }
    }
}
