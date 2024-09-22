using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace JSONMapper {
    public class CustomLists {
            public readonly List<string> Emojis = new() {
                "Emojis"
            };
            public readonly List<string> Photos = new() {
                "Photos"
            };
            public readonly List<string> Contacts = new() {
                "Contacts"
            };
        public CustomLists() {
            string EmojiDirectory = $"{Application.dataPath}/Resources/Images/Emojis";
            if (!Directory.Exists(EmojiDirectory)) {
                Directory.CreateDirectory(EmojiDirectory);
                Debug.LogWarning("Emoji Directory not found, created one, insert emojis here");
            }
            string[] EmojiList = Directory.GetFiles("Assets/Resources/Images/Emojis","*.png");
            foreach (string Emoji in EmojiList) {
                Emojis.Add(Emoji[31..^4]);
            }

            string PhotosDirectory = $"{Application.dataPath}/Resources/Images/Photos";
            if(!Directory.Exists(PhotosDirectory)) {
                Directory.CreateDirectory(PhotosDirectory);
                Debug.LogWarning("Photos Directory not found, created one, insert photos here");
            }
            string[] PhotoList = Directory.GetFiles("Assets/Resources/Images/Photos","*.png");
            foreach (string Photo in PhotoList) {
                Photos.Add(Photo[31..^4]);
            }

            string ContactsDirectory = $"{Application.dataPath}/Resources/Images/Headshots";
            if(!Directory.Exists(ContactsDirectory)) {
                Directory.CreateDirectory(ContactsDirectory);
                Debug.LogWarning("Contacts Directory not found, created one, Drop headshots of contacts here");
            }
            string[] ContactList = Directory.GetFiles("Assets/Resources/Images/Headshots","*.png");
            foreach (string Contact in ContactList) {
                Contacts.Add(Contact[34..^4]);
            }
        }
    }
}
