namespace WebAPIDemo.Models.Repositories
{
    public static class ShirtRepository
    {
        private static List<Shirt> shirts = new List<Shirt>()
        {
            new Shirt{shirtId=1,Brand="My Brand 1",color="Blue",Gender="Men",Price=20,Size=10},
            new Shirt{shirtId=2,Brand="My Brand 2",color="Black",Gender="Men",Price=30,Size=12},
            new Shirt{shirtId=3,Brand="My Brand 3",color="Pink",Gender="Women",Price=10,Size=8},
            new Shirt{shirtId=4,Brand="My Brand 4",color="Yellow",Gender="Wome",Price=25,Size=9}

        };
        public static bool shirtExists(int shirtId)
        {
            return shirts.Any(x => x.shirtId == shirtId);
        }
        public static Shirt? GetShirtById(int id)
        { return shirts.FirstOrDefault(x => x.shirtId == id); }
        public static List<Shirt> GetShirt()
        { return shirts; }
        public static Shirt? GetShirtByProperties(string? brand,string? gender,string? color,int? size)
        {
            return shirts.FirstOrDefault(x =>
            !string.IsNullOrWhiteSpace(brand) &&
            !string.IsNullOrWhiteSpace(x.Brand) &&
            x.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase) &&
            !string.IsNullOrWhiteSpace(gender) &&
            !string.IsNullOrWhiteSpace(x.Gender) &&
            x.Gender.Equals(gender, StringComparison.OrdinalIgnoreCase) &&
            !string.IsNullOrWhiteSpace(gender) &&
            !string.IsNullOrWhiteSpace(x.Gender) &&
            x.Gender.Equals(gender, StringComparison.OrdinalIgnoreCase) &&
            !string.IsNullOrWhiteSpace(gender) &&
            !string.IsNullOrWhiteSpace(x.Gender) &&
            x.Gender.Equals(gender, StringComparison.OrdinalIgnoreCase)
            );
        }
        public static void AddShirt(Shirt shirt)
        {
            shirt.shirtId = shirts.Max(x => x.shirtId) + 1;
            shirts.Add(shirt);
        }
        public static void UpdateShirt(Shirt shirt)
        {
            var shirtToUpdate = shirts.First(x=>x.shirtId == shirt.shirtId);
            shirtToUpdate.Brand = shirt.Brand;
            shirtToUpdate.Price = shirt.Price;
            shirtToUpdate.Size= shirt.Size;
            shirtToUpdate.color = shirt.color;
            shirtToUpdate.Gender = shirt.Gender;
        }

        public static void DeleteShirt(int ShirtID)
        {
            var shirt = GetShirtById(ShirtID);
            if(shirt != null)
            {
                shirts.Remove(shirt);
            }
        }
    }
}
