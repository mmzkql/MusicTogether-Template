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

        // 计算可用的像素数
        int pixelsAvailable = Mathf.FloorToInt(width);

        // 降采样：每个像素对应的样本数
        int samplesPerPixel = Mathf.CeilToInt((float)audioSamples.Length / pixelsAvailable);

        // 计算实际需要的顶点数和索引数
        int vertexCount = pixelsAvailable * 4; // 每个像素对应4个顶点
        int indexCount = pixelsAvailable * 6;  // 每个像素对应6个索引

        // 分配网格数据
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

            // 设置顶点位置
            verts[0].position = new Vector3(xPos - lineWidth / 2, yMax, Vertex.nearZ);
            verts[1].position = new Vector3(xPos + lineWidth / 2, yMax, Vertex.nearZ);
            verts[2].position = new Vector3(xPos + lineWidth / 2, yMin, Vertex.nearZ);
            verts[3].position = new Vector3(xPos - lineWidth / 2, yMin, Vertex.nearZ);

            // 设置顶点颜色
            for (int i = 0; i < 4; i++)
            {
                verts[i].tint = waveformColor;
                mesh.SetNextVertex(verts[i]);
            }

            // 构建三角形索引
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
        // 基础安全检查
        if (!clip)
        {
            Debug.LogError("AudioClip cannot be null!");
            return;
        }

        // 验证音频加载设置
        if (clip.loadType != AudioClipLoadType.DecompressOnLoad)
        {
            Debug.LogError($"AudioClip '{clip.name}' must have Load Type set to DecompressedOnLoad!");
            Debug.Log("请在Unity编辑器中选择音频文件，在Inspector窗口中将Load Type改为DecompressedOnLoad");
            return;
        }

        // 如果未指定结束时间，则默认使用音频的总长度
        if (endTime < 0f)
        {
            endTime = clip.length;
        }

        // 检查时间范围是否有效
        if (startTime < 0f || endTime > clip.length || startTime >= endTime)
        {
            Debug.LogError($"Invalid time range: startTime={startTime}, endTime={endTime}");
            return;
        }

        try
        {
            // 计算总样本数（样本数 × 声道数）
            int totalSamples = clip.samples * clip.channels;

            // 计算起始和结束样本索引
            int startSample = Mathf.FloorToInt(startTime * clip.frequency) * clip.channels;
            int endSample = Mathf.FloorToInt(endTime * clip.frequency) * clip.channels;

            // 确保样本索引在有效范围内
            startSample = Mathf.Clamp(startSample, 0, totalSamples - 1);
            endSample = Mathf.Clamp(endSample, 0, totalSamples - 1);

            // 计算需要提取的样本数量
            int sampleCount = endSample - startSample;

            // 创建样本存储数组
            audioSamples = new float[sampleCount];

            // 获取音频数据（从起始样本开始读取）
            bool success = clip.GetData(audioSamples, startSample);

            if (!success)
            {
                Debug.LogError("音频数据读取失败，请检查音频设置");
                audioSamples = null;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"音频加载异常: {e.Message}");
            audioSamples = null;
        }

        // 触发波形重绘
        MarkDirtyRepaint();
    }
}
// 使用示例：
// 在UI初始化代码中添加：
// var waveform = new AudioWaveformElement();
// rootVisualElement.Add(waveform);
// waveform.LoadAudioClip(yourAudioClip);