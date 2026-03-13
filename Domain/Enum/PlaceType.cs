using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum
{
    public enum PlaceType
    {
        // Food (0-99)
        Restaurant = 0,
        Cafe = 1,
        Bar = 2,
        FastFood = 3,
        Bakery = 4,

        // Accommodation (100-199)
        Hotel = 100,
        Hostel = 101,
        Apartment = 102,
        Guesthouse = 103,
        Resort = 104,

        // Culture (200-299)
        Museum = 200,
        Gallery = 201,
        Theater = 202,
        Monument = 203,
        Library = 204,

        // Nature (300-399)
        Park = 300,
        Beach = 301,
        Mountain = 302,
        Forest = 303,
        Lake = 304,

        // Entertainment (400-499)
        Cinema = 400,
        NightClub = 401,
        Casino = 402,
        AmusementPark = 403,
        Zoo = 404,

        // Shopping (500-599)
        ShoppingMall = 500,
        Market = 501,
        Boutique = 502,
        Supermarket = 503,

        // Transport (600-699)
        Airport = 600,
        TrainStation = 601,
        BusStation = 602,
        Port = 603,

        // Services (700-799)
        Hospital = 700,
        Bank = 701,
        PostOffice = 702,
        TouristInfo = 703
    }
}
