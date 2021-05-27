using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace EEG {
    //P.S: LAST REFACTOR 11.03.21
    public class EegDataExporter : MonoBehaviour {
        private const string FILE_EXTENSTION = ".csv";

        private const string SEPARATOR = ";";
        private const string NEW_LINE = "\n";
        private const string ALL_CHANNELS_NAMES_SEPARATED = @"AF3" + SEPARATOR + "PZ" + SEPARATOR + "T8" + SEPARATOR;
        private const string FIVE_SEPARATORS = SEPARATOR + SEPARATOR + SEPARATOR + SEPARATOR + SEPARATOR;
        private const string HEADER_EEG_DATA = @"alpha" + FIVE_SEPARATORS + "theta" + FIVE_SEPARATORS + "beta";
        private const string HEADER_EEG_RATIOS = @"alpha/theta" + FIVE_SEPARATORS + "beta/alpha";

        private const string HEADER_DATA = ALL_CHANNELS_NAMES_SEPARATED +
                                           ALL_CHANNELS_NAMES_SEPARATED +
                                           ALL_CHANNELS_NAMES_SEPARATED;

        private const string HEADER_RATIOS = ALL_CHANNELS_NAMES_SEPARATED +
                                             ALL_CHANNELS_NAMES_SEPARATED;

        public string filePath = @"C:\UNITY_PROJECT\VR - Bachelor thesis\Assets\";
        public string fileNameEegData = @"TEMP";
        public string fileNameEegRatios = @"TEMP";

        private DataProcessing _dataProcessing;

        private void Awake() {
            _dataProcessing = GetComponent<DataProcessing>();
        }

        public void ExportEegDataToFile() {
            ExportEegPackageListToFile(_dataProcessing.GetBoundEegDataForSavingToFile());
            ExportEegPackageRatioListToFile(_dataProcessing.GetBoundEegDataRatio());
        }

        private void ExportEegPackageListToFile(
            Dictionary<BandPowerType, List<EegPackage>> eegPackagesOverBandPowerData) {
            var writer = new StreamWriter(filePath + fileNameEegData + FILE_EXTENSTION);
            var parsedData = CreateStringFromEegPackagesData(eegPackagesOverBandPowerData);
            writer.WriteLine(parsedData);
            writer.Close();
        }

        private string CreateStringFromEegPackagesData(
            Dictionary<BandPowerType, List<EegPackage>> eegPackagesOverBandPowerData) {
            var sb = new StringBuilder();

            sb.Append(HEADER_EEG_DATA).Append(NEW_LINE);
            sb.Append(HEADER_DATA).Append(NEW_LINE);

            var alpha = eegPackagesOverBandPowerData[BandPowerType.Alpha];
            var theta = eegPackagesOverBandPowerData[BandPowerType.Theta];
            var beta = eegPackagesOverBandPowerData[BandPowerType.Beta];

            var numberOfSamples =
                DataProcessing.GetSmallestSizeFromLists(new List<List<EegPackage>> {alpha, theta, beta});

            for (var i = 0; i < numberOfSamples; i++) {
                sb.Append(alpha[i].ToString(SEPARATOR)).Append(SEPARATOR);
                sb.Append(theta[i].ToString(SEPARATOR)).Append(SEPARATOR);
                sb.Append(beta[i].ToString(SEPARATOR)).Append(SEPARATOR);
                sb.Append(CalculateTimespan(beta[i]).ToString(CultureInfo.InvariantCulture))
                    .Append(NEW_LINE);
            }

            return sb.ToString();
        }

        private double CalculateTimespan(EegPackage package) {
            return (package.GetTimestamp() - _dataProcessing.StartTimeOfBoundAverageMeasurement).TotalSeconds;
        }

        private void ExportEegPackageRatioListToFile(
            Dictionary<BandPowerRatios, List<EegPackage>> eegPackagesOverBandPowerRatio) {
            var writer = new StreamWriter(filePath + fileNameEegRatios + FILE_EXTENSTION);
            var parsedData = CreateStringFromEegRatioData(eegPackagesOverBandPowerRatio);
            writer.WriteLine(parsedData);
            writer.Close();
        }

        private string CreateStringFromEegRatioData(
            Dictionary<BandPowerRatios, List<EegPackage>> eegPackagesOverBandPowerRatio) {
            var sb = new StringBuilder();
            sb.Append(HEADER_EEG_RATIOS).Append(NEW_LINE);
            sb.Append(HEADER_RATIOS).Append(NEW_LINE);

            var alphaOverTheta = eegPackagesOverBandPowerRatio[BandPowerRatios.AlphaOverTheta];
            var betaOverAlpha = eegPackagesOverBandPowerRatio[BandPowerRatios.BetaOverAlpha];

            var numberOfSamples = DataProcessing.GetSmallestSizeFromLists(new List<List<EegPackage>> {
                alphaOverTheta, betaOverAlpha
            });

            for (var i = 0; i < numberOfSamples; i++) {
                sb.Append(alphaOverTheta[i].ToString(SEPARATOR))
                    .Append(SEPARATOR);
                sb.Append(betaOverAlpha[i].ToString(SEPARATOR))
                    .Append(SEPARATOR);
                sb.Append(CalculateTimespan(betaOverAlpha[i])
                    .ToString(CultureInfo.InvariantCulture)).Append(NEW_LINE);
            }

            return sb.ToString();
        }
    }
}