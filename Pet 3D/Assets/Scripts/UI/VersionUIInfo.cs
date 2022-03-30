using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VersionUIInfo : MonoBehaviour
{
    List<TMP_Text> texts;
    TMP_Text versionInfo;
    TMP_Text platformInfo;

    private void Awake()
    {
        texts = new List<TMP_Text>();
        texts.AddRange(GetComponentsInChildren<TMP_Text>());

        foreach(var text in texts)
        {
            if (text.name.Contains("Version"))
                versionInfo = text;
            else if (text.name.Contains("Platform"))
                platformInfo = text;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        versionInfo.text = "Version: ";
        versionInfo.text += Application.version.ToString();

        platformInfo.text = "Platform: ";
        platformInfo.text += Application.platform.ToString();
    }
}
