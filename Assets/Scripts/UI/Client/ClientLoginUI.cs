using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace ubv.ui.client
{
    public class ClientLoginUI : TabBehaviour
    {
        [SerializeField] private ubv.client.logic.ClientSyncLogin m_lobby;

        [SerializeField] private TMP_InputField m_userInputField;
        [SerializeField] private TMP_InputField m_passwordInputField;

        [SerializeField] private Button m_loginButton;

        private void Awake()
        {

        }


        protected override void Update()
        {
            base.Update();
            if (string.IsNullOrEmpty(m_userInputField.text) || string.IsNullOrEmpty(m_passwordInputField.text))
            {
                m_loginButton.interactable = false;
            }
            else
            {
                m_loginButton.interactable = true;
            }

            if (Input.GetKeyDown(KeyCode.Return) && (!string.IsNullOrEmpty(m_userInputField.text) && !string.IsNullOrEmpty(m_passwordInputField.text)))
            {
                this.Login();
            }
        }

        public void Login()
        {
            m_lobby.SendLoginRequest(m_userInputField.text, m_passwordInputField.text);
        }

        public void CreateAccount()
        {
            Application.OpenURL("http://dev.ubivius.tk/signup");
        }
    }
}
