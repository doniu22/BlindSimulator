using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Symulator_Rolety
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Boolean autoMode = new Boolean();
        private Boolean isOpenedBlind = new Boolean();
        private Boolean isFoldedBlind = new Boolean();
        private List<Rectangle> blindParts = new List<Rectangle>();
        private Storyboard closeBlind = new Storyboard();
        private Storyboard openBlind = new Storyboard();
        private Storyboard sensorsFlashing = new Storyboard();
        private Storyboard foldBlind = new Storyboard();
        private Storyboard unfoldBlind = new Storyboard();
        private DispatcherTimer openTimer = new DispatcherTimer();
        private DispatcherTimer closeTimer = new DispatcherTimer();
        private DispatcherTimer foldTimer = new DispatcherTimer();
        private DispatcherTimer unfoldTimer = new DispatcherTimer();
        private double openTimerCounter = 0.0;
        private double closeTimerCounter = 0.0;
        private double foldTimerCounter = 0.0;
        private double unfoldTimerCounter = 0.0;
        private double blindSliderValue = 0.0;
        private double timeToStartOpen = 5.0;
        private double timeToStartClose = 0.0;
        private double timeToStartFold = 0.0;
        private double timeToStartUnfold = 2.0;

        public MainWindow()
        {
            InitializeComponent();
            InitializeBlindPartsList();
            closeBlind = this.FindResource("CloseBlind") as Storyboard;
            openBlind = this.FindResource("OpenBlind") as Storyboard;
            foldBlind = this.FindResource("FoldBlind") as Storyboard;
            unfoldBlind = this.FindResource("UnfoldBlind") as Storyboard;
            sensorsFlashing = this.FindResource("SensorsFlashing") as Storyboard;
            sensorsFlashing.Begin();
            autoMode = false;
            isOpenedBlind = true;
            isFoldedBlind = false;

            openTimer.Tick += new EventHandler(openTimer_Tick);
            openTimer.Interval = TimeSpan.FromMilliseconds(100);
            closeTimer.Tick += new EventHandler(closeTimer_Tick);
            closeTimer.Interval = TimeSpan.FromMilliseconds(100);
            foldTimer.Tick += new EventHandler(foldTimer_Tick);
            foldTimer.Interval = TimeSpan.FromMilliseconds(100);
            unfoldTimer.Tick += new EventHandler(unfoldTimer_Tick);
            unfoldTimer.Interval = TimeSpan.FromMilliseconds(100);

        }


        private void InitializeBlindPartsList()
        {
            blindParts.Add(blaszka1);
            blindParts.Add(blaszka2);
            blindParts.Add(blaszka3);
            blindParts.Add(blaszka4);
            blindParts.Add(blaszka5);
            blindParts.Add(blaszka6);
            blindParts.Add(blaszka7);
            blindParts.Add(blaszka8);
            blindParts.Add(blaszka9);
        }

        private void openTimer_Tick(object sender, EventArgs e)
        {
            if (openTimerCounter < 5.0)
                openTimerCounter = openTimerCounter + 0.1;
        }

        private void closeTimer_Tick(object sender, EventArgs e)
        {
            if (closeTimerCounter < 5.0)
                closeTimerCounter = closeTimerCounter + 0.1;
        }

        private void foldTimer_Tick(object sender, EventArgs e)
        {
            if (foldTimerCounter < 2.0)
                foldTimerCounter = foldTimerCounter + 0.1;
            else
                foldTimer.Stop();
        }

        private void unfoldTimer_Tick(object sender, EventArgs e)
        {
            if (unfoldTimerCounter < 2.0)
                unfoldTimerCounter = unfoldTimerCounter + 0.1;
            else
            {
                blindSlider.IsEnabled = true;
                workModeBtn.IsEnabled = true;
                unfoldTimer.Stop();
            }
        }

        private void workModeBtn_Click(object sender, RoutedEventArgs e)
        {
                foldBlind.Stop(this);
                unfoldBlind.Stop(this);

                if (workModeBtn.Content.Equals("Auto Work"))
                {
                    // change work to auto mode
                    autoMode = true;
                    workModeBtn.Content = "Manual Work";
                    blindSlider.IsEnabled = false;
                    // check power light and end close or open blind
                    CheckBlindOpenLevel();

                }
                else
                {
                    // change work to manual mode
                    double pom = blindSliderValue;
                    openBlind.Stop(this);
                    closeBlind.Stop(this);
                    autoMode = false;
                    workModeBtn.Content = "Auto Work";
                    blindSlider.IsEnabled = true;
                    blindSliderValue = pom;
                    blindSlider.Value = blindSliderValue;
                    ChangeBlindProperty();
                }
            

        }

        private void ControlBlinds(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            blindSliderValue = e.NewValue;
            foldBlind.Stop(this);
            unfoldBlind.Stop(this);

            if (!autoMode)
                ChangeBlindProperty();         
        }

        private void ControlLamps(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double lightpower = e.NewValue;
            double czas2=0.0;
            byte newG = (byte)(243 - (e.NewValue * 36)/100);
            byte newB = (byte)(189 - (e.NewValue * 189)/100);
            
            lampa1.Fill = new SolidColorBrush(Color.FromRgb(255,newG,newB));
            lampa2.Fill = new SolidColorBrush(Color.FromRgb(255,newG,newB));


            if (lightpower > 50 && autoMode && isOpenedBlind)
            {
                isOpenedBlind = false;
                openTimer.Stop();
                czas2 = timeToStartOpen + openTimerCounter;
                timeToStartClose = 5.0 - czas2;
                if (timeToStartClose < 0)
                    timeToStartClose = 0.0;
                closeTimer.Start();
                closeTimerCounter = 0.0;
                closeBlind.Begin(this,true);
                closeBlind.Seek(this, TimeSpan.FromSeconds(timeToStartClose), TimeSeekOrigin.BeginTime);
                             
            }
            else if (lightpower<=50 && autoMode && !isOpenedBlind)
            {

                isOpenedBlind = true;
                closeTimer.Stop();
                czas2 = timeToStartClose + closeTimerCounter;
                timeToStartOpen = 5.0 - czas2;
                if (timeToStartOpen < 0)
                    timeToStartOpen = 0.0;
                openTimer.Start();
                openTimerCounter = 0.0;
                openBlind.Begin(this,true);
                openBlind.Seek(this,TimeSpan.FromSeconds(timeToStartOpen), TimeSeekOrigin.BeginTime);
                
            }

        }

        private void ChangeBlindProperty()
        {
            double left, right, bottom, top;

            for (int i = 0; i < blindParts.Count; i++)
            {
                top = 170 + (30 * i) - (blindSliderValue * 0.1); 
                bottom = blindParts[i].Margin.Bottom;
                left = blindParts[i].Margin.Left;
                right = blindParts[i].Margin.Right;
                blindParts[i].Height = 10 + blindSliderValue * 0.2;
                blindParts[i].Margin = new Thickness(left, top , right, bottom);
            }
        }

        private void CheckBlindOpenLevel()
        {
            
            if (lightSlider.Value < 50 && blindSlider.Value >= 0)
            {
                isOpenedBlind = true;
                timeToStartOpen = 5.0 - blindSliderValue * 0.05 ;
                if (timeToStartOpen < 0)
                    timeToStartOpen = 0.0;
                openTimer.Start();
                openTimerCounter = 0.0;
                openBlind.Begin(this, true);
                openBlind.Seek(this, TimeSpan.FromSeconds(timeToStartOpen), TimeSeekOrigin.BeginTime);

            }
            else if (lightSlider.Value >= 50 && blindSlider.Value >= 0)
            {

                isOpenedBlind = false;
                timeToStartClose = blindSliderValue * 0.05;
                if (timeToStartClose < 0)
                    timeToStartClose = 0.0;
                closeTimer.Start();
                closeTimerCounter = 0.0;
                closeBlind.Begin(this, true);
                closeBlind.Seek(this, TimeSpan.FromSeconds(timeToStartClose), TimeSeekOrigin.BeginTime);

            }


        }

        private void FoldBlindBtn_Click(object sender, RoutedEventArgs e)
        {
            double czas2;
            if (!autoMode && blindSliderValue == 0 && !isFoldedBlind)
            {
                blindSlider.IsEnabled = false;
                workModeBtn.IsEnabled = false;
                isFoldedBlind = true;

                unfoldTimer.Stop();
                czas2 = timeToStartUnfold + unfoldTimerCounter;
                timeToStartFold = 2.0 - czas2;
                if (timeToStartFold < 0)
                    timeToStartFold = 0.0;
                foldTimer.Start();
                foldTimerCounter = 0.0;
                foldBlind.Begin(this, true);
                foldBlind.Seek(this, TimeSpan.FromSeconds(timeToStartFold), TimeSeekOrigin.BeginTime);
            }
        }

        private void UnfoldBlindBtnClick(object sender, RoutedEventArgs e)
        {
            double czas2;

            if (!autoMode && blindSliderValue == 0 && isFoldedBlind)
            {
                isFoldedBlind = false;
                foldTimer.Stop();
                czas2 = timeToStartFold + foldTimerCounter;
                timeToStartUnfold = 2.0 - czas2;
                if (timeToStartUnfold < 0)
                    timeToStartUnfold = 0.0;
                unfoldTimer.Start();
                unfoldTimerCounter = 0.0;
                unfoldBlind.Begin(this, true);
                unfoldBlind.Seek(this, TimeSpan.FromSeconds(timeToStartUnfold), TimeSeekOrigin.BeginTime);
            }
        }
    }
}
