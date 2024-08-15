using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstaPostsManager : MonoBehaviour
{

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

    public PostsList GetPostsList() {
        TextAsset InstaPostsFile = Resources.Load<TextAsset>("instaPosts-posts");
        postsList = JsonUtility.FromJson<PostsList>(InstaPostsFile.text);
        return postsList;
    }

    /*
    Create List for enabled and list for liked
    create empty list of indexes to push to the app when it's loaded
    populate list with index of post based on subchap content if it includes an index of a post to unlock    

    */

}
