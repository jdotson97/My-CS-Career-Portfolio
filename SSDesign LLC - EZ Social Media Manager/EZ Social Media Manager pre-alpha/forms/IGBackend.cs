using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SSDesign.MediaAPI;

/// <summary>
/// READ ME NOTES
/// 
/// The exception bug where it says value cannot be null, was from it finding the posts about doing a new cover photo, but there
/// were no words invloved in the post. In the future, I need to check if ANYTHING is in the post, not just a message
/// 
/// there is a for loop in refresh messages. It is in reverse order. This might mess up the selection of messages, though i cant say 
/// it really matters that a message is selected since you only reply to the conversation
/// </summary>

namespace EZ_Social_Media_Manager_pre_alpha
{
    public partial class ManageMedia : Form
    {
        string selectedIGObjectID { get; set; }
        string IG_PostingEdge { get; set; }

        private void IG_Account_DropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            Page_Selector.SelectedIndex = IG_Account_DropDown.SelectedIndex;
        }
        private void IG_Posts_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshIGComments();
            IG_Comment_Replies.Visible = false;

            if (IG_Posts.SelectedIndex != -1)
            {
                selectedIGObjectID = FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].instagram_business_account.posts[IG_Posts.SelectedIndex].id;
                IG_Post_Entry.Text = FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex].instagram_business_account.posts[IG_Posts.SelectedIndex].caption;
                IG_Post_Entry.ForeColor = Color.Gray;
                IG_PostingEdge = "comments";
            }
        }
        private void IG_Comments_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if a comment is selected, update the textbox with its text
            if (IG_Comments.SelectedIndex != -1)
            {
                selectedIGObjectID = FacebookAPIInfo.IGCommentCache.data[IG_Comments.SelectedIndex].id;
                IG_Post_Entry.Text = FacebookAPIInfo.IGCommentCache.data[IG_Comments.SelectedIndex].text;
                IG_PostingEdge = "replies";
            }
        }
        private void IG_Comment_Replies_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IG_Comment_Replies.SelectedIndex == 0)
            {
                IG_Comment_Replies.Visible = false;
            }
        }
        private void IG_Post_Entry_Clicked(object sender, EventArgs e)
        {
            IG_Post_Entry.Text = "";
            IG_Post_Entry.ForeColor = Color.Black;
        }
        private void IG_Comments_DoubleClicked(object sender, EventArgs e)
        {
            RefreshIGCommentReplies();
        }        

        private void IG_SendReply(object sender, EventArgs e)
        {
            if (selectedIGObjectID == null)
            {
                IG_Notafier.Text = "Please select a post to comment on!";
                return;
            }

            IG_Notafier.Text = "Sending reply...";
            FacebookAPIInfo.CreateIGComment(selectedIGObjectID, IG_Post_Entry.Text, IG_PostingEdge);
            RefreshIGComments();

            IG_Notafier.Text = "Your reply has been sent!";
            IG_Post_Entry.Text = "";
        }

        private void RefreshIGPosts()
        {
            IG_Posts.Items.Clear();

            if (Page_Selector.SelectedIndex != -1)
            {
                FacebookPage page = FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex];
                page.instagram_business_account.posts = Task.Run(() => FacebookAPIInfo.facebookService.GetFacebookObject<InstagramPost>(FacebookAPIInfo.userAccessToken, page.instagram_business_account.id, "media", "fields=caption,like_count")).Result.data;

                var posts = page.instagram_business_account.posts;
                for (int i = 0; i < posts.Count; ++i)
                {
                    if (posts[i] != null)
                    {
                        IG_Posts.Items.Add($"[{posts[i].like_count} likes] > \"{posts[i].caption}\"");
                    }
                }
            }
        }
        private void RefreshIGComments()
        {
            IG_Comments.Items.Clear();

            if (IG_Posts.SelectedIndex != -1)
            {
                FacebookPage page = FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex];
                FacebookAPIInfo.IGCommentCache = Task.Run(() => FacebookAPIInfo.facebookService.GetFacebookObject<InstagramComment>(FacebookAPIInfo.userAccessToken, page.instagram_business_account.posts[IG_Posts.SelectedIndex].id, "comments", "fields=like_count,id,text")).Result;

                var comments = FacebookAPIInfo.IGCommentCache.data;
                for (int i = 0; i < comments.Count; ++i)
                {
                    if (comments[i] != null)
                    {
                        Comment_Replies.Visible = false;
                        IG_Comments.Items.Add($"[{comments[i].like_count} likes] >  \"{comments[i].text}\"");
                    }
                }
            }
        }
        private void RefreshIGCommentReplies()
        {
            IG_Comment_Replies.Items.Clear();

            if (IG_Comments.SelectedIndex != -1)
            {
                List<InstagramComment> replies = Task.Run(() => FacebookAPIInfo.facebookService.GetFacebookObject<InstagramComment>(FacebookAPIInfo.userAccessToken, FacebookAPIInfo.IGCommentCache.data[IG_Comments.SelectedIndex].id, "replies", "fields=like_count,id,text")).Result.data;

                IG_Comment_Replies.Visible = true;

                IG_Comment_Replies.Items.Add("(Back)");
                for (int i = 0; i < replies.Count; ++i)
                {
                    IG_Comment_Replies.Items.Add($"[{replies[i].like_count} likes] >  \"{replies[i].text}\"");
                }
            }
        }

        private void Instagram_Button_Click(object sender, EventArgs e)
        {
            DisableAllPanels();
            Instagram_Panel.Visible = true;
        }

        private void IG_Add_Picture_Message_Button_Click(object sender, EventArgs e)
        {

        }

        private void Follower_Summary_Button_Click(object sender, EventArgs e)
        {
            IGFollowerSummary summary = new IGFollowerSummary(FacebookAPIInfo.FBPages[Page_Selector.SelectedIndex]);

            summary.Show();
        }
    }
}