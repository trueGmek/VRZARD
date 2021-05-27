using System;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace Systems {
    public class DataExporter : MonoBehaviour {
        public string targetLocation = @"C:\Lab_session_" + Today.ToString("g") + @"\";

        private static readonly DateTime Today = DateTime.Today;

        private const string EVENTS_FILE_NAME = "events";
        private const string POSITIONS_FILE_NAME = "positions";
        private const string FILE_EXTENSTION = ".csv";
        private const string SEPARATOR = ";";
        private const string FILEPATH_SEPARATOR = @"\";
        private const string NEW_LINE = "\n";

        private DateTime _startTime;
        private readonly StringBuilder _eventsStringBuilder = new StringBuilder();
        private readonly StringBuilder _positionsStringBuilder = new StringBuilder();

        private PositionSecretary _positionSecretary;

        private void Start() {
            _startTime = DateTime.Now;
            NoteEvent("Start time: " + _startTime.ToString("f"));
        }

        private string GETSecondsSinceGameStart() {
            return (DateTime.Now - _startTime).TotalSeconds.ToString(CultureInfo.InvariantCulture);
        }

        public void NoteEvent(string eventName) {
            _eventsStringBuilder.Append(eventName).Append(SEPARATOR);
            _eventsStringBuilder.Append(GETSecondsSinceGameStart());
            _eventsStringBuilder.Append(NEW_LINE);
        }

        public void NotePosition(Vector3 position) {
            _positionsStringBuilder.Append(position).Append(SEPARATOR);
            _positionsStringBuilder.Append(GETSecondsSinceGameStart());
            _positionsStringBuilder.Append(NEW_LINE);
        }

        private void OnApplicationQuit() {
            ExportData();
        }

        public void ExportData() {
            WriteData(EVENTS_FILE_NAME, _eventsStringBuilder);
            WriteData(POSITIONS_FILE_NAME, _positionsStringBuilder);
        }

        private void WriteData(string fileName, StringBuilder data) {
            var writer = new StreamWriter(targetLocation + FILEPATH_SEPARATOR + fileName + FILE_EXTENSTION);
            writer.Write(data.ToString());
            writer.Close();
        }
    }
}