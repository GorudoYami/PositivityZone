using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using PositivityZone.API.Models;
using System.Threading.Tasks;
using Plugin.Connectivity;
using System.Security.Cryptography;
using RestSharp.Extensions;
using System.Threading;

namespace PositivityZone.API {
    public enum Status {
        Ok,
        Maintenance,
        Development,
        NoInternet,
        ConnectionError
    }

    public class WebAPI {
        public string UID { get; set; }
        private RestClient Client { get; set; }
        private const string ApiKey = "a75a7044beb042103b734791ee8d4b5b58a66198d30e3e89ae87f914830b56639325831237e64b796013fd67294d66e7b8e320740681a3566b0b4bec99ea95c3";
        public WebAPI() {
            Client = new RestClient("https://10.0.1.5:5001/api");
            // Remove at production
            Client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
        }

        public void PostUID() {
            RestRequest request = new RestRequest("user", Method.POST);
            request.AddHeader("X-Api-Key", ApiKey);
            request.AddJsonBody(UID);
            Client.Execute(request);
        }

        public bool HasPass() {
            RestRequest request = new RestRequest("user/haspass", Method.GET);
            request.AddHeader("X-Api-Key", ApiKey);
            request.AddParameter("uid", UID);
            var response = Client.Execute<bool>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return response.Data;
            else
                return false;
        }

        public bool Recover(string _UID, string password) {
            RestRequest request = new RestRequest("user/recover", Method.GET);
            request.AddHeader("X-Api-Key", ApiKey);
            request.AddParameter("uid", _UID);
            string hashStr = "";
            using (SHA256 sha = SHA256.Create()) {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                foreach (byte b in hash) {
                    hashStr += b.ToString("x2");
                }
            }
            request.AddParameter("hash", hashStr);
            var response = Client.Execute<bool>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                if (response.Data) {
                    UID = _UID;
                    return true;
                }
            }
            return false;
        }

        public bool ChangePass(string oldPass, string newPass) {
            RestRequest request = new RestRequest("user/changepass", Method.POST);
            request.AddHeader("X-Api-Key", ApiKey);
            string oldHashStr = "";
            string newHashStr = "";
            using (SHA256 sha = SHA256.Create()) {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(oldPass));
                foreach (byte b in hash) {
                    oldHashStr += b.ToString("x2");
                }
            }

            using (SHA256 sha = SHA256.Create()) {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(newPass));
                foreach (byte b in hash) {
                    newHashStr += b.ToString("x2");
                }
            }
            
            request.AddParameter("oldHash", oldHashStr);
            request.AddParameter("newHash", newHashStr);
            request.AddParameter("uid", UID);
            var response = Client.Execute<bool>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return response.Data;
            else
                return false;
        }

        public bool SetPass(string newPass) {
            return true;
        }

        public async Task<List<Entry>> GetEntries(bool? answered, bool? approved, bool? disapproved, string lang) {
            RestRequest request = new RestRequest("entries", Method.GET);
            request.AddHeader("X-Api-Key", ApiKey);
            if (answered != null)
                request.AddParameter("answered", answered.Value);
            if (approved != null)
                request.AddParameter("approved", approved.Value);
            if (disapproved != null)
                request.AddParameter("disapproved", disapproved.Value);
            if (!string.IsNullOrEmpty(lang))
                request.AddParameter("lang", lang);

            var response = await Client.ExecuteAsync<List<Entry>>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                return null;
            else
                return response.Data;
        }

        public async Task<List<Answer>> GetAnswers(bool? approved, bool? disapproved, string lang) {
            RestRequest request = new RestRequest("answers", Method.GET);
            request.AddHeader("X-Api-Key", ApiKey);
            if (approved != null)
                request.AddParameter("approved", approved.Value);
            if (disapproved != null)
                request.AddParameter("disapproved", disapproved.Value);
            if (!string.IsNullOrEmpty(lang))
                request.AddParameter("lang", lang);

            var response = await Client.ExecuteAsync<List<Answer>>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return null;
            else
                return response.Data;
        }

        public bool PostEntry(Entry entry) {
            RestRequest request = new RestRequest("entry", Method.POST);
            request.AddHeader("X-Api-Key", ApiKey);
            request.AddJsonBody(entry);
            var response = Client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return true;
            else
                return false;
        }

        public async Task<bool> PostAnswer(Answer answer) {
            answer.UID = UID;
            RestRequest request = new RestRequest("answer", Method.POST);
            request.AddHeader("X-Api-Key", ApiKey);
            request.AddJsonBody(answer);
            var response = await Client.ExecuteAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return true;
            else
                return false;
        }

        public async Task<Status> GetStatus() {
            // Check if device is connected to internet (wifi or cell data)
            if (!CrossConnectivity.Current.IsConnected)
                return Status.NoInternet;

            // Get status from API
            RestRequest request = new RestRequest("status", Method.GET) {
                Timeout = 5000,
                ReadWriteTimeout = 5000
            };
            request.AddHeader("X-Api-Key", ApiKey);

            var response = await Client.ExecuteAsync<Status>(request);
            if (!response.IsSuccessful)
                return Status.ConnectionError;
            else
                return response.Data;
        }

        public async Task<bool> Approve(Entry entry) {
            RestRequest request = new RestRequest("entry/approve", Method.GET);
            request.AddHeader("X-Api-Key", ApiKey);
            request.AddParameter("id", entry.ID);
            var response = await Client.ExecuteAsync(request);
            if (response.IsSuccessful)
                return true;
            else 
                return false;
        }

        public async Task<bool> Approve(Answer answer) {
            RestRequest request = new RestRequest("answer/approve", Method.GET);
            request.AddHeader("X-Api-Key", ApiKey);
            request.AddParameter("id", answer.ID);
            var response = await Client.ExecuteAsync(request);
            if (response.IsSuccessful)
                return true;
            else
                return false;
        }

        public async Task<bool> Disapprove(Entry entry) {
            RestRequest request = new RestRequest("entry/disapprove", Method.GET);
            request.AddHeader("X-Api-Key", ApiKey);
            request.AddParameter("id", entry.ID);
            var response = await Client.ExecuteAsync(request);
            if (response.IsSuccessful)
                return true;
            else
                return false;
        }

        public async Task<bool> Disapprove(Answer answer) {
            RestRequest request = new RestRequest("answer/disapprove", Method.GET);
            request.AddHeader("X-Api-Key", ApiKey);
            request.AddParameter("id", answer.ID);
            var response = await Client.ExecuteAsync(request);
            if (response.IsSuccessful)
                return true;
            else
                return false;
        }

        public async Task<List<Entry>> GetUserEntries() {
            RestRequest request = new RestRequest("entries", Method.GET);
            request.AddHeader("X-Api-Key", ApiKey);
            request.AddParameter("uid", UID);

            var response = await Client.ExecuteAsync<List<Entry>>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                return null;
            else
                return response.Data;
        }

        public async Task<Answer> GetAnswer(int EntryID) {
            RestRequest request = new RestRequest("answers", Method.GET);
            request.AddHeader("X-Api-Key", ApiKey);
            request.AddParameter("entryId", EntryID);
            request.AddParameter("approved", true);
            request.AddParameter("disapproved", false);

            var response = await Client.ExecuteAsync<List<Answer>>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                return null;
            else
                return response.Data[0];
        }
    }
}