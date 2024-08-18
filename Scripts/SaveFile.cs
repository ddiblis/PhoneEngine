using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
    public class Contact
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
        public string UserName;
        public int NumberOfPosts;
        public bool Unlocked;
        public string Followers;
        public string Following;
        public string ProfileInfo;
    }

    [System.Serializable]
    public class SavedPost
    {
        public string CharacterName;
        public string UserName;
        public string Image;
        public string Description;
        public bool Liked;
        public bool Unlocked;
    }

    [System.Serializable]
    public class SaveFileRoot {
        public int contactPush;
        public int selectedIndex;
        public int NumberOfSaves;
        public string CurrWallPaper;
        public bool ChoiceNeeded;
        public List<Contact> ContactsList = new List<Contact>();
        public List<string> ChapterList = new List<string>();
        public CurrStoryPoint CurrStoryPoint = new CurrStoryPoint();
        public List<SavedMessage> SavedMessages = new List<SavedMessage>();
        public List<InstaAccount> InstaAccounts = new List<InstaAccount>();
        public List<SavedPost> Posts = new List<SavedPost>();
        public List<PhotoCategory> PhotoCategories = new List<PhotoCategory>();
        public List<Photo> Photos = new List<Photo>();

        
    }

    [System.Serializable]
    public class SavedMessage
    {
        public string TextContent;
        public int WhosIsItFor;
        public int TypeOfText;
    }

    [System.Serializable]
    public class Photo {
        public string Category;
        public string ImageName;
        public bool Seen;

    }

    [System.Serializable]
    public class PhotoCategory {
        public string Category;
        public int NumberSeen;
        public int NumberAvaliable;
    }

public class SaveFile : MonoBehaviour
{
    public SaveFileRoot saveFile = new SaveFileRoot();

}
