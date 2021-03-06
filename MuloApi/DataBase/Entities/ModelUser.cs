﻿using System.ComponentModel.DataAnnotations;

namespace MuloApi.DataBase.Entities
{
    public class ModelUser
    {
        public int Id { get; set; }

        [Required] public string Login { get; set; }

        [Required] public string Password { get; set; }
    }
}