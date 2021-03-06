﻿using System;
using Bit.Core.Models.Table;
using Newtonsoft.Json;
using Core.Models.Data;

namespace Bit.Core.Models.Api
{
    public class LoginDataModel
    {
        public LoginDataModel() { }

        public LoginDataModel(LoginRequestModel login)
        {
            Name = login.Name;
            Uri = login.Uri;
            Username = login.Username;
            Password = login.Password;
            Notes = login.Notes;
        }

        public LoginDataModel(CipherRequestModel cipher)
        {
            Name = cipher.Name;
            Uri = cipher.Uri;
            Username = cipher.Username;
            Password = cipher.Password;
            Notes = cipher.Notes;
        }

        public LoginDataModel(Cipher cipher)
        {
            if(cipher.Type != Enums.CipherType.Login)
            {
                throw new ArgumentException("Cipher is not correct type.");
            }

            var data = JsonConvert.DeserializeObject<LoginDataModel>(cipher.Data);

            Name = data.Name;
            Uri = data.Uri;
            Username = data.Username;
            Password = data.Password;
            Notes = data.Notes;
        }

        public LoginDataModel(CipherDetails cipher)
        {
            if(cipher.Type != Enums.CipherType.Login)
            {
                throw new ArgumentException("Cipher is not correct type.");
            }

            var data = JsonConvert.DeserializeObject<LoginDataModel>(cipher.Data);

            Name = data.Name;
            Uri = data.Uri;
            Username = data.Username;
            Password = data.Password;
            Notes = data.Notes;
        }

        public string Name { get; set; }
        public string Uri { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Notes { get; set; }
    }
}
