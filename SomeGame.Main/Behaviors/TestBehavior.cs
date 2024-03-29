﻿using Microsoft.Xna.Framework;
using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    class TestBehavior : Behavior
    {
        private int _counter = 0;

        protected override void DoUpdate()
        {
            if(_counter==0)
                Actor.MotionVector.Set(new PixelPoint(2, 1));
            if (_counter == 50)
                Actor.MotionVector.Set(new PixelPoint(-2, -1));
          
            _counter++;
            if (_counter == 100)
                _counter = 0;
        }
    }
}
