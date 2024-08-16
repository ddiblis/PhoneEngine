using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine.TextCore.Text;
using System.IO;
using System;
using System.Linq;
public class SaveFile : MonoBehaviour
{
    [System.Serializable]
    public class ContactsList
    {
        public string NameOfContact;
        public bool Unlocked;
    }

    [System.Serializable]
    public class CurrStoryPoint
    {
        public int ChapIndex;
        public int SubChapIndex;
        public int CurrTextIndex;
    }

    [System.Serializable]
    public class InstaAccount
    {
        public string AccountOwner;
        public int NumberOfPosts;
        public bool Unlocked;
    }

    [System.Serializable]
    public class Post
    {
        public string CharacterName;
        public string UserName;
        public string Image;
        public string Description;
        public bool Liked;
    }

    [System.Serializable]
    public class SaveFileRoot
    {
        public int contactPush;
        public int selectedIndex;
        public List<ContactsList> ContactsList = new List<ContactsList>();
        public List<string> ChapterList = new List<string>();
        public bool ChoiceNeeded;
        public CurrStoryPoint CurrStoryPoint = new CurrStoryPoint();
        public List<SavedMessage> SavedMessages = new List<SavedMessage>();
        public int NumberOfSaved;
        public string CurrWallPaper;
        public List<InstaAccount> InstaAccounts = new List<InstaAccount>();
        public List<Post> Posts = new List<Post>();
        public List<SeenImage> SeenImages = new List<SeenImage>();
    }

    [System.Serializable]
    public class SavedMessage
    {
        public string TextContent;
        public int WhosIsItFor;
        public int TypeOfText;
    }

    [System.Serializable]
    public class SeenImage
    {
        public string Character;
        public string ImagesName;
    }

    public SaveFileRoot saveFile = new SaveFileRoot();
}
