using System;
using System.Collections;
using UnityEngine;

namespace Drone.Random.Editor
{
    public static class PrngDebugDrawing
    {
        private static Texture2D _aaLineTex;
        private static Texture2D _lineTex;

        private static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width, bool antiAlias)
        {
            Color savedColor = GUI.color;
            Matrix4x4 savedMatrix = GUI.matrix;

            if (!_lineTex) {
                _lineTex = new Texture2D(1, 1, TextureFormat.ARGB32, true);
                _lineTex.SetPixel(0, 1, Color.white);
                _lineTex.Apply();
            }
            if (!_aaLineTex) {
                _aaLineTex = new Texture2D(1, 3, TextureFormat.ARGB32, true);
                _aaLineTex.SetPixel(0, 0, new Color(1, 1, 1, 0));
                _aaLineTex.SetPixel(0, 1, Color.white);
                _aaLineTex.SetPixel(0, 2, new Color(1, 1, 1, 0));
                _aaLineTex.Apply();
            }
            if (antiAlias) {
                width *= 3;
            }
            float angle = Vector3.Angle(pointB - pointA, Vector2.right) * (pointA.y <= pointB.y ? 1 : -1);
            float m = (pointB - pointA).magnitude;
            if (m > 0.01f) {
                Vector3 dz = new Vector3(pointA.x, pointA.y, 0);

                GUI.color = color;
                GUI.matrix = TranslationMatrix(dz) * GUI.matrix;
                GUIUtility.ScaleAroundPivot(new Vector2(m, width), new Vector3(-0.5f, 0, 0));
                GUI.matrix = TranslationMatrix(-dz) * GUI.matrix;
                GUIUtility.RotateAroundPivot(angle, Vector2.zero);
                GUI.matrix = TranslationMatrix(dz + new Vector3(width / 2, -m / 2) * Mathf.Sin(angle * Mathf.Deg2Rad)) * GUI.matrix;

                GUI.DrawTexture(new Rect(0, 0, 1, 1), !antiAlias ? _lineTex : _aaLineTex);
            }
            GUI.matrix = savedMatrix;
            GUI.color = savedColor;
        }

        private static Matrix4x4 TranslationMatrix(Vector3 v)
        {
            return Matrix4x4.TRS(v, Quaternion.identity, Vector3.one);
        }

        public static void DrawPoints(ArrayList rlist, PrngDebugWindow.MersenneWindowOptionsType op, int width, int height)
        {
            int counter = 0;

            foreach (object obj in rlist) {
                counter++;

                // 1. make a Cross around zero
                Vector2 pointA = new Vector2(0.0f, -1.0f);
                Vector2 pointB = new Vector2(0.0f, 1.0f);

                Vector2 pointC = new Vector2(1.0f, 0.0f);
                Vector2 pointD = new Vector2(-1.0f, 0.0f);

                // 2. move the cross into place

                // Y: value
                // X: Position	

                float myy;
                switch (op) {
                    case PrngDebugWindow.MersenneWindowOptionsType.INT:
                        myy = (Convert.ToSingle(obj) / int.MaxValue);
                        break;
                    case PrngDebugWindow.MersenneWindowOptionsType.FLOAT:
                        myy = Convert.ToSingle(obj);
                        break;
                    default:
                        myy = 0;
                        break;
                }

                float posy = myy * height;
                float posx = counter * (Convert.ToSingle(width) / rlist.Count);

                Vector2 myScalingFactor = new Vector2(posx, posy);

                Vector2 pointAs = pointA + myScalingFactor;
                Vector2 pointBs = pointB + myScalingFactor;

                Vector2 pointCs = pointC + myScalingFactor;
                Vector2 pointDs = pointD + myScalingFactor;

                DrawLine(pointAs, pointBs, Color.blue, 1.0f, true);
                DrawLine(pointCs, pointDs, Color.blue, 1.0f, true);
            }
        }
    }
}