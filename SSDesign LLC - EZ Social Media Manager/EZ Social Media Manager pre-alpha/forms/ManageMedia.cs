using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using SSDesign.MediaAPI;

/// <summary>
/// READ ME NOTES
/// 
/// The exception bug where it says value cannot be null, was from it finding the posts about doing a new cover photo, but there
/// were no words invloved in the post. In the future, I need to check if ANYTHING is in the post, not just a message
/// 
/// there is a for loop in refresh messages. It is in reverse order. This might mess up the selection of messages, though i cant say 
/// it really matters that a message is selected since you only reply to the conversation
/// 
/// THIS IS A NOTE FOR JANUARY 2019 - MAKE THE REFRESH BUTTONS INDEPENDENT OF THE ENTIRE WINFORMS STUFF AND MAKE THEM PART OF API TOOLS
/// THIS WILL DRAMATICALLY IMPROVE THE ORGANIZATIONAL STRUCTURE OF THE PROGRAM AND MAKE IT ALOT EASIER TO MAINTAIN <----------------------
/// 
/// 
/// </summary>

namespace EZ_Social_Media_Manager_pre_alpha
{
    public partial class ManageMedia : Form
    {
        private string _startingMedia { get; set; }

        private string SelectedObjectID { get; set; }
        private string PostingEdge { get; set; }

        private Timer _standardRefreshTimer;
        private /*System.Threading.*/Timer _messageRefreshTimer;

        private Size _originalSize;

        //private SoundPlayer _alertTone;

        private const int COMMENT_REPLIES_BACKBUTTON_INDEX = 0;
        private const int NULL_INDEX = -1;

        private delegate void VoidObjectDelegate(object state);

        public ManageMedia()
        {
            InitializeComponent();

            _standardRefreshTimer = new Timer();
            _messageRefreshTimer = new Timer();

            PostingEdge = "comments";

            FBPostsList.IntegralHeight = false;
            FBCommentsList.IntegralHeight = false;

            //_alertTone = new SoundPlayer($"{AppDomain.CurrentDomain.BaseDirectory}messagealert.wav");
        }
        public ManageMedia(string loadMedia) : this()
        {
            _startingMedia = loadMedia;
        }

        private void ManageMedia_Load(object sender, EventArgs e)
        {
            IG_Notafier.Text = "";

            UpdateText(ref Sent_Notafication_Label, "");
            UpdateText(ref FBPostsReplyBox, "(Nothing selected)", Color.Gray);
            UpdateText(ref FBMessageReplyBox, "(Nothing selected)", Color.Gray);

            //auto refresh feature
            _standardRefreshTimer.Interval = EZ_Social_Media_Manager_pre_alpha.Properties.Settings.Default.RefreshInterval * 1000;
            _standardRefreshTimer.Tick += new EventHandler(RefreshFields);
            _standardRefreshTimer.Start();
            _messageRefreshTimer.Interval = EZ_Social_Media_Manager_pre_alpha.Properties.Settings.Default.RefreshMessageInterval * 1000;
            _messageRefreshTimer.Tick += new EventHandler(RefreshJustMessages);
            _messageRefreshTimer.Start();

            Facebook_Button.Text = $"\n\n{FacebookAPIInfo.FBAccount.name}";

            _originalSize = Size;

            if (FacebookAPIInfo.FBAccount != null)
            {
                for (int i = 0; i < FacebookAPIInfo.FBPages.Count; ++i)
                {
                    Page_Selector.Items.Add($"{FacebookAPIInfo.FBPages[i].name} - {FacebookAPIInfo.FBPages[i].instagram_business_account.name}");
                    IG_Account_DropDown.Items.Add($"{FacebookAPIInfo.FBPages[i].name} - {FacebookAPIInfo.FBPages[i].instagram_business_account.name}");

                    for (int j = 0; j < FacebookAPIInfo.FBPages[i].conversations.data.Count; ++j)
                    {
                        Conversations.Items.Add(FacebookAPIInfo.FBPages[i].conversations.data[j].id);
                    }
                }

                if (Page_Selector.Items.Count >  0)
                { //possibly make these one liners
                    Page_Selector.SelectedIndex = 0;
                }
                if (IG_Account_DropDown.Items.Count > 0)
                {
                    IG_Account_DropDown.SelectedIndex = 0;
                }

                Facebook_Reaction_DropDown.Items.AddRange(new[] { "👍🏻", "❤️", "😆", "😮", "😢", "😡" });
                Facebook_Reaction_DropDown.SelectedIndex = 0;
                Instagram_Reaction_DropDown.Items.AddRange(new[] { "👍🏻", "❤️", "😆", "😮", "😢", "😡" });
                Instagram_Reaction_DropDown.SelectedIndex = 0;
            }

            DisableAllPanels();
            switch (_startingMedia)
            {
                case "Facebook":
                    FBMainPanel.Visible = true;
                    break;
                case "Instagram":
                    Instagram_Panel.Visible = true;
                    break;
                default:
                    FBMainPanel.Visible = true;
                    break;
            }

            if (Properties.Settings.Default.DarkTheme)
            {
                DarkTheme();
            }
        }
        private void DarkTheme()
        {
            //main window
            BackColor = Color.DimGray;
            FBMainPanel.BackColor = Color.Gray;
            Instagram_Panel.BackColor = Color.Gray;
            MediaScrollPanel.BackColor = Color.Gray;

            //facebook panel things
            Conversations_Label.ForeColor   = Color.LightGray;
            Message_Panel_Label.ForeColor   = Color.LightGray;
            Post_Box_Label.ForeColor        = Color.LightGray;
            Comment_Box_Label.ForeColor     = Color.LightGray;
            Page_Selector_Label.ForeColor   = Color.LightGray;
            Likes_Label.ForeColor           = Color.LightGray;
            Like_Count.ForeColor            = Color.LightGray;

            foreach (Panel Control in FBMainPanel.Controls)
            {
                Control.BackColor = Color.DimGray;

                foreach (Control panelControl in Control.Controls)
                {
                    if (panelControl is ListBox || panelControl is RichTextBox)
                    {
                        panelControl.BackColor = (Color)new ColorConverter().ConvertFromString("#565656"); ;
                        panelControl.ForeColor = Color.LightGray;
                    }
                    else if (panelControl is Button)
                    {
                        panelControl.BackColor = Color.DimGray;
                        panelControl.ForeColor = Color.LightGray;
                    }
                }
            }

            //instagram panel things
            label4.ForeColor = Color.LightGray;
            label5.ForeColor = Color.LightGray;
            label7.ForeColor = Color.LightGray;
            label8.ForeColor = Color.LightGray;
            Account_Label.ForeColor = Color.LightGray;
            Followers_Count_Label.ForeColor = Color.LightGray;
            IG_Followers_Count.ForeColor = Color.LightGray;
           
            foreach (Panel Control in Instagram_Panel.Controls)
            {
                Control.BackColor = Color.DimGray;
            }
        }

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResizeEnd(e);

            if (_originalSize != Size.Empty)
            {
                if (FBMainPanel.Visible)
                {
                    var deltaSizeWidth = (Size.Width - _originalSize.Width);
                    var deltaSizeHeight = (Size.Height - _originalSize.Height);

                    MediaScrollPanel.Height += deltaSizeHeight;
                    FBPostsPanel.Width += deltaSizeWidth;
                    FBPostsPanel.Height += deltaSizeHeight;
                    FBConversationPanel.Width = FBPostsPanel.Width;

                    FBMainPanel.Width += deltaSizeWidth;
                    FBMainPanel.Height += deltaSizeHeight;

                    FBPostsList.Height += deltaSizeHeight;
                    FBCommentsList.Height = FBPostsList.Height;

                    FBInsightPanel.Width = ((FBPostsPanel.Width - 10) / 2);
                    panel5.Width = FBInsightPanel.Width;

                    FBCommentsList.Width = ((FBPostsPanel.Width - 14) / 2);
                    FBPostsList.Width = FBCommentsList.Width;
                    FBMessagesList.Width += deltaSizeWidth;

                    FBPostsReplyBox.Width += deltaSizeWidth;
                    FBMessageReplyBox.Width += deltaSizeWidth;

                    _originalSize = Size;
                }
                else if (Instagram_Panel.Visible)
                {

                }
            }
        }

        private void Pages_SelectedIndexChanged(object sender = null, EventArgs e = null)
        {
            IG_Account_DropDown.SelectedIndex = Page_Selector.SelectedIndex;

            ClearFields();
            RefreshPosts(false);
            RefreshConversations(false);

            //RefreshFields(new object(), new EventArgs());
            //UpdateInsights(FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex]);
        }
        private void Posts_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if the user selects the last index in the posts, he or she wants to load more posts...
            if ((FBPostsList.SelectedIndex == (FBPostsList.Items.Count - 1) && FBPostsList.SelectedIndex != -1) && (FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].posts.paging.next != ""))
            {
                FBPostsList.Items.RemoveAt(FBPostsList.Items.Count - 1);
                FBPostsList.Items.Add("Loading more posts...");

                var getNextPost = FacebookAPIInfo.facebookService.GetFacebookObject<FacebookMessageObject>(

                    access_token:   FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].access_token,
                    objectID:       FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].id,
                    edge:           "feed",
                    arguments:      $"fields=id,from,message,created_time&limit=25&after={FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].posts.paging.cursors.after}"

                );
                Task.WaitAll(getNextPost);

                if (getNextPost.Result.paging != null)
                {
                    FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].posts.paging = getNextPost.Result.paging;
                }
               
                FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].posts.data.AddRange(getNextPost.Result.data);

                FBPostsList.Items.RemoveAt(FBPostsList.Items.Count - 1);
                for (int i = FBPostsList.Items.Count; i < FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].posts.data.Count; ++i)
                {
                    FBPostsList.Items.Add($"[{FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].posts.data[i].likes.summary.total_count} like(s)]> \"{FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].posts.data[i].message ?? "null"}\"");
                }

                if (FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].posts.paging.next != "" && FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].posts.paging.next != null)
                {
                    FBPostsList.Items.Add($"Click to load more posts...");
                }
            }

            RefreshComments();
            Comment_Replies.Visible = false;

            if (FBPostsList.SelectedIndex != NULL_INDEX)
            {
                UpdateText(ref FBPostsReplyBox, FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].posts.data[FBPostsList.SelectedIndex].message, Color.Gray);
                UpdateSelectedObjectID(FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].posts.data[FBPostsList.SelectedIndex].id);

                FBPostsReplyBox.SelectionStart = 0;
                FBPostsReplyBox.SelectionLength = FBPostsReplyBox.Text.Length;
            }
        }
        private void Conversations_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((Conversations.SelectedIndex != -1) && (Conversations.SelectedIndex == (Conversations.Items.Count - 1)) && (FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].conversations.paging.next != ""))
            {
                Conversations.Items.RemoveAt(Conversations.Items.Count - 1);
                Conversations.Items.Add("Loading more conversations...");

                var conversationTask = FacebookAPIInfo.facebookService.GetFacebookObject<FacebookConversation>(

                    access_token: FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].access_token,
                    objectID: FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].id,
                    edge: "conversations",
                    arguments: $"fields=id,updated_time,participants&limit=25&after={FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].conversations.paging.cursors.after}"

                );
                Task.WaitAll(conversationTask);

                if (conversationTask.Result.paging != null)
                {
                    FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].conversations.paging = conversationTask.Result.paging;
                }

                FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].conversations.data.AddRange(conversationTask.Result.data);

                Conversations.Items.RemoveAt(Conversations.Items.Count - 1);
                for (int i = Conversations.Items.Count; i < FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].conversations.data.Count; ++i)
                {
                    string participants = "";

                    for (int j = 0; j < FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].conversations.data[i].participants.data.Count; ++j)
                    {
                        participants += $"{FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].conversations.data[i].participants.data[j].name}, ";
                    }

                    Conversations.Items.Add(((participants != "") ? participants : "(Unknown)"));
                }

                Conversations.Items.Add($"Click to load more conversations...");
            }

            RefreshMessages();
        }
        private void Comments_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((FBCommentsList.SelectedIndex != -1) && (FBCommentsList.SelectedIndex == (FBCommentsList.Items.Count - 1))) && (FacebookAPIInfo.FBCommentCache.paging.next != "" && FacebookAPIInfo.FBCommentCache.paging.next != null))
            {
                FBCommentsList.Items.RemoveAt(FBCommentsList.Items.Count - 1);
                FBCommentsList.Items.Add("Loading more comments...");

                var getNextComment = FacebookAPIInfo.facebookService.GetFacebookObject<FacebookMessageObject>(

                    access_token: FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].access_token,
                    objectID: FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].posts.data[FBPostsList.SelectedIndex].id,
                    edge: "comments",
                    arguments: $"fields=id,from,message,created_time&limit=25&after={FacebookAPIInfo.FBCommentCache.paging.cursors.after}"

                );
                Task.WaitAll(getNextComment);

                if (getNextComment.Result.paging != null)
                {
                    FacebookAPIInfo.FBCommentCache.paging = getNextComment.Result.paging;
                }

                FacebookAPIInfo.FBCommentCache.data.AddRange(getNextComment.Result.data);

                FBCommentsList.Items.RemoveAt(FBCommentsList.Items.Count - 1);
                for (int i = FBCommentsList.Items.Count; i < FacebookAPIInfo.FBCommentCache.data.Count; ++i)
                {
                    FBCommentsList.Items.Add($"{((FacebookAPIInfo.FBCommentCache.data[i].from != null) ? FacebookAPIInfo.FBCommentCache.data[i].from.name : "(Unknown)")}: [{FacebookAPIInfo.FBCommentCache.data[i].likes.summary.total_count} like(s)]> \"{FacebookAPIInfo.FBCommentCache.data[i].message ?? "null"}\"");
                }

                if (FacebookAPIInfo.FBCommentCache.paging.next != "" && FacebookAPIInfo.FBCommentCache.paging.next != null)
                {
                    FBCommentsList.Items.Add($"Click to load more comments...");
                }
            }

            //if a comment is selected, update the textbox with its text
            if (FBCommentsList.SelectedIndex != NULL_INDEX)
            {
                UpdateText(ref FBPostsReplyBox, FacebookAPIInfo.FBCommentCache.data[FBCommentsList.SelectedIndex].message, Color.Gray);
                UpdateSelectedObjectID(FacebookAPIInfo.FBCommentCache.data[FBCommentsList.SelectedIndex].id);
            }
        }
        private void Comment_Replies_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Comment_Replies.SelectedIndex == COMMENT_REPLIES_BACKBUTTON_INDEX)
            {
                Comment_Replies.Visible = false;
            }
        }    
        private void Messages_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateText(ref FBMessageReplyBox, FacebookAPIInfo.FBMessageCache.data[FBMessagesList.Items.Count - FBMessagesList.SelectedIndex - 1].message);
        }

        private void Posts_Comments_Reply_Text_Box_Clicked(object sender, EventArgs e)
        {
            //Reply_Post_Comment_Button.Enabled = (Posts_Comments_Reply_Text_Box.Text.Length > 0);
            AcceptButton = Reply_Post_Comment_Button;

            UpdateText(ref FBPostsReplyBox, "", Color.Black);
        }
        private void Message_Reply_Box_Clicked(object sender, EventArgs e)
        {
            AcceptButton = Messaging_Reply_Button;

            UpdateText(ref FBMessageReplyBox, "", Color.Black);
        }

        private void Comments_DoubleClicked(object sender, EventArgs e)
        {
            Comment_Replies.Visible = true;
            RefreshCommentReplies();
        }
        
        private void SendReply(object sender, EventArgs e)
        {
            if (SelectedObjectID == null)
            {
                UpdateText(ref Sent_Notafication_Label, "Please select a post to comment on!");
                return;
            }
            if (FBPostsReplyBox.Text.Length <= 0)
            {
                UpdateText(ref Sent_Notafication_Label, "Please write a message to post!");
                return;
            }

            FacebookAPIInfo.ReinitializeFacebookService();
            UpdateText(ref Sent_Notafication_Label, "Sending post...");

            FacebookAPIInfo.CreateComment(Page_Selector.SelectedIndex, SelectedObjectID, FBPostsReplyBox.Text, PostingEdge);

            if (FBCommentsList.SelectedIndex == NULL_INDEX)
            {
                RefreshComments();
            }
            else
            {
                RefreshCommentReplies();
            }

            UpdateText(ref Sent_Notafication_Label, "Your post has been sent!");
            UpdateText(ref FBPostsReplyBox, "", Color.Black);
        }
        private void SendMessage(object sender, EventArgs e)
        {
            if (Conversations.SelectedIndex == -1)
            {
                FBMessageReplyBox.Text = "";
                return;
            }

            FacebookAPIInfo.ReinitializeFacebookService();
            //eventually greatne the scope of this, and call it selectedObject
            string objectID = FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].conversations.data[Conversations.SelectedIndex].id;
            UpdateText(ref Sent_Notafication_Label, "Sending message...");

            FacebookAPIInfo.SendMessage(Page_Selector.SelectedIndex, objectID, FBMessageReplyBox.Text);
            RefreshMessages();

            UpdateText(ref Sent_Notafication_Label, "Your message has been sent!");
            FBMessageReplyBox.Text = "";
        }
        //AT SOME POINT MAKE THIS ONLY USED BY THE REFRESH BUTTON AND THE AUTOMATIC REFRESH FEATURE
        private void RefreshFields(object sender, EventArgs e)
        {
            RefreshPosts();
            RefreshComments();
            RefreshConversations(); //TEMPORARY UNTIL WE MAKE THE REFRESH RUN ON MULTIPLE THREADS!!!
            RefreshMessages();
            UpdateSelectedObjectID(null);

            RefreshIGPosts();
            RefreshIGComments();

            PostingEdge = "comments";
            IG_PostingEdge = "comments";
        }
        private void ClearFields()
        {
            FBPostsList.Items.Clear();
            FBCommentsList.Items.Clear();
            Conversations.Items.Clear();
            FBMessagesList.Items.Clear();
        }

        /// <summary>
        /// Gets and lists posts from the API onto the Posts listbox
        /// </summary>
        /// <param name="reload">Whether or not you want this to get new posts from API</param>
        private void RefreshPosts(bool reload = true)
        {
            FBPostsList.Items.Clear();

            if (Page_Selector.SelectedIndex == NULL_INDEX)
                return;

            FacebookPage page = FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex];

            // If the caller wants to reload from the API, refresh the posts cache for the selected page
            if (reload)
            {
                FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].posts = Task.Run(() => FacebookAPIInfo.facebookService.GetFacebookObject<FacebookMessageObject>(page.access_token, page.id, "feed")).Result;
                page = FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex];
            }

            // Iterate through the page cache of the selected page and add the posts into the posts list
            for (int i = 0; i < page.posts.data.Count; ++i)
            {
                // Make sure that the post actually contains information worth reporting
                if (page.posts.data[i] != null)
                {
                    FBPostsList.Items.Add($"[{page.posts.data[i].likes.summary.total_count} like(s)]> \"{page.posts.data[i].message}\"");
                }
            }

            // If it is greater than the amount specified in the settings...
            if (!string.IsNullOrEmpty(page.posts.paging.next))
            {
                FBPostsList.Items.Add($"Click to load more posts...");
            }
        }

        /// <summary>
        /// Gets and lists comments from the API onto the Comments listbox 
        /// </summary>
        private async void RefreshComments()
        {
            FBCommentsList.Items.Clear();

            if (FBPostsList.SelectedIndex == NULL_INDEX)
                return;

            // Get the new comment from the API
            var comments = await FacebookAPIInfo.facebookService.GetFacebookObject<FacebookMessageObject>(

                access_token: FacebookAPIInfo.userAccessToken,
                objectID: FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].posts.data[FBPostsList.SelectedIndex].id,
                edge: "comments",
                arguments: "fields=id,created_time,from,message,likes.summary(true)"

            );

            // Update the comment cache
            FacebookAPIInfo.FBCommentCache = comments;

            // Iterate through the new set of comments and add them into the comment list
            for (int i = 0; i < comments.data.Count; ++i)
            {
                //check to make sure a reply had a message in it, and not just a picture
                if (comments.data[i] != null)
                {
                    FBCommentsList.Items.Add($"{((comments.data[i].from != null) ? comments.data[i].from.name : "(Unknown)")}: [{comments.data[i].likes.summary.total_count} like(s)]> \"{comments.data[i].message}\"");
                }
            }

            // If there is paging data, notify the user with a 'load more' to load the rest of the data
            if (!string.IsNullOrEmpty(FacebookAPIInfo.FBCommentCache.paging.next))
            {
                FBCommentsList.Items.Add($"Click to load more comments...");
            }
        }
        private void RefreshCommentReplies()
        {
            if (FBCommentsList.SelectedIndex != -1)
            {
                List<FacebookMessageObject> replies = FacebookAPIInfo.GetCommentReplies(FBCommentsList.SelectedIndex);

                Comment_Replies.Items.Clear();

                Comment_Replies.Items.Add("(Back)");
                for (int i = 0; i < replies.Count; ++i)
                {
                    Comment_Replies.Items.Add($"{((replies[i].from != null) ? replies[i].from.name : "(Unknown)")}: \"{replies[i].message}\"");
                }
            }
        }
        private async void RefreshConversations(bool reload = true)
        {
            if (Page_Selector.SelectedIndex != -1)
            {
                FacebookPage page = FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex];

                if (reload)
                {
                    var conversations = await FacebookAPIInfo.facebookService.GetFacebookObject<FacebookConversation>(page.access_token, page.id, "conversations", "fields=updated_time,participants");
                    //the below line isnt exactly comprehensive and will need modified in the future to fit all the use cases
                    if (FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].conversations.data.Count != conversations.data.Count)
                    {
                        FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].conversations = conversations;
                    }
                    else if (Conversations.Items.Count > conversations.data.Count)
                    {
                        return;
                    }

                    page = FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex];
                }

                Conversations.Items.Clear();
                for (int i = 0; i < page.conversations.data.Count; ++i)
                {
                    string participants = "";

                    for (int j = 0; j < page.conversations.data[i].participants.data.Count; ++j)
                    {
                        participants += $"{page.conversations.data[i].participants.data[j].name}, ";
                    }

                    Conversations.Items.Add(((participants != "") ? participants : "(Unknown)"));                 
                }

                if (!string.IsNullOrEmpty(FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].conversations.paging.next))
                {
                    Conversations.Items.Add($"Click to load more conversations...");
                }
            }
        }
        private async void RefreshMessages()
        {
            //Possibly restart the damn timer here

            if (Conversations.SelectedIndex == NULL_INDEX)
                return;
            
            var messages = await FacebookAPIInfo.facebookService.GetFacebookObject<FacebookMessageObject>(

                access_token:   FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].access_token, 
                objectID:       FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].conversations.data[Conversations.SelectedIndex].id, 
                edge:           "messages", 
                arguments:      "fields=id,message,from"
                    
            );

            if (FacebookAPIInfo.FBMessageCache.data.Count == 0 || FBMessagesList.Items.Count == 0 || (FacebookAPIInfo.FBMessageCache.data[0].id != messages.data[0].id))
            { //the latter statement in the if is to check whether or not the managemedia is initially loading or just updating
                FacebookAPIInfo.FBMessageCache.data = messages.data;

                if (!((FBMessagesList.Items.Count == 0) && (messages.data.Count > 0)))
                { //if its not the inital startup, play the message alert tone if the lists are different
                    //_alertTone.Play();
                }

                FBMessagesList.Items.Clear();
                for (int i = messages.data.Count - 1; i >= 0; --i)
                { //check to make sure a reply had a message in it, and not just a picture
                    if (messages.data[i] != null)
                    {
                        FBMessagesList.Items.Add($"{((messages.data[i].from.name != null) ? messages.data[i].from.name : "(Unknown)")}: \"{messages.data[i].message}\"");
                    }
                }

                //dont need a load more button because scrolling up on the list should load them automatically
            }
            
        
            FBMessagesList.SelectedIndex = FBMessagesList.Items.Count - 1;
        }
        //AT SOME POINT, MAKE IT SO IF THE REFRESHED MESSAGES ARE THE SAME AS THE OLD ONES, IT DOESNT REFRESH THE LIST BOX AND MAKE IT BLINK
        private void RefreshJustMessages(object sender, EventArgs e)
        {
            int conversationIndex = Conversations.SelectedIndex;

            // Do NOT make the following awaited as controls cannot operate on a different thread then they were created on
            RefreshConversations();
            RefreshMessages();

            try
            {
                Conversations.SelectedIndex = conversationIndex;
                FBMessagesList.SelectedIndex = FBMessagesList.Items.Count - 1;
            }
            catch
            {
                return;
            }
        }

        private static void UpdateText<T>(ref T @object, string message, Color? textColor = null) where T : class
        {
            (@object as Control).ForeColor = textColor ?? Color.Green;
            (@object as Control).Text = message;
        }

        private void UpdateSelectedObjectID(string objectID)
        {
            SelectedObjectID = objectID;
        }

        /*private void UpdateInsights(FacebookPage page)
        {
            Like_Count.Text = page.fan_count ?? "0";

            if (EZ_Social_Media_Manager_pre_alpha.Properties.Settings.Default.FacebookLikesCount != Convert.ToInt32(Like_Count.Text))
            {
                int newLikeCount = Convert.ToInt32(Like_Count.Text);

                Like_Count.Text += " (";
                if (EZ_Social_Media_Manager_pre_alpha.Properties.Settings.Default.FacebookLikesCount < newLikeCount)
                {
                    Like_Count.Text += "+";
                }

                Like_Count.Text += ($"{newLikeCount - EZ_Social_Media_Manager_pre_alpha.Properties.Settings.Default.FacebookLikesCount})");

                EZ_Social_Media_Manager_pre_alpha.Properties.Settings.Default.FacebookLikesCount = newLikeCount;
                EZ_Social_Media_Manager_pre_alpha.Properties.Settings.Default.Save();
            }
        } */

        #region ** Context Menu Event Handlers **
        private void LikeContext_Click(object sender, EventArgs e)
        {

        }
        private void EditContext_Click(object sender, EventArgs e)
        {
            //if ()
            // you can only edit if it is your post

            if (FBPostsList.SelectedIndex != NULL_INDEX)
            {
                PostingEdge = "";
                UpdateText(ref Sent_Notafication_Label, $"Editing post ID {SelectedObjectID}", Color.GreenYellow);
            }
            else
            {
                UpdateText(ref Sent_Notafication_Label, $"Please select a post to edit!", Color.Red);
            }
        }
        private void DeleteContext_Click(object sender, EventArgs e)
        {
            if (SelectedObjectID != null)
            {
                FacebookPage page = FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex];
                FacebookAPIInfo.ReinitializeFacebookService();
                MessagePopup deleteDialog = new MessagePopup("Are you sure you want to delete this?");

                if (deleteDialog.ShowDialog() == DialogResult.OK)
                {
                    var deleteTask = FacebookAPIInfo.facebookService.DeleteFacebookObject(FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].access_token, SelectedObjectID);
                }
            }
            else
            {
                UpdateText(ref Sent_Notafication_Label, $"Please select a post to delete!", Color.Red);
            }
        }
        #endregion
   
        private void DisableAllPanels()
        {
            FBMainPanel.Visible = false;
            Instagram_Panel.Visible = false;
        }

        private void Facebook_Button_Click(object sender, EventArgs e)
        {
            DisableAllPanels();
            FBMainPanel.Visible = true;
        }
    }
}
