using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Grid = Systems.Grid;

namespace EEG {
    public class DataProcessing : MonoBehaviour {
        private const float PERIOD_FOCUS_STATE_CHECK = 0.5f;
        private const int MAXIMAL_SIZE_OF_RECENT_EEG_DATA = 100;

        public Dictionary<BandPowerRatios, EegPackage> boundAveragePackageOverBandPowerRatioType =
            new Dictionary<BandPowerRatios, EegPackage> {
                {BandPowerRatios.AlphaOverTheta, new EegPackage()},
                {BandPowerRatios.BetaLOverAlpha, new EegPackage()},
                {BandPowerRatios.BetaHOverAlpha, new EegPackage()},
                {BandPowerRatios.BetaOverAlpha, new EegPackage()}
            };


        [Header("Thresholds")] public float alphaOverThetaThreshold;

        public int eegAverageSampleSize = 20;

        private readonly Dictionary<BandPowerType, List<EegPackage>> _boundDataOverBandPowerType =
            new Dictionary<BandPowerType, List<EegPackage>> {
                {BandPowerType.Alpha, new List<EegPackage>()}, {BandPowerType.Theta, new List<EegPackage>()},
                {BandPowerType.BetaL, new List<EegPackage>()}, {BandPowerType.BetaH, new List<EegPackage>()},
                {BandPowerType.Beta, new List<EegPackage>()}
            };

        private readonly Dictionary<BandPowerType, List<EegPackage>> _recentEegDataOverBandPowerType =
            new Dictionary<BandPowerType, List<EegPackage>> {
                {BandPowerType.Alpha, new List<EegPackage>()}, {BandPowerType.Theta, new List<EegPackage>()},
                {BandPowerType.BetaL, new List<EegPackage>()}, {BandPowerType.BetaH, new List<EegPackage>()},
                {BandPowerType.Beta, new List<EegPackage>()}
            };

        private CalibrationService _calibrationService;
        private bool _isCalculatingBoundedAverage;

        private float _timeFocusStateLastCheck;
        public FocusState FocusState { get; private set; } = FocusState.Incorrect;
        public float AveragePzAlphaOverTheta { get; private set; }
        public float AveragePzBetaOverTheta { get; private set; }
        public float AveragePzBetaLOverAlpha { get; private set; }
        public float AveragePzBetaHOverAlpha { get; private set; }
        public float MovingAverageAlphaOverTheta { get; private set; }
        public float MovingAverageBetaOverAlpha { get; private set; }
        public DateTime StartTimeOfBoundAverageMeasurement { get; private set; }

        private void Awake() {
            Grid.GameEvents.OnStartMeasurement += () => {
                ClearBoundAverageData();
                StartTimeOfBoundAverageMeasurement = DateTime.Now;
                Debug.Log("StartMeasurement");
                _isCalculatingBoundedAverage = !_isCalculatingBoundedAverage;
            };

            Grid.GameEvents.OnEndMeasurement += () => {
                _isCalculatingBoundedAverage = !_isCalculatingBoundedAverage;
                Debug.Log("EndMeasurement");
                CalibrateUserParameters();
            };
        }

        private void CalibrateUserParameters() {
            _calibrationService = new CalibrationService(GetBoundEegDataRatio());
            if (_calibrationService.CalculateThresholds()) {
                Debug.Log("CALIBRATION WAS SUCCESSFUL");
                alphaOverThetaThreshold = _calibrationService.LowMediumThreshold.PzValue;
            }
            else {
                Debug.Log("CALIBRATION FAILED");
            }
        }

        private void Update() {
            CalculateTotalAverageEegValues();
            CalculateBoundedAverageEegValues();
            ReduceSizeOfRecentEegData();
        }

        private void CalculateTotalAverageEegValues() {
            var alpha = _recentEegDataOverBandPowerType[BandPowerType.Alpha];
            var theta = _recentEegDataOverBandPowerType[BandPowerType.Theta];
            var beta = _recentEegDataOverBandPowerType[BandPowerType.Beta];
            var betaL = _recentEegDataOverBandPowerType[BandPowerType.BetaL];
            var betaH = _recentEegDataOverBandPowerType[BandPowerType.BetaH];

            var alphaOverThetaList = GetListOfFractionsFromLists(alpha, theta);
            var betaOverAlphaList = GetListOfFractionsFromLists(beta, alpha);
            var betaLOverAlphaList = GetListOfFractionsFromLists(betaL, alpha);
            var betaHOverAlphaList = GetListOfFractionsFromLists(betaH, alpha);

            var smallestSizeFormLists =
                GetSmallestSizeFromLists(new List<List<EegPackage>> {alpha, theta, beta, betaL, betaH});

            TrimListsToSize(ref alpha, ref theta, ref beta, ref betaL, ref betaH, smallestSizeFormLists);

            AveragePzAlphaOverTheta = CalculateAverageEegValueFromList(alphaOverThetaList)?.PzValue ?? float.MinValue;
            AveragePzBetaOverTheta = CalculateAverageEegValueFromList(betaOverAlphaList)?.PzValue ?? float.MinValue;
            AveragePzBetaLOverAlpha = CalculateAverageEegValueFromList(betaLOverAlphaList)?.PzValue ?? float.MinValue;
            AveragePzBetaHOverAlpha = CalculateAverageEegValueFromList(betaHOverAlphaList)?.PzValue ?? float.MinValue;
        }

        private void CalculateBoundedAverageEegValues() {
            if (!_isCalculatingBoundedAverage) return;

            var alpha = _boundDataOverBandPowerType[BandPowerType.Alpha];
            var theta = _boundDataOverBandPowerType[BandPowerType.Theta];
            var beta = _boundDataOverBandPowerType[BandPowerType.Beta];
            var betaL = _boundDataOverBandPowerType[BandPowerType.BetaL];
            var betaH = _boundDataOverBandPowerType[BandPowerType.BetaH];

            var alphaOverThetaList = GetListOfFractionsFromLists(alpha, theta);
            var betaOverAlphaList = GetListOfFractionsFromLists(beta, alpha);
            var betaLOverAlphaList = GetListOfFractionsFromLists(betaL, alpha);
            var betaHOverAlphaList = GetListOfFractionsFromLists(betaH, alpha);

            boundAveragePackageOverBandPowerRatioType[BandPowerRatios.AlphaOverTheta] =
                CalculateAverageEegValueFromList(alphaOverThetaList) ?? new EegPackage();
            boundAveragePackageOverBandPowerRatioType[BandPowerRatios.BetaLOverAlpha] =
                CalculateAverageEegValueFromList(betaLOverAlphaList) ?? new EegPackage();
            boundAveragePackageOverBandPowerRatioType[BandPowerRatios.BetaHOverAlpha] =
                CalculateAverageEegValueFromList(betaHOverAlphaList) ?? new EegPackage();
            boundAveragePackageOverBandPowerRatioType[BandPowerRatios.BetaOverAlpha] =
                CalculateAverageEegValueFromList(betaOverAlphaList) ?? new EegPackage();
        }

        private void ReduceSizeOfRecentEegData() {
            var bandPowerTypes = Enum.GetValues(typeof(BandPowerType)).Cast<BandPowerType>().ToList();
            foreach (var bandPowerType in bandPowerTypes) {
                var element = _recentEegDataOverBandPowerType[bandPowerType];
                if (element.Count >= MAXIMAL_SIZE_OF_RECENT_EEG_DATA)
                    _recentEegDataOverBandPowerType[bandPowerType] = element.GetRange(
                        element.Count - eegAverageSampleSize - 1,
                        eegAverageSampleSize);
            }
        }

        public Dictionary<BandPowerRatios, List<EegPackage>> GetBoundEegDataRatio() {
            var boundDataOverBandPowerRatios =
                new Dictionary<BandPowerRatios, List<EegPackage>>();

            var alpha = _boundDataOverBandPowerType[BandPowerType.Alpha];
            var theta = _boundDataOverBandPowerType[BandPowerType.Theta];
            var beta = _boundDataOverBandPowerType[BandPowerType.Beta];
            var betaL = _boundDataOverBandPowerType[BandPowerType.BetaL];
            var betaH = _boundDataOverBandPowerType[BandPowerType.BetaH];

            var smallestSizeFormLists =
                GetSmallestSizeFromLists(new List<List<EegPackage>> {alpha, theta, beta, betaL, betaH});

            TrimListsToSize(ref alpha, ref theta, ref beta, ref betaL, ref betaH, smallestSizeFormLists);

            boundDataOverBandPowerRatios[BandPowerRatios.AlphaOverTheta] = GetListOfFractionsFromLists(alpha, theta);
            boundDataOverBandPowerRatios[BandPowerRatios.BetaLOverAlpha] = GetListOfFractionsFromLists(betaL, alpha);
            boundDataOverBandPowerRatios[BandPowerRatios.BetaHOverAlpha] = GetListOfFractionsFromLists(betaH, alpha);
            boundDataOverBandPowerRatios[BandPowerRatios.BetaOverAlpha] = GetListOfFractionsFromLists(beta, alpha);

            return boundDataOverBandPowerRatios;
        }

        private void ClearBoundAverageData() {
            foreach (var keyValuePair in _boundDataOverBandPowerType) keyValuePair.Value.Clear();
        }

        private static List<EegPackage> GetListOfFractionsFromLists(IReadOnlyList<EegPackage> numerator,
            IReadOnlyList<EegPackage> denominator) {
            var alphaOverThetaList = new List<EegPackage>();

            var size = numerator.Count > denominator.Count ? denominator.Count : numerator.Count;

            for (var i = 0; i < size; i++)
                if (denominator[i] != null && denominator[i] != 0 && numerator[i] != null)
                    alphaOverThetaList.Add(numerator[i] / denominator[i]);

            return alphaOverThetaList;
        }

        public static EegPackage CalculateAverageEegValueFromList(IReadOnlyCollection<EegPackage> eegPackages) {
            if (eegPackages.Count == 0) return null;
            var sum = new EegPackage();
            foreach (var eegPackage in eegPackages) sum += eegPackage;

            return sum / eegPackages.Count;
        }

        public FocusState GetFocusState() {
            var alpha = GetAverageBandPowerFromType(BandPowerType.Alpha).PzValue;
            var theta = GetAverageBandPowerFromType(BandPowerType.Theta).PzValue;

            if (alpha == 0 || theta == 0) FocusState = FocusState.Incorrect;

            MovingAverageAlphaOverTheta = alpha / theta;

            FocusState = MovingAverageAlphaOverTheta > alphaOverThetaThreshold ? FocusState.Low : FocusState.Medium;
            return FocusState;
        }

        private EegPackage GetAverageBandPowerFromType(BandPowerType type) {
            var size = _recentEegDataOverBandPowerType[type].Count;
            var value = new EegPackage();
            if (size >= eegAverageSampleSize)
                value = EegPackage.AverageEegPackages(_recentEegDataOverBandPowerType[type]
                    .GetRange(size - eegAverageSampleSize, eegAverageSampleSize));

            return value;
        }

        public void AddNewAlphaValue(EegPackage value) {
            _recentEegDataOverBandPowerType[BandPowerType.Alpha].Add(value);
            if (_isCalculatingBoundedAverage) _boundDataOverBandPowerType[BandPowerType.Alpha].Add(value);
        }

        public void AddNewThetaValue(EegPackage value) {
            _recentEegDataOverBandPowerType[BandPowerType.Theta].Add(value);
            if (_isCalculatingBoundedAverage) _boundDataOverBandPowerType[BandPowerType.Theta].Add(value);
        }

        public void AddNewBetaLValue(EegPackage value) {
            _recentEegDataOverBandPowerType[BandPowerType.BetaL].Add(value);
            if (_isCalculatingBoundedAverage) _boundDataOverBandPowerType[BandPowerType.BetaL].Add(value);
        }

        public void AddNewBetaHValue(EegPackage value) {
            _recentEegDataOverBandPowerType[BandPowerType.BetaH].Add(value);
            if (_isCalculatingBoundedAverage) _boundDataOverBandPowerType[BandPowerType.BetaH].Add(value);
        }

        public void AddNewBetaValue(EegPackage value) {
            _recentEegDataOverBandPowerType[BandPowerType.Beta].Add(value);
            if (_isCalculatingBoundedAverage) _boundDataOverBandPowerType[BandPowerType.Beta].Add(value);
        }

        private bool HasNotEnoughTimePassed() {
            _timeFocusStateLastCheck += Time.deltaTime;
            if (!(_timeFocusStateLastCheck > PERIOD_FOCUS_STATE_CHECK)) return true;
            _timeFocusStateLastCheck -= PERIOD_FOCUS_STATE_CHECK;
            return false;
        }

        public Dictionary<BandPowerType, List<EegPackage>> GetBoundEegDataForSavingToFile() {
            return _boundDataOverBandPowerType;
        }

        private static void TrimListsToSize(ref List<EegPackage> alpha, ref List<EegPackage> theta,
            ref List<EegPackage> beta, ref List<EegPackage> betaL, ref List<EegPackage> betaH, int size) {
            alpha = alpha?.GetRange(0, size);
            theta = theta?.GetRange(0, size);
            beta = beta?.GetRange(0, size);
            betaL = betaL?.GetRange(0, size);
            betaH = betaH?.GetRange(0, size);
        }

        public static int GetSmallestSizeFromLists(List<List<EegPackage>> lists) {
            var smallestSize = int.MaxValue;
            foreach (var list in lists)
                if (smallestSize > list.Count)
                    smallestSize = list.Count;

            return smallestSize;
        }
    }
}