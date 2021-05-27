using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EEG {
    public class EegPackage {
        private const float TOLERANCE = 0.001f;
        private readonly DateTime _timestamp;
        public float Af3Value;
        public float Af4Value;
        public float PzValue;
        public float T7Value;
        public float T8Value;

        public EegPackage() {
            Af4Value = 0;
            Af3Value = 0;
            T7Value = 0;
            T8Value = 0;
            PzValue = 0;
            _timestamp = DateTime.Now;
        }

        public EegPackage(DateTime timestamp) {
            Af4Value = 0;
            Af3Value = 0;
            T7Value = 0;
            T8Value = 0;
            PzValue = 0;
            _timestamp = timestamp;
        }

        public static bool operator ==(EegPackage a, int b) {
            return Math.Abs(a.Af3Value - b) < TOLERANCE ||
                   Math.Abs(a.Af4Value - b) < TOLERANCE ||
                   Math.Abs(a.T7Value - b) < TOLERANCE ||
                   Math.Abs(a.T8Value - b) < TOLERANCE ||
                   Math.Abs(a.PzValue - b) < TOLERANCE;
        }

        public static bool operator !=(EegPackage a, int b) {
            return !(a == b);
        }

        public static EegPackage operator +(EegPackage a, EegPackage b) {
            return new EegPackage(a._timestamp) {
                T7Value = a.T7Value + b.T7Value,
                Af3Value = a.Af3Value + b.Af3Value,
                Af4Value = a.Af4Value + b.Af4Value,
                T8Value = a.T8Value + b.T8Value,
                PzValue = a.PzValue + b.PzValue
            };
        }

        public static EegPackage operator /(EegPackage a, int b) {
            if (b == 0) throw new DivideByZeroException();
            return new EegPackage(a._timestamp) {
                T7Value = a.T7Value / b,
                Af3Value = a.Af3Value / b,
                Af4Value = a.Af4Value / b,
                T8Value = a.T8Value / b,
                PzValue = a.PzValue / b
            };
        }

        public static EegPackage operator /(EegPackage a, EegPackage b) {
            if (b.Af3Value == 0 || b.Af4Value == 0 || b.PzValue == 0 || b.T7Value == 0 || b.T8Value == 0)
                throw new DivideByZeroException();
            return new EegPackage(a._timestamp) {
                T7Value = a.T7Value / b.T7Value,
                Af3Value = a.Af3Value / b.Af3Value,
                Af4Value = a.Af4Value / b.Af4Value,
                T8Value = a.T8Value / b.T8Value,
                PzValue = a.PzValue / b.PzValue
            };
        }

        public static EegPackage AverageEegPackages(List<EegPackage> values) {
            var sum = new EegPackage();
            foreach (var package in values) sum += package;

            return sum / values.Count;
        }

        public override string ToString() {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(Af3Value).Append(",");
            stringBuilder.Append(Af4Value).Append(",");
            stringBuilder.Append(PzValue).Append(",");
            stringBuilder.Append(T7Value).Append(",");
            stringBuilder.Append(T8Value);
            return stringBuilder.ToString();
        }

        public string ToString(string separator) {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(Af3Value.ToString(CultureInfo.InvariantCulture)).Append(separator);
            stringBuilder.Append(Af4Value.ToString(CultureInfo.InvariantCulture)).Append(separator);
            stringBuilder.Append(PzValue.ToString(CultureInfo.InvariantCulture)).Append(separator);
            stringBuilder.Append(T7Value.ToString(CultureInfo.InvariantCulture)).Append(separator);
            stringBuilder.Append(T8Value.ToString(CultureInfo.InvariantCulture));
            return stringBuilder.ToString();
        }

        public DateTime GetTimestamp() {
            return _timestamp;
        }
    }
}