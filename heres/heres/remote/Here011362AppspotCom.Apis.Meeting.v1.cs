// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on
// an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by google-apis-code-generator 1.4.2
//     C# generater version: 
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Here011362AppspotCom.Apis.Meeting.v1.Data
{    

    public class MeetingsApiMeetingMsg : Google.Apis.Requests.IDirectResponseSchema
    {
        [Newtonsoft.Json.JsonPropertyAttribute("id")]
        public virtual string Id { get; set; } 

        [Newtonsoft.Json.JsonPropertyAttribute("startTime")]
        public virtual string StartTime { get; set; } 

        [Newtonsoft.Json.JsonPropertyAttribute("title")]
        public virtual string Title { get; set; } 

        /// <summary>The ETag of the item.</summary>
        public virtual string ETag { get; set; }
    }    

    public class MeetingsApiMeetingsCollection : Google.Apis.Requests.IDirectResponseSchema
    {
        [Newtonsoft.Json.JsonPropertyAttribute("items")]
        public virtual System.Collections.Generic.IList<MeetingsApiMeetingMsg> Items { get; set; } 

        /// <summary>The ETag of the item.</summary>
        public virtual string ETag { get; set; }
    }
}

namespace Here011362AppspotCom.Apis.Meeting.v1
{
    /// <summary>The Meeting Service.</summary>
    public class MeetingService : Google.Apis.Services.BaseClientService
    {
        /// <summary>The API version.</summary>
        public const string Version = "v1";

        /// <summary>The discovery version used to generate this service.</summary>
        public static Google.Apis.Discovery.DiscoveryVersion DiscoveryVersionUsed =
            Google.Apis.Discovery.DiscoveryVersion.Version_1_0;

        /// <summary>Constructs a new service.</summary>
        public MeetingService() :
            this(new Google.Apis.Services.BaseClientService.Initializer()) {}

        /// <summary>Constructs a new service.</summary>
        /// <param name="initializer">The service initializer.</param>
        public MeetingService(Google.Apis.Services.BaseClientService.Initializer initializer)
            : base(initializer)
        {
            meetings = new MeetingsResource(this);
        }

        /// <summary>Gets the service supported features.</summary>
        public override System.Collections.Generic.IList<string> Features
        {
            get { return new string[0]; }
        }

        /// <summary>Gets the service name.</summary>
        public override string Name
        {
            get { return "meeting"; }
        }

        /// <summary>Gets the service base URI.</summary>
        public override string BaseUri
        {
            get { return "https://here01-1362.appspot.com/_ah/api/meeting/v1/"; }
        }

        /// <summary>Gets the service base path.</summary>
        public override string BasePath
        {
            get { return "meeting/v1/"; }
        }

        /// <summary>Available OAuth 2.0 scopes for use with the .</summary>
        public enum Scopes
        {
            /// <summary>View your email address</summary>
            [Google.Apis.Util.StringValueAttribute("https://www.googleapis.com/auth/userinfo.email")]
            UserinfoEmail,
        }



        private readonly MeetingsResource meetings;

        /// <summary>Gets the Meetings resource.</summary>
        public virtual MeetingsResource Meetings
        {
            get { return meetings; }
        }
    }

    ///<summary>A base abstract class for Meeting requests.</summary>
    public abstract class MeetingBaseServiceRequest<TResponse> : Google.Apis.Requests.ClientServiceRequest<TResponse>
    {
        ///<summary>Constructs a new MeetingBaseServiceRequest instance.</summary>
        protected MeetingBaseServiceRequest(Google.Apis.Services.IClientService service)
            : base(service)
        {
        }

        /// <summary>Data format for the response.</summary>
        /// [default: json]
        [Google.Apis.Util.RequestParameterAttribute("alt", Google.Apis.Util.RequestParameterType.Query)]
        public virtual System.Nullable<AltEnum> Alt { get; set; }

        /// <summary>Data format for the response.</summary>
        public enum AltEnum
        {
            /// <summary>Responses with Content-Type of application/json</summary>
            [Google.Apis.Util.StringValueAttribute("json")]
            Json,
        }

        /// <summary>Selector specifying which fields to include in a partial response.</summary>
        [Google.Apis.Util.RequestParameterAttribute("fields", Google.Apis.Util.RequestParameterType.Query)]
        public virtual string Fields { get; set; }

        /// <summary>API key. Your API key identifies your project and provides you with API access, quota, and reports.
        /// Required unless you provide an OAuth 2.0 token.</summary>
        [Google.Apis.Util.RequestParameterAttribute("key", Google.Apis.Util.RequestParameterType.Query)]
        public virtual string Key { get; set; }

        /// <summary>OAuth 2.0 token for the current user.</summary>
        [Google.Apis.Util.RequestParameterAttribute("oauth_token", Google.Apis.Util.RequestParameterType.Query)]
        public virtual string OauthToken { get; set; }

        /// <summary>Returns response with indentations and line breaks.</summary>
        /// [default: true]
        [Google.Apis.Util.RequestParameterAttribute("prettyPrint", Google.Apis.Util.RequestParameterType.Query)]
        public virtual System.Nullable<bool> PrettyPrint { get; set; }

        /// <summary>Available to use for quota purposes for server-side applications. Can be any arbitrary string
        /// assigned to a user, but should not exceed 40 characters. Overrides userIp if both are provided.</summary>
        [Google.Apis.Util.RequestParameterAttribute("quotaUser", Google.Apis.Util.RequestParameterType.Query)]
        public virtual string QuotaUser { get; set; }

        /// <summary>IP address of the site where the request originates. Use this if you want to enforce per-user
        /// limits.</summary>
        [Google.Apis.Util.RequestParameterAttribute("userIp", Google.Apis.Util.RequestParameterType.Query)]
        public virtual string UserIp { get; set; }

        /// <summary>Initializes Meeting parameter list.</summary>
        protected override void InitParameters()
        {
            base.InitParameters();

            RequestParameters.Add(
                "alt", new Google.Apis.Discovery.Parameter
                {
                    Name = "alt",
                    IsRequired = false,
                    ParameterType = "query",
                    DefaultValue = "json",
                    Pattern = null,
                });
            RequestParameters.Add(
                "fields", new Google.Apis.Discovery.Parameter
                {
                    Name = "fields",
                    IsRequired = false,
                    ParameterType = "query",
                    DefaultValue = null,
                    Pattern = null,
                });
            RequestParameters.Add(
                "key", new Google.Apis.Discovery.Parameter
                {
                    Name = "key",
                    IsRequired = false,
                    ParameterType = "query",
                    DefaultValue = null,
                    Pattern = null,
                });
            RequestParameters.Add(
                "oauth_token", new Google.Apis.Discovery.Parameter
                {
                    Name = "oauth_token",
                    IsRequired = false,
                    ParameterType = "query",
                    DefaultValue = null,
                    Pattern = null,
                });
            RequestParameters.Add(
                "prettyPrint", new Google.Apis.Discovery.Parameter
                {
                    Name = "prettyPrint",
                    IsRequired = false,
                    ParameterType = "query",
                    DefaultValue = "true",
                    Pattern = null,
                });
            RequestParameters.Add(
                "quotaUser", new Google.Apis.Discovery.Parameter
                {
                    Name = "quotaUser",
                    IsRequired = false,
                    ParameterType = "query",
                    DefaultValue = null,
                    Pattern = null,
                });
            RequestParameters.Add(
                "userIp", new Google.Apis.Discovery.Parameter
                {
                    Name = "userIp",
                    IsRequired = false,
                    ParameterType = "query",
                    DefaultValue = null,
                    Pattern = null,
                });
        }
    }

    /// <summary>The "meetings" collection of methods.</summary>
    public class MeetingsResource
    {
        private const string Resource = "meetings";

        ///<summary> The service whih this resource is belong to.</summary>
        private readonly Google.Apis.Services.IClientService service;

        /// <summary>Constructs a new resource.</summary>
        public MeetingsResource(Google.Apis.Services.IClientService service)
        {
            this.service = service;

        }



        /// <param name="body">The body of the request.</param>
        public virtual CreateRequest Create(Here011362AppspotCom.Apis.Meeting.v1.Data.MeetingsApiMeetingMsg body)
        {
            return new CreateRequest(service, body);
        }


        public class CreateRequest : MeetingBaseServiceRequest<Here011362AppspotCom.Apis.Meeting.v1.Data.MeetingsApiMeetingMsg>
        {
            /// <summary>Constructs a new Create request.</summary>
            public CreateRequest(Google.Apis.Services.IClientService service, Here011362AppspotCom.Apis.Meeting.v1.Data.MeetingsApiMeetingMsg body)
                : base(service)
            {
                Body = body;
                InitParameters();
            }



            /// <summary>Gets or sets the body of this request.</summary>
            Here011362AppspotCom.Apis.Meeting.v1.Data.MeetingsApiMeetingMsg Body { get; set; }

            ///<summary>Returns the body of the request.</summary>
            protected override object GetBody() { return Body; }

            ///<summary>Gets the method name.</summary>
            public override string MethodName
            {
                get { return "create"; }
            }

            ///<summary>Gets the HTTP method.</summary>
            public override string HttpMethod
            {
                get { return "POST"; }
            }

            ///<summary>Gets the REST path.</summary>
            public override string RestPath
            {
                get { return "meetings"; }
            }

            /// <summary>Initializes Create parameter list.</summary>
            protected override void InitParameters()
            {
                base.InitParameters();

            }

        }


        /// <param name="id"></param>
        public virtual DeleteRequest Delete(int id)
        {
            return new DeleteRequest(service, id);
        }


        public class DeleteRequest : MeetingBaseServiceRequest<string>
        {
            /// <summary>Constructs a new Delete request.</summary>
            public DeleteRequest(Google.Apis.Services.IClientService service, int id)
                : base(service)
            {
                Id = id;
                InitParameters();
            }



            [Google.Apis.Util.RequestParameterAttribute("id", Google.Apis.Util.RequestParameterType.Path)]
            public virtual int Id { get; private set; }


            ///<summary>Gets the method name.</summary>
            public override string MethodName
            {
                get { return "delete"; }
            }

            ///<summary>Gets the HTTP method.</summary>
            public override string HttpMethod
            {
                get { return "DELETE"; }
            }

            ///<summary>Gets the REST path.</summary>
            public override string RestPath
            {
                get { return "meetings/{id}"; }
            }

            /// <summary>Initializes Delete parameter list.</summary>
            protected override void InitParameters()
            {
                base.InitParameters();

                RequestParameters.Add(
                    "id", new Google.Apis.Discovery.Parameter
                    {
                        Name = "id",
                        IsRequired = true,
                        ParameterType = "path",
                        DefaultValue = null,
                        Pattern = null,
                    });
            }

        }


        /// <param name="userId"></param>
        public virtual ListRequest List(string userId)
        {
            return new ListRequest(service, userId);
        }


        public class ListRequest : MeetingBaseServiceRequest<Here011362AppspotCom.Apis.Meeting.v1.Data.MeetingsApiMeetingsCollection>
        {
            /// <summary>Constructs a new List request.</summary>
            public ListRequest(Google.Apis.Services.IClientService service, string userId)
                : base(service)
            {
                UserId = userId;
                InitParameters();
            }



            [Google.Apis.Util.RequestParameterAttribute("userId", Google.Apis.Util.RequestParameterType.Path)]
            public virtual string UserId { get; private set; }


            ///<summary>Gets the method name.</summary>
            public override string MethodName
            {
                get { return "list"; }
            }

            ///<summary>Gets the HTTP method.</summary>
            public override string HttpMethod
            {
                get { return "GET"; }
            }

            ///<summary>Gets the REST path.</summary>
            public override string RestPath
            {
                get { return "meetings/{userId}"; }
            }

            /// <summary>Initializes List parameter list.</summary>
            protected override void InitParameters()
            {
                base.InitParameters();

                RequestParameters.Add(
                    "userId", new Google.Apis.Discovery.Parameter
                    {
                        Name = "userId",
                        IsRequired = true,
                        ParameterType = "path",
                        DefaultValue = null,
                        Pattern = null,
                    });
            }

        }
    }
}
