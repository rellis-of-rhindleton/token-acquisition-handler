using System;
using System.Text.Json.Serialization;

namespace WebApi.Core.MicrosoftGraph
{
    public class User
    {
        public const string FieldNames =
            "displayName,givenName,surname,userPrincipalName,jobTitle,mail,officeLocation,preferredLanguage,employeeId,department,id";
        
        [JsonPropertyName("@odata.context")]
        public string ODataContext { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("givenName")]
        public string GivenName { get; set; }

        [JsonPropertyName("surname")]
        public string Surname { get; set; }

        [JsonPropertyName("userPrincipalName")]
        public string UserPrincipalName { get; set; }

        [JsonPropertyName("jobTitle")]
        public string JobTitle { get; set; }

        [JsonPropertyName("mail")]
        public string Mail { get; set; }

        [JsonPropertyName("officeLocation")]
        public string OfficeLocation { get; set; }

        [JsonPropertyName("preferredLanguage")]
        public string PreferredLanguge { get; set; }

        [JsonPropertyName("employeeId")]
        public string EmployeeId { get; set; }

        [JsonPropertyName("department")]
        public string Department { get; set; }

        [JsonPropertyName("id")]
        public string ObjectId { get; set; }


        public DateTime QueryDateTimeUtc { get; set; }
    }
}
