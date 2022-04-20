using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UIButtonType
{
    None,
    Close
}

public class UIButton : MonoBehaviour
{
    [SerializeField] UIButtonType type;
    Button _button;

    public delegate void OnClick();
    public event OnClick close;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GetButtonFunction(_button);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetButtonFunction(Button button)
    {
        if (type == UIButtonType.Close)
            button.onClick.AddListener(Close);
    }

    void Close()
    {
        close();
    }
}
