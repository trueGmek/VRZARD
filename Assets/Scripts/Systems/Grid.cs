using UnityEngine;
using UnityEngine.Rendering;

namespace Systems {
    public static class Grid {
        public static readonly GameEvents GameEvents;
        public static readonly Volume Volume;
        public static readonly AudioManager AudioManager;
        public static readonly GameManager GameManager;
        public static readonly DataExporter DataExporter;

        static Grid() {
            var app = SafeFind("__app");
            GameEvents = (GameEvents) SafeComponent(app, "GameEvents");
            Volume = (Volume) SafeComponent(app, "Volume");
            AudioManager = (AudioManager) SafeComponent(app, "AudioManager");
            GameManager = (GameManager) SafeComponent(app, "GameManager");
            DataExporter = (DataExporter) SafeComponent(app, "DataExporter");
        }

        private static GameObject SafeFind(string gameObjectName) {
            var g = GameObject.Find(gameObjectName);
            if (g == null) Woe("GameObject " + gameObjectName + "  not on _preload.");
            return g;
        }

        private static Component SafeComponent(GameObject app, string componentName) {
            var c = app.GetComponent(componentName);
            if (c == null) Woe("Component " + componentName + " not on _preload.");
            return c;
        }

        private static void Woe(string error) {
            Debug.Log(">>> Cannot proceed... " + error);
            Debug.Log(">>> It is very likely you just forgot to launch");
            Debug.Log(">>> from scene zero, the _preload scene.");
        }

        // be sure to read this:
        // http://stackoverflow.com/a/35891919/294884
    }
}