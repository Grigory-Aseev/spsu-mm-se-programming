﻿using GameTable.BetsType;
using GameTable.SectorTypeEnum;
using GameTable.TitleEnum;


namespace GameTable.BetCreator
{
    public class EqualChancesCreator : IBetCreator
    {
        public int Cash { get; set; }
        static Random rnd = new Random();

        public Bet FormBet ()
        {
            var value = rnd.RandomEnumVal<EqualChancesEnum>();
            if(value == EqualChancesEnum.Colour)
            {
                var colour = rnd.RandomEnumVal<ColourEnum>();
                return new ColourBet(Cash, colour);
            }
            else
            {
                var parity = rnd.RandomEnumVal<ParityEnum>();
                return new ParityBet(Cash, parity);
            }

            
        }
    }
}
