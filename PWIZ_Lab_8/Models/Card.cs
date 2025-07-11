﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace PWIZ_Lab_8.Models
{
    public enum Suit {Hearts, Diamonds, Clubs, Spades }
    public class Card
    {
        public Suit Suit { get; }
        public int Value { get; }

        public Card(Suit suit, int value) 
        {
            this.Suit = suit;
            this.Value = value;
        }

        public string ImagePath => $"/Assets/Cards/{Suit} {Value}.png";

        

    }
}
