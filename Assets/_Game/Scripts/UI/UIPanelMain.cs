using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelMain : MonoBehaviour, IMenu
{
    [SerializeField] private Button btnTimer;
    [SerializeField] private Button btnNormal;
    [SerializeField] private Button btnAutoWin;
    [SerializeField] private Button btnAutoLose;

    private UIMainManager m_mngr;

    private void Awake()
    {
        btnNormal.onClick.AddListener(OnClickMoves);
        btnTimer.onClick.AddListener(OnClickTimer);
        btnAutoWin.onClick.AddListener(OnClickAutoPlayWin);
        btnAutoLose.onClick.AddListener(OnClickAutoPlayLose);
    }

    private void OnDestroy()
    {
        if (btnNormal) btnNormal.onClick.RemoveAllListeners();
        if (btnTimer) btnTimer.onClick.RemoveAllListeners();
        if (btnAutoWin) btnAutoWin.onClick.RemoveAllListeners();
        if (btnAutoLose) btnAutoLose.onClick.RemoveAllListeners();
    }

    public void Setup(UIMainManager mngr)
    {
        m_mngr = mngr;
    }

    private void OnClickTimer()
    {
        m_mngr.LoadLevelTimer();
    }

    private void OnClickMoves()
    {
        m_mngr.LoadLevelNormal();
    }

    private void OnClickAutoPlayWin()
    {
        m_mngr.LoadAutoPlay(AutoPlayController.eAutoMode.WIN);
    }

    private void OnClickAutoPlayLose()
    {
        m_mngr.LoadAutoPlay(AutoPlayController.eAutoMode.LOSE);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
