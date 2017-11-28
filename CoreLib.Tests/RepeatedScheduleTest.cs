using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CoreLib.DataAccess;
using CoreLib.Models;
using System.Collections.Generic;

namespace CoreLib.Tests
{
    [TestClass]
    public class RepeatedScheduleTest
    {
        [TestMethod]
        public void InsertMultipleTest()
        {
            List<RepeatedSchedule> list = new List<RepeatedSchedule>
            {
                new RepeatedSchedule
                {
                    Description = "test 1",
                    StartDateTime = DateTime.Now,
                    Repetition = Repetition.Daily,
                    AudioLocation = @"C:\Users\skyline\Music\Free YouTube Downloader\MONA L - SURAT CINTA (Official Music Video).mp3",
                    PlayerLocation = @"C:\Program Files (x86)\GRETECH\GomAudio\Goma.exe"
                },
                new RepeatedSchedule
                {
                    Description = "test 2",
                    StartDateTime = DateTime.Now,
                    Repetition = Repetition.Hourly,
                                       AudioLocation = @"D:\new Music\ratih-purwasih-surat-cinta.mp3",
                    PlayerLocation = @"C:\Program Files (x86)\GRETECH\GomAudio\Goma.exe"
                },
                new RepeatedSchedule
                {
                    Description = "test 1",
                    StartDateTime = DateTime.Now,
                    Repetition = Repetition.Monthly,
                    AudioLocation = @"D:\MyMidi\Teluk Bayur.mp3",
                    PlayerLocation = @"C:\Program Files (x86)\GRETECH\GomAudio\Goma.exe"
                }

            };
            using (Repository<RepeatedSchedule> repo = new Repository<RepeatedSchedule>())
            {
                list.ForEach(x =>
                {
                    var result = repo.Insert(x);
                    Assert.IsTrue(result.Success);
                });
            }
        }
        [TestMethod]
        public void UpdateLocalTest()
        {
            using (Repository<RepeatedSchedule> repo = new Repository<RepeatedSchedule>())
            {
                var result = repo.Get(3);
                Assert.IsTrue(result.Status.Success);
                if (result.Status.Success)
                {
                    RepeatedSchedule repeatedSchedule = result.Data;
                    repeatedSchedule.Description = "test 3";
                    var updateResult = repo.Update(repeatedSchedule);
                    Assert.IsTrue(updateResult.Success);
                }
            }
        }
        [TestMethod]
        public void UpdateAttachTest()
        {
            RepeatedSchedule repeatedSchedule = null;
            using (Repository<RepeatedSchedule> repo = new Repository<RepeatedSchedule>())
            {
                var result = repo.Get(3);
                Assert.IsTrue(result.Status.Success);
                if (result.Status.Success)
                {
                    repeatedSchedule = result.Data;
                }
            }
            Assert.IsNotNull(repeatedSchedule);
            repeatedSchedule.StartDateTime = DateTime.Now.AddDays(2d);
            using (Repository<RepeatedSchedule> repo = new Repository<RepeatedSchedule>())
            {
                var result = repo.Update(repeatedSchedule);
                Assert.IsTrue(result.Success);
            }

        }
        [TestMethod]
        public void DeleteTest()
        {
            using (Repository<RepeatedSchedule> repo = new Repository<RepeatedSchedule>())
            {
                var result = repo.Delete(2);
                Assert.IsTrue(result.Success);
            }
        }
    }
}
