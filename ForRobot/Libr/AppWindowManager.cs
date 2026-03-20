using System;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

using ForRobot.Views.Windows;

namespace ForRobot.Libr
{
    /// <summary>
    /// Класс-менеджер для централизованного управления окнами приложения
    /// </summary>
    public class AppWindowManager
    {
        private static AppWindowManager _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Словарь активных окон
        /// </summary>
        private readonly ConcurrentDictionary<string, Window> _activeWindows = new ConcurrentDictionary<string, Window>();

        public static AppWindowManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new AppWindowManager();
                    }
                }
                return _instance;
            }
        }

        #region Private functions

        private void FocusedWindow(Window window)
        {
            if (window.WindowState == WindowState.Minimized)
                window.WindowState = WindowState.Normal;

            window.Topmost = true;
            window.Topmost = false;
            window.Activate();
            window.Focus();
        }
        
        private bool RemoveWindow(string key, out Window window)
        {
            lock (_lock)
            {
                return this._activeWindows.TryRemove(key, out window);
            }
        }

        /// <summary>
        /// Получение существующего окна или открытие нового
        /// </summary>
        /// <param name="key">Ключ (наименование типа)</param>
        /// <returns>Окно приложения</returns>
        private Window GetOrAddWindow(string key)
        {
            lock (_lock)
            {
                if (!this._activeWindows.TryGetValue(key, out var window))
                {
                    window = CreateWindow(key);

                    if (window != null)
                    {
                        window.Closed += (s, e) =>
                        {
                            RemoveWindow(s.GetType().Name, out var w);
                            if (w != null && w.Owner != null)
                                FocusedWindow(w.Owner);
                        };
                        this._activeWindows[key] = window;
                    }
                }
                else
                    this.FocusedWindow(window);
                return window;
            }
        }

        private Window CreateWindow(string key)
        {
            Window window = null;
            switch (key)
            {
                case nameof(InputWindow):
                    window = new InputWindow();
                    break;

                case nameof(SelectWindow):
                    window = new SelectWindow();
                    break;

                case nameof(SelectorWindow):
                    window = new SelectorWindow();
                    break;

                case nameof(MainWindow):
                    window = new MainWindow();
                    break;

                case nameof(CreateWindow):
                    window = new CreateWindow();
                    break;

                case nameof(PropertiesWindow):
                    window = new PropertiesWindow();
                    break;
            }
            return window;
        }

        #endregion Private functions

        #region Public functions

        /// <summary>
        /// Отображение окна ввода пин-кода и его хэширование
        /// </summary>
        /// <param name="code">Хэш пин-кода</param>
        /// <returns>Совпабает ли хэш введенного пин-кода с ожидаемым <paramref name="code"/></returns>
        public bool PinCodeInputWindowShow(string code)
        {
            string pin = InputWindowShow("Введите пин-код");
            return !string.IsNullOrEmpty(pin) && ForRobot.Libr.Cryptography.Hashing.Sha256(pin) == code;
        }

        /// <summary>
        /// Отображение окна ввода текста
        /// </summary>
        /// <param name="sInputBoxText">Текст подсказки</param>
        /// <returns>Введённый текст</returns>
        public string InputWindowShow(string sInputBoxText)
        {
            string answer = string.Empty;
            using (InputWindow inputWindow = GetOrAddWindow(nameof(InputWindow)) as InputWindow)
            {
                if (inputWindow == null)
                    return string.Empty;

                inputWindow.Question.Content = sInputBoxText;
                if (inputWindow.ShowDialog() == true)
                    answer = inputWindow.Answer;
            }
            return answer;
        }

        /// <summary>
        /// Главное окно приложения
        /// </summary>
        /// <returns></returns>
        public Window AppMainWindowShow() => GetOrAddWindow(nameof(MainWindow));

        /// <summary>
        /// Отображение окна одиночного выбора
        /// </summary>
        /// <param name="itemsSource">Источник данных для выбора</param>
        /// <returns>Выбранный элемент</returns>
        public object SelectWindowShow(IEnumerable itemsSource)
        {
            object selectItem = null;
            using (SelectWindow selectWindow = GetOrAddWindow(nameof(SelectWindow)) as SelectWindow)
            {
                if (selectWindow == null)
                    return null;

                selectWindow.ItemsSource = itemsSource;
                if (selectWindow.ShowDialog() == true)
                    selectItem = selectWindow.SelectedItem;
            }
            return selectItem;
        }

        /// <summary>
        /// Отображение окна множественного выбора
        /// </summary>
        /// <param name="itemsSource">Источник данных для выбора</param>
        /// <param name="selectedItems">Выбранные элементы</param>
        /// <returns></returns>
        public IEnumerable SelectorWindowShow(IEnumerable itemsSource, IEnumerable selectedItems = null)
        {
            List<object> result = new List<object>();
            using (SelectorWindow selectorWindow = GetOrAddWindow(nameof(SelectorWindow)) as SelectorWindow)
            {
                if (selectorWindow == null)
                    return null;

                selectorWindow.ItemsSource = itemsSource;
                selectorWindow.SelectedItems = selectedItems;

                if (selectorWindow.ShowDialog() == true)
                    result = selectorWindow.SelectedItems as List<object>;
            }
            return result;
        }

        /// <summary>
        /// Отображение окна создания файла
        /// </summary>
        /// <param name="detalType"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public ForRobot.Models.File3D.IFile3D CreateWindowShow(ForRobot.Models.Detals.DetalType detalType, string path = null) => null;

        /// <summary>
        /// Отображение окна настроек
        /// </summary>
        /// <param name="settings">Настройки приложения</param>
        /// <returns>Сохранены ли изменения</returns>
        public bool SettingsWindowShow(out ForRobot.Models.Settings.Settings settings)
        {
            bool result = false;
            settings = null;
            using (PropertiesWindow propertiesWindow = GetOrAddWindow(nameof(PropertiesWindow)) as PropertiesWindow)
            {
                if (propertiesWindow == null)
                    return false;

                propertiesWindow.Owner = App.Current.MainWindow;
                //propertiesWindow.Settings = App.Current.Settings.Clone() as ForRobot.Models.Settings.Settings;

                if (propertiesWindow.ShowDialog() == true)
                {
                    result = true;
                    //settings = propertiesWindow.Settings;
                }
            }
            return result;
        }

        #region Statics functions

        public static bool PinCode(string code) => Instance.PinCodeInputWindowShow(code);
        public static string Input(string sInputBoxText) => Instance.InputWindowShow(sInputBoxText);
        public static Window Main() => Instance.AppMainWindowShow();
        public static object Select(IEnumerable itemsSource) => Instance.SelectWindowShow(itemsSource);
        public static IEnumerable Selector(IEnumerable itemsSource, IEnumerable selectedItems = null) => Instance.SelectorWindowShow(itemsSource, selectedItems);
        public static ForRobot.Models.File3D.IFile3D Create(ForRobot.Models.Detals.DetalType detalType, string path = null) => Instance.CreateWindowShow(detalType, path);
        public static bool Settings(out ForRobot.Models.Settings.Settings settings) => Instance.SettingsWindowShow(out settings);

        #endregion Statics functions

        #endregion Public functions
    }
}
