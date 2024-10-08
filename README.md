# PhoneEngine
Chapter Schema:
Root :
    bool AllowMidrolls: Makes it so midrolls are allowed after the chapter, if midrolls don't make sense right after a chapter that ends on a cliff hanger you can 
        disable them here
    int StoryCheckpoint: See MidrollsList in DB for explination, Here you set the current story point (EG. AltheaHadSodaForTheFirstTime, place this into enum)

    List<SubChap> SubChaps: List of the subchapters

    SubChaps:
        string Contact: name of the contact you're pushing the messages to
        List<TextMessage> TextList: Texts to be recieved from that contact *if left empty it's considered a response only event, allows sending multiple texts      
            instead of always being the contact texting multiple times or even first.
        string UnlockInstaPostsAccount: name of insta post to unlock *can be empty string **must be name of person, not their @name ***If unlocking a post, must 
            include
        List<int> UnlockPosts: list of indexes of posts to unlock from the instaposts JSON *can be empty
        Responses Responses: Object of the responses after all the auto texts are recieved

        TextMessage:
            int Type: the type of text to be sent, used for figuring out which prefab to use for your message push, enum is below
            int Tendency: not yet implemented, will change behaviour of characters through implemented logic
            string TextContent: the text being sent *if you're sending an image you place the name here
            float TextDelay: the delay you want in recieving the texts, doesn't make sense to recieve 20 texts in 0.2 seconds

        Responses:
            List<Response> RespContent: Object of the actual responses to be displayed

            Response:
                bool RespTree: allows you to make response trees, EG. "I want to...", "I wish you would..." and then in the next choice it gives more detail. This 
                    will ensure the initial ... text doesn't get sent after click. After you've finished the tree which is indicated by a response with the RespTree set to false does it send the final message.
                string TextContent: the text to be displayed on the button or sent, if you're sending an image or emoji you place the name of it here
                int SubChapNum: the index number of the next subchapter to play after making your choice
                int Type: the type of text to be sent, used for figuring out which prefab to use for your message push, enum is below


<!-- All Chapters need to be placed into the Resources/Chapters directory
MidRolls are the exact same schema, just place them into the Resources/Midrolls for separation and count purposes -->

Types of text Enum:
    sentText: 0
    recText: 1 (Recieved text)
    sentImage: 2
    recImage: 3 (Recieved Image)
    sentEmoji: 4
    recEmoji: 5 (Recieved Emoji)
    chapEnd: 6
    indicateTime: 8

<!-- Unity sadly doesn't have a smarter way of doing this as far as I could find and this is a decent way of centralising important information -->
DB (found in the Resources directory): 
    List<string> ContactList: the list of the names of the contacts for messaging
    List<string> ChapterList: list of the names of the chapter files, it doesn't matter what you call them as long as they match the names in your Chapters directory
    List<string> PhotoList: Schema matters here, it goes CategoryName-ActualNameOfImage. This way the gallary app can categorise them for you
    List<DBMidRoll> MidrollsList: the names of the midrolls as they appear in the Midrolls directory.

        DBMidRoll:
            string MidrollName: Basically the chapter name, no specific format needed
            int Checkpoint: A number that the enum StoryCheckpoint will use for determining if the random event should happen at this point in the story
                (Explination: A character drinks soda at a major plot point, create an index of that plotpoint in enum, add it to chapterNode.cs, 
                the game will then check to see if you're at or past that event so as to not display options where the dialogue may conflict with current
                Story points)



InstaPosts-Profiles (found in the Resources directory):
    List<Profile> Profiles: an object with the profile information of people

    Profile:
        string Name: Name of the person who the profile belongs to
        string Username: The username of the person
        string Followers: The number of people that follow them, left as a string so you can just add k or m next to a big number if they're popular, don't recommend 
            using numbers larger than 3 digits, 4 max
        string Following: same thing as followers but for the number of people they follow
        string ProfileInfo: Their bio for the account

InstaPosts-Posts (found in the Resources directory):
    List<Post> Posts: the object containing the info about the post

    Post:
        int Index: Only there so you know what the index of the post is for the sake of unlocking it later
        string CharacterName: the actual name of the person, used for finding which posts belong to which contact
        string UserName: Less efficient to type in twice, way more efficient than loading a whole second json file just to get a single value though
        string Image: Just the image name, full thing CategoryName-ActualNameOfImage EG. Althea-beach1
        string Description: the small text under posts that you'd see under an instagram post.

Resources/Images:
    ChapImages: Place Image you want associated with the chapter here, make sure it has the same name as the chapter EG. Chapter1.json would be Chapter1.png
    Emojis: The emojis avaliable, the ones preloaded are all from X/Twitter which are open source, I recommend using them
    Headshots: Place a headshot of the contact here with the name of the contact being the name of the headshot .png EG. Althea.png
    Photos: The actual photos you want to use, again use the schema mentioned above CategoryName-ActualNameOfImage EG. Althea-beach1
    Settings: I used this one to drop in sound files for clicks and such, the ones in right now are open source or screen recorded from my phone.
    