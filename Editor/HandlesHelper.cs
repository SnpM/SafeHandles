//=======================================================================
// Copyright (c) 2015 John Pan
// Distributed under the MIT License.
// (See accompanying file LICENSE or copy at
// http://opensource.org/licenses/MIT)
//=======================================================================

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace SafeHandles
{
    public static class HandlesHelper
    {
        static HandlesHelper()
        {
            SceneView.onSceneGUIDelegate += OnScene;
        }

        static readonly Dictionary<string,HandleInfo> HandleBuffers = new Dictionary<string, HandleInfo>(100);

        private static TInfo ActivateHandle<TInfo>(string id) where TInfo : HandleInfo, new()
        {
            HandleInfo info;
            if (!HandleBuffers.TryGetValue(id, out info))
            {
                info = new TInfo();
                HandleBuffers.Add(id, info);
            }
            info.Active = true;
            TInfo tInfo = info as TInfo;
            if (tInfo == null)
                throw new System.ArgumentException("Buffered handle is not a position handle!");
            return tInfo;
        }

        public static Vector3 PositionHandle(string id, Vector3 pos, Quaternion rot)
        {
            var posInfo = ActivateHandle<PositionHandleInfo>(id);
            if (posInfo.Changed == false)
            {
                posInfo.rotation = rot;
                posInfo._vector3Value = pos;
            } else
            {
                posInfo.Changed = false;
            }
            posInfo.Update();
            return posInfo.Vector3Value;
        }

        public static void LabelHandle(string id, string label)
        {
            var info = ActivateHandle<LabelHandleInfo>(id);
            if (info.Changed == false)
            {
                info.label = new GUIContent(label);
            }
            info.Update();
        }

        static void OnScene(SceneView view)
        {
            foreach (HandleInfo info in HandleBuffers.Values)
            {
                if (info.Active)
                    info.Draw();
            }
        }

    #region Info types
        private abstract class HandleInfo
        {
            public bool Active;
            public int ID;

            public bool Changed { get; set; }

            public void Draw()
            {

                OnDraw();
            }

            public void Update()
            {
                Changed = false;
            }

            protected abstract void OnDraw();
        }

        private sealed class PositionHandleInfo : HandleInfo
        {
            public Quaternion rotation;
            public Vector3 _vector3Value;

            public Vector3 Vector3Value { get { return _vector3Value; } }

            protected override void OnDraw()
            {
                Vector3 lastValue = _vector3Value;
                _vector3Value = Handles.PositionHandle(_vector3Value, rotation);
                if (lastValue != _vector3Value)
                    Changed = true;
            }
        }

        private sealed class LabelHandleInfo : HandleInfo
        {
            public Vector3 position;
            public GUIContent label;

            protected override void OnDraw()
            {
                Handles.Label(position, label);
            }
        }
    #endregion
    }
}