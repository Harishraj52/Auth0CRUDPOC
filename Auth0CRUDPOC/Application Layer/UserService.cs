using Auth0CRUDPOC.Core_Layer.Models;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using static System.Net.WebRequestMethods;

namespace Auth0CRUDPOC.Application_Layer
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private readonly Auth0Settings _auth0Settings;
        private readonly UpdateUserResponse _updateUserResponse;

        public UserService(HttpClient httpClient, Auth0Settings auth0Settings, UpdateUserResponse updateUserResponse)
        {
            _httpClient = httpClient;
            _auth0Settings = auth0Settings;
            _updateUserResponse = updateUserResponse;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _auth0Settings.Token);
        }

        public async Task<string> GetUser(string email)
        {
            var url = $"https://dev-a4mofoxoe3g6y4gq.us.auth0.com/api/v2/users-by-email?email=biruduraju@yahoo.com";
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
        public async Task<CreateUserResponse> CreateUser(UserModel usermodel)
        {
            var url =_auth0Settings.Uri;
            var createUserRequest = new
            {
                connection = "Username-Password-Authentication",
                email = usermodel.Email,
                password = usermodel.Password,
                email_verified = usermodel.EmailVerified,
                app_metadata = new { roles = new int[] { 1, 2, 3 } }
            };
            var jsonRequest = JsonConvert.SerializeObject(createUserRequest);
            var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(url, httpContent);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var createUserResponse = JsonConvert.DeserializeObject<CreateUserResponse>(jsonResponse);
                Console.WriteLine("User created successfully!");
                return createUserResponse;
            }
            throw new Exception($"Failed to create user. StatusCode: {response.StatusCode}, Reason:{response.ReasonPhrase}");
        }

        public async Task<UpdateUserResponse> UpdateUser(string userId, UserModel usermodel)
        {
            var url = $"{_auth0Settings.Uri}/{userId}";

            await UpdateUserEmail(url, usermodel);
            await UpdateUserPassword(url, usermodel);
            await UpdateUserEmailVerified(url, usermodel);
            await BlockOrUnblockUser(url, usermodel);

            return _updateUserResponse;
        }

            public async Task<HttpResponseMessage> DeleteUser(string userId)
        {
            var url = $"{_auth0Settings.Uri}/{userId}";
            var response = await _httpClient.DeleteAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to delete user. StatusCode: {response.StatusCode}, Reason:{response.ReasonPhrase}");
            }
            return response;
        }

        public async Task<BlockUserResponse> BlockUser(string userId, UserModel usermodel)
        {
            var url = $"{_auth0Settings.Uri}/{userId}";

            var blockUserRequest = new
            {
                blocked = usermodel.Blocked
            };
            var jsonRequest = JsonConvert.SerializeObject(blockUserRequest);
            var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var response = await _httpClient.PatchAsync(url, httpContent);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var blockUserResponse = JsonConvert.DeserializeObject<BlockUserResponse>(jsonResponse);
                Console.WriteLine("User blocked successfully!");
                return blockUserResponse;
            }
            else
            {
                throw new Exception($"Failed to update user. StatusCode: {response.StatusCode}, Reason:{response.ReasonPhrase}");
            }

        }

        public async Task<BlockUserResponse> BlockUser(string userId)
        {
            var url = $"{_auth0Settings.Uri}/user-blocks";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _auth0Settings.Token);
            request.Content = new StringContent($"{{ \"identifier\": \"{userId}\" }}", Encoding.UTF8, "application/json");


            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to block user. StatusCode: {response.StatusCode}, Reason:{response.ReasonPhrase} && {response.Content.ReadAsStringAsync()}");
            }
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var blockUserResponse = JsonConvert.DeserializeObject<BlockUserResponse>(jsonResponse);
            Console.WriteLine("User created successfully!");
            return blockUserResponse;
        }

        private async Task<HttpResponseMessage> UpdateUserFields(string url, object updateUserRequest)
        {
            var jsonRequest = JsonConvert.SerializeObject(updateUserRequest);
            var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var response = await _httpClient.PatchAsync(url, httpContent);
            return response;
        }

        private async Task UpdateUserEmail(string url, UserModel usermodel)
        {
            if (string.IsNullOrWhiteSpace(usermodel.Email))
            {
                return;
            }
                var updateUserRequest = new
                {
                    email = usermodel.Email
                };
                var response = await UpdateUserFields(url, updateUserRequest);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var updateUserEmailResponse = JsonConvert.DeserializeObject<UpdateUserResponse>(jsonResponse);
                    Console.WriteLine("User updated successfully!");
                    _updateUserResponse.email = updateUserEmailResponse.email;
                }
                else
                {
                    throw new Exception($"Failed to update user. StatusCode: {response.StatusCode}, Reason:{response.ReasonPhrase}");
                }
        }

        private async Task UpdateUserPassword(string url, UserModel usermodel)
        {
            if (string.IsNullOrWhiteSpace(usermodel.Password))
            {
                return;
            }
            var updateUserRequest = new
            {
                password = usermodel.Password
            };
            var response = await UpdateUserFields(url, updateUserRequest);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var updateUserPasswordResponse = JsonConvert.DeserializeObject<UpdateUserResponse>(jsonResponse);
                Console.WriteLine("User updated successfully!");
                _updateUserResponse.password = updateUserPasswordResponse.password;
            }
            else
            {
                throw new Exception($"Failed to update user. StatusCode: {response.StatusCode}, Reason:{response.ReasonPhrase}");
            }
        }

        private async Task UpdateUserEmailVerified(string url, UserModel usermodel)
        {
            if (!usermodel.EmailVerified)
            {
                return;
            }
            var updateUserRequest = new
            {
                email_verified = usermodel.EmailVerified
            };
            var response = await UpdateUserFields(url, updateUserRequest);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var updateUserEmailVerifiedResponse = JsonConvert.DeserializeObject<UpdateUserResponse>(jsonResponse);
                Console.WriteLine("User updated successfully!");
                _updateUserResponse.email_verified = updateUserEmailVerifiedResponse.email_verified;
            }
            else
            {
                throw new Exception($"Failed to update user. StatusCode: {response.StatusCode}, Reason:{response.ReasonPhrase}");
            }
        }

        // Since There is not an explicit call for doing this on the management API, you simply patch the user with { blocked: true } or { blocked: false }
        private async Task BlockOrUnblockUser(string url, UserModel usermodel)
        {
            var updateUserRequest = new
            {
                blocked = usermodel.Blocked
            };
            var response = await UpdateUserFields(url, updateUserRequest);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var updateUserBlockedResponse = JsonConvert.DeserializeObject<UpdateUserResponse>(jsonResponse);
                Console.WriteLine("User updated successfully!");
                _updateUserResponse.blocked = updateUserBlockedResponse.blocked;
            }
            else
            {
                throw new Exception($"Failed to update user. StatusCode: {response.StatusCode}, Reason:{response.ReasonPhrase}");
            }
        }
    }
}
