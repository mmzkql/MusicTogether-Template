using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif

public class ToggleButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab; // ��ťPrefab
    [SerializeField] private Transform ButtonHolder; // ��ť�ĸ�����
    [SerializeField] private string[] options; // ѡ����������
    public int selectedOptionIndex = -1; // ��ǰѡ�е�ѡ��������-1��ʾδѡ��
    private int extraCount
    {
        get => ButtonHolder.childCount - options.Length;
    }

    //private List<GameObject> buttons = new List<GameObject>(); // �洢���ɵİ�ť

    private void Start()
    {
        ReorganizeButtons();
    }
    public bool CheckResources()
    {
        bool result = true;
        if (buttonPrefab == null)
        {
            Debug.LogError("Button Prefabδ���ã�");
            result = true;
        }
        if (ButtonHolder == null)
        {
            Debug.LogError("Button Holderδ���ã�");
            result = true;
        }
        if (options == null || options.Length == 0)
        {
            Debug.LogError("ѡ������Ϊ�գ�");
            result = true;
        }
        return result;
    }
    [Button]
    public void ReorganizeButtons()
    {
        if (!CheckResources())
            return;
#if UNITY_EDITOR
        if (PrefabUtility.IsPartOfPrefabAsset(ButtonHolder))
            return;
        if (Application.isPlaying) return; // ����ʱ��������
#endif
        if (extraCount > 0)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += (() => {
                for (int i = 0; i < extraCount; i++)
                {
                    DestroyImmediate(ButtonHolder.GetChild(0).gameObject);
                }
            });
#endif
            for (int i = 0; i < extraCount; i++)
            {
                Destroy(ButtonHolder.GetChild(0).gameObject);
            }
        }
        if(extraCount < 0)
        {
            GameObject button = null;
#if UNITY_EDITOR
            for (int i = extraCount; i < 0; i++)
            {
                button = (GameObject)PrefabUtility.InstantiatePrefab(buttonPrefab, ButtonHolder);
                if (button == null)
                {
                    Debug.LogError("��ťPrefabʵ����ʧ�ܣ�");
                    continue;
                }
            }
#endif
            for (int i = extraCount; i < 0; i++)
            {
                button = Instantiate(buttonPrefab, ButtonHolder);
                if (button == null)
                {
                    Debug.LogError("��ťPrefabʵ����ʧ�ܣ�");
                    continue;
                }
            }
        }

        UpdateButtonInformation();
    }
    public void UpdateButtonInformation()
    {
        if (extraCount != 0)
            ReorganizeButtons();
        for (int i = 0; i < options.Length; i++)
        {
            ButtonHolder.GetChild(i).name = "Option " + i + ":" + options[i];
            ButtonHolder.GetChild(i).GetComponent<ToggleButton>().Content = options[i];
            Button uiButton = ButtonHolder.GetChild(i).GetComponent<Button>();
            if (uiButton != null)
            {
                int currentIndex = i; // ����ǰѭ������������һ�в���ʡ������
                uiButton.onClick.AddListener(() => OnButtonClicked(currentIndex));
            }
            else
            {
                Debug.LogError("��ťPrefab��û��Button�����");
            }
        }
    }
    // �ڱ༭������֤ʱ��̬���ɰ�ť
    void OnValidate()
    {
        ReorganizeButtons();
    }

    // ��ť����¼�����
    private void OnButtonClicked(int index)
    {
        // ����������ͬһ����ť��ֱ�ӷ���
        if (selectedOptionIndex == index)
        {
            return;
        }
        // ����ѡ��״̬
        selectedOptionIndex = index;

        // ���������������߼����������UI��ʾѡ��״̬
        UpdateButtonStates();
    }

    // ���°�ť״̬���������ѡ�еİ�ť
    private void UpdateButtonStates()
    {
        foreach(ToggleButton i in GetComponentsInChildren<ToggleButton>())
        {
            i.SetState(selectedOptionIndex == i.transform.GetSiblingIndex());
        }
    }
    // �����Ҫ��������ѡ����Ե��ô˷���
    public void SetOptions(string[] newOptions)
    {
        options = newOptions;
        ReorganizeButtons();
        UpdateButtonInformation();
    }
}