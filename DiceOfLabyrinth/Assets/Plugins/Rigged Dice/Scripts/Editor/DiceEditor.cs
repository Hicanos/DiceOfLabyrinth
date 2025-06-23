using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PredictedDice.Editor
{
    [CustomEditor(typeof(Dice))]
    public class DiceEditor : UnityEditor.Editor
    {
        enum SelectionType
        {
            Face,
            Vertex,
            Edge,
            FreeForm,
        }

        private Dice _dice;
        private Mesh _mesh;
        private bool _isEditingFaces;
        private SelectionType _selectionType;
        Dictionary<Vector3, Vector3> normalToCenter = new Dictionary<Vector3, Vector3>();
        Dictionary<Vector3, int> normalToCount = new Dictionary<Vector3, int>();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            using (new EditorGUI.DisabledScope(Application.isPlaying))
            {
                GUIStyle editButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 16,
                    fontStyle = FontStyle.Bold,
                    richText = true
                };
                if (GUILayout.Button(_isEditingFaces ? "Save" : "Edit Faces", editButtonStyle))
                {
                    _isEditingFaces = !_isEditingFaces;
                    if (_isEditingFaces && Selection.activeGameObject &&
                        Selection.activeGameObject.TryGetComponent<MeshFilter>(out var meshFilter))
                    {
                        _mesh = meshFilter.sharedMesh;
                    }

                    if (!_isEditingFaces)
                    {
                        EditorUtility.SetDirty(_dice);
                        AssetDatabase.SaveAssets();
                    }

                    SceneView.RepaintAll();
                }

                if (_isEditingFaces)
                {
                    _selectionType = (SelectionType)EditorGUILayout.EnumPopup("Selection Type", _selectionType);
                }
            }
        }

        private void OnEnable()
        {
            if (Application.isPlaying) return;
            _dice = (Dice)target;
            if (_dice.TryGetComponent<MeshFilter>(out var meshFilter))
                _mesh = meshFilter.sharedMesh;
            else
                Debug.LogError("MeshFilter not found in the Dice");
        }

        private void OnSceneGUI()
        {
            if (!_mesh) return;
            DiceFaceSelectionEditor();
        }

        private void DiceFaceSelectionEditor()
        {
            if (!_mesh) return;
            if (!_isEditingFaces) return;
            Graphics.DrawMeshNow(_mesh, _dice.transform.position, _dice.transform.rotation);
            var previousMatrix = Handles.matrix;
            Handles.matrix = _dice.transform.localToWorldMatrix;
            DrawEdges();
            switch (_selectionType)
            {
                case SelectionType.Vertex:
                    EditVertices();
                    break;
                case SelectionType.Edge:
                    EditEdges();
                    break;
                case SelectionType.Face:
                    EditFaces();
                    break;
                case SelectionType.FreeForm:
                    EditFreeForm();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Handles.matrix = previousMatrix;
        }

        private void EditFreeForm()
        {
            //Ray from SceneView camera to mouse position
            Ray camRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            // For debug purposes
            if (Physics.Raycast(camRay, out var hitDebug, float.MaxValue, -1, QueryTriggerInteraction.Ignore))
            {
                Handles.matrix = Matrix4x4.identity;
                if (DrawButton(hitDebug.point, "Free Form",
                        _dice.faceMap.Faces.Any(x => x.faceDirection == hitDebug.normal)))
                {
                    OpenWindowForFaceIndexSet((faceIndex) =>
                    {
                        _dice.faceMap.AddFace(new Face(faceIndex, hitDebug.point - hitDebug.transform.position));
                    });
                }

                Handles.matrix = _dice.transform.localToWorldMatrix;
                SceneView.RepaintAll();
            }
        }


        private void EditVertices()
        {
            foreach (var t in _mesh.vertices)
            {
                if (DrawButton(t, "Vertex", _dice.faceMap.Faces.Any(x => x.faceDirection == t)))
                {
                    OpenWindowForFaceIndexSet((faceIndex) => { _dice.faceMap.AddFace(new Face(faceIndex, t)); });
                }
            }
        }

        private void EditEdges()
        {
            for (int i = 0; i < _mesh.triangles.Length; i += 3)
            {
                int index1 = _mesh.triangles[i];
                int index2 = _mesh.triangles[i + 1];
                int index3 = _mesh.triangles[i + 2];

                Vector3 vertex1 = _mesh.vertices[index1];
                Vector3 vertex2 = _mesh.vertices[index2];
                Vector3 vertex3 = _mesh.vertices[index3];

                Vector3 center = (vertex1 + vertex2 + vertex3) / 3;
                if (DrawButton(center, "Edge " + i / 3, _dice.faceMap.Faces.Any(x => x.faceDirection == center)))
                {
                    OpenWindowForFaceIndexSet((faceIndex) => { _dice.faceMap.AddFace(new Face(faceIndex, center)); });
                }
            }
        }

        private void EditFaces()
        {
            if (normalToCenter.Count == 0)
                GetFaceDictionaries();
            foreach (var kvp in normalToCenter)
            {
                var combinedCenter = kvp.Value / normalToCount[kvp.Key];
                if (DrawButton(combinedCenter, "Face " + kvp.Key,
                        _dice.faceMap.Faces.Any(x => x.faceDirection == kvp.Key)))
                {
                    OpenWindowForFaceIndexSet((faceIndex) => { _dice.faceMap.AddFace(new Face(faceIndex, kvp.Key)); });
                }
            }
        }

        private void GetFaceDictionaries()
        {
            for (int i = 0; i < _mesh.triangles.Length / 3; i++)
            {
                var meshTriangle = i * 3;
                var triangle = new Vector3[3];
                for (int y = 0; y < 3; y++)
                {
                    triangle[y] = _mesh.vertices[_mesh.triangles[meshTriangle + y]];
                }

                var center = (triangle[0] + triangle[1] + triangle[2]) / 3;
                var normal = Vector3.Cross(triangle[1] - triangle[0], triangle[2] - triangle[0]).normalized;
                if (!normalToCenter.ContainsKey(normal))
                {
                    normalToCenter[normal] = Vector3.zero;
                    normalToCount[normal] = 0;
                }

                normalToCenter[normal] += center;
                normalToCount[normal]++;
            }
        }

        private void DrawEdges()
        {
            Handles.color = Color.black;
            for (int i = 0; i < _mesh.triangles.Length; i += 3)
            {
                int index1 = _mesh.triangles[i];
                int index2 = _mesh.triangles[i + 1];
                int index3 = _mesh.triangles[i + 2];

                Vector3 vertex1 = _mesh.vertices[index1];
                Vector3 vertex2 = _mesh.vertices[index2];
                Vector3 vertex3 = _mesh.vertices[index3];

                Handles.DrawLine(vertex1, vertex2);
                Handles.DrawLine(vertex2, vertex3);
                Handles.DrawLine(vertex3, vertex1);
            }

            Handles.color = Color.white;
        }

        private void OpenWindowForFaceIndexSet(Action<int> onComplete)
        {
            var window = EditorWindow.GetWindow<FaceIndexSetWindow>("Face Data");
            window.minSize = new Vector2(250, 100);
            window.maxSize = new Vector2(250, 100);
            window.OnClosed = (faceIndex) => { onComplete?.Invoke(faceIndex); };
        }

        private bool DrawButton(Vector3 position, string text, bool isFaceExist)
        {
            if (isFaceExist)
            {
                Handles.color = Color.green;
            }
            else
            {
                Handles.color = Color.white;
            }

            return Handles.Button(position, Quaternion.identity, .1f, .1f, Handles.SphereHandleCap) &&
                   EditorUtility.DisplayDialog("Face Index", text, "Set", "Cancel");
        }

        [DrawGizmo(GizmoType.Selected, typeof(Dice))]
        private static void DrawGizmo(Dice target, GizmoType gizmoType)
        {
            var targetTransform = target.transform;
            var diceGraphic = target.DiceGraphic;
            var simulation = target.Simulation;
            if (target.faceMap == null) return;
            DrawGizmos(target.faceMap.Faces,
                diceGraphic == null ? targetTransform.localToWorldMatrix : diceGraphic.transform.localToWorldMatrix,
                simulation == null ? -1 : simulation.OutCome(), target.RollData);
        }

        private static void DrawGizmos(List<Face> faces, Matrix4x4 matrix4X4, int outCome, RollData data)
        {
            for (int i = 0; i < faces.Count; i++)
            {
                Handles.matrix = matrix4X4;
                Color textColor = faces[i].faceValue == outCome ? Color.red : Color.black;
                if (!data.Equals(RollData.Default))
                {
                    textColor = faces[i].faceValue == data.faceValue ? Color.green : textColor;
                }

                GUIStyle style = new GUIStyle
                {
                    fontSize = 20,
                    fontStyle = FontStyle.Bold,
                    richText = true,
                    normal =
                    {
                        textColor = textColor
                    }
                };
                Handles.Label(faces[i].faceDirection, faces[i].faceValue.ToString(), style);
            }
        }
    }

    internal class FaceIndexSetWindow : EditorWindow
    {
        private bool changed;
        private int faceIndex = 0;
        public Action<int> OnClosed;

        public void OnGUI()
        {
            GUILayout.Label("Face Index Set");
            EditorGUI.BeginChangeCheck();
            faceIndex = EditorGUILayout.IntField("Face Index", faceIndex);
            if (EditorGUI.EndChangeCheck())
            {
                changed = true;
            }

            using (new EditorGUI.DisabledScope(!changed))
            {
                if (GUILayout.Button("Set"))
                {
                    OnClosed?.Invoke(faceIndex);
                    Close();
                }
            }
        }
    }
}