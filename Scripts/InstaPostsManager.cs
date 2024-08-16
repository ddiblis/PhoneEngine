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

    PostsList postsList = new PostsList();

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
                }
            }
        }
    }

    public void PopulateList() {

    }

    public void closeApp() {
        for (int i = 0; i < PostsDisplay.childCount; i++) {
            Destroy(PostsDisplay.GetChild(i).gameObject);
        }
    }

    public void OpenInstaApp() {
        List<Post> postsList = GetPostsFile().Posts;

        // Since posts in real life start from newest, this starts the iterator at the newest post and goes backwards to populate
        for (int i = saved.UnlockedPosts.Count-1; i > -1; i--) {
            if (saved.UnlockedPosts[i]) {
                int indxOfPost = i;
                string Contact = postsList[i].CharacterName;
                int indexOfContact = saved.InstaPostsAccounts.IndexOf(Contact);


                Sprite pfp = Resources.Load("Images/Headshots/" + indexOfContact + Contact, typeof(Sprite)) as Sprite;
                Sprite Photo = Resources.Load("Images/Photos/" + postsList[i].Image, typeof(Sprite)) as Sprite;

                Transform InstaPostCard = Instantiate(preFabs.InstaPostCard, new Vector3(0, 0, 0), Quaternion.identity, PostsDisplay);
                // Photo of the post
                InstaPostCard.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Photo;
                InstaPostCard.GetChild(1).GetChild(0).GetComponent<Image>().sprite = pfp;
                InstaPostCard.GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(() => {});
                InstaPostCard.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = postsList[i].UserName;
                InstaPostCard.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = postsList[i].Description;
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
        }
        // Sprite Photo = Resources.Load("Images/Photos/"+ "name here", typeof(Sprite)) as Sprite;
        // Sprite pfp = Resources.Load("Images/Headshots/"+ "name here", typeof(Sprite)) as Sprite;

        // Transform InstaPostCard = Instantiate(preFabs.InstaPostCard, new Vector3(0, 0, 0), Quaternion.identity, PostsDisplay);
        // // Photo of the post
        // InstaPostCard.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Photo;
        // InstaPostCard.GetChild(1).GetComponent<Image>().sprite = pfp;
        // InstaPostCard.GetChild(2).GetComponent<TextMeshProUGUI>().text = "username";
        // InstaPostCard.GetChild(3).GetComponent<TextMeshProUGUI>().text = "post content here";
        // InstaPostCard.GetChild(4).GetComponent<Button>().onClick.AddListener(() => {});

    }

    /*
    Create List for enabled and list for liked
    create empty list of indexes to push to the app when it's loaded
    populate list with index of post based on subchap content if it includes an index of a post to unlock    
    */

}
