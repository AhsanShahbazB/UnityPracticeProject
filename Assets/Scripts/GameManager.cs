using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum Layouts
    {
        _2x3,_3x2
    };


    public Layouts SelectedUserLayout;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(this);
        }
    }

    public void SelectLayout(int selectLayout)
    {
        if(selectLayout == 0)
        {
            SelectedUserLayout = Layouts._2x3;
        }
        else
        {
            SelectedUserLayout = Layouts._3x2;
        }
    }

}
