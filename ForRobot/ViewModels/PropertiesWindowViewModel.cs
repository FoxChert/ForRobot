using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

using AvalonDock.Themes;
using AvalonDock.Layout;

using ForRobot.Models.Settings;
using ForRobot.Models.Detals;

namespace ForRobot.ViewModels
{
    public class PropertiesWindowViewModel : BaseClass
    {
        private Settings _settings;

        private ObservableCollection<string> _detalTypesCollection;
        private ObservableCollection<HorizontalAlignment> _horizontalAlignments;
        private ObservableCollection<VerticalAlignment> _verticalAlignments;

        private ICommand _editPathForUpdateCommand;
        private ICommand _checkedAvailableFolderCommand;
        private ICommand _standartSettingsCommand;
        private ICommand _selectClosedControlCommand;

        #region Public variables

        /// <summary>
        /// Временный пин-код для последующего сохранений
        /// </summary>
        public static string TempPinCode { get; private set; }
        /// <summary>
        /// Временный пусть до папки с обновлениями
        /// </summary>
        public static string TempUpdatePath { get; private set; }

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

        /// <summary>
        /// Коллекция видов деталей
        /// </summary>
        //public ObservableCollection<ForRobot.Models.Detals.DetalType> DetalTypesCollection { get; } = new ObservableCollection<ForRobot.Models.Detals.DetalType>(ForRobot.Models.Detals.DetalType.All);
        public ObservableCollection<string> DetalTypesCollection 
        {
            get
            {
                if(_detalTypesCollection == null)
                {
                    _detalTypesCollection = new ObservableCollection<string>(ForRobot.Libr.EnumExtensions.GetDescriptions(typeof(DetalType)));
                    _detalTypesCollection.Remove(ForRobot.Libr.EnumExtensions.GetDescription(ForRobot.Models.Detals.DetalType.All));
                }
                return _detalTypesCollection;
            }
        }

        public ObservableCollection<HorizontalAlignment> HorizontalAlignments { get => _horizontalAlignments 
                ?? (_horizontalAlignments = new ObservableCollection<HorizontalAlignment>(Enum.GetValues(typeof(HorizontalAlignment)).Cast<HorizontalAlignment>())); }
        public ObservableCollection<VerticalAlignment> VerticalAlignments { get => _verticalAlignments 
                ?? (_verticalAlignments = new ObservableCollection<VerticalAlignment>(Enum.GetValues(typeof(VerticalAlignment)).Cast<VerticalAlignment>())); }

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
                        TempUpdatePath = fbd.SelectedPath;
                    }
                }
            }));
        }

        public ICommand EditPinCodeCommand { get; } = new RelayCommand(_ => 
        {
            if (!ForRobot.Libr.AppWindowManager.PinCode(TempPinCode))
                return;

            string answer = ForRobot.Libr.AppWindowManager.Input("Введите новый пин-код");

            if (string.IsNullOrEmpty(answer))
                return;

            TempPinCode = ForRobot.Libr.Cryptography.Hashing.Sha256(answer);
        });

        ///// <summary>
        ///// Комманда изменения checkBox отображающихся папок
        ///// </summary>
        //public ICommand CheckBoxAvailableFolderCommand

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
        public ICommand SaveSettingsCommand { get; } = new RelayCommand(_ => 
        {
            ForRobot.Properties.Settings.Default.PinCode = TempPinCode;
            ForRobot.Properties.Settings.Default.UpdatePath = TempUpdatePath;
            ForRobot.Properties.Settings.Default.Save();
        });

        public class AttemptSelectedCommand : RelayCommand
        {
            private readonly Func<object, bool> _shouldBlock;

            public AttemptSelectedCommand(Action<object> execute,
                                         Func<object, bool> canExecute = null,
                                         Func<object, bool> shouldBlock = null) : base(execute, canExecute ?? (_ => true))
            {
                _shouldBlock = shouldBlock ?? (_ => false);
            }

            public override void Execute(object parameter)
            {
                if (this.ShouldBlock(parameter))
                    return;

                base.Execute(parameter);
            }

            public bool ShouldBlock(object parameter) => _shouldBlock(parameter);
        }

        /// <summary>
        /// Выбор закрытого элемента управления
        /// </summary>
        public ICommand SelectClosedControlCommand
        {
            get => this._selectClosedControlCommand ?? (this._selectClosedControlCommand = new AttemptSelectedCommand(
            execute: obj =>
            {
                if (obj == null)
                    throw new ArgumentNullException(nameof(obj));

                if (obj is ForRobot.Libr.AttachedProperties.AttemptSelectedEventArgs args)
                {
                    args.Cancel = false;
                    var control = args.Source as System.Windows.Controls.Control;
                    switch (control)
                    {
                        case TreeView tree:
                            break;

                        case TreeViewItem treeViewItem:
                            //treeViewItem.IsExpanded = false;
                            //treeViewItem.IsSelected = false;

                            var childrenItems = ForRobot.Libr.DependencyObjectExtensions.FindVisualChildren<TreeViewItem>(control);
                            childrenItems?.First().Focus();
                            break;

                        case CheckBox checkBox:
                            checkBox.IsChecked = !checkBox.IsChecked;
                            break;

                        case TextBox textBox:
                            textBox.Focus();
                            //textBox = control as TextBox;
                            //if (textBox == null) return;

                            //var parent = textBox.Parent as UIElement;
                            //if (parent != null && parent.Focusable)
                            //{
                            //    parent.Focus();
                            //}
                            //else
                            //{
                            //    var page = ForRobot.Libr.DependencyObjectExtensions.FindParent<Window>(textBox);
                            //    if (page != null)
                            //    {
                            //        page.Focus();
                            //    }
                            //}
                            //Keyboard.ClearFocus();
                            break;

                        default:
                            if (control == null) return;

                            // Сброс фокуса для всех областей фокуса
                            var focusScope = FocusManager.GetFocusScope(control);
                            FocusManager.SetFocusedElement(focusScope, null);

                            // Дополнительно: поиск и сброс фокуса во всех дочерних элементах
                            var children = ForRobot.Libr.DependencyObjectExtensions.FindVisualChildren<UIElement>(control);
                            foreach (var child in children)
                            {
                                if (child.IsKeyboardFocused)
                                {
                                    Keyboard.ClearFocus();
                                    break;
                                }
                            }
                            break;
                    }
                }
            },
            shouldBlock: _ => !ForRobot.Libr.AppWindowManager.PinCode(TempPinCode)));
        }

        //{
        //    get => this._selectClosedControlCommand ?? (this._selectClosedControlCommand = new RelayCommand(obj =>
        //    {
        //        if (obj is ForRobot.Libr.AttachedProperties.AttemptSelectedEventArgs args)
        //        {
        //            if (!ForRobot.Libr.AppWindowManager.PinCode(ForRobot.Properties.Settings.Default.PinCode))
        //            {
        //                args.Cancel = true;
        //            }
        //        }

        //        //if (!ForRobot.Libr.AppWindowManager.PinCode(ForRobot.Properties.Settings.Default.PinCode))
        //        //    return;

        //        //ForRobot.Libr.ControlExtensions.FocusCancel(((System.Windows.RoutedEventArgs)obj).Source as System.Windows.Controls.Control);

        //        //switch (obj)
        //        //{
        //        //    case AvalonDock.Layout.LayoutAnchorable layoutContent:
        //        //        layoutContent.IsSelected = false;
        //        //        layoutContent.IsActive = false;
        //        //        layoutContent.Hide();
        //        //        break;


        //        //    default:
        //        //        if (obj == null) return;

        //        //        var control = obj as System.Windows.Controls.Control;

        //        //        // Сброс фокуса для всех областей фокуса
        //        //        var focusScope = FocusManager.GetFocusScope(control);
        //        //        FocusManager.SetFocusedElement(focusScope, null);

        //        //        // Дополнительно: поиск и сброс фокуса во всех дочерних элементах
        //        //        var children = ForRobot.Libr.DependencyObjectExtensions.FindVisualChildren<UIElement>(control);
        //        //        foreach (var child in children)
        //        //        {
        //        //            if (child.IsKeyboardFocused)
        //        //            {
        //        //                Keyboard.ClearFocus();
        //        //                break;
        //        //            }
        //        //        }
        //        //        break;
        //        //}
        //    }));
        //}

        #endregion Commands

        #endregion Public variables

        public PropertiesWindowViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            TempPinCode = ForRobot.Properties.Settings.Default.PinCode;
            TempUpdatePath = ForRobot.Properties.Settings.Default.UpdatePath;
            this.Settings = App.Current.Settings.Clone() as Settings;
        }
    }
}
