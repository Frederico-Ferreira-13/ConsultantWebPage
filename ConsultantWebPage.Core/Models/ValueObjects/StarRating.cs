using System;

namespace ConsultantWebPage.Core.Models.ValueObjects
{
    public class StarRating
    {
        public int Value { get; }
        private const int MinRating = 1;
        private const int MaxRating = 5;

        private StarRating(int value)
        {
            if (value < MinRating || value > MaxRating)
            {
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"A avaliação deve estar {MinRating} e {MaxRating} estrelas.");
            }
            Value = value;
        }

        public static StarRating Create(int value) => new StarRating(value);

        //Permite usar o StarRating em cálculos como se fosse um int
        public static implicit operator int(StarRating rating) => rating.Value;
    }
}


