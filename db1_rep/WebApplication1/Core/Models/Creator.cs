﻿using Core.Momento;

namespace Core.Models
{
    public class Creator : IOriginal
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public void SetMomento(Momento.Momento momento)
        {
            Id = momento.Id;
            Name = momento.Name;
        }

        public Momento.Momento CreateMomento()
        {
            return new Momento.Momento(Id, Name);
        }
    }
}
