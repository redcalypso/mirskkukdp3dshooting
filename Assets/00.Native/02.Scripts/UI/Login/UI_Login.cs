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
        // 1. ID �Է� Ȯ��
        string id = RegisterInputFields.IDInputField.text;
        if (string.IsNullOrEmpty(id))
        {
            RegisterInputFields.ResultText.text = "Please enter an ID.";
            return;
        }

        // 2. PW �Է� Ȯ��
        string password = RegisterInputFields.PasswordInputField.text;
        if (string.IsNullOrEmpty(password))
        {
            RegisterInputFields.ResultText.text = "Please enter a password.";
            return;
        }

        // 3. ��й�ȣ Ȯ�� ��ġ �˻�
        string passwordCheck = RegisterInputFields.PasswordCheckInputField.text;
        if (password != passwordCheck)
        {
            RegisterInputFields.ResultText.text = "Passwords do not match.";
            return;
        }

        // 4. �̹� �����ϴ� ID���� Ȯ��
        if (PlayerPrefs.HasKey("user_" + id))
        {
            RegisterInputFields.ResultText.text = "This ID is already registered.";
            return;
        }

        // 5. ID �� PW ����
        PlayerPrefs.SetString("user_" + id, password);
        PlayerPrefs.Save();

        // 6. �Ϸ� �޽���
        RegisterInputFields.ResultText.text = "Registration complete!";
    }

}
