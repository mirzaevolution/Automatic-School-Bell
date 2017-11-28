using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CoreLib.DataAccess;
using CoreLib.Models;
namespace CoreLib.Tests
{
    [TestClass]
    public class EventTest
    {
        [TestMethod]
        public void InsertTest()
        {
            Event _event = new Event
            {
                Description = "First Event",
                StartTime = new TimeSpan(7, 0, 0),
                Date = new DateTime(2017, 12, 1),
                AudioLocation = @"C:\Users\skyline\Music\Free YouTube Downloader\MONA L - SURAT CINTA (Official Music Video).mp3",
                PlayerLocation = @"C:\Program Files (x86)\GRETECH\GomAudio\Goma.exe"
            };
            using (Repository<Event> repo = new Repository<Event>())
            {
                var result = repo.Insert(_event);

                Assert.IsTrue(result.Success);
            }
        }
        [TestMethod]
        public void InsertMultipleItemsTest()
        {
            Event[] events = new Event[]
            {
                new Event
                {
                    Date = new DateTime(2017,12,2),
                    Description = "Second Event",
                    AudioLocation = @"D:\from cyber-hacker\special musics\Teringat Selalu - Rani.mp3",
                     PlayerLocation = @"C:\Program Files (x86)\GRETECH\GomAudio\Goma.exe",
                     StartTime = new TimeSpan(9,0,0)
                },
                new Event
                {
                    Date = new DateTime(2017,12,3),
                    Description = "Third Hour",
                    AudioLocation = @"D:\new Music\ratih-purwasih-surat-cinta.mp3",
                    PlayerLocation = @"C:\Program Files (x86)\GRETECH\GomAudio\Goma.exe",
                    StartTime = new TimeSpan(11,0,0)
                },

                new Event
                {
                    Date = new DateTime(2017,12,4),
                    Description = "Fourth Hour",
                    AudioLocation = @"D:\MyMidi\Teluk Bayur.mp3",
                    PlayerLocation = @"C:\Program Files (x86)\GRETECH\GomAudio\Goma.exe",
                    StartTime = new TimeSpan(13,0,0)
                },

            };
            using (Repository<Event> repo = new Repository<Event>())
            {
                foreach (Event _event in events)
                {
                    var result = repo.Insert(_event);
                    Assert.IsTrue(result.Success);
                }
            }
        }
        [TestMethod]
        public void UpdateLocalTest()
        {
            using (Repository<Event> repo = new Repository<Event>())
            {
                var result = repo.Get(3);
                Assert.IsTrue(result.Status.Success);
                if (result.Status.Success)
                {
                    Event _event = result.Data;
                    _event.Description = "Third Event";
                    var updateResult = repo.Update(_event);
                    Assert.IsTrue(updateResult.Success);
                }
            }
        }
        [TestMethod]
        public void UpdateAttachTest()
        {
            Event _event = null;
            using (Repository<Event> repo = new Repository<Event>())
            {
                var result = repo.Get(4);
                if (result.Status.Success)
                {
                    _event = result.Data;
                }
            }
            Assert.IsNotNull(_event);
            _event.Description = "Fourth Event";
            using (Repository<Event> repo = new Repository<Event>())
            {
                var result = repo.Update(_event);
                Assert.IsTrue(result.Success);
            }
        }
        [TestMethod]
        public void DeleteTest()
        {
            using (Repository<Event> repo = new Repository<Event>())
            {
                var result = repo.Delete(2);
                Assert.IsTrue(result.Success);
            }
        }
    }
}
