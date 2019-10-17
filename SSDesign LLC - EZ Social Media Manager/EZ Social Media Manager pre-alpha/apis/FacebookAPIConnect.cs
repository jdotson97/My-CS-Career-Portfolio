using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using SSDesign.MediaAPI;

/// <summary>
/// Brief Facebook Graph API explanation
/// 
/// Requesting facebook info requires an access token
/// Access tokens are hash strings that contain the userID, and all of the permissions that the application is requesting to get from graph.facebook
/// 
/// The base address for these requests is https://graph.facebook.com/v3.1/, that endpoint being the Id of the resource and access token
/// FACEBOOK LOGIN is the feature we are attempting to introduce into our app, as it will allow the user to create, post, and manage pages and accounts
/// from within this application itself
/// 
/// How the program obtains a USER access token, is it will open a web dialog box from within the program, allow the user to login, and then, if approved,
/// the program will be provided a user access token that will allow it to obtain a specified list of information about the user.
/// 
/// It will be provided this by means of sticking it the redirect url, for use to have to parse out of the string.
/// 
/// In here we need a way to poll to check whether or not the access token is expired and a new one needs aquired
/// 
/// UPDATE: due to the recent scandel with cambridge analytica, we are not able to post things directly on someones wall from this application. The only
/// interaction that we may have with someones account is managing there pages which should be fine as they will mostly be business owners.
/// 
/// </summary>

namespace EZ_Social_Media_Manager_pre_alpha
{
    [Serializable]
    public struct FacebookAPIInfo
    {
        public static string userAccessToken = null;
        public static long AccessTokenValidUntil = -1;

        public static readonly string FBApiVersion = "3.1";

        public static bool IsUserLoggedIn = false;

        public static FacebookService facebookService = new FacebookService(new FacebookClient());
        //statically loaded objects
        public static FacebookAccount FBAccount                                   = new FacebookAccount();
        public static List<FacebookPage> FBPages                                  = new List<FacebookPage>();
        //dynamically loaded objects
        public static FacebookResponse<FacebookMessageObject> FBCommentCache    = new FacebookResponse<FacebookMessageObject>();
        public static FacebookResponse<FacebookMessageObject> FBMessageCache    = new FacebookResponse<FacebookMessageObject>();

        public static FacebookResponse<InstagramComment> IGCommentCache         = new FacebookResponse<InstagramComment>();

        public static void      ReinitializeFacebookService()
        {
            facebookService = new FacebookService(new FacebookClient());
        }
        public static void      ParseUserAccessTokenFromURL(ref string URL)
        {
            try
            {
                string TTL = URL.Substring(URL.IndexOf("&expires_in=") + 12);

                if (TTL.IndexOf("&") != -1) //check for extra shit on the end of the url that is there fore no ass reason
                {
                    TTL = TTL.Substring(0, TTL.IndexOf("&"));
                }

                if (TTL != "0")
                {
                    AccessTokenValidUntil = System.Environment.TickCount + Convert.ToInt64(TTL);
                }
                else
                {
                    AccessTokenValidUntil = 0;
                }

                URL = (URL.Substring(URL.IndexOf("access_token=") + 13));
                URL = URL.Substring(0, URL.IndexOf("&"));
            }
            catch
            {
                return;
            }

            userAccessToken = URL;
            IsUserLoggedIn = true;
        }
        public static async Task      ImportFacebookAccountData()
        {
            //all of these things will need to be awaited so they do not clog up the loading and shit
            ReinitializeFacebookService();

            FBAccount =   (await facebookService.GetFacebookAccountAsync(userAccessToken));
            FBPages =     (await facebookService.GetFacebookObject<FacebookPage>(userAccessToken, FBAccount.id, "accounts", "fields=access_token,name,fan_count,instagram_business_account{name,id,media_count,followers_count}")).data; //facebookService.GetUserPagesAsync(userAccessToken););;
            
            for (int i = 0; i < FBPages.Count; ++i)
            {
                FBPages[i].posts = (Task.Run(() => facebookService.GetFacebookObject<FacebookMessageObject>(FBPages[i].access_token, FBPages[i].id, "feed", "fields=id,from,message,created_time,likes.summary(true)")).Result);
                FBPages[i].conversations = (Task.Run(() => facebookService.GetFacebookObject<FacebookConversation> (FBPages[i].access_token, FBPages[i].id, "conversations", "fields=id,updated_time,participants")).Result);
            }
        }

        public static void                          CreateComment(int pageNumber, string objectID, string message, string edge = "comments")
        {
            FacebookPage page = FBPages[pageNumber];

            var getCommentTask = facebookService.CreateFacebookObject(page.access_token, objectID, edge, message);
            Task.WaitAll(getCommentTask);
        }

        public static List<FacebookMessageObject>   GetCommentReplies(int commentIndex)
        {
            //this is makeing the call to get the comments; THIS IS CORRECT DO NOT MESS THIS UP 
            var getRepliesTask = facebookService.GetFacebookObject<FacebookMessageObject>(userAccessToken, FBCommentCache.data[commentIndex].id, "comments");
            Task.WaitAll(getRepliesTask);

            return getRepliesTask.Result.data;
        }
        public static void                          SendMessage(int pageNumber, string objectID, string message)
        {
            FacebookPage page = FBPages[pageNumber];

            var getMessageTask = facebookService.CreateFacebookObject(page.access_token, objectID, "messages", message);
            Task.WaitAll(getMessageTask);
        }

        public static void                          CreateIGComment(string objectID, string message, string edge = "comments")
        {
            var getCommentIGTask = facebookService.CreateFacebookObject(userAccessToken, objectID, edge, message);
            Task.WaitAll(getCommentIGTask);
        }
    }

    static class FacebookInfo
    {
        public const string AppID = "227526774595960";//"2183029721956987";
    }

    public class FacebookClient
    {
        private readonly HttpClient httpClient;
        public FacebookClient()
        {
            httpClient = new HttpClient
            {
                BaseAddress = new Uri(String.Format("https://graph.facebook.com/v{0}/", FacebookAPIInfo.FBApiVersion))
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<T> GetAsync<T>(string access_token, string endpoint, string args = null)
        {
            try
            {
                var response = await httpClient.GetAsync($"{endpoint}?access_token={access_token}&{args}").ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                    return default(T);

                var result = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(result);
            }
            catch (Exception ex) // IN THE FUTURE, MAKE THIS THROW EX UP THE CALLSTACK
            {
                return default(T);
            }
        }
        public async Task PostAsync(string access_token, string endpoint, string data, string attachmentsPath, string arguments = null)
        {
            try
            {
                var payload = new MultipartFormDataContent();
                if (attachmentsPath != null)
                {
                    payload = EncodeOtherContent(attachmentsPath);
                }

                await httpClient.PostAsync($"{endpoint}?access_token={access_token}&message={data}&{arguments}", payload).ConfigureAwait(false);
            }
            catch (Exception ex) // IN THE FUTURE, MAKE THIS THROW EX UP THE CALLSTACK
            {
                return;
            }           
        }
        public async Task DeleteAsync(string access_token, string endpoint)
        {
            try
            {
                await httpClient.DeleteAsync($"{endpoint}?access_token={access_token}").ConfigureAwait(false);
            }
            catch (Exception ex) // IN THE FUTURE, MAKE THIS THROW EX UP THE CALLSTACK
            {
                return;
            }
        }

        private static MultipartFormDataContent EncodeOtherContent(string path)
        {
            byte[] fileBytes = File.ReadAllBytes(path);

            MultipartFormDataContent form = new MultipartFormDataContent();

            var fileName = path.Substring(path.LastIndexOf(@"\"));

            form.Add(new ByteArrayContent(fileBytes, 0, fileBytes.Length), "source", fileName);

            return form;
        }
    }
    public class FacebookService
    {
        private readonly FacebookClient FacebookClient;
        public FacebookService(FacebookClient facebookClient)
        {
            FacebookClient = facebookClient;
        }

        public async Task<FacebookAccount> GetFacebookAccountAsync(string accessToken)
        {
            var result = await FacebookClient.GetAsync<dynamic>(
                accessToken, "me", "fields=id,name").ConfigureAwait(false);

            if (result == null)
            {
                return new FacebookAccount();
            }

            var account = new FacebookAccount
            {
                id = result.id,
                name = result.name,
            };

            return account;
        }
        
        public async Task<FacebookResponse<T>> GetFacebookObject<T>(string access_token, string objectID, string edge, string arguments = null) where T : class, new()
        {
            JObject result = await FacebookClient.GetAsync<dynamic>(access_token, $"{objectID}/{edge}", arguments).ConfigureAwait(false);

            if (result == null)
            {
                return new FacebookResponse<T>();
            }

            FacebookResponse<T> response = new JavaScriptSerializer().Deserialize<FacebookResponse<T>>(result.ToString());

            return response;
        }
        public async Task CreateFacebookObject(string access_token, string objectID, string edge, string message, string attachments = null, string scheduledTime = null, string arguments = null)
        {
            await FacebookClient.PostAsync(

                access_token:    access_token, 
                endpoint:       $"{objectID}{((edge != "") ? $"/{edge}" : "")}", 
                data:           message, 
                attachmentsPath:     attachments, 
                arguments:           ((scheduledTime != null) ? $"scheduled_publish_time={scheduledTime}&published=false" : "") + arguments
                
                ).ConfigureAwait(false);
        }
        public async Task           DeleteFacebookObject(string access_token, string objectID)
        {
            await FacebookClient.DeleteAsync(access_token, objectID).ConfigureAwait(false);
        }
    }
}