﻿using CardStorageService.Data;

namespace CardStorageService.Models
{
    public class SessionInfo
    {
        public int SessionId { get; set; }
        public string SessionToken { get; set; }
        public AccountDto AccountDto { get; set; }


    }
}
