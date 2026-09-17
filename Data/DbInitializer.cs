using dalabat.Models;

namespace dalabat.Data;

public static class DbInitializer
{
    public static void Initialize(DalabatDbContext context)
    {
        // Ensure database tables exist
        context.Database.EnsureCreated();

        // 1. Seed Categories if empty
        if (!context.Categories.Any())
        {
            var categories = new List<Category>
            {
                new Category { Name = "Burgers & Fast Food", Description = "Juicy burgers, crispy chicken and loaded fries", IconUrl = "fa-hamburger" },
                new Category { Name = "Pizza & Italian", Description = "Fresh oven-baked pizzas & pasta specialties", IconUrl = "fa-pizza-slice" },
                new Category { Name = "Middle Eastern & Grills", Description = "Delicious shawarmas, kebabs, hummus and mezza", IconUrl = "fa-utensils" },
                new Category { Name = "Beverages & Shakes", Description = "Fresh juices, specialty lattes, milkshakes and sodas", IconUrl = "fa-coffee" }
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();
        }

        // 2. Seed Default Restaurants if empty
        if (!context.Restaurants.Any())
        {
            var burgerPlace = new Restaurant
            {
                Name = "Burger Central",
                Address = "123 Main Street, Downtown",
                Phone = "+971501234567"
            };

            var pizzaPlace = new Restaurant
            {
                Name = "Pizza Supreme",
                Address = "456 Palm Boulevard",
                Phone = "+971507654321"
            };

            var shawarmaPlace = new Restaurant
            {
                Name = "Al Sultan Shawarma",
                Address = "789 Marina Walk",
                Phone = "+971509988776"
            };

            context.Restaurants.AddRange(burgerPlace, pizzaPlace, shawarmaPlace);
            context.SaveChanges();
        }

        var burgerPlaceObj = context.Restaurants.FirstOrDefault(r => r.Name.Contains("Burger"));
        var pizzaPlaceObj = context.Restaurants.FirstOrDefault(r => r.Name.Contains("Pizza"));
        var shawarmaPlaceObj = context.Restaurants.FirstOrDefault(r => r.Name.Contains("Sultan"));

        var catBurger = context.Categories.FirstOrDefault(c => c.Name.Contains("Burgers"))?.CategoryID;
        var catPizza = context.Categories.FirstOrDefault(c => c.Name.Contains("Pizza"))?.CategoryID;
        var catME = context.Categories.FirstOrDefault(c => c.Name.Contains("Middle Eastern"))?.CategoryID;
        var catDrink = context.Categories.FirstOrDefault(c => c.Name.Contains("Beverages"))?.CategoryID;

        if (burgerPlaceObj != null && pizzaPlaceObj != null && shawarmaPlaceObj != null)
        {
            var newItems = new List<FoodItem>();

            void AddIfMissing(int restId, int? catId, string name, string desc, decimal price)
            {
                if (!context.FoodItems.Any(f => f.Name == name && f.RestaurantID == restId))
                {
                    newItems.Add(new FoodItem
                    {
                        RestaurantID = restId,
                        CategoryID = catId,
                        Name = name,
                        Description = desc,
                        Price = price,
                        Availability = true
                    });
                }
            }

            // Burger Central
            AddIfMissing(burgerPlaceObj.RestaurantID, catBurger, "Classic Cheeseburger", "Juicy beef patty with cheddar cheese, lettuce, tomato & special sauce.", 28.50m);
            AddIfMissing(burgerPlaceObj.RestaurantID, catBurger, "Crispy Chicken Burger", "Crispy fried chicken breast with mayo and pickles.", 26.00m);
            AddIfMissing(burgerPlaceObj.RestaurantID, catBurger, "Double Bacon Smoked Burger", "Double beef patty, smoked bacon, caramelized onions & BBQ sauce.", 36.50m);
            AddIfMissing(burgerPlaceObj.RestaurantID, catBurger, "Truffle Mushroom Burger", "Wagyu beef patty, swiss cheese, sauteed mushrooms & truffle mayo.", 39.00m);
            AddIfMissing(burgerPlaceObj.RestaurantID, catBurger, "Loaded Cheese Fries", "Golden fries topped with melted cheddar, jalapenos & ranch.", 16.50m);
            AddIfMissing(burgerPlaceObj.RestaurantID, catBurger, "Mozzarella Sticks (6pcs)", "Crispy breaded mozzarella served with warm marinara sauce.", 14.00m);

            // Pizza Supreme
            AddIfMissing(pizzaPlaceObj.RestaurantID, catPizza, "Pepperoni Supreme Pizza", "Loaded with mozzarella cheese and premium beef pepperoni.", 45.00m);
            AddIfMissing(pizzaPlaceObj.RestaurantID, catPizza, "Margherita Pizza", "Classic tomato sauce, fresh mozzarella, and basil.", 38.00m);
            AddIfMissing(pizzaPlaceObj.RestaurantID, catPizza, "Truffle Four Cheese Pizza", "Mozzarella, gorgonzola, parmesan, ricotta & black truffle oil.", 52.00m);
            AddIfMissing(pizzaPlaceObj.RestaurantID, catPizza, "Creamy Chicken Alfredo Pasta", "Fettuccine pasta in rich parmesan garlic cream sauce with grilled chicken.", 42.00m);
            AddIfMissing(pizzaPlaceObj.RestaurantID, catPizza, "Garlic Dough Balls", "Freshly baked dough balls brushed with garlic butter and parsley.", 15.00m);

            // Al Sultan Shawarma
            AddIfMissing(shawarmaPlaceObj.RestaurantID, catME, "Chicken Shawarma Wrap", "Marinated chicken, garlic sauce, and fries inside warm pita.", 15.00m);
            AddIfMissing(shawarmaPlaceObj.RestaurantID, catME, "Beef Shawarma Plate", "Tender beef slices with tahini sauce, pickles, and fresh bread.", 32.00m);
            AddIfMissing(shawarmaPlaceObj.RestaurantID, catME, "Mix Grill Platter", "Tender lamb kebabs, chicken shish tawook & grilled vegetables.", 65.00m);
            AddIfMissing(shawarmaPlaceObj.RestaurantID, catME, "Hummus & Fresh Baked Bread", "Creamy chickpeas dip topped with olive oil and paprika.", 18.00m);
            AddIfMissing(shawarmaPlaceObj.RestaurantID, catME, "Falafel Sandwich", "Crispy golden falafels with tahini, tomatoes and pickles.", 12.50m);

            // Beverages & Drinks
            AddIfMissing(burgerPlaceObj.RestaurantID, catDrink, "Fresh Mango Juice", "100% natural fresh sweet mango juice.", 14.00m);
            AddIfMissing(burgerPlaceObj.RestaurantID, catDrink, "Chocolate Fudge Milkshake", "Rich chocolate ice cream blended with dark chocolate syrup.", 19.00m);
            AddIfMissing(pizzaPlaceObj.RestaurantID, catDrink, "Iced Spanish Latte", "Double shot espresso with condensed milk and chilled milk.", 18.50m);
            AddIfMissing(shawarmaPlaceObj.RestaurantID, catDrink, "Fresh Mint Lemonade", "Refreshing crushed ice lemonade with fresh mint leaves.", 13.50m);
            AddIfMissing(burgerPlaceObj.RestaurantID, catDrink, "Coca-Cola Can (330ml)", "Chilled original Coca-Cola soft drink.", 6.00m);

            if (newItems.Any())
            {
                context.FoodItems.AddRange(newItems);
                context.SaveChanges();
            }
        }

        // 3. Seed Coupons if empty
        if (!context.Coupons.Any())
        {
            var coupons = new List<Coupon>
            {
                new Coupon { Code = "DALABAT20", DiscountPercentage = 20.00m, MaxDiscountAmount = 30.00m, ExpiryDate = DateTime.UtcNow.AddMonths(6), IsActive = true },
                new Coupon { Code = "WELCOME10", DiscountPercentage = 10.00m, MaxDiscountAmount = 15.00m, ExpiryDate = DateTime.UtcNow.AddMonths(12), IsActive = true },
                new Coupon { Code = "SAVE15", DiscountPercentage = 15.00m, MaxDiscountAmount = 25.00m, ExpiryDate = DateTime.UtcNow.AddMonths(3), IsActive = true }
            };
            context.Coupons.AddRange(coupons);
            context.SaveChanges();
        }

        // 4. Seed Drivers if empty
        if (!context.Drivers.Any())
        {
            var drivers = new List<Driver>
            {
                new Driver { Name = "Salim Express", Phone = "+971501112233", VehicleType = "Motorcycle", LicensePlate = "DXB-9876", IsAvailable = true, CurrentLocation = "Downtown" },
                new Driver { Name = "Tariq Delivery", Phone = "+971504445566", VehicleType = "Motorcycle", LicensePlate = "DXB-5432", IsAvailable = true, CurrentLocation = "Marina Walk" },
                new Driver { Name = "Khalid Driver", Phone = "+971507778899", VehicleType = "Car", LicensePlate = "DXB-1122", IsAvailable = true, CurrentLocation = "Palm Jumeirah" }
            };
            context.Drivers.AddRange(drivers);
            context.SaveChanges();
        }

        // 5. Seed Reviews if empty
        if (!context.Reviews.Any())
        {
            var demoUser = context.Users.FirstOrDefault();
            var burgerRest = context.Restaurants.FirstOrDefault(r => r.Name.Contains("Burger"));
            var pizzaRest = context.Restaurants.FirstOrDefault(r => r.Name.Contains("Pizza"));
            var shawarmaRest = context.Restaurants.FirstOrDefault(r => r.Name.Contains("Sultan"));

            if (demoUser != null && burgerRest != null && pizzaRest != null && shawarmaRest != null)
            {
                context.Reviews.AddRange(
                    new Review { UserID = demoUser.UserID, RestaurantID = burgerRest.RestaurantID, Rating = 5, Comment = "Best burgers in town! The Truffle Mushroom Burger was cooked to absolute perfection.", CreatedAt = DateTime.UtcNow.AddDays(-2) },
                    new Review { UserID = demoUser.UserID, RestaurantID = pizzaRest.RestaurantID, Rating = 5, Comment = "The Pepperoni Supreme Pizza was hot, crispy, and delivered in less than 25 minutes. 10/10!", CreatedAt = DateTime.UtcNow.AddDays(-5) },
                    new Review { UserID = demoUser.UserID, RestaurantID = shawarmaRest.RestaurantID, Rating = 4, Comment = "Authentic Middle Eastern flavors! The Chicken Shawarma Wrap and fresh mint lemonade were amazing.", CreatedAt = DateTime.UtcNow.AddDays(-7) }
                );
                context.SaveChanges();
            }
        }
    }
}
