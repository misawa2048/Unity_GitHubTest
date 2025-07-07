using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Constellation
{
#if UNITY_EDITOR
    [CustomEditor(typeof(PipelineSelector))]
    public class PipelineSelectorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Open \"Project Settings/Graphics\""))
            {
                SettingsService.OpenProjectSettings("Project/Graphics");
            }
            if (GUILayout.Button("Open \"Project Settings/Quality\""))
            {
                SettingsService.OpenProjectSettings("Project/Quality");
            }
        }
    }
#endif

    [DefaultExecutionOrder(-10)]
    [HelpURL("https://docs.unity3d.com/ja/2021.2/Manual/srp-setting-render-pipeline-asset.html")]
    public class PipelineSelector : MonoBehaviour
    {
        public enum PipelineType
        {
            None,
            Builtin,
            URP,
        }
        [SerializeField] PipelineType m_startPipeline;
        [SerializeField] RenderPipelineAsset m_URPAsset;
        [SerializeField] bool m_isChangable;
        [SerializeField] bool m_useBeareMessage = true;
        [SerializeField, Multiline(4)] string m_bewareMessage = "<color=#FFFF00>BEWARE! YOU MUST SET</color> " +
#if UNITY_6000_0_OR_NEWER
                    "<b>PorjectSettings >Graphics > Default Render Pipeline</b> to <color=#00FFFF>{URP_ASSET_NAME}</color> " +
#else
                    "<b>PorjectSettings >Graphics > Scriptable Render Pipeline Settings</b> to <color=#00FFFF>{URP_ASSET_NAME}</color> " +
#endif
                    "and <b>PorjectSettings > Quality > Render Pipeline Asset</b> to <color=#00FFFF>{URP_ASSET_NAME}</color> " +
                    "<color=#FFFF00>BEFORE PLAY!</color>";
        [SerializeField] Vector2Int m_messagePos = new Vector2Int(4, 0);

        RenderPipelineAsset m_previousRPAsset;
        RenderPipelineAsset m_previousRPQualityRPAsset;

        private void Awake()
        {
        }
        void Start()
        {
            if (!string.IsNullOrEmpty(m_bewareMessage))
            {
                string name = (m_URPAsset == null || m_startPipeline == PipelineType.Builtin) ? "None" : m_URPAsset.name;
                m_bewareMessage = m_bewareMessage.Replace("{URP_ASSET_NAME}", name);
            }

            m_previousRPQualityRPAsset = QualitySettings.renderPipeline;
            m_previousRPAsset = GraphicsSettings.defaultRenderPipeline;
            if (m_startPipeline != PipelineType.None)
            {
                SetRenderPipeline(m_startPipeline == PipelineType.URP ? m_URPAsset : null);
            }
        }
        void Update()
        {
        }

        private void OnDestroy()
        {
            Debug.Log("OnDestroy");
        }

        IEnumerator restoreCo()
        {
            if (m_startPipeline != PipelineType.None)
            {
                yield return new WaitForEndOfFrame();
                GraphicsSettings.defaultRenderPipeline = m_previousRPAsset;
                yield return new WaitForEndOfFrame();
                QualitySettings.renderPipeline = m_previousRPQualityRPAsset;
            }
        }

        private void OnApplicationQuit()
        {
            Debug.Log("OnApplicationQuit");
            StartCoroutine(restoreCo());
        }

        /// <summary>
        /// Set defaultRenderPipeline
        /// </summary>
        /// <param name="_asset"></param>
        /// <returns></returns>
        public RenderPipelineAsset SetRenderPipeline(RenderPipelineAsset _asset)
        {
            RenderPipelineAsset previousAsset = GraphicsSettings.defaultRenderPipeline;
            RenderPipelineAsset currentAsset = QualitySettings.renderPipeline;
            if (_asset != previousAsset)
            {
                if (_asset == null && (previousAsset != null))
                {
                    currentAsset = _asset;
                }
                else if (_asset != null && (previousAsset == null || previousAsset.name != _asset.name))
                {
                    currentAsset = _asset;
                }
            }
            StartCoroutine(setRenderPipelineCo(_asset));
            return previousAsset;
        }

        IEnumerator setRenderPipelineCo(RenderPipelineAsset _asset)
        {
            yield return new WaitForEndOfFrame();
            GraphicsSettings.defaultRenderPipeline = _asset;
            yield return new WaitForEndOfFrame();
            QualitySettings.renderPipeline = _asset;
        }

        void OnGUI()
        {
            if (!string.IsNullOrEmpty(m_bewareMessage) && m_useBeareMessage)
            {
                GUI.Label(new Rect(m_messagePos.x, m_messagePos.y, Screen.width - m_messagePos.x, 120), m_bewareMessage);
            }
            if (m_isChangable)
            {
                if (GUI.Button(new Rect(10, 150, 150, 20), "Change to Default"))
                {
                    SetRenderPipeline(null);
                }
                if (GUI.Button(new Rect(10, 170, 150, 20), "Change to URP"))
                {
                    SetRenderPipeline(m_URPAsset);
                }
            }
        }
    }
}
