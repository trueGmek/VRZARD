using System.Collections.Generic;

namespace EEG {
    public class CalibrationService {
        private const int NUMBER_OF_SAMPLES_PER_SECOND = 2;

        private const int TIME_OF_START_LOW_MEASUREMENT = 0;
        private const int TIME_OF_END_LOW_MEASUREMENT = 60;

        private const int TIME_OF_START_MEDIUM_MEASUREMENT = 80;
        private const int TIME_OF_END_MEDIUM_MEASUREMENT = 140;

        private const int TIME_OF_START_HIGH_MEASUREMENT = 160;
        private const int TIME_OF_END_HIGH_MEASUREMENT = 218;
        private readonly Dictionary<BandPowerRatios, List<EegPackage>> _calibrationSamples;

        private readonly Dictionary<BandPowerRatios, List<EegPackage>> _lowSamples;
        private readonly Dictionary<BandPowerRatios, List<EegPackage>> _mediumSamples;

        public EegPackage LowMediumThreshold;

        public CalibrationService(Dictionary<BandPowerRatios, List<EegPackage>> calibrationSamples) {
            _calibrationSamples = calibrationSamples;
            _lowSamples = ExtractLowSamples();
            _mediumSamples = ExtractMediumSamples();
        }

        public bool CalculateThresholds() {
            return CalculateLowMediumThreshold();
        }

        private bool CalculateLowMediumThreshold() {
            var lowAverage =
                DataProcessing.CalculateAverageEegValueFromList(_lowSamples[BandPowerRatios.AlphaOverTheta]);
            var mediumAverage =
                DataProcessing.CalculateAverageEegValueFromList(_mediumSamples[BandPowerRatios.AlphaOverTheta]);

            LowMediumThreshold = (lowAverage + mediumAverage) / 2;
            return true;
        }

        private Dictionary<BandPowerRatios, List<EegPackage>> ExtractHighSamples() {
            return ExtractSamplesInPeriod(TIME_OF_START_HIGH_MEASUREMENT, TIME_OF_END_HIGH_MEASUREMENT);
        }

        private Dictionary<BandPowerRatios, List<EegPackage>> ExtractMediumSamples() {
            return ExtractSamplesInPeriod(TIME_OF_START_MEDIUM_MEASUREMENT, TIME_OF_END_MEDIUM_MEASUREMENT);
        }

        private Dictionary<BandPowerRatios, List<EegPackage>> ExtractLowSamples() {
            return ExtractSamplesInPeriod(TIME_OF_START_LOW_MEASUREMENT, TIME_OF_END_LOW_MEASUREMENT);
        }

        private Dictionary<BandPowerRatios, List<EegPackage>> ExtractSamplesInPeriod(int start, int end) {
            var extractedSamples = new Dictionary<BandPowerRatios, List<EegPackage>>();
            var startSampleIndex = start * NUMBER_OF_SAMPLES_PER_SECOND;
            var endSampleIndex = end * NUMBER_OF_SAMPLES_PER_SECOND - 1;
            var numberOfSamplesFromStartToEnd = endSampleIndex - startSampleIndex;
            extractedSamples[BandPowerRatios.AlphaOverTheta] = _calibrationSamples[BandPowerRatios.AlphaOverTheta]
                .GetRange(startSampleIndex, numberOfSamplesFromStartToEnd);
            extractedSamples[BandPowerRatios.BetaOverAlpha] = _calibrationSamples[BandPowerRatios.BetaOverAlpha]
                .GetRange(startSampleIndex, numberOfSamplesFromStartToEnd);

            return extractedSamples;
        }
    }
}