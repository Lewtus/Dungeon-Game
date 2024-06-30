using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace gam
{
    public struct Location
    {
        public int x;
        public int y;


        public Location(int xLocation, int yLocation)
        {
            x = xLocation;
            y = yLocation;

        }

        public Location ToDirection(Direction direction)
        {
            return direction switch
            {
                Direction.North => new Location(x, y + 1),
                Direction.South => new Location(x, y - 1),
                Direction.East => new Location(x + 1, y),
                Direction.West => new Location(x - 1, y),
                _ => throw new ArgumentException("Attempting to move an invalid direction"),
            };
        }

        public override string ToString()
        {
            return x + " " + y;
        }

    }
    public struct MapExtremes
    {
        public Location bottomLeft;
        public Location topRight;
    }

    public struct ExitRoom
    {
        public Location exitLocation;
    
    }

    class Map
    {
        public Dictionary<Location, Room> roomList = new Dictionary<Location, Room>();
        public Dictionary<Location, Merchant> guildOfMerchants = new Dictionary<Location, Merchant>();        
        //public Dictionary<Merchant, Personality> merchantPersonalities = new Dictionary<Merchant, Personality>();

        Random rnd = new Random();
        public Room GetRoomOrAdd(Location locationIdentifier)
        {
            Room newRoom;
            if (!roomList.TryGetValue(locationIdentifier, out newRoom))
            {
                newRoom = new Room();
                Direction[] directions = new Direction[] { Direction.North, Direction.South, Direction.East, Direction.West };
                foreach (var direction in directions)
                {
                    var targetLocation = locationIdentifier.ToDirection(direction);
                    var targetRoom = GetRoom(targetLocation);
                    if (targetRoom == null)
                    {
                        continue;
                    }

                    var hasConnectedDoor = targetRoom.HasDoor(direction.InvertDirection());
                    newRoom.SetDoor(direction, hasConnectedDoor);
                }
                roomList.Add(locationIdentifier, newRoom);
            }
            return newRoom;
        }
        
        public Room GetRoom(Location locationIdentifier)
        {
            if (roomList.TryGetValue(locationIdentifier, out Room currentRoom))
            {
                return currentRoom;
            }
            return null;
        }
        public void AddMerchantToGuild(Location location, Merchant newMerchant)//4fun function name, adds merchant to list of merchants in map
        {
            guildOfMerchants.Add(location, newMerchant);
        }

        public int GetRoomCount()
        {
            int travelledRooms = 0;
            foreach (var room in roomList)
            {
                if (room.Value.playerVisited)
                {
                    travelledRooms += 1;
                }
            }
            return travelledRooms;
        }

        public void PlayerFindsMap()
        {
            foreach (var locationData in roomList)
            {
                //locationData.Value.playerVisited = true;
            }
        }        

        public static Map GenerateDungeon()
        {
            //TODO: use global rnd to create a seed to allow maps to be replayed
            var mapBuilder = new Player();
            Map newMap = new Map();
            mapBuilder.playerXLocation = 0;
            mapBuilder.playerYLocation = 0;
            Room startRoom = new Room();
            while (startRoom.AvailableDoors().ToArray().Length <= 0)
            {
                startRoom = new Room();
            }
            startRoom.playerVisited = true;
            newMap.roomList.Add(new Location(0, 0), startRoom);
            Room currentRoom = null;
            for (int i = 1; i <= 500; i++)//the map creator will move 500 times in a random direction, which will create a room at that location if one is not present
            {
                Direction newRandomDirection = GetRandomDirection();
                Location mapBuilderCurrentLocation = mapBuilder.Location();
                currentRoom = newMap.GetRoom(mapBuilderCurrentLocation);
                Direction[] previousRoomDoors = currentRoom.AvailableDoors().ToArray();
                Direction moveDirection = RandomHelpers.PickFromArray(previousRoomDoors);
                mapBuilder.MovePlayer(moveDirection);
                Location mapBuilderFutureLocation = mapBuilder.Location();
                if (newMap.GetRoom(mapBuilderFutureLocation) == null)
                {
                    Room newRoom = new Room();
                    newMap.GetRoomOrAdd(mapBuilderFutureLocation);
                    newRoom.SetDoor(newRandomDirection, true);

                }
            }
            currentRoom.exitRoom = true;
            currentRoom.encounter = Encounter.Boss;
            newMap.FixDeadEnds(newMap);
            newMap.FixOneWayCorridors();
            return newMap;
        }


        public Dictionary<Location, Room> tempRoomList = new Dictionary<Location, Room>();
        public void FixDeadEnds(Map newMap)
        {
            //goes through created rooms ensuring that there are no dead ends (doors that do not lead to rooms)
            
            foreach (var locationData in roomList)
            {
                var roomDoors = locationData.Value.AvailableDoors();
                foreach (var doorDirection in roomDoors)
                {
                    var doorRoom = locationData.Key.ToDirection(doorDirection);
                    var doorHasRoom = GetRoom(doorRoom);
                    if (doorHasRoom == null)
                    {
                        Room newRoom = new Room();
                        newRoom.CullDirections();
                        newRoom.SetDoor(doorDirection.InvertDirection(), true);
                        int xCoord = locationData.Key.x;
                        int yCoord = locationData.Key.y;
                        switch (doorDirection)
                        {
                            case Direction.North:
                                yCoord++;
                                break;
                            case Direction.East:
                                xCoord++;
                                break;
                            case Direction.South:
                                yCoord--;
                                break;
                            case Direction.West:
                                xCoord--;
                                break;
                            default:
                                break;
                        }

                        if (!tempRoomList.ContainsKey(new Location(xCoord, yCoord)))
                        {
                            tempRoomList.Add(new Location(xCoord, yCoord), newRoom);
                        }
                    }

                }
            }
            foreach(var locationData in tempRoomList)
            {
                int xCoord = locationData.Key.x;
                int yCoord = locationData.Key.y;
                Room newRoom = locationData.Value;
                newMap.roomList.Add(new Location(xCoord, yCoord), newRoom);

            }
        }




        //goes through the currently generated map and makes sure there are no one way streets
        public void FixOneWayCorridors()
        {
            foreach (var locationData in roomList)
            {
                var neighborDirections = locationData.Value.AvailableDoors();
                foreach (var neighborDirection in neighborDirections)
                {
                    var neighborLocation = locationData.Key.ToDirection(neighborDirection);
                    var neighborRoom = GetRoom(neighborLocation);
                    if (neighborRoom != null)
                    {
                        neighborRoom.SetDoor(neighborDirection.InvertDirection(), true);
                    }
                }
            }
        }

        public static Direction GetRandomDirection()//returns a random direction for the room passed to it
        {
            Random rnd = new Random();
            int randomDirectionInt = rnd.Next(1, 5);
            return randomDirectionInt switch
            {
                1 => Direction.North,
                2 => Direction.East,
                3 => Direction.South,
                4 => Direction.West,
                _ => Direction.North,
            };
        }

        public void TeleportPlayerToRandomLocation(Player player)
        {
            var mapExtents = FindMapExtents();
            int newRandomWidth = rnd.Next(mapExtents.bottomLeft.x+1, mapExtents.topRight.x);
            int newRandomHeight = rnd.Next(mapExtents.bottomLeft.y + 1, mapExtents.topRight.y);
            player.playerXLocation = newRandomWidth;
            player.playerYLocation = newRandomHeight;
        }

        public MapExtremes FindMapExtents()
        {
            Location bottomLeft = new Location(0, 0);
            Location topRight = new Location(0, 0);

            foreach (var entry in roomList)
            {
                bottomLeft.x = Math.Min(bottomLeft.x, entry.Key.x);
                bottomLeft.y = Math.Min(bottomLeft.y, entry.Key.y);
                topRight.x = Math.Max(topRight.x, entry.Key.x);
                topRight.y = Math.Max(topRight.y, entry.Key.y);
            }
            return new MapExtremes() { bottomLeft = bottomLeft, topRight = topRight };
        }

        public void PrintMap(Location playerLocation, Player playerChar)
        {
            //TODO why do map extreme edges not show rooms? - fix it
            var mapExtents = FindMapExtents();
            
            int mapwidth = mapExtents.topRight.x - mapExtents.bottomLeft.x+1;
            int mapheight = mapExtents.topRight.y - mapExtents.bottomLeft.y+1;
            int fullmapwidth = mapwidth * 3 + 2; //extra characters for the paths between rooms requires map to be larger
            int fullmapheight = mapheight * 2 + 1; //smaller as vertical paths only use a single character
            char[,] renderTarget = new char[fullmapheight, fullmapwidth];

            for (int row = 0; row < fullmapheight; row++)
            {               
                for (int column = 0; column < fullmapwidth; column++)
                {                    
                    renderTarget[row, column] = ' ';

                }
            }
            bool playerHasMap = playerChar.FindItemInInventory(Item.Map) > 0;
            foreach (var locationRoom in roomList)
            {
                int renderTargetX = (locationRoom.Key.x - mapExtents.bottomLeft.x) *3 + 2; //translation from "room space" to "map space"
                int renderTargetY = (locationRoom.Key.y - mapExtents.bottomLeft.y) *2 + 1; // ditto for y

                if (locationRoom.Key.Equals(playerLocation))
                {
                    renderTarget[renderTargetY, renderTargetX] = 'X';
                }
                else if (playerHasMap)
                {
                    if (locationRoom.Value.exitRoom)
                    {
                        renderTarget[renderTargetY, renderTargetX] = 'E';
                    }
                    else if (locationRoom.Value.encounter == Encounter.Boss && locationRoom.Value.playerVisited)
                    {
                        renderTarget[renderTargetY, renderTargetX] = 'B';
                    }
                    else if (locationRoom.Value.encounter == Encounter.Trap && locationRoom.Value.playerVisited)
                    {
                        renderTarget[renderTargetY, renderTargetX] = 'T';
                    }
                    else if (locationRoom.Value.encounter == Encounter.Monster && locationRoom.Value.playerVisited)
                    {
                        renderTarget[renderTargetY, renderTargetX] = 'M';
                    }
                    else if (locationRoom.Value.encounter == Encounter.Merchant && locationRoom.Value.playerVisited)
                    {
                        renderTarget[renderTargetY, renderTargetX] = 'V';
                    }
                    else if (locationRoom.Value.encounter == Encounter.Chest)
                    {
                        renderTarget[renderTargetY, renderTargetX] = 'C';
                    }
                    else
                    {
                        renderTarget[renderTargetY, renderTargetX] = '■';
                    }
                }
                else if (locationRoom.Value.playerVisited)
                {
                    if (locationRoom.Value.exitRoom)
                    {
                        renderTarget[renderTargetY, renderTargetX] = 'E';
                    }
                    else if (locationRoom.Value.encounter == Encounter.Boss)
                    {
                        renderTarget[renderTargetY, renderTargetX] = 'B';
                    }
                    else if (locationRoom.Value.encounter == Encounter.Trap)
                    {
                        renderTarget[renderTargetY, renderTargetX] = 'T';
                    }
                    else if (locationRoom.Value.encounter == Encounter.Monster)
                    {
                        renderTarget[renderTargetY, renderTargetX] = 'M';
                    }
                    else if (locationRoom.Value.encounter == Encounter.Merchant)
                    {
                        renderTarget[renderTargetY, renderTargetX] = 'V';
                    }
                    else if (locationRoom.Value.encounter == Encounter.Chest)
                    {
                        renderTarget[renderTargetY, renderTargetX] = 'C';
                    }
                    else
                    {
                        renderTarget[renderTargetY, renderTargetX] = '■';
                    }
                }
                if (locationRoom.Value.playerVisited || playerHasMap)
                {
                    foreach (Direction direction in locationRoom.Value.AvailableDoors())
                    {
                        switch (direction)
                        {
                            case Direction.North:
                                renderTarget[renderTargetY + 1, renderTargetX] = '|';
                                break;
                            case Direction.East:
                                renderTarget[renderTargetY, renderTargetX + 1] = '-';
                                renderTarget[renderTargetY, renderTargetX + 2] = '-';
                                break;
                            case Direction.South:
                                renderTarget[renderTargetY - 1, renderTargetX] = '|';
                                break;
                            case Direction.West:
                                renderTarget[renderTargetY, renderTargetX - 1] = '-';
                                renderTarget[renderTargetY, renderTargetX - 2] = '-';
                                break;
                        }
                    }
                }
            }


            for (int row = 0; row < fullmapheight; row++)
            {
                for (int column = 0; column < fullmapwidth; column++)
                {
                    Console.Write(renderTarget[fullmapheight-row-1, column]);
                }
                Console.WriteLine();
            }
            Console.WriteLine("Key:");
            Console.WriteLine("X : Player Location | E : Exit");
            Console.WriteLine("T : Trap | C : Chest");
            Console.WriteLine("M : Monster | B : Boss");
            Console.WriteLine("V : Merchant | ■ : Empty");

        }
    }
}
