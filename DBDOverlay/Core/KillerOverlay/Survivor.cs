﻿namespace DBDOverlay.Core.KillerOverlay
{
    public class Survivor
    {
        public SurvivorState State { get; set; }
        public int Hooks { get; set; }

        public Survivor() 
        {
            State = SurvivorState.Fresh;
            Hooks = 0;
        }
    }
}
