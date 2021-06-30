using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace ubv.ui.client
{
    public class ClientMenuUI : TabBehaviour
    {
        [SerializeField] private string m_gameSearchScene;

        [SerializeField] private Button m_playButton;
        [SerializeField] private Button m_statsButton;
        [SerializeField] private Button m_optionsButton;
        [SerializeField] private Button m_quitButton;

        public void Play()
        {
            AsyncOperation loadLobby = SceneManager.LoadSceneAsync(m_gameSearchScene);
        }

        public void Stats()
        {
            // to be implemented
        }

        public void Options()
        {
            // to be implemented
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
