using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedItems : MonoBehaviour {

    // The int of which message list to send the messages to based on their name and their position in the contactsList
    public int contactPush;
    // The index of which messageList you're currently viewing, derived from the contact card you click.
    public int selectedIndex;
    public List<string> ContactsList;
    public List<bool> UnlockedContacts;
    public List<string> ChapterList;
    public bool ChoiceNeeded;
    public int CurrChapIndex;
    public int CurrSubChapIndex;
    public int CurrText;
    public List<string> seenImages;
    public List<string> savedMessages;
    public List<int> whosTheMessageFor;
    public List<int> typeOfText;
    public int NumberOfSaves;
    public string currWallPaper;
    public List<string> InstaPostsAccounts;
    public List<bool> UnlockedAccounts;
    public List<bool> UnlockedPosts;
    public List<bool> LikedPosts;
    public List<int> NumberOfPosts;

}
