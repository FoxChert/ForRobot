using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ForRobot.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для ToolTipContainer.xaml
    /// </summary>
    public partial class ToolTipContainer : UserControl
    {
        /// <summary>
        /// Свойство для хранения последнего открытого ToolTip
        /// </summary>
        public static readonly DependencyProperty LastToolTipProperty = DependencyProperty.Register(nameof(LastToolTip),
                                                                                                    typeof(string),
                                                                                                    typeof(ToolTipContainer),
                                                                                                    new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string LastToolTip
        {
            get => (string)GetValue(LastToolTipProperty);
            set => SetValue(LastToolTipProperty, value);
        }

        public ToolTipContainer()
        {
            InitializeComponent();
        }
    }
}
