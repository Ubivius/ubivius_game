using UnityEngine;
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

        private void Awake()
        {

        }

        public void Login()
        {
            m_lobby.SendLoginRequest(m_userInputField.text, m_passwordInputField.text);
        }
    }
}
