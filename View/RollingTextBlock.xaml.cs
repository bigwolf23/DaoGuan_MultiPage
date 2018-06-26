using System;
using System.Collections;
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
using System.Windows.Threading;

namespace RenJiCaoZuo
{
    /// <summary>
    /// RoilingTextBlock.xaml 的交互逻辑
    /// </summary>
    public partial class RoilingTextBlock : UserControl
    {
        private bool canRoll = false;
        private double rollingInterval = 16;//每一步的偏移量
        private double offset = 18;//最大的偏移量
        private TextBlock currentTextBlock = null;
        //private DispatcherTimer currentTimer = null;
        private System.Timers.Timer currentTimer;
        public RoilingTextBlock()
        {
            InitializeComponent();
            Loaded += RoilingTextBlock_Loaded;
        }
//         public static readonly DependencyProperty ItemsProperty = 
//             DependencyProperty.Register("Text", typeof(IEnumerable), typeof(RoilingTextBlock));
//         public IEnumerable Items
//         {
//             get { return (IEnumerable)GetValue(ItemsProperty); }
//             set { SetValue(ItemsProperty, value); }
//         }


        void RoilingTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.currentTextBlock != null)
            {
                canRoll = this.currentTextBlock.ActualHeight > this.ActualHeight;
            }

            currentTimer = new System.Timers.Timer();
            currentTimer.Interval = 1000;
            currentTimer.Elapsed += currentTimer_Tick;
            currentTimer.Start();
        }

        public override void OnApplyTemplate()
        {
            try
            {
                base.OnApplyTemplate();
                currentTextBlock = this.GetTemplateChild("textBlock") as TextBlock;
            }
            catch (Exception ex)
            {

            }

        }

        void currentTimer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                string strtemp = currentTextBlock.Text;
                if (this.currentTextBlock != null && canRoll)
                {
                    if (Math.Abs(Top) <= this.currentTextBlock.ActualHeight - offset)
                    {
                        Top -= rollingInterval;
                    }
                    else
                    {
                        Top = this.ActualHeight;
                    }

                }
            }), null);

            
        }

        #region Dependency Properties
        public static DependencyProperty TextProperty =
           DependencyProperty.Register("Text", typeof(string), typeof(RoilingTextBlock),
           new PropertyMetadata(""));

        public static DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(RoilingTextBlock),
            new PropertyMetadata(14D));

        public static readonly DependencyProperty ForegroundProperty =
           DependencyProperty.Register("Foreground", typeof(Brush), typeof(RoilingTextBlock), new FrameworkPropertyMetadata(Brushes.Black));

        public static DependencyProperty LeftProperty =
           DependencyProperty.Register("Left", typeof(double), typeof(RoilingTextBlock), new PropertyMetadata(0D));

        public static DependencyProperty TopProperty =
           DependencyProperty.Register("Top", typeof(double), typeof(RoilingTextBlock), new PropertyMetadata(0D));

        #endregion

        #region Public Variables
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public double Left
        {
            get { return (double)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        public double Top
        {
            get { return (double)GetValue(TopProperty); }
            set { SetValue(TopProperty, value); }
        }
        #endregion
    }
}