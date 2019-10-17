using System;
using System.Collections;
using System.Collections.Generic;

namespace SSDesign.MediaAPI
{
    public class SSDContainer<T> : IDisposable, IEnumerable<T>
    {
        public List<T> items { get; set; }

        public SSDContainer()
        {
            items = new List<T>();
        }

        public void Add()
        {

        }

        #region ** Implementation of IEnumerable **
        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region ** Implementation of IDisposable **
        void IDisposable.Dispose()
        {
            items.Clear();
        }
        #endregion
    }

    public class FacebookResponse<T>
    {
        public FacebookResponse()
        {
            data = new List<T>();
            paging = new FacebookPaging();
        }

        public List<T> data;
        public FacebookPaging paging { get; set; }
    }

    public class FacebookPaging
    {
        public FacebookPaging()
        {
            cursors = new FacebookPagingCursor();

            previous = "";
            next = "";
        }

        public FacebookPagingCursor cursors { get; set; }

        public string previous { get; set; }
        public string next { get; set; }
    }
    public class FacebookPagingCursor
    {
        public FacebookPagingCursor()
        {
            before = "";
            after = "";
        }

        public string before { get; set; }
        public string after { get; set; }
    }

    public class FacebookObjectLikes
    {
        public FacebookObjectLikes()
        {
            summary = new FacebookObjectLikesSummary();
        }
        
        //public data
        public FacebookObjectLikesSummary summary { get; set; }
    }
    public class FacebookObjectLikesSummary
    {
        public FacebookObjectLikesSummary()
        {
            total_count = 0;
            can_like = false;
            has_liked = false;
        }

        public int total_count;
        public bool can_like;
        public bool has_liked;
    }

    public interface IHaveMessage
    {
        string message { get; set; }
        string created_time { get; set; }
    }
    public interface IHaveAName
    {
        string name { get; set; }
    }
    public interface IHaveAccessToken
    {
        string access_token { get; set; }
    }
    public interface IWasCreatedAtCertainTime
    {
        string created_time { get; set; }
    }

    public abstract class APIObject
    {
        public string id { get; set; }
    }

    public class FacebookAccount : APIObject, IHaveAName
    {
        public FacebookAccount()
        {
            id = "-1";
            name = "not logged in";
        }

        public string name { get; set; }
    }
    public class FacebookPage : APIObject, IHaveAName, IHaveAccessToken
    {
        public FacebookPage()
        {
            access_token = "-1";
            name = "NONE";
            fan_count = "-1";

            instagram_business_account = new InstagramAccount();
            posts = new FacebookResponse<FacebookMessageObject>();
            conversations = new FacebookResponse<FacebookConversation>();
        }

        public string access_token { get; set; }
        public string name { get; set; }
        public string fan_count { get; set; }

        public InstagramAccount instagram_business_account { get; set; }
        public FacebookResponse<FacebookMessageObject> posts { get; set; }
        public FacebookResponse<FacebookConversation> conversations { get; set; }
    }
    /// <summary>
    /// Good for posts, comments, and messages
    /// </summary>
    public class FacebookMessageObject : APIObject, IHaveMessage, IWasCreatedAtCertainTime
    {
        public FacebookMessageObject()
        {
            from = new FacebookAccount();
            created_time = "-1";
            message = "null";
            likes = new FacebookObjectLikes();
        }
        public FacebookMessageObject(ref FacebookMessageObject copy)
        {
            from            = copy.from             ?? new FacebookAccount();
            created_time    = copy.created_time     ?? "-1";
            message         = copy.message          ?? "null";
            likes           = copy.likes            ?? new FacebookObjectLikes();
        }

        public FacebookAccount from { get; set; }

        public string created_time { get; set; }
        public string message { get; set; }
        public FacebookObjectLikes likes { get; set; }
        public FacebookResponse<FacebookMessageObject> comments { get; set; }

        public bool Equals(ref FacebookMessageObject other)
        {
            return (id == other.id);
        }
    }
    public class FacebookConversation : APIObject
    {
        public FacebookConversation()
        {
            updated_time = "-1";
            participants = new FacebookResponse<FacebookAccount>();
        }

        public string updated_time { get; set; }
        public FacebookResponse<FacebookAccount> participants { get; set; }
    }

    public class InstagramAccount : APIObject
    {
        public InstagramAccount()
        {
            id = "-1";
            name = "(No IG)";
            media_count = "0";
            followers_count = "0";
        }

        public string name { get; set; }
        public string media_count { get; set; }
        public string followers_count { get; set; }

        public List<InstagramPost> posts { get; set; }
    }
    public class InstagramPost : APIObject
    {
        public InstagramPost()
        {
            caption = "(No Caption)";
            like_count = "0";
        }

        public string caption { get; set; }
        public string like_count { get; set; }
    }
    public class InstagramComment : APIObject
    {
        public InstagramComment()
        {
            text = "NULL";
            like_count = "0";
        }

        public string text { get; set; }
        public string like_count { get; set; }
    }
}
