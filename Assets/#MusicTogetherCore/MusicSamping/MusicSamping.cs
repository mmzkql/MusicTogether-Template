using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class MusicSamping : EditorWindow
{
    //BasicInformation
    private AudioClip musicClip;
    private int bpm = 120;

    //States
    private bool isPlaying = false;
    private float playbackPosition = 0.0f;
    private float playProgress = 0.0f;
    private float[] audioSamples;

    //Rendering
    private Color waveformColor = new Color(0.2f, 0.5f, 1f); // 波形颜色（蓝）
    private float waveformZoom = 1.0f;

    [MenuItem("Window/MusicSampingMenu")]
    public static void ShowExample()
    {
        MusicSamping wnd = GetWindow<MusicSamping>();
        wnd.titleContent = new GUIContent("MusicSamping");
    }
    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/#MusicTogetherCore/MusicSamping/MusicSamping.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/#MusicTogetherCore/MusicSamping.uss");
        var noteStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/#MusicTogetherCore/MusicSamping/Note.uss");

        /*
        VisualElement labelWithStyle = new Label("Hello World! With Style");
        labelWithStyle.styleSheets.Add(styleSheet);
        root.Add(labelWithStyle);*/

        // 获取UXML元素引用
        VisualElement mainContainer = root.Q<VisualElement>("mainContainer");
        VisualElement audioInfoBox = root.Q<VisualElement>("AudioInfoBox");
        VisualElement videoBox = root.Q<VisualElement>("VideoBox");
        VisualElement controlBox = root.Q<VisualElement>("ControlBox");
        VisualElement waveformBox = root.Q<VisualElement>("WaveformBox");

        ObjectField musicField = root.Q<ObjectField>("musicField");
        IntegerField bpmField = root.Q<IntegerField>("bpmField");
        Button selectVideoButton = root.Q<Button>("selectVideoButton");
        Button playButton = root.Q<Button>("playButton");
        Slider progerssSlider = root.Q<Slider>("progerssSlider");
        Slider zoomSlider = root.Q<Slider>("zoomSlider");
        VisualElement waveformContainer = root.Q<VisualElement>("waveformContainer");

        //var waveform = new AudioWaveformElement();

        //waveformContainer.Add(waveform);
        musicField.RegisterValueChangedCallback(evt =>
        {
            musicClip = evt.newValue as AudioClip;
            if (musicClip != null)
            {
                Debug.Log("AudioClip selected: " + musicClip.samples);
                //waveform.LoadAudioClip(musicClip,0,0.05f);
            }
            else
            {
                Debug.Log("No AudioClip selected.");
            }
        });
        bpmField.RegisterValueChangedCallback(evt => { bpm = evt.newValue;Debug.Log($"BPM:{evt.newValue}"); });
    }
}
