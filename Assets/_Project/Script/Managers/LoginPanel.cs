using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public TMP_InputField nickNameInputField;
    public TMP_Text resultText;
    public Button signInWithEmailButton;
    public Button signInAnnomyousButton;

    public TMP_Text signInAndCreateAccountButtonText;
    public Toggle createNewUserToggle;

    GameManager gameManager;

    private void Awake()
    {
        signInWithEmailButton.onClick.AddListener(()=>SignInWithEmail().Forget());
        signInAnnomyousButton.onClick.AddListener(()=>SignInAnnomyous().Forget());
        createNewUserToggle.onValueChanged.AddListener(OnCreateNewUserToggleValueChanged);
    }

    public void Construct(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    private void OnCreateNewUserToggleValueChanged(bool value)
    {
        if (value)
        {
            nickNameInputField.transform.parent.gameObject.SetActive(true);
            signInAndCreateAccountButtonText.text = "회원가입";
        }
        else
        {
            nickNameInputField.transform.parent.gameObject.SetActive(false);
            signInAndCreateAccountButtonText.text = "로그인";
        }
    }

    private async UniTaskVoid SignInWithEmail()
    {
        if (createNewUserToggle.isOn)
        {
            if(string.IsNullOrEmpty(nickNameInputField.text.Trim()) || 
               string.IsNullOrEmpty(emailInputField.text.Trim()) ||
               string.IsNullOrEmpty(passwordInputField.text.Trim()))
            {
                resultText.text = "모든 항목을 입력해주세요.";
                return;
            }

            var result = await FirebaseAuthManager.Instance.CreateUserWithEmail(emailInputField.text.Trim(), passwordInputField.text.Trim());

            if (result.success)
            {
                this.gameObject.SetActive(false);
                await gameManager.LoadServerData();
                gameManager.personalData.NickName = nickNameInputField.text.Trim();
                await gameManager.SaveServerData();
            }
            else
            {
                resultText.text = result.error;
            }
        }
        else
        {
            if (string.IsNullOrEmpty(emailInputField.text.Trim()) ||
                string.IsNullOrEmpty(passwordInputField.text.Trim()))
            {
                resultText.text = "모든 항목을 입력해주세요.";
                return;
            }


            var result = await FirebaseAuthManager.Instance.SignInWithEmail(emailInputField.text.Trim(), passwordInputField.text.Trim());

            if (result.success)
            {
                this.gameObject.SetActive(false);
                await gameManager.LoadServerData();
            }
            else
            {
                resultText.text = result.error;
            }
        }

    }

    private async UniTaskVoid SignInAnnomyous()
    {
        var result = await FirebaseAuthManager.Instance.SignInAnonymousAsync();

        if (result.success)
        {
            this.gameObject.SetActive(false);
            await gameManager.LoadServerData();
        }
        else
        {
            resultText.text = result.error;
        }
    }
}