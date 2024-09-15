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
            string[] EmojiList = Directory.GetFiles("Assets/Resources/Images/Emojis","*.png");
            foreach (string Emoji in EmojiList) {
                Emojis.Add(Emoji[31..^4]);
            }
            string[] PhotoList = Directory.GetFiles("Assets/Resources/Images/Photos","*.png");
            foreach (string Photo in PhotoList) {
                Photos.Add(Photo[31..^4]);
            }
            string[] ContactList = Directory.GetFiles("Assets/Resources/Images/Headshots","*.png");
            foreach (string Contact in ContactList) {
                Contacts.Add(Contact[34..^4]);
            }
        }
    }
}
