using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable] public class UI_InputFields
{
    public TMP_InputField IDInputField;
    public TMP_InputField PasswordInputField;
    public TMP_InputField PasswordCheckInputField;

    public TextMeshProUGUI ResultText;
    public Button ConfirmButton;
    public Button ExtraButton;
}

public class UI_Login : MonoBehaviour
{
    [Header("Panel")]
    public GameObject LoginPanel;
    public GameObject RegisterPanel;

    [Header("Login")]
    public UI_InputFields LoginInputFields;

    [Header("Register")]
    public UI_InputFields RegisterInputFields;

    private void Awake()
    {
        LoginPanel.SetActive(true);
        RegisterPanel.SetActive(false);
    }

    public void Register()
    {
        // 1. ID 입력 확인
        string id = RegisterInputFields.IDInputField.text;
        if (string.IsNullOrEmpty(id))
        {
            RegisterInputFields.ResultText.text = "Please enter an ID.";
            return;
        }

        // 2. PW 입력 확인
        string password = RegisterInputFields.PasswordInputField.text;
        if (string.IsNullOrEmpty(password))
        {
            RegisterInputFields.ResultText.text = "Please enter a password.";
            return;
        }

        // 3. 비밀번호 확인 일치 검사
        string passwordCheck = RegisterInputFields.PasswordCheckInputField.text;
        if (password != passwordCheck)
        {
            RegisterInputFields.ResultText.text = "Passwords do not match.";
            return;
        }

        // 4. 이미 존재하는 ID인지 확인
        if (PlayerPrefs.HasKey("user_" + id))
        {
            RegisterInputFields.ResultText.text = "This ID is already registered.";
            return;
        }

        // 5. ID 및 PW 저장
        PlayerPrefs.SetString("user_" + id, password);
        PlayerPrefs.Save();

        // 6. 완료 메시지
        RegisterInputFields.ResultText.text = "Registration complete!";
    }

}
