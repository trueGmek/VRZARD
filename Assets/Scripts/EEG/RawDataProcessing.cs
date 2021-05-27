using System;
using System.Collections.Generic;
using EmotivUnityPlugin;
using UnityEngine;

namespace EEG {
    public class RawDataProcessing {
        private const int NUMBER_OF_SENSORS = 16; // 14 channel EEG, CMS, DRL

        private static RawDataProcessing instance;
        private static readonly object Object = new object();

        private readonly Channels[] _channelList = {
            Channels.AF3, Channels.AF4, Channels.F3,
            Channels.F4, Channels.F7, Channels.F8,
            Channels.FC5, Channels.FC6, Channels.O1,
            Channels.O2, Channels.P7, Channels.P8,
            Channels.T7, Channels.T8
        };

        private readonly ContactQualityValue[] _contactQualityData = new ContactQualityValue[NUMBER_OF_SENSORS];
        private readonly Dictionary<string, Headset> _headsetList = new Dictionary<string, Headset>();
        private int _buferSize = 0;


        private Headset _curHeadsetObjectConnected;
        private bool _enableQueryHeadset = true;

        // This value is for fix issue that noted in description of the class. 
        // if it is null. the connected event is processed. or haven't no event yet. 
        // if it is not null. the connected event is waiting for process. 
        private string _headsetIdConnected;
        private bool _isConnect;
        private double _lastCQOverAll;
        private int _lastNSamples;

        // -1: haven't checked yet
        // 0: license expried
        // > 0 and <= 7. Free license
        // a big number. License valid
        private double _nRemainingDay = -1;

        private RecordManager _recordManager = RecordManager.Instance;

        public DeviceScan[] deviceNames;

        public RawDataProcessing() {
            DataStreamManager.Instance.SessionActivatedOK += OnSessionActivatedOK;
            DataStreamManager.Instance.LicenseValidTo += LicenseValidTo;
        }

        public static RawDataProcessing Instance {
            get {
                if (instance == null) instance = new RawDataProcessing();

                return instance;
            }
        }

        public event EventHandler onHeadsetChange;
        public event EventHandler onCurrHeadsetRemoved;
        public event EventHandler onDetectManyDevices;
        public event EventHandler<string> onHeadsetConnected;

        public event EventHandler<string> HeadsetConnectFail {
            add => DataStreamManager.Instance.HeadsetConnectFail += value;
            remove => DataStreamManager.Instance.HeadsetConnectFail -= value;
        }

        ~RawDataProcessing() {
        }

        public Channels[] GetEEGChannelList() {
            lock (Object) {
                return _channelList;
            }
        }

        public void SetConnectedHeadset(Headset headsetInfos) {
            lock (Object) {
                // Debug.Log("======== SetConnectedHeadset " + headsetInfos.HeadsetID);
                _curHeadsetObjectConnected = headsetInfos;
            }
        }

        // disable query headset while connecting to a headset
        // and enable it again after done connecting process(success or failed)
        public void EnableQueryHeadset(bool enable) {
            lock (Object) {
                _enableQueryHeadset = enable;
            }
        }

        public int GetNumCQChannelMax() {
            return NUMBER_OF_SENSORS;
        }

        private void HeadsetListUpdate() {
            if (onHeadsetChange != null)
                onHeadsetChange(null, null);
        }

        public void UpdateContactQuality() {
            lock (Object) {
                if (_curHeadsetObjectConnected == null) {
                    _lastCQOverAll = 0;
                    return;
                }

                for (var i = 0; i < _contactQualityData.Length; i++)
                    _contactQualityData[i] = ContactQualityValue.NO_SIGNAL;

                var cqSize = DataStreamManager.Instance.GetNumberCQSamples();

                // header: Battery, Signal, AF3, T7, Pz, T8, AF4, OVERALL,
                // We keep using Channels because this app still follow it
                _contactQualityData[(int) Channels.AF3] =
                    (ContactQualityValue) (int) DataStreamManager.Instance
                        .GetContactQuality(Channel_t.CHAN_AF3);
                _contactQualityData[(int) Channels.T7] =
                    (ContactQualityValue) (int) DataStreamManager.Instance.GetContactQuality(Channel_t.CHAN_T7);
                _contactQualityData[(int) Channels.O1] =
                    (ContactQualityValue) (int) DataStreamManager.Instance.GetContactQuality(Channel_t.CHAN_Pz);
                _contactQualityData[(int) Channels.T8] =
                    (ContactQualityValue) (int) DataStreamManager.Instance.GetContactQuality(Channel_t.CHAN_T8);
                _contactQualityData[(int) Channels.AF4] =
                    (ContactQualityValue) (int) DataStreamManager.Instance
                        .GetContactQuality(Channel_t.CHAN_AF4);

                _lastCQOverAll = DataStreamManager.Instance.GetContactQuality(Channel_t.CHAN_CQ_OVERALL);

                _contactQualityData[(int) Channels.CMS] = ContactQualityValue.VERY_BAD;
                _contactQualityData[(int) Channels.DRL] = ContactQualityValue.VERY_BAD;
                for (var i = 1; i < _contactQualityData.Length; i++)
                    if (_contactQualityData[i] > ContactQualityValue.VERY_BAD) {
                        _contactQualityData[(int) Channels.CMS] = ContactQualityValue.GOOD;
                        _contactQualityData[(int) Channels.DRL] = ContactQualityValue.GOOD;
                        break;
                    }
            }
        }

        public bool IsHeadsetConnected() {
            lock (Object) {
                return _isConnect;
            }
        }

        public int GetNumberEegSamples() {
            return DataStreamManager.Instance.GetNumberEEGSamples();
        }

        //TODO: Improve it next time
        public int GetLastNumberEEGSamples() {
            return _lastNSamples;
        }

        public double[][] GetDataPoker() {
            lock (Object) {
                _lastNSamples = DataStreamManager.Instance.GetNumberEEGSamples();
                if (_lastNSamples <= 0)
                    return null;

                if (_curHeadsetObjectConnected == null)
                    return null;

                var buf = new double[_channelList.Length][];

                buf[(int) Channels.AF3] = DataStreamManager.Instance.GetEEGData(Channel_t.CHAN_AF3);
                buf[(int) Channels.T7] = DataStreamManager.Instance.GetEEGData(Channel_t.CHAN_T7);
                buf[(int) Channels.T8] = DataStreamManager.Instance.GetEEGData(Channel_t.CHAN_T8);
                buf[(int) Channels.AF4] = DataStreamManager.Instance.GetEEGData(Channel_t.CHAN_AF4);

                if (_curHeadsetObjectConnected.HeadsetType != HeadsetTypes.HEADSET_TYPE_INSIGHT) {
                    buf[(int) Channels.O1] = DataStreamManager.Instance.GetEEGData(Channel_t.CHAN_O1);
                    buf[(int) Channels.F7] = DataStreamManager.Instance.GetEEGData(Channel_t.CHAN_F7);
                    buf[(int) Channels.F3] = DataStreamManager.Instance.GetEEGData(Channel_t.CHAN_F3);
                    buf[(int) Channels.FC5] = DataStreamManager.Instance.GetEEGData(Channel_t.CHAN_FC5);
                    buf[(int) Channels.P7] = DataStreamManager.Instance.GetEEGData(Channel_t.CHAN_P7);
                    buf[(int) Channels.O2] = DataStreamManager.Instance.GetEEGData(Channel_t.CHAN_O2);
                    buf[(int) Channels.P8] = DataStreamManager.Instance.GetEEGData(Channel_t.CHAN_P8);
                    buf[(int) Channels.FC6] = DataStreamManager.Instance.GetEEGData(Channel_t.CHAN_FC6);
                    buf[(int) Channels.F4] = DataStreamManager.Instance.GetEEGData(Channel_t.CHAN_F4);
                    buf[(int) Channels.F8] = DataStreamManager.Instance.GetEEGData(Channel_t.CHAN_F8);
                }
                else {
                    buf[(int) Channels.O1] = DataStreamManager.Instance.GetEEGData(Channel_t.CHAN_Pz);
                }

                if (buf[(int) Channels.AF3] == null)
                    return null;

                // UnityEngine.Debug.Log(buf[(int)Channels.AF3][0] + ", " + 
                // buf[(int)Channels.T7][0] + ", " + 
                // buf[(int)Channels.T8][0] + ", " + 
                // buf[(int)Channels.AF4][0] + ", " + 
                // buf[(int)Channels.O1][0] + ", " + 
                // buf[(int)Channels.F7][0] + ", " + 
                // buf[(int)Channels.F3][0] + ", " + 
                // buf[(int)Channels.FC5][0] + ", " + 
                // buf[(int)Channels.P7][0] + ", " + 
                // buf[(int)Channels.O2][0] + ", " + 
                // buf[(int)Channels.P8][0] + ", " + 
                // buf[(int)Channels.FC6][0] + ", " + 
                // buf[(int)Channels.F4][0] + ", " + 
                // buf[(int)Channels.F8][0]);

                // double[] counterList = EmotivDataStream.Instance.GetEEGData(Channel_t.CHAN_COUNTER);
                // for (int i = 0; i < _lastNSamples; i++) {
                //     UnityEngine.Debug.Log("====== " + _lastNSamples + ", " + counterList[i]);
                // }

                return buf;
            }
        }

        public ContactQualityValue[] GetContactQuality() {
            lock (Object) {
                return _contactQualityData;
            }
        }

        public ContactQualityValue GetContactQuality(Channels channel) {
            lock (Object) {
                return _contactQualityData[(int) channel];
            }
        }

        public Dictionary<string, Headset> GetHeadsetList() {
            lock (Object) {
                return _headsetList;
            }
        }

        public double GetCQOverAll() {
            lock (Object) {
                return _lastCQOverAll;
            }
        }

        // return number of headset discovered
        private int queryHeadset() {
            if (!_enableQueryHeadset)
                return 0;

            var detectedHeadset = DataStreamManager.Instance.GetDetectedHeadsets();

            _headsetList.Clear();
            foreach (var item in detectedHeadset) _headsetList[item.HeadsetID] = item;

            // Detect the headset is disconnected
            if (_curHeadsetObjectConnected != null) {
                var isDisconnected = false;
                if (!_headsetList.ContainsKey(_curHeadsetObjectConnected.HeadsetID))
                    isDisconnected = true;
                else
                    isDisconnected = _headsetList[_curHeadsetObjectConnected.HeadsetID].Status ==
                                     HeadsetConnectionStatus.DISCOVERED;

                if (isDisconnected) {
                    Debug.Log("RawDataProcessing:queryHeadset - Disconnected the headset");

                    if (onCurrHeadsetRemoved != null)
                        onCurrHeadsetRemoved(null, null);

                    _curHeadsetObjectConnected = null;
                }
            }

            if (_headsetIdConnected != null && _headsetList.ContainsKey(_headsetIdConnected)) {
                if (_headsetList[_headsetIdConnected].Status == HeadsetConnectionStatus.CONNECTED) {
                    Debug.Log("RawDataProcessing:queryHeadset - emit headset connected");
                    onHeadsetConnected(this, _headsetIdConnected);
                    _headsetIdConnected = null;
                }
                else if (_headsetList[_headsetIdConnected].Status == HeadsetConnectionStatus.DISCOVERED) {
                    Debug.Log("RawDataProcessing:queryHeadset - remove event headset connected");
                    // _headsetIdConnected = null;
                }
                else {
                    Debug.Log("RawDataProcessing:queryHeadset - the headset still connecting");
                }
            }

            HeadsetListUpdate();

            return detectedHeadset.Count;
        }

        private bool CheckHeadsetConnected() {
            if (_curHeadsetObjectConnected == null || _curHeadsetObjectConnected.HeadsetID == "")
                _isConnect = false;
            else
                _isConnect = true;

            return _isConnect;
        }

        public void Process() {
            lock (Object) {
                if (queryHeadset() <= 0)
                    return;

                if (!CheckHeadsetConnected())
                    for (var i = 0; i < _contactQualityData.Length; i++)
                        _contactQualityData[i] = ContactQualityValue.NO_SIGNAL;
            }
        }

        // Slots
        private void OnSessionActivatedOK(object sender, string headsetID) {
            lock (Object) {
                _headsetIdConnected = headsetID;

                // TEST Start Record
                // _recordManager.StartRecord("tung260420", "demoUnity");
            }
        }

        private void LicenseValidTo(object sender, DateTime validToDate) {
            var curUTC_Now = DateTime.UtcNow;
            var diffDate = validToDate - curUTC_Now;
            var nDay = (double) (int) (diffDate.TotalDays * 10) / 10;
            if (nDay < 0)
                _nRemainingDay = 0;
            else
                _nRemainingDay = nDay;

            // Debug.Log("ValidToDate = " + validToDate + ", UTC = " + curUTC_Now + ", nDay = " + nDay);
        }

        // -1: haven't checked yet
        // 0: license expried
        // > 0 and <= 7. Free license
        // a big number. License valid
        public double GetLicenseRemainingDay() {
            return _nRemainingDay;
        }

        public struct DeviceScan {
            public int Index;
            public string Name;
        }
    }
}