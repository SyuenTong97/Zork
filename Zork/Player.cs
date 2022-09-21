namespace Zork
{
    internal class Player
    {
        public Room CurrentRoom
        {
            get
            {
                return _world.Rooms[_location.Vertical, _location.Horizontal];
            }
        }

        public int Score { get; }

        public int Moves { get; }

        public Player(World world)
        {
            _world = world;
        }
        public bool Move(Commands command)
        {
            bool didMove = false;
            switch (command)
            {
                case Commands.North when _location.Vertical < _world.Rooms.GetLength(1) - 1:
                    _location.Vertical++;
                    didMove = true;
                    break;
                case Commands.South when _location.Vertical > 0:
                    _location.Vertical--;
                    didMove = true;
                    break;

                case Commands.East when _location.Horizontal < _world.Rooms.GetLength(0) - 1:
                    _location.Horizontal++;
                    didMove = true;
                    break;
                case Commands.West when _location.Horizontal > 0:
                    _location.Horizontal--;
                    didMove = true;
                    break;
                default:
                    break;
            }
            return didMove;
        }


        private World _world;
        private static (int Vertical, int Horizontal) _location = (1, 1);
    }
}
