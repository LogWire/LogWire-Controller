﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogWire.Controller.Data.Repository;

namespace LogWire.Controller.Utils
{
    public class ApiToken
    {

        private static ApiToken _instance;
        private string _token;

        public static ApiToken Instance =>_instance ??= new ApiToken();

        public string Token => _token;

        public void Init(string token)
        {
            _token = token;
        }

    }
}