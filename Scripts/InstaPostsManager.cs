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
using System.Linq;
using System;


public class InstaPostsManager : MonoBehaviour
{
    public Transform PostsDisplay;
    public Transform HeadShotList;
    public Transform SidePanel;
    public PreFabs preFabs;
    public GeneralHandlers gen;
    public SaveFile SF;


    [System.Serializable]
    public class Post
    {
        public bool Display;
        public string CharacterName;
        public string UserName;
        public string Image;
        public string Description;
        public bool Liked;
    }

    [System.Serializable]
    public class PostsList
    {
        public List<Post> Posts;
    }

    [System.Serializable]
    public class Profile {
        public string Name;
        public string Username;
        public string Followers;
        public string Following;
        public string ProfileInfo;
    }

    [System.Serializable]
    public class ProfilesList {
        public List<Profile> Profiles;
    }



    PostsList postsList = new PostsList();
    ProfilesList profilesList = new ProfilesList();

    public ProfilesList GetProfilesList() {
        UnityEngine.TextAsset InstaProfilesFile = Resources.Load<UnityEngine.TextAsset>("InstaPosts-Profiles");
        profilesList = JsonUtility.FromJson<ProfilesList>(InstaProfilesFile.text);
        return profilesList;
    }

    public PostsList GetPostsFile() {
        UnityEngine.TextAsset InstaPostsFile = Resources.Load<UnityEngine.TextAsset>("instaPosts-posts");
        postsList = JsonUtility.FromJson<PostsList>(InstaPostsFile.text);
        return postsList;
    }

    public void GenPostsList() {
        List<Post> postsList = GetPostsFile().Posts;
        if (postsList.Count != SF.saveFile.Posts.Count) {
            List<Profile> ProfileList = GetProfilesList().Profiles;
            for (int i = SF.saveFile.Posts.Count; i < postsList.Count; i++) {
                Post item = postsList[i];
                int indexOfProfile = ProfileList.FindIndex(x => x.Name == item.CharacterName);
                SF.saveFile.Posts.Add(new SavedPost{ 
                    Unlocked = false,
                    Liked = false,
                    CharacterName = item.CharacterName,
                    Image = item.Image,
                    UserName = item.UserName,
                    Description = item.Description
                    // indexOfProfile = indexOfProfile
                });
                Profile profile = ProfileList[indexOfProfile];
                if (!SF.saveFile.InstaAccounts.Any(x => x.AccountOwner == item.CharacterName)){
                    SF.saveFile.InstaAccounts.Add(new InstaAccount { 
                        AccountOwner = item.CharacterName,
                        Unlocked = false,
                        NumberOfPosts = 0,
                        // indexOfProfile = indexOfProfile,
                        Following = profile.Following,
                        Followers = profile.Followers,
                        ProfileInfo = profile.ProfileInfo,
                        UserName = profile.Username
                    });

                }
            }
        }
    }

    public void DestroyProfileList() {
        for (int i = 1; i < HeadShotList.childCount; i++) {
            Destroy(HeadShotList.GetChild(i).gameObject);
        }
    }

    public void GenerateProfileButton(InstaAccount account) {
        Sprite pfp = Resources.Load(
            "Images/Headshots/" + account.AccountOwner,
            typeof(Sprite)
        ) as Sprite;

        Transform ProfileButtonClone = Instantiate(preFabs.ProfileButton, new Vector3(0, 0, 0), Quaternion.identity, HeadShotList);
        ProfileButtonClone.GetComponent<Image>().sprite = pfp;
        ProfileButtonClone.GetComponent<Button>().onClick.AddListener(() => {
            ClearPostsList();
            GenerateProfile(account.AccountOwner);
            SidePanel.GetComponent<Animator>().Play("Close-Side-Menu");
            DestroyProfileList();
        });
    }

    public void GenerateProfileList() {
        for (int i = 0; i < SF.saveFile.InstaAccounts.Count; i++) {
            if (SF.saveFile.InstaAccounts[i].Unlocked) {
                GenerateProfileButton(SF.saveFile.InstaAccounts[i]);
            }
        }        
    }

    public void ClearPostsList() {
        for (int i = 0; i < PostsDisplay.childCount; i++) {
            Destroy(PostsDisplay.GetChild(i).gameObject);
        }
    }

    public void GenerateProfileHeader(InstaAccount Profile) {
        Sprite pfp = Resources.Load(
            "Images/Headshots/" + Profile.AccountOwner,
            typeof(Sprite)
        ) as Sprite;

        Transform ProfileHeaderClone =
            Instantiate(preFabs.InstaPostProfileHeader, new Vector3(0, 0, 0), Quaternion.identity, PostsDisplay);
        ProfileHeaderClone.GetChild(0).GetComponent<Image>().sprite = pfp;
        ProfileHeaderClone.GetChild(1).GetComponent<TextMeshProUGUI>().text = Profile.UserName;
        Transform InfoContainer = ProfileHeaderClone.GetChild(2).transform;
        InfoContainer.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = Profile.NumberOfPosts.ToString();
        InfoContainer.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = Profile.Followers;
        InfoContainer.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = Profile.Following;
        ProfileHeaderClone.GetChild(3).GetComponent<TextMeshProUGUI>().text = Profile.ProfileInfo;
    }

    public void GenerateInstaPost(SavedPost post) {
        Sprite pfp = Resources.Load(
            "Images/Headshots/" + post.CharacterName,
            typeof(Sprite)
        ) as Sprite;
        Sprite Photo = Resources.Load("Images/Photos/" + post.Image, typeof(Sprite)) as Sprite;

        Transform InstaPostCard = Instantiate(preFabs.InstaPostCard, new Vector3(0, 0, 0), Quaternion.identity, PostsDisplay);
        // Photo of the post
        InstaPostCard.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Photo;
        InstaPostCard.GetChild(1).GetChild(0).GetComponent<Image>().sprite = pfp;
        InstaPostCard.GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(() => {
            ClearPostsList();
            GenerateProfile(post.CharacterName);
        });
        InstaPostCard.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = post.UserName;
        InstaPostCard.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = post.Description;
        Transform NotLikedButton = InstaPostCard.GetChild(2).GetChild(2).transform;
        if (post.Liked == true) {
            gen.Hide(NotLikedButton);
        }
        // This is the red heart button
        InstaPostCard.GetChild(2).GetChild(1).GetComponent<Button>().onClick.AddListener(() => {
            post.Liked = false;
            gen.Show(NotLikedButton);
        });
        // this is the empty heart button
        NotLikedButton.GetComponent<Button>().onClick.AddListener(() => {
            post.Liked = true;
            gen.Hide(NotLikedButton);
        });
    }

    public void GenerateProfile(string nameOfProfileOwner) {
        int indexOfProfile = SF.saveFile.InstaAccounts.FindIndex(x => x.AccountOwner == nameOfProfileOwner);
        InstaAccount account = SF.saveFile.InstaAccounts[indexOfProfile];
        GenerateProfileHeader(account);

        // Since posts in real life start from newest, this starts the iterator at the newest post and goes backwards to populate
        for (int i = SF.saveFile.Posts.Count-1; i > -1; i--) {
            if (SF.saveFile.Posts[i].Unlocked && nameOfProfileOwner == SF.saveFile.Posts[i].CharacterName) {
                GenerateInstaPost(SF.saveFile.Posts[i]);   
            }
        }
        
    }

    public void DisplayAllPosts() {
        // Since posts in real life start from newest, this starts the iterator at the newest post and goes backwards to populate
        for (int i = SF.saveFile.Posts.Count-1; i > -1; i--) {
            if (SF.saveFile.Posts[i].Unlocked) {
                int indexOfProfile = SF.saveFile.InstaAccounts.FindIndex(x => x.AccountOwner == SF.saveFile.Posts[i].CharacterName);
                InstaAccount account = SF.saveFile.InstaAccounts[indexOfProfile];
                GenerateInstaPost(SF.saveFile.Posts[i]);
            }
        }
    }
}
