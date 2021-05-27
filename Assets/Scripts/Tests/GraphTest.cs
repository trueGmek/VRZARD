using System.Collections.Generic;
using UnityEngine;
using Utils.Graph;

namespace Tests {
    public class GraphTest : MonoBehaviour {
        public List<WindowGraph> graphs = new List<WindowGraph>();
        private int _dir = 1;
        private int _value;

        private void Update() {
            _value += _dir;
            foreach (var windowGraph in graphs) {
                windowGraph.AddValue(_value);
                windowGraph.ShowGraph();
            }

            if (_value >= 200) _dir = -1;
            if (_value <= 0) _dir = 1;
        }
    }
}