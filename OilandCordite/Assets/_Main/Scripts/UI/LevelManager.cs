using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] levels;
    private GameObject spawnedLevel;
    public int currentSelection;
    void Start()
    {
        currentSelection = 0;
        if (levels.Length > 0)
        {
            spawnCurLevelSelect();
        }
    }
    private void spawnCurLevelSelect()
    {
        if (spawnedLevel != null)
        {
            Destroy(spawnedLevel);
        }
        spawnedLevel = Instantiate(levels[currentSelection]);
        spawnedLevel.transform.SetParent(this.transform);
        spawnedLevel.transform.localPosition = new Vector3(0, 0, 0);
    }
    public void getNext()
    {
        if (levels.Length-1 > currentSelection)
        {
            currentSelection++;
        } else if (levels.Length > currentSelection)
        {
            currentSelection = 0;
        }
        spawnCurLevelSelect();
    }
    public void getPrev()
    {
        if (levels.Length < currentSelection)
        {
            currentSelection=0;
        }
        else if (levels.Length >0)
        {
            currentSelection--;
        }
        spawnCurLevelSelect();
    }
    public void loadScene()
    {
        string temp = spawnedLevel.name.Replace("(Clone)","");
        SceneManager.LoadScene(temp, LoadSceneMode.Single);
    }
}
