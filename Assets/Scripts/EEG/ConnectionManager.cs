using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using EmotivUnityPlugin;
using UnityEngine;
using UnityEngine.Assertions;
using Grid = Systems.Grid;

namespace EEG {
    public class ConnectionManager : MonoBehaviour {
        private const float PERIOD_BAND_POWER_DATA_CHECK = 0.1f;
        private const int MAX_NUMBER_OF_FAILED_ATTEMPTS = 5;

        private const string LOG_PREFIX = "ConnectionManager";
        public DataProcessing eegDataProcessing;
        private readonly DataStreamManager _dataStreamManager = DataStreamManager.Instance;
        private readonly List<Headset> _headsets = new List<Headset>();

        private int _numberOfFailedAttemptsToConnect;

        // ReSharper disable once InconsistentNaming
        private float _timeEEGDataLastCheck;
        private float _timeMotorDataLastCheck;

        private void Start() {
            Configure();
            HeadsetFinder.Instance.QueryHeadsetOK += GetListOfFoundHeadsets;
            _dataStreamManager.HeadsetConnectFail += HandleFailedAttempt;
            _dataStreamManager.SessionActivatedOK += HandleSessionActivated;
        }

        private void Update() {
            DisplayBandPowerData();
        }

        private void OnApplicationQuit() {
            _dataStreamManager.Stop();
        }

        private void Configure() {
            _dataStreamManager.SetAppConfig(AppConfig.ClientId, AppConfig.ClientSecret, AppConfig.AppVersion,
                AppConfig.AppName, AppConfig.TmpAppDataDir, AppConfig.AppUrl, EmotivApplicationPath());
            _dataStreamManager.StartAuthorize(AppConfig.AppLicenseId);
            CortexClient.Instance.RequestAccess();
            RawDataProcessing.Instance.EnableQueryHeadset(true);
        }

        private void HandleFailedAttempt(object sender, string e) {
            Log("Connection failed");
            _numberOfFailedAttemptsToConnect++;
            if (_numberOfFailedAttemptsToConnect < MAX_NUMBER_OF_FAILED_ATTEMPTS)
                ConnectToHeadset();
            else
                Debug.LogError("Unable to connect with the headset");
        }

        private void GetListOfFoundHeadsets(object sender, List<Headset> headsets) {
            foreach (var headset in headsets.Where(headset => !_headsets.Contains(headset))) _headsets.Add(headset);
        }

        private void HandleSessionActivated(object sender, string e) {
            Debug.Log("OnSessionActivatedOK");
        }

        private void SubscribeForBandPowerData() {
            _dataStreamManager.SubscribeMoreData(new List<string> {
                DataStreamName.BandPower, DataStreamName.DevInfos
            });
        }

        public void ConnectToHeadset() {
            var dataStreamList = new List<string> {DataStreamName.DevInfos};
            Assert.IsFalse(_headsets.Count == 0);
            _dataStreamManager.StartDataStream(dataStreamList, _headsets[0]?.HeadsetID);
            Thread.Sleep(500);
            SubscribeForBandPowerData();
            Grid.GameEvents.HeadsetConnected();
        }

        private void DisplayBandPowerData() {
            if (IsNotLongEnoughSinceLastCheck()) return;

            var alpha = GetAlphaPackage();
            var theta = GetThetaPackage();
            var betaL = GetBetaLPackage();
            var betaH = GetBetaHPackage();

            PassDataForProcessing(alpha, theta, betaL, betaH);
        }

        private bool IsNotLongEnoughSinceLastCheck() {
            _timeEEGDataLastCheck += Time.deltaTime;
            if (!(_timeEEGDataLastCheck > PERIOD_BAND_POWER_DATA_CHECK)) return true;
            _timeEEGDataLastCheck -= PERIOD_BAND_POWER_DATA_CHECK;
            return false;
        }

        private EegPackage GetAlphaPackage() {
            return new EegPackage {
                T7Value = (float) _dataStreamManager.GetAlphaData(Channel_t.CHAN_T7),
                T8Value = (float) _dataStreamManager.GetAlphaData(Channel_t.CHAN_T8),
                PzValue = (float) _dataStreamManager.GetAlphaData(Channel_t.CHAN_Pz),
                Af4Value = (float) _dataStreamManager.GetAlphaData(Channel_t.CHAN_AF3),
                Af3Value = (float) _dataStreamManager.GetAlphaData(Channel_t.CHAN_AF4)
            };
        }

        private EegPackage GetThetaPackage() {
            return new EegPackage {
                T7Value = (float) _dataStreamManager.GetThetaData(Channel_t.CHAN_T7),
                T8Value = (float) _dataStreamManager.GetThetaData(Channel_t.CHAN_T8),
                PzValue = (float) _dataStreamManager.GetThetaData(Channel_t.CHAN_Pz),
                Af4Value = (float) _dataStreamManager.GetThetaData(Channel_t.CHAN_AF3),
                Af3Value = (float) _dataStreamManager.GetThetaData(Channel_t.CHAN_AF4)
            };
        }

        private EegPackage GetBetaLPackage() {
            return new EegPackage {
                T7Value = (float) _dataStreamManager.GetLowBetaData(Channel_t.CHAN_T7),
                T8Value = (float) _dataStreamManager.GetLowBetaData(Channel_t.CHAN_T8),
                PzValue = (float) _dataStreamManager.GetLowBetaData(Channel_t.CHAN_Pz),
                Af4Value = (float) _dataStreamManager.GetLowBetaData(Channel_t.CHAN_AF3),
                Af3Value = (float) _dataStreamManager.GetLowBetaData(Channel_t.CHAN_AF4)
            };
        }

        private EegPackage GetBetaHPackage() {
            return new EegPackage {
                T7Value = (float) _dataStreamManager.GetHighBetaData(Channel_t.CHAN_T7),
                T8Value = (float) _dataStreamManager.GetHighBetaData(Channel_t.CHAN_T8),
                PzValue = (float) _dataStreamManager.GetHighBetaData(Channel_t.CHAN_Pz),
                Af4Value = (float) _dataStreamManager.GetHighBetaData(Channel_t.CHAN_AF3),
                Af3Value = (float) _dataStreamManager.GetHighBetaData(Channel_t.CHAN_AF4)
            };
        }

        private void PassDataForProcessing(EegPackage alpha, EegPackage theta, EegPackage lowBeta,
            EegPackage highBeta) {
            if (alpha != 0) eegDataProcessing.AddNewAlphaValue(alpha);

            if (theta != 0) eegDataProcessing.AddNewThetaValue(theta);

            if (lowBeta != 0) eegDataProcessing.AddNewBetaLValue(lowBeta);

            if (highBeta != 0) eegDataProcessing.AddNewBetaHValue(highBeta);

            if (highBeta != 0 && lowBeta != 0) eegDataProcessing.AddNewBetaValue(lowBeta + highBeta);
        }


        private static string EmotivApplicationPath() {
            var path = Application.dataPath;

            var newPath = "";
            if (Application.platform == RuntimePlatform.OSXPlayer)
                newPath = Path.GetFullPath(Path.Combine(path, @"../../"));
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
                newPath = Path.GetFullPath(Path.Combine(path, @"../"));

            return newPath;
        }

        private static void Log(string message) {
            Debug.Log($"{LOG_PREFIX}: {message}");
        }
    }
}