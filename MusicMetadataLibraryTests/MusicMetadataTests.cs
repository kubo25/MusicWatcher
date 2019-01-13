using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicMetadataLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicMetadataLibrary.Tests {
    [TestClass()]
    public class MusicMetadataTests {
        [TestMethod()]
        public void FullTagsSaveTest() {
            //Arrange
            string testBackupLocation = "TestFiles/fullTagsBackup.mp3";
            if (File.Exists(testBackupLocation)) {
                File.Delete(testBackupLocation);
            }
            File.Copy("TestFiles/testFullTags.mp3", testBackupLocation);

            MusicMetadata original = new MusicMetadata(testBackupLocation);
            string testString = "test";
            uint testUint = 2099;

            //Act
            original.Title = testString;
            original.Artist = testString;
            original.Album = testString;
            original.Year = testUint;
            original.Track = testUint;
            original.Genre = testString;
            original.Comment = testString;
            original.AlbumArtist = testString;
            original.Composer = testString;
            original.Discnumber = testUint;

            original.Save();
            original.Dispose();

            original = new MusicMetadata(testBackupLocation);

            //Assert
            Assert.AreEqual(original.Title, testString);
            Assert.AreEqual(original.Artist, testString);
            Assert.AreEqual(original.Album, testString);
            Assert.AreEqual(original.Year, testUint);
            Assert.AreEqual(original.Track, testUint);
            Assert.AreEqual(original.Genre, testString);
            Assert.AreEqual(original.Comment, testString);
            Assert.AreEqual(original.AlbumArtist, testString);
            Assert.AreEqual(original.Composer, testString);
            Assert.AreEqual(original.Discnumber, testUint);
        }

        [TestMethod()]
        public void NotTagsSaveTest() {
            //Arrange
            string testBackupLocation = "TestFiles/noTagsBackup.mp3";
            if (File.Exists(testBackupLocation)) {
                File.Delete(testBackupLocation);
            }
            File.Copy("TestFiles/testNoTags.mp3", testBackupLocation);

            MusicMetadata original = new MusicMetadata(testBackupLocation);
            string testString = "test";
            uint testUint = 2099;

            //Act
            original.Title = testString;
            original.Artist = testString;
            original.Album = testString;
            original.Year = testUint;
            original.Track = testUint;
            original.Genre = testString;
            original.Comment = testString;
            original.AlbumArtist = testString;
            original.Composer = testString;
            original.Discnumber = testUint;

            original.Save();
            original.Dispose();

            original = new MusicMetadata(testBackupLocation);

            //Assert
            Assert.AreEqual(original.Title, testString);
            Assert.AreEqual(original.Artist, testString);
            Assert.AreEqual(original.Album, testString);
            Assert.AreEqual(original.Year, testUint);
            Assert.AreEqual(original.Track, testUint);
            Assert.AreEqual(original.Genre, testString);
            Assert.AreEqual(original.Comment, testString);
            Assert.AreEqual(original.AlbumArtist, testString);
            Assert.AreEqual(original.Composer, testString);
            Assert.AreEqual(original.Discnumber, testUint);
            Assert.AreEqual(original.AlbumArtSize, "No album art.");
        }

        [TestMethod()]
        public void BlackArtSaveTest() {
            //Arrange
            string testBackupLocation = "TestFiles/blackArtBackup.mp3";
            if (File.Exists(testBackupLocation)) {
                File.Delete(testBackupLocation);
            }
            File.Copy("TestFiles/testBlackArt.mp3", testBackupLocation);

            MusicMetadata original = new MusicMetadata(testBackupLocation);
            string testString = "test";
            uint testUint = 2099;

            //Act
            original.Title = testString;
            original.Artist = testString;
            original.Album = testString;
            original.Year = testUint;
            original.Track = testUint;
            original.Genre = testString;
            original.Comment = testString;
            original.AlbumArtist = testString;
            original.Composer = testString;
            original.Discnumber = testUint;

            original.Save();
            original.Dispose();

            original = new MusicMetadata(testBackupLocation);

            //Assert
            Assert.AreEqual(original.Title, testString);
            Assert.AreEqual(original.Artist, testString);
            Assert.AreEqual(original.Album, testString);
            Assert.AreEqual(original.Year, testUint);
            Assert.AreEqual(original.Track, testUint);
            Assert.AreEqual(original.Genre, testString);
            Assert.AreEqual(original.Comment, testString);
            Assert.AreEqual(original.AlbumArtist, testString);
            Assert.AreEqual(original.Composer, testString);
            Assert.AreEqual(original.Discnumber, testUint);
        }
    }
}