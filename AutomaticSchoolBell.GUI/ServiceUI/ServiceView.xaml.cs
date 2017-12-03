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
using AutomaticSchoolBell.GUI.WindowsService;
namespace AutomaticSchoolBell.GUI.ServiceUI
{
    public partial class ServiceView : UserControl
    {
        private Dictionary<ServiceStatus, Uri> ImageUris = new Dictionary<ServiceStatus, Uri>
        {
            { ServiceStatus.Running, new Uri("/Content/ico-status-running.png",UriKind.Relative) },
            { ServiceStatus.Paused, new Uri("/Content/ico-status-paused.png",UriKind.Relative) },
            { ServiceStatus.Stopped, new Uri("/Content/ico-status-stopped.png",UriKind.Relative) },
            { ServiceStatus.NotRegistered, new Uri("/Content/ico-status-stopped.png",UriKind.Relative) }
        };
        enum TaskType
        {
            Start,
            Stop,
            Pause,
            Continue
        }
        public ServiceView()
        {
            InitializeComponent();
            TextServiceName.Text = $"Service Name: {Controller.SERVICE_NAME}";
            Load();
        }
        private async void Load()
        {
            bool success = true;
            string message = "";
            ServiceStatus status = ServiceStatus.NotRegistered;
            ProgressLoading.IsActive = true;
            await Task.Run(() =>
            {
                var result = Controller.CheckServiceStatus();
                if (result.Status.Success)
                    status = result.Data;
                else
                {
                    success = false;
                    message = result.Status.ErrorMessage;
                }
            });
           
            if(success)
            {
                BitmapImage image = new BitmapImage(ImageUris[status]);
                ImageServiceStatus.Source = image;
                TextStatus.Text = status.ToString();
                switch(status)
                {
                    case ServiceStatus.NotRegistered:
                        ButtonStart.IsEnabled = false;
                        ButtonPause.IsEnabled = false;
                        ButtonContinue.IsEnabled = false;
                        ButtonStop.IsEnabled = false;
                        break;
                    case ServiceStatus.Stopped:
                        ButtonStart.IsEnabled = true;
                        ButtonPause.IsEnabled = false;
                        ButtonContinue.IsEnabled = false;
                        ButtonStop.IsEnabled = false;
                        break;
                    case ServiceStatus.Paused:
                        ButtonStart.IsEnabled = false;
                        ButtonPause.IsEnabled = false;
                        ButtonContinue.IsEnabled = true;
                        ButtonStop.IsEnabled = false;
                        break;
                    case ServiceStatus.Running:
                        ButtonStart.IsEnabled = false;
                        ButtonPause.IsEnabled = true;
                        ButtonContinue.IsEnabled = false;
                        ButtonStop.IsEnabled = true;
                        break;
                }
            }
            else
            {
                BitmapImage image = new BitmapImage(ImageUris[ServiceStatus.NotRegistered]);
                ImageServiceStatus.Source = image;
                TextStatus.Text = "Unknown";
                ButtonStart.IsEnabled = false;
                ButtonPause.IsEnabled = false;
                ButtonContinue.IsEnabled = false;
                ButtonStop.IsEnabled = false;
            }
            ProgressLoading.IsActive = false;
        }

        private async void StartTask(TaskType taskType)
        {
            bool success = true;
            string message = "";
            ProgressLoading.IsActive = true;
            ServiceStatus status = ServiceStatus.NotRegistered;
            switch(taskType)
            {
                case TaskType.Start:
                    await Task.Run(() =>
                    {
                        var result = Controller.StartService();
                        if (!result.Success)
                        {
                            success = false;
                            message = result.ErrorMessage;
                        }

                    });
                    if (success)
                    {
                        status = ServiceStatus.Running;
                    }
                    else
                    {
                        status = ServiceStatus.NotRegistered;
                    }
                    break;
                case TaskType.Stop:
                    await Task.Run(() =>
                    {
                        var result = Controller.StopService();
                        if (!result.Success)
                        {
                            success = false;
                            message = result.ErrorMessage;
                        }

                    });
                    if (success)
                    {
                        status = ServiceStatus.Stopped;
                    }
                    else
                    {
                        status = ServiceStatus.NotRegistered;
                    }
                    break;
                case TaskType.Continue:
                    await Task.Run(() =>
                    {
                        var result = Controller.ContinueService();
                        if (!result.Success)
                        {
                            success = false;
                            message = result.ErrorMessage;
                        }

                    });
                    if (success)
                    {
                        status = ServiceStatus.Running;
                    }
                    else
                    {
                        status = ServiceStatus.NotRegistered;
                    }
                    break;
                case TaskType.Pause:
                    await Task.Run(() =>
                    {
                        var result = Controller.PauseService();
                        if (!result.Success)
                        {
                            success = false;
                            message = result.ErrorMessage;
                        }

                    });
                    if (success)
                    {
                        status = ServiceStatus.Paused;
                    }
                    else
                    {
                        status = ServiceStatus.NotRegistered;
                    }
                    break;
            }
            
            if (success)
            {
                BitmapImage image = new BitmapImage(ImageUris[status]);
                ImageServiceStatus.Source = image;
                TextStatus.Text = status.ToString();
                switch (status)
                {
                    case ServiceStatus.NotRegistered:
                        ButtonStart.IsEnabled = false;
                        ButtonPause.IsEnabled = false;
                        ButtonContinue.IsEnabled = false;
                        ButtonStop.IsEnabled = false;
                        break;
                    case ServiceStatus.Stopped:
                        ButtonStart.IsEnabled = true;
                        ButtonPause.IsEnabled = false;
                        ButtonContinue.IsEnabled = false;
                        ButtonStop.IsEnabled = false;
                        break;
                    case ServiceStatus.Paused:
                        ButtonStart.IsEnabled = false;
                        ButtonPause.IsEnabled = false;
                        ButtonContinue.IsEnabled = true;
                        ButtonStop.IsEnabled = false;
                        break;
                    case ServiceStatus.Running:
                        ButtonStart.IsEnabled = false;
                        ButtonPause.IsEnabled = true;
                        ButtonContinue.IsEnabled = false;
                        ButtonStop.IsEnabled = true;
                        break;
                }
            }
            else
            {
                BitmapImage image = new BitmapImage(ImageUris[ServiceStatus.NotRegistered]);
                ImageServiceStatus.Source = image;
                TextStatus.Text = "Unknown";
                ButtonStart.IsEnabled = false;
                ButtonPause.IsEnabled = false;
                ButtonContinue.IsEnabled = false;
                ButtonStop.IsEnabled = false;

            }
            ProgressLoading.IsActive = false;

        }
        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            StartTask(TaskType.Start);
        }
        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            StartTask(TaskType.Pause);
        }
        private void ButtonContinue_Click(object sender, RoutedEventArgs e)
        {
            StartTask(TaskType.Continue);
        }
        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            StartTask(TaskType.Stop);
        }
        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            Load();
        }
    }
}
