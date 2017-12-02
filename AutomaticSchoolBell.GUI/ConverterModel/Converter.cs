using CoreLib.Models;
namespace AutomaticSchoolBell.GUI
{
    public class Converter
    {
        public static ScheduleModel ConvertToScheduleModel(Schedule schedule)
        {
            return new ScheduleModel
            {
                Id = schedule.ID,
                Day = schedule.Day,
                StartTime = schedule.StartTime,
                Description = schedule.Description,
                AudioLocation = schedule.AudioLocation,
                PlayerLocation = schedule.PlayerLocation
            };
        }
        public static Schedule ConvertFromScheduleModel(ScheduleModel model)
        {
            return new Schedule
            {
                ID = model.Id,
                StartTime = model.StartTime,
                Day = model.Day,
                Description = model.Description,
                AudioLocation = model.AudioLocation,
                PlayerLocation = model.PlayerLocation
            };
        }
        public static EventModel ConvertToEventModel(Event _event)
        {
            return new EventModel
            {
                Id = _event.ID,
                Date = _event.Date,
                StartTime = _event.StartTime,
                Description = _event.Description,
                AudioLocation = _event.AudioLocation,
                PlayerLocation = _event.PlayerLocation
            };
        }
        public static Event ConvertFromEventModel(EventModel model)
        {
            return new Event
            {
                ID = model.Id,
                Date = model.Date,
                StartTime = model.StartTime,
                Description = model.Description,
                AudioLocation = model.AudioLocation,
                PlayerLocation = model.PlayerLocation
            };
        }
        public static RepeatedScheduleModel ConvertToRepeatedScheduleModel(RepeatedSchedule repeatedSchedule)
        {
            return new RepeatedScheduleModel
            {
                Id = repeatedSchedule.ID,
                StartDateTime = repeatedSchedule.StartDateTime,
                Repetition = repeatedSchedule.Repetition,
                Description = repeatedSchedule.Description,
                AudioLocation = repeatedSchedule.AudioLocation,
                PlayerLocation = repeatedSchedule.PlayerLocation
            };
        }
        public static RepeatedSchedule ConvertFromRepeatedScheduleModel(RepeatedScheduleModel model)
        {
            return new RepeatedSchedule
            {
                ID = model.Id,
                StartDateTime = model.StartDateTime.Value,
                Repetition = model.Repetition,
                Description = model.Description,
                AudioLocation = model.AudioLocation,
                PlayerLocation = model.PlayerLocation
            };
        }
    }
}
