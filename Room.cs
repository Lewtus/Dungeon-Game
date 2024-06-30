using System;
using System.Collections.Generic;
using System.Text;

namespace gam
{
    public enum Direction
    {
        North,
        South,
        East,
        West
     }  


    public static class DirectionExtension
    {
  
        public static string DirectionToString(this Direction direction)
        {
            return direction.ToString();
        }

        public static Direction? ToDirection(this string direction)
        {
            if (Enum.TryParse<Direction>(direction, true, out Direction newDirection))
            {
                return newDirection;
            }
            return null;
        }

        public static Direction InvertDirection(this Direction direction)
        {
            return direction switch
            {
                Direction.North => Direction.South,
                Direction.East => Direction.West,
                Direction.South => Direction.North,
                Direction.West => Direction.East,
                _ => throw new ArgumentException("Somehow this is not a direction"),
            };
        }
    }
    class Room
    {
        public bool hasNorth;
        public bool hasEast;
        public bool hasSouth;
        public bool hasWest;
        public Encounter? encounter = null;
        public bool playerVisited;
        public bool exitRoom;
        public Trap? trapType;


        public Room()
        {
            Random rnd = new Random();
            hasNorth = rnd.Next(0, 2) == 1;
            hasEast = rnd.Next(0, 2) == 1;
            hasSouth = rnd.Next(0, 2) == 1;
            hasWest = rnd.Next(0, 2) == 1;
            exitRoom = false;
            encounter = Helper.GenerateEncounter();
            trapType = null;

        }        
        public List<Direction> AvailableDoors()
        {
            List<Direction> doorsFound = new List<Direction>();

            if (hasNorth)
            {
                doorsFound.Add(Direction.North);
            }
            if (hasEast)
            {
                doorsFound.Add(Direction.East);
            }
            if (hasSouth)
            {
                doorsFound.Add(Direction.South);
            }
            if (hasWest)
            {
                doorsFound.Add(Direction.West);
            }
            
            return doorsFound;
        }

        public bool IsEscapable()
        {
            return hasNorth || hasEast || hasSouth || hasWest;
        }

        public bool HasDoor(Direction direction)
        {
            return direction switch
            {
                Direction.North => hasNorth,
                Direction.East => hasEast,
                Direction.South => hasSouth,
                Direction.West => hasWest,
                _ => throw new ArgumentException("Direction't"),
            };
        }
        public void SetDoor(Direction direction, bool doorExists)
        {
            switch (direction)
            {
                case Direction.North:
                    hasNorth = doorExists;
                    break;
                case Direction.East:
                    hasEast = doorExists;
                    break;
                case Direction.South:
                    hasSouth = doorExists;
                    break;
                case Direction.West:
                    hasWest = doorExists;
                    break;
            }
        }

        public void CullDirections()
        {
            hasNorth = false;
            hasEast = false;
            hasSouth = false;
            hasWest = false;
        }

        public int DoorCount()
        {
            int doorCount = 0;

            return doorCount;
        }
    }
}
