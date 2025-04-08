using UnityEngine;
using UnityEngine.UIElements;

public class AudioWaveformElement : VisualElement
{
    private float[] audioSamples;
    private Color waveformColor = new Color(0.2f, 0.5f, 1f);
    private float lineWidth = 1.5f;

    public AudioWaveformElement()
    {
        style.height = 80;
        style.backgroundColor = new Color(0.1f, 0.1f, 0.1f);
        generateVisualContent += OnGenerateVisualContent;
    }

    void OnGenerateVisualContent(MeshGenerationContext ctx)
    {
        if (audioSamples == null || audioSamples.Length == 0)
            return;

        float width = contentRect.width;
        float height = contentRect.height;
        float centerY = height * 0.5f;

        // ������õ�������
        int pixelsAvailable = Mathf.FloorToInt(width);

        // ��������ÿ�����ض�Ӧ��������
        int samplesPerPixel = Mathf.CeilToInt((float)audioSamples.Length / pixelsAvailable);

        // ����ʵ����Ҫ�Ķ�������������
        int vertexCount = pixelsAvailable * 4; // ÿ�����ض�Ӧ4������
        int indexCount = pixelsAvailable * 6;  // ÿ�����ض�Ӧ6������

        // ������������
        MeshWriteData mesh = ctx.Allocate(vertexCount, indexCount);

        int vertexIndex = 0;
        int triangleIndex = 0;

        for (int x = 0; x < pixelsAvailable; x++)
        {
            int startSample = Mathf.FloorToInt(x * samplesPerPixel);
            int endSample = Mathf.FloorToInt((x + 1) * samplesPerPixel);
            endSample = Mathf.Min(endSample, audioSamples.Length - 1);

            float min = float.MaxValue;
            float max = float.MinValue;
            for (int i = startSample; i < endSample; i++)
            {
                float sample = audioSamples[i];
                if (sample < min) min = sample;
                if (sample > max) max = sample;
            }

            float yMax = centerY - max * centerY;
            float yMin = centerY - min * centerY;
            float xPos = x;

            Vertex[] verts = new Vertex[4];

            // ���ö���λ��
            verts[0].position = new Vector3(xPos - lineWidth / 2, yMax, Vertex.nearZ);
            verts[1].position = new Vector3(xPos + lineWidth / 2, yMax, Vertex.nearZ);
            verts[2].position = new Vector3(xPos + lineWidth / 2, yMin, Vertex.nearZ);
            verts[3].position = new Vector3(xPos - lineWidth / 2, yMin, Vertex.nearZ);

            // ���ö�����ɫ
            for (int i = 0; i < 4; i++)
            {
                verts[i].tint = waveformColor;
                mesh.SetNextVertex(verts[i]);
            }

            // ��������������
            mesh.SetNextIndex((ushort)(vertexIndex + 0));
            mesh.SetNextIndex((ushort)(vertexIndex + 1));
            mesh.SetNextIndex((ushort)(vertexIndex + 2));
            mesh.SetNextIndex((ushort)(vertexIndex + 0));
            mesh.SetNextIndex((ushort)(vertexIndex + 2));
            mesh.SetNextIndex((ushort)(vertexIndex + 3));

            vertexIndex += 4;
            triangleIndex += 6;
        }
    }

    public void LoadAudioClip(AudioClip clip, float startTime, float endTime)
    {
        // ������ȫ���
        if (!clip)
        {
            Debug.LogError("AudioClip cannot be null!");
            return;
        }

        // ��֤��Ƶ��������
        if (clip.loadType != AudioClipLoadType.DecompressOnLoad)
        {
            Debug.LogError($"AudioClip '{clip.name}' must have Load Type set to DecompressedOnLoad!");
            Debug.Log("����Unity�༭����ѡ����Ƶ�ļ�����Inspector�����н�Load Type��ΪDecompressedOnLoad");
            return;
        }

        // ���δָ������ʱ�䣬��Ĭ��ʹ����Ƶ���ܳ���
        if (endTime < 0f)
        {
            endTime = clip.length;
        }

        // ���ʱ�䷶Χ�Ƿ���Ч
        if (startTime < 0f || endTime > clip.length || startTime >= endTime)
        {
            Debug.LogError($"Invalid time range: startTime={startTime}, endTime={endTime}");
            return;
        }

        try
        {
            // �������������������� �� ��������
            int totalSamples = clip.samples * clip.channels;

            // ������ʼ�ͽ�����������
            int startSample = Mathf.FloorToInt(startTime * clip.frequency) * clip.channels;
            int endSample = Mathf.FloorToInt(endTime * clip.frequency) * clip.channels;

            // ȷ��������������Ч��Χ��
            startSample = Mathf.Clamp(startSample, 0, totalSamples - 1);
            endSample = Mathf.Clamp(endSample, 0, totalSamples - 1);

            // ������Ҫ��ȡ����������
            int sampleCount = endSample - startSample;

            // ���������洢����
            audioSamples = new float[sampleCount];

            // ��ȡ��Ƶ���ݣ�����ʼ������ʼ��ȡ��
            bool success = clip.GetData(audioSamples, startSample);

            if (!success)
            {
                Debug.LogError("��Ƶ���ݶ�ȡʧ�ܣ�������Ƶ����");
                audioSamples = null;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"��Ƶ�����쳣: {e.Message}");
            audioSamples = null;
        }

        // ���������ػ�
        MarkDirtyRepaint();
    }
}
// ʹ��ʾ����
// ��UI��ʼ����������ӣ�
// var waveform = new AudioWaveformElement();
// rootVisualElement.Add(waveform);
// waveform.LoadAudioClip(yourAudioClip);