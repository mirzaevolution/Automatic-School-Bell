using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CoreLib.DataAccess;
using CoreLib.Models;
namespace CoreLib.Tests
{
    [TestClass]
    public class ScheduleTest
    {
        [TestMethod]
        public void InsertTest()
        {
            Schedule schedule = new Schedule
            {
                Day = DayOfWeek.Monday,
                Description = "First Hour",
                AudioLocation = @"C:\Users\skyline\Music\Free YouTube Downloader\MONA L - SURAT CINTA (Official Music Video).mp3",
                PlayerLocation = @"C:\Program Files (x86)\GRETECH\GomAudio\Goma.exe",
                StartTime = new TimeSpan(7, 0, 0)
            };
            using (Repository<Schedule> repo = new Repository<Schedule>())
            {
                var result = repo.Insert(schedule);

                Assert.IsTrue(result.Success);
            }
        }
        [TestMethod]
        public void InsertMultipleItemsTest()
        {
            Schedule[] schedules = new Schedule[]
            {
                new Schedule
                {
                    Day = DayOfWeek.Tuesday,
                    Description = "Second Hour",
                    AudioLocation = @"D:\from cyber-hacker\special musics\Teringat Selalu - Rani.mp3",
                     PlayerLocation = @"C:\Program Files (x86)\GRETECH\GomAudio\Goma.exe",
                     StartTime = new TimeSpan(9,0,0)
                },
                new Schedule
                {
                    Day = DayOfWeek.Wednesday,
                    Description = "Third Hour",
                    AudioLocation = @"D:\new Music\ratih-purwasih-surat-cinta.mp3",
                     PlayerLocation = @"C:\Program Files (x86)\GRETECH\GomAudio\Goma.exe",
                     StartTime = new TimeSpan(11,0,0)
                },

                new Schedule
                {
                    Day = DayOfWeek.Tuesday,
                    Description = "Fourth Hour",
                    AudioLocation = @"D:\MyMidi\Teluk Bayur.mp3",
                     PlayerLocation = @"C:\Program Files (x86)\GRETECH\GomAudio\Goma.exe",
                     StartTime = new TimeSpan(13,0,0)
                },

            };
            using (Repository<Schedule> repo = new Repository<Schedule>())
            {
                foreach (Schedule schedule in schedules)
                {
                    var result = repo.Insert(schedule);
                    Assert.IsTrue(result.Success);
                }
            }
        }
        [TestMethod]
        public void UpdateLocalTest()
        {
            using (Repository<Schedule> repo = new Repository<Schedule>())
            {
                var result = repo.Get(4);
                Assert.IsTrue(result.Status.Success);
                if(result.Status.Success)
                {
                    Schedule schedule = result.Data;
                    schedule.Day = DayOfWeek.Thursday;
                    var updateResult = repo.Update(schedule);
                    Assert.IsTrue(updateResult.Success);
                }
            }
        }
        [TestMethod]
        public void UpdateAttachTest()
        {
            Schedule schedule = null;
            using (Repository<Schedule> repo = new Repository<Schedule>())
            {
                var result = repo.Get(4);
                if(result.Status.Success)
                {
                    schedule = result.Data;
                }
            }
            Assert.IsNotNull(schedule);
            schedule.AudioLocation = @"D:\new Music\nur-afni-octaviani-surat-cinta.mp3";
            using (Repository<Schedule> repo = new Repository<Schedule>())
            {
                var result = repo.Update(schedule);
                Assert.IsTrue(result.Success);
            }
        }
        [TestMethod]
        public void DeleteTest()
        {
            using (Repository<Schedule> repo = new Repository<Schedule>())
            {
                var result = repo.Delete(2);
                Assert.IsTrue(result.Success);
            }
        }
    }
}
