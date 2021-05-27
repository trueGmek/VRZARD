using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utils.Graph {
    public class WindowGraph : MonoBehaviour {
        private const float X_OFFSET = 25;
        private const float Y_OFFSET = 10;
        private const float NUMBER_OF_DIVISORS = 11;
        public float yMax = 100f;
        public int numberOfPoints = 100;
        public string title = "Title", xUnitName = "xUnitName", yUnitName = "yUnitName";
        [SerializeField] private Sprite circleSprite;
        [HideInInspector] public List<float> valueList = new List<float>();
        private readonly List<GameObject> _dots = new List<GameObject>();
        private DataProcessing _dataProcessing;
        private RectTransform _graphContainer;
        private float _graphHeight;
        private Text _titleObject;
        private GameObject _xAxis, _yAxis;

        private float _xUnitValue;

        private void Awake() {
            _dataProcessing = GetComponent<DataProcessing>();
            _graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
            var sizeDelta = _graphContainer.sizeDelta;
            _graphHeight = sizeDelta.y - 50;
            _xUnitValue = (sizeDelta.x - 50) / numberOfPoints;
            for (var i = 0; i < numberOfPoints; i++) {
                var circleGameObject = new GameObject("circle", typeof(Image));
                _dots.Add(circleGameObject);
                circleGameObject.transform.SetParent(_graphContainer, false);
                circleGameObject.GetComponent<Image>().sprite = circleSprite;
                circleGameObject.GetComponent<Image>().useSpriteMesh = true;
                var rectTransform = circleGameObject.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(5, 5);
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);
                rectTransform.anchoredPosition = new Vector3(0, 0, -1);
            }

            _xAxis = new GameObject("xAxis", typeof(Image));
            _xAxis.transform.SetParent(_graphContainer, false);
            _xAxis.GetComponent<Image>().color = new Color(1, 1, 1, .5f);

            _yAxis = new GameObject("yAxis", typeof(Image));
            _yAxis.transform.SetParent(_graphContainer, false);
            _yAxis.GetComponent<Image>().color = new Color(1, 1, 1, .5f);
            DrawAxis();
            CreateDivisors();

            _titleObject = _graphContainer.Find("Title").GetComponent<Text>();

            _titleObject.text = title;
        }

        private void CreateDivisors() {
            var divisorDistancePx = _graphHeight / NUMBER_OF_DIVISORS;
            var divisorDistanceValues = yMax / NUMBER_OF_DIVISORS;
            for (var i = 1; i < NUMBER_OF_DIVISORS + 1; i++) {
                var divisorX = new GameObject("divisorX", typeof(Image));
                divisorX.transform.SetParent(_graphContainer, false);
                divisorX.GetComponent<Image>().color = new Color(1, 1, 1, .5f);
                var divisorXRectTransform = divisorX.GetComponent<RectTransform>();
                divisorXRectTransform.anchorMin = new Vector2(0, 0);
                divisorXRectTransform.anchorMax = new Vector2(0, 0);
                divisorXRectTransform.sizeDelta = new Vector2(15, 2);
                divisorXRectTransform.anchoredPosition = new Vector2(X_OFFSET, divisorDistancePx * i + Y_OFFSET);

                var divisorXTextGameObject = new GameObject("divisorXText", typeof(Text));
                var divisorXText = divisorXTextGameObject.GetComponent<Text>();
                divisorXText.text = $"{i * divisorDistanceValues}";
                divisorXText.font = Font.CreateDynamicFontFromOSFont("Roboto-Bold", 9);
                divisorXText.fontSize = 9;
                divisorXText.alignment = TextAnchor.MiddleCenter;
                divisorXTextGameObject.transform.SetParent(_graphContainer, false);
                var divisorXTextRectTransform = divisorXTextGameObject.GetComponent<RectTransform>();
                divisorXTextRectTransform.anchorMin = new Vector2(0, 0);
                divisorXTextRectTransform.anchorMax = new Vector2(0, 0);
                divisorXTextRectTransform.sizeDelta = new Vector2(22, 10);
                divisorXTextRectTransform.anchoredPosition =
                    new Vector2(X_OFFSET - 15, divisorDistancePx * i + Y_OFFSET);
            }
        }

        private void MoveValues() {
            var tempList = new List<float>(valueList);
            for (var i = 0; i < _dots.Count - 1; i++) tempList[i + 1] = valueList[i];

            valueList = tempList;
        }

        public void AddValue(float value) {
            if (valueList.Count >= numberOfPoints) {
                MoveValues();
                valueList.RemoveAt(0);
            }

            valueList.Insert(0, value);
            _dataProcessing.OnAddValue(value);
        }

        public void ShowGraph() {
            for (var i = 0; i < valueList.Count; i++) {
                var xPosition = i * _xUnitValue + X_OFFSET;
                var yPosition = valueList[i] / yMax * _graphHeight + Y_OFFSET;
                var rectTransform = _dots[i].GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector3(xPosition, yPosition, 0);
            }
        }

        private void DrawAxis() {
            var xAxisTransform = _xAxis.GetComponent<RectTransform>();
            xAxisTransform.anchorMin = new Vector2(0, 0);
            xAxisTransform.anchorMax = new Vector2(0, 0);
            xAxisTransform.sizeDelta = new Vector2(900, 2f);
            xAxisTransform.anchoredPosition = new Vector2(475, Y_OFFSET);

            var yAxisTransform = _yAxis.GetComponent<RectTransform>();
            yAxisTransform.anchorMin = new Vector2(0, 0);
            yAxisTransform.anchorMax = new Vector2(0, 0);
            yAxisTransform.sizeDelta = new Vector2(2, 500);
            yAxisTransform.anchoredPosition = new Vector2(X_OFFSET, 250 + Y_OFFSET);
        }

        private void DrawLineBetweenTwoPoints(Vector2 positionA, Vector2 positionB) {
            var imageGameObject = new GameObject("dotConnection", typeof(Image));
            imageGameObject.transform.SetParent(_graphContainer, false);
            imageGameObject.GetComponent<Image>().color = new Color(1, 1, 1, .5f);

            var rectTransform = imageGameObject.GetComponent<RectTransform>();
            var dir = (positionA - positionB).normalized;
            var distance = Vector2.Distance(positionA, positionB);

            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(100, 3f);
            rectTransform.anchoredPosition = positionA + dir * distance * .5f;
            rectTransform.localEulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan(dir.y / dir.x));
        }
    }
}