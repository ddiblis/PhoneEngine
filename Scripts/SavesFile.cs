using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavesFile : MonoBehaviour {

    public int MostRecentSaveIndex;
    public List<string> NameOfSaves;
    public List<int> ChapterOfSaves;
    public List<int> TendencyOfSaves;
    public List<string> DateTimeOfSave;
    public bool AutoSaveMostRecent;
    public int AutoSaveChapter;
    public int AutoSaveTendency;
    public string AutoSaveDateTime;
}
