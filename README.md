# PhoneEngine
Chapter Schema:
Root :
    bool AllowMidrolls: makes it so midrolls are allowed after the chapter, if midrolls don't make sense right after a chapter that ends on a cliff hanger you can disable them here

    List<SubChap> SubChaps: List of the subchapters

    SubChaps:
        int SubChapNum: not parsed by C#, Only there to help you with figuring out which subchap number to use on choice click
        string Contact: name of the contact you're pushing the messages to
        string TimeIndicator: simple middle notification in message list that indicates the fictional times passed *can be empty string
        List<TextMessage> TextList: Texts to be recieved from that contact *if left empty it's considered a response only event, allows sending multiple texts instead of always being the contact texting multiple times or even first.
        string UnlockInstaPostsAccount: name of insta post to unlock *can be empty string **must be name of person, not their @name
        List<int> UnlockPosts: list of indexes of posts to unlock from the instaposts JSON *can be empty
        Responses Responses: Object of the responses after all the auto texts are recieved

        TextMessage:
            int Type: the type of text to be sent, used for figuring out which prefab to use for your message push, enum is below
            int Tendency: not yet implemented, will change behaviour of characters through implemented logic
            string TextContent: the text being sent *if you're sending an image you place the name here
            float TextDelay: the delay you want in recieving the texts, doesn't make sense to recieve 20 texts in 0.2 seconds

        Responses:
            bool RespTree: allows you to make response trees, EG. "I want to...", "I wish you would..." and then in the next choice it gives more detail. This will ensure the initial ... text doesn't get sent after click and only after you've finished the tree indicated by a subchap with the RespTree set to false does it sent the final message.
            List<Response> RespContent: Object of the actual responses to be displayed

            Response:
                string TextContent: the text to be displayed on the button or sent, if you're sending an image or emoji you place the name of it here
                int SubChapNum: the index number of the next subchapter to play after making your choice
                int Type: the type of text to be sent, used for figuring out which prefab to use for your message push, enum is below


<!-- All Chapters need to be placed into the Resources/Chapters directory
MidRolls are the exact same schema, just place them into the Resources/Midrolls for separation and count purposes -->

<!-- You will never need to use enum 8, it's automatically detected if theres a TimeIndicator of length more than 0 in the current SubChap -->
Types of text Enum:
    sentText: 0
    recText: 1
    sentImage: 2
    recImage: 3
    sentEmoji: 4
    recEmoji: 5
    chapEnd: 6
    indicateTime: 8

<!-- Unity sadly doesn't have a smarter way of doing this as far as I could find and this is a decent way of centralising important information -->
DB (found in the Resources directory): 
    List<string> ContactList: the list of the names of the contacts for messaging
    List<string> ChapterList: list of the names of the chapter files, it doesn't matter what you call them as long as they match the names in your Chapters directory
    List<string> PhotoList: Schema matters here, it goes CategoryName-ActualNameOfImage. This way the gallary app can categorise them for you
    List<string> MidrollsList: the names of the midrolls as they appear in the Midrolls directory.

InstaPosts-Profiles (found in the Resources directory):
    List<Profile> Profiles: an object with the profile information of people

    Profile:
        string Name: Name of the person who the profile belongs to
        string Username: The username of the person
        string Followers: The number of people that follow them, left as a string so you can just add k or m next to a big number if they're popular, don't recommend using numbers larger than 3 digits, 4 max
        string Following: same thing as followers but for the number of people they follow
        string ProfileInfo: Their bio for the account

InstaPosts-Posts (found in the Resources directory):
    List<Post> Posts: the object containing the info about the post

    Post:
        string CharacterName: the actual name of the person, used for finding which posts belong to which contact
        string UserName: Less efficient to type in twice, way more efficient than loading a whole second json file just to get a single value though
        string Image: Just the image name, full thing CategoryName-ActualNameOfImage
        string Description: the small text under posts that you'd see under an instagram post.

Resources/Images:
    ChapImages: Place Image you want associated with the chapter here, make sure it has the same name as the chapter EG. Chapter1.json would be Chapter1.png
    Emojis: The emojis avaliable, the ones preloaded are all from X/Twitter which are open source, I recommend using them
    Headshots: Place a headshot of the contact here with the name of the contact being the name of the headshot .png
    Photos: The actual photos you want to use, again use the schema mentioned above CategoryName-ActualNameOfImage
    Settings: I used this one to drop in sound files for clicks and such, the ones in right now are open source or screen recorded from my phone.
    