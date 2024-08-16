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

public class InstaPostsManager : MonoBehaviour
{
    public SavedItems saved;
    public Transform PostsDisplay;
    public Transform HeadShotList;
    public Transform SidePanel;
    public PreFabs preFabs;
    public GeneralHandlers gen;

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
        if (postsList.Count != saved.UnlockedPosts.Count) {
            for (int i = saved.UnlockedPosts.Count; i < postsList.Count; i++) {
                saved.UnlockedPosts.Add(false);
                saved.LikedPosts.Add(false);
                if (!saved.InstaPostsAccounts.Contains(postsList[i].CharacterName)){
                    saved.InstaPostsAccounts.Add(postsList[i].CharacterName);
                    saved.UnlockedAccounts.Add(false);
                    saved.NumberOfPosts.Add(0);
                }
            }
        }
    }

    public void DestroyProfileList() {
        for (int i = 1; i < HeadShotList.childCount; i++) {
            Destroy(HeadShotList.GetChild(i).gameObject);
        }
    }

    public void GenerateProfileList() {
        for (int i = 0; i < saved.InstaPostsAccounts.Count; i++) {
            if (saved.UnlockedAccounts[i]) {
                int indexOfContact = i;
                Sprite pfp = Resources.Load("Images/Headshots/" + indexOfContact + saved.InstaPostsAccounts[i], typeof(Sprite)) as Sprite;
                Transform ProfileButtonClone = Instantiate(preFabs.ProfileButton, new Vector3(0, 0, 0), Quaternion.identity, HeadShotList);
                ProfileButtonClone.GetComponent<Image>().sprite = pfp;
                ProfileButtonClone.GetComponent<Button>().onClick.AddListener(() => {
                    ClearPostsList();
                    GenerateProfile(saved.InstaPostsAccounts[indexOfContact]);
                    SidePanel.GetComponent<Animator>().Play("Close-Side-Menu");
                    DestroyProfileList();
                });
            }
        }        
    }

    public void ClearPostsList() {
        for (int i = 0; i < PostsDisplay.childCount; i++) {
            Destroy(PostsDisplay.GetChild(i).gameObject);
        }
    }

    public void GenerateProfileHeader(int indexOfContact, Profile selectedProfile, int NumberOfPosts) {
        Sprite pfp = Resources.Load("Images/Headshots/" + indexOfContact + selectedProfile.Name, typeof(Sprite)) as Sprite;
        Transform ProfileHeaderClone = Instantiate(preFabs.InstaPostProfileHeader, new Vector3(0, 0, 0), Quaternion.identity, PostsDisplay);
        ProfileHeaderClone.GetChild(0).GetComponent<Image>().sprite = pfp;
        ProfileHeaderClone.GetChild(1).GetComponent<TextMeshProUGUI>().text = selectedProfile.Username;
        Transform InfoContainer = ProfileHeaderClone.GetChild(2).transform;
        InfoContainer.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = NumberOfPosts.ToString();
        InfoContainer.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = selectedProfile.Followers;
        InfoContainer.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = selectedProfile.Following;
        ProfileHeaderClone.GetChild(3).GetComponent<TextMeshProUGUI>().text = selectedProfile.ProfileInfo;
    }

    public void GenerateInstaPost(int indexOfContact, string Contact, Post post, int indxOfPost) {
        Sprite pfp = Resources.Load("Images/Headshots/" + indexOfContact + Contact, typeof(Sprite)) as Sprite;
        Sprite Photo = Resources.Load("Images/Photos/" + post.Image, typeof(Sprite)) as Sprite;

        Transform InstaPostCard = Instantiate(preFabs.InstaPostCard, new Vector3(0, 0, 0), Quaternion.identity, PostsDisplay);
        // Photo of the post
        InstaPostCard.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Photo;
        InstaPostCard.GetChild(1).GetChild(0).GetComponent<Image>().sprite = pfp;
        InstaPostCard.GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(() => {
            ClearPostsList();
            GenerateProfile(Contact);
        });
        InstaPostCard.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = post.UserName;
        InstaPostCard.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = post.Description;
        Transform NotLikedButton = InstaPostCard.GetChild(2).GetChild(2).transform;
        if (saved.LikedPosts[indxOfPost] == true) {
            gen.Hide(NotLikedButton);
        }
        // This is the red heart button
        InstaPostCard.GetChild(2).GetChild(1).GetComponent<Button>().onClick.AddListener(() => {
            saved.LikedPosts[indxOfPost] = false;
            gen.Show(NotLikedButton);
        });
        // this is the empty heart button
        NotLikedButton.GetComponent<Button>().onClick.AddListener(() => {
            saved.LikedPosts[indxOfPost] = true;
            gen.Hide(NotLikedButton);
        });
    }

    public void GenerateProfile(string nameOfProfileOwner) {
        List<Profile> ProfileList = GetProfilesList().Profiles;
        Profile selectedProfile = new Profile();
        int indexOfContact = saved.InstaPostsAccounts.IndexOf(nameOfProfileOwner);;
        for (int i = 0; i < ProfileList.Count; i++) {
            if (ProfileList[i].Name == nameOfProfileOwner) {
                selectedProfile = ProfileList[i];
            }
        }
        List<Post> postsList = GetPostsFile().Posts;

        GenerateProfileHeader(indexOfContact, selectedProfile, saved.NumberOfPosts[indexOfContact]);
        // Since posts in real life start from newest, this starts the iterator at the newest post and goes backwards to populate
        for (int i = saved.UnlockedPosts.Count-1; i > -1; i--) {
            if (saved.UnlockedPosts[i] && nameOfProfileOwner == postsList[i].CharacterName) {
                int indxOfPost = i;

                GenerateInstaPost(indexOfContact, nameOfProfileOwner, postsList[i], indxOfPost);   
            }
        }
        
    }

    public void DisplayAllPosts() {
        List<Post> postsList = GetPostsFile().Posts;

        // Since posts in real life start from newest, this starts the iterator at the newest post and goes backwards to populate
        for (int i = saved.UnlockedPosts.Count-1; i > -1; i--) {
            if (saved.UnlockedPosts[i]) {
                int indxOfPost = i;
                string Contact = postsList[i].CharacterName;
                int indexOfContact = saved.InstaPostsAccounts.IndexOf(Contact);
                Post post = postsList[i];

                GenerateInstaPost(indexOfContact, Contact, post, indxOfPost);
                
            }
        }
    }
}
