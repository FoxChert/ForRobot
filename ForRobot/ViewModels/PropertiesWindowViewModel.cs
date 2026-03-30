using AvalonDock.Layout;
using ForRobot.Libr;
using ForRobot.Models.Detals;
using ForRobot.Models.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForRobot.ViewModels
{
    public class PropertiesWindowViewModel : BaseClass
    {
        private Settings _settings;

        private MutableKeyValuePair<DetalType, string> _selectedTumpleProgramName;
        private MutableKeyValuePair<DetalType, string> _selectedTumpleScriptsName;

        private List<LayoutAnchorable> _anchorablesCollection;
        private ObservableCollection<string> _detalTypesCollection;
        private ObservableCollection<string> _scriptsCollection;
        private ObservableCollection<HorizontalAlignment> _horizontalAlignments;
        private ObservableCollection<VerticalAlignment> _verticalAlignments;

        private ICommand _editPathForUpdateCommand;
        private ICommand _checkedAvailableFolderCommand;
        private ICommand _standartSettingsCommand;
        private ICommand _selectClosedControlCommand;
        private ICommand _deleteAvalonConfigFileCommand;
        private ICommand _saveSettingsCommand;

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
        /// Выбранный кортеж представляющий тип детали и имя итоговой программы
        /// </summary>
        public MutableKeyValuePair<DetalType, string> SelectedTumpleProgramName
        {
            get => _selectedTumpleProgramName ?? (_selectedTumpleProgramName = this.Settings.DetalsProgramNames.First()); 
            set => Set(ref _selectedTumpleProgramName, value);
        }

        /// <summary>
        /// Выбранный кортеж представляющий тип детали и имя скрипта-генератора
        /// </summary>
        public MutableKeyValuePair<DetalType, string> SelectedTumpleScriptsName
        {
            get => _selectedTumpleScriptsName ?? (_selectedTumpleScriptsName = this.Settings.DetalsScriptNames.First());
            set => Set(ref _selectedTumpleScriptsName, value);
        }

        /// <summary>
        /// Коллекция панелей макета интерфейса
        /// </summary>
        public List<LayoutAnchorable> AnchorablesCollection
        {
            get
            {
                if(_anchorablesCollection == null)
                {
                    var dockingManager = (App.Current.MainWindow as ForRobot.Views.Windows.MainWindow).DockingManeger;
                    _anchorablesCollection = dockingManager.Layout.Descendents().OfType<LayoutAnchorable>().ToList();
                }
                return _anchorablesCollection;
            }
        }

        /// <summary>
        /// Коллекция видов деталей
        /// </summary>
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

        /// <summary>
        /// Коллекция файлов в папке 'Scripts'
        /// </summary>
        public ObservableCollection<string> ScriptsCollection
        {
            get
            {
                if (this._scriptsCollection == null)
                {
                    this._scriptsCollection = new ObservableCollection<string>(GetScripts());
                    //this._scriptsCollection.CollectionChanged += HandleCollectionChanged;
                }
                return this._scriptsCollection;
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

        /// <summary>
        /// Удаление изменений интерфейса, удалеием AvalonDock.config файла
        /// </summary>
        public ICommand DeleteAvalonConfigFileCommand { get => _deleteAvalonConfigFileCommand ?? (_deleteAvalonConfigFileCommand = new RelayCommand(_ =>
        {
            if (MessageBox.Show("Для удаления изменений необходим перезапуск!\n\nПерезапустить приложение?",
                               "Предупреждение",
                               MessageBoxButton.OKCancel,
                               MessageBoxImage.Warning,
                               MessageBoxResult.Cancel,
                               MessageBoxOptions.DefaultDesktopOnly) != MessageBoxResult.OK)
                return;

            if (MessageBox.Show("Не сохраненные настроки будут сброшены.\n\nСохранить текущие настройки?") == MessageBoxResult.OK)
                SaveSettings();

            System.Diagnostics.Process process = new System.Diagnostics.Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = @".\",
                    CreateNoWindow = true,
                    FileName = "cmd.exe",
                    Arguments = $"/K taskkill /im {Application.ResourceAssembly.GetName().Name}.exe /f& del {App.Current.AvalonConfigPath}& START \"\" \"{Application.ResourceAssembly.Location}\"",
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
                }
            };
            new System.Threading.Thread(() => process.Start()).Start();
        })); } 

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
        public ICommand SaveSettingsCommand 
        { 
            get => _saveSettingsCommand ?? (_saveSettingsCommand = new RelayCommand(_ => SaveSettings())); 
        }

        /// <summary>
        /// Выбор закрытого элемента управления
        /// </summary>
        public ICommand SelectClosedControlCommand
        {
            get => this._selectClosedControlCommand ?? (this._selectClosedControlCommand = new ForRobot.Libr.AttachedProperties.AttemptSelectedCommand(
            execute: obj => 
            {
                if (obj == null)
                    throw new ArgumentNullException(nameof(obj));

                if (!(obj is ForRobot.Libr.AttachedProperties.AttemptSelectedEventArgs args))
                    return;

                args.Cancel = false;

                switch (args.Source)
                {
                    case AvalonDock.Layout.LayoutAnchorable layoutContent:
                        layoutContent.IsSelected = false;
                        layoutContent.IsActive = false;
                        layoutContent.Hide();
                        break;
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

        /// <summary>
        /// Возврат содержимого папки Scripts
        /// </summary>
        /// <returns></returns>
        private List<string> GetScripts()
        {
            string path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Scripts");

            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException("Не найдена папка Scripts!");

            List<string> fileList = new List<string>();
            foreach (var file in Directory.GetFiles(path))
                fileList.Add(file);

            return new List<string>(fileList);
        }

        private void SaveSettings()
        {
            ForRobot.Properties.Settings.Default.PinCode = TempPinCode;
            ForRobot.Properties.Settings.Default.UpdatePath = TempUpdatePath;
            ForRobot.Properties.Settings.Default.Save();
        }
    }
}
