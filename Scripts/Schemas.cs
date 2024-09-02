using System.Collections.Generic;


/* ====================== Chapter Schema ============================ */
[System.Serializable]
public class Chapter
{
    public bool AllowMidrolls;
    public List<SubChap> SubChaps;
}

[System.Serializable]
public class SubChap
{
    public string Contact;
    public string TimeIndicator;
    public List<TextMessage> TextList;
    public string UnlockInstaPostsAccount;
    public List<int> UnlockPosts;
    public List<Response> Responses;
}

[System.Serializable]
public class TextMessage
{
    public int Type;
    public string TextContent;
    public int Tendency;
    public float TextDelay;
}

[System.Serializable]
public class Response
{
    public bool RespTree;
    public string TextContent;
    public int SubChapNum;
    public int Type;
}

/* ====================== End ============================ */

/* ====================== Nodes Schema ============================ */

[System.Serializable]
public class Location {
    public float x;
    public float y;
    public float Width;
    public float Height;
}

[System.Serializable]
public class ChapterData
{
    public bool AllowMidrolls;
    public Location location;
    public List<SubChapData> SubChaps;
}

[System.Serializable]
public class SubChapData
{
    public string Contact;
    public string TimeIndicator;
    public List<TextMessageData> TextList;
    public string UnlockInstaPostsAccount;
    public List<int> UnlockPosts;
    public List<ResponseData> Responses;
    public Location location;

}

[System.Serializable]
public class TextMessageData
{
    public int Type;
    public string TextContent;
    public float TextDelay;
    public int Tendency;
    public Location location;
}

[System.Serializable]
public class ResponseData
{
    public bool RespTree;
    public string TextContent;
    public int SubChapNum;
    public int Type;
    public Location location;

}

/* ====================== End ============================ */

/* ====================== Save Schema ============================ */
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
    public int playTime;
    public int contactPush;
    public int selectedIndex;
    public bool AllowMidrolls;
    public int MidRollCount = 2;
    public int CurrMidRoll;
    public int NumberOfSaves;
    public string CurrWallPaper;
    public bool ChoiceNeeded;
    public bool PlayingMidRoll;
    public Stats Stats;
    public Settings Settings = new();
    public List<Contact> ContactsList = new();
    public List<string> ChapterList = new();
    public CurrStoryPoint CurrStoryPoint = new();
    public List<SavedMessage> SavedMessages = new();
    public List<InstaAccount> InstaAccounts = new();
    public List<SavedPost> Posts = new();
    public List<PhotoCategory> PhotoCategories = new();
    public List<Photo> Photos = new();
    public List<MidRoll> MidRolls = new(); 
}

[System.Serializable]
public class Stats {
    public int Tendency;
}

[System.Serializable]
public class MidRoll {
    public string MidrollName;
    public bool Seen;
}

[System.Serializable]
public class Settings {
    public bool FasterReplies;
    public bool MuteGame;
    public bool FullScreen;
    public bool GameMode;
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

/* ====================== End ============================ */

/* ====================== DB ============================ */
[System.Serializable]
public class DBRoot {
    public List<string> ContactList;
    public List<string> ChapterList;
    public List<string> PhotoList;
    public List<string> MidrollsList;
}
/* ====================== End ============================ */

/* ====================== Enums ============================ */
public enum TypeOfText {
    sentText = 0,
    recText = 1,
    sentImage = 2,
    recImage = 3,
    sentEmoji = 4,
    recEmoji = 5,
    chapEnd = 6,
    indicateTime = 8,
}
public enum Tendency {
    Neutral = 0,
    Submissive = 1,
    Dominant = 2
}
public enum EStats {
    Tendency = 0,
    InterestInThreesomes = 1,
    IntersetInOtherGirls = 2
}
/* ====================== End ============================ */
