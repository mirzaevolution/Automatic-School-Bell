using System;
using System.ServiceProcess;
using CoreLib.Models;

namespace AutomaticSchoolBell.GUI.WindowsService
{
    public enum ServiceStatus
    {
        NotRegistered = 0,
        Running = 1,
        Paused = 2,
        Stopped = 3
    }
    public class Controller
    {
        public const string SERVICE_NAME = "BellRunnerService";
        public static DataResult<ServiceStatus> CheckServiceStatus()
        {
            var result = CheckServiceRegistration();
            if (result.Success)
            {
                bool success = true;
                string error = "";
                ServiceStatus status = ServiceStatus.Stopped;
                try
                {
                    using (ServiceController controller = new ServiceController(SERVICE_NAME))
                    {
                        switch (controller.Status)
                        {
                            case ServiceControllerStatus.Running:
                                status = ServiceStatus.Running;
                                break;
                            case ServiceControllerStatus.Paused:
                                status = ServiceStatus.Paused;
                                break;
                        }
                    }
                }
                catch(Exception ex)
                {
                    success = false;
                    error = $"Service Error: {ex.Message}";
                }
                return new DataResult<ServiceStatus>
                {
                    Data = status,
                    Status = new MainResult
                    {
                        Success = success,
                        ErrorMessage = error
                    }
                };
            }
            return new DataResult<ServiceStatus>
            {
                Data = ServiceStatus.NotRegistered,
                Status = result
            };
        }
        private static MainResult CheckServiceRegistration()
        {
            bool success = true;
            string error = "";
            try
            {
                using (ServiceController controller = new ServiceController(SERVICE_NAME)) { }
            }
            catch (Exception ex)
            {
                success = false;
                error = $"Service Error: {ex.Message}";
            }
            return new MainResult
            {
                Success = success,
                ErrorMessage = error
            };
        }
        public static MainResult StartService()
        {
            bool success = true;
            string error = "";
            try
            {
                using (ServiceController controller = new ServiceController(SERVICE_NAME))
                {
                    controller.Start();
                    controller.WaitForStatus(ServiceControllerStatus.Running);
                }
            }
            catch(Exception ex)
            {
                success = false;
                error = $"Service Error: {ex.Message}";
            }
            return new MainResult
            {
                Success = success,
                ErrorMessage = error
            };
        }
        public static MainResult PauseService()
        {
            bool success = true;
            string error = "";
            try
            {
                using (ServiceController controller = new ServiceController(SERVICE_NAME))
                {
                    controller.Pause();
                    controller.WaitForStatus(ServiceControllerStatus.Paused);
                }
            }
            catch (Exception ex)
            {
                success = false;
                error = $"Service Error: {ex.Message}";
            }
            return new MainResult
            {
                Success = success,
                ErrorMessage = error
            };
        }
        public static MainResult ContinueService()
        {
            bool success = true;
            string error = "";
            try
            {
                using (ServiceController controller = new ServiceController(SERVICE_NAME))
                {
                    controller.Continue();
                    controller.WaitForStatus(ServiceControllerStatus.Running);
                }
            }
            catch (Exception ex)
            {
                success = false;
                error = $"Service Error: {ex.Message}";
            }
            return new MainResult
            {
                Success = success,
                ErrorMessage = error
            };
        }
        public static MainResult StopService()
        {
            bool success = true;
            string error = "";
            try
            {
                using (ServiceController controller = new ServiceController(SERVICE_NAME))
                {
                    controller.Stop();
                    controller.WaitForStatus(ServiceControllerStatus.Stopped);
                }
            }
            catch (Exception ex)
            {
                success = false;
                error = $"Service Error: {ex.Message}";
            }
            return new MainResult
            {
                Success = success,
                ErrorMessage = error
            };
        }
        public static MainResult RefreshService()
        {
            var statusResult = CheckServiceStatus();
            if (statusResult.Status.Success)
            {
                ServiceStatus status = statusResult.Data;
                switch(status)
                {
                    case ServiceStatus.Paused:
                        return ContinueService();
                    case ServiceStatus.Stopped:
                        return StartService();
                    case ServiceStatus.Running:
                        var pauseResult = PauseService();
                        if(pauseResult.Success)
                        {
                            var continueResult = ContinueService();
                            return continueResult;
                        }
                        return pauseResult;
                        
                }
            }
            return statusResult.Status;
        }
    }
}
