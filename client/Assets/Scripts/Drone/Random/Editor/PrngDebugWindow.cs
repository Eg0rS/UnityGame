using System.Collections;
using System.Linq;
using Drone.Random.MersenneTwister;
using UnityEditor;
using UnityEngine;
using static System.Int32;

namespace Drone.Random.Editor
{
    public class PrngDebugWindow : EditorWindow
    {
        public enum MersenneWindowOptionsType
        {
            INT = 0,
            FLOAT = 1,
        }

        private MTRandomGenerator _randomGenerator;
        private int _samplingSize = 500;
        private float _temperature = 5.0f;
        private ArrayList _randomList;
        private int _seed = 0;
        private MersenneWindowOptionsType _op = MersenneWindowOptionsType.FLOAT;
        private bool _normalizeToggle = false;

        [MenuItem("Tortuga/PrngDebug")]
        private static void Init()
        {
            PrngDebugWindow window = (PrngDebugWindow) GetWindowWithRect(typeof(PrngDebugWindow), new Rect(0, 0, 420, 600));
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 400, 400));
            GUILayout.Box("RANDOM NUMBER DISTRIBUTION", GUILayout.Width(400), GUILayout.Height(400));
            if (_randomList != null && _randomList.Count > 0) {
                PrngDebugDrawing.DrawPoints(_randomList, _op, 400, 400);
            }
            GUILayout.EndArea();
            if (_randomList != null) {
                string min = _randomList.ToArray().Min().ToString();
                string max = _randomList.ToArray().Max().ToString();

                GUILayout.BeginArea(new Rect(10, 420, 200, 20));
                GUILayout.Label(min);
                GUILayout.EndArea();

                GUILayout.BeginArea(new Rect(410 - (max.Length * 8), 420, 200, 20));
                GUILayout.Label(max);
                GUILayout.EndArea();
            }

            GUILayout.BeginArea(new Rect(10, 440, 400, 200));
            _seed = EditorGUILayout.IntSlider("Seed:", _seed, MinValue, MaxValue);
            _op = (MersenneWindowOptionsType) EditorGUILayout.EnumPopup("Type:", _op);
            _samplingSize = EditorGUILayout.IntSlider("#N", _samplingSize, 1, 1000);
            _normalizeToggle = EditorGUILayout.Toggle("Normalize", _normalizeToggle);

            if (_normalizeToggle) {
                _temperature = EditorGUILayout.Slider("Temp", _temperature, 0.0f, 10.0f);
            }

            if (GUILayout.Button("Generate Random Numbers")) {
                Sample();
            }
            GUILayout.EndArea();
        }

        private void Sample()
        {
            Debug.Log("GENERATING RANDOM NUMBERS WITH SEED: " + _seed);
            _randomGenerator = new MTRandomGenerator(RandomSeedGenerator.Crypto());
            _randomList = new ArrayList();
            for (int i = 0; i < _samplingSize; i++) {
                double rn = 0;

                switch (_op) {
                    case MersenneWindowOptionsType.INT:
                        rn = _randomGenerator.GetInt();
                        break;

                    case MersenneWindowOptionsType.FLOAT:
                        rn = _randomGenerator.GetFloat();
                        break;
                }

                double eval = _normalizeToggle ? UnityNormalDistribution.ToNormalDistribution(rn, _temperature) : rn;

                _randomList.Add(eval);
            }
            _randomList.Sort();
            _randomList.Reverse();
            Repaint();
        }
    }
}