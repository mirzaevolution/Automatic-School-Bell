using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading.Tasks;
using CoreLib.DataAccess;
using CoreLib.Models;
using System.Timers;
namespace RunnerService
{
    public partial class RunnerServiceWin32 : ServiceBase
    {
        private Timer _timerDailySchedule;
        private Timer _timerEvent;
        private Timer _timerRepeatedSchedule;
        private EventLog _eventLog;
        private List<Schedule> _schedules;
        private List<Event> _events;
        private List<RepeatedSchedule> _repeatedSchedules;
        private bool _isTimerDailyScheduleRunning;
        private bool _isTimerEventRunning;
        private bool _isTimerRepeatedScheduleRunning;

        public RunnerServiceWin32()
        {
            InitializeComponent();
            LoadComponents();
        }
        protected override void OnStart(string[] args)
        {
            StartContinue();
        }
        protected override void OnPause()
        {
            PauseStop();
        }
        protected override void OnContinue()
        {
            StartContinue();
        }
        protected override void OnStop()
        {
            PauseStop();
        }


        private void LoadComponents()
        {
            try
            {
                
                _eventLog = new EventLog("Application")
                {
                    Source = "BellRunnerService"
                };

                _timerDailySchedule = new Timer
                {
                    Interval = 1000
                };
                _timerEvent = new Timer
                {
                    Interval = 1000
                };
                _timerRepeatedSchedule = new Timer
                {
                    Interval = 1000
                };
                _timerDailySchedule.Elapsed += DailyScheduleElapsed;
                _timerEvent.Elapsed += EventElapsed;
                _timerRepeatedSchedule.Elapsed += RepeatedScheduleElapsed;
            }
            catch(Exception ex)
            {
                _eventLog.WriteEntry($"Error while loading components.\n{ex.Message}", EventLogEntryType.Error);
            }
        }

        private void DailyScheduleElapsed(object sender, ElapsedEventArgs e)
        {
            for (int i = 0; i < _schedules.Count; i++)
            {
                Schedule schedule = _schedules[i];
                TimeSpan now = DateTime.Now.TimeOfDay;
                if ((schedule.Day == DateTime.Now.DayOfWeek) && 
                    (schedule.StartTime.Hours == now.Hours) && 
                    (schedule.StartTime.Minutes == now.Minutes) && 
                    (schedule.StartTime.Seconds == now.Seconds))
                {
                    Task.Run(() => StartSchedule(schedule.AudioLocation, schedule.PlayerLocation));
                    _eventLog.WriteEntry($"Daily Schedule matches!");
                }
            }
        }

        private void EventElapsed(object sender, ElapsedEventArgs e)
        {
            for (int i = 0; i < _events.Count; i++)
            {
                Event _event = _events[i];
                TimeSpan now = DateTime.Now.TimeOfDay;
                if ((_event.Date.Date == DateTime.Now.Date) &&
                    (_event.StartTime.Hours == now.Hours) && 
                    (_event.StartTime.Minutes == now.Minutes) && 
                    (_event.StartTime.Seconds == now.Seconds))
                {
                    Task.Run(() => StartSchedule(_event.AudioLocation, _event.PlayerLocation));
                    _eventLog.WriteEntry($"Event matches!");
                }
            }
        }

        private void RepeatedScheduleElapsed(object sender, ElapsedEventArgs e)
        {
            for(int i=0;i<_repeatedSchedules.Count;i++)
            {
                RepeatedSchedule schedule = _repeatedSchedules[i];
                TimeSpan now;
                switch (schedule.Repetition)
                {
                    case Repetition.Hourly:
                        now = DateTime.Now.TimeOfDay;
                        if ((schedule.StartDateTime.TimeOfDay.Minutes == now.Minutes) && 
                            (schedule.StartDateTime.TimeOfDay.Seconds == now.Seconds))
                        {
                            Task.Run(() => StartSchedule(schedule.AudioLocation, schedule.PlayerLocation));
                            _eventLog.WriteEntry($"Hourly Repeated Schedule matches!");
                        }
                        break;
                    case Repetition.Daily:
                        now = DateTime.Now.TimeOfDay;
                        if ((schedule.StartDateTime.TimeOfDay.Hours == now.Hours) &&
                            (schedule.StartDateTime.TimeOfDay.Minutes == now.Minutes) &&
                            (schedule.StartDateTime.TimeOfDay.Seconds == now.Seconds))
                        {
                            Task.Run(() => StartSchedule(schedule.AudioLocation, schedule.PlayerLocation));
                            _eventLog.WriteEntry($"Daily Repeated Schedule matches!");
                        }
                        break;
                    case Repetition.Monthly:
                        
                        if ((schedule.StartDateTime.Month == DateTime.Now.Month) &&
                            (schedule.StartDateTime.Day == DateTime.Now.Day) && 
                            (schedule.StartDateTime.TimeOfDay.Hours == DateTime.Now.TimeOfDay.Hours) &&
                            (schedule.StartDateTime.TimeOfDay.Minutes == DateTime.Now.TimeOfDay.Minutes) &&
                            (schedule.StartDateTime.TimeOfDay.Seconds == DateTime.Now.TimeOfDay.Seconds))
                        {
                            Task.Run(() => StartSchedule(schedule.AudioLocation, schedule.PlayerLocation));
                            _eventLog.WriteEntry($"Daily Repeated Schedule matches!");
                        }
                        break;
                }
            }
        }

        private void StartContinue()
        {
          
            try
            {
                using (Repository<Schedule> repo = new Repository<Schedule>())
                {
                    var result = repo.GetAll();
                    if (result.Status.Success)
                    {
                        if (result.Data.Count > 0)
                        {
                            _schedules = result.Data as List<Schedule>;
                            _timerDailySchedule.Start();
                            _isTimerDailyScheduleRunning = true;
                            _eventLog.WriteEntry($"Starting Daily Schedule Timer at {DateTime.Now}");
                        }
                        else
                            _isTimerDailyScheduleRunning = false;
                    }
                    else
                    {
                        _eventLog.WriteEntry($"Failed to start Daily Schedule Timer", EventLogEntryType.Error);
                        _isTimerDailyScheduleRunning = false;
                    }
                }
                using (Repository<Event> repo = new Repository<Event>())
                {
                    var result = repo.GetAll();
                    if (result.Status.Success)
                    {
                        if (result.Data.Count > 0)
                        {
                            _events = result.Data as List<Event>;
                            _timerEvent.Start();
                            _isTimerEventRunning = true;
                            _eventLog.WriteEntry($"Starting Event Timer at {DateTime.Now}");
                        }
                        else
                            _isTimerEventRunning = false;
                    }
                    else
                    {
                        _eventLog.WriteEntry($"Failed to start Event Timer", EventLogEntryType.Error);
                        _isTimerEventRunning = false;
                    }
                }
                using (Repository<RepeatedSchedule> repo = new Repository<RepeatedSchedule>())
                {
                    var result = repo.GetAll();
                    if (result.Status.Success)
                    {
                        if (result.Data.Count > 0)
                        {
                            _repeatedSchedules = result.Data as List<RepeatedSchedule>;
                            _timerRepeatedSchedule.Start();
                            _isTimerRepeatedScheduleRunning = true;
                            _eventLog.WriteEntry($"Starting Repeated Schedule Timer at {DateTime.Now}");
                        }
                        else
                            _isTimerRepeatedScheduleRunning = false;
                    }
                    else
                    {
                        _eventLog.WriteEntry($"Failed to start Repeated Schedule Timer", EventLogEntryType.Error);
                        _isTimerRepeatedScheduleRunning = false;
                    }
                }

            }
            catch (Exception ex)
            {
                _eventLog.WriteEntry($"Error while loading data.\n{ex.Message}", EventLogEntryType.Error);
            }
        }
        
        private void PauseStop()
        {
            
            try
            {
                if (_isTimerDailyScheduleRunning)
                {
                    _timerDailySchedule.Stop();
                    _isTimerDailyScheduleRunning = false;
                }
                if (_isTimerEventRunning)
                {
                    _timerEvent.Stop();
                    _isTimerEventRunning = false;
                }
                if (_isTimerRepeatedScheduleRunning)
                {
                    _timerRepeatedSchedule.Stop();
                    _isTimerRepeatedScheduleRunning = false;
                }
            }
            catch(Exception ex)
            {
                _eventLog.WriteEntry($"Error while stopping all timers.\n{ex.Message}", EventLogEntryType.Error);
            }
        }

        private void StartSchedule(string media, string player)
        {
            try
            {
                ProcessStartInfo process = new ProcessStartInfo(player, $"\"{media}\"")
                {
                    UseShellExecute = false
                };
                Process.Start(process);
            }
            catch { }
        }
    }
}
