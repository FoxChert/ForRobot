using ForRobot.Libr.Behavior;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ForRobot.Libr.AttachedProperties
{
    public class TreeViewAttachedProperties
    {
        #region SelectedItem Attached Property

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.RegisterAttached("SelectedItem",
                                                                                                             typeof(object),
                                                                                                             typeof(TreeViewAttachedProperties),
                                                                                                             new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemChanged));

        public static object GetSelectedItem(DependencyObject obj) => obj.GetValue(SelectedItemProperty);

        public static void SetSelectedItem(DependencyObject obj, object value) => obj.SetValue(SelectedItemProperty, value);

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is TreeView treeView)
            {
                treeView.SelectedItemChanged -= OnTreeViewSelectedItemChanged;

                try
                {
                    UpdateTreeViewSelection(treeView, e.NewValue);
                }
                finally
                {
                    treeView.SelectedItemChanged += OnTreeViewSelectedItemChanged;
                }
            }
        }

        #endregion SelectedItem Attached Property

        #region Event Handlers

        /// <summary>
        /// Обработчик события изменения выбранного элемента в TreeView
        /// </summary>
        private static void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (sender is TreeView treeView)
            {
                treeView.SelectedItemChanged -= OnTreeViewSelectedItemChanged;

                try
                {
                    SetSelectedItem(treeView, e.NewValue);
                }
                finally
                {
                    treeView.SelectedItemChanged += OnTreeViewSelectedItemChanged;
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Обновляет выбор в TreeView в соответствии со значением модели
        /// </summary>
        private static void UpdateTreeViewSelection(TreeView treeView, object selectedItem)
        {
            treeView.SelectedItemChanged -= OnTreeViewSelectedItemChanged;

            try
            {
                //if (selectedItem == null)
                //{
                //    if (treeView.SelectedItem != null)
                //        treeView.SelectedItem = null;
                //}
                //else
                //{
                //    treeView.SelectedItem = selectedItem;
                //}
            }
            finally
            {
                treeView.SelectedItemChanged += OnTreeViewSelectedItemChanged;
            }
        }

        #endregion
    }
}
